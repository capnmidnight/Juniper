import { connect } from "@juniper-lib/audio/nodes";
import { Pose } from "@juniper-lib/audio/Pose";
import { AudioStreamSource } from "@juniper-lib/audio/sources/AudioStreamSource";
import { getMonospaceFonts } from "@juniper-lib/dom/css";
import { star } from "@juniper-lib/emoji";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import type { IDisposable } from "@juniper-lib/tslib";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";
import { UserPointerEvent, UserPosedEvent } from "@juniper-lib/webrtc/ConferenceEvents";
import type { RemoteUser } from "@juniper-lib/webrtc/RemoteUser";
import { BodyFollower } from "./animation/BodyFollower";
import { getLookHeading } from "./animation/lookAngles";
import type { Environment } from "./environment/Environment";
import { PointerRemote } from "./eventSystem/PointerRemote";
import { objGraph } from "./objects";
import { setMatrixFromUpFwdPos } from "./setMatrixFromUpFwdPos";
import { TextMesh } from "./TextMesh";

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
    private readonly pointers = new Map<PointerName, PointerRemote>();

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
        this.add(this.nameTag);

        user.addEventListener("userPosed", (evt: UserPosedEvent) =>
            this.setPose(evt.pose, evt.height));

        user.addEventListener("userPointer", (evt: UserPointerEvent) => {
            this.setPointer(this.env.avatar.worldPos, evt.name, evt.pose);
        });

        this.activity = new ActivityDetector(`remote-user-activity-${user.userName}-${user.userID}`, this.env.audio.audioCtx);

        source.addEventListener("sourceadded", (evt) => {
            connect(evt.source, this.activity);
        });
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
                this.body.parent.add(this.headFollower);
                this.body.add(this.nameTag);
                this.headFollower.add(this.body);
            }

            this.add(this.avatar);
        }
    }

    private removeArm(name: PointerName): void {
        const pointer = this.pointers.get(name);
        if (pointer) {
            this.body.remove(pointer.object);
            this.pointers.delete(name);
            if (pointer.cursor) {
                this.env.stage.remove(pointer.cursor.object);
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
        if (this.head && this.body) {
            this.head.position.copy(this.pEnd);
            this.head.quaternion.copy(this.qEnd);
            this.head.getWorldPosition(P);
            this.head.getWorldDirection(F);
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

    setPose(pose: Pose, height: number): void {
        O.fromArray(pose.o);
        P.fromArray(pose.p)
            .add(O);
        F.fromArray(pose.f);
        U.fromArray(pose.u);
        setMatrixFromUpFwdPos(U, F, P, M);
        M.decompose(this.pTarget, this.qTarget, this.scale);
        this.height = height;
    }

    setPointer(avatarHeadPos: THREE.Vector3, name: PointerName, pose: Pose): void {
        let pointer = this.pointers.get(name);

        if (!pointer) {
            pointer = new PointerRemote(
                this.env.eventSystem,
                this.userName,
                this.isInstructor,
                name,
                this.env.cursor3D && this.env.cursor3D.clone());

            this.pointers.set(name, pointer);

            objGraph(this.body, pointer);
            if (pointer.cursor) {
                objGraph(this.env.stage, pointer.cursor);
            }

            if (name === PointerName.Mouse) {
                this.removeArm(PointerName.MotionController);
                this.removeArm(PointerName.MotionControllerLeft);
                this.removeArm(PointerName.MotionControllerRight);
            }
            else if (name === PointerName.MotionController
                || name === PointerName.MotionControllerLeft
                || name === PointerName.MotionControllerRight) {
                this.removeArm(PointerName.Mouse);
                this.removeArm(PointerName.Pen);
                this.removeArm(PointerName.Touch0);
                this.removeArm(PointerName.Touch1);
                this.removeArm(PointerName.Touch2);
                this.removeArm(PointerName.Touch3);
                this.removeArm(PointerName.Touch4);
                this.removeArm(PointerName.Touch5);
                this.removeArm(PointerName.Touch6);
                this.removeArm(PointerName.Touch7);
                this.removeArm(PointerName.Touch8);
                this.removeArm(PointerName.Touch9);
                this.removeArm(PointerName.Touch10);
                this.removeArm(PointerName.Touches);
            }
        }

        P.fromArray(pose.p);
        F.fromArray(pose.f);
        U.fromArray(pose.u);
        O.fromArray(pose.o);

        if (name === PointerName.Mouse && this.body) {
            E.set(0.2, -0.6, 0)
                .applyQuaternion(this.body.quaternion);
        }
        else if (PointerName[name].startsWith("Touch") && this.body) {
            E.set(0, -0.5, 0)
                .applyQuaternion(this.body.quaternion);
        }
        else {
            E.setScalar(0);
        }

        O.add(E);

        pointer.setState(avatarHeadPos, P, F, U, O);
    }
}
