using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Name { get; }

        public int Priority { get; }

        public HttpProtocols Protocol { get; }

        public HttpMethods Verb { get; }

        public IReadOnlyList<HttpStatusCode> ExpectedStatuses { get; }

        public AuthenticationSchemes Authentication { get; }

        public IReadOnlyList<MediaType> AcceptTypes { get; }

        internal HttpServer Server { get; set; }

        public event EventHandler<StringEventArgs> Info;
        public event EventHandler<StringEventArgs> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        protected AbstractResponse(
            int priority,
            HttpProtocols protocol,
            HttpMethods method,
            IReadOnlyList<HttpStatusCode> expectedStatuses,
            AuthenticationSchemes authScheme,
            IReadOnlyList<MediaType> acceptTypes,
            string name = null)
        {
            Name = name ?? GetType().FullName;
            Priority = priority;
            Protocol = protocol;
            Verb = method;
            ExpectedStatuses = expectedStatuses ?? throw new ArgumentNullException(nameof(expectedStatuses));
            Authentication = authScheme;
            AcceptTypes = acceptTypes;
        }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocol,
            HttpMethods method,
            HttpStatusCode expectedStatus,
            AuthenticationSchemes authScheme,
            IReadOnlyList<MediaType> acceptTypes,
            string name = null)
            : this(priority,
                  protocol,
                  method,
                  new[] { expectedStatus },
                  authScheme,
                  acceptTypes,
                  name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocol,
            HttpMethods method,
            AuthenticationSchemes authScheme,
            IReadOnlyList<MediaType> acceptTypes,
            string name = null)
            : this(priority,
                  protocol,
                  method,
                  Array.Empty<HttpStatusCode>(),
                  authScheme,
                  acceptTypes,
                  name)
        { }

        public virtual bool IsMatch(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (ExpectedStatuses.Count == 0
                    || ExpectedStatuses.Contains(context.Response.GetStatus()))
                && IsMatch(context.Request);
        }

        public virtual bool IsMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestedTypes = MediaType.ParseAll(request.AcceptTypes);

            return requestedTypes.Any(t1 => AcceptTypes.Any(t2 => t1 == t2))
                && Enum.TryParse<HttpProtocols>(request.Url.Scheme, true, out var protocol)
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
            return $"{Name}[{Priority,10}]: {Protocol} {Verb} {ExpectedStatuses} {Authentication}";
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
                return Priority.CompareTo(other.Priority);
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -40035775;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + Priority.GetHashCode();
            hashCode = (hashCode * -1521134295) + Verb.GetHashCode();
            hashCode = (hashCode * -1521134295) + Authentication.GetHashCode();
            hashCode = (hashCode * -1521134295) + ExpectedStatuses.GetHashCode();
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