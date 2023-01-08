using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApiFunction.Data.Web.MIME
{
    public abstract class GeneralMimeFileFormData
    {

        [Required()]
        virtual public IFormFile File { get; set; }
        [Required()]
        virtual public string FileName { get; set; }

        public Stream GetStream(IFormFile formFile) => formFile.OpenReadStream();
        public byte[] ReadIFormFile(IFormFile formFile)
        {
            Stream stream = GetStream(formFile);
            BinaryReader binaryReader = new BinaryReader(stream);
            byte[] buffer = binaryReader.ReadBytes((int)stream.Length);
            return buffer;
        }

    }
}
