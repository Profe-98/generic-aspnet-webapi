using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebApiApplicationService.Models
{
    [Serializable]
    public class ApiLinkModel
    {
        #region Private
        private string _self = null;
        private string _next = null;
        private string _previous = null;
        private string _last = null;
        private string _related = null;
        #endregion
        #region Public
        [JsonPropertyName("self")]
        public string Self 
        {
            get
            {
                return _self;
            }
            set
            {
                _self = String.IsNullOrEmpty(value) ?null: value.ToLower();
            }
        }
        [JsonPropertyName("next")]
        public string Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = String.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("previous")]
        public string Previous
        {
            get
            {
                return _previous;
            }
            set
            {
                _previous = String.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("last")]
        public string Last
        {
            get
            {
                return _last;
            }
            set
            {
                _last = String.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("related")]
        public string Related
        {
            get
            {
                return _related;
            }
            set
            {
                _related = String.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("meta")]
        public ApiMetaModel Meta { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
