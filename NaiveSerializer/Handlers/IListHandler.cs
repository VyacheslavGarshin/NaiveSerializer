using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NaiveSerializer.Handlers
{
    public class IListHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.IList;

        public bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IList));
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            var collection = (IList)obj;
            
            var itemType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
            var itemHandler = NaiveSerializer.GetTypeHandler(itemType);
            var nullHandler = NaiveSerializer.GetHandler((byte)HandlerType.Null);

            writer.Write(collection.Count);

            foreach (var item in collection)
            {
                nullHandler.Write(writer, item, itemType);

                if (item != null)
                {
                    itemHandler.Write(writer, item, itemType);
                }
            }
        }

        public object Read(BinaryReader reader, Type type)
        {
            var itemType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

            var count = reader.ReadInt32();
            var result = type.IsArray ? Array.CreateInstance(itemType, count) : (IList)Activator.CreateInstance(type);

            var itemHandler = NaiveSerializer.GetTypeHandler(itemType);
            var nullHandler = NaiveSerializer.GetHandler((byte)HandlerType.Null);

            for (var i = 0; i < count; i++)
            {
                object item = null;

                if ((byte)nullHandler.Read(reader, itemType) != 0)
                {
                    item = itemHandler.Read(reader, itemType);
                }

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
