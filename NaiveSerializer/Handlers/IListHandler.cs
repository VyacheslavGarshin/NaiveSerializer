using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace NaiveSerializer.Handlers
{
    public class IListHandler : AbstractHandler<IListHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.IList;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IList));
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            var list = (IList)obj;

            var itemType = WriteType.IsArray ? WriteType.GetElementType() : WriteType.GetGenericArguments()[0];

            IHandler itemHandler = null;

            if (itemType != typeof(object))
            {
                itemHandler = NaiveSerializer.GetTypeHandler(itemType);
            }

            writer.Write(list.Count);

            foreach (var item in list)
            {
                NaiveSerializer.Write(writer, item, options, itemHandler);
            }
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            if (type == null)
            {
                type = typeof(object[]);
            }

            var itemType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

            var count = reader.ReadInt32();
            var result = type.IsArray ? Array.CreateInstance(itemType, count) : (IList)Activator.CreateInstance(type);

            IHandler itemHandler = null;

            if (itemType != typeof(object))
            {
                itemHandler = NaiveSerializer.GetTypeHandler(itemType);
            }

            for (var i = 0; i < count; i++)
            {
                object item = NaiveSerializer.Read(reader, itemType, options, itemHandler);

                if (type.IsArray)
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
