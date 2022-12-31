using NaiveSerializer.Handlers;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace NaiveSerializer
{
    public static class NaiveSerializer
    {
        private static readonly IHandler[] _handlers;

        private static readonly ConcurrentDictionary<Type, IHandler> _typeHandlers = new();

        static NaiveSerializer()
        {
            var maxHandlerType = 0;

            foreach (var val in Enum.GetValues(typeof(HandlerType)))
            {
                if (maxHandlerType < (int)val)
                {
                    maxHandlerType = (int)val;
                }
            }

            _handlers = new IHandler[maxHandlerType + 1];

            foreach (var handlerType in typeof(NaiveSerializer).Assembly.GetTypes()
                .Where(x => x.GetInterface(nameof(IHandler)) != null && !x.IsInterface))
            {
                var handler = Activator.CreateInstance(handlerType) as IHandler;

                if (_handlers[(int)handler.HandlerType] != null)
                {
                    throw new Exception($"Handler for {handler.HandlerType} already registered.");
                }

                _handlers[(int)handler.HandlerType] = handler;
            }
        }

        public static byte[] Serialize(object obj)
        {
            using var ms = new MemoryStream();
            Serialize(obj, ms);
            return ms.ToArray();
        }

        public static void Serialize(object obj, Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

            Serialize(writer, obj);
        }

        public static object Deserialize(byte[] bytes)
        {
            return Deserialize(bytes, null);
        }

        public static object Deserialize(Stream stream)
        {
            return Deserialize(stream, null);
        }

        public static T Deserialize<T>(byte[] bytes)
        { 
            return (T)Deserialize(bytes, typeof(T));
        }

        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        public static object Deserialize(byte[] bytes, Type type)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using var ms = new MemoryStream(bytes);
            
            return Deserialize(ms, type);
        }

        public static object Deserialize(Stream stream, Type type)
        {
            if (stream.Length == 0)
            {
                return null;
            }

            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            return Deserialize(reader, type);
        }

        public static IHandler GetHandler(HandlerType handlerType)
        {
            var result = _handlers[(byte)handlerType];

            if (result == null)
            {
                throw new NotSupportedException($"Handler for type {handlerType} is not supported.");
            }

            return result;
        }

        public static IHandler GetTypeHandler(Type type)
        {
            if (!_typeHandlers.TryGetValue(type, out var result))
            {
                foreach (var handler in _handlers)
                {
                    if (handler != null && handler.Match(type))
                    {
                        _typeHandlers.TryAdd(type, handler.Create(type) ?? handler);
                        result = handler;
                        break;
                    }
                }
            }

            if (result == null)
            {
                throw new NotSupportedException($"Handler for type {type.Name} is not found.");
            }

            return result;
        }

        private static void Serialize(BinaryWriter writer, object obj)
        {
            _handlers[(int)HandlerType.Null].Write(writer, obj, null);

            if (obj == null)
            {
                return;
            }

            var type = obj.GetType();

            var handler = GetTypeHandler(type);
            
            writer.Write((byte)handler.HandlerType);

            handler.Write(writer, obj, type);
        }

        private static object Deserialize(BinaryReader reader, Type type)
        {
            if ((byte)_handlers[(int)HandlerType.Null].Read(reader, type) == 0)
            {
                return null;
            };

            var handlerType = reader.ReadByte();

            if (handlerType > _handlers.Length - 1)
            {
                throw new IndexOutOfRangeException($"Handler type {handlerType} is out of range.");
            }

            return GetHandler((HandlerType)handlerType).Read(reader, type);
        }       
    }
}
