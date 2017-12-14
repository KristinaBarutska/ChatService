using System;
using System.ServiceModel;
using System.Threading;
using Contracts;
using System.Collections.Generic;

namespace Services
{
    /// <summary>
    /// Implements the chat service interface.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChatService : IChatService
    {
        private readonly Dictionary<Guid, IChatServiceCallback> clients =
         new Dictionary<Guid, IChatServiceCallback>();

        #region IChatService

        Guid IChatService.Subscribe()
        {
            IChatServiceCallback callback =
                OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();

            Guid clientId = Guid.NewGuid();

            if (callback != null)
            {
                lock (clients)
                {
                    clients.Add(clientId, callback);
                }
            }

            return clientId;
        }

        void IChatService.Unsubscribe(Guid clientId)
        {
            lock (clients)
            {
                if (clients.ContainsKey(clientId))
                {
                    clients.Remove(clientId);
                }
            }
        }

        void IChatService.SendMessage(Guid clientId, string message)
        {
            BroadcastMessage(clientId, message);
        }

        #endregion

        /// <summary>
        /// Notifies the clients of messages.
        /// </summary>
        /// <param name="clientId">Identifies the client that sent the message.</param>
        /// <param name="message">The message to be sent to all connected clients.</param>
        private void BroadcastMessage(Guid clientId, string message)
        {
            // Call each client's callback method
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (clients)
                    {
                        List<Guid> disconnectedClientGuids = new List<Guid>();

                        foreach (KeyValuePair<Guid, IChatServiceCallback> client in clients)
                        {
                            try
                            {
                                client.Value.HandleMessage(message);
                            }
                            catch (Exception)
                            {                            
                            disconnectedClientGuids.Add(client.Key);
                            }
                        }

                        foreach (Guid clientGuid in disconnectedClientGuids)
                        {
                            clients.Remove(clientGuid);
                        }
                    }
                }
            );
        }
    }
}
