import { Pose } from "@juniper-lib/audio/Pose";
import { AudioStreamSource } from "@juniper-lib/audio/sources/AudioStreamSource";
import { connect } from "@juniper-lib/audio/util";
import { getMonospaceFonts } from "@juniper-lib/dom/css";
import { star } from "@juniper-lib/emoji";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { PointerID } from "@juniper-lib/tslib/events/Pointers";
import { FWD, HalfPi, UP } from "@juniper-lib/tslib/math";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";
import { UserPointerEvent, UserPosedEvent } from "@juniper-lib/webrtc/ConferenceEvents";
import type { RemoteUser } from "@juniper-lib/webrtc/RemoteUser";
import { Matrix4, Object3D, Quaternion, Vector3 } from "three";
import { BodyFollower } from "./animation/BodyFollower";
import { getLookHeadingRadians } from "./animation/lookAngles";
import type { Environment } from "./environment/Environment";
import { PointerRemote } from "./eventSystem/devices/PointerRemote";
import { objectRemove, objGraph } from "./objects";
import { setMatrixFromUpFwdPos } from "./setMatrixFromUpFwdPos";
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
        this.nameTag = new TextMesh(this.env, `nameTag-${user.userName}-${user.userID}`, Object.assign({}, nameTagFont, font));
        this.nameTag.position.y = 0.25;
        this.userName = user.userName;

        objGraph(this, this.nameTag);

        user.addEventListener("userPosed", (evt: UserPosedEvent) =>
            this.setPose(evt.pose, evt.height));


        user.addEventListener("userPointer", (evt: UserPointerEvent) =>
            this.setPointer(evt.pointerID, evt.pose));

        this.activity = new ActivityDetector(`remote-user-activity-${user.userName}-${user.userID}`, this.env.audio.audioCtx);

        source.addEventListener("sourceadded", (evt) => {
            connect(evt.source, this.activity);
        });

        this.headFollower = new BodyFollower("AvatarBody", 0.05, HalfPi, 0, 5);

        objGraph(this.body.parent,
            objGraph(this.headFollower,
                objGraph(this.body, this.nameTag)));

        objGraph(this, this.avatar);
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

        this.F.copy(this.env.avatar.worldPos);
        this.body.worldToLocal(this.F);
        this.F.sub(this.body.position)
            .normalize()
            .multiplyScalar(0.25);
        this.nameTag.position.set(0, -0.25, 0)
            .add(this.F);

        for (const pointer of this.pointers.values()) {
            pointer.animate(dt);
        }

        this.nameTag.lookAt(this.env.avatar.worldPos);
    }

    private setPose(pose: Pose, height: number): void {
        this.P.fromArray(pose.p)
            .add(this.comfortOffset);
        this.F.fromArray(pose.f);
        this.U.fromArray(pose.u);
        setMatrixFromUpFwdPos(this.U, this.F, this.P, this.M);
        this.M.decompose(this.pTarget, this.qTarget, this.scale);
        this.height = height;
    }

    private setPointer(id: PointerID, pose: Pose): void {
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
                this.removeArmsExcept(PointerID.MotionControllerLeft, PointerID.MotionControllerRight);
            }
            else {
                this.removeArmsExcept(id);
            }
        }

        this.P.fromArray(pose.p);
        this.F.fromArray(pose.f);
        this.U.fromArray(pose.u);

        pointer.setState(this.P, this.F, this.U);
    }

    private removeArmsExcept(...names: PointerID[]): void {
        for (const name of this.pointers.keys()) {
            if (names.indexOf(name) === -1) {
                this.removeArm(name);
            }
        }
    }

    private removeArm(name: PointerID): void {
        const pointer = this.pointers.get(name);
        if (pointer) {
            objectRemove(this.body, pointer);
            this.pointers.delete(name);
            if (pointer.cursor) {
                objectRemove(this.env.stage, pointer.cursor);
            }
        }
    }
}
