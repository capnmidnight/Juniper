namespace Juniper.Progress;

/// <summary>
/// Progress reporting interface for asynchronous operations.
/// </summary>
public interface IProgress
{
    /// <summary>
    /// The message of the most recent progress report.
    /// </summary>
    string? Status { get; }

    /// <summary>
    /// The value of the most recent progress report.
    /// </summary>
    float Progress { get; }

    /// <summary>
    /// Report progress to a listener implementation.
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="status"></param>
    void Report(float progress, string? status = null);
}