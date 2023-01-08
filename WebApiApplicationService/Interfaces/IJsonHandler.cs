using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models;
using WebApiApplicationService.Handler;
using System.Text.Json;

namespace WebApiApplicationService
{
    public interface IJsonHandler
    {
        public string JsonSerialize<T>(T obj, JsonSerializerOptions presets = null);
        public T JsonDeserialize<T>(string json, JsonSerializerOptions presets = null);
        public object JsonDeserialize(string json, Type type, JsonSerializerOptions presets = null);
        public Dictionary<string,dynamic> JsonDeserialize(string json, JsonSerializerOptions presets = null);
    }

    public interface ISingletonJsonHandler : IJsonHandler
    {

    }
    public interface IScopedJsonHandler : IJsonHandler
    {

    }
}
