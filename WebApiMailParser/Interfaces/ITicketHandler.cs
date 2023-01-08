using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Application.Model.Database.MySql.Table;
using WebApiMailParser.InternalModels;

namespace WebApiMailParser
{
    public interface ITicketHandler
    {
        public Task<Dictionary<Guid, List<AccountModel>>> GetMessageAccounts(MessageModel messageModel); 
        public Task<List<MessageQueueModel>> GetQueues();
        public Task<MessageConversationModel> CreateConversation(Guid messageConversationStateTypeUuid, Guid communicationMediumUuid,Guid queueUuid);
        public Task<MessageConversationModel> CreateNewTicket(Guid messageConversationStateTypeUuid, Guid communicationMediumUuid,Guid queueUuid);
        public Task<MessageModel> AddMessageToTicket(string subject, byte[] content, bool checkForSexualContent, bool checkForViolence, bool checkForVirus, string checkForSexualContentMetaData, MessageConversationModel messageConversationModel, List<MessageAttachmentModel> messageAttachments = null);
        public Task<MessageModel> AddMessageToConversation(string subject, byte[] content, bool checkForSexualContent, bool checkForViolence, bool checkForVirus, string checkForSexualContentMetaData, MessageConversationModel messageConversationModel, List<MessageAttachmentModel> messageAttachments = null);
        public Task<MessageAttachmentModel> CreateMessageAttachment(MessageAttachmentModel attachmentModel);
        public Task<MessageRelationToAccountModel> CreateRelationBetweenMessageAndAccount(MessageModel messageModel, AccountModel accountModel, Guid type);
        public Task<MessageRelationToAttachmentModel> CreateRelationBetweenMessageAndAttachment(MessageModel messageModel, MessageAttachmentModel attachmentModel);
        public Task<string> CalculateMessageAttachmentHash(MessageAttachmentModel attachmentModel);
        public Task<MessageAttachmentModel> GetMessageAttachment(MessageAttachmentModel attachmentModel, bool hashCalcInnerFunction = false);
        public Task<AccountModel> AddAccount(string user, Guid communicationMediumUuid);
        public bool IsAccountPartOfList(AccountModel account, List<AccountModel> accountModels);
        public Task<List<AccountModel>> GetAllConversationParticipants(MessageConversationModel messageConversationModel);
        public Task<AccountModel> GetAccount(string userNameOrAccountName, Guid communicationMediumUuid);
        public Task<MessageConversationModel> GetConversation(TicketIdentifierCheckResponse ticketIdentifier);
        public TicketIdentifierCheckResponse StringContainsTicketIdentifier(string input);
        public Task<string> CalculateMessageAttachmentHash(string path);
        public Task<MessageQueueModel> GetQueue(MessageQueueModel whereClause);
        public Task<MessageQueueModel> GetQueue(Guid systemMessageUserUuid);
        public Task<MessageQueueModel> GetQueue(Guid uuid,bool poly);
        public Task<MessageQueueModel> GetQueueChilds(Guid parentUuid);
        
    }

    public interface IScopedTicketHandler : ITicketHandler
    {

    }
    public interface ISingletonTicketHandler : ITicketHandler
    {

    }
}
