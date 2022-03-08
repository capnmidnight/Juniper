import * as _allApplication from "./application";
import * as _allAudio from "./audio";
import * as _allChemical from "./chemical";
import * as _allFont from "./font";
import * as _allImage from "./image";
import * as _allMessage from "./message";
import * as _allModel from "./model";
import * as _allMultipart from "./multipart";
import * as _allText from "./text";
import * as _allVideo from "./video";
import * as _allXConference from "./xConference";
import * as _allXShader from "./xShader";

import { create } from "./util";

export { MediaType, mediaTypeGuessByExtension, mediaTypeGuessByFileName, mediaTypeNormalizeFileName, mediaTypeParse, mediaTypesMatch } from "./util";

export const anyApplication = create("application", "*");
export const allApplication = Object.values(_allApplication);

export const anyAudio = create("audio", "*");
export const allAudio = Object.values(_allAudio);

export const anyChemical = create("chemical", "*");
export const allChemical = Object.values(_allChemical);

export const anyFont = create("font", "*");
export const allFont = Object.values(_allFont);

export const anyImage = create("image", "*");
export const allImage = Object.values(_allImage);

export const anyMessage = create("message", "*");
export const allMessage = Object.values(_allMessage);

export const anyModel = create("model", "*");
export const allModel = Object.values(_allModel);

export const anyMultipart = create("multipart", "*");
export const allMultipart = Object.values(_allMultipart);

export const anyText = create("text", "*");
export const allText = Object.values(_allText);

export const anyVideo = create("video", "*");
export const allVideo = Object.values(_allVideo);

export const anyXConference = create("x-conference", "*");
export const allXConference = Object.values(_allXConference);

export const anyXShader = create("x-shader", "*");
export const allXShader = Object.values(_allXShader);

export function makeXUrlBlob(url: URL): Blob {
    return new Blob([url.href], { type: _allApplication.Application_X_Url.value });
}

export function makeTextBlob(text: string): Blob {
    return new Blob([text], { type: _allText.Text_Plain.value });
}

export function makeBlobURL(obj: Blob | MediaSource): URL {
    return new URL(URL.createObjectURL(obj));
}