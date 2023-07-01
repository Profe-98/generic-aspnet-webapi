﻿using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.DataTransferObject.Jellyfish;
using WebApiFunction.Web.Websocket.SignalR.HubService;

namespace WebApiFunction.Application.WebSocket.SignalR.JellyFish
{
    /// <summary>
    /// Defines Methods that should invoked to client
    /// Direction is 'from server/hub to client'
    /// The advantage of this strongly typed client method defines is: you cant missspell them 
    /// The client muss implement these functions to communicate with the backend without issues
    /// </summary>
    public interface IMessengerClient : IStronglyTypedSignalRClient
    {
        public Task ReceiveMessage(List<MessageDTO> messages);
        public Task ReceiveFriendshipRequest(UserFriendshipRequestDTO request);
        public Task AcceptFriendshipRequest(UserDTO userDTO);
    }
}
