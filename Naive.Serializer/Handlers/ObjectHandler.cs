using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Naive.Serializer.Handlers
{
    public class ObjectHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Object;

        private ConcurrentDictionary<string, Property> _properties = new();

        private Property[] _sortedProperties = new Property[0];

        private bool _isObject;

        public override bool Match(Type type)
        {
            return true;
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            IsNullable = true;
            IsSimple = false;

            if (Type == null)
            {
                Type = typeof(object);
            }
            else
            {
                var nullableStruct = type.IsValueType ? Nullable.GetUnderlyingType(type) : null;

                if (nullableStruct != null)
                {
                    Type = nullableStruct;
                }

                var dataContract = Type.GetCustomAttribute<DataContractAttribute>();

                var definitions = Type.GetProperties()
                    .Where(x => x.CanRead && x.CanWrite
                        && x.GetCustomAttribute<IgnoreDataMemberAttribute>() == null
                        && (dataContract == null || x.GetCustomAttribute<DataMemberAttribute>() != null))
                    .Select(x => new Property { Info = x, Name = x.Name }).ToArray();

                foreach (var definition in definitions)
                {
                    var propertyInfo = definition.Info;

                    var dataMember = propertyInfo.GetCustomAttribute<DataMemberAttribute>();

                    if (dataMember != null)
                    {
                        definition.Order = dataMember.Order;

                        if (!string.IsNullOrEmpty(dataMember.Name))
                        {
                            definition.Name = dataMember.Name;
                        }
                    }

                    definition.GetValue = CreateGetter(definition.Info);
                    definition.SetValue = CreateSetter(definition.Info);

                    _properties.TryAdd(definition.Name, definition);
                }

                _sortedProperties = definitions.OrderBy(x => x.Order).ToArray();
            }

            _isObject = Type != typeof(object);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            foreach (var property in _sortedProperties)
            {
                var value = property.GetValue(obj);

                if (value != null || !options.IgnoreNullValue)
                {
                    writer.Write(property.Name);

                    property.Handler ??= NaiveSerializer.GetTypeHandler(property.Info.PropertyType);

                    NaiveSerializer.Write(writer, value, options, property.Handler);
                }
            }

            writer.Write(string.Empty);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var result = _isObject ? Activator.CreateInstance(Type) : new Dictionary<string, object>();

            do
            {
                var name = reader.ReadString();

                if (string.IsNullOrEmpty(name))
                {
                    break;
                }

                object value;

                if (_isObject && _properties.TryGetValue(name, out var property))
                {
                    property.Handler ??= NaiveSerializer.GetTypeHandler(property.Info.PropertyType);

                    value = NaiveSerializer.Read(reader, property.Info.PropertyType, options, property.Handler);

                    property.SetValue(result, value);
                }
                else
                {
                    if (!_isObject || options.IgnoreMissingMember)
                    {
                        value = NaiveSerializer.Read(reader, null, options);

                        if (!_isObject)
                        {
                            ((Dictionary<string, object>)result).Add(name, value);
                        }
                    }
                    else
                    {
                        throw new MissingMemberException($"Property with name '{name}' is not found on class '{result.GetType().FullName}'.");
                    }
                }
            } while (true);

            return result;
        }

        private static Func<object, object> CreateGetter(PropertyInfo property)
        {
            var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
            var instanceExpr = property.DeclaringType.IsValueType 
                ? Expression.Convert(objInstanceExpr, property.DeclaringType)
                : Expression.TypeAs(objInstanceExpr, property.DeclaringType);
            var propertyExpr = Expression.Property(instanceExpr, property);
            var propertyObjExpr = Expression.Convert(propertyExpr, typeof(object));

            return Expression.Lambda<Func<object, object>>(propertyObjExpr, objInstanceExpr).Compile();
        }

        private static Action<object, object> CreateSetter(PropertyInfo property)
        {
            if (property.DeclaringType.IsValueType)
            {
                return property.SetValue;
            }
            else
            {
                var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
                var instanceExpr = Expression.Convert(objInstanceExpr, property.DeclaringType);
                var propertyExpr = Expression.Property(instanceExpr, property);
                var objValueExpr = Expression.Parameter(typeof(object), "value");
                var valueExpr = Expression.Convert(objValueExpr, property.PropertyType);
                var propertyAssignExpr = Expression.Assign(propertyExpr, valueExpr);

                return Expression.Lambda<Action<object, object>>(propertyAssignExpr, objInstanceExpr, objValueExpr).Compile();
            }
        }

        private class Property
        {
            public string Name { get; set; }

            public int Order { get; set; }

            public PropertyInfo Info { get; set; }

            public IHandler Handler { get; set; }
            
            public Func<object, object> GetValue { get; set; }
            
            public Action<object, object> SetValue { get; set; }

            public override string ToString()
            {
                return $"{Info.DeclaringType.Name}.{Info.Name}{(Name != Info.Name ? $"({Name})" : string.Empty)}";
            }
        }
    }
}
