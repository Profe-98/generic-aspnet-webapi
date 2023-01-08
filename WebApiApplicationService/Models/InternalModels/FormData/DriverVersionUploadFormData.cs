using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApiApplicationService.InternalModels
{
    public class DriverVersionUploadFormData : GeneralMimeFileFormData
    {
        public new IFormFile File { get => FileLibrary; set => FileLibrary = value; }
        public new string FileName { get => FileNameLibrary; set => FileNameLibrary = value; }

        [Required()]
        public IFormFile FileLibrary { get; set; }
        [Required()]
        public string FileNameLibrary { get; set; }
        [Required()]
        public IFormFile FileSetup { get; set; }
        [Required()]
        public string FileNameSetup { get; set; }

        public DriverVersionUploadFormData() : base()
        {

        }
    }
}
