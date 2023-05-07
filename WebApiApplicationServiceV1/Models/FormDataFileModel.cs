using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApiApplicationService.Models
{
    public class FormDataFileModel
    {
        [Required()]
        public IFormFile File { get; set; }
        [Required()]
        public string FileName { get; set; }
    }
}
