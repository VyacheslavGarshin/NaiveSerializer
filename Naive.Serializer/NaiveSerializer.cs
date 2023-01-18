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
            var context = new WriteContext(options ?? NaiveSerializerOptions.Default);

            Write(writer, obj, context);
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
            using var context = new ReadContext(options ?? NaiveSerializerOptions.Default);

            return Read(reader, type, context);
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

        internal static void Write(BinaryWriterInternal writer, object obj, WriteContext context, IHandler handler = null)
        {
            if (obj == null)
            {
                writer.Write((byte)HandlerType.Null);
                return;
            }

            handler ??= GetTypeHandler(obj.GetType());

            if (handler.IsObject)
            {
                if (context.Options.ReferenceLoopHandling != ReferenceLoopHandling.Serialize)
                {
                    if (context.Stack.Count > 0)
                    {
                        foreach (var parent in context.Stack)
                        {
                            if (obj == parent)
                            {
                                if (context.Options.ReferenceLoopHandling == ReferenceLoopHandling.Error)
                                {
                                    throw new ArgumentException($"Reference loop detected for object type '{obj.GetType().Name}'.");
                                }

                                writer.Write((byte)HandlerType.Null);
                                return;
                            }
                        }
                    }

                    context.Stack.Push(obj);
                }

                context.Depth++;

                if (context.Depth > context.Options.MaxDepth)
                {
                    throw new ArgumentException($"Maximum depth of {context.Options.MaxDepth} is reached.");
                }
            }

            writer.Write((byte)handler.HandlerType);

            handler.Write(writer, obj, context);

            if (handler.IsObject)
            {
                if (context.Options.ReferenceLoopHandling != ReferenceLoopHandling.Serialize)
                {
                    context.Stack.Pop();
                }

                context.Depth--;
            }
        }

        internal static object Read(BinaryReaderInternal reader, Type type, ReadContext context, IHandler handler = null)
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

            return handler.Read(reader, context);
        }
    }
}
