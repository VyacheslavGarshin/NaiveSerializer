﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Naive.Serializer.Handlers
{
    public class ObjectHandler : AbstractHandler<ObjectHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Object;

        private ConcurrentDictionary<string, Property> _properties = new();

        private Property[] _sortedProperties = new Property[0];

        public override bool Match(Type type)
        {
            return type.IsClass;
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            if (Type == null)
            {
                Type = typeof(object);
            }
            else
            {
                var dataContract = Type.GetCustomAttribute<DataContractAttribute>();

                var definitions = Type.GetProperties()
                    .Where(x =>
                        x.CanRead
                        && x.CanWrite
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

                    definition.Getter = CreateGetterLambda(definition.Info);
                    definition.Setter = CreateSetterLambda(definition.Info);

                    _properties.TryAdd(definition.Name, definition);
                }

                _sortedProperties = definitions.OrderBy(x => x.Order).ToArray();
            }
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            foreach (var property in _sortedProperties)
            {
                var value = property.Getter(obj);

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
            var isObject = Type != typeof(object);

            var result = isObject ? Activator.CreateInstance(Type) : new Dictionary<string, object>();

            do
            {
                var name = reader.ReadString();

                if (string.IsNullOrEmpty(name))
                {
                    break;
                }

                object value;

                if (isObject && _properties.TryGetValue(name, out var property))
                {
                    property.Handler ??= NaiveSerializer.GetTypeHandler(property.Info.PropertyType);

                    value = NaiveSerializer.Read(reader, property.Info.PropertyType, options, property.Handler);

                    property.Setter(result, value);
                }
                else
                {
                    if (!isObject || options.IgnoreMissingMember)
                    {
                        value = NaiveSerializer.Read(reader, null, options, null);

                        if (!isObject)
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

        private static Func<object, object> CreateGetterLambda(PropertyInfo property)
        {
            var objParameterExpr = Expression.Parameter(typeof(object), "instance");
            var instanceExpr = Expression.TypeAs(objParameterExpr, property.DeclaringType);
            var propertyExpr = Expression.Property(instanceExpr, property);
            var propertyObjExpr = Expression.Convert(propertyExpr, typeof(object));

            return Expression.Lambda<Func<object, object>>(propertyObjExpr, objParameterExpr).Compile();
        }

        private static Action<object, object> CreateSetterLambda(PropertyInfo property)
        {
            var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
            var instanceExpr = Expression.TypeAs(objInstanceExpr, property.DeclaringType);
            var propertyExpr = Expression.Property(instanceExpr, property);
            var objValueExpr = Expression.Parameter(typeof(object), "value");
            var valueExpr = Expression.Convert(objValueExpr, property.PropertyType);
            var propertyAssignExpr = Expression.Assign(propertyExpr, valueExpr);

            return Expression.Lambda<Action<object, object>>(propertyAssignExpr, objInstanceExpr, objValueExpr).Compile();
        }

        private class Property
        {
            public string Name { get; set; }

            public int Order { get; set; }

            public PropertyInfo Info { get; set; }

            public IHandler Handler { get; set; }
            
            public Func<object, object> Getter { get; set; }
            
            public Action<object, object> Setter { get; set; }

            public override string ToString()
            {
                return $"{Info.DeclaringType.Name}.{Info.Name}{(Name != Info.Name ? $"({Name})" : string.Empty)}";
            }
        }
    }
}
