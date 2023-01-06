using Naive.Serializer.Cogs;
using Naive.Serializer.Handlers;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;

namespace Naive.Serializer
{
    /// <summary>
    /// Contains methods for serializing/deserializing objects into binary format.
    /// </summary>
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
                var handler = Activator.CreateInstance(handlerType, new object[] { null }) as IHandler;

                if (_handlers[(int)handler.HandlerType] != null)
                {
                    throw new Exception($"Handler for {handler.HandlerType} already registered.");
                }

                _handlers[(int)handler.HandlerType] = handler;
            }
        }

        /// <summary>
        /// Serialize object into byte array.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj, NaiveSerializerOptions options = null)
        {
            using var ms = new MemoryStream();
            Serialize(obj, ms, options);
            return ms.ToArray();
        }

        /// <summary>
        /// Serialize object into stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static void Serialize(object obj, Stream stream, NaiveSerializerOptions options = null)
        {
            using var writer = new BinaryWriterInternal(stream, Encoding.UTF8, true);

            Write(writer, obj, options ?? new());
        }

        /// <summary>
        /// Deserialize ReadOnlyMemory.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object Deserialize(ReadOnlyMemory<byte> rom, NaiveSerializerOptions options = null)
        {
            using var stream = new RomStream(rom);
            return Deserialize(stream, null, options);
        }

        /// <summary>
        /// Deserialize byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] bytes, NaiveSerializerOptions options = null)
        {
            return Deserialize(bytes, null, options);
        }

        /// <summary>
        /// Deserialize stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object Deserialize(Stream stream, NaiveSerializerOptions options = null)
        {
            return Deserialize(stream, null, options);
        }

        /// <summary>
        /// Deserialize ReadOnlyMemory with type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rom"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T Deserialize<T>(ReadOnlyMemory<byte> rom, NaiveSerializerOptions options = null)
        {
            using var stream = new RomStream(rom);
            return (T)Deserialize(stream, typeof(T), options);
        }

        /// <summary>
        /// Deserialize byte array with type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] bytes, NaiveSerializerOptions options = null)
        {
            return (T)Deserialize(bytes, typeof(T), options);
        }

        /// <summary>
        /// Deserialize stream with type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream, NaiveSerializerOptions options = null)
        {
            return (T)Deserialize(stream, typeof(T), options);
        }

        /// <summary>
        /// Deserialize ReadOnlyMemory with type parameter.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object Deserialize(ReadOnlyMemory<byte> bytes, Type type, NaiveSerializerOptions options = null)
        {
            if (bytes.Length == 0)
            {
                return null;
            }

            using var stream = new RomStream(bytes);

            return Deserialize(stream, type, options);
        }

        /// <summary>
        /// Deserialize byte array with type parameter
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] bytes, Type type, NaiveSerializerOptions options = null)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using var ms = new MemoryStream(bytes);

            return Deserialize(ms, type, options);
        }

        /// <summary>
        /// Deserialize stream with type parameter
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object Deserialize(Stream stream, Type type, NaiveSerializerOptions options = null)
        {
            if (stream.Length == 0)
            {
                return null;
            }

            using var reader = new BinaryReaderInternal(stream, Encoding.UTF8, true);

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
                        result = (IHandler)Activator.CreateInstance(handler.GetType(), type);
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

        internal static void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options, IHandler handler = null)
        {
            if (obj == null)
            {
                writer.Write((byte)HandlerType.Null);
                return;
            }

            handler ??= GetTypeHandler(obj.GetType());

            writer.Write((byte)handler.HandlerType);

            handler.Write(writer, obj, options);
        }

        internal static object Read(BinaryReaderInternal reader, Type type, NaiveSerializerOptions options, IHandler handler = null)
        {
            var handlerType = (HandlerType)reader.ReadByte();

            if (handlerType == HandlerType.Null)
            {
                return null;
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
