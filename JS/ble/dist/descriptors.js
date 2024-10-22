import { makeLookup } from "./makeLookup";
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
export const Descriptors = /*@__PURE__*/ makeLookup({
    "00002905": "Characteristic Aggregate Format",
    "00002900": "Characteristic Extended Properties",
    "00002904": "Characteristic Presentation Format",
    "00002901": "Characteristic User Description",
    "00002902": "Client Characteristic Configuration",
    "0000290B": "Environmental Sensing Configuration",
    "0000290C": "Environmental Sensing Measurement",
    "0000290D": "Environmental Sensing Trigger Setting",
    "00002907": "External Report Reference",
    "00002909": "Number of Digitals",
    "00002908": "Report Reference",
    "00002903": "Server Characteristic Configuration",
    "0000290E": "Time Trigger Setting",
    "00002906": "Valid Range",
    "0000290A": "Value Trigger Setting"
});
//# sourceMappingURL=descriptors.js.map