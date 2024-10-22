namespace Juniper.Input;

/// <summary>
/// An extension of Unity's UnityEngine.EventSystem.PointerInputModule.InputButton,
/// to include a "None" value, for making explicit selections of no support for
/// input features while working in the editor.
/// </summary>
public enum InputEventButton
{
    /// <summary>
    /// No input selection.
    /// </summary>
    None = -1,

    /// <summary>
    /// The left mouse button.
    /// </summary>
    Left = 0,

    /// <summary>
    /// The right mouse button.
    /// </summary>
    Right = 1,

    /// <summary>
    /// The middle mouse button.
    /// </summary>
    Middle = 2
}