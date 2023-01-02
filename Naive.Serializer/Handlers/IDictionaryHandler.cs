using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Naive.Serializer.Handlers
{
    public class IDictionaryHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.IDictionary;

        private Type _keyType;

        private Type _itemType;

        private IHandler _keyHandler;

        private IHandler _itemHandler;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IDictionary));
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            IsNullable = true;

            if (Type == null)
            {
                Type = typeof(Dictionary<object, object>);
                _keyType = typeof(object);
                _itemType = typeof(object);
            }
            else
            {
                _keyType = Type.GetGenericArguments()[0];
                _itemType = Type.GetGenericArguments()[1];

                if (_keyType != typeof(object))
                {
                    _keyHandler = NaiveSerializer.GetTypeHandler(_keyType);
                }

                if (_itemType != typeof(object))
                {
                    _itemHandler = NaiveSerializer.GetTypeHandler(_itemType);             
                }
            }
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            var list = (IDictionary)obj;

            writer.Write(list.Count);

            foreach (DictionaryEntry item in list)
            {
                NaiveSerializer.Write(writer, item.Key, options, _keyHandler);
                NaiveSerializer.Write(writer, item.Value, options, _itemHandler);
            }
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {           
            var count = reader.ReadInt32();

            var result = (IDictionary)Activator.CreateInstance(Type);

            var addMethod = result.GetType().GetMethod("Add");

            for (var i = 0; i < count; i++)
            {
                var key = NaiveSerializer.Read(reader, _keyType, options, _keyHandler);
                var value = NaiveSerializer.Read(reader, _itemType, options, _itemHandler);

                addMethod.Invoke(result, new[] { key, value });
            }

            return result;
        }
    }
}
