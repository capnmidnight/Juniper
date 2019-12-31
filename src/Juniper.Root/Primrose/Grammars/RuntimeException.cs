using System;
using System.Linq;
using System.Runtime.Serialization;

using Line = System.Collections.Generic.List<Juniper.Primrose.Token>;

namespace Juniper.Primrose
{
    [Serializable]
    public sealed class RuntimeException : Exception
    {
        public readonly string source;

        public readonly string evaluatedScript;

        public readonly Token[] line;

        public RuntimeException(string source, Line line, string evaluatedScript, Exception innerException)
            : base("Runtime Error", innerException)
        {
            this.source = source;
            this.evaluatedScript = evaluatedScript;
            this.line = (from t in line
                         select t.Clone())
                    .ToArray();
        }

        private RuntimeException()
        {
        }

        public RuntimeException(string message) : base(message)
        {
        }

        public RuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private RuntimeException(SerializationInfo info, StreamingContext streamingContext)
            : base(info, streamingContext)
        {
            source = info.GetString(nameof(source));
            evaluatedScript = info.GetString(nameof(evaluatedScript));
            line = info.GetValue<Token[]>(nameof(line));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(source), source);
            info.AddValue(nameof(evaluatedScript), evaluatedScript);
            info.AddValue(nameof(line), line);
        }
    }
}