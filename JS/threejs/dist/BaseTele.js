import { arrayRemove, compareBy, dispose, identity, insertSorted, isDefined, Tau } from "@juniper-lib/util";
import { AssetFile } from "@juniper-lib/fetcher";
import { Audio_Mpeg, Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import { AssetGltfModel } from "./AssetGltfModel";
import { AvatarRemote } from "./AvatarRemote";
import { BufferReaderWriter } from "./BufferReaderWriter";
import { DebugObject } from "./DebugObject";
import { cleanup } from "./cleanup";
import { Application } from "./environment/Application";
import { convertMaterials, materialStandardToPhong } from "./materials";
import { obj, objGraph } from "./objects";
export const HANDEDNESSES = [
    "none",
    "right",
    "left"
];
const comparer = compareBy(identity);
export class BaseTele extends Application {
    constructor(env) {
        super(env);
        this.sortedUserIDs = new Array();
        this.avatars = new Map();
        this.remoteUsers = obj("RemoteUsers");
        this.conference = null;
        this.avatarModel = null;
        this.avatarNameTagFont = null;
        this.userType = null;
        this.userName = null;
        this.meetingID = null;
        this.roomName = null;
        this._offsetRadius = 0;
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
    async init(params) {
        this.avatarNameTagFont = params.get("nameTagFont");
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
        this.env.addScopedEventListener(this, "sceneclearing", () => this.hiding());
        this.env.addScopedEventListener(this, "scenecleared", () => objGraph(this.env.foreground, this.remoteUsers));
        this.env.muteMicButton.visible = true;
        this.env.muteCamButton.visible = true;
        this.remoteUsers.name = "Remote Users";
        this.offsetRadius = 1.25;
        this.conference = this.createConference();
        const DT = 33;
        let t = 0;
        const buffer = new BufferReaderWriter();
        this.env.addScopedEventListener(this, "update", (evt) => {
            t += evt.dt;
            if (t >= DT) {
                t -= DT;
                this.env.avatar.writeState(buffer);
                this.conference.sendUserState(buffer.buffer);
            }
        });
        const onLocalUserIDChange = (evt) => {
            arrayRemove(this.sortedUserIDs, this.env.avatar.name);
            this.env.avatar.name = evt.userID;
            insertSorted(this.sortedUserIDs, this.env.avatar.name, comparer);
            this.updateUserOffsets();
        };
        this.conference.addScopedEventListener(this, "roomJoined", onLocalUserIDChange);
        this.conference.addScopedEventListener(this, "roomLeft", onLocalUserIDChange);
        this.conference.addScopedEventListener(this, "userJoined", (evt) => {
            const model = this.avatarModel
                ? this.avatarModel.clone()
                : new DebugObject(0xffff00);
            const avatar = new AvatarRemote(this.env, evt.user, evt.source, model, this.env.defaultAvatarHeight, this.avatarNameTagFont);
            avatar.userName = evt.user.userName;
            this.avatars.set(evt.user.userID, avatar);
            insertSorted(this.sortedUserIDs, evt.user.userID, comparer);
            objGraph(this.remoteUsers, avatar);
            this.updateUserOffsets();
            this.env.audio.playClip("join");
        });
        this.conference.addScopedEventListener(this, "trackAdded", (evt) => {
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
        this.conference.addScopedEventListener(this, "trackRemoved", (evt) => {
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
        this.conference.addScopedEventListener(this, "userNameChanged", (evt) => {
            const user = this.avatars.get(evt.user.userID);
            if (user) {
                user.userName = evt.newUserName;
            }
        });
        this.conference.addScopedEventListener(this, "userLeft", (evt) => {
            const user = this.avatars.get(evt.user.userID);
            if (user) {
                user.content3d.removeFromParent();
                this.avatars.delete(evt.user.userID);
                arrayRemove(this.sortedUserIDs, evt.user.userID);
                cleanup(user);
                this.updateUserOffsets();
                this.env.audio.playClip("leave");
            }
        });
        if (this.env.speech) {
            this.env.speech.addScopedEventListener(this, "result", (evt) => {
                if (this.visible) {
                    this.conference.sendChat(evt.results);
                }
            });
        }
        return await super.init(params);
    }
    async load(prog) {
        await this.env.fetcher.assets(prog, ...this.assets);
        await Promise.all([
            this.env.audio.createBasicClip("join", this.doorOpenSound, 0.25),
            this.env.audio.createBasicClip("leave", this.doorCloseSound, 0.25)
        ]);
        this.avatarModel = this.avatarModelAsset.result.scene.children[0];
        convertMaterials(this.avatarModel, materialStandardToPhong);
    }
    async showing(_onProgress) {
        objGraph(this.env.foreground, this.remoteUsers);
        if (isDefined(this.env.currentRoom)) {
            await this.join(this.env.currentRoom);
        }
    }
    dispose() {
        this.hiding();
        if (this.env.speech) {
            this.env.speech.removeScope(this);
        }
        this.env.avatar.removeScope(this);
        this.env.eventSys.removeScope(this);
        this.env.removeScope(this);
        this.conference.removeScope(this);
        dispose(this.conference);
    }
    hiding() {
        this.remoteUsers.removeFromParent();
    }
    get visible() {
        return isDefined(this.remoteUsers.parent);
    }
    async setConferenceInfo(userType, userName, meetingID) {
        this.userType = userType;
        this.userName = userName;
        this.meetingID = meetingID;
        await this.checkConference();
    }
    async join(roomName) {
        this.roomName = roomName;
        await this.checkConference();
    }
    withUser(id, action) {
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
    updateUserOffsets() {
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
    setUserOffset(id, x, y, z) {
        this.withUser(id, (user) => user.comfortOffset.set(x, y, z));
    }
    /**
     * Get the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     */
    getUserOffset(id) {
        return this.withUser(id, (user) => user.comfortOffset);
    }
    async checkConference() {
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
//# sourceMappingURL=BaseTele.js.map