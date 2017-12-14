using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Contracts
{
    /// <summary>
    /// Service contract containing chat-related operations.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {
        /// <summary>
        /// Subcribes a client for any message broadcast.
        /// </summary>
        /// <returns>An id that will identify a client.</returns>
        [OperationContract]
        Guid Subscribe();

        /// <summary>
        /// Unsubscribes a client from any message broadcast.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        [OperationContract(IsOneWay = true)]
        void Unsubscribe(Guid clientId);

        /// <summary>
        /// Keeps the connection between the client and server.
        /// Connection between a client and server has a time-out,
        /// so the client needs to call this before that happens
        /// to remain connected to the server.
        /// </summary>
        //[OperationContract(IsOneWay = true)]
        //void KeepConnection();

        /// <summary>
        /// Broadcasts a message to other connected clients.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="message">The message to be broadcasted.</param>
        [OperationContract]
        void SendMessage(Guid clientId, string message);
    }
    /// <summary>
    /// The callback contract to be implemented by the client
    /// application.
    /// </summary>
    public interface IChatServiceCallback
    {
        /// <summary>
        /// Implemented by the client so that the server may call
        /// this when it receives a message to be broadcasted.
        /// </summary>
        /// <param name="message">
        /// The message to broadcast.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void HandleMessage(string message);
    }
}
