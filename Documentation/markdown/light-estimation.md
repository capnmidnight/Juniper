# Light Estimation

# Introduction

Light Estimation is complex and requires multiple parts

- The scene needs to be reoriented to Magnetic North, because the AR subsystem initializes its view of the scene to a completely arbitrary direction.
- The position of the sun needs to be calculated, which itself requires,
- Acquiring a GPS lock.
- Optionally, a Weather Report can be retrieved to enhance the estimate with Cloud Coverage data.
- Extremely optionally, a WindZone can be added to the scene to take advantage of the Wind Direction and Wind Speed values also returned with the Weather Report.
- A platform-specific light intensity and color value, estimated from the camera image, must be acquired from the AR subsystem
- And finally, all of this data needs to be fused into a composite light estimate.

## Using Light Estimation in Your Project

The vast majority of these tasks are packed up together in a Prefab at:

    Juniper/Prefabs/World.prefab

This prefab contains GameObjects and Components:

-   [GO] World: having the `CompassRose` component to reorient its children to North,
  - [GO] SunRig:
    -   [GO] Sun: a directional light with the following components:
      -   GPSLocation: starts Unity's location service and receives updates
      -   SunPosition: calculates the position of the sun with respect to the user given the current Latitude, Longitude, and Time.
      -   Weather: connects to a weather reporting API to receive the current weather conditions.
        -   That weather reporting API is currently [OpenWeatherMap.org](https://openweathermap.org/). The service requires an authorization key, so you'll have to sign up to get one.
      -   OutdoorLightEstimate: fuses the data from GPSLocation, SunPosition, and Weather to modify the orientation, intensity, and shadowStrength of the light, while also changing the ambientIntensity and ambientColor in RenderSettings.
  -   [GO] WindZone: with the components:
    -   [CMP] WindZone: pushes particles and trees around
    -   [CMP] OutdoorWindEstimate: reads the weather report and applies the Wind Speed and Wind Direction values to the WindZone.

## Component Settings

Once you have the World prefab in your scene, expand the World object and find the Sun object. Pay the most attention to the highlighted fields:

![World Settings](WorldSettings.png)

Notes:
-   Make sure the Light's type is set to Directional
-   There is nothing to do in the GPSLocation component unless you want to simulate different locations in the world.
-   The Weather component is optional, it can be removed if you don't want to make network requests or don't want cloud cover reporting.
    -   If you do use the Weather component, you'll need an API key from the OpenWeatherMap.org. There is a development key in the repository, but it's meant for only one user at a time, so you should create your own.
    -   The weather report is necessary to receive cloud cover values. Cloud cover values allow us to skew the light estimate towards more ambient or more directional light. The AR subsystem only provides one light estimation value, and that value is based on the average intensity of pixels captured by the AR camera. Most systems treat this as a scale factor on the ambient light, but with our usage of cloud cover, we can achieve more accurate results.
-   The OutdoorLightEstimate component is where all the work is done of combining the data from all of the previous components and use the to set the light's orientation and intensity.
    -   The Weather and SunPosition fields refer to the previous two components. Both components may be treated as optional. They should automatically populate when the OutdoorLightEstimate component is created, but in case they don't, configure their value here.
    -   The DefaultColor field is for setting a light color on systems that don't have light color analysis (ARKit is the only system that is known to have it right now). Keep this set to White unless you know you'll be running in an environment with a skewed environment color.
    -   SetAmbientLight and SetDirectionalLight checkboxes control whether or not the component will modify the ambient and/or directional light settings.
    -   AmbientScale controls the final output of the ambient light value after the estimation is complete.
        -   Set it to 1 to not modify the ambient estimate.
        -   Set it to 0 to completely delete the ambient light.
        -   Set it to values greater than 1 to push the ambient light beyond what the light estimation predicts.
        -   Set it between 0 and 1 to subdue it under the estimate.
    -   DirectionalScale similarly controls the final output of the directional light value, with the same control over values.
    - The ShadowScale field changes the darkness of the shadows that are cast on the ground. Completely black shadows look very unnatural. The system will automatically make shadows lighter as the ambient light estimate increases, but this scale factor can be used to control it further.
    - If the Cloud Cover cannot be determined (due to lack of access to a weather report for whatever reason), then a default value of 50% is used. The CloudCoverScale value controls how much that Cloud Scale value skews the light between ambient and directional.
        -   A value of 0.5 means that the default cloud cover value of 50% will skew the light estimate 25% to ambient and 75% to directional.
