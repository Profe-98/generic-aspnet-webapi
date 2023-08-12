using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiLinkModel : BaseModel
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
                _self = string.IsNullOrEmpty(value) ? null : value.ToLower();
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
                _next = string.IsNullOrEmpty(value) ? null : value.ToLower();
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
                _previous = string.IsNullOrEmpty(value) ? null : value.ToLower();
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
                _last = string.IsNullOrEmpty(value) ? null : value.ToLower();
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
                _related = string.IsNullOrEmpty(value) ? null : value.ToLower();
            }
        }
        [JsonPropertyName("meta")]
        public ApiMetaModel Meta { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
