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

import { specialize } from "./util";

export { MediaType, mediaTypeGuessByExtension, mediaTypeGuessByFileName, mediaTypeNormalizeFileName, mediaTypeParse, mediaTypesMatch } from "./util";

export const anyApplication = specialize("application")("*");
export const allApplication = Object.values(_allApplication);

export const anyAudio = specialize("audio")("*");
export const allAudio = Object.values(_allAudio);

export const anyChemical = specialize("chemical")("*");
export const allChemical = Object.values(_allChemical);

export const anyFont = specialize("font")("*");
export const allFont = Object.values(_allFont);

export const anyImage = specialize("image")("*");
export const allImage = Object.values(_allImage);

export const anyMessage = specialize("message")("*");
export const allMessage = Object.values(_allMessage);

export const anyModel = specialize("model")("*");
export const allModel = Object.values(_allModel);

export const anyMultipart = specialize("multipart")("*");
export const allMultipart = Object.values(_allMultipart);

export const anyText = specialize("text")("*");
export const allText = Object.values(_allText);

export const anyVideo = specialize("video")("*");
export const allVideo = Object.values(_allVideo);

export const anyXConference = specialize("x-conference")("*");
export const allXConference = Object.values(_allXConference);

export const anyXShader = specialize("x-shader")("*");
export const allXShader = Object.values(_allXShader);