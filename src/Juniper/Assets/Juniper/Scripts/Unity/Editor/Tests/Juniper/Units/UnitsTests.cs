using NUnit.Framework;

using UnityEngine;

namespace Juniper
{
    public class UnitsTests
    {
        [Test]
        public void farenheit_2_celsius()
        {
            FloatCompare(0, Units.Farenheit.Celsius(32));
            FloatCompare(100, Units.Farenheit.Celsius(212));
        }

        [Test]
        public void Farenheit_2_kelvin()
        {
            FloatCompare(273.15f, Units.Farenheit.Kelvin(32));
            FloatCompare(373.15f, Units.Farenheit.Kelvin(212));
        }

        [Test]
        public void celsius_2_farenheit()
        {
            FloatCompare(32, Units.Celsius.Farenheit(0));
            FloatCompare(212, Units.Celsius.Farenheit(100));
        }

        [Test]
        public void celsius_2_kelvin()
        {
            FloatCompare(273.15f, Units.Celsius.Kelvin(0));
            FloatCompare(373.15f, Units.Celsius.Kelvin(100));
        }

        [Test]
        public void kelvin_2_farenheit()
        {
            FloatCompare(32, Units.Kelvin.Farenheit(273.15f));
            FloatCompare(212, Units.Kelvin.Farenheit(373.15f));
        }

        [Test]
        public void kelvin_2_celsius()
        {
            FloatCompare(0, Units.Kelvin.Celsius(273.15f));
            FloatCompare(100, Units.Kelvin.Celsius(373.15f));
        }

        [Test]
        public void degrees_2_radians()
        {
            FloatCompare(2 * Mathf.PI, Units.Degrees.Radians(360));
        }

        [Test]
        public void degrees_2_hours()
        {
            FloatCompare(24, Units.Degrees.Hours(360));
        }

        [Test]
        public void hours_2_degrees()
        {
            FloatCompare(360, Units.Hours.Degrees(24));
        }

        [Test]
        public void hours_2_radians()
        {
            FloatCompare(2 * Mathf.PI, Units.Hours.Radians(24));
        }

        [Test]
        public void radians_2_degrees()
        {
            FloatCompare(360, Units.Radians.Degrees(2 * Mathf.PI));
        }

        [Test]
        public void radians_2_hours()
        {
            FloatCompare(24, Units.Radians.Hours(2 * Mathf.PI));
        }

        [Test]
        public void grams_2_ounces()
        {
            FloatCompare(0.035273962f, Units.Grams.Ounces(1));
        }

        [Test]
        public void grams_2_pounds()
        {
            FloatCompare(0.0022046228f, Units.Grams.Pounds(1));
        }

        [Test]
        public void grams_2_kilograms()
        {
            FloatCompare(0.001f, Units.Grams.Kilograms(1));
        }

        [Test]
        public void grams_2_tons()
        {
            FloatCompare(0.0000011023113f, Units.Grams.Tons(1));
        }

        [Test]
        public void ounces_2_grams()
        {
            FloatCompare(28.349523f, Units.Ounces.Grams(1));
        }

        [Test]
        public void ounces_2_pounds()
        {
            FloatCompare(0.0625f, Units.Ounces.Pounds(1));
        }

        [Test]
        public void ounces_2_kilograms()
        {
            FloatCompare(0.028349523f, Units.Ounces.Kilograms(1));
        }

        [Test]
        public void ounces_2_tons()
        {
            FloatCompare(3.125e-5f, Units.Ounces.Tons(1));
        }

        [Test]
        public void pounds_2_grams()
        {
            FloatCompare(453.59233f, Units.Pounds.Grams(1));
        }

        [Test]
        public void pounds_2_ounces()
        {
            FloatCompare(16, Units.Pounds.Ounces(1));
        }

        [Test]
        public void pounds_2_kilograms()
        {
            FloatCompare(0.45359233f, Units.Pounds.Kilograms(1));
        }

        [Test]
        public void pounds_2_tons()
        {
            FloatCompare(0.0005f, Units.Pounds.Tons(1));
        }

        [Test]
        public void kilograms_2_grams()
        {
            FloatCompare(1000f, Units.Kilograms.Grams(1));
        }

        [Test]
        public void kilograms_2_ounces()
        {
            FloatCompare(35.273962f, Units.Kilograms.Ounces(1));
        }

        [Test]
        public void kilograms_2_pounds()
        {
            FloatCompare(2.2046228f, Units.Kilograms.Pounds(1));
        }

        [Test]
        public void kilograms_2_tons()
        {
            FloatCompare(0.0011023113f, Units.Kilograms.Tons(1));
        }

        [Test]
        public void tons_2_grams()
        {
            FloatCompare(907184.75f, Units.Tons.Grams(1));
        }

        [Test]
        public void tons_2_ounces()
        {
            FloatCompare(32000f, Units.Tons.Ounces(1));
        }

        [Test]
        public void tons_2_pounds()
        {
            FloatCompare(2000, Units.Tons.Pounds(1));
        }

        [Test]
        public void tons_2_kilograms()
        {
            FloatCompare(907.18475f, Units.Tons.Kilograms(1));
        }

        [Test]
        public void millimeters_2_centimeters()
        {
            FloatCompare(0.1f, Units.Millimeters.Centimeters(1));
        }

        [Test]
        public void millimeters_2_inches()
        {
            FloatCompare(0.039370079f, Units.Millimeters.Inches(1));
        }

        [Test]
        public void millimeters_2_feet()
        {
            FloatCompare(0.00328084f, Units.Millimeters.Feet(1));
        }

        [Test]
        public void millimeters_2_miles()
        {
            FloatCompare(0.00000062137119f, Units.Millimeters.Miles(1));
        }

        [Test]
        public void millimeters_2_kilometers()
        {
            FloatCompare(1e-6f, Units.Millimeters.Kilometers(1));
        }

        [Test]
        public void centimeters_2_inches()
        {
            FloatCompare(0.39370079f, Units.Centimeters.Inches(1));
        }

        [Test]
        public void centimeters_2_feet()
        {
            FloatCompare(0.0328084f, Units.Centimeters.Feet(1));
        }

        [Test]
        public void centimeters_2_miles()
        {
            FloatCompare(0.0000062137119f, Units.Centimeters.Miles(1));
        }

        [Test]
        public void centimeters_2_millimeters()
        {
            FloatCompare(10, Units.Centimeters.Millimeters(1));
        }

        [Test]
        public void centimeters_2_kilometers()
        {
            FloatCompare(1e-5f, Units.Centimeters.Kilometers(1));
        }

        [Test]
        public void inches_2_feet()
        {
            FloatCompare(1 / 12f, Units.Inches.Feet(1));
        }

        [Test]
        public void inches_2_miles()
        {
            FloatCompare(0.000015782828f, Units.Inches.Miles(1));
        }

        [Test]
        public void inches_2_centimeters()
        {
            FloatCompare(2.54f, Units.Inches.Centimeters(1));
        }

        [Test]
        public void inches_2_millimeters()
        {
            FloatCompare(25.4f, Units.Inches.Millimeters(1));
        }

        [Test]
        public void inches_2_meters()
        {
            FloatCompare(0.0254f, Units.Inches.Meters(1));
        }

        [Test]
        public void inches_2_kilometers()
        {
            FloatCompare(0.0000254f, Units.Inches.Kilometers(1));
        }

        [Test]
        public void feet_2_inches()
        {
            FloatCompare(12, Units.Feet.Inches(1));
        }

        [Test]
        public void feet_2_miles()
        {
            FloatCompare(0.00018939394f, Units.Feet.Miles(1));
        }

        [Test]
        public void feet_2_centimeters()
        {
            FloatCompare(30.48f, Units.Feet.Centimeters(1));
        }

        [Test]
        public void feet_2_millimeters()
        {
            FloatCompare(304.8f, Units.Feet.Millimeters(1));
        }

        [Test]
        public void feet_2_meters()
        {
            FloatCompare(0.3048f, Units.Feet.Meters(1));
        }

        [Test]
        public void feet_2_kilometers()
        {
            FloatCompare(0.0003048f, Units.Feet.Kilometers(1));
        }

        [Test]
        public void meters_2_inches()
        {
            FloatCompare(39.370079f, Units.Meters.Inches(1));
        }

        [Test]
        public void meters_2_feet()
        {
            FloatCompare(3.28084f, Units.Meters.Feet(1));
        }

        [Test]
        public void meters_2_miles()
        {
            FloatCompare(0.00062137119f, Units.Meters.Miles(1));
        }

        [Test]
        public void meters_2_centimeters()
        {
            FloatCompare(100f, Units.Meters.Centimeters(1));
        }

        [Test]
        public void meters_2_millimeters()
        {
            FloatCompare(1000f, Units.Meters.Millimeters(1));
        }

        [Test]
        public void meters_2_kilometers()
        {
            FloatCompare(0.001f, Units.Meters.Kilometers(1));
        }

        [Test]
        public void kilometers_2_inches()
        {
            FloatCompare(39370.079f, Units.Kilometers.Inches(1));
        }

        [Test]
        public void kilometers_2_feet()
        {
            FloatCompare(3280.84f, Units.Kilometers.Feet(1));
        }

        [Test]
        public void kilometers_2_miles()
        {
            FloatCompare(0.62137119f, Units.Kilometers.Miles(1));
        }

        [Test]
        public void kilometers_2_centimeters()
        {
            FloatCompare(100000f, Units.Kilometers.Centimeters(1));
        }

        [Test]
        public void kilometers_2_millimeters()
        {
            FloatCompare(1000000f, Units.Kilometers.Millimeters(1));
        }

        [Test]
        public void miles_2_inches()
        {
            FloatCompare(63360f, Units.Miles.Inches(1));
        }

        [Test]
        public void miles_2_feet()
        {
            FloatCompare(5280f, Units.Miles.Feet(1));
        }

        [Test]
        public void miles_2_centimeters()
        {
            FloatCompare(160934.40f, Units.Miles.Centimeters(1));
        }

        [Test]
        public void miles_2_millimeters()
        {
            FloatCompare(1609344.0f, Units.Miles.Millimeters(1));
        }

        [Test]
        public void miles_2_meters()
        {
            FloatCompare(1609.3440f, Units.Miles.Meters(1));
        }

        [Test]
        public void miles_2_kilometers()
        {
            FloatCompare(1.6093440f, Units.Miles.Kilometers(1));
        }

        [Test]
        public void milesperhour_2_kilometersperhour()
        {
            FloatCompare(1.6093440f, Units.MilesPerHour.KilometersPerHour(1));
        }

        [Test]
        public void milesperhour_2_meterspersecond()
        {
            FloatCompare(0.44704f, Units.MilesPerHour.MetersPerSecond(1));
        }

        [Test]
        public void milesperhour_2_feetpersecond()
        {
            FloatCompare(1.46666666666666666666666666666667f, Units.MilesPerHour.FeetPerSecond(1));
        }

        [Test]
        public void kilometersperhour_2_milesperhour()
        {
            FloatCompare(0.62137119f, Units.KilometersPerHour.MilesPerHour(1));
        }

        [Test]
        public void kilometersperhour_2_meterspersecond()
        {
            FloatCompare(0.27777777777777777777777777777777777778f, Units.KilometersPerHour.MetersPerSecond(1));
        }

        [Test]
        public void kilometersperhour_2_feetpersecond()
        {
            FloatCompare(0.91134442f, Units.KilometersPerHour.FeetPerSecond(1));
        }

        [Test]
        public void meterspersecond_2_milesperhour()
        {
            FloatCompare(2.2369363f, Units.MetersPerSecond.MilesPerHour(1));
        }

        [Test]
        public void meterspersecond_2_kilometersperhour()
        {
            FloatCompare(3.6f, Units.MetersPerSecond.KilometersPerHour(1));
        }

        [Test]
        public void meterspersecond_2_feetpersecond()
        {
            FloatCompare(3.28084f, Units.MetersPerSecond.FeetPerSecond(1));
        }

        [Test]
        public void feetpersecond_2_milesperhour()
        {
            FloatCompare(0.68181818f, Units.FeetPerSecond.MilesPerHour(1));
        }

        [Test]
        public void feetpersecond_2_kilometersperhour()
        {
            FloatCompare(1.09728f, Units.FeetPerSecond.KilometersPerHour(1));
        }

        [Test]
        public void feetpersecond_2_meterspersecond()
        {
            FloatCompare(0.3048f, Units.FeetPerSecond.MetersPerSecond(1));
        }

        [Test]
        public void pascals_2_hectopascals()
        {
            FloatCompare(0.01f, Units.Pascals.Hectopascals(1));
        }

        [Test]
        public void pascals_2_poundspersquareinch()
        {
            FloatCompare(0.00014503773800722f, Units.Pascals.PoundsPerSquareInch(1));
        }

        [Test]
        public void pascals_2_kilopascals()
        {
            FloatCompare(0.001f, Units.Pascals.Kilopascals(1));
        }

        [Test]
        public void hectopascals_2_pascals()
        {
            FloatCompare(100f, Units.Hectopascals.Pascals(1));
        }

        [Test]
        public void hectopascals_2_poundspersquareinch()
        {
            FloatCompare(0.014503773800722f, Units.Hectopascals.PoundsPerSquareInch(1));
        }

        [Test]
        public void hectopascals_2_kilopascals()
        {
            FloatCompare(0.1f, Units.Hectopascals.Kilopascals(1));
        }

        [Test]
        public void poundspersquareinch_2_pascals()
        {
            FloatCompare(6894.7572799999125f, Units.PoundsPerSquareInch.Pascals(1));
        }

        [Test]
        public void poundspersquareinch_2_hectopascals()
        {
            FloatCompare(68.947572799999125f, Units.PoundsPerSquareInch.Hectopascals(1));
        }

        [Test]
        public void poundspersquareinch_2_kilopascals()
        {
            FloatCompare(6.8947572799999125f, Units.PoundsPerSquareInch.Kilopascals(1));
        }

        [Test]
        public void kilopascals_2_hectopascals()
        {
            FloatCompare(10f, Units.Kilopascals.Hectopascals(1));
        }

        [Test]
        public void kilopascals_2_poundspersquareinch()
        {
            FloatCompare(0.14503773800722f, Units.Kilopascals.PoundsPerSquareInch(1));
        }

        [Test]
        public void kilopascals_2_pascals()
        {
            FloatCompare(1000f, Units.Kilopascals.Pascals(1));
        }

        [Test]
        public void FormatMeters()
        {
            Assert.AreEqual(Mathf.PI.Label(UnitOfMeasure.Meters, 3), "3.14 m");
        }

        private const int SIGFIGS = 9;

        private static string SigFig(float value, int sigfigs)
        {
            var q = (int)Mathf.Log10(value) + 1;
            sigfigs -= q;
            var p = Mathf.Pow(10, sigfigs);
            var v = (Mathf.Round(value * p) / p).ToString();
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

        private static void FloatCompare(float expected, float actual, string msg = null, params object[] args)
        {
            Assert.AreEqual(SigFig(expected, SIGFIGS), SigFig(actual, SIGFIGS), msg, args);
        }

        //// A UnityTest behaves like a coroutine in PlayMode
        //// and allows you to yield null to skip a frame in EditMode
        //[UnityTest]
        //public IEnumerator test_UnitConversionsWithEnumeratorPasses() {
        //	// Use the Assert class to test conditions.
        //	// yield to skip a frame
        //	yield return null;
        //}
    }
}
