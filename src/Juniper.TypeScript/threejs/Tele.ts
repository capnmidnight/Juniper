import type { TextImageOptions } from "juniper-2d/TextImage";
import type { IProgress, TimerTickEvent } from "juniper-tslib";
import {
    isDefined,
    progressTasks,
    TypedEventBase
} from "juniper-tslib";
import { RoomJoinedEvent, RoomLeftEvent, UserJoinedEvent, UserLeftEvent, UserNameChangedEvent } from "juniper-webrtc/ConferenceEvents";
import { TeleconferenceManager } from "juniper-webrtc/TeleconferenceManager";
import { AvatarRemote } from "./AvatarRemote";
import { cleanup } from "./cleanup";
import { DebugObject } from "./DebugObject";
import type { Application, ApplicationEvents } from "./environment/Application";
import type { Environment } from "./environment/Environment";

export class Tele
    extends TypedEventBase<ApplicationEvents>
    implements Application {

    readonly users = new Map<string, AvatarRemote>();
    readonly remoteUsers = new THREE.Object3D();

    conference: TeleconferenceManager = null;

    private defaultAvatarHeight = 1.75;
    private avatarModel: THREE.Object3D = null;
    private avatarNameTagFont: Partial<TextImageOptions> = null;
    private hubName: string = null;
    private userType: string = null;
    private userName: string = null;
    private meetingID: string = null;
    private roomName: string = null;

    constructor(private readonly env: Environment) {
        super();
    }

    get ready() {
        return this.conference && this.conference.ready;
    }

    async init(params: Map<string, unknown>): Promise<void> {
        this.defaultAvatarHeight = params.get("defaultAvatarHeight") as number;
        if (!this.defaultAvatarHeight) {
            throw new Error("Missing defaultAvatarHeight parameter");
        }

        this.hubName = params.get("hub") as string;
        if (!this.hubName) {
            throw new Error("Missing hub parameter");
        }

        this.avatarNameTagFont = params.get("nameTagFont") as Partial<TextImageOptions>;
        if (!this.avatarNameTagFont) {
            throw new Error("Missing nameTagFont parameter");
        }

        this.env.avatar.addEventListener("avatarmoved", (evt) =>
            this.conference.setLocalPose(
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz,
                evt.height));

        this.env.eventSystem.addEventListener("objectMoved", (evt) => {
            this.conference.setLocalPointer(
                evt.name,
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz);
        });

        this.env.addEventListener("newcursorloaded", () => {
            for (const user of this.users.values()) {
                user.refreshCursors();
            }
        });

        this.env.addEventListener("roomjoined", (evt) => this.join(evt.roomName));
        this.env.addEventListener("sceneclearing", () => this.env.foreground.remove(this.remoteUsers));
        this.env.addEventListener("scenecleared", () => this.env.foreground.add(this.remoteUsers));

        this.env.muteMicButton.visible = true;
        this.env.muteMicButton.addEventListener("click", async () => {
            this.conference.audioMuted
                = this.env.muteMicButton.active
                = !this.conference.audioMuted;
        });

        this.remoteUsers.name = "Remote Users";

        this.env.audio.offsetRadius = 1.25;

        this.conference = new TeleconferenceManager(this.env.audio, this.hubName, false);

        const onLocalUserIDChange = (evt: RoomJoinedEvent | RoomLeftEvent) => {
            this.env.avatar.name = evt.userID;
        };

        this.conference.addEventListener("roomJoined", onLocalUserIDChange);
        this.conference.addEventListener("roomLeft", onLocalUserIDChange);

        this.conference.addEventListener("userJoined", (evt: UserJoinedEvent) => {
            const user = new AvatarRemote(
                this.env.eventSystem,
                this.env.stage,
                this.env.audio.audioCtx,
                this.env.avatar,
                this.env.cursor3D,
                evt.user,
                evt.source,
                this.avatarNameTagFont,
                this.defaultAvatarHeight);
            const avatar = this.avatarModel
                ? this.avatarModel.clone()
                : new DebugObject(0xffff00);
            user.setAvatar(avatar);
            user.userName = evt.user.userName;
            this.users.set(evt.user.userID, user);
            this.remoteUsers.add(user);
            this.env.audio.playClip("join");
        });

        this.conference.addEventListener("userNameChanged", (evt: UserNameChangedEvent) => {
            const user = this.users.get(evt.user.userID);
            if (user) {
                user.userName = evt.newUserName;
            }
        });

        this.conference.addEventListener("userLeft", (evt: UserLeftEvent) => {
            const user = this.users.get(evt.user.userID);
            if (user) {
                this.remoteUsers.remove(user);
                this.users.delete(evt.user.userID);
                cleanup(user);
                this.env.audio.playClip("leave");
            }
        });
    }

    async show(_onProgress?: IProgress): Promise<void> {
        this.env.foreground.add(this.remoteUsers);
        if (isDefined(this.env.currentRoom)) {
            await this.join(this.env.currentRoom);
        }
    }

    dispose(): void {
        this.env.foreground.remove(this.remoteUsers);
    }

    async loadAvatar(path: string, onProgress?: IProgress) {
        this.avatarModel = await this.env.loadModel(path, onProgress);
        this.avatarModel = this.avatarModel.children[0];
        for (const id of this.users.keys()) {
            const user = this.users.get(id);
            const oldAvatar = user.avatar;
            user.setAvatar(this.avatarModel.clone());
            cleanup(oldAvatar);
        }
    }

    async setConferenceInfo(userType: string, userName: string, meetingID: string): Promise<void> {
        this.userType = userType;
        this.userName = userName;
        this.meetingID = meetingID;
        await this.checkConference();
    }

    async join(roomName: string): Promise<void> {
        this.roomName = roomName;
        await this.checkConference();
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

    async load(onProgress?: IProgress) {
        await progressTasks(onProgress,
            prog => this.env.audio.createBasicClip("join", "/audio/door_open.mp3", 0.25, prog),
            prog => this.env.audio.createBasicClip("leave", "/audio/door_close.mp3", 0.25, prog),
            prog => this.loadAvatar("/models/Avatar.glb", prog));
    }

    update(evt: TimerTickEvent) {
        for (const user of this.users.values()) {
            user.update(evt.dt);
        }
    }
}