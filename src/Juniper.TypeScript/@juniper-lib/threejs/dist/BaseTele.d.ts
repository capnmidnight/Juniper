/// <reference types="webxr" />
import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { TeleconferenceManager } from "@juniper-lib/webrtc/dist/TeleconferenceManager";
import { Object3D, Vector3 } from "three";
import { AvatarRemote } from "./AvatarRemote";
import { Application } from "./environment/Application";
import type { Environment } from "./environment/Environment";
export declare const HANDEDNESSES: XRHandedness[];
export declare abstract class BaseTele extends Application {
    private readonly sortedUserIDs;
    readonly avatars: Map<string, AvatarRemote>;
    readonly remoteUsers: Object3D<import("three").Event>;
    conference: TeleconferenceManager;
    private avatarModel;
    private avatarNameTagFont;
    private userType;
    private userName;
    private meetingID;
    private roomName;
    private _offsetRadius;
    private readonly doorOpenSound;
    private readonly doorCloseSound;
    private readonly avatarModelAsset;
    private readonly assets;
    constructor(env: Environment);
    init(params: Map<string, unknown>): Promise<void>;
    load(prog?: IProgress): Promise<void>;
    protected abstract createConference(): TeleconferenceManager;
    showing(_onProgress?: IProgress): Promise<void>;
    dispose(): void;
    protected hiding(): void;
    get visible(): boolean;
    setConferenceInfo(userType: string, userName: string, meetingID: string): Promise<void>;
    join(roomName: string): Promise<void>;
    private withUser;
    get offsetRadius(): number;
    set offsetRadius(v: number);
    updateUserOffsets(): void;
    /**
     * Set the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     * @param x - the horizontal component of the offset.
     * @param y - the vertical component of the offset.
     * @param z - the lateral component of the offset.
     */
    private setUserOffset;
    /**
     * Get the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     */
    getUserOffset(id: string): Vector3;
    private checkConference;
}
//# sourceMappingURL=BaseTele.d.ts.map