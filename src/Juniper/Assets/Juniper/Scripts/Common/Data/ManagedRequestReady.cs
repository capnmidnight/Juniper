using System;

namespace Juniper.Data
{
    /// <summary>
    /// Event arguments for when the request poller is ready to perform a request.
    /// </summary>
    public class ManagedRequestReadyEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Create a new message from a response processor to send to a requester that the response
        /// processor is ready to start receiving responses.
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public ManagedRequestReadyEventArgs(Action<T> onSuccess, Action<Exception> onError)
        {
            OnSuccess = onSuccess;
            OnError = onError;
        }

        /// <summary>
        /// The callback to use to pass to an asynchronous requester
        /// </summary>
        public Action<T> OnSuccess
        {
            get; private set;
        }

        /// <summary>
        /// The error handling callback to use to pass to an asynchronous requester
        /// </summary>
        public Action<Exception> OnError
        {
            get; private set;
        }
    }
}
