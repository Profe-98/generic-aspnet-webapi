﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WebApiFunction.Web.Authentification
{
    public class AuthentificationTokenModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
