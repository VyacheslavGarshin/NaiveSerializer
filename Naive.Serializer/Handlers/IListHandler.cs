using Naive.Serializer;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Naive.Serializer.Handlers
{
    public class IListHandler : AbstractHandler<IListHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.IList;

        private Type _itemType;
        
        private IHandler _itemHandler;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IList));
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

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
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            var list = (IList)obj;

            writer.Write(list.Count);

            foreach (var item in list)
            {
                NaiveSerializer.Write(writer, item, options, _itemHandler);
            }
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {           
            var count = reader.ReadInt32();

            var result = Type.IsArray ? Array.CreateInstance(_itemType, count) : (IList)Activator.CreateInstance(Type);

            for (var i = 0; i < count; i++)
            {
                var item = NaiveSerializer.Read(reader, _itemType, options, _itemHandler);

                if (Type.IsArray)
                {
                    result[i] = item;
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
