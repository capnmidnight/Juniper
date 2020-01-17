using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Logging;
using Juniper.Logic;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server.Controllers
{
    public abstract class AbstractResponse :
        IEquatable<AbstractResponse>,
        IComparable,
        IComparable<AbstractResponse>,
        IRequestHandler,
        ILoggingSource
    {
        public const AuthenticationSchemes AllAuthSchemes = AuthenticationSchemes.Digest
            | AuthenticationSchemes.Negotiate
            | AuthenticationSchemes.Ntlm
            | AuthenticationSchemes.Basic
            | AuthenticationSchemes.Anonymous;

        public const HttpProtocols AllProtocols = HttpProtocols.All;

        public const HttpMethods AllMethods = HttpMethods.All;

        public static readonly Regex AllRoutes = new Regex(".*", RegexOptions.Compiled);

        public static readonly IExpression<HttpStatusCode> DefaultCode = Expr(HttpStatusCode.Continue);

        public static readonly IExpression<HttpStatusCode> AllStatusCodes = Empty<HttpStatusCode>();

        public static readonly IExpression<MediaType> AnyMediaTypes = Empty<MediaType>();

        public static readonly IExpression<(string Key, string Value)> NoHeaders = Empty<(string Key, string Value)>();

        public string Name { get; }

        public Regex Pattern { get; }

        public string RegexSource { get; }

        public int ParameterCount { get; }

        public int Priority { get; }

        public HttpProtocols Protocols { get; }

        public HttpMethods Methods { get; }

        public AuthenticationSchemes Authentication { get; }

        public IExpression<HttpStatusCode> StatusCodes { get; }

        public IExpression<MediaType> Accept { get; }

        public IExpression<(string Key, string Value)> Headers { get; }

        internal HttpServer Server { get; set; }

        public event EventHandler<StringEventArgs> Info;
        public event EventHandler<StringEventArgs> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        protected AbstractResponse(RouteAttribute route, string name = null)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            Name = name ?? GetType().Name;

            Priority = route.Priority;
            Protocols = route.Protocols;
            Methods = route.Methods;
            StatusCodes = route.StatusCodes ?? DefaultCode;
            Authentication = route.Authentication;
            Accept = route.Accept;
            Headers = route.Headers;
            Pattern = route.Pattern;
            RegexSource = Pattern.ToString();
            ParameterCount = Pattern.GetGroupNumbers().Length;
        }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocols,
            HttpMethods methods,
            Regex pattern,
            AuthenticationSchemes authSchemes,
            IExpression<MediaType> acceptTypes,
            IExpression<HttpStatusCode> statusCodes,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocols,
                Methods = methods,
                StatusCodes = statusCodes ?? throw new ArgumentNullException(nameof(statusCodes)),
                Authentication = authSchemes,
                Accept = acceptTypes ?? throw new ArgumentNullException(nameof(acceptTypes)),
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocol,
            HttpMethods methods,
            Regex pattern,
            AuthenticationSchemes authSchemes,
            IExpression<MediaType> acceptTypes,
            HttpStatusCode statusCode,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocol,
                Methods = methods,
                StatusCodes = Expr(statusCode),
                Authentication = authSchemes,
                Accept = acceptTypes,
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocols,
            HttpMethods methods,
            Regex pattern,
            AuthenticationSchemes authSchemes,
            MediaType acceptType,
            IExpression<HttpStatusCode> statusCodes,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocols,
                Methods = methods,
                StatusCodes = statusCodes ?? throw new ArgumentNullException(nameof(statusCodes)),
                Authentication = authSchemes,
                Accept = Expr(acceptType ?? throw new ArgumentNullException(nameof(acceptType))),
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocols,
            HttpMethods methods,
            Regex pattern,
            AuthenticationSchemes authSchemes,
            MediaType acceptType,
            HttpStatusCode statusCode,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocols,
                Methods = methods,
                StatusCodes = Expr(statusCode),
                Authentication = authSchemes,
                Accept = Expr(acceptType ?? throw new ArgumentNullException(nameof(acceptType))),
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }
        protected AbstractResponse(
            int priority,
            HttpProtocols protocols,
            HttpMethods methods,
            string pattern,
            AuthenticationSchemes authSchemes,
            IExpression<MediaType> acceptTypes,
            IExpression<HttpStatusCode> statusCodes,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocols,
                Methods = methods,
                StatusCodes = statusCodes ?? throw new ArgumentNullException(nameof(statusCodes)),
                Authentication = authSchemes,
                Accept = acceptTypes ?? throw new ArgumentNullException(nameof(acceptTypes)),
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocol,
            HttpMethods methods,
            string pattern,
            AuthenticationSchemes authSchemes,
            IExpression<MediaType> acceptTypes,
            HttpStatusCode statusCode,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocol,
                Methods = methods,
                StatusCodes = Expr(statusCode),
                Authentication = authSchemes,
                Accept = acceptTypes,
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocols,
            HttpMethods methods,
            string pattern,
            AuthenticationSchemes authSchemes,
            MediaType acceptType,
            IExpression<HttpStatusCode> statusCodes,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocols,
                Methods = methods,
                StatusCodes = statusCodes ?? throw new ArgumentNullException(nameof(statusCodes)),
                Authentication = authSchemes,
                Accept = Expr(acceptType ?? throw new ArgumentNullException(nameof(acceptType))),
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        protected AbstractResponse(
            int priority,
            HttpProtocols protocols,
            HttpMethods methods,
            string pattern,
            AuthenticationSchemes authSchemes,
            MediaType acceptType,
            HttpStatusCode statusCode,
            IExpression<(string Key, string Value)> headers,
            string name = null)
            : this(new RouteAttribute(pattern)
            {
                Priority = priority,
                Protocols = protocols,
                Methods = methods,
                StatusCodes = Expr(statusCode),
                Authentication = authSchemes,
                Accept = Expr(acceptType ?? throw new ArgumentNullException(nameof(acceptType))),
                Headers = headers ?? throw new ArgumentNullException(nameof(headers))
            }, name)
        { }

        public bool IsProtocolMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var reqProtocol = request.Url.Scheme;
            var hasProtocol = Enum.TryParse<HttpProtocols>(reqProtocol, true, out var protocol);
            var isProtocolMatch = (Protocols & protocol) != 0;
            return hasProtocol && isProtocolMatch;
        }

        public bool IsMethodMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var reqMethod = request.HttpMethod;
            var hasMethod = Enum.TryParse<HttpMethods>(reqMethod, true, out var verb);
            var isMethodMatch = (Methods & verb) != 0;
            return hasMethod && isMethodMatch;
        }

        public bool IsPatternMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return Pattern.IsMatch(request.Url.PathAndQuery);
        }

        public bool IsAcceptable(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestedTypes = MediaType.ParseAll(request.AcceptTypes);
            return requestedTypes.Any(t1 => Accept.Evaluate(t2 => t1 == t2));
        }

        public bool IsStatusCodeMatch(HttpListenerResponse response)
        {
            var curStatus = response.GetStatus();
            return StatusCodes.Evaluate(status => status == curStatus);
        }

        public bool IsHeaderMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var headers = request.Headers;
            return Headers.Evaluate(h =>
                (h.Value is null && headers[h.Key] is null)
                || (h.Value is object && h.Value.Equals(
                        request.Headers[h.Key],
                        StringComparison.InvariantCultureIgnoreCase)));
        }

        public virtual bool IsContextMatch(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return IsRequestMatch(context.Request)
                && IsStatusCodeMatch(context.Response);
        }

        public virtual bool IsRequestMatch(HttpListenerRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return IsProtocolMatch(request)
                && IsMethodMatch(request)
                && IsPatternMatch(request);
        }

        public async Task ExecuteAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.Request;
            var response = context.Response;

            if (!IsMethodMatch(request))
            {
                response.SetStatus(HttpStatusCode.MethodNotAllowed);
            }
            else if (!IsAcceptable(request))
            {
                response.SetStatus(HttpStatusCode.NotAcceptable);
            }
            else if (!IsHeaderMatch(request))
            {
                response.SetStatus(HttpStatusCode.BadRequest);
            }
            else
            {
                await InvokeAsync(context)
                    .ConfigureAwait(false);
            }
        }

        protected abstract Task InvokeAsync(HttpListenerContext context);

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
            return $"{Name}[{Priority,10}]: {Protocols} {Methods} {StatusCodes} {Authentication}";
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as AbstractResponse);
        }

        public virtual int CompareTo(AbstractResponse other)
        {
            if (other is null)
            {
                // sort null values to the end of the collection.
                return -1;
            }
            else
            {
                // smaller Priority numbers before larger Priority numbers (i.e. int.MaxValue being the "Highest Priority")
                var priorityCompare = ((double)Priority - other.Priority).Clamp();
                var patternCompare = string.CompareOrdinal(RegexSource, other.RegexSource).Clamp();

                var protocolCompare = 0;
                if (this.HasHttps() && !other.HasHttps())
                {
                    protocolCompare = -1;
                }
                else if (other.HasHttps() && !this.HasHttps())
                {
                    protocolCompare = 1;
                }

                var comparison = priorityCompare;
                comparison = (comparison << 1) + patternCompare;
                comparison = (comparison << 1) + protocolCompare;

                return comparison.Clamp();
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -40035775;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Regex>.Default.GetHashCode(Pattern);
            hashCode = (hashCode * -1521134295) + Priority.GetHashCode();
            hashCode = (hashCode * -1521134295) + Protocols.GetHashCode();
            hashCode = (hashCode * -1521134295) + Methods.GetHashCode();
            hashCode = (hashCode * -1521134295) + StatusCodes.GetHashCode();
            hashCode = (hashCode * -1521134295) + Authentication.GetHashCode();
            hashCode = (hashCode * -1521134295) + Accept.GetHashCode();
            hashCode = (hashCode * -1521134295) + Headers.GetHashCode();
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