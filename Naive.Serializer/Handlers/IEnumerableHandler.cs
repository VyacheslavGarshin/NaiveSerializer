using Naive.Serializer.Cogs;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
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
        
        private readonly Func<object> _creator;

        public IEnumerableHandler(Type type) : base(type)
        {
            IsObject = true;
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

            if (!_createArray)
            {
                _creator = CreateCreator();
            }
        }

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write((byte)(IsNullable ? HandlerType.Null : _itemHandler.HandlerType));
            writer.Write7BitEncodedInt(GetCount(obj));

            foreach (var item in obj as IEnumerable)
            {
                WriteItem(writer, context, item);
            }
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            var handlerType = (HandlerType)reader.ReadByte();
            var count = reader.Read7BitEncodedInt();

            var isNullable = handlerType == HandlerType.Null;
            var itemHandler = !isNullable && (_itemHandler == null || _itemHandler.HandlerType != handlerType)
                ? NaiveSerializer.GetHandler(handlerType)
                : _itemHandler;

            var result = _createArray ? Array.CreateInstance(_itemType, count) : _creator();

            var addMethod = GetAddMethod(result);
            var asIList = result as IList;

            for (var i = 0; i < count; i++)
            {
                var item = ReadItem(reader, context, isNullable, itemHandler);

                AddItem(result, asIList, addMethod, i, item);
            }

            return result;
        }

        private Func<object> CreateCreator()
        {
            var newExpr = Expression.New(Type);
            var toObjExpr = Expression.TypeAs(newExpr, typeof(object));
            return Expression.Lambda<Func<object>>(toObjExpr).Compile();
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

        private void WriteItem(BinaryWriterInternal writer, Context context, object item)
        {
            if (IsNullable)
            {
                NaiveSerializer.Write(writer, item, context, _itemHandler);
            }
            else
            {
                _itemHandler.Write(writer, item, context);
            }
        }

        private object ReadItem(BinaryReaderInternal reader, Context context, bool isNullable, IHandler itemHandler)
        {
            object result;

            if (isNullable)
            {
                result = NaiveSerializer.Read(reader, _isKnownItemType ? _itemType : null, context, itemHandler);
            }
            else
            {
                result = itemHandler.Read(reader, context);
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
