using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace WebApiFunction.Data.Format.Json
{
    public interface IJsonHandler
    {

        event EventHandler<Exception> ExceptionOccuredWhileSerializingEvent;
        event EventHandler<Exception> ExceptionOccuredWhileDeserializingEvent;

        public event EventHandler<Exception> OnExceptionWhileSerializingOccured
        {
            add => ExceptionOccuredWhileSerializingEvent += value;
            remove => ExceptionOccuredWhileSerializingEvent -= value;
        }
        public event EventHandler<Exception> OnExceptionWhileDeserializingOccured
        {
            add => ExceptionOccuredWhileDeserializingEvent += value;
            remove => ExceptionOccuredWhileDeserializingEvent -= value;
        }
        public string JsonSerialize<T>(T obj, JsonSerializerOptions presets = null);
        public T JsonDeserialize<T>(string json, JsonSerializerOptions presets = null);
        public object JsonDeserialize(string json, Type type, JsonSerializerOptions presets = null);
        public Dictionary<string, dynamic> JsonDeserialize(string json, JsonSerializerOptions presets = null);
    }

    public interface ISingletonJsonHandler : IJsonHandler
    {

    }
    public interface IScopedJsonHandler : IJsonHandler
    {

    }
}
