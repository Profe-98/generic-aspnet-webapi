using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApiApplicationService.InternalModels
{
    public class DriverMediaUploadFormData : GeneralMimeFileFormData
    {
        public new IFormFile File { get => FileIcon; set => FileIcon = value; }
        public new string FileName { get => FileNameIcon; set => FileNameIcon = value; }

        [Required()]
        public IFormFile FileBanner { get; set; }
        [Required()]
        public string FileNameBanner { get; set; }
        [Required()]
        public IFormFile FileIcon { get; set; }
        [Required()]
        public string FileNameIcon { get; set; }

        public DriverMediaUploadFormData() : base()
        {

        }
    }
}
