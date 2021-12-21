/**
 * Sounds to play during certain interaction events
 **/
export declare type Interaction = 
/**
 * No interaction.
 **/
"none"
/**
 * A pointer entering a control.
 **/
 | "entered"
/**
 * A pointer entering a control that has been disabled.
 **/
 | "entereddisabled"
/**
 * A pointer pressing down on a control.
 **/
 | "pressed"
/**
 * A pointer being clicked on a control that has been disabled.
 **/
 | "presseddisabled"
/**
 * A pointer pressing down and releasing in rapid succession on a control.
 **/
 | "clicked"
/**
 * A pointer pressing down and releasing in rapid succession on a control that has been disabled.
 **/
 | "clickeddisabled"
/**
 * The first time dragging occured.
 **/
 | "draggingstarted"
/**
 * The first time dragging occured on a contral that has been disabled.
 **/
 | "draggingstarteddisabled"
/**
 * A pointer pressing down and moving on a control.
 **/
 | "dragged"
/**
 * The last time dragging occured.
 **/
 | "draggingended"
/**
 * A pointer no longer being pressed on a control.
 **/
 | "released"
/**
 * A pointer leaving a control.
 **/
 | "exited"
/**
 * A container element being opened, to have its contents become visible.
 **/
 | "opened"
/**
 * A container element being closed, to hide its contents.
 **/
 | "closed"
/**
 * A generic error sound.
 **/
 | "error"
/**
 * A generic completion sound.
 **/
 | "success"
/**
 * Application start up.
 **/
 | "startup"
/**
 * Application shut down.
 **/
 | "shutdown"
/**
 * A list being scrolled.
 **/
 | "scrolled";
