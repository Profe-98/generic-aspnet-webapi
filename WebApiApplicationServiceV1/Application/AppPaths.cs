using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace WebApiApplicationService
{
    public static class AppPaths
    {
        #region FolderPaths
        private static string _rootDir = ".";
        public static string RootDir
        {
            get
            {
                return _rootDir;
            }
            set
            {
                _rootDir = value;
            }
        }

        public static readonly string LogFolderPath = Path.Combine(RootDir, AppResources.LogFolderName);

        public static readonly string AvQuarantinePath = Path.Combine(RootDir, AppResources.AvQuarantineFolderName);

        public static readonly string UserProfilePicturesPath = Path.Combine(RootDir, AppResources.FileServingFolderName,AppResources.UserFolderName, AppResources.UserProfileFolderName);


        #endregion FolderPaths

        #region FilePaths

        public static readonly string LogFilePath = LogFolderPath;


        #endregion FilePaths

    }
}
