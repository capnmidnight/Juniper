using System;

using Juniper.Input.Speech;

namespace Juniper.Unity.Input.Speech
{
    public interface IKeywordRecognizer
    {
        event EventHandler<KeywordRecognizedEventArgs> KeywordRecognized;

        void RefreshKeywords();
    }
}
