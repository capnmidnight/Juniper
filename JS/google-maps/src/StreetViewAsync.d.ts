import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
type StreetViewAsyncEvents = {
    positionchanged: TypedEvent<"positionchanged">;
    _positionchanged: TypedEvent<"_positionchanged">;
};
export declare class StreetViewAsync extends TypedEventTarget<StreetViewAsyncEvents> {
    private streetView;
    private publicChangedEvt;
    private privateChangedEvt;
    private firePublicEvt;
    constructor(streetView: google.maps.StreetViewPanorama);
    private hasChanged;
    searchPano(pano: string): Promise<void>;
    setPano(pano: string): Promise<void>;
    close(): void;
    getPano(): string;
    getLocation(): google.maps.StreetViewLocation;
    get visible(): boolean;
    set visible(v: boolean);
}
export {};
//# sourceMappingURL=StreetViewAsync.d.ts.map