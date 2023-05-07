using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService
{
    public static class AppManager
    {
        /// <summary>
        /// Allgemeine Klasse zum Verwalten von der App
        /// </summary>
        public enum MESSAGE_LEVEL : int
        {
            LEVEL_INVALID = -1,
            LEVEL_NORMAL = 0,
            LEVEL_INFO = 1,
            LEVEL_NOTE = 2,
            LEVEL_WARNING = 3,
            LEVEL_CRITICAL = 4
        }

        private static List<ApiModel> _controller = new List<ApiModel>();
        private static ThreadManager _theadManager = null;
        private static TranslationManager _translationManager = new TranslationManager();

        static AppManager()
        {
        }

        public static List<ApiModel> Api
        {
            get
            {
                return _controller;
            }
        }
        public static List<SystemMessageUserModel> SystemUsedMediumAccess { get; set; }
        public static ThreadManager Th = ThreadManager;//alias
        public static ThreadManager ThreadManager
        {
            get
            {
                InitThreadManager();
                return _theadManager;
            }
        }
        private static void InitThreadManager()
        {
            if (_theadManager == null)
            {
                _theadManager = new ThreadManager();
            }
        }
        public static TranslationManager T = TranslationManager;//alias
        public static TranslationManager TranslationManager
        {
            get { return _translationManager; }
        }


        #region Methods
        public static void InitRegisterApi(List<ApiModel> endpoints)
        {
            if (_controller.Count == 0)
                _controller = endpoints;
        }
        #endregion Methods
    }
}
