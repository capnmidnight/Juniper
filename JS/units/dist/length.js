const MICROMETERS_PER_MILLIMETER = /*@__PURE__*/ (function () { return 1000; })();
const MILLIMETERS_PER_CENTIMETER = /*@__PURE__*/ (function () { return 10; })();
const CENTIMETERS_PER_INCH = /*@__PURE__*/ (function () { return 2.54; })();
const CENTIMETERS_PER_METER = /*@__PURE__*/ (function () { return 100; })();
const INCHES_PER_HAND = /*@__PURE__*/ (function () { return 4; })();
const HANDS_PER_FOOT = /*@__PURE__*/ (function () { return 3; })();
const FEET_PER_YARD = /*@__PURE__*/ (function () { return 3; })();
const FEET_PER_ROD = /*@__PURE__*/ (function () { return 16.5; })();
const METERS_PER_KILOMETER = /*@__PURE__*/ (function () { return 1000; })();
const RODS_PER_FURLONG = /*@__PURE__*/ (function () { return 40; })();
const FURLONGS_PER_MILE = /*@__PURE__*/ (function () { return 8; })();
const MICROMETERS_PER_CENTIMETER = /*@__PURE__*/ (function () { return MICROMETERS_PER_MILLIMETER * MILLIMETERS_PER_CENTIMETER; })();
const MICROMETERS_PER_INCH = /*@__PURE__*/ (function () { return MICROMETERS_PER_CENTIMETER * CENTIMETERS_PER_INCH; })();
const MICROMETERS_PER_HAND = /*@__PURE__*/ (function () { return MICROMETERS_PER_INCH * INCHES_PER_HAND; })();
const MICROMETERS_PER_FOOT = /*@__PURE__*/ (function () { return MICROMETERS_PER_HAND * HANDS_PER_FOOT; })();
const MICROMETERS_PER_YARD = /*@__PURE__*/ (function () { return MICROMETERS_PER_FOOT * FEET_PER_YARD; })();
const MICROMETERS_PER_METER = /*@__PURE__*/ (function () { return MICROMETERS_PER_CENTIMETER * CENTIMETERS_PER_METER; })();
const MICROMETERS_PER_ROD = /*@__PURE__*/ (function () { return MICROMETERS_PER_FOOT * FEET_PER_ROD; })();
const MICROMETERS_PER_FURLONG = /*@__PURE__*/ (function () { return MICROMETERS_PER_ROD * RODS_PER_FURLONG; })();
const MICROMETERS_PER_KILOMETER = /*@__PURE__*/ (function () { return MICROMETERS_PER_METER * METERS_PER_KILOMETER; })();
const MICROMETERS_PER_MILE = /*@__PURE__*/ (function () { return MICROMETERS_PER_FURLONG * FURLONGS_PER_MILE; })();
const MILLIMETERS_PER_INCH = /*@__PURE__*/ (function () { return MILLIMETERS_PER_CENTIMETER * CENTIMETERS_PER_INCH; })();
const MILLIMETERS_PER_HAND = /*@__PURE__*/ (function () { return MILLIMETERS_PER_INCH * INCHES_PER_HAND; })();
const MILLIMETERS_PER_FOOT = /*@__PURE__*/ (function () { return MILLIMETERS_PER_HAND * HANDS_PER_FOOT; })();
const MILLIMETERS_PER_YARD = /*@__PURE__*/ (function () { return MILLIMETERS_PER_FOOT * FEET_PER_YARD; })();
const MILLIMETERS_PER_METER = /*@__PURE__*/ (function () { return MILLIMETERS_PER_CENTIMETER * CENTIMETERS_PER_METER; })();
const MILLIMETERS_PER_ROD = /*@__PURE__*/ (function () { return MILLIMETERS_PER_FOOT * FEET_PER_ROD; })();
const MILLIMETERS_PER_FURLONG = /*@__PURE__*/ (function () { return MILLIMETERS_PER_ROD * RODS_PER_FURLONG; })();
const MILLIMETERS_PER_KILOMETER = /*@__PURE__*/ (function () { return MILLIMETERS_PER_METER * METERS_PER_KILOMETER; })();
const MILLIMETERS_PER_MILE = /*@__PURE__*/ (function () { return MILLIMETERS_PER_FURLONG * FURLONGS_PER_MILE; })();
const CENTIMETERS_PER_HAND = /*@__PURE__*/ (function () { return CENTIMETERS_PER_INCH * INCHES_PER_HAND; })();
const CENTIMETERS_PER_FOOT = /*@__PURE__*/ (function () { return CENTIMETERS_PER_HAND * HANDS_PER_FOOT; })();
const CENTIMETERS_PER_YARD = /*@__PURE__*/ (function () { return CENTIMETERS_PER_FOOT * FEET_PER_YARD; })();
const CENTIMETERS_PER_ROD = /*@__PURE__*/ (function () { return CENTIMETERS_PER_FOOT * FEET_PER_ROD; })();
const CENTIMETERS_PER_FURLONG = /*@__PURE__*/ (function () { return CENTIMETERS_PER_ROD * RODS_PER_FURLONG; })();
const CENTIMETERS_PER_KILOMETER = /*@__PURE__*/ (function () { return CENTIMETERS_PER_METER * METERS_PER_KILOMETER; })();
const CENTIMETERS_PER_MILE = /*@__PURE__*/ (function () { return CENTIMETERS_PER_FURLONG * FURLONGS_PER_MILE; })();
const INCHES_PER_FOOT = /*@__PURE__*/ (function () { return INCHES_PER_HAND * HANDS_PER_FOOT; })();
const INCHES_PER_YARD = /*@__PURE__*/ (function () { return INCHES_PER_FOOT * FEET_PER_YARD; })();
const INCHES_PER_METER = /*@__PURE__*/ (function () { return CENTIMETERS_PER_METER / CENTIMETERS_PER_INCH; })();
const INCHES_PER_ROD = /*@__PURE__*/ (function () { return INCHES_PER_FOOT * FEET_PER_ROD; })();
const INCHES_PER_FURLONG = /*@__PURE__*/ (function () { return INCHES_PER_ROD * RODS_PER_FURLONG; })();
const INCHES_PER_KILOMETER = /*@__PURE__*/ (function () { return INCHES_PER_METER * METERS_PER_KILOMETER; })();
const INCHES_PER_MILE = /*@__PURE__*/ (function () { return INCHES_PER_FURLONG * FURLONGS_PER_MILE; })();
const HANDS_PER_YARD = /*@__PURE__*/ (function () { return HANDS_PER_FOOT * FEET_PER_YARD; })();
const HANDS_PER_METER = /*@__PURE__*/ (function () { return CENTIMETERS_PER_METER / CENTIMETERS_PER_HAND; })();
const HANDS_PER_ROD = /*@__PURE__*/ (function () { return HANDS_PER_FOOT * FEET_PER_ROD; })();
const HANDS_PER_FURLONG = /*@__PURE__*/ (function () { return HANDS_PER_ROD * RODS_PER_FURLONG; })();
const HANDS_PER_KILOMETER = /*@__PURE__*/ (function () { return HANDS_PER_METER * METERS_PER_KILOMETER; })();
const HANDS_PER_MILE = /*@__PURE__*/ (function () { return HANDS_PER_FURLONG * FURLONGS_PER_MILE; })();
const FEET_PER_METER = /*@__PURE__*/ (function () { return INCHES_PER_METER / INCHES_PER_FOOT; })();
const FEET_PER_FURLONG = /*@__PURE__*/ (function () { return FEET_PER_ROD * RODS_PER_FURLONG; })();
const FEET_PER_KILOMETER = /*@__PURE__*/ (function () { return FEET_PER_METER * METERS_PER_KILOMETER; })();
const FEET_PER_MILE = /*@__PURE__*/ (function () { return FEET_PER_FURLONG * FURLONGS_PER_MILE; })();
const YARDS_PER_METER = /*@__PURE__*/ (function () { return INCHES_PER_METER / INCHES_PER_YARD; })();
const YARDS_PER_ROD = /*@__PURE__*/ (function () { return FEET_PER_ROD / FEET_PER_YARD; })();
const YARDS_PER_FURLONG = /*@__PURE__*/ (function () { return YARDS_PER_ROD * RODS_PER_FURLONG; })();
const YARDS_PER_KILOMETER = /*@__PURE__*/ (function () { return YARDS_PER_METER * METERS_PER_KILOMETER; })();
const YARDS_PER_MILE = /*@__PURE__*/ (function () { return YARDS_PER_FURLONG * FURLONGS_PER_MILE; })();
const METERS_PER_ROD = /*@__PURE__*/ (function () { return FEET_PER_ROD / FEET_PER_METER; })();
const METERS_PER_FURLONG = /*@__PURE__*/ (function () { return METERS_PER_ROD * RODS_PER_FURLONG; })();
const METERS_PER_MILE = /*@__PURE__*/ (function () { return METERS_PER_FURLONG * FURLONGS_PER_MILE; })();
const RODS_PER_KILOMETER = /*@__PURE__*/ (function () { return METERS_PER_KILOMETER / METERS_PER_ROD; })();
const RODS_PER_MILE = /*@__PURE__*/ (function () { return RODS_PER_FURLONG * FURLONGS_PER_MILE; })();
const FURLONGS_PER_KILOMETER = /*@__PURE__*/ (function () { return METERS_PER_KILOMETER / METERS_PER_FURLONG; })();
const KILOMETERS_PER_MILE = /*@__PURE__*/ (function () { return FURLONGS_PER_MILE / FURLONGS_PER_KILOMETER; })();
export function centimeters2Micrometers(centimeters) {
    return centimeters * MICROMETERS_PER_CENTIMETER;
}
export function centimeters2Millimeters(centimeters) {
    return centimeters * MILLIMETERS_PER_CENTIMETER;
}
export function centimeters2Inches(centimeters) {
    return centimeters / CENTIMETERS_PER_INCH;
}
export function centimeters2Hands(centimeters) {
    return centimeters / CENTIMETERS_PER_HAND;
}
export function centimeters2Feet(centimeters) {
    return centimeters / CENTIMETERS_PER_FOOT;
}
export function centimeters2Yards(centimeters) {
    return centimeters / CENTIMETERS_PER_YARD;
}
export function centimeters2Meters(centimeters) {
    return centimeters / CENTIMETERS_PER_METER;
}
export function centimeters2Rods(centimeters) {
    return centimeters / CENTIMETERS_PER_ROD;
}
export function centimeters2Furlongs(centimeters) {
    return centimeters / CENTIMETERS_PER_FURLONG;
}
export function centimeters2Kilometers(centimeters) {
    return centimeters / CENTIMETERS_PER_KILOMETER;
}
export function centimeters2Miles(centimeters) {
    return centimeters / CENTIMETERS_PER_MILE;
}
export function feet2Micrometers(feet) {
    return feet * MICROMETERS_PER_FOOT;
}
export function feet2Millimeters(feet) {
    return feet * MILLIMETERS_PER_FOOT;
}
export function feet2Centimeters(feet) {
    return feet * CENTIMETERS_PER_FOOT;
}
export function feet2Inches(feet) {
    return feet * INCHES_PER_FOOT;
}
export function feet2Hands(feet) {
    return feet * HANDS_PER_FOOT;
}
export function feet2Yards(feet) {
    return feet / FEET_PER_YARD;
}
export function feet2Meters(feet) {
    return feet / FEET_PER_METER;
}
export function feet2Rods(feet) {
    return feet / FEET_PER_ROD;
}
export function feet2Furlongs(feet) {
    return feet / FEET_PER_FURLONG;
}
export function feet2Kilometers(feet) {
    return feet / FEET_PER_KILOMETER;
}
export function feet2Miles(feet) {
    return feet / FEET_PER_MILE;
}
export function furlongs2Micrometers(furlongs) {
    return furlongs * MICROMETERS_PER_FURLONG;
}
export function furlongs2Millimeters(furlongs) {
    return furlongs * MILLIMETERS_PER_FURLONG;
}
export function furlongs2Centimeters(furlongs) {
    return furlongs * CENTIMETERS_PER_FURLONG;
}
export function furlongs2Inches(furlongs) {
    return furlongs * INCHES_PER_FURLONG;
}
export function furlongs2Hands(furlongs) {
    return furlongs * HANDS_PER_FURLONG;
}
export function furlongs2Feet(furlongs) {
    return furlongs * FEET_PER_FURLONG;
}
export function furlongs2Yards(furlongs) {
    return furlongs * YARDS_PER_FURLONG;
}
export function furlongs2Meters(furlongs) {
    return furlongs * METERS_PER_FURLONG;
}
export function furlongs2Rods(furlongs) {
    return furlongs * RODS_PER_FURLONG;
}
export function furlongs2Kilometers(furlongs) {
    return furlongs / FURLONGS_PER_KILOMETER;
}
export function furlongs2Miles(furlongs) {
    return furlongs / FURLONGS_PER_MILE;
}
export function hands2Micrometers(hands) {
    return hands * MICROMETERS_PER_HAND;
}
export function hands2Millimeters(hands) {
    return hands * MILLIMETERS_PER_HAND;
}
export function hands2Centimeters(hands) {
    return hands * CENTIMETERS_PER_HAND;
}
export function hands2Inches(hands) {
    return hands * INCHES_PER_HAND;
}
export function hands2Feet(hands) {
    return hands / HANDS_PER_FOOT;
}
export function hands2Yards(hands) {
    return hands / HANDS_PER_YARD;
}
export function hands2Meters(hands) {
    return hands / HANDS_PER_METER;
}
export function hands2Rods(hands) {
    return hands / HANDS_PER_ROD;
}
export function hands2Furlongs(hands) {
    return hands / HANDS_PER_FURLONG;
}
export function hands2Kilometers(hands) {
    return hands / HANDS_PER_KILOMETER;
}
export function hands2Miles(hands) {
    return hands / HANDS_PER_MILE;
}
export function inches2Micrometers(inches) {
    return inches * MICROMETERS_PER_INCH;
}
export function inches2Millimeters(inches) {
    return inches * MILLIMETERS_PER_INCH;
}
export function inches2Centimeters(inches) {
    return inches * CENTIMETERS_PER_INCH;
}
export function inches2Hands(inches) {
    return inches / INCHES_PER_HAND;
}
export function inches2Feet(inches) {
    return inches / INCHES_PER_FOOT;
}
export function inches2Yards(inches) {
    return inches / INCHES_PER_YARD;
}
export function inches2Meters(inches) {
    return inches / INCHES_PER_METER;
}
export function inches2Rods(inches) {
    return inches / INCHES_PER_ROD;
}
export function inches2Furlongs(inches) {
    return inches / INCHES_PER_FURLONG;
}
export function inches2Kilometers(inches) {
    return inches / INCHES_PER_KILOMETER;
}
export function inches2Miles(inches) {
    return inches / INCHES_PER_MILE;
}
export function kilometers2Micrometers(kilometers) {
    return kilometers * MICROMETERS_PER_KILOMETER;
}
export function kilometers2Millimeters(kilometers) {
    return kilometers * MILLIMETERS_PER_KILOMETER;
}
export function kilometers2Centimeters(kilometers) {
    return kilometers * CENTIMETERS_PER_KILOMETER;
}
export function kilometers2Inches(kilometers) {
    return kilometers * INCHES_PER_KILOMETER;
}
export function kilometers2Hands(kilometers) {
    return kilometers * HANDS_PER_KILOMETER;
}
export function kilometers2Feet(kilometers) {
    return kilometers * FEET_PER_KILOMETER;
}
export function kilometers2Yards(kilometers) {
    return kilometers * YARDS_PER_KILOMETER;
}
export function kilometers2Meters(kilometers) {
    return kilometers * METERS_PER_KILOMETER;
}
export function kilometers2Rods(kilometers) {
    return kilometers * RODS_PER_KILOMETER;
}
export function kilometers2Furlongs(kilometers) {
    return kilometers * FURLONGS_PER_KILOMETER;
}
export function kilometers2Miles(kilometers) {
    return kilometers / KILOMETERS_PER_MILE;
}
export function meters2Micrometers(meters) {
    return meters * MICROMETERS_PER_METER;
}
export function meters2Millimeters(meters) {
    return meters * MILLIMETERS_PER_METER;
}
export function meters2Centimeters(meters) {
    return meters * CENTIMETERS_PER_METER;
}
export function meters2Inches(meters) {
    return meters * INCHES_PER_METER;
}
export function meters2Hands(meters) {
    return meters * HANDS_PER_METER;
}
export function meters2Feet(meters) {
    return meters * FEET_PER_METER;
}
export function meters2Yards(meters) {
    return meters * YARDS_PER_METER;
}
export function meters2Rods(meters) {
    return meters / METERS_PER_ROD;
}
export function meters2Furlongs(meters) {
    return meters / METERS_PER_FURLONG;
}
export function meters2Kilometers(meters) {
    return meters / METERS_PER_KILOMETER;
}
export function meters2Miles(meters) {
    return meters / METERS_PER_MILE;
}
export function micrometers2Millimeters(micrometers) {
    return micrometers / MICROMETERS_PER_MILLIMETER;
}
export function micrometers2Centimeters(micrometers) {
    return micrometers / MICROMETERS_PER_CENTIMETER;
}
export function micrometers2Inches(micrometers) {
    return micrometers / MICROMETERS_PER_INCH;
}
export function micrometers2Hands(micrometers) {
    return micrometers / MICROMETERS_PER_HAND;
}
export function micrometers2Feet(micrometers) {
    return micrometers / MICROMETERS_PER_FOOT;
}
export function micrometers2Yards(micrometers) {
    return micrometers / MICROMETERS_PER_YARD;
}
export function micrometers2Meters(micrometers) {
    return micrometers / MICROMETERS_PER_METER;
}
export function micrometers2Rods(micrometers) {
    return micrometers / MICROMETERS_PER_ROD;
}
export function micrometers2Furlongs(micrometers) {
    return micrometers / MICROMETERS_PER_FURLONG;
}
export function micrometers2Kilometers(micrometers) {
    return micrometers / MICROMETERS_PER_KILOMETER;
}
export function micrometers2Miles(micrometers) {
    return micrometers / MICROMETERS_PER_MILE;
}
export function miles2Micrometers(miles) {
    return miles * MICROMETERS_PER_MILE;
}
export function miles2Millimeters(miles) {
    return miles * MILLIMETERS_PER_MILE;
}
export function miles2Centimeters(miles) {
    return miles * CENTIMETERS_PER_MILE;
}
export function miles2Inches(miles) {
    return miles * INCHES_PER_MILE;
}
export function miles2Hands(miles) {
    return miles * HANDS_PER_MILE;
}
export function miles2Feet(miles) {
    return miles * FEET_PER_MILE;
}
export function miles2Yards(miles) {
    return miles * YARDS_PER_MILE;
}
export function miles2Meters(miles) {
    return miles * METERS_PER_MILE;
}
export function miles2Rods(miles) {
    return miles * RODS_PER_MILE;
}
export function miles2Furlongs(miles) {
    return miles * FURLONGS_PER_MILE;
}
export function miles2Kilometers(miles) {
    return miles * KILOMETERS_PER_MILE;
}
export function millimeters2Micrometers(millimeters) {
    return millimeters * MICROMETERS_PER_MILLIMETER;
}
export function millimeters2Centimeters(millimeters) {
    return millimeters / MILLIMETERS_PER_CENTIMETER;
}
export function millimeters2Inches(millimeters) {
    return millimeters / MILLIMETERS_PER_INCH;
}
export function millimeters2Hands(millimeters) {
    return millimeters / MILLIMETERS_PER_HAND;
}
export function millimeters2Feet(millimeters) {
    return millimeters / MILLIMETERS_PER_FOOT;
}
export function millimeters2Yards(millimeters) {
    return millimeters / MILLIMETERS_PER_YARD;
}
export function millimeters2Meters(millimeters) {
    return millimeters / MILLIMETERS_PER_METER;
}
export function millimeters2Rods(millimeters) {
    return millimeters / MILLIMETERS_PER_ROD;
}
export function millimeters2Furlongs(millimeters) {
    return millimeters / MILLIMETERS_PER_FURLONG;
}
export function millimeters2Kilometers(millimeters) {
    return millimeters / MILLIMETERS_PER_KILOMETER;
}
export function millimeters2Miles(millimeters) {
    return millimeters / MILLIMETERS_PER_MILE;
}
export function rods2Micrometers(rods) {
    return rods * MICROMETERS_PER_ROD;
}
export function rods2Millimeters(rods) {
    return rods * MILLIMETERS_PER_ROD;
}
export function rods2Centimeters(rods) {
    return rods * CENTIMETERS_PER_ROD;
}
export function rods2Inches(rods) {
    return rods * INCHES_PER_ROD;
}
export function rods2Hands(rods) {
    return rods * HANDS_PER_ROD;
}
export function rods2Feet(rods) {
    return rods * FEET_PER_ROD;
}
export function rods2Yards(rods) {
    return rods * YARDS_PER_ROD;
}
export function rods2Meters(rods) {
    return rods * METERS_PER_ROD;
}
export function rods2Furlongs(rods) {
    return rods / RODS_PER_FURLONG;
}
export function rods2Kilometers(rods) {
    return rods / RODS_PER_KILOMETER;
}
export function rods2Miles(rods) {
    return rods / RODS_PER_MILE;
}
export function yards2Micrometers(yards) {
    return yards * MICROMETERS_PER_YARD;
}
export function yards2Millimeters(yards) {
    return yards * MILLIMETERS_PER_YARD;
}
export function yards2Centimeters(yards) {
    return yards * CENTIMETERS_PER_YARD;
}
export function yards2Inches(yards) {
    return yards * INCHES_PER_YARD;
}
export function yards2Hands(yards) {
    return yards * HANDS_PER_YARD;
}
export function yards2Feet(yards) {
    return yards * FEET_PER_YARD;
}
export function yards2Meters(yards) {
    return yards / YARDS_PER_METER;
}
export function yards2Rods(yards) {
    return yards / YARDS_PER_ROD;
}
export function yards2Furlongs(yards) {
    return yards / YARDS_PER_FURLONG;
}
export function yards2Kilometers(yards) {
    return yards / YARDS_PER_KILOMETER;
}
export function yards2Miles(yards) {
    return yards / YARDS_PER_MILE;
}
//# sourceMappingURL=length.js.map