﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WebApiFunction.Data.Web.MIME;

namespace WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer
{
    public class SoftwareVersionUploadFormData : GeneralMimeFileFormData
    {
        public new IFormFile File { get => FileSetup; set => FileSetup = value; }
        public new string FileName { get => FileNameSetup; set => FileNameSetup = value; }

        [Required()]
        public IFormFile FileSetup { get; set; }
        [Required()]
        public string FileNameSetup { get; set; }

        public SoftwareVersionUploadFormData() : base()
        {

        }
    }
}
