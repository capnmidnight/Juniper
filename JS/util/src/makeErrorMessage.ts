import { isObject, isString } from "./typeChecks";

export function makeErrorMessage(error: unknown): string;
export function makeErrorMessage(errorMessage: string): string;
export function makeErrorMessage(errorMessage: string, error: unknown): string;
export function makeErrorMessage(errorMessageOrError: string | unknown, maybeError?: unknown): string;
export function makeErrorMessage(errorMessageOrError: string | unknown, maybeError?: unknown): string {
    let errorMessage = "$1";

    if (isString(errorMessageOrError)) {
        errorMessage = errorMessageOrError;
    }

    if (isObject(maybeError) && "target" in maybeError) {
        maybeError = maybeError.target;
    }
    if (isObject(maybeError) && "message" in maybeError) {
        maybeError = maybeError.message;
    }
    if (isObject(maybeError) && "error" in maybeError) {
        maybeError = maybeError.error;
    }

    const error = JSON.stringify(maybeError);

    errorMessage = errorMessage.replaceAll(/\$1\b/g, error);
    
    return errorMessage;
}