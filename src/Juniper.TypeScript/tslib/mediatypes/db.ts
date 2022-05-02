import { singleton } from "../singleton";
import { isDefined, isString } from "../typeChecks";
import * as _allApplication from "./application";
import * as _allAudio from "./audio";
import * as _allChemical from "./chemical";
import * as _allFont from "./font";
import * as _allImage from "./image";
import * as _allMessage from "./message";
import * as _allModel from "./model";
import * as _allMultipart from "./multipart";
import * as _allText from "./text";
import { MediaType, typePattern } from "./util";
import * as _allVideo from "./video";
import * as _allXConference from "./xConference";
import * as _allXShader from "./xShader";

export const MediaTypeDB = singleton("Juniper:TSLib:MediaTypeDB", () => {

    const byValue = new Map<string, MediaType>();
    const byExtension = new Map<string, MediaType[]>();


    function parse(value: string): MediaType {
        if (!value) {
            return null;
        }

        const match = value.match(typePattern);
        if (!match) {
            return null;
        }

        const type = match[1];
        const subType = match[2];
        const parsedType = new MediaType(type, subType);
        const weight = parsedType.parameters.get("q");
        const basicType = byValue.get(parsedType.value)
            || parsedType;

        if (isDefined(weight)) {
            return basicType.withParameter("q", weight);
        }
        else {
            return basicType;
        }
    }

    function matches(pattern: string | MediaType, value: string | MediaType): boolean {
        if (isString(pattern)) {
            pattern = parse(pattern);
        }

        return pattern.matches(value);
    }

    function matchesFileName(pattern: string | MediaType, fileName: string): boolean {
        if (!fileName) {
            return false;
        }

        if (isString(pattern)) {
            pattern = parse(pattern);
        }

        const types = guessByFileName(fileName);
        for (const type of types) {
            if (pattern.matches(type)) {
                return true;
            }
        }

        return false;
    }

    function guessByFileName(fileName: string): MediaType[] {
        if (!fileName) {
            console.warn("Couldn't guess media type. Must provide a valid fileName.");
            return [];
        }

        const idx = fileName.lastIndexOf(".");
        if (idx === -1) {
            console.warn("Couldn't guess media type. FileName has no extension.");
            return [];
        }

        const ext = fileName.substring(idx);
        return guessByExtension(ext);
    }

    function guessByExtension(ext: string): MediaType[] {
        if (!ext) {
            ext = "unknown";
        }
        else if (ext[0] == '.') {
            ext = ext.substring(1);
        }

        if (byExtension.has(ext)) {
            return byExtension.get(ext);
        }
        else {
            return [new MediaType("unknown", ext, [ext])];
        }
    }

    function normalizeFileType(fileName: string, fileType: string): string {
        if (!fileType && fileName.indexOf(".") > -1) {
            const guesses = guessByFileName(fileName);
            if (guesses.length > 0) {
                fileType = guesses[0].value;
            }
        }

        return fileType;
    }

    function register(type: MediaType): MediaType {
        let isNew = false;
        const value = type.__getValueUnsafe();
        type = singleton("Juniper.MediaTypes:" + value, () => {
            isNew = true;
            return type;
        });

        if (isNew) {
            byValue.set(value, type);

            for (const ext of type.__getExtensionsUnsafe()) {
                if (!byExtension.has(ext)) {
                    byExtension.set(ext, new Array<MediaType>());
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
        parse,
        matches,
        matchesFileName,
        guessByFileName,
        guessByExtension,
        normalizeFileType,
        register
    };

    function regAll(values: any) {
        Object.values<MediaType>(values)
            .forEach(v => db.register(v));
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