export * from "./stringRandom";
export * from "./stringRepeat";
export * from "./stringReverse";
export * from "./stringSearch";
export * from "./stringSimilarity";
export * from "./stringToName";
import { isDefined, isNullOrUndefined } from "../typeChecks";
/**
* Calls the `trim` method on a string. Useful for functional application.
*/
export function stringTrim(v) {
    return v.trim();
}
/**
 * Calls the `toUpperCase` method on a string. Useful for functional application.
 */
export function toUpper(str) {
    return str.toUpperCase();
}
/**
 * Calls the `toLowerCase` method on a string. Useful for functional application.
 */
export function toLower(str) {
    return str.toLowerCase();
}
/**
 * Calls the `toString` method on an object. Useful for functional application.
 */
export function toString(obj) {
    return obj.toString();
}
/**
 * Creats a callback function that calls the `split` method on an object. Useful for functional application.
 */
export function split(delim, limit) {
    return (str) => str.split(delim, limit);
}
/**
 * Formats an email address into a link.
 */
export function makeEmailLink(address, missingLabel) {
    if (address) {
        return `<a href="mailto:${address}">${address}</a>`;
    }
    else {
        return missingLabel;
    }
}
function splitTitle(nameStr) {
    const match = nameStr.match(/^\s*(dr|mr|ms|mrs)\.?;?\s*(.+)\s*/i);
    const title = match && match[1] || null;
    const name = match && match[2] || nameStr;
    return [title, name];
}
/**
 * Format a name in first/last format.
 */
export function formatNameFirstLast(firstNameStr, lastNameStr) {
    let value = "";
    if (firstNameStr) {
        const [firstTitle, firstName] = splitTitle(firstNameStr);
        value = firstName;
        let title = firstTitle;
        if (lastNameStr) {
            const [lastTitle, lastName] = splitTitle(lastNameStr);
            title = title || lastTitle;
            value += " " + lastName;
        }
        if (title) {
            value = title + ". " + value;
        }
    }
    else if (lastNameStr) {
        const [title, lastName] = splitTitle(lastNameStr);
        value = lastName;
        if (title) {
            value = title + ". " + value;
        }
    }
    return value.replace(/ {2,}/g, " ").trim();
}
/**
 * Format a name in last/first format.
 */
export function formatNameLastFirst(firstNameStr, lastNameStr) {
    let value = "";
    if (lastNameStr) {
        const [lastTitle, lastName] = splitTitle(lastNameStr);
        value = lastName;
        let title = lastTitle;
        if (firstNameStr) {
            const [firstTitle, firstName] = splitTitle(firstNameStr);
            title = title || firstTitle;
            value += ", ";
            if (title) {
                value += title + ". ";
            }
            value += firstName;
        }
        else if (title) {
            value += ", " + title + ".";
        }
    }
    else if (firstNameStr) {
        const [title, firstName] = splitTitle(firstNameStr);
        value = firstName;
        if (title) {
            value = title + ". " + value;
        }
    }
    return value.replace(/ {2,}/g, " ").trim();
}
/**
 * Names in the data files come in a variety of formats.
 *  - {firstname} {lastname}
 *  - {lastname}, {firstname}
 *  - {lastname}, {title};{firstname}
 *  - etc.
 * With some bad habits regarding whitespace.
 * This function attempts to understand the name that
 * has been provided and reformat it into a single format
 * that can better be used for lookups.
 */
export function normalizeNameLastFirst(name, stripTitle = false) {
    const lastNameComesFirst = name.indexOf(",") >= 0;
    const separator = lastNameComesFirst
        ? ","
        : " ";
    let parts = name
        .split(separator)
        .map(stringTrim);
    const lastNameStr = lastNameComesFirst
        ? parts.shift()
        : parts.pop();
    const [lastTitle, lastName] = splitTitle(lastNameStr);
    const firstNameStr = parts.join(" ");
    const [firstTitle, firstName] = splitTitle(firstNameStr);
    const title = firstTitle || lastTitle || "";
    if (stripTitle) {
        return formatNameLastFirst(firstName, lastName);
    }
    else {
        return formatNameLastFirst(`${title} ${firstName}`, lastName);
    }
}
export function formatPhoneNumber(countryCode, areaCode, exchange, extension) {
    if (isNullOrUndefined(exchange) || isNullOrUndefined(extension)) {
        return null;
    }
    return [
        countryCode,
        areaCode,
        exchange,
        extension
    ].filter(isDefined)
        .join('-');
}
export function splitPhoneNumber(number) {
    if (number) {
        const sectionedNumber = number.split("-");
        const validSplit = [];
        if (sectionedNumber.length == 3) {
            validSplit[0] = "";
            validSplit[1] = sectionedNumber[0];
            validSplit[2] = sectionedNumber[1];
            validSplit[3] = sectionedNumber[2];
        }
        else if (sectionedNumber.length == 4) {
            validSplit[0] = sectionedNumber[0];
            validSplit[1] = sectionedNumber[1];
            validSplit[2] = sectionedNumber[2];
            validSplit[3] = sectionedNumber[3];
        }
        else {
            validSplit[0] = "";
            validSplit[1] = "";
            validSplit[2] = "";
            validSplit[3] = "";
        }
        return validSplit;
    }
    return null;
}
export function checkUnknownValue(data, inputType) {
    if (data && data != "UNK") {
        return data;
    }
    else if (inputType == "select") {
        return "0";
    }
    else {
        return "";
    }
}
export function leftPad(v, l, c) {
    while (v.length < l) {
        v = c + v;
    }
    return v;
}
export function rightPad(v, l, c) {
    while (v.length < l) {
        v += c;
    }
    return v;
}
/**
 * Convert three-value logic strings to three-value boolean values.
 * Useful for use with radio buttons
 */
export function yesNoToBool(value) {
    return value === "yes"
        ? true
        : value === "no"
            ? false
            : null;
}
/**
 * Convert three-value boolean values to three-value logic strings.
 * Useful for use with radio buttons
 */
export function boolToYesNo(value) {
    return isDefined(value) ? value ? "yes" : "no" : "";
}
//# sourceMappingURL=index.js.map