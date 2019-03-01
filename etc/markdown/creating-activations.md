# Creating Augmented Reality Activations

# Introduction

Activations are combinations of parallel Vuforia Image Marker Targets and series video clips that get displayed over top of the markers. Creating them requires a number of Unity GameObjects and Components to be configured and calibrated against each other.

## Object Hierarchy

Each Activation must have the following structure (`[GO]` symbols denote GameObjects, `[CMP]` symbols denote Components, unmarked lines are notes on how to configure the preceding object):

- [GO] **&lt;Activation Name&gt;**, (e.g. 'Baltimore', 'T-Shirt', 'Mars', etc) - the root of the activation.
  - [CMP] *Transform*
    - Set scale to &lt;1, 1, 1&gt;
    - The value of the other properties do not matter.
  - [CMP] *Activation* - the main controller script.
    - Set the `Keyboard Shortcut` value to force the first marker in the list of targets to activate when running in the editor. This is helpful for testing workflow without testing detection or tracking.
    - Set `Remove Graphic` checkbox if you would like the target image to not appear when running in the editor. When running the live application, the target image is always removed.
    - Set the `On Complete` event to fire the `ScaleTransform.Enter()` method of the `Main > Padding > moveRightPanel (4)` GameObject.
  - [GO] **Targets** - the collection of targets, any one of which may activate the experience.
    - [CMP] *Transform*
      - Set scale to &lt;1, 1, 1&gt;
      - The value of the other properties does not matter.
    - [GO] **Target (n)** - where `n` is a number.
      - [CMP] *Transform* - there is no need to set the transform. Vuforia will automatically configure the scale and the position and rotation do not matter. If you create the target object using the `GameObject > Vuforia > Image` menu option, then most of the following components will have most of their properties configured correctly.
      - [CMP] *ImageTargetBehaviour* - this is Vuforia's target trackable component.
        - Set `Type` to `Predefined`.
        - Set `Database` to `Exelon`.
        - Set `Image Target` to the target you desire.
        - Open the `Advanced` group panel and set the `Extended Tracking` checkbox to filled.
      - [CMP] *MeshRenderer* - Used to visualize the target image. Allow Vuforia to configure this component.
      - [CMP] *ImageTargetMesh-(XXXX)* - Sets the aspect ratio of the target image. Allow Vuforia to configure this component.
      - [CMP] *TrackableFoundEventHandler* - This is a Juniper component that manages the understanding of the target--when it was found, whether or not we're standing a safe distance from it, firing events when it is found, etc.
         - After creating the `Attachment Point` object below, come back and set the `Attachment Point` property to point to it.
         - Set the `Safe Distance` to a minimum distance in meters that the user should stand from the marker.
         - There is no need to set any of the events. The `Activation` script will manage it.
         - [GO] Activation Point - this will be the parent of the Videos and is used to callibrate the position of the videos against the target. See "Calibrating Videos Against Image Targets" below for more details.
  - [GO] **Videos** - the collection of videos, all of which will play in sequence.
    - [CMP] *Transform* - none of the values matter, they will be reset to the origin position, zero rotation, and unity scale by the `Activation`.
    - [GO] **Video (n)** - where `n` is a number.
      - [CMP] *Transform*
         - Generally speaking, one should set the `Position` to &lt;0, 0, 0&gt;. Only set the position to a different value if you explicitly want the video offset from the poster.
         - Set `Rotation` to &lt;90, 0, 0&gt;
         - Set `Scale` to &lt;1, videoClip.height/videoClip.width, 1&gt;
      - [CMP] *MeshFilter*
         - Set the `Mesh` field to `Quad`.
      - [CMP] *MeshRenderer*
         - Set `Light Probes` to `Off`.
         - Set `Reflection Probes` to `Off`.
         - Set `Cast Shadows` to `Off`.
         - Clear the `Receive Shadows` textbox.
         - Set `Materials` to one element: the `VideoOutput` material.
      - [CMP] *VideoPlayer*
         - Set `Source` to `Video Clip`
         - Set `Video Clip` to the video you would like to play at this position.
         - Fill the `Play on Awake` checkbox.
         - Fill the `Wait for First Frame` checkbox.
         - Clear the `Loop` checkbox. The `Activation` will set the `Loop` field for the last video in the series.
         - Set `Playback Speed` to `1`.
         - Set `Render Mode` to `Material Override`.
         - Set `Renderer` to this very same GameObject.
         - Set `Material Property` to `_MainTex`
         - Set `Audio Output Mode` to `None`.
      - [CMP] *VideoVader* - this is a Juniper component that manages the fade transition.
         - There is no need to set any of the events.
         - Set `Attack` to `0`.
         - If the SpiralTransition follows, set `Release` to `0.75`. Otherwise, set `Release` to `0`.
         - Clear the `Looping` checkbox.
         - Set `Tween` to `Quadratic`
         - Set `Tween K` to `0`.
         - Set `Min Alpha` to `0`.
         - Set `Max Alpha` to `1`.
         - Set `Length` to `0.5` (seconds).
         - Set `Restart Timeout` to `10` (seconds).
         - Set `Linger Timeout` to `0`.
      - (optional) [CMP] *SpiralTransition* - this is a Juniper component that manages the transition from world-space to "fake camera space", to make the video appears as if it is full-screen.
         - There is no need to set any of the events.
         - Set `Attack Time` to `0.75`.
         - Set `Release Time` to `0`.
         - Set `Tween` to `Sine`.
         - Set `Tween K` to `0`.
         - Set `Start Orientat` to &lt;90, 0, 0&gt;
         - Set `End Orientation` to &lt;0, 0, -90&gt;
         - Fill the `Full Screen` checkbox.
         - Do not set `Distance`. With the `Full Screen` property set, the Distance will be calculated automatically.
         - Set `Length` to `1`.

## Calibrating Videos Against Image Targets

Because the image targets are subsets of the poster image, the videos and the targets do not have the same orientation. In many cases, they don't even have the same scale.

- First, make sure the MeshRenderer on the target that you want to calibrate is enabled. Only the first target in the list of targets will be activated, so to calibrate all targets you'll have to complete this process multiple times after reordering the targets in the list of targets.
- Start the app in the Unity editor.
- Click the `Start` button.
- Wait for `Camera Initialization` to complete.
- Prepare your mouse pointer over Unity's Pause button.
- When the `Scanning` graphic appears, hit the shortcut key for the activation you want to edit:
  - `B` for Baltimore
  - `P` for Philadelphia
  - `C` for Chicago
  - `W` for Washington
- When the target appears, the video will be playing right away. Click the Pause button.
- Drill down through the Hierarchy to find the currently playing video. It will be the only one in the list of videos for the activation that is an active game object. The list of videos will have been reparented to the active target.
- Find the video's MeshRenderer and double-click its material.
- Set the `Opacity` field of the material instance to a value that lets you see both the video and the image target comfortably. Somewhere are `0.5` usually works.
- Return to the target object and select its `Attachment Point` game object underneath.
- Scale and Translate the Attachment Point until the images overlap satisfactorily.
- Click the context menu of the Attachment Point's Transform and select "Copy Component".
- Stop the Unity Editor.
- Click the context menu of the Attachment Point's Transform and select "Past Component Values".
- Apply the changes to the Activation's prefab.
- Save the scene.
- Repeat for all targets.
