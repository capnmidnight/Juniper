import type { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { MediaType, Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import { arrayRemove, arraySortedInsert } from "@juniper-lib/tslib/collections/arrays";
import { Task } from "@juniper-lib/tslib/events/Task";
import { Tau } from "@juniper-lib/tslib/math";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { progressTasks } from "@juniper-lib/tslib/progress/progressTasks";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import {
    RoomJoinedEvent,
    RoomLeftEvent,
    UserJoinedEvent,
    UserLeftEvent,
    UserNameChangedEvent
} from "@juniper-lib/webrtc/ConferenceEvents";
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
    readonly users = new Map<string, AvatarRemote>();
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

    constructor(env: Environment) {
        super(env);

        this.env.addScopedEventListener(this, "update", (evt) => {
            for (const user of this.users.values()) {
                user.update(evt.dt);
            }
        });
    }

    public readonly ready = new Task<void>();

    async init(params: Map<string, unknown>): Promise<void> {
        this.defaultAvatarHeight = params.get("defaultAvatarHeight") as number;
        if (!this.defaultAvatarHeight) {
            throw new Error("Missing defaultAvatarHeight parameter");
        }

        this.avatarNameTagFont = params.get("nameTagFont") as Partial<TextImageOptions>;
        if (!this.avatarNameTagFont) {
            throw new Error("Missing nameTagFont parameter");
        }

        this.env.addScopedEventListener(this, "newcursorloaded", () => {
            for (const user of this.users.values()) {
                user.refreshCursors();
            }
        });

        this.env.addScopedEventListener(this, "roomjoined", (evt) => {
            console.log(evt);
            this.join(evt.roomName);
        });
        this.env.addScopedEventListener(this, "sceneclearing", () => this.env.foreground.remove(this.remoteUsers));
        this.env.addScopedEventListener(this, "scenecleared", () => objGraph(this.env.foreground, this.remoteUsers));

        this.env.muteMicButton.visible = true;
        this.env.muteMicButton.addEventListener("click", async () => {
            this.conference.audioMuted
                = this.env.muteMicButton.active
                = !this.conference.audioMuted;
        });

        this.remoteUsers.name = "Remote Users";

        this.offsetRadius = 1.25;

        this.conference = this.createConference();
        this.conference.ready.then(this.ready.resolver());

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
            const { id, origin, direction, up } = evt.pointer;
            this.conference.setLocalPointer(
                id,
                origin.x, origin.y, origin.z,
                direction.x, direction.y, direction.z,
                up.x, up.y, up.z);
        });

        this.conference.addScopedEventListener(this, "roomJoined", onLocalUserIDChange);
        this.conference.addScopedEventListener(this, "roomLeft", onLocalUserIDChange);

        this.conference.addScopedEventListener(this, "userJoined", (evt: UserJoinedEvent) => {
            const avatar = this.avatarModel
                ? this.avatarModel.clone()
                : new DebugObject(0xffff00);
            const user = new AvatarRemote(
                this.env,
                evt.user,
                evt.source,
                avatar,
                this.defaultAvatarHeight,
                this.avatarNameTagFont);

            user.userName = evt.user.userName;
            this.users.set(evt.user.userID, user);
            arraySortedInsert(this.sortedUserIDs, evt.user.userID);
            objGraph(this.remoteUsers, user);

            this.updateUserOffsets();
            this.env.audio.playClip("join");
        });

        this.conference.addScopedEventListener(this, "userNameChanged", (evt: UserNameChangedEvent) => {
            const user = this.users.get(evt.user.userID);
            if (user) {
                user.userName = evt.newUserName;
            }
        });

        this.conference.addScopedEventListener(this, "userLeft", (evt: UserLeftEvent) => {
            const user = this.users.get(evt.user.userID);
            if (user) {
                this.remoteUsers.remove(user);
                this.users.delete(evt.user.userID);
                arrayRemove(this.sortedUserIDs, evt.user.userID);
                cleanup(user);
                this.updateUserOffsets();
                this.env.audio.playClip("leave");
            }
        });
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

    async loadAvatar(path: string, type: string | MediaType, prog?: IProgress) {
        const avatarAsset = new AssetGltfModel(this.env, path, type, !this.env.DEBUG);
        await this.env.fetcher.assets(prog, avatarAsset);
        this.avatarModel = avatarAsset.result.scene.children[0];
        convertMaterials(this.avatarModel, materialStandardToPhong);
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
        if (this.users.has(id)) {
            const user = this.users.get(id);
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

    async load(prog?: IProgress) {
        await progressTasks(prog,
            (prog) => this.env.audio.loadBasicClip("join", "/audio/door_open.mp3", 0.25, prog),
            (prog) => this.env.audio.loadBasicClip("leave", "/audio/door_close.mp3", 0.25, prog),
            (prog) => this.loadAvatar("/models/Avatar.glb", Model_Gltf_Binary, prog));
    }
}