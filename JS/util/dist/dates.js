export const DAYS_5 = /* @__PURE__ */ (function () { return 5 * 24 * 60 * 60 * 1000; })();
export const DAYS_30 = /* @__PURE__ */ (function () { return 30 * 24 * 60 * 60 * 1000; })();
export const TIME_MIN = /*@__PURE__*/ (function () { return -8.64e15; })();
export const MIN_DATE = /*@__PURE__*/ (function () { return new Date(TIME_MIN); })();
export const TIME_MAX = /*@__PURE__*/ (function () { return 8.64e15; })();
export const MAX_DATE = /*@__PURE__*/ (function () { return new Date(TIME_MAX); })();
/**
 * Checks to see if two Date objects fall on the same day.
 */
export function sameDay(date1, date2) {
    if (date1.getFullYear() === date2.getFullYear() &&
        date1.getMonth() === date2.getMonth() &&
        date1.getDate() === date2.getDate())
        return true;
    else
        return false;
}
/**
 * Find a substring that looks like a date and parse it as a date.
 * The default Date parser will do this for strings that end with
 * date-like substrings, but it will not do it for strings that
 * have text after the date-like substring.
 */
export function extractDate(str) {
    const match = str.match(/\b\d+\/\d+\/\d+\b/);
    if (match) {
        new Date(match[0]);
    }
    return null;
}
const shortDateFormatter = new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "short",
    day: "2-digit"
});
function extractParts(formatter, dateOrStringOrNumber) {
    let date = dateOrStringOrNumber;
    if (!date) {
        return null;
    }
    if (!(date instanceof Date)) {
        date = new Date(date);
    }
    if (!date || isNaN(date.getTime())) {
        return null;
    }
    try {
        const parts = new Map(formatter.formatToParts(date)
            .filter(v => v.type !== "literal")
            .map(v => [v.type, v.value]));
        return parts;
    }
    catch (err) {
        console.error({ dateOrStringOrNumber, date, err });
        return null;
    }
}
/**
 * Returns a string that is in "YYYY MMM DD" format.
 */
export function formatDate(date) {
    const parts = extractParts(shortDateFormatter, date);
    if (!parts) {
        return null;
    }
    return [
        parts.get("year"),
        parts.get("month").toLocaleUpperCase(),
        parts.get("day")
    ].join(" ");
}
const USDateFormatter = /*@__PURE__*/ (function () {
    return new Intl.DateTimeFormat("en-US", {
        month: "2-digit",
        day: "2-digit",
        year: "numeric"
    });
})();
/**
 * Returns a string that is in "YYYY MMM DD" format.
 */
export function formatUSDate(date) {
    const parts = extractParts(USDateFormatter, date);
    if (!parts) {
        return null;
    }
    return [
        parts.get("month"),
        parts.get("day"),
        parts.get("year")
    ].join("/");
}
export const monthNames = /*@__PURE__*/ (function () {
    return [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    ];
})();
export function getMinDate(dates) {
    let minDate = MAX_DATE;
    for (const date of dates) {
        if (date < minDate) {
            minDate = date;
        }
    }
    return minDate;
}
export function getMaxDate(dates) {
    let maxDate = MIN_DATE;
    for (const date of dates) {
        if (date > maxDate) {
            maxDate = date;
        }
    }
    return maxDate;
}
export function dateISOToLocal(date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes() + date.getTimezoneOffset(), date.getSeconds(), date.getMinutes());
}
export function dateLocalToISO(date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes() - date.getTimezoneOffset(), date.getSeconds(), date.getMinutes());
}
export function startOfDay(date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}
export function endOfDay(date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1, 0, 0, 0, -1);
}
//# sourceMappingURL=dates.js.map