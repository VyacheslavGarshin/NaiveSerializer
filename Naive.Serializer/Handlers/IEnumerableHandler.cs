using System;
using System.Collections;
using System.Collections.Generic;
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

            writer.Write(count);

            foreach (var item in obj as IEnumerable)
            {
                NaiveSerializer.Write(writer, item, options, _itemHandler);
            }
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {           
            var count = reader.ReadInt32();
            
            var result = _createArray ? Array.CreateInstance(_itemType, count) : Activator.CreateInstance(Type);

            MethodInfo addMethod = null;

            if (!_createArray && !_isList)
            {
                addMethod = result.GetType().GetMethod("Add");

                if (addMethod == null)
                {
                    throw new NotSupportedException($"Cannot find Add method on type {result.GetType().Name}.");
                }
            }

            for (var i = 0; i < count; i++)
            {
                var item = NaiveSerializer.Read(reader, _itemType != typeof(object) ? _itemType : null, options, _itemHandler);

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

            return result;
        }
    }
}
