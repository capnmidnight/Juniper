export declare const DAYS_5: number;
export declare const DAYS_30: number;
export declare const TIME_MIN: number;
export declare const MIN_DATE: Date;
export declare const TIME_MAX: number;
export declare const MAX_DATE: Date;
/**
 * Checks to see if two Date objects fall on the same day.
 */
export declare function sameDay(date1: Date, date2: Date): boolean;
/**
 * Find a substring that looks like a date and parse it as a date.
 * The default Date parser will do this for strings that end with
 * date-like substrings, but it will not do it for strings that
 * have text after the date-like substring.
 */
export declare function extractDate(str: string): Date;
/**
 * Returns a string that is in "YYYY MMM DD" format.
 */
export declare function formatDate(date: string | number | Date): string;
/**
 * Returns a string that is in "YYYY MMM DD" format.
 */
export declare function formatUSDate(date: string | number | Date): string;
export declare const monthNames: string[];
export declare function getMinDate(dates: Iterable<Date>): Date;
export declare function getMaxDate(dates: Iterable<Date>): Date;
export declare function dateISOToLocal(date: Date): Date;
export declare function dateLocalToISO(date: Date): Date;
export declare function startOfDay(date: Date): Date;
export declare function endOfDay(date: Date): Date;
//# sourceMappingURL=dates.d.ts.map