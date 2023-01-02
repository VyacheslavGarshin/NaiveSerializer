using Naive.Serializer.Handlers;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace Naive.Serializer
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
                .Where(x => x.GetInterface(nameof(IHandler)) != null && !x.IsInterface && !x.IsAbstract))
            {
                var handler = Activator.CreateInstance(handlerType) as IHandler;
                handler.SetType(null);

                if (_handlers[(int)handler.HandlerType] != null)
                {
                    throw new Exception($"Handler for {handler.HandlerType} already registered.");
                }

                _handlers[(int)handler.HandlerType] = handler;
            }
        }

        public static byte[] Serialize(object obj, NaiveSerializerOptions options = null)
        {
            using var ms = new MemoryStream();
            Serialize(obj, ms, options);
            return ms.ToArray();
        }

        public static void Serialize(object obj, Stream stream, NaiveSerializerOptions options = null)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

            Write(writer, obj, options ?? new());
        }

        public static object Deserialize(byte[] bytes, NaiveSerializerOptions options = null)
        {
            return Deserialize(bytes, null, options);
        }

        public static object Deserialize(Stream stream, NaiveSerializerOptions options = null)
        {
            return Deserialize(stream, null, options);
        }

        public static T Deserialize<T>(byte[] bytes, NaiveSerializerOptions options = null)
        {
            return (T)Deserialize(bytes, typeof(T), options);
        }

        public static T Deserialize<T>(Stream stream, NaiveSerializerOptions options = null)
        {
            return (T)Deserialize(stream, typeof(T), options);
        }

        public static object Deserialize(byte[] bytes, Type type, NaiveSerializerOptions options = null)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using var ms = new MemoryStream(bytes);

            return Deserialize(ms, type, options);
        }

        public static object Deserialize(Stream stream, Type type, NaiveSerializerOptions options = null)
        {
            if (stream.Length == 0)
            {
                return null;
            }

            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            return Read(reader, type, options ?? new());
        }

        internal static IHandler GetHandler(HandlerType handlerType)
        {
            if ((byte)handlerType > _handlers.Length)
            {
                throw new IndexOutOfRangeException($"Handler type {handlerType} is out of range.");
            }

            var result = _handlers[(byte)handlerType];

            if (result == null)
            {
                throw new NotSupportedException($"Handler for type {handlerType} is not supported.");
            }

            return result;
        }

        internal static IHandler GetTypeHandler(Type type)
        {
            if (!_typeHandlers.TryGetValue(type, out var result))
            {
                foreach (var handler in _handlers)
                {
                    if (handler != null && handler.Match(type))
                    {
                        result = (IHandler)Activator.CreateInstance(handler.GetType());
                        result.SetType(type);
                        _typeHandlers.TryAdd(type, result);
                        return result;
                    }
                }
            }
            else
            {
                return result;
            }

            throw new NotSupportedException($"Handler for type {type.Name} is not found.");
        }

        internal static void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options, IHandler handler = null)
        {
            _handlers[(int)HandlerType.Null].Write(writer, obj, options);

            if (obj == null)
            {
                return;
            }

            handler ??= GetTypeHandler(obj.GetType());

            writer.Write((byte)handler.HandlerType);

            handler.Write(writer, obj, options);
        }

        internal static object Read(BinaryReader reader, Type type, NaiveSerializerOptions options, IHandler handler = null)
        {
            if ((byte)_handlers[(int)HandlerType.Null].Read(reader, options) == 0)
            {
                return null;
            };

            var handlerType = (HandlerType)reader.ReadByte();

            if ((int)handlerType > _handlers.Length - 1)
            {
                throw new IndexOutOfRangeException($"Handler type {handlerType} is out of range.");
            }

            if (handler == null)
            {
                handler = GetHandler(handlerType);

                if (!handler.IsSimple && type != null)
                {
                    handler = GetTypeHandler(type);
                }
            }

            if (handler.HandlerType != handlerType)
            {
                handler = GetHandler(handlerType);
            }

            return handler.Read(reader, options);
        }
    }
}
