using System;

namespace Juniper.Data
{
    /// <summary>
    /// A ManagedRequest is a <see cref="AbstractBackgroundRepeatingThread"/> that keeps a
    /// parameterized type value as its work result.
    /// </summary>
    /// <typeparam name="T">The type of the work result.</typeparam>
    public class ManagedRequest<T> : AbstractBackgroundRepeatingThread
    {
        /// <summary>
        /// The default number of seconds between data polls
        /// </summary>
        public const float DEFAULT_POLL_RATE = 30;

        /// <summary>
        /// The default number of seconds to wait on an HTTP request to consider it timed out
        /// </summary>
        public const float DEFAULT_TIMEOUT = 3;

        /// <summary>
        /// Create an object that runs an HTTP request process in a background thread on a regular
        /// interval, firing events on error or success.
        /// </summary>
        /// <param name="pollRate">The number of seconds between data polls. Defaults to <see cref="DEFAULT_POLL_RATE"/>.</param>
        /// <param name="timeout">
        /// The number of seconds to wait on an HTTP request to consider it timed out. Defaults to
        /// <see cref="DEFAULT_TIMEOUT"/>.
        /// </param>
        public ManagedRequest(float pollRate = DEFAULT_POLL_RATE, float timeout = DEFAULT_TIMEOUT)
        {
            PollRate = pollRate;
            TimeoutRate = timeout;
            NextRequest = DateTime.Now.AddSeconds(pollRate / 4);
        }

        #region Events

        /// <summary>
        /// Capture when the request poller is ready.
        /// </summary>
        public event EventHandler<ManagedRequestReadyEventArgs<T>> Ready;

        /// <summary>
        /// Capture when the request poller returns a good result.
        /// </summary>
        public event EventHandler<ManagedRequestMessageArgs<T>> Message;

        /// <summary>
        /// Capture when the request poller returns an error result.
        /// </summary>
        public event EventHandler<BackgroundThreadErrorEventArgs> ServerError;

        /// <summary>
        /// Capture when the request poller times out.
        /// </summary>
        public event EventHandler TimedOut;

        /// <summary>
        /// Execute the Ready event, if it has listeners attached to it.
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        private void OnReady(Action<T> onSuccess, Action<Exception> onError) =>
            Ready?.Invoke(this, new ManagedRequestReadyEventArgs<T>(onSuccess, onError));

        /// <summary>
        /// Execute the Message event, if it has listeners attached to it.
        /// </summary>
        /// <param name="args"></param>
        private void OnMessage(T args)
        {
            if (Running)
            {
                Message?.Invoke(this, new ManagedRequestMessageArgs<T>(args));
            }
        }

        /// <summary>
        /// Execute the ServerError event, if it has listeners attached to it.
        /// </summary>
        /// <param name="exp"></param>
        private void OnServerError(Exception exp)
        {
            if (Running)
            {
                ServerError?.Invoke(this, new BackgroundThreadErrorEventArgs(exp));
            }
        }

        /// <summary>
        /// Execute the TimedOut event, if it has listeners attached to it.
        /// </summary>
        private void OnTimedOut()
        {
            if (Running)
            {
                TimedOut?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Events

        /// <summary>
        /// The timestamp at which the next request will be performed.
        /// </summary>
        public DateTime NextRequest { get; private set; }

        /// <summary>
        /// The timestamp (if any) at which the most recent request was performed.
        /// </summary>
        public DateTime? LastRequest { get; private set; }

        /// <summary>
        /// The timestamp (if any) at which the most recent response was received.
        /// </summary>
        public DateTime? LastResponse { get; private set; }

        /// <summary>
        /// The timestamp (if any) at which the most recent error was received.
        /// </summary>
        public Exception LastError { get; private set; }

        /// <summary>
        /// The current rate at which requests are considered timed out.
        /// </summary>
        public float TimeoutRate { get; set; }

        /// <summary>
        /// The current rate at which requests are issued.
        /// </summary>
        public float PollRate { get; set; }

        /// <summary>
        /// The that that has passed since the last request (if any) was performed.
        /// </summary>
        public TimeSpan? SinceRequest =>
            DateTime.Now - LastRequest;

        /// <summary>
        /// The number of seconds that have passed since the last request (if any) was performed.
        /// </summary>
        public float? SinceRequestSeconds
        {
            get
            {
                if (SinceRequest == null)
                {
                    return null;
                }
                else
                {
                    return (float)SinceRequest.Value.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// The amount of time until the next request is performed.
        /// </summary>
        public TimeSpan UntilNextRequest =>
            NextRequest - DateTime.Now;

        /// <summary>
        /// The number of seconds until the next request is performed.
        /// </summary>
        public float UntilNextRequestSeconds =>
            (float)UntilNextRequest.TotalSeconds;

        /// <summary>
        /// Returns true if there has been an error or timeout since the last request, before a
        /// response could occur.
        /// </summary>
        public bool Errored =>
            LastError != null || IsTimedOut;

        /// <summary>
        /// The amount of time since the last response (if any).
        /// </summary>
        public TimeSpan? SinceResponse =>
            DateTime.Now - LastResponse;

        /// <summary>
        /// The number of seconds since the last response (if any).
        /// </summary>
        public float? SinceResponseSeconds
        {
            get
            {
                if (SinceResponse == null)
                {
                    return null;
                }
                else
                {
                    return (float)SinceResponse.Value.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// Returns true if there has been no response in a set amount of time since the last request.
        /// </summary>
        public bool IsTimedOut =>
            SinceResponse == null && SinceRequestSeconds > TimeoutRate;

        /// <summary>
        /// The amount of time that passed between the last request (if any) and last response (if any)
        /// </summary>
        public TimeSpan? ResponseTime =>
            LastResponse - LastRequest;

        /// <summary>
        /// The number of seconds that passed between the last request (if any) and last response (if any)
        /// </summary>
        public float? ResponseTimeSeconds =>
            (float?)ResponseTime?.TotalSeconds;

        /// <summary>
        /// Clear out all errors and schedule a new request.
        /// </summary>
        /// <param name="seconds">The number of seconds from now at which to start the next request.</param>
        public void RetryIn(float seconds)
        {
            LastRequest = null;
            NextRequest = DateTime.Now.AddSeconds(seconds);
            LastError = null;
            LastResponse = null;
        }

        /// <summary>
        /// If we aren't waiting on a response to a previous request, wait until the request time is
        /// nigh, then issue the request. If we are waiting on a response to a previous request, then
        /// check how long it has taken, and issue the TimeOut event if it has taken too long.
        /// </summary>
        protected override void Loop()
        {
            if (UntilNextRequestSeconds <= 0)
            {
                var now = DateTime.Now;
                LastRequest = now;
                NextRequest = now.AddSeconds(PollRate);
                LastResponse = null;
                OnReady(OnSuccess, OnServerError);
            }

            if (IsTimedOut && !wasTimedOut)
            {
                OnTimedOut();
            }
            wasTimedOut = IsTimedOut;
        }

        /// <summary>
        /// Whether or not the thread had timed out waiting to a response from its last request.
        /// </summary>
        private bool wasTimedOut;

        /// <summary>
        /// When the request is successful, issue the Message event. <see cref="OnMessage(T)"/>
        /// </summary>
        /// <param name="obj"></param>
        private void OnSuccess(T obj)
        {
            LastResponse = DateTime.Now;
            OnMessage(obj);
        }
    }
}
