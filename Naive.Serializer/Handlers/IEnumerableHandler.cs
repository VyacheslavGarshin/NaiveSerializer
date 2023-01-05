using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Naive.Serializer.Handlers
{
    internal class IEnumerableHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.IEnumerable;

        private readonly Type _itemType;
        
        private readonly IHandler _itemHandler;
        
        private readonly bool _createArray;
        
        private readonly bool _isCollection;
        
        private readonly bool _isIEnumerable;

        private readonly bool _isList;
        
        private readonly bool _isKnownItemType;

        public IEnumerableHandler(Type type) : base(type)
        {
            IsNullable = true;
            IsSimple = false;

            if (Type == null)
            {
                Type = typeof(object[]);
                _itemType = typeof(object);
            }
            else
            {
                _itemType = Type.IsArray ? Type.GetElementType() : Type.GetGenericArguments()[0];

                if (_itemType != typeof(object))
                {
                    _itemHandler = NaiveSerializer.GetTypeHandler(_itemType);             
                }

                if (_itemType.IsValueType && Nullable.GetUnderlyingType(_itemType) == null)
                {
                    IsNullable = false;
                }
            }

            _createArray = Type.IsArray || Type.IsInterface || Type.ReflectedType == typeof(Enumerable);
            _isCollection = Type.GetInterface(nameof(ICollection)) != null;
            _isList = Type.GetInterface(nameof(IList)) != null;
            _isKnownItemType = _itemType != typeof(object);

            if (!_isCollection)
            {
                _isIEnumerable = Type.GetInterface(nameof(IEnumerable)) != null;
            }
        }

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((byte)(IsNullable ? HandlerType.Null : _itemHandler.HandlerType));
            writer.Write(GetCount(obj));

            foreach (var item in obj as IEnumerable)
            {
                WriteItem(writer, options, item);
            }
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var handlerType = (HandlerType)reader.ReadByte();
            var count = reader.ReadInt32();

            var isNullable = handlerType == HandlerType.Null;
            var itemHandler = !isNullable && (_itemHandler == null || _itemHandler.HandlerType != handlerType)
                ? NaiveSerializer.GetHandler(handlerType)
                : _itemHandler;

            var result = _createArray ? Array.CreateInstance(_itemType, count) : Activator.CreateInstance(Type);

            var addMethod = GetAddMethod(result);
            var asIList = result as IList;

            for (var i = 0; i < count; i++)
            {
                var item = ReadItem(reader, options, isNullable, itemHandler);

                AddItem(result, asIList, addMethod, i, item);
            }

            return result;
        }

        private int GetCount(object obj)
        {
            var result = 0;

            if (_isCollection)
            {
                result = ((ICollection)obj).Count;
            }
            else if (_isIEnumerable)
            {
                foreach (var item in (IEnumerable)obj)
                {
                    result++;
                }
            }
            else
            {
                throw new NotSupportedException($"Type '{Type.Name}' is not supported in lists.");
            }

            return result;
        }

        private MethodInfo GetAddMethod(object collection)
        {
            MethodInfo result = null;

            if (!_createArray && !_isList)
            {
                result = collection.GetType().GetMethod("Add");

                if (result == null)
                {
                    throw new NotSupportedException($"Cannot find Add method on type {collection.GetType().Name}.");
                }
            }

            return result;
        }

        private void WriteItem(BinaryWriter writer, NaiveSerializerOptions options, object item)
        {
            if (IsNullable)
            {
                NaiveSerializer.Write(writer, item, options, _itemHandler);
            }
            else
            {
                _itemHandler.Write(writer, item, options);
            }
        }

        private object ReadItem(BinaryReader reader, NaiveSerializerOptions options, bool isNullable, IHandler itemHandler)
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

        private void AddItem(object result, IList asIList, MethodInfo addMethod, int i, object item)
        {
            if (_createArray)
            {
                asIList[i] = item;
            }
            else if (_isList)
            {
                asIList.Add(item);
            }
            else
            {
                addMethod.Invoke(result, new object[] { item });
            }
        }
    }
}
