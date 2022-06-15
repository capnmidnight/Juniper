var __create = Object.create;
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __getProtoOf = Object.getPrototypeOf;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __require = /* @__PURE__ */ ((x) => typeof require !== "undefined" ? require : typeof Proxy !== "undefined" ? new Proxy(x, {
  get: (a, b) => (typeof require !== "undefined" ? require : a)[b]
}) : x)(function(x) {
  if (typeof require !== "undefined")
    return require.apply(this, arguments);
  throw new Error('Dynamic require of "' + x + '" is not supported');
});
var __commonJS = (cb, mod) => function __require2() {
  return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
};
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toESM = (mod, isNodeMode, target) => (target = mod != null ? __create(__getProtoOf(mod)) : {}, __copyProps(isNodeMode || !mod || !mod.__esModule ? __defProp(target, "default", { value: mod, enumerable: true }) : target, mod));

// ../../node_modules/sdp/sdp.js
var require_sdp = __commonJS({
  "../../node_modules/sdp/sdp.js"(exports, module) {
    "use strict";
    var SDPUtils2 = {};
    SDPUtils2.generateIdentifier = function() {
      return Math.random().toString(36).substr(2, 10);
    };
    SDPUtils2.localCName = SDPUtils2.generateIdentifier();
    SDPUtils2.splitLines = function(blob) {
      return blob.trim().split("\n").map((line) => line.trim());
    };
    SDPUtils2.splitSections = function(blob) {
      const parts = blob.split("\nm=");
      return parts.map((part, index) => (index > 0 ? "m=" + part : part).trim() + "\r\n");
    };
    SDPUtils2.getDescription = function(blob) {
      const sections = SDPUtils2.splitSections(blob);
      return sections && sections[0];
    };
    SDPUtils2.getMediaSections = function(blob) {
      const sections = SDPUtils2.splitSections(blob);
      sections.shift();
      return sections;
    };
    SDPUtils2.matchPrefix = function(blob, prefix) {
      return SDPUtils2.splitLines(blob).filter((line) => line.indexOf(prefix) === 0);
    };
    SDPUtils2.parseCandidate = function(line) {
      let parts;
      if (line.indexOf("a=candidate:") === 0) {
        parts = line.substring(12).split(" ");
      } else {
        parts = line.substring(10).split(" ");
      }
      const candidate = {
        foundation: parts[0],
        component: { 1: "rtp", 2: "rtcp" }[parts[1]] || parts[1],
        protocol: parts[2].toLowerCase(),
        priority: parseInt(parts[3], 10),
        ip: parts[4],
        address: parts[4],
        port: parseInt(parts[5], 10),
        type: parts[7]
      };
      for (let i = 8; i < parts.length; i += 2) {
        switch (parts[i]) {
          case "raddr":
            candidate.relatedAddress = parts[i + 1];
            break;
          case "rport":
            candidate.relatedPort = parseInt(parts[i + 1], 10);
            break;
          case "tcptype":
            candidate.tcpType = parts[i + 1];
            break;
          case "ufrag":
            candidate.ufrag = parts[i + 1];
            candidate.usernameFragment = parts[i + 1];
            break;
          default:
            if (candidate[parts[i]] === void 0) {
              candidate[parts[i]] = parts[i + 1];
            }
            break;
        }
      }
      return candidate;
    };
    SDPUtils2.writeCandidate = function(candidate) {
      const sdp2 = [];
      sdp2.push(candidate.foundation);
      const component = candidate.component;
      if (component === "rtp") {
        sdp2.push(1);
      } else if (component === "rtcp") {
        sdp2.push(2);
      } else {
        sdp2.push(component);
      }
      sdp2.push(candidate.protocol.toUpperCase());
      sdp2.push(candidate.priority);
      sdp2.push(candidate.address || candidate.ip);
      sdp2.push(candidate.port);
      const type2 = candidate.type;
      sdp2.push("typ");
      sdp2.push(type2);
      if (type2 !== "host" && candidate.relatedAddress && candidate.relatedPort) {
        sdp2.push("raddr");
        sdp2.push(candidate.relatedAddress);
        sdp2.push("rport");
        sdp2.push(candidate.relatedPort);
      }
      if (candidate.tcpType && candidate.protocol.toLowerCase() === "tcp") {
        sdp2.push("tcptype");
        sdp2.push(candidate.tcpType);
      }
      if (candidate.usernameFragment || candidate.ufrag) {
        sdp2.push("ufrag");
        sdp2.push(candidate.usernameFragment || candidate.ufrag);
      }
      return "candidate:" + sdp2.join(" ");
    };
    SDPUtils2.parseIceOptions = function(line) {
      return line.substr(14).split(" ");
    };
    SDPUtils2.parseRtpMap = function(line) {
      let parts = line.substr(9).split(" ");
      const parsed = {
        payloadType: parseInt(parts.shift(), 10)
      };
      parts = parts[0].split("/");
      parsed.name = parts[0];
      parsed.clockRate = parseInt(parts[1], 10);
      parsed.channels = parts.length === 3 ? parseInt(parts[2], 10) : 1;
      parsed.numChannels = parsed.channels;
      return parsed;
    };
    SDPUtils2.writeRtpMap = function(codec) {
      let pt = codec.payloadType;
      if (codec.preferredPayloadType !== void 0) {
        pt = codec.preferredPayloadType;
      }
      const channels = codec.channels || codec.numChannels || 1;
      return "a=rtpmap:" + pt + " " + codec.name + "/" + codec.clockRate + (channels !== 1 ? "/" + channels : "") + "\r\n";
    };
    SDPUtils2.parseExtmap = function(line) {
      const parts = line.substr(9).split(" ");
      return {
        id: parseInt(parts[0], 10),
        direction: parts[0].indexOf("/") > 0 ? parts[0].split("/")[1] : "sendrecv",
        uri: parts[1]
      };
    };
    SDPUtils2.writeExtmap = function(headerExtension) {
      return "a=extmap:" + (headerExtension.id || headerExtension.preferredId) + (headerExtension.direction && headerExtension.direction !== "sendrecv" ? "/" + headerExtension.direction : "") + " " + headerExtension.uri + "\r\n";
    };
    SDPUtils2.parseFmtp = function(line) {
      const parsed = {};
      let kv;
      const parts = line.substr(line.indexOf(" ") + 1).split(";");
      for (let j = 0; j < parts.length; j++) {
        kv = parts[j].trim().split("=");
        parsed[kv[0].trim()] = kv[1];
      }
      return parsed;
    };
    SDPUtils2.writeFmtp = function(codec) {
      let line = "";
      let pt = codec.payloadType;
      if (codec.preferredPayloadType !== void 0) {
        pt = codec.preferredPayloadType;
      }
      if (codec.parameters && Object.keys(codec.parameters).length) {
        const params = [];
        Object.keys(codec.parameters).forEach((param) => {
          if (codec.parameters[param] !== void 0) {
            params.push(param + "=" + codec.parameters[param]);
          } else {
            params.push(param);
          }
        });
        line += "a=fmtp:" + pt + " " + params.join(";") + "\r\n";
      }
      return line;
    };
    SDPUtils2.parseRtcpFb = function(line) {
      const parts = line.substr(line.indexOf(" ") + 1).split(" ");
      return {
        type: parts.shift(),
        parameter: parts.join(" ")
      };
    };
    SDPUtils2.writeRtcpFb = function(codec) {
      let lines = "";
      let pt = codec.payloadType;
      if (codec.preferredPayloadType !== void 0) {
        pt = codec.preferredPayloadType;
      }
      if (codec.rtcpFeedback && codec.rtcpFeedback.length) {
        codec.rtcpFeedback.forEach((fb) => {
          lines += "a=rtcp-fb:" + pt + " " + fb.type + (fb.parameter && fb.parameter.length ? " " + fb.parameter : "") + "\r\n";
        });
      }
      return lines;
    };
    SDPUtils2.parseSsrcMedia = function(line) {
      const sp = line.indexOf(" ");
      const parts = {
        ssrc: parseInt(line.substr(7, sp - 7), 10)
      };
      const colon = line.indexOf(":", sp);
      if (colon > -1) {
        parts.attribute = line.substr(sp + 1, colon - sp - 1);
        parts.value = line.substr(colon + 1);
      } else {
        parts.attribute = line.substr(sp + 1);
      }
      return parts;
    };
    SDPUtils2.parseSsrcGroup = function(line) {
      const parts = line.substr(13).split(" ");
      return {
        semantics: parts.shift(),
        ssrcs: parts.map((ssrc) => parseInt(ssrc, 10))
      };
    };
    SDPUtils2.getMid = function(mediaSection) {
      const mid = SDPUtils2.matchPrefix(mediaSection, "a=mid:")[0];
      if (mid) {
        return mid.substr(6);
      }
    };
    SDPUtils2.parseFingerprint = function(line) {
      const parts = line.substr(14).split(" ");
      return {
        algorithm: parts[0].toLowerCase(),
        value: parts[1].toUpperCase()
      };
    };
    SDPUtils2.getDtlsParameters = function(mediaSection, sessionpart) {
      const lines = SDPUtils2.matchPrefix(mediaSection + sessionpart, "a=fingerprint:");
      return {
        role: "auto",
        fingerprints: lines.map(SDPUtils2.parseFingerprint)
      };
    };
    SDPUtils2.writeDtlsParameters = function(params, setupType) {
      let sdp2 = "a=setup:" + setupType + "\r\n";
      params.fingerprints.forEach((fp) => {
        sdp2 += "a=fingerprint:" + fp.algorithm + " " + fp.value + "\r\n";
      });
      return sdp2;
    };
    SDPUtils2.parseCryptoLine = function(line) {
      const parts = line.substr(9).split(" ");
      return {
        tag: parseInt(parts[0], 10),
        cryptoSuite: parts[1],
        keyParams: parts[2],
        sessionParams: parts.slice(3)
      };
    };
    SDPUtils2.writeCryptoLine = function(parameters) {
      return "a=crypto:" + parameters.tag + " " + parameters.cryptoSuite + " " + (typeof parameters.keyParams === "object" ? SDPUtils2.writeCryptoKeyParams(parameters.keyParams) : parameters.keyParams) + (parameters.sessionParams ? " " + parameters.sessionParams.join(" ") : "") + "\r\n";
    };
    SDPUtils2.parseCryptoKeyParams = function(keyParams) {
      if (keyParams.indexOf("inline:") !== 0) {
        return null;
      }
      const parts = keyParams.substr(7).split("|");
      return {
        keyMethod: "inline",
        keySalt: parts[0],
        lifeTime: parts[1],
        mkiValue: parts[2] ? parts[2].split(":")[0] : void 0,
        mkiLength: parts[2] ? parts[2].split(":")[1] : void 0
      };
    };
    SDPUtils2.writeCryptoKeyParams = function(keyParams) {
      return keyParams.keyMethod + ":" + keyParams.keySalt + (keyParams.lifeTime ? "|" + keyParams.lifeTime : "") + (keyParams.mkiValue && keyParams.mkiLength ? "|" + keyParams.mkiValue + ":" + keyParams.mkiLength : "");
    };
    SDPUtils2.getCryptoParameters = function(mediaSection, sessionpart) {
      const lines = SDPUtils2.matchPrefix(mediaSection + sessionpart, "a=crypto:");
      return lines.map(SDPUtils2.parseCryptoLine);
    };
    SDPUtils2.getIceParameters = function(mediaSection, sessionpart) {
      const ufrag = SDPUtils2.matchPrefix(mediaSection + sessionpart, "a=ice-ufrag:")[0];
      const pwd = SDPUtils2.matchPrefix(mediaSection + sessionpart, "a=ice-pwd:")[0];
      if (!(ufrag && pwd)) {
        return null;
      }
      return {
        usernameFragment: ufrag.substr(12),
        password: pwd.substr(10)
      };
    };
    SDPUtils2.writeIceParameters = function(params) {
      let sdp2 = "a=ice-ufrag:" + params.usernameFragment + "\r\na=ice-pwd:" + params.password + "\r\n";
      if (params.iceLite) {
        sdp2 += "a=ice-lite\r\n";
      }
      return sdp2;
    };
    SDPUtils2.parseRtpParameters = function(mediaSection) {
      const description = {
        codecs: [],
        headerExtensions: [],
        fecMechanisms: [],
        rtcp: []
      };
      const lines = SDPUtils2.splitLines(mediaSection);
      const mline = lines[0].split(" ");
      for (let i = 3; i < mline.length; i++) {
        const pt = mline[i];
        const rtpmapline = SDPUtils2.matchPrefix(mediaSection, "a=rtpmap:" + pt + " ")[0];
        if (rtpmapline) {
          const codec = SDPUtils2.parseRtpMap(rtpmapline);
          const fmtps = SDPUtils2.matchPrefix(mediaSection, "a=fmtp:" + pt + " ");
          codec.parameters = fmtps.length ? SDPUtils2.parseFmtp(fmtps[0]) : {};
          codec.rtcpFeedback = SDPUtils2.matchPrefix(mediaSection, "a=rtcp-fb:" + pt + " ").map(SDPUtils2.parseRtcpFb);
          description.codecs.push(codec);
          switch (codec.name.toUpperCase()) {
            case "RED":
            case "ULPFEC":
              description.fecMechanisms.push(codec.name.toUpperCase());
              break;
            default:
              break;
          }
        }
      }
      SDPUtils2.matchPrefix(mediaSection, "a=extmap:").forEach((line) => {
        description.headerExtensions.push(SDPUtils2.parseExtmap(line));
      });
      return description;
    };
    SDPUtils2.writeRtpDescription = function(kind, caps) {
      let sdp2 = "";
      sdp2 += "m=" + kind + " ";
      sdp2 += caps.codecs.length > 0 ? "9" : "0";
      sdp2 += " UDP/TLS/RTP/SAVPF ";
      sdp2 += caps.codecs.map((codec) => {
        if (codec.preferredPayloadType !== void 0) {
          return codec.preferredPayloadType;
        }
        return codec.payloadType;
      }).join(" ") + "\r\n";
      sdp2 += "c=IN IP4 0.0.0.0\r\n";
      sdp2 += "a=rtcp:9 IN IP4 0.0.0.0\r\n";
      caps.codecs.forEach((codec) => {
        sdp2 += SDPUtils2.writeRtpMap(codec);
        sdp2 += SDPUtils2.writeFmtp(codec);
        sdp2 += SDPUtils2.writeRtcpFb(codec);
      });
      let maxptime = 0;
      caps.codecs.forEach((codec) => {
        if (codec.maxptime > maxptime) {
          maxptime = codec.maxptime;
        }
      });
      if (maxptime > 0) {
        sdp2 += "a=maxptime:" + maxptime + "\r\n";
      }
      if (caps.headerExtensions) {
        caps.headerExtensions.forEach((extension) => {
          sdp2 += SDPUtils2.writeExtmap(extension);
        });
      }
      return sdp2;
    };
    SDPUtils2.parseRtpEncodingParameters = function(mediaSection) {
      const encodingParameters = [];
      const description = SDPUtils2.parseRtpParameters(mediaSection);
      const hasRed = description.fecMechanisms.indexOf("RED") !== -1;
      const hasUlpfec = description.fecMechanisms.indexOf("ULPFEC") !== -1;
      const ssrcs = SDPUtils2.matchPrefix(mediaSection, "a=ssrc:").map((line) => SDPUtils2.parseSsrcMedia(line)).filter((parts) => parts.attribute === "cname");
      const primarySsrc = ssrcs.length > 0 && ssrcs[0].ssrc;
      let secondarySsrc;
      const flows = SDPUtils2.matchPrefix(mediaSection, "a=ssrc-group:FID").map((line) => {
        const parts = line.substr(17).split(" ");
        return parts.map((part) => parseInt(part, 10));
      });
      if (flows.length > 0 && flows[0].length > 1 && flows[0][0] === primarySsrc) {
        secondarySsrc = flows[0][1];
      }
      description.codecs.forEach((codec) => {
        if (codec.name.toUpperCase() === "RTX" && codec.parameters.apt) {
          let encParam = {
            ssrc: primarySsrc,
            codecPayloadType: parseInt(codec.parameters.apt, 10)
          };
          if (primarySsrc && secondarySsrc) {
            encParam.rtx = { ssrc: secondarySsrc };
          }
          encodingParameters.push(encParam);
          if (hasRed) {
            encParam = JSON.parse(JSON.stringify(encParam));
            encParam.fec = {
              ssrc: primarySsrc,
              mechanism: hasUlpfec ? "red+ulpfec" : "red"
            };
            encodingParameters.push(encParam);
          }
        }
      });
      if (encodingParameters.length === 0 && primarySsrc) {
        encodingParameters.push({
          ssrc: primarySsrc
        });
      }
      let bandwidth = SDPUtils2.matchPrefix(mediaSection, "b=");
      if (bandwidth.length) {
        if (bandwidth[0].indexOf("b=TIAS:") === 0) {
          bandwidth = parseInt(bandwidth[0].substr(7), 10);
        } else if (bandwidth[0].indexOf("b=AS:") === 0) {
          bandwidth = parseInt(bandwidth[0].substr(5), 10) * 1e3 * 0.95 - 50 * 40 * 8;
        } else {
          bandwidth = void 0;
        }
        encodingParameters.forEach((params) => {
          params.maxBitrate = bandwidth;
        });
      }
      return encodingParameters;
    };
    SDPUtils2.parseRtcpParameters = function(mediaSection) {
      const rtcpParameters = {};
      const remoteSsrc = SDPUtils2.matchPrefix(mediaSection, "a=ssrc:").map((line) => SDPUtils2.parseSsrcMedia(line)).filter((obj2) => obj2.attribute === "cname")[0];
      if (remoteSsrc) {
        rtcpParameters.cname = remoteSsrc.value;
        rtcpParameters.ssrc = remoteSsrc.ssrc;
      }
      const rsize = SDPUtils2.matchPrefix(mediaSection, "a=rtcp-rsize");
      rtcpParameters.reducedSize = rsize.length > 0;
      rtcpParameters.compound = rsize.length === 0;
      const mux = SDPUtils2.matchPrefix(mediaSection, "a=rtcp-mux");
      rtcpParameters.mux = mux.length > 0;
      return rtcpParameters;
    };
    SDPUtils2.writeRtcpParameters = function(rtcpParameters) {
      let sdp2 = "";
      if (rtcpParameters.reducedSize) {
        sdp2 += "a=rtcp-rsize\r\n";
      }
      if (rtcpParameters.mux) {
        sdp2 += "a=rtcp-mux\r\n";
      }
      if (rtcpParameters.ssrc !== void 0 && rtcpParameters.cname) {
        sdp2 += "a=ssrc:" + rtcpParameters.ssrc + " cname:" + rtcpParameters.cname + "\r\n";
      }
      return sdp2;
    };
    SDPUtils2.parseMsid = function(mediaSection) {
      let parts;
      const spec = SDPUtils2.matchPrefix(mediaSection, "a=msid:");
      if (spec.length === 1) {
        parts = spec[0].substr(7).split(" ");
        return { stream: parts[0], track: parts[1] };
      }
      const planB = SDPUtils2.matchPrefix(mediaSection, "a=ssrc:").map((line) => SDPUtils2.parseSsrcMedia(line)).filter((msidParts) => msidParts.attribute === "msid");
      if (planB.length > 0) {
        parts = planB[0].value.split(" ");
        return { stream: parts[0], track: parts[1] };
      }
    };
    SDPUtils2.parseSctpDescription = function(mediaSection) {
      const mline = SDPUtils2.parseMLine(mediaSection);
      const maxSizeLine = SDPUtils2.matchPrefix(mediaSection, "a=max-message-size:");
      let maxMessageSize;
      if (maxSizeLine.length > 0) {
        maxMessageSize = parseInt(maxSizeLine[0].substr(19), 10);
      }
      if (isNaN(maxMessageSize)) {
        maxMessageSize = 65536;
      }
      const sctpPort = SDPUtils2.matchPrefix(mediaSection, "a=sctp-port:");
      if (sctpPort.length > 0) {
        return {
          port: parseInt(sctpPort[0].substr(12), 10),
          protocol: mline.fmt,
          maxMessageSize
        };
      }
      const sctpMapLines = SDPUtils2.matchPrefix(mediaSection, "a=sctpmap:");
      if (sctpMapLines.length > 0) {
        const parts = sctpMapLines[0].substr(10).split(" ");
        return {
          port: parseInt(parts[0], 10),
          protocol: parts[1],
          maxMessageSize
        };
      }
    };
    SDPUtils2.writeSctpDescription = function(media, sctp) {
      let output = [];
      if (media.protocol !== "DTLS/SCTP") {
        output = [
          "m=" + media.kind + " 9 " + media.protocol + " " + sctp.protocol + "\r\n",
          "c=IN IP4 0.0.0.0\r\n",
          "a=sctp-port:" + sctp.port + "\r\n"
        ];
      } else {
        output = [
          "m=" + media.kind + " 9 " + media.protocol + " " + sctp.port + "\r\n",
          "c=IN IP4 0.0.0.0\r\n",
          "a=sctpmap:" + sctp.port + " " + sctp.protocol + " 65535\r\n"
        ];
      }
      if (sctp.maxMessageSize !== void 0) {
        output.push("a=max-message-size:" + sctp.maxMessageSize + "\r\n");
      }
      return output.join("");
    };
    SDPUtils2.generateSessionId = function() {
      return Math.random().toString().substr(2, 21);
    };
    SDPUtils2.writeSessionBoilerplate = function(sessId, sessVer, sessUser) {
      let sessionId;
      const version = sessVer !== void 0 ? sessVer : 2;
      if (sessId) {
        sessionId = sessId;
      } else {
        sessionId = SDPUtils2.generateSessionId();
      }
      const user = sessUser || "thisisadapterortc";
      return "v=0\r\no=" + user + " " + sessionId + " " + version + " IN IP4 127.0.0.1\r\ns=-\r\nt=0 0\r\n";
    };
    SDPUtils2.getDirection = function(mediaSection, sessionpart) {
      const lines = SDPUtils2.splitLines(mediaSection);
      for (let i = 0; i < lines.length; i++) {
        switch (lines[i]) {
          case "a=sendrecv":
          case "a=sendonly":
          case "a=recvonly":
          case "a=inactive":
            return lines[i].substr(2);
          default:
        }
      }
      if (sessionpart) {
        return SDPUtils2.getDirection(sessionpart);
      }
      return "sendrecv";
    };
    SDPUtils2.getKind = function(mediaSection) {
      const lines = SDPUtils2.splitLines(mediaSection);
      const mline = lines[0].split(" ");
      return mline[0].substr(2);
    };
    SDPUtils2.isRejected = function(mediaSection) {
      return mediaSection.split(" ", 2)[1] === "0";
    };
    SDPUtils2.parseMLine = function(mediaSection) {
      const lines = SDPUtils2.splitLines(mediaSection);
      const parts = lines[0].substr(2).split(" ");
      return {
        kind: parts[0],
        port: parseInt(parts[1], 10),
        protocol: parts[2],
        fmt: parts.slice(3).join(" ")
      };
    };
    SDPUtils2.parseOLine = function(mediaSection) {
      const line = SDPUtils2.matchPrefix(mediaSection, "o=")[0];
      const parts = line.substr(2).split(" ");
      return {
        username: parts[0],
        sessionId: parts[1],
        sessionVersion: parseInt(parts[2], 10),
        netType: parts[3],
        addressType: parts[4],
        address: parts[5]
      };
    };
    SDPUtils2.isValidSDP = function(blob) {
      if (typeof blob !== "string" || blob.length === 0) {
        return false;
      }
      const lines = SDPUtils2.splitLines(blob);
      for (let i = 0; i < lines.length; i++) {
        if (lines[i].length < 2 || lines[i].charAt(1) !== "=") {
          return false;
        }
      }
      return true;
    };
    if (typeof module === "object") {
      module.exports = SDPUtils2;
    }
  }
});

// ../tslib/collections/arrayBinarySearch.ts
function defaultKeySelector(obj2) {
  return obj2;
}
function arrayBinarySearchByKey(arr, itemKey, keySelector) {
  let left = 0;
  let right = arr.length;
  let idx = Math.floor((left + right) / 2);
  let found = false;
  while (left < right && idx < arr.length) {
    const compareTo = arr[idx];
    const compareToKey = isNullOrUndefined(compareTo) ? null : keySelector(compareTo);
    if (isDefined(compareToKey) && itemKey < compareToKey) {
      right = idx;
    } else {
      if (itemKey === compareToKey) {
        found = true;
      }
      left = idx + 1;
    }
    idx = Math.floor((left + right) / 2);
  }
  if (!found) {
    idx += 0.5;
  }
  return idx;
}
function arrayBinarySearch(arr, item, keySelector) {
  keySelector = keySelector || defaultKeySelector;
  const itemKey = keySelector(item);
  return arrayBinarySearchByKey(arr, itemKey, keySelector);
}

// ../tslib/collections/arrayRemoveAt.ts
function arrayRemoveAt(arr, idx) {
  return arr.splice(idx, 1)[0];
}

// ../tslib/collections/arrayClear.ts
function arrayClear(arr) {
  return arr.splice(0);
}

// ../tslib/collections/arrayCompare.ts
function arrayCompare(arr1, arr2) {
  for (let i = 0; i < arr1.length; ++i) {
    if (arr1[i] !== arr2[i]) {
      return i;
    }
  }
  return -1;
}

// ../tslib/collections/arrayInsertAt.ts
function arrayInsertAt(arr, item, idx) {
  arr.splice(idx, 0, item);
}

// ../tslib/collections/arrayRemove.ts
function arrayRemove(arr, value) {
  const idx = arr.indexOf(value);
  if (idx > -1) {
    arrayRemoveAt(arr, idx);
    return true;
  }
  return false;
}

// ../tslib/collections/arrayScan.ts
function arrayScan(arr, ...tests) {
  for (const test of tests) {
    for (const item of arr) {
      if (test(item)) {
        return item;
      }
    }
  }
  return null;
}

// ../tslib/collections/arraySortedInsert.ts
function arraySortedInsert(arr, item, keySelector, allowDuplicates) {
  let ks;
  if (isFunction(keySelector)) {
    ks = keySelector;
  } else if (isBoolean(keySelector)) {
    allowDuplicates = keySelector;
  }
  if (isNullOrUndefined(allowDuplicates)) {
    allowDuplicates = true;
  }
  return arraySortedInsertInternal(arr, item, ks, allowDuplicates);
}
function arraySortedInsertInternal(arr, item, ks, allowDuplicates) {
  let idx = arrayBinarySearch(arr, item, ks);
  const found = idx % 1 === 0;
  idx = idx | 0;
  if (!found || allowDuplicates) {
    arrayInsertAt(arr, item, idx);
  }
  return idx;
}

// ../tslib/collections/BaseGraphNode.ts
var BaseGraphNode = class {
  constructor(value) {
    this.value = value;
    this._forward = new Array();
    this._reverse = new Array();
  }
  connectSorted(child, keySelector) {
    if (isDefined(keySelector)) {
      arraySortedInsert(this._forward, child, (n2) => keySelector(n2.value));
      child._reverse.push(this);
    } else {
      this.connectTo(child);
    }
  }
  connectTo(child) {
    this.connectAt(child, this._forward.length);
  }
  connectAt(child, index) {
    arrayInsertAt(this._forward, child, index);
    child._reverse.push(this);
  }
  disconnectFrom(child) {
    arrayRemove(this._forward, child);
    arrayRemove(child._reverse, this);
  }
  isConnectedTo(node) {
    return this._forward.indexOf(node) >= 0 || this._reverse.indexOf(node) >= 0;
  }
  flatten() {
    const nodes = /* @__PURE__ */ new Set();
    const queue = [this];
    while (queue.length > 0) {
      const here = queue.shift();
      if (isDefined(here) && !nodes.has(here)) {
        nodes.add(here);
        queue.push(...here._forward);
      }
    }
    return Array.from(nodes);
  }
  get _isEntryPoint() {
    return this._reverse.length === 0;
  }
  get _isExitPoint() {
    return this._forward.length === 0;
  }
  get isDisconnected() {
    return this._isEntryPoint && this._isExitPoint;
  }
  get isConnected() {
    return !this._isExitPoint || !this._isEntryPoint;
  }
  get isTerminus() {
    return this._isEntryPoint || this._isExitPoint;
  }
  get isInternal() {
    return !this._isEntryPoint && !this._isExitPoint;
  }
};

// ../tslib/collections/GraphNode.ts
var GraphNode = class extends BaseGraphNode {
  connectTo(node) {
    super.connectTo(node);
  }
  connectAt(child, index) {
    super.connectAt(child, index);
  }
  connectSorted(child, sortKey) {
    super.connectSorted(child, sortKey);
  }
  disconnectFrom(node) {
    super.disconnectFrom(node);
  }
  isConnectedTo(node) {
    return super.isConnectedTo(node);
  }
  flatten() {
    return super.flatten();
  }
  get connections() {
    return this._forward;
  }
  get isEntryPoint() {
    return this._isEntryPoint;
  }
  get isExitPoint() {
    return this._isExitPoint;
  }
};

// ../tslib/collections/PriorityList.ts
var PriorityList = class {
  constructor(init) {
    this.items = /* @__PURE__ */ new Map();
    this.defaultItems = new Array();
    if (isDefined(init)) {
      for (const [key, value] of init) {
        this.add(key, value);
      }
    }
  }
  add(key, value) {
    if (isNullOrUndefined(key)) {
      this.defaultItems.push(value);
    } else {
      let list = this.items.get(key);
      if (isNullOrUndefined(list)) {
        this.items.set(key, list = []);
      }
      list.push(value);
    }
    return this;
  }
  entries() {
    return this.items.entries();
  }
  [Symbol.iterator]() {
    return this.entries();
  }
  keys() {
    return this.items.keys();
  }
  *values() {
    for (const item of this.defaultItems) {
      yield item;
    }
    for (const list of this.items.values()) {
      for (const item of list) {
        yield item;
      }
    }
  }
  has(key) {
    if (isDefined(key)) {
      return this.items.has(key);
    } else {
      return this.defaultItems.length > 0;
    }
  }
  get(key) {
    if (isNullOrUndefined(key)) {
      return this.defaultItems;
    }
    return this.items.get(key) || [];
  }
  count(key) {
    if (isNullOrUndefined(key)) {
      return this.defaultItems.length;
    }
    const list = this.get(key);
    if (isDefined(list)) {
      return list.length;
    }
    return 0;
  }
  get size() {
    let size = this.defaultItems.length;
    for (const list of this.items.values()) {
      size += list.length;
    }
    return size;
  }
  delete(key) {
    if (isNullOrUndefined(key)) {
      return arrayClear(this.defaultItems).length > 0;
    } else {
      return this.items.delete(key);
    }
  }
  remove(key, value) {
    if (isNullOrUndefined(key)) {
      arrayRemove(this.defaultItems, value);
    } else {
      const list = this.items.get(key);
      if (isDefined(list)) {
        arrayRemove(list, value);
        if (list.length === 0) {
          this.items.delete(key);
        }
      }
    }
  }
  clear() {
    this.items.clear();
    arrayClear(this.defaultItems);
  }
};

// ../tslib/events/EventBase.ts
var EventBase = class {
  constructor() {
    this.listeners = /* @__PURE__ */ new Map();
    this.listenerOptions = /* @__PURE__ */ new Map();
  }
  addEventListener(type2, callback, options) {
    if (isFunction(callback)) {
      let listeners = this.listeners.get(type2);
      if (!listeners) {
        listeners = new Array();
        this.listeners.set(type2, listeners);
      }
      if (!listeners.find((c) => c === callback)) {
        listeners.push(callback);
        if (options) {
          this.listenerOptions.set(callback, options);
        }
      }
    }
  }
  removeEventListener(type2, callback) {
    if (isFunction(callback)) {
      const listeners = this.listeners.get(type2);
      if (listeners) {
        this.removeListener(listeners, callback);
      }
    }
  }
  clearEventListeners(type2) {
    for (const [evtName, handlers] of this.listeners) {
      if (isNullOrUndefined(type2) || type2 === evtName) {
        for (const handler of handlers) {
          this.removeEventListener(type2, handler);
        }
        arrayClear(handlers);
        this.listeners.delete(evtName);
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
  constructor(type2) {
    super(type2);
  }
};
var TypedEventBase = class extends EventBase {
  constructor() {
    super(...arguments);
    this.bubblers = /* @__PURE__ */ new Set();
    this.scopes = /* @__PURE__ */ new WeakMap();
  }
  addBubbler(bubbler) {
    this.bubblers.add(bubbler);
  }
  removeBubbler(bubbler) {
    this.bubblers.delete(bubbler);
  }
  addEventListener(type2, callback, options) {
    super.addEventListener(type2, callback, options);
  }
  removeEventListener(type2, callback) {
    super.removeEventListener(type2, callback);
  }
  clearEventListeners(type2) {
    return super.clearEventListeners(type2);
  }
  addScopedEventListener(scope, type2, callback, options) {
    if (!this.scopes.has(scope)) {
      this.scopes.set(scope, []);
    }
    this.scopes.get(scope).push([type2, callback]);
    this.addEventListener(type2, callback, options);
  }
  removeScope(scope) {
    const listeners = this.scopes.get(scope);
    if (listeners) {
      this.scopes.delete(scope);
      for (const [type2, listener] of listeners) {
        this.removeEventListener(type2, listener);
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

// ../tslib/events/Task.ts
var Task = class {
  constructor(resolveTestOrAutoStart, rejectTestOrAutoStart, autoStart = true) {
    this._resolve = null;
    this._reject = null;
    this._result = null;
    this._error = null;
    this._started = false;
    this._finished = false;
    this.resolve = null;
    this.reject = null;
    let resolveTest = alwaysTrue;
    let rejectTest = alwaysTrue;
    if (isFunction(resolveTestOrAutoStart)) {
      resolveTest = resolveTestOrAutoStart;
    }
    if (isFunction(rejectTestOrAutoStart)) {
      rejectTest = rejectTestOrAutoStart;
    }
    if (isBoolean(resolveTestOrAutoStart)) {
      autoStart = resolveTestOrAutoStart;
    } else if (isBoolean(rejectTestOrAutoStart)) {
      autoStart = rejectTestOrAutoStart;
    }
    this.resolve = (value) => {
      if (isDefined(this._resolve)) {
        this._resolve(value);
      }
    };
    this.reject = (reason) => {
      if (isDefined(this._reject)) {
        this._reject(reason);
      }
    };
    this.promise = new Promise((resolve, reject) => {
      this._resolve = (value) => {
        if (resolveTest(value)) {
          this._result = value;
          this._finished = true;
          resolve(value);
        }
      };
      this._reject = (reason) => {
        if (rejectTest(reason)) {
          this._error = reason;
          this._finished = true;
          reject(reason);
        }
      };
    });
    if (autoStart) {
      this.start();
    }
  }
  get result() {
    if (isDefined(this.error)) {
      throw this.error;
    }
    return this._result;
  }
  get error() {
    return this._error;
  }
  get started() {
    return this._started;
  }
  get finished() {
    return this._finished;
  }
  start() {
    this._started = true;
  }
  get [Symbol.toStringTag]() {
    return this.promise.toString();
  }
  then(onfulfilled, onrejected) {
    return this.promise.then(onfulfilled, onrejected);
  }
  catch(onrejected) {
    return this.promise.catch(onrejected);
  }
  finally(onfinally) {
    return this.promise.finally(onfinally);
  }
};

// ../tslib/events/Promisifier.ts
var Promisifier = class {
  constructor(resolveRejectTest, selectValue, selectRejectionReason) {
    this.callback = null;
    this.promise = new Promise((resolve, reject) => {
      this.callback = (...args) => {
        if (resolveRejectTest(...args)) {
          resolve(selectValue(...args));
        } else {
          reject(selectRejectionReason(...args));
        }
      };
    });
  }
  get [Symbol.toStringTag]() {
    return this.promise.toString();
  }
  then(onfulfilled, onrejected) {
    return this.promise.then(onfulfilled, onrejected);
  }
  catch(onrejected) {
    return this.promise.catch(onrejected);
  }
  finally(onfinally) {
    return this.promise.finally(onfinally);
  }
};

// ../tslib/events/waitFor.ts
function waitFor(test) {
  const task = new Task(test);
  const handle = setInterval(() => {
    if (test()) {
      clearInterval(handle);
      task.resolve();
    }
  }, 100);
  return task;
}

// ../tslib/flags.ts
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

// ../tslib/gis/Datum.ts
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

// ../../node_modules/gl-matrix/esm/common.js
var EPSILON = 1e-6;
var ARRAY_TYPE = typeof Float32Array !== "undefined" ? Float32Array : Array;
var RANDOM = Math.random;
var degree = Math.PI / 180;
if (!Math.hypot)
  Math.hypot = function() {
    var y = 0, i = arguments.length;
    while (i--) {
      y += arguments[i] * arguments[i];
    }
    return Math.sqrt(y);
  };

// ../../node_modules/gl-matrix/esm/vec3.js
var vec3_exports = {};
__export(vec3_exports, {
  add: () => add,
  angle: () => angle,
  bezier: () => bezier,
  ceil: () => ceil,
  clone: () => clone,
  copy: () => copy,
  create: () => create,
  cross: () => cross,
  dist: () => dist,
  distance: () => distance,
  div: () => div,
  divide: () => divide,
  dot: () => dot,
  equals: () => equals,
  exactEquals: () => exactEquals,
  floor: () => floor,
  forEach: () => forEach,
  fromValues: () => fromValues,
  hermite: () => hermite,
  inverse: () => inverse,
  len: () => len,
  length: () => length,
  lerp: () => lerp,
  max: () => max,
  min: () => min,
  mul: () => mul,
  multiply: () => multiply,
  negate: () => negate,
  normalize: () => normalize,
  random: () => random,
  rotateX: () => rotateX,
  rotateY: () => rotateY,
  rotateZ: () => rotateZ,
  round: () => round,
  scale: () => scale,
  scaleAndAdd: () => scaleAndAdd,
  set: () => set,
  sqrDist: () => sqrDist,
  sqrLen: () => sqrLen,
  squaredDistance: () => squaredDistance,
  squaredLength: () => squaredLength,
  str: () => str,
  sub: () => sub,
  subtract: () => subtract,
  transformMat3: () => transformMat3,
  transformMat4: () => transformMat4,
  transformQuat: () => transformQuat,
  zero: () => zero
});
function create() {
  var out = new ARRAY_TYPE(3);
  if (ARRAY_TYPE != Float32Array) {
    out[0] = 0;
    out[1] = 0;
    out[2] = 0;
  }
  return out;
}
function clone(a) {
  var out = new ARRAY_TYPE(3);
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  return out;
}
function length(a) {
  var x = a[0];
  var y = a[1];
  var z = a[2];
  return Math.hypot(x, y, z);
}
function fromValues(x, y, z) {
  var out = new ARRAY_TYPE(3);
  out[0] = x;
  out[1] = y;
  out[2] = z;
  return out;
}
function copy(out, a) {
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  return out;
}
function set(out, x, y, z) {
  out[0] = x;
  out[1] = y;
  out[2] = z;
  return out;
}
function add(out, a, b) {
  out[0] = a[0] + b[0];
  out[1] = a[1] + b[1];
  out[2] = a[2] + b[2];
  return out;
}
function subtract(out, a, b) {
  out[0] = a[0] - b[0];
  out[1] = a[1] - b[1];
  out[2] = a[2] - b[2];
  return out;
}
function multiply(out, a, b) {
  out[0] = a[0] * b[0];
  out[1] = a[1] * b[1];
  out[2] = a[2] * b[2];
  return out;
}
function divide(out, a, b) {
  out[0] = a[0] / b[0];
  out[1] = a[1] / b[1];
  out[2] = a[2] / b[2];
  return out;
}
function ceil(out, a) {
  out[0] = Math.ceil(a[0]);
  out[1] = Math.ceil(a[1]);
  out[2] = Math.ceil(a[2]);
  return out;
}
function floor(out, a) {
  out[0] = Math.floor(a[0]);
  out[1] = Math.floor(a[1]);
  out[2] = Math.floor(a[2]);
  return out;
}
function min(out, a, b) {
  out[0] = Math.min(a[0], b[0]);
  out[1] = Math.min(a[1], b[1]);
  out[2] = Math.min(a[2], b[2]);
  return out;
}
function max(out, a, b) {
  out[0] = Math.max(a[0], b[0]);
  out[1] = Math.max(a[1], b[1]);
  out[2] = Math.max(a[2], b[2]);
  return out;
}
function round(out, a) {
  out[0] = Math.round(a[0]);
  out[1] = Math.round(a[1]);
  out[2] = Math.round(a[2]);
  return out;
}
function scale(out, a, b) {
  out[0] = a[0] * b;
  out[1] = a[1] * b;
  out[2] = a[2] * b;
  return out;
}
function scaleAndAdd(out, a, b, scale2) {
  out[0] = a[0] + b[0] * scale2;
  out[1] = a[1] + b[1] * scale2;
  out[2] = a[2] + b[2] * scale2;
  return out;
}
function distance(a, b) {
  var x = b[0] - a[0];
  var y = b[1] - a[1];
  var z = b[2] - a[2];
  return Math.hypot(x, y, z);
}
function squaredDistance(a, b) {
  var x = b[0] - a[0];
  var y = b[1] - a[1];
  var z = b[2] - a[2];
  return x * x + y * y + z * z;
}
function squaredLength(a) {
  var x = a[0];
  var y = a[1];
  var z = a[2];
  return x * x + y * y + z * z;
}
function negate(out, a) {
  out[0] = -a[0];
  out[1] = -a[1];
  out[2] = -a[2];
  return out;
}
function inverse(out, a) {
  out[0] = 1 / a[0];
  out[1] = 1 / a[1];
  out[2] = 1 / a[2];
  return out;
}
function normalize(out, a) {
  var x = a[0];
  var y = a[1];
  var z = a[2];
  var len2 = x * x + y * y + z * z;
  if (len2 > 0) {
    len2 = 1 / Math.sqrt(len2);
  }
  out[0] = a[0] * len2;
  out[1] = a[1] * len2;
  out[2] = a[2] * len2;
  return out;
}
function dot(a, b) {
  return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
}
function cross(out, a, b) {
  var ax = a[0], ay = a[1], az = a[2];
  var bx = b[0], by = b[1], bz = b[2];
  out[0] = ay * bz - az * by;
  out[1] = az * bx - ax * bz;
  out[2] = ax * by - ay * bx;
  return out;
}
function lerp(out, a, b, t2) {
  var ax = a[0];
  var ay = a[1];
  var az = a[2];
  out[0] = ax + t2 * (b[0] - ax);
  out[1] = ay + t2 * (b[1] - ay);
  out[2] = az + t2 * (b[2] - az);
  return out;
}
function hermite(out, a, b, c, d, t2) {
  var factorTimes2 = t2 * t2;
  var factor1 = factorTimes2 * (2 * t2 - 3) + 1;
  var factor2 = factorTimes2 * (t2 - 2) + t2;
  var factor3 = factorTimes2 * (t2 - 1);
  var factor4 = factorTimes2 * (3 - 2 * t2);
  out[0] = a[0] * factor1 + b[0] * factor2 + c[0] * factor3 + d[0] * factor4;
  out[1] = a[1] * factor1 + b[1] * factor2 + c[1] * factor3 + d[1] * factor4;
  out[2] = a[2] * factor1 + b[2] * factor2 + c[2] * factor3 + d[2] * factor4;
  return out;
}
function bezier(out, a, b, c, d, t2) {
  var inverseFactor = 1 - t2;
  var inverseFactorTimesTwo = inverseFactor * inverseFactor;
  var factorTimes2 = t2 * t2;
  var factor1 = inverseFactorTimesTwo * inverseFactor;
  var factor2 = 3 * t2 * inverseFactorTimesTwo;
  var factor3 = 3 * factorTimes2 * inverseFactor;
  var factor4 = factorTimes2 * t2;
  out[0] = a[0] * factor1 + b[0] * factor2 + c[0] * factor3 + d[0] * factor4;
  out[1] = a[1] * factor1 + b[1] * factor2 + c[1] * factor3 + d[1] * factor4;
  out[2] = a[2] * factor1 + b[2] * factor2 + c[2] * factor3 + d[2] * factor4;
  return out;
}
function random(out, scale2) {
  scale2 = scale2 || 1;
  var r = RANDOM() * 2 * Math.PI;
  var z = RANDOM() * 2 - 1;
  var zScale = Math.sqrt(1 - z * z) * scale2;
  out[0] = Math.cos(r) * zScale;
  out[1] = Math.sin(r) * zScale;
  out[2] = z * scale2;
  return out;
}
function transformMat4(out, a, m) {
  var x = a[0], y = a[1], z = a[2];
  var w = m[3] * x + m[7] * y + m[11] * z + m[15];
  w = w || 1;
  out[0] = (m[0] * x + m[4] * y + m[8] * z + m[12]) / w;
  out[1] = (m[1] * x + m[5] * y + m[9] * z + m[13]) / w;
  out[2] = (m[2] * x + m[6] * y + m[10] * z + m[14]) / w;
  return out;
}
function transformMat3(out, a, m) {
  var x = a[0], y = a[1], z = a[2];
  out[0] = x * m[0] + y * m[3] + z * m[6];
  out[1] = x * m[1] + y * m[4] + z * m[7];
  out[2] = x * m[2] + y * m[5] + z * m[8];
  return out;
}
function transformQuat(out, a, q) {
  var qx = q[0], qy = q[1], qz = q[2], qw = q[3];
  var x = a[0], y = a[1], z = a[2];
  var uvx = qy * z - qz * y, uvy = qz * x - qx * z, uvz = qx * y - qy * x;
  var uuvx = qy * uvz - qz * uvy, uuvy = qz * uvx - qx * uvz, uuvz = qx * uvy - qy * uvx;
  var w2 = qw * 2;
  uvx *= w2;
  uvy *= w2;
  uvz *= w2;
  uuvx *= 2;
  uuvy *= 2;
  uuvz *= 2;
  out[0] = x + uvx + uuvx;
  out[1] = y + uvy + uuvy;
  out[2] = z + uvz + uuvz;
  return out;
}
function rotateX(out, a, b, rad) {
  var p = [], r = [];
  p[0] = a[0] - b[0];
  p[1] = a[1] - b[1];
  p[2] = a[2] - b[2];
  r[0] = p[0];
  r[1] = p[1] * Math.cos(rad) - p[2] * Math.sin(rad);
  r[2] = p[1] * Math.sin(rad) + p[2] * Math.cos(rad);
  out[0] = r[0] + b[0];
  out[1] = r[1] + b[1];
  out[2] = r[2] + b[2];
  return out;
}
function rotateY(out, a, b, rad) {
  var p = [], r = [];
  p[0] = a[0] - b[0];
  p[1] = a[1] - b[1];
  p[2] = a[2] - b[2];
  r[0] = p[2] * Math.sin(rad) + p[0] * Math.cos(rad);
  r[1] = p[1];
  r[2] = p[2] * Math.cos(rad) - p[0] * Math.sin(rad);
  out[0] = r[0] + b[0];
  out[1] = r[1] + b[1];
  out[2] = r[2] + b[2];
  return out;
}
function rotateZ(out, a, b, rad) {
  var p = [], r = [];
  p[0] = a[0] - b[0];
  p[1] = a[1] - b[1];
  p[2] = a[2] - b[2];
  r[0] = p[0] * Math.cos(rad) - p[1] * Math.sin(rad);
  r[1] = p[0] * Math.sin(rad) + p[1] * Math.cos(rad);
  r[2] = p[2];
  out[0] = r[0] + b[0];
  out[1] = r[1] + b[1];
  out[2] = r[2] + b[2];
  return out;
}
function angle(a, b) {
  var ax = a[0], ay = a[1], az = a[2], bx = b[0], by = b[1], bz = b[2], mag1 = Math.sqrt(ax * ax + ay * ay + az * az), mag2 = Math.sqrt(bx * bx + by * by + bz * bz), mag = mag1 * mag2, cosine = mag && dot(a, b) / mag;
  return Math.acos(Math.min(Math.max(cosine, -1), 1));
}
function zero(out) {
  out[0] = 0;
  out[1] = 0;
  out[2] = 0;
  return out;
}
function str(a) {
  return "vec3(" + a[0] + ", " + a[1] + ", " + a[2] + ")";
}
function exactEquals(a, b) {
  return a[0] === b[0] && a[1] === b[1] && a[2] === b[2];
}
function equals(a, b) {
  var a0 = a[0], a1 = a[1], a2 = a[2];
  var b0 = b[0], b1 = b[1], b2 = b[2];
  return Math.abs(a0 - b0) <= EPSILON * Math.max(1, Math.abs(a0), Math.abs(b0)) && Math.abs(a1 - b1) <= EPSILON * Math.max(1, Math.abs(a1), Math.abs(b1)) && Math.abs(a2 - b2) <= EPSILON * Math.max(1, Math.abs(a2), Math.abs(b2));
}
var sub = subtract;
var mul = multiply;
var div = divide;
var dist = distance;
var sqrDist = squaredDistance;
var len = length;
var sqrLen = squaredLength;
var forEach = function() {
  var vec = create();
  return function(a, stride, offset, count, fn, arg) {
    var i, l;
    if (!stride) {
      stride = 3;
    }
    if (!offset) {
      offset = 0;
    }
    if (count) {
      l = Math.min(count * stride + offset, a.length);
    } else {
      l = a.length;
    }
    for (i = offset; i < l; i += stride) {
      vec[0] = a[i];
      vec[1] = a[i + 1];
      vec[2] = a[i + 2];
      fn(vec, vec, arg);
      a[i] = vec[0];
      a[i + 1] = vec[1];
      a[i + 2] = vec[2];
    }
    return a;
  };
}();

// ../tslib/identity.ts
function alwaysTrue() {
  return true;
}

// ../tslib/math/angleClamp.ts
var Tau = 2 * Math.PI;
function angleClamp(v) {
  return (v % Tau + Tau) % Tau;
}

// ../tslib/math/clamp.ts
function clamp(v, min2, max2) {
  return Math.min(max2, Math.max(min2, v));
}

// ../tslib/math/deg2rad.ts
function deg2rad(deg) {
  return deg * Math.PI / 180;
}

// ../tslib/math/unproject.ts
function unproject(v, min2, max2) {
  return v * (max2 - min2) + min2;
}

// ../tslib/math/lerp.ts
function lerp2(a, b, p) {
  return (1 - p) * a + p * b;
}

// ../tslib/progress/BaseProgress.ts
var BaseProgress = class extends TypedEventBase {
  constructor() {
    super(...arguments);
    this.attached = new Array();
    this.soFar = null;
    this.total = null;
    this.msg = null;
    this.est = null;
  }
  get p() {
    return this.total > 0 ? this.soFar / this.total : 0;
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
  clear() {
    this.report(0, 0);
    this._clear();
  }
  start(msg) {
    this.report(0, 1, msg || "starting");
  }
  end(msg) {
    this.report(1, 1, msg || "done");
    this._clear();
  }
  _clear() {
    this.soFar = null;
    this.total = null;
    this.msg = null;
    this.est = null;
    arrayClear(this.attached);
  }
};

// ../tslib/progress/ChildProgressCallback.ts
var ChildProgressCallback = class extends BaseProgress {
  constructor(i, prog) {
    super();
    this.i = i;
    this.prog = prog;
  }
  report(soFar, total, msg, est) {
    super.report(soFar, total, msg, est);
    this.prog.update(this.i, soFar, total, msg);
  }
};

// ../tslib/progress/BaseParentProgressCallback.ts
var BaseParentProgressCallback = class {
  constructor(prog) {
    this.prog = prog;
    this.weightTotal = 0;
    this.subProgressCallbacks = new Array();
    this.subProgressWeights = new Array();
    this.subProgressValues = new Array();
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

// ../tslib/progress/progressSplit.ts
function progressSplitWeighted(prog, subProgressWeights) {
  const subProg = new WeightedParentProgressCallback(subProgressWeights, prog);
  return subProg.subProgressCallbacks;
}
var WeightedParentProgressCallback = class extends BaseParentProgressCallback {
  constructor(subProgressWeights, prog) {
    super(prog);
    for (const weight of subProgressWeights) {
      this.addSubProgress(weight);
    }
  }
};

// ../tslib/progress/progressTasks.ts
async function progressTasksWeighted(prog, taskDefs) {
  const weights = new Array(taskDefs.length);
  const callbacks = new Array(taskDefs.length);
  for (let i = 0; i < taskDefs.length; ++i) {
    const taskDef = taskDefs[i];
    weights[i] = taskDef[0];
    callbacks[i] = taskDef[1];
  }
  const progs = progressSplitWeighted(prog, weights);
  const tasks = new Array(taskDefs.length);
  for (let i = 0; i < taskDefs.length; ++i) {
    tasks[i] = callbacks[i](progs[i]);
  }
  return await Promise.all(tasks);
}
function progressTasks(prog, ...subTaskDef) {
  const taskDefs = subTaskDef.map((t2) => [1, t2]);
  return progressTasksWeighted(prog, taskDefs);
}

// ../tslib/typeChecks.ts
function t(o, s, c) {
  return typeof o === s || o instanceof c;
}
function isFunction(obj2) {
  return t(obj2, "function", Function);
}
function isBoolean(obj2) {
  return t(obj2, "boolean", Boolean);
}
function isNumber(obj2) {
  return t(obj2, "number", Number);
}
function isObject(obj2) {
  return isDefined(obj2) && t(obj2, "object", Object);
}
function isArray(obj2) {
  return obj2 instanceof Array;
}
function assertNever(x, msg) {
  throw new Error((msg || "Unexpected object: ") + x);
}
function isNullOrUndefined(obj2) {
  return obj2 === null || obj2 === void 0;
}
function isDefined(obj2) {
  return !isNullOrUndefined(obj2);
}
function isArrayBuffer(val) {
  return val && typeof ArrayBuffer !== "undefined" && (val instanceof ArrayBuffer || val.constructor && val.constructor.name === "ArrayBuffer");
}

// ../tslib/singleton.ts
function singleton(name, create2) {
  const box = globalThis;
  let value = box[name];
  if (isNullOrUndefined(value)) {
    if (isNullOrUndefined(create2)) {
      throw new Error(`No value ${name} found`);
    }
    value = create2();
    box[name] = value;
  }
  return value;
}

// ../tslib/timers/ITimer.ts
var BaseTimerTickEvent = class {
  constructor() {
    this.t = 0;
    this.dt = 0;
    this.sdt = 0;
    this.fps = 0;
  }
  set(t2, dt) {
    this.t = t2;
    this.dt = dt;
    this.sdt = lerp2(this.sdt, dt, 0.01);
    if (dt > 0) {
      this.fps = 1e3 / dt;
    }
  }
};
var TimerTickEvent = class extends BaseTimerTickEvent {
  constructor() {
    super();
    Object.seal(this);
  }
};

// ../tslib/timers/BaseTimer.ts
var BaseTimer = class {
  constructor(targetFrameRate) {
    this.timer = null;
    this.lt = -1;
    this.tickHandlers = new Array();
    this._targetFPS = null;
    this.targetFPS = targetFrameRate;
    const tickEvt = new TimerTickEvent();
    let dt = 0;
    this.onTick = (t2) => {
      if (this.lt >= 0) {
        dt = t2 - this.lt;
        tickEvt.set(t2, dt);
        this.tick(tickEvt);
      }
      this.lt = t2;
    };
  }
  get targetFPS() {
    return this._targetFPS;
  }
  set targetFPS(v) {
    this._targetFPS = v;
  }
  addTickHandler(onTick) {
    this.tickHandlers.push(onTick);
  }
  removeTickHandler(onTick) {
    arrayRemove(this.tickHandlers, onTick);
  }
  tick(evt) {
    for (const handler of this.tickHandlers) {
      handler(evt);
    }
  }
  restart() {
    this.stop();
    this.start();
  }
  get isRunning() {
    return this.timer != null;
  }
  stop() {
    this.timer = null;
    this.lt = -1;
  }
  get targetFrameTime() {
    return 1e3 / this.targetFPS;
  }
};

// ../tslib/timers/SetIntervalTimer.ts
var SetIntervalTimer = class extends BaseTimer {
  constructor(targetFrameRate) {
    super(targetFrameRate);
  }
  start() {
    this.timer = setInterval(() => this.onTick(performance.now()), this.targetFrameTime);
  }
  stop() {
    if (this.timer) {
      clearInterval(this.timer);
      super.stop();
    }
  }
  get targetFPS() {
    return super.targetFPS;
  }
  set targetFPS(fps) {
    super.targetFPS = fps;
    if (this.isRunning) {
      this.restart();
    }
  }
};

// ../tslib/collections/mapInvert.ts
function mapInvert(map) {
  const mapOut = /* @__PURE__ */ new Map();
  for (const [key, value] of map) {
    mapOut.set(value, key);
  }
  return mapOut;
}

// ../tslib/units/fileSize.ts
var base2Labels = /* @__PURE__ */ new Map([
  [1, "KiB"],
  [2, "MiB"],
  [3, "GiB"],
  [4, "TiB"]
]);
var base10Labels = /* @__PURE__ */ new Map([
  [1, "KB"],
  [2, "MB"],
  [3, "GB"],
  [4, "TB"]
]);
var base2Sizes = mapInvert(base2Labels);
var base10Sizes = mapInvert(base10Labels);

// ../tslib/units/length.ts
var MICROMETERS_PER_MILLIMETER = 1e3;
var MILLIMETERS_PER_CENTIMETER = 10;
var CENTIMETERS_PER_INCH = 2.54;
var CENTIMETERS_PER_METER = 100;
var INCHES_PER_HAND = 4;
var HANDS_PER_FOOT = 3;
var FEET_PER_YARD = 3;
var FEET_PER_ROD = 16.5;
var METERS_PER_KILOMETER = 1e3;
var RODS_PER_FURLONG = 40;
var FURLONGS_PER_MILE = 8;
var MICROMETERS_PER_CENTIMETER = MICROMETERS_PER_MILLIMETER * MILLIMETERS_PER_CENTIMETER;
var MICROMETERS_PER_INCH = MICROMETERS_PER_CENTIMETER * CENTIMETERS_PER_INCH;
var MICROMETERS_PER_HAND = MICROMETERS_PER_INCH * INCHES_PER_HAND;
var MICROMETERS_PER_FOOT = MICROMETERS_PER_HAND * HANDS_PER_FOOT;
var MICROMETERS_PER_YARD = MICROMETERS_PER_FOOT * FEET_PER_YARD;
var MICROMETERS_PER_METER = MICROMETERS_PER_CENTIMETER * CENTIMETERS_PER_METER;
var MICROMETERS_PER_ROD = MICROMETERS_PER_FOOT * FEET_PER_ROD;
var MICROMETERS_PER_FURLONG = MICROMETERS_PER_ROD * RODS_PER_FURLONG;
var MICROMETERS_PER_KILOMETER = MICROMETERS_PER_METER * METERS_PER_KILOMETER;
var MICROMETERS_PER_MILE = MICROMETERS_PER_FURLONG * FURLONGS_PER_MILE;
var MILLIMETERS_PER_INCH = MILLIMETERS_PER_CENTIMETER * CENTIMETERS_PER_INCH;
var MILLIMETERS_PER_HAND = MILLIMETERS_PER_INCH * INCHES_PER_HAND;
var MILLIMETERS_PER_FOOT = MILLIMETERS_PER_HAND * HANDS_PER_FOOT;
var MILLIMETERS_PER_YARD = MILLIMETERS_PER_FOOT * FEET_PER_YARD;
var MILLIMETERS_PER_METER = MILLIMETERS_PER_CENTIMETER * CENTIMETERS_PER_METER;
var MILLIMETERS_PER_ROD = MILLIMETERS_PER_FOOT * FEET_PER_ROD;
var MILLIMETERS_PER_FURLONG = MILLIMETERS_PER_ROD * RODS_PER_FURLONG;
var MILLIMETERS_PER_KILOMETER = MILLIMETERS_PER_METER * METERS_PER_KILOMETER;
var MILLIMETERS_PER_MILE = MILLIMETERS_PER_FURLONG * FURLONGS_PER_MILE;
var CENTIMETERS_PER_HAND = CENTIMETERS_PER_INCH * INCHES_PER_HAND;
var CENTIMETERS_PER_FOOT = CENTIMETERS_PER_HAND * HANDS_PER_FOOT;
var CENTIMETERS_PER_YARD = CENTIMETERS_PER_FOOT * FEET_PER_YARD;
var CENTIMETERS_PER_ROD = CENTIMETERS_PER_FOOT * FEET_PER_ROD;
var CENTIMETERS_PER_FURLONG = CENTIMETERS_PER_ROD * RODS_PER_FURLONG;
var CENTIMETERS_PER_KILOMETER = CENTIMETERS_PER_METER * METERS_PER_KILOMETER;
var CENTIMETERS_PER_MILE = CENTIMETERS_PER_FURLONG * FURLONGS_PER_MILE;
var INCHES_PER_FOOT = INCHES_PER_HAND * HANDS_PER_FOOT;
var INCHES_PER_YARD = INCHES_PER_FOOT * FEET_PER_YARD;
var INCHES_PER_METER = CENTIMETERS_PER_METER / CENTIMETERS_PER_INCH;
var INCHES_PER_ROD = INCHES_PER_FOOT * FEET_PER_ROD;
var INCHES_PER_FURLONG = INCHES_PER_ROD * RODS_PER_FURLONG;
var INCHES_PER_KILOMETER = INCHES_PER_METER * METERS_PER_KILOMETER;
var INCHES_PER_MILE = INCHES_PER_FURLONG * FURLONGS_PER_MILE;
var HANDS_PER_YARD = HANDS_PER_FOOT * FEET_PER_YARD;
var HANDS_PER_METER = CENTIMETERS_PER_METER / CENTIMETERS_PER_HAND;
var HANDS_PER_ROD = HANDS_PER_FOOT * FEET_PER_ROD;
var HANDS_PER_FURLONG = HANDS_PER_ROD * RODS_PER_FURLONG;
var HANDS_PER_KILOMETER = HANDS_PER_METER * METERS_PER_KILOMETER;
var HANDS_PER_MILE = HANDS_PER_FURLONG * FURLONGS_PER_MILE;
var FEET_PER_METER = INCHES_PER_METER / INCHES_PER_FOOT;
var FEET_PER_FURLONG = FEET_PER_ROD * RODS_PER_FURLONG;
var FEET_PER_KILOMETER = FEET_PER_METER * METERS_PER_KILOMETER;
var FEET_PER_MILE = FEET_PER_FURLONG * FURLONGS_PER_MILE;
var YARDS_PER_METER = INCHES_PER_METER / INCHES_PER_YARD;
var YARDS_PER_ROD = FEET_PER_ROD / FEET_PER_YARD;
var YARDS_PER_FURLONG = YARDS_PER_ROD * RODS_PER_FURLONG;
var YARDS_PER_KILOMETER = YARDS_PER_METER * METERS_PER_KILOMETER;
var YARDS_PER_MILE = YARDS_PER_FURLONG * FURLONGS_PER_MILE;
var METERS_PER_ROD = FEET_PER_ROD / FEET_PER_METER;
var METERS_PER_FURLONG = METERS_PER_ROD * RODS_PER_FURLONG;
var METERS_PER_MILE = METERS_PER_FURLONG * FURLONGS_PER_MILE;
var RODS_PER_KILOMETER = METERS_PER_KILOMETER / METERS_PER_ROD;
var RODS_PER_MILE = RODS_PER_FURLONG * FURLONGS_PER_MILE;
var FURLONGS_PER_KILOMETER = METERS_PER_KILOMETER / METERS_PER_FURLONG;
var KILOMETERS_PER_MILE = FURLONGS_PER_MILE / FURLONGS_PER_KILOMETER;

// ../tslib/URLBuilder.ts
function parsePort(portString) {
  if (isDefined(portString) && portString.length > 0) {
    return parseFloat(portString);
  }
  return null;
}
var URLBuilder = class {
  constructor(url, base) {
    this._url = null;
    this._base = void 0;
    this._protocol = null;
    this._host = null;
    this._hostName = null;
    this._userName = null;
    this._password = null;
    this._port = null;
    this._pathName = null;
    this._hash = null;
    this._query = /* @__PURE__ */ new Map();
    if (url !== void 0) {
      this._url = new URL(url, base);
      this.rehydrate();
    }
  }
  rehydrate() {
    if (isDefined(this._protocol) && this._protocol !== this._url.protocol) {
      this._url.protocol = this._protocol;
    }
    if (isDefined(this._host) && this._host !== this._url.host) {
      this._url.host = this._host;
    }
    if (isDefined(this._hostName) && this._hostName !== this._url.hostname) {
      this._url.hostname = this._hostName;
    }
    if (isDefined(this._userName) && this._userName !== this._url.username) {
      this._url.username = this._userName;
    }
    if (isDefined(this._password) && this._password !== this._url.password) {
      this._url.password = this._password;
    }
    if (isDefined(this._port) && this._port.toFixed(0) !== this._url.port) {
      this._url.port = this._port.toFixed(0);
    }
    if (isDefined(this._pathName) && this._pathName !== this._url.pathname) {
      this._url.pathname = this._pathName;
    }
    if (isDefined(this._hash) && this._hash !== this._url.hash) {
      this._url.hash = this._hash;
    }
    for (const [k, v] of this._query) {
      this._url.searchParams.set(k, v);
    }
    this._protocol = this._url.protocol;
    this._host = this._url.host;
    this._hostName = this._url.hostname;
    this._userName = this._url.username;
    this._password = this._url.password;
    this._port = parsePort(this._url.port);
    this._pathName = this._url.pathname;
    this._hash = this._url.hash;
    this._url.searchParams.forEach((v, k) => this._query.set(k, v));
  }
  refresh() {
    if (this._url === null) {
      if (isDefined(this._protocol) && (isDefined(this._host) || isDefined(this._hostName))) {
        if (isDefined(this._host)) {
          this._url = new URL(`${this._protocol}//${this._host}`, this._base);
          this._port = parsePort(this._url.port);
          this.rehydrate();
          return false;
        } else if (isDefined(this._hostName)) {
          this._url = new URL(`${this._protocol}//${this._hostName}`, this._base);
          this.rehydrate();
          return false;
        }
      } else if (isDefined(this._pathName) && isDefined(this._base)) {
        this._url = new URL(this._pathName, this._base);
        this.rehydrate();
        return false;
      }
    }
    return isDefined(this._url);
  }
  base(base) {
    if (this._url !== null) {
      throw new Error("Cannot redefine base after defining the protocol and domain");
    }
    this._base = base;
    this.refresh();
    return this;
  }
  protocol(protocol) {
    this._protocol = protocol;
    if (this.refresh()) {
      this._url.protocol = protocol;
    }
    return this;
  }
  host(host) {
    this._host = host;
    if (this.refresh()) {
      this._url.host = host;
      this._hostName = this._url.hostname;
      this._port = parsePort(this._url.port);
    }
    return this;
  }
  hostName(hostName) {
    this._hostName = hostName;
    if (this.refresh()) {
      this._url.hostname = hostName;
      this._host = `${this._url.hostname}:${this._url.port}`;
    }
    return this;
  }
  port(port) {
    this._port = port;
    if (this.refresh()) {
      this._url.port = port.toFixed(0);
      this._host = `${this._url.hostname}:${this._url.port}`;
    }
    return this;
  }
  userName(userName) {
    this._userName = userName;
    if (this.refresh()) {
      this._url.username = userName;
    }
    return this;
  }
  password(password) {
    this._password = password;
    if (this.refresh()) {
      this._url.password = password;
    }
    return this;
  }
  path(path) {
    this._pathName = path;
    if (this.refresh()) {
      this._url.pathname = path;
    }
    return this;
  }
  pathPop(pattern) {
    pattern = pattern || /\/[^\/]+\/?$/;
    return this.path(this._pathName.replace(pattern, ""));
  }
  pathPush(part) {
    let path = this._pathName;
    if (!path.endsWith("/")) {
      path += "/";
    }
    path += part;
    return this.path(path);
  }
  query(name, value) {
    this._query.set(name, value);
    if (this.refresh()) {
      this._url.searchParams.set(name, value);
    }
    return this;
  }
  hash(hash) {
    this._hash = hash;
    if (this.refresh()) {
      this._url.hash = hash;
    }
    return this;
  }
  toURL() {
    return this._url;
  }
  toString() {
    return this._url.href;
  }
  [Symbol.toStringTag]() {
    return this.toString();
  }
};

// ../tslib/using.ts
function interfaceSigCheck(obj2, ...funcNames) {
  if (!isObject(obj2)) {
    return false;
  }
  obj2 = obj2;
  for (const funcName of funcNames) {
    if (!(funcName in obj2)) {
      return false;
    }
    const func = obj2[funcName];
    if (!isFunction(func)) {
      return false;
    }
  }
  return true;
}
function isDisposable(obj2) {
  return interfaceSigCheck(obj2, "dispose");
}
function isDestroyable(obj2) {
  return interfaceSigCheck(obj2, "destroy");
}
function isClosable(obj2) {
  return interfaceSigCheck(obj2, "close");
}
function dispose(val) {
  if (isDisposable(val)) {
    val.dispose();
  }
  if (isClosable(val)) {
    val.close();
  }
  if (isDestroyable(val)) {
    val.destroy();
  }
}

// ../audio/nodes.ts
var hasAudioContext = "AudioContext" in globalThis;
var hasAudioListener = hasAudioContext && "AudioListener" in globalThis;
var hasOldAudioListener = hasAudioListener && "setPosition" in AudioListener.prototype;
var hasNewAudioListener = hasAudioListener && "positionX" in AudioListener.prototype;
var canCaptureStream = isFunction(HTMLMediaElement.prototype.captureStream) || isFunction(HTMLMediaElement.prototype.mozCaptureStream);
function isWrappedAudioNode(value) {
  return isDefined(value) && value.node instanceof AudioNode;
}
function isErsatzAudioNode(value) {
  return isDefined(value) && value.input instanceof AudioNode && value.output instanceof AudioNode;
}
var connections = singleton("Juniper:Audio:connections", () => /* @__PURE__ */ new Map());
var names = singleton("Juniper:Audio:names", () => /* @__PURE__ */ new Map());
function resolveOutput(node) {
  if (isErsatzAudioNode(node)) {
    return node.output;
  } else if (isWrappedAudioNode(node)) {
    return node.node;
  }
  return node;
}
function resolveInput(node) {
  if (isNullOrUndefined(node)) {
    return void 0;
  }
  let n2 = null;
  if (isArray(node)) {
    if (node.length === 2) {
      n2 = node[1];
    } else {
      n2 = node[2];
    }
  } else {
    n2 = node;
  }
  let n22 = null;
  if (isErsatzAudioNode(n2)) {
    n22 = n2.input;
  } else if (isWrappedAudioNode(n2)) {
    n22 = n2.node;
  } else {
    n22 = n2;
  }
  return n22;
}
function resolveArray(node) {
  if (!isArray(node)) {
    return [];
  } else if (node.length === 2) {
    return [node[0]];
  } else {
    return [node[0], node[1]];
  }
}
function isAudioNode(a) {
  return isDefined(a) && isDefined(a.context);
}
function nameVertex(name, v) {
  names.set(v, name);
}
function removeVertex(v) {
  names.delete(v);
  if (isAudioNode(v)) {
    disconnect(v);
  }
}
function connect(left, right) {
  const a = resolveOutput(left);
  const b = resolveInput(right);
  const c = resolveArray(right);
  if (isNullOrUndefined(b)) {
    throw new Error("Must have a target to connect to");
  } else if (b instanceof AudioParam) {
    a.connect(b, c[0]);
  } else if (b instanceof AudioNode) {
    a.connect(b, c[0], c[1]);
  } else {
    assertNever(b);
  }
  if (!connections.has(a)) {
    connections.set(a, /* @__PURE__ */ new Set());
  }
  const g = connections.get(a);
  if (g.has(b)) {
    return false;
  }
  g.add(b);
  return true;
}
function disconnect(left, right) {
  const a = resolveOutput(left);
  const b = resolveInput(right);
  const c = resolveArray(right);
  if (isNullOrUndefined(b)) {
    a.disconnect();
  } else if (b instanceof AudioParam) {
    a.disconnect(b, c[0]);
  } else if (b instanceof AudioNode) {
    a.disconnect(b, c[0], c[1]);
  } else {
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
  } else if (g.has(b)) {
    removed = true;
    g.delete(b);
  }
  if (g.size === 0) {
    connections.delete(a);
  }
  return removed;
}
function getAudioGraph() {
  const nodes = /* @__PURE__ */ new Map();
  function maybeAdd(node) {
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
globalThis.getAudioGraph = getAudioGraph;
function initAudio(name, left, ...rest) {
  nameVertex(name, left);
  for (const right of rest) {
    if (isDefined(right)) {
      connect(left, right);
    }
  }
  return left;
}
function Analyser(name, audioCtx, options, ...rest) {
  return initAudio(name, new AnalyserNode(audioCtx, options), ...rest);
}
function MediaStreamSource(name, audioCtx, mediaStream, ...rest) {
  return initAudio(name, audioCtx.createMediaStreamSource(mediaStream), ...rest);
}

// ../../node_modules/@microsoft/signalr/dist/esm/Errors.js
var HttpError = class extends Error {
  constructor(errorMessage, statusCode) {
    const trueProto = new.target.prototype;
    super(`${errorMessage}: Status code '${statusCode}'`);
    this.statusCode = statusCode;
    this.__proto__ = trueProto;
  }
};
var TimeoutError = class extends Error {
  constructor(errorMessage = "A timeout occurred.") {
    const trueProto = new.target.prototype;
    super(errorMessage);
    this.__proto__ = trueProto;
  }
};
var AbortError = class extends Error {
  constructor(errorMessage = "An abort occurred.") {
    const trueProto = new.target.prototype;
    super(errorMessage);
    this.__proto__ = trueProto;
  }
};
var UnsupportedTransportError = class extends Error {
  constructor(message, transport) {
    const trueProto = new.target.prototype;
    super(message);
    this.transport = transport;
    this.errorType = "UnsupportedTransportError";
    this.__proto__ = trueProto;
  }
};
var DisabledTransportError = class extends Error {
  constructor(message, transport) {
    const trueProto = new.target.prototype;
    super(message);
    this.transport = transport;
    this.errorType = "DisabledTransportError";
    this.__proto__ = trueProto;
  }
};
var FailedToStartTransportError = class extends Error {
  constructor(message, transport) {
    const trueProto = new.target.prototype;
    super(message);
    this.transport = transport;
    this.errorType = "FailedToStartTransportError";
    this.__proto__ = trueProto;
  }
};
var FailedToNegotiateWithServerError = class extends Error {
  constructor(message) {
    const trueProto = new.target.prototype;
    super(message);
    this.errorType = "FailedToNegotiateWithServerError";
    this.__proto__ = trueProto;
  }
};
var AggregateErrors = class extends Error {
  constructor(message, innerErrors) {
    const trueProto = new.target.prototype;
    super(message);
    this.innerErrors = innerErrors;
    this.__proto__ = trueProto;
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/HttpClient.js
var HttpResponse = class {
  constructor(statusCode, statusText, content) {
    this.statusCode = statusCode;
    this.statusText = statusText;
    this.content = content;
  }
};
var HttpClient = class {
  get(url, options) {
    return this.send({
      ...options,
      method: "GET",
      url
    });
  }
  post(url, options) {
    return this.send({
      ...options,
      method: "POST",
      url
    });
  }
  delete(url, options) {
    return this.send({
      ...options,
      method: "DELETE",
      url
    });
  }
  getCookieString(url) {
    return "";
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/ILogger.js
var LogLevel;
(function(LogLevel2) {
  LogLevel2[LogLevel2["Trace"] = 0] = "Trace";
  LogLevel2[LogLevel2["Debug"] = 1] = "Debug";
  LogLevel2[LogLevel2["Information"] = 2] = "Information";
  LogLevel2[LogLevel2["Warning"] = 3] = "Warning";
  LogLevel2[LogLevel2["Error"] = 4] = "Error";
  LogLevel2[LogLevel2["Critical"] = 5] = "Critical";
  LogLevel2[LogLevel2["None"] = 6] = "None";
})(LogLevel || (LogLevel = {}));

// ../../node_modules/@microsoft/signalr/dist/esm/Loggers.js
var NullLogger = class {
  constructor() {
  }
  log(_logLevel, _message) {
  }
};
NullLogger.instance = new NullLogger();

// ../../node_modules/@microsoft/signalr/dist/esm/Utils.js
var VERSION = "6.0.6";
var Arg = class {
  static isRequired(val, name) {
    if (val === null || val === void 0) {
      throw new Error(`The '${name}' argument is required.`);
    }
  }
  static isNotEmpty(val, name) {
    if (!val || val.match(/^\s*$/)) {
      throw new Error(`The '${name}' argument should not be empty.`);
    }
  }
  static isIn(val, values, name) {
    if (!(val in values)) {
      throw new Error(`Unknown ${name} value: ${val}.`);
    }
  }
};
var Platform = class {
  static get isBrowser() {
    return typeof window === "object" && typeof window.document === "object";
  }
  static get isWebWorker() {
    return typeof self === "object" && "importScripts" in self;
  }
  static get isReactNative() {
    return typeof window === "object" && typeof window.document === "undefined";
  }
  static get isNode() {
    return !this.isBrowser && !this.isWebWorker && !this.isReactNative;
  }
};
function getDataDetail(data, includeContent) {
  let detail = "";
  if (isArrayBuffer2(data)) {
    detail = `Binary data of length ${data.byteLength}`;
    if (includeContent) {
      detail += `. Content: '${formatArrayBuffer(data)}'`;
    }
  } else if (typeof data === "string") {
    detail = `String data of length ${data.length}`;
    if (includeContent) {
      detail += `. Content: '${data}'`;
    }
  }
  return detail;
}
function formatArrayBuffer(data) {
  const view = new Uint8Array(data);
  let str2 = "";
  view.forEach((num) => {
    const pad = num < 16 ? "0" : "";
    str2 += `0x${pad}${num.toString(16)} `;
  });
  return str2.substr(0, str2.length - 1);
}
function isArrayBuffer2(val) {
  return val && typeof ArrayBuffer !== "undefined" && (val instanceof ArrayBuffer || val.constructor && val.constructor.name === "ArrayBuffer");
}
async function sendMessage(logger, transportName, httpClient, url, accessTokenFactory, content, options) {
  let headers = {};
  if (accessTokenFactory) {
    const token = await accessTokenFactory();
    if (token) {
      headers = {
        ["Authorization"]: `Bearer ${token}`
      };
    }
  }
  const [name, value] = getUserAgentHeader();
  headers[name] = value;
  logger.log(LogLevel.Trace, `(${transportName} transport) sending data. ${getDataDetail(content, options.logMessageContent)}.`);
  const responseType = isArrayBuffer2(content) ? "arraybuffer" : "text";
  const response = await httpClient.post(url, {
    content,
    headers: { ...headers, ...options.headers },
    responseType,
    timeout: options.timeout,
    withCredentials: options.withCredentials
  });
  logger.log(LogLevel.Trace, `(${transportName} transport) request complete. Response status: ${response.statusCode}.`);
}
function createLogger(logger) {
  if (logger === void 0) {
    return new ConsoleLogger(LogLevel.Information);
  }
  if (logger === null) {
    return NullLogger.instance;
  }
  if (logger.log !== void 0) {
    return logger;
  }
  return new ConsoleLogger(logger);
}
var SubjectSubscription = class {
  constructor(subject, observer) {
    this._subject = subject;
    this._observer = observer;
  }
  dispose() {
    const index = this._subject.observers.indexOf(this._observer);
    if (index > -1) {
      this._subject.observers.splice(index, 1);
    }
    if (this._subject.observers.length === 0 && this._subject.cancelCallback) {
      this._subject.cancelCallback().catch((_) => {
      });
    }
  }
};
var ConsoleLogger = class {
  constructor(minimumLogLevel) {
    this._minLevel = minimumLogLevel;
    this.out = console;
  }
  log(logLevel, message) {
    if (logLevel >= this._minLevel) {
      const msg = `[${new Date().toISOString()}] ${LogLevel[logLevel]}: ${message}`;
      switch (logLevel) {
        case LogLevel.Critical:
        case LogLevel.Error:
          this.out.error(msg);
          break;
        case LogLevel.Warning:
          this.out.warn(msg);
          break;
        case LogLevel.Information:
          this.out.info(msg);
          break;
        default:
          this.out.log(msg);
          break;
      }
    }
  }
};
function getUserAgentHeader() {
  let userAgentHeaderName = "X-SignalR-User-Agent";
  if (Platform.isNode) {
    userAgentHeaderName = "User-Agent";
  }
  return [userAgentHeaderName, constructUserAgent(VERSION, getOsName(), getRuntime(), getRuntimeVersion())];
}
function constructUserAgent(version, os, runtime, runtimeVersion) {
  let userAgent = "Microsoft SignalR/";
  const majorAndMinor = version.split(".");
  userAgent += `${majorAndMinor[0]}.${majorAndMinor[1]}`;
  userAgent += ` (${version}; `;
  if (os && os !== "") {
    userAgent += `${os}; `;
  } else {
    userAgent += "Unknown OS; ";
  }
  userAgent += `${runtime}`;
  if (runtimeVersion) {
    userAgent += `; ${runtimeVersion}`;
  } else {
    userAgent += "; Unknown Runtime Version";
  }
  userAgent += ")";
  return userAgent;
}
function getOsName() {
  if (Platform.isNode) {
    switch (process.platform) {
      case "win32":
        return "Windows NT";
      case "darwin":
        return "macOS";
      case "linux":
        return "Linux";
      default:
        return process.platform;
    }
  } else {
    return "";
  }
}
function getRuntimeVersion() {
  if (Platform.isNode) {
    return process.versions.node;
  }
  return void 0;
}
function getRuntime() {
  if (Platform.isNode) {
    return "NodeJS";
  } else {
    return "Browser";
  }
}
function getErrorString(e2) {
  if (e2.stack) {
    return e2.stack;
  } else if (e2.message) {
    return e2.message;
  }
  return `${e2}`;
}
function getGlobalThis() {
  if (typeof globalThis !== "undefined") {
    return globalThis;
  }
  if (typeof self !== "undefined") {
    return self;
  }
  if (typeof window !== "undefined") {
    return window;
  }
  if (typeof global !== "undefined") {
    return global;
  }
  throw new Error("could not find global");
}

// ../../node_modules/@microsoft/signalr/dist/esm/FetchHttpClient.js
var FetchHttpClient = class extends HttpClient {
  constructor(logger) {
    super();
    this._logger = logger;
    if (typeof fetch === "undefined") {
      const requireFunc = typeof __webpack_require__ === "function" ? __non_webpack_require__ : __require;
      this._jar = new (requireFunc("tough-cookie")).CookieJar();
      this._fetchType = requireFunc("node-fetch");
      this._fetchType = requireFunc("fetch-cookie")(this._fetchType, this._jar);
    } else {
      this._fetchType = fetch.bind(getGlobalThis());
    }
    if (typeof AbortController === "undefined") {
      const requireFunc = typeof __webpack_require__ === "function" ? __non_webpack_require__ : __require;
      this._abortControllerType = requireFunc("abort-controller");
    } else {
      this._abortControllerType = AbortController;
    }
  }
  async send(request) {
    if (request.abortSignal && request.abortSignal.aborted) {
      throw new AbortError();
    }
    if (!request.method) {
      throw new Error("No method defined.");
    }
    if (!request.url) {
      throw new Error("No url defined.");
    }
    const abortController = new this._abortControllerType();
    let error;
    if (request.abortSignal) {
      request.abortSignal.onabort = () => {
        abortController.abort();
        error = new AbortError();
      };
    }
    let timeoutId = null;
    if (request.timeout) {
      const msTimeout = request.timeout;
      timeoutId = setTimeout(() => {
        abortController.abort();
        this._logger.log(LogLevel.Warning, `Timeout from HTTP request.`);
        error = new TimeoutError();
      }, msTimeout);
    }
    let response;
    try {
      response = await this._fetchType(request.url, {
        body: request.content,
        cache: "no-cache",
        credentials: request.withCredentials === true ? "include" : "same-origin",
        headers: {
          "Content-Type": "text/plain;charset=UTF-8",
          "X-Requested-With": "XMLHttpRequest",
          ...request.headers
        },
        method: request.method,
        mode: "cors",
        redirect: "follow",
        signal: abortController.signal
      });
    } catch (e2) {
      if (error) {
        throw error;
      }
      this._logger.log(LogLevel.Warning, `Error from HTTP request. ${e2}.`);
      throw e2;
    } finally {
      if (timeoutId) {
        clearTimeout(timeoutId);
      }
      if (request.abortSignal) {
        request.abortSignal.onabort = null;
      }
    }
    if (!response.ok) {
      const errorMessage = await deserializeContent(response, "text");
      throw new HttpError(errorMessage || response.statusText, response.status);
    }
    const content = deserializeContent(response, request.responseType);
    const payload = await content;
    return new HttpResponse(response.status, response.statusText, payload);
  }
  getCookieString(url) {
    let cookies = "";
    if (Platform.isNode && this._jar) {
      this._jar.getCookies(url, (e2, c) => cookies = c.join("; "));
    }
    return cookies;
  }
};
function deserializeContent(response, responseType) {
  let content;
  switch (responseType) {
    case "arraybuffer":
      content = response.arrayBuffer();
      break;
    case "text":
      content = response.text();
      break;
    case "blob":
    case "document":
    case "json":
      throw new Error(`${responseType} is not supported.`);
    default:
      content = response.text();
      break;
  }
  return content;
}

// ../../node_modules/@microsoft/signalr/dist/esm/XhrHttpClient.js
var XhrHttpClient = class extends HttpClient {
  constructor(logger) {
    super();
    this._logger = logger;
  }
  send(request) {
    if (request.abortSignal && request.abortSignal.aborted) {
      return Promise.reject(new AbortError());
    }
    if (!request.method) {
      return Promise.reject(new Error("No method defined."));
    }
    if (!request.url) {
      return Promise.reject(new Error("No url defined."));
    }
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open(request.method, request.url, true);
      xhr.withCredentials = request.withCredentials === void 0 ? true : request.withCredentials;
      xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      xhr.setRequestHeader("Content-Type", "text/plain;charset=UTF-8");
      const headers = request.headers;
      if (headers) {
        Object.keys(headers).forEach((header) => {
          xhr.setRequestHeader(header, headers[header]);
        });
      }
      if (request.responseType) {
        xhr.responseType = request.responseType;
      }
      if (request.abortSignal) {
        request.abortSignal.onabort = () => {
          xhr.abort();
          reject(new AbortError());
        };
      }
      if (request.timeout) {
        xhr.timeout = request.timeout;
      }
      xhr.onload = () => {
        if (request.abortSignal) {
          request.abortSignal.onabort = null;
        }
        if (xhr.status >= 200 && xhr.status < 300) {
          resolve(new HttpResponse(xhr.status, xhr.statusText, xhr.response || xhr.responseText));
        } else {
          reject(new HttpError(xhr.response || xhr.responseText || xhr.statusText, xhr.status));
        }
      };
      xhr.onerror = () => {
        this._logger.log(LogLevel.Warning, `Error from HTTP request. ${xhr.status}: ${xhr.statusText}.`);
        reject(new HttpError(xhr.statusText, xhr.status));
      };
      xhr.ontimeout = () => {
        this._logger.log(LogLevel.Warning, `Timeout from HTTP request.`);
        reject(new TimeoutError());
      };
      xhr.send(request.content || "");
    });
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/DefaultHttpClient.js
var DefaultHttpClient = class extends HttpClient {
  constructor(logger) {
    super();
    if (typeof fetch !== "undefined" || Platform.isNode) {
      this._httpClient = new FetchHttpClient(logger);
    } else if (typeof XMLHttpRequest !== "undefined") {
      this._httpClient = new XhrHttpClient(logger);
    } else {
      throw new Error("No usable HttpClient found.");
    }
  }
  send(request) {
    if (request.abortSignal && request.abortSignal.aborted) {
      return Promise.reject(new AbortError());
    }
    if (!request.method) {
      return Promise.reject(new Error("No method defined."));
    }
    if (!request.url) {
      return Promise.reject(new Error("No url defined."));
    }
    return this._httpClient.send(request);
  }
  getCookieString(url) {
    return this._httpClient.getCookieString(url);
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/TextMessageFormat.js
var TextMessageFormat = class {
  static write(output) {
    return `${output}${TextMessageFormat.RecordSeparator}`;
  }
  static parse(input) {
    if (input[input.length - 1] !== TextMessageFormat.RecordSeparator) {
      throw new Error("Message is incomplete.");
    }
    const messages = input.split(TextMessageFormat.RecordSeparator);
    messages.pop();
    return messages;
  }
};
TextMessageFormat.RecordSeparatorCode = 30;
TextMessageFormat.RecordSeparator = String.fromCharCode(TextMessageFormat.RecordSeparatorCode);

// ../../node_modules/@microsoft/signalr/dist/esm/HandshakeProtocol.js
var HandshakeProtocol = class {
  writeHandshakeRequest(handshakeRequest) {
    return TextMessageFormat.write(JSON.stringify(handshakeRequest));
  }
  parseHandshakeResponse(data) {
    let messageData;
    let remainingData;
    if (isArrayBuffer2(data)) {
      const binaryData = new Uint8Array(data);
      const separatorIndex = binaryData.indexOf(TextMessageFormat.RecordSeparatorCode);
      if (separatorIndex === -1) {
        throw new Error("Message is incomplete.");
      }
      const responseLength = separatorIndex + 1;
      messageData = String.fromCharCode.apply(null, Array.prototype.slice.call(binaryData.slice(0, responseLength)));
      remainingData = binaryData.byteLength > responseLength ? binaryData.slice(responseLength).buffer : null;
    } else {
      const textData = data;
      const separatorIndex = textData.indexOf(TextMessageFormat.RecordSeparator);
      if (separatorIndex === -1) {
        throw new Error("Message is incomplete.");
      }
      const responseLength = separatorIndex + 1;
      messageData = textData.substring(0, responseLength);
      remainingData = textData.length > responseLength ? textData.substring(responseLength) : null;
    }
    const messages = TextMessageFormat.parse(messageData);
    const response = JSON.parse(messages[0]);
    if (response.type) {
      throw new Error("Expected a handshake response from the server.");
    }
    const responseMessage = response;
    return [remainingData, responseMessage];
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/IHubProtocol.js
var MessageType;
(function(MessageType2) {
  MessageType2[MessageType2["Invocation"] = 1] = "Invocation";
  MessageType2[MessageType2["StreamItem"] = 2] = "StreamItem";
  MessageType2[MessageType2["Completion"] = 3] = "Completion";
  MessageType2[MessageType2["StreamInvocation"] = 4] = "StreamInvocation";
  MessageType2[MessageType2["CancelInvocation"] = 5] = "CancelInvocation";
  MessageType2[MessageType2["Ping"] = 6] = "Ping";
  MessageType2[MessageType2["Close"] = 7] = "Close";
})(MessageType || (MessageType = {}));

// ../../node_modules/@microsoft/signalr/dist/esm/Subject.js
var Subject = class {
  constructor() {
    this.observers = [];
  }
  next(item) {
    for (const observer of this.observers) {
      observer.next(item);
    }
  }
  error(err) {
    for (const observer of this.observers) {
      if (observer.error) {
        observer.error(err);
      }
    }
  }
  complete() {
    for (const observer of this.observers) {
      if (observer.complete) {
        observer.complete();
      }
    }
  }
  subscribe(observer) {
    this.observers.push(observer);
    return new SubjectSubscription(this, observer);
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/HubConnection.js
var DEFAULT_TIMEOUT_IN_MS = 30 * 1e3;
var DEFAULT_PING_INTERVAL_IN_MS = 15 * 1e3;
var HubConnectionState;
(function(HubConnectionState2) {
  HubConnectionState2["Disconnected"] = "Disconnected";
  HubConnectionState2["Connecting"] = "Connecting";
  HubConnectionState2["Connected"] = "Connected";
  HubConnectionState2["Disconnecting"] = "Disconnecting";
  HubConnectionState2["Reconnecting"] = "Reconnecting";
})(HubConnectionState || (HubConnectionState = {}));
var HubConnection = class {
  constructor(connection, logger, protocol, reconnectPolicy) {
    this._nextKeepAlive = 0;
    this._freezeEventListener = () => {
      this._logger.log(LogLevel.Warning, "The page is being frozen, this will likely lead to the connection being closed and messages being lost. For more information see the docs at https://docs.microsoft.com/aspnet/core/signalr/javascript-client#bsleep");
    };
    Arg.isRequired(connection, "connection");
    Arg.isRequired(logger, "logger");
    Arg.isRequired(protocol, "protocol");
    this.serverTimeoutInMilliseconds = DEFAULT_TIMEOUT_IN_MS;
    this.keepAliveIntervalInMilliseconds = DEFAULT_PING_INTERVAL_IN_MS;
    this._logger = logger;
    this._protocol = protocol;
    this.connection = connection;
    this._reconnectPolicy = reconnectPolicy;
    this._handshakeProtocol = new HandshakeProtocol();
    this.connection.onreceive = (data) => this._processIncomingData(data);
    this.connection.onclose = (error) => this._connectionClosed(error);
    this._callbacks = {};
    this._methods = {};
    this._closedCallbacks = [];
    this._reconnectingCallbacks = [];
    this._reconnectedCallbacks = [];
    this._invocationId = 0;
    this._receivedHandshakeResponse = false;
    this._connectionState = HubConnectionState.Disconnected;
    this._connectionStarted = false;
    this._cachedPingMessage = this._protocol.writeMessage({ type: MessageType.Ping });
  }
  static create(connection, logger, protocol, reconnectPolicy) {
    return new HubConnection(connection, logger, protocol, reconnectPolicy);
  }
  get state() {
    return this._connectionState;
  }
  get connectionId() {
    return this.connection ? this.connection.connectionId || null : null;
  }
  get baseUrl() {
    return this.connection.baseUrl || "";
  }
  set baseUrl(url) {
    if (this._connectionState !== HubConnectionState.Disconnected && this._connectionState !== HubConnectionState.Reconnecting) {
      throw new Error("The HubConnection must be in the Disconnected or Reconnecting state to change the url.");
    }
    if (!url) {
      throw new Error("The HubConnection url must be a valid url.");
    }
    this.connection.baseUrl = url;
  }
  start() {
    this._startPromise = this._startWithStateTransitions();
    return this._startPromise;
  }
  async _startWithStateTransitions() {
    if (this._connectionState !== HubConnectionState.Disconnected) {
      return Promise.reject(new Error("Cannot start a HubConnection that is not in the 'Disconnected' state."));
    }
    this._connectionState = HubConnectionState.Connecting;
    this._logger.log(LogLevel.Debug, "Starting HubConnection.");
    try {
      await this._startInternal();
      if (Platform.isBrowser) {
        window.document.addEventListener("freeze", this._freezeEventListener);
      }
      this._connectionState = HubConnectionState.Connected;
      this._connectionStarted = true;
      this._logger.log(LogLevel.Debug, "HubConnection connected successfully.");
    } catch (e2) {
      this._connectionState = HubConnectionState.Disconnected;
      this._logger.log(LogLevel.Debug, `HubConnection failed to start successfully because of error '${e2}'.`);
      return Promise.reject(e2);
    }
  }
  async _startInternal() {
    this._stopDuringStartError = void 0;
    this._receivedHandshakeResponse = false;
    const handshakePromise = new Promise((resolve, reject) => {
      this._handshakeResolver = resolve;
      this._handshakeRejecter = reject;
    });
    await this.connection.start(this._protocol.transferFormat);
    try {
      const handshakeRequest = {
        protocol: this._protocol.name,
        version: this._protocol.version
      };
      this._logger.log(LogLevel.Debug, "Sending handshake request.");
      await this._sendMessage(this._handshakeProtocol.writeHandshakeRequest(handshakeRequest));
      this._logger.log(LogLevel.Information, `Using HubProtocol '${this._protocol.name}'.`);
      this._cleanupTimeout();
      this._resetTimeoutPeriod();
      this._resetKeepAliveInterval();
      await handshakePromise;
      if (this._stopDuringStartError) {
        throw this._stopDuringStartError;
      }
    } catch (e2) {
      this._logger.log(LogLevel.Debug, `Hub handshake failed with error '${e2}' during start(). Stopping HubConnection.`);
      this._cleanupTimeout();
      this._cleanupPingTimer();
      await this.connection.stop(e2);
      throw e2;
    }
  }
  async stop() {
    const startPromise = this._startPromise;
    this._stopPromise = this._stopInternal();
    await this._stopPromise;
    try {
      await startPromise;
    } catch (e2) {
    }
  }
  _stopInternal(error) {
    if (this._connectionState === HubConnectionState.Disconnected) {
      this._logger.log(LogLevel.Debug, `Call to HubConnection.stop(${error}) ignored because it is already in the disconnected state.`);
      return Promise.resolve();
    }
    if (this._connectionState === HubConnectionState.Disconnecting) {
      this._logger.log(LogLevel.Debug, `Call to HttpConnection.stop(${error}) ignored because the connection is already in the disconnecting state.`);
      return this._stopPromise;
    }
    this._connectionState = HubConnectionState.Disconnecting;
    this._logger.log(LogLevel.Debug, "Stopping HubConnection.");
    if (this._reconnectDelayHandle) {
      this._logger.log(LogLevel.Debug, "Connection stopped during reconnect delay. Done reconnecting.");
      clearTimeout(this._reconnectDelayHandle);
      this._reconnectDelayHandle = void 0;
      this._completeClose();
      return Promise.resolve();
    }
    this._cleanupTimeout();
    this._cleanupPingTimer();
    this._stopDuringStartError = error || new Error("The connection was stopped before the hub handshake could complete.");
    return this.connection.stop(error);
  }
  stream(methodName, ...args) {
    const [streams, streamIds] = this._replaceStreamingParams(args);
    const invocationDescriptor = this._createStreamInvocation(methodName, args, streamIds);
    let promiseQueue;
    const subject = new Subject();
    subject.cancelCallback = () => {
      const cancelInvocation = this._createCancelInvocation(invocationDescriptor.invocationId);
      delete this._callbacks[invocationDescriptor.invocationId];
      return promiseQueue.then(() => {
        return this._sendWithProtocol(cancelInvocation);
      });
    };
    this._callbacks[invocationDescriptor.invocationId] = (invocationEvent, error) => {
      if (error) {
        subject.error(error);
        return;
      } else if (invocationEvent) {
        if (invocationEvent.type === MessageType.Completion) {
          if (invocationEvent.error) {
            subject.error(new Error(invocationEvent.error));
          } else {
            subject.complete();
          }
        } else {
          subject.next(invocationEvent.item);
        }
      }
    };
    promiseQueue = this._sendWithProtocol(invocationDescriptor).catch((e2) => {
      subject.error(e2);
      delete this._callbacks[invocationDescriptor.invocationId];
    });
    this._launchStreams(streams, promiseQueue);
    return subject;
  }
  _sendMessage(message) {
    this._resetKeepAliveInterval();
    return this.connection.send(message);
  }
  _sendWithProtocol(message) {
    return this._sendMessage(this._protocol.writeMessage(message));
  }
  send(methodName, ...args) {
    const [streams, streamIds] = this._replaceStreamingParams(args);
    const sendPromise = this._sendWithProtocol(this._createInvocation(methodName, args, true, streamIds));
    this._launchStreams(streams, sendPromise);
    return sendPromise;
  }
  invoke(methodName, ...args) {
    const [streams, streamIds] = this._replaceStreamingParams(args);
    const invocationDescriptor = this._createInvocation(methodName, args, false, streamIds);
    const p = new Promise((resolve, reject) => {
      this._callbacks[invocationDescriptor.invocationId] = (invocationEvent, error) => {
        if (error) {
          reject(error);
          return;
        } else if (invocationEvent) {
          if (invocationEvent.type === MessageType.Completion) {
            if (invocationEvent.error) {
              reject(new Error(invocationEvent.error));
            } else {
              resolve(invocationEvent.result);
            }
          } else {
            reject(new Error(`Unexpected message type: ${invocationEvent.type}`));
          }
        }
      };
      const promiseQueue = this._sendWithProtocol(invocationDescriptor).catch((e2) => {
        reject(e2);
        delete this._callbacks[invocationDescriptor.invocationId];
      });
      this._launchStreams(streams, promiseQueue);
    });
    return p;
  }
  on(methodName, newMethod) {
    if (!methodName || !newMethod) {
      return;
    }
    methodName = methodName.toLowerCase();
    if (!this._methods[methodName]) {
      this._methods[methodName] = [];
    }
    if (this._methods[methodName].indexOf(newMethod) !== -1) {
      return;
    }
    this._methods[methodName].push(newMethod);
  }
  off(methodName, method) {
    if (!methodName) {
      return;
    }
    methodName = methodName.toLowerCase();
    const handlers = this._methods[methodName];
    if (!handlers) {
      return;
    }
    if (method) {
      const removeIdx = handlers.indexOf(method);
      if (removeIdx !== -1) {
        handlers.splice(removeIdx, 1);
        if (handlers.length === 0) {
          delete this._methods[methodName];
        }
      }
    } else {
      delete this._methods[methodName];
    }
  }
  onclose(callback) {
    if (callback) {
      this._closedCallbacks.push(callback);
    }
  }
  onreconnecting(callback) {
    if (callback) {
      this._reconnectingCallbacks.push(callback);
    }
  }
  onreconnected(callback) {
    if (callback) {
      this._reconnectedCallbacks.push(callback);
    }
  }
  _processIncomingData(data) {
    this._cleanupTimeout();
    if (!this._receivedHandshakeResponse) {
      data = this._processHandshakeResponse(data);
      this._receivedHandshakeResponse = true;
    }
    if (data) {
      const messages = this._protocol.parseMessages(data, this._logger);
      for (const message of messages) {
        switch (message.type) {
          case MessageType.Invocation:
            this._invokeClientMethod(message);
            break;
          case MessageType.StreamItem:
          case MessageType.Completion: {
            const callback = this._callbacks[message.invocationId];
            if (callback) {
              if (message.type === MessageType.Completion) {
                delete this._callbacks[message.invocationId];
              }
              try {
                callback(message);
              } catch (e2) {
                this._logger.log(LogLevel.Error, `Stream callback threw error: ${getErrorString(e2)}`);
              }
            }
            break;
          }
          case MessageType.Ping:
            break;
          case MessageType.Close: {
            this._logger.log(LogLevel.Information, "Close message received from server.");
            const error = message.error ? new Error("Server returned an error on close: " + message.error) : void 0;
            if (message.allowReconnect === true) {
              this.connection.stop(error);
            } else {
              this._stopPromise = this._stopInternal(error);
            }
            break;
          }
          default:
            this._logger.log(LogLevel.Warning, `Invalid message type: ${message.type}.`);
            break;
        }
      }
    }
    this._resetTimeoutPeriod();
  }
  _processHandshakeResponse(data) {
    let responseMessage;
    let remainingData;
    try {
      [remainingData, responseMessage] = this._handshakeProtocol.parseHandshakeResponse(data);
    } catch (e2) {
      const message = "Error parsing handshake response: " + e2;
      this._logger.log(LogLevel.Error, message);
      const error = new Error(message);
      this._handshakeRejecter(error);
      throw error;
    }
    if (responseMessage.error) {
      const message = "Server returned handshake error: " + responseMessage.error;
      this._logger.log(LogLevel.Error, message);
      const error = new Error(message);
      this._handshakeRejecter(error);
      throw error;
    } else {
      this._logger.log(LogLevel.Debug, "Server handshake complete.");
    }
    this._handshakeResolver();
    return remainingData;
  }
  _resetKeepAliveInterval() {
    if (this.connection.features.inherentKeepAlive) {
      return;
    }
    this._nextKeepAlive = new Date().getTime() + this.keepAliveIntervalInMilliseconds;
    this._cleanupPingTimer();
  }
  _resetTimeoutPeriod() {
    if (!this.connection.features || !this.connection.features.inherentKeepAlive) {
      this._timeoutHandle = setTimeout(() => this.serverTimeout(), this.serverTimeoutInMilliseconds);
      if (this._pingServerHandle === void 0) {
        let nextPing = this._nextKeepAlive - new Date().getTime();
        if (nextPing < 0) {
          nextPing = 0;
        }
        this._pingServerHandle = setTimeout(async () => {
          if (this._connectionState === HubConnectionState.Connected) {
            try {
              await this._sendMessage(this._cachedPingMessage);
            } catch {
              this._cleanupPingTimer();
            }
          }
        }, nextPing);
      }
    }
  }
  serverTimeout() {
    this.connection.stop(new Error("Server timeout elapsed without receiving a message from the server."));
  }
  _invokeClientMethod(invocationMessage) {
    const methods = this._methods[invocationMessage.target.toLowerCase()];
    if (methods) {
      try {
        methods.forEach((m) => m.apply(this, invocationMessage.arguments));
      } catch (e2) {
        this._logger.log(LogLevel.Error, `A callback for the method ${invocationMessage.target.toLowerCase()} threw error '${e2}'.`);
      }
      if (invocationMessage.invocationId) {
        const message = "Server requested a response, which is not supported in this version of the client.";
        this._logger.log(LogLevel.Error, message);
        this._stopPromise = this._stopInternal(new Error(message));
      }
    } else {
      this._logger.log(LogLevel.Warning, `No client method with the name '${invocationMessage.target}' found.`);
    }
  }
  _connectionClosed(error) {
    this._logger.log(LogLevel.Debug, `HubConnection.connectionClosed(${error}) called while in state ${this._connectionState}.`);
    this._stopDuringStartError = this._stopDuringStartError || error || new Error("The underlying connection was closed before the hub handshake could complete.");
    if (this._handshakeResolver) {
      this._handshakeResolver();
    }
    this._cancelCallbacksWithError(error || new Error("Invocation canceled due to the underlying connection being closed."));
    this._cleanupTimeout();
    this._cleanupPingTimer();
    if (this._connectionState === HubConnectionState.Disconnecting) {
      this._completeClose(error);
    } else if (this._connectionState === HubConnectionState.Connected && this._reconnectPolicy) {
      this._reconnect(error);
    } else if (this._connectionState === HubConnectionState.Connected) {
      this._completeClose(error);
    }
  }
  _completeClose(error) {
    if (this._connectionStarted) {
      this._connectionState = HubConnectionState.Disconnected;
      this._connectionStarted = false;
      if (Platform.isBrowser) {
        window.document.removeEventListener("freeze", this._freezeEventListener);
      }
      try {
        this._closedCallbacks.forEach((c) => c.apply(this, [error]));
      } catch (e2) {
        this._logger.log(LogLevel.Error, `An onclose callback called with error '${error}' threw error '${e2}'.`);
      }
    }
  }
  async _reconnect(error) {
    const reconnectStartTime = Date.now();
    let previousReconnectAttempts = 0;
    let retryError = error !== void 0 ? error : new Error("Attempting to reconnect due to a unknown error.");
    let nextRetryDelay = this._getNextRetryDelay(previousReconnectAttempts++, 0, retryError);
    if (nextRetryDelay === null) {
      this._logger.log(LogLevel.Debug, "Connection not reconnecting because the IRetryPolicy returned null on the first reconnect attempt.");
      this._completeClose(error);
      return;
    }
    this._connectionState = HubConnectionState.Reconnecting;
    if (error) {
      this._logger.log(LogLevel.Information, `Connection reconnecting because of error '${error}'.`);
    } else {
      this._logger.log(LogLevel.Information, "Connection reconnecting.");
    }
    if (this._reconnectingCallbacks.length !== 0) {
      try {
        this._reconnectingCallbacks.forEach((c) => c.apply(this, [error]));
      } catch (e2) {
        this._logger.log(LogLevel.Error, `An onreconnecting callback called with error '${error}' threw error '${e2}'.`);
      }
      if (this._connectionState !== HubConnectionState.Reconnecting) {
        this._logger.log(LogLevel.Debug, "Connection left the reconnecting state in onreconnecting callback. Done reconnecting.");
        return;
      }
    }
    while (nextRetryDelay !== null) {
      this._logger.log(LogLevel.Information, `Reconnect attempt number ${previousReconnectAttempts} will start in ${nextRetryDelay} ms.`);
      await new Promise((resolve) => {
        this._reconnectDelayHandle = setTimeout(resolve, nextRetryDelay);
      });
      this._reconnectDelayHandle = void 0;
      if (this._connectionState !== HubConnectionState.Reconnecting) {
        this._logger.log(LogLevel.Debug, "Connection left the reconnecting state during reconnect delay. Done reconnecting.");
        return;
      }
      try {
        await this._startInternal();
        this._connectionState = HubConnectionState.Connected;
        this._logger.log(LogLevel.Information, "HubConnection reconnected successfully.");
        if (this._reconnectedCallbacks.length !== 0) {
          try {
            this._reconnectedCallbacks.forEach((c) => c.apply(this, [this.connection.connectionId]));
          } catch (e2) {
            this._logger.log(LogLevel.Error, `An onreconnected callback called with connectionId '${this.connection.connectionId}; threw error '${e2}'.`);
          }
        }
        return;
      } catch (e2) {
        this._logger.log(LogLevel.Information, `Reconnect attempt failed because of error '${e2}'.`);
        if (this._connectionState !== HubConnectionState.Reconnecting) {
          this._logger.log(LogLevel.Debug, `Connection moved to the '${this._connectionState}' from the reconnecting state during reconnect attempt. Done reconnecting.`);
          if (this._connectionState === HubConnectionState.Disconnecting) {
            this._completeClose();
          }
          return;
        }
        retryError = e2 instanceof Error ? e2 : new Error(e2.toString());
        nextRetryDelay = this._getNextRetryDelay(previousReconnectAttempts++, Date.now() - reconnectStartTime, retryError);
      }
    }
    this._logger.log(LogLevel.Information, `Reconnect retries have been exhausted after ${Date.now() - reconnectStartTime} ms and ${previousReconnectAttempts} failed attempts. Connection disconnecting.`);
    this._completeClose();
  }
  _getNextRetryDelay(previousRetryCount, elapsedMilliseconds, retryReason) {
    try {
      return this._reconnectPolicy.nextRetryDelayInMilliseconds({
        elapsedMilliseconds,
        previousRetryCount,
        retryReason
      });
    } catch (e2) {
      this._logger.log(LogLevel.Error, `IRetryPolicy.nextRetryDelayInMilliseconds(${previousRetryCount}, ${elapsedMilliseconds}) threw error '${e2}'.`);
      return null;
    }
  }
  _cancelCallbacksWithError(error) {
    const callbacks = this._callbacks;
    this._callbacks = {};
    Object.keys(callbacks).forEach((key) => {
      const callback = callbacks[key];
      try {
        callback(null, error);
      } catch (e2) {
        this._logger.log(LogLevel.Error, `Stream 'error' callback called with '${error}' threw error: ${getErrorString(e2)}`);
      }
    });
  }
  _cleanupPingTimer() {
    if (this._pingServerHandle) {
      clearTimeout(this._pingServerHandle);
      this._pingServerHandle = void 0;
    }
  }
  _cleanupTimeout() {
    if (this._timeoutHandle) {
      clearTimeout(this._timeoutHandle);
    }
  }
  _createInvocation(methodName, args, nonblocking, streamIds) {
    if (nonblocking) {
      if (streamIds.length !== 0) {
        return {
          arguments: args,
          streamIds,
          target: methodName,
          type: MessageType.Invocation
        };
      } else {
        return {
          arguments: args,
          target: methodName,
          type: MessageType.Invocation
        };
      }
    } else {
      const invocationId = this._invocationId;
      this._invocationId++;
      if (streamIds.length !== 0) {
        return {
          arguments: args,
          invocationId: invocationId.toString(),
          streamIds,
          target: methodName,
          type: MessageType.Invocation
        };
      } else {
        return {
          arguments: args,
          invocationId: invocationId.toString(),
          target: methodName,
          type: MessageType.Invocation
        };
      }
    }
  }
  _launchStreams(streams, promiseQueue) {
    if (streams.length === 0) {
      return;
    }
    if (!promiseQueue) {
      promiseQueue = Promise.resolve();
    }
    for (const streamId in streams) {
      streams[streamId].subscribe({
        complete: () => {
          promiseQueue = promiseQueue.then(() => this._sendWithProtocol(this._createCompletionMessage(streamId)));
        },
        error: (err) => {
          let message;
          if (err instanceof Error) {
            message = err.message;
          } else if (err && err.toString) {
            message = err.toString();
          } else {
            message = "Unknown error";
          }
          promiseQueue = promiseQueue.then(() => this._sendWithProtocol(this._createCompletionMessage(streamId, message)));
        },
        next: (item) => {
          promiseQueue = promiseQueue.then(() => this._sendWithProtocol(this._createStreamItemMessage(streamId, item)));
        }
      });
    }
  }
  _replaceStreamingParams(args) {
    const streams = [];
    const streamIds = [];
    for (let i = 0; i < args.length; i++) {
      const argument = args[i];
      if (this._isObservable(argument)) {
        const streamId = this._invocationId;
        this._invocationId++;
        streams[streamId] = argument;
        streamIds.push(streamId.toString());
        args.splice(i, 1);
      }
    }
    return [streams, streamIds];
  }
  _isObservable(arg) {
    return arg && arg.subscribe && typeof arg.subscribe === "function";
  }
  _createStreamInvocation(methodName, args, streamIds) {
    const invocationId = this._invocationId;
    this._invocationId++;
    if (streamIds.length !== 0) {
      return {
        arguments: args,
        invocationId: invocationId.toString(),
        streamIds,
        target: methodName,
        type: MessageType.StreamInvocation
      };
    } else {
      return {
        arguments: args,
        invocationId: invocationId.toString(),
        target: methodName,
        type: MessageType.StreamInvocation
      };
    }
  }
  _createCancelInvocation(id) {
    return {
      invocationId: id,
      type: MessageType.CancelInvocation
    };
  }
  _createStreamItemMessage(id, item) {
    return {
      invocationId: id,
      item,
      type: MessageType.StreamItem
    };
  }
  _createCompletionMessage(id, error, result) {
    if (error) {
      return {
        error,
        invocationId: id,
        type: MessageType.Completion
      };
    }
    return {
      invocationId: id,
      result,
      type: MessageType.Completion
    };
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/DefaultReconnectPolicy.js
var DEFAULT_RETRY_DELAYS_IN_MILLISECONDS = [0, 2e3, 1e4, 3e4, null];
var DefaultReconnectPolicy = class {
  constructor(retryDelays) {
    this._retryDelays = retryDelays !== void 0 ? [...retryDelays, null] : DEFAULT_RETRY_DELAYS_IN_MILLISECONDS;
  }
  nextRetryDelayInMilliseconds(retryContext) {
    return this._retryDelays[retryContext.previousRetryCount];
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/HeaderNames.js
var HeaderNames = class {
};
HeaderNames.Authorization = "Authorization";
HeaderNames.Cookie = "Cookie";

// ../../node_modules/@microsoft/signalr/dist/esm/ITransport.js
var HttpTransportType;
(function(HttpTransportType2) {
  HttpTransportType2[HttpTransportType2["None"] = 0] = "None";
  HttpTransportType2[HttpTransportType2["WebSockets"] = 1] = "WebSockets";
  HttpTransportType2[HttpTransportType2["ServerSentEvents"] = 2] = "ServerSentEvents";
  HttpTransportType2[HttpTransportType2["LongPolling"] = 4] = "LongPolling";
})(HttpTransportType || (HttpTransportType = {}));
var TransferFormat;
(function(TransferFormat2) {
  TransferFormat2[TransferFormat2["Text"] = 1] = "Text";
  TransferFormat2[TransferFormat2["Binary"] = 2] = "Binary";
})(TransferFormat || (TransferFormat = {}));

// ../../node_modules/@microsoft/signalr/dist/esm/AbortController.js
var AbortController2 = class {
  constructor() {
    this._isAborted = false;
    this.onabort = null;
  }
  abort() {
    if (!this._isAborted) {
      this._isAborted = true;
      if (this.onabort) {
        this.onabort();
      }
    }
  }
  get signal() {
    return this;
  }
  get aborted() {
    return this._isAborted;
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/LongPollingTransport.js
var LongPollingTransport = class {
  constructor(httpClient, accessTokenFactory, logger, options) {
    this._httpClient = httpClient;
    this._accessTokenFactory = accessTokenFactory;
    this._logger = logger;
    this._pollAbort = new AbortController2();
    this._options = options;
    this._running = false;
    this.onreceive = null;
    this.onclose = null;
  }
  get pollAborted() {
    return this._pollAbort.aborted;
  }
  async connect(url, transferFormat) {
    Arg.isRequired(url, "url");
    Arg.isRequired(transferFormat, "transferFormat");
    Arg.isIn(transferFormat, TransferFormat, "transferFormat");
    this._url = url;
    this._logger.log(LogLevel.Trace, "(LongPolling transport) Connecting.");
    if (transferFormat === TransferFormat.Binary && (typeof XMLHttpRequest !== "undefined" && typeof new XMLHttpRequest().responseType !== "string")) {
      throw new Error("Binary protocols over XmlHttpRequest not implementing advanced features are not supported.");
    }
    const [name, value] = getUserAgentHeader();
    const headers = { [name]: value, ...this._options.headers };
    const pollOptions = {
      abortSignal: this._pollAbort.signal,
      headers,
      timeout: 1e5,
      withCredentials: this._options.withCredentials
    };
    if (transferFormat === TransferFormat.Binary) {
      pollOptions.responseType = "arraybuffer";
    }
    const token = await this._getAccessToken();
    this._updateHeaderToken(pollOptions, token);
    const pollUrl = `${url}&_=${Date.now()}`;
    this._logger.log(LogLevel.Trace, `(LongPolling transport) polling: ${pollUrl}.`);
    const response = await this._httpClient.get(pollUrl, pollOptions);
    if (response.statusCode !== 200) {
      this._logger.log(LogLevel.Error, `(LongPolling transport) Unexpected response code: ${response.statusCode}.`);
      this._closeError = new HttpError(response.statusText || "", response.statusCode);
      this._running = false;
    } else {
      this._running = true;
    }
    this._receiving = this._poll(this._url, pollOptions);
  }
  async _getAccessToken() {
    if (this._accessTokenFactory) {
      return await this._accessTokenFactory();
    }
    return null;
  }
  _updateHeaderToken(request, token) {
    if (!request.headers) {
      request.headers = {};
    }
    if (token) {
      request.headers[HeaderNames.Authorization] = `Bearer ${token}`;
      return;
    }
    if (request.headers[HeaderNames.Authorization]) {
      delete request.headers[HeaderNames.Authorization];
    }
  }
  async _poll(url, pollOptions) {
    try {
      while (this._running) {
        const token = await this._getAccessToken();
        this._updateHeaderToken(pollOptions, token);
        try {
          const pollUrl = `${url}&_=${Date.now()}`;
          this._logger.log(LogLevel.Trace, `(LongPolling transport) polling: ${pollUrl}.`);
          const response = await this._httpClient.get(pollUrl, pollOptions);
          if (response.statusCode === 204) {
            this._logger.log(LogLevel.Information, "(LongPolling transport) Poll terminated by server.");
            this._running = false;
          } else if (response.statusCode !== 200) {
            this._logger.log(LogLevel.Error, `(LongPolling transport) Unexpected response code: ${response.statusCode}.`);
            this._closeError = new HttpError(response.statusText || "", response.statusCode);
            this._running = false;
          } else {
            if (response.content) {
              this._logger.log(LogLevel.Trace, `(LongPolling transport) data received. ${getDataDetail(response.content, this._options.logMessageContent)}.`);
              if (this.onreceive) {
                this.onreceive(response.content);
              }
            } else {
              this._logger.log(LogLevel.Trace, "(LongPolling transport) Poll timed out, reissuing.");
            }
          }
        } catch (e2) {
          if (!this._running) {
            this._logger.log(LogLevel.Trace, `(LongPolling transport) Poll errored after shutdown: ${e2.message}`);
          } else {
            if (e2 instanceof TimeoutError) {
              this._logger.log(LogLevel.Trace, "(LongPolling transport) Poll timed out, reissuing.");
            } else {
              this._closeError = e2;
              this._running = false;
            }
          }
        }
      }
    } finally {
      this._logger.log(LogLevel.Trace, "(LongPolling transport) Polling complete.");
      if (!this.pollAborted) {
        this._raiseOnClose();
      }
    }
  }
  async send(data) {
    if (!this._running) {
      return Promise.reject(new Error("Cannot send until the transport is connected"));
    }
    return sendMessage(this._logger, "LongPolling", this._httpClient, this._url, this._accessTokenFactory, data, this._options);
  }
  async stop() {
    this._logger.log(LogLevel.Trace, "(LongPolling transport) Stopping polling.");
    this._running = false;
    this._pollAbort.abort();
    try {
      await this._receiving;
      this._logger.log(LogLevel.Trace, `(LongPolling transport) sending DELETE request to ${this._url}.`);
      const headers = {};
      const [name, value] = getUserAgentHeader();
      headers[name] = value;
      const deleteOptions = {
        headers: { ...headers, ...this._options.headers },
        timeout: this._options.timeout,
        withCredentials: this._options.withCredentials
      };
      const token = await this._getAccessToken();
      this._updateHeaderToken(deleteOptions, token);
      await this._httpClient.delete(this._url, deleteOptions);
      this._logger.log(LogLevel.Trace, "(LongPolling transport) DELETE request sent.");
    } finally {
      this._logger.log(LogLevel.Trace, "(LongPolling transport) Stop finished.");
      this._raiseOnClose();
    }
  }
  _raiseOnClose() {
    if (this.onclose) {
      let logMessage = "(LongPolling transport) Firing onclose event.";
      if (this._closeError) {
        logMessage += " Error: " + this._closeError;
      }
      this._logger.log(LogLevel.Trace, logMessage);
      this.onclose(this._closeError);
    }
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/ServerSentEventsTransport.js
var ServerSentEventsTransport = class {
  constructor(httpClient, accessTokenFactory, logger, options) {
    this._httpClient = httpClient;
    this._accessTokenFactory = accessTokenFactory;
    this._logger = logger;
    this._options = options;
    this.onreceive = null;
    this.onclose = null;
  }
  async connect(url, transferFormat) {
    Arg.isRequired(url, "url");
    Arg.isRequired(transferFormat, "transferFormat");
    Arg.isIn(transferFormat, TransferFormat, "transferFormat");
    this._logger.log(LogLevel.Trace, "(SSE transport) Connecting.");
    this._url = url;
    if (this._accessTokenFactory) {
      const token = await this._accessTokenFactory();
      if (token) {
        url += (url.indexOf("?") < 0 ? "?" : "&") + `access_token=${encodeURIComponent(token)}`;
      }
    }
    return new Promise((resolve, reject) => {
      let opened = false;
      if (transferFormat !== TransferFormat.Text) {
        reject(new Error("The Server-Sent Events transport only supports the 'Text' transfer format"));
        return;
      }
      let eventSource;
      if (Platform.isBrowser || Platform.isWebWorker) {
        eventSource = new this._options.EventSource(url, { withCredentials: this._options.withCredentials });
      } else {
        const cookies = this._httpClient.getCookieString(url);
        const headers = {};
        headers.Cookie = cookies;
        const [name, value] = getUserAgentHeader();
        headers[name] = value;
        eventSource = new this._options.EventSource(url, { withCredentials: this._options.withCredentials, headers: { ...headers, ...this._options.headers } });
      }
      try {
        eventSource.onmessage = (e2) => {
          if (this.onreceive) {
            try {
              this._logger.log(LogLevel.Trace, `(SSE transport) data received. ${getDataDetail(e2.data, this._options.logMessageContent)}.`);
              this.onreceive(e2.data);
            } catch (error) {
              this._close(error);
              return;
            }
          }
        };
        eventSource.onerror = (e2) => {
          if (opened) {
            this._close();
          } else {
            reject(new Error("EventSource failed to connect. The connection could not be found on the server, either the connection ID is not present on the server, or a proxy is refusing/buffering the connection. If you have multiple servers check that sticky sessions are enabled."));
          }
        };
        eventSource.onopen = () => {
          this._logger.log(LogLevel.Information, `SSE connected to ${this._url}`);
          this._eventSource = eventSource;
          opened = true;
          resolve();
        };
      } catch (e2) {
        reject(e2);
        return;
      }
    });
  }
  async send(data) {
    if (!this._eventSource) {
      return Promise.reject(new Error("Cannot send until the transport is connected"));
    }
    return sendMessage(this._logger, "SSE", this._httpClient, this._url, this._accessTokenFactory, data, this._options);
  }
  stop() {
    this._close();
    return Promise.resolve();
  }
  _close(e2) {
    if (this._eventSource) {
      this._eventSource.close();
      this._eventSource = void 0;
      if (this.onclose) {
        this.onclose(e2);
      }
    }
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/WebSocketTransport.js
var WebSocketTransport = class {
  constructor(httpClient, accessTokenFactory, logger, logMessageContent, webSocketConstructor, headers) {
    this._logger = logger;
    this._accessTokenFactory = accessTokenFactory;
    this._logMessageContent = logMessageContent;
    this._webSocketConstructor = webSocketConstructor;
    this._httpClient = httpClient;
    this.onreceive = null;
    this.onclose = null;
    this._headers = headers;
  }
  async connect(url, transferFormat) {
    Arg.isRequired(url, "url");
    Arg.isRequired(transferFormat, "transferFormat");
    Arg.isIn(transferFormat, TransferFormat, "transferFormat");
    this._logger.log(LogLevel.Trace, "(WebSockets transport) Connecting.");
    if (this._accessTokenFactory) {
      const token = await this._accessTokenFactory();
      if (token) {
        url += (url.indexOf("?") < 0 ? "?" : "&") + `access_token=${encodeURIComponent(token)}`;
      }
    }
    return new Promise((resolve, reject) => {
      url = url.replace(/^http/, "ws");
      let webSocket;
      const cookies = this._httpClient.getCookieString(url);
      let opened = false;
      if (Platform.isNode) {
        const headers = {};
        const [name, value] = getUserAgentHeader();
        headers[name] = value;
        if (cookies) {
          headers[HeaderNames.Cookie] = `${cookies}`;
        }
        webSocket = new this._webSocketConstructor(url, void 0, {
          headers: { ...headers, ...this._headers }
        });
      }
      if (!webSocket) {
        webSocket = new this._webSocketConstructor(url);
      }
      if (transferFormat === TransferFormat.Binary) {
        webSocket.binaryType = "arraybuffer";
      }
      webSocket.onopen = (_event) => {
        this._logger.log(LogLevel.Information, `WebSocket connected to ${url}.`);
        this._webSocket = webSocket;
        opened = true;
        resolve();
      };
      webSocket.onerror = (event) => {
        let error = null;
        if (typeof ErrorEvent !== "undefined" && event instanceof ErrorEvent) {
          error = event.error;
        } else {
          error = "There was an error with the transport";
        }
        this._logger.log(LogLevel.Information, `(WebSockets transport) ${error}.`);
      };
      webSocket.onmessage = (message) => {
        this._logger.log(LogLevel.Trace, `(WebSockets transport) data received. ${getDataDetail(message.data, this._logMessageContent)}.`);
        if (this.onreceive) {
          try {
            this.onreceive(message.data);
          } catch (error) {
            this._close(error);
            return;
          }
        }
      };
      webSocket.onclose = (event) => {
        if (opened) {
          this._close(event);
        } else {
          let error = null;
          if (typeof ErrorEvent !== "undefined" && event instanceof ErrorEvent) {
            error = event.error;
          } else {
            error = "WebSocket failed to connect. The connection could not be found on the server, either the endpoint may not be a SignalR endpoint, the connection ID is not present on the server, or there is a proxy blocking WebSockets. If you have multiple servers check that sticky sessions are enabled.";
          }
          reject(new Error(error));
        }
      };
    });
  }
  send(data) {
    if (this._webSocket && this._webSocket.readyState === this._webSocketConstructor.OPEN) {
      this._logger.log(LogLevel.Trace, `(WebSockets transport) sending data. ${getDataDetail(data, this._logMessageContent)}.`);
      this._webSocket.send(data);
      return Promise.resolve();
    }
    return Promise.reject("WebSocket is not in the OPEN state");
  }
  stop() {
    if (this._webSocket) {
      this._close(void 0);
    }
    return Promise.resolve();
  }
  _close(event) {
    if (this._webSocket) {
      this._webSocket.onclose = () => {
      };
      this._webSocket.onmessage = () => {
      };
      this._webSocket.onerror = () => {
      };
      this._webSocket.close();
      this._webSocket = void 0;
    }
    this._logger.log(LogLevel.Trace, "(WebSockets transport) socket closed.");
    if (this.onclose) {
      if (this._isCloseEvent(event) && (event.wasClean === false || event.code !== 1e3)) {
        this.onclose(new Error(`WebSocket closed with status code: ${event.code} (${event.reason || "no reason given"}).`));
      } else if (event instanceof Error) {
        this.onclose(event);
      } else {
        this.onclose();
      }
    }
  }
  _isCloseEvent(event) {
    return event && typeof event.wasClean === "boolean" && typeof event.code === "number";
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/HttpConnection.js
var MAX_REDIRECTS = 100;
var HttpConnection = class {
  constructor(url, options = {}) {
    this._stopPromiseResolver = () => {
    };
    this.features = {};
    this._negotiateVersion = 1;
    Arg.isRequired(url, "url");
    this._logger = createLogger(options.logger);
    this.baseUrl = this._resolveUrl(url);
    options = options || {};
    options.logMessageContent = options.logMessageContent === void 0 ? false : options.logMessageContent;
    if (typeof options.withCredentials === "boolean" || options.withCredentials === void 0) {
      options.withCredentials = options.withCredentials === void 0 ? true : options.withCredentials;
    } else {
      throw new Error("withCredentials option was not a 'boolean' or 'undefined' value");
    }
    options.timeout = options.timeout === void 0 ? 100 * 1e3 : options.timeout;
    let webSocketModule = null;
    let eventSourceModule = null;
    if (Platform.isNode && typeof __require !== "undefined") {
      const requireFunc = typeof __webpack_require__ === "function" ? __non_webpack_require__ : __require;
      webSocketModule = requireFunc("ws");
      eventSourceModule = requireFunc("eventsource");
    }
    if (!Platform.isNode && typeof WebSocket !== "undefined" && !options.WebSocket) {
      options.WebSocket = WebSocket;
    } else if (Platform.isNode && !options.WebSocket) {
      if (webSocketModule) {
        options.WebSocket = webSocketModule;
      }
    }
    if (!Platform.isNode && typeof EventSource !== "undefined" && !options.EventSource) {
      options.EventSource = EventSource;
    } else if (Platform.isNode && !options.EventSource) {
      if (typeof eventSourceModule !== "undefined") {
        options.EventSource = eventSourceModule;
      }
    }
    this._httpClient = options.httpClient || new DefaultHttpClient(this._logger);
    this._connectionState = "Disconnected";
    this._connectionStarted = false;
    this._options = options;
    this.onreceive = null;
    this.onclose = null;
  }
  async start(transferFormat) {
    transferFormat = transferFormat || TransferFormat.Binary;
    Arg.isIn(transferFormat, TransferFormat, "transferFormat");
    this._logger.log(LogLevel.Debug, `Starting connection with transfer format '${TransferFormat[transferFormat]}'.`);
    if (this._connectionState !== "Disconnected") {
      return Promise.reject(new Error("Cannot start an HttpConnection that is not in the 'Disconnected' state."));
    }
    this._connectionState = "Connecting";
    this._startInternalPromise = this._startInternal(transferFormat);
    await this._startInternalPromise;
    if (this._connectionState === "Disconnecting") {
      const message = "Failed to start the HttpConnection before stop() was called.";
      this._logger.log(LogLevel.Error, message);
      await this._stopPromise;
      return Promise.reject(new Error(message));
    } else if (this._connectionState !== "Connected") {
      const message = "HttpConnection.startInternal completed gracefully but didn't enter the connection into the connected state!";
      this._logger.log(LogLevel.Error, message);
      return Promise.reject(new Error(message));
    }
    this._connectionStarted = true;
  }
  send(data) {
    if (this._connectionState !== "Connected") {
      return Promise.reject(new Error("Cannot send data if the connection is not in the 'Connected' State."));
    }
    if (!this._sendQueue) {
      this._sendQueue = new TransportSendQueue(this.transport);
    }
    return this._sendQueue.send(data);
  }
  async stop(error) {
    if (this._connectionState === "Disconnected") {
      this._logger.log(LogLevel.Debug, `Call to HttpConnection.stop(${error}) ignored because the connection is already in the disconnected state.`);
      return Promise.resolve();
    }
    if (this._connectionState === "Disconnecting") {
      this._logger.log(LogLevel.Debug, `Call to HttpConnection.stop(${error}) ignored because the connection is already in the disconnecting state.`);
      return this._stopPromise;
    }
    this._connectionState = "Disconnecting";
    this._stopPromise = new Promise((resolve) => {
      this._stopPromiseResolver = resolve;
    });
    await this._stopInternal(error);
    await this._stopPromise;
  }
  async _stopInternal(error) {
    this._stopError = error;
    try {
      await this._startInternalPromise;
    } catch (e2) {
    }
    if (this.transport) {
      try {
        await this.transport.stop();
      } catch (e2) {
        this._logger.log(LogLevel.Error, `HttpConnection.transport.stop() threw error '${e2}'.`);
        this._stopConnection();
      }
      this.transport = void 0;
    } else {
      this._logger.log(LogLevel.Debug, "HttpConnection.transport is undefined in HttpConnection.stop() because start() failed.");
    }
  }
  async _startInternal(transferFormat) {
    let url = this.baseUrl;
    this._accessTokenFactory = this._options.accessTokenFactory;
    try {
      if (this._options.skipNegotiation) {
        if (this._options.transport === HttpTransportType.WebSockets) {
          this.transport = this._constructTransport(HttpTransportType.WebSockets);
          await this._startTransport(url, transferFormat);
        } else {
          throw new Error("Negotiation can only be skipped when using the WebSocket transport directly.");
        }
      } else {
        let negotiateResponse = null;
        let redirects = 0;
        do {
          negotiateResponse = await this._getNegotiationResponse(url);
          if (this._connectionState === "Disconnecting" || this._connectionState === "Disconnected") {
            throw new Error("The connection was stopped during negotiation.");
          }
          if (negotiateResponse.error) {
            throw new Error(negotiateResponse.error);
          }
          if (negotiateResponse.ProtocolVersion) {
            throw new Error("Detected a connection attempt to an ASP.NET SignalR Server. This client only supports connecting to an ASP.NET Core SignalR Server. See https://aka.ms/signalr-core-differences for details.");
          }
          if (negotiateResponse.url) {
            url = negotiateResponse.url;
          }
          if (negotiateResponse.accessToken) {
            const accessToken = negotiateResponse.accessToken;
            this._accessTokenFactory = () => accessToken;
          }
          redirects++;
        } while (negotiateResponse.url && redirects < MAX_REDIRECTS);
        if (redirects === MAX_REDIRECTS && negotiateResponse.url) {
          throw new Error("Negotiate redirection limit exceeded.");
        }
        await this._createTransport(url, this._options.transport, negotiateResponse, transferFormat);
      }
      if (this.transport instanceof LongPollingTransport) {
        this.features.inherentKeepAlive = true;
      }
      if (this._connectionState === "Connecting") {
        this._logger.log(LogLevel.Debug, "The HttpConnection connected successfully.");
        this._connectionState = "Connected";
      }
    } catch (e2) {
      this._logger.log(LogLevel.Error, "Failed to start the connection: " + e2);
      this._connectionState = "Disconnected";
      this.transport = void 0;
      this._stopPromiseResolver();
      return Promise.reject(e2);
    }
  }
  async _getNegotiationResponse(url) {
    const headers = {};
    if (this._accessTokenFactory) {
      const token = await this._accessTokenFactory();
      if (token) {
        headers[HeaderNames.Authorization] = `Bearer ${token}`;
      }
    }
    const [name, value] = getUserAgentHeader();
    headers[name] = value;
    const negotiateUrl = this._resolveNegotiateUrl(url);
    this._logger.log(LogLevel.Debug, `Sending negotiation request: ${negotiateUrl}.`);
    try {
      const response = await this._httpClient.post(negotiateUrl, {
        content: "",
        headers: { ...headers, ...this._options.headers },
        timeout: this._options.timeout,
        withCredentials: this._options.withCredentials
      });
      if (response.statusCode !== 200) {
        return Promise.reject(new Error(`Unexpected status code returned from negotiate '${response.statusCode}'`));
      }
      const negotiateResponse = JSON.parse(response.content);
      if (!negotiateResponse.negotiateVersion || negotiateResponse.negotiateVersion < 1) {
        negotiateResponse.connectionToken = negotiateResponse.connectionId;
      }
      return negotiateResponse;
    } catch (e2) {
      let errorMessage = "Failed to complete negotiation with the server: " + e2;
      if (e2 instanceof HttpError) {
        if (e2.statusCode === 404) {
          errorMessage = errorMessage + " Either this is not a SignalR endpoint or there is a proxy blocking the connection.";
        }
      }
      this._logger.log(LogLevel.Error, errorMessage);
      return Promise.reject(new FailedToNegotiateWithServerError(errorMessage));
    }
  }
  _createConnectUrl(url, connectionToken) {
    if (!connectionToken) {
      return url;
    }
    return url + (url.indexOf("?") === -1 ? "?" : "&") + `id=${connectionToken}`;
  }
  async _createTransport(url, requestedTransport, negotiateResponse, requestedTransferFormat) {
    let connectUrl = this._createConnectUrl(url, negotiateResponse.connectionToken);
    if (this._isITransport(requestedTransport)) {
      this._logger.log(LogLevel.Debug, "Connection was provided an instance of ITransport, using that directly.");
      this.transport = requestedTransport;
      await this._startTransport(connectUrl, requestedTransferFormat);
      this.connectionId = negotiateResponse.connectionId;
      return;
    }
    const transportExceptions = [];
    const transports = negotiateResponse.availableTransports || [];
    let negotiate = negotiateResponse;
    for (const endpoint of transports) {
      const transportOrError = this._resolveTransportOrError(endpoint, requestedTransport, requestedTransferFormat);
      if (transportOrError instanceof Error) {
        transportExceptions.push(`${endpoint.transport} failed:`);
        transportExceptions.push(transportOrError);
      } else if (this._isITransport(transportOrError)) {
        this.transport = transportOrError;
        if (!negotiate) {
          try {
            negotiate = await this._getNegotiationResponse(url);
          } catch (ex) {
            return Promise.reject(ex);
          }
          connectUrl = this._createConnectUrl(url, negotiate.connectionToken);
        }
        try {
          await this._startTransport(connectUrl, requestedTransferFormat);
          this.connectionId = negotiate.connectionId;
          return;
        } catch (ex) {
          this._logger.log(LogLevel.Error, `Failed to start the transport '${endpoint.transport}': ${ex}`);
          negotiate = void 0;
          transportExceptions.push(new FailedToStartTransportError(`${endpoint.transport} failed: ${ex}`, HttpTransportType[endpoint.transport]));
          if (this._connectionState !== "Connecting") {
            const message = "Failed to select transport before stop() was called.";
            this._logger.log(LogLevel.Debug, message);
            return Promise.reject(new Error(message));
          }
        }
      }
    }
    if (transportExceptions.length > 0) {
      return Promise.reject(new AggregateErrors(`Unable to connect to the server with any of the available transports. ${transportExceptions.join(" ")}`, transportExceptions));
    }
    return Promise.reject(new Error("None of the transports supported by the client are supported by the server."));
  }
  _constructTransport(transport) {
    switch (transport) {
      case HttpTransportType.WebSockets:
        if (!this._options.WebSocket) {
          throw new Error("'WebSocket' is not supported in your environment.");
        }
        return new WebSocketTransport(this._httpClient, this._accessTokenFactory, this._logger, this._options.logMessageContent, this._options.WebSocket, this._options.headers || {});
      case HttpTransportType.ServerSentEvents:
        if (!this._options.EventSource) {
          throw new Error("'EventSource' is not supported in your environment.");
        }
        return new ServerSentEventsTransport(this._httpClient, this._accessTokenFactory, this._logger, this._options);
      case HttpTransportType.LongPolling:
        return new LongPollingTransport(this._httpClient, this._accessTokenFactory, this._logger, this._options);
      default:
        throw new Error(`Unknown transport: ${transport}.`);
    }
  }
  _startTransport(url, transferFormat) {
    this.transport.onreceive = this.onreceive;
    this.transport.onclose = (e2) => this._stopConnection(e2);
    return this.transport.connect(url, transferFormat);
  }
  _resolveTransportOrError(endpoint, requestedTransport, requestedTransferFormat) {
    const transport = HttpTransportType[endpoint.transport];
    if (transport === null || transport === void 0) {
      this._logger.log(LogLevel.Debug, `Skipping transport '${endpoint.transport}' because it is not supported by this client.`);
      return new Error(`Skipping transport '${endpoint.transport}' because it is not supported by this client.`);
    } else {
      if (transportMatches(requestedTransport, transport)) {
        const transferFormats = endpoint.transferFormats.map((s) => TransferFormat[s]);
        if (transferFormats.indexOf(requestedTransferFormat) >= 0) {
          if (transport === HttpTransportType.WebSockets && !this._options.WebSocket || transport === HttpTransportType.ServerSentEvents && !this._options.EventSource) {
            this._logger.log(LogLevel.Debug, `Skipping transport '${HttpTransportType[transport]}' because it is not supported in your environment.'`);
            return new UnsupportedTransportError(`'${HttpTransportType[transport]}' is not supported in your environment.`, transport);
          } else {
            this._logger.log(LogLevel.Debug, `Selecting transport '${HttpTransportType[transport]}'.`);
            try {
              return this._constructTransport(transport);
            } catch (ex) {
              return ex;
            }
          }
        } else {
          this._logger.log(LogLevel.Debug, `Skipping transport '${HttpTransportType[transport]}' because it does not support the requested transfer format '${TransferFormat[requestedTransferFormat]}'.`);
          return new Error(`'${HttpTransportType[transport]}' does not support ${TransferFormat[requestedTransferFormat]}.`);
        }
      } else {
        this._logger.log(LogLevel.Debug, `Skipping transport '${HttpTransportType[transport]}' because it was disabled by the client.`);
        return new DisabledTransportError(`'${HttpTransportType[transport]}' is disabled by the client.`, transport);
      }
    }
  }
  _isITransport(transport) {
    return transport && typeof transport === "object" && "connect" in transport;
  }
  _stopConnection(error) {
    this._logger.log(LogLevel.Debug, `HttpConnection.stopConnection(${error}) called while in state ${this._connectionState}.`);
    this.transport = void 0;
    error = this._stopError || error;
    this._stopError = void 0;
    if (this._connectionState === "Disconnected") {
      this._logger.log(LogLevel.Debug, `Call to HttpConnection.stopConnection(${error}) was ignored because the connection is already in the disconnected state.`);
      return;
    }
    if (this._connectionState === "Connecting") {
      this._logger.log(LogLevel.Warning, `Call to HttpConnection.stopConnection(${error}) was ignored because the connection is still in the connecting state.`);
      throw new Error(`HttpConnection.stopConnection(${error}) was called while the connection is still in the connecting state.`);
    }
    if (this._connectionState === "Disconnecting") {
      this._stopPromiseResolver();
    }
    if (error) {
      this._logger.log(LogLevel.Error, `Connection disconnected with error '${error}'.`);
    } else {
      this._logger.log(LogLevel.Information, "Connection disconnected.");
    }
    if (this._sendQueue) {
      this._sendQueue.stop().catch((e2) => {
        this._logger.log(LogLevel.Error, `TransportSendQueue.stop() threw error '${e2}'.`);
      });
      this._sendQueue = void 0;
    }
    this.connectionId = void 0;
    this._connectionState = "Disconnected";
    if (this._connectionStarted) {
      this._connectionStarted = false;
      try {
        if (this.onclose) {
          this.onclose(error);
        }
      } catch (e2) {
        this._logger.log(LogLevel.Error, `HttpConnection.onclose(${error}) threw error '${e2}'.`);
      }
    }
  }
  _resolveUrl(url) {
    if (url.lastIndexOf("https://", 0) === 0 || url.lastIndexOf("http://", 0) === 0) {
      return url;
    }
    if (!Platform.isBrowser) {
      throw new Error(`Cannot resolve '${url}'.`);
    }
    const aTag = window.document.createElement("a");
    aTag.href = url;
    this._logger.log(LogLevel.Information, `Normalizing '${url}' to '${aTag.href}'.`);
    return aTag.href;
  }
  _resolveNegotiateUrl(url) {
    const index = url.indexOf("?");
    let negotiateUrl = url.substring(0, index === -1 ? url.length : index);
    if (negotiateUrl[negotiateUrl.length - 1] !== "/") {
      negotiateUrl += "/";
    }
    negotiateUrl += "negotiate";
    negotiateUrl += index === -1 ? "" : url.substring(index);
    if (negotiateUrl.indexOf("negotiateVersion") === -1) {
      negotiateUrl += index === -1 ? "?" : "&";
      negotiateUrl += "negotiateVersion=" + this._negotiateVersion;
    }
    return negotiateUrl;
  }
};
function transportMatches(requestedTransport, actualTransport) {
  return !requestedTransport || (actualTransport & requestedTransport) !== 0;
}
var TransportSendQueue = class {
  constructor(_transport) {
    this._transport = _transport;
    this._buffer = [];
    this._executing = true;
    this._sendBufferedData = new PromiseSource();
    this._transportResult = new PromiseSource();
    this._sendLoopPromise = this._sendLoop();
  }
  send(data) {
    this._bufferData(data);
    if (!this._transportResult) {
      this._transportResult = new PromiseSource();
    }
    return this._transportResult.promise;
  }
  stop() {
    this._executing = false;
    this._sendBufferedData.resolve();
    return this._sendLoopPromise;
  }
  _bufferData(data) {
    if (this._buffer.length && typeof this._buffer[0] !== typeof data) {
      throw new Error(`Expected data to be of type ${typeof this._buffer} but was of type ${typeof data}`);
    }
    this._buffer.push(data);
    this._sendBufferedData.resolve();
  }
  async _sendLoop() {
    while (true) {
      await this._sendBufferedData.promise;
      if (!this._executing) {
        if (this._transportResult) {
          this._transportResult.reject("Connection stopped.");
        }
        break;
      }
      this._sendBufferedData = new PromiseSource();
      const transportResult = this._transportResult;
      this._transportResult = void 0;
      const data = typeof this._buffer[0] === "string" ? this._buffer.join("") : TransportSendQueue._concatBuffers(this._buffer);
      this._buffer.length = 0;
      try {
        await this._transport.send(data);
        transportResult.resolve();
      } catch (error) {
        transportResult.reject(error);
      }
    }
  }
  static _concatBuffers(arrayBuffers) {
    const totalLength = arrayBuffers.map((b) => b.byteLength).reduce((a, b) => a + b);
    const result = new Uint8Array(totalLength);
    let offset = 0;
    for (const item of arrayBuffers) {
      result.set(new Uint8Array(item), offset);
      offset += item.byteLength;
    }
    return result.buffer;
  }
};
var PromiseSource = class {
  constructor() {
    this.promise = new Promise((resolve, reject) => [this._resolver, this._rejecter] = [resolve, reject]);
  }
  resolve() {
    this._resolver();
  }
  reject(reason) {
    this._rejecter(reason);
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/JsonHubProtocol.js
var JSON_HUB_PROTOCOL_NAME = "json";
var JsonHubProtocol = class {
  constructor() {
    this.name = JSON_HUB_PROTOCOL_NAME;
    this.version = 1;
    this.transferFormat = TransferFormat.Text;
  }
  parseMessages(input, logger) {
    if (typeof input !== "string") {
      throw new Error("Invalid input for JSON hub protocol. Expected a string.");
    }
    if (!input) {
      return [];
    }
    if (logger === null) {
      logger = NullLogger.instance;
    }
    const messages = TextMessageFormat.parse(input);
    const hubMessages = [];
    for (const message of messages) {
      const parsedMessage = JSON.parse(message);
      if (typeof parsedMessage.type !== "number") {
        throw new Error("Invalid payload.");
      }
      switch (parsedMessage.type) {
        case MessageType.Invocation:
          this._isInvocationMessage(parsedMessage);
          break;
        case MessageType.StreamItem:
          this._isStreamItemMessage(parsedMessage);
          break;
        case MessageType.Completion:
          this._isCompletionMessage(parsedMessage);
          break;
        case MessageType.Ping:
          break;
        case MessageType.Close:
          break;
        default:
          logger.log(LogLevel.Information, "Unknown message type '" + parsedMessage.type + "' ignored.");
          continue;
      }
      hubMessages.push(parsedMessage);
    }
    return hubMessages;
  }
  writeMessage(message) {
    return TextMessageFormat.write(JSON.stringify(message));
  }
  _isInvocationMessage(message) {
    this._assertNotEmptyString(message.target, "Invalid payload for Invocation message.");
    if (message.invocationId !== void 0) {
      this._assertNotEmptyString(message.invocationId, "Invalid payload for Invocation message.");
    }
  }
  _isStreamItemMessage(message) {
    this._assertNotEmptyString(message.invocationId, "Invalid payload for StreamItem message.");
    if (message.item === void 0) {
      throw new Error("Invalid payload for StreamItem message.");
    }
  }
  _isCompletionMessage(message) {
    if (message.result && message.error) {
      throw new Error("Invalid payload for Completion message.");
    }
    if (!message.result && message.error) {
      this._assertNotEmptyString(message.error, "Invalid payload for Completion message.");
    }
    this._assertNotEmptyString(message.invocationId, "Invalid payload for Completion message.");
  }
  _assertNotEmptyString(value, errorMessage) {
    if (typeof value !== "string" || value === "") {
      throw new Error(errorMessage);
    }
  }
};

// ../../node_modules/@microsoft/signalr/dist/esm/HubConnectionBuilder.js
var LogLevelNameMapping = {
  trace: LogLevel.Trace,
  debug: LogLevel.Debug,
  info: LogLevel.Information,
  information: LogLevel.Information,
  warn: LogLevel.Warning,
  warning: LogLevel.Warning,
  error: LogLevel.Error,
  critical: LogLevel.Critical,
  none: LogLevel.None
};
function parseLogLevel(name) {
  const mapping = LogLevelNameMapping[name.toLowerCase()];
  if (typeof mapping !== "undefined") {
    return mapping;
  } else {
    throw new Error(`Unknown log level: ${name}`);
  }
}
var HubConnectionBuilder = class {
  configureLogging(logging2) {
    Arg.isRequired(logging2, "logging");
    if (isLogger(logging2)) {
      this.logger = logging2;
    } else if (typeof logging2 === "string") {
      const logLevel = parseLogLevel(logging2);
      this.logger = new ConsoleLogger(logLevel);
    } else {
      this.logger = new ConsoleLogger(logging2);
    }
    return this;
  }
  withUrl(url, transportTypeOrOptions) {
    Arg.isRequired(url, "url");
    Arg.isNotEmpty(url, "url");
    this.url = url;
    if (typeof transportTypeOrOptions === "object") {
      this.httpConnectionOptions = { ...this.httpConnectionOptions, ...transportTypeOrOptions };
    } else {
      this.httpConnectionOptions = {
        ...this.httpConnectionOptions,
        transport: transportTypeOrOptions
      };
    }
    return this;
  }
  withHubProtocol(protocol) {
    Arg.isRequired(protocol, "protocol");
    this.protocol = protocol;
    return this;
  }
  withAutomaticReconnect(retryDelaysOrReconnectPolicy) {
    if (this.reconnectPolicy) {
      throw new Error("A reconnectPolicy has already been set.");
    }
    if (!retryDelaysOrReconnectPolicy) {
      this.reconnectPolicy = new DefaultReconnectPolicy();
    } else if (Array.isArray(retryDelaysOrReconnectPolicy)) {
      this.reconnectPolicy = new DefaultReconnectPolicy(retryDelaysOrReconnectPolicy);
    } else {
      this.reconnectPolicy = retryDelaysOrReconnectPolicy;
    }
    return this;
  }
  build() {
    const httpConnectionOptions = this.httpConnectionOptions || {};
    if (httpConnectionOptions.logger === void 0) {
      httpConnectionOptions.logger = this.logger;
    }
    if (!this.url) {
      throw new Error("The 'HubConnectionBuilder.withUrl' method must be called before building the connection.");
    }
    const connection = new HttpConnection(this.url, httpConnectionOptions);
    return HubConnection.create(connection, this.logger || NullLogger.instance, this.protocol || new JsonHubProtocol(), this.reconnectPolicy);
  }
};
function isLogger(logger) {
  return logger.log !== void 0;
}

// ../../node_modules/webrtc-adapter/src/js/utils.js
var logDisabled_ = true;
var deprecationWarnings_ = true;
function extractVersion(uastring, expr, pos) {
  const match = uastring.match(expr);
  return match && match.length >= pos && parseInt(match[pos], 10);
}
function wrapPeerConnectionEvent(window2, eventNameToWrap, wrapper) {
  if (!window2.RTCPeerConnection) {
    return;
  }
  const proto = window2.RTCPeerConnection.prototype;
  const nativeAddEventListener = proto.addEventListener;
  proto.addEventListener = function(nativeEventName, cb) {
    if (nativeEventName !== eventNameToWrap) {
      return nativeAddEventListener.apply(this, arguments);
    }
    const wrappedCallback = (e2) => {
      const modifiedEvent = wrapper(e2);
      if (modifiedEvent) {
        if (cb.handleEvent) {
          cb.handleEvent(modifiedEvent);
        } else {
          cb(modifiedEvent);
        }
      }
    };
    this._eventMap = this._eventMap || {};
    if (!this._eventMap[eventNameToWrap]) {
      this._eventMap[eventNameToWrap] = /* @__PURE__ */ new Map();
    }
    this._eventMap[eventNameToWrap].set(cb, wrappedCallback);
    return nativeAddEventListener.apply(this, [
      nativeEventName,
      wrappedCallback
    ]);
  };
  const nativeRemoveEventListener = proto.removeEventListener;
  proto.removeEventListener = function(nativeEventName, cb) {
    if (nativeEventName !== eventNameToWrap || !this._eventMap || !this._eventMap[eventNameToWrap]) {
      return nativeRemoveEventListener.apply(this, arguments);
    }
    if (!this._eventMap[eventNameToWrap].has(cb)) {
      return nativeRemoveEventListener.apply(this, arguments);
    }
    const unwrappedCb = this._eventMap[eventNameToWrap].get(cb);
    this._eventMap[eventNameToWrap].delete(cb);
    if (this._eventMap[eventNameToWrap].size === 0) {
      delete this._eventMap[eventNameToWrap];
    }
    if (Object.keys(this._eventMap).length === 0) {
      delete this._eventMap;
    }
    return nativeRemoveEventListener.apply(this, [
      nativeEventName,
      unwrappedCb
    ]);
  };
  Object.defineProperty(proto, "on" + eventNameToWrap, {
    get() {
      return this["_on" + eventNameToWrap];
    },
    set(cb) {
      if (this["_on" + eventNameToWrap]) {
        this.removeEventListener(eventNameToWrap, this["_on" + eventNameToWrap]);
        delete this["_on" + eventNameToWrap];
      }
      if (cb) {
        this.addEventListener(eventNameToWrap, this["_on" + eventNameToWrap] = cb);
      }
    },
    enumerable: true,
    configurable: true
  });
}
function disableLog(bool) {
  if (typeof bool !== "boolean") {
    return new Error("Argument type: " + typeof bool + ". Please use a boolean.");
  }
  logDisabled_ = bool;
  return bool ? "adapter.js logging disabled" : "adapter.js logging enabled";
}
function disableWarnings(bool) {
  if (typeof bool !== "boolean") {
    return new Error("Argument type: " + typeof bool + ". Please use a boolean.");
  }
  deprecationWarnings_ = !bool;
  return "adapter.js deprecation warnings " + (bool ? "disabled" : "enabled");
}
function log() {
  if (typeof window === "object") {
    if (logDisabled_) {
      return;
    }
    if (typeof console !== "undefined" && typeof console.log === "function") {
      console.log.apply(console, arguments);
    }
  }
}
function deprecated(oldMethod, newMethod) {
  if (!deprecationWarnings_) {
    return;
  }
  console.warn(oldMethod + " is deprecated, please use " + newMethod + " instead.");
}
function detectBrowser(window2) {
  const result = { browser: null, version: null };
  if (typeof window2 === "undefined" || !window2.navigator) {
    result.browser = "Not a browser.";
    return result;
  }
  const { navigator: navigator2 } = window2;
  if (navigator2.mozGetUserMedia) {
    result.browser = "firefox";
    result.version = extractVersion(navigator2.userAgent, /Firefox\/(\d+)\./, 1);
  } else if (navigator2.webkitGetUserMedia || window2.isSecureContext === false && window2.webkitRTCPeerConnection && !window2.RTCIceGatherer) {
    result.browser = "chrome";
    result.version = extractVersion(navigator2.userAgent, /Chrom(e|ium)\/(\d+)\./, 2);
  } else if (window2.RTCPeerConnection && navigator2.userAgent.match(/AppleWebKit\/(\d+)\./)) {
    result.browser = "safari";
    result.version = extractVersion(navigator2.userAgent, /AppleWebKit\/(\d+)\./, 1);
    result.supportsUnifiedPlan = window2.RTCRtpTransceiver && "currentDirection" in window2.RTCRtpTransceiver.prototype;
  } else {
    result.browser = "Not a supported browser.";
    return result;
  }
  return result;
}
function isObject2(val) {
  return Object.prototype.toString.call(val) === "[object Object]";
}
function compactObject(data) {
  if (!isObject2(data)) {
    return data;
  }
  return Object.keys(data).reduce(function(accumulator, key) {
    const isObj = isObject2(data[key]);
    const value = isObj ? compactObject(data[key]) : data[key];
    const isEmptyObject = isObj && !Object.keys(value).length;
    if (value === void 0 || isEmptyObject) {
      return accumulator;
    }
    return Object.assign(accumulator, { [key]: value });
  }, {});
}
function walkStats(stats, base, resultSet) {
  if (!base || resultSet.has(base.id)) {
    return;
  }
  resultSet.set(base.id, base);
  Object.keys(base).forEach((name) => {
    if (name.endsWith("Id")) {
      walkStats(stats, stats.get(base[name]), resultSet);
    } else if (name.endsWith("Ids")) {
      base[name].forEach((id) => {
        walkStats(stats, stats.get(id), resultSet);
      });
    }
  });
}
function filterStats(result, track, outbound) {
  const streamStatsType = outbound ? "outbound-rtp" : "inbound-rtp";
  const filteredResult = /* @__PURE__ */ new Map();
  if (track === null) {
    return filteredResult;
  }
  const trackStats = [];
  result.forEach((value) => {
    if (value.type === "track" && value.trackIdentifier === track.id) {
      trackStats.push(value);
    }
  });
  trackStats.forEach((trackStat) => {
    result.forEach((stats) => {
      if (stats.type === streamStatsType && stats.trackId === trackStat.id) {
        walkStats(result, stats, filteredResult);
      }
    });
  });
  return filteredResult;
}

// ../../node_modules/webrtc-adapter/src/js/chrome/chrome_shim.js
var chrome_shim_exports = {};
__export(chrome_shim_exports, {
  fixNegotiationNeeded: () => fixNegotiationNeeded,
  shimAddTrackRemoveTrack: () => shimAddTrackRemoveTrack,
  shimAddTrackRemoveTrackWithNative: () => shimAddTrackRemoveTrackWithNative,
  shimGetDisplayMedia: () => shimGetDisplayMedia,
  shimGetSendersWithDtmf: () => shimGetSendersWithDtmf,
  shimGetStats: () => shimGetStats,
  shimGetUserMedia: () => shimGetUserMedia,
  shimMediaStream: () => shimMediaStream,
  shimOnTrack: () => shimOnTrack,
  shimPeerConnection: () => shimPeerConnection,
  shimSenderReceiverGetStats: () => shimSenderReceiverGetStats
});

// ../../node_modules/webrtc-adapter/src/js/chrome/getusermedia.js
var logging = log;
function shimGetUserMedia(window2, browserDetails) {
  const navigator2 = window2 && window2.navigator;
  if (!navigator2.mediaDevices) {
    return;
  }
  const constraintsToChrome_ = function(c) {
    if (typeof c !== "object" || c.mandatory || c.optional) {
      return c;
    }
    const cc = {};
    Object.keys(c).forEach((key) => {
      if (key === "require" || key === "advanced" || key === "mediaSource") {
        return;
      }
      const r = typeof c[key] === "object" ? c[key] : { ideal: c[key] };
      if (r.exact !== void 0 && typeof r.exact === "number") {
        r.min = r.max = r.exact;
      }
      const oldname_ = function(prefix, name) {
        if (prefix) {
          return prefix + name.charAt(0).toUpperCase() + name.slice(1);
        }
        return name === "deviceId" ? "sourceId" : name;
      };
      if (r.ideal !== void 0) {
        cc.optional = cc.optional || [];
        let oc = {};
        if (typeof r.ideal === "number") {
          oc[oldname_("min", key)] = r.ideal;
          cc.optional.push(oc);
          oc = {};
          oc[oldname_("max", key)] = r.ideal;
          cc.optional.push(oc);
        } else {
          oc[oldname_("", key)] = r.ideal;
          cc.optional.push(oc);
        }
      }
      if (r.exact !== void 0 && typeof r.exact !== "number") {
        cc.mandatory = cc.mandatory || {};
        cc.mandatory[oldname_("", key)] = r.exact;
      } else {
        ["min", "max"].forEach((mix) => {
          if (r[mix] !== void 0) {
            cc.mandatory = cc.mandatory || {};
            cc.mandatory[oldname_(mix, key)] = r[mix];
          }
        });
      }
    });
    if (c.advanced) {
      cc.optional = (cc.optional || []).concat(c.advanced);
    }
    return cc;
  };
  const shimConstraints_ = function(constraints, func) {
    if (browserDetails.version >= 61) {
      return func(constraints);
    }
    constraints = JSON.parse(JSON.stringify(constraints));
    if (constraints && typeof constraints.audio === "object") {
      const remap = function(obj2, a, b) {
        if (a in obj2 && !(b in obj2)) {
          obj2[b] = obj2[a];
          delete obj2[a];
        }
      };
      constraints = JSON.parse(JSON.stringify(constraints));
      remap(constraints.audio, "autoGainControl", "googAutoGainControl");
      remap(constraints.audio, "noiseSuppression", "googNoiseSuppression");
      constraints.audio = constraintsToChrome_(constraints.audio);
    }
    if (constraints && typeof constraints.video === "object") {
      let face = constraints.video.facingMode;
      face = face && (typeof face === "object" ? face : { ideal: face });
      const getSupportedFacingModeLies = browserDetails.version < 66;
      if (face && (face.exact === "user" || face.exact === "environment" || face.ideal === "user" || face.ideal === "environment") && !(navigator2.mediaDevices.getSupportedConstraints && navigator2.mediaDevices.getSupportedConstraints().facingMode && !getSupportedFacingModeLies)) {
        delete constraints.video.facingMode;
        let matches;
        if (face.exact === "environment" || face.ideal === "environment") {
          matches = ["back", "rear"];
        } else if (face.exact === "user" || face.ideal === "user") {
          matches = ["front"];
        }
        if (matches) {
          return navigator2.mediaDevices.enumerateDevices().then((devices) => {
            devices = devices.filter((d) => d.kind === "videoinput");
            let dev = devices.find((d) => matches.some((match) => d.label.toLowerCase().includes(match)));
            if (!dev && devices.length && matches.includes("back")) {
              dev = devices[devices.length - 1];
            }
            if (dev) {
              constraints.video.deviceId = face.exact ? { exact: dev.deviceId } : { ideal: dev.deviceId };
            }
            constraints.video = constraintsToChrome_(constraints.video);
            logging("chrome: " + JSON.stringify(constraints));
            return func(constraints);
          });
        }
      }
      constraints.video = constraintsToChrome_(constraints.video);
    }
    logging("chrome: " + JSON.stringify(constraints));
    return func(constraints);
  };
  const shimError_ = function(e2) {
    if (browserDetails.version >= 64) {
      return e2;
    }
    return {
      name: {
        PermissionDeniedError: "NotAllowedError",
        PermissionDismissedError: "NotAllowedError",
        InvalidStateError: "NotAllowedError",
        DevicesNotFoundError: "NotFoundError",
        ConstraintNotSatisfiedError: "OverconstrainedError",
        TrackStartError: "NotReadableError",
        MediaDeviceFailedDueToShutdown: "NotAllowedError",
        MediaDeviceKillSwitchOn: "NotAllowedError",
        TabCaptureError: "AbortError",
        ScreenCaptureError: "AbortError",
        DeviceCaptureError: "AbortError"
      }[e2.name] || e2.name,
      message: e2.message,
      constraint: e2.constraint || e2.constraintName,
      toString() {
        return this.name + (this.message && ": ") + this.message;
      }
    };
  };
  const getUserMedia_ = function(constraints, onSuccess, onError) {
    shimConstraints_(constraints, (c) => {
      navigator2.webkitGetUserMedia(c, onSuccess, (e2) => {
        if (onError) {
          onError(shimError_(e2));
        }
      });
    });
  };
  navigator2.getUserMedia = getUserMedia_.bind(navigator2);
  if (navigator2.mediaDevices.getUserMedia) {
    const origGetUserMedia = navigator2.mediaDevices.getUserMedia.bind(navigator2.mediaDevices);
    navigator2.mediaDevices.getUserMedia = function(cs) {
      return shimConstraints_(cs, (c) => origGetUserMedia(c).then((stream) => {
        if (c.audio && !stream.getAudioTracks().length || c.video && !stream.getVideoTracks().length) {
          stream.getTracks().forEach((track) => {
            track.stop();
          });
          throw new DOMException("", "NotFoundError");
        }
        return stream;
      }, (e2) => Promise.reject(shimError_(e2))));
    };
  }
}

// ../../node_modules/webrtc-adapter/src/js/chrome/getdisplaymedia.js
function shimGetDisplayMedia(window2, getSourceId) {
  if (window2.navigator.mediaDevices && "getDisplayMedia" in window2.navigator.mediaDevices) {
    return;
  }
  if (!window2.navigator.mediaDevices) {
    return;
  }
  if (typeof getSourceId !== "function") {
    console.error("shimGetDisplayMedia: getSourceId argument is not a function");
    return;
  }
  window2.navigator.mediaDevices.getDisplayMedia = function getDisplayMedia(constraints) {
    return getSourceId(constraints).then((sourceId) => {
      const widthSpecified = constraints.video && constraints.video.width;
      const heightSpecified = constraints.video && constraints.video.height;
      const frameRateSpecified = constraints.video && constraints.video.frameRate;
      constraints.video = {
        mandatory: {
          chromeMediaSource: "desktop",
          chromeMediaSourceId: sourceId,
          maxFrameRate: frameRateSpecified || 3
        }
      };
      if (widthSpecified) {
        constraints.video.mandatory.maxWidth = widthSpecified;
      }
      if (heightSpecified) {
        constraints.video.mandatory.maxHeight = heightSpecified;
      }
      return window2.navigator.mediaDevices.getUserMedia(constraints);
    });
  };
}

// ../../node_modules/webrtc-adapter/src/js/chrome/chrome_shim.js
function shimMediaStream(window2) {
  window2.MediaStream = window2.MediaStream || window2.webkitMediaStream;
}
function shimOnTrack(window2) {
  if (typeof window2 === "object" && window2.RTCPeerConnection && !("ontrack" in window2.RTCPeerConnection.prototype)) {
    Object.defineProperty(window2.RTCPeerConnection.prototype, "ontrack", {
      get() {
        return this._ontrack;
      },
      set(f) {
        if (this._ontrack) {
          this.removeEventListener("track", this._ontrack);
        }
        this.addEventListener("track", this._ontrack = f);
      },
      enumerable: true,
      configurable: true
    });
    const origSetRemoteDescription = window2.RTCPeerConnection.prototype.setRemoteDescription;
    window2.RTCPeerConnection.prototype.setRemoteDescription = function setRemoteDescription() {
      if (!this._ontrackpoly) {
        this._ontrackpoly = (e2) => {
          e2.stream.addEventListener("addtrack", (te) => {
            let receiver;
            if (window2.RTCPeerConnection.prototype.getReceivers) {
              receiver = this.getReceivers().find((r) => r.track && r.track.id === te.track.id);
            } else {
              receiver = { track: te.track };
            }
            const event = new Event("track");
            event.track = te.track;
            event.receiver = receiver;
            event.transceiver = { receiver };
            event.streams = [e2.stream];
            this.dispatchEvent(event);
          });
          e2.stream.getTracks().forEach((track) => {
            let receiver;
            if (window2.RTCPeerConnection.prototype.getReceivers) {
              receiver = this.getReceivers().find((r) => r.track && r.track.id === track.id);
            } else {
              receiver = { track };
            }
            const event = new Event("track");
            event.track = track;
            event.receiver = receiver;
            event.transceiver = { receiver };
            event.streams = [e2.stream];
            this.dispatchEvent(event);
          });
        };
        this.addEventListener("addstream", this._ontrackpoly);
      }
      return origSetRemoteDescription.apply(this, arguments);
    };
  } else {
    wrapPeerConnectionEvent(window2, "track", (e2) => {
      if (!e2.transceiver) {
        Object.defineProperty(e2, "transceiver", { value: { receiver: e2.receiver } });
      }
      return e2;
    });
  }
}
function shimGetSendersWithDtmf(window2) {
  if (typeof window2 === "object" && window2.RTCPeerConnection && !("getSenders" in window2.RTCPeerConnection.prototype) && "createDTMFSender" in window2.RTCPeerConnection.prototype) {
    const shimSenderWithDtmf = function(pc, track) {
      return {
        track,
        get dtmf() {
          if (this._dtmf === void 0) {
            if (track.kind === "audio") {
              this._dtmf = pc.createDTMFSender(track);
            } else {
              this._dtmf = null;
            }
          }
          return this._dtmf;
        },
        _pc: pc
      };
    };
    if (!window2.RTCPeerConnection.prototype.getSenders) {
      window2.RTCPeerConnection.prototype.getSenders = function getSenders() {
        this._senders = this._senders || [];
        return this._senders.slice();
      };
      const origAddTrack = window2.RTCPeerConnection.prototype.addTrack;
      window2.RTCPeerConnection.prototype.addTrack = function addTrack(track, stream) {
        let sender = origAddTrack.apply(this, arguments);
        if (!sender) {
          sender = shimSenderWithDtmf(this, track);
          this._senders.push(sender);
        }
        return sender;
      };
      const origRemoveTrack = window2.RTCPeerConnection.prototype.removeTrack;
      window2.RTCPeerConnection.prototype.removeTrack = function removeTrack(sender) {
        origRemoveTrack.apply(this, arguments);
        const idx = this._senders.indexOf(sender);
        if (idx !== -1) {
          this._senders.splice(idx, 1);
        }
      };
    }
    const origAddStream = window2.RTCPeerConnection.prototype.addStream;
    window2.RTCPeerConnection.prototype.addStream = function addStream(stream) {
      this._senders = this._senders || [];
      origAddStream.apply(this, [stream]);
      stream.getTracks().forEach((track) => {
        this._senders.push(shimSenderWithDtmf(this, track));
      });
    };
    const origRemoveStream = window2.RTCPeerConnection.prototype.removeStream;
    window2.RTCPeerConnection.prototype.removeStream = function removeStream(stream) {
      this._senders = this._senders || [];
      origRemoveStream.apply(this, [stream]);
      stream.getTracks().forEach((track) => {
        const sender = this._senders.find((s) => s.track === track);
        if (sender) {
          this._senders.splice(this._senders.indexOf(sender), 1);
        }
      });
    };
  } else if (typeof window2 === "object" && window2.RTCPeerConnection && "getSenders" in window2.RTCPeerConnection.prototype && "createDTMFSender" in window2.RTCPeerConnection.prototype && window2.RTCRtpSender && !("dtmf" in window2.RTCRtpSender.prototype)) {
    const origGetSenders = window2.RTCPeerConnection.prototype.getSenders;
    window2.RTCPeerConnection.prototype.getSenders = function getSenders() {
      const senders = origGetSenders.apply(this, []);
      senders.forEach((sender) => sender._pc = this);
      return senders;
    };
    Object.defineProperty(window2.RTCRtpSender.prototype, "dtmf", {
      get() {
        if (this._dtmf === void 0) {
          if (this.track.kind === "audio") {
            this._dtmf = this._pc.createDTMFSender(this.track);
          } else {
            this._dtmf = null;
          }
        }
        return this._dtmf;
      }
    });
  }
}
function shimGetStats(window2) {
  if (!window2.RTCPeerConnection) {
    return;
  }
  const origGetStats = window2.RTCPeerConnection.prototype.getStats;
  window2.RTCPeerConnection.prototype.getStats = function getStats() {
    const [selector, onSucc, onErr] = arguments;
    if (arguments.length > 0 && typeof selector === "function") {
      return origGetStats.apply(this, arguments);
    }
    if (origGetStats.length === 0 && (arguments.length === 0 || typeof selector !== "function")) {
      return origGetStats.apply(this, []);
    }
    const fixChromeStats_ = function(response) {
      const standardReport = {};
      const reports = response.result();
      reports.forEach((report) => {
        const standardStats = {
          id: report.id,
          timestamp: report.timestamp,
          type: {
            localcandidate: "local-candidate",
            remotecandidate: "remote-candidate"
          }[report.type] || report.type
        };
        report.names().forEach((name) => {
          standardStats[name] = report.stat(name);
        });
        standardReport[standardStats.id] = standardStats;
      });
      return standardReport;
    };
    const makeMapStats = function(stats) {
      return new Map(Object.keys(stats).map((key) => [key, stats[key]]));
    };
    if (arguments.length >= 2) {
      const successCallbackWrapper_ = function(response) {
        onSucc(makeMapStats(fixChromeStats_(response)));
      };
      return origGetStats.apply(this, [
        successCallbackWrapper_,
        selector
      ]);
    }
    return new Promise((resolve, reject) => {
      origGetStats.apply(this, [
        function(response) {
          resolve(makeMapStats(fixChromeStats_(response)));
        },
        reject
      ]);
    }).then(onSucc, onErr);
  };
}
function shimSenderReceiverGetStats(window2) {
  if (!(typeof window2 === "object" && window2.RTCPeerConnection && window2.RTCRtpSender && window2.RTCRtpReceiver)) {
    return;
  }
  if (!("getStats" in window2.RTCRtpSender.prototype)) {
    const origGetSenders = window2.RTCPeerConnection.prototype.getSenders;
    if (origGetSenders) {
      window2.RTCPeerConnection.prototype.getSenders = function getSenders() {
        const senders = origGetSenders.apply(this, []);
        senders.forEach((sender) => sender._pc = this);
        return senders;
      };
    }
    const origAddTrack = window2.RTCPeerConnection.prototype.addTrack;
    if (origAddTrack) {
      window2.RTCPeerConnection.prototype.addTrack = function addTrack() {
        const sender = origAddTrack.apply(this, arguments);
        sender._pc = this;
        return sender;
      };
    }
    window2.RTCRtpSender.prototype.getStats = function getStats() {
      const sender = this;
      return this._pc.getStats().then((result) => filterStats(result, sender.track, true));
    };
  }
  if (!("getStats" in window2.RTCRtpReceiver.prototype)) {
    const origGetReceivers = window2.RTCPeerConnection.prototype.getReceivers;
    if (origGetReceivers) {
      window2.RTCPeerConnection.prototype.getReceivers = function getReceivers() {
        const receivers = origGetReceivers.apply(this, []);
        receivers.forEach((receiver) => receiver._pc = this);
        return receivers;
      };
    }
    wrapPeerConnectionEvent(window2, "track", (e2) => {
      e2.receiver._pc = e2.srcElement;
      return e2;
    });
    window2.RTCRtpReceiver.prototype.getStats = function getStats() {
      const receiver = this;
      return this._pc.getStats().then((result) => filterStats(result, receiver.track, false));
    };
  }
  if (!("getStats" in window2.RTCRtpSender.prototype && "getStats" in window2.RTCRtpReceiver.prototype)) {
    return;
  }
  const origGetStats = window2.RTCPeerConnection.prototype.getStats;
  window2.RTCPeerConnection.prototype.getStats = function getStats() {
    if (arguments.length > 0 && arguments[0] instanceof window2.MediaStreamTrack) {
      const track = arguments[0];
      let sender;
      let receiver;
      let err;
      this.getSenders().forEach((s) => {
        if (s.track === track) {
          if (sender) {
            err = true;
          } else {
            sender = s;
          }
        }
      });
      this.getReceivers().forEach((r) => {
        if (r.track === track) {
          if (receiver) {
            err = true;
          } else {
            receiver = r;
          }
        }
        return r.track === track;
      });
      if (err || sender && receiver) {
        return Promise.reject(new DOMException("There are more than one sender or receiver for the track.", "InvalidAccessError"));
      } else if (sender) {
        return sender.getStats();
      } else if (receiver) {
        return receiver.getStats();
      }
      return Promise.reject(new DOMException("There is no sender or receiver for the track.", "InvalidAccessError"));
    }
    return origGetStats.apply(this, arguments);
  };
}
function shimAddTrackRemoveTrackWithNative(window2) {
  window2.RTCPeerConnection.prototype.getLocalStreams = function getLocalStreams() {
    this._shimmedLocalStreams = this._shimmedLocalStreams || {};
    return Object.keys(this._shimmedLocalStreams).map((streamId) => this._shimmedLocalStreams[streamId][0]);
  };
  const origAddTrack = window2.RTCPeerConnection.prototype.addTrack;
  window2.RTCPeerConnection.prototype.addTrack = function addTrack(track, stream) {
    if (!stream) {
      return origAddTrack.apply(this, arguments);
    }
    this._shimmedLocalStreams = this._shimmedLocalStreams || {};
    const sender = origAddTrack.apply(this, arguments);
    if (!this._shimmedLocalStreams[stream.id]) {
      this._shimmedLocalStreams[stream.id] = [stream, sender];
    } else if (this._shimmedLocalStreams[stream.id].indexOf(sender) === -1) {
      this._shimmedLocalStreams[stream.id].push(sender);
    }
    return sender;
  };
  const origAddStream = window2.RTCPeerConnection.prototype.addStream;
  window2.RTCPeerConnection.prototype.addStream = function addStream(stream) {
    this._shimmedLocalStreams = this._shimmedLocalStreams || {};
    stream.getTracks().forEach((track) => {
      const alreadyExists = this.getSenders().find((s) => s.track === track);
      if (alreadyExists) {
        throw new DOMException("Track already exists.", "InvalidAccessError");
      }
    });
    const existingSenders = this.getSenders();
    origAddStream.apply(this, arguments);
    const newSenders = this.getSenders().filter((newSender) => existingSenders.indexOf(newSender) === -1);
    this._shimmedLocalStreams[stream.id] = [stream].concat(newSenders);
  };
  const origRemoveStream = window2.RTCPeerConnection.prototype.removeStream;
  window2.RTCPeerConnection.prototype.removeStream = function removeStream(stream) {
    this._shimmedLocalStreams = this._shimmedLocalStreams || {};
    delete this._shimmedLocalStreams[stream.id];
    return origRemoveStream.apply(this, arguments);
  };
  const origRemoveTrack = window2.RTCPeerConnection.prototype.removeTrack;
  window2.RTCPeerConnection.prototype.removeTrack = function removeTrack(sender) {
    this._shimmedLocalStreams = this._shimmedLocalStreams || {};
    if (sender) {
      Object.keys(this._shimmedLocalStreams).forEach((streamId) => {
        const idx = this._shimmedLocalStreams[streamId].indexOf(sender);
        if (idx !== -1) {
          this._shimmedLocalStreams[streamId].splice(idx, 1);
        }
        if (this._shimmedLocalStreams[streamId].length === 1) {
          delete this._shimmedLocalStreams[streamId];
        }
      });
    }
    return origRemoveTrack.apply(this, arguments);
  };
}
function shimAddTrackRemoveTrack(window2, browserDetails) {
  if (!window2.RTCPeerConnection) {
    return;
  }
  if (window2.RTCPeerConnection.prototype.addTrack && browserDetails.version >= 65) {
    return shimAddTrackRemoveTrackWithNative(window2);
  }
  const origGetLocalStreams = window2.RTCPeerConnection.prototype.getLocalStreams;
  window2.RTCPeerConnection.prototype.getLocalStreams = function getLocalStreams() {
    const nativeStreams = origGetLocalStreams.apply(this);
    this._reverseStreams = this._reverseStreams || {};
    return nativeStreams.map((stream) => this._reverseStreams[stream.id]);
  };
  const origAddStream = window2.RTCPeerConnection.prototype.addStream;
  window2.RTCPeerConnection.prototype.addStream = function addStream(stream) {
    this._streams = this._streams || {};
    this._reverseStreams = this._reverseStreams || {};
    stream.getTracks().forEach((track) => {
      const alreadyExists = this.getSenders().find((s) => s.track === track);
      if (alreadyExists) {
        throw new DOMException("Track already exists.", "InvalidAccessError");
      }
    });
    if (!this._reverseStreams[stream.id]) {
      const newStream = new window2.MediaStream(stream.getTracks());
      this._streams[stream.id] = newStream;
      this._reverseStreams[newStream.id] = stream;
      stream = newStream;
    }
    origAddStream.apply(this, [stream]);
  };
  const origRemoveStream = window2.RTCPeerConnection.prototype.removeStream;
  window2.RTCPeerConnection.prototype.removeStream = function removeStream(stream) {
    this._streams = this._streams || {};
    this._reverseStreams = this._reverseStreams || {};
    origRemoveStream.apply(this, [this._streams[stream.id] || stream]);
    delete this._reverseStreams[this._streams[stream.id] ? this._streams[stream.id].id : stream.id];
    delete this._streams[stream.id];
  };
  window2.RTCPeerConnection.prototype.addTrack = function addTrack(track, stream) {
    if (this.signalingState === "closed") {
      throw new DOMException("The RTCPeerConnection's signalingState is 'closed'.", "InvalidStateError");
    }
    const streams = [].slice.call(arguments, 1);
    if (streams.length !== 1 || !streams[0].getTracks().find((t2) => t2 === track)) {
      throw new DOMException("The adapter.js addTrack polyfill only supports a single  stream which is associated with the specified track.", "NotSupportedError");
    }
    const alreadyExists = this.getSenders().find((s) => s.track === track);
    if (alreadyExists) {
      throw new DOMException("Track already exists.", "InvalidAccessError");
    }
    this._streams = this._streams || {};
    this._reverseStreams = this._reverseStreams || {};
    const oldStream = this._streams[stream.id];
    if (oldStream) {
      oldStream.addTrack(track);
      Promise.resolve().then(() => {
        this.dispatchEvent(new Event("negotiationneeded"));
      });
    } else {
      const newStream = new window2.MediaStream([track]);
      this._streams[stream.id] = newStream;
      this._reverseStreams[newStream.id] = stream;
      this.addStream(newStream);
    }
    return this.getSenders().find((s) => s.track === track);
  };
  function replaceInternalStreamId(pc, description) {
    let sdp2 = description.sdp;
    Object.keys(pc._reverseStreams || []).forEach((internalId) => {
      const externalStream = pc._reverseStreams[internalId];
      const internalStream = pc._streams[externalStream.id];
      sdp2 = sdp2.replace(new RegExp(internalStream.id, "g"), externalStream.id);
    });
    return new RTCSessionDescription({
      type: description.type,
      sdp: sdp2
    });
  }
  function replaceExternalStreamId(pc, description) {
    let sdp2 = description.sdp;
    Object.keys(pc._reverseStreams || []).forEach((internalId) => {
      const externalStream = pc._reverseStreams[internalId];
      const internalStream = pc._streams[externalStream.id];
      sdp2 = sdp2.replace(new RegExp(externalStream.id, "g"), internalStream.id);
    });
    return new RTCSessionDescription({
      type: description.type,
      sdp: sdp2
    });
  }
  ["createOffer", "createAnswer"].forEach(function(method) {
    const nativeMethod = window2.RTCPeerConnection.prototype[method];
    const methodObj = { [method]() {
      const args = arguments;
      const isLegacyCall = arguments.length && typeof arguments[0] === "function";
      if (isLegacyCall) {
        return nativeMethod.apply(this, [
          (description) => {
            const desc = replaceInternalStreamId(this, description);
            args[0].apply(null, [desc]);
          },
          (err) => {
            if (args[1]) {
              args[1].apply(null, err);
            }
          },
          arguments[2]
        ]);
      }
      return nativeMethod.apply(this, arguments).then((description) => replaceInternalStreamId(this, description));
    } };
    window2.RTCPeerConnection.prototype[method] = methodObj[method];
  });
  const origSetLocalDescription = window2.RTCPeerConnection.prototype.setLocalDescription;
  window2.RTCPeerConnection.prototype.setLocalDescription = function setLocalDescription() {
    if (!arguments.length || !arguments[0].type) {
      return origSetLocalDescription.apply(this, arguments);
    }
    arguments[0] = replaceExternalStreamId(this, arguments[0]);
    return origSetLocalDescription.apply(this, arguments);
  };
  const origLocalDescription = Object.getOwnPropertyDescriptor(window2.RTCPeerConnection.prototype, "localDescription");
  Object.defineProperty(window2.RTCPeerConnection.prototype, "localDescription", {
    get() {
      const description = origLocalDescription.get.apply(this);
      if (description.type === "") {
        return description;
      }
      return replaceInternalStreamId(this, description);
    }
  });
  window2.RTCPeerConnection.prototype.removeTrack = function removeTrack(sender) {
    if (this.signalingState === "closed") {
      throw new DOMException("The RTCPeerConnection's signalingState is 'closed'.", "InvalidStateError");
    }
    if (!sender._pc) {
      throw new DOMException("Argument 1 of RTCPeerConnection.removeTrack does not implement interface RTCRtpSender.", "TypeError");
    }
    const isLocal = sender._pc === this;
    if (!isLocal) {
      throw new DOMException("Sender was not created by this connection.", "InvalidAccessError");
    }
    this._streams = this._streams || {};
    let stream;
    Object.keys(this._streams).forEach((streamid) => {
      const hasTrack = this._streams[streamid].getTracks().find((track) => sender.track === track);
      if (hasTrack) {
        stream = this._streams[streamid];
      }
    });
    if (stream) {
      if (stream.getTracks().length === 1) {
        this.removeStream(this._reverseStreams[stream.id]);
      } else {
        stream.removeTrack(sender.track);
      }
      this.dispatchEvent(new Event("negotiationneeded"));
    }
  };
}
function shimPeerConnection(window2, browserDetails) {
  if (!window2.RTCPeerConnection && window2.webkitRTCPeerConnection) {
    window2.RTCPeerConnection = window2.webkitRTCPeerConnection;
  }
  if (!window2.RTCPeerConnection) {
    return;
  }
  if (browserDetails.version < 53) {
    ["setLocalDescription", "setRemoteDescription", "addIceCandidate"].forEach(function(method) {
      const nativeMethod = window2.RTCPeerConnection.prototype[method];
      const methodObj = { [method]() {
        arguments[0] = new (method === "addIceCandidate" ? window2.RTCIceCandidate : window2.RTCSessionDescription)(arguments[0]);
        return nativeMethod.apply(this, arguments);
      } };
      window2.RTCPeerConnection.prototype[method] = methodObj[method];
    });
  }
}
function fixNegotiationNeeded(window2, browserDetails) {
  wrapPeerConnectionEvent(window2, "negotiationneeded", (e2) => {
    const pc = e2.target;
    if (browserDetails.version < 72 || pc.getConfiguration && pc.getConfiguration().sdpSemantics === "plan-b") {
      if (pc.signalingState !== "stable") {
        return;
      }
    }
    return e2;
  });
}

// ../../node_modules/webrtc-adapter/src/js/firefox/firefox_shim.js
var firefox_shim_exports = {};
__export(firefox_shim_exports, {
  shimAddTransceiver: () => shimAddTransceiver,
  shimCreateAnswer: () => shimCreateAnswer,
  shimCreateOffer: () => shimCreateOffer,
  shimGetDisplayMedia: () => shimGetDisplayMedia2,
  shimGetParameters: () => shimGetParameters,
  shimGetUserMedia: () => shimGetUserMedia2,
  shimOnTrack: () => shimOnTrack2,
  shimPeerConnection: () => shimPeerConnection2,
  shimRTCDataChannel: () => shimRTCDataChannel,
  shimReceiverGetStats: () => shimReceiverGetStats,
  shimRemoveStream: () => shimRemoveStream,
  shimSenderGetStats: () => shimSenderGetStats
});

// ../../node_modules/webrtc-adapter/src/js/firefox/getusermedia.js
function shimGetUserMedia2(window2, browserDetails) {
  const navigator2 = window2 && window2.navigator;
  const MediaStreamTrack = window2 && window2.MediaStreamTrack;
  navigator2.getUserMedia = function(constraints, onSuccess, onError) {
    deprecated("navigator.getUserMedia", "navigator.mediaDevices.getUserMedia");
    navigator2.mediaDevices.getUserMedia(constraints).then(onSuccess, onError);
  };
  if (!(browserDetails.version > 55 && "autoGainControl" in navigator2.mediaDevices.getSupportedConstraints())) {
    const remap = function(obj2, a, b) {
      if (a in obj2 && !(b in obj2)) {
        obj2[b] = obj2[a];
        delete obj2[a];
      }
    };
    const nativeGetUserMedia = navigator2.mediaDevices.getUserMedia.bind(navigator2.mediaDevices);
    navigator2.mediaDevices.getUserMedia = function(c) {
      if (typeof c === "object" && typeof c.audio === "object") {
        c = JSON.parse(JSON.stringify(c));
        remap(c.audio, "autoGainControl", "mozAutoGainControl");
        remap(c.audio, "noiseSuppression", "mozNoiseSuppression");
      }
      return nativeGetUserMedia(c);
    };
    if (MediaStreamTrack && MediaStreamTrack.prototype.getSettings) {
      const nativeGetSettings = MediaStreamTrack.prototype.getSettings;
      MediaStreamTrack.prototype.getSettings = function() {
        const obj2 = nativeGetSettings.apply(this, arguments);
        remap(obj2, "mozAutoGainControl", "autoGainControl");
        remap(obj2, "mozNoiseSuppression", "noiseSuppression");
        return obj2;
      };
    }
    if (MediaStreamTrack && MediaStreamTrack.prototype.applyConstraints) {
      const nativeApplyConstraints = MediaStreamTrack.prototype.applyConstraints;
      MediaStreamTrack.prototype.applyConstraints = function(c) {
        if (this.kind === "audio" && typeof c === "object") {
          c = JSON.parse(JSON.stringify(c));
          remap(c, "autoGainControl", "mozAutoGainControl");
          remap(c, "noiseSuppression", "mozNoiseSuppression");
        }
        return nativeApplyConstraints.apply(this, [c]);
      };
    }
  }
}

// ../../node_modules/webrtc-adapter/src/js/firefox/getdisplaymedia.js
function shimGetDisplayMedia2(window2, preferredMediaSource) {
  if (window2.navigator.mediaDevices && "getDisplayMedia" in window2.navigator.mediaDevices) {
    return;
  }
  if (!window2.navigator.mediaDevices) {
    return;
  }
  window2.navigator.mediaDevices.getDisplayMedia = function getDisplayMedia(constraints) {
    if (!(constraints && constraints.video)) {
      const err = new DOMException("getDisplayMedia without video constraints is undefined");
      err.name = "NotFoundError";
      err.code = 8;
      return Promise.reject(err);
    }
    if (constraints.video === true) {
      constraints.video = { mediaSource: preferredMediaSource };
    } else {
      constraints.video.mediaSource = preferredMediaSource;
    }
    return window2.navigator.mediaDevices.getUserMedia(constraints);
  };
}

// ../../node_modules/webrtc-adapter/src/js/firefox/firefox_shim.js
function shimOnTrack2(window2) {
  if (typeof window2 === "object" && window2.RTCTrackEvent && "receiver" in window2.RTCTrackEvent.prototype && !("transceiver" in window2.RTCTrackEvent.prototype)) {
    Object.defineProperty(window2.RTCTrackEvent.prototype, "transceiver", {
      get() {
        return { receiver: this.receiver };
      }
    });
  }
}
function shimPeerConnection2(window2, browserDetails) {
  if (typeof window2 !== "object" || !(window2.RTCPeerConnection || window2.mozRTCPeerConnection)) {
    return;
  }
  if (!window2.RTCPeerConnection && window2.mozRTCPeerConnection) {
    window2.RTCPeerConnection = window2.mozRTCPeerConnection;
  }
  if (browserDetails.version < 53) {
    ["setLocalDescription", "setRemoteDescription", "addIceCandidate"].forEach(function(method) {
      const nativeMethod = window2.RTCPeerConnection.prototype[method];
      const methodObj = { [method]() {
        arguments[0] = new (method === "addIceCandidate" ? window2.RTCIceCandidate : window2.RTCSessionDescription)(arguments[0]);
        return nativeMethod.apply(this, arguments);
      } };
      window2.RTCPeerConnection.prototype[method] = methodObj[method];
    });
  }
  const modernStatsTypes = {
    inboundrtp: "inbound-rtp",
    outboundrtp: "outbound-rtp",
    candidatepair: "candidate-pair",
    localcandidate: "local-candidate",
    remotecandidate: "remote-candidate"
  };
  const nativeGetStats = window2.RTCPeerConnection.prototype.getStats;
  window2.RTCPeerConnection.prototype.getStats = function getStats() {
    const [selector, onSucc, onErr] = arguments;
    return nativeGetStats.apply(this, [selector || null]).then((stats) => {
      if (browserDetails.version < 53 && !onSucc) {
        try {
          stats.forEach((stat) => {
            stat.type = modernStatsTypes[stat.type] || stat.type;
          });
        } catch (e2) {
          if (e2.name !== "TypeError") {
            throw e2;
          }
          stats.forEach((stat, i) => {
            stats.set(i, Object.assign({}, stat, {
              type: modernStatsTypes[stat.type] || stat.type
            }));
          });
        }
      }
      return stats;
    }).then(onSucc, onErr);
  };
}
function shimSenderGetStats(window2) {
  if (!(typeof window2 === "object" && window2.RTCPeerConnection && window2.RTCRtpSender)) {
    return;
  }
  if (window2.RTCRtpSender && "getStats" in window2.RTCRtpSender.prototype) {
    return;
  }
  const origGetSenders = window2.RTCPeerConnection.prototype.getSenders;
  if (origGetSenders) {
    window2.RTCPeerConnection.prototype.getSenders = function getSenders() {
      const senders = origGetSenders.apply(this, []);
      senders.forEach((sender) => sender._pc = this);
      return senders;
    };
  }
  const origAddTrack = window2.RTCPeerConnection.prototype.addTrack;
  if (origAddTrack) {
    window2.RTCPeerConnection.prototype.addTrack = function addTrack() {
      const sender = origAddTrack.apply(this, arguments);
      sender._pc = this;
      return sender;
    };
  }
  window2.RTCRtpSender.prototype.getStats = function getStats() {
    return this.track ? this._pc.getStats(this.track) : Promise.resolve(/* @__PURE__ */ new Map());
  };
}
function shimReceiverGetStats(window2) {
  if (!(typeof window2 === "object" && window2.RTCPeerConnection && window2.RTCRtpSender)) {
    return;
  }
  if (window2.RTCRtpSender && "getStats" in window2.RTCRtpReceiver.prototype) {
    return;
  }
  const origGetReceivers = window2.RTCPeerConnection.prototype.getReceivers;
  if (origGetReceivers) {
    window2.RTCPeerConnection.prototype.getReceivers = function getReceivers() {
      const receivers = origGetReceivers.apply(this, []);
      receivers.forEach((receiver) => receiver._pc = this);
      return receivers;
    };
  }
  wrapPeerConnectionEvent(window2, "track", (e2) => {
    e2.receiver._pc = e2.srcElement;
    return e2;
  });
  window2.RTCRtpReceiver.prototype.getStats = function getStats() {
    return this._pc.getStats(this.track);
  };
}
function shimRemoveStream(window2) {
  if (!window2.RTCPeerConnection || "removeStream" in window2.RTCPeerConnection.prototype) {
    return;
  }
  window2.RTCPeerConnection.prototype.removeStream = function removeStream(stream) {
    deprecated("removeStream", "removeTrack");
    this.getSenders().forEach((sender) => {
      if (sender.track && stream.getTracks().includes(sender.track)) {
        this.removeTrack(sender);
      }
    });
  };
}
function shimRTCDataChannel(window2) {
  if (window2.DataChannel && !window2.RTCDataChannel) {
    window2.RTCDataChannel = window2.DataChannel;
  }
}
function shimAddTransceiver(window2) {
  if (!(typeof window2 === "object" && window2.RTCPeerConnection)) {
    return;
  }
  const origAddTransceiver = window2.RTCPeerConnection.prototype.addTransceiver;
  if (origAddTransceiver) {
    window2.RTCPeerConnection.prototype.addTransceiver = function addTransceiver() {
      this.setParametersPromises = [];
      const initParameters = arguments[1];
      const shouldPerformCheck = initParameters && "sendEncodings" in initParameters;
      if (shouldPerformCheck) {
        initParameters.sendEncodings.forEach((encodingParam) => {
          if ("rid" in encodingParam) {
            const ridRegex = /^[a-z0-9]{0,16}$/i;
            if (!ridRegex.test(encodingParam.rid)) {
              throw new TypeError("Invalid RID value provided.");
            }
          }
          if ("scaleResolutionDownBy" in encodingParam) {
            if (!(parseFloat(encodingParam.scaleResolutionDownBy) >= 1)) {
              throw new RangeError("scale_resolution_down_by must be >= 1.0");
            }
          }
          if ("maxFramerate" in encodingParam) {
            if (!(parseFloat(encodingParam.maxFramerate) >= 0)) {
              throw new RangeError("max_framerate must be >= 0.0");
            }
          }
        });
      }
      const transceiver = origAddTransceiver.apply(this, arguments);
      if (shouldPerformCheck) {
        const { sender } = transceiver;
        const params = sender.getParameters();
        if (!("encodings" in params) || params.encodings.length === 1 && Object.keys(params.encodings[0]).length === 0) {
          params.encodings = initParameters.sendEncodings;
          sender.sendEncodings = initParameters.sendEncodings;
          this.setParametersPromises.push(sender.setParameters(params).then(() => {
            delete sender.sendEncodings;
          }).catch(() => {
            delete sender.sendEncodings;
          }));
        }
      }
      return transceiver;
    };
  }
}
function shimGetParameters(window2) {
  if (!(typeof window2 === "object" && window2.RTCRtpSender)) {
    return;
  }
  const origGetParameters = window2.RTCRtpSender.prototype.getParameters;
  if (origGetParameters) {
    window2.RTCRtpSender.prototype.getParameters = function getParameters() {
      const params = origGetParameters.apply(this, arguments);
      if (!("encodings" in params)) {
        params.encodings = [].concat(this.sendEncodings || [{}]);
      }
      return params;
    };
  }
}
function shimCreateOffer(window2) {
  if (!(typeof window2 === "object" && window2.RTCPeerConnection)) {
    return;
  }
  const origCreateOffer = window2.RTCPeerConnection.prototype.createOffer;
  window2.RTCPeerConnection.prototype.createOffer = function createOffer() {
    if (this.setParametersPromises && this.setParametersPromises.length) {
      return Promise.all(this.setParametersPromises).then(() => {
        return origCreateOffer.apply(this, arguments);
      }).finally(() => {
        this.setParametersPromises = [];
      });
    }
    return origCreateOffer.apply(this, arguments);
  };
}
function shimCreateAnswer(window2) {
  if (!(typeof window2 === "object" && window2.RTCPeerConnection)) {
    return;
  }
  const origCreateAnswer = window2.RTCPeerConnection.prototype.createAnswer;
  window2.RTCPeerConnection.prototype.createAnswer = function createAnswer() {
    if (this.setParametersPromises && this.setParametersPromises.length) {
      return Promise.all(this.setParametersPromises).then(() => {
        return origCreateAnswer.apply(this, arguments);
      }).finally(() => {
        this.setParametersPromises = [];
      });
    }
    return origCreateAnswer.apply(this, arguments);
  };
}

// ../../node_modules/webrtc-adapter/src/js/safari/safari_shim.js
var safari_shim_exports = {};
__export(safari_shim_exports, {
  shimAudioContext: () => shimAudioContext,
  shimCallbacksAPI: () => shimCallbacksAPI,
  shimConstraints: () => shimConstraints,
  shimCreateOfferLegacy: () => shimCreateOfferLegacy,
  shimGetUserMedia: () => shimGetUserMedia3,
  shimLocalStreamsAPI: () => shimLocalStreamsAPI,
  shimRTCIceServerUrls: () => shimRTCIceServerUrls,
  shimRemoteStreamsAPI: () => shimRemoteStreamsAPI,
  shimTrackEventTransceiver: () => shimTrackEventTransceiver
});
function shimLocalStreamsAPI(window2) {
  if (typeof window2 !== "object" || !window2.RTCPeerConnection) {
    return;
  }
  if (!("getLocalStreams" in window2.RTCPeerConnection.prototype)) {
    window2.RTCPeerConnection.prototype.getLocalStreams = function getLocalStreams() {
      if (!this._localStreams) {
        this._localStreams = [];
      }
      return this._localStreams;
    };
  }
  if (!("addStream" in window2.RTCPeerConnection.prototype)) {
    const _addTrack = window2.RTCPeerConnection.prototype.addTrack;
    window2.RTCPeerConnection.prototype.addStream = function addStream(stream) {
      if (!this._localStreams) {
        this._localStreams = [];
      }
      if (!this._localStreams.includes(stream)) {
        this._localStreams.push(stream);
      }
      stream.getAudioTracks().forEach((track) => _addTrack.call(this, track, stream));
      stream.getVideoTracks().forEach((track) => _addTrack.call(this, track, stream));
    };
    window2.RTCPeerConnection.prototype.addTrack = function addTrack(track, ...streams) {
      if (streams) {
        streams.forEach((stream) => {
          if (!this._localStreams) {
            this._localStreams = [stream];
          } else if (!this._localStreams.includes(stream)) {
            this._localStreams.push(stream);
          }
        });
      }
      return _addTrack.apply(this, arguments);
    };
  }
  if (!("removeStream" in window2.RTCPeerConnection.prototype)) {
    window2.RTCPeerConnection.prototype.removeStream = function removeStream(stream) {
      if (!this._localStreams) {
        this._localStreams = [];
      }
      const index = this._localStreams.indexOf(stream);
      if (index === -1) {
        return;
      }
      this._localStreams.splice(index, 1);
      const tracks = stream.getTracks();
      this.getSenders().forEach((sender) => {
        if (tracks.includes(sender.track)) {
          this.removeTrack(sender);
        }
      });
    };
  }
}
function shimRemoteStreamsAPI(window2) {
  if (typeof window2 !== "object" || !window2.RTCPeerConnection) {
    return;
  }
  if (!("getRemoteStreams" in window2.RTCPeerConnection.prototype)) {
    window2.RTCPeerConnection.prototype.getRemoteStreams = function getRemoteStreams() {
      return this._remoteStreams ? this._remoteStreams : [];
    };
  }
  if (!("onaddstream" in window2.RTCPeerConnection.prototype)) {
    Object.defineProperty(window2.RTCPeerConnection.prototype, "onaddstream", {
      get() {
        return this._onaddstream;
      },
      set(f) {
        if (this._onaddstream) {
          this.removeEventListener("addstream", this._onaddstream);
          this.removeEventListener("track", this._onaddstreampoly);
        }
        this.addEventListener("addstream", this._onaddstream = f);
        this.addEventListener("track", this._onaddstreampoly = (e2) => {
          e2.streams.forEach((stream) => {
            if (!this._remoteStreams) {
              this._remoteStreams = [];
            }
            if (this._remoteStreams.includes(stream)) {
              return;
            }
            this._remoteStreams.push(stream);
            const event = new Event("addstream");
            event.stream = stream;
            this.dispatchEvent(event);
          });
        });
      }
    });
    const origSetRemoteDescription = window2.RTCPeerConnection.prototype.setRemoteDescription;
    window2.RTCPeerConnection.prototype.setRemoteDescription = function setRemoteDescription() {
      const pc = this;
      if (!this._onaddstreampoly) {
        this.addEventListener("track", this._onaddstreampoly = function(e2) {
          e2.streams.forEach((stream) => {
            if (!pc._remoteStreams) {
              pc._remoteStreams = [];
            }
            if (pc._remoteStreams.indexOf(stream) >= 0) {
              return;
            }
            pc._remoteStreams.push(stream);
            const event = new Event("addstream");
            event.stream = stream;
            pc.dispatchEvent(event);
          });
        });
      }
      return origSetRemoteDescription.apply(pc, arguments);
    };
  }
}
function shimCallbacksAPI(window2) {
  if (typeof window2 !== "object" || !window2.RTCPeerConnection) {
    return;
  }
  const prototype = window2.RTCPeerConnection.prototype;
  const origCreateOffer = prototype.createOffer;
  const origCreateAnswer = prototype.createAnswer;
  const setLocalDescription = prototype.setLocalDescription;
  const setRemoteDescription = prototype.setRemoteDescription;
  const addIceCandidate = prototype.addIceCandidate;
  prototype.createOffer = function createOffer(successCallback, failureCallback) {
    const options = arguments.length >= 2 ? arguments[2] : arguments[0];
    const promise = origCreateOffer.apply(this, [options]);
    if (!failureCallback) {
      return promise;
    }
    promise.then(successCallback, failureCallback);
    return Promise.resolve();
  };
  prototype.createAnswer = function createAnswer(successCallback, failureCallback) {
    const options = arguments.length >= 2 ? arguments[2] : arguments[0];
    const promise = origCreateAnswer.apply(this, [options]);
    if (!failureCallback) {
      return promise;
    }
    promise.then(successCallback, failureCallback);
    return Promise.resolve();
  };
  let withCallback = function(description, successCallback, failureCallback) {
    const promise = setLocalDescription.apply(this, [description]);
    if (!failureCallback) {
      return promise;
    }
    promise.then(successCallback, failureCallback);
    return Promise.resolve();
  };
  prototype.setLocalDescription = withCallback;
  withCallback = function(description, successCallback, failureCallback) {
    const promise = setRemoteDescription.apply(this, [description]);
    if (!failureCallback) {
      return promise;
    }
    promise.then(successCallback, failureCallback);
    return Promise.resolve();
  };
  prototype.setRemoteDescription = withCallback;
  withCallback = function(candidate, successCallback, failureCallback) {
    const promise = addIceCandidate.apply(this, [candidate]);
    if (!failureCallback) {
      return promise;
    }
    promise.then(successCallback, failureCallback);
    return Promise.resolve();
  };
  prototype.addIceCandidate = withCallback;
}
function shimGetUserMedia3(window2) {
  const navigator2 = window2 && window2.navigator;
  if (navigator2.mediaDevices && navigator2.mediaDevices.getUserMedia) {
    const mediaDevices = navigator2.mediaDevices;
    const _getUserMedia = mediaDevices.getUserMedia.bind(mediaDevices);
    navigator2.mediaDevices.getUserMedia = (constraints) => {
      return _getUserMedia(shimConstraints(constraints));
    };
  }
  if (!navigator2.getUserMedia && navigator2.mediaDevices && navigator2.mediaDevices.getUserMedia) {
    navigator2.getUserMedia = function getUserMedia(constraints, cb, errcb) {
      navigator2.mediaDevices.getUserMedia(constraints).then(cb, errcb);
    }.bind(navigator2);
  }
}
function shimConstraints(constraints) {
  if (constraints && constraints.video !== void 0) {
    return Object.assign({}, constraints, { video: compactObject(constraints.video) });
  }
  return constraints;
}
function shimRTCIceServerUrls(window2) {
  if (!window2.RTCPeerConnection) {
    return;
  }
  const OrigPeerConnection = window2.RTCPeerConnection;
  window2.RTCPeerConnection = function RTCPeerConnection2(pcConfig, pcConstraints) {
    if (pcConfig && pcConfig.iceServers) {
      const newIceServers = [];
      for (let i = 0; i < pcConfig.iceServers.length; i++) {
        let server = pcConfig.iceServers[i];
        if (!server.hasOwnProperty("urls") && server.hasOwnProperty("url")) {
          deprecated("RTCIceServer.url", "RTCIceServer.urls");
          server = JSON.parse(JSON.stringify(server));
          server.urls = server.url;
          delete server.url;
          newIceServers.push(server);
        } else {
          newIceServers.push(pcConfig.iceServers[i]);
        }
      }
      pcConfig.iceServers = newIceServers;
    }
    return new OrigPeerConnection(pcConfig, pcConstraints);
  };
  window2.RTCPeerConnection.prototype = OrigPeerConnection.prototype;
  if ("generateCertificate" in OrigPeerConnection) {
    Object.defineProperty(window2.RTCPeerConnection, "generateCertificate", {
      get() {
        return OrigPeerConnection.generateCertificate;
      }
    });
  }
}
function shimTrackEventTransceiver(window2) {
  if (typeof window2 === "object" && window2.RTCTrackEvent && "receiver" in window2.RTCTrackEvent.prototype && !("transceiver" in window2.RTCTrackEvent.prototype)) {
    Object.defineProperty(window2.RTCTrackEvent.prototype, "transceiver", {
      get() {
        return { receiver: this.receiver };
      }
    });
  }
}
function shimCreateOfferLegacy(window2) {
  const origCreateOffer = window2.RTCPeerConnection.prototype.createOffer;
  window2.RTCPeerConnection.prototype.createOffer = function createOffer(offerOptions) {
    if (offerOptions) {
      if (typeof offerOptions.offerToReceiveAudio !== "undefined") {
        offerOptions.offerToReceiveAudio = !!offerOptions.offerToReceiveAudio;
      }
      const audioTransceiver = this.getTransceivers().find((transceiver) => transceiver.receiver.track.kind === "audio");
      if (offerOptions.offerToReceiveAudio === false && audioTransceiver) {
        if (audioTransceiver.direction === "sendrecv") {
          if (audioTransceiver.setDirection) {
            audioTransceiver.setDirection("sendonly");
          } else {
            audioTransceiver.direction = "sendonly";
          }
        } else if (audioTransceiver.direction === "recvonly") {
          if (audioTransceiver.setDirection) {
            audioTransceiver.setDirection("inactive");
          } else {
            audioTransceiver.direction = "inactive";
          }
        }
      } else if (offerOptions.offerToReceiveAudio === true && !audioTransceiver) {
        this.addTransceiver("audio", { direction: "recvonly" });
      }
      if (typeof offerOptions.offerToReceiveVideo !== "undefined") {
        offerOptions.offerToReceiveVideo = !!offerOptions.offerToReceiveVideo;
      }
      const videoTransceiver = this.getTransceivers().find((transceiver) => transceiver.receiver.track.kind === "video");
      if (offerOptions.offerToReceiveVideo === false && videoTransceiver) {
        if (videoTransceiver.direction === "sendrecv") {
          if (videoTransceiver.setDirection) {
            videoTransceiver.setDirection("sendonly");
          } else {
            videoTransceiver.direction = "sendonly";
          }
        } else if (videoTransceiver.direction === "recvonly") {
          if (videoTransceiver.setDirection) {
            videoTransceiver.setDirection("inactive");
          } else {
            videoTransceiver.direction = "inactive";
          }
        }
      } else if (offerOptions.offerToReceiveVideo === true && !videoTransceiver) {
        this.addTransceiver("video", { direction: "recvonly" });
      }
    }
    return origCreateOffer.apply(this, arguments);
  };
}
function shimAudioContext(window2) {
  if (typeof window2 !== "object" || window2.AudioContext) {
    return;
  }
  window2.AudioContext = window2.webkitAudioContext;
}

// ../../node_modules/webrtc-adapter/src/js/common_shim.js
var common_shim_exports = {};
__export(common_shim_exports, {
  removeExtmapAllowMixed: () => removeExtmapAllowMixed,
  shimAddIceCandidateNullOrEmpty: () => shimAddIceCandidateNullOrEmpty,
  shimConnectionState: () => shimConnectionState,
  shimMaxMessageSize: () => shimMaxMessageSize,
  shimParameterlessSetLocalDescription: () => shimParameterlessSetLocalDescription,
  shimRTCIceCandidate: () => shimRTCIceCandidate,
  shimSendThrowTypeError: () => shimSendThrowTypeError
});
var import_sdp = __toESM(require_sdp());
function shimRTCIceCandidate(window2) {
  if (!window2.RTCIceCandidate || window2.RTCIceCandidate && "foundation" in window2.RTCIceCandidate.prototype) {
    return;
  }
  const NativeRTCIceCandidate = window2.RTCIceCandidate;
  window2.RTCIceCandidate = function RTCIceCandidate(args) {
    if (typeof args === "object" && args.candidate && args.candidate.indexOf("a=") === 0) {
      args = JSON.parse(JSON.stringify(args));
      args.candidate = args.candidate.substr(2);
    }
    if (args.candidate && args.candidate.length) {
      const nativeCandidate = new NativeRTCIceCandidate(args);
      const parsedCandidate = import_sdp.default.parseCandidate(args.candidate);
      const augmentedCandidate = Object.assign(nativeCandidate, parsedCandidate);
      augmentedCandidate.toJSON = function toJSON() {
        return {
          candidate: augmentedCandidate.candidate,
          sdpMid: augmentedCandidate.sdpMid,
          sdpMLineIndex: augmentedCandidate.sdpMLineIndex,
          usernameFragment: augmentedCandidate.usernameFragment
        };
      };
      return augmentedCandidate;
    }
    return new NativeRTCIceCandidate(args);
  };
  window2.RTCIceCandidate.prototype = NativeRTCIceCandidate.prototype;
  wrapPeerConnectionEvent(window2, "icecandidate", (e2) => {
    if (e2.candidate) {
      Object.defineProperty(e2, "candidate", {
        value: new window2.RTCIceCandidate(e2.candidate),
        writable: "false"
      });
    }
    return e2;
  });
}
function shimMaxMessageSize(window2, browserDetails) {
  if (!window2.RTCPeerConnection) {
    return;
  }
  if (!("sctp" in window2.RTCPeerConnection.prototype)) {
    Object.defineProperty(window2.RTCPeerConnection.prototype, "sctp", {
      get() {
        return typeof this._sctp === "undefined" ? null : this._sctp;
      }
    });
  }
  const sctpInDescription = function(description) {
    if (!description || !description.sdp) {
      return false;
    }
    const sections = import_sdp.default.splitSections(description.sdp);
    sections.shift();
    return sections.some((mediaSection) => {
      const mLine = import_sdp.default.parseMLine(mediaSection);
      return mLine && mLine.kind === "application" && mLine.protocol.indexOf("SCTP") !== -1;
    });
  };
  const getRemoteFirefoxVersion = function(description) {
    const match = description.sdp.match(/mozilla...THIS_IS_SDPARTA-(\d+)/);
    if (match === null || match.length < 2) {
      return -1;
    }
    const version = parseInt(match[1], 10);
    return version !== version ? -1 : version;
  };
  const getCanSendMaxMessageSize = function(remoteIsFirefox) {
    let canSendMaxMessageSize = 65536;
    if (browserDetails.browser === "firefox") {
      if (browserDetails.version < 57) {
        if (remoteIsFirefox === -1) {
          canSendMaxMessageSize = 16384;
        } else {
          canSendMaxMessageSize = 2147483637;
        }
      } else if (browserDetails.version < 60) {
        canSendMaxMessageSize = browserDetails.version === 57 ? 65535 : 65536;
      } else {
        canSendMaxMessageSize = 2147483637;
      }
    }
    return canSendMaxMessageSize;
  };
  const getMaxMessageSize = function(description, remoteIsFirefox) {
    let maxMessageSize = 65536;
    if (browserDetails.browser === "firefox" && browserDetails.version === 57) {
      maxMessageSize = 65535;
    }
    const match = import_sdp.default.matchPrefix(description.sdp, "a=max-message-size:");
    if (match.length > 0) {
      maxMessageSize = parseInt(match[0].substr(19), 10);
    } else if (browserDetails.browser === "firefox" && remoteIsFirefox !== -1) {
      maxMessageSize = 2147483637;
    }
    return maxMessageSize;
  };
  const origSetRemoteDescription = window2.RTCPeerConnection.prototype.setRemoteDescription;
  window2.RTCPeerConnection.prototype.setRemoteDescription = function setRemoteDescription() {
    this._sctp = null;
    if (browserDetails.browser === "chrome" && browserDetails.version >= 76) {
      const { sdpSemantics } = this.getConfiguration();
      if (sdpSemantics === "plan-b") {
        Object.defineProperty(this, "sctp", {
          get() {
            return typeof this._sctp === "undefined" ? null : this._sctp;
          },
          enumerable: true,
          configurable: true
        });
      }
    }
    if (sctpInDescription(arguments[0])) {
      const isFirefox = getRemoteFirefoxVersion(arguments[0]);
      const canSendMMS = getCanSendMaxMessageSize(isFirefox);
      const remoteMMS = getMaxMessageSize(arguments[0], isFirefox);
      let maxMessageSize;
      if (canSendMMS === 0 && remoteMMS === 0) {
        maxMessageSize = Number.POSITIVE_INFINITY;
      } else if (canSendMMS === 0 || remoteMMS === 0) {
        maxMessageSize = Math.max(canSendMMS, remoteMMS);
      } else {
        maxMessageSize = Math.min(canSendMMS, remoteMMS);
      }
      const sctp = {};
      Object.defineProperty(sctp, "maxMessageSize", {
        get() {
          return maxMessageSize;
        }
      });
      this._sctp = sctp;
    }
    return origSetRemoteDescription.apply(this, arguments);
  };
}
function shimSendThrowTypeError(window2) {
  if (!(window2.RTCPeerConnection && "createDataChannel" in window2.RTCPeerConnection.prototype)) {
    return;
  }
  function wrapDcSend(dc, pc) {
    const origDataChannelSend = dc.send;
    dc.send = function send() {
      const data = arguments[0];
      const length2 = data.length || data.size || data.byteLength;
      if (dc.readyState === "open" && pc.sctp && length2 > pc.sctp.maxMessageSize) {
        throw new TypeError("Message too large (can send a maximum of " + pc.sctp.maxMessageSize + " bytes)");
      }
      return origDataChannelSend.apply(dc, arguments);
    };
  }
  const origCreateDataChannel = window2.RTCPeerConnection.prototype.createDataChannel;
  window2.RTCPeerConnection.prototype.createDataChannel = function createDataChannel() {
    const dataChannel = origCreateDataChannel.apply(this, arguments);
    wrapDcSend(dataChannel, this);
    return dataChannel;
  };
  wrapPeerConnectionEvent(window2, "datachannel", (e2) => {
    wrapDcSend(e2.channel, e2.target);
    return e2;
  });
}
function shimConnectionState(window2) {
  if (!window2.RTCPeerConnection || "connectionState" in window2.RTCPeerConnection.prototype) {
    return;
  }
  const proto = window2.RTCPeerConnection.prototype;
  Object.defineProperty(proto, "connectionState", {
    get() {
      return {
        completed: "connected",
        checking: "connecting"
      }[this.iceConnectionState] || this.iceConnectionState;
    },
    enumerable: true,
    configurable: true
  });
  Object.defineProperty(proto, "onconnectionstatechange", {
    get() {
      return this._onconnectionstatechange || null;
    },
    set(cb) {
      if (this._onconnectionstatechange) {
        this.removeEventListener("connectionstatechange", this._onconnectionstatechange);
        delete this._onconnectionstatechange;
      }
      if (cb) {
        this.addEventListener("connectionstatechange", this._onconnectionstatechange = cb);
      }
    },
    enumerable: true,
    configurable: true
  });
  ["setLocalDescription", "setRemoteDescription"].forEach((method) => {
    const origMethod = proto[method];
    proto[method] = function() {
      if (!this._connectionstatechangepoly) {
        this._connectionstatechangepoly = (e2) => {
          const pc = e2.target;
          if (pc._lastConnectionState !== pc.connectionState) {
            pc._lastConnectionState = pc.connectionState;
            const newEvent = new Event("connectionstatechange", e2);
            pc.dispatchEvent(newEvent);
          }
          return e2;
        };
        this.addEventListener("iceconnectionstatechange", this._connectionstatechangepoly);
      }
      return origMethod.apply(this, arguments);
    };
  });
}
function removeExtmapAllowMixed(window2, browserDetails) {
  if (!window2.RTCPeerConnection) {
    return;
  }
  if (browserDetails.browser === "chrome" && browserDetails.version >= 71) {
    return;
  }
  if (browserDetails.browser === "safari" && browserDetails.version >= 605) {
    return;
  }
  const nativeSRD = window2.RTCPeerConnection.prototype.setRemoteDescription;
  window2.RTCPeerConnection.prototype.setRemoteDescription = function setRemoteDescription(desc) {
    if (desc && desc.sdp && desc.sdp.indexOf("\na=extmap-allow-mixed") !== -1) {
      const sdp2 = desc.sdp.split("\n").filter((line) => {
        return line.trim() !== "a=extmap-allow-mixed";
      }).join("\n");
      if (window2.RTCSessionDescription && desc instanceof window2.RTCSessionDescription) {
        arguments[0] = new window2.RTCSessionDescription({
          type: desc.type,
          sdp: sdp2
        });
      } else {
        desc.sdp = sdp2;
      }
    }
    return nativeSRD.apply(this, arguments);
  };
}
function shimAddIceCandidateNullOrEmpty(window2, browserDetails) {
  if (!(window2.RTCPeerConnection && window2.RTCPeerConnection.prototype)) {
    return;
  }
  const nativeAddIceCandidate = window2.RTCPeerConnection.prototype.addIceCandidate;
  if (!nativeAddIceCandidate || nativeAddIceCandidate.length === 0) {
    return;
  }
  window2.RTCPeerConnection.prototype.addIceCandidate = function addIceCandidate() {
    if (!arguments[0]) {
      if (arguments[1]) {
        arguments[1].apply(null);
      }
      return Promise.resolve();
    }
    if ((browserDetails.browser === "chrome" && browserDetails.version < 78 || browserDetails.browser === "firefox" && browserDetails.version < 68 || browserDetails.browser === "safari") && arguments[0] && arguments[0].candidate === "") {
      return Promise.resolve();
    }
    return nativeAddIceCandidate.apply(this, arguments);
  };
}
function shimParameterlessSetLocalDescription(window2, browserDetails) {
  if (!(window2.RTCPeerConnection && window2.RTCPeerConnection.prototype)) {
    return;
  }
  const nativeSetLocalDescription = window2.RTCPeerConnection.prototype.setLocalDescription;
  if (!nativeSetLocalDescription || nativeSetLocalDescription.length === 0) {
    return;
  }
  window2.RTCPeerConnection.prototype.setLocalDescription = function setLocalDescription() {
    let desc = arguments[0] || {};
    if (typeof desc !== "object" || desc.type && desc.sdp) {
      return nativeSetLocalDescription.apply(this, arguments);
    }
    desc = { type: desc.type, sdp: desc.sdp };
    if (!desc.type) {
      switch (this.signalingState) {
        case "stable":
        case "have-local-offer":
        case "have-remote-pranswer":
          desc.type = "offer";
          break;
        default:
          desc.type = "answer";
          break;
      }
    }
    if (desc.sdp || desc.type !== "offer" && desc.type !== "answer") {
      return nativeSetLocalDescription.apply(this, [desc]);
    }
    const func = desc.type === "offer" ? this.createOffer : this.createAnswer;
    return func.apply(this).then((d) => nativeSetLocalDescription.apply(this, [d]));
  };
}

// ../../node_modules/webrtc-adapter/src/js/adapter_factory.js
var sdp = __toESM(require_sdp());
function adapterFactory({ window: window2 } = {}, options = {
  shimChrome: true,
  shimFirefox: true,
  shimSafari: true
}) {
  const logging2 = log;
  const browserDetails = detectBrowser(window2);
  const adapter2 = {
    browserDetails,
    commonShim: common_shim_exports,
    extractVersion,
    disableLog,
    disableWarnings,
    sdp
  };
  switch (browserDetails.browser) {
    case "chrome":
      if (!chrome_shim_exports || !shimPeerConnection || !options.shimChrome) {
        logging2("Chrome shim is not included in this adapter release.");
        return adapter2;
      }
      if (browserDetails.version === null) {
        logging2("Chrome shim can not determine version, not shimming.");
        return adapter2;
      }
      logging2("adapter.js shimming chrome.");
      adapter2.browserShim = chrome_shim_exports;
      shimAddIceCandidateNullOrEmpty(window2, browserDetails);
      shimParameterlessSetLocalDescription(window2, browserDetails);
      shimGetUserMedia(window2, browserDetails);
      shimMediaStream(window2, browserDetails);
      shimPeerConnection(window2, browserDetails);
      shimOnTrack(window2, browserDetails);
      shimAddTrackRemoveTrack(window2, browserDetails);
      shimGetSendersWithDtmf(window2, browserDetails);
      shimGetStats(window2, browserDetails);
      shimSenderReceiverGetStats(window2, browserDetails);
      fixNegotiationNeeded(window2, browserDetails);
      shimRTCIceCandidate(window2, browserDetails);
      shimConnectionState(window2, browserDetails);
      shimMaxMessageSize(window2, browserDetails);
      shimSendThrowTypeError(window2, browserDetails);
      removeExtmapAllowMixed(window2, browserDetails);
      break;
    case "firefox":
      if (!firefox_shim_exports || !shimPeerConnection2 || !options.shimFirefox) {
        logging2("Firefox shim is not included in this adapter release.");
        return adapter2;
      }
      logging2("adapter.js shimming firefox.");
      adapter2.browserShim = firefox_shim_exports;
      shimAddIceCandidateNullOrEmpty(window2, browserDetails);
      shimParameterlessSetLocalDescription(window2, browserDetails);
      shimGetUserMedia2(window2, browserDetails);
      shimPeerConnection2(window2, browserDetails);
      shimOnTrack2(window2, browserDetails);
      shimRemoveStream(window2, browserDetails);
      shimSenderGetStats(window2, browserDetails);
      shimReceiverGetStats(window2, browserDetails);
      shimRTCDataChannel(window2, browserDetails);
      shimAddTransceiver(window2, browserDetails);
      shimGetParameters(window2, browserDetails);
      shimCreateOffer(window2, browserDetails);
      shimCreateAnswer(window2, browserDetails);
      shimRTCIceCandidate(window2, browserDetails);
      shimConnectionState(window2, browserDetails);
      shimMaxMessageSize(window2, browserDetails);
      shimSendThrowTypeError(window2, browserDetails);
      break;
    case "safari":
      if (!safari_shim_exports || !options.shimSafari) {
        logging2("Safari shim is not included in this adapter release.");
        return adapter2;
      }
      logging2("adapter.js shimming safari.");
      adapter2.browserShim = safari_shim_exports;
      shimAddIceCandidateNullOrEmpty(window2, browserDetails);
      shimParameterlessSetLocalDescription(window2, browserDetails);
      shimRTCIceServerUrls(window2, browserDetails);
      shimCreateOfferLegacy(window2, browserDetails);
      shimCallbacksAPI(window2, browserDetails);
      shimLocalStreamsAPI(window2, browserDetails);
      shimRemoteStreamsAPI(window2, browserDetails);
      shimTrackEventTransceiver(window2, browserDetails);
      shimGetUserMedia3(window2, browserDetails);
      shimAudioContext(window2, browserDetails);
      shimRTCIceCandidate(window2, browserDetails);
      shimMaxMessageSize(window2, browserDetails);
      shimSendThrowTypeError(window2, browserDetails);
      removeExtmapAllowMixed(window2, browserDetails);
      break;
    default:
      logging2("Unsupported browser!");
      break;
  }
  return adapter2;
}

// ../../node_modules/webrtc-adapter/src/js/adapter_core.js
var adapter = adapterFactory({ window: typeof window === "undefined" ? void 0 : window });
var adapter_core_default = adapter;

// ../audio/Pose.ts
var Pose = class {
  t = 0;
  p = vec3_exports.create();
  f = vec3_exports.set(vec3_exports.create(), 0, 0, -1);
  u = vec3_exports.set(vec3_exports.create(), 0, 1, 0);
  o = vec3_exports.create();
  constructor() {
    Object.seal(this);
  }
  set(px, py, pz, fx, fy, fz, ux, uy, uz) {
    this.setPosition(px, py, pz);
    this.setOrientation(fx, fy, fz, ux, uy, uz);
  }
  setPosition(px, py, pz) {
    vec3_exports.set(this.p, px, py, pz);
  }
  setOrientation(fx, fy, fz, ux, uy, uz) {
    vec3_exports.set(this.f, fx, fy, fz);
    vec3_exports.set(this.u, ux, uy, uz);
  }
  setOffset(ox, oy, oz) {
    vec3_exports.set(this.o, ox, oy, oz);
  }
  copy(other) {
    vec3_exports.copy(this.p, other.p);
    vec3_exports.copy(this.f, other.f);
    vec3_exports.copy(this.u, other.u);
    vec3_exports.copy(this.o, other.o);
  }
};

// ../tslib/events/PointerName.ts
var PointerName = /* @__PURE__ */ ((PointerName2) => {
  PointerName2[PointerName2["LocalUser"] = 0] = "LocalUser";
  PointerName2[PointerName2["Mouse"] = 1] = "Mouse";
  PointerName2[PointerName2["Pen"] = 2] = "Pen";
  PointerName2[PointerName2["Touch0"] = 3] = "Touch0";
  PointerName2[PointerName2["Touch1"] = 4] = "Touch1";
  PointerName2[PointerName2["Touch2"] = 5] = "Touch2";
  PointerName2[PointerName2["Touch3"] = 6] = "Touch3";
  PointerName2[PointerName2["Touch4"] = 7] = "Touch4";
  PointerName2[PointerName2["Touch5"] = 8] = "Touch5";
  PointerName2[PointerName2["Touch6"] = 9] = "Touch6";
  PointerName2[PointerName2["Touch7"] = 10] = "Touch7";
  PointerName2[PointerName2["Touch8"] = 11] = "Touch8";
  PointerName2[PointerName2["Touch9"] = 12] = "Touch9";
  PointerName2[PointerName2["Touch10"] = 13] = "Touch10";
  PointerName2[PointerName2["Touches"] = 14] = "Touches";
  PointerName2[PointerName2["MotionController"] = 15] = "MotionController";
  PointerName2[PointerName2["MotionControllerLeft"] = 16] = "MotionControllerLeft";
  PointerName2[PointerName2["MotionControllerRight"] = 17] = "MotionControllerRight";
  PointerName2[PointerName2["RemoteUser"] = 18] = "RemoteUser";
  return PointerName2;
})(PointerName || {});

// ../webrtc/ConferenceEvents.ts
var ConferenceEvent = class extends Event {
  constructor(eventType) {
    super(eventType);
    this.eventType = eventType;
  }
};
var ConferenceErrorEvent = class extends ConferenceEvent {
  constructor(error) {
    super("error");
    this.error = error;
  }
};
var ConferenceServerConnectedEvent = class extends ConferenceEvent {
  constructor() {
    super("serverConnected");
  }
};
var ConferenceServerDisconnectedEvent = class extends ConferenceEvent {
  constructor() {
    super("serverDisconnected");
  }
};
var LocalUserEvent = class extends ConferenceEvent {
  constructor(type2, userID) {
    super(type2);
    this.userID = userID;
  }
};
var RoomJoinedEvent = class extends LocalUserEvent {
  constructor(userID, pose) {
    super("roomJoined", userID);
    this.pose = pose;
  }
};
var RoomLeftEvent = class extends LocalUserEvent {
  constructor(userID) {
    super("roomLeft", userID);
  }
};
var RemoteUserEvent = class extends ConferenceEvent {
  constructor(type2, user) {
    super(type2);
    this.user = user;
  }
};
var UserJoinedEvent = class extends RemoteUserEvent {
  constructor(user, source) {
    super("userJoined", user);
    this.source = source;
  }
};
var UserLeftEvent = class extends RemoteUserEvent {
  constructor(user) {
    super("userLeft", user);
  }
};
var UserMutedEvent = class extends RemoteUserEvent {
  constructor(type2, user, muted2) {
    super(type2, user);
    this.muted = muted2;
  }
};
var UserAudioMutedEvent = class extends UserMutedEvent {
  constructor(user, muted2) {
    super("audioMuteStatusChanged", user, muted2);
  }
};
var UserVideoMutedEvent = class extends UserMutedEvent {
  constructor(user, muted2) {
    super("videoMuteStatusChanged", user, muted2);
  }
};
var UserStreamEvent = class extends RemoteUserEvent {
  constructor(type2, kind, op, user, stream) {
    super(type2, user);
    this.kind = kind;
    this.op = op;
    this.stream = stream;
  }
};
var UserStreamAddedEvent = class extends UserStreamEvent {
  constructor(type2, kind, user, stream) {
    super(type2, kind, "added" /* Added */, user, stream);
  }
};
var UserStreamRemovedEvent = class extends UserStreamEvent {
  constructor(type2, kind, user, stream) {
    super(type2, kind, "removed" /* Removed */, user, stream);
  }
};
var UserAudioStreamAddedEvent = class extends UserStreamAddedEvent {
  constructor(user, stream) {
    super("audioAdded", "audio" /* Audio */, user, stream);
  }
};
var UserAudioStreamRemovedEvent = class extends UserStreamRemovedEvent {
  constructor(user, stream) {
    super("audioRemoved", "audio" /* Audio */, user, stream);
  }
};
var UserVideoStreamAddedEvent = class extends UserStreamAddedEvent {
  constructor(user, stream) {
    super("videoAdded", "video" /* Video */, user, stream);
  }
};
var UserVideoStreamRemovedEvent = class extends UserStreamRemovedEvent {
  constructor(user, stream) {
    super("videoRemoved", "video" /* Video */, user, stream);
  }
};
var UserPoseEvent = class extends RemoteUserEvent {
  constructor(type2, user) {
    super(type2, user);
    this.pose = new Pose();
  }
};
var UserPosedEvent = class extends UserPoseEvent {
  constructor(user) {
    super("userPosed", user);
    this.height = 0;
  }
};
var UserPointerEvent = class extends UserPoseEvent {
  constructor(user) {
    super("userPointer", user);
    this.name = 18 /* RemoteUser */;
  }
};
var UserChatEvent = class extends RemoteUserEvent {
  constructor(user, text) {
    super("chat", user);
    this.text = text;
  }
};

// ../webrtc/ConnectionState.ts
async function settleState(_name, getState, act, target, movingToTarget, leavingTarget, antiTarget) {
  if (getState() === movingToTarget) {
    await waitFor(() => getState() === target);
  } else {
    if (getState() === leavingTarget) {
      await waitFor(() => getState() === antiTarget);
    }
    if (getState() === antiTarget) {
      await act();
    }
  }
}
function whenDisconnected(name, getState, act) {
  return settleState(name, getState, act, "Connected" /* Connected */, "Connecting" /* Connecting */, "Disconnecting" /* Disconnecting */, "Disconnected" /* Disconnected */);
}
function settleConnected(name, getState, act) {
  return settleState(name, getState, act, "Disconnected" /* Disconnected */, "Disconnecting" /* Disconnecting */, "Connecting" /* Connecting */, "Connected" /* Connected */);
}

// ../webrtc/constants.ts
var DEFAULT_LOCAL_USER_ID = "local-user";

// ../webrtc/ActivityDetector.ts
var ActivityDetector = class {
  constructor(name, audioCtx) {
    this.name = name;
    this.analyzer = Analyser(this.name, audioCtx, {
      fftSize: 32,
      minDecibels: -70
    });
    this.buffer = new Uint8Array(this.analyzer.frequencyBinCount);
  }
  _level = 0;
  maxLevel = 0;
  analyzer;
  buffer;
  dispose() {
    removeVertex(this.analyzer);
  }
  get level() {
    this.analyzer.getByteFrequencyData(this.buffer);
    this._level = Math.max(...this.buffer);
    if (isFinite(this._level)) {
      this.maxLevel = Math.max(this.maxLevel, this._level);
      if (this.maxLevel > 0) {
        this._level /= this.maxLevel;
      }
    }
    return this._level;
  }
  get input() {
    return this.analyzer;
  }
  get output() {
    return this.analyzer;
  }
};

// ../webrtc/DecayingGain.ts
var DecayingGain = class {
  constructor(audioCtx, input, output, min2, max2, threshold, attack, decay, sustain, hold, release) {
    this.input = input;
    this.output = output;
    this.min = min2;
    this.max = max2;
    this.threshold = threshold;
    this.attack = attack;
    this.decay = decay;
    this.sustain = sustain;
    this.hold = hold;
    this.release = release;
    this.curLength = 0;
    this.timer = null;
    this._enabled = true;
    this.shouldRun = false;
    this.activity = new ActivityDetector("remote-audio-activity", audioCtx);
    connect(this.input, this.activity);
    this.timer = new SetIntervalTimer(30);
    this.timer.addTickHandler((evt) => this.update(evt));
  }
  update(evt) {
    if (this.enabled) {
      const level = this.activity.level;
      if (level >= this.threshold && this.time >= this.length) {
        this.time = 0;
      }
      this.time += evt.dt;
      if (this.time > this.length) {
        this.time = this.length;
      }
      if (level >= this.threshold && this.holding) {
        this.time = this.holdStart;
      }
      this.output.gain.value = this.gain;
    }
  }
  get enabled() {
    return this._enabled;
  }
  setEnabled(v) {
    this._enabled = v;
    this.refresh();
  }
  start() {
    this.shouldRun = true;
    this.refresh();
  }
  stop() {
    this.shouldRun = false;
    this.refresh();
  }
  refresh() {
    if (this.timer != null) {
      const canRun = this.shouldRun && this.enabled;
      if (canRun !== this.timer.isRunning) {
        if (canRun) {
          this.timer.start();
        } else {
          this.timer.stop();
          this.output.gain.value = 1;
        }
      }
    }
  }
  get length() {
    return this.attack + this.decay + this.hold + this.release;
  }
  get time() {
    return this.length - this.curLength;
  }
  set time(v) {
    this.curLength = this.length - v;
  }
  get attackStart() {
    return 0;
  }
  get attackEnd() {
    return this.attackStart + this.attack;
  }
  get decayStart() {
    return this.attackEnd;
  }
  get decayEnd() {
    return this.decayStart + this.decay;
  }
  get holdStart() {
    return this.decayEnd;
  }
  get holdEnd() {
    return this.holdStart + this.hold;
  }
  get releaseStart() {
    return this.holdEnd;
  }
  get releaseEnd() {
    return this.releaseStart + this.release;
  }
  get attacking() {
    return this.attackStart <= this.time && this.time < this.attackEnd;
  }
  get decaying() {
    return this.decayStart <= this.time && this.time < this.decayEnd;
  }
  get holding() {
    return this.holdStart <= this.time && this.time < this.holdEnd;
  }
  get releasing() {
    return this.releaseStart < this.time && this.time < this.releaseEnd;
  }
  get pAttack() {
    return (this.time - this.attackStart) / this.attack;
  }
  get pDecay() {
    return (this.time - this.decayStart) / this.decay;
  }
  get pRelease() {
    return (this.time - this.releaseStart) / this.release;
  }
  get value() {
    if (this.attacking) {
      return 1 - this.pAttack;
    } else if (this.decaying) {
      return this.sustain * this.pDecay;
    } else if (this.holding) {
      return this.sustain;
    } else if (this.releasing) {
      return this.sustain + (1 - this.sustain) * this.pRelease;
    } else {
      return 1;
    }
  }
  get gain() {
    return unproject(this.value, this.min, this.max);
  }
};

// ../webrtc/RemoteUser.ts
var Locker = class {
  constructor() {
    this.locks = /* @__PURE__ */ new Set();
  }
  isLocked(name) {
    return this.locks.has(name);
  }
  isUnlocked(name) {
    return !this.locks.has(name);
  }
  lock(name) {
    this.locks.add(name);
  }
  unlock(name) {
    this.locks.delete(name);
  }
  async withSkipLock(name, act) {
    if (this.isUnlocked(name)) {
      this.lock(name);
      try {
        await act();
      } finally {
        this.unlock(name);
      }
    }
  }
};
var RemoteUserEvent2 = class extends TypedEvent {
  constructor(type2, user) {
    super(type2);
    this.user = user;
  }
};
var RemoteUserIceErrorEvent = class extends RemoteUserEvent2 {
  constructor(user, evt) {
    super("iceError", user);
    this.address = evt.address;
    this.errorCode = evt.errorCode;
    this.errorText = evt.errorText;
    this.port = evt.port;
    this.url = evt.url;
    Object.seal(this);
  }
};
var RemoteUserIceCandidateEvent = class extends RemoteUserEvent2 {
  constructor(user, candidate) {
    super("iceCandidate", user);
    this.candidate = candidate;
  }
};
var RemoteUserOfferEvent = class extends RemoteUserEvent2 {
  constructor(user, offer) {
    super("offer", user);
    this.offer = offer;
  }
};
var RemoteUserAnswerEvent = class extends RemoteUserEvent2 {
  constructor(user, answer) {
    super("answer", user);
    this.answer = answer;
  }
};
var RemoteUserStreamNeededEvent = class extends RemoteUserEvent2 {
  constructor(user) {
    super("streamNeeded", user);
  }
};
var RemoteUserTrackAddedEvent = class extends RemoteUserEvent2 {
  constructor(user, track, stream) {
    super("trackAdded", user);
    this.track = track;
    this.stream = stream;
  }
};
var RemoteUserTrackMutedEvent = class extends RemoteUserEvent2 {
  constructor(user, track) {
    super("trackMuted", user);
    this.track = track;
  }
};
var RemoteUserTrackRemovedEvent = class extends RemoteUserEvent2 {
  constructor(user, track, stream) {
    super("trackRemoved", user);
    this.track = track;
    this.stream = stream;
  }
};
var seenUsers = /* @__PURE__ */ new Set();
var RemoteUser = class extends TypedEventBase {
  constructor(userID, userName, rtcConfig, confirmReceipt) {
    super();
    this.userID = userID;
    this.userName = userName;
    this.confirmReceipt = confirmReceipt;
    this.userPosedEvt = new UserPosedEvent(this);
    this.userPointerEvt = new UserPointerEvent(this);
    this.sendPoseBuffer = new Float32Array(12);
    this.sendPointerBuffer = new Float32Array(12);
    this.sendInvocationCompleteBuffer = new Float32Array(2);
    this.transceivers = new Array();
    this.invocationCount = 0;
    this.invocations = /* @__PURE__ */ new Map();
    this.locks = new Locker();
    this.channel = null;
    this.gotOffer = false;
    this.disposed = false;
    this.trackSent = false;
    this.sendPoseBuffer[0] = 2 /* Pose */;
    this.sendPointerBuffer[0] = 1 /* Pointer */;
    this.sendInvocationCompleteBuffer[0] = 3 /* InvocationComplete */;
    this.connection = new RTCPeerConnection(rtcConfig);
    this.connection.addEventListener("icecandidateerror", (evt) => {
      this.dispatchEvent(new RemoteUserIceErrorEvent(this, evt));
    });
    this.connection.addEventListener("icecandidate", async (evt) => {
      if (evt.candidate) {
        this.dispatchEvent(new RemoteUserIceCandidateEvent(this, evt.candidate));
      }
    });
    this.connection.addEventListener("negotiationneeded", async () => {
      if (this.trackSent || this.gotOffer) {
        const iceRestart = seenUsers.has(this.userID);
        const offer = await this.connection.createOffer({
          iceRestart
        });
        if (!iceRestart) {
          seenUsers.add(this.userID);
        }
        await this.connection.setLocalDescription(offer);
        this.dispatchEvent(new RemoteUserOfferEvent(this, this.connection.localDescription.toJSON()));
      }
    });
    this.connection.addEventListener("datachannel", (evt) => {
      this.setChannel(evt.channel);
    });
    this.connection.addEventListener("track", async (evt) => {
      const transceiver = evt.transceiver;
      const track = evt.track;
      const stream = arrayScan(evt.streams, (s) => {
        const tracks = s.getTracks();
        for (const t2 of tracks) {
          if (t2 === track) {
            return true;
          }
        }
        return false;
      });
      if (this.transceivers.indexOf(transceiver) === -1) {
        this.transceivers.push(transceiver);
      }
      const onMute = () => {
        this.dispatchEvent(new RemoteUserTrackMutedEvent(this, track));
      };
      const onEnd = () => {
        track.removeEventListener("ended", onEnd);
        track.removeEventListener("mute", onMute);
        arrayRemove(this.transceivers, transceiver);
        this.dispatchEvent(new RemoteUserTrackRemovedEvent(this, track, stream));
      };
      track.addEventListener("ended", onEnd);
      track.addEventListener("mute", onMute);
      this.dispatchEvent(new RemoteUserTrackAddedEvent(this, track, stream));
      if (!this.trackSent) {
        this.start();
      } else {
        this.setChannel(this.connection.createDataChannel("poses", {
          ordered: false,
          maxRetransmits: 0
        }));
      }
    });
    let wasConnected = false;
    this.connection.addEventListener("connectionstatechange", () => {
      if (this.connection.connectionState === "failed" && wasConnected) {
        this.dispatchEvent(new UserLeftEvent(this));
      } else if (this.connection.connectionState === "connected") {
        wasConnected = true;
      }
    });
    Object.seal(this);
  }
  dispose() {
    if (!this.disposed) {
      if (this.channel) {
        this.channel.close();
        this.channel = null;
      }
      for (const transceiver of this.transceivers) {
        transceiver.stop();
      }
      arrayClear(this.transceivers);
      this.connection.close();
      this.disposed = true;
    }
  }
  setChannel(channel) {
    this.channel = channel;
    this.channel.binaryType = "arraybuffer";
    this.channel.addEventListener("message", (evt) => {
      if (isArrayBuffer(evt.data)) {
        const data = new Float32Array(evt.data);
        const msg = data[0];
        const invocationID = data[1];
        if (msg === 3 /* InvocationComplete */) {
          const resolve = this.invocations.get(invocationID);
          if (resolve) {
            resolve();
          }
        } else if (this.channel && this.channel.readyState === "open") {
          if (msg === 2 /* Pose */) {
            this.recvPose(data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11]);
          } else {
            this.recvPointer(data[11], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10]);
          }
          if (invocationID >= 0) {
            this.sendInvocationCompleteBuffer[1] = invocationID;
            this.channel.send(this.sendInvocationCompleteBuffer);
          }
        }
      }
    });
  }
  recvPose(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
    this.userPosedEvt.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
    this.userPosedEvt.height = height;
    this.dispatchEvent(this.userPosedEvt);
  }
  sendPose(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
    return this.sendMessage(2 /* Pose */, px, py, pz, fx, fy, fz, ux, uy, uz, height);
  }
  recvPointer(pointerName, px, py, pz, fx, fy, fz, ux, uy, uz) {
    this.userPointerEvt.name = pointerName;
    this.userPointerEvt.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
    this.dispatchEvent(this.userPointerEvt);
  }
  sendPointer(pointerName, px, py, pz, fx, fy, fz, ux, uy, uz) {
    return this.sendMessage(1 /* Pointer */, px, py, pz, fx, fy, fz, ux, uy, uz, pointerName);
  }
  async sendMessage(msgName, px, py, pz, fx, fy, fz, ux, uy, uz, pointerNameOrHeight) {
    if (this.channel && this.channel.readyState === "open") {
      this.locks.withSkipLock(msgName, async () => {
        const buffer = msgName === 2 /* Pose */ ? this.sendPoseBuffer : this.sendPointerBuffer;
        const invocationID = ++this.invocationCount;
        buffer[1] = invocationID;
        buffer[2] = px;
        buffer[3] = py;
        buffer[4] = pz;
        buffer[5] = fx;
        buffer[6] = fy;
        buffer[7] = fz;
        buffer[8] = ux;
        buffer[9] = uy;
        buffer[10] = uz;
        buffer[11] = pointerNameOrHeight;
        if (!this.confirmReceipt) {
          this.channel.send(buffer);
        } else {
          const task = new Task();
          const timeout = setTimeout(task.reject, 100, "timeout");
          const onComplete = () => {
            clearTimeout(timeout);
            task.resolve();
          };
          this.invocations.set(invocationID, onComplete);
          this.channel.send(buffer);
          try {
            await task;
          } catch (exp) {
            if (exp !== "timeout") {
              console.error(exp);
            }
          } finally {
            this.invocations.delete(invocationID);
          }
        }
      });
    }
  }
  async addIceCandidate(ice) {
    await this.connection.addIceCandidate(ice);
  }
  async acceptOffer(offer) {
    await this.connection.setRemoteDescription(offer);
    this.gotOffer = true;
    const answer = await this.connection.createAnswer();
    await this.connection.setLocalDescription(answer);
    this.dispatchEvent(new RemoteUserAnswerEvent(this, this.connection.localDescription.toJSON()));
  }
  async acceptAnswer(answer) {
    await this.connection.setRemoteDescription(answer);
  }
  start() {
    if (!this.trackSent) {
      this.trackSent = true;
      this.dispatchEvent(new RemoteUserStreamNeededEvent(this));
    }
  }
  sendStream(stream) {
    for (const track of stream.getTracks()) {
      if (this.trackSent) {
        this.connection.addTrack(track, stream);
      } else {
        this.transceivers.push(this.connection.addTransceiver(track, {
          streams: [stream]
        }));
      }
    }
  }
};

// ../webrtc/TeleconferenceManager.ts
var loggingEnabled = window.location.hostname === "localhost" || /\bdebug\b/.test(window.location.search);
var sockets = singleton("Juniper:Sockets", () => new Array());
function fakeSocket(...args) {
  console.log("New connection", ...args);
  const socket = new WebSocket(...args);
  sockets.push(socket);
  return socket;
}
fakeSocket.CLOSED = WebSocket.CLOSED;
fakeSocket.CLOSING = WebSocket.CLOSING;
fakeSocket.CONNECTING = WebSocket.CONNECTING;
fakeSocket.OPEN = WebSocket.OPEN;
Object.assign(window, {
  sockets: {
    list: () => {
      return sockets;
    },
    kill: () => {
      for (const socket of sockets) {
        socket.close();
      }
    }
  }
});
var TeleconferenceManager = class extends TypedEventBase {
  constructor(audio, signalRPath, autoSetPosition = true, needsVideoDevice = false) {
    super();
    this.audio = audio;
    this.signalRPath = signalRPath;
    this.autoSetPosition = autoSetPosition;
    this.needsVideoDevice = needsVideoDevice;
    if (loggingEnabled) {
      console.log(adapter_core_default);
    }
    let hubBuilder = new HubConnectionBuilder().withAutomaticReconnect();
    hubBuilder.withUrl(this.signalRPath, HttpTransportType.WebSockets);
    this.hub = hubBuilder.build();
    this.audio.devices.addEventListener("audioinputchanged", (evt) => this.onAudioInputChanged(evt));
    this.hub.onclose(() => {
      this.lastRoom = null;
      this.lastUserID = null;
      this.setConferenceState("Disconnected" /* Disconnected */);
    });
    this.hub.onreconnecting((err) => {
      this.dispatchEvent(new ConferenceErrorEvent(err));
    });
    this.hub.onreconnected(async () => {
      this.lastRoom = null;
      this.lastUserID = null;
      this.setConferenceState("Disconnected" /* Disconnected */);
      await this.identify(this.localUserName);
      await this.join(this.roomName);
    });
    this.hub.on("userJoined", this.onUserJoined.bind(this));
    this.hub.on("iceReceived", this.onIceReceived.bind(this));
    this.hub.on("offerReceived", this.onOfferReceived.bind(this));
    this.hub.on("answerReceived", this.onAnswerReceived.bind(this));
    this.hub.on("userLeft", (fromUserID) => {
      const user = this.users.get(fromUserID);
      if (user) {
        this.onUserLeft(user);
      }
    });
    this.hub.on("userPosed", (fromUserID, px, py, pz, fx, fy, fz, ux, uy, uz, height) => {
      const user = this.users.get(fromUserID);
      if (user) {
        user.recvPose(px, py, pz, fx, fy, fz, ux, uy, uz, height);
      }
    });
    this.hub.on("userPointer", (fromUserID, name, px, py, pz, fx, fy, fz, ux, uy, uz) => {
      const user = this.users.get(fromUserID);
      if (user) {
        user.recvPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz);
      }
    });
    this.hub.on("chat", (fromUserID, text) => {
      const user = this.users.get(fromUserID);
      if (user) {
        this.dispatchEvent(new UserChatEvent(user, text));
      }
    });
    this.remoteGainDecay = new DecayingGain(this.audio.audioCtx, this.audio.audioDestination.remoteUserInput, this.audio.localAutoControlledGain, 0.05, 1, 0.25, 0, 250, 0.25, 1e3, 250);
    this.remoteGainDecay.setEnabled(!this.audio.useHeadphones);
    this.audio.addEventListener("useheadphonestoggled", () => {
      this.remoteGainDecay.setEnabled(!this.audio.useHeadphones);
      this.restartStream();
    });
    const onWindowClosed = () => {
      if (this.conferenceState === "Connected" /* Connected */) {
        this.toRoom("leave");
      }
    };
    window.addEventListener("beforeunload", onWindowClosed);
    window.addEventListener("unload", onWindowClosed);
    window.addEventListener("pagehide", onWindowClosed);
    this.localStream = this.audio.devices.currentStream;
    Object.seal(this);
  }
  toggleLogging() {
    loggingEnabled = !loggingEnabled;
  }
  _isAudioMuted = null;
  get isAudioMuted() {
    return this._isAudioMuted;
  }
  _isVideoMuted = null;
  get isVideoMuted() {
    return this._isVideoMuted;
  }
  _localUserID = DEFAULT_LOCAL_USER_ID;
  get localUserID() {
    return this._localUserID;
  }
  _localUserName = null;
  get localUserName() {
    return this._localUserName;
  }
  _roomName = null;
  get roomName() {
    return this._roomName;
  }
  _conferenceState = "Disconnected" /* Disconnected */;
  _hasAudioPermission = false;
  get hasAudioPermission() {
    return this._hasAudioPermission;
  }
  _hasVideoPermission = false;
  get hasVideoPermission() {
    return this._hasVideoPermission;
  }
  lastRoom = null;
  lastUserID = null;
  users = /* @__PURE__ */ new Map();
  localStreamIn = null;
  hub;
  remoteGainDecay;
  _ready = null;
  get ready() {
    if (this._ready === null) {
      this._ready = this.startInternal();
    }
    return this._ready;
  }
  async startInternal() {
    await this.audio.ready;
    await this.audio.devices.startPreferredAudioInput();
    this.localStream = this.audio.devices.currentStream;
  }
  get connectionState() {
    switch (this.hub.state) {
      case HubConnectionState.Connected:
        return "Connected" /* Connected */;
      case HubConnectionState.Connecting:
      case HubConnectionState.Reconnecting:
        return "Connecting" /* Connecting */;
      case HubConnectionState.Disconnected:
        return "Disconnected" /* Disconnected */;
      case HubConnectionState.Disconnecting:
        return "Disconnecting" /* Disconnecting */;
      default:
        assertNever(this.hub.state);
    }
  }
  get echoControl() {
    return this.remoteGainDecay;
  }
  get isConnected() {
    return this.connectionState === "Connected" /* Connected */;
  }
  get conferenceState() {
    return this._conferenceState;
  }
  get isConferenced() {
    return this.conferenceState === "Connected" /* Connected */;
  }
  setConferenceState(state) {
    this._conferenceState = state;
  }
  disposed = false;
  dispose() {
    if (!this.disposed) {
      this.leave();
      this.disconnect();
      this.disposed = true;
    }
  }
  err(source, ...msg) {
    console.warn(source, ...msg);
  }
  async toServer(method, ...rest) {
    return await this.hub.invoke(method, ...rest);
  }
  async toRoom(method, ...rest) {
    if (this.isConnected) {
      await this.hub.invoke(method, this.localUserID, this.roomName, ...rest);
    }
  }
  async toUser(method, toUserID, ...rest) {
    if (this.isConnected) {
      await this.hub.invoke(method, this.localUserID, toUserID, ...rest);
    }
  }
  async connect() {
    await this.ready;
    await whenDisconnected("Connecting", () => this.connectionState, async () => {
      await this.hub.start();
      this.dispatchEvent(new ConferenceServerConnectedEvent());
    });
  }
  async join(roomName) {
    if (this.lastRoom !== null && roomName !== this.lastRoom) {
      await this.leave();
    }
    await whenDisconnected(`Joining room ${roomName}`, () => this.conferenceState, async () => {
      this.setConferenceState("Connecting" /* Connecting */);
      this._roomName = roomName;
      await this.toServer("join", this.roomName);
      this.lastRoom = this.roomName;
      this.setConferenceState("Connected" /* Connected */);
      const destination = this.audio.setLocalUserID(this.localUserID);
      this.dispatchEvent(new RoomJoinedEvent(this.localUserID, destination.pose));
      await this.toRoom("greetEveryone", this.localUserName);
    });
  }
  async identify(userName) {
    if (this.localUserID === DEFAULT_LOCAL_USER_ID) {
      this._localUserID = null;
    }
    this._localUserID = this.localUserID || await this.toServer("getNewUserID");
    this._localUserName = userName;
    if (this.localUserID && this.localUserID !== this.lastUserID && this.isConnected) {
      this.lastUserID = this.localUserID;
      await this.toServer("identify", this.localUserID);
    }
  }
  async leave() {
    await settleConnected(`Leaving room ${this.lastRoom}`, () => this.conferenceState, async () => {
      this.setConferenceState("Disconnecting" /* Disconnecting */);
      await this.toRoom("leave");
    });
    for (const user of this.users.values()) {
      this.onUserLeft(user);
    }
    this._roomName = this.lastRoom = null;
    this.setConferenceState("Disconnected" /* Disconnected */);
    this.dispatchEvent(new RoomLeftEvent(this.localUserID));
  }
  async disconnect() {
    if (this.conferenceState !== "Disconnected" /* Disconnected */) {
      await this.leave();
    }
    await settleConnected("Disconnecting", () => this.connectionState, async () => {
      await this.hub.stop();
      this._localUserID = this.lastUserID = DEFAULT_LOCAL_USER_ID;
      this.audio.setLocalUserID(this.localUserID);
      this.dispatchEvent(new ConferenceServerDisconnectedEvent());
    });
  }
  async onUserJoined(fromUserID, fromUserName) {
    if (this.users.has(fromUserID)) {
      const user = this.users.get(fromUserID);
      user.start();
    } else {
      if (this.users.size === 0) {
        this.remoteGainDecay.start();
      }
      const rtcConfig = await this.getRTCConfiguration();
      const user = new RemoteUser(fromUserID, fromUserName, rtcConfig, true);
      this.users.set(user.userID, user);
      user.addEventListener("iceError", this.onIceError.bind(this));
      user.addEventListener("iceCandidate", this.onIceCandidate.bind(this));
      user.addEventListener("offer", this.onOfferCreated.bind(this));
      user.addEventListener("answer", this.onAnswerCreated.bind(this));
      user.addEventListener("streamNeeded", this.onStreamNeeded.bind(this));
      user.addEventListener("trackAdded", this.onTrackAdded.bind(this));
      user.addEventListener("trackMuted", this.onTrackMuted.bind(this));
      user.addEventListener("trackRemoved", this.onTrackRemoved.bind(this));
      user.addEventListener("userLeft", (evt) => this.onUserLeft(evt.user));
      user.addEventListener("userPosed", (evt) => {
        this.onRemoteUserPosed(evt);
        const { p, f, u } = evt.pose;
        this.audio.setUserPose(evt.user.userID, p[0], p[1], p[2], f[0], f[1], f[2], u[0], u[1], u[2]);
      });
      user.addEventListener("userPointer", (evt) => this.onRemoteUserPosed(evt));
      this.toUser("greet", user.userID, this.localUserName);
      const source = this.audio.createUser(user.userID, user.userName);
      this.dispatchEvent(new UserJoinedEvent(user, source));
    }
  }
  async getRTCConfiguration() {
    return await this.toServer("getRTCConfiguration", this.localUserID);
  }
  onIceError(evt) {
    this.err("icecandidateerror", this.localUserName, evt.user.userName, `${evt.url} [${evt.errorCode}]: ${evt.errorText}`);
  }
  async onIceCandidate(evt) {
    await this.sendIce(evt.user.userID, evt.candidate);
  }
  async onIceReceived(fromUserID, candidateJSON) {
    try {
      const user = this.users.get(fromUserID);
      if (user) {
        const ice = JSON.parse(candidateJSON);
        await user.addIceCandidate(ice);
      }
    } catch (exp) {
      this.err("iceReceived", `${exp.message} [${fromUserID}] (${candidateJSON})`);
    }
  }
  async onOfferCreated(evt) {
    await this.sendOffer(evt.user.userID, evt.offer);
  }
  async onOfferReceived(fromUserID, offerJSON) {
    try {
      const user = this.users.get(fromUserID);
      if (user) {
        const offer = JSON.parse(offerJSON);
        await user.acceptOffer(offer);
      }
    } catch (exp) {
      this.err("offerReceived", exp.message);
    }
  }
  async onAnswerCreated(evt) {
    await this.sendAnswer(evt.user.userID, evt.answer);
  }
  async onAnswerReceived(fromUserID, answerJSON) {
    try {
      const user = this.users.get(fromUserID);
      if (user) {
        const answer = JSON.parse(answerJSON);
        await user.acceptAnswer(answer);
      }
    } catch (exp) {
      this.err("answerReceived", exp.message);
    }
  }
  onStreamNeeded(evt) {
    evt.user.sendStream(this.audio.output.stream);
  }
  onTrackAdded(evt) {
    if (evt.track.kind === "audio") {
      this.audio.setUserStream(evt.user.userID, evt.user.userName, evt.stream);
      this.dispatchEvent(new UserAudioStreamAddedEvent(evt.user, evt.stream));
    } else if (evt.track.kind === "video") {
      this.dispatchEvent(new UserVideoStreamAddedEvent(evt.user, evt.stream));
    }
  }
  onTrackMuted(evt) {
    if (evt.track.kind === "audio") {
      this.dispatchEvent(new UserAudioMutedEvent(evt.user, evt.track.muted));
    } else if (evt.track.kind === "video") {
      this.dispatchEvent(new UserVideoMutedEvent(evt.user, evt.track.muted));
    }
  }
  onTrackRemoved(evt) {
    if (evt.track.kind === "audio") {
      this.audio.setUserStream(evt.user.userID, evt.user.userName, null);
      this.dispatchEvent(new UserAudioStreamRemovedEvent(evt.user, evt.stream));
    } else if (evt.track.kind === "video") {
      this.dispatchEvent(new UserVideoStreamRemovedEvent(evt.user, evt.stream));
    }
  }
  onUserLeft(user) {
    user.dispose();
    this.users.delete(user.userID);
    if (this.users.size === 0) {
      this.remoteGainDecay.stop();
    }
    this.audio.removeUser(user.userID);
    this.dispatchEvent(new UserLeftEvent(user));
  }
  async sendIce(toUserID, candidate) {
    await this.toUser("sendIce", toUserID, JSON.stringify(candidate));
  }
  async sendOffer(toUserID, offer) {
    await this.toUser("sendOffer", toUserID, JSON.stringify(offer));
  }
  async sendAnswer(toUserID, answer) {
    await this.toUser("sendAnswer", toUserID, JSON.stringify(answer));
  }
  async setLocalPose(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
    if (this.autoSetPosition) {
      this.audio.setUserPose(this.localUserID, px, py, pz, fx, fy, fz, ux, uy, uz);
    }
    if (this.conferenceState === "Connected" /* Connected */) {
      await Promise.all(Array.from(this.users.values()).map((user) => user.sendPose(px, py, pz, fx, fy, fz, ux, uy, uz, height)));
    }
  }
  async setLocalPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz) {
    if (this.conferenceState === "Connected" /* Connected */) {
      await Promise.all(Array.from(this.users.values()).map((user) => user.sendPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz)));
    }
  }
  onRemoteUserPosed(evt) {
    const offset = this.audio.getUserOffset(evt.user.userID);
    if (offset) {
      evt.pose.setOffset(offset[0], offset[1], offset[2]);
    }
  }
  chat(text) {
    if (this.conferenceState === "Connected" /* Connected */) {
      this.toRoom("chat", text);
    }
  }
  userExists(id) {
    return this.users.has(id);
  }
  getUserNames() {
    return Array.from(this.users.values()).map((u) => [u.userID, u.userName]);
  }
  async onAudioInputChanged(evt) {
    const deviceId = evt.audio && evt.audio.deviceId;
    await this.startStream(deviceId, this.audio.useHeadphones);
  }
  async startStream(deviceId, usingHeadphones) {
    if (!deviceId) {
      this.localStream = null;
    } else {
      this.localStream = await navigator.mediaDevices.getUserMedia({
        audio: {
          deviceId,
          echoCancellation: !usingHeadphones,
          autoGainControl: true,
          noiseSuppression: true
        }
      });
    }
  }
  async restartStream() {
    this.localStream = null;
    await this.startStream(this.audio.devices.preferredAudioOutputID, this.audio.useHeadphones);
  }
  get localStream() {
    return this.localStreamIn && this.localStreamIn.mediaStream || null;
  }
  set localStream(v) {
    if (v !== this.localStream) {
      if (this.localStreamIn) {
        removeVertex(this.localStreamIn);
        this.localStreamIn = null;
      }
      this.audio.devices.currentStream = v;
      if (v) {
        this.localStreamIn = MediaStreamSource("local-mic", this.audio.audioCtx, v, this.audio);
      }
    }
  }
  isMediaMuted(type2) {
    for (const track of this.audio.output.stream.getTracks()) {
      if (track.kind === type2) {
        return !track.enabled;
      }
    }
    return true;
  }
  setMediaMuted(type2, muted2) {
    for (const track of this.audio.output.stream.getTracks()) {
      if (track.kind === type2) {
        track.enabled = !muted2;
      }
    }
  }
  get audioMuted() {
    return this.isMediaMuted("audio" /* Audio */);
  }
  set audioMuted(muted2) {
    this.setMediaMuted("audio" /* Audio */, muted2);
  }
  get videoMuted() {
    return this.isMediaMuted("video" /* Video */);
  }
  set videoMuted(muted2) {
    this.setMediaMuted("video" /* Video */, muted2);
  }
};

// ../dom/css.ts
function getMonospaceFonts() {
  return "ui-monospace, 'Droid Sans Mono', 'Cascadia Mono', 'Segoe UI Mono', 'Ubuntu Mono', 'Roboto Mono', Menlo, Monaco, Consolas, monospace";
}

// ../emoji/Emoji.ts
var Emoji = class {
  constructor(value, desc, props = null) {
    this.value = value;
    this.desc = desc;
    this.value = value;
    this.desc = desc;
    this.props = props || {};
  }
  contains(e2) {
    if (e2 instanceof Emoji) {
      return this.contains(e2.value);
    } else {
      return this.value.indexOf(e2) >= 0;
    }
  }
};

// ../emoji/index.ts
var star = /* @__PURE__ */ new Emoji("\u2B50", "Star");

// ../threejs/animation/lookAngles.ts
var D = new THREE.Vector3();
function getLookHeading(dir) {
  D.copy(dir);
  D.y = 0;
  D.normalize();
  return angleClamp(Math.atan2(D.x, D.z));
}

// ../threejs/animation/BodyFollower.ts
var targetPos = new THREE.Vector3();
var targetAngle = 0;
var dPos = new THREE.Vector3();
var curPos = new THREE.Vector3();
var curDir = new THREE.Vector3();
var dQuat = new THREE.Quaternion();
var curAngle = 0;
var copyCounter = 0;
function minRotAngle(to, from) {
  const a = to - from;
  const b = a + 2 * Math.PI;
  const c = a - 2 * Math.PI;
  const A2 = Math.abs(a);
  const B = Math.abs(b);
  const C = Math.abs(c);
  if (A2 < B && A2 < C) {
    return a;
  } else if (B < C) {
    return b;
  } else {
    return c;
  }
}
var BodyFollower = class extends THREE.Object3D {
  constructor(name, minDistance, minAngle, heightOffset, speed = 1) {
    super();
    this.minDistance = minDistance;
    this.heightOffset = heightOffset;
    this.speed = speed;
    this.name = name;
    this.lerp = this.minAngle > 0 || this.minDistance > 0;
    this.maxDistance = this.minDistance * 5;
    this.minAngle = deg2rad(minAngle);
    this.maxAngle = Math.PI - this.minAngle;
    Object.seal(this);
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this.name = source.name + ++copyCounter;
    this.lerp = source.lerp;
    this.maxDistance = source.maxDistance;
    this.minAngle = source.minAngle;
    this.maxAngle = source.maxAngle;
    return this;
  }
  update(height, position, angle3, dt) {
    dt *= 1e-3;
    this.clampTo(this.lerp, height, position, this.minDistance, this.maxDistance, angle3, this.minAngle, this.maxAngle, dt);
  }
  reset(height, position, angle3) {
    this.clampTo(false, height, position, 0, 0, angle3, 0, 0, 0);
  }
  clampTo(lerp3, height, position, minDistance, maxDistance, angle3, minAngle, maxAngle, dt) {
    targetPos.copy(position);
    targetPos.y -= this.heightOffset * height;
    targetAngle = angle3;
    this.getWorldPosition(curPos);
    this.getWorldDirection(curDir);
    curAngle = getLookHeading(curDir);
    dQuat.identity();
    let setPos = !lerp3;
    let setRot = !lerp3;
    if (lerp3) {
      const dist2 = dPos.copy(targetPos).sub(curPos).length();
      if (minDistance < dist2) {
        if (dist2 < maxDistance) {
          targetPos.lerpVectors(curPos, targetPos, this.speed * dt);
        }
        setPos = true;
      }
      const dAngle = minRotAngle(targetAngle, curAngle);
      const mAngle = Math.abs(dAngle);
      if (minAngle < mAngle) {
        if (mAngle < maxAngle) {
          dQuat.setFromAxisAngle(this.up, dAngle * this.speed * dt);
        } else {
          dQuat.setFromAxisAngle(this.up, dAngle);
        }
        setRot = true;
      }
    }
    if (setPos || setRot) {
      if (setPos) {
        this.position.add(targetPos.sub(curPos));
      }
      if (setRot) {
        this.quaternion.multiply(dQuat);
      }
    }
  }
};

// ../threejs/examples/lines/LineMaterial.js
THREE.UniformsLib.line = {
  worldUnits: { value: 1 },
  linewidth: { value: 1 },
  resolution: { value: new THREE.Vector2(1, 1) },
  dashOffset: { value: 0 },
  dashScale: { value: 1 },
  dashSize: { value: 1 },
  gapSize: { value: 1 }
};
THREE.ShaderLib["line"] = {
  uniforms: THREE.UniformsUtils.merge([
    THREE.UniformsLib.common,
    THREE.UniformsLib.fog,
    THREE.UniformsLib.line
  ]),
  vertexShader: `
		#include <common>
		#include <color_pars_vertex>
		#include <fog_pars_vertex>
		#include <logdepthbuf_pars_vertex>
		#include <clipping_planes_pars_vertex>

		uniform float linewidth;
		uniform vec2 resolution;

		attribute vec3 instanceStart;
		attribute vec3 instanceEnd;

		attribute vec3 instanceColorStart;
		attribute vec3 instanceColorEnd;

		#ifdef WORLD_UNITS

			varying vec4 worldPos;
			varying vec3 worldStart;
			varying vec3 worldEnd;

			#ifdef USE_DASH

				varying vec2 vUv;

			#endif

		#else

			varying vec2 vUv;

		#endif

		#ifdef USE_DASH

			uniform float dashScale;
			attribute float instanceDistanceStart;
			attribute float instanceDistanceEnd;
			varying float vLineDistance;

		#endif

		void trimSegment( const in vec4 start, inout vec4 end ) {

			// trim end segment so it terminates between the camera plane and the near plane

			// conservative estimate of the near plane
			float a = projectionMatrix[ 2 ][ 2 ]; // 3nd entry in 3th column
			float b = projectionMatrix[ 3 ][ 2 ]; // 3nd entry in 4th column
			float nearEstimate = - 0.5 * b / a;

			float alpha = ( nearEstimate - start.z ) / ( end.z - start.z );

			end.xyz = mix( start.xyz, end.xyz, alpha );

		}

		void main() {

			#ifdef USE_COLOR

				vColor.xyz = ( position.y < 0.5 ) ? instanceColorStart : instanceColorEnd;

			#endif

			#ifdef USE_DASH

				vLineDistance = ( position.y < 0.5 ) ? dashScale * instanceDistanceStart : dashScale * instanceDistanceEnd;
				vUv = uv;

			#endif

			float aspect = resolution.x / resolution.y;

			// camera space
			vec4 start = modelViewMatrix * vec4( instanceStart, 1.0 );
			vec4 end = modelViewMatrix * vec4( instanceEnd, 1.0 );

			#ifdef WORLD_UNITS

				worldStart = start.xyz;
				worldEnd = end.xyz;

			#else

				vUv = uv;

			#endif

			// special case for perspective projection, and segments that terminate either in, or behind, the camera plane
			// clearly the gpu firmware has a way of addressing this issue when projecting into ndc space
			// but we need to perform ndc-space calculations in the shader, so we must address this issue directly
			// perhaps there is a more elegant solution -- WestLangley

			bool perspective = ( projectionMatrix[ 2 ][ 3 ] == - 1.0 ); // 4th entry in the 3rd column

			if ( perspective ) {

				if ( start.z < 0.0 && end.z >= 0.0 ) {

					trimSegment( start, end );

				} else if ( end.z < 0.0 && start.z >= 0.0 ) {

					trimSegment( end, start );

				}

			}

			// clip space
			vec4 clipStart = projectionMatrix * start;
			vec4 clipEnd = projectionMatrix * end;

			// ndc space
			vec3 ndcStart = clipStart.xyz / clipStart.w;
			vec3 ndcEnd = clipEnd.xyz / clipEnd.w;

			// direction
			vec2 dir = ndcEnd.xy - ndcStart.xy;

			// account for clip-space aspect ratio
			dir.x *= aspect;
			dir = normalize( dir );

			#ifdef WORLD_UNITS

				// get the offset direction as perpendicular to the view vector
				vec3 worldDir = normalize( end.xyz - start.xyz );
				vec3 offset;
				if ( position.y < 0.5 ) {

					offset = normalize( cross( start.xyz, worldDir ) );

				} else {

					offset = normalize( cross( end.xyz, worldDir ) );

				}

				// sign flip
				if ( position.x < 0.0 ) offset *= - 1.0;

				float forwardOffset = dot( worldDir, vec3( 0.0, 0.0, 1.0 ) );

				// don't extend the line if we're rendering dashes because we
				// won't be rendering the endcaps
				#ifndef USE_DASH

					// extend the line bounds to encompass  endcaps
					start.xyz += - worldDir * linewidth * 0.5;
					end.xyz += worldDir * linewidth * 0.5;

					// shift the position of the quad so it hugs the forward edge of the line
					offset.xy -= dir * forwardOffset;
					offset.z += 0.5;

				#endif

				// endcaps
				if ( position.y > 1.0 || position.y < 0.0 ) {

					offset.xy += dir * 2.0 * forwardOffset;

				}

				// adjust for linewidth
				offset *= linewidth * 0.5;

				// set the world position
				worldPos = ( position.y < 0.5 ) ? start : end;
				worldPos.xyz += offset;

				// project the worldpos
				vec4 clip = projectionMatrix * worldPos;

				// shift the depth of the projected points so the line
				// segements overlap neatly
				vec3 clipPose = ( position.y < 0.5 ) ? ndcStart : ndcEnd;
				clip.z = clipPose.z * clip.w;

			#else

				vec2 offset = vec2( dir.y, - dir.x );
				// undo aspect ratio adjustment
				dir.x /= aspect;
				offset.x /= aspect;

				// sign flip
				if ( position.x < 0.0 ) offset *= - 1.0;

				// endcaps
				if ( position.y < 0.0 ) {

					offset += - dir;

				} else if ( position.y > 1.0 ) {

					offset += dir;

				}

				// adjust for linewidth
				offset *= linewidth;

				// adjust for clip-space to screen-space conversion // maybe resolution should be based on viewport ...
				offset /= resolution.y;

				// select end
				vec4 clip = ( position.y < 0.5 ) ? clipStart : clipEnd;

				// back to clip space
				offset *= clip.w;

				clip.xy += offset;

			#endif

			gl_Position = clip;

			vec4 mvPosition = ( position.y < 0.5 ) ? start : end; // this is an approximation

			#include <logdepthbuf_vertex>
			#include <clipping_planes_vertex>
			#include <fog_vertex>

		}
		`,
  fragmentShader: `
		uniform vec3 diffuse;
		uniform float opacity;
		uniform float linewidth;

		#ifdef USE_DASH

			uniform float dashOffset;
			uniform float dashSize;
			uniform float gapSize;

		#endif

		varying float vLineDistance;

		#ifdef WORLD_UNITS

			varying vec4 worldPos;
			varying vec3 worldStart;
			varying vec3 worldEnd;

			#ifdef USE_DASH

				varying vec2 vUv;

			#endif

		#else

			varying vec2 vUv;

		#endif

		#include <common>
		#include <color_pars_fragment>
		#include <fog_pars_fragment>
		#include <logdepthbuf_pars_fragment>
		#include <clipping_planes_pars_fragment>

		vec2 closestLineToLine(vec3 p1, vec3 p2, vec3 p3, vec3 p4) {

			float mua;
			float mub;

			vec3 p13 = p1 - p3;
			vec3 p43 = p4 - p3;

			vec3 p21 = p2 - p1;

			float d1343 = dot( p13, p43 );
			float d4321 = dot( p43, p21 );
			float d1321 = dot( p13, p21 );
			float d4343 = dot( p43, p43 );
			float d2121 = dot( p21, p21 );

			float denom = d2121 * d4343 - d4321 * d4321;

			float numer = d1343 * d4321 - d1321 * d4343;

			mua = numer / denom;
			mua = clamp( mua, 0.0, 1.0 );
			mub = ( d1343 + d4321 * ( mua ) ) / d4343;
			mub = clamp( mub, 0.0, 1.0 );

			return vec2( mua, mub );

		}

		void main() {

			#include <clipping_planes_fragment>

			#ifdef USE_DASH

				if ( vUv.y < - 1.0 || vUv.y > 1.0 ) discard; // discard endcaps

				if ( mod( vLineDistance + dashOffset, dashSize + gapSize ) > dashSize ) discard; // todo - FIX

			#endif

			float alpha = opacity;

			#ifdef WORLD_UNITS

				// Find the closest points on the view ray and the line segment
				vec3 rayEnd = normalize( worldPos.xyz ) * 1e5;
				vec3 lineDir = worldEnd - worldStart;
				vec2 params = closestLineToLine( worldStart, worldEnd, vec3( 0.0, 0.0, 0.0 ), rayEnd );

				vec3 p1 = worldStart + lineDir * params.x;
				vec3 p2 = rayEnd * params.y;
				vec3 delta = p1 - p2;
				float len = length( delta );
				float norm = len / linewidth;

				#ifndef USE_DASH

					#ifdef USE_ALPHA_TO_COVERAGE

						float dnorm = fwidth( norm );
						alpha = 1.0 - smoothstep( 0.5 - dnorm, 0.5 + dnorm, norm );

					#else

						if ( norm > 0.5 ) {

							discard;

						}

					#endif

				#endif

			#else

				#ifdef USE_ALPHA_TO_COVERAGE

					// artifacts appear on some hardware if a derivative is taken within a conditional
					float a = vUv.x;
					float b = ( vUv.y > 0.0 ) ? vUv.y - 1.0 : vUv.y + 1.0;
					float len2 = a * a + b * b;
					float dlen = fwidth( len2 );

					if ( abs( vUv.y ) > 1.0 ) {

						alpha = 1.0 - smoothstep( 1.0 - dlen, 1.0 + dlen, len2 );

					}

				#else

					if ( abs( vUv.y ) > 1.0 ) {

						float a = vUv.x;
						float b = ( vUv.y > 0.0 ) ? vUv.y - 1.0 : vUv.y + 1.0;
						float len2 = a * a + b * b;

						if ( len2 > 1.0 ) discard;

					}

				#endif

			#endif

			vec4 diffuseColor = vec4( diffuse, alpha );

			#include <logdepthbuf_fragment>
			#include <color_fragment>

			gl_FragColor = vec4( diffuseColor.rgb, alpha );

			#include <tonemapping_fragment>
			#include <encodings_fragment>
			#include <fog_fragment>
			#include <premultiplied_alpha_fragment>

		}
		`
};
var LineMaterial = class extends THREE.ShaderMaterial {
  constructor(parameters) {
    super({
      type: "LineMaterial",
      uniforms: THREE.UniformsUtils.clone(THREE.ShaderLib["line"].uniforms),
      vertexShader: THREE.ShaderLib["line"].vertexShader,
      fragmentShader: THREE.ShaderLib["line"].fragmentShader,
      clipping: true
    });
    Object.defineProperties(this, {
      color: {
        enumerable: true,
        get: function() {
          return this.uniforms.diffuse.value;
        },
        set: function(value) {
          this.uniforms.diffuse.value = value;
        }
      },
      worldUnits: {
        enumerable: true,
        get: function() {
          return "WORLD_UNITS" in this.defines;
        },
        set: function(value) {
          if (value === true) {
            this.defines.WORLD_UNITS = "";
          } else {
            delete this.defines.WORLD_UNITS;
          }
        }
      },
      linewidth: {
        enumerable: true,
        get: function() {
          return this.uniforms.linewidth.value;
        },
        set: function(value) {
          this.uniforms.linewidth.value = value;
        }
      },
      dashed: {
        enumerable: true,
        get: function() {
          return Boolean("USE_DASH" in this.defines);
        },
        set(value) {
          if (Boolean(value) !== Boolean("USE_DASH" in this.defines)) {
            this.needsUpdate = true;
          }
          if (value === true) {
            this.defines.USE_DASH = "";
          } else {
            delete this.defines.USE_DASH;
          }
        }
      },
      dashScale: {
        enumerable: true,
        get: function() {
          return this.uniforms.dashScale.value;
        },
        set: function(value) {
          this.uniforms.dashScale.value = value;
        }
      },
      dashSize: {
        enumerable: true,
        get: function() {
          return this.uniforms.dashSize.value;
        },
        set: function(value) {
          this.uniforms.dashSize.value = value;
        }
      },
      dashOffset: {
        enumerable: true,
        get: function() {
          return this.uniforms.dashOffset.value;
        },
        set: function(value) {
          this.uniforms.dashOffset.value = value;
        }
      },
      gapSize: {
        enumerable: true,
        get: function() {
          return this.uniforms.gapSize.value;
        },
        set: function(value) {
          this.uniforms.gapSize.value = value;
        }
      },
      opacity: {
        enumerable: true,
        get: function() {
          return this.uniforms.opacity.value;
        },
        set: function(value) {
          this.uniforms.opacity.value = value;
        }
      },
      resolution: {
        enumerable: true,
        get: function() {
          return this.uniforms.resolution.value;
        },
        set: function(value) {
          this.uniforms.resolution.value.copy(value);
        }
      },
      alphaToCoverage: {
        enumerable: true,
        get: function() {
          return Boolean("USE_ALPHA_TO_COVERAGE" in this.defines);
        },
        set: function(value) {
          if (Boolean(value) !== Boolean("USE_ALPHA_TO_COVERAGE" in this.defines)) {
            this.needsUpdate = true;
          }
          if (value === true) {
            this.defines.USE_ALPHA_TO_COVERAGE = "";
            this.extensions.derivatives = true;
          } else {
            delete this.defines.USE_ALPHA_TO_COVERAGE;
            this.extensions.derivatives = false;
          }
        }
      }
    });
    this.setValues(parameters);
  }
};
LineMaterial.prototype.isLineMaterial = true;

// ../threejs/materials.ts
var materials = singleton("Juniper:Three:Materials", () => /* @__PURE__ */ new Map());
function del(obj2, name) {
  if (name in obj2) {
    delete obj2[name];
  }
}
function makeMaterial(slug, material, options) {
  const key = `${slug}_${Object.keys(options).map((k) => `${k}:${options[k]}`).join(",")}`;
  if (!materials.has(key)) {
    del(options, "name");
    materials.set(key, new material(options));
  }
  return materials.get(key);
}
function trans(options) {
  return Object.assign(options, {
    transparent: true
  });
}
function solid(options) {
  return makeMaterial("solid", THREE.MeshBasicMaterial, options);
}
function solidTransparent(options) {
  return makeMaterial("solidTransparent", THREE.MeshBasicMaterial, trans(options));
}
function lit(options) {
  return makeMaterial("lit", THREE.MeshStandardMaterial, options);
}
function line2(options) {
  return makeMaterial("line2", LineMaterial, options);
}
var blue = 255;
var green = 65280;
var red = 16711680;
var yellow = 16776960;
var grey = 12632256;
var solidBlue = /* @__PURE__ */ solid({ color: blue });
var solidGreen = /* @__PURE__ */ solid({ color: green });
var solidRed = /* @__PURE__ */ solid({ color: red });
var litGrey = /* @__PURE__ */ lit({ color: grey });

// ../threejs/setGeometryUVsForCubemaps.ts
function setGeometryUVsForCubemaps(geom2) {
  const positions = geom2.attributes.position;
  const normals = geom2.attributes.normal;
  const uvs = geom2.attributes.uv;
  for (let n2 = 0; n2 < normals.count; ++n2) {
    const _x = n2 * normals.itemSize, _y = n2 * normals.itemSize + 1, _z = n2 * normals.itemSize + 2, nx = normals.array[_x], ny = normals.array[_y], nz = normals.array[_z], _nx_ = Math.abs(nx), _ny_ = Math.abs(ny), _nz_ = Math.abs(nz), px = positions.array[_x], py = positions.array[_y], pz = positions.array[_z], _px_ = Math.abs(px), _py_ = Math.abs(py), _pz_ = Math.abs(pz), _u = n2 * uvs.itemSize, _v = n2 * uvs.itemSize + 1;
    let u = uvs.array[_u], v = uvs.array[_v], largest = 0, mx = _nx_, max2 = _px_;
    if (_ny_ > mx) {
      largest = 1;
      mx = _ny_;
      max2 = _py_;
    }
    if (_nz_ > mx) {
      largest = 2;
      mx = _nz_;
      max2 = _pz_;
    }
    if (largest === 0) {
      if (px < 0) {
        u = -pz;
        v = py;
      } else {
        u = pz;
        v = py;
      }
    } else if (largest === 1) {
      if (py < 0) {
        u = px;
        v = -pz;
      } else {
        u = px;
        v = pz;
      }
    } else {
      if (pz < 0) {
        u = px;
        v = py;
      } else {
        u = -px;
        v = py;
      }
    }
    u = (u / max2 + 1) / 8;
    v = (v / max2 + 1) / 6;
    if (largest === 0) {
      if (px < 0) {
        u += 0;
        v += 1 / 3;
      } else {
        u += 0.5;
        v += 1 / 3;
      }
    } else if (largest === 1) {
      if (py < 0) {
        u += 0.25;
        v += 0;
      } else {
        u += 0.25;
        v += 2 / 3;
      }
    } else {
      if (pz < 0) {
        u += 0.25;
        v += 1 / 3;
      } else {
        u += 0.75;
        v += 1 / 3;
      }
    }
    const arr = uvs.array;
    arr[_u] = u;
    arr[_v] = v;
  }
}

// ../threejs/Cube.ts
var cube = new THREE.BoxBufferGeometry(1, 1, 1, 1, 1, 1);
cube.name = "CubeGeom";
var invCube = cube.clone();
invCube.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCube);
var BaseCube = class extends THREE.Mesh {
  constructor(sx, sy, sz, material, isCollider2) {
    super(cube, material);
    this.isCollider = isCollider2;
    this.scale.set(sx, sy, sz);
  }
};
var Cube = class extends BaseCube {
  constructor(sx, sy, sz, material) {
    super(sx, sy, sz, material, false);
    this.isDraggable = false;
  }
};

// ../dom/attrs.ts
var Attr = class {
  constructor(key, value, bySetAttribute, ...tags) {
    this.key = key;
    this.value = value;
    this.bySetAttribute = bySetAttribute;
    this.tags = tags.map((t2) => t2.toLocaleUpperCase());
    Object.freeze(this);
  }
  applyToElement(elem) {
    const isDataSet = this.key.startsWith("data-");
    const isValid = this.tags.length === 0 || this.tags.indexOf(elem.tagName) > -1 || isDataSet;
    if (!isValid) {
      console.warn(`Element ${elem.tagName} does not support Attribute ${this.key}`);
    } else if (isDataSet) {
      const subkey = this.key.substring(5);
      elem.dataset[subkey] = this.value;
    } else if (this.key === "style") {
      Object.assign(elem.style, this.value);
    } else if (this.key === "classList") {
      this.value.forEach((v) => elem.classList.add(v));
    } else if (this.bySetAttribute) {
      elem.setAttribute(this.key, this.value);
    } else if (this.key in elem) {
      elem[this.key] = this.value;
    } else if (this.value === false) {
      elem.removeAttribute(this.key);
    } else if (this.value === true) {
      elem.setAttribute(this.key, "");
    } else {
      elem.setAttribute(this.key, this.value);
    }
  }
};
function htmlHeight(value) {
  return new Attr("height", value, false, "canvas", "embed", "iframe", "img", "input", "object", "video");
}
function htmlWidth(value) {
  return new Attr("width", value, false, "canvas", "embed", "iframe", "img", "input", "object", "video");
}

// ../dom/tags.ts
function isErsatzElement(obj2) {
  if (!isObject(obj2)) {
    return false;
  }
  const elem = obj2;
  return elem.element instanceof Node;
}
function isErsatzElements(obj2) {
  return isObject(obj2) && "elements" in obj2 && obj2.elements instanceof Array;
}
function resolveElement(elem) {
  if (isErsatzElement(elem)) {
    return elem.element;
  }
  return elem;
}
function isIElementAppliable(obj2) {
  return isObject(obj2) && "applyToElement" in obj2 && isFunction(obj2.applyToElement);
}
function elementApply(elem, ...children) {
  elem = resolveElement(elem);
  for (const child of children) {
    if (isDefined(child)) {
      if (child instanceof Node) {
        elem.append(child);
      } else if (isErsatzElement(child)) {
        elem.append(resolveElement(child));
      } else if (isErsatzElements(child)) {
        elem.append(...child.elements.map(resolveElement));
      } else if (isIElementAppliable(child)) {
        child.applyToElement(elem);
      } else {
        elem.append(document.createTextNode(child.toLocaleString()));
      }
    }
  }
  return elem;
}
function tag(name, ...rest) {
  let elem = null;
  for (const attr of rest) {
    if (attr instanceof Attr && attr.key === "id") {
      elem = document.getElementById(attr.value);
      break;
    }
  }
  if (elem == null) {
    elem = document.createElement(name);
  }
  elementApply(elem, ...rest);
  return elem;
}
function Canvas(...rest) {
  return tag("canvas", ...rest);
}

// ../threejs/typeChecks.ts
function isMesh(obj2) {
  return isDefined(obj2) && obj2.isMesh;
}
function isMaterial(obj2) {
  return isDefined(obj2) && obj2.isMaterial;
}
function isMeshBasicMaterial(obj2) {
  return isMaterial(obj2) && obj2.type === "MeshBasicMaterial";
}
function isObject3D(obj2) {
  return isDefined(obj2) && obj2.isObject3D;
}

// ../threejs/objects.ts
function isErsatzObject(obj2) {
  return isDefined(obj2) && isObject3D(obj2.object);
}
function objectResolve(obj2) {
  if (isErsatzObject(obj2)) {
    return obj2.object;
  }
  return obj2;
}
function objectSetVisible(obj2, visible) {
  obj2 = objectResolve(obj2);
  obj2.visible = visible;
  return visible;
}
function objectIsVisible(obj2) {
  obj2 = objectResolve(obj2);
  return obj2.visible;
}
function objectIsFullyVisible(obj2) {
  obj2 = objectResolve(obj2);
  while (obj2) {
    if (!obj2.visible) {
      return false;
    }
    obj2 = obj2.parent;
  }
  return true;
}
function objGraph(obj2, ...children) {
  const toAdd = children.filter(isDefined).map(objectResolve);
  if (toAdd.length > 0) {
    objectResolve(obj2).add(...toAdd);
  }
  return obj2;
}
function obj(name, ...rest) {
  const obj2 = new THREE.Object3D();
  obj2.name = name;
  objGraph(obj2, ...rest);
  return obj2;
}

// ../threejs/setMatrixFromUpFwdPos.ts
var R = new THREE.Vector3();
function setMatrixFromUpFwdPos(U2, F2, P4, matrix) {
  R.crossVectors(F2, U2);
  U2.crossVectors(R, F2);
  R.normalize();
  U2.normalize();
  F2.normalize();
  matrix.set(R.x, U2.x, -F2.x, P4.x, R.y, U2.y, -F2.y, P4.y, R.z, U2.z, -F2.z, P4.z, 0, 0, 0, 1);
}

// ../threejs/eventSystem/PointerState.ts
var PointerState = class {
  buttons = 0;
  moveDistance = 0;
  dragDistance = 0;
  position = new THREE.Vector2();
  motion = new THREE.Vector2();
  dz = 0;
  uv = new THREE.Vector2();
  duv = new THREE.Vector2();
  canClick = false;
  dragging = false;
  ctrl = false;
  alt = false;
  shift = false;
  meta = false;
  constructor() {
    Object.seal(this);
  }
  copy(ptr) {
    this.buttons = ptr.buttons;
    this.moveDistance = ptr.moveDistance;
    this.dragDistance = ptr.dragDistance;
    this.position.copy(ptr.position);
    this.motion.copy(ptr.motion);
    this.dz = ptr.dz;
    this.uv.copy(ptr.uv);
    this.duv.copy(ptr.duv);
    this.canClick = ptr.canClick;
    this.dragging = ptr.dragging;
    this.ctrl = ptr.ctrl;
    this.alt = ptr.alt;
    this.shift = ptr.shift;
    this.meta = ptr.meta;
  }
};

// ../threejs/eventSystem/InteractiveObject3D.ts
function isCollider(obj2) {
  return isObject3D(obj2) && isBoolean(obj2.isCollider) && isDefined(obj2.parent);
}
function isInteractiveHit(hit) {
  return isDefined(hit) && isCollider(hit.object);
}
function isInteractiveObject3D(obj2) {
  return isObject3D(obj2) && (isBoolean(obj2.disabled) || isBoolean(obj2.isDraggable) || isBoolean(obj2.isClickable));
}
function checkClickable(obj2) {
  return isInteractiveObject3D(obj2) && obj2.isClickable && !obj2.disabled;
}
function isClickable(hit) {
  return isInteractiveHit(hit) && isCollider(hit.object) && (checkClickable(hit.object) || checkClickable(hit.object.parent));
}
function checkDraggable(obj2) {
  return isInteractiveObject3D(obj2) && obj2.isDraggable && !obj2.disabled;
}
function isDraggable(hit) {
  return isInteractiveHit(hit) && isCollider(hit.object) && (checkDraggable(hit.object) || checkDraggable(hit.object.parent));
}
function checkDisabled(obj2) {
  return isInteractiveObject3D(obj2) && obj2.disabled;
}
function isDisabled(hit) {
  return isInteractiveHit(hit) && isCollider(hit.object) && (checkDisabled(hit.object) || checkDisabled(hit.object.parent));
}

// ../threejs/eventSystem/BaseCursor.ts
var T = new THREE.Vector3();
var V = new THREE.Vector3();
var Q = new THREE.Quaternion();
var BaseCursor = class {
  constructor() {
    this._object = null;
    this._visible = true;
    this._style = "default";
  }
  get object() {
    return this._object;
  }
  set object(v) {
    this._object = v;
  }
  get style() {
    return this._style;
  }
  set style(v) {
    this._style = v;
  }
  get visible() {
    return this._visible;
  }
  set visible(v) {
    this._visible = v;
  }
  update(avatarHeadPos, hit, defaultDistance, canMoveView, state, origin, direction) {
    if (hit && hit.face) {
      this.position.copy(hit.point);
      hit.object.getWorldQuaternion(Q);
      T.copy(hit.face.normal).applyQuaternion(Q);
      V.copy(T).multiplyScalar(0.02);
      this.position.add(V);
      V.copy(T).multiplyScalar(10).add(this.position);
    } else {
      this.position.copy(direction).multiplyScalar(defaultDistance).add(origin);
      V.copy(avatarHeadPos);
    }
    this.object.parent.worldToLocal(this.position);
    this.lookAt(V);
    this.style = hit ? isDisabled(hit) ? "not-allowed" : isDraggable(hit) ? state.dragging ? "grabbing" : "move" : isClickable(hit) ? "pointer" : "default" : canMoveView ? state.buttons === 1 /* Mouse0 */ ? "grabbing" : "grab" : "default";
  }
  lookAt(_v) {
  }
};

// ../threejs/eventSystem/CursorColor.ts
var CursorColor = class extends BaseCursor {
  constructor() {
    super();
    this.material = solid({
      name: "CursorMat",
      color: 16776960
    });
    this.object = new Cube(0.01, 0.01, 0.01, this.material);
  }
  get position() {
    return this.object.position;
  }
  get style() {
    return this._currentStyle;
  }
  set style(v) {
    this._currentStyle = v;
    if (isMesh(this.object) && !isArray(this.object.material)) {
      switch (this._currentStyle) {
        case "pointer":
          this.material.color = new THREE.Color(65280);
          this.material.needsUpdate = true;
          break;
        case "not-allowed":
          this.material.color = new THREE.Color(16711680);
          this.material.needsUpdate = true;
          break;
        case "move":
          this.material.color = new THREE.Color(255);
          this.material.needsUpdate = true;
          break;
        case "grab":
          this.material.color = new THREE.Color(16711935);
          this.material.needsUpdate = true;
          break;
        case "grabbing":
          this.material.color = new THREE.Color(65535);
          this.material.needsUpdate = true;
          break;
        default:
          this._currentStyle = "default";
          this.material.color = new THREE.Color(16776960);
          this.material.needsUpdate = true;
          break;
      }
    }
  }
  get visible() {
    return objectIsVisible(this);
  }
  set visible(v) {
    objectSetVisible(this, v);
  }
};

// ../threejs/eventSystem/CursorSystem.ts
var CursorSystem = class extends BaseCursor {
  constructor(element) {
    super();
    this.element = element;
    this._hidden = false;
    this.visible = true;
    this.style = "default";
    document.addEventListener("pointerlockchange", () => {
      this._hidden = !!document.pointerLockElement;
      this.refresh();
    });
  }
  get position() {
    throw new Error("BaseCursor::get_Position(): Method not implemented.");
  }
  get style() {
    return super.style;
  }
  set style(v) {
    super.style = v;
    this.refresh();
  }
  get visible() {
    return super.visible && !this._hidden;
  }
  set visible(v) {
    super.visible = v;
    this.refresh();
  }
  refresh() {
    this.element.style.cursor = this.visible ? this.style : "none";
  }
};

// ../threejs/eventSystem/CursorXRMouse.ts
var CursorXRMouse = class extends BaseCursor {
  constructor(renderer) {
    super();
    this.renderer = renderer;
    this.xr = new CursorColor();
    this.system = new CursorSystem(this.renderer.domElement);
    this.visible = false;
  }
  get object() {
    return this.xr.object;
  }
  get cursor() {
    return this.xr;
  }
  set cursor(v) {
    this.xr = v;
    this._refresh();
  }
  get position() {
    return this.object.position;
  }
  get style() {
    return this.system.style;
  }
  get visible() {
    return this.renderer.xr.isPresenting && this.xr.visible || !this.renderer.xr.isPresenting && this.system.visible;
  }
  set visible(v) {
    super.visible = v;
    this._refresh();
  }
  set style(v) {
    this.system.style = v;
    this.xr.style = v;
    this._refresh();
  }
  _refresh() {
    objectSetVisible(this.xr, this.visible && (this.renderer.xr.isPresenting || document.pointerLockElement != null));
  }
  lookAt(v) {
    this.xr.lookAt(v);
  }
};

// ../threejs/eventSystem/BasePointer.ts
var MAX_DRAG_DISTANCE = 5;
var BasePointer = class {
  constructor(type2, name, evtSys, cursor) {
    this.type = type2;
    this.name = name;
    this.evtSys = evtSys;
    this._canMoveView = false;
    this._enabled = false;
    this.isActive = false;
    this.movementDragThreshold = MAX_DRAG_DISTANCE;
    this.state = new PointerState();
    this.lastState = null;
    this.origin = new THREE.Vector3();
    this.direction = new THREE.Vector3();
    this.curHit = null;
    this.hoveredHit = null;
    this._pressedHit = null;
    this.draggedHit = null;
    this._cursor = cursor;
    this.enabled = false;
    this.canMoveView = false;
  }
  get pressedHit() {
    return this._pressedHit;
  }
  set pressedHit(v) {
    this._pressedHit = v;
    if (isDraggable(v) && !isClickable(v)) {
      this.onDragStart();
    }
  }
  get canMoveView() {
    return this._canMoveView;
  }
  set canMoveView(v) {
    this._canMoveView = v;
  }
  vibrate() {
  }
  get cursor() {
    return this._cursor;
  }
  set cursor(newCursor) {
    if (newCursor !== this.cursor) {
      const oldCursor = this.cursor;
      const oldName = this.cursor && this.cursor.object && this.cursor.object.name || "cursor";
      const oldParent = oldCursor && oldCursor.object && oldCursor.object.parent;
      if (oldParent) {
        oldParent.remove(oldCursor.object);
      }
      if (newCursor) {
        newCursor.object.name = oldName;
        if (oldCursor instanceof CursorXRMouse) {
          oldCursor.cursor = newCursor;
          if (oldParent) {
            oldParent.add(oldCursor.object);
          }
        } else {
          this._cursor = newCursor;
          if (oldCursor) {
            if (oldParent) {
              oldParent.add(newCursor.object);
            }
            newCursor.style = oldCursor.style;
            newCursor.visible = oldCursor.visible;
          }
        }
      }
    }
  }
  get enabled() {
    return this._enabled;
  }
  set enabled(v) {
    this._enabled = v;
    if (this.cursor) {
      this.cursor.visible = v;
    }
  }
  get needsUpdate() {
    return this.enabled && this.isActive;
  }
  setEventState(type2) {
    this.evtSys.checkPointer(this, type2);
  }
  update() {
    this.onUpdate();
    if (!this.lastState) {
      this.lastState = new PointerState();
    }
    this.lastState.copy(this.state);
    this.state.motion.setScalar(0);
    this.state.dz = 0;
    this.state.duv.setScalar(0);
  }
  updateCursor(avatarHeadPos, curHit, defaultDistance) {
    if (this.cursor) {
      this.cursor.update(avatarHeadPos, curHit, defaultDistance, this.canMoveView, this.state, this.origin, this.direction);
    }
  }
  onPointerDown() {
    this.state.dragging = false;
    this.state.canClick = true;
    this.setEventState("down");
  }
  onPointerMove() {
    this.setEventState("move");
    if (this.state.buttons !== 0 /* None */) {
      const canDrag = isNullOrUndefined(this.pressedHit) || isDraggable(this.pressedHit);
      if (canDrag) {
        if (this.lastState && this.lastState.buttons === this.state.buttons) {
          this.state.dragDistance += this.state.moveDistance;
          if (this.state.dragDistance > this.movementDragThreshold) {
            this.onDragStart();
          }
        } else if (this.state.dragging) {
          this.state.dragging = false;
          this.setEventState("dragcancel");
        }
      }
    }
  }
  onDragStart() {
    this.state.dragging = true;
    if (this.lastState && !this.lastState.dragging) {
      this.setEventState("dragstart");
    }
    this.state.canClick = false;
    this.setEventState("drag");
  }
  onPointerUp() {
    if (this.state.canClick && this.lastState) {
      const lastButtons = this.state.buttons;
      this.state.buttons = this.lastState.buttons;
      this.setEventState("click");
      this.state.buttons = lastButtons;
    }
    this.setEventState("up");
    this.state.dragDistance = 0;
    this.state.dragging = false;
    if (this.lastState && this.lastState.dragging) {
      this.setEventState("dragend");
    }
  }
};

// ../threejs/examples/lines/LineSegmentsGeometry.js
var _box = new THREE.Box3();
var _vector = new THREE.Vector3();
var LineSegmentsGeometry = class extends THREE.InstancedBufferGeometry {
  constructor() {
    super();
    this.type = "LineSegmentsGeometry";
    const positions = [-1, 2, 0, 1, 2, 0, -1, 1, 0, 1, 1, 0, -1, 0, 0, 1, 0, 0, -1, -1, 0, 1, -1, 0];
    const uvs = [-1, 2, 1, 2, -1, 1, 1, 1, -1, -1, 1, -1, -1, -2, 1, -2];
    const index = [0, 2, 1, 2, 3, 1, 2, 4, 3, 4, 5, 3, 4, 6, 5, 6, 7, 5];
    this.setIndex(index);
    this.setAttribute("position", new THREE.Float32BufferAttribute(positions, 3));
    this.setAttribute("uv", new THREE.Float32BufferAttribute(uvs, 2));
  }
  applyMatrix4(matrix) {
    const start = this.attributes.instanceStart;
    const end = this.attributes.instanceEnd;
    if (start !== void 0) {
      start.applyMatrix4(matrix);
      end.applyMatrix4(matrix);
      start.needsUpdate = true;
    }
    if (this.boundingBox !== null) {
      this.computeBoundingBox();
    }
    if (this.boundingSphere !== null) {
      this.computeBoundingSphere();
    }
    return this;
  }
  setPositions(array) {
    let lineSegments;
    if (array instanceof Float32Array) {
      lineSegments = array;
    } else if (Array.isArray(array)) {
      lineSegments = new Float32Array(array);
    }
    const instanceBuffer = new THREE.InstancedInterleavedBuffer(lineSegments, 6, 1);
    this.setAttribute("instanceStart", new THREE.InterleavedBufferAttribute(instanceBuffer, 3, 0));
    this.setAttribute("instanceEnd", new THREE.InterleavedBufferAttribute(instanceBuffer, 3, 3));
    this.computeBoundingBox();
    this.computeBoundingSphere();
    return this;
  }
  setColors(array) {
    let colors;
    if (array instanceof Float32Array) {
      colors = array;
    } else if (Array.isArray(array)) {
      colors = new Float32Array(array);
    }
    const instanceColorBuffer = new THREE.InstancedInterleavedBuffer(colors, 6, 1);
    this.setAttribute("instanceColorStart", new THREE.InterleavedBufferAttribute(instanceColorBuffer, 3, 0));
    this.setAttribute("instanceColorEnd", new THREE.InterleavedBufferAttribute(instanceColorBuffer, 3, 3));
    return this;
  }
  fromWireframeGeometry(geometry) {
    this.setPositions(geometry.attributes.position.array);
    return this;
  }
  fromEdgesGeometry(geometry) {
    this.setPositions(geometry.attributes.position.array);
    return this;
  }
  fromMesh(mesh) {
    this.fromWireframeGeometry(new THREE.WireframeGeometry(mesh.geometry));
    return this;
  }
  fromLineSegments(lineSegments) {
    const geometry = lineSegments.geometry;
    if (geometry.isGeometry) {
      console.error("THREE.LineSegmentsGeometry no longer supports Geometry. Use THREE.BufferGeometry instead.");
      return;
    } else if (geometry.isBufferGeometry) {
      this.setPositions(geometry.attributes.position.array);
    }
    return this;
  }
  computeBoundingBox() {
    if (this.boundingBox === null) {
      this.boundingBox = new THREE.Box3();
    }
    const start = this.attributes.instanceStart;
    const end = this.attributes.instanceEnd;
    if (start !== void 0 && end !== void 0) {
      this.boundingBox.setFromBufferAttribute(start);
      _box.setFromBufferAttribute(end);
      this.boundingBox.union(_box);
    }
  }
  computeBoundingSphere() {
    if (this.boundingSphere === null) {
      this.boundingSphere = new THREE.Sphere();
    }
    if (this.boundingBox === null) {
      this.computeBoundingBox();
    }
    const start = this.attributes.instanceStart;
    const end = this.attributes.instanceEnd;
    if (start !== void 0 && end !== void 0) {
      const center = this.boundingSphere.center;
      this.boundingBox.getCenter(center);
      let maxRadiusSq = 0;
      for (let i = 0, il = start.count; i < il; i++) {
        _vector.fromBufferAttribute(start, i);
        maxRadiusSq = Math.max(maxRadiusSq, center.distanceToSquared(_vector));
        _vector.fromBufferAttribute(end, i);
        maxRadiusSq = Math.max(maxRadiusSq, center.distanceToSquared(_vector));
      }
      this.boundingSphere.radius = Math.sqrt(maxRadiusSq);
      if (isNaN(this.boundingSphere.radius)) {
        console.error("THREE.LineSegmentsGeometry.computeBoundingSphere(): Computed radius is NaN. The instanced position data is likely to have NaN values.", this);
      }
    }
  }
  toJSON() {
  }
  applyMatrix(matrix) {
    console.warn("THREE.LineSegmentsGeometry: applyMatrix() has been renamed to applyMatrix4().");
    return this.applyMatrix4(matrix);
  }
};
LineSegmentsGeometry.prototype.isLineSegmentsGeometry = true;

// ../threejs/examples/lines/LineSegments2.js
var _start = new THREE.Vector3();
var _end = new THREE.Vector3();
var _start4 = new THREE.Vector4();
var _end4 = new THREE.Vector4();
var _ssOrigin = new THREE.Vector4();
var _ssOrigin3 = new THREE.Vector3();
var _mvMatrix = new THREE.Matrix4();
var _line = new THREE.Line3();
var _closestPoint = new THREE.Vector3();
var _box2 = new THREE.Box3();
var _sphere = new THREE.Sphere();
var _clipToWorldVector = new THREE.Vector4();
function getWorldSpaceHalfWidth(camera, distance2, lineWidth, resolution) {
  _clipToWorldVector.set(0, 0, -distance2, 1).applyMatrix4(camera.projectionMatrix);
  _clipToWorldVector.multiplyScalar(1 / _clipToWorldVector.w);
  _clipToWorldVector.x = lineWidth / resolution.width;
  _clipToWorldVector.y = lineWidth / resolution.height;
  _clipToWorldVector.applyMatrix4(camera.projectionMatrixInverse);
  _clipToWorldVector.multiplyScalar(1 / _clipToWorldVector.w);
  return Math.abs(Math.max(_clipToWorldVector.x, _clipToWorldVector.y));
}
var LineSegments2 = class extends THREE.Mesh {
  constructor(geometry = new LineSegmentsGeometry(), material = new LineMaterial({ color: Math.random() * 16777215 })) {
    super(geometry, material);
    this.type = "LineSegments2";
  }
  computeLineDistances() {
    const geometry = this.geometry;
    const instanceStart = geometry.attributes.instanceStart;
    const instanceEnd = geometry.attributes.instanceEnd;
    const lineDistances = new Float32Array(2 * instanceStart.count);
    for (let i = 0, j = 0, l = instanceStart.count; i < l; i++, j += 2) {
      _start.fromBufferAttribute(instanceStart, i);
      _end.fromBufferAttribute(instanceEnd, i);
      lineDistances[j] = j === 0 ? 0 : lineDistances[j - 1];
      lineDistances[j + 1] = lineDistances[j] + _start.distanceTo(_end);
    }
    const instanceDistanceBuffer = new THREE.InstancedInterleavedBuffer(lineDistances, 2, 1);
    geometry.setAttribute("instanceDistanceStart", new THREE.InterleavedBufferAttribute(instanceDistanceBuffer, 1, 0));
    geometry.setAttribute("instanceDistanceEnd", new THREE.InterleavedBufferAttribute(instanceDistanceBuffer, 1, 1));
    return this;
  }
  raycast(raycaster, intersects) {
    if (raycaster.camera === null) {
      console.error('LineSegments2: "Raycaster.camera" needs to be set in order to raycast against LineSegments2.');
    }
    const threshold = raycaster.params.Line2 !== void 0 ? raycaster.params.Line2.threshold || 0 : 0;
    const ray = raycaster.ray;
    const camera = raycaster.camera;
    const projectionMatrix = camera.projectionMatrix;
    const matrixWorld = this.matrixWorld;
    const geometry = this.geometry;
    const material = this.material;
    const resolution = material.resolution;
    const lineWidth = material.linewidth + threshold;
    const instanceStart = geometry.attributes.instanceStart;
    const instanceEnd = geometry.attributes.instanceEnd;
    const near = -camera.near;
    if (geometry.boundingSphere === null) {
      geometry.computeBoundingSphere();
    }
    _sphere.copy(geometry.boundingSphere).applyMatrix4(matrixWorld);
    const distanceToSphere = Math.max(camera.near, _sphere.distanceToPoint(ray.origin));
    const sphereMargin = getWorldSpaceHalfWidth(camera, distanceToSphere, lineWidth, resolution);
    _sphere.radius += sphereMargin;
    if (raycaster.ray.intersectsSphere(_sphere) === false) {
      return;
    }
    if (geometry.boundingBox === null) {
      geometry.computeBoundingBox();
    }
    _box2.copy(geometry.boundingBox).applyMatrix4(matrixWorld);
    const distanceToBox = Math.max(camera.near, _box2.distanceToPoint(ray.origin));
    const boxMargin = getWorldSpaceHalfWidth(camera, distanceToBox, lineWidth, resolution);
    _box2.max.x += boxMargin;
    _box2.max.y += boxMargin;
    _box2.max.z += boxMargin;
    _box2.min.x -= boxMargin;
    _box2.min.y -= boxMargin;
    _box2.min.z -= boxMargin;
    if (raycaster.ray.intersectsBox(_box2) === false) {
      return;
    }
    ray.at(1, _ssOrigin);
    _ssOrigin.w = 1;
    _ssOrigin.applyMatrix4(camera.matrixWorldInverse);
    _ssOrigin.applyMatrix4(projectionMatrix);
    _ssOrigin.multiplyScalar(1 / _ssOrigin.w);
    _ssOrigin.x *= resolution.x / 2;
    _ssOrigin.y *= resolution.y / 2;
    _ssOrigin.z = 0;
    _ssOrigin3.copy(_ssOrigin);
    _mvMatrix.multiplyMatrices(camera.matrixWorldInverse, matrixWorld);
    for (let i = 0, l = instanceStart.count; i < l; i++) {
      _start4.fromBufferAttribute(instanceStart, i);
      _end4.fromBufferAttribute(instanceEnd, i);
      _start4.w = 1;
      _end4.w = 1;
      _start4.applyMatrix4(_mvMatrix);
      _end4.applyMatrix4(_mvMatrix);
      var isBehindCameraNear = _start4.z > near && _end4.z > near;
      if (isBehindCameraNear) {
        continue;
      }
      if (_start4.z > near) {
        const deltaDist = _start4.z - _end4.z;
        const t2 = (_start4.z - near) / deltaDist;
        _start4.lerp(_end4, t2);
      } else if (_end4.z > near) {
        const deltaDist = _end4.z - _start4.z;
        const t2 = (_end4.z - near) / deltaDist;
        _end4.lerp(_start4, t2);
      }
      _start4.applyMatrix4(projectionMatrix);
      _end4.applyMatrix4(projectionMatrix);
      _start4.multiplyScalar(1 / _start4.w);
      _end4.multiplyScalar(1 / _end4.w);
      _start4.x *= resolution.x / 2;
      _start4.y *= resolution.y / 2;
      _end4.x *= resolution.x / 2;
      _end4.y *= resolution.y / 2;
      _line.start.copy(_start4);
      _line.start.z = 0;
      _line.end.copy(_end4);
      _line.end.z = 0;
      const param = _line.closestPointToPointParameter(_ssOrigin3, true);
      _line.at(param, _closestPoint);
      const zPos = THREE.MathUtils.lerp(_start4.z, _end4.z, param);
      const isInClipSpace = zPos >= -1 && zPos <= 1;
      const isInside = _ssOrigin3.distanceTo(_closestPoint) < lineWidth * 0.5;
      if (isInClipSpace && isInside) {
        _line.start.fromBufferAttribute(instanceStart, i);
        _line.end.fromBufferAttribute(instanceEnd, i);
        _line.start.applyMatrix4(matrixWorld);
        _line.end.applyMatrix4(matrixWorld);
        const pointOnLine = new THREE.Vector3();
        const point = new THREE.Vector3();
        ray.distanceSqToSegment(_line.start, _line.end, point, pointOnLine);
        intersects.push({
          point,
          pointOnLine,
          distance: ray.origin.distanceTo(point),
          object: this,
          face: null,
          faceIndex: i,
          uv: null,
          uv2: null
        });
      }
    }
  }
};
LineSegments2.prototype.isLineSegments2 = true;

// ../threejs/examples/lines/LineGeometry.js
var LineGeometry = class extends LineSegmentsGeometry {
  constructor() {
    super();
    this.type = "LineGeometry";
  }
  setPositions(array) {
    var length2 = array.length - 3;
    var points = new Float32Array(2 * length2);
    for (var i = 0; i < length2; i += 3) {
      points[2 * i] = array[i];
      points[2 * i + 1] = array[i + 1];
      points[2 * i + 2] = array[i + 2];
      points[2 * i + 3] = array[i + 3];
      points[2 * i + 4] = array[i + 4];
      points[2 * i + 5] = array[i + 5];
    }
    super.setPositions(points);
    return this;
  }
  setColors(array) {
    var length2 = array.length - 3;
    var colors = new Float32Array(2 * length2);
    for (var i = 0; i < length2; i += 3) {
      colors[2 * i] = array[i];
      colors[2 * i + 1] = array[i + 1];
      colors[2 * i + 2] = array[i + 2];
      colors[2 * i + 3] = array[i + 3];
      colors[2 * i + 4] = array[i + 4];
      colors[2 * i + 5] = array[i + 5];
    }
    super.setColors(colors);
    return this;
  }
  fromLine(line) {
    var geometry = line.geometry;
    if (geometry.isGeometry) {
      console.error("THREE.LineGeometry no longer supports Geometry. Use THREE.BufferGeometry instead.");
      return;
    } else if (geometry.isBufferGeometry) {
      this.setPositions(geometry.attributes.position.array);
    }
    return this;
  }
};
LineGeometry.prototype.isLineGeometry = true;

// ../threejs/examples/lines/Line2.js
var Line2 = class extends LineSegments2 {
  constructor(geometry = new LineGeometry(), material = new LineMaterial({ color: Math.random() * 16777215 })) {
    super(geometry, material);
    this.type = "Line2";
  }
};
Line2.prototype.isLine2 = true;

// ../threejs/eventSystem/Laser.ts
var geom = new LineGeometry();
geom.setPositions([
  0,
  0,
  0,
  0,
  0,
  -1
]);
var Laser = class extends THREE.Object3D {
  constructor(color, linewidth = 1) {
    super();
    this._length = 1;
    this.line = new Line2(geom, line2({
      color,
      linewidth
    }));
    this.line.computeLineDistances();
    this.add(this.line);
  }
  get length() {
    return this._length;
  }
  set length(v) {
    this._length = v;
    this.line.scale.set(1, 1, v);
  }
};

// ../threejs/eventSystem/PointerRemote.ts
var PointerRemote = class extends BasePointer {
  constructor(evtSys, userName, isInstructor, pointerName, cursor) {
    super("remote", 18 /* RemoteUser */, evtSys, cursor || new CursorColor());
    this.pTarget = new THREE.Vector3();
    this.qTarget = new THREE.Quaternion().identity();
    this.laser = new Laser(isInstructor ? green : yellow, 2e-3);
    this.laser.length = 30;
    const hand = new Cube(0.05, 0.05, 0.05, litGrey);
    hand.add(this.laser);
    const elbow = new Cube(0.05, 0.05, 0.05, litGrey);
    this.object = obj(`${userName}-arm`, hand, elbow);
    if (pointerName === 1 /* Mouse */) {
      hand.position.set(0, 0, -0.2);
    } else if (pointerName === 15 /* MotionController */ || pointerName === 16 /* MotionControllerLeft */ || pointerName === 17 /* MotionControllerRight */) {
      elbow.position.set(0, 0, 0.2);
    }
    this.object.name = `remote:${userName}:${PointerName[pointerName]}`;
    this.cursor.object.name = `${this.object.name}:cursor`;
  }
  setState(avatarHeadPos, position, forward, up, offset) {
    this.origin.copy(position);
    this.direction.copy(forward);
    this.cursor.visible = true;
    this.evtSys.fireRay(this);
    this.updateCursor(avatarHeadPos, this.curHit, 3);
    position.add(offset);
    if (this.curHit) {
      forward.copy(this.curHit.point).sub(position).normalize();
    }
    setMatrixFromUpFwdPos(up, forward, position, MW);
    M.copy(this.object.parent.matrixWorld).invert().multiply(MW).decompose(this.pTarget, this.qTarget, dumpS);
  }
  onUpdate() {
  }
  animate(dt) {
    this.object.position.lerp(this.pTarget, dt * 0.01);
    this.object.quaternion.slerp(this.qTarget, dt * 0.01);
  }
  isPressed(_button) {
    return false;
  }
};
var dumpS = new THREE.Vector3();
var M = new THREE.Matrix4();
var MW = new THREE.Matrix4();

// ../dom/canvas.ts
var hasHTMLCanvas = "HTMLCanvasElement" in globalThis;
var hasHTMLImage = "HTMLImageElement" in globalThis;
var disableAdvancedSettings = false;
var hasOffscreenCanvas = !disableAdvancedSettings && "OffscreenCanvas" in globalThis;
var hasImageBitmap = !disableAdvancedSettings && "createImageBitmap" in globalThis;
function isHTMLCanvas(obj2) {
  return hasHTMLCanvas && obj2 instanceof HTMLCanvasElement;
}
function isOffscreenCanvas(obj2) {
  return hasOffscreenCanvas && obj2 instanceof OffscreenCanvas;
}
function isImageBitmap(img) {
  return hasImageBitmap && img instanceof ImageBitmap;
}
function drawImageBitmapToCanvas2D(canv, img) {
  const g = canv.getContext("2d");
  if (isNullOrUndefined(g)) {
    throw new Error("Could not create 2d context for canvas");
  }
  g.drawImage(img, 0, 0);
}
function testOffscreen2D() {
  try {
    const canv = new OffscreenCanvas(1, 1);
    const g = canv.getContext("2d");
    return g != null;
  } catch (exp) {
    return false;
  }
}
var hasOffscreenCanvasRenderingContext2D = hasOffscreenCanvas && testOffscreen2D();
var createUtilityCanvas = hasOffscreenCanvasRenderingContext2D && createOffscreenCanvas || hasHTMLCanvas && createCanvas || null;
var createUICanvas = hasHTMLCanvas ? createCanvas : createUtilityCanvas;
function testOffscreen3D() {
  try {
    const canv = new OffscreenCanvas(1, 1);
    const g = canv.getContext("webgl2");
    return g != null;
  } catch (exp) {
    return false;
  }
}
var hasOffscreenCanvasRenderingContext3D = hasOffscreenCanvas && testOffscreen3D();
function testBitmapRenderer() {
  if (!hasHTMLCanvas && !hasOffscreenCanvas) {
    return false;
  }
  try {
    const canv = createUtilityCanvas(1, 1);
    const g = canv.getContext("bitmaprenderer");
    return g != null;
  } catch (exp) {
    return false;
  }
}
var hasImageBitmapRenderingContext = hasImageBitmap && testBitmapRenderer();
function createOffscreenCanvas(width, height) {
  return new OffscreenCanvas(width, height);
}
function createCanvas(w, h) {
  return Canvas(htmlWidth(w), htmlHeight(h));
}
function createCanvasFromImageBitmap(img) {
  const canv = createCanvas(img.width, img.height);
  drawImageBitmapToCanvas2D(canv, img);
  return canv;
}
function setCanvasSize(canv, w, h, superscale = 1) {
  w = Math.floor(w * superscale);
  h = Math.floor(h * superscale);
  if (canv.width != w || canv.height != h) {
    canv.width = w;
    canv.height = h;
    return true;
  }
  return false;
}
function is2DRenderingContext(ctx) {
  return isDefined(ctx.textBaseline);
}
function setCanvas2DContextSize(ctx, w, h, superscale = 1) {
  const oldImageSmoothingEnabled = ctx.imageSmoothingEnabled, oldTextBaseline = ctx.textBaseline, oldTextAlign = ctx.textAlign, oldFont = ctx.font, resized = setCanvasSize(ctx.canvas, w, h, superscale);
  if (resized) {
    ctx.imageSmoothingEnabled = oldImageSmoothingEnabled;
    ctx.textBaseline = oldTextBaseline;
    ctx.textAlign = oldTextAlign;
    ctx.font = oldFont;
  }
  return resized;
}
function setContextSize(ctx, w, h, superscale = 1) {
  if (is2DRenderingContext(ctx)) {
    return setCanvas2DContextSize(ctx, w, h, superscale);
  } else {
    return setCanvasSize(ctx.canvas, w, h, superscale);
  }
}

// ../dom/fonts.ts
var loadedFonts = singleton("juniper::loadedFonts", () => []);
function makeFont(style) {
  const fontParts = [];
  if (style.fontStyle && style.fontStyle !== "normal") {
    fontParts.push(style.fontStyle);
  }
  if (style.fontVariant && style.fontVariant !== "normal") {
    fontParts.push(style.fontVariant);
  }
  if (style.fontWeight && style.fontWeight !== "normal") {
    fontParts.push(style.fontWeight);
  }
  fontParts.push(`${style.fontSize}px`);
  fontParts.push(style.fontFamily);
  return fontParts.join(" ");
}

// ../graphics2d/CanvasImage.ts
var CanvasImage = class extends TypedEventBase {
  constructor(width, height, options) {
    super();
    this._scale = 250;
    this._visible = true;
    this.wasVisible = null;
    this.redrawnEvt = new TypedEvent("redrawn");
    this.element = null;
    if (isDefined(options)) {
      if (isDefined(options.scale)) {
        this._scale = options.scale;
      }
    }
    this._canvas = createUICanvas(width, height);
    this._g = this.canvas.getContext("2d");
    if (isHTMLCanvas(this._canvas)) {
      this.element = this._canvas;
    }
  }
  fillRect(color, x, y, width, height, margin2) {
    this.g.fillStyle = color;
    this.g.fillRect(x + margin2, y + margin2, width - 2 * margin2, height - 2 * margin2);
  }
  drawText(text, x, y, align) {
    this.g.textAlign = align;
    this.g.strokeText(text, x, y);
    this.g.fillText(text, x, y);
  }
  redraw() {
    if ((this.visible || this.wasVisible) && this.onRedraw()) {
      this.wasVisible = this.visible;
      this.dispatchEvent(this.redrawnEvt);
    }
  }
  get canvas() {
    return this._canvas;
  }
  get g() {
    return this._g;
  }
  get imageWidth() {
    return this.canvas.width;
  }
  get imageHeight() {
    return this.canvas.height;
  }
  get aspectRatio() {
    return this.imageWidth / this.imageHeight;
  }
  get width() {
    return this.imageWidth / this.scale;
  }
  get height() {
    return this.imageHeight / this.scale;
  }
  get scale() {
    return this._scale;
  }
  set scale(v) {
    if (this.scale !== v) {
      this._scale = v;
      this.redraw();
    }
  }
  get visible() {
    return this._visible;
  }
  set visible(v) {
    if (this.visible !== v) {
      this.wasVisible = this._visible;
      this._visible = v;
      this.redraw();
    }
  }
};

// ../graphics2d/TextImage.ts
var TextImage = class extends CanvasImage {
  trueWidth = null;
  trueHeight = null;
  trueFontSize = null;
  dx = null;
  _minWidth = null;
  _maxWidth = null;
  _minHeight = null;
  _maxHeight = null;
  _freezeDimensions = false;
  _dimensionsFrozen = false;
  _bgFillColor = null;
  _bgStrokeColor = null;
  _bgStrokeSize = null;
  _textStrokeColor = null;
  _textStrokeSize = null;
  _textFillColor = "black";
  _textDirection = "horizontal";
  _wrapWords = true;
  _fontStyle = "normal";
  _fontVariant = "normal";
  _fontWeight = "normal";
  _fontFamily = "sans-serif";
  _fontSize = 20;
  _padding;
  _value = null;
  constructor(options) {
    super(10, 10, options);
    if (isDefined(options)) {
      if (isDefined(options.minWidth)) {
        this._minWidth = options.minWidth;
      }
      if (isDefined(options.maxWidth)) {
        this._maxWidth = options.maxWidth;
      }
      if (isDefined(options.minHeight)) {
        this._minHeight = options.minHeight;
      }
      if (isDefined(options.maxHeight)) {
        this._maxHeight = options.maxHeight;
      }
      if (isDefined(options.freezeDimensions)) {
        this._freezeDimensions = options.freezeDimensions;
      }
      if (isDefined(options.textStrokeColor)) {
        this._textStrokeColor = options.textStrokeColor;
      }
      if (isDefined(options.textStrokeSize)) {
        this._textStrokeSize = options.textStrokeSize;
      }
      if (isDefined(options.bgFillColor)) {
        this._bgFillColor = options.bgFillColor;
      }
      if (isDefined(options.bgStrokeColor)) {
        this._bgStrokeColor = options.bgStrokeColor;
      }
      if (isDefined(options.bgStrokeSize)) {
        this._bgStrokeSize = options.bgStrokeSize;
      }
      if (isDefined(options.value)) {
        this._value = options.value;
      }
      if (isDefined(options.textFillColor)) {
        this._textFillColor = options.textFillColor;
      }
      if (isDefined(options.textDirection)) {
        this._textDirection = options.textDirection;
      }
      if (isDefined(options.wrapWords)) {
        this._wrapWords = options.wrapWords;
      }
      if (isDefined(options.fontStyle)) {
        this._fontStyle = options.fontStyle;
      }
      if (isDefined(options.fontVariant)) {
        this._fontVariant = options.fontVariant;
      }
      if (isDefined(options.fontWeight)) {
        this._fontWeight = options.fontWeight;
      }
      if (isDefined(options.fontFamily)) {
        this._fontFamily = options.fontFamily;
      }
      if (isDefined(options.fontSize)) {
        this._fontSize = options.fontSize;
      }
      if (isDefined(options.padding)) {
        if (isNumber(options.padding)) {
          this._padding = {
            left: options.padding,
            right: options.padding,
            top: options.padding,
            bottom: options.padding
          };
        } else {
          this._padding = options.padding;
        }
      }
    }
    if (isNullOrUndefined(this._padding)) {
      this._padding = {
        top: 0,
        right: 0,
        bottom: 0,
        left: 0
      };
    }
    this.redraw();
  }
  get minWidth() {
    return this._minWidth;
  }
  set minWidth(v) {
    if (this.minWidth !== v) {
      this._minWidth = v;
      this.redraw();
    }
  }
  get maxWidth() {
    return this._maxWidth;
  }
  set maxWidth(v) {
    if (this.maxWidth !== v) {
      this._maxWidth = v;
      this.redraw();
    }
  }
  get minHeight() {
    return this._minHeight;
  }
  set minHeight(v) {
    if (this.minHeight !== v) {
      this._minHeight = v;
      this.redraw();
    }
  }
  get maxHeight() {
    return this._maxHeight;
  }
  set maxHeight(v) {
    if (this.maxHeight !== v) {
      this._maxHeight = v;
      this.redraw();
    }
  }
  get padding() {
    return this._padding;
  }
  set padding(v) {
    if (v instanceof Array) {
      throw new Error("Invalid padding");
    }
    if (this.padding.top !== v.top || this.padding.right != v.right || this.padding.bottom != v.bottom || this.padding.left != v.left) {
      this._padding = v;
      this.redraw();
    }
  }
  get wrapWords() {
    return this._wrapWords;
  }
  set wrapWords(v) {
    if (this.wrapWords !== v) {
      this._wrapWords = v;
      this.redraw();
    }
  }
  get textDirection() {
    return this._textDirection;
  }
  set textDirection(v) {
    if (this.textDirection !== v) {
      this._textDirection = v;
      this.redraw();
    }
  }
  get fontStyle() {
    return this._fontStyle;
  }
  set fontStyle(v) {
    if (this.fontStyle !== v) {
      this._fontStyle = v;
      this.redraw();
    }
  }
  get fontVariant() {
    return this._fontVariant;
  }
  set fontVariant(v) {
    if (this.fontVariant !== v) {
      this._fontVariant = v;
      this.redraw();
    }
  }
  get fontWeight() {
    return this._fontWeight;
  }
  set fontWeight(v) {
    if (this.fontWeight !== v) {
      this._fontWeight = v;
      this.redraw();
    }
  }
  get fontSize() {
    return this._fontSize;
  }
  set fontSize(v) {
    if (this.fontSize !== v) {
      this._fontSize = v;
      this.redraw();
    }
  }
  get fontFamily() {
    return this._fontFamily;
  }
  set fontFamily(v) {
    if (this.fontFamily !== v) {
      this._fontFamily = v;
      this.redraw();
    }
  }
  get textFillColor() {
    return this._textFillColor;
  }
  set textFillColor(v) {
    if (this.textFillColor !== v) {
      this._textFillColor = v;
      this.redraw();
    }
  }
  get textStrokeColor() {
    return this._textStrokeColor;
  }
  set textStrokeColor(v) {
    if (this.textStrokeColor !== v) {
      this._textStrokeColor = v;
      this.redraw();
    }
  }
  get textStrokeSize() {
    return this._textStrokeSize;
  }
  set textStrokeSize(v) {
    if (this.textStrokeSize !== v) {
      this._textStrokeSize = v;
      this.redraw();
    }
  }
  get bgFillColor() {
    return this._bgFillColor;
  }
  set bgFillColor(v) {
    if (this.bgFillColor !== v) {
      this._bgFillColor = v;
      this.redraw();
    }
  }
  get bgStrokeColor() {
    return this._bgStrokeColor;
  }
  set bgStrokeColor(v) {
    if (this.bgStrokeColor !== v) {
      this._bgStrokeColor = v;
      this.redraw();
    }
  }
  get bgStrokeSize() {
    return this._bgStrokeSize;
  }
  set bgStrokeSize(v) {
    if (this.bgStrokeSize !== v) {
      this._bgStrokeSize = v;
      this.redraw();
    }
  }
  get value() {
    return this._value;
  }
  set value(v) {
    if (this.value !== v) {
      this._value = v;
      this.redraw();
    }
  }
  draw(g, x, y) {
    if (this.canvas.width > 0 && this.canvas.height > 0) {
      g.drawImage(this.canvas, x, y, this.width, this.height);
    }
  }
  split(value) {
    if (this.wrapWords) {
      return value.split(" ").join("\n").replace(/\r\n/, "\n").split("\n");
    } else {
      return value.replace(/\r\n/, "\n").split("\n");
    }
  }
  unfreeze() {
    this._dimensionsFrozen = false;
  }
  onRedraw() {
    this.g.clearRect(0, 0, this.canvas.width, this.canvas.height);
    if (this.visible && this.fontFamily && this.fontSize && (this.textFillColor || this.textStrokeColor && this.textStrokeSize) && this.value) {
      const lines = this.split(this.value);
      const isVertical = this.textDirection && this.textDirection.indexOf("vertical") === 0;
      if (this.trueWidth === null || this.trueHeight === null || this.dx === null || this.trueFontSize === null || !this._dimensionsFrozen) {
        this._dimensionsFrozen = this._freezeDimensions;
        const autoResize = this.minWidth != null || this.maxWidth != null || this.minHeight != null || this.maxHeight != null;
        const _targetMinWidth = ((this.minWidth || 0) - this.padding.right - this.padding.left) * this.scale;
        const _targetMaxWidth = ((this.maxWidth || 4096) - this.padding.right - this.padding.left) * this.scale;
        const _targetMinHeight = ((this.minHeight || 0) - this.padding.top - this.padding.bottom) * this.scale;
        const _targetMaxHeight = ((this.maxHeight || 4096) - this.padding.top - this.padding.bottom) * this.scale;
        const targetMinWidth = isVertical ? _targetMinHeight : _targetMinWidth;
        const targetMaxWidth = isVertical ? _targetMaxHeight : _targetMaxWidth;
        const targetMinHeight = isVertical ? _targetMinWidth : _targetMinHeight;
        const targetMaxHeight = isVertical ? _targetMaxWidth : _targetMaxHeight;
        const tried = [];
        this.trueWidth = 0;
        this.trueHeight = 0;
        this.dx = 0;
        let tooBig = false, tooSmall = false, highFontSize = 1e4, lowFontSize = 0;
        this.trueFontSize = clamp(this.fontSize * this.scale, lowFontSize, highFontSize);
        let minFont = null, minFontDelta = Number.MAX_VALUE;
        do {
          const realFontSize = this.fontSize;
          this._fontSize = this.trueFontSize;
          const font = makeFont(this);
          this._fontSize = realFontSize;
          this.g.textAlign = "center";
          this.g.textBaseline = "middle";
          this.g.font = font;
          this.trueWidth = 0;
          this.trueHeight = 0;
          for (const line of lines) {
            const metrics = this.g.measureText(line);
            this.trueWidth = Math.max(this.trueWidth, metrics.width);
            this.trueHeight += this.trueFontSize;
            if (isNumber(metrics.actualBoundingBoxLeft) && isNumber(metrics.actualBoundingBoxRight) && isNumber(metrics.actualBoundingBoxAscent) && isNumber(metrics.actualBoundingBoxDescent)) {
              if (!autoResize) {
                this.trueWidth = metrics.actualBoundingBoxLeft + metrics.actualBoundingBoxRight;
                this.trueHeight = metrics.actualBoundingBoxAscent + metrics.actualBoundingBoxDescent;
                this.dx = (metrics.actualBoundingBoxLeft - this.trueWidth / 2) / 2;
              }
            }
          }
          if (autoResize) {
            const dMinWidth = this.trueWidth - targetMinWidth;
            const dMaxWidth = this.trueWidth - targetMaxWidth;
            const dMinHeight = this.trueHeight - targetMinHeight;
            const dMaxHeight = this.trueHeight - targetMaxHeight;
            const mdMinWidth = Math.abs(dMinWidth);
            const mdMaxWidth = Math.abs(dMaxWidth);
            const mdMinHeight = Math.abs(dMinHeight);
            const mdMaxHeight = Math.abs(dMaxHeight);
            tooBig = dMaxWidth > 1 || dMaxHeight > 1;
            tooSmall = dMinWidth < -1 && dMinHeight < -1;
            const minDif = Math.min(mdMinWidth, Math.min(mdMaxWidth, Math.min(mdMinHeight, mdMaxHeight)));
            if (minDif < minFontDelta) {
              minFontDelta = minDif;
              minFont = this.g.font;
            }
            if ((tooBig || tooSmall) && tried.indexOf(this.g.font) > -1 && minFont) {
              this.g.font = minFont;
              tooBig = false;
              tooSmall = false;
            }
            if (tooBig) {
              highFontSize = this.trueFontSize;
              this.trueFontSize = (lowFontSize + this.trueFontSize) / 2;
            } else if (tooSmall) {
              lowFontSize = this.trueFontSize;
              this.trueFontSize = (this.trueFontSize + highFontSize) / 2;
            }
          }
          tried.push(this.g.font);
        } while (tooBig || tooSmall);
        if (autoResize) {
          if (this.trueWidth < targetMinWidth) {
            this.trueWidth = targetMinWidth;
          } else if (this.trueWidth > targetMaxWidth) {
            this.trueWidth = targetMaxWidth;
          }
          if (this.trueHeight < targetMinHeight) {
            this.trueHeight = targetMinHeight;
          } else if (this.trueHeight > targetMaxHeight) {
            this.trueHeight = targetMaxHeight;
          }
        }
        const newW = this.trueWidth + this.scale * (this.padding.right + this.padding.left);
        const newH = this.trueHeight + this.scale * (this.padding.top + this.padding.bottom);
        try {
          setContextSize(this.g, newW, newH);
        } catch (exp) {
          console.error(exp);
          throw exp;
        }
      }
      if (this.bgFillColor) {
        this.g.fillStyle = this.bgFillColor;
        this.g.fillRect(0, 0, this.canvas.width, this.canvas.height);
      } else {
        this.g.clearRect(0, 0, this.canvas.width, this.canvas.height);
      }
      if (this.textStrokeColor && this.textStrokeSize) {
        this.g.lineWidth = this.textStrokeSize * this.scale;
        this.g.strokeStyle = this.textStrokeColor;
      }
      if (this.textFillColor) {
        this.g.fillStyle = this.textFillColor;
      }
      const di = 0.5 * (lines.length - 1);
      for (let i = 0; i < lines.length; ++i) {
        const line = lines[i];
        const dy = (i - di) * this.trueFontSize;
        const x = this.dx + this.trueWidth / 2 + this.scale * this.padding.left;
        const y = dy + this.trueHeight / 2 + this.scale * this.padding.top;
        if (this.textStrokeColor && this.textStrokeSize) {
          this.g.strokeText(line, x, y);
        }
        if (this.textFillColor) {
          this.g.fillText(line, x, y);
        }
      }
      if (this.bgStrokeColor && this.bgStrokeSize) {
        this.g.strokeStyle = this.bgStrokeColor;
        this.g.lineWidth = this.bgStrokeSize * this.scale;
        const s = this.bgStrokeSize / 2;
        this.g.strokeRect(s, s, this.canvas.width - this.bgStrokeSize, this.canvas.height - this.bgStrokeSize);
      }
      if (isVertical) {
        const canv = createUtilityCanvas(this.canvas.height, this.canvas.width);
        const g = canv.getContext("2d");
        if (g) {
          g.translate(canv.width / 2, canv.height / 2);
          if (this.textDirection === "vertical" || this.textDirection === "vertical-left") {
            g.rotate(Math.PI / 2);
          } else if (this.textDirection === "vertical-right") {
            g.rotate(-Math.PI / 2);
          }
          g.translate(-this.canvas.width / 2, -this.canvas.height / 2);
          g.drawImage(this.canvas, 0, 0);
          setContextSize(this.g, canv.width, canv.height);
        } else {
          console.warn("Couldn't rotate the TextImage");
        }
        this.g.drawImage(canv, 0, 0);
      }
      return true;
    } else {
      return false;
    }
  }
};

// ../threejs/animation/scaleOnHover.ts
var scaledItems = singleton("Juniper:ScaledItems", () => /* @__PURE__ */ new Map());
function removeScaledObj(obj2) {
  const state = scaledItems.get(obj2);
  if (state) {
    scaledItems.delete(obj2);
    state.dispose();
  }
}

// ../threejs/cleanup.ts
function cleanup(obj2) {
  const cleanupQ = new Array();
  const cleanupSeen = /* @__PURE__ */ new Set();
  cleanupQ.push(obj2);
  while (cleanupQ.length > 0) {
    const here = cleanupQ.shift();
    if (here && !cleanupSeen.has(here)) {
      cleanupSeen.add(here);
      if (here.isMesh) {
        cleanupQ.push(here.material, here.geometry);
      }
      if (here.isMaterial) {
        cleanupQ.push(...Object.values(here));
      }
      if (here.isObject3D) {
        cleanupQ.push(...here.children);
        here.clear();
        removeScaledObj(here);
      }
      if (isArray(here)) {
        cleanupQ.push(...here);
      }
      dispose(here);
    }
  }
  cleanupSeen.clear();
}

// ../threejs/objectGetRelativePose.ts
var M2 = new THREE.Matrix4();
var P = new THREE.Vector3();
function objectGetRelativePose(ref, obj2, position, quaternion, scale2) {
  M2.copy(ref.matrixWorld).invert().multiply(obj2.matrixWorld).decompose(P, quaternion, scale2);
  position.set(P.x, P.y, P.z, 1);
}

// ../threejs/Plane.ts
var plane = new THREE.PlaneBufferGeometry(1, 1, 1, 1);
plane.name = "PlaneGeom";
var BasePlane = class extends THREE.Mesh {
  constructor(sx, sy, material, isCollider2) {
    super(plane, material);
    this.isCollider = isCollider2;
    this.scale.set(sx, sy, 1);
  }
};

// ../threejs/TexturedMesh.ts
var inchesPerMeter = 39.3701;
var TexturedMesh = class extends THREE.Mesh {
  constructor(geom2, mat) {
    super(geom2, mat);
    this._imageWidth = 0;
    this._imageHeight = 0;
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this._imageWidth = source.imageWidth;
    this._imageHeight = source.imageHeight;
    return this;
  }
  get imageWidth() {
    return this._imageWidth;
  }
  get imageHeight() {
    return this._imageHeight;
  }
  get imageAspectRatio() {
    return this.imageWidth / this.imageHeight;
  }
  get objectWidth() {
    return this.scale.x;
  }
  set objectWidth(v) {
    this.scale.x = v;
    this.scale.y = v / this.imageAspectRatio;
  }
  get objectHeight() {
    return this.scale.y;
  }
  set objectHeight(v) {
    this.scale.x = this.imageAspectRatio * v;
    this.scale.y = v;
  }
  get pixelDensity() {
    const ppm = this.imageWidth / this.objectWidth;
    const ppi = ppm / inchesPerMeter;
    return ppi;
  }
  set pixelDensity(ppi) {
    const ppm = ppi * inchesPerMeter;
    this.objectWidth = this.imageWidth / ppm;
  }
  setImage(img) {
    if (isImageBitmap(img)) {
      img = createCanvasFromImageBitmap(img);
    }
    if (isOffscreenCanvas(img)) {
      img = img;
    }
    if (img instanceof HTMLVideoElement) {
      this.material.map = new THREE.VideoTexture(img);
      this._imageWidth = img.videoWidth;
      this._imageHeight = img.videoHeight;
    } else {
      this.material.map = new THREE.Texture(img);
      this._imageWidth = img.width;
      this._imageHeight = img.height;
      this.material.map.needsUpdate = true;
    }
    this.material.needsUpdate = true;
    return this.material.map;
  }
  async loadImage(fetcher, path, prog) {
    let { content: img } = await fetcher.get(path).progress(prog).image();
    const texture = this.setImage(img);
    texture.name = path;
  }
  updateTexture() {
    const img = this.material.map.image;
    if (isNumber(img.width) && isNumber(img.height) && (this.imageWidth !== img.width || this.imageHeight !== img.height)) {
      this._imageWidth = img.width;
      this._imageHeight = img.height;
      this.material.map.dispose();
      this.material.map = new THREE.Texture(img);
      this.material.needsUpdate = true;
    }
    this.material.map.needsUpdate = true;
  }
};

// ../threejs/Image2DMesh.ts
var P2 = new THREE.Vector4();
var Q2 = new THREE.Quaternion();
var S = new THREE.Vector3();
var copyCounter2 = 0;
var Image2DMesh = class extends THREE.Object3D {
  constructor(env, name, isStatic, materialOrOptions = null) {
    super();
    this.isStatic = isStatic;
    this.lastMatrixWorld = new THREE.Matrix4();
    this.layer = null;
    this.tryWebXRLayers = true;
    this.wasUsingLayer = false;
    this.lastImage = null;
    this.lastWidth = null;
    this.lastHeight = null;
    this.stereoLayoutName = "mono";
    this.env = null;
    this.mesh = null;
    this.webXRLayersEnabled = true;
    if (env) {
      this.setEnvAndName(env, name);
      let material = isMeshBasicMaterial(materialOrOptions) ? materialOrOptions : solidTransparent(Object.assign({}, materialOrOptions, { name: this.name }));
      this.mesh = new TexturedMesh(plane, material);
      this.add(this.mesh);
    }
  }
  dispose() {
    cleanup(this.layer);
  }
  setEnvAndName(env, name) {
    this.env = env;
    this.name = name;
    this.tryWebXRLayers &&= this.env && this.env.hasXRCompositionLayers;
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this.setEnvAndName(source.env, source.name + ++copyCounter2);
    for (let i = this.children.length - 1; i >= 0; --i) {
      const child = this.children[i];
      if (child.parent instanceof Image2DMesh && child instanceof TexturedMesh) {
        child.removeFromParent();
        this.mesh = new TexturedMesh(child.geometry, child.material);
      }
    }
    if (isNullOrUndefined(this.mesh)) {
      this.mesh = source.mesh.clone();
      this.add(this.mesh);
    }
    return this;
  }
  get needsLayer() {
    if (!objectIsFullyVisible(this) || isNullOrUndefined(this.mesh.material.map) || isNullOrUndefined(this.mesh.material.map.image)) {
      return false;
    }
    const img = this.mesh.material.map.image;
    if (!(img instanceof HTMLVideoElement)) {
      return true;
    }
    return !img.paused || img.currentTime > 0;
  }
  removeWebXRLayer() {
    if (isDefined(this.layer)) {
      this.wasUsingLayer = false;
      this.env.removeWebXRLayer(this.layer);
      const layer = this.layer;
      this.layer = null;
      setTimeout(() => {
        layer.destroy();
        this.mesh.visible = true;
      }, 100);
    }
  }
  update(_dt, frame) {
    if (this.mesh.material.map && this.mesh.material.map.image) {
      const isVideo = this.mesh.material.map instanceof THREE.VideoTexture;
      const isLayersAvailable = this.tryWebXRLayers && this.webXRLayersEnabled && isDefined(frame) && (isVideo && isDefined(this.env.xrMediaBinding) || !isVideo && isDefined(this.env.xrBinding));
      const useLayer = isLayersAvailable && this.needsLayer;
      const useLayerChanged = useLayer !== this.wasUsingLayer;
      const imageChanged = this.mesh.material.map.image !== this.lastImage || this.mesh.material.needsUpdate || this.mesh.material.map.needsUpdate;
      const sizeChanged = this.mesh.imageWidth !== this.lastWidth || this.mesh.imageHeight !== this.lastHeight;
      this.wasUsingLayer = useLayer;
      this.lastImage = this.mesh.material.map.image;
      this.lastWidth = this.mesh.imageWidth;
      this.lastHeight = this.mesh.imageHeight;
      if (useLayerChanged || sizeChanged) {
        if ((!useLayer || sizeChanged) && this.layer) {
          this.removeWebXRLayer();
        }
        if (useLayer) {
          const space = this.env.referenceSpace;
          objectGetRelativePose(this.env.stage, this.mesh, P2, Q2, S);
          this.lastMatrixWorld.copy(this.matrixWorld);
          const transform = new XRRigidTransform(P2, Q2);
          const width = S.x / 2;
          const height = S.y / 2;
          const layout = this.stereoLayoutName === "mono" ? "mono" : this.stereoLayoutName === "left-right" || this.stereoLayoutName === "right-left" ? "stereo-left-right" : "stereo-top-bottom";
          if (isVideo) {
            const invertStereo = this.stereoLayoutName === "right-left" || this.stereoLayoutName === "bottom-top";
            this.layer = this.env.xrMediaBinding.createQuadLayer(this.mesh.material.map.image, {
              space,
              layout,
              invertStereo,
              transform,
              width,
              height
            });
          } else {
            this.layer = this.env.xrBinding.createQuadLayer({
              space,
              layout,
              textureType: "texture",
              isStatic: this.isStatic,
              viewPixelWidth: this.mesh.imageWidth,
              viewPixelHeight: this.mesh.imageHeight,
              transform,
              width,
              height
            });
          }
          this.env.addWebXRLayer(this.layer, 500);
          this.mesh.visible = false;
        }
      }
      if (this.layer) {
        if (imageChanged || this.layer.needsRedraw) {
          const gl = this.env.gl;
          const gLayer = this.env.xrBinding.getSubImage(this.layer, frame);
          gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
          gl.bindTexture(gl.TEXTURE_2D, gLayer.colorTexture);
          gl.texSubImage2D(gl.TEXTURE_2D, 0, 0, 0, gl.RGBA, gl.UNSIGNED_BYTE, this.mesh.material.map.image);
          gl.generateMipmap(gl.TEXTURE_2D);
          gl.bindTexture(gl.TEXTURE_2D, null);
        }
        if (arrayCompare(this.matrixWorld.elements, this.lastMatrixWorld.elements) >= 0) {
          objectGetRelativePose(this.env.stage, this.mesh, P2, Q2, S);
          this.lastMatrixWorld.copy(this.matrixWorld);
          this.layer.transform = new XRRigidTransform(P2, Q2);
          this.layer.width = S.x / 2;
          this.layer.height = S.y / 2;
        }
      }
    }
  }
};

// ../threejs/TextMesh.ts
var redrawnEvt = { type: "redrawn" };
var TextMesh = class extends Image2DMesh {
  constructor(env, name, materialOptions) {
    super(env, name, false, materialOptions);
    this._textImage = null;
    this._onRedrawn = this.onRedrawn.bind(this);
  }
  onRedrawn() {
    this.mesh.updateTexture();
    this.scale.set(this._textImage.width, this._textImage.height, 0.01);
    this.dispatchEvent(redrawnEvt);
  }
  get textImage() {
    return this._textImage;
  }
  set textImage(v) {
    if (v !== this.textImage) {
      if (this.textImage) {
        this.textImage.clearEventListeners();
      }
      this._textImage = v;
      if (this.textImage) {
        this.textImage.addEventListener("redrawn", this._onRedrawn);
        this.mesh.setImage(this.textImage.canvas);
        this._onRedrawn();
      }
    }
  }
  createTextImage(textImageOptions) {
    this.textImage = new TextImage(textImageOptions);
  }
  get wrapWords() {
    return this._textImage.wrapWords;
  }
  set wrapWords(v) {
    this._textImage.wrapWords = v;
  }
  get minWidth() {
    return this._textImage.minWidth;
  }
  set minWidth(v) {
    this._textImage.minWidth = v;
  }
  get maxWidth() {
    return this._textImage.maxWidth;
  }
  set maxWidth(v) {
    this._textImage.maxWidth = v;
  }
  get minHeight() {
    return this._textImage.minHeight;
  }
  set minHeight(v) {
    this._textImage.minHeight = v;
  }
  get maxHeight() {
    return this._textImage.maxHeight;
  }
  set maxHeight(v) {
    this._textImage.maxHeight = v;
  }
  get textDirection() {
    return this._textImage.textDirection;
  }
  set textDirection(v) {
    this._textImage.textDirection = v;
  }
  get textScale() {
    return this._textImage.scale;
  }
  set textScale(v) {
    this._textImage.scale = v;
  }
  get textWidth() {
    return this._textImage.width;
  }
  get textHeight() {
    return this._textImage.height;
  }
  get textPadding() {
    return this._textImage.padding;
  }
  set textPadding(v) {
    this._textImage.padding = v;
  }
  get fontStyle() {
    return this._textImage.fontStyle;
  }
  set fontStyle(v) {
    this._textImage.fontStyle = v;
  }
  get fontVariant() {
    return this._textImage.fontVariant;
  }
  set fontVariant(v) {
    this._textImage.fontVariant = v;
  }
  get fontWeight() {
    return this._textImage.fontWeight;
  }
  set fontWeight(v) {
    this._textImage.fontWeight = v;
  }
  get fontSize() {
    return this._textImage.fontSize;
  }
  set fontSize(v) {
    this._textImage.fontSize = v;
  }
  get fontFamily() {
    return this._textImage.fontFamily;
  }
  set fontFamily(v) {
    this._textImage.fontFamily = v;
  }
  get textFillColor() {
    return this._textImage.textFillColor;
  }
  set textFillColor(v) {
    this._textImage.textFillColor = v;
  }
  get textStrokeColor() {
    return this._textImage.textStrokeColor;
  }
  set textStrokeColor(v) {
    this._textImage.textStrokeColor = v;
  }
  get textStrokeSize() {
    return this._textImage.textStrokeSize;
  }
  set textStrokeSize(v) {
    this._textImage.textStrokeSize = v;
  }
  get textBgColor() {
    return this._textImage.bgFillColor;
  }
  set textBgColor(v) {
    this._textImage.bgFillColor = v;
  }
  get value() {
    return this._textImage.value;
  }
  set value(v) {
    this._textImage.value = v;
  }
};

// ../threejs/AvatarRemote.ts
var P3 = new THREE.Vector3();
var F = new THREE.Vector3();
var U = new THREE.Vector3();
var O = new THREE.Vector3();
var E = new THREE.Vector3();
var M3 = new THREE.Matrix4();
var angle2 = Math.PI / 2;
var nameTagFont = {
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
  maxHeight: 0.2
};
var AvatarRemote = class extends THREE.Object3D {
  constructor(env, user, source, font, defaultAvatarHeight) {
    super();
    this.env = env;
    this.defaultAvatarHeight = defaultAvatarHeight;
    this.avatar = null;
    this.isInstructor = false;
    this.pointers = /* @__PURE__ */ new Map();
    this.head = null;
    this.body = null;
    this.pTarget = new THREE.Vector3();
    this.pEnd = new THREE.Vector3();
    this.qTarget = new THREE.Quaternion().identity();
    this.qEnd = new THREE.Quaternion().identity();
    this.headFollower = null;
    this._headSize = 1;
    this._headPulse = 1;
    this.height = this.defaultAvatarHeight;
    this.name = user.userName;
    this.nameTag = new TextMesh(this.env, `nameTag-${user.userName}-${user.userID}`);
    this.nameTag.createTextImage(Object.assign({}, nameTagFont, font));
    this.nameTag.position.y = 0.25;
    this.userName = user.userName;
    this.add(this.nameTag);
    user.addEventListener("userPosed", (evt) => this.setPose(evt.pose, evt.height));
    user.addEventListener("userPointer", (evt) => {
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
    return this.nameTag.value;
  }
  set userName(name) {
    if (name) {
      const words = name.match(/^(?:((?:student|instructor))_)?([^<>{}"]+)$/i);
      if (words) {
        if (words.length === 2) {
          this.nameTag.value = words[1];
        } else if (words.length === 3) {
          this.isInstructor = words[1] && words[1].toLocaleLowerCase() === "instructor";
          if (this.isInstructor) {
            this.nameTag.value = star.value + words[2];
          } else {
            this.nameTag.value = words[2];
          }
        } else {
          this.nameTag.value = "???";
        }
      }
    }
  }
  setAvatar(obj2) {
    if (this.avatar) {
      this.remove(this.avatar);
      this.head = null;
      this.body = null;
    }
    if (obj2) {
      this.avatar = obj2;
      this.avatar.traverse((child) => {
        if (child.name === "Head") {
          this.head = child;
        } else if (child.name === "Body") {
          this.body = child;
        }
      });
      if (this.head && this.body) {
        this.headFollower = new BodyFollower("AvatarBody", 0.05, angle2, 0, 5);
        this.body.parent.add(this.headFollower);
        this.body.add(this.nameTag);
        this.headFollower.add(this.body);
      }
      this.add(this.avatar);
    }
  }
  removeArm(name) {
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
  update(dt) {
    this.headPulse = 0.2 * this.activity.level + 1;
    this.pEnd.lerp(this.pTarget, dt * 0.01);
    this.qEnd.slerp(this.qTarget, dt * 0.01);
    if (this.head && this.body) {
      this.head.position.copy(this.pEnd);
      this.head.quaternion.copy(this.qEnd);
      this.head.getWorldPosition(P3);
      this.head.getWorldDirection(F);
      const angle3 = getLookHeading(F);
      this.headFollower.update(P3.y - this.parent.position.y, P3, angle3, dt);
      const scale2 = this.height / this.defaultAvatarHeight;
      this.headSize = scale2;
      this.body.scale.setScalar(scale2);
      F.copy(this.env.avatar.worldPos);
      this.body.worldToLocal(F);
      F.sub(this.body.position).normalize().multiplyScalar(0.25);
      this.nameTag.position.set(0, -0.25, 0).add(F);
    } else {
      this.position.copy(this.pEnd);
      this.quaternion.copy(this.qEnd);
    }
    for (const pointer of this.pointers.values()) {
      pointer.animate(dt);
    }
    this.nameTag.lookAt(this.env.avatar.worldPos);
  }
  setPose(pose, height) {
    O.fromArray(pose.o);
    P3.fromArray(pose.p).add(O);
    F.fromArray(pose.f);
    U.fromArray(pose.u);
    setMatrixFromUpFwdPos(U, F, P3, M3);
    M3.decompose(this.pTarget, this.qTarget, this.scale);
    this.height = height;
  }
  setPointer(avatarHeadPos, name, pose) {
    let pointer = this.pointers.get(name);
    if (!pointer) {
      pointer = new PointerRemote(this.env.eventSystem, this.userName, this.isInstructor, name, this.env.cursor3D && this.env.cursor3D.clone());
      this.pointers.set(name, pointer);
      objGraph(this.body, pointer);
      if (pointer.cursor) {
        objGraph(this.env.stage, pointer.cursor);
      }
      if (name === 1 /* Mouse */) {
        this.removeArm(15 /* MotionController */);
        this.removeArm(16 /* MotionControllerLeft */);
        this.removeArm(17 /* MotionControllerRight */);
      } else if (name === 15 /* MotionController */ || name === 16 /* MotionControllerLeft */ || name === 17 /* MotionControllerRight */) {
        this.removeArm(1 /* Mouse */);
        this.removeArm(2 /* Pen */);
        this.removeArm(3 /* Touch0 */);
        this.removeArm(4 /* Touch1 */);
        this.removeArm(5 /* Touch2 */);
        this.removeArm(6 /* Touch3 */);
        this.removeArm(7 /* Touch4 */);
        this.removeArm(8 /* Touch5 */);
        this.removeArm(9 /* Touch6 */);
        this.removeArm(10 /* Touch7 */);
        this.removeArm(11 /* Touch8 */);
        this.removeArm(12 /* Touch9 */);
        this.removeArm(13 /* Touch10 */);
        this.removeArm(14 /* Touches */);
      }
    }
    P3.fromArray(pose.p);
    F.fromArray(pose.f);
    U.fromArray(pose.u);
    O.fromArray(pose.o);
    if (name === 1 /* Mouse */ && this.body) {
      E.set(0.2, -0.6, 0).applyQuaternion(this.body.quaternion);
    } else if (PointerName[name].startsWith("Touch") && this.body) {
      E.set(0, -0.5, 0).applyQuaternion(this.body.quaternion);
    } else {
      E.setScalar(0);
    }
    O.add(E);
    pointer.setState(avatarHeadPos, P3, F, U, O);
  }
};

// ../threejs/DebugObject.ts
var DebugObject = class extends THREE.Object3D {
  constructor(color) {
    super();
    this.color = color;
    this.center = null;
    if (isDefined(this.color)) {
      this.center = new Cube(0.5, 0.5, 0.5, lit({ color: this.color }));
      this.add(this.center);
    }
    this.xp = new Cube(1, 0.1, 0.1, solidRed);
    this.yp = new Cube(0.1, 1, 0.1, solidGreen);
    this.zn = new Cube(0.1, 0.1, 1, solidBlue);
    this.xp.position.x = 1;
    this.yp.position.y = 1;
    this.zn.position.z = -1;
    this.add(this.xp, this.yp, this.zn);
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this.color = source.color;
    this.center = new Cube(0.5, 0.5, 0.5, lit({ color: source.color }));
    return this;
  }
  get size() {
    return this.xp.scale.x;
  }
  set size(v) {
    if (isDefined(this.center)) {
      this.center.scale.setScalar(v);
    }
    this.xp.scale.setScalar(0.1 * v);
    this.yp.scale.setScalar(0.1 * v);
    this.zn.scale.setScalar(0.1 * v);
    this.xp.scale.x = this.yp.scale.y = this.zn.scale.z = v;
    this.xp.position.x = this.yp.position.y = v;
    this.zn.position.z = -v;
  }
};

// ../threejs/environment/Application.ts
var Application = class extends TypedEventBase {
  constructor(env) {
    super();
    this.env = env;
  }
};

// ../threejs/Tele.ts
var Tele = class extends Application {
  users = /* @__PURE__ */ new Map();
  remoteUsers = new THREE.Object3D();
  conference = null;
  defaultAvatarHeight = 1.75;
  avatarModel = null;
  avatarNameTagFont = null;
  hubName = null;
  userType = null;
  userName = null;
  meetingID = null;
  roomName = null;
  constructor(env) {
    super(env);
  }
  get ready() {
    return this.conference && this.conference.ready;
  }
  async init(params) {
    this.defaultAvatarHeight = params.get("defaultAvatarHeight");
    if (!this.defaultAvatarHeight) {
      throw new Error("Missing defaultAvatarHeight parameter");
    }
    this.hubName = params.get("hub");
    if (!this.hubName) {
      throw new Error("Missing hub parameter");
    }
    this.avatarNameTagFont = params.get("nameTagFont");
    if (!this.avatarNameTagFont) {
      throw new Error("Missing nameTagFont parameter");
    }
    this.env.avatar.addEventListener("avatarmoved", (evt) => this.conference.setLocalPose(evt.px, evt.py, evt.pz, evt.fx, evt.fy, evt.fz, evt.ux, evt.uy, evt.uz, evt.height));
    this.env.eventSystem.addEventListener("objectMoved", (evt) => {
      this.conference.setLocalPointer(evt.name, evt.px, evt.py, evt.pz, evt.fx, evt.fy, evt.fz, evt.ux, evt.uy, evt.uz);
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
      this.conference.audioMuted = this.env.muteMicButton.active = !this.conference.audioMuted;
    });
    this.remoteUsers.name = "Remote Users";
    this.env.audio.offsetRadius = 1.25;
    this.conference = new TeleconferenceManager(this.env.audio, this.hubName, false);
    const onLocalUserIDChange = (evt) => {
      this.env.avatar.name = evt.userID;
    };
    this.conference.addEventListener("roomJoined", onLocalUserIDChange);
    this.conference.addEventListener("roomLeft", onLocalUserIDChange);
    this.conference.addEventListener("userJoined", (evt) => {
      const user = new AvatarRemote(this.env, evt.user, evt.source, this.avatarNameTagFont, this.defaultAvatarHeight);
      const avatar = this.avatarModel ? this.avatarModel.clone() : new DebugObject(16776960);
      user.setAvatar(avatar);
      user.userName = evt.user.userName;
      this.users.set(evt.user.userID, user);
      this.remoteUsers.add(user);
      this.env.audio.playClip("join");
    });
    this.conference.addEventListener("userNameChanged", (evt) => {
      const user = this.users.get(evt.user.userID);
      if (user) {
        user.userName = evt.newUserName;
      }
    });
    this.conference.addEventListener("userLeft", (evt) => {
      const user = this.users.get(evt.user.userID);
      if (user) {
        this.remoteUsers.remove(user);
        this.users.delete(evt.user.userID);
        cleanup(user);
        this.env.audio.playClip("leave");
      }
    });
  }
  async show(_onProgress) {
    this.env.foreground.add(this.remoteUsers);
    if (isDefined(this.env.currentRoom)) {
      await this.join(this.env.currentRoom);
    }
  }
  dispose() {
    this.env.foreground.remove(this.remoteUsers);
  }
  async loadAvatar(path, prog) {
    this.avatarModel = await this.env.loadModel(path, prog);
    this.avatarModel = this.avatarModel.children[0];
    for (const id of this.users.keys()) {
      const user = this.users.get(id);
      const oldAvatar = user.avatar;
      user.setAvatar(this.avatarModel.clone());
      cleanup(oldAvatar);
    }
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
  async checkConference() {
    if (isDefined(this.userType) && isDefined(this.userName) && isDefined(this.meetingID) && isDefined(this.roomName)) {
      const isoRoomName = `${this.roomName}_${this.meetingID}`.toLocaleLowerCase();
      const isoUserName = `${this.userType}_${this.userName}`;
      if (this.conference.roomName !== isoRoomName) {
        await this.conference.connect();
        await this.conference.identify(isoUserName);
        await this.conference.join(isoRoomName);
      }
    }
  }
  async load(prog) {
    await progressTasks(prog, (prog2) => this.env.audio.loadBasicClip("join", "/audio/door_open.mp3", 0.25, prog2), (prog2) => this.env.audio.loadBasicClip("leave", "/audio/door_close.mp3", 0.25, prog2), (prog2) => this.loadAvatar("/models/Avatar.glb", prog2));
  }
  update(evt) {
    for (const user of this.users.values()) {
      user.update(evt.dt);
    }
  }
};

// src/index.ts
var src_default = Tele;
export {
  src_default as default
};
//# sourceMappingURL=index.js.map
