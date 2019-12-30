using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Logging;

namespace Juniper.HTTP.Server.Controllers
{
    public abstract class AbstractRouteHandler :
        IEquatable<AbstractRouteHandler>,
        IComparable,
        IComparable<AbstractRouteHandler>,
        ILoggingSource
    {
        private readonly string name;
        private readonly int priority;
        private readonly HttpMethods verb;
        private readonly bool canContinue;

        public event EventHandler<string> Info;
        public event EventHandler<string> Warning;
        public event EventHandler<Exception> Error;

        public AuthenticationSchemes Authentication { get; }

        public virtual bool CanContinue(HttpListenerRequest request)
        {
            return canContinue;
        }

        public HttpProtocols Protocol { get; }

        protected AbstractRouteHandler(
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
            return obj is AbstractRouteHandler other
                && Equals(other);
        }

        public bool Equals(AbstractRouteHandler other)
        {
            return CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return $"[{priority.ToString(CultureInfo.CurrentCulture)}] {name}";
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as AbstractRouteHandler);
        }

        public virtual int CompareTo(AbstractRouteHandler other)
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

        public static bool operator ==(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return left is null && right is null
                || left is object && left.CompareTo(right) == 0
                || right is object && right.CompareTo(left) == 0;
        }

        public static bool operator !=(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return !(left == right);
        }

        public static bool operator <(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return left is object && left.CompareTo(right) == -1
                || right is object && right.CompareTo(left) == 1;
        }

        public static bool operator >(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return left is object && left.CompareTo(right) == 1
                || right is object && right.CompareTo(left) == -1;
        }

        public static bool operator <=(AbstractRouteHandler left, AbstractRouteHandler right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(AbstractRouteHandler left, AbstractRouteHandler right)
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
            Error?.Invoke(this, exp);
        }
    }
}