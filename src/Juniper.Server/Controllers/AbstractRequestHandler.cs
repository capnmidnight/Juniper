using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Juniper.Logging;

namespace Juniper.HTTP.Server.Controllers
{
    public abstract class AbstractRequestHandler :
        IEquatable<AbstractRequestHandler>,
        IComparable,
        IComparable<AbstractRequestHandler>,
        ILoggingSource
    {
        private readonly string name;
        private readonly int priority;
        private readonly HttpMethods verb;
        private readonly bool canContinue;

        private HttpServer parent;

        public event EventHandler<string> Info;
        public event EventHandler<string> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        public AuthenticationSchemes Authentication { get; }

        public virtual bool CanContinue(HttpListenerRequest request)
        {
            return canContinue;
        }

        public HttpProtocols Protocol { get; }

        public virtual HttpServer Server
        {
            get { return parent; }
            set { parent = value; }
        }

        protected AbstractRequestHandler(
            string name = null,
            int priority = 0,
            HttpProtocols protocol = HttpProtocols.All,
            HttpMethods verb = HttpMethods.GET,
            bool canContinue = false,
            AuthenticationSchemes authentication = AuthenticationSchemes.Anonymous)
        {
            this.name = name ?? GetType().Name;
            this.priority = priority;
            this.verb = verb;
            Protocol = protocol;
            this.canContinue = canContinue;
            Authentication = authentication;
        }

        public virtual bool IsMatch(HttpListenerRequest request)
        {
            return Enum.TryParse<HttpProtocols>(request.Url.Scheme, true, out var protocol)
                && Enum.TryParse<HttpMethods>(request.HttpMethod, true, out var verb)
                && (Protocol & protocol) != 0
                && (this.verb & verb) != 0;
        }

        public abstract Task InvokeAsync(HttpListenerContext context);

        public override bool Equals(object obj)
        {
            return obj is AbstractRequestHandler other
                && Equals(other);
        }

        public bool Equals(AbstractRequestHandler other)
        {
            return CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return $"[{priority.ToString(CultureInfo.CurrentCulture)}] {name}";
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as AbstractRequestHandler);
        }

        public virtual int CompareTo(AbstractRequestHandler other)
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
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + priority.GetHashCode();
            hashCode = hashCode * -1521134295 + verb.GetHashCode();
            hashCode = hashCode * -1521134295 + Authentication.GetHashCode();
            hashCode = hashCode * -1521134295 + canContinue.GetHashCode();
            hashCode = hashCode * -1521134295 + Protocol.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(AbstractRequestHandler left, AbstractRequestHandler right)
        {
            return left is null && right is null
                || left is object && left.CompareTo(right) == 0
                || right is object && right.CompareTo(left) == 0;
        }

        public static bool operator !=(AbstractRequestHandler left, AbstractRequestHandler right)
        {
            return !(left == right);
        }

        public static bool operator <(AbstractRequestHandler left, AbstractRequestHandler right)
        {
            return left is object && left.CompareTo(right) == -1
                || right is object && right.CompareTo(left) == 1;
        }

        public static bool operator >(AbstractRequestHandler left, AbstractRequestHandler right)
        {
            return left is object && left.CompareTo(right) == 1
                || right is object && right.CompareTo(left) == -1;
        }

        public static bool operator <=(AbstractRequestHandler left, AbstractRequestHandler right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(AbstractRequestHandler left, AbstractRequestHandler right)
        {
            return left > right || left == right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnInfo(string message)
        {
            Info?.Invoke(this, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnWarning(string message)
        {
            Warning?.Invoke(this, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}