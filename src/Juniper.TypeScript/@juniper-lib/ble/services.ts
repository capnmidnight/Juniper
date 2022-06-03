import { makeLookup } from "./makeLookup";

/*
  In Bluetooth GATT, Services are collections of Characteristics. The collection
  of Characteristics into Services is intended to make certain combinations of
  basic devices into larger, aggregated devices easier and faster.

  The following lookup table provides all of the standard, registered Services
  as of September 1st, 2017.

  Theoretically, Services have a required set of Characteristics, though I've
  never seen this enforced. A lot of apps assume any particular Service will only
  ever contain one of any type of Characteristic, but this is not expressed in the
  spec and only comes about from folks who don't understand how Descriptors work
  to differentiate between multiple Characteristics of the same type. Services in
  practice are nothing but an arbitrary ID coupled with an arbitrary set of
  Characteristics. This makes it possible for vendors to make up their own
  Services whenever they want. Just be sure to stick to the higher value ranges so
  you don't step on anyone else's toes.
*/

export const Services = makeLookup({
    "00001811": "Alert Notification Service",
    "00001815": "Automation IO",
    "0000180F": "Battery Service",
    "00001810": "Blood Pressure",
    "0000181B": "Body Composition",
    "0000181E": "Bond Management",
    "0000181F": "Continuous Glucose Monitoring",
    "00001805": "Current Time Service",
    "00001818": "Cycling Power",
    "00001816": "Cycling Speed and Cadence",
    "0000180A": "Device Information",
    "0000181A": "Environmental Sensing",
    "00001800": "Generic Access",
    "00001801": "Generic Attribute",
    "00001808": "Glucose",
    "00001809": "Health Thermometer",
    "0000180D": "Heart Rate",
    "00001823": "HTTP Proxy",
    "00001812": "Human Interface Device",
    "00001802": "Immediate Alert",
    "00001821": "Indoor Positioning",
    "00001820": "Internet Protocol Support",
    "00001803": "Link Loss",
    "00001819": "Location and Navigation",
    "00001807": "Next DST Change Service",
    "00001825": "Object Transfer",
    "0000180E": "Phone Alert Status Service",
    "00001822": "Pulse Oximeter",
    "00001806": "Reference Time Update Service",
    "00001814": "Running Speed and Cadence",
    "00001813": "Scan Parameters",
    "00001824": "Transport Discovery",
    "00001804": "Tx Power",
    "0000181C": "User Data",
    "0000181D": "Weight Scale",
    "00002000": "RFID Scanner"
});
