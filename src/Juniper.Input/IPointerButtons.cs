namespace Juniper.Input.Pointers
{
    /// <summary>
    /// An interface for specifying how devices with buttons should behave.
    /// </summary>
    /// <typeparam name="ButtonIDType">An enumeration type for the native button field for the device.</typeparam>
    public interface IPointerButtons<ButtonIDType>
        where ButtonIDType : struct
    {
        /// <summary>
        /// Implementers should use this to indicate the connection status of the controller.
        /// </summary>
        bool IsConnected
        {
            get;
        }

        /// <summary>
        /// Implmeneters should return true when any button on the controller has been depressed.
        /// </summary>
        bool AnyButtonPressed
        {
            get;
        }

        /// <summary>
        /// Implementers should return true when a button is first depressed.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool IsButtonDown(ButtonIDType button);

        /// <summary>
        /// Implementers should return true while the button is depressed.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool IsButtonPressed(ButtonIDType button);

        /// <summary>
        /// Implementers should return true when the button is first released.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool IsButtonUp(ButtonIDType button);

        /// <summary>
        /// Add a new button mapping to the pointer.
        /// </summary>
        /// <param name="buttonID"></param>
        /// <param name="buttonValue"></param>
        void AddButton(ButtonIDType buttonID, InputEventButton buttonValue);

        /// <summary>
        /// Remove a button mapping from the pointer.
        /// </summary>
        /// <param name="buttonID"></param>
        void RemoveButton(ButtonIDType buttonID);
    }
}
