import { AssetFile, BaseFetchedAsset } from "@juniper-lib/fetcher/Asset";
import type { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { Audio_Mpeg, Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import { arrayRemove, arraySortedInsert } from "@juniper-lib/tslib/collections/arrays";
import { all } from "@juniper-lib/tslib/events/all";
import { PointerID } from "@juniper-lib/tslib/events/Pointers";
import { Tau } from "@juniper-lib/tslib/math";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import {
    RoomJoinedEvent,
    RoomLeftEvent,
    UserJoinedEvent,
    UserLeftEvent,
    UserNameChangedEvent
} from "@juniper-lib/webrtc/ConferenceEvents";
import { RemoteUserTrackAddedEvent, RemoteUserTrackRemovedEvent } from "@juniper-lib/webrtc/RemoteUser";
import { TeleconferenceManager } from "@juniper-lib/webrtc/TeleconferenceManager";
import { Object3D, Vector3 } from "three";
import { AssetGltfModel } from "./AssetGltfModel";
import { AvatarRemote } from "./AvatarRemote";
import { cleanup } from "./cleanup";
import { DebugObject } from "./DebugObject";
import { Application } from "./environment/Application";
import type { Environment } from "./environment/Environment";
import { convertMaterials, materialStandardToPhong } from "./materials";
import { obj, objGraph } from "./objects";

export abstract class BaseTele extends Application {

    private readonly sortedUserIDs = new Array<string>();
    readonly avatars = new Map<string, AvatarRemote>();
    readonly remoteUsers = obj("RemoteUsers");

    conference: TeleconferenceManager = null;

    private defaultAvatarHeight = 1.75;
    private avatarModel: Object3D = null;
    private avatarNameTagFont: Partial<TextImageOptions> = null;
    private userType: string = null;
    private userName: string = null;
    private meetingID: string = null;
    private roomName: string = null;
    private _offsetRadius = 0;


    private readonly doorOpenSound: AssetFile;
    private readonly doorCloseSound: AssetFile;
    private readonly avatarModelAsset: AssetGltfModel;

    private readonly assets: ReadonlyArray<BaseFetchedAsset<any>>;

    constructor(env: Environment) {
        super(env);

        this.doorOpenSound = new AssetFile("/audio/door_open.mp3", Audio_Mpeg, !this.env.DEBUG);
        this.doorCloseSound = new AssetFile("/audio/door_close.mp3", Audio_Mpeg, !this.env.DEBUG);
        this.avatarModelAsset = new AssetGltfModel(this.env, "/models/Avatar.glb", Model_Gltf_Binary, !this.env.DEBUG);
        this.assets = [
            this.doorOpenSound,
            this.doorCloseSound,
            this.avatarModelAsset
        ];

        this.env.addScopedEventListener(this, "update", (evt) => {
            for (const user of this.avatars.values()) {
                user.update(evt.dt);
            }
        });
    }

    override async init(params: Map<string, unknown>): Promise<void> {
        this.defaultAvatarHeight = params.get("defaultAvatarHeight") as number;
        if (!this.defaultAvatarHeight) {
            throw new Error("Missing defaultAvatarHeight parameter");
        }

        this.avatarNameTagFont = params.get("nameTagFont") as Partial<TextImageOptions>;
        if (!this.avatarNameTagFont) {
            throw new Error("Missing nameTagFont parameter");
        }

        this.env.addScopedEventListener(this, "newcursorloaded", () => {
            for (const user of this.avatars.values()) {
                user.refreshCursors();
            }
        });

        this.env.addScopedEventListener(this, "roomjoined", (evt) => {
            this.join(evt.roomName);
        });

        this.env.addScopedEventListener(this, "sceneclearing", () => this.env.foreground.remove(this.remoteUsers));
        this.env.addScopedEventListener(this, "scenecleared", () => objGraph(this.env.foreground, this.remoteUsers));

        this.env.muteMicButton.visible = true;
        this.env.muteCamButton.visible = true;

        this.remoteUsers.name = "Remote Users";

        this.offsetRadius = 1.25;

        this.conference = this.createConference();

        const onLocalUserIDChange = (evt: RoomJoinedEvent | RoomLeftEvent) => {
            arrayRemove(this.sortedUserIDs, this.env.avatar.name);
            this.env.avatar.name = evt.userID;
            arraySortedInsert(this.sortedUserIDs, this.env.avatar.name);
            this.updateUserOffsets();
        };

        this.env.avatar.addScopedEventListener(this, "avatarmoved", (evt) => {
            this.conference.setLocalPose(
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz,
                evt.height);
        });

        this.env.eventSys.addScopedEventListener(this, "move", (evt) => {
            if (evt.pointer.id !== PointerID.Nose) {
                const { id, origin, direction, up } = evt.pointer;
                this.conference.setLocalPointer(
                    id,
                    origin.x, origin.y, origin.z,
                    direction.x, direction.y, direction.z,
                    up.x, up.y, up.z);
            }
        });

        this.conference.addScopedEventListener(this, "roomJoined", onLocalUserIDChange);
        this.conference.addScopedEventListener(this, "roomLeft", onLocalUserIDChange);

        this.conference.addScopedEventListener(this, "userJoined", (evt: UserJoinedEvent) => {
            const model = this.avatarModel
                ? this.avatarModel.clone()
                : new DebugObject(0xffff00);
            const avatar = new AvatarRemote(
                this.env,
                evt.user,
                evt.source,
                model,
                this.defaultAvatarHeight,
                this.avatarNameTagFont);

            avatar.userName = evt.user.userName;
            this.avatars.set(evt.user.userID, avatar);
            arraySortedInsert(this.sortedUserIDs, evt.user.userID);
            objGraph(this.remoteUsers, avatar);

            this.updateUserOffsets();
            this.env.audio.playClip("join");
        });

        this.conference.addScopedEventListener(this, "trackAdded", (evt: RemoteUserTrackAddedEvent) => {
            if (evt.stream.getAudioTracks().length > 0) {
                this.env.audio.setUserStream(evt.user.userID, evt.stream);
            }

            if (evt.stream.getVideoTracks().length > 0) {
                const user = this.avatars.get(evt.user.userID);
                if (user) {
                    user.videoStream = evt.stream;
                }
            }
        });

        this.conference.addScopedEventListener(this, "trackRemoved", (evt: RemoteUserTrackRemovedEvent) => {
            if (evt.stream.getAudioTracks().length > 0) {
                this.env.audio.setUserStream(evt.user.userID, null);
            }

            if (evt.stream.getVideoTracks().length > 0) {
                const user = this.avatars.get(evt.user.userID);
                if (user) {
                    user.videoStream = null;
                }
            }
        });

        this.conference.addScopedEventListener(this, "userNameChanged", (evt: UserNameChangedEvent) => {
            const user = this.avatars.get(evt.user.userID);
            if (user) {
                user.userName = evt.newUserName;
            }
        });

        this.conference.addScopedEventListener(this, "userLeft", (evt: UserLeftEvent) => {
            const user = this.avatars.get(evt.user.userID);
            if (user) {
                this.remoteUsers.remove(user);
                this.avatars.delete(evt.user.userID);
                arrayRemove(this.sortedUserIDs, evt.user.userID);
                cleanup(user);
                this.updateUserOffsets();
                this.env.audio.playClip("leave");
            }
        });

        return await super.init(params);
    }

    async load(prog?: IProgress) {
        await this.env.fetcher.assets(prog, ...this.assets);
        await all(
            this.env.audio.createBasicClip("join", this.doorOpenSound, 0.25),
            this.env.audio.createBasicClip("leave", this.doorCloseSound, 0.25)
        );
        this.avatarModel = this.avatarModelAsset.result.scene.children[0];
        convertMaterials(this.avatarModel, materialStandardToPhong);
    }

    protected abstract createConference(): TeleconferenceManager;

    async showing(_onProgress?: IProgress): Promise<void> {
        objGraph(this.env.foreground, this.remoteUsers);
        if (isDefined(this.env.currentRoom)) {
            await this.join(this.env.currentRoom);
        }
    }

    dispose(): void {
        this.hiding();
        this.env.avatar.removeScope(this);
        this.env.eventSys.removeScope(this);
        this.env.removeScope(this);
        this.conference.removeScope(this);
        this.conference.dispose();
    }

    protected hiding() {
        this.env.foreground.remove(this.remoteUsers);
    }

    get visible() {
        return isDefined(this.remoteUsers.parent);
    }

    async setConferenceInfo(userType: string, userName: string, meetingID: string): Promise<void> {
        this.userType = userType;
        this.userName = userName;
        this.meetingID = meetingID;
        await this.checkConference();
    }

    override async join(roomName: string): Promise<void> {
        this.roomName = roomName;
        await this.checkConference();
    }

    private withUser<T>(id: string, action: (user: AvatarRemote) => T): T {
        if (this.avatars.has(id)) {
            const user = this.avatars.get(id);
            if (isDefined(user)) {
                return action(user);
            }
        }

        return null;
    }

    get offsetRadius() {
        return this._offsetRadius;
    }

    set offsetRadius(v) {
        this._offsetRadius = v;
        this.updateUserOffsets();
    }

    updateUserOffsets(): void {
        if (this.offsetRadius > 0) {
            const idx = this.sortedUserIDs.indexOf(this.env.avatar.name);
            const dRadians = Tau / this.sortedUserIDs.length;
            const localRadians = (idx + 1) * dRadians;
            const dx = this.offsetRadius * Math.sin(localRadians);
            const dy = this.offsetRadius * (Math.cos(localRadians) - 1);
            for (let i = 0; i < this.sortedUserIDs.length; ++i) {
                const id = this.sortedUserIDs[i];
                const radians = (i + 1) * dRadians;
                const x = this.offsetRadius * Math.sin(radians) - dx;
                const z = this.offsetRadius * (Math.cos(radians) - 1) - dy;
                this.setUserOffset(id, x, 0, z);
            }
        }
    }

    /**
     * Set the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     * @param x - the horizontal component of the offset.
     * @param y - the vertical component of the offset.
     * @param z - the lateral component of the offset.
     */
    private setUserOffset(id: string, x: number, y: number, z: number): void {
        this.withUser(id, (user) =>
            user.comfortOffset.set(x, y, z));
    }

    /**
     * Get the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     */
    public getUserOffset(id: string): Vector3 {
        return this.withUser(id, (user) => user.comfortOffset);
    }

    private async checkConference(): Promise<void> {
        if (isDefined(this.userType)
            && isDefined(this.userName)
            && isDefined(this.meetingID)
            && isDefined(this.roomName)) {

            const isoRoomName = `${this.roomName}_${this.meetingID}`.toLocaleLowerCase();
            const isoUserName = `${this.userType}_${this.userName}`;
            if (this.conference.roomName !== isoRoomName) {
                await this.conference.connect();
                await this.conference.identify(isoUserName);
                await this.conference.join(isoRoomName);
            }
        }
    }
}
