import { ActivityDetector } from "@juniper-lib/audio/dist/ActivityDetector";
import { AutoPlay, SrcObject } from "@juniper-lib/dom/dist/attrs";
import { getMonospaceFonts } from "@juniper-lib/dom/dist/css";
import { Video } from "@juniper-lib/dom/dist/tags";
import { star } from "@juniper-lib/emoji";
import { FWD, HalfPi } from "@juniper-lib/tslib/dist/math";
import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
import { dispose } from "@juniper-lib/tslib/dist/using";
import { FrontSide, Matrix4, Object3D, Quaternion, Vector3 } from "three";
import { BufferReaderWriter } from "./BufferReaderWriter";
import { BodyFollower } from "./animation/BodyFollower";
import { getLookHeadingRadians } from "./animation/lookAngles";
import { cleanup } from "./cleanup";
import { PointerID } from "./eventSystem/Pointers";
import { PointerRemote } from "./eventSystem/devices/PointerRemote";
import { obj, objGraph } from "./objects";
import { Image2D } from "./widgets/Image2D";
import { TextMesh } from "./widgets/TextMesh";
const nameTagFont = {
    fontFamily: getMonospaceFonts(),
    fontSize: 20,
    fontWeight: "bold",
    textFillColor: "white",
    textStrokeColor: "black",
    textStrokeSize: 0.01,
    padding: {
        top: 0.025,
        right: 0.05,
        bottom: 0.025,
        left: 0.05
    },
    maxHeight: 0.20
};
export class AvatarRemote {
    get object() { return this.avatar; }
    get bodyQuaternion() { return this.headFollower.quaternion; }
    constructor(env, user, source, avatar, defaultAvatarHeight, font) {
        this.env = env;
        this.avatar = avatar;
        this.defaultAvatarHeight = defaultAvatarHeight;
        this._isInstructor = false;
        this.pointers = new Map();
        this.stage = new Object3D();
        this.stagePositionTarget = new Vector3();
        this.stageOrientationTarget = new Quaternion().identity();
        this.headPositionTarget = new Vector3();
        this.headOrientationTarget = new Quaternion().identity();
        this.worldPos = new Vector3();
        this.worldQuat = new Quaternion();
        this.F = new Vector3();
        this.M = new Matrix4();
        this._headSize = 1;
        this._headPulse = 1;
        this.comfortOffset = new Vector3();
        this.videoElement = null;
        this.videoMesh = null;
        this._videoStream = null;
        this.clearChatTimer = null;
        this.height = this.defaultAvatarHeight;
        if (isNullOrUndefined(avatar)) {
            throw new Error("Avatar is required");
        }
        this.avatar = avatar;
        const q = [this.avatar];
        while (q.length > 0) {
            const child = q.shift();
            if (child.name === "Head") {
                this.head = child;
            }
            else if (child.name === "Body") {
                this.body = child;
            }
            if (this.head && this.body) {
                break;
            }
            q.push(...child.children);
        }
        if (!this.head || !this.body) {
            throw new Error("Avatar doesn't have a Head or Body element");
        }
        this.userID = user.userID;
        this.avatar.name = user.userName;
        this.billboard = obj("billboard");
        this.nameTag = new TextMesh(this.env, `nameTag-${user.userName}-${user.userID}`, "none", Object.assign({}, nameTagFont, font));
        this.nameTag.position.y = -0.25;
        this.userName = user.userName;
        this.chatBox = new TextMesh(this.env, `chat-${user.userName}-${user.userID}`, "none", Object.assign({}, nameTagFont, font));
        this.chatBox.position.y = -0.4;
        user.addEventListener("chat", (evt) => this.chatText = evt.text);
        const buffer = new BufferReaderWriter();
        user.addEventListener("userState", (evt) => {
            buffer.buffer = evt.buffer;
            this.readState(buffer);
        });
        user.addEventListener("chat", (evt) => {
            this.chatText = evt.text;
        });
        this.activity = new ActivityDetector(this.env.audio.context);
        this.activity.name = `remote-user-activity-${user.userName}-${user.userID}`;
        source.addEventListener("sourceadded", (evt) => {
            evt.source.connect(this.activity);
            evt.source.name = `remote-user-stream-${user.userName}-${user.userID}`;
        });
        this.headFollower = new BodyFollower("AvatarBody", 0.05, HalfPi, 0, 5);
        objGraph(this.avatar, this.stage, objGraph(this.headFollower, objGraph(this.body, objGraph(this.billboard, this.nameTag, this.chatBox))));
        this.activity.addEventListener("activity", (evt) => {
            this.headPulse = 0.2 * evt.level + 1;
        });
        this.activity.start();
    }
    get audioStream() {
        const source = this.env.audio.getUser(this.userID);
        return source && source.stream || null;
    }
    set audioStream(v) {
        this.env.audio.setUserStream(this.userID, v);
    }
    get videoStream() {
        return this._videoStream;
    }
    set videoStream(v) {
        if (v !== this.videoStream) {
            if (this.videoMesh) {
                this.videoMesh.removeFromParent();
                cleanup(this.videoMesh);
                this.videoMesh = null;
            }
            if (this.videoElement) {
                this.videoElement.pause();
                this.videoElement = null;
            }
            this._videoStream = v;
            if (this.videoStream) {
                this.videoElement = Video(SrcObject(this.videoStream), AutoPlay(true));
                this.videoElement.play();
                this.videoMesh = new Image2D(this.env, `webcam-${this.userID}`, "none", {
                    side: FrontSide
                });
                this.videoMesh.sizeMode = "fixed-height";
                this.videoMesh.scale.setScalar(0.25);
                this.videoMesh.position.z = 0.25;
                this.videoMesh.setTextureMap(this.videoElement);
            }
        }
    }
    dispose() {
        for (const pointerName of this.pointers.keys()) {
            this.removePointer(pointerName);
        }
        this.activity.stop();
        dispose(this.activity);
    }
    get isInstructor() {
        return this._isInstructor;
    }
    get headSize() {
        return this._headSize;
    }
    set headSize(v) {
        this._headSize = v;
        this.refreshHead();
    }
    get headPulse() {
        return this._headPulse;
    }
    set headPulse(v) {
        this._headPulse = v;
        this.refreshHead();
    }
    refreshHead() {
        if (this.head) {
            this.head.scale.setScalar(this.headSize * this.headPulse);
        }
    }
    get userName() {
        return this.nameTag.image.value;
    }
    set userName(name) {
        if (name) {
            const words = name.match(/^(?:((?:student|instructor))_)?([^<>{}"]+)$/i);
            if (words) {
                if (words.length === 2) {
                    this.nameTag.image.value = words[1];
                }
                else if (words.length === 3) {
                    this._isInstructor = words[1]
                        && words[1].toLocaleLowerCase() === "instructor";
                    if (this.isInstructor) {
                        this.nameTag.image.value = star.value + words[2];
                    }
                    else {
                        this.nameTag.image.value = words[2];
                    }
                }
                else {
                    this.nameTag.image.value = "???";
                }
            }
        }
    }
    get chatText() {
        return this.chatBox.image.value;
    }
    set chatText(v) {
        this.chatBox.image.value = v || "";
        if (this.clearChatTimer !== null) {
            clearTimeout(this.clearChatTimer);
            this.clearChatTimer = null;
        }
        if (v && v.length > 0) {
            this.clearChatTimer = setTimeout(() => {
                this.chatText = null;
            }, 3000);
        }
    }
    refreshCursors() {
        for (const pointer of this.pointers.values()) {
            if (pointer.cursor) {
                pointer.cursor = this.env.cursor3D.clone();
            }
        }
    }
    update(dt) {
        const lt = dt * 0.01;
        this.stage.position.lerp(this.stagePositionTarget, lt);
        this.stage.quaternion.slerp(this.stageOrientationTarget, lt);
        this.head.position.lerp(this.headPositionTarget, lt);
        this.head.quaternion.slerp(this.headOrientationTarget, lt);
        this.head.getWorldPosition(this.worldPos);
        this.head.getWorldQuaternion(this.worldQuat);
        this.F.fromArray(FWD)
            .applyQuaternion(this.worldQuat);
        const headingRadians = getLookHeadingRadians(this.F);
        this.env.audio.setUserPose(this.userID, this.worldPos.x, this.worldPos.y, this.worldPos.z, this.worldQuat.x, this.worldQuat.y, this.worldQuat.z, this.worldQuat.w);
        this.headFollower.update(this.worldPos.y - this.avatar.parent.position.y, this.worldPos, headingRadians, dt);
        const scale = this.height / this.defaultAvatarHeight;
        this.headSize = scale;
        this.body.scale.setScalar(scale);
        this.F.copy(this.env.avatar.worldPos);
        this.body.worldToLocal(this.F);
        this.F.sub(this.body.position)
            .normalize()
            .multiplyScalar(0.25);
        this.billboard.position.copy(this.F);
        for (const pointer of this.pointers.values()) {
            pointer.animate(dt);
        }
        this.billboard.lookAt(this.env.avatar.worldPos);
        if (this.videoMesh) {
            if (this.videoStream
                && !this.videoStream.active
                && this.videoMesh.parent) {
                this.videoStream = null;
            }
            if (this.videoStream
                && this.videoStream.active
                && this.videoElement.videoWidth > 0
                && !this.videoMesh.parent) {
                this.billboard.add(this.videoMesh);
            }
            if (this.videoMesh) {
                this.videoMesh.updateTexture();
            }
        }
    }
    assurePointer(id) {
        let pointer = this.pointers.get(id);
        if (!pointer) {
            pointer = new PointerRemote(this, this.env, id);
            this.pointers.set(id, pointer);
            objGraph(this.body, pointer);
            if (pointer.cursor) {
                objGraph(this.env.stage, pointer.cursor);
            }
            if (id === PointerID.MotionControllerLeft
                || id === PointerID.MotionControllerRight) {
                this.removePointersExcept(PointerID.MotionControllerLeft, PointerID.MotionControllerRight);
            }
            else {
                this.removePointersExcept(id);
            }
        }
        pointer.deferExecution(3, () => this.removePointer(id));
        return pointer;
    }
    removePointersExcept(...ids) {
        for (const id of this.pointers.keys()) {
            if (ids.indexOf(id) === -1) {
                this.removePointer(id);
            }
        }
    }
    removePointer(id) {
        const pointer = this.pointers.get(id);
        if (pointer) {
            pointer.object.removeFromParent();
            this.pointers.delete(id);
            if (pointer.cursor) {
                pointer.cursor.object.removeFromParent();
            }
        }
    }
    readState(buffer) {
        buffer.position = 0;
        this.height = buffer.readFloat32();
        buffer.readMatrix512(this.M);
        this.M.decompose(this.stagePositionTarget, this.stageOrientationTarget, this.stage.scale);
        this.stagePositionTarget.add(this.comfortOffset);
        buffer.readMatrix512(this.M);
        this.M.decompose(this.headPositionTarget, this.headOrientationTarget, this.avatar.scale);
        this.headPositionTarget.add(this.comfortOffset);
        const numPointers = buffer.readUint8();
        for (let n = 0; n < numPointers; ++n) {
            const pointerID = buffer.readUint8();
            const pointer = this.assurePointer(pointerID);
            pointer.readState(buffer);
        }
    }
}
//# sourceMappingURL=AvatarRemote.js.map