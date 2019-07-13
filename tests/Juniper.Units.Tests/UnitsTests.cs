using System;
using Juniper.World.GIS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Units.Tests
{
    [TestClass]
    public class UnitsTests
    {
        [TestMethod]
        public void LatLng_2_UTM()
        {
            var latLng = new LatLngPoint(38.8974146f, -77.0743107f);
            var utm = latLng.ToUTM();
            var latLng2 = utm.ToLatLng();
            var utm2 = latLng2.ToUTM();
            Assert.AreEqual(latLng.Altitude, latLng2.Altitude, 0.0001);
            Assert.AreEqual(latLng.Longitude, latLng2.Longitude, 0.0001);
            Assert.AreEqual(latLng.Latitude, latLng2.Latitude, 0.0001);
            Assert.AreEqual(utm.X, utm2.X, 0.5);
            Assert.AreEqual(utm.Y, utm2.Y, 0.5);
            Assert.AreEqual(utm.Zone, utm2.Zone);
        }

        [TestMethod]
        public void farenheit_2_celsius()
        {
            FloatCompare(0, Farenheit.Celsius(32), 4);
            FloatCompare(100, Farenheit.Celsius(212), 4);
        }

        [TestMethod]
        public void Farenheit_2_kelvin()
        {
            FloatCompare(273.15f, Farenheit.Kelvin(32));
            FloatCompare(373.15f, Farenheit.Kelvin(212));
        }

        [TestMethod]
        public void celsius_2_farenheit()
        {
            FloatCompare(32, Celsius.Farenheit(0));
            FloatCompare(212, Celsius.Farenheit(100));
        }

        [TestMethod]
        public void celsius_2_kelvin()
        {
            FloatCompare(273.15f, Celsius.Kelvin(0));
            FloatCompare(373.15f, Celsius.Kelvin(100));
        }

        [TestMethod]
        public void kelvin_2_farenheit()
        {
            FloatCompare(32, Kelvin.Farenheit(273.15f));
            FloatCompare(212, Kelvin.Farenheit(373.15f));
        }

        [TestMethod]
        public void kelvin_2_celsius()
        {
            FloatCompare(0, Kelvin.Celsius(273.15f));
            FloatCompare(100, Kelvin.Celsius(373.15f));
        }

        [TestMethod]
        public void degrees_2_radians()
        {
            FloatCompare(TAU, Degrees.Radians(360));
        }

        [TestMethod]
        public void degrees_2_hours()
        {
            FloatCompare(24, Degrees.Hours(360), 3);
        }

        [TestMethod]
        public void hours_2_degrees()
        {
            FloatCompare(360, Hours.Degrees(24));
        }

        [TestMethod]
        public void hours_2_radians()
        {
            FloatCompare(TAU, Hours.Radians(24));
        }

        [TestMethod]
        public void radians_2_degrees()
        {
            FloatCompare(360, Radians.Degrees(TAU));
        }

        [TestMethod]
        public void radians_2_hours()
        {
            FloatCompare(24, Radians.Hours(TAU));
        }

        [TestMethod]
        public void grams_2_ounces()
        {
            FloatCompare(0.035273962f, Grams.Ounces(1));
        }

        [TestMethod]
        public void grams_2_pounds()
        {
            FloatCompare(0.0022046228f, Grams.Pounds(1));
        }

        [TestMethod]
        public void grams_2_kilograms()
        {
            FloatCompare(0.001f, Grams.Kilograms(1));
        }

        [TestMethod]
        public void grams_2_tons()
        {
            FloatCompare(0.0000011023113f, Grams.Tons(1));
        }

        [TestMethod]
        public void ounces_2_grams()
        {
            FloatCompare(28.349523f, Ounces.Grams(1));
        }

        [TestMethod]
        public void ounces_2_pounds()
        {
            FloatCompare(0.0625f, Ounces.Pounds(1));
        }

        [TestMethod]
        public void ounces_2_kilograms()
        {
            FloatCompare(0.028349524f, Ounces.Kilograms(1));
        }

        [TestMethod]
        public void ounces_2_tons()
        {
            FloatCompare(3.125e-5f, Ounces.Tons(1));
        }

        [TestMethod]
        public void pounds_2_grams()
        {
            FloatCompare(453.59233f, Pounds.Grams(1));
        }

        [TestMethod]
        public void pounds_2_ounces()
        {
            FloatCompare(16, Pounds.Ounces(1));
        }

        [TestMethod]
        public void pounds_2_kilograms()
        {
            FloatCompare(0.45359233f, Pounds.Kilograms(1));
        }

        [TestMethod]
        public void pounds_2_tons()
        {
            FloatCompare(0.0005f, Pounds.Tons(1));
        }

        [TestMethod]
        public void kilograms_2_grams()
        {
            FloatCompare(1000f, Kilograms.Grams(1));
        }

        [TestMethod]
        public void kilograms_2_ounces()
        {
            FloatCompare(35.273962f, Kilograms.Ounces(1));
        }

        [TestMethod]
        public void kilograms_2_pounds()
        {
            FloatCompare(2.2046228f, Kilograms.Pounds(1));
        }

        [TestMethod]
        public void kilograms_2_tons()
        {
            FloatCompare(0.0011023113f, Kilograms.Tons(1));
        }

        [TestMethod]
        public void tons_2_grams()
        {
            FloatCompare(907184.75f, Tons.Grams(1));
        }

        [TestMethod]
        public void tons_2_ounces()
        {
            FloatCompare(32000f, Tons.Ounces(1));
        }

        [TestMethod]
        public void tons_2_pounds()
        {
            FloatCompare(2000, Tons.Pounds(1));
        }

        [TestMethod]
        public void tons_2_kilograms()
        {
            FloatCompare(907.18475f, Tons.Kilograms(1));
        }

        [TestMethod]
        public void millimeters_2_centimeters()
        {
            FloatCompare(0.1f, Millimeters.Centimeters(1));
        }

        [TestMethod]
        public void millimeters_2_inches()
        {
            FloatCompare(0.039370079f, Millimeters.Inches(1));
        }

        [TestMethod]
        public void millimeters_2_feet()
        {
            FloatCompare(0.00328084f, Millimeters.Feet(1));
        }

        [TestMethod]
        public void millimeters_2_miles()
        {
            FloatCompare(0.00000062137119f, Millimeters.Miles(1));
        }

        [TestMethod]
        public void millimeters_2_kilometers()
        {
            FloatCompare(1e-6f, Millimeters.Kilometers(1));
        }

        [TestMethod]
        public void centimeters_2_inches()
        {
            FloatCompare(0.39370079f, Centimeters.Inches(1));
        }

        [TestMethod]
        public void centimeters_2_feet()
        {
            FloatCompare(0.0328084f, Centimeters.Feet(1));
        }

        [TestMethod]
        public void centimeters_2_miles()
        {
            FloatCompare(6.21371e-6f, Centimeters.Miles(1), 6);
        }

        [TestMethod]
        public void centimeters_2_millimeters()
        {
            FloatCompare(10, Centimeters.Millimeters(1));
        }

        [TestMethod]
        public void centimeters_2_kilometers()
        {
            FloatCompare(1e-5f, Centimeters.Kilometers(1));
        }

        [TestMethod]
        public void inches_2_feet()
        {
            FloatCompare(1 / 12f, Inches.Feet(1));
        }

        [TestMethod]
        public void inches_2_miles()
        {
            FloatCompare(0.000015782828f, Inches.Miles(1), 8);
        }

        [TestMethod]
        public void inches_2_centimeters()
        {
            FloatCompare(2.54f, Inches.Centimeters(1));
        }

        [TestMethod]
        public void inches_2_millimeters()
        {
            FloatCompare(25.4f, Inches.Millimeters(1));
        }

        [TestMethod]
        public void inches_2_meters()
        {
            FloatCompare(0.0254f, Inches.Meters(1));
        }

        [TestMethod]
        public void inches_2_kilometers()
        {
            FloatCompare(0.0000254f, Inches.Kilometers(1));
        }

        [TestMethod]
        public void feet_2_inches()
        {
            FloatCompare(12, Feet.Inches(1));
        }

        [TestMethod]
        public void feet_2_miles()
        {
            FloatCompare(0.00018939394f, Feet.Miles(1));
        }

        [TestMethod]
        public void feet_2_centimeters()
        {
            FloatCompare(30.48f, Feet.Centimeters(1));
        }

        [TestMethod]
        public void feet_2_millimeters()
        {
            FloatCompare(304.8f, Feet.Millimeters(1));
        }

        [TestMethod]
        public void feet_2_meters()
        {
            FloatCompare(0.3048f, Feet.Meters(1));
        }

        [TestMethod]
        public void feet_2_kilometers()
        {
            FloatCompare(0.0003048f, Feet.Kilometers(1));
        }

        [TestMethod]
        public void meters_2_inches()
        {
            FloatCompare(39.370079f, Meters.Inches(1));
        }

        [TestMethod]
        public void meters_2_feet()
        {
            FloatCompare(3.28084f, Meters.Feet(1));
        }

        [TestMethod]
        public void meters_2_miles()
        {
            FloatCompare(0.00062137119f, Meters.Miles(1));
        }

        [TestMethod]
        public void meters_2_centimeters()
        {
            FloatCompare(100f, Meters.Centimeters(1));
        }

        [TestMethod]
        public void meters_2_millimeters()
        {
            FloatCompare(1000f, Meters.Millimeters(1));
        }

        [TestMethod]
        public void meters_2_kilometers()
        {
            FloatCompare(0.001f, Meters.Kilometers(1));
        }

        [TestMethod]
        public void kilometers_2_inches()
        {
            FloatCompare(39370.079f, Kilometers.Inches(1));
        }

        [TestMethod]
        public void kilometers_2_feet()
        {
            FloatCompare(3280.84f, Kilometers.Feet(1), 6);
        }

        [TestMethod]
        public void kilometers_2_miles()
        {
            FloatCompare(0.62137119f, Kilometers.Miles(1));
        }

        [TestMethod]
        public void kilometers_2_centimeters()
        {
            FloatCompare(100000f, Kilometers.Centimeters(1));
        }

        [TestMethod]
        public void kilometers_2_millimeters()
        {
            FloatCompare(1000000f, Kilometers.Millimeters(1));
        }

        [TestMethod]
        public void miles_2_inches()
        {
            FloatCompare(63360f, Miles.Inches(1));
        }

        [TestMethod]
        public void miles_2_feet()
        {
            FloatCompare(5280f, Miles.Feet(1));
        }

        [TestMethod]
        public void miles_2_centimeters()
        {
            FloatCompare(160934.391f, Miles.Centimeters(1));
        }

        [TestMethod]
        public void miles_2_millimeters()
        {
            FloatCompare(1609343.88f, Miles.Millimeters(1));
        }

        [TestMethod]
        public void miles_2_meters()
        {
            FloatCompare(1609.3440f, Miles.Meters(1));
        }

        [TestMethod]
        public void miles_2_kilometers()
        {
            FloatCompare(1.6093440f, Miles.Kilometers(1));
        }

        [TestMethod]
        public void milesperhour_2_kilometersperhour()
        {
            FloatCompare(1.6093440f, MilesPerHour.KilometersPerHour(1));
        }

        [TestMethod]
        public void milesperhour_2_meterspersecond()
        {
            FloatCompare(0.44704f, MilesPerHour.MetersPerSecond(1));
        }

        [TestMethod]
        public void milesperhour_2_feetpersecond()
        {
            FloatCompare(1.46666666666666666666666666666667f, MilesPerHour.FeetPerSecond(1));
        }

        [TestMethod]
        public void kilometersperhour_2_milesperhour()
        {
            FloatCompare(0.62137119f, KilometersPerHour.MilesPerHour(1));
        }

        [TestMethod]
        public void kilometersperhour_2_meterspersecond()
        {
            FloatCompare(0.27777777777777777777777777777777777778f, KilometersPerHour.MetersPerSecond(1));
        }

        [TestMethod]
        public void kilometersperhour_2_feetpersecond()
        {
            FloatCompare(0.91134442f, KilometersPerHour.FeetPerSecond(1));
        }

        [TestMethod]
        public void meterspersecond_2_milesperhour()
        {
            FloatCompare(2.2369363f, MetersPerSecond.MilesPerHour(1));
        }

        [TestMethod]
        public void meterspersecond_2_kilometersperhour()
        {
            FloatCompare(3.6f, MetersPerSecond.KilometersPerHour(1), 5);
        }

        [TestMethod]
        public void meterspersecond_2_feetpersecond()
        {
            FloatCompare(3.28084f, MetersPerSecond.FeetPerSecond(1));
        }

        [TestMethod]
        public void feetpersecond_2_milesperhour()
        {
            FloatCompare(0.68181818f, FeetPerSecond.MilesPerHour(1));
        }

        [TestMethod]
        public void feetpersecond_2_kilometersperhour()
        {
            FloatCompare(1.09728f, FeetPerSecond.KilometersPerHour(1));
        }

        [TestMethod]
        public void feetpersecond_2_meterspersecond()
        {
            FloatCompare(0.3048f, FeetPerSecond.MetersPerSecond(1));
        }

        [TestMethod]
        public void pascals_2_hectopascals()
        {
            FloatCompare(0.01f, Pascals.Hectopascals(1));
        }

        [TestMethod]
        public void pascals_2_poundspersquareinch()
        {
            FloatCompare(0.00014503773800722f, Pascals.PoundsPerSquareInch(1));
        }

        [TestMethod]
        public void pascals_2_kilopascals()
        {
            FloatCompare(0.001f, Pascals.Kilopascals(1));
        }

        [TestMethod]
        public void hectopascals_2_pascals()
        {
            FloatCompare(100f, Hectopascals.Pascals(1));
        }

        [TestMethod]
        public void hectopascals_2_poundspersquareinch()
        {
            FloatCompare(0.014503773800722f, Hectopascals.PoundsPerSquareInch(1));
        }

        [TestMethod]
        public void hectopascals_2_kilopascals()
        {
            FloatCompare(0.1f, Hectopascals.Kilopascals(1));
        }

        [TestMethod]
        public void poundspersquareinch_2_pascals()
        {
            FloatCompare(6894.7572799999125f, PoundsPerSquareInch.Pascals(1));
        }

        [TestMethod]
        public void poundspersquareinch_2_hectopascals()
        {
            FloatCompare(68.947572799999125f, PoundsPerSquareInch.Hectopascals(1));
        }

        [TestMethod]
        public void poundspersquareinch_2_kilopascals()
        {
            FloatCompare(6.8947572799999125f, PoundsPerSquareInch.Kilopascals(1));
        }

        [TestMethod]
        public void kilopascals_2_hectopascals()
        {
            FloatCompare(10f, Kilopascals.Hectopascals(1));
        }

        [TestMethod]
        public void kilopascals_2_poundspersquareinch()
        {
            FloatCompare(0.14503773800722f, Kilopascals.PoundsPerSquareInch(1));
        }

        [TestMethod]
        public void kilopascals_2_pascals()
        {
            FloatCompare(1000f, Kilopascals.Pascals(1));
        }

        [TestMethod]
        public void FormatMeters()
        {
            Assert.AreEqual(PI.Label(UnitOfMeasure.Meters, 3), "3.14 m");
        }

        private const float PI = (float)Math.PI;
        private const float TAU = (float)(2 * Math.PI);

        private static string SigFig(float value, int sigfigs)
        {
            var q = (int)Math.Log10(value) + 1;
            sigfigs -= q;
            var p = Math.Pow(10, sigfigs);
            var v = (Math.Round(value * p) / p).ToString();
            if (sigfigs > 0)
            {
                var i = v.IndexOf(".", System.StringComparison.Ordinal);
                if (i == -1)
                {
                    v += ".";
                    i = v.Length - 1;
                }

                var z = sigfigs + i + 1;
                while (v.Length < z)
                {
                    v += "0";
                }
            }
            return v;
        }

        private static void FloatCompare(float expected, float actual, int sigFigs = 9, string msg = null, params object[] args)
        {
            Assert.AreEqual(SigFig(expected, sigFigs), SigFig(actual, sigFigs), msg, args);
        }

    }
}
