using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Application.Shared.Kernel.Infrastructure.LocalSystem.IO.File
{

    public class FileLocationDescriptor
    {
        public string RootDirPath { get; set; }
        public string FileName { get; set; }
        public string FullPath { get => Path.Combine(RootDirPath ?? "", FileName ?? ""); }
        public bool IsFullPathExistend { get => System.IO.File.Exists(FullPath); }
        public bool IsRootDirPathExistend { get => RootDirPath != null && System.IO.File.Exists(RootDirPath); }
        public byte[] Content { get; set; }
    }
}
