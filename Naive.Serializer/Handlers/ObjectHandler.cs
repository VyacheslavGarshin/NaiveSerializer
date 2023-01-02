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

                var definitionCandidates =
                    Type.GetProperties().Where(x => x.CanRead && x.CanWrite)
                        .Select(x => new Property { PropertyInfo = x, Name = x.Name }).Concat(
                    Type.GetFields().Where(x => x.IsPublic)
                        .Select(x => new Property { FieldInfo = x, Name = x.Name }))
                    .ToArray();

                var definitions = new List<Property>();

                foreach (var definition in definitionCandidates)
                {
                    var memberInfo = (MemberInfo)definition.PropertyInfo ?? definition.FieldInfo;

                    if (memberInfo.GetCustomAttribute<IgnoreDataMemberAttribute>() != null
                        || (dataContract != null && memberInfo.GetCustomAttribute<DataMemberAttribute>() == null))
                    {
                        continue;
                    }

                    var dataMember = memberInfo.GetCustomAttribute<DataMemberAttribute>();

                    if (dataMember != null)
                    {
                        definition.Order = dataMember.Order;

                        if (!string.IsNullOrEmpty(dataMember.Name))
                        {
                            definition.Name = dataMember.Name;
                        }
                    }

                    definition.GetValue = CreateGetter(definition);
                    definition.SetValue = CreateSetter(definition);

                    _properties.TryAdd(definition.Name, definition);
                    definitions.Add(definition);
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

                    property.Handler ??= NaiveSerializer.GetTypeHandler(GetMemberType(property));

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
                    property.Handler ??= NaiveSerializer.GetTypeHandler(GetMemberType(property));

                    value = NaiveSerializer.Read(reader, GetMemberType(property), options, property.Handler);

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

        private static Type GetMemberType(Property property)
        {
            return property.PropertyInfo?.PropertyType ?? property.FieldInfo.FieldType;
        }

        private static Func<object, object> CreateGetter(Property property)
        {
            var declaringType = GetDeclaringType(property);

            var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
            var instanceExpr = declaringType.IsValueType
                ? Expression.Convert(objInstanceExpr, declaringType)
                : Expression.TypeAs(objInstanceExpr, declaringType);
            var propertyExpr = property.PropertyInfo != null
                ? Expression.Property(instanceExpr, property.PropertyInfo)
                : Expression.Field(instanceExpr, property.FieldInfo);
            var propertyObjExpr = Expression.Convert(propertyExpr, typeof(object));

            return Expression.Lambda<Func<object, object>>(propertyObjExpr, objInstanceExpr).Compile();
        }

        private static Type GetDeclaringType(Property property)
        {
            return property.PropertyInfo?.DeclaringType ?? property.FieldInfo.DeclaringType;
        }

        private static Action<object, object> CreateSetter(Property property)
        {
            var declaringType = GetDeclaringType(property);
            var propertyType = GetMemberType(property);

            if (declaringType.IsValueType)
            {
                return property.PropertyInfo != null ? property.PropertyInfo.SetValue : property.FieldInfo.SetValue;
            }
            else
            {
                var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
                var instanceExpr = Expression.Convert(objInstanceExpr, declaringType);
                var propertyExpr = property.PropertyInfo != null
                    ? Expression.Property(instanceExpr, property.PropertyInfo)
                    : Expression.Field(instanceExpr, property.FieldInfo);
                var objValueExpr = Expression.Parameter(typeof(object), "value");
                var valueExpr = Expression.Convert(objValueExpr, propertyType);
                var propertyAssignExpr = Expression.Assign(propertyExpr, valueExpr);

                return Expression.Lambda<Action<object, object>>(propertyAssignExpr, objInstanceExpr, objValueExpr).Compile();
            }
        }

        private class Property
        {
            public string Name { get; set; }

            public int Order { get; set; }

            public PropertyInfo PropertyInfo { get; set; }

            public FieldInfo FieldInfo { get; set; }

            public IHandler Handler { get; set; }
            
            public Func<object, object> GetValue { get; set; }
            
            public Action<object, object> SetValue { get; set; }

            public override string ToString()
            {
                return $"{PropertyInfo.DeclaringType.Name}.{PropertyInfo.Name}{(Name != PropertyInfo.Name ? $"({Name})" : string.Empty)}";
            }
        }
    }
}
