import { AudioStreamSource } from "@juniper-lib/audio/sources/AudioStreamSource";
import { autoPlay, srcObject } from "@juniper-lib/dom/attrs";
import { getMonospaceFonts } from "@juniper-lib/dom/css";
import { Video } from "@juniper-lib/dom/tags";
import { star } from "@juniper-lib/emoji";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { FWD, HalfPi, UP } from "@juniper-lib/tslib/math";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";
import { UserChatEvent, UserStateEvent } from "@juniper-lib/webrtc/ConferenceEvents";
import type { RemoteUser } from "@juniper-lib/webrtc/RemoteUser";
import { FrontSide, Matrix4, Object3D, Quaternion, Vector3 } from "three";
import { BodyFollower } from "./animation/BodyFollower";
import { getLookHeadingRadians } from "./animation/lookAngles";
import { cleanup } from "./cleanup";
import type { Environment } from "./environment/Environment";
import { PointerRemote } from "./eventSystem/devices/PointerRemote";
import { getPointerType, PointerID } from "./eventSystem/Pointers";
import { obj, objectRemove, objGraph } from "./objects";
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

export class AvatarRemote extends Object3D implements IDisposable {
    avatar: Object3D = null;

    private _isInstructor = false;
    private readonly pointers = new Map<PointerID, PointerRemote>();

    private height: number;
    private readonly userID: string = null;
    private readonly head: Object3D = null;
    readonly body: Object3D = null;
    private readonly hands = new Object3D();
    private readonly billboard: Object3D;
    private readonly nameTag: TextMesh;
    private readonly activity: ActivityDetector;
    private readonly pTarget = new Vector3();
    private readonly pEnd = new Vector3();
    private readonly qTarget = new Quaternion().identity();
    private readonly qEnd = new Quaternion().identity();
    readonly worldPos = new Vector3();
    readonly worldQuat = new Quaternion();
    private readonly P = new Vector3();
    private readonly F = new Vector3();
    private readonly U = new Vector3();
    private readonly M = new Matrix4();
    private readonly killers = new Map<PointerRemote, number>();

    private headFollower: BodyFollower = null;
    private _headSize = 1;
    private _headPulse = 1;

    readonly comfortOffset = new Vector3();

    constructor(
        private readonly env: Environment,
        user: RemoteUser,
        source: AudioStreamSource,
        avatar: Object3D,
        private defaultAvatarHeight: number,
        font: Partial<TextImageOptions>) {
        super();

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
        this.name = user.userName;
        this.billboard = obj("billboard");

        this.nameTag = new TextMesh(this.env, `nameTag-${user.userName}-${user.userID}`, Object.assign({}, nameTagFont, font));
        this.nameTag.position.y = -0.25;
        this.userName = user.userName;



        let buffer: Float32Array = null;
        let i = 0;

        const getNumber = () => {
            return buffer[i++];
        };

        const getHandedness = (): XRHandedness => {
            const h = getNumber();
            switch (h) {
                case 2: return "left";
                case 1: return "right";
                default: return "none";
            }
        };

        const getVector3 = (v: Vector3) => {
            v.fromArray(buffer, i);
            i += 3;
        };

        const getMatrix16 = (m: Matrix4) => {
            m.fromArray(buffer, i);
            i += 16;
        };

        user.addEventListener("userState", (evt: UserStateEvent) => {
            buffer = evt.buffer;
            i = 0;

            const numPointers = getNumber();
            this.height = getNumber();
            this.hands.position.y = -this.height;
            getMatrix16(this.M);

            this.M.decompose(this.pTarget, this.qTarget, this.scale);
            this.pTarget.add(this.comfortOffset);

            for (let n = 0; n < numPointers; ++n) {
                const pointerID = getNumber() as PointerID;
                getVector3(this.P);
                getVector3(this.F);
                getVector3(this.U);

                const pointer = this.assurePointer(pointerID);
                pointer.setState(this.P, this.F, this.U);

                if (PointerID.MotionController <= pointerID && pointerID <= PointerID.MotionControllerRight) {
                    const handedness = getHandedness();
                    if (handedness === "none") {
                        this.deferExecution(1, pointer, () => pointer.hand = null);
                        if (pointer.hand) {
                            pointer.hand = null;
                        }
                    }
                    else {
                        if (!pointer.hand) {
                            pointer.hand = this.env.handModelFactory.createHandModel(handedness);
                        }

                        const numFingerJoints = getNumber();
                        for (let n = 0; n < numFingerJoints; ++n) {
                            getMatrix16(this.M);
                            pointer.hand.setMatrixAt(n, this.M);
                        }
                        pointer.hand.count = numFingerJoints;
                        pointer.hand.updateMesh();
                    }
                }
            }
        });

        this.activity = new ActivityDetector(this.env.audio.context);
        this.activity.name = `remote-user-activity-${user.userName}-${user.userID}`;

        source.addEventListener("sourceadded", (evt) => {
            evt.source.connect(this.activity);
            evt.source.name = `remote-user-stream-${user.userName}-${user.userID}`
        });

        this.headFollower = new BodyFollower("AvatarBody", 0.05, HalfPi, 0, 5);

        objGraph(this,
            objGraph(this.avatar,
                this.hands,
                objGraph(this.headFollower,
                    objGraph(this.body,
                        objGraph(this.billboard,
                            this.nameTag)))));
    }

    private deferExecution(killTime: number, pointer: PointerRemote, killAction: () => void) {
        if (this.killers.has(pointer)) {
            clearTimeout(this.killers.get(pointer));
            this.killers.delete(pointer);
        }

        this.killers.set(pointer, setTimeout(() => {
            this.killers.delete(pointer);
            killAction();
        }, killTime * 1000) as any);
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
            this.removeArm(pointerName);
        }

        this.activity.dispose();
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

    refreshCursors() {
        for (const pointer of this.pointers.values()) {
            if (pointer.cursor) {
                pointer.cursor = this.env.cursor3D.clone();
            }
        }
    }

    update(dt: number) {
        this.headPulse = 0.2 * this.activity.level + 1;
        this.pEnd.lerp(this.pTarget, dt * 0.01);
        this.qEnd.slerp(this.qTarget, dt * 0.01);
        this.head.position.copy(this.pEnd);
        this.head.quaternion.copy(this.qEnd);
        this.head.getWorldPosition(this.worldPos);
        this.head.getWorldQuaternion(this.worldQuat);

        this.F.fromArray(FWD)
            .applyQuaternion(this.worldQuat);
        this.U.fromArray(UP)
            .applyQuaternion(this.worldQuat);

        const headingRadians = getLookHeadingRadians(this.F);

        this.env.audio.setUserPose(
            this.userID,
            this.worldPos.x, this.worldPos.y, this.worldPos.z,
            this.F.x, this.F.y, this.F.z,
            this.U.x, this.U.y, this.U.z);

        this.headFollower.update(this.worldPos.y - this.parent.position.y, this.worldPos, headingRadians, dt);
        const scale = this.height / this.defaultAvatarHeight;
        this.headSize = scale;
        this.body.scale.setScalar(scale);
        this.hands.position.copy(this.comfortOffset);

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
            pointer = new PointerRemote(this, this.env, id, this.hands);

            this.pointers.set(id, pointer);

            objGraph(this.body, pointer);
            if (pointer.cursor) {
                objGraph(this.env.stage, pointer.cursor);
            }

            if (id === PointerID.MotionControllerLeft
                || id === PointerID.MotionControllerRight) {
                this.removeArmsExcept(PointerID.MotionControllerLeft, PointerID.MotionControllerRight);
            }
            else {
                this.removeArmsExcept(id);
            }
        }

        this.deferExecution(3, pointer, () => this.removeArm(id));

        return pointer;
    }

    private removeArmsExcept(...ids: PointerID[]): void {
        for (const id of this.pointers.keys()) {
            if (ids.indexOf(id) === -1) {
                this.removeArm(id);
            }
        }
    }

    private removeArm(id: PointerID): void {
        const pointer = this.pointers.get(id);
        console.log("removeArm", getPointerType(id), !!pointer);
        if (pointer) {
            objectRemove(this.body, pointer);
            this.pointers.delete(id);
            if (pointer.cursor) {
                objectRemove(this.env.stage, pointer.cursor);
            }
        }
    }
}
