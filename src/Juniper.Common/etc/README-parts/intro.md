# Introduction

[TOC]

Juniper.Units is a .NET library for converting values between units of measure.  It is a component of the larger [Juniper](https://capnmidnight.github.io/Juniper) project.

With Juniper.Units, one may:
* Directly convert a value from one unit to another, directly: 
 * `Units.Meters.Feet(1.0f) == 3.28084f` (approx)
* Parametrically convert a value: 
 * `Units.Convert(5280.0f, UnitOfMeasure.Feet, UnitOfMeasure.Miles) == 1.0f`
* Express a value with its unit and have it converted to an analogous unit for target system of units (this is heretofor referred to as "system conversion"): 
 * `Units.Convert(1.0f, UnitOfMeasure.Meters, SystemOfMeasure.USCustomary) == 3.28084f`

Direct conversions are single-line methods using constant value parameters, so they will optimize to inlined function calls in Release mode.

System conversions use a Dictionary lookup precomputed at startup for each conversion function. No reflection takes place post-startup.

Juniper.Units can also be used for pretty-printing:
* Get the abbreviation for a unit of measure: 
 * `Units.Abbreviate(UnitOfMeasure.MetersPerSecondSquared) == "m/s²"`,
* Format a string for a value with a given unit: 
 * `Units.Label(180.0f, UnitOfMeasure.Degrees) == "180 °"`
* Format a string for a value after parametric conversion: 
 * `Units.Label(1100.0f, UnitOfMeasure.Megabytes, UnitOfMeasure.Gigabytes) == "1.1 GB"`
* Format a string for a value after system conversion: 
 * `Units.Label(212.0f, UnitOfMeasure.Farenheit, SystemOfMeasure.Metric) == "100 C"`
* Format a string for a value to a number of significant figures: 
 * `Units.Label(Math.PI, UnitOfMeasure.Radians, 3) == "3.14 rad"`
 * All other versions of `Label` also accept the significant figures parameter.

`Convert` and `Label` are also available as extension methods on `float` and `Nullable<float>`:
* `float x = 2.5f; x.Convert(UnitOfMeasure.Mibibytes, UnitOfMeasure.Kibibytes) == "2560.0 MiB"`
* `float? y = null; y.Label(UnitOfMeasure.Pascals) == "N/A"`

See Juniper.UnitOfMeasure for a complete list of supported units. Unit types are easy to add, so if the current list is short, make a request!

# BONUS

With the Juniper.World.GIS.LatLngPoint, conversions to UTM coordinates are available, as well as calculations for the position of planet Earth's star Sol in the sky at a given time of day. See: Juniper.Units.LatLng.SunPosition.