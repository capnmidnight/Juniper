import { isString } from "@juniper-lib/util";
export function makeLookup(descriptions) {
    const idLookup = {}, descLookup = {}, all = Object.keys(descriptions)
        .map((id) => {
        const idValue = parseInt(id, 16), descValue = descriptions[id];
        idLookup[descValue] = idValue;
        descLookup[idValue] = descValue;
        return idValue;
    });
    function description(id) {
        /*
          The UUIDs that GATT uses are quite long, unnecessarily so. The first 8 digits
          (4 bytes) are guaranteed to be unique amongst all Services, Characteristics,
          and Descriptors, because there just aren't that many of them.
        */
        if (isString(id)) {
            id = parseInt(id.substring(0, 8).toLocaleUpperCase(), 16);
        }
        return descLookup[id] || id;
    }
    function id(description) {
        return idLookup[description];
    }
    return {
        all,
        id,
        description
    };
}
//# sourceMappingURL=makeLookup.js.map