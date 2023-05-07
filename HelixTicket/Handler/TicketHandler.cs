using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelixTicket.InternalModels;
using WebApiFunction.Database;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Application.Model.Database.MySql.Helix;
using WebApiFunction.Application.Model.Database.MySql.Table;

namespace HelixTicket.Handler
{
    public class TicketHandler : IScopedTicketHandler, ISingletonTicketHandler
    {
        #region Public
        /// <summary>
        /// Example match for this expression = [#CONF-20210528-1#], Date Part of the Identifier is every time 8 characters long, days & month with a numeric length of one char will fill with leading zeros by database trigger,
        /// 
        /// </summary>
        public static readonly string TicketIdentifierRegExPattern = @"\[#+[CONF-]+([0-9]{8})+[-]+[0-9]+#\]";
        public static readonly string TicketIdentifierDatePartExtractRegExPattern = @"([0-9]{8})";
        public static readonly string TicketIdentifierIncrementCounterExtractRegExPattern = @"([0-9])#\]";
        #endregion
        #region Private
        private readonly ISingletonDatabaseHandler _databaseHandler;
        private readonly ISingletonEncryptionHandler _encryptionHandler;
        #endregion
        #region Ctor & Dtor
        public TicketHandler(ISingletonDatabaseHandler databaseHandler, ISingletonEncryptionHandler encryptionHandler)
        {
            _databaseHandler = databaseHandler;
            _encryptionHandler = encryptionHandler;
        }
        #endregion
        #region Methods
        public async Task<Dictionary<Guid, List<AccountModel>>> GetMessageAccounts(MessageModel messageModel)
        {
            Dictionary<Guid, List<AccountModel>> response = new Dictionary<Guid, List<AccountModel>>();


            return response;
        }
        public TicketIdentifierCheckResponse StringContainsTicketIdentifier(string input)
        {
           return new TicketIdentifierCheckResponse(input);
        }
        public async Task<MessageConversationModel> GetConversation(TicketIdentifierCheckResponse ticketIdentifier)
        {
            if(ticketIdentifier.IsValidTicketIdentifier)
            {
                var conf = new MessageConversationModel { DisplayConfIdentifier = ticketIdentifier.TicketIdentifierFully };
                //prüfen ob offene konversation unter dieser ticketnummer und ob der absender der email in der konversation enthalten ist, wenn nicht dann wird dieser abgewiesen, cc & bcc mit in konversation nehmen
                QueryResponseData<MessageConversationModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageConversationModel>("SELECT * FROM message_conversation WHERE display_conf_identifier = @display_conf_identifier;",
                    conf);

                if (queryResponseData.HasStorageData)
                {
                    return queryResponseData.FirstRow;
                }
            }
            return null;
        }

        public async Task<List<MessageQueueModel>> GetQueues()
        {
            QueryResponseData<MessageQueueModel> queryResponseData = await _databaseHandler.ExecuteQuery<MessageQueueModel>("SELECT * FROM message_queue;");
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.DataStorage;
            }
            return null;
        }
        public async Task<MessageQueueModel> GetQueue(Guid systemMessageUserUuid)
        {
            MessageQueueModel whereClause = new MessageQueueModel { 
            SystemMessageUserUuid = systemMessageUserUuid,
            Deleted = false};
            QueryResponseData<MessageQueueModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageQueueModel>("SELECT * FROM message_queue WHERE system_message_user_uuid = @system_message_user_uuid and deleted = @deleted;", whereClause);
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.FirstRow;
            }
            return null;
        }
        public async Task<MessageQueueModel> GetQueue(MessageQueueModel whereClause)
        {

            QueryResponseData<MessageQueueModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageQueueModel>("SELECT * FROM message_queue WHERE system_message_user_uuid = @system_message_user_uuid AND message_queue_uuid = @message_queue_uuid AND name = @name;", whereClause);
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.FirstRow;
            }
            return null;
        }
        public async Task<MessageQueueModel> GetQueue(Guid uuid, bool msgUser = false)//bool param wegen polymorphie
        {

            QueryResponseData<MessageQueueModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageQueueModel>("SELECT * FROM message_queue WHERE uuid=@uuid;", new MessageQueueModel { Uuid =uuid });
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.FirstRow;
            }
            return null;
        }
        public async Task<MessageQueueModel> GetQueueChilds(Guid uuidParent)
        {
            MessageQueueModel whereClause = new MessageQueueModel{
                MessageQueueUuid = uuidParent
            };
            QueryResponseData<MessageQueueModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageQueueModel>("SELECT * FROM message_queue WHERE message_queue_uuid = @message_queue_uuid;", whereClause);
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.FirstRow;
            }
            return null;
        }
        public async Task<AccountModel> GetAccount(string userNameOrAccountName,Guid communicationMediumUuid)
        {
            if (String.IsNullOrEmpty(userNameOrAccountName))
                return null;

            AccountModel whereClause = new AccountModel { 
                User = userNameOrAccountName, 
                CommunicationMediumUuid = communicationMediumUuid 
            };

            QueryResponseData<AccountModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<AccountModel>("SELECT * FROM account WHERE user = @user AND communication_medium_uuid = @communication_medium_uuid;", whereClause);
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.FirstRow;
            }
            return null;
        }
        public async Task<List<AccountModel>> GetAllConversationParticipants(MessageConversationModel messageConversationModel)
        {
            if (messageConversationModel == null)
                return null;

            QueryResponseData<AccountModel> data = await _databaseHandler.ExecuteQuery<AccountModel>("SELECT acc.* FROM message_conversation as conv inner join message as msg on(msg.message_conversation_uuid = conv.uuid) inner join message_relation_to_account as msgrltoacc on(msgrltoacc.message_uuid = msg.uuid) inner join account as acc on(acc.uuid = msgrltoacc.account_uuid) inner join message_sending_type as msgt on(msgt.uuid = msgrltoacc.message_sending_type_uuid) where conv.uuid = '"+ messageConversationModel .Uuid+ "' group by acc.uuid;");
            if(data.HasSuccess && data.HasStorageData)
            {
                return data.DataStorage;
            }
            return null;
        }
        public bool IsAccountPartOfList(AccountModel account,List<AccountModel> accountModels)
        {
            if(account != null && accountModels != null)
            {
                return accountModels.Find(x => (x.User != null && account.User != null) && x.User.ToLower() == account.User.ToLower()) != null;
            }
            return false;
        }
        public async Task<AccountModel> AddAccount(string user, Guid communicationMediumUuid)
        {
            AccountModel fieldValueList = new AccountModel
            {
                User = user,
                Active = true,
                Deleted = false,
                CommunicationMediumUuid = communicationMediumUuid//communication_medium_uuid
            };
            QueryResponseData<AccountModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<AccountModel>("INSERT INTO `account` (`uuid`,`user`,`communication_medium_uuid`,`active`) VALUES (@uuid,@user,@communication_medium_uuid,@active)", fieldValueList);
            if (queryResponseData.HasStorageData)
            {
                return queryResponseData.FirstRow;
            }
            //prüfen ob DataStorage AccountModel rows beinhaltet vom create
            return null;
        }
        public async Task<MessageAttachmentModel> GetMessageAttachment(MessageAttachmentModel attachmentModel, bool hashCalcInnerFunction = false)
        {
            if(attachmentModel != null)
            {

                if(hashCalcInnerFunction)
                {
                    string hash = await CalculateMessageAttachmentHash(attachmentModel);
                    if (hash == null)
                        return null;

                    attachmentModel.Hash = hash;
                }
                QueryResponseData<MessageAttachmentModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageAttachmentModel>("SELECT * FROM message_attachment WHERE hash = @hash;", attachmentModel);
                if (queryResponseData.HasStorageData)
                {
                    return queryResponseData.FirstRow;
                }
            }
            return null;
        }
        public async Task<string> CalculateMessageAttachmentHash(MessageAttachmentModel attachmentModel)
        {
            if (attachmentModel != null)
            {
                if (attachmentModel.FileExists)
                {
                    return await CalcFileHash(attachmentModel.Path);
                }
            }
            return null;
        }
        public async Task<string> CalculateMessageAttachmentHash(string path)
        {
            if(path != null)
            {
                return await CalcFileHash(path);
            }
            return null;
        }
        private async Task<string> CalcFileHash(string path)
        {
            string hash = null;
            using (System.IO.FileStream stream = System.IO.File.OpenRead(path))
            {
                hash = await _encryptionHandler.MD5Async(stream);
            }
            return hash;
        }
        public async Task<MessageRelationToAttachmentModel> CreateRelationBetweenMessageAndAttachment(MessageModel messageModel, MessageAttachmentModel attachmentModel)
        {
            if (messageModel != null && attachmentModel != null)
            {
                string query = "INSERT INTO `message_relation_to_attachment` (`uuid`,`message_uuid`,`message_attachment_uuid`,`file_extension`)VALUES (@uuid,@message_uuid,@message_attachment_uuid,@file_extension)";
                MessageRelationToAttachmentModel fieldValueList = new MessageRelationToAttachmentModel
                {
                    MessageUuid = messageModel.Uuid,//message_uuid
                    MessageAttachmentUuid = attachmentModel.Uuid,//message_attachment_uuid
                    FileExtension = attachmentModel.FileExtension
                };
                QueryResponseData<MessageRelationToAttachmentModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageRelationToAttachmentModel>(query, fieldValueList);
                //prüfen ob MessageRelationToAttachmentModel als response von query exec kommt
                if (queryResponseData.HasStorageData)
                {
                    return queryResponseData.FirstRow;
                }
                return null;
            }
            return null;
        }
        public struct MESSAGE_SENDING_TYPE
        {
            public static Guid To = new Guid("6e9e9021-bfa0-11eb-a11f-309c2364fdb6");
            public static Guid From = new Guid("6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6");
            public static Guid Cc = new Guid("6e9eaf47-bfa0-11eb-a11f-309c2364fdb6");
            public static Guid Bcc = new Guid("6e9ebb70-bfa0-11eb-a11f-309c2364fdb6");
        }
        public async Task<MessageRelationToAccountModel> CreateRelationBetweenMessageAndAccount(MessageModel messageModel, AccountModel accountModel, Guid type)
        {
            if (messageModel != null && accountModel != null)
            {
                string query = "INSERT INTO `message_relation_to_account` (`uuid`,`message_uuid`,`account_uuid`,`message_sending_type_uuid`)VALUES (@uuid,@message_uuid,@account_uuid,@message_sending_type_uuid)";
                MessageRelationToAccountModel fieldValueList = new MessageRelationToAccountModel
                {
                    MessageUuid = messageModel.Uuid,//message_uuid
                    AccountUuid = accountModel.Uuid,//account_uuid
                    MessageSendingTypeUuid = type//message_sending_type_uuid
                };
                QueryResponseData<MessageRelationToAccountModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageRelationToAccountModel>(query, fieldValueList);
                //prüfen ob MessageRelationToMessageAccountModel als response von query exec kommt
                if (queryResponseData.HasStorageData)
                {
                    return queryResponseData.FirstRow;
                }
                return null;
            }
            return null;
        }
        public async Task<MessageAttachmentModel> CreateMessageAttachment(MessageAttachmentModel attachmentModel)
        {
            if (attachmentModel != null)
            {
                string query = "INSERT INTO `message_attachment` (`uuid`,`first_source_message_uuid`,`account_uuid`,`file_extension`,`hash`,`checked_for_virus`,`checked_for_violence`,`checked_for_sexual_content`,`checked_for_sexual_content_meta`) VALUES (@uuid,@first_source_message_uuid,@account_uuid,@file_extension,@hash,@checked_for_virus,@checked_for_violence,@checked_for_sexual_content,@checked_for_sexual_content_meta);";
                
                QueryResponseData<MessageAttachmentModel> queryResponseData = await _databaseHandler.ExecuteQueryWithMap<MessageAttachmentModel>(query, attachmentModel);
                //prüfen ob MessageAttachmentModel als response von query exec kommt
                if (queryResponseData.HasStorageData)
                {
                    return queryResponseData.FirstRow;
                }
                return null;
            }
            return null;
        }
        public async Task<MessageModel> AddMessageToConversation(string subject,byte[] content,bool checkForSexualContent,bool checkForViolence,bool checkForVirus,string checkForSexualContentMetaData,MessageConversationModel messageConversationModel,List<MessageAttachmentModel> messageAttachments = null)
        {
            if (messageConversationModel == null)
                return null;

            MessageModel fieldValueList = new MessageModel
            {
                MessageConversationUuid = messageConversationModel.Uuid,//message_conversation_uuid
                Content = content,
                Subject = subject,
                CheckForSexualContent = checkForSexualContent,
                CheckForSexualContentMeta = checkForSexualContentMetaData,
                CheckForViolence = checkForViolence,
                CheckForVirus = checkForVirus,
                Hash = (subject+content)//MD5 wird in Query für die beiden Cols berechnet
            };
            QueryResponseData<MessageModel> responseMsg = await _databaseHandler.ExecuteQueryWithMap<MessageModel>("INSERT INTO `message` (`uuid`,`message_conversation_uuid`,`subject`,`content`,`checked_for_virus`,`checked_for_violence`,`checked_for_sexual_content`,`checked_for_sexual_content_meta`,`hash`) VALUES (@uuid,@message_conversation_uuid,@subject,@content,@checked_for_virus,@checked_for_violence,@checked_for_sexual_content,@checked_for_sexual_content_meta,MD5(@hash));", fieldValueList);
            if(responseMsg.HasStorageData)
            {
                MessageModel refMsgModel = responseMsg.FirstRow;
                //prüfen ob DataStorage MessageModel rows beinhaltet vom create
                if (messageAttachments != null && refMsgModel != null)
                {
                    foreach (MessageAttachmentModel attachmentModel in messageAttachments)
                    {
                        MessageAttachmentModel refModel = await GetMessageAttachment(attachmentModel);
                        if (refModel == null)
                        {
                            refModel = await CreateMessageAttachment(attachmentModel);

                        }
                        else
                        {
                            var refRelBetMsgAndAtt = await CreateRelationBetweenMessageAndAttachment(refMsgModel, refModel);

                        }
                    }
                }
                return refMsgModel;
            }
            
            return null;
        }
        public async Task<MessageModel> AddMessageToTicket(string subject, byte[] content, bool checkForSexualContent, bool checkForViolence, bool checkForVirus, string checkForSexualContentMetaData, MessageConversationModel messageConversationModel, List<MessageAttachmentModel> messageAttachments = null)
        {
            return await AddMessageToConversation(subject,content,checkForSexualContent,checkForViolence,checkForVirus,checkForSexualContentMetaData,messageConversationModel,messageAttachments);
        }

        public async Task<MessageConversationModel> CreateNewTicket(Guid messageConversationStateTypeUuid, Guid communicationMediumUuid, Guid queueUuid)
        {
            return await CreateConversation(messageConversationStateTypeUuid,communicationMediumUuid,queueUuid);
        }
        public async Task<MessageConversationModel> CreateConversation(Guid messageConversationStateTypeUuid, Guid communicationMediumUuid, Guid queueUuid)
        {
            if (messageConversationStateTypeUuid == Guid.Empty || communicationMediumUuid == Guid.Empty || queueUuid == Guid.Empty)
                return null;

            MessageConversationModel fieldValueList = new MessageConversationModel
            {
                MessageConversationStateTypeUuid = messageConversationStateTypeUuid,//message_conversation_state_type_uuid,
                CommunicationMediumUuid = communicationMediumUuid,//communication_medium_uuid
                MessageQueueUuid = queueUuid
            };
            QueryResponseData<MessageConversationModel> responseMsg = await _databaseHandler.ExecuteQueryWithMap<MessageConversationModel>("INSERT INTO `message_conversation` (`uuid`,`message_conversation_state_type_uuid`,`message_queue_uuid`,`communication_medium_uuid`) VALUES (@uuid,@message_conversation_state_type_uuid,@message_queue_uuid,@communication_medium_uuid);", fieldValueList);
            if (responseMsg.HasStorageData)
            {
                return responseMsg.FirstRow;
            }
            //prüfen ob DataStorage MessageModel rows beinhaltet vom create

            return null;
        }
        
        #endregion
    }

}
