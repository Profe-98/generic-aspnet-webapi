using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.InternalModels;

namespace WebApiApplicationService.Handler
{

    public static class ExceptionHandler
    {
        private static Dictionary<int, ExceptionObject> exceptions = new Dictionary<int, ExceptionObject>();

        public static void ReportException(string exceptionEnvironment, ExceptionObject exceptionObject)
        {
            int nextKey = GetExceptionCount();
            nextKey++;

            exceptions.TryAdd(nextKey, exceptionObject);
        }
        public static void ReportException(string exceptionEnvironment, Exception exception, AppManager.MESSAGE_LEVEL exceptionLevel)
        {
            int nextKey = GetExceptionCount();
            nextKey++;

            Console.WriteLine(exception.Message);

            exceptions.TryAdd(nextKey, new ExceptionObject { ExceptionEnvironment = exceptionEnvironment, Exception = exception, ExceptionDateTime = DateTime.Now, ExceptionLevel = exceptionLevel });
        }

        public static ExceptionObject GetException(int key)
        {
            if (!exceptions.ContainsKey(key))
            {
                return null;
            }
            return exceptions[key];
        }

        private static int GetExceptionCount(AppManager.MESSAGE_LEVEL filterByExceptionLevel = AppManager.MESSAGE_LEVEL.LEVEL_INVALID)//invalid == alles
        {

            int currentCount = 0;
            if (filterByExceptionLevel != AppManager.MESSAGE_LEVEL.LEVEL_INVALID)
            {
                foreach (int key in exceptions.Keys)
                {
                    if (exceptions[key].ExceptionLevel == filterByExceptionLevel)
                    {
                        currentCount++;
                    }
                }
            }
            else
            {
                currentCount = exceptions.Keys.Count;
            }
            return currentCount;
        }
    }
}
