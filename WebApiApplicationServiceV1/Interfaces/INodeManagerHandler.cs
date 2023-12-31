﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService
{
    public interface INodeManagerHandler
    {
        NodeModel NodeModel { get; }
        public void Register(Guid typeUuid);
        public void KeepAlive();
        public void Unregister();
        public NodeModel GetCurrentNodeData();
    }
}
