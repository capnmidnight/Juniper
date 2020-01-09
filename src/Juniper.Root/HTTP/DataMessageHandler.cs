using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

namespace Juniper.HTTP
{
    public class DataMessageHandler<ResultT, FactoryT>
        where FactoryT : class, IFactory<ResultT, MediaType.Application>
    {
        private readonly WebSocketConnection socket;
        private readonly string message;
        private readonly FactoryT factory;

        public event EventHandler<EventArgs<ResultT>> DataMessage;
        public event EventHandler<ErrorEventArgs> Error;

        public DataMessageHandler(WebSocketConnection socket, string message, FactoryT factory)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message), $"{nameof(message)} parameter must not be null or empty.");
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.socket = socket;
            this.message = message;
            this.factory = factory;

            socket.DataMessage += Socket_DataMessage;
            socket.Closed += Socket_Closed;
        }

        public Task SendAsync(ResultT value)
        {
            return factory.SerializeAsync(socket, value);
        }

        private void Socket_Closed(object sender, EventArgs e)
        {
            socket.DataMessage -= Socket_DataMessage;
            socket.Closed -= Socket_Closed;
        }

        private void Socket_DataMessage(object sender, DataMessageEventArgs e)
        {
            if (e.Value.Message == message)
            {
                try
                {
                    var value = factory.Deserialize(e.Value.Data);
                    OnDataMessage(value);
                }
                catch (Exception exp)
                {
                    OnError(exp);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnDataMessage(ResultT value)
        {
            DataMessage?.Invoke(this, new EventArgs<ResultT>(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnError(Exception exp)
        {
            Error?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}
