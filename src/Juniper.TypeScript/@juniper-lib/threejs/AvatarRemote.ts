import { connect } from "@juniper-lib/audio/nodes";
import { Pose } from "@juniper-lib/audio/Pose";
import { AudioStreamSource } from "@juniper-lib/audio/sources/AudioStreamSource";
import { getMonospaceFonts } from "@juniper-lib/dom/css";
import { star } from "@juniper-lib/emoji";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import type { IDisposable } from "@juniper-lib/tslib";
import { PointerID } from "@juniper-lib/tslib";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";
import { ConferenceEventTypes, UserPointerEvent, UserPosedEvent, UserPoseEvent } from "@juniper-lib/webrtc/ConferenceEvents";
import type { RemoteUser } from "@juniper-lib/webrtc/RemoteUser";
import { BodyFollower } from "./animation/BodyFollower";
import { getLookHeading } from "./animation/lookAngles";
import type { Environment } from "./environment/Environment";
import { PointerRemote } from "./eventSystem/PointerRemote";
import { objectRemove, objGraph } from "./objects";
import { setMatrixFromUpFwdPos } from "./setMatrixFromUpFwdPos";
import { TextMesh } from "./widgets/TextMesh";

const P = new THREE.Vector3();
const F = new THREE.Vector3();
const U = new THREE.Vector3();
const O = new THREE.Vector3();

const E = new THREE.Vector3();

const M = new THREE.Matrix4();

const angle = Math.PI / 2;

const nameTagFont: Partial<TextImageOptions> = {
    fontFamily: getMonospaceFonts(),
    fontSize: 20,
    fontWeight: "bold",
    textFillColor: "#ffffff",
    textStrokeColor: "#000000",
    textStrokeSize: 0.01,
    wrapWords: false,
    padding: {
        top: 0.025,
        right: 0.05,
        bottom: 0.025,
        left: 0.05
    },
    maxHeight: 0.20
};

export class AvatarRemote extends THREE.Object3D implements IDisposable {
    avatar: THREE.Object3D = null;

    private isInstructor = false;
    private readonly pointers = new Map<PointerID, PointerRemote>();

    private head: THREE.Object3D = null;
    private body: THREE.Object3D = null;
    private height: number;
    private readonly nameTag: TextMesh;
    private readonly activity: ActivityDetector;
    private readonly pTarget = new THREE.Vector3();
    private readonly pEnd = new THREE.Vector3();
    private readonly qTarget = new THREE.Quaternion().identity();
    private readonly qEnd = new THREE.Quaternion().identity();
    private headFollower: BodyFollower = null;
    private _headSize = 1;
    private _headPulse = 1;

    constructor(
        private readonly env: Environment,
        user: RemoteUser,
        source: AudioStreamSource,
        font: Partial<TextImageOptions>,
        private defaultAvatarHeight: number) {
        super();

        this.height = this.defaultAvatarHeight;

        this.name = user.userName;
        this.nameTag = new TextMesh(this.env, `nameTag-${user.userName}-${user.userID}`, Object.assign({}, nameTagFont, font));
        this.nameTag.position.y = 0.25;
        this.userName = user.userName;

        objGraph(this, this.nameTag);

        user.addEventListener("userPosed", (evt: UserPosedEvent) => {
            this.onRemoteUserPosed(evt);
            const { p, f, u } = evt.pose;
            this.env.audio.setUserPose(
                evt.user.userID,
                p[0], p[1], p[2],
                f[0], f[1], f[2],
                u[0], u[1], u[2]);

            this.setPose(evt.pose, evt.height);
        });


        user.addEventListener("userPointer", (evt: UserPointerEvent) => {
            this.onRemoteUserPosed(evt);
            this.setPointer(this.env.avatar.worldPos, evt.pointerID, evt.pose);
        });

        this.activity = new ActivityDetector(`remote-user-activity-${user.userName}-${user.userID}`, this.env.audio.audioCtx);

        source.addEventListener("sourceadded", (evt) => {
            connect(evt.source, this.activity);
        });
    }

    private onRemoteUserPosed<T extends ConferenceEventTypes>(evt: UserPoseEvent<T>): void {
        const offset = this.env.audio.getUserOffset(evt.user.userID);
        if (offset) {
            evt.pose.setOffset(offset[0], offset[1], offset[2]);
        }
    }

    dispose() {
        for (const pointerName of this.pointers.keys()) {
            this.removeArm(pointerName);
        }

        this.activity.dispose();
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
                    this.isInstructor = words[1]
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

    setAvatar(obj: THREE.Object3D): void {
        if (this.avatar) {
            this.remove(this.avatar);
            this.head = null;
            this.body = null;
        }

        if (obj) {
            this.avatar = obj;
            this.avatar.traverse((child) => {
                if (child.name === "Head") {
                    this.head = child;
                }
                else if (child.name === "Body") {
                    this.body = child;
                }
            });

            if (this.head && this.body) {
                this.headFollower = new BodyFollower("AvatarBody", 0.05, angle, 0, 5);
                objGraph(this.body.parent,
                    objGraph(this.headFollower,
                        objGraph(this.body, this.nameTag)));
            }

            objGraph(this, this.avatar);
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
        if (this.head && this.body) {
            this.head.position.copy(this.pEnd);
            this.head.quaternion.copy(this.qEnd);
            this.head.getWorldPosition(P);
            this.head.getWorldDirection(F);
            F.negate();

            const angle = getLookHeading(F);
            this.headFollower.update(P.y - this.parent.position.y, P, angle, dt);
            const scale = this.height / this.defaultAvatarHeight;
            this.headSize = scale;
            this.body.scale.setScalar(scale);

            F.copy(this.env.avatar.worldPos);
            this.body.worldToLocal(F);
            F.sub(this.body.position)
                .normalize()
                .multiplyScalar(0.25);
            this.nameTag.position.set(0, -0.25, 0)
                .add(F);
        }
        else {
            this.position.copy(this.pEnd);
            this.quaternion.copy(this.qEnd);
        }

        for (const pointer of this.pointers.values()) {
            pointer.animate(dt);
        }

        this.nameTag.lookAt(this.env.avatar.worldPos);
    }

    private setPose(pose: Pose, height: number): void {
        O.fromArray(pose.o);
        P.fromArray(pose.p)
            .add(O);
        F.fromArray(pose.f);
        U.fromArray(pose.u);
        setMatrixFromUpFwdPos(U, F, P, M);
        M.decompose(this.pTarget, this.qTarget, this.scale);
        this.height = height;
    }

    private setPointer(avatarHeadPos: THREE.Vector3, id: PointerID, pose: Pose): void {
        let pointer = this.pointers.get(id);

        if (!pointer) {
            pointer = new PointerRemote(
                this.env,
                this.userName,
                this.isInstructor,
                id,
                this.env.cursor3D && this.env.cursor3D.clone());

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

        P.fromArray(pose.p);
        F.fromArray(pose.f);
        U.fromArray(pose.u);
        O.fromArray(pose.o);

        if (id === PointerID.Mouse && this.body) {
            E.set(0.2, -0.6, 0)
                .applyQuaternion(this.body.quaternion);
        }
        else if (PointerID[id].startsWith("Touch") && this.body) {
            E.set(0, -0.5, 0)
                .applyQuaternion(this.body.quaternion);
        }
        else {
            E.setScalar(0);
        }

        O.add(E);

        pointer.setState(avatarHeadPos, P, F, U, O);
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
