var __defProp = Object.defineProperty;
var __defNormalProp = (obj, key, value) => key in obj ? __defProp(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
var __publicField = (obj, key, value) => {
  __defNormalProp(obj, typeof key !== "symbol" ? key + "" : key, value);
  return value;
};

// ../tslib/dist/typeChecks.js
function t(o, s, c) {
  return typeof o === s || o instanceof c;
}
function isFunction(obj) {
  return t(obj, "function", Function);
}
function isString(obj) {
  return t(obj, "string", String);
}
function isBoolean(obj) {
  return t(obj, "boolean", Boolean);
}
function isNumber(obj) {
  return t(obj, "number", Number);
}
function isArray(obj) {
  return obj instanceof Array;
}
function assertNever(x, msg) {
  throw new Error((msg || "Unexpected object: ") + x);
}
function isNullOrUndefined(obj) {
  return obj === null || obj === void 0;
}
function isDefined(obj) {
  return !isNullOrUndefined(obj);
}
function isArrayBufferView(obj) {
  return obj instanceof Uint8Array || obj instanceof Uint8ClampedArray || obj instanceof Int8Array || obj instanceof Uint16Array || obj instanceof Int16Array || obj instanceof Uint32Array || obj instanceof Int32Array || obj instanceof Float32Array || obj instanceof Float64Array || "BigUint64Array" in globalThis && obj instanceof globalThis["BigUint64Array"] || "BigInt64Array" in globalThis && obj instanceof globalThis["BigInt64Array"];
}
function isArrayBuffer(val) {
  return val && typeof ArrayBuffer !== "undefined" && (val instanceof ArrayBuffer || val.constructor && val.constructor.name === "ArrayBuffer");
}
function isXHRBodyInit(obj) {
  return isString(obj) || isArrayBufferView(obj) || obj instanceof Blob || obj instanceof FormData || isArrayBuffer(obj) || obj instanceof ReadableStream || "Document" in globalThis && obj instanceof Document;
}

// ../tslib/dist/collections/arrayRemoveAt.js
function arrayRemoveAt(arr, idx) {
  return arr.splice(idx, 1)[0];
}

// ../tslib/dist/collections/arrayClear.js
function arrayClear(arr) {
  return arr.splice(0);
}

// ../tslib/dist/events/EventBase.js
var allListeners = new WeakMap();
var EventBase = class {
  constructor() {
    __publicField(this, "listeners", new Map());
    __publicField(this, "listenerOptions", new Map());
    allListeners.set(this, this.listeners);
  }
  addEventListener(type, callback, options) {
    if (isFunction(callback)) {
      let listeners = this.listeners.get(type);
      if (!listeners) {
        listeners = new Array();
        this.listeners.set(type, listeners);
      }
      if (!listeners.find((c) => c === callback)) {
        listeners.push(callback);
        if (options) {
          this.listenerOptions.set(callback, options);
        }
      }
    }
  }
  removeEventListener(type, callback) {
    if (isFunction(callback)) {
      const listeners = this.listeners.get(type);
      if (listeners) {
        this.removeListener(listeners, callback);
      }
    }
  }
  removeListener(listeners, callback) {
    const idx = listeners.findIndex((c) => c === callback);
    if (idx >= 0) {
      arrayRemoveAt(listeners, idx);
      if (this.listenerOptions.has(callback)) {
        this.listenerOptions.delete(callback);
      }
    }
  }
  dispatchEvent(evt) {
    const listeners = this.listeners.get(evt.type);
    if (listeners) {
      for (const callback of listeners) {
        const options = this.listenerOptions.get(callback);
        if (isDefined(options) && !isBoolean(options) && options.once) {
          this.removeListener(listeners, callback);
        }
        callback.call(this, evt);
      }
    }
    return !evt.defaultPrevented;
  }
};
var TypedEvent = class extends Event {
  get type() {
    return super.type;
  }
  constructor(type) {
    super(type);
  }
};
var TypedEventBase = class extends EventBase {
  constructor() {
    super(...arguments);
    __publicField(this, "bubblers", new Set());
    __publicField(this, "scopes", new WeakMap());
  }
  addBubbler(bubbler) {
    this.bubblers.add(bubbler);
  }
  removeBubbler(bubbler) {
    this.bubblers.delete(bubbler);
  }
  addEventListener(type, callback, options) {
    super.addEventListener(type, callback, options);
  }
  removeEventListener(type, callback) {
    super.removeEventListener(type, callback);
  }
  addScopedEventListener(scope, type, callback, options) {
    if (!this.scopes.has(scope)) {
      this.scopes.set(scope, []);
    }
    this.scopes.get(scope).push([type, callback]);
    this.addEventListener(type, callback, options);
  }
  removeScope(scope) {
    const listeners = this.scopes.get(scope);
    if (listeners) {
      this.scopes.delete(scope);
      for (const [type, listener] of listeners) {
        this.removeEventListener(type, listener);
      }
    }
  }
  clearEventListeners(type) {
    const listeners = allListeners.get(this);
    for (const [evtName, handlers] of listeners) {
      if (isNullOrUndefined(type) || type === evtName) {
        arrayClear(handlers);
        listeners.delete(evtName);
      }
    }
  }
  dispatchEvent(evt) {
    if (!super.dispatchEvent(evt)) {
      return false;
    }
    for (const bubbler of this.bubblers) {
      if (!bubbler.dispatchEvent(evt)) {
        return false;
      }
    }
    return true;
  }
};

// ../tslib/dist/collections/mapJoin.js
function mapJoin(dest, ...sources) {
  for (const source of sources) {
    if (isDefined(source)) {
      for (const [key, value] of source) {
        dest.set(key, value);
      }
    }
  }
  return dest;
}

// ../tslib/dist/flags.js
var oculusBrowserPattern = /OculusBrowser\/(\d+)\.(\d+)\.(\d+)/i;
var oculusMatch = navigator.userAgent.match(oculusBrowserPattern);
var isOculusBrowser = !!oculusMatch;
var oculusBrowserVersion = isOculusBrowser && {
  major: parseFloat(oculusMatch[1]),
  minor: parseFloat(oculusMatch[2]),
  patch: parseFloat(oculusMatch[3])
};
var isOculusGo = isOculusBrowser && /pacific/i.test(navigator.userAgent);
var isOculusQuest = isOculusBrowser && /quest/i.test(navigator.userAgent);
var isOculusQuest2 = isOculusBrowser && /quest 2/i.test(navigator.userAgent);
var isWorker = !("Document" in globalThis);

// ../tslib/dist/gis/Datum.js
var invF = 298.257223563;
var equatorialRadius = 6378137;
var flattening = 1 / invF;
var flatteningComp = 1 - flattening;
var n = flattening / (2 - flattening);
var A = equatorialRadius / (1 + n) * (1 + n * n / 4 + n * n * n * n / 64);
var e = Math.sqrt(1 - flatteningComp * flatteningComp);
var esq = 1 - flatteningComp * flatteningComp;
var e0sq = e * e / (1 - e * e);
var alpha1 = 1 - esq * (0.25 + esq * (3 / 64 + 5 * esq / 256));
var alpha2 = esq * (3 / 8 + esq * (3 / 32 + 45 * esq / 1024));
var alpha3 = esq * esq * (15 / 256 + esq * 45 / 1024);
var alpha4 = esq * esq * esq * (35 / 3072);
var beta = [
  n / 2 - 2 * n * n / 3 + 37 * n * n * n / 96,
  n * n / 48 + n * n * n / 15,
  17 * n * n * n / 480
];
var delta = [
  2 * n - 2 * n * n / 3,
  7 * n * n / 3 - 8 * n * n * n / 5,
  56 * n * n * n / 15
];

// ../tslib/dist/gis/UTMPoint.js
var GlobeHemisphere;
(function(GlobeHemisphere2) {
  GlobeHemisphere2[GlobeHemisphere2["Northern"] = 0] = "Northern";
  GlobeHemisphere2[GlobeHemisphere2["Southern"] = 1] = "Southern";
})(GlobeHemisphere || (GlobeHemisphere = {}));

// ../tslib/dist/math/angleClamp.js
var Tau = 2 * Math.PI;

// ../tslib/dist/progress/BaseProgress.js
var BaseProgress = class {
  constructor() {
    __publicField(this, "attached", new Array());
    __publicField(this, "soFar", null);
    __publicField(this, "total", null);
    __publicField(this, "msg", null);
    __publicField(this, "est", null);
  }
  report(soFar, total, msg, est) {
    this.soFar = soFar;
    this.total = total;
    this.msg = msg;
    this.est = est;
    for (const attach of this.attached) {
      attach.report(soFar, total, msg, est);
    }
  }
  attach(prog) {
    this.attached.push(prog);
    prog.report(this.soFar, this.total, this.msg, this.est);
  }
  end() {
    this.report(1, 1, "done");
    this.soFar = null;
    this.total = null;
    this.msg = null;
    this.est = null;
    arrayClear(this.attached);
  }
};

// ../tslib/dist/progress/ChildProgressCallback.js
var ChildProgressCallback = class extends BaseProgress {
  constructor(i, prog) {
    super();
    __publicField(this, "i");
    __publicField(this, "prog");
    this.i = i;
    this.prog = prog;
  }
  report(soFar, total, msg, est) {
    super.report(soFar, total, msg, est);
    this.prog.update(this.i, soFar, total, msg);
  }
};

// ../tslib/dist/progress/BaseParentProgressCallback.js
var BaseParentProgressCallback = class {
  constructor(prog) {
    __publicField(this, "prog");
    __publicField(this, "weightTotal", 0);
    __publicField(this, "start");
    __publicField(this, "subProgressCallbacks", new Array());
    __publicField(this, "subProgressWeights", new Array());
    __publicField(this, "subProgressValues", new Array());
    this.prog = prog;
    this.start = performance.now();
    for (let i = 0; i < this.subProgressWeights.length; ++i) {
      this.subProgressValues[i] = 0;
      this.subProgressCallbacks[i] = new ChildProgressCallback(i, this);
    }
  }
  addSubProgress(weight) {
    weight = weight || 1;
    this.weightTotal += weight;
    this.subProgressWeights.push(weight);
    this.subProgressValues.push(0);
    const child = new ChildProgressCallback(this.subProgressCallbacks.length, this);
    this.subProgressCallbacks.push(child);
    return child;
  }
  update(i, subSoFar, subTotal, msg) {
    if (this.prog) {
      this.subProgressValues[i] = subSoFar / subTotal;
      let soFar = 0;
      for (let j = 0; j < this.subProgressWeights.length; ++j) {
        soFar += this.subProgressValues[j] * this.subProgressWeights[j];
      }
      const end = performance.now();
      const delta2 = end - this.start;
      const est = this.start - end + delta2 * this.weightTotal / soFar;
      this.prog.report(soFar, this.weightTotal, msg, est);
    }
  }
};

// ../tslib/dist/progress/IProgress.js
function isProgressCallback(obj) {
  return isDefined(obj) && isFunction(obj.report) && isFunction(obj.attach) && isFunction(obj.end);
}

// ../tslib/dist/progress/progressPopper.js
function progressPopper(progress) {
  return new PoppableParentProgressCallback(progress);
}
var PoppableParentProgressCallback = class extends BaseParentProgressCallback {
  pop(weight) {
    return this.addSubProgress(weight);
  }
};

// ../tslib/dist/workers/WorkerClient.js
var _WorkerClient = class extends TypedEventBase {
  constructor(worker) {
    super();
    __publicField(this, "worker");
    __publicField(this, "taskCounter", 0);
    __publicField(this, "invocations", new Map());
    this.worker = worker;
    if (!_WorkerClient.isSupported) {
      console.warn("Workers are not supported on this system.");
    }
    this.worker.addEventListener("message", (evt) => {
      const data = evt.data;
      switch (data.type) {
        case "event":
          this.propogateEvent(data);
          break;
        case "progress":
          this.progressReport(data);
          break;
        case "return":
          this.methodReturned(data);
          break;
        case "error":
          this.invocationError(data);
          break;
        default:
          assertNever(data);
      }
    });
  }
  postMessage(message, transferables) {
    if (message.type !== "methodCall") {
      assertNever(message.type);
    }
    if (transferables) {
      this.worker.postMessage(message, transferables);
    } else {
      this.worker.postMessage(message);
    }
  }
  dispose() {
    this.worker.terminate();
  }
  propogateEvent(data) {
    const evt = new TypedEvent(data.eventName);
    this.dispatchEvent(Object.assign(evt, data.data));
  }
  progressReport(data) {
    const invocation = this.invocations.get(data.taskID);
    const { onProgress } = invocation;
    if (onProgress) {
      onProgress.report(data.soFar, data.total, data.msg, data.est);
    }
  }
  methodReturned(data) {
    const messageHandler = this.removeInvocation(data.taskID);
    const { resolve } = messageHandler;
    resolve(data.returnValue);
  }
  invocationError(data) {
    const messageHandler = this.removeInvocation(data.taskID);
    const { reject, methodName } = messageHandler;
    reject(new Error(`${methodName} failed. Reason: ${data.errorMessage}`));
  }
  removeInvocation(taskID) {
    const invocation = this.invocations.get(taskID);
    this.invocations.delete(taskID);
    return invocation;
  }
  callMethod(methodName, parameters, transferables, onProgress) {
    if (!_WorkerClient.isSupported) {
      return Promise.reject(new Error("Workers are not supported on this system."));
    }
    let params = null;
    let tfers = null;
    if (isProgressCallback(parameters)) {
      onProgress = parameters;
      parameters = null;
      transferables = null;
    }
    if (isProgressCallback(transferables) && !onProgress) {
      onProgress = transferables;
      transferables = null;
    }
    if (isArray(parameters)) {
      params = parameters;
    }
    if (isArray(transferables)) {
      tfers = transferables;
    }
    const taskID = this.taskCounter++;
    return new Promise((resolve, reject) => {
      const invocation = {
        onProgress,
        resolve,
        reject,
        methodName
      };
      this.invocations.set(taskID, invocation);
      let message = null;
      if (isDefined(parameters)) {
        message = {
          type: "methodCall",
          taskID,
          methodName,
          params
        };
      } else {
        message = {
          type: "methodCall",
          taskID,
          methodName
        };
      }
      this.postMessage(message, tfers);
    });
  }
};
var WorkerClient = _WorkerClient;
__publicField(WorkerClient, "isSupported", "Worker" in globalThis);

// ../tslib/dist/workers/WorkerPool.js
var WorkerPool = class extends TypedEventBase {
  constructor(options, WorkerClientClass) {
    super();
    __publicField(this, "scriptPath");
    __publicField(this, "taskCounter");
    __publicField(this, "workers");
    this.scriptPath = options.scriptPath;
    let workerPoolSize = -1;
    const workersDef = options.workers;
    let workers = null;
    if (isNumber(workersDef)) {
      workerPoolSize = workersDef;
    } else if (isDefined(workersDef)) {
      this.taskCounter = workersDef.curTaskCounter;
      workers = workersDef.workers;
      workerPoolSize = workers.length;
    } else {
      workerPoolSize = navigator.hardwareConcurrency || 4;
    }
    if (workerPoolSize < 1) {
      throw new Error("Worker pool size must be a postive integer greater than 0");
    }
    this.workers = new Array(workerPoolSize);
    if (isNullOrUndefined(workers)) {
      this.taskCounter = 0;
      for (let i = 0; i < workerPoolSize; ++i) {
        this.workers[i] = new WorkerClientClass(new Worker(this.scriptPath, { type: "module" }));
      }
    } else {
      for (let i = 0; i < workerPoolSize; ++i) {
        this.workers[i] = new WorkerClientClass(workers[i]);
      }
    }
    for (const worker of this.workers) {
      worker.addBubbler(this);
    }
  }
  dispose() {
    for (const worker of this.workers) {
      worker.dispose();
    }
    arrayClear(this.workers);
  }
  callMethod(methodName, params, transferables, onProgress) {
    if (!WorkerClient.isSupported) {
      return Promise.reject(new Error("Workers are not supported on this system."));
    }
    let parameters = null;
    let tfers = null;
    if (isProgressCallback(params)) {
      onProgress = params;
      params = null;
      transferables = null;
    }
    if (isProgressCallback(transferables) && !onProgress) {
      onProgress = transferables;
      transferables = null;
    }
    if (isArray(params)) {
      parameters = params;
    }
    if (isArray(transferables)) {
      tfers = transferables;
    }
    const worker = this.nextWorker();
    return worker.callMethod(methodName, parameters, tfers, onProgress);
  }
  nextWorker() {
    const taskID = this.taskCounter++;
    const workerID = taskID % this.workers.length;
    return this.workers[workerID];
  }
};
__publicField(WorkerPool, "isSupported", "Worker" in globalThis);

// ../tslib/dist/workers/WorkerServer.js
var WorkerServerProgress = class extends BaseProgress {
  constructor(server, taskID) {
    super();
    __publicField(this, "server");
    __publicField(this, "taskID");
    this.server = server;
    this.taskID = taskID;
  }
  report(soFar, total, msg, est) {
    const message = {
      type: "progress",
      taskID: this.taskID,
      soFar,
      total,
      msg,
      est
    };
    this.server.postMessage(message);
  }
};
var WorkerServer = class {
  constructor(self) {
    __publicField(this, "self");
    __publicField(this, "methods", new Map());
    this.self = self;
    this.self.addEventListener("message", (evt) => {
      const data = evt.data;
      this.callMethod(data);
    });
  }
  postMessage(message, transferables) {
    if (isDefined(transferables)) {
      this.self.postMessage(message, transferables);
    } else {
      this.self.postMessage(message);
    }
  }
  callMethod(data) {
    const method = this.methods.get(data.methodName);
    if (method) {
      try {
        if (isArray(data.params)) {
          method(data.taskID, ...data.params);
        } else if (isDefined(data.params)) {
          method(data.taskID, data.params);
        } else {
          method(data.taskID);
        }
      } catch (exp) {
        this.onError(data.taskID, `method invocation error: ${data.methodName}(${exp.message || exp})`);
      }
    } else {
      this.onError(data.taskID, `method not found: ${data.methodName}`);
    }
  }
  onError(taskID, errorMessage) {
    const message = {
      type: "error",
      taskID,
      errorMessage
    };
    this.postMessage(message);
  }
  onReturn(taskID, returnValue, transferReturnValue) {
    let message = null;
    if (returnValue === void 0) {
      message = {
        type: "return",
        taskID
      };
    } else {
      message = {
        type: "return",
        taskID,
        returnValue
      };
    }
    if (isDefined(transferReturnValue)) {
      const transferables = transferReturnValue(returnValue);
      this.postMessage(message, transferables);
    } else {
      this.postMessage(message);
    }
  }
  onEvent(eventName, evt, makePayload, transferReturnValue) {
    let message = null;
    if (isDefined(makePayload)) {
      message = {
        type: "event",
        eventName,
        data: makePayload(evt)
      };
    } else {
      message = {
        type: "event",
        eventName
      };
    }
    if (message.data !== void 0 && isDefined(transferReturnValue)) {
      const transferables = transferReturnValue(message.data);
      this.postMessage(message, transferables);
    } else {
      this.postMessage(message);
    }
  }
  addMethodInternal(methodName, asyncFunc, transferReturnValue) {
    if (this.methods.has(methodName)) {
      throw new Error(`${methodName} method has already been mapped.`);
    }
    this.methods.set(methodName, async (taskID, ...params) => {
      const onProgress = new WorkerServerProgress(this, taskID);
      try {
        const returnValue = await asyncFunc(...params, onProgress);
        this.onReturn(taskID, returnValue, transferReturnValue);
      } catch (exp) {
        console.error(exp);
        this.onError(taskID, exp.message || exp);
      }
    });
  }
  addFunction(methodName, asyncFunc, transferReturnValue) {
    this.addMethodInternal(methodName, asyncFunc, transferReturnValue);
  }
  addVoidFunction(methodName, asyncFunc) {
    this.addMethodInternal(methodName, asyncFunc);
  }
  addMethod(methodName, obj, method, transferReturnValue) {
    this.addFunction(methodName, method.bind(obj), transferReturnValue);
  }
  addVoidMethod(methodName, obj, method) {
    this.addVoidFunction(methodName, method.bind(obj));
  }
  addEvent(object, type, makePayload, transferReturnValue) {
    object.addEventListener(type, (evt) => this.onEvent(type, evt, makePayload, transferReturnValue));
  }
};

// ../fetcher-base/dist/ResponseTranslator.js
var ResponseTranslator = class {
  async translateResponse(responseTask, translate) {
    const { status, content, contentType, contentLength, fileName, headers, date } = await responseTask;
    return {
      status,
      content: await translate(content),
      contentType,
      contentLength,
      fileName,
      headers,
      date
    };
  }
};

// ../fetcher-base/dist/FetchingServiceImpl.js
function trackProgress(name, xhr, target, onProgress, skipLoading, prevTask) {
  return new Promise((resolve, reject) => {
    let prevDone = !prevTask;
    if (prevTask) {
      prevTask.then(() => prevDone = true);
    }
    let done = false;
    let loaded = skipLoading;
    function maybeResolve() {
      if (loaded && done) {
        resolve();
      }
    }
    function onError(msg) {
      return () => {
        if (prevDone) {
          reject(`${msg} (${xhr.status})`);
        }
      };
    }
    target.addEventListener("loadstart", () => {
      if (prevDone && !done && onProgress) {
        onProgress.report(0, 1, name);
      }
    });
    target.addEventListener("progress", (ev) => {
      if (prevDone && !done) {
        const evt = ev;
        if (onProgress) {
          onProgress.report(evt.loaded, Math.max(evt.loaded, evt.total), name);
        }
        if (evt.loaded === evt.total) {
          loaded = true;
          maybeResolve();
        }
      }
    });
    target.addEventListener("load", () => {
      if (prevDone && !done) {
        if (onProgress) {
          onProgress.report(1, 1, name);
        }
        done = true;
        maybeResolve();
      }
    });
    target.addEventListener("error", onError("error"));
    target.addEventListener("abort", onError("abort"));
    target.addEventListener("timeout", onError("timeout"));
  });
}
function sendRequest(xhr, xhrType, method, path, timeout, headers, body) {
  xhr.open(method, path);
  xhr.responseType = xhrType;
  xhr.timeout = timeout;
  if (headers) {
    for (const [key, value] of headers) {
      xhr.setRequestHeader(key, value);
    }
  }
  if (isDefined(body)) {
    xhr.send(body);
  } else {
    xhr.send();
  }
}
function readResponseHeader(headers, key, translate) {
  if (!headers.has(key)) {
    return null;
  }
  const value = headers.get(key);
  try {
    const translated = translate(value);
    headers.delete(key);
    return translated;
  } catch (exp) {
    console.warn(key, exp);
  }
  return null;
}
var FILE_NAME_PATTERN = /filename=\"(.+)\"(;|$)/;
function readResponse(xhr) {
  const parts = xhr.getAllResponseHeaders().split(/[\r\n]+/).map((line) => {
    const parts2 = line.split(": ");
    const key = parts2.shift();
    const value = parts2.join(": ");
    return [key, value];
  }).filter((kv) => kv[0].length > 0);
  const headers = new Map(parts);
  const response = {
    status: xhr.status,
    content: xhr.response,
    contentType: readResponseHeader(headers, "content-type", (v) => v),
    contentLength: readResponseHeader(headers, "content-length", parseFloat),
    fileName: readResponseHeader(headers, "content-disposition", (v) => {
      if (isDefined(v)) {
        const match = v.match(FILE_NAME_PATTERN);
        if (isDefined(match)) {
          return match[1];
        }
      }
      return null;
    }),
    date: readResponseHeader(headers, "date", (v) => new Date(v)),
    headers
  };
  return response;
}
var FetchingServiceImpl = class extends ResponseTranslator {
  constructor() {
    super(...arguments);
    __publicField(this, "defaultPostHeaders", new Map());
  }
  setRequestVerificationToken(value) {
    this.defaultPostHeaders.set("RequestVerificationToken", value);
  }
  async headOrGetXHR(method, xhrType, request, progress) {
    const xhr = new XMLHttpRequest();
    const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);
    sendRequest(xhr, xhrType, method, request.path, request.timeout, request.headers);
    await download;
    return readResponse(xhr);
  }
  getXHR(xhrType, request, progress) {
    return this.headOrGetXHR("GET", xhrType, request, progress);
  }
  head(request) {
    return this.headOrGetXHR("HEAD", "", request, null);
  }
  async postXHR(xhrType, request, prog) {
    let body = null;
    const headers = mapJoin(new Map(), this.defaultPostHeaders, request.headers);
    if (request.body instanceof FormData && isDefined(headers)) {
      const toDelete = new Array();
      for (const key of headers.keys()) {
        if (key.toLowerCase() === "content-type") {
          toDelete.push(key);
        }
      }
      for (const key of toDelete) {
        headers.delete(key);
      }
    }
    if (isXHRBodyInit(request.body) && !isString(request.body)) {
      body = request.body;
    } else if (isDefined(request.body)) {
      body = JSON.stringify(request.body);
    }
    const progs = progressPopper(prog);
    const xhr = new XMLHttpRequest();
    const upload = isDefined(body) ? trackProgress("uploading", xhr, xhr.upload, progs.pop(), false) : Promise.resolve();
    const download = trackProgress("saving", xhr, xhr, progs.pop(), true, upload);
    sendRequest(xhr, xhrType, "POST", request.path, request.timeout, headers, body);
    await upload;
    await download;
    return readResponse(xhr);
  }
  getBlob(request, progress) {
    return this.getXHR("blob", request, progress);
  }
  postObjectForBlob(request, progress) {
    return this.postXHR("blob", request, progress);
  }
  getBuffer(request, progress) {
    return this.getXHR("arraybuffer", request, progress);
  }
  postObjectForBuffer(request, progress) {
    return this.postXHR("arraybuffer", request, progress);
  }
  getText(request, progress) {
    return this.getXHR("text", request, progress);
  }
  postObjectForText(request, progress) {
    return this.postXHR("text", request, progress);
  }
  async getObject(request, progress) {
    const response = await this.getXHR("json", request, progress);
    return response.content;
  }
  async postObjectForObject(request, progress) {
    const response = await this.postXHR("json", request, progress);
    return response.content;
  }
  postObject(request, progress) {
    return this.postXHR("", request, progress);
  }
  getFile(request, progress) {
    return this.translateResponse(this.getBlob(request, progress), URL.createObjectURL);
  }
  postObjectForFile(request, progress) {
    return this.translateResponse(this.postObjectForBlob(request, progress), URL.createObjectURL);
  }
  getXml(request, progress) {
    return this.translateResponse(this.getXHR("document", request, progress), (doc) => doc.documentElement);
  }
  postObjectForXml(request, progress) {
    return this.translateResponse(this.postXHR("document", request, progress), (doc) => doc.documentElement);
  }
  getImageBitmap(request, progress) {
    return this.translateResponse(this.getBlob(request, progress), createImageBitmap);
  }
  async postObjectForImageBitmap(request, progress) {
    return this.translateResponse(this.postObjectForBlob(request, progress), createImageBitmap);
  }
};

// src/FetchingServiceServer.ts
var FetchingServiceServer = class extends WorkerServer {
  constructor(self) {
    super(self);
    const fetcher = new FetchingServiceImpl();
    addFetcherMethods(this, fetcher);
  }
};
function getContent(response) {
  return [response.content];
}
function addFetcherMethods(server, fetcher) {
  server.addVoidMethod("setRequestVerificationToken", fetcher, fetcher.setRequestVerificationToken);
  server.addMethod("getBuffer", fetcher, fetcher.getBuffer, getContent);
  server.addMethod("postObjectForBuffer", fetcher, fetcher.postObjectForBuffer, getContent);
  server.addMethod("getImageBitmap", fetcher, fetcher.getImageBitmap, getContent);
  server.addMethod("postObjectForImageBitmap", fetcher, fetcher.postObjectForImageBitmap, getContent);
  server.addMethod("getObject", fetcher, fetcher.getObject);
  server.addMethod("getFile", fetcher, fetcher.getFile);
  server.addMethod("getText", fetcher, fetcher.getText);
  server.addMethod("postObject", fetcher, fetcher.postObject);
  server.addMethod("postObjectForObject", fetcher, fetcher.postObjectForObject);
  server.addMethod("postObjectForFile", fetcher, fetcher.postObjectForFile);
  server.addMethod("postObjectForText", fetcher, fetcher.postObjectForText);
  server.addMethod("head", fetcher, fetcher.head);
}

// src/index.ts
globalThis.server = new FetchingServiceServer(globalThis);
//# sourceMappingURL=index.js.map
