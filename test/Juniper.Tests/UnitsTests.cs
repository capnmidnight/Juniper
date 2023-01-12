using Juniper.World.GIS;

using NUnit.Framework;

using System.Globalization;

using static System.Math;

namespace Juniper.Units
{
    [TestFixture]
    public class UnitsTests
    {
        [Test]
        public void LatLng_2_UTM()
        {
            var latLng = new LatLngPoint(38.8974146, -77.0743107);
            var utm = latLng.ToUTM();
            var latLng2 = utm.ToLatLng();
            var utm2 = latLng2.ToUTM();
            Assert.AreEqual(latLng.Alt, latLng2.Alt, 0.0001);
            Assert.AreEqual(latLng.Lng, latLng2.Lng, 0.0001);
            Assert.AreEqual(latLng.Lat, latLng2.Lat, 0.0001);
            Assert.AreEqual(utm.Easting, utm2.Easting, 0.5);
            Assert.AreEqual(utm.Northing, utm2.Northing, 0.5);
            Assert.AreEqual(utm.Altitude, utm2.Altitude, 0.0001);
            Assert.AreEqual(utm.Zone, utm2.Zone);
        }

        [Test]
        public void Farenheit_2_celsius()
        {
            NumberCompare(0, Farenheit.Celsius(32), 4);
            NumberCompare(100, Farenheit.Celsius(212), 4);
        }

        [Test]
        public void Farenheit_2_kelvin()
        {
            NumberCompare(273.15, Farenheit.Kelvin(32));
            NumberCompare(373.15, Farenheit.Kelvin(212));
        }

        [Test]
        public void Celsius_2_farenheit()
        {
            NumberCompare(32, Celsius.Farenheit(0));
            NumberCompare(212, Celsius.Farenheit(100));
        }

        [Test]
        public void Celsius_2_kelvin()
        {
            NumberCompare(273.15, Celsius.Kelvin(0));
            NumberCompare(373.15, Celsius.Kelvin(100));
        }

        [Test]
        public void Kelvin_2_farenheit()
        {
            NumberCompare(32, Kelvin.Farenheit(273.15));
            NumberCompare(212, Kelvin.Farenheit(373.15));
        }

        [Test]
        public void Kelvin_2_celsius()
        {
            NumberCompare(0, Kelvin.Celsius(273.15));
            NumberCompare(100, Kelvin.Celsius(373.15));
        }

        [Test]
        public void Degrees_2_radians()
        {
            NumberCompare(TAU, Degrees.Radians(360));
        }

        [Test]
        public void Degrees_2_hours()
        {
            NumberCompare(24, Degrees.Hours(360), 3);
        }

        [Test]
        public void Hours_2_degrees()
        {
            NumberCompare(360, Hours.Degrees(24));
        }

        [Test]
        public void Hours_2_radians()
        {
            NumberCompare(TAU, Hours.Radians(24));
        }

        [Test]
        public void Radians_2_degrees()
        {
            NumberCompare(360, Radians.Degrees(TAU));
        }

        [Test]
        public void Radians_2_hours()
        {
            NumberCompare(24, Radians.Hours(TAU));
        }

        [Test]
        public void Grams_2_ounces()
        {
            NumberCompare(0.035273962, Grams.Ounces(1));
        }

        [Test]
        public void Grams_2_pounds()
        {
            NumberCompare(0.0022046228, Grams.Pounds(1));
        }

        [Test]
        public void Grams_2_kilograms()
        {
            NumberCompare(0.001, Grams.Kilograms(1));
        }

        [Test]
        public void Grams_2_tons()
        {
            NumberCompare(0.0000011023113, Grams.Tons(1));
        }

        [Test]
        public void Ounces_2_grams()
        {
            NumberCompare(28.349523, Ounces.Grams(1));
        }

        [Test]
        public void Ounces_2_pounds()
        {
            NumberCompare(0.0625, Ounces.Pounds(1));
        }

        [Test]
        public void Ounces_2_kilograms()
        {
            NumberCompare(0.028349523, Ounces.Kilograms(1));
        }

        [Test]
        public void Ounces_2_tons()
        {
            NumberCompare(3.125e-5, Ounces.Tons(1));
        }

        [Test]
        public void Pounds_2_grams()
        {
            NumberCompare(453.59233, Pounds.Grams(1));
        }

        [Test]
        public void Pounds_2_ounces()
        {
            NumberCompare(16, Pounds.Ounces(1));
        }

        [Test]
        public void Pounds_2_kilograms()
        {
            NumberCompare(0.45359237, Pounds.Kilograms(1));
        }

        [Test]
        public void Pounds_2_tons()
        {
            NumberCompare(0.0005, Pounds.Tons(1));
        }

        [Test]
        public void Kilograms_2_grams()
        {
            NumberCompare(1000, Kilograms.Grams(1));
        }

        [Test]
        public void Kilograms_2_ounces()
        {
            NumberCompare(35.2739621, Kilograms.Ounces(1));
        }

        [Test]
        public void Kilograms_2_pounds()
        {
            NumberCompare(2.20462262, Kilograms.Pounds(1));
        }

        [Test]
        public void Kilograms_2_tons()
        {
            NumberCompare(0.0011023113, Kilograms.Tons(1));
        }

        [Test]
        public void Tons_2_grams()
        {
            NumberCompare(907184.75, Tons.Grams(1));
        }

        [Test]
        public void Tons_2_ounces()
        {
            NumberCompare(32000, Tons.Ounces(1));
        }

        [Test]
        public void Tons_2_pounds()
        {
            NumberCompare(2000, Tons.Pounds(1));
        }

        [Test]
        public void Tons_2_kilograms()
        {
            NumberCompare(907.18475, Tons.Kilograms(1));
        }

        [Test]
        public void Millimeters_2_centimeters()
        {
            NumberCompare(0.1, Millimeters.Centimeters(1));
        }

        [Test]
        public void Millimeters_2_inches()
        {
            NumberCompare(0.039370079, Millimeters.Inches(1));
        }

        [Test]
        public void Millimeters_2_feet()
        {
            NumberCompare(0.00328083989501312335958005249344, Millimeters.Feet(1));
        }

        [Test]
        public void Millimeters_2_miles()
        {
            NumberCompare(0.00000062137119, Millimeters.Miles(1));
        }

        [Test]
        public void Millimeters_2_kilometers()
        {
            NumberCompare(1e-6, Millimeters.Kilometers(1));
        }

        [Test]
        public void Centimeters_2_inches()
        {
            NumberCompare(0.39370079, Centimeters.Inches(1));
        }

        [Test]
        public void Centimeters_2_feet()
        {
            NumberCompare(0.03280839895013123359580052493438, Centimeters.Feet(1));
        }

        [Test]
        public void Centimeters_2_miles()
        {
            NumberCompare(6.21371e-6, Centimeters.Miles(1), 6);
        }

        [Test]
        public void Centimeters_2_millimeters()
        {
            NumberCompare(10, Centimeters.Millimeters(1));
        }

        [Test]
        public void Centimeters_2_kilometers()
        {
            NumberCompare(1e-5, Centimeters.Kilometers(1));
        }

        [Test]
        public void Inches_2_feet()
        {
            NumberCompare(1 / 12.0, Inches.Feet(1));
        }

        [Test]
        public void Inches_2_miles()
        {
            NumberCompare(0.000015782828, Inches.Miles(1), 8);
        }

        [Test]
        public void Inches_2_centimeters()
        {
            NumberCompare(2.54, Inches.Centimeters(1));
        }

        [Test]
        public void Inches_2_millimeters()
        {
            NumberCompare(25.4, Inches.Millimeters(1));
        }

        [Test]
        public void Inches_2_meters()
        {
            NumberCompare(0.0254, Inches.Meters(1));
        }

        [Test]
        public void Inches_2_kilometers()
        {
            NumberCompare(0.0000254, Inches.Kilometers(1));
        }

        [Test]
        public void Feet_2_inches()
        {
            NumberCompare(12, Feet.Inches(1));
        }

        [Test]
        public void Feet_2_miles()
        {
            NumberCompare(0.00018939394, Feet.Miles(1));
        }

        [Test]
        public void Feet_2_centimeters()
        {
            NumberCompare(30.48, Feet.Centimeters(1));
        }

        [Test]
        public void Feet_2_millimeters()
        {
            NumberCompare(304.8, Feet.Millimeters(1));
        }

        [Test]
        public void Feet_2_meters()
        {
            NumberCompare(0.3048, Feet.Meters(1));
        }

        [Test]
        public void Feet_2_kilometers()
        {
            NumberCompare(0.0003048, Feet.Kilometers(1));
        }

        [Test]
        public void Meters_2_inches()
        {
            NumberCompare(39.37007874015748031496062992126, Meters.Inches(1));
        }

        [Test]
        public void Meters_2_feet()
        {
            NumberCompare(3.2808398950131233595800524934383, Meters.Feet(1));
        }

        [Test]
        public void Meters_2_miles()
        {
            NumberCompare(0.00062137119, Meters.Miles(1));
        }

        [Test]
        public void Meters_2_centimeters()
        {
            NumberCompare(100, Meters.Centimeters(1));
        }

        [Test]
        public void Meters_2_millimeters()
        {
            NumberCompare(1000, Meters.Millimeters(1));
        }

        [Test]
        public void Meters_2_kilometers()
        {
            NumberCompare(0.001, Meters.Kilometers(1));
        }

        [Test]
        public void Kilometers_2_inches()
        {
            NumberCompare(39370.07874015748031496062992126, Kilometers.Inches(1));
        }

        [Test]
        public void Kilometers_2_feet()
        {
            NumberCompare(3280.84, Kilometers.Feet(1), 6);
        }

        [Test]
        public void Kilometers_2_miles()
        {
            NumberCompare(0.62137119, Kilometers.Miles(1));
        }

        [Test]
        public void Kilometers_2_centimeters()
        {
            NumberCompare(100000, Kilometers.Centimeters(1));
        }

        [Test]
        public void Kilometers_2_millimeters()
        {
            NumberCompare(1000000, Kilometers.Millimeters(1));
        }

        [Test]
        public void Miles_2_inches()
        {
            NumberCompare(63360, Miles.Inches(1));
        }

        [Test]
        public void Miles_2_feet()
        {
            NumberCompare(5280, Miles.Feet(1));
        }

        [Test]
        public void Miles_2_centimeters()
        {
            NumberCompare(160934.4, Miles.Centimeters(1));
        }

        [Test]
        public void Miles_2_millimeters()
        {
            NumberCompare(1609344, Miles.Millimeters(1));
        }

        [Test]
        public void Miles_2_meters()
        {
            NumberCompare(1609.3440, Miles.Meters(1));
        }

        [Test]
        public void Miles_2_kilometers()
        {
            NumberCompare(1.6093440, Miles.Kilometers(1));
        }

        [Test]
        public void Milesperhour_2_kilometersperhour()
        {
            NumberCompare(1.6093440, MilesPerHour.KilometersPerHour(1));
        }

        [Test]
        public void Milesperhour_2_meterspersecond()
        {
            NumberCompare(0.44704, MilesPerHour.MetersPerSecond(1));
        }

        [Test]
        public void Milesperhour_2_feetpersecond()
        {
            NumberCompare(1.46666666666666666666666666666667, MilesPerHour.FeetPerSecond(1));
        }

        [Test]
        public void Kilometersperhour_2_milesperhour()
        {
            NumberCompare(0.62137119, KilometersPerHour.MilesPerHour(1));
        }

        [Test]
        public void Kilometersperhour_2_meterspersecond()
        {
            NumberCompare(0.27777777777777777777777777777777777778, KilometersPerHour.MetersPerSecond(1));
        }

        [Test]
        public void Kilometersperhour_2_feetpersecond()
        {
            NumberCompare(0.91134442, KilometersPerHour.FeetPerSecond(1));
        }

        [Test]
        public void Meterspersecond_2_milesperhour()
        {
            NumberCompare(2.2369362920544022906227630637079, MetersPerSecond.MilesPerHour(1));
        }

        [Test]
        public void Meterspersecond_2_kilometersperhour()
        {
            NumberCompare(3.6, MetersPerSecond.KilometersPerHour(1), 5);
        }

        [Test]
        public void Meterspersecond_2_feetpersecond()
        {
            NumberCompare(3.2808398950131233595800524934383, MetersPerSecond.FeetPerSecond(1));
        }

        [Test]
        public void Feetpersecond_2_milesperhour()
        {
            NumberCompare(0.68181818, FeetPerSecond.MilesPerHour(1));
        }

        [Test]
        public void Feetpersecond_2_kilometersperhour()
        {
            NumberCompare(1.09728, FeetPerSecond.KilometersPerHour(1));
        }

        [Test]
        public void Feetpersecond_2_meterspersecond()
        {
            NumberCompare(0.3048, FeetPerSecond.MetersPerSecond(1));
        }

        [Test]
        public void Pascals_2_hectopascals()
        {
            NumberCompare(0.01, Pascals.Hectopascals(1));
        }

        [Test]
        public void Pascals_2_poundspersquareinch()
        {
            NumberCompare(0.00014503773800722, Pascals.PoundsPerSquareInch(1));
        }

        [Test]
        public void Pascals_2_kilopascals()
        {
            NumberCompare(0.001, Pascals.Kilopascals(1));
        }

        [Test]
        public void Hectopascals_2_pascals()
        {
            NumberCompare(100, Hectopascals.Pascals(1));
        }

        [Test]
        public void Hectopascals_2_poundspersquareinch()
        {
            NumberCompare(0.014503773800722, Hectopascals.PoundsPerSquareInch(1));
        }

        [Test]
        public void Hectopascals_2_kilopascals()
        {
            NumberCompare(0.1, Hectopascals.Kilopascals(1));
        }

        [Test]
        public void Poundspersquareinch_2_pascals()
        {
            NumberCompare(6894.7572799999125, PoundsPerSquareInch.Pascals(1));
        }

        [Test]
        public void Poundspersquareinch_2_hectopascals()
        {
            NumberCompare(68.947572799999125, PoundsPerSquareInch.Hectopascals(1));
        }

        [Test]
        public void Poundspersquareinch_2_kilopascals()
        {
            NumberCompare(6.8947572799999125, PoundsPerSquareInch.Kilopascals(1));
        }

        [Test]
        public void Kilopascals_2_hectopascals()
        {
            NumberCompare(10, Kilopascals.Hectopascals(1));
        }

        [Test]
        public void Kilopascals_2_poundspersquareinch()
        {
            NumberCompare(0.14503773800722, Kilopascals.PoundsPerSquareInch(1));
        }

        [Test]
        public void Kilopascals_2_pascals()
        {
            NumberCompare(1000, Kilopascals.Pascals(1));
        }

        [Test]
        public void FormatMeters()
        {
            Assert.AreEqual(PI.Label(UnitOfMeasure.Meters, 3), "3.14 m");
        }

        private const double PI = System.Math.PI;
        private const double TAU = 2 * PI;

        private static string SigFig(float value, int sigfigs)
        {
            var q = (int)Log10(value) + 1;
            sigfigs -= q;
            var p = Pow(10, sigfigs);
            var v = (Round(value * p) / p).ToString(CultureInfo.InvariantCulture);
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
        private static string SigFig(double value, int sigfigs)
        {
            var q = (int)Log10(value) + 1;
            sigfigs -= q;
            var p = Pow(10, sigfigs);
            var v = (Round(value * p) / p).ToString(CultureInfo.InvariantCulture);
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

        private static void NumberCompare(float expected, float actual, int sigFigs = 9, string msg = null, params object[] args)
        {
            Assert.AreEqual(SigFig(expected, sigFigs), SigFig(actual, sigFigs), msg, args);
        }

        private static void NumberCompare(double expected, double actual, int sigFigs = 9, string msg = null, params object[] args)
        {
            Assert.AreEqual(SigFig(expected, sigFigs), SigFig(actual, sigFigs), msg, args);
        }
    }
}