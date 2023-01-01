using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NaiveSerializer.Handlers
{
    public class ObjectHandler : AbstractHandler<ObjectHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Object;

        private ConcurrentDictionary<string, Property> _properties = new();

        private Property[] _sortedProperties;

        public override bool Match(Type type)
        {
            return type.IsClass;
        }

        public override IHandler Create(Type type)
        {
            var result = base.Create(type) as ObjectHandler;

            var dataContract = type.GetCustomAttribute<DataContractAttribute>();

            var definitions = type.GetProperties()
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

                result._properties.TryAdd(definition.Name, definition);
            }

            result._sortedProperties = definitions.OrderBy(x => x.Order).ToArray();

            return result;
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            foreach (var property in _sortedProperties)
            {
                var value = property.Info.GetValue(obj);

                if (value != null || !options.IgnoreNullValue)
                {
                    writer.Write(property.Name);

                    property.Handler ??= NaiveSerializer.GetTypeHandler(property.Info.PropertyType);

                    NaiveSerializer.Write(writer, value, options, property.Handler);
                }
            }

            writer.Write(string.Empty);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            var isObject = type != null;
            var result = isObject ? Activator.CreateInstance(type) : new Dictionary<string, object>();

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
                    
                    property.Info.SetValue(result, value);
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

        private class Property
        {
            public string Name { get; set; }

            public int Order { get; set; }

            public PropertyInfo Info { get; set; }

            public IHandler Handler { get; set; }            

            public override string ToString()
            {
                return $"{Info.DeclaringType.Name}.{Info.Name}{(Name != Info.Name ? $"({Name})" : string.Empty)}";
            }
        }
    }
}
