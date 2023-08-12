global using System;

// Application.Shared.Kernel.Infrastructure
global using Application.Shared.Kernel.Infrastructure.Mail;
global using Application.Shared.Kernel.Infrastructure.Ampq;
global using Application.Shared.Kernel.Infrastructure.Database;
global using Application.Shared.Kernel.Infrastructure.Database.Dapper;
global using Application.Shared.Kernel.Infrastructure.Database.Dapper.Converter;
global using Application.Shared.Kernel.Infrastructure.Database.Mysql;
global using Application.Shared.Kernel.Infrastructure.LocalSystem.IO.File;
global using Application.Shared.Kernel.Infrastructure.Log;
global using Application.Shared.Kernel.Infrastructure.Metric.Influxdb;
global using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;




global using Application.Shared.Kernel.Application;
global using Application.Shared.Kernel.Application.Model;

global using Application.Shared.Kernel.Application.Model.Dapper;
global using Application.Shared.Kernel.Application.Model.Dapper.Mysql;
global using Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context;
global using Application.Shared.Kernel.Application.Model.Dapper.TypeMapper;

global using Application.Shared.Kernel.Application.Model.DataTransferObject;
global using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation;
global using Application.Shared.Kernel.Application.Model.DataTransferObject.Attribute;
global using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;

global using Application.Shared.Kernel.Application.Model.Database;
global using Application.Shared.Kernel.Application.Model.Database.MySQL;
global using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema;

global using Application.Shared.Kernel.Application.Model.Internal;

global using Application.Shared.Kernel.Data.Format.Json;
global using Application.Shared.Kernel.Data.Format.Converter;

global using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1; 



namespace Application.Shared.Kernel
{
    public class Main
    {
        public Main()
        {
            
        }
    }
}