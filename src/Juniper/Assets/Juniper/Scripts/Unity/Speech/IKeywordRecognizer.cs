using System;

using Juniper.Speech;

namespace Juniper.Unity.Speech
{
    public interface IKeywordRecognizer
    {
        event EventHandler<KeywordRecognizedEventArgs> KeywordRecognized;

        void RefreshKeywords();
    }
}
