using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Infrastructure.Log
{
    public struct CustomLogEvents
    {
        public const int HttpResponse = 101;
        public const int HttpRequest = 100;
        #region CRUD
        public const int CreateItem = 1002;
        public const int UpdateItem = 1003;
        public const int ReadItem = 1001;
        public const int DeleteItem = 1004;
        #endregion
        #region General

        public const int GeneralInfo = 10000;
        #endregion General
        #region AntiVirus

        public const int AvCheckDetectVulnerarblity = 20000;
        public const int AvCheckMoveMilisciousFileToQuarantine = 20001;
        public const int AvCheckScan = 19999;
        public const int AvInit = 20003;
        public const int AvExit = 20004;
        public const int AvConnect = 20004;
        #endregion
        #region MailTicketSystem
        public const int TicketSystemInit = 30001;
        public const int TicketSystemExit = 30002;
        public const int TicketSystemGeneral = 30003;
        #endregion
    }
}
