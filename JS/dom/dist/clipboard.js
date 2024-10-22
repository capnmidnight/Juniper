import { Value } from "./attrs";
import { left, position, px } from "./css";
import { TextArea } from "./tags";
export async function copyToClipboard(text) {
    // Navigator clipboard api needs a secure context (https)
    if (navigator.clipboard && window.isSecureContext) {
        await navigator.clipboard.writeText(text);
    }
    else {
        // Use the 'out of viewport hidden text area' trick
        const textArea = TextArea(Value(text), position("absolute"), left(px(-9999999)));
        document.body.prepend(textArea);
        textArea.select();
        try {
            document.execCommand('copy');
        }
        catch (error) {
            console.error(error);
        }
        finally {
            textArea.remove();
        }
    }
}
//# sourceMappingURL=clipboard.js.map