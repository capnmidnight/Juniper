import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { AudioStreamSource } from "@juniper-lib/audio/sources/AudioStreamSource";
import { autoPlay, srcObject } from "@juniper-lib/dom/attrs";
import { getMonospaceFonts } from "@juniper-lib/dom/css";
import { Video } from "@juniper-lib/dom/tags";
import { star } from "@juniper-lib/emoji";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { FWD, HalfPi } from "@juniper-lib/tslib/math";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable, dispose } from "@juniper-lib/tslib/using";
import { UserChatEvent } from "@juniper-lib/webrtc/ConferenceEvents";
import type { RemoteUser } from "@juniper-lib/webrtc/RemoteUser";
import { FrontSide, Matrix4, Object3D, Quaternion, Vector3 } from "three";
import { BodyFollower } from "./animation/BodyFollower";
import { getLookHeadingRadians } from "./animation/lookAngles";
import { BufferReaderWriter } from "./BufferReaderWriter";
import { cleanup } from "./cleanup";
import type { Environment } from "./environment/Environment";
import { PointerRemote } from "./eventSystem/devices/PointerRemote";
import { PointerID } from "./eventSystem/Pointers";
import { ErsatzObject, obj, objGraph } from "./objects";
import { Image2D } from "./widgets/Image2D";
import { TextMesh } from "./widgets/TextMesh";

const nameTagFont: Partial<TextImageOptions> = {
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

export class AvatarRemote implements ErsatzObject, IDisposable {
    get object() { return this.avatar; }
    private _isInstructor = false;
    private readonly pointers = new Map<PointerID, PointerRemote>();

    private height: number;
    private readonly userID: string;
    private readonly head: Object3D;
    private readonly body: Object3D;
    private readonly headFollower: BodyFollower;

    get bodyQuaternion() { return this.headFollower.quaternion; }

    readonly stage = new Object3D();
    private readonly billboard: Object3D;
    private readonly nameTag: TextMesh;
    private readonly chatBox: TextMesh;
    private readonly activity: ActivityDetector;
    private readonly stagePositionTarget = new Vector3();
    private readonly stageOrientationTarget = new Quaternion().identity();
    private readonly headPositionTarget = new Vector3();
    private readonly headOrientationTarget = new Quaternion().identity();
    readonly worldPos = new Vector3();
    readonly worldQuat = new Quaternion();
    private readonly F = new Vector3();
    private readonly M = new Matrix4();

    private _headSize = 1;
    private _headPulse = 1;

    readonly comfortOffset = new Vector3();

    constructor(
        private readonly env: Environment,
        user: RemoteUser,
        source: AudioStreamSource,
        readonly avatar: Object3D,
        private defaultAvatarHeight: number,
        font: Partial<TextImageOptions>) {

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

        user.addEventListener("chat", (evt: UserChatEvent) =>
            this.chatText = evt.text);

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
            evt.source.name = `remote-user-stream-${user.userName}-${user.userID}`
        });

        this.headFollower = new BodyFollower("AvatarBody", 0.05, HalfPi, 0, 5);

        objGraph(this.avatar,
            this.stage,
            objGraph(this.headFollower,
                objGraph(this.body,
                    objGraph(this.billboard,
                        this.nameTag,
                        this.chatBox))));

        this.activity.addEventListener("activity", (evt) => {
            this.headPulse = 0.2 * evt.level + 1;
        });
        this.activity.start();
    }

    get audioStream(): MediaStream {
        const source = this.env.audio.getUser(this.userID);
        return source && source.stream || null;
    }

    set audioStream(v: MediaStream) {
        this.env.audio.setUserStream(this.userID, v);
    }

    private videoElement: HTMLVideoElement = null;
    private videoMesh: Image2D = null;
    private _videoStream: MediaStream = null;
    get videoStream(): MediaStream {
        return this._videoStream;
    }

    set videoStream(v: MediaStream) {
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

                this.videoElement = Video(
                    srcObject(this.videoStream),
                    autoPlay(true)
                );

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

    private get headSize() {
        return this._headSize;
    }

    private set headSize(v) {
        this._headSize = v;
        this.refreshHead();
    }

    private get headPulse() {
        return this._headPulse;
    }

    private set headPulse(v) {
        this._headPulse = v;
        this.refreshHead();
    }

    private refreshHead() {
        if (this.head) {
            this.head.scale.setScalar(this.headSize * this.headPulse);
        }
    }

    get userName(): string {
        return this.nameTag.image.value;
    }

    set userName(name: string) {
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

    get chatText(): string {
        return this.chatBox.image.value;
    }

    private clearChatTimer: number = null;
    set chatText(v: string) {
        this.chatBox.image.value = v || "";

        if (this.clearChatTimer !== null) {
            clearTimeout(this.clearChatTimer);
            this.clearChatTimer = null;
        }

        if (v && v.length > 0) {
            this.clearChatTimer = setTimeout(() => {
                this.chatText = null;
            }, 3000) as any;
        }
    }

    refreshCursors() {
        for (const pointer of this.pointers.values()) {
            if (pointer.cursor) {
                pointer.cursor = this.env.cursor3D.clone();
            }
        }
    }

    update(dt: number) {
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

        this.env.audio.setUserPose(
            this.userID,
            this.worldPos.x, this.worldPos.y, this.worldPos.z,
            this.worldQuat.x, this.worldQuat.y, this.worldQuat.z, this.worldQuat.w);

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

    private assurePointer(id: PointerID): PointerRemote {
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

    private removePointersExcept(...ids: PointerID[]): void {
        for (const id of this.pointers.keys()) {
            if (ids.indexOf(id) === -1) {
                this.removePointer(id);
            }
        }
    }

    private removePointer(id: PointerID): void {
        const pointer = this.pointers.get(id);
        if (pointer) {
            pointer.object.removeFromParent();
            this.pointers.delete(id);
            if (pointer.cursor) {
                pointer.cursor.object.removeFromParent();
            }
        }
    }

    private readState(buffer: BufferReaderWriter) {
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
            const pointerID = buffer.readUint8() as PointerID;
            const pointer = this.assurePointer(pointerID);
            pointer.readState(buffer);
        }
    }
}
