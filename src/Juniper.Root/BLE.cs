namespace Juniper
{
    public static class BLE
    {
        /*
  In Bluetooth GATT, Services are collections of Characteristics. The collection
  of Characteristics into Services is intended to make certain combinations of
  basic devices into larger, aggregated devices easier and faster.
  The following lookup table provides all of the standard, registered Services
  as of September 1st, 2017.
  Theoretically, Services have a required set of Characteristics, though I've
  never seen this enforced. A lot of apps assume any particular Service will only
  ever contain one of any type of Characteristic, but this is not expressed in
  the spec and only comes about from folks who don't understand how Descriptors
  work to differentiate between multiple Characteristics of the same type. Services
  in practice are nothing but an arbitrary ID coupled with an arbitrary set of 
  Characteristics. This makes it possible for vendors to make up their own Services 
  whenever they want. Just be sure to stick to the higher value ranges so you don't 
  step on anyone else's toes.
*/

        public static readonly Dictionary<string, string> services = new()
        {
            ["00001811"] = "Alert Notification Service",
            ["00001815"] = "Automation IO",
            ["0000180F"] = "Battery Service",
            ["00001810"] = "Blood Pressure",
            ["0000181B"] = "Body Composition",
            ["0000181E"] = "Bond Management",
            ["0000181F"] = "Continuous Glucose Monitoring",
            ["00001805"] = "Current Time Service",
            ["00001818"] = "Cycling Power",
            ["00001816"] = "Cycling Speed and Cadence",
            ["0000180A"] = "Device Information",
            ["0000181A"] = "Environmental Sensing",
            ["00001800"] = "Generic Access",
            ["00001801"] = "Generic Attribute",
            ["00001808"] = "Glucose",
            ["00001809"] = "Health Thermometer",
            ["0000180D"] = "Heart Rate",
            ["00001823"] = "HTTP Proxy",
            ["00001812"] = "Human Interface Device",
            ["00001802"] = "Immediate Alert",
            ["00001821"] = "Indoor Positioning",
            ["00001820"] = "Internet Protocol Support",
            ["00001803"] = "Link Loss",
            ["00001819"] = "Location and Navigation",
            ["00001807"] = "Next DST Change Service",
            ["00001825"] = "Object Transfer",
            ["0000180E"] = "Phone Alert Status Service",
            ["00001822"] = "Pulse Oximeter",
            ["00001806"] = "Reference Time Update Service",
            ["00001814"] = "Running Speed and Cadence",
            ["00001813"] = "Scan Parameters",
            ["00001824"] = "Transport Discovery",
            ["00001804"] = "Tx Power",
            ["0000181C"] = "User Data",
            ["0000181D"] = "Weight Scale",
            ["00002000"] = "RFID Scanner"
        };

        public static readonly Dictionary<string, string> declarations = new()
        {
            ["00002803"] = "Characteristic Declaration",
            ["00002802"] = "Include",
            ["00002800"] = "Primary Service",
            ["00002801"] = "Secondary Service"
        };

        /*
          In Bluetooth GATT, Characteristics are key-value pairs, collected into "Services",
          that represent the raw functionality of your device. Characteristics support
          both reading from and writing to, as well as notification events when values
          change.
          The following lookup table provides all of the standard, registered Characteristics
          as of September 1st, 2017.
          The supported data types are very primitive:
            - Strings of up to 20 bytes in length, and strictly in ASCII encoding
            - Integers in 1, 2, or 4 byte lengths
            - Floats in 4 or 8 byte lengths
            - "Auto", which is basically just a String
          Characteristics can also have Descriptors, which provide more metadata about
          the value, e.g. friendly, readable descriptions when more than one "ANALOG"
          type characteristic is included in a Service.
          Theoretically, Services have a required set of Characteristics, though I've
          never seen this enforced. Characteristics are supposed to also have a proscribed
          data type and length, but this is not enforced by the Bluetooth stack, either,
          and is really only an aid to consuming application. A lot of apps assume any
          particular Service will only ever contain one of any type of Characteristic,
          but this is not expressed in the spec and only comes about from folks who don't
          understand how Descriptors work to differentiate between multiple Characteristics
          of the same type. Characteristics in practice are nothing but an arbitrary ID
          coupled with an arbitrary set of metadata and an arbitrary value. This makes it
          possible for vendors to make up their own Characteristics whenever they want.
          Just be sure to stick to the higher value ranges so you don't step on anyone
          else's toes.
        */

        public static readonly Dictionary<string, string> characteristics = new()
        {
            ["00002A7E"] = "Aerobic Heart Rate Lower Limit",
            ["00002A84"] = "Aerobic Heart Rate Upper Limit",
            ["00002A7F"] = "Aerobic Threshold",
            ["00002A80"] = "Age",
            ["00002A5A"] = "Aggregate",
            ["00002A43"] = "Alert Category ID",
            ["00002A42"] = "Alert Category ID Bit Mask",
            ["00002A06"] = "Alert Level",
            ["00002A44"] = "Alert Notification Control Point",
            ["00002A3F"] = "Alert Status",
            ["00002AB3"] = "Altitude",
            ["00002A81"] = "Anaerobic Heart Rate Lower Limit",
            ["00002A82"] = "Anaerobic Heart Rate Upper Limit",
            ["00002A83"] = "Anaerobic Threshold",
            ["00002A58"] = "Analog",
            ["00002A59"] = "Analog Output",
            ["00002A73"] = "Apparent Wind Direction",
            ["00002A72"] = "Apparent Wind Speed",
            ["00002A01"] = "Appearance",
            ["00002AA3"] = "Barometric Pressure Trend",
            ["00002A19"] = "Battery Level",
            ["00002A1B"] = "Battery Level State",
            ["00002A1A"] = "Battery Power State",
            ["00002A49"] = "Blood Pressure Feature",
            ["00002A35"] = "Blood Pressure Measurement",
            ["00002A9B"] = "Body Composition Feature",
            ["00002A9C"] = "Body Composition Measurement",
            ["00002A38"] = "Body Sensor Location",
            ["00002AA4"] = "Bond Management Control Point",
            ["00002AA5"] = "Bond Management Features",
            ["00002A22"] = "Boot Keyboard Input Report",
            ["00002A32"] = "Boot Keyboard Output Report",
            ["00002A33"] = "Boot Mouse Input Report",
            ["00002AA6"] = "Central Address Resolution",
            ["00002AA8"] = "CGM Feature",
            ["00002AA7"] = "CGM Measurement",
            ["00002AAB"] = "CGM Session Run Time",
            ["00002AAA"] = "CGM Session Start Time",
            ["00002AAC"] = "CGM Specific Ops Control Point",
            ["00002AA9"] = "CGM Status",
            ["00002ACE"] = "Cross Trainer Data",
            ["00002A5C"] = "CSC Feature",
            ["00002A5B"] = "CSC Measurement",
            ["00002A2B"] = "Current Time",
            ["00002A66"] = "Cycling Power Control Point",
            ["00002A65"] = "Cycling Power Feature",
            ["00002A63"] = "Cycling Power Measurement",
            ["00002A64"] = "Cycling Power Vector",
            ["00002A99"] = "Database Change Increment",
            ["00002A85"] = "Date of Birth",
            ["00002A86"] = "Date of Threshold Assessment",
            ["00002A08"] = "Date Time",
            ["00002A0A"] = "Day Date Time",
            ["00002A09"] = "Day of Week",
            ["00002A7D"] = "Descriptor Value Changed",
            ["00002A00"] = "Device Name",
            ["00002A7B"] = "Dew Point",
            ["00002A56"] = "Digital",
            ["00002A57"] = "Digital Output",
            ["00002A0D"] = "DST Offset",
            ["00002A6C"] = "Elevation",
            ["00002A87"] = "Email Address",
            ["00002A0B"] = "Exact Time 100",
            ["00002A0C"] = "Exact Time 256",
            ["00002A88"] = "Fat Burn Heart Rate Lower Limit",
            ["00002A89"] = "Fat Burn Heart Rate Upper Limit",
            ["00002A26"] = "Firmware Revision String",
            ["00002A8A"] = "First Name",
            ["00002AD9"] = "Fitness Machine Control Point",
            ["00002ACC"] = "Fitness Machine Feature",
            ["00002ADA"] = "Fitness Machine Status",
            ["00002A8B"] = "Five Zone Heart Rate Limits",
            ["00002AB2"] = "Floor Number",
            ["00002A8C"] = "Gender",
            ["00002A51"] = "Glucose Feature",
            ["00002A18"] = "Glucose Measurement",
            ["00002A34"] = "Glucose Measurement Context",
            ["00002A74"] = "Gust Factor",
            ["00002A27"] = "Hardware Revision String",
            ["00002A39"] = "Heart Rate Control Point",
            ["00002A8D"] = "Heart Rate Max",
            ["00002A37"] = "Heart Rate Measurement",
            ["00002A7A"] = "Heat Index",
            ["00002A8E"] = "Height",
            ["00002A4C"] = "HID Control Point",
            ["00002A4A"] = "HID Information",
            ["00002A8F"] = "Hip Circumference",
            ["00002ABA"] = "HTTP Control Point",
            ["00002AB9"] = "HTTP Entity Body",
            ["00002AB7"] = "HTTP Headers",
            ["00002AB8"] = "HTTP Status Code",
            ["00002ABB"] = "HTTPS Security",
            ["00002A6F"] = "Humidity",
            ["00002A2A"] = "IEEE 11073-20601 Regulatory Certification Data List",
            ["00002AD2"] = "Indoor Bike Data",
            ["00002AAD"] = "Indoor Positioning Configuration",
            ["00002A36"] = "Intermediate Cuff Pressure",
            ["00002A1E"] = "Intermediate Temperature",
            ["00002A77"] = "Irradiance",
            ["00002AA2"] = "Language",
            ["00002A90"] = "Last Name",
            ["00002AAE"] = "Latitude",
            ["00002A6B"] = "LN Control Point",
            ["00002A6A"] = "LN Feature",
            ["00002AB1"] = "Local East Coordinate",
            ["00002AB0"] = "Local North Coordinate",
            ["00002A0F"] = "Local Time Information",
            ["00002A67"] = "Location and Speed Characteristic",
            ["00002AB5"] = "Location Name",
            ["00002AAF"] = "Longitude",
            ["00002A2C"] = "Magnetic Declination",
            ["00002AA0"] = "Magnetic Flux Density - 2D",
            ["00002AA1"] = "Magnetic Flux Density - 3D",
            ["00002A29"] = "Manufacturer Name String",
            ["00002A91"] = "Maximum Recommended Heart Rate",
            ["00002A21"] = "Measurement Interval",
            ["00002A24"] = "Model Number String",
            ["00002A68"] = "Navigation",
            ["00002A3E"] = "Network Availability",
            ["00002A46"] = "New Alert",
            ["00002AC5"] = "Object Action Control Point",
            ["00002AC8"] = "Object Changed",
            ["00002AC1"] = "Object First-Created",
            ["00002AC3"] = "Object ID",
            ["00002AC2"] = "Object Last-Modified",
            ["00002AC6"] = "Object List Control Point",
            ["00002AC7"] = "Object List Filter",
            ["00002ABE"] = "Object Name",
            ["00002AC4"] = "Object Properties",
            ["00002AC0"] = "Object Size",
            ["00002ABF"] = "Object Type",
            ["00002ABD"] = "OTS Feature",
            ["00002A04"] = "Peripheral Preferred Connection Parameters",
            ["00002A02"] = "Peripheral Privacy Flag",
            ["00002A5F"] = "PLX Continuous Measurement Characteristic",
            //["00002A60"] = "PLX Features",
            ["00002A5E"] = "PLX Spot-Check Measurement",
            ["00002A50"] = "PnP ID",
            ["00002A75"] = "Pollen Concentration",
            ["00002A2F"] = "Position 2D",
            ["00002A30"] = "Position 3D",
            ["00002A69"] = "Position Quality",
            ["00002A6D"] = "Pressure",
            ["00002A4E"] = "Protocol Mode",
            ["00002A62"] = "Pulse Oximetry Control Point",
            ["00002A60"] = "Pulse Oximetry Pulsatile Event Characteristic",
            ["00002A78"] = "Rainfall",
            ["00002A03"] = "Reconnection Address",
            ["00002A52"] = "Record Access Control Point",
            ["00002A14"] = "Reference Time Information",
            ["00002A3A"] = "Removable",
            ["00002A4D"] = "Report",
            ["00002A4B"] = "Report Map",
            ["00002AC9"] = "Resolvable Private Address Only",
            ["00002A92"] = "Resting Heart Rate",
            ["00002A40"] = "Ringer Control point",
            ["00002A41"] = "Ringer Setting",
            ["00002AD1"] = "Rower Data",
            ["00002A54"] = "RSC Feature",
            ["00002A53"] = "RSC Measurement",
            ["00002A55"] = "SC Control Point",
            ["00002A4F"] = "Scan Interval Window",
            ["00002A31"] = "Scan Refresh",
            ["00002A3C"] = "Scientific Temperature Celsius",
            ["00002A10"] = "Secondary Time Zone",
            ["00002A5D"] = "Sensor Location",
            ["00002A25"] = "Serial Number String",
            ["00002A05"] = "Service Changed",
            ["00002A3B"] = "Service Required",
            ["00002A28"] = "Software Revision String",
            ["00002A93"] = "Sport Type for Aerobic and Anaerobic Thresholds",
            ["00002AD0"] = "Stair Climber Data",
            ["00002ACF"] = "Step Climber Data",
            ["00002A3D"] = "String",
            ["00002AD7"] = "Supported Heart Rate Range",
            ["00002AD5"] = "Supported Inclination Range",
            ["00002A47"] = "Supported New Alert Category",
            ["00002AD8"] = "Supported Power Range",
            ["00002AD6"] = "Supported Resistance Level Range",
            ["00002AD4"] = "Supported Speed Range",
            ["00002A48"] = "Supported Unread Alert Category",
            ["00002A23"] = "System ID",
            ["00002ABC"] = "TDS Control Point",
            ["00002A6E"] = "Temperature",
            ["00002A1F"] = "Temperature Celsius",
            ["00002A20"] = "Temperature Fahrenheit",
            ["00002A1C"] = "Temperature Measurement",
            ["00002A1D"] = "Temperature Type",
            ["00002A94"] = "Three Zone Heart Rate Limits",
            ["00002A12"] = "Time Accuracy",
            ["00002A15"] = "Time Broadcast",
            ["00002A13"] = "Time Source",
            ["00002A16"] = "Time Update Control Point",
            ["00002A17"] = "Time Update State",
            ["00002A11"] = "Time with DST",
            ["00002A0E"] = "Time Zone",
            ["00002AD3"] = "Training Status",
            ["00002ACD"] = "Treadmill Data",
            ["00002A71"] = "True Wind Direction",
            ["00002A70"] = "True Wind Speed",
            ["00002A95"] = "Two Zone Heart Rate Limit",
            ["00002A07"] = "Tx Power Level",
            ["00002AB4"] = "Uncertainty",
            ["00002A45"] = "Unread Alert Status",
            ["00002AB6"] = "URI",
            ["00002A9F"] = "User Control Point",
            ["00002A9A"] = "User Index",
            ["00002A76"] = "UV Index",
            ["00002A96"] = "VO2 Max",
            ["00002A97"] = "Waist Circumference",
            ["00002A98"] = "Weight",
            ["00002A9D"] = "Weight Measurement",
            ["00002A9E"] = "Weight Scale Feature",
            ["00002A79"] = "Wind Chill",
        };

        /*
          In Bluetooth GATT, Descriptors are key-value pairs, stored on a "Characteristic",
          that provide metadata about how the Characteristic should work. They are technically
          Characteristics themselves, so they use many of the same functions.
          The following lookup table provides all of the standard, registered Descriptors
          as of September 1st, 2017.
          The supported data types are very primitive:
            - Strings of up to 20 bytes in length, and strictly in ASCII encoding
            - Integers in 1, 2, or 4 byte lengths
            - Floats in 4 or 8 byte lengths
            - "Auto", which is basically just a String
          Theoretically, Descriptors are supposed to also have a proscribed data type and
          length, but this is not enforced by the Bluetooth stack, and is really only an
          aid to consuming application. A lot of apps ignore Descriptors entirely, but
          that is a big mistake. Descriptors in practice are nothing but an arbitrary ID
          coupled with an arbitrary an arbitrary value. This makes it possible for vendors
          to make up their own Descriptors whenever they want. Just be sure to stick to
          the higher value ranges so you don't step on anyone else's toes.
        */

        public static readonly Dictionary<string, string> descriptors = new()
        {
            ["00002905"] = "Characteristic Aggregate Format",
            ["00002900"] = "Characteristic Extended Properties",
            ["00002904"] = "Characteristic Presentation Format",
            ["00002901"] = "Characteristic User Description",
            ["00002902"] = "Client Characteristic Configuration",
            ["0000290B"] = "Environmental Sensing Configuration",
            ["0000290C"] = "Environmental Sensing Measurement",
            ["0000290D"] = "Environmental Sensing Trigger Setting",
            ["00002907"] = "External Report Reference",
            ["00002909"] = "Number of Digitals",
            ["00002908"] = "Report Reference",
            ["00002903"] = "Server Characteristic Configuration",
            ["0000290E"] = "Time Trigger Setting",
            ["00002906"] = "Valid Range",
            ["0000290A"] = "Value Trigger Setting"
        };
    }
}
