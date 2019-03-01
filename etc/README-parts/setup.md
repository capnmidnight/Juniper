# Setup

## Installation

There are two ways to install Juniper.

- [Download the UnityPackage](Juniper.unitypackage) and install it in your project, or
- [Clone the repository](https://github.com/capnmidnight/Juniper) and copy the contents of the `Package` directory to your Assets folder in a directory named `Juniper`.

## Project Integration

There are a number of different components that go into configuring a Juniper application:

- The AR or VR subsystem needs to be setup for use.
- The EventSystem needs to be configured,
    - along with an InputModuel that understands different types of 3D pointers, and,
    - those pointers appropriate for the XR subsystem.
- The user-interface interaction feedback audio system, which includes haptic feedback.
- A manager for AR targets, if they are being used.
- Mouse and Keyboard movement for use in the Editor.
- A camera configuration that,
    - updates the clear flags when switching between types of XR subsystems, and
    - reduces rendering quality to keep the target frame-rate as close to the native display refresh rate as possible.
- And, a graphical component for fading views in and out during scene transitions and other long, blocking operations, so the user doesn't see the camera locked in place.


To that end, there is a Prefab that has most of this pre-configured, requiring only project-specific configuration once included in the project. It is located at

    Juniper/Prefabs/UserRig.prefab

The prefab contains the following GameObjects and Components:

- **[GO] UserRig**: The root of the prefab
  - *[CMP] XRSystem*: manages system configuration and AR/VR feature sets.
  - *[CMP] EventSystem*: Unity's EventSystem component for triggering pointer events.
  - *[CMP] UnifiedInputModule*: A custom InputModule that fuses a variety of different pointers between the computer mouse, the touch screen, and any potential motion controllers or hand tracking systems connected to the application.
  - *[CMP] Mouse*: The mouse pointer, mostly only used in the Editor.
  - *[CMP] Touches*: Invisible pointers for each finger.
  - *[CMP] TrackerKeeper*: A top-level manager for AR image targets, with event handlers for a variety of different scenarios in managing the AR experience.
  - *[CMP] R2D2*: Configuration setting for different audio clips to playback during different types of user interactions in the system. This also provides haptic feedback for most of the interaction types.
  - **[GO] Stage**: The immediate area around the user, separate from the camera. Keyboard/GamePad movement, or VR teleportation will move the stage instead of the camera. Moving the camera is reserved for the XR subsystem, to maintain the right view angle for the user.
    - *[CMP] KeyboardMovement*: Moves the stage with Unity's configured keyboard movement keys in the direction the camera is looking. Mostly only used in the Editor for testing.
    - **[GO] Camera**: The main camera.
      - *[CMP] Camera*: The main camera component. You may want to configure the Clipping Planes. Do not change the Field of View, as it will be ignored and recomputed at runtime for the specific XR subsystem.
      - *[CMP] CameraExtensions*: Manages camera attributes like camera FOV.
      - *[CMP] QualityDegrader*: Projects start out at the highest quality setting and degrade through the quality settings enabled in the Editor until either there are no more quality setting to go through or the app consistently hits 60FPS.
      - *[CMP] PhysicsRaycaster*: The raycaster used for all pointers that fire through the main camera, including the Moues and Touches.
      - **[GO] Fader**: A box that sits around the user's head for fading the camera view in and out in a way that works on all XR subsystems.
        - *[CMP] Darth*: Darth Fader is the component that manages fading of the camera view. You can retrieve Darth Fader and use it to fade out the screen if you ever have any long, blocking operations that could lock up the camera.

## Configuration

Once you have the UserRig prefab in your scene, select the UserRig game object and find the XRSystem component. Pay the most attention to the highlighted fields:

![XR System Settings](XRSystemSettings.png)

Notes:
- Make sure to set the TrackingSystem on XRSystem. It will initially be set to None.
- Additionally, if this is the first time you are setting up a Vuforia-based project and you select the "Marker Based AR" tracking type, you may need to use the GameObject menu in Unity to add a Vuforia prefab in order to force the Vuforia components to install into your project. You may delete it afterwards.
- R2D2 will be configured with a default set of sounds that come with Juniper. These sounds are Star Trek sound effects, so they will not be appropriate to deploy to a production system. Make sure you find a new set of sounds appropriate for your application.
- TrackerKeeper has a number of interesting events for managing your project workflow. It's recommended that you run Vuforia in Delayed Initialization mode, so your application can start with a splash-screen/menu view without the camera constantly running an draining the battery.
