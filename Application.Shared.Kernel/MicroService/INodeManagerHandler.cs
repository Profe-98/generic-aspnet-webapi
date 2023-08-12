using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.MicroService
{
    public interface INodeManagerHandler
    {
        NodeModel NodeModel { get; }
        public void Register();
        public void KeepAlive();
        public void Unregister();
        public NodeModel GetCurrentNodeData();
    }
}
