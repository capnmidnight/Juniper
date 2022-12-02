import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { GraphNode } from "@juniper-lib/tslib/collections/GraphNode";
import { once } from "@juniper-lib/tslib/events/once";
import { singleton } from "@juniper-lib/tslib/singleton";
import { assertNever, isArray, isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";


export const hasAudioContext = /*@__PURE__*/ "AudioContext" in globalThis;
export const hasAudioListener = /*@__PURE__*/ hasAudioContext && "AudioListener" in globalThis;
export const hasOldAudioListener = /*@__PURE__*/ hasAudioListener && "setPosition" in AudioListener.prototype;
export const hasNewAudioListener = /*@__PURE__*/ hasAudioListener && "positionX" in AudioListener.prototype;
export const canCaptureStream = /*@__PURE__*/ isFunction(HTMLMediaElement.prototype.captureStream)
    || isFunction(HTMLMediaElement.prototype.mozCaptureStream);

export function isAudioContext(context: any): context is BaseAudioContext {
    return context instanceof BaseAudioContext;
}

export interface ErsatzAudioNode extends IDisposable {
    input: AudioNode;
    output: AudioNode;
}

export function isErsatzAudioNode(value: any): value is ErsatzAudioNode {
    return isDefined(value)
        && value.input instanceof AudioNode
        && value.output instanceof AudioNode;
}

export type AudioNodeType = AudioNode | ErsatzAudioNode;

export type WebAudioEndpoint = AudioNode | AudioParam;

export type AudioVertex = AudioNodeType | AudioParam;

export type AudioConnectionWithOutputChannel = [number, AudioVertex];
export type AudioConnectionWithInputOutputChannels = [number, number, AudioNodeType];

export type AudioConnection
    = AudioVertex
    | AudioConnectionWithOutputChannel
    | AudioConnectionWithInputOutputChannels;

export function isAudioConnectionWithOutputChannel(conn: AudioConnection): conn is AudioConnectionWithOutputChannel {
    return isArray(conn)
        && conn.length === 2;
}

export function isAudioConnectionWithInputOutputChannels(conn: AudioConnection): conn is AudioConnectionWithInputOutputChannels {
    return isArray(conn)
        && conn.length === 3;
}

export function isAudioConnectionAudioVertex(conn: AudioConnection): conn is AudioVertex {
    return isDefined(conn)
        && !isArray(conn)
}

export type BaseAudioNodeParamType = number | ChannelCountMode | ChannelInterpretation;

const connections = singleton("Juniper:Audio:connections", () => new Map<AudioNode, Set<WebAudioEndpoint>>());
const names = singleton("Juniper:Audio:names", () => new Map<WebAudioEndpoint, string>());

export function resolveOutput(node: AudioNodeType): AudioNode {
    if (isErsatzAudioNode(node)) {
        return node.output;
    }

    return node;
}

export function resolveInput(node: AudioConnection): WebAudioEndpoint {
    if (isNullOrUndefined(node)) {
        return undefined;
    }

    let n: AudioVertex = null;
    if (isArray(node)) {
        if (node.length === 2) {
            n = node[1];
        }
        else {
            n = node[2];
        }
    }
    else {
        n = node;
    }

    let n2: WebAudioEndpoint = null;
    if (isErsatzAudioNode(n)) {
        n2 = n.input;
    }
    else {
        n2 = n;
    }

    return n2;
}

export function resolveArray(node: AudioConnection): number[] {
    if (!isArray(node)) {
        return [];
    }
    else if (node.length === 2) {
        return [node[0]];
    }
    else {
        return [node[0], node[1]];
    }
}

function isAudioNode(a: AudioVertex | number): a is AudioNode {
    return isDefined(a)
        && isDefined((a as AudioNode).context);
}

function nameVertex(name: string, v: WebAudioEndpoint): void {
    names.set(v, name);
}

export function getVertexName(v: WebAudioEndpoint): string {
    return names.get(v);
}

export function removeVertex(v: WebAudioEndpoint): void {
    names.delete(v);
    if (isAudioNode(v)) {
        disconnect(v);
    }
}

export function chain(...nodes: AudioNodeType[]): void {
    for (let i = 1; i < nodes.length; ++i) {
        connect(nodes[i - 1], nodes[i]);
    }
}

export function connect(left: AudioNodeType, right: AudioConnection): boolean {
    const a = resolveOutput(left);
    const b = resolveInput(right);
    const c = resolveArray(right);

    if (isNullOrUndefined(b)) {
        throw new Error("Must have a target to connect to");
    }
    else if (b instanceof AudioParam) {
        a.connect(b, c[0]);
    }
    else if (b instanceof AudioNode) {
        a.connect(b, c[0], c[1]);
    }
    else {
        assertNever(b);
    }

    if (!connections.has(a)) {
        connections.set(a, new Set<WebAudioEndpoint>());
    }

    const g = connections.get(a);
    if (g.has(b)) {
        return false;
    }

    g.add(b);

    return true;
}

export function disconnect(left: AudioNodeType, right?: AudioConnection): boolean {
    const a = resolveOutput(left);
    const b = resolveInput(right);
    const c = resolveArray(right);

    if (isNullOrUndefined(b)) {
        a.disconnect();
    }
    else if (b instanceof AudioParam) {
        a.disconnect(b, c[0]);
    }
    else if (b instanceof AudioNode) {
        a.disconnect(b, c[0], c[1]);
    }
    else {
        assertNever(b);
    }

    if (!connections.has(a)) {
        return false;
    }

    const g = connections.get(a);

    let removed = false;
    if (isNullOrUndefined(b)) {
        removed = g.size > 0;
        g.clear();
    }
    else if (g.has(b)) {
        removed = true;
        g.delete(b);
    }

    if (g.size === 0) {
        connections.delete(a);
    }

    return removed;
}

export function getAudioGraph(): Array<GraphNode<WebAudioEndpoint>> {
    const nodes = new Map<WebAudioEndpoint, GraphNode<WebAudioEndpoint>>();

    function maybeAdd(node: WebAudioEndpoint) {
        if (!nodes.has(node)) {
            nodes.set(node, new GraphNode(node));
        }
    }

    for (const node of names.keys()) {
        maybeAdd(node);
    }

    for (const [parent, children] of connections) {
        maybeAdd(parent);
        for (const child of children) {
            maybeAdd(child);
        }
    }

    for (const [parent, children] of connections) {
        const branch = nodes.get(parent);
        for (const child of children) {
            if (nodes.has(child)) {
                branch.connectTo(nodes.get(child));
            }
        }
    }

    return Array.from(nodes.values());
}

(globalThis as any).getAudioGraph = getAudioGraph;

export async function audioReady(audioCtx: AudioContext) {
    nameVertex("speakers", audioCtx.destination);
    if (audioCtx.state !== "running") {
        if (audioCtx.state === "closed") {
            await audioCtx.resume();
        }
        else if (audioCtx.state === "suspended") {
            const stateChange = once<BaseAudioContextEventMap>(audioCtx, "statechange");
            onUserGesture(() => audioCtx.resume());
            await stateChange;
        }
        else {
            assertNever(audioCtx.state);
        }
    }
}

export function initAudio<NodeT extends AudioNode>(name: string, left: NodeT, ...rest: AudioConnection[]) {
    nameVertex(name, left);

    for (const right of rest) {
        if (isDefined(right)) {
            connect(left, right);
        }
    }

    return left;
}


