namespace Juniper.IO;

/// <summary>
/// Defines an interface for objects that can be installed and have their
/// installation progress tracked.
/// </summary>
public interface IInstallable
{
    /// <summary>
    /// Install, with or without a full reset.
    /// </summary>
    /// <param name="reset"></param>
    /// <returns>True, if the installation completed successfully. False if the installable should be
    /// recycled and tried again.</returns>
    void Install(bool reset);

    /// <summary>
    /// Reinstall the object with a full reset.
    /// </summary>
    void Reinstall();

    /// <summary>
    /// Remove platform-specific features from the object.
    /// </summary>
    void Uninstall();
}