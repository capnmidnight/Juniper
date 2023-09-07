import { singleton } from "@juniper-lib/tslib/singleton";
import * as _allApplication from "./application";
import * as _allAudio from "./audio";
import * as _allChemical from "./chemical";
import * as _allFont from "./font";
import * as _allImage from "./image";
import * as _allMessage from "./message";
import * as _allModel from "./model";
import * as _allMultipart from "./multipart";
import * as _allText from "./text";
import { MediaType } from "./util";
import * as _allVideo from "./video";
import * as _allXConference from "./xConference";
import * as _allXShader from "./xShader";
export const MediaTypeDB = /*@__PURE__*/ singleton("Juniper:TSLib:MediaTypeDB", () => {
    const byExtension = new Map();
    function guessByFileName(fileName) {
        if (!fileName) {
            console.warn("Couldn't guess media type. Must provide a valid fileName.");
            return [];
        }
        const idx = fileName.lastIndexOf(".");
        if (idx === -1) {
            console.warn("Couldn't guess media type. FileName has no extension.");
            return [];
        }
        let ext = fileName.substring(idx);
        if (!ext) {
            ext = "unknown";
        }
        else if (ext[0] == ".") {
            ext = ext.substring(1);
        }
        if (byExtension.has(ext)) {
            return byExtension.get(ext);
        }
        else {
            return [new MediaType("unknown", ext, [ext])];
        }
    }
    function normalizeFileType(fileName, fileType) {
        if (!fileType && fileName.indexOf(".") > -1) {
            const guesses = guessByFileName(fileName);
            if (guesses.length > 0) {
                fileType = guesses[0].value;
            }
        }
        return fileType;
    }
    function register(type) {
        let isNew = false;
        const value = type.__getValueUnsafe();
        type = singleton("Juniper.MediaTypes:" + value, () => {
            isNew = true;
            return type;
        });
        if (isNew) {
            for (const ext of type.__getExtensionsUnsafe()) {
                if (!byExtension.has(ext)) {
                    byExtension.set(ext, new Array());
                }
                const byExts = byExtension.get(ext);
                if (byExts.indexOf(type) < 0) {
                    byExts.push(type);
                }
            }
        }
        return type;
    }
    const db = {
        normalizeFileType
    };
    function regAll(values) {
        Object.values(values)
            .forEach(register);
    }
    regAll(_allApplication);
    regAll(_allAudio);
    regAll(_allChemical);
    regAll(_allFont);
    regAll(_allImage);
    regAll(_allMessage);
    regAll(_allModel);
    regAll(_allMultipart);
    regAll(_allText);
    regAll(_allVideo);
    regAll(_allXConference);
    regAll(_allXShader);
    return db;
});
//# sourceMappingURL=db.js.map