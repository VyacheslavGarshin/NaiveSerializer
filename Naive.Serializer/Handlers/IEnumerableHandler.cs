using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Naive.Serializer.Handlers
{
    public class IEnumerableHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.IEnumerable;

        private Type _itemType;
        
        private IHandler _itemHandler;
        
        private bool _createArray;
        
        private bool _isCollection;
        
        private bool _isIEnumerable;

        private bool _isList;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

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

            if (!_isCollection)
            {
                _isIEnumerable = Type.GetInterface(nameof(IEnumerable)) != null;
            }

            _isList = Type.GetInterface(nameof(IList)) != null;
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            int count = 0;

            if (_isCollection)
            {
                count = ((ICollection)obj).Count;
            }
            else if (_isIEnumerable)
            {
                foreach (var item in (IEnumerable)obj)
                {
                    count++;
                }
            }
            else
            {
                throw new NotSupportedException($"Type '{Type.Name}' is not supported in lists.");
            }

            writer.Write((byte)(IsNullable ? HandlerType.Null : _itemHandler.HandlerType));
            writer.Write(count);

            foreach (var item in obj as IEnumerable)
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

            for (var i = 0; i < count; i++)
            {
                var item = ReadItem(reader, options, isNullable, itemHandler);

                AddItem(result, addMethod, i, item);
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

        private object ReadItem(BinaryReader reader, NaiveSerializerOptions options, bool isNullable, IHandler itemHandler)
        {
            object result;

            if (isNullable)
            {
                result = NaiveSerializer.Read(reader, _itemType != typeof(object) ? _itemType : null, options, itemHandler);
            }
            else
            {
                result = itemHandler.Read(reader, options);
            }

            return result;
        }

        private void AddItem(object result, MethodInfo addMethod, int i, object item)
        {
            if (_createArray)
            {
                ((IList)result)[i] = item;
            }
            else if (_isList)
            {
                ((IList)result).Add(item);
            }
            else
            {
                addMethod.Invoke(result, new object[] { item });
            }
        }
    }
}
