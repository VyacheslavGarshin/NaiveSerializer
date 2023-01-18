using Naive.Serializer.Cogs;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Naive.Serializer.Handlers
{
    internal class ObjectHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Object;

        private readonly ConcurrentDictionary<ReadOnlyMemory<byte>, Property> _properties = new(new BytesComparer());

        private readonly Property[] _sortedProperties = new Property[0];

        private readonly bool _isKnownObject;
        
        private readonly Func<object> _creator;

        private readonly QuickTable<Property> _quickTable = new();

        private readonly bool _isQuickTableFull;

        public ObjectHandler (Type type) : base(type)
        {
            IsObject = true;
            IsNullable = true;
            IsSimple = false;

            if (Type == null)
            {
                Type = typeof(object);
            }
            else
            {
                SetIsNullable(type);

                var dataContract = Type.GetCustomAttribute<DataContractAttribute>();

                var definitions = new List<Property>();

                foreach (var definition in GetDefinitionCandidates())
                {
                    var memberInfo = (MemberInfo)definition.PropertyInfo ?? definition.FieldInfo;

                    if (memberInfo.GetCustomAttribute<IgnoreDataMemberAttribute>() != null
                        || (dataContract != null && memberInfo.GetCustomAttribute<DataMemberAttribute>() == null))
                    {
                        continue;
                    }

                    PrepareDefinition(definition, memberInfo);

                    if (!_properties.TryAdd(new ReadOnlyMemory<byte>(definition.NameBytes), definition))
                    {
                        throw new ArgumentException($"Property or field '{definition.Name}' already registered on object '{Type.FullName}'.");
                    };

                    if (definition.OriginalName != definition.Name && !_properties.TryAdd(new ReadOnlyMemory<byte>(definition.OriginalNameBytes), definition))
                    {
                        throw new ArgumentException($"Property or field '{definition.OriginalName}' already registered on object '{Type.FullName}'.");
                    };

                    definitions.Add(definition);
                }

                _sortedProperties = definitions.OrderBy(x => x.Order).ToArray();

                foreach (var prop in _sortedProperties)
                {
                    _quickTable.Add(prop.NameBytes, prop);
                }

                _isQuickTableFull = _quickTable.Count() == _sortedProperties.Length;
            }

            _isKnownObject = Type != typeof(object);

            _creator = CreateCreator();
        }

        public override bool Match(Type type)
        {
            return true;
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            foreach (var property in _sortedProperties)
            {
                var value = property.GetValue(obj);

                if (value != null || !context.Options.IgnoreNullValue)
                {
                    writer.Write((byte)property.NameBytes.Length);
                    writer.Write(property.NameBytes);

                    property.Handler ??= NaiveSerializer.GetTypeHandler(property.MemberType);

                    NaiveSerializer.Write(writer, value, context, property.Handler);
                }
            }

            writer.Write((byte)0);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            var result = _isKnownObject ? _creator() : new Dictionary<string, object>();

            do
            {
                var nameLength = reader.ReadByte();

                if (nameLength == 0)
                {
                    break;
                }

                reader.BaseStream.Read(context.NameBuffer, 0, nameLength);
                var nameRom = new ReadOnlyMemory<byte>(context.NameBuffer, 0, nameLength);

                object value;

                if (_isKnownObject)
                {
                    Property property = null;

                    if (context.Options.UsePropertiesIndex)
                    {
                        property = _quickTable.Get(nameRom, context.Options.OptimisticIndexSearch && _isQuickTableFull);
                    }

                    if (property == null)
                    {
                        _properties.TryGetValue(nameRom, out property);
                    }

                    if (property != null)
                    {
                        property.Handler ??= NaiveSerializer.GetTypeHandler(property.MemberType);

                        value = NaiveSerializer.Read(reader, property.MemberType, context, property.Handler);
                        property.SetValue(result, value);
                    }
                    else
                    {
                        if (!context.Options.IgnoreMissingMember)
                        {
                            throw new MissingMemberException($"Property with name '{Encoding.UTF8.GetString(nameRom.Span)}' is not found on class '{result.GetType().FullName}'.");
                        }

                        // skipread
                        NaiveSerializer.Read(reader, null, context);
                    }
                }
                else
                {
                    var name = Encoding.UTF8.GetString(nameRom.Span);
                    value = NaiveSerializer.Read(reader, null, context);
                    
                    ((Dictionary<string, object>)result).Add(name, value);
                }
            } while (true);

            return result;
        }

        private Property[] GetDefinitionCandidates()
        {
            return
                Type.GetProperties().Where(x => x.CanRead && x.CanWrite)
                    .Select(x => new Property { PropertyInfo = x, OriginalName = x.Name, Name = x.Name }).Concat(
                Type.GetFields().Where(x => x.IsPublic)
                    .Select(x => new Property { FieldInfo = x, OriginalName = x.Name, Name = x.Name }))
                .ToArray();
        }

        private void SetIsNullable(Type type)
        {
            if (type.IsValueType)
            {
                var nullableStruct = Nullable.GetUnderlyingType(type);

                if (nullableStruct != null)
                {
                    Type = nullableStruct;
                }
                else
                {
                    IsNullable = false;
                }
            }
        }

        private void PrepareDefinition(Property definition, MemberInfo memberInfo)
        {
            var dataMember = memberInfo.GetCustomAttribute<DataMemberAttribute>();

            if (dataMember != null)
            {
                definition.Order = dataMember.Order;

                if (!string.IsNullOrEmpty(dataMember.Name))
                {
                    definition.Name = dataMember.Name;
                }
            }

            if (definition.OriginalName != definition.Name)
            {
                definition.OriginalNameBytes = Encoding.UTF8.GetBytes(definition.OriginalName);
            }

            definition.NameBytes = Encoding.UTF8.GetBytes(definition.Name);
            definition.MemberType = definition.PropertyInfo?.PropertyType ?? definition.FieldInfo.FieldType;
            definition.GetValue = CreateGetter(definition);
            definition.SetValue = CreateSetter(definition);
        }

        private Func<object> CreateCreator()
        {
            var newExpr = Expression.New(Type);
            var toObjExpr = Type.IsValueType
               ? Expression.Convert(newExpr, typeof(object))
               : Expression.TypeAs(newExpr, typeof(object));
            return Expression.Lambda<Func<object>>(toObjExpr).Compile();
        }

        private Func<object, object> CreateGetter(Property property)
        {
            var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
            var instanceExpr = Type.IsValueType
                ? Expression.Convert(objInstanceExpr, Type)
                : Expression.TypeAs(objInstanceExpr, Type);
            var propertyExpr = property.PropertyInfo != null
                ? Expression.Property(instanceExpr, property.PropertyInfo)
                : Expression.Field(instanceExpr, property.FieldInfo);
            var propertyObjExpr = property.MemberType.IsValueType 
                ? Expression.Convert(propertyExpr, typeof(object))
                : Expression.TypeAs(propertyExpr, typeof(object));

            return Expression.Lambda<Func<object, object>>(propertyObjExpr, objInstanceExpr).Compile();
        }

        private Action<object, object> CreateSetter(Property property)
        {
            var propertyType = property.MemberType;

            if (Type.IsValueType)
            {
                return property.PropertyInfo != null ? property.PropertyInfo.SetValue : property.FieldInfo.SetValue;
            }
            else
            {
                var objInstanceExpr = Expression.Parameter(typeof(object), "instance");
                var instanceExpr = Expression.Convert(objInstanceExpr, Type);
                var propertyExpr = property.PropertyInfo != null
                    ? Expression.Property(instanceExpr, property.PropertyInfo)
                    : Expression.Field(instanceExpr, property.FieldInfo);
                var objValueExpr = Expression.Parameter(typeof(object), "value");
                var valueExpr = property.MemberType.IsValueType 
                    ? Expression.Convert(objValueExpr, propertyType)
                    : Expression.TypeAs(objValueExpr, propertyType);
                var propertyAssignExpr = Expression.Assign(propertyExpr, valueExpr);

                return Expression.Lambda<Action<object, object>>(propertyAssignExpr, objInstanceExpr, objValueExpr).Compile();
            }
        }

        private class Property
        {
            public string OriginalName { get; set; }

            public byte[] OriginalNameBytes { get; set; }

            public string Name { get; set; }

            public byte[] NameBytes { get; set; }

            public int Order { get; set; }

            public PropertyInfo PropertyInfo { get; set; }

            public FieldInfo FieldInfo { get; set; }

            public Type MemberType { get; set; }

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
