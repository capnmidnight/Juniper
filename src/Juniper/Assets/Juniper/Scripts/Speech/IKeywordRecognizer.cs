using System;

namespace Juniper.Speech
{
    public interface IKeywordRecognizer
    {
        event EventHandler<KeywordRecognizedEventArgs> KeywordRecognized;

        bool IsAvailable { get; }

        void RefreshKeywords();
    }
}
