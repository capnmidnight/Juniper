using System;

namespace Juniper.Speech
{
    public interface IKeywordRecognizer
    {
        bool IsAvailable { get; }
    }
}
