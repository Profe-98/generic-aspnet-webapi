﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace WebApiApplicationService.Handler
{

    public class JsonHandler : IScopedJsonHandler, ISingletonJsonHandler, IDisposable
    {
        private bool _tryToFillValues = true;
        private JsonSerializerOptions _jsonSerializerOptions = null;
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryTooFillValues">When its set to true, the JsonDeserializer try to map equivalent fields from json to class T</param>
        public JsonHandler(bool tryTooFillValues = true, JsonSerializerOptions jsonSerializerOptions = null)
        {
            _tryToFillValues = tryTooFillValues;
            _jsonSerializerOptions = jsonSerializerOptions;
        }
        public static JsonSerializerOptions Settings = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        ~JsonHandler()
        {
            this.Dispose();
        }

        public string JsonSerialize<T>(T obj, JsonSerializerOptions presets = null)
        {
            if (obj == null)
                return null;

            if (presets == null && _jsonSerializerOptions == null)
                presets = Settings;

            presets = presets ?? _jsonSerializerOptions;
            try
            {

                string response = JsonSerializer.Serialize<T>(obj, presets);
                return response;
            }
            catch (Exception ex)
            {
                ExceptionHandler.ReportException(this.ToString(), ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            return null;
        }
        public T JsonDeserialize<T>(string json, JsonSerializerOptions presets = null)
        {
            if (String.IsNullOrEmpty(json))
                return default;

            Type t = typeof(T);
            if (presets == null && _jsonSerializerOptions == null)
                presets = Settings;

            presets = presets ?? _jsonSerializerOptions;
            try
            {
                object response = null;

                response = JsonSerializer.Deserialize<T>(json, presets);
                return (T)response;
            }
            catch (Exception ex)
            {
                if (_tryToFillValues)
                {
                    return (T)FillValues(json, t);
                }
                else
                {
                    ExceptionHandler.ReportException(this.ToString(), ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
            }
            return default;
        }
        public Dictionary<string,dynamic> JsonDeserialize(string json, JsonSerializerOptions presets = null)
        {
            if (String.IsNullOrEmpty(json))
                return default;

            if (presets == null)
                presets = Settings;
            try
            {
                Dictionary<string, dynamic> response = null;

                response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json, presets);
                return response;
            }
            catch (Exception ex)
            {
                ExceptionHandler.ReportException(this.ToString(), ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            return null;
        }
        public object JsonDeserialize(string json,Type type, JsonSerializerOptions presets = null)
        {
            if (String.IsNullOrEmpty(json))
                return default;

            if (presets == null)
                presets = Settings;
            try
            {
                object response = null;

                response = JsonSerializer.Deserialize(json,type, presets);
                return response;
            }
            catch (Exception ex)
            {
                if (_tryToFillValues)
                {
                    return FillValues(json, type);
                }
                else
                {
                    ExceptionHandler.ReportException(this.ToString(), ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
            }
            return default;
        }

        private object FillValues(string json,Type type)
        {
            object responseValue = Activator.CreateInstance(type);
            List<PropertyInfo> responseValueProperties = responseValue.GetType().GetProperties().ToList<PropertyInfo>();
            try
            {

                object tmpResponseObject = JsonSerializer.Deserialize(json, type);
                Type tmpType = tmpResponseObject.GetType();
                foreach (PropertyInfo property in tmpType.GetProperties())
                {
                    string tmpObjectPropertyName = property.Name;
                    object tmpObjectPropertyValue = property.GetValue(tmpResponseObject);
                    PropertyInfo p = responseValueProperties.Find(x => x.Name.ToLower().Equals(tmpObjectPropertyName.ToLower()));
                    if (p != null)
                    {
                        p.SetValue(responseValue, tmpObjectPropertyValue);
                    }
                }
            }
            catch(JsonException ex)
            {
                responseValue = null;
            }
            return responseValue;
        }
    }
}
