using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections;
using WebApiApplicationService.Handler;
using System.Text;
using System.Net;

namespace WebApiApplicationService
{
    public static class Utils
    {
        public static IPAddress ParseEx(string ipOrHostname)
        {
            if (!IPAddress.TryParse(ipOrHostname, out IPAddress ip))
            {
                ip = Dns.GetHostEntry(ipOrHostname).AddressList.FirstOrDefault(IPAddress.Loopback);
            }
            return ip;
        }
        public static Guid GenerateRequestGuid(string controllerName, Guid requestId)
        {
            Guid response = Guid.Empty;
            string name = controllerName + (requestId == Guid.Empty ? "" : requestId);
            using (EncryptionHandler encryp = new EncryptionHandler())
            {
                string md5 = encryp.MD5(name);
                byte[] bytes = Encoding.Default.GetBytes(md5);
                response = new Guid(md5);
            }
            return response;
        }
        public static bool IsList(object data)
        {

            bool genericType = false;
            if (data == null)
                return genericType;
            Type t = data.GetType();
            if (t.IsGenericType)
            {
                Type tmp = t.GetGenericTypeDefinition();
                genericType = tmp.Equals(typeof(List<>));
            }
            return data is IList && data.GetType().IsGenericType ||
                data is IList ||
                (data is IList && genericType);
        }
        public static bool IsList<T>(object data)
        {
            return data is IList<T> || data is IList;
        }
        /// <summary>
        /// Example Code: 
        /*
         * Func<MimeKit.InternetAddress, System.Threading.Tasks.Task<AccountModel>> fAcc = (x) => Utils.CallAsyncFunc<MimeKit.InternetAddress, AccountModel>(x, async (x) =>
                                                {

                                                    string userName = String.IsNullOrEmpty(x.Name) ? x.ToString() : x.Name;
                                                    _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: try to find account: " + userName + "", MethodBase.GetCurrentMethod(), this.GetType().Name, WebApiApplicationService.Logging.CustomLogEvents.TicketSystemGeneral);
                                                    var accountModel = await _ticketHandler.GetAccount(userName, communicationMediumUuid);
                                                    if (accountModel == null && !String.IsNullOrEmpty(userName))
                                                    {
                                                        _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: account could not be found, try to add", MethodBase.GetCurrentMethod(), this.GetType().Name, WebApiApplicationService.Logging.CustomLogEvents.TicketSystemGeneral);

                                                        accountModel = await _ticketHandler.AddAccount(userName, communicationMediumUuid);
                                                        if (accountModel == null)
                                                        {
                                                            _logger.Logging(LogLevel.Error, "\t\t\t[database:uid:" + key + ":error]: account could not be added", MethodBase.GetCurrentMethod(), this.GetType().Name, WebApiApplicationService.Logging.CustomLogEvents.TicketSystemGeneral);

                                                        }
                                                    }
                                                    return accountModel;
                                                });
         */
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="input"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<TOut> CallAsyncFunc<TIn, TOut>(TIn input, Func<TIn, Task<TOut>> func)
        {
            return await func.Invoke(input);
        }
        public static async Task<TOut> CallAsyncFunc<TIn, TIn2, TOut>(TIn input1, TIn2 input2, Func<TIn, TIn2, Task<TOut>> func)
        {
            return await func.Invoke(input1,input2);
        }
    }
}
