using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Log
{
    public static class General
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

        public static MESSAGE_LEVEL LoggingLevel = MESSAGE_LEVEL.LEVEL_NORMAL;
    }
}
