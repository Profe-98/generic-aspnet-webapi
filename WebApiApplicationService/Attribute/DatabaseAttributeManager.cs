using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using WebApiApplicationService.Models;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Attribute
{
    public class DatabaseAttributeManager : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        #region Enum
        
        #endregion Enum

        #region Ctor & Dtor
        public DatabaseAttributeManager()
        {
        }
        #endregion Ctor & Dtor
        #region Methods
        public void FetchObjectToCommandParameters<T>(T objectInstance,ref MySqlCommand command,SQLDefinitionProperties.SQL_STATEMENT_ART statementArt)
        {
            try
            {
                if(objectInstance != null)
                {
                    List<PropertyInfo> propertyInfos = objectInstance.GetType().GetProperties().ToList();
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {

                        List<System.Attribute> customAttributeData = propertyInfo.GetCustomAttributes(typeof(DatabaseColumnPropertyAttribute)).ToList();



                        if (customAttributeData != null)
                        {
                            if (customAttributeData.Count > 1)
                            {
                                bool wait = false;
                            }
                            if (customAttributeData.Count != 0)
                            {
                                object value = propertyInfo.GetValue(objectInstance);

                                switch (statementArt)
                                {
                                    case SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT:
                                        break;
                                    case SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE:
                                        break;
                                    case SQLDefinitionProperties.SQL_STATEMENT_ART.DELETE:

                                        break;
                                    default:
                                        break;
                                }
                                DatabaseColumnPropertyAttribute databaseColumn = (DatabaseColumnPropertyAttribute)customAttributeData[0];
                                MySqlParameter mySqlParameter = new MySqlParameter();

                                mySqlParameter.MySqlDbType = databaseColumn.DataType;
                                mySqlParameter.ParameterName = "@" + databaseColumn.ColumnName;
                                mySqlParameter.Value = value;
                                command.Parameters.Add(mySqlParameter);
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {

            }
            
        }
        /// <summary>
        /// Maps the DataTable Rows on generice class T, mapping of class members by DatabaseColumnPropertyAttribute Columnname with match of datatable column name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns>The Datatable Rows converted and filled to generice class T</returns>
        public List<object> MapValueFromDataTableToGenericList(DataTable dataTable,Type type)
        {
            List<object> responseValues = new List<object>();



            if (dataTable != null)
            {
                if (dataTable.Rows.Count != 0)
                {
                    object responseModel = Activator.CreateInstance(type);
                    List<PropertyInfo> propertyInfos = responseModel.GetType().GetProperties().ToList();
                    Dictionary<int, int> staticClassPropertyIndexes = GetPropertyFromClass(responseModel, dataTable.Columns);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        responseModel = Activator.CreateInstance(type);
                        foreach (int key in staticClassPropertyIndexes.Keys)
                        {
                            int colIdx = key;
                            int propertyIdx = staticClassPropertyIndexes[key];
                            object value = row.ItemArray[colIdx];
                            Type destinationType = propertyInfos[propertyIdx].PropertyType;
                            Type sourceType = value.GetType();
                            DatabaseColumnPropertyAttribute databaseColumnPropertyAttribute = propertyInfos[propertyIdx].GetCustomAttributes<DatabaseColumnPropertyAttribute>().FirstOrDefault();

                            object convertedValue = null;
                            if (destinationType == typeof(bool))
                            {
                                convertedValue = false;
                                try
                                {
                                    convertedValue = value == DBNull.Value ?
                                        false : Convert.ToBoolean(value);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            else if (destinationType == typeof(Guid))
                            {
                                try
                                {
                                    if (Guid.TryParse(value.ToString(), out Guid guid))
                                    {
                                        convertedValue = guid;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            else if (destinationType == typeof(DateTime) && destinationType != sourceType)
                            {
                                DateTime dateTime = DateTime.MinValue;
                                if (sourceType != typeof(DBNull))
                                {
                                    try
                                    {
                                        MySql.Data.Types.MySqlDateTime tmp = ((MySql.Data.Types.MySqlDateTime)value);
                                        dateTime = new DateTime(tmp.Year, tmp.Month, tmp.Day, tmp.Hour, tmp.Minute, tmp.Second, tmp.Millisecond);

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                convertedValue = dateTime;
                            }
                            else if (databaseColumnPropertyAttribute != null && sourceType == typeof(string) && databaseColumnPropertyAttribute.DataType == MySqlDbType.JSON)
                            {
                                using (JsonHandler jsonHandler = new JsonHandler())
                                {
                                    convertedValue = jsonHandler.JsonDeserialize(value as string, destinationType);
                                }
                            }
                            else if (sourceType != destinationType)
                            {
                                try
                                {
                                    if (sourceType != typeof(DBNull))
                                    {
                                        TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
                                        convertedValue = converter.ConvertTo(value, destinationType);
                                    }

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            else
                            {
                                convertedValue = value;
                            }
                            SetProperty(responseModel, propertyInfos[propertyIdx], convertedValue);
                        }
                        responseValues.Add(responseModel);
                    }
                }
            }

            return responseValues;
        }
        public List<T> MapValueFromDataTableToGenericList<T>(DataTable dataTable)
        {
            return MapValueFromDataTableToGenericList(dataTable, typeof(T)).OfType<T>().ToList();


        }


        /// <summary>
        /// Returns the Indexes of Propertymembers that are matching with the DataColumn ColumnName of given DataColumnCollection
        /// </summary>
        /// <param name="parentModel"></param>
        /// <param name="dataColumnCollection"></param>
        /// <returns>Dictionary<columnindex,propertymemberindex> with the indexes of property members of parentModel</returns>
        /// 
        public Dictionary<int,int> GetPropertyFromClass(object classInstance,DataColumnCollection dataColumnCollection)
        {
            Dictionary<int,int> responseValues = new Dictionary<int,int>();
            List<PropertyInfo> parentModelProperties = classInstance.GetType().GetProperties().ToList();
            try
            {
                if (dataColumnCollection != null && parentModelProperties.Count != 0)
                {
                    for (int i = 0; i < dataColumnCollection.Count; i++)
                    {
                        DataColumn column = dataColumnCollection[i];

                        int index = GeneralDefs.NotFoundResponseValue;
                        for (int j = 0; j < parentModelProperties.Count; j++)
                        {
                            PropertyInfo item = parentModelProperties[j];
                            List<System.Attribute> attributes  = item.GetCustomAttributes(typeof(DatabaseColumnPropertyAttribute)).ToList();
                            if(attributes.Count != 0)
                            {
                                System.Attribute attribute = attributes.First();
                                if (attribute != null)
                                {
                                    DatabaseColumnPropertyAttribute databaseColumn = (DatabaseColumnPropertyAttribute)attribute;
                                    if (databaseColumn.ColumnName == column.ColumnName)
                                    {
                                        index = j;
                                        break;
                                    }
                                }
                            }
                        }
                        if (index != GeneralDefs.NotFoundResponseValue)
                        {
                            if (!responseValues.ContainsKey(i))
                                responseValues.Add(i, index);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
            return responseValues;
        }

        public object GetProperty(PropertyInfo propertyInfo, object parentModel)
        {
            return propertyInfo.GetValue(parentModel);
        }
        public void SetProperty(object parentModel,PropertyInfo propertyInfo,object value)
        {
            propertyInfo.SetValue(parentModel,value);
        }

        #endregion Methods
    }
}
