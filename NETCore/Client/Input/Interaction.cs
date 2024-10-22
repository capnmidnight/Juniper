namespace Juniper.Input
{
    /// <summary>
    /// Sounds to play during certain interaction events
    /// </summary>
    public enum Interaction
    {
        /// <summary>
        /// No interaction.
        /// </summary>
        None,

        /// <summary>
        /// A pointer entering a control.
        /// </summary>
        Entered,

        /// <summary>
        /// A pointer entering a control that has been disabled.
        /// </summary>
        EnteredDisabled,

        /// <summary>
        /// A pointer pressing down on a control.
        /// </summary>
        Pressed,

        /// <summary>
        /// A pointer being clicked on a control that has been disabled.
        /// </summary>
        PressedDisabled,

        /// <summary>
        /// A pointer pressing down and releasing in rapid succession on a control.
        /// </summary>
        Clicked,

        /// <summary>
        /// A pointer pressing down and releasing in rapid succession on a control that has been disabled.
        /// </summary>
        ClickedDisabled,

        /// <summary>
        /// The first time dragging occured.
        /// </summary>
        DraggingStarted,

        /// <summary>
        /// The first time dragging occured on a contral that has been disabled.
        /// </summary>
        DraggingStartedDisabled,

        /// <summary>
        /// A pointer pressing down and moving on a control.
        /// </summary>
        Dragged,

        /// <summary>
        /// The last time dragging occured.
        /// </summary>
        DraggingEnded,

        /// <summary>
        /// A pointer no longer being pressed on a control.
        /// </summary>
        Released,

        /// <summary>
        /// A pointer leaving a control.
        /// </summary>
        Exited,

        /// <summary>
        /// A container element being opened, to have its contents become visible.
        /// </summary>
        Opened,

        /// <summary>
        /// A container element being closed, to hide its contents.
        /// </summary>
        Closed,

        /// <summary>
        /// A generic error sound.
        /// </summary>
        Error,

        /// <summary>
        /// A generic completion sound.
        /// </summary>
        Success,

        /// <summary>
        /// Application start up.
        /// </summary>
        StartUp,

        /// <summary>
        /// Application shut down.
        /// </summary>
        ShutDown,

        /// <summary>
        /// A list being scrolled.
        /// </summary>
        Scrolled
    }
}