using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Juniper.Logging;

namespace Juniper.HTTP.Server.Controllers
{
    public abstract class AbstractResponse :
        IEquatable<AbstractResponse>,
        IComparable,
        IComparable<AbstractResponse>,
        ILoggingSource
    {
        public const AuthenticationSchemes AnyAuth = AuthenticationSchemes.Digest
            | AuthenticationSchemes.Negotiate
            | AuthenticationSchemes.Ntlm
            | AuthenticationSchemes.Basic
            | AuthenticationSchemes.Anonymous;

        private readonly string name;
        private readonly int priority;

        public event EventHandler<StringEventArgs> Info;
        public event EventHandler<StringEventArgs> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        public AuthenticationSchemes Authentication { get; protected set; } = AnyAuth;

        public HttpProtocols Protocol { get; protected set; } = HttpProtocols.Default;

        public HttpStatusCode ExpectedStatus { get; protected set; }

        public HttpMethods Verb { get; protected set; }

        public HttpServer Server { get; internal set; }

        protected AbstractResponse(int priority, string name = null)
        {
            this.priority = priority;
            this.name = name ?? GetType().Name;
        }

        public virtual bool IsMatch(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (ExpectedStatus == 0
                    || context.Response.StatusCode == (int)ExpectedStatus)
                && IsMatch(context.Request);
        }

        public virtual bool IsMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return Enum.TryParse<HttpProtocols>(request.Url.Scheme, true, out var protocol)
                && Enum.TryParse<HttpMethods>(request.HttpMethod, true, out var verb)
                && (Protocol & protocol) != 0
                && (Verb & verb) != 0;
        }

        public abstract Task InvokeAsync(HttpListenerContext context);

        public override bool Equals(object obj)
        {
            return obj is AbstractResponse other
                && Equals(other);
        }

        public bool Equals(AbstractResponse other)
        {
            return CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return $"[{priority.ToString(CultureInfo.CurrentCulture)}] {name}";
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as AbstractResponse);
        }

        public virtual int CompareTo(AbstractResponse other)
        {
            if (other is null)
            {
                return -1;
            }
            else
            {
                // smaller Priority numbers before larger Priority numbers (i.e. 0 being the "Highest Priority")
                return priority.CompareTo(other.priority);
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -40035775;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = (hashCode * -1521134295) + priority.GetHashCode();
            hashCode = (hashCode * -1521134295) + Verb.GetHashCode();
            hashCode = (hashCode * -1521134295) + Authentication.GetHashCode();
            hashCode = (hashCode * -1521134295) + ExpectedStatus.GetHashCode();
            hashCode = (hashCode * -1521134295) + Protocol.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(AbstractResponse left, AbstractResponse right)
        {
            return (left is null && right is null)
                || (left is object && left.CompareTo(right) == 0)
                || (right is object && right.CompareTo(left) == 0);
        }

        public static bool operator !=(AbstractResponse left, AbstractResponse right)
        {
            return !(left == right);
        }

        public static bool operator <(AbstractResponse left, AbstractResponse right)
        {
            return (left is object && left.CompareTo(right) == -1)
                || (right is object && right.CompareTo(left) == 1);
        }

        public static bool operator >(AbstractResponse left, AbstractResponse right)
        {
            return (left is object && left.CompareTo(right) == 1)
                || (right is object && right.CompareTo(left) == -1);
        }

        public static bool operator <=(AbstractResponse left, AbstractResponse right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(AbstractResponse left, AbstractResponse right)
        {
            return left > right || left == right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnInfo(string message)
        {
            Info?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnWarning(string message)
        {
            Warning?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}