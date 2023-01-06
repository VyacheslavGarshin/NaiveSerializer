using Naive.Serializer.Cogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Naive.Serializer.Handlers
{
    internal class IDictionaryHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.IDictionary;

        private readonly Type _keyType;

        private readonly Type _itemType;

        private readonly IHandler _keyHandler;

        private readonly IHandler _itemHandler;
        
        private readonly bool _isKeyNullable;

        private readonly bool _isKnownItemType;

        public IDictionaryHandler(Type type) : base(type)
        {
            _isKeyNullable = true;
            IsNullable = true;
            IsSimple = false;

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

                if (_keyType.IsValueType && Nullable.GetUnderlyingType(_keyType) == null)
                {
                    _isKeyNullable = false;
                }

                if (_itemType.IsValueType && Nullable.GetUnderlyingType(_itemType) == null)
                {
                    IsNullable = false;
                }
            }

            _isKnownItemType = _itemType != typeof(object);
        }

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IDictionary));
        }

        public override void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((byte)(_isKeyNullable ? HandlerType.Null : _keyHandler.HandlerType));
            writer.Write((byte)(IsNullable ? HandlerType.Null : _itemHandler.HandlerType));
            var list = (IDictionary)obj;

            writer.Write7BitEncodedInt(list.Count);

            foreach (DictionaryEntry item in list)
            {
                if (_isKeyNullable)
                {
                    NaiveSerializer.Write(writer, item.Key, options, _keyHandler);
                }
                else
                {
                    _keyHandler.Write(writer, item.Key, options);
                }

                if (IsNullable)
                {
                    NaiveSerializer.Write(writer, item.Value, options, _itemHandler);
                }
                else
                {
                    _itemHandler.Write(writer, item.Value, options);
                }
            }
        }

        public override object Read(BinaryReaderInternal reader, NaiveSerializerOptions options)
        {
            var keyHandlerType = (HandlerType)reader.ReadByte();
            var handlerType = (HandlerType)reader.ReadByte();
            var count = reader.Read7BitEncodedInt();

            var isKeyNullable = keyHandlerType == HandlerType.Null;
            var keyHandler = !isKeyNullable && (_keyHandler == null || _keyHandler.HandlerType != keyHandlerType)
                ? NaiveSerializer.GetHandler(keyHandlerType)
                : _keyHandler;

            var isNullable = handlerType == HandlerType.Null;
            var itemHandler = !isNullable && (_itemHandler == null || _itemHandler.HandlerType != handlerType)
                ? NaiveSerializer.GetHandler(handlerType)
                : _itemHandler;

            var result = (IDictionary)Activator.CreateInstance(Type);

            var addMethod = result.GetType().GetMethod("Add");

            for (var i = 0; i < count; i++)
            {
                var key = ReadKey(reader, options, isKeyNullable, keyHandler);
                var value = ReadValue(reader, options, isNullable, itemHandler);

                addMethod.Invoke(result, new[] { key, value });
            }

            return result;
        }

        private object ReadKey(BinaryReaderInternal reader, NaiveSerializerOptions options, bool isKeyNullable, IHandler keyHandler)
        {
            object result;

            if (isKeyNullable)
            {
                result = NaiveSerializer.Read(reader, _keyType, options, keyHandler);
            }
            else
            {
                result = keyHandler.Read(reader, options);
            }

            return result;
        }

        private object ReadValue(BinaryReaderInternal reader, NaiveSerializerOptions options, bool isNullable, IHandler itemHandler)
        {
            object result;

            if (isNullable)
            {
                result = NaiveSerializer.Read(reader, _isKnownItemType ? _itemType : null, options, itemHandler);
            }
            else
            {
                result = itemHandler.Read(reader, options);
            }

            return result;
        }
    }
}
