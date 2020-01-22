using System;
using System.Linq;
using System.Runtime.Serialization;

using Line = System.Collections.Generic.List<Juniper.Primrose.Token>;

namespace Juniper.Primrose
{
    [Serializable]
    public sealed class RuntimeException : Exception
    {
        public string SourceCode { get; }

        public string EvaluatedScript { get; }

        public Token[] Line { get; }

        public RuntimeException(string source, Line line, string evaluatedScript, Exception innerException)
            : base("Runtime Error", innerException)
        {
            SourceCode = source;
            EvaluatedScript = evaluatedScript;
            Line = (from t in line
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
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            SourceCode = info.GetString(nameof(SourceCode));
            EvaluatedScript = info.GetString(nameof(EvaluatedScript));
            Line = info.GetValue<Token[]>(nameof(Line));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            base.GetObjectData(info, context);
            info.AddValue(nameof(SourceCode), SourceCode);
            info.AddValue(nameof(EvaluatedScript), EvaluatedScript);
            info.AddValue(nameof(Line), Line);
        }
    }
}