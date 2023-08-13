﻿using System.Text.Json.Serialization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Application.Model.DataTransferObject;
using Application.Shared.Kernel.Utility;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Data;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Web.AspNet.Startup;
using Application.Shared.Kernel.Infrastructure.Database.Mysql;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;


namespace Application.Shared.Kernel.Application.Model.Database.MySQL
{
    public class AbstractModel : IDisposable
    {
        #region Private
        private string _databaseName = null;
        private List<ClassRelationModel> _relations = null;
        private Guid _internalId = Guid.Empty;
        #endregion Private
        #region Public
        public Func<string, string> fGenerateTableNameFromNetClassName = new Func<string, string>((x) =>
        {
            string tabName = x.Replace("Model", "");
            List<int> upperCaseLettersIdx = new List<int>();

            string firstUpperCase = tabName.Substring(0, 1);
            string rest = tabName.Substring(1, tabName.Length - 1);
            tabName = firstUpperCase.ToLower() + rest;
            for (int i = 0; i < tabName.Length; i++)
            {
                if (char.IsUpper(tabName[i]))
                {
                    upperCaseLettersIdx.Add(i);
                }
            }
            for (int i = 0; i < upperCaseLettersIdx.Count; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(tabName);
                int underlineInsertIdx = upperCaseLettersIdx[i];
                if (underlineInsertIdx < tabName.Length)
                {
                    char myUpperChar = tabName[underlineInsertIdx + i];
                    stringBuilder = stringBuilder.Remove(underlineInsertIdx + i, 1);
                    stringBuilder = stringBuilder.Insert(underlineInsertIdx + i, ('_' + myUpperChar.ToString().ToLower()).ToString());

                    tabName = stringBuilder.ToString();
                }
            }
            return tabName;
        });
        public Func<string, string> fGenerateNetClassNameFromTableName = new Func<string, string>((x) =>
        {
            string netModelName = x.ToLower();

            Dictionary<int, char> underLineIdx = new Dictionary<int, char>();
            underLineIdx.Add(0, char.ToUpper(x[0]));
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == '_')
                {
                    underLineIdx.Add(i + 1, char.ToUpper(x[i + 1]));
                }

            }
            StringBuilder stringBuilder = new StringBuilder(netModelName);
            foreach (int key in underLineIdx.Keys)
            {
                stringBuilder[key] = underLineIdx[key];
            }
            stringBuilder = stringBuilder.Replace("_", "");


            return stringBuilder.ToString() + "Model";
        });
        /// <summary>
        /// Internal Identifier for General Seach in Cache, InternalIdenfier is generated by this.ClassName converted to Guid Hex
        /// </summary>
        [JsonIgnore]
        public Guid GeneralClassIdentifier
        {
            get
            {
                if (_internalId == Guid.Empty)
                {
                    string currentClassName = GetType().Name.ToLower();
                    _internalId = Utils.GenerateRequestGuid(currentClassName, Guid.Empty);
                }
                return _internalId;
            }
        }
        [JsonIgnore]
        public bool OnlyFieldValuesSetted
        {
            get
            {
                bool response = false;
                AbstractModel instance = Activator.CreateInstance<AbstractModel>();
                List<object> defaultValues = instance.KeyValues;
                List<object> thisCurrentValues = KeyValues;
                List<bool> hits = new List<bool>(defaultValues.Count);
                for (int i = 0; i < defaultValues.Count; i++)
                {
                    if (thisCurrentValues[i] == defaultValues[i])
                        hits[i] = true;
                }
                response = hits.FindAll(x => x == true).Count == defaultValues.Count ?
                    true : false;
                return response;
            }
        }
        [JsonIgnore]
        public List<object> KeyValues
        {
            get
            {
                List<object> values = new List<object>();

                return values;
            }
        }

        [JsonIgnore]
        public List<ClassRelationModel> DatabaseRelations
        {
            get
            {
                if (_relations == null)
                {
                    if (MySqlDefinitionProperties.BackendTablesEx.ContainsKey(DatabaseTable))
                    {
                        _relations = MySqlDefinitionProperties.BackendTablesEx.Keys.Count != 0 ? MySqlDefinitionProperties.BackendTablesEx[DatabaseTable].Relations?.FindAll(x => !x.Deleted) : null;
                    }

                }
                return _relations;
            }
        }
        [JsonIgnore]
        public string DatabaseTable
        {
            get
            {
                return _databaseName;
            }
        }
        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                return GetDisplayName(_databaseName);
            }
        }

        public ClassModelWrapper GetDatabaseClassModelWrapper(AbstractModel model = null)
        {
            string table = model == null ? DatabaseTable : model.DatabaseTable;
            if (MySqlDefinitionProperties.BackendTablesEx.ContainsKey(table))
            {
                return MySqlDefinitionProperties.BackendTablesEx[table];
            }
            return null;
        }
        #endregion Public

        [JsonPropertyName("uuid")]
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        virtual public Guid Uuid { get; set; } = Guid.Empty;

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("creation_datetime")]
        [DatabaseColumnProperty("creation_datetime", MySqlDbType.DateTime)]
        virtual public DateTime CreationDateTime { get; set; }

        [JsonPropertyName("active")]
        [DatabaseColumnProperty("active", MySqlDbType.Bit)]
        virtual public bool Active { get; set; } = false;

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("activation_datetime")]
        [DatabaseColumnProperty("activation_datetime", MySqlDbType.DateTime)]
        virtual public DateTime ActivationDateTime { get; set; }

        [JsonPropertyName("deleted")]
        [DatabaseColumnProperty("deleted", MySqlDbType.Bit, true)]
        virtual public bool Deleted { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("deletion_datetime")]
        [DatabaseColumnProperty("deletion_datetime", MySqlDbType.DateTime)]
        virtual public DateTime DeletionDateTime { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("changed_datetime")]
        [DatabaseColumnProperty("changed_datetime", MySqlDbType.DateTime)]
        virtual public DateTime ChangedDateTime { get; set; }

        #region Ctor & Dtor
        public AbstractModel()
        {

            _databaseName = fGenerateTableNameFromNetClassName(GetType().Name);
        }
        #endregion Ctor & Dtor
        #region Methods
        /// <summary>
        /// Mapped the data from the AbstractModel Type to a concrete Type of DataTransferModelAbstract
        /// The Propertynames must be equal to map the data correct from A to B
        /// </summary>
        /// <typeparam name="T">Concrete Implementation of DataTransferModelAbstract</typeparam>
        /// <returns></returns>
        public T GetMappedDataTransferModel<T>() where T : DataTransferModelAbstract
        {
            T mappedDataTransferModel = Activator.CreateInstance<T>();
            var dtoProperties = mappedDataTransferModel.GetType().GetProperties()?.ToList();
            var currentInstanceProperties = this.GetType().GetProperties()?.ToList();
            foreach (var prop in currentInstanceProperties)
            {
                PropertyInfo foundInDto = dtoProperties.Find(x => x.Name == prop.Name && x.PropertyType == prop.PropertyType);
                if (foundInDto == null)
                    continue;
                var val = prop.GetValue(this);
                foundInDto.SetValue(mappedDataTransferModel, val);
            }
            return mappedDataTransferModel;
        }
        public string GetDisplayName(string str)
        {
            return str.Replace("_", "");
        }
        public string ActionUri(string actionUri, string fileName)
        {
            return "/" + actionUri.Replace(BackendAPIDefinitionsProperties.ActionParameterIdWildcard, Uuid.ToString()).Replace(BackendAPIDefinitionsProperties.ActionParameterFileOptWildcard, fileName ?? "").ToLower();
        }
        public string GetJsonDisplayName(string classMemberName)
        {
            List<MemberInfo> members = GetType().GetMember(classMemberName).ToList();
            foreach (MemberInfo info in members)
            {
                JsonPropertyNameAttribute attr = GetMemberAttribute<JsonPropertyNameAttribute>(info);
                if (attr != null)
                {
                    return attr.Name;
                }
            }
            return null;
        }
        public PropertyInfo GetMemberByJsonPropertyName(string jsonNodeName)
        {
            List<PropertyInfo> members = GetType().GetProperties().ToList();
            foreach (PropertyInfo info in members)
            {
                JsonPropertyNameAttribute attr = info.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attr != null)
                {
                    if (attr.Name.ToLower().Equals(jsonNodeName.ToLower()))
                        return info;
                }
            }
            return null;
        }
        /// <summary>
        /// Get an attribute by given MemberInfo
        /// </summary>
        /// <typeparam name="T">Generic System.Attribute which should by find and extracted by given MemberInfo</typeparam>
        /// <param name="member">The MemberInfo of a specific class</param>
        /// <returns>Generic System.Attribute with all matching System.Attributes T by given MemberInfo member</returns>
        private T GetMemberAttribute<T>(MemberInfo member) where T : Attribute
        {
            T responseValue = null;
            List<T> attributes = GetMemberAttributes<T>(member);
            if (attributes.Count != 0)
            {
                Attribute attribute = attributes.First();
                if (attribute != null)
                {
                    T databaseColumn = (T)attribute;
                    responseValue = databaseColumn;
                }
            }
            return responseValue;
        }
        /// <summary>
        /// Get a list of attributes by given MemberInfo
        /// </summary>
        /// <typeparam name="T">Generic System.Attribute which should by find and extracted by given MemberInfo</typeparam>
        /// <param name="member">The MemberInfo of a specific class</param>
        /// <returns>List<System.Attribute> with all matching System.Attributes T by given MemberInfo member</returns>
        private List<T> GetMemberAttributes<T>(MemberInfo member) where T : Attribute
        {
            List<T> attributes = member.GetCustomAttributes(typeof(T)).ToList().OfType<T>().ToList();
            return attributes;
        }
        /// <summary>
        /// Extract all class members by given type which have the DatabaseColumnPropertyAttribute
        /// </summary>
        /// <param name="type">Type of AbstractModel.cs e.g. SoftwareModel</param>
        /// <returns>List<DatabaseColumnPropertyAttribute> of given Type type</returns>
        public List<DatabaseColumnPropertyAttribute> GetDatabaseColumns(Type type)
        {
            List<DatabaseColumnPropertyAttribute> responseValues = new List<DatabaseColumnPropertyAttribute>();
            List<PropertyInfo> parentModelProperties = type.GetProperties().ToList();
            for (int j = 0; j < parentModelProperties.Count; j++)
            {
                PropertyInfo item = parentModelProperties[j];
                List<DatabaseColumnPropertyAttribute> databaseColumnAttr = GetMemberAttributes<DatabaseColumnPropertyAttribute>(item);
                if (databaseColumnAttr != null)
                {
                    responseValues.AddRange(databaseColumnAttr);
                }
            }
            return responseValues;
        }
        /// <summary>
        /// Get all class members of current class instance with DatabaseColumnPropertyAttribute
        /// </summary>
        /// <returns>List<DatabaseColumnPropertyAttribute> of current class instance</returns>
        public List<DatabaseColumnPropertyAttribute> GetDatabaseColumns()
        {

            return GetDatabaseColumns(GetType());
        }
        /// <summary>
        /// Generate a query dynamic by given object
        /// </summary>
        /// <param name="typeOfStatement">One correct value of enum MySqlDefinitionProperties.SQL_STATEMENT_ART</param>
        /// <param name="customWhereClauseObject">The class instance with setted members for where clause</param>
        /// <param name="classInstance">The class instance for update or insert statement so set the values in query by class members</param>
        /// <returns>Valid SQL prepared for execution</returns>
        /// 
        public class QueryObject
        {
            public string SqlStatement { get; set; }
            public Dictionary<string, object> WhereClauseValues { get; set; } = null;

            public override string ToString()
            {
                return SqlStatement;
            }
        }
        public QueryObject GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART typeOfStatement, object customWhereClauseObject = null, object classInstance = null)
        {
            QueryObject responseObject = new QueryObject();
            responseObject.WhereClauseValues = new Dictionary<string, object>();
            string query = null;

            object currentClassInstance = classInstance == null ?
                this : classInstance;

            string tablename = ((AbstractModel)currentClassInstance).DatabaseTable;
            ClassModelWrapper classModelWrapper = MySqlDefinitionProperties.BackendTablesEx.ContainsKey(tablename) ? MySqlDefinitionProperties.BackendTablesEx[tablename] : null;
            if (classModelWrapper == null)
                throw new InvalidOperationException("classModelWrapper for table '" + tablename + "' is not existend, target class could be in wrong namespace (targetnamespace is defined in startup class via " + nameof(WebApiStartup) + ")");
            object instance = Activator.CreateInstance(currentClassInstance.GetType());

            List<PropertyInfo> defaultPropertiesWithValues = instance.GetType().GetProperties().ToList();
            List<PropertyInfo> thisPropertiesWithValues = currentClassInstance.GetType().GetProperties().ToList();

            List<PropertyInfo> custWhereClausePropertiesWithValues = customWhereClauseObject == null ?
                new List<PropertyInfo>() : customWhereClauseObject.GetType().GetProperties().ToList();

            Dictionary<PropertyInfo, DatabaseColumnPropertyAttribute> queryAttributes = new Dictionary<PropertyInfo, DatabaseColumnPropertyAttribute>();
            Dictionary<PropertyInfo, DatabaseColumnPropertyAttribute> queryCustWhereAttributes = new Dictionary<PropertyInfo, DatabaseColumnPropertyAttribute>();
            Dictionary<string, object> customWhereClauseValues = new Dictionary<string, object>();
            bool isCustWhereSetted = custWhereClausePropertiesWithValues.Count != 0 ? true : false;

            if (defaultPropertiesWithValues.Count == thisPropertiesWithValues.Count)
            {
                foreach (PropertyInfo propertyInfoDefault in defaultPropertiesWithValues)
                {
                    PropertyInfo thisCurrentProperty = thisPropertiesWithValues.Find(x => x.Name == propertyInfoDefault.Name);
                    if (thisCurrentProperty == null)
                    {
                        continue;
                    }
                    object defaultValue = propertyInfoDefault.GetValue(instance);
                    object thisCurrentPropertyValue = thisCurrentProperty.GetValue(currentClassInstance);
                    TypeConverter converter = null;
                    object convertedCurrentPropertyValue = thisCurrentPropertyValue;
                    Type defaultValueType = null;
                    if (defaultValue != null)
                    {
                        defaultValueType = defaultValue.GetType();
                        converter = TypeDescriptor.GetConverter(defaultValueType);
                        if (thisCurrentPropertyValue != null)
                        {
                            if (defaultValue.GetType() != thisCurrentPropertyValue.GetType())
                            {
                                convertedCurrentPropertyValue = converter.ConvertFrom(thisCurrentPropertyValue);
                            }
                        }
                    }
                    bool filledValue = defaultValue == null ? defaultValue != convertedCurrentPropertyValue : !defaultValue.Equals(convertedCurrentPropertyValue);
                    bool booleanValue = defaultValueType != null ? defaultValueType == typeof(bool) : false;
                    bool isUuidForInsert = !filledValue && thisCurrentProperty.Name.ToLower() == MySqlDefinitionProperties.GeneralUuidFieldName && typeOfStatement == MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT;
                    if (filledValue || booleanValue || isUuidForInsert)
                    {

                        DatabaseColumnPropertyAttribute databaseColumn = GetMemberAttribute<DatabaseColumnPropertyAttribute>(thisCurrentProperty);
                        if (databaseColumn != null)
                        {
                            if (filledValue || booleanValue && databaseColumn.MandantoryFieldInQueryGenerating || isUuidForInsert)
                            {
                                queryAttributes.Add(thisCurrentProperty, databaseColumn);
                            }
                        }
                    }
                    if (isCustWhereSetted)
                    {
                        PropertyInfo custWhereProperty = custWhereClausePropertiesWithValues.Find(x => x.Name == propertyInfoDefault.Name);
                        if (custWhereProperty != null)
                        {
                            object custWherePropertyValue = custWhereProperty.GetValue(customWhereClauseObject);

                            object convertedCustWherePropertyValue = custWherePropertyValue;



                            Type custWhereType = null;
                            if (custWherePropertyValue != null)
                            {
                                custWhereType = custWherePropertyValue.GetType();
                                if (defaultValue != null)
                                {

                                    if (custWhereType != defaultValue.GetType())
                                        convertedCustWherePropertyValue = converter.ConvertFrom(custWherePropertyValue);
                                }
                            }
                            filledValue = defaultValue == null ? defaultValue != convertedCustWherePropertyValue : !defaultValue.Equals(convertedCustWherePropertyValue);
                            booleanValue = custWhereType != null ? custWhereType == typeof(bool) : false;
                            if (filledValue || booleanValue)
                            {
                                DatabaseColumnPropertyAttribute databaseColumn = GetMemberAttribute<DatabaseColumnPropertyAttribute>(custWhereProperty);
                                if (databaseColumn != null)
                                {
                                    if (filledValue || booleanValue && databaseColumn.MandantoryFieldInQueryGenerating)
                                    {
                                        queryCustWhereAttributes.Add(custWhereProperty, databaseColumn);
                                        var attr = custWhereProperty.GetCustomAttribute<JsonPropertyNameAttribute>();
                                        if (attr != null)
                                            customWhereClauseValues.Add(attr.Name, thisCurrentPropertyValue);
                                    }
                                }
                            }
                        }
                    }
                }

                string fieldInsertList = null;
                string fieldInsertValues = null;
                string whereClauseList = null;
                string updateStmntList = null;
                int count = 0;
                int maxKeysCount = queryAttributes.Keys.Count - 1;
                List<DatabaseColumnPropertyAttribute> cols = new List<DatabaseColumnPropertyAttribute>();
                foreach (PropertyInfo propertyInfo in queryAttributes.Keys)
                {

                    object value = propertyInfo.GetValue(currentClassInstance);
                    DatabaseColumnPropertyAttribute databaseColumn = queryAttributes[propertyInfo];
                    cols.Add(databaseColumn);
                }
                bool anyKeyFilled = cols?.FindAll(x => classModelWrapper.IsKeyColumn(x)).Count != 0;
                foreach (DatabaseColumnPropertyAttribute databaseColumn in cols)
                {
                    fieldInsertList += databaseColumn.ColumnName;
                    fieldInsertValues += "@" + databaseColumn.ColumnName;
                    PropertyInfo prop = currentClassInstance.GetType().GetProperties().ToList().Find(x => x.GetCustomAttribute<DatabaseColumnPropertyAttribute>()?.ColumnName == databaseColumn.ColumnName);
                    var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                    object value = prop.GetValue(currentClassInstance);
                    updateStmntList += databaseColumn.ColumnName + " = " + "@" + databaseColumn.ColumnName;
                    if (!anyKeyFilled)
                    {

                        whereClauseList += databaseColumn.ColumnName + " = " + "@" + databaseColumn.ColumnName;
                        if (jsonAttr != null)
                            responseObject.WhereClauseValues.Add(jsonAttr.Name, value);
                        if (count < maxKeysCount)
                        {
                            whereClauseList += " AND ";
                        }
                    }
                    else
                    {
                        if (classModelWrapper.IsKeyColumn(databaseColumn))
                        {
                            whereClauseList += databaseColumn.ColumnName + " = " + "@" + databaseColumn.ColumnName;

                            if (jsonAttr != null)
                                responseObject.WhereClauseValues.Add(jsonAttr.Name, value);
                            if (count < maxKeysCount)
                            {
                                whereClauseList += " AND ";
                            }
                        }
                    }

                    if (count < maxKeysCount)
                    {
                        updateStmntList += ",";
                        fieldInsertList += ",";
                        fieldInsertValues += ",";
                    }
                    count++;
                }

                string custWhereClauseList = null;
                if (isCustWhereSetted)
                {
                    count = 0;
                    maxKeysCount = queryCustWhereAttributes.Keys.Count - 1;
                    foreach (PropertyInfo propertyInfo in queryCustWhereAttributes.Keys)
                    {
                        try
                        {

                            object value = propertyInfo.GetValue(customWhereClauseObject);
                            DatabaseColumnPropertyAttribute databaseColumnPropertyAttribute = propertyInfo.GetCustomAttribute<DatabaseColumnPropertyAttribute>();
                            string columnName = propertyInfo.Name;
                            if (databaseColumnPropertyAttribute != null)
                            {
                                columnName = databaseColumnPropertyAttribute.ColumnName;
                            }
                            custWhereClauseList += columnName + " = " + "@" + columnName;
                            if (count < maxKeysCount)
                            {
                                custWhereClauseList += " AND ";
                            }
                            count++;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }



                whereClauseList = isCustWhereSetted ?
                        custWhereClauseList : whereClauseList;

                string table = DatabaseTable;
                switch (typeOfStatement)
                {
                    case MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT:
                        query = "INSERT INTO " + table + " (" + fieldInsertList + ") VALUES (" + fieldInsertValues + ");";
                        break;
                    case MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE:
                        query = "UPDATE " + table + " SET " + updateStmntList + " WHERE " + whereClauseList + ";";
                        break;
                    case MySqlDefinitionProperties.SQL_STATEMENT_ART.DELETE:
                        query = "DELETE FROM " + table + " WHERE " + whereClauseList + ";";
                        break;
                    case MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT:
                        query = "SELECT * FROM " + table + "" + (whereClauseList == null ? string.Empty : " WHERE " + whereClauseList + "") + ";";
                        break;
                }

            }
            responseObject.SqlStatement = query;
            return responseObject;
        }

        public Dictionary<DatabaseColumnPropertyAttribute, DatabaseColumnPropertyAttribute> GetForeignKeyValuePairs(ClassModelWrapper parentTable, ClassModelWrapper foreignTable, ApiDataModel relationBefore)
        {
            Dictionary<DatabaseColumnPropertyAttribute, DatabaseColumnPropertyAttribute> responseValues = new Dictionary<DatabaseColumnPropertyAttribute, DatabaseColumnPropertyAttribute>();
            foreach (var prRel in parentTable.Relations)
            {
                if (prRel.EntityOne == foreignTable.ClassModel.TableName)
                {
                    string parentCol = prRel.EntityTwoKeyCol;
                    string fkCol = prRel.EntityOneKeyCol;

                    var fAttr = foreignTable.Columns_WithAttributation.Find(x => x.ColumnName == fkCol);
                    var pAttr = parentTable.Columns_WithAttributation.Find(x => x.ColumnName == parentCol);
                    responseValues.Add(pAttr, fAttr);
                }
                else if (prRel.EntityTwo == foreignTable.ClassModel.TableName)
                {
                    string fkCol = prRel.EntityOneKeyCol;
                    string parentCol = prRel.EntityTwoKeyCol;
                    var fAttr = foreignTable.Columns_WithAttributation.Find(x => x.ColumnName == fkCol);
                    var pAttr = parentTable.Columns_WithAttributation.Find(x => x.ColumnName == parentCol);
                    responseValues.Add(pAttr, fAttr);

                }
            }
            return responseValues;
        }
        /// <summary>
        /// Find class members of given object instance with given DatabaseColumnPropertyAttribute data
        /// </summary>
        /// <param name="instance">Class instance with DatabaseColumnPropertyAttribute Members</param>
        /// <param name="databaseColumn">DatabaseColumnPropertyAttribute which would to find</param>
        /// <returns>PropertyInfo of found member</returns>
        public PropertyInfo FindPropertyByColumnName(object instance, DatabaseColumnPropertyAttribute databaseColumn)
        {
            if (databaseColumn == null)
                return null;
            if (instance != null)
            {
                List<PropertyInfo> propertyInfos = instance.GetType().GetProperties().ToList();

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    DatabaseColumnPropertyAttribute attr = GetMemberAttribute<DatabaseColumnPropertyAttribute>(propertyInfo);
                    if (attr != null)
                    {
                        if (attr.ColumnName == databaseColumn.ColumnName)
                            return propertyInfo;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Get class member value of given object instance with given DatabaseColumnPropertyAttribute data
        /// </summary>
        /// <param name="instance">Class instance with DatabaseColumnPropertyAttribute Members</param>
        /// <param name="databaseColumn">DatabaseColumnPropertyAttribute which would to find</param>
        /// <returns>Value of found class member in type of object</returns>
        public object GetPropertyValueByColumnName(object instance, DatabaseColumnPropertyAttribute databaseColumn)
        {
            PropertyInfo propertyInfo = FindPropertyByColumnName(instance, databaseColumn);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(instance);
            }
            return null;
        }
        /// <summary>
        /// Sets a specific class member value of given object instance dependend on searching databaseColumn
        /// </summary>
        /// <param name="instance">Class instance that should include searched databaseColumn</param>
        /// <param name="databaseColumn">The DatabaseColumnPropertyAttribute of the class member that would be setted</param>
        /// <param name="setterValue">The object setterValue that should be setted</param>
        /// <returns>Returns the object instance with new setted value</returns>
        public object SetPropertyValueByColumnName(object instance, DatabaseColumnPropertyAttribute databaseColumn, object setterValue)
        {
            PropertyInfo propertyInfo = FindPropertyByColumnName(instance, databaseColumn);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(instance, setterValue);
            }
            return instance;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }


}