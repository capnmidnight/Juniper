import { TypedHtmlProp } from "@juniper-lib/dom";
import { FileViewElement, FileViewValue } from "./FileViewElement";

export function FileAttr(file: FileViewValue) { return TypedHtmlProp<FileViewElement, "file", FileViewValue>("file", file); }
