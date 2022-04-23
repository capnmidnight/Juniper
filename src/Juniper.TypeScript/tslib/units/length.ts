const MICROMETERS_PER_MILLIMETER = 1000;
const MILLIMETERS_PER_CENTIMETER = 10;
const CENTIMETERS_PER_INCH = 2.54;
const CENTIMETERS_PER_METER = 100;
const INCHES_PER_HAND = 4;
const HANDS_PER_FOOT = 3;
const FEET_PER_YARD = 3;
const FEET_PER_ROD = 16.5;
const METERS_PER_KILOMETER = 1000;
const RODS_PER_FURLONG = 40;
const FURLONGS_PER_MILE = 8;

const MICROMETERS_PER_CENTIMETER = MICROMETERS_PER_MILLIMETER * MILLIMETERS_PER_CENTIMETER;
const MICROMETERS_PER_INCH = MICROMETERS_PER_CENTIMETER * CENTIMETERS_PER_INCH;
const MICROMETERS_PER_HAND = MICROMETERS_PER_INCH * INCHES_PER_HAND;
const MICROMETERS_PER_FOOT = MICROMETERS_PER_HAND * HANDS_PER_FOOT;
const MICROMETERS_PER_YARD = MICROMETERS_PER_FOOT * FEET_PER_YARD;
const MICROMETERS_PER_METER = MICROMETERS_PER_CENTIMETER * CENTIMETERS_PER_METER;
const MICROMETERS_PER_ROD = MICROMETERS_PER_FOOT * FEET_PER_ROD;
const MICROMETERS_PER_FURLONG = MICROMETERS_PER_ROD * RODS_PER_FURLONG;
const MICROMETERS_PER_KILOMETER = MICROMETERS_PER_METER * METERS_PER_KILOMETER;
const MICROMETERS_PER_MILE = MICROMETERS_PER_FURLONG * FURLONGS_PER_MILE;

const MILLIMETERS_PER_INCH = MILLIMETERS_PER_CENTIMETER * CENTIMETERS_PER_INCH;
const MILLIMETERS_PER_HAND = MILLIMETERS_PER_INCH * INCHES_PER_HAND;
const MILLIMETERS_PER_FOOT = MILLIMETERS_PER_HAND * HANDS_PER_FOOT;
const MILLIMETERS_PER_YARD = MILLIMETERS_PER_FOOT * FEET_PER_YARD;
const MILLIMETERS_PER_METER = MILLIMETERS_PER_CENTIMETER * CENTIMETERS_PER_METER;
const MILLIMETERS_PER_ROD = MILLIMETERS_PER_FOOT * FEET_PER_ROD;
const MILLIMETERS_PER_FURLONG = MILLIMETERS_PER_ROD * RODS_PER_FURLONG;
const MILLIMETERS_PER_KILOMETER = MILLIMETERS_PER_METER * METERS_PER_KILOMETER;
const MILLIMETERS_PER_MILE = MILLIMETERS_PER_FURLONG * FURLONGS_PER_MILE;

const CENTIMETERS_PER_HAND = CENTIMETERS_PER_INCH * INCHES_PER_HAND;
const CENTIMETERS_PER_FOOT = CENTIMETERS_PER_HAND * HANDS_PER_FOOT;
const CENTIMETERS_PER_YARD = CENTIMETERS_PER_FOOT * FEET_PER_YARD;
const CENTIMETERS_PER_ROD = CENTIMETERS_PER_FOOT * FEET_PER_ROD;
const CENTIMETERS_PER_FURLONG = CENTIMETERS_PER_ROD * RODS_PER_FURLONG;
const CENTIMETERS_PER_KILOMETER = CENTIMETERS_PER_METER * METERS_PER_KILOMETER;
const CENTIMETERS_PER_MILE = CENTIMETERS_PER_FURLONG * FURLONGS_PER_MILE;

const INCHES_PER_FOOT = INCHES_PER_HAND * HANDS_PER_FOOT;
const INCHES_PER_YARD = INCHES_PER_FOOT * FEET_PER_YARD;
const INCHES_PER_METER = CENTIMETERS_PER_METER / CENTIMETERS_PER_INCH;
const INCHES_PER_ROD = INCHES_PER_FOOT * FEET_PER_ROD;
const INCHES_PER_FURLONG = INCHES_PER_ROD * RODS_PER_FURLONG;
const INCHES_PER_KILOMETER = INCHES_PER_METER * METERS_PER_KILOMETER;
const INCHES_PER_MILE = INCHES_PER_FURLONG * FURLONGS_PER_MILE;

const HANDS_PER_YARD = HANDS_PER_FOOT * FEET_PER_YARD;
const HANDS_PER_METER = CENTIMETERS_PER_METER / CENTIMETERS_PER_HAND;
const HANDS_PER_ROD = HANDS_PER_FOOT * FEET_PER_ROD;
const HANDS_PER_FURLONG = HANDS_PER_ROD * RODS_PER_FURLONG;
const HANDS_PER_KILOMETER = HANDS_PER_METER * METERS_PER_KILOMETER;
const HANDS_PER_MILE = HANDS_PER_FURLONG * FURLONGS_PER_MILE;

const FEET_PER_METER = INCHES_PER_METER / INCHES_PER_FOOT;
const FEET_PER_FURLONG = FEET_PER_ROD * RODS_PER_FURLONG;
const FEET_PER_KILOMETER = FEET_PER_METER * METERS_PER_KILOMETER;
const FEET_PER_MILE = FEET_PER_FURLONG * FURLONGS_PER_MILE;

const YARDS_PER_METER = INCHES_PER_METER / INCHES_PER_YARD;
const YARDS_PER_ROD = FEET_PER_ROD / FEET_PER_YARD;
const YARDS_PER_FURLONG = YARDS_PER_ROD * RODS_PER_FURLONG;
const YARDS_PER_KILOMETER = YARDS_PER_METER * METERS_PER_KILOMETER;
const YARDS_PER_MILE = YARDS_PER_FURLONG * FURLONGS_PER_MILE;

const METERS_PER_ROD = FEET_PER_ROD / FEET_PER_METER;
const METERS_PER_FURLONG = METERS_PER_ROD * RODS_PER_FURLONG;
const METERS_PER_MILE = METERS_PER_FURLONG * FURLONGS_PER_MILE;

const RODS_PER_KILOMETER = METERS_PER_KILOMETER / METERS_PER_ROD;
const RODS_PER_MILE = RODS_PER_FURLONG * FURLONGS_PER_MILE;

const FURLONGS_PER_KILOMETER = METERS_PER_KILOMETER /METERS_PER_FURLONG;

const KILOMETERS_PER_MILE = FURLONGS_PER_MILE / FURLONGS_PER_KILOMETER;


export function centimeters2Micrometers(centimeters: number): number {
    return centimeters * MICROMETERS_PER_CENTIMETER;
}

export function centimeters2Millimeters(centimeters: number): number {
    return centimeters * MILLIMETERS_PER_CENTIMETER;
}

export function centimeters2Inches(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_INCH;
}

export function centimeters2Hands(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_HAND;
}

export function centimeters2Feet(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_FOOT;
}

export function centimeters2Yards(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_YARD;
}

export function centimeters2Meters(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_METER;
}

export function centimeters2Rods(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_ROD;
}

export function centimeters2Furlongs(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_FURLONG;
}

export function centimeters2Kilometers(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_KILOMETER;
}

export function centimeters2Miles(centimeters: number): number {
    return centimeters / CENTIMETERS_PER_MILE;
}

export function feet2Micrometers(feet: number): number {
    return feet * MICROMETERS_PER_FOOT;
}

export function feet2Millimeters(feet: number): number {
    return feet * MILLIMETERS_PER_FOOT;
}

export function feet2Centimeters(feet: number): number {
    return feet * CENTIMETERS_PER_FOOT;
}

export function feet2Inches(feet: number): number {
    return feet * INCHES_PER_FOOT;
}

export function feet2Hands(feet: number): number {
    return feet * HANDS_PER_FOOT;
}

export function feet2Yards(feet: number): number {
    return feet / FEET_PER_YARD;
}

export function feet2Meters(feet: number): number {
    return feet / FEET_PER_METER;
}

export function feet2Rods(feet: number): number {
    return feet / FEET_PER_ROD;
}

export function feet2Furlongs(feet: number): number {
    return feet / FEET_PER_FURLONG;
}

export function feet2Kilometers(feet: number): number {
    return feet / FEET_PER_KILOMETER;
}

export function feet2Miles(feet: number): number {
    return feet / FEET_PER_MILE;
}

export function furlongs2Micrometers(furlongs: number): number {
    return furlongs * MICROMETERS_PER_FURLONG;
}

export function furlongs2Millimeters(furlongs: number): number {
    return furlongs * MILLIMETERS_PER_FURLONG;
}

export function furlongs2Centimeters(furlongs: number): number {
    return furlongs * CENTIMETERS_PER_FURLONG;
}

export function furlongs2Inches(furlongs: number): number {
    return furlongs * INCHES_PER_FURLONG;
}

export function furlongs2Hands(furlongs: number): number {
    return furlongs * HANDS_PER_FURLONG;
}

export function furlongs2Feet(furlongs: number): number {
    return furlongs * FEET_PER_FURLONG;
}

export function furlongs2Yards(furlongs: number): number {
    return furlongs * YARDS_PER_FURLONG;
}

export function furlongs2Meters(furlongs: number): number {
    return furlongs * METERS_PER_FURLONG;
}

export function furlongs2Rods(furlongs: number): number {
    return furlongs * RODS_PER_FURLONG;
}

export function furlongs2Kilometers(furlongs: number): number {
    return furlongs / FURLONGS_PER_KILOMETER;
}

export function furlongs2Miles(furlongs: number): number {
    return furlongs / FURLONGS_PER_MILE;
}

export function hands2Micrometers(hands: number): number {
    return hands * MICROMETERS_PER_HAND;
}

export function hands2Millimeters(hands: number): number {
    return hands * MILLIMETERS_PER_HAND;
}

export function hands2Centimeters(hands: number): number {
    return hands * CENTIMETERS_PER_HAND;
}

export function hands2Inches(hands: number): number {
    return hands * INCHES_PER_HAND;
}

export function hands2Feet(hands: number): number {
    return hands / HANDS_PER_FOOT;
}

export function hands2Yards(hands: number): number {
    return hands / HANDS_PER_YARD;
}

export function hands2Meters(hands: number): number {
    return hands / HANDS_PER_METER;
}

export function hands2Rods(hands: number): number {
    return hands / HANDS_PER_ROD;
}

export function hands2Furlongs(hands: number): number {
    return hands / HANDS_PER_FURLONG;
}

export function hands2Kilometers(hands: number): number {
    return hands / HANDS_PER_KILOMETER;
}

export function hands2Miles(hands: number): number {
    return hands / HANDS_PER_MILE;
}

export function inches2Micrometers(inches: number): number {
    return inches * MICROMETERS_PER_INCH;
}

export function inches2Millimeters(inches: number): number {
    return inches * MILLIMETERS_PER_INCH;
}

export function inches2Centimeters(inches: number): number {
    return inches * CENTIMETERS_PER_INCH;
}

export function inches2Hands(inches: number): number {
    return inches / INCHES_PER_HAND;
}

export function inches2Feet(inches: number): number {
    return inches / INCHES_PER_FOOT;
}

export function inches2Yards(inches: number): number {
    return inches / INCHES_PER_YARD;
}

export function inches2Meters(inches: number): number {
    return inches / INCHES_PER_METER;
}

export function inches2Rods(inches: number): number {
    return inches / INCHES_PER_ROD;
}

export function inches2Furlongs(inches: number): number {
    return inches / INCHES_PER_FURLONG;
}

export function inches2Kilometers(inches: number): number {
    return inches / INCHES_PER_KILOMETER;
}

export function inches2Miles(inches: number): number {
    return inches / INCHES_PER_MILE;
}

export function kilometers2Micrometers(kilometers: number): number {
    return kilometers * MICROMETERS_PER_KILOMETER;
}

export function kilometers2Millimeters(kilometers: number): number {
    return kilometers * MILLIMETERS_PER_KILOMETER;
}

export function kilometers2Centimeters(kilometers: number): number {
    return kilometers * CENTIMETERS_PER_KILOMETER;
}

export function kilometers2Inches(kilometers: number): number {
    return kilometers * INCHES_PER_KILOMETER;
}

export function kilometers2Hands(kilometers: number): number {
    return kilometers * HANDS_PER_KILOMETER;
}

export function kilometers2Feet(kilometers: number): number {
    return kilometers * FEET_PER_KILOMETER;
}

export function kilometers2Yards(kilometers: number): number {
    return kilometers * YARDS_PER_KILOMETER;
}

export function kilometers2Meters(kilometers: number): number {
    return kilometers * METERS_PER_KILOMETER;
}

export function kilometers2Rods(kilometers: number): number {
    return kilometers * RODS_PER_KILOMETER;
}
export function kilometers2Furlongs(kilometers: number): number {
    return kilometers * FURLONGS_PER_KILOMETER;
}

export function kilometers2Miles(kilometers: number): number {
    return kilometers / KILOMETERS_PER_MILE;
}

export function meters2Micrometers(meters: number): number {
    return meters * MICROMETERS_PER_METER;
}

export function meters2Millimeters(meters: number): number {
    return meters * MILLIMETERS_PER_METER;
}

export function meters2Centimeters(meters: number): number {
    return meters * CENTIMETERS_PER_METER;
}

export function meters2Inches(meters: number): number {
    return meters * INCHES_PER_METER;
}

export function meters2Hands(meters: number): number {
    return meters * HANDS_PER_METER;
}

export function meters2Feet(meters: number): number {
    return meters * FEET_PER_METER;
}

export function meters2Yards(meters: number): number {
    return meters * YARDS_PER_METER;
}

export function meters2Rods(meters: number): number {
    return meters / METERS_PER_ROD;
}

export function meters2Furlongs(meters: number): number {
    return meters / METERS_PER_FURLONG;
}

export function meters2Kilometers(meters: number): number {
    return meters / METERS_PER_KILOMETER;
}

export function meters2Miles(meters: number): number {
    return meters / METERS_PER_MILE;
}

export function micrometers2Millimeters(micrometers: number): number {
    return micrometers / MICROMETERS_PER_MILLIMETER;
}

export function micrometers2Centimeters(micrometers: number): number {
    return micrometers / MICROMETERS_PER_CENTIMETER;
}

export function micrometers2Inches(micrometers: number): number {
    return micrometers / MICROMETERS_PER_INCH;
}

export function micrometers2Hands(micrometers: number): number {
    return micrometers / MICROMETERS_PER_HAND;
}

export function micrometers2Feet(micrometers: number): number {
    return micrometers / MICROMETERS_PER_FOOT;
}

export function micrometers2Yards(micrometers: number): number {
    return micrometers / MICROMETERS_PER_YARD;
}

export function micrometers2Meters(micrometers: number): number {
    return micrometers / MICROMETERS_PER_METER;
}

export function micrometers2Rods(micrometers: number): number {
    return micrometers / MICROMETERS_PER_ROD;
}

export function micrometers2Furlongs(micrometers: number): number {
    return micrometers / MICROMETERS_PER_FURLONG;
}

export function micrometers2Kilometers(micrometers: number): number {
    return micrometers / MICROMETERS_PER_KILOMETER;
}

export function micrometers2Miles(micrometers: number): number {
    return micrometers / MICROMETERS_PER_MILE;
}

export function miles2Micrometers(miles: number): number {
    return miles * MICROMETERS_PER_MILE;
}

export function miles2Millimeters(miles: number): number {
    return miles * MILLIMETERS_PER_MILE;
}

export function miles2Centimeters(miles: number): number {
    return miles * CENTIMETERS_PER_MILE;
}

export function miles2Inches(miles: number): number {
    return miles * INCHES_PER_MILE;
}

export function miles2Hands(miles: number): number {
    return miles * HANDS_PER_MILE;
}

export function miles2Feet(miles: number): number {
    return miles * FEET_PER_MILE;
}

export function miles2Yards(miles: number): number {
    return miles * YARDS_PER_MILE;
}

export function miles2Meters(miles: number): number {
    return miles * METERS_PER_MILE;
}

export function miles2Rods(miles: number): number {
    return miles * RODS_PER_MILE;
}

export function miles2Furlongs(miles: number): number {
    return miles * FURLONGS_PER_MILE;
}

export function miles2Kilometers(miles: number): number {
    return miles * KILOMETERS_PER_MILE;
}

export function millimeters2Micrometers(millimeters: number): number {
    return millimeters * MICROMETERS_PER_MILLIMETER;
}

export function millimeters2Centimeters(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_CENTIMETER;
}

export function millimeters2Inches(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_INCH;
}

export function millimeters2Hands(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_HAND;
}

export function millimeters2Feet(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_FOOT;
}

export function millimeters2Yards(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_YARD;
}

export function millimeters2Meters(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_METER;
}

export function millimeters2Rods(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_ROD;
}

export function millimeters2Furlongs(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_FURLONG;
}

export function millimeters2Kilometers(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_KILOMETER;
}

export function millimeters2Miles(millimeters: number): number {
    return millimeters / MILLIMETERS_PER_MILE;
}

export function rods2Micrometers(rods: number): number {
    return rods * MICROMETERS_PER_ROD;
}

export function rods2Millimeters(rods: number): number {
    return rods * MILLIMETERS_PER_ROD;
}

export function rods2Centimeters(rods: number): number {
    return rods * CENTIMETERS_PER_ROD;
}

export function rods2Inches(rods: number): number {
    return rods * INCHES_PER_ROD;
}

export function rods2Hands(rods: number): number {
    return rods * HANDS_PER_ROD;
}

export function rods2Feet(rods: number): number {
    return rods * FEET_PER_ROD;
}

export function rods2Yards(rods: number): number {
    return rods * YARDS_PER_ROD;
}

export function rods2Meters(rods: number): number {
    return rods * METERS_PER_ROD;
}

export function rods2Furlongs(rods: number): number {
    return rods / RODS_PER_FURLONG;
}

export function rods2Kilometers(rods: number): number {
    return rods / RODS_PER_KILOMETER;
}

export function rods2Miles(rods: number): number {
    return rods / RODS_PER_MILE;
}

export function yards2Micrometers(yards: number): number {
    return yards * MICROMETERS_PER_YARD;
}

export function yards2Millimeters(yards: number): number {
    return yards * MILLIMETERS_PER_YARD;
}

export function yards2Centimeters(yards: number): number {
    return yards * CENTIMETERS_PER_YARD;
}

export function yards2Inches(yards: number): number {
    return yards * INCHES_PER_YARD;
}

export function yards2Hands(yards: number): number {
    return yards * HANDS_PER_YARD;
}

export function yards2Feet(yards: number): number {
    return yards * FEET_PER_YARD;
}

export function yards2Meters(yards: number): number {
    return yards / YARDS_PER_METER;
}

export function yards2Rods(yards: number): number {
    return yards / YARDS_PER_ROD;
}

export function yards2Furlongs(yards: number): number {
    return yards / YARDS_PER_FURLONG;
}

export function yards2Kilometers(yards: number): number {
    return yards / YARDS_PER_KILOMETER;
}

export function yards2Miles(yards: number): number {
    return yards / YARDS_PER_MILE;
}