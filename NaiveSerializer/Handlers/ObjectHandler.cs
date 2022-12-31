using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace NaiveSerializer.Handlers
{
    public class ObjectHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Object;

        private ConcurrentDictionary<string, PropertyDefinition> _propertyDefinitions = new();

        public bool Match(Type type)
        {
            return type.IsClass;
        }

        public IHandler Create(Type type)
        {
            var result = new ObjectHandler();

            var definitions = type.GetProperties()
                .Where(x => x.CanRead && x.CanWrite && x.GetCustomAttribute<IgnoreDataMemberAttribute>() == null)
                .Select(x => new PropertyDefinition { PropertyInfo = x, Name = x.Name });

            foreach ( var definition in definitions )
            {
                var propertyInfo = definition.PropertyInfo;

                var dataMember = propertyInfo.GetCustomAttribute<DataMemberAttribute>();

                if (dataMember != null && !string.IsNullOrEmpty(dataMember.Name))
                {
                    definition.Name = dataMember.Name;
                }

                definition.Handler = NaiveSerializer.GetTypeHandler(propertyInfo.PropertyType);

                definition.NameBytes = Encoding.UTF8.GetBytes(definition.Name);

                result._propertyDefinitions.TryAdd(definition.Name, definition);
            }

            return result;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public object Read(BinaryReader reader, Type type)
        {
            throw new NotImplementedException();
        }

        private class PropertyDefinition
        {
            public string Name { get; set; }

            public byte[] NameBytes { get; set; }

            public PropertyInfo PropertyInfo { get; set; }

            public IHandler Handler { get; set; }

            public override string ToString()
            {
                return $"{PropertyInfo.DeclaringType.Name}.{PropertyInfo.Name}{(Name != PropertyInfo.Name ? $"({Name})" : string.Empty)}";
            }
        }
    }
}
