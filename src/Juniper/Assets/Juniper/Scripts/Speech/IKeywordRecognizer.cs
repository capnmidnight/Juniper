using System;

namespace Juniper.Speech
{
    public interface IKeywordRecognizer
    {
        event EventHandler<KeywordRecognizedEventArgs> KeywordRecognized;

        void RefreshKeywords();
    }
}
