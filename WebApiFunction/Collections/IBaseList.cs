using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Collections
{
    public abstract class IBaseList<T> : IList<T>
    {
        #region Private

        private IList<T> internalList = new List<T>();

        #endregion
        #region Public

        public T this[int i]
        {
            get
            {
                return internalList[i];
            }
            set
            {
                internalList[i] = value;
            }
        }
        public int Count
        {
            get
            {
                return internalList.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return internalList.IsReadOnly;
            }
        }
        #endregion
        #region Ctor & Dtor
        public IBaseList()
        {

        }
        ~IBaseList()
        {

        }
        #endregion
        #region Methods
        public void Add(T exception)
        {
            internalList.Add(exception);
        }
        public void Insert(int index, T exception)
        {
            internalList.Insert(index, exception);
        }
        public void RemoveAt(int index)
        {
            internalList.RemoveAt(index);
        }
        public bool Remove(T exception)
        {
            return internalList.Remove(exception);
        }
        public int IndexOf(T exception)
        {
            return GeneralDefs.NotFoundResponseValue;
        }
        public void Clear()
        {
            internalList.Clear();
        }
        public bool Contains(T exception)
        {
            return internalList.Contains(exception);
        }
        public void CopyTo(T[] exceptions, int index)
        {
            internalList.CopyTo(exceptions, index);
        }
        private IEnumerator<T> Enumerator()
        {
            return internalList.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }
        #endregion
        #region Events

        #endregion
        #region EventArgs
        #endregion
    }
    public static class IBaseListExtension
    {
        /// <summary>
        /// Partitioned a list into n-partitions dependend on size for each partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}
