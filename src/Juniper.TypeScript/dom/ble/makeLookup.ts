import { isString } from "juniper-tslib/typeChecks";

export function makeLookup(descriptions: { [key: string]: string }) {
    const idLookup: { [key: string]: number } = {},
        descLookup: { [key: number]: string } = {},
        all = Object.keys(descriptions)
            .map((id) => {
                const idValue = parseInt(id, 16),
                    descValue = descriptions[id];
                idLookup[descValue] = idValue;
                descLookup[idValue] = descValue;
                return idValue;
            });

    function description(id: string | number) {
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

    function id(description: string) {
        return idLookup[description];
    }

    return {
        all,
        id,
        description
    };
}
