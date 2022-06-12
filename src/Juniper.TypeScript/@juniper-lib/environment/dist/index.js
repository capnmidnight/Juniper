var __create = Object.create;
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __getProtoOf = Object.getPrototypeOf;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __defNormalProp = (obj2, key, value2) => key in obj2 ? __defProp(obj2, key, { enumerable: true, configurable: true, writable: true, value: value2 }) : obj2[key] = value2;
var __commonJS = (cb, mod) => function __require() {
  return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
};
var __export = (target, all) => {
  for (var name2 in all)
    __defProp(target, name2, { get: all[name2], enumerable: true });
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
var __publicField = (obj2, key, value2) => {
  __defNormalProp(obj2, typeof key !== "symbol" ? key + "" : key, value2);
  return value2;
};

// ../../node_modules/cardboard-vr-display/dist/cardboard-vr-display.js
var require_cardboard_vr_display = __commonJS({
  "../../node_modules/cardboard-vr-display/dist/cardboard-vr-display.js"(exports, module) {
    (function(global2, factory) {
      typeof exports === "object" && typeof module !== "undefined" ? module.exports = factory() : typeof define === "function" && define.amd ? define(factory) : global2.CardboardVRDisplay = factory();
    })(exports, function() {
      "use strict";
      var asyncGenerator = function() {
        function AwaitValue(value2) {
          this.value = value2;
        }
        function AsyncGenerator(gen) {
          var front, back;
          function send(key, arg) {
            return new Promise(function(resolve, reject) {
              var request = {
                key,
                arg,
                resolve,
                reject,
                next: null
              };
              if (back) {
                back = back.next = request;
              } else {
                front = back = request;
                resume(key, arg);
              }
            });
          }
          function resume(key, arg) {
            try {
              var result = gen[key](arg);
              var value2 = result.value;
              if (value2 instanceof AwaitValue) {
                Promise.resolve(value2.value).then(function(arg2) {
                  resume("next", arg2);
                }, function(arg2) {
                  resume("throw", arg2);
                });
              } else {
                settle(result.done ? "return" : "normal", result.value);
              }
            } catch (err) {
              settle("throw", err);
            }
          }
          function settle(type2, value2) {
            switch (type2) {
              case "return":
                front.resolve({
                  value: value2,
                  done: true
                });
                break;
              case "throw":
                front.reject(value2);
                break;
              default:
                front.resolve({
                  value: value2,
                  done: false
                });
                break;
            }
            front = front.next;
            if (front) {
              resume(front.key, front.arg);
            } else {
              back = null;
            }
          }
          this._invoke = send;
          if (typeof gen.return !== "function") {
            this.return = void 0;
          }
        }
        if (typeof Symbol === "function" && Symbol.asyncIterator) {
          AsyncGenerator.prototype[Symbol.asyncIterator] = function() {
            return this;
          };
        }
        AsyncGenerator.prototype.next = function(arg) {
          return this._invoke("next", arg);
        };
        AsyncGenerator.prototype.throw = function(arg) {
          return this._invoke("throw", arg);
        };
        AsyncGenerator.prototype.return = function(arg) {
          return this._invoke("return", arg);
        };
        return {
          wrap: function(fn) {
            return function() {
              return new AsyncGenerator(fn.apply(this, arguments));
            };
          },
          await: function(value2) {
            return new AwaitValue(value2);
          }
        };
      }();
      var classCallCheck = function(instance, Constructor) {
        if (!(instance instanceof Constructor)) {
          throw new TypeError("Cannot call a class as a function");
        }
      };
      var createClass = function() {
        function defineProperties(target, props) {
          for (var i = 0; i < props.length; i++) {
            var descriptor = props[i];
            descriptor.enumerable = descriptor.enumerable || false;
            descriptor.configurable = true;
            if ("value" in descriptor)
              descriptor.writable = true;
            Object.defineProperty(target, descriptor.key, descriptor);
          }
        }
        return function(Constructor, protoProps, staticProps) {
          if (protoProps)
            defineProperties(Constructor.prototype, protoProps);
          if (staticProps)
            defineProperties(Constructor, staticProps);
          return Constructor;
        };
      }();
      var slicedToArray = function() {
        function sliceIterator(arr, i) {
          var _arr = [];
          var _n = true;
          var _d = false;
          var _e = void 0;
          try {
            for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) {
              _arr.push(_s.value);
              if (i && _arr.length === i)
                break;
            }
          } catch (err) {
            _d = true;
            _e = err;
          } finally {
            try {
              if (!_n && _i["return"])
                _i["return"]();
            } finally {
              if (_d)
                throw _e;
            }
          }
          return _arr;
        }
        return function(arr, i) {
          if (Array.isArray(arr)) {
            return arr;
          } else if (Symbol.iterator in Object(arr)) {
            return sliceIterator(arr, i);
          } else {
            throw new TypeError("Invalid attempt to destructure non-iterable instance");
          }
        };
      }();
      var MIN_TIMESTEP = 1e-3;
      var MAX_TIMESTEP = 1;
      var dataUri = function dataUri2(mimeType, svg) {
        return "data:" + mimeType + "," + encodeURIComponent(svg);
      };
      var lerp4 = function lerp5(a, b, t2) {
        return a + (b - a) * t2;
      };
      var isIOS2 = function() {
        var isIOS3 = /iPad|iPhone|iPod/.test(navigator.platform);
        return function() {
          return isIOS3;
        };
      }();
      var isWebViewAndroid = function() {
        var isWebViewAndroid2 = navigator.userAgent.indexOf("Version") !== -1 && navigator.userAgent.indexOf("Android") !== -1 && navigator.userAgent.indexOf("Chrome") !== -1;
        return function() {
          return isWebViewAndroid2;
        };
      }();
      var isSafari = function() {
        var isSafari2 = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
        return function() {
          return isSafari2;
        };
      }();
      var isFirefoxAndroid = function() {
        var isFirefoxAndroid2 = navigator.userAgent.indexOf("Firefox") !== -1 && navigator.userAgent.indexOf("Android") !== -1;
        return function() {
          return isFirefoxAndroid2;
        };
      }();
      var getChromeVersion = function() {
        var match = navigator.userAgent.match(/.*Chrome\/([0-9]+)/);
        var value2 = match ? parseInt(match[1], 10) : null;
        return function() {
          return value2;
        };
      }();
      var isSafariWithoutDeviceMotion = function() {
        var value2 = false;
        value2 = isIOS2() && isSafari() && navigator.userAgent.indexOf("13_4") !== -1;
        return function() {
          return value2;
        };
      }();
      var isChromeWithoutDeviceMotion = function() {
        var value2 = false;
        if (getChromeVersion() === 65) {
          var match = navigator.userAgent.match(/.*Chrome\/([0-9\.]*)/);
          if (match) {
            var _match$1$split = match[1].split("."), _match$1$split2 = slicedToArray(_match$1$split, 4), major = _match$1$split2[0], minor = _match$1$split2[1], branch = _match$1$split2[2], build = _match$1$split2[3];
            value2 = parseInt(branch, 10) === 3325 && parseInt(build, 10) < 148;
          }
        }
        return function() {
          return value2;
        };
      }();
      var isR7 = function() {
        var isR72 = navigator.userAgent.indexOf("R7 Build") !== -1;
        return function() {
          return isR72;
        };
      }();
      var isLandscapeMode = function isLandscapeMode2() {
        var rtn = window.orientation == 90 || window.orientation == -90;
        return isR7() ? !rtn : rtn;
      };
      var isTimestampDeltaValid = function isTimestampDeltaValid2(timestampDeltaS) {
        if (isNaN(timestampDeltaS)) {
          return false;
        }
        if (timestampDeltaS <= MIN_TIMESTEP) {
          return false;
        }
        if (timestampDeltaS > MAX_TIMESTEP) {
          return false;
        }
        return true;
      };
      var getScreenWidth = function getScreenWidth2() {
        return Math.max(window.screen.width, window.screen.height) * window.devicePixelRatio;
      };
      var getScreenHeight = function getScreenHeight2() {
        return Math.min(window.screen.width, window.screen.height) * window.devicePixelRatio;
      };
      var requestFullscreen = function requestFullscreen2(element) {
        if (isWebViewAndroid()) {
          return false;
        }
        if (element.requestFullscreen) {
          element.requestFullscreen();
        } else if (element.webkitRequestFullscreen) {
          element.webkitRequestFullscreen();
        } else if (element.mozRequestFullScreen) {
          element.mozRequestFullScreen();
        } else if (element.msRequestFullscreen) {
          element.msRequestFullscreen();
        } else {
          return false;
        }
        return true;
      };
      var exitFullscreen = function exitFullscreen2() {
        if (document.exitFullscreen) {
          document.exitFullscreen();
        } else if (document.webkitExitFullscreen) {
          document.webkitExitFullscreen();
        } else if (document.mozCancelFullScreen) {
          document.mozCancelFullScreen();
        } else if (document.msExitFullscreen) {
          document.msExitFullscreen();
        } else {
          return false;
        }
        return true;
      };
      var getFullscreenElement = function getFullscreenElement2() {
        return document.fullscreenElement || document.webkitFullscreenElement || document.mozFullScreenElement || document.msFullscreenElement;
      };
      var linkProgram = function linkProgram2(gl, vertexSource, fragmentSource, attribLocationMap) {
        var vertexShader = gl.createShader(gl.VERTEX_SHADER);
        gl.shaderSource(vertexShader, vertexSource);
        gl.compileShader(vertexShader);
        var fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);
        gl.shaderSource(fragmentShader, fragmentSource);
        gl.compileShader(fragmentShader);
        var program = gl.createProgram();
        gl.attachShader(program, vertexShader);
        gl.attachShader(program, fragmentShader);
        for (var attribName in attribLocationMap) {
          gl.bindAttribLocation(program, attribLocationMap[attribName], attribName);
        }
        gl.linkProgram(program);
        gl.deleteShader(vertexShader);
        gl.deleteShader(fragmentShader);
        return program;
      };
      var getProgramUniforms = function getProgramUniforms2(gl, program) {
        var uniforms = {};
        var uniformCount = gl.getProgramParameter(program, gl.ACTIVE_UNIFORMS);
        var uniformName = "";
        for (var i = 0; i < uniformCount; i++) {
          var uniformInfo = gl.getActiveUniform(program, i);
          uniformName = uniformInfo.name.replace("[0]", "");
          uniforms[uniformName] = gl.getUniformLocation(program, uniformName);
        }
        return uniforms;
      };
      var orthoMatrix = function orthoMatrix2(out, left2, right, bottom, top2, near, far) {
        var lr = 1 / (left2 - right), bt = 1 / (bottom - top2), nf = 1 / (near - far);
        out[0] = -2 * lr;
        out[1] = 0;
        out[2] = 0;
        out[3] = 0;
        out[4] = 0;
        out[5] = -2 * bt;
        out[6] = 0;
        out[7] = 0;
        out[8] = 0;
        out[9] = 0;
        out[10] = 2 * nf;
        out[11] = 0;
        out[12] = (left2 + right) * lr;
        out[13] = (top2 + bottom) * bt;
        out[14] = (far + near) * nf;
        out[15] = 1;
        return out;
      };
      var isMobile3 = function isMobile4() {
        var check = false;
        (function(a) {
          if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4)))
            check = true;
        })(navigator.userAgent || navigator.vendor || window.opera);
        return check;
      };
      var extend = function extend2(dest, src2) {
        for (var key in src2) {
          if (src2.hasOwnProperty(key)) {
            dest[key] = src2[key];
          }
        }
        return dest;
      };
      var safariCssSizeWorkaround = function safariCssSizeWorkaround2(canvas) {
        if (isIOS2()) {
          var width2 = canvas.style.width;
          var height2 = canvas.style.height;
          canvas.style.width = parseInt(width2) + 1 + "px";
          canvas.style.height = parseInt(height2) + "px";
          setTimeout(function() {
            canvas.style.width = width2;
            canvas.style.height = height2;
          }, 100);
        }
        window.canvas = canvas;
      };
      var frameDataFromPose = function() {
        var piOver180 = Math.PI / 180;
        var rad45 = Math.PI * 0.25;
        function mat4_perspectiveFromFieldOfView(out, fov, near, far) {
          var upTan = Math.tan(fov ? fov.upDegrees * piOver180 : rad45), downTan = Math.tan(fov ? fov.downDegrees * piOver180 : rad45), leftTan = Math.tan(fov ? fov.leftDegrees * piOver180 : rad45), rightTan = Math.tan(fov ? fov.rightDegrees * piOver180 : rad45), xScale = 2 / (leftTan + rightTan), yScale = 2 / (upTan + downTan);
          out[0] = xScale;
          out[1] = 0;
          out[2] = 0;
          out[3] = 0;
          out[4] = 0;
          out[5] = yScale;
          out[6] = 0;
          out[7] = 0;
          out[8] = -((leftTan - rightTan) * xScale * 0.5);
          out[9] = (upTan - downTan) * yScale * 0.5;
          out[10] = far / (near - far);
          out[11] = -1;
          out[12] = 0;
          out[13] = 0;
          out[14] = far * near / (near - far);
          out[15] = 0;
          return out;
        }
        function mat4_fromRotationTranslation(out, q, v) {
          var x = q[0], y = q[1], z = q[2], w = q[3], x2 = x + x, y2 = y + y, z2 = z + z, xx = x * x2, xy = x * y2, xz = x * z2, yy = y * y2, yz = y * z2, zz = z * z2, wx = w * x2, wy = w * y2, wz = w * z2;
          out[0] = 1 - (yy + zz);
          out[1] = xy + wz;
          out[2] = xz - wy;
          out[3] = 0;
          out[4] = xy - wz;
          out[5] = 1 - (xx + zz);
          out[6] = yz + wx;
          out[7] = 0;
          out[8] = xz + wy;
          out[9] = yz - wx;
          out[10] = 1 - (xx + yy);
          out[11] = 0;
          out[12] = v[0];
          out[13] = v[1];
          out[14] = v[2];
          out[15] = 1;
          return out;
        }
        function mat4_translate(out, a, v) {
          var x = v[0], y = v[1], z = v[2], a00, a01, a02, a03, a10, a11, a12, a13, a20, a21, a22, a23;
          if (a === out) {
            out[12] = a[0] * x + a[4] * y + a[8] * z + a[12];
            out[13] = a[1] * x + a[5] * y + a[9] * z + a[13];
            out[14] = a[2] * x + a[6] * y + a[10] * z + a[14];
            out[15] = a[3] * x + a[7] * y + a[11] * z + a[15];
          } else {
            a00 = a[0];
            a01 = a[1];
            a02 = a[2];
            a03 = a[3];
            a10 = a[4];
            a11 = a[5];
            a12 = a[6];
            a13 = a[7];
            a20 = a[8];
            a21 = a[9];
            a22 = a[10];
            a23 = a[11];
            out[0] = a00;
            out[1] = a01;
            out[2] = a02;
            out[3] = a03;
            out[4] = a10;
            out[5] = a11;
            out[6] = a12;
            out[7] = a13;
            out[8] = a20;
            out[9] = a21;
            out[10] = a22;
            out[11] = a23;
            out[12] = a00 * x + a10 * y + a20 * z + a[12];
            out[13] = a01 * x + a11 * y + a21 * z + a[13];
            out[14] = a02 * x + a12 * y + a22 * z + a[14];
            out[15] = a03 * x + a13 * y + a23 * z + a[15];
          }
          return out;
        }
        function mat4_invert(out, a) {
          var a00 = a[0], a01 = a[1], a02 = a[2], a03 = a[3], a10 = a[4], a11 = a[5], a12 = a[6], a13 = a[7], a20 = a[8], a21 = a[9], a22 = a[10], a23 = a[11], a30 = a[12], a31 = a[13], a32 = a[14], a33 = a[15], b00 = a00 * a11 - a01 * a10, b01 = a00 * a12 - a02 * a10, b02 = a00 * a13 - a03 * a10, b03 = a01 * a12 - a02 * a11, b04 = a01 * a13 - a03 * a11, b05 = a02 * a13 - a03 * a12, b06 = a20 * a31 - a21 * a30, b07 = a20 * a32 - a22 * a30, b08 = a20 * a33 - a23 * a30, b09 = a21 * a32 - a22 * a31, b10 = a21 * a33 - a23 * a31, b11 = a22 * a33 - a23 * a32, det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
          if (!det) {
            return null;
          }
          det = 1 / det;
          out[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
          out[1] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
          out[2] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
          out[3] = (a22 * b04 - a21 * b05 - a23 * b03) * det;
          out[4] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
          out[5] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
          out[6] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
          out[7] = (a20 * b05 - a22 * b02 + a23 * b01) * det;
          out[8] = (a10 * b10 - a11 * b08 + a13 * b06) * det;
          out[9] = (a01 * b08 - a00 * b10 - a03 * b06) * det;
          out[10] = (a30 * b04 - a31 * b02 + a33 * b00) * det;
          out[11] = (a21 * b02 - a20 * b04 - a23 * b00) * det;
          out[12] = (a11 * b07 - a10 * b09 - a12 * b06) * det;
          out[13] = (a00 * b09 - a01 * b07 + a02 * b06) * det;
          out[14] = (a31 * b01 - a30 * b03 - a32 * b00) * det;
          out[15] = (a20 * b03 - a21 * b01 + a22 * b00) * det;
          return out;
        }
        var defaultOrientation = new Float32Array([0, 0, 0, 1]);
        var defaultPosition = new Float32Array([0, 0, 0]);
        function updateEyeMatrices(projection, view, pose, fov, offset, vrDisplay) {
          mat4_perspectiveFromFieldOfView(projection, fov || null, vrDisplay.depthNear, vrDisplay.depthFar);
          var orientation = pose.orientation || defaultOrientation;
          var position2 = pose.position || defaultPosition;
          mat4_fromRotationTranslation(view, orientation, position2);
          if (offset)
            mat4_translate(view, view, offset);
          mat4_invert(view, view);
        }
        return function(frameData, pose, vrDisplay) {
          if (!frameData || !pose)
            return false;
          frameData.pose = pose;
          frameData.timestamp = pose.timestamp;
          updateEyeMatrices(frameData.leftProjectionMatrix, frameData.leftViewMatrix, pose, vrDisplay._getFieldOfView("left"), vrDisplay._getEyeOffset("left"), vrDisplay);
          updateEyeMatrices(frameData.rightProjectionMatrix, frameData.rightViewMatrix, pose, vrDisplay._getFieldOfView("right"), vrDisplay._getEyeOffset("right"), vrDisplay);
          return true;
        };
      }();
      var isInsideCrossOriginIFrame = function isInsideCrossOriginIFrame2() {
        var isFramed = window.self !== window.top;
        var refOrigin = getOriginFromUrl(document.referrer);
        var thisOrigin = getOriginFromUrl(window.location.href);
        return isFramed && refOrigin !== thisOrigin;
      };
      var getOriginFromUrl = function getOriginFromUrl2(url) {
        var domainIdx;
        var protoSepIdx = url.indexOf("://");
        if (protoSepIdx !== -1) {
          domainIdx = protoSepIdx + 3;
        } else {
          domainIdx = 0;
        }
        var domainEndIdx = url.indexOf("/", domainIdx);
        if (domainEndIdx === -1) {
          domainEndIdx = url.length;
        }
        return url.substring(0, domainEndIdx);
      };
      var getQuaternionAngle = function getQuaternionAngle2(quat) {
        if (quat.w > 1) {
          console.warn("getQuaternionAngle: w > 1");
          return 0;
        }
        var angle3 = 2 * Math.acos(quat.w);
        return angle3;
      };
      var warnOnce = function() {
        var observedWarnings = {};
        return function(key, message) {
          if (observedWarnings[key] === void 0) {
            console.warn("webvr-polyfill: " + message);
            observedWarnings[key] = true;
          }
        };
      }();
      var deprecateWarning = function deprecateWarning2(deprecated, suggested) {
        var alternative = suggested ? "Please use " + suggested + " instead." : "";
        warnOnce(deprecated, deprecated + " has been deprecated. This may not work on native WebVR displays. " + alternative);
      };
      function WGLUPreserveGLState(gl, bindings, callback) {
        if (!bindings) {
          callback(gl);
          return;
        }
        var boundValues = [];
        var activeTexture = null;
        for (var i = 0; i < bindings.length; ++i) {
          var binding = bindings[i];
          switch (binding) {
            case gl.TEXTURE_BINDING_2D:
            case gl.TEXTURE_BINDING_CUBE_MAP:
              var textureUnit = bindings[++i];
              if (textureUnit < gl.TEXTURE0 || textureUnit > gl.TEXTURE31) {
                console.error("TEXTURE_BINDING_2D or TEXTURE_BINDING_CUBE_MAP must be followed by a valid texture unit");
                boundValues.push(null, null);
                break;
              }
              if (!activeTexture) {
                activeTexture = gl.getParameter(gl.ACTIVE_TEXTURE);
              }
              gl.activeTexture(textureUnit);
              boundValues.push(gl.getParameter(binding), null);
              break;
            case gl.ACTIVE_TEXTURE:
              activeTexture = gl.getParameter(gl.ACTIVE_TEXTURE);
              boundValues.push(null);
              break;
            default:
              boundValues.push(gl.getParameter(binding));
              break;
          }
        }
        callback(gl);
        for (var i = 0; i < bindings.length; ++i) {
          var binding = bindings[i];
          var boundValue = boundValues[i];
          switch (binding) {
            case gl.ACTIVE_TEXTURE:
              break;
            case gl.ARRAY_BUFFER_BINDING:
              gl.bindBuffer(gl.ARRAY_BUFFER, boundValue);
              break;
            case gl.COLOR_CLEAR_VALUE:
              gl.clearColor(boundValue[0], boundValue[1], boundValue[2], boundValue[3]);
              break;
            case gl.COLOR_WRITEMASK:
              gl.colorMask(boundValue[0], boundValue[1], boundValue[2], boundValue[3]);
              break;
            case gl.CURRENT_PROGRAM:
              gl.useProgram(boundValue);
              break;
            case gl.ELEMENT_ARRAY_BUFFER_BINDING:
              gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, boundValue);
              break;
            case gl.FRAMEBUFFER_BINDING:
              gl.bindFramebuffer(gl.FRAMEBUFFER, boundValue);
              break;
            case gl.RENDERBUFFER_BINDING:
              gl.bindRenderbuffer(gl.RENDERBUFFER, boundValue);
              break;
            case gl.TEXTURE_BINDING_2D:
              var textureUnit = bindings[++i];
              if (textureUnit < gl.TEXTURE0 || textureUnit > gl.TEXTURE31)
                break;
              gl.activeTexture(textureUnit);
              gl.bindTexture(gl.TEXTURE_2D, boundValue);
              break;
            case gl.TEXTURE_BINDING_CUBE_MAP:
              var textureUnit = bindings[++i];
              if (textureUnit < gl.TEXTURE0 || textureUnit > gl.TEXTURE31)
                break;
              gl.activeTexture(textureUnit);
              gl.bindTexture(gl.TEXTURE_CUBE_MAP, boundValue);
              break;
            case gl.VIEWPORT:
              gl.viewport(boundValue[0], boundValue[1], boundValue[2], boundValue[3]);
              break;
            case gl.BLEND:
            case gl.CULL_FACE:
            case gl.DEPTH_TEST:
            case gl.SCISSOR_TEST:
            case gl.STENCIL_TEST:
              if (boundValue) {
                gl.enable(binding);
              } else {
                gl.disable(binding);
              }
              break;
            default:
              console.log("No GL restore behavior for 0x" + binding.toString(16));
              break;
          }
          if (activeTexture) {
            gl.activeTexture(activeTexture);
          }
        }
      }
      var glPreserveState = WGLUPreserveGLState;
      var distortionVS = ["attribute vec2 position;", "attribute vec3 texCoord;", "varying vec2 vTexCoord;", "uniform vec4 viewportOffsetScale[2];", "void main() {", "  vec4 viewport = viewportOffsetScale[int(texCoord.z)];", "  vTexCoord = (texCoord.xy * viewport.zw) + viewport.xy;", "  gl_Position = vec4( position, 1.0, 1.0 );", "}"].join("\n");
      var distortionFS = ["precision mediump float;", "uniform sampler2D diffuse;", "varying vec2 vTexCoord;", "void main() {", "  gl_FragColor = texture2D(diffuse, vTexCoord);", "}"].join("\n");
      function CardboardDistorter(gl, cardboardUI, bufferScale, dirtySubmitFrameBindings) {
        this.gl = gl;
        this.cardboardUI = cardboardUI;
        this.bufferScale = bufferScale;
        this.dirtySubmitFrameBindings = dirtySubmitFrameBindings;
        this.ctxAttribs = gl.getContextAttributes();
        this.instanceExt = gl.getExtension("ANGLE_instanced_arrays");
        this.meshWidth = 20;
        this.meshHeight = 20;
        this.bufferWidth = gl.drawingBufferWidth;
        this.bufferHeight = gl.drawingBufferHeight;
        this.realBindFramebuffer = gl.bindFramebuffer;
        this.realEnable = gl.enable;
        this.realDisable = gl.disable;
        this.realColorMask = gl.colorMask;
        this.realClearColor = gl.clearColor;
        this.realViewport = gl.viewport;
        if (!isIOS2()) {
          this.realCanvasWidth = Object.getOwnPropertyDescriptor(gl.canvas.__proto__, "width");
          this.realCanvasHeight = Object.getOwnPropertyDescriptor(gl.canvas.__proto__, "height");
        }
        this.isPatched = false;
        this.lastBoundFramebuffer = null;
        this.cullFace = false;
        this.depthTest = false;
        this.blend = false;
        this.scissorTest = false;
        this.stencilTest = false;
        this.viewport = [0, 0, 0, 0];
        this.colorMask = [true, true, true, true];
        this.clearColor = [0, 0, 0, 0];
        this.attribs = {
          position: 0,
          texCoord: 1
        };
        this.program = linkProgram(gl, distortionVS, distortionFS, this.attribs);
        this.uniforms = getProgramUniforms(gl, this.program);
        this.viewportOffsetScale = new Float32Array(8);
        this.setTextureBounds();
        this.vertexBuffer = gl.createBuffer();
        this.indexBuffer = gl.createBuffer();
        this.indexCount = 0;
        this.renderTarget = gl.createTexture();
        this.framebuffer = gl.createFramebuffer();
        this.depthStencilBuffer = null;
        this.depthBuffer = null;
        this.stencilBuffer = null;
        if (this.ctxAttribs.depth && this.ctxAttribs.stencil) {
          this.depthStencilBuffer = gl.createRenderbuffer();
        } else if (this.ctxAttribs.depth) {
          this.depthBuffer = gl.createRenderbuffer();
        } else if (this.ctxAttribs.stencil) {
          this.stencilBuffer = gl.createRenderbuffer();
        }
        this.patch();
        this.onResize();
      }
      CardboardDistorter.prototype.destroy = function() {
        var gl = this.gl;
        this.unpatch();
        gl.deleteProgram(this.program);
        gl.deleteBuffer(this.vertexBuffer);
        gl.deleteBuffer(this.indexBuffer);
        gl.deleteTexture(this.renderTarget);
        gl.deleteFramebuffer(this.framebuffer);
        if (this.depthStencilBuffer) {
          gl.deleteRenderbuffer(this.depthStencilBuffer);
        }
        if (this.depthBuffer) {
          gl.deleteRenderbuffer(this.depthBuffer);
        }
        if (this.stencilBuffer) {
          gl.deleteRenderbuffer(this.stencilBuffer);
        }
        if (this.cardboardUI) {
          this.cardboardUI.destroy();
        }
      };
      CardboardDistorter.prototype.onResize = function() {
        var gl = this.gl;
        var self2 = this;
        var glState = [gl.RENDERBUFFER_BINDING, gl.TEXTURE_BINDING_2D, gl.TEXTURE0];
        glPreserveState(gl, glState, function(gl2) {
          self2.realBindFramebuffer.call(gl2, gl2.FRAMEBUFFER, null);
          if (self2.scissorTest) {
            self2.realDisable.call(gl2, gl2.SCISSOR_TEST);
          }
          self2.realColorMask.call(gl2, true, true, true, true);
          self2.realViewport.call(gl2, 0, 0, gl2.drawingBufferWidth, gl2.drawingBufferHeight);
          self2.realClearColor.call(gl2, 0, 0, 0, 1);
          gl2.clear(gl2.COLOR_BUFFER_BIT);
          self2.realBindFramebuffer.call(gl2, gl2.FRAMEBUFFER, self2.framebuffer);
          gl2.bindTexture(gl2.TEXTURE_2D, self2.renderTarget);
          gl2.texImage2D(gl2.TEXTURE_2D, 0, self2.ctxAttribs.alpha ? gl2.RGBA : gl2.RGB, self2.bufferWidth, self2.bufferHeight, 0, self2.ctxAttribs.alpha ? gl2.RGBA : gl2.RGB, gl2.UNSIGNED_BYTE, null);
          gl2.texParameteri(gl2.TEXTURE_2D, gl2.TEXTURE_MAG_FILTER, gl2.LINEAR);
          gl2.texParameteri(gl2.TEXTURE_2D, gl2.TEXTURE_MIN_FILTER, gl2.LINEAR);
          gl2.texParameteri(gl2.TEXTURE_2D, gl2.TEXTURE_WRAP_S, gl2.CLAMP_TO_EDGE);
          gl2.texParameteri(gl2.TEXTURE_2D, gl2.TEXTURE_WRAP_T, gl2.CLAMP_TO_EDGE);
          gl2.framebufferTexture2D(gl2.FRAMEBUFFER, gl2.COLOR_ATTACHMENT0, gl2.TEXTURE_2D, self2.renderTarget, 0);
          if (self2.ctxAttribs.depth && self2.ctxAttribs.stencil) {
            gl2.bindRenderbuffer(gl2.RENDERBUFFER, self2.depthStencilBuffer);
            gl2.renderbufferStorage(gl2.RENDERBUFFER, gl2.DEPTH_STENCIL, self2.bufferWidth, self2.bufferHeight);
            gl2.framebufferRenderbuffer(gl2.FRAMEBUFFER, gl2.DEPTH_STENCIL_ATTACHMENT, gl2.RENDERBUFFER, self2.depthStencilBuffer);
          } else if (self2.ctxAttribs.depth) {
            gl2.bindRenderbuffer(gl2.RENDERBUFFER, self2.depthBuffer);
            gl2.renderbufferStorage(gl2.RENDERBUFFER, gl2.DEPTH_COMPONENT16, self2.bufferWidth, self2.bufferHeight);
            gl2.framebufferRenderbuffer(gl2.FRAMEBUFFER, gl2.DEPTH_ATTACHMENT, gl2.RENDERBUFFER, self2.depthBuffer);
          } else if (self2.ctxAttribs.stencil) {
            gl2.bindRenderbuffer(gl2.RENDERBUFFER, self2.stencilBuffer);
            gl2.renderbufferStorage(gl2.RENDERBUFFER, gl2.STENCIL_INDEX8, self2.bufferWidth, self2.bufferHeight);
            gl2.framebufferRenderbuffer(gl2.FRAMEBUFFER, gl2.STENCIL_ATTACHMENT, gl2.RENDERBUFFER, self2.stencilBuffer);
          }
          if (!gl2.checkFramebufferStatus(gl2.FRAMEBUFFER) === gl2.FRAMEBUFFER_COMPLETE) {
            console.error("Framebuffer incomplete!");
          }
          self2.realBindFramebuffer.call(gl2, gl2.FRAMEBUFFER, self2.lastBoundFramebuffer);
          if (self2.scissorTest) {
            self2.realEnable.call(gl2, gl2.SCISSOR_TEST);
          }
          self2.realColorMask.apply(gl2, self2.colorMask);
          self2.realViewport.apply(gl2, self2.viewport);
          self2.realClearColor.apply(gl2, self2.clearColor);
        });
        if (this.cardboardUI) {
          this.cardboardUI.onResize();
        }
      };
      CardboardDistorter.prototype.patch = function() {
        if (this.isPatched) {
          return;
        }
        var self2 = this;
        var canvas = this.gl.canvas;
        var gl = this.gl;
        if (!isIOS2()) {
          canvas.width = getScreenWidth() * this.bufferScale;
          canvas.height = getScreenHeight() * this.bufferScale;
          Object.defineProperty(canvas, "width", {
            configurable: true,
            enumerable: true,
            get: function get() {
              return self2.bufferWidth;
            },
            set: function set3(value2) {
              self2.bufferWidth = value2;
              self2.realCanvasWidth.set.call(canvas, value2);
              self2.onResize();
            }
          });
          Object.defineProperty(canvas, "height", {
            configurable: true,
            enumerable: true,
            get: function get() {
              return self2.bufferHeight;
            },
            set: function set3(value2) {
              self2.bufferHeight = value2;
              self2.realCanvasHeight.set.call(canvas, value2);
              self2.onResize();
            }
          });
        }
        this.lastBoundFramebuffer = gl.getParameter(gl.FRAMEBUFFER_BINDING);
        if (this.lastBoundFramebuffer == null) {
          this.lastBoundFramebuffer = this.framebuffer;
          this.gl.bindFramebuffer(gl.FRAMEBUFFER, this.framebuffer);
        }
        this.gl.bindFramebuffer = function(target, framebuffer) {
          self2.lastBoundFramebuffer = framebuffer ? framebuffer : self2.framebuffer;
          self2.realBindFramebuffer.call(gl, target, self2.lastBoundFramebuffer);
        };
        this.cullFace = gl.getParameter(gl.CULL_FACE);
        this.depthTest = gl.getParameter(gl.DEPTH_TEST);
        this.blend = gl.getParameter(gl.BLEND);
        this.scissorTest = gl.getParameter(gl.SCISSOR_TEST);
        this.stencilTest = gl.getParameter(gl.STENCIL_TEST);
        gl.enable = function(pname) {
          switch (pname) {
            case gl.CULL_FACE:
              self2.cullFace = true;
              break;
            case gl.DEPTH_TEST:
              self2.depthTest = true;
              break;
            case gl.BLEND:
              self2.blend = true;
              break;
            case gl.SCISSOR_TEST:
              self2.scissorTest = true;
              break;
            case gl.STENCIL_TEST:
              self2.stencilTest = true;
              break;
          }
          self2.realEnable.call(gl, pname);
        };
        gl.disable = function(pname) {
          switch (pname) {
            case gl.CULL_FACE:
              self2.cullFace = false;
              break;
            case gl.DEPTH_TEST:
              self2.depthTest = false;
              break;
            case gl.BLEND:
              self2.blend = false;
              break;
            case gl.SCISSOR_TEST:
              self2.scissorTest = false;
              break;
            case gl.STENCIL_TEST:
              self2.stencilTest = false;
              break;
          }
          self2.realDisable.call(gl, pname);
        };
        this.colorMask = gl.getParameter(gl.COLOR_WRITEMASK);
        gl.colorMask = function(r, g, b, a) {
          self2.colorMask[0] = r;
          self2.colorMask[1] = g;
          self2.colorMask[2] = b;
          self2.colorMask[3] = a;
          self2.realColorMask.call(gl, r, g, b, a);
        };
        this.clearColor = gl.getParameter(gl.COLOR_CLEAR_VALUE);
        gl.clearColor = function(r, g, b, a) {
          self2.clearColor[0] = r;
          self2.clearColor[1] = g;
          self2.clearColor[2] = b;
          self2.clearColor[3] = a;
          self2.realClearColor.call(gl, r, g, b, a);
        };
        this.viewport = gl.getParameter(gl.VIEWPORT);
        gl.viewport = function(x, y, w, h) {
          self2.viewport[0] = x;
          self2.viewport[1] = y;
          self2.viewport[2] = w;
          self2.viewport[3] = h;
          self2.realViewport.call(gl, x, y, w, h);
        };
        this.isPatched = true;
        safariCssSizeWorkaround(canvas);
      };
      CardboardDistorter.prototype.unpatch = function() {
        if (!this.isPatched) {
          return;
        }
        var gl = this.gl;
        var canvas = this.gl.canvas;
        if (!isIOS2()) {
          Object.defineProperty(canvas, "width", this.realCanvasWidth);
          Object.defineProperty(canvas, "height", this.realCanvasHeight);
        }
        canvas.width = this.bufferWidth;
        canvas.height = this.bufferHeight;
        gl.bindFramebuffer = this.realBindFramebuffer;
        gl.enable = this.realEnable;
        gl.disable = this.realDisable;
        gl.colorMask = this.realColorMask;
        gl.clearColor = this.realClearColor;
        gl.viewport = this.realViewport;
        if (this.lastBoundFramebuffer == this.framebuffer) {
          gl.bindFramebuffer(gl.FRAMEBUFFER, null);
        }
        this.isPatched = false;
        setTimeout(function() {
          safariCssSizeWorkaround(canvas);
        }, 1);
      };
      CardboardDistorter.prototype.setTextureBounds = function(leftBounds, rightBounds) {
        if (!leftBounds) {
          leftBounds = [0, 0, 0.5, 1];
        }
        if (!rightBounds) {
          rightBounds = [0.5, 0, 0.5, 1];
        }
        this.viewportOffsetScale[0] = leftBounds[0];
        this.viewportOffsetScale[1] = leftBounds[1];
        this.viewportOffsetScale[2] = leftBounds[2];
        this.viewportOffsetScale[3] = leftBounds[3];
        this.viewportOffsetScale[4] = rightBounds[0];
        this.viewportOffsetScale[5] = rightBounds[1];
        this.viewportOffsetScale[6] = rightBounds[2];
        this.viewportOffsetScale[7] = rightBounds[3];
      };
      CardboardDistorter.prototype.submitFrame = function() {
        var gl = this.gl;
        var self2 = this;
        var glState = [];
        if (!this.dirtySubmitFrameBindings) {
          glState.push(gl.CURRENT_PROGRAM, gl.ARRAY_BUFFER_BINDING, gl.ELEMENT_ARRAY_BUFFER_BINDING, gl.TEXTURE_BINDING_2D, gl.TEXTURE0);
        }
        glPreserveState(gl, glState, function(gl2) {
          self2.realBindFramebuffer.call(gl2, gl2.FRAMEBUFFER, null);
          var positionDivisor = 0;
          var texCoordDivisor = 0;
          if (self2.instanceExt) {
            positionDivisor = gl2.getVertexAttrib(self2.attribs.position, self2.instanceExt.VERTEX_ATTRIB_ARRAY_DIVISOR_ANGLE);
            texCoordDivisor = gl2.getVertexAttrib(self2.attribs.texCoord, self2.instanceExt.VERTEX_ATTRIB_ARRAY_DIVISOR_ANGLE);
          }
          if (self2.cullFace) {
            self2.realDisable.call(gl2, gl2.CULL_FACE);
          }
          if (self2.depthTest) {
            self2.realDisable.call(gl2, gl2.DEPTH_TEST);
          }
          if (self2.blend) {
            self2.realDisable.call(gl2, gl2.BLEND);
          }
          if (self2.scissorTest) {
            self2.realDisable.call(gl2, gl2.SCISSOR_TEST);
          }
          if (self2.stencilTest) {
            self2.realDisable.call(gl2, gl2.STENCIL_TEST);
          }
          self2.realColorMask.call(gl2, true, true, true, true);
          self2.realViewport.call(gl2, 0, 0, gl2.drawingBufferWidth, gl2.drawingBufferHeight);
          if (self2.ctxAttribs.alpha || isIOS2()) {
            self2.realClearColor.call(gl2, 0, 0, 0, 1);
            gl2.clear(gl2.COLOR_BUFFER_BIT);
          }
          gl2.useProgram(self2.program);
          gl2.bindBuffer(gl2.ELEMENT_ARRAY_BUFFER, self2.indexBuffer);
          gl2.bindBuffer(gl2.ARRAY_BUFFER, self2.vertexBuffer);
          gl2.enableVertexAttribArray(self2.attribs.position);
          gl2.enableVertexAttribArray(self2.attribs.texCoord);
          gl2.vertexAttribPointer(self2.attribs.position, 2, gl2.FLOAT, false, 20, 0);
          gl2.vertexAttribPointer(self2.attribs.texCoord, 3, gl2.FLOAT, false, 20, 8);
          if (self2.instanceExt) {
            if (positionDivisor != 0) {
              self2.instanceExt.vertexAttribDivisorANGLE(self2.attribs.position, 0);
            }
            if (texCoordDivisor != 0) {
              self2.instanceExt.vertexAttribDivisorANGLE(self2.attribs.texCoord, 0);
            }
          }
          gl2.activeTexture(gl2.TEXTURE0);
          gl2.uniform1i(self2.uniforms.diffuse, 0);
          gl2.bindTexture(gl2.TEXTURE_2D, self2.renderTarget);
          gl2.uniform4fv(self2.uniforms.viewportOffsetScale, self2.viewportOffsetScale);
          gl2.drawElements(gl2.TRIANGLES, self2.indexCount, gl2.UNSIGNED_SHORT, 0);
          if (self2.cardboardUI) {
            self2.cardboardUI.renderNoState();
          }
          self2.realBindFramebuffer.call(self2.gl, gl2.FRAMEBUFFER, self2.framebuffer);
          if (!self2.ctxAttribs.preserveDrawingBuffer) {
            self2.realClearColor.call(gl2, 0, 0, 0, 0);
            gl2.clear(gl2.COLOR_BUFFER_BIT);
          }
          if (!self2.dirtySubmitFrameBindings) {
            self2.realBindFramebuffer.call(gl2, gl2.FRAMEBUFFER, self2.lastBoundFramebuffer);
          }
          if (self2.cullFace) {
            self2.realEnable.call(gl2, gl2.CULL_FACE);
          }
          if (self2.depthTest) {
            self2.realEnable.call(gl2, gl2.DEPTH_TEST);
          }
          if (self2.blend) {
            self2.realEnable.call(gl2, gl2.BLEND);
          }
          if (self2.scissorTest) {
            self2.realEnable.call(gl2, gl2.SCISSOR_TEST);
          }
          if (self2.stencilTest) {
            self2.realEnable.call(gl2, gl2.STENCIL_TEST);
          }
          self2.realColorMask.apply(gl2, self2.colorMask);
          self2.realViewport.apply(gl2, self2.viewport);
          if (self2.ctxAttribs.alpha || !self2.ctxAttribs.preserveDrawingBuffer) {
            self2.realClearColor.apply(gl2, self2.clearColor);
          }
          if (self2.instanceExt) {
            if (positionDivisor != 0) {
              self2.instanceExt.vertexAttribDivisorANGLE(self2.attribs.position, positionDivisor);
            }
            if (texCoordDivisor != 0) {
              self2.instanceExt.vertexAttribDivisorANGLE(self2.attribs.texCoord, texCoordDivisor);
            }
          }
        });
        if (isIOS2()) {
          var canvas = gl.canvas;
          if (canvas.width != self2.bufferWidth || canvas.height != self2.bufferHeight) {
            self2.bufferWidth = canvas.width;
            self2.bufferHeight = canvas.height;
            self2.onResize();
          }
        }
      };
      CardboardDistorter.prototype.updateDeviceInfo = function(deviceInfo) {
        var gl = this.gl;
        var self2 = this;
        var glState = [gl.ARRAY_BUFFER_BINDING, gl.ELEMENT_ARRAY_BUFFER_BINDING];
        glPreserveState(gl, glState, function(gl2) {
          var vertices = self2.computeMeshVertices_(self2.meshWidth, self2.meshHeight, deviceInfo);
          gl2.bindBuffer(gl2.ARRAY_BUFFER, self2.vertexBuffer);
          gl2.bufferData(gl2.ARRAY_BUFFER, vertices, gl2.STATIC_DRAW);
          if (!self2.indexCount) {
            var indices = self2.computeMeshIndices_(self2.meshWidth, self2.meshHeight);
            gl2.bindBuffer(gl2.ELEMENT_ARRAY_BUFFER, self2.indexBuffer);
            gl2.bufferData(gl2.ELEMENT_ARRAY_BUFFER, indices, gl2.STATIC_DRAW);
            self2.indexCount = indices.length;
          }
        });
      };
      CardboardDistorter.prototype.computeMeshVertices_ = function(width2, height2, deviceInfo) {
        var vertices = new Float32Array(2 * width2 * height2 * 5);
        var lensFrustum = deviceInfo.getLeftEyeVisibleTanAngles();
        var noLensFrustum = deviceInfo.getLeftEyeNoLensTanAngles();
        var viewport = deviceInfo.getLeftEyeVisibleScreenRect(noLensFrustum);
        var vidx = 0;
        for (var e2 = 0; e2 < 2; e2++) {
          for (var j = 0; j < height2; j++) {
            for (var i = 0; i < width2; i++, vidx++) {
              var u = i / (width2 - 1);
              var v = j / (height2 - 1);
              var s = u;
              var t2 = v;
              var x = lerp4(lensFrustum[0], lensFrustum[2], u);
              var y = lerp4(lensFrustum[3], lensFrustum[1], v);
              var d = Math.sqrt(x * x + y * y);
              var r = deviceInfo.distortion.distortInverse(d);
              var p = x * r / d;
              var q = y * r / d;
              u = (p - noLensFrustum[0]) / (noLensFrustum[2] - noLensFrustum[0]);
              v = (q - noLensFrustum[3]) / (noLensFrustum[1] - noLensFrustum[3]);
              u = (viewport.x + u * viewport.width - 0.5) * 2;
              v = (viewport.y + v * viewport.height - 0.5) * 2;
              vertices[vidx * 5 + 0] = u;
              vertices[vidx * 5 + 1] = v;
              vertices[vidx * 5 + 2] = s;
              vertices[vidx * 5 + 3] = t2;
              vertices[vidx * 5 + 4] = e2;
            }
          }
          var w = lensFrustum[2] - lensFrustum[0];
          lensFrustum[0] = -(w + lensFrustum[0]);
          lensFrustum[2] = w - lensFrustum[2];
          w = noLensFrustum[2] - noLensFrustum[0];
          noLensFrustum[0] = -(w + noLensFrustum[0]);
          noLensFrustum[2] = w - noLensFrustum[2];
          viewport.x = 1 - (viewport.x + viewport.width);
        }
        return vertices;
      };
      CardboardDistorter.prototype.computeMeshIndices_ = function(width2, height2) {
        var indices = new Uint16Array(2 * (width2 - 1) * (height2 - 1) * 6);
        var halfwidth = width2 / 2;
        var halfheight = height2 / 2;
        var vidx = 0;
        var iidx = 0;
        for (var e2 = 0; e2 < 2; e2++) {
          for (var j = 0; j < height2; j++) {
            for (var i = 0; i < width2; i++, vidx++) {
              if (i == 0 || j == 0)
                continue;
              if (i <= halfwidth == j <= halfheight) {
                indices[iidx++] = vidx;
                indices[iidx++] = vidx - width2 - 1;
                indices[iidx++] = vidx - width2;
                indices[iidx++] = vidx - width2 - 1;
                indices[iidx++] = vidx;
                indices[iidx++] = vidx - 1;
              } else {
                indices[iidx++] = vidx - 1;
                indices[iidx++] = vidx - width2;
                indices[iidx++] = vidx;
                indices[iidx++] = vidx - width2;
                indices[iidx++] = vidx - 1;
                indices[iidx++] = vidx - width2 - 1;
              }
            }
          }
        }
        return indices;
      };
      CardboardDistorter.prototype.getOwnPropertyDescriptor_ = function(proto, attrName) {
        var descriptor = Object.getOwnPropertyDescriptor(proto, attrName);
        if (descriptor.get === void 0 || descriptor.set === void 0) {
          descriptor.configurable = true;
          descriptor.enumerable = true;
          descriptor.get = function() {
            return this.getAttribute(attrName);
          };
          descriptor.set = function(val) {
            this.setAttribute(attrName, val);
          };
        }
        return descriptor;
      };
      var uiVS = ["attribute vec2 position;", "uniform mat4 projectionMat;", "void main() {", "  gl_Position = projectionMat * vec4( position, -1.0, 1.0 );", "}"].join("\n");
      var uiFS = ["precision mediump float;", "uniform vec4 color;", "void main() {", "  gl_FragColor = color;", "}"].join("\n");
      var DEG2RAD = Math.PI / 180;
      var kAnglePerGearSection = 60;
      var kOuterRimEndAngle = 12;
      var kInnerRimBeginAngle = 20;
      var kOuterRadius = 1;
      var kMiddleRadius = 0.75;
      var kInnerRadius = 0.3125;
      var kCenterLineThicknessDp = 4;
      var kButtonWidthDp = 28;
      var kTouchSlopFactor = 1.5;
      function CardboardUI(gl) {
        this.gl = gl;
        this.attribs = {
          position: 0
        };
        this.program = linkProgram(gl, uiVS, uiFS, this.attribs);
        this.uniforms = getProgramUniforms(gl, this.program);
        this.vertexBuffer = gl.createBuffer();
        this.gearOffset = 0;
        this.gearVertexCount = 0;
        this.arrowOffset = 0;
        this.arrowVertexCount = 0;
        this.projMat = new Float32Array(16);
        this.listener = null;
        this.onResize();
      }
      CardboardUI.prototype.destroy = function() {
        var gl = this.gl;
        if (this.listener) {
          gl.canvas.removeEventListener("click", this.listener, false);
        }
        gl.deleteProgram(this.program);
        gl.deleteBuffer(this.vertexBuffer);
      };
      CardboardUI.prototype.listen = function(optionsCallback, backCallback) {
        var canvas = this.gl.canvas;
        this.listener = function(event) {
          var midline = canvas.clientWidth / 2;
          var buttonSize = kButtonWidthDp * kTouchSlopFactor;
          if (event.clientX > midline - buttonSize && event.clientX < midline + buttonSize && event.clientY > canvas.clientHeight - buttonSize) {
            optionsCallback(event);
          } else if (event.clientX < buttonSize && event.clientY < buttonSize) {
            backCallback(event);
          }
        };
        canvas.addEventListener("click", this.listener, false);
      };
      CardboardUI.prototype.onResize = function() {
        var gl = this.gl;
        var self2 = this;
        var glState = [gl.ARRAY_BUFFER_BINDING];
        glPreserveState(gl, glState, function(gl2) {
          var vertices = [];
          var midline = gl2.drawingBufferWidth / 2;
          var physicalPixels = Math.max(screen.width, screen.height) * window.devicePixelRatio;
          var scalingRatio = gl2.drawingBufferWidth / physicalPixels;
          var dps = scalingRatio * window.devicePixelRatio;
          var lineWidth = kCenterLineThicknessDp * dps / 2;
          var buttonSize = kButtonWidthDp * kTouchSlopFactor * dps;
          var buttonScale = kButtonWidthDp * dps / 2;
          var buttonBorder = (kButtonWidthDp * kTouchSlopFactor - kButtonWidthDp) * dps;
          vertices.push(midline - lineWidth, buttonSize);
          vertices.push(midline - lineWidth, gl2.drawingBufferHeight);
          vertices.push(midline + lineWidth, buttonSize);
          vertices.push(midline + lineWidth, gl2.drawingBufferHeight);
          self2.gearOffset = vertices.length / 2;
          function addGearSegment(theta, r) {
            var angle3 = (90 - theta) * DEG2RAD;
            var x = Math.cos(angle3);
            var y = Math.sin(angle3);
            vertices.push(kInnerRadius * x * buttonScale + midline, kInnerRadius * y * buttonScale + buttonScale);
            vertices.push(r * x * buttonScale + midline, r * y * buttonScale + buttonScale);
          }
          for (var i = 0; i <= 6; i++) {
            var segmentTheta = i * kAnglePerGearSection;
            addGearSegment(segmentTheta, kOuterRadius);
            addGearSegment(segmentTheta + kOuterRimEndAngle, kOuterRadius);
            addGearSegment(segmentTheta + kInnerRimBeginAngle, kMiddleRadius);
            addGearSegment(segmentTheta + (kAnglePerGearSection - kInnerRimBeginAngle), kMiddleRadius);
            addGearSegment(segmentTheta + (kAnglePerGearSection - kOuterRimEndAngle), kOuterRadius);
          }
          self2.gearVertexCount = vertices.length / 2 - self2.gearOffset;
          self2.arrowOffset = vertices.length / 2;
          function addArrowVertex(x, y) {
            vertices.push(buttonBorder + x, gl2.drawingBufferHeight - buttonBorder - y);
          }
          var angledLineWidth = lineWidth / Math.sin(45 * DEG2RAD);
          addArrowVertex(0, buttonScale);
          addArrowVertex(buttonScale, 0);
          addArrowVertex(buttonScale + angledLineWidth, angledLineWidth);
          addArrowVertex(angledLineWidth, buttonScale + angledLineWidth);
          addArrowVertex(angledLineWidth, buttonScale - angledLineWidth);
          addArrowVertex(0, buttonScale);
          addArrowVertex(buttonScale, buttonScale * 2);
          addArrowVertex(buttonScale + angledLineWidth, buttonScale * 2 - angledLineWidth);
          addArrowVertex(angledLineWidth, buttonScale - angledLineWidth);
          addArrowVertex(0, buttonScale);
          addArrowVertex(angledLineWidth, buttonScale - lineWidth);
          addArrowVertex(kButtonWidthDp * dps, buttonScale - lineWidth);
          addArrowVertex(angledLineWidth, buttonScale + lineWidth);
          addArrowVertex(kButtonWidthDp * dps, buttonScale + lineWidth);
          self2.arrowVertexCount = vertices.length / 2 - self2.arrowOffset;
          gl2.bindBuffer(gl2.ARRAY_BUFFER, self2.vertexBuffer);
          gl2.bufferData(gl2.ARRAY_BUFFER, new Float32Array(vertices), gl2.STATIC_DRAW);
        });
      };
      CardboardUI.prototype.render = function() {
        var gl = this.gl;
        var self2 = this;
        var glState = [gl.CULL_FACE, gl.DEPTH_TEST, gl.BLEND, gl.SCISSOR_TEST, gl.STENCIL_TEST, gl.COLOR_WRITEMASK, gl.VIEWPORT, gl.CURRENT_PROGRAM, gl.ARRAY_BUFFER_BINDING];
        glPreserveState(gl, glState, function(gl2) {
          gl2.disable(gl2.CULL_FACE);
          gl2.disable(gl2.DEPTH_TEST);
          gl2.disable(gl2.BLEND);
          gl2.disable(gl2.SCISSOR_TEST);
          gl2.disable(gl2.STENCIL_TEST);
          gl2.colorMask(true, true, true, true);
          gl2.viewport(0, 0, gl2.drawingBufferWidth, gl2.drawingBufferHeight);
          self2.renderNoState();
        });
      };
      CardboardUI.prototype.renderNoState = function() {
        var gl = this.gl;
        gl.useProgram(this.program);
        gl.bindBuffer(gl.ARRAY_BUFFER, this.vertexBuffer);
        gl.enableVertexAttribArray(this.attribs.position);
        gl.vertexAttribPointer(this.attribs.position, 2, gl.FLOAT, false, 8, 0);
        gl.uniform4f(this.uniforms.color, 1, 1, 1, 1);
        orthoMatrix(this.projMat, 0, gl.drawingBufferWidth, 0, gl.drawingBufferHeight, 0.1, 1024);
        gl.uniformMatrix4fv(this.uniforms.projectionMat, false, this.projMat);
        gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
        gl.drawArrays(gl.TRIANGLE_STRIP, this.gearOffset, this.gearVertexCount);
        gl.drawArrays(gl.TRIANGLE_STRIP, this.arrowOffset, this.arrowVertexCount);
      };
      function Distortion(coefficients) {
        this.coefficients = coefficients;
      }
      Distortion.prototype.distortInverse = function(radius2) {
        var r0 = 0;
        var r1 = 1;
        var dr0 = radius2 - this.distort(r0);
        while (Math.abs(r1 - r0) > 1e-4) {
          var dr1 = radius2 - this.distort(r1);
          var r2 = r1 - dr1 * ((r1 - r0) / (dr1 - dr0));
          r0 = r1;
          r1 = r2;
          dr0 = dr1;
        }
        return r1;
      };
      Distortion.prototype.distort = function(radius2) {
        var r2 = radius2 * radius2;
        var ret = 0;
        for (var i = 0; i < this.coefficients.length; i++) {
          ret = r2 * (ret + this.coefficients[i]);
        }
        return (ret + 1) * radius2;
      };
      var degToRad = Math.PI / 180;
      var radToDeg = 180 / Math.PI;
      var Vector3 = function Vector32(x, y, z) {
        this.x = x || 0;
        this.y = y || 0;
        this.z = z || 0;
      };
      Vector3.prototype = {
        constructor: Vector3,
        set: function set3(x, y, z) {
          this.x = x;
          this.y = y;
          this.z = z;
          return this;
        },
        copy: function copy6(v) {
          this.x = v.x;
          this.y = v.y;
          this.z = v.z;
          return this;
        },
        length: function length4() {
          return Math.sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        },
        normalize: function normalize5() {
          var scalar = this.length();
          if (scalar !== 0) {
            var invScalar = 1 / scalar;
            this.multiplyScalar(invScalar);
          } else {
            this.x = 0;
            this.y = 0;
            this.z = 0;
          }
          return this;
        },
        multiplyScalar: function multiplyScalar(scalar) {
          this.x *= scalar;
          this.y *= scalar;
          this.z *= scalar;
        },
        applyQuaternion: function applyQuaternion(q) {
          var x = this.x;
          var y = this.y;
          var z = this.z;
          var qx = q.x;
          var qy = q.y;
          var qz = q.z;
          var qw = q.w;
          var ix = qw * x + qy * z - qz * y;
          var iy = qw * y + qz * x - qx * z;
          var iz = qw * z + qx * y - qy * x;
          var iw = -qx * x - qy * y - qz * z;
          this.x = ix * qw + iw * -qx + iy * -qz - iz * -qy;
          this.y = iy * qw + iw * -qy + iz * -qx - ix * -qz;
          this.z = iz * qw + iw * -qz + ix * -qy - iy * -qx;
          return this;
        },
        dot: function dot4(v) {
          return this.x * v.x + this.y * v.y + this.z * v.z;
        },
        crossVectors: function crossVectors(a, b) {
          var ax = a.x, ay = a.y, az = a.z;
          var bx = b.x, by = b.y, bz = b.z;
          this.x = ay * bz - az * by;
          this.y = az * bx - ax * bz;
          this.z = ax * by - ay * bx;
          return this;
        }
      };
      var Quaternion = function Quaternion2(x, y, z, w) {
        this.x = x || 0;
        this.y = y || 0;
        this.z = z || 0;
        this.w = w !== void 0 ? w : 1;
      };
      Quaternion.prototype = {
        constructor: Quaternion,
        set: function set3(x, y, z, w) {
          this.x = x;
          this.y = y;
          this.z = z;
          this.w = w;
          return this;
        },
        copy: function copy6(quaternion) {
          this.x = quaternion.x;
          this.y = quaternion.y;
          this.z = quaternion.z;
          this.w = quaternion.w;
          return this;
        },
        setFromEulerXYZ: function setFromEulerXYZ(x, y, z) {
          var c1 = Math.cos(x / 2);
          var c2 = Math.cos(y / 2);
          var c3 = Math.cos(z / 2);
          var s1 = Math.sin(x / 2);
          var s2 = Math.sin(y / 2);
          var s3 = Math.sin(z / 2);
          this.x = s1 * c2 * c3 + c1 * s2 * s3;
          this.y = c1 * s2 * c3 - s1 * c2 * s3;
          this.z = c1 * c2 * s3 + s1 * s2 * c3;
          this.w = c1 * c2 * c3 - s1 * s2 * s3;
          return this;
        },
        setFromEulerYXZ: function setFromEulerYXZ(x, y, z) {
          var c1 = Math.cos(x / 2);
          var c2 = Math.cos(y / 2);
          var c3 = Math.cos(z / 2);
          var s1 = Math.sin(x / 2);
          var s2 = Math.sin(y / 2);
          var s3 = Math.sin(z / 2);
          this.x = s1 * c2 * c3 + c1 * s2 * s3;
          this.y = c1 * s2 * c3 - s1 * c2 * s3;
          this.z = c1 * c2 * s3 - s1 * s2 * c3;
          this.w = c1 * c2 * c3 + s1 * s2 * s3;
          return this;
        },
        setFromAxisAngle: function setFromAxisAngle(axis, angle3) {
          var halfAngle = angle3 / 2, s = Math.sin(halfAngle);
          this.x = axis.x * s;
          this.y = axis.y * s;
          this.z = axis.z * s;
          this.w = Math.cos(halfAngle);
          return this;
        },
        multiply: function multiply4(q) {
          return this.multiplyQuaternions(this, q);
        },
        multiplyQuaternions: function multiplyQuaternions(a, b) {
          var qax = a.x, qay = a.y, qaz = a.z, qaw = a.w;
          var qbx = b.x, qby = b.y, qbz = b.z, qbw = b.w;
          this.x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
          this.y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
          this.z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
          this.w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;
          return this;
        },
        inverse: function inverse2() {
          this.x *= -1;
          this.y *= -1;
          this.z *= -1;
          this.normalize();
          return this;
        },
        normalize: function normalize5() {
          var l = Math.sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
          if (l === 0) {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 1;
          } else {
            l = 1 / l;
            this.x = this.x * l;
            this.y = this.y * l;
            this.z = this.z * l;
            this.w = this.w * l;
          }
          return this;
        },
        slerp: function slerp2(qb, t2) {
          if (t2 === 0)
            return this;
          if (t2 === 1)
            return this.copy(qb);
          var x = this.x, y = this.y, z = this.z, w = this.w;
          var cosHalfTheta = w * qb.w + x * qb.x + y * qb.y + z * qb.z;
          if (cosHalfTheta < 0) {
            this.w = -qb.w;
            this.x = -qb.x;
            this.y = -qb.y;
            this.z = -qb.z;
            cosHalfTheta = -cosHalfTheta;
          } else {
            this.copy(qb);
          }
          if (cosHalfTheta >= 1) {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
            return this;
          }
          var halfTheta = Math.acos(cosHalfTheta);
          var sinHalfTheta = Math.sqrt(1 - cosHalfTheta * cosHalfTheta);
          if (Math.abs(sinHalfTheta) < 1e-3) {
            this.w = 0.5 * (w + this.w);
            this.x = 0.5 * (x + this.x);
            this.y = 0.5 * (y + this.y);
            this.z = 0.5 * (z + this.z);
            return this;
          }
          var ratioA = Math.sin((1 - t2) * halfTheta) / sinHalfTheta, ratioB = Math.sin(t2 * halfTheta) / sinHalfTheta;
          this.w = w * ratioA + this.w * ratioB;
          this.x = x * ratioA + this.x * ratioB;
          this.y = y * ratioA + this.y * ratioB;
          this.z = z * ratioA + this.z * ratioB;
          return this;
        },
        setFromUnitVectors: function() {
          var v1, r;
          var EPS = 1e-6;
          return function(vFrom, vTo) {
            if (v1 === void 0)
              v1 = new Vector3();
            r = vFrom.dot(vTo) + 1;
            if (r < EPS) {
              r = 0;
              if (Math.abs(vFrom.x) > Math.abs(vFrom.z)) {
                v1.set(-vFrom.y, vFrom.x, 0);
              } else {
                v1.set(0, -vFrom.z, vFrom.y);
              }
            } else {
              v1.crossVectors(vFrom, vTo);
            }
            this.x = v1.x;
            this.y = v1.y;
            this.z = v1.z;
            this.w = r;
            this.normalize();
            return this;
          };
        }()
      };
      function Device(params) {
        this.width = params.width || getScreenWidth();
        this.height = params.height || getScreenHeight();
        this.widthMeters = params.widthMeters;
        this.heightMeters = params.heightMeters;
        this.bevelMeters = params.bevelMeters;
      }
      var DEFAULT_ANDROID = new Device({
        widthMeters: 0.11,
        heightMeters: 0.062,
        bevelMeters: 4e-3
      });
      var DEFAULT_IOS = new Device({
        widthMeters: 0.1038,
        heightMeters: 0.0584,
        bevelMeters: 4e-3
      });
      var Viewers = {
        CardboardV1: new CardboardViewer({
          id: "CardboardV1",
          label: "Cardboard I/O 2014",
          fov: 40,
          interLensDistance: 0.06,
          baselineLensDistance: 0.035,
          screenLensDistance: 0.042,
          distortionCoefficients: [0.441, 0.156],
          inverseCoefficients: [-0.4410035, 0.42756155, -0.4804439, 0.5460139, -0.58821183, 0.5733938, -0.48303202, 0.33299083, -0.17573841, 0.0651772, -0.01488963, 1559834e-9]
        }),
        CardboardV2: new CardboardViewer({
          id: "CardboardV2",
          label: "Cardboard I/O 2015",
          fov: 60,
          interLensDistance: 0.064,
          baselineLensDistance: 0.035,
          screenLensDistance: 0.039,
          distortionCoefficients: [0.34, 0.55],
          inverseCoefficients: [-0.33836704, -0.18162185, 0.862655, -1.2462051, 1.0560602, -0.58208317, 0.21609078, -0.05444823, 9177956e-9, -9904169e-10, 6183535e-11, -16981803e-13]
        })
      };
      function DeviceInfo(deviceParams, additionalViewers) {
        this.viewer = Viewers.CardboardV2;
        this.updateDeviceParams(deviceParams);
        this.distortion = new Distortion(this.viewer.distortionCoefficients);
        for (var i = 0; i < additionalViewers.length; i++) {
          var viewer = additionalViewers[i];
          Viewers[viewer.id] = new CardboardViewer(viewer);
        }
      }
      DeviceInfo.prototype.updateDeviceParams = function(deviceParams) {
        this.device = this.determineDevice_(deviceParams) || this.device;
      };
      DeviceInfo.prototype.getDevice = function() {
        return this.device;
      };
      DeviceInfo.prototype.setViewer = function(viewer) {
        this.viewer = viewer;
        this.distortion = new Distortion(this.viewer.distortionCoefficients);
      };
      DeviceInfo.prototype.determineDevice_ = function(deviceParams) {
        if (!deviceParams) {
          if (isIOS2()) {
            console.warn("Using fallback iOS device measurements.");
            return DEFAULT_IOS;
          } else {
            console.warn("Using fallback Android device measurements.");
            return DEFAULT_ANDROID;
          }
        }
        var METERS_PER_INCH = 0.0254;
        var metersPerPixelX = METERS_PER_INCH / deviceParams.xdpi;
        var metersPerPixelY = METERS_PER_INCH / deviceParams.ydpi;
        var width2 = getScreenWidth();
        var height2 = getScreenHeight();
        return new Device({
          widthMeters: metersPerPixelX * width2,
          heightMeters: metersPerPixelY * height2,
          bevelMeters: deviceParams.bevelMm * 1e-3
        });
      };
      DeviceInfo.prototype.getDistortedFieldOfViewLeftEye = function() {
        var viewer = this.viewer;
        var device = this.device;
        var distortion = this.distortion;
        var eyeToScreenDistance = viewer.screenLensDistance;
        var outerDist = (device.widthMeters - viewer.interLensDistance) / 2;
        var innerDist = viewer.interLensDistance / 2;
        var bottomDist = viewer.baselineLensDistance - device.bevelMeters;
        var topDist = device.heightMeters - bottomDist;
        var outerAngle = radToDeg * Math.atan(distortion.distort(outerDist / eyeToScreenDistance));
        var innerAngle = radToDeg * Math.atan(distortion.distort(innerDist / eyeToScreenDistance));
        var bottomAngle = radToDeg * Math.atan(distortion.distort(bottomDist / eyeToScreenDistance));
        var topAngle = radToDeg * Math.atan(distortion.distort(topDist / eyeToScreenDistance));
        return {
          leftDegrees: Math.min(outerAngle, viewer.fov),
          rightDegrees: Math.min(innerAngle, viewer.fov),
          downDegrees: Math.min(bottomAngle, viewer.fov),
          upDegrees: Math.min(topAngle, viewer.fov)
        };
      };
      DeviceInfo.prototype.getLeftEyeVisibleTanAngles = function() {
        var viewer = this.viewer;
        var device = this.device;
        var distortion = this.distortion;
        var fovLeft = Math.tan(-degToRad * viewer.fov);
        var fovTop = Math.tan(degToRad * viewer.fov);
        var fovRight = Math.tan(degToRad * viewer.fov);
        var fovBottom = Math.tan(-degToRad * viewer.fov);
        var halfWidth = device.widthMeters / 4;
        var halfHeight = device.heightMeters / 2;
        var verticalLensOffset = viewer.baselineLensDistance - device.bevelMeters - halfHeight;
        var centerX = viewer.interLensDistance / 2 - halfWidth;
        var centerY = -verticalLensOffset;
        var centerZ = viewer.screenLensDistance;
        var screenLeft = distortion.distort((centerX - halfWidth) / centerZ);
        var screenTop = distortion.distort((centerY + halfHeight) / centerZ);
        var screenRight = distortion.distort((centerX + halfWidth) / centerZ);
        var screenBottom = distortion.distort((centerY - halfHeight) / centerZ);
        var result = new Float32Array(4);
        result[0] = Math.max(fovLeft, screenLeft);
        result[1] = Math.min(fovTop, screenTop);
        result[2] = Math.min(fovRight, screenRight);
        result[3] = Math.max(fovBottom, screenBottom);
        return result;
      };
      DeviceInfo.prototype.getLeftEyeNoLensTanAngles = function() {
        var viewer = this.viewer;
        var device = this.device;
        var distortion = this.distortion;
        var result = new Float32Array(4);
        var fovLeft = distortion.distortInverse(Math.tan(-degToRad * viewer.fov));
        var fovTop = distortion.distortInverse(Math.tan(degToRad * viewer.fov));
        var fovRight = distortion.distortInverse(Math.tan(degToRad * viewer.fov));
        var fovBottom = distortion.distortInverse(Math.tan(-degToRad * viewer.fov));
        var halfWidth = device.widthMeters / 4;
        var halfHeight = device.heightMeters / 2;
        var verticalLensOffset = viewer.baselineLensDistance - device.bevelMeters - halfHeight;
        var centerX = viewer.interLensDistance / 2 - halfWidth;
        var centerY = -verticalLensOffset;
        var centerZ = viewer.screenLensDistance;
        var screenLeft = (centerX - halfWidth) / centerZ;
        var screenTop = (centerY + halfHeight) / centerZ;
        var screenRight = (centerX + halfWidth) / centerZ;
        var screenBottom = (centerY - halfHeight) / centerZ;
        result[0] = Math.max(fovLeft, screenLeft);
        result[1] = Math.min(fovTop, screenTop);
        result[2] = Math.min(fovRight, screenRight);
        result[3] = Math.max(fovBottom, screenBottom);
        return result;
      };
      DeviceInfo.prototype.getLeftEyeVisibleScreenRect = function(undistortedFrustum) {
        var viewer = this.viewer;
        var device = this.device;
        var dist3 = viewer.screenLensDistance;
        var eyeX = (device.widthMeters - viewer.interLensDistance) / 2;
        var eyeY = viewer.baselineLensDistance - device.bevelMeters;
        var left2 = (undistortedFrustum[0] * dist3 + eyeX) / device.widthMeters;
        var top2 = (undistortedFrustum[1] * dist3 + eyeY) / device.heightMeters;
        var right = (undistortedFrustum[2] * dist3 + eyeX) / device.widthMeters;
        var bottom = (undistortedFrustum[3] * dist3 + eyeY) / device.heightMeters;
        return {
          x: left2,
          y: bottom,
          width: right - left2,
          height: top2 - bottom
        };
      };
      DeviceInfo.prototype.getFieldOfViewLeftEye = function(opt_isUndistorted) {
        return opt_isUndistorted ? this.getUndistortedFieldOfViewLeftEye() : this.getDistortedFieldOfViewLeftEye();
      };
      DeviceInfo.prototype.getFieldOfViewRightEye = function(opt_isUndistorted) {
        var fov = this.getFieldOfViewLeftEye(opt_isUndistorted);
        return {
          leftDegrees: fov.rightDegrees,
          rightDegrees: fov.leftDegrees,
          upDegrees: fov.upDegrees,
          downDegrees: fov.downDegrees
        };
      };
      DeviceInfo.prototype.getUndistortedFieldOfViewLeftEye = function() {
        var p = this.getUndistortedParams_();
        return {
          leftDegrees: radToDeg * Math.atan(p.outerDist),
          rightDegrees: radToDeg * Math.atan(p.innerDist),
          downDegrees: radToDeg * Math.atan(p.bottomDist),
          upDegrees: radToDeg * Math.atan(p.topDist)
        };
      };
      DeviceInfo.prototype.getUndistortedViewportLeftEye = function() {
        var p = this.getUndistortedParams_();
        var viewer = this.viewer;
        var device = this.device;
        var eyeToScreenDistance = viewer.screenLensDistance;
        var screenWidth = device.widthMeters / eyeToScreenDistance;
        var screenHeight = device.heightMeters / eyeToScreenDistance;
        var xPxPerTanAngle = device.width / screenWidth;
        var yPxPerTanAngle = device.height / screenHeight;
        var x = Math.round((p.eyePosX - p.outerDist) * xPxPerTanAngle);
        var y = Math.round((p.eyePosY - p.bottomDist) * yPxPerTanAngle);
        return {
          x,
          y,
          width: Math.round((p.eyePosX + p.innerDist) * xPxPerTanAngle) - x,
          height: Math.round((p.eyePosY + p.topDist) * yPxPerTanAngle) - y
        };
      };
      DeviceInfo.prototype.getUndistortedParams_ = function() {
        var viewer = this.viewer;
        var device = this.device;
        var distortion = this.distortion;
        var eyeToScreenDistance = viewer.screenLensDistance;
        var halfLensDistance = viewer.interLensDistance / 2 / eyeToScreenDistance;
        var screenWidth = device.widthMeters / eyeToScreenDistance;
        var screenHeight = device.heightMeters / eyeToScreenDistance;
        var eyePosX = screenWidth / 2 - halfLensDistance;
        var eyePosY = (viewer.baselineLensDistance - device.bevelMeters) / eyeToScreenDistance;
        var maxFov = viewer.fov;
        var viewerMax = distortion.distortInverse(Math.tan(degToRad * maxFov));
        var outerDist = Math.min(eyePosX, viewerMax);
        var innerDist = Math.min(halfLensDistance, viewerMax);
        var bottomDist = Math.min(eyePosY, viewerMax);
        var topDist = Math.min(screenHeight - eyePosY, viewerMax);
        return {
          outerDist,
          innerDist,
          topDist,
          bottomDist,
          eyePosX,
          eyePosY
        };
      };
      function CardboardViewer(params) {
        this.id = params.id;
        this.label = params.label;
        this.fov = params.fov;
        this.interLensDistance = params.interLensDistance;
        this.baselineLensDistance = params.baselineLensDistance;
        this.screenLensDistance = params.screenLensDistance;
        this.distortionCoefficients = params.distortionCoefficients;
        this.inverseCoefficients = params.inverseCoefficients;
      }
      DeviceInfo.Viewers = Viewers;
      var format = 1;
      var last_updated = "2019-11-09T17:36:14Z";
      var devices = [{ "type": "android", "rules": [{ "mdmh": "asus/*/Nexus 7/*" }, { "ua": "Nexus 7" }], "dpi": [320.8, 323], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "asus/*/ASUS_X00PD/*" }, { "ua": "ASUS_X00PD" }], "dpi": 245, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "asus/*/ASUS_X008D/*" }, { "ua": "ASUS_X008D" }], "dpi": 282, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "asus/*/ASUS_Z00AD/*" }, { "ua": "ASUS_Z00AD" }], "dpi": [403, 404.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Google/*/Pixel 2 XL/*" }, { "ua": "Pixel 2 XL" }], "dpi": 537.9, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Google/*/Pixel 3 XL/*" }, { "ua": "Pixel 3 XL" }], "dpi": [558.5, 553.8], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Google/*/Pixel XL/*" }, { "ua": "Pixel XL" }], "dpi": [537.9, 533], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Google/*/Pixel 3/*" }, { "ua": "Pixel 3" }], "dpi": 442.4, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Google/*/Pixel 2/*" }, { "ua": "Pixel 2" }], "dpi": 441, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "Google/*/Pixel/*" }, { "ua": "Pixel" }], "dpi": [432.6, 436.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "HTC/*/HTC6435LVW/*" }, { "ua": "HTC6435LVW" }], "dpi": [449.7, 443.3], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "HTC/*/HTC One XL/*" }, { "ua": "HTC One XL" }], "dpi": [315.3, 314.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "htc/*/Nexus 9/*" }, { "ua": "Nexus 9" }], "dpi": 289, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "HTC/*/HTC One M9/*" }, { "ua": "HTC One M9" }], "dpi": [442.5, 443.3], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "HTC/*/HTC One_M8/*" }, { "ua": "HTC One_M8" }], "dpi": [449.7, 447.4], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "HTC/*/HTC One/*" }, { "ua": "HTC One" }], "dpi": 472.8, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Huawei/*/Nexus 6P/*" }, { "ua": "Nexus 6P" }], "dpi": [515.1, 518], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Huawei/*/BLN-L24/*" }, { "ua": "HONORBLN-L24" }], "dpi": 480, "bw": 4, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "Huawei/*/BKL-L09/*" }, { "ua": "BKL-L09" }], "dpi": 403, "bw": 3.47, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "LENOVO/*/Lenovo PB2-690Y/*" }, { "ua": "Lenovo PB2-690Y" }], "dpi": [457.2, 454.713], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/Nexus 5X/*" }, { "ua": "Nexus 5X" }], "dpi": [422, 419.9], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/LGMS345/*" }, { "ua": "LGMS345" }], "dpi": [221.7, 219.1], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/LG-D800/*" }, { "ua": "LG-D800" }], "dpi": [422, 424.1], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/LG-D850/*" }, { "ua": "LG-D850" }], "dpi": [537.9, 541.9], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/VS985 4G/*" }, { "ua": "VS985 4G" }], "dpi": [537.9, 535.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/Nexus 5/*" }, { "ua": "Nexus 5 B" }], "dpi": [442.4, 444.8], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/Nexus 4/*" }, { "ua": "Nexus 4" }], "dpi": [319.8, 318.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/LG-P769/*" }, { "ua": "LG-P769" }], "dpi": [240.6, 247.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/LGMS323/*" }, { "ua": "LGMS323" }], "dpi": [206.6, 204.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "LGE/*/LGLS996/*" }, { "ua": "LGLS996" }], "dpi": [403.4, 401.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Micromax/*/4560MMX/*" }, { "ua": "4560MMX" }], "dpi": [240, 219.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Micromax/*/A250/*" }, { "ua": "Micromax A250" }], "dpi": [480, 446.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Micromax/*/Micromax AQ4501/*" }, { "ua": "Micromax AQ4501" }], "dpi": 240, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/G5/*" }, { "ua": "Moto G (5) Plus" }], "dpi": [403.4, 403], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/DROID RAZR/*" }, { "ua": "DROID RAZR" }], "dpi": [368.1, 256.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT830C/*" }, { "ua": "XT830C" }], "dpi": [254, 255.9], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1021/*" }, { "ua": "XT1021" }], "dpi": [254, 256.7], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1023/*" }, { "ua": "XT1023" }], "dpi": [254, 256.7], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1028/*" }, { "ua": "XT1028" }], "dpi": [326.6, 327.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1034/*" }, { "ua": "XT1034" }], "dpi": [326.6, 328.4], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1053/*" }, { "ua": "XT1053" }], "dpi": [315.3, 316.1], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1562/*" }, { "ua": "XT1562" }], "dpi": [403.4, 402.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/Nexus 6/*" }, { "ua": "Nexus 6 B" }], "dpi": [494.3, 489.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1063/*" }, { "ua": "XT1063" }], "dpi": [295, 296.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1064/*" }, { "ua": "XT1064" }], "dpi": [295, 295.6], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1092/*" }, { "ua": "XT1092" }], "dpi": [422, 424.1], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/XT1095/*" }, { "ua": "XT1095" }], "dpi": [422, 423.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "motorola/*/G4/*" }, { "ua": "Moto G (4)" }], "dpi": 401, "bw": 4, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/A0001/*" }, { "ua": "A0001" }], "dpi": [403.4, 401], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE E1001/*" }, { "ua": "ONE E1001" }], "dpi": [442.4, 441.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE E1003/*" }, { "ua": "ONE E1003" }], "dpi": [442.4, 441.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE E1005/*" }, { "ua": "ONE E1005" }], "dpi": [442.4, 441.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE A2001/*" }, { "ua": "ONE A2001" }], "dpi": [391.9, 405.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE A2003/*" }, { "ua": "ONE A2003" }], "dpi": [391.9, 405.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE A2005/*" }, { "ua": "ONE A2005" }], "dpi": [391.9, 405.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A3000/*" }, { "ua": "ONEPLUS A3000" }], "dpi": 401, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A3003/*" }, { "ua": "ONEPLUS A3003" }], "dpi": 401, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A3010/*" }, { "ua": "ONEPLUS A3010" }], "dpi": 401, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A5000/*" }, { "ua": "ONEPLUS A5000 " }], "dpi": [403.411, 399.737], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONE A5010/*" }, { "ua": "ONEPLUS A5010" }], "dpi": [403, 400], "bw": 2, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A6000/*" }, { "ua": "ONEPLUS A6000" }], "dpi": 401, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A6003/*" }, { "ua": "ONEPLUS A6003" }], "dpi": 401, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A6010/*" }, { "ua": "ONEPLUS A6010" }], "dpi": 401, "bw": 2, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OnePlus/*/ONEPLUS A6013/*" }, { "ua": "ONEPLUS A6013" }], "dpi": 401, "bw": 2, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "OPPO/*/X909/*" }, { "ua": "X909" }], "dpi": [442.4, 444.1], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9082/*" }, { "ua": "GT-I9082" }], "dpi": [184.7, 185.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G360P/*" }, { "ua": "SM-G360P" }], "dpi": [196.7, 205.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/Nexus S/*" }, { "ua": "Nexus S" }], "dpi": [234.5, 229.8], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9300/*" }, { "ua": "GT-I9300" }], "dpi": [304.8, 303.9], "bw": 5, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-T230NU/*" }, { "ua": "SM-T230NU" }], "dpi": 216, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SGH-T399/*" }, { "ua": "SGH-T399" }], "dpi": [217.7, 231.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SGH-M919/*" }, { "ua": "SGH-M919" }], "dpi": [440.8, 437.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-N9005/*" }, { "ua": "SM-N9005" }], "dpi": [386.4, 387], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SAMSUNG-SM-N900A/*" }, { "ua": "SAMSUNG-SM-N900A" }], "dpi": [386.4, 387.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9500/*" }, { "ua": "GT-I9500" }], "dpi": [442.5, 443.3], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9505/*" }, { "ua": "GT-I9505" }], "dpi": 439.4, "bw": 4, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G900F/*" }, { "ua": "SM-G900F" }], "dpi": [415.6, 431.6], "bw": 5, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G900M/*" }, { "ua": "SM-G900M" }], "dpi": [415.6, 431.6], "bw": 5, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G800F/*" }, { "ua": "SM-G800F" }], "dpi": 326.8, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G906S/*" }, { "ua": "SM-G906S" }], "dpi": [562.7, 572.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9300/*" }, { "ua": "GT-I9300" }], "dpi": [306.7, 304.8], "bw": 5, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-T535/*" }, { "ua": "SM-T535" }], "dpi": [142.6, 136.4], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-N920C/*" }, { "ua": "SM-N920C" }], "dpi": [515.1, 518.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-N920P/*" }, { "ua": "SM-N920P" }], "dpi": [386.3655, 390.144], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-N920W8/*" }, { "ua": "SM-N920W8" }], "dpi": [515.1, 518.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9300I/*" }, { "ua": "GT-I9300I" }], "dpi": [304.8, 305.8], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-I9195/*" }, { "ua": "GT-I9195" }], "dpi": [249.4, 256.7], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SPH-L520/*" }, { "ua": "SPH-L520" }], "dpi": [249.4, 255.9], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SAMSUNG-SGH-I717/*" }, { "ua": "SAMSUNG-SGH-I717" }], "dpi": 285.8, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SPH-D710/*" }, { "ua": "SPH-D710" }], "dpi": [217.7, 204.2], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/GT-N7100/*" }, { "ua": "GT-N7100" }], "dpi": 265.1, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SCH-I605/*" }, { "ua": "SCH-I605" }], "dpi": 265.1, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/Galaxy Nexus/*" }, { "ua": "Galaxy Nexus" }], "dpi": [315.3, 314.2], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-N910H/*" }, { "ua": "SM-N910H" }], "dpi": [515.1, 518], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-N910C/*" }, { "ua": "SM-N910C" }], "dpi": [515.2, 520.2], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G130M/*" }, { "ua": "SM-G130M" }], "dpi": [165.9, 164.8], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G928I/*" }, { "ua": "SM-G928I" }], "dpi": [515.1, 518.4], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G920F/*" }, { "ua": "SM-G920F" }], "dpi": 580.6, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G920P/*" }, { "ua": "SM-G920P" }], "dpi": [522.5, 577], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G925F/*" }, { "ua": "SM-G925F" }], "dpi": 580.6, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G925V/*" }, { "ua": "SM-G925V" }], "dpi": [522.5, 576.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G930F/*" }, { "ua": "SM-G930F" }], "dpi": 576.6, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G935F/*" }, { "ua": "SM-G935F" }], "dpi": 533, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G950F/*" }, { "ua": "SM-G950F" }], "dpi": [562.707, 565.293], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G955U/*" }, { "ua": "SM-G955U" }], "dpi": [522.514, 525.762], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G955F/*" }, { "ua": "SM-G955F" }], "dpi": [522.514, 525.762], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G960F/*" }, { "ua": "SM-G960F" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G9600/*" }, { "ua": "SM-G9600" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G960T/*" }, { "ua": "SM-G960T" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G960N/*" }, { "ua": "SM-G960N" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G960U/*" }, { "ua": "SM-G960U" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G9608/*" }, { "ua": "SM-G9608" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G960FD/*" }, { "ua": "SM-G960FD" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G960W/*" }, { "ua": "SM-G960W" }], "dpi": [569.575, 571.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G965F/*" }, { "ua": "SM-G965F" }], "dpi": 529, "bw": 2, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Sony/*/C6903/*" }, { "ua": "C6903" }], "dpi": [442.5, 443.3], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "Sony/*/D6653/*" }, { "ua": "D6653" }], "dpi": [428.6, 427.6], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Sony/*/E6653/*" }, { "ua": "E6653" }], "dpi": [428.6, 425.7], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Sony/*/E6853/*" }, { "ua": "E6853" }], "dpi": [403.4, 401.9], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Sony/*/SGP321/*" }, { "ua": "SGP321" }], "dpi": [224.7, 224.1], "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "TCT/*/ALCATEL ONE TOUCH Fierce/*" }, { "ua": "ALCATEL ONE TOUCH Fierce" }], "dpi": [240, 247.5], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "THL/*/thl 5000/*" }, { "ua": "thl 5000" }], "dpi": [480, 443.3], "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Fly/*/IQ4412/*" }, { "ua": "IQ4412" }], "dpi": 307.9, "bw": 3, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "ZTE/*/ZTE Blade L2/*" }, { "ua": "ZTE Blade L2" }], "dpi": 240, "bw": 3, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "BENEVE/*/VR518/*" }, { "ua": "VR518" }], "dpi": 480, "bw": 3, "ac": 500 }, { "type": "ios", "rules": [{ "res": [640, 960] }], "dpi": [325.1, 328.4], "bw": 4, "ac": 1e3 }, { "type": "ios", "rules": [{ "res": [640, 1136] }], "dpi": [317.1, 320.2], "bw": 3, "ac": 1e3 }, { "type": "ios", "rules": [{ "res": [750, 1334] }], "dpi": 326.4, "bw": 4, "ac": 1e3 }, { "type": "ios", "rules": [{ "res": [1242, 2208] }], "dpi": [453.6, 458.4], "bw": 4, "ac": 1e3 }, { "type": "ios", "rules": [{ "res": [1125, 2001] }], "dpi": [410.9, 415.4], "bw": 4, "ac": 1e3 }, { "type": "ios", "rules": [{ "res": [1125, 2436] }], "dpi": 458, "bw": 4, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "Huawei/*/EML-L29/*" }, { "ua": "EML-L29" }], "dpi": 428, "bw": 3.45, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "Nokia/*/Nokia 7.1/*" }, { "ua": "Nokia 7.1" }], "dpi": [432, 431.9], "bw": 3, "ac": 500 }, { "type": "ios", "rules": [{ "res": [1242, 2688] }], "dpi": 458, "bw": 4, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G570M/*" }, { "ua": "SM-G570M" }], "dpi": 320, "bw": 3.684, "ac": 1e3 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G970F/*" }, { "ua": "SM-G970F" }], "dpi": 438, "bw": 2.281, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G973F/*" }, { "ua": "SM-G973F" }], "dpi": 550, "bw": 2.002, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G975F/*" }, { "ua": "SM-G975F" }], "dpi": 522, "bw": 2.054, "ac": 500 }, { "type": "android", "rules": [{ "mdmh": "samsung/*/SM-G977F/*" }, { "ua": "SM-G977F" }], "dpi": 505, "bw": 2.334, "ac": 500 }, { "type": "ios", "rules": [{ "res": [828, 1792] }], "dpi": 326, "bw": 5, "ac": 500 }];
      var DPDB_CACHE = {
        format,
        last_updated,
        devices
      };
      function Dpdb(url, onDeviceParamsUpdated) {
        this.dpdb = DPDB_CACHE;
        this.recalculateDeviceParams_();
        if (url) {
          this.onDeviceParamsUpdated = onDeviceParamsUpdated;
          var xhr = new XMLHttpRequest();
          var obj2 = this;
          xhr.open("GET", url, true);
          xhr.addEventListener("load", function() {
            obj2.loading = false;
            if (xhr.status >= 200 && xhr.status <= 299) {
              obj2.dpdb = JSON.parse(xhr.response);
              obj2.recalculateDeviceParams_();
            } else {
              console.error("Error loading online DPDB!");
            }
          });
          xhr.send();
        }
      }
      Dpdb.prototype.getDeviceParams = function() {
        return this.deviceParams;
      };
      Dpdb.prototype.recalculateDeviceParams_ = function() {
        var newDeviceParams = this.calcDeviceParams_();
        if (newDeviceParams) {
          this.deviceParams = newDeviceParams;
          if (this.onDeviceParamsUpdated) {
            this.onDeviceParamsUpdated(this.deviceParams);
          }
        } else {
          console.error("Failed to recalculate device parameters.");
        }
      };
      Dpdb.prototype.calcDeviceParams_ = function() {
        var db = this.dpdb;
        if (!db) {
          console.error("DPDB not available.");
          return null;
        }
        if (db.format != 1) {
          console.error("DPDB has unexpected format version.");
          return null;
        }
        if (!db.devices || !db.devices.length) {
          console.error("DPDB does not have a devices section.");
          return null;
        }
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        var width2 = getScreenWidth();
        var height2 = getScreenHeight();
        if (!db.devices) {
          console.error("DPDB has no devices section.");
          return null;
        }
        for (var i = 0; i < db.devices.length; i++) {
          var device = db.devices[i];
          if (!device.rules) {
            console.warn("Device[" + i + "] has no rules section.");
            continue;
          }
          if (device.type != "ios" && device.type != "android") {
            console.warn("Device[" + i + "] has invalid type.");
            continue;
          }
          if (isIOS2() != (device.type == "ios"))
            continue;
          var matched = false;
          for (var j = 0; j < device.rules.length; j++) {
            var rule2 = device.rules[j];
            if (this.ruleMatches_(rule2, userAgent, width2, height2)) {
              matched = true;
              break;
            }
          }
          if (!matched)
            continue;
          var xdpi = device.dpi[0] || device.dpi;
          var ydpi = device.dpi[1] || device.dpi;
          return new DeviceParams({ xdpi, ydpi, bevelMm: device.bw });
        }
        console.warn("No DPDB device match.");
        return null;
      };
      Dpdb.prototype.ruleMatches_ = function(rule2, ua, screenWidth, screenHeight) {
        if (!rule2.ua && !rule2.res)
          return false;
        if (rule2.ua && rule2.ua.substring(0, 2) === "SM")
          rule2.ua = rule2.ua.substring(0, 7);
        if (rule2.ua && ua.indexOf(rule2.ua) < 0)
          return false;
        if (rule2.res) {
          if (!rule2.res[0] || !rule2.res[1])
            return false;
          var resX = rule2.res[0];
          var resY = rule2.res[1];
          if (Math.min(screenWidth, screenHeight) != Math.min(resX, resY) || Math.max(screenWidth, screenHeight) != Math.max(resX, resY)) {
            return false;
          }
        }
        return true;
      };
      function DeviceParams(params) {
        this.xdpi = params.xdpi;
        this.ydpi = params.ydpi;
        this.bevelMm = params.bevelMm;
      }
      function SensorSample(sample, timestampS) {
        this.set(sample, timestampS);
      }
      SensorSample.prototype.set = function(sample, timestampS) {
        this.sample = sample;
        this.timestampS = timestampS;
      };
      SensorSample.prototype.copy = function(sensorSample) {
        this.set(sensorSample.sample, sensorSample.timestampS);
      };
      function ComplementaryFilter(kFilter, isDebug) {
        this.kFilter = kFilter;
        this.isDebug = isDebug;
        this.currentAccelMeasurement = new SensorSample();
        this.currentGyroMeasurement = new SensorSample();
        this.previousGyroMeasurement = new SensorSample();
        if (isIOS2()) {
          this.filterQ = new Quaternion(-1, 0, 0, 1);
        } else {
          this.filterQ = new Quaternion(1, 0, 0, 1);
        }
        this.previousFilterQ = new Quaternion();
        this.previousFilterQ.copy(this.filterQ);
        this.accelQ = new Quaternion();
        this.isOrientationInitialized = false;
        this.estimatedGravity = new Vector3();
        this.measuredGravity = new Vector3();
        this.gyroIntegralQ = new Quaternion();
      }
      ComplementaryFilter.prototype.addAccelMeasurement = function(vector, timestampS) {
        this.currentAccelMeasurement.set(vector, timestampS);
      };
      ComplementaryFilter.prototype.addGyroMeasurement = function(vector, timestampS) {
        this.currentGyroMeasurement.set(vector, timestampS);
        var deltaT = timestampS - this.previousGyroMeasurement.timestampS;
        if (isTimestampDeltaValid(deltaT)) {
          this.run_();
        }
        this.previousGyroMeasurement.copy(this.currentGyroMeasurement);
      };
      ComplementaryFilter.prototype.run_ = function() {
        if (!this.isOrientationInitialized) {
          this.accelQ = this.accelToQuaternion_(this.currentAccelMeasurement.sample);
          this.previousFilterQ.copy(this.accelQ);
          this.isOrientationInitialized = true;
          return;
        }
        var deltaT = this.currentGyroMeasurement.timestampS - this.previousGyroMeasurement.timestampS;
        var gyroDeltaQ = this.gyroToQuaternionDelta_(this.currentGyroMeasurement.sample, deltaT);
        this.gyroIntegralQ.multiply(gyroDeltaQ);
        this.filterQ.copy(this.previousFilterQ);
        this.filterQ.multiply(gyroDeltaQ);
        var invFilterQ = new Quaternion();
        invFilterQ.copy(this.filterQ);
        invFilterQ.inverse();
        this.estimatedGravity.set(0, 0, -1);
        this.estimatedGravity.applyQuaternion(invFilterQ);
        this.estimatedGravity.normalize();
        this.measuredGravity.copy(this.currentAccelMeasurement.sample);
        this.measuredGravity.normalize();
        var deltaQ = new Quaternion();
        deltaQ.setFromUnitVectors(this.estimatedGravity, this.measuredGravity);
        deltaQ.inverse();
        if (this.isDebug) {
          console.log("Delta: %d deg, G_est: (%s, %s, %s), G_meas: (%s, %s, %s)", radToDeg * getQuaternionAngle(deltaQ), this.estimatedGravity.x.toFixed(1), this.estimatedGravity.y.toFixed(1), this.estimatedGravity.z.toFixed(1), this.measuredGravity.x.toFixed(1), this.measuredGravity.y.toFixed(1), this.measuredGravity.z.toFixed(1));
        }
        var targetQ = new Quaternion();
        targetQ.copy(this.filterQ);
        targetQ.multiply(deltaQ);
        this.filterQ.slerp(targetQ, 1 - this.kFilter);
        this.previousFilterQ.copy(this.filterQ);
      };
      ComplementaryFilter.prototype.getOrientation = function() {
        return this.filterQ;
      };
      ComplementaryFilter.prototype.accelToQuaternion_ = function(accel) {
        var normAccel = new Vector3();
        normAccel.copy(accel);
        normAccel.normalize();
        var quat = new Quaternion();
        quat.setFromUnitVectors(new Vector3(0, 0, -1), normAccel);
        quat.inverse();
        return quat;
      };
      ComplementaryFilter.prototype.gyroToQuaternionDelta_ = function(gyro, dt) {
        var quat = new Quaternion();
        var axis = new Vector3();
        axis.copy(gyro);
        axis.normalize();
        quat.setFromAxisAngle(axis, gyro.length() * dt);
        return quat;
      };
      function PosePredictor(predictionTimeS, isDebug) {
        this.predictionTimeS = predictionTimeS;
        this.isDebug = isDebug;
        this.previousQ = new Quaternion();
        this.previousTimestampS = null;
        this.deltaQ = new Quaternion();
        this.outQ = new Quaternion();
      }
      PosePredictor.prototype.getPrediction = function(currentQ, gyro, timestampS) {
        if (!this.previousTimestampS) {
          this.previousQ.copy(currentQ);
          this.previousTimestampS = timestampS;
          return currentQ;
        }
        var axis = new Vector3();
        axis.copy(gyro);
        axis.normalize();
        var angularSpeed = gyro.length();
        if (angularSpeed < degToRad * 20) {
          if (this.isDebug) {
            console.log("Moving slowly, at %s deg/s: no prediction", (radToDeg * angularSpeed).toFixed(1));
          }
          this.outQ.copy(currentQ);
          this.previousQ.copy(currentQ);
          return this.outQ;
        }
        var predictAngle = angularSpeed * this.predictionTimeS;
        this.deltaQ.setFromAxisAngle(axis, predictAngle);
        this.outQ.copy(this.previousQ);
        this.outQ.multiply(this.deltaQ);
        this.previousQ.copy(currentQ);
        this.previousTimestampS = timestampS;
        return this.outQ;
      };
      function FusionPoseSensor(kFilter, predictionTime, yawOnly, isDebug) {
        this.yawOnly = yawOnly;
        this.accelerometer = new Vector3();
        this.gyroscope = new Vector3();
        this.filter = new ComplementaryFilter(kFilter, isDebug);
        this.posePredictor = new PosePredictor(predictionTime, isDebug);
        this.isFirefoxAndroid = isFirefoxAndroid();
        this.isIOS = isIOS2();
        var chromeVersion = getChromeVersion();
        this.isDeviceMotionInRadians = !this.isIOS && chromeVersion && chromeVersion < 66;
        this.isWithoutDeviceMotion = isChromeWithoutDeviceMotion() || isSafariWithoutDeviceMotion();
        this.filterToWorldQ = new Quaternion();
        if (isIOS2()) {
          this.filterToWorldQ.setFromAxisAngle(new Vector3(1, 0, 0), Math.PI / 2);
        } else {
          this.filterToWorldQ.setFromAxisAngle(new Vector3(1, 0, 0), -Math.PI / 2);
        }
        this.inverseWorldToScreenQ = new Quaternion();
        this.worldToScreenQ = new Quaternion();
        this.originalPoseAdjustQ = new Quaternion();
        this.originalPoseAdjustQ.setFromAxisAngle(new Vector3(0, 0, 1), -window.orientation * Math.PI / 180);
        this.setScreenTransform_();
        if (isLandscapeMode()) {
          this.filterToWorldQ.multiply(this.inverseWorldToScreenQ);
        }
        this.resetQ = new Quaternion();
        this.orientationOut_ = new Float32Array(4);
        this.start();
      }
      FusionPoseSensor.prototype.getPosition = function() {
        return null;
      };
      FusionPoseSensor.prototype.getOrientation = function() {
        var orientation = void 0;
        if (this.isWithoutDeviceMotion && this._deviceOrientationQ) {
          this.deviceOrientationFixQ = this.deviceOrientationFixQ || function() {
            var z = new Quaternion().setFromAxisAngle(new Vector3(0, 0, -1), 0);
            var y = new Quaternion();
            if (window.orientation === -90) {
              y.setFromAxisAngle(new Vector3(0, 1, 0), Math.PI / -2);
            } else {
              y.setFromAxisAngle(new Vector3(0, 1, 0), Math.PI / 2);
            }
            return z.multiply(y);
          }();
          this.deviceOrientationFilterToWorldQ = this.deviceOrientationFilterToWorldQ || function() {
            var q = new Quaternion();
            q.setFromAxisAngle(new Vector3(1, 0, 0), -Math.PI / 2);
            return q;
          }();
          orientation = this._deviceOrientationQ;
          var out = new Quaternion();
          out.copy(orientation);
          out.multiply(this.deviceOrientationFilterToWorldQ);
          out.multiply(this.resetQ);
          out.multiply(this.worldToScreenQ);
          out.multiplyQuaternions(this.deviceOrientationFixQ, out);
          if (this.yawOnly) {
            out.x = 0;
            out.z = 0;
            out.normalize();
          }
          this.orientationOut_[0] = out.x;
          this.orientationOut_[1] = out.y;
          this.orientationOut_[2] = out.z;
          this.orientationOut_[3] = out.w;
          return this.orientationOut_;
        } else {
          var filterOrientation = this.filter.getOrientation();
          orientation = this.posePredictor.getPrediction(filterOrientation, this.gyroscope, this.previousTimestampS);
        }
        var out = new Quaternion();
        out.copy(this.filterToWorldQ);
        out.multiply(this.resetQ);
        out.multiply(orientation);
        out.multiply(this.worldToScreenQ);
        if (this.yawOnly) {
          out.x = 0;
          out.z = 0;
          out.normalize();
        }
        this.orientationOut_[0] = out.x;
        this.orientationOut_[1] = out.y;
        this.orientationOut_[2] = out.z;
        this.orientationOut_[3] = out.w;
        return this.orientationOut_;
      };
      FusionPoseSensor.prototype.resetPose = function() {
        this.resetQ.copy(this.filter.getOrientation());
        this.resetQ.x = 0;
        this.resetQ.y = 0;
        this.resetQ.z *= -1;
        this.resetQ.normalize();
        if (isLandscapeMode()) {
          this.resetQ.multiply(this.inverseWorldToScreenQ);
        }
        this.resetQ.multiply(this.originalPoseAdjustQ);
      };
      FusionPoseSensor.prototype.onDeviceOrientation_ = function(e2) {
        this._deviceOrientationQ = this._deviceOrientationQ || new Quaternion();
        var alpha = e2.alpha, beta2 = e2.beta, gamma = e2.gamma;
        alpha = (alpha || 0) * Math.PI / 180;
        beta2 = (beta2 || 0) * Math.PI / 180;
        gamma = (gamma || 0) * Math.PI / 180;
        this._deviceOrientationQ.setFromEulerYXZ(beta2, alpha, -gamma);
      };
      FusionPoseSensor.prototype.onDeviceMotion_ = function(deviceMotion) {
        this.updateDeviceMotion_(deviceMotion);
      };
      FusionPoseSensor.prototype.updateDeviceMotion_ = function(deviceMotion) {
        var accGravity = deviceMotion.accelerationIncludingGravity;
        var rotRate = deviceMotion.rotationRate;
        var timestampS = deviceMotion.timeStamp / 1e3;
        var deltaS = timestampS - this.previousTimestampS;
        if (deltaS < 0) {
          warnOnce("fusion-pose-sensor:invalid:non-monotonic", "Invalid timestamps detected: non-monotonic timestamp from devicemotion");
          this.previousTimestampS = timestampS;
          return;
        } else if (deltaS <= MIN_TIMESTEP || deltaS > MAX_TIMESTEP) {
          warnOnce("fusion-pose-sensor:invalid:outside-threshold", "Invalid timestamps detected: Timestamp from devicemotion outside expected range.");
          this.previousTimestampS = timestampS;
          return;
        }
        this.accelerometer.set(-accGravity.x, -accGravity.y, -accGravity.z);
        if (rotRate) {
          if (isR7()) {
            this.gyroscope.set(-rotRate.beta, rotRate.alpha, rotRate.gamma);
          } else {
            this.gyroscope.set(rotRate.alpha, rotRate.beta, rotRate.gamma);
          }
          if (!this.isDeviceMotionInRadians) {
            this.gyroscope.multiplyScalar(Math.PI / 180);
          }
          this.filter.addGyroMeasurement(this.gyroscope, timestampS);
        }
        this.filter.addAccelMeasurement(this.accelerometer, timestampS);
        this.previousTimestampS = timestampS;
      };
      FusionPoseSensor.prototype.onOrientationChange_ = function(screenOrientation) {
        this.setScreenTransform_();
      };
      FusionPoseSensor.prototype.onMessage_ = function(event) {
        var message = event.data;
        if (!message || !message.type) {
          return;
        }
        var type2 = message.type.toLowerCase();
        if (type2 !== "devicemotion") {
          return;
        }
        this.updateDeviceMotion_(message.deviceMotionEvent);
      };
      FusionPoseSensor.prototype.setScreenTransform_ = function() {
        this.worldToScreenQ.set(0, 0, 0, 1);
        switch (window.orientation) {
          case 0:
            break;
          case 90:
            this.worldToScreenQ.setFromAxisAngle(new Vector3(0, 0, 1), -Math.PI / 2);
            break;
          case -90:
            this.worldToScreenQ.setFromAxisAngle(new Vector3(0, 0, 1), Math.PI / 2);
            break;
          case 180:
            break;
        }
        this.inverseWorldToScreenQ.copy(this.worldToScreenQ);
        this.inverseWorldToScreenQ.inverse();
      };
      FusionPoseSensor.prototype.start = function() {
        this.onDeviceMotionCallback_ = this.onDeviceMotion_.bind(this);
        this.onOrientationChangeCallback_ = this.onOrientationChange_.bind(this);
        this.onMessageCallback_ = this.onMessage_.bind(this);
        this.onDeviceOrientationCallback_ = this.onDeviceOrientation_.bind(this);
        if (isIOS2() && isInsideCrossOriginIFrame()) {
          window.addEventListener("message", this.onMessageCallback_);
        }
        window.addEventListener("orientationchange", this.onOrientationChangeCallback_);
        if (this.isWithoutDeviceMotion) {
          window.addEventListener("deviceorientation", this.onDeviceOrientationCallback_);
        } else {
          window.addEventListener("devicemotion", this.onDeviceMotionCallback_);
        }
      };
      FusionPoseSensor.prototype.stop = function() {
        window.removeEventListener("devicemotion", this.onDeviceMotionCallback_);
        window.removeEventListener("deviceorientation", this.onDeviceOrientationCallback_);
        window.removeEventListener("orientationchange", this.onOrientationChangeCallback_);
        window.removeEventListener("message", this.onMessageCallback_);
      };
      var SENSOR_FREQUENCY = 60;
      var X_AXIS = new Vector3(1, 0, 0);
      var Z_AXIS = new Vector3(0, 0, 1);
      var SENSOR_TO_VR = new Quaternion();
      SENSOR_TO_VR.setFromAxisAngle(X_AXIS, -Math.PI / 2);
      SENSOR_TO_VR.multiply(new Quaternion().setFromAxisAngle(Z_AXIS, Math.PI / 2));
      var PoseSensor = function() {
        function PoseSensor2(config2) {
          classCallCheck(this, PoseSensor2);
          this.config = config2;
          this.sensor = null;
          this.fusionSensor = null;
          this._out = new Float32Array(4);
          this.api = null;
          this.errors = [];
          this._sensorQ = new Quaternion();
          this._outQ = new Quaternion();
          this._onSensorRead = this._onSensorRead.bind(this);
          this._onSensorError = this._onSensorError.bind(this);
          this.init();
        }
        createClass(PoseSensor2, [{
          key: "init",
          value: function init() {
            var sensor = null;
            try {
              sensor = new RelativeOrientationSensor({
                frequency: SENSOR_FREQUENCY,
                referenceFrame: "screen"
              });
              sensor.addEventListener("error", this._onSensorError);
            } catch (error) {
              this.errors.push(error);
              if (error.name === "SecurityError") {
                console.error("Cannot construct sensors due to the Feature Policy");
                console.warn('Attempting to fall back using "devicemotion"; however this will fail in the future without correct permissions.');
                this.useDeviceMotion();
              } else if (error.name === "ReferenceError") {
                this.useDeviceMotion();
              } else {
                console.error(error);
              }
            }
            if (sensor) {
              this.api = "sensor";
              this.sensor = sensor;
              this.sensor.addEventListener("reading", this._onSensorRead);
              this.sensor.start();
            }
          }
        }, {
          key: "useDeviceMotion",
          value: function useDeviceMotion() {
            this.api = "devicemotion";
            this.fusionSensor = new FusionPoseSensor(this.config.K_FILTER, this.config.PREDICTION_TIME_S, this.config.YAW_ONLY, this.config.DEBUG);
            if (this.sensor) {
              this.sensor.removeEventListener("reading", this._onSensorRead);
              this.sensor.removeEventListener("error", this._onSensorError);
              this.sensor = null;
            }
          }
        }, {
          key: "getOrientation",
          value: function getOrientation() {
            if (this.fusionSensor) {
              return this.fusionSensor.getOrientation();
            }
            if (!this.sensor || !this.sensor.quaternion) {
              this._out[0] = this._out[1] = this._out[2] = 0;
              this._out[3] = 1;
              return this._out;
            }
            var q = this.sensor.quaternion;
            this._sensorQ.set(q[0], q[1], q[2], q[3]);
            var out = this._outQ;
            out.copy(SENSOR_TO_VR);
            out.multiply(this._sensorQ);
            if (this.config.YAW_ONLY) {
              out.x = out.z = 0;
              out.normalize();
            }
            this._out[0] = out.x;
            this._out[1] = out.y;
            this._out[2] = out.z;
            this._out[3] = out.w;
            return this._out;
          }
        }, {
          key: "_onSensorError",
          value: function _onSensorError(event) {
            this.errors.push(event.error);
            if (event.error.name === "NotAllowedError") {
              console.error("Permission to access sensor was denied");
            } else if (event.error.name === "NotReadableError") {
              console.error("Sensor could not be read");
            } else {
              console.error(event.error);
            }
            this.useDeviceMotion();
          }
        }, {
          key: "_onSensorRead",
          value: function _onSensorRead() {
          }
        }]);
        return PoseSensor2;
      }();
      var rotateInstructionsAsset = "<svg width='198' height='240' viewBox='0 0 198 240' xmlns='http://www.w3.org/2000/svg'><g fill='none' fill-rule='evenodd'><path d='M149.625 109.527l6.737 3.891v.886c0 .177.013.36.038.549.01.081.02.162.027.242.14 1.415.974 2.998 2.105 3.999l5.72 5.062.081-.09s4.382-2.53 5.235-3.024l25.97 14.993v54.001c0 .771-.386 1.217-.948 1.217-.233 0-.495-.076-.772-.236l-23.967-13.838-.014.024-27.322 15.775-.85-1.323c-4.731-1.529-9.748-2.74-14.951-3.61a.27.27 0 0 0-.007.024l-5.067 16.961-7.891 4.556-.037-.063v27.59c0 .772-.386 1.217-.948 1.217-.232 0-.495-.076-.772-.236l-42.473-24.522c-.95-.549-1.72-1.877-1.72-2.967v-1.035l-.021.047a5.111 5.111 0 0 0-1.816-.399 5.682 5.682 0 0 0-.546.001 13.724 13.724 0 0 1-1.918-.041c-1.655-.153-3.2-.6-4.404-1.296l-46.576-26.89.005.012-10.278-18.75c-1.001-1.827-.241-4.216 1.698-5.336l56.011-32.345a4.194 4.194 0 0 1 2.099-.572c1.326 0 2.572.659 3.227 1.853l.005-.003.227.413-.006.004a9.63 9.63 0 0 0 1.477 2.018l.277.27c1.914 1.85 4.468 2.801 7.113 2.801 1.949 0 3.948-.517 5.775-1.572.013 0 7.319-4.219 7.319-4.219a4.194 4.194 0 0 1 2.099-.572c1.326 0 2.572.658 3.226 1.853l3.25 5.928.022-.018 6.785 3.917-.105-.182 46.881-26.965m0-1.635c-.282 0-.563.073-.815.218l-46.169 26.556-5.41-3.124-3.005-5.481c-.913-1.667-2.699-2.702-4.66-2.703-1.011 0-2.02.274-2.917.792a3825 3825 0 0 1-7.275 4.195l-.044.024a9.937 9.937 0 0 1-4.957 1.353c-2.292 0-4.414-.832-5.976-2.342l-.252-.245a7.992 7.992 0 0 1-1.139-1.534 1.379 1.379 0 0 0-.06-.122l-.227-.414a1.718 1.718 0 0 0-.095-.154c-.938-1.574-2.673-2.545-4.571-2.545-1.011 0-2.02.274-2.917.792L3.125 155.502c-2.699 1.559-3.738 4.94-2.314 7.538l10.278 18.75c.177.323.448.563.761.704l46.426 26.804c1.403.81 3.157 1.332 5.072 1.508a15.661 15.661 0 0 0 2.146.046 4.766 4.766 0 0 1 .396 0c.096.004.19.011.283.022.109 1.593 1.159 3.323 2.529 4.114l42.472 24.522c.524.302 1.058.455 1.59.455 1.497 0 2.583-1.2 2.583-2.852v-26.562l7.111-4.105a1.64 1.64 0 0 0 .749-.948l4.658-15.593c4.414.797 8.692 1.848 12.742 3.128l.533.829a1.634 1.634 0 0 0 2.193.531l26.532-15.317L193 192.433c.523.302 1.058.455 1.59.455 1.497 0 2.583-1.199 2.583-2.852v-54.001c0-.584-.312-1.124-.818-1.416l-25.97-14.993a1.633 1.633 0 0 0-1.636.001c-.606.351-2.993 1.73-4.325 2.498l-4.809-4.255c-.819-.725-1.461-1.933-1.561-2.936a7.776 7.776 0 0 0-.033-.294 2.487 2.487 0 0 1-.023-.336v-.886c0-.584-.312-1.123-.817-1.416l-6.739-3.891a1.633 1.633 0 0 0-.817-.219' fill='#455A64'/><path d='M96.027 132.636l46.576 26.891c1.204.695 1.979 1.587 2.242 2.541l-.01.007-81.374 46.982h-.001c-1.654-.152-3.199-.6-4.403-1.295l-46.576-26.891 83.546-48.235' fill='#FAFAFA'/><path d='M63.461 209.174c-.008 0-.015 0-.022-.002-1.693-.156-3.228-.609-4.441-1.309l-46.576-26.89a.118.118 0 0 1 0-.203l83.546-48.235a.117.117 0 0 1 .117 0l46.576 26.891c1.227.708 2.021 1.612 2.296 2.611a.116.116 0 0 1-.042.124l-.021.016-81.375 46.981a.11.11 0 0 1-.058.016zm-50.747-28.303l46.401 26.79c1.178.68 2.671 1.121 4.32 1.276l81.272-46.922c-.279-.907-1.025-1.73-2.163-2.387l-46.517-26.857-83.313 48.1z' fill='#607D8B'/><path d='M148.327 165.471a5.85 5.85 0 0 1-.546.001c-1.894-.083-3.302-1.038-3.145-2.132a2.693 2.693 0 0 0-.072-1.105l-81.103 46.822c.628.058 1.272.073 1.918.042.182-.009.364-.009.546-.001 1.894.083 3.302 1.038 3.145 2.132l79.257-45.759' fill='#FFF'/><path d='M69.07 211.347a.118.118 0 0 1-.115-.134c.045-.317-.057-.637-.297-.925-.505-.61-1.555-1.022-2.738-1.074a5.966 5.966 0 0 0-.535.001 14.03 14.03 0 0 1-1.935-.041.117.117 0 0 1-.103-.092.116.116 0 0 1 .055-.126l81.104-46.822a.117.117 0 0 1 .171.07c.104.381.129.768.074 1.153-.045.316.057.637.296.925.506.61 1.555 1.021 2.739 1.073.178.008.357.008.535-.001a.117.117 0 0 1 .064.218l-79.256 45.759a.114.114 0 0 1-.059.016zm-3.405-2.372c.089 0 .177.002.265.006 1.266.056 2.353.488 2.908 1.158.227.274.35.575.36.882l78.685-45.429c-.036 0-.072-.001-.107-.003-1.267-.056-2.354-.489-2.909-1.158-.282-.34-.402-.724-.347-1.107a2.604 2.604 0 0 0-.032-.91L63.846 208.97a13.91 13.91 0 0 0 1.528.012c.097-.005.194-.007.291-.007z' fill='#607D8B'/><path d='M2.208 162.134c-1.001-1.827-.241-4.217 1.698-5.337l56.011-32.344c1.939-1.12 4.324-.546 5.326 1.281l.232.41a9.344 9.344 0 0 0 1.47 2.021l.278.27c3.325 3.214 8.583 3.716 12.888 1.23l7.319-4.22c1.94-1.119 4.324-.546 5.325 1.282l3.25 5.928-83.519 48.229-10.278-18.75z' fill='#FAFAFA'/><path d='M12.486 181.001a.112.112 0 0 1-.031-.005.114.114 0 0 1-.071-.056L2.106 162.19c-1.031-1.88-.249-4.345 1.742-5.494l56.01-32.344a4.328 4.328 0 0 1 2.158-.588c1.415 0 2.65.702 3.311 1.882.01.008.018.017.024.028l.227.414a.122.122 0 0 1 .013.038 9.508 9.508 0 0 0 1.439 1.959l.275.266c1.846 1.786 4.344 2.769 7.031 2.769 1.977 0 3.954-.538 5.717-1.557a.148.148 0 0 1 .035-.013l7.284-4.206a4.321 4.321 0 0 1 2.157-.588c1.427 0 2.672.716 3.329 1.914l3.249 5.929a.116.116 0 0 1-.044.157l-83.518 48.229a.116.116 0 0 1-.059.016zm49.53-57.004c-.704 0-1.41.193-2.041.557l-56.01 32.345c-1.882 1.086-2.624 3.409-1.655 5.179l10.221 18.645 83.317-48.112-3.195-5.829c-.615-1.122-1.783-1.792-3.124-1.792a4.08 4.08 0 0 0-2.04.557l-7.317 4.225a.148.148 0 0 1-.035.013 11.7 11.7 0 0 1-5.801 1.569c-2.748 0-5.303-1.007-7.194-2.835l-.278-.27a9.716 9.716 0 0 1-1.497-2.046.096.096 0 0 1-.013-.037l-.191-.347a.11.11 0 0 1-.023-.029c-.615-1.123-1.783-1.793-3.124-1.793z' fill='#607D8B'/><path d='M42.434 155.808c-2.51-.001-4.697-1.258-5.852-3.365-1.811-3.304-.438-7.634 3.059-9.654l12.291-7.098a7.599 7.599 0 0 1 3.789-1.033c2.51 0 4.697 1.258 5.852 3.365 1.811 3.304.439 7.634-3.059 9.654l-12.291 7.098a7.606 7.606 0 0 1-3.789 1.033zm13.287-20.683a7.128 7.128 0 0 0-3.555.971l-12.291 7.098c-3.279 1.893-4.573 5.942-2.883 9.024 1.071 1.955 3.106 3.122 5.442 3.122a7.13 7.13 0 0 0 3.556-.97l12.291-7.098c3.279-1.893 4.572-5.942 2.883-9.024-1.072-1.955-3.106-3.123-5.443-3.123z' fill='#607D8B'/><path d='M149.588 109.407l6.737 3.89v.887c0 .176.013.36.037.549.011.081.02.161.028.242.14 1.415.973 2.998 2.105 3.999l7.396 6.545c.177.156.358.295.541.415 1.579 1.04 2.95.466 3.062-1.282.049-.784.057-1.595.023-2.429l-.003-.16v-1.151l25.987 15.003v54c0 1.09-.77 1.53-1.72.982l-42.473-24.523c-.95-.548-1.72-1.877-1.72-2.966v-34.033' fill='#FAFAFA'/><path d='M194.553 191.25c-.257 0-.54-.085-.831-.253l-42.472-24.521c-.981-.567-1.779-1.943-1.779-3.068v-34.033h.234v34.033c0 1.051.745 2.336 1.661 2.866l42.473 24.521c.424.245.816.288 1.103.122.285-.164.442-.52.442-1.002v-53.933l-25.753-14.868.003 1.106c.034.832.026 1.654-.024 2.439-.054.844-.396 1.464-.963 1.746-.619.309-1.45.173-2.28-.373a5.023 5.023 0 0 1-.553-.426l-7.397-6.544c-1.158-1.026-1.999-2.625-2.143-4.076a9.624 9.624 0 0 0-.027-.238 4.241 4.241 0 0 1-.038-.564v-.82l-6.68-3.856.117-.202 6.738 3.89.058.034v.954c0 .171.012.351.036.533.011.083.021.165.029.246.138 1.395.948 2.935 2.065 3.923l7.397 6.545c.173.153.35.289.527.406.758.499 1.504.63 2.047.359.49-.243.786-.795.834-1.551.05-.778.057-1.591.024-2.417l-.004-.163v-1.355l.175.1 25.987 15.004.059.033v54.068c0 .569-.198.996-.559 1.204a1.002 1.002 0 0 1-.506.131' fill='#607D8B'/><path d='M145.685 163.161l24.115 13.922-25.978 14.998-1.462-.307c-6.534-2.17-13.628-3.728-21.019-4.616-4.365-.524-8.663 1.096-9.598 3.62a2.746 2.746 0 0 0-.011 1.928c1.538 4.267 4.236 8.363 7.995 12.135l.532.845-25.977 14.997-24.115-13.922 75.518-43.6' fill='#FFF'/><path d='M94.282 220.818l-.059-.033-24.29-14.024.175-.101 75.577-43.634.058.033 24.29 14.024-26.191 15.122-.045-.01-1.461-.307c-6.549-2.174-13.613-3.725-21.009-4.614a13.744 13.744 0 0 0-1.638-.097c-3.758 0-7.054 1.531-7.837 3.642a2.62 2.62 0 0 0-.01 1.848c1.535 4.258 4.216 8.326 7.968 12.091l.016.021.526.835.006.01.064.102-.105.061-25.977 14.998-.058.033zm-23.881-14.057l23.881 13.788 24.802-14.32c.546-.315.846-.489 1.017-.575l-.466-.74c-3.771-3.787-6.467-7.881-8.013-12.168a2.851 2.851 0 0 1 .011-2.008c.815-2.199 4.203-3.795 8.056-3.795.557 0 1.117.033 1.666.099 7.412.891 14.491 2.445 21.041 4.621.836.175 1.215.254 1.39.304l25.78-14.884-23.881-13.788-75.284 43.466z' fill='#607D8B'/><path d='M167.23 125.979v50.871l-27.321 15.773-6.461-14.167c-.91-1.996-3.428-1.738-5.624.574a10.238 10.238 0 0 0-2.33 4.018l-6.46 21.628-27.322 15.774v-50.871l75.518-43.6' fill='#FFF'/><path d='M91.712 220.567a.127.127 0 0 1-.059-.016.118.118 0 0 1-.058-.101v-50.871c0-.042.023-.08.058-.101l75.519-43.6a.117.117 0 0 1 .175.101v50.871c0 .041-.023.08-.059.1l-27.321 15.775a.118.118 0 0 1-.094.01.12.12 0 0 1-.071-.063l-6.46-14.168c-.375-.822-1.062-1.275-1.934-1.275-1.089 0-2.364.686-3.5 1.881a10.206 10.206 0 0 0-2.302 3.972l-6.46 21.627a.118.118 0 0 1-.054.068L91.77 220.551a.12.12 0 0 1-.058.016zm.117-50.92v50.601l27.106-15.65 6.447-21.583a10.286 10.286 0 0 1 2.357-4.065c1.18-1.242 2.517-1.954 3.669-1.954.969 0 1.731.501 2.146 1.411l6.407 14.051 27.152-15.676v-50.601l-75.284 43.466z' fill='#607D8B'/><path d='M168.543 126.213v50.87l-27.322 15.774-6.46-14.168c-.91-1.995-3.428-1.738-5.624.574a10.248 10.248 0 0 0-2.33 4.019l-6.461 21.627-27.321 15.774v-50.87l75.518-43.6' fill='#FFF'/><path d='M93.025 220.8a.123.123 0 0 1-.059-.015.12.12 0 0 1-.058-.101v-50.871c0-.042.023-.08.058-.101l75.518-43.6a.112.112 0 0 1 .117 0c.036.02.059.059.059.1v50.871a.116.116 0 0 1-.059.101l-27.321 15.774a.111.111 0 0 1-.094.01.115.115 0 0 1-.071-.062l-6.46-14.168c-.375-.823-1.062-1.275-1.935-1.275-1.088 0-2.363.685-3.499 1.881a10.19 10.19 0 0 0-2.302 3.971l-6.461 21.628a.108.108 0 0 1-.053.067l-27.322 15.775a.12.12 0 0 1-.058.015zm.117-50.919v50.6l27.106-15.649 6.447-21.584a10.293 10.293 0 0 1 2.357-4.065c1.179-1.241 2.516-1.954 3.668-1.954.969 0 1.732.502 2.147 1.412l6.407 14.051 27.152-15.676v-50.601l-75.284 43.466z' fill='#607D8B'/><path d='M169.8 177.083l-27.322 15.774-6.46-14.168c-.91-1.995-3.428-1.738-5.625.574a10.246 10.246 0 0 0-2.329 4.019l-6.461 21.627-27.321 15.774v-50.87l75.518-43.6v50.87z' fill='#FAFAFA'/><path d='M94.282 220.917a.234.234 0 0 1-.234-.233v-50.871c0-.083.045-.161.117-.202l75.518-43.601a.234.234 0 1 1 .35.202v50.871a.233.233 0 0 1-.116.202l-27.322 15.775a.232.232 0 0 1-.329-.106l-6.461-14.168c-.36-.789-.992-1.206-1.828-1.206-1.056 0-2.301.672-3.415 1.844a10.099 10.099 0 0 0-2.275 3.924l-6.46 21.628a.235.235 0 0 1-.107.136l-27.322 15.774a.23.23 0 0 1-.116.031zm.233-50.969v50.331l26.891-15.525 6.434-21.539a10.41 10.41 0 0 1 2.384-4.112c1.201-1.265 2.569-1.991 3.753-1.991 1.018 0 1.818.526 2.253 1.48l6.354 13.934 26.982-15.578v-50.331l-75.051 43.331z' fill='#607D8B'/><path d='M109.894 199.943c-1.774 0-3.241-.725-4.244-2.12a.224.224 0 0 1 .023-.294.233.233 0 0 1 .301-.023c.78.547 1.705.827 2.75.827 1.323 0 2.754-.439 4.256-1.306 5.311-3.067 9.631-10.518 9.631-16.611 0-1.927-.442-3.56-1.278-4.724a.232.232 0 0 1 .323-.327c1.671 1.172 2.591 3.381 2.591 6.219 0 6.242-4.426 13.863-9.865 17.003-1.574.908-3.084 1.356-4.488 1.356zm-2.969-1.542c.813.651 1.82.877 2.968.877h.001c1.321 0 2.753-.327 4.254-1.194 5.311-3.067 9.632-10.463 9.632-16.556 0-1.979-.463-3.599-1.326-4.761.411 1.035.625 2.275.625 3.635 0 6.243-4.426 13.883-9.865 17.023-1.574.909-3.084 1.317-4.49 1.317-.641 0-1.243-.149-1.799-.341z' fill='#607D8B'/><path d='M113.097 197.23c5.384-3.108 9.748-10.636 9.748-16.814 0-2.051-.483-3.692-1.323-4.86-1.784-1.252-4.374-1.194-7.257.47-5.384 3.108-9.748 10.636-9.748 16.814 0 2.051.483 3.692 1.323 4.86 1.784 1.252 4.374 1.194 7.257-.47' fill='#FAFAFA'/><path d='M108.724 198.614c-1.142 0-2.158-.213-3.019-.817-.021-.014-.04.014-.055-.007-.894-1.244-1.367-2.948-1.367-4.973 0-6.242 4.426-13.864 9.865-17.005 1.574-.908 3.084-1.363 4.49-1.363 1.142 0 2.158.309 3.018.913a.23.23 0 0 1 .056.056c.894 1.244 1.367 2.972 1.367 4.997 0 6.243-4.426 13.783-9.865 16.923-1.574.909-3.084 1.276-4.49 1.276zm-2.718-1.109c.774.532 1.688.776 2.718.776 1.323 0 2.754-.413 4.256-1.28 5.311-3.066 9.631-10.505 9.631-16.598 0-1.909-.434-3.523-1.255-4.685-.774-.533-1.688-.799-2.718-.799-1.323 0-2.755.441-4.256 1.308-5.311 3.066-9.631 10.506-9.631 16.599 0 1.909.434 3.517 1.255 4.679z' fill='#607D8B'/><path d='M149.318 114.262l-9.984 8.878 15.893 11.031 5.589-6.112-11.498-13.797' fill='#FAFAFA'/><path d='M169.676 120.84l-9.748 5.627c-3.642 2.103-9.528 2.113-13.147.024-3.62-2.089-3.601-5.488.041-7.591l9.495-5.608-6.729-3.885-81.836 47.071 45.923 26.514 3.081-1.779c.631-.365.869-.898.618-1.39-2.357-4.632-2.593-9.546-.683-14.262 5.638-13.92 24.509-24.815 48.618-28.07 8.169-1.103 16.68-.967 24.704.394.852.145 1.776.008 2.407-.357l3.081-1.778-25.825-14.91' fill='#FAFAFA'/><path d='M113.675 183.459a.47.47 0 0 1-.233-.062l-45.924-26.515a.468.468 0 0 1 .001-.809l81.836-47.071a.467.467 0 0 1 .466 0l6.729 3.885a.467.467 0 0 1-.467.809l-6.496-3.75-80.9 46.533 44.988 25.973 2.848-1.644c.192-.111.62-.409.435-.773-2.416-4.748-2.658-9.814-.7-14.65 2.806-6.927 8.885-13.242 17.582-18.263 8.657-4.998 19.518-8.489 31.407-10.094 8.198-1.107 16.79-.97 24.844.397.739.125 1.561.007 2.095-.301l2.381-1.374-25.125-14.506a.467.467 0 0 1 .467-.809l25.825 14.91a.467.467 0 0 1 0 .809l-3.081 1.779c-.721.417-1.763.575-2.718.413-7.963-1.351-16.457-1.486-24.563-.392-11.77 1.589-22.512 5.039-31.065 9.977-8.514 4.916-14.456 11.073-17.183 17.805-1.854 4.578-1.623 9.376.666 13.875.37.725.055 1.513-.8 2.006l-3.081 1.78a.476.476 0 0 1-.234.062' fill='#455A64'/><path d='M153.316 128.279c-2.413 0-4.821-.528-6.652-1.586-1.818-1.049-2.82-2.461-2.82-3.975 0-1.527 1.016-2.955 2.861-4.02l9.493-5.607a.233.233 0 1 1 .238.402l-9.496 5.609c-1.696.979-2.628 2.263-2.628 3.616 0 1.34.918 2.608 2.585 3.571 3.549 2.049 9.343 2.038 12.914-.024l9.748-5.628a.234.234 0 0 1 .234.405l-9.748 5.628c-1.858 1.072-4.296 1.609-6.729 1.609' fill='#607D8B'/><path d='M113.675 182.992l-45.913-26.508M113.675 183.342a.346.346 0 0 1-.175-.047l-45.913-26.508a.35.35 0 1 1 .35-.607l45.913 26.508a.35.35 0 0 1-.175.654' fill='#455A64'/><path d='M67.762 156.484v54.001c0 1.09.77 2.418 1.72 2.967l42.473 24.521c.95.549 1.72.11 1.72-.98v-54.001' fill='#FAFAFA'/><path d='M112.727 238.561c-.297 0-.62-.095-.947-.285l-42.473-24.521c-1.063-.613-1.895-2.05-1.895-3.27v-54.001a.35.35 0 1 1 .701 0v54.001c0 .96.707 2.18 1.544 2.663l42.473 24.522c.344.198.661.243.87.122.206-.119.325-.411.325-.799v-54.001a.35.35 0 1 1 .7 0v54.001c0 .655-.239 1.154-.675 1.406a1.235 1.235 0 0 1-.623.162' fill='#455A64'/><path d='M112.86 147.512h-.001c-2.318 0-4.499-.522-6.142-1.471-1.705-.984-2.643-2.315-2.643-3.749 0-1.445.952-2.791 2.68-3.788l12.041-6.953c1.668-.962 3.874-1.493 6.212-1.493 2.318 0 4.499.523 6.143 1.472 1.704.984 2.643 2.315 2.643 3.748 0 1.446-.952 2.791-2.68 3.789l-12.042 6.952c-1.668.963-3.874 1.493-6.211 1.493zm12.147-16.753c-2.217 0-4.298.497-5.861 1.399l-12.042 6.952c-1.502.868-2.33 1.998-2.33 3.182 0 1.173.815 2.289 2.293 3.142 1.538.889 3.596 1.378 5.792 1.378h.001c2.216 0 4.298-.497 5.861-1.399l12.041-6.953c1.502-.867 2.33-1.997 2.33-3.182 0-1.172-.814-2.288-2.292-3.142-1.539-.888-3.596-1.377-5.793-1.377z' fill='#607D8B'/><path d='M165.63 123.219l-5.734 3.311c-3.167 1.828-8.286 1.837-11.433.02-3.147-1.817-3.131-4.772.036-6.601l5.734-3.31 11.397 6.58' fill='#FAFAFA'/><path d='M154.233 117.448l9.995 5.771-4.682 2.704c-1.434.827-3.352 1.283-5.399 1.283-2.029 0-3.923-.449-5.333-1.263-1.29-.744-2-1.694-2-2.674 0-.991.723-1.955 2.036-2.713l5.383-3.108m0-.809l-5.734 3.31c-3.167 1.829-3.183 4.784-.036 6.601 1.568.905 3.623 1.357 5.684 1.357 2.077 0 4.159-.46 5.749-1.377l5.734-3.311-11.397-6.58M145.445 179.667c-1.773 0-3.241-.85-4.243-2.245-.067-.092-.057-.275.023-.356.08-.081.207-.12.3-.055.781.548 1.706.812 2.751.811 1.322 0 2.754-.446 4.256-1.313 5.31-3.066 9.631-10.522 9.631-16.615 0-1.927-.442-3.562-1.279-4.726a.235.235 0 0 1 .024-.301.232.232 0 0 1 .3-.027c1.67 1.172 2.59 3.38 2.59 6.219 0 6.242-4.425 13.987-9.865 17.127-1.573.908-3.083 1.481-4.488 1.481zM142.476 178c.814.651 1.82 1.002 2.969 1.002 1.322 0 2.753-.452 4.255-1.32 5.31-3.065 9.631-10.523 9.631-16.617 0-1.98-.463-3.63-1.325-4.793.411 1.035.624 2.26.624 3.62 0 6.242-4.425 13.875-9.865 17.015-1.573.909-3.084 1.376-4.489 1.376a5.49 5.49 0 0 1-1.8-.283z' fill='#607D8B'/><path d='M148.648 176.704c5.384-3.108 9.748-10.636 9.748-16.813 0-2.052-.483-3.693-1.322-4.861-1.785-1.252-4.375-1.194-7.258.471-5.383 3.108-9.748 10.636-9.748 16.813 0 2.051.484 3.692 1.323 4.86 1.785 1.253 4.374 1.195 7.257-.47' fill='#FAFAFA'/><path d='M144.276 178.276c-1.143 0-2.158-.307-3.019-.911a.217.217 0 0 1-.055-.054c-.895-1.244-1.367-2.972-1.367-4.997 0-6.241 4.425-13.875 9.865-17.016 1.573-.908 3.084-1.369 4.489-1.369 1.143 0 2.158.307 3.019.91a.24.24 0 0 1 .055.055c.894 1.244 1.367 2.971 1.367 4.997 0 6.241-4.425 13.875-9.865 17.016-1.573.908-3.084 1.369-4.489 1.369zm-2.718-1.172c.773.533 1.687.901 2.718.901 1.322 0 2.754-.538 4.256-1.405 5.31-3.066 9.631-10.567 9.631-16.661 0-1.908-.434-3.554-1.256-4.716-.774-.532-1.688-.814-2.718-.814-1.322 0-2.754.433-4.256 1.3-5.31 3.066-9.631 10.564-9.631 16.657 0 1.91.434 3.576 1.256 4.738z' fill='#607D8B'/><path d='M150.72 172.361l-.363-.295a24.105 24.105 0 0 0 2.148-3.128 24.05 24.05 0 0 0 1.977-4.375l.443.149a24.54 24.54 0 0 1-2.015 4.46 24.61 24.61 0 0 1-2.19 3.189M115.917 191.514l-.363-.294a24.174 24.174 0 0 0 2.148-3.128 24.038 24.038 0 0 0 1.976-4.375l.443.148a24.48 24.48 0 0 1-2.015 4.461 24.662 24.662 0 0 1-2.189 3.188M114 237.476V182.584 237.476' fill='#607D8B'/><g><path d='M81.822 37.474c.017-.135-.075-.28-.267-.392-.327-.188-.826-.21-1.109-.045l-6.012 3.471c-.131.076-.194.178-.191.285.002.132.002.461.002.578v.043l-.007.128-6.591 3.779c-.001 0-2.077 1.046-2.787 5.192 0 0-.912 6.961-.898 19.745.015 12.57.606 17.07 1.167 21.351.22 1.684 3.001 2.125 3.001 2.125.331.04.698-.027 1.08-.248l75.273-43.551c1.808-1.069 2.667-3.719 3.056-6.284 1.213-7.99 1.675-32.978-.275-39.878-.196-.693-.51-1.083-.868-1.282l-2.086-.79c-.727.028-1.416.467-1.534.535L82.032 37.072l-.21.402' fill='#FFF'/><path d='M144.311 1.701l2.085.79c.358.199.672.589.868 1.282 1.949 6.9 1.487 31.887.275 39.878-.39 2.565-1.249 5.215-3.056 6.284L69.21 93.486a1.78 1.78 0 0 1-.896.258l-.183-.011c0 .001-2.782-.44-3.003-2.124-.56-4.282-1.151-8.781-1.165-21.351-.015-12.784.897-19.745.897-19.745.71-4.146 2.787-5.192 2.787-5.192l6.591-3.779.007-.128v-.043c0-.117 0-.446-.002-.578-.003-.107.059-.21.191-.285l6.012-3.472a.98.98 0 0 1 .481-.11c.218 0 .449.053.627.156.193.112.285.258.268.392l.211-.402 60.744-34.836c.117-.068.806-.507 1.534-.535m0-.997l-.039.001c-.618.023-1.283.244-1.974.656l-.021.012-60.519 34.706a2.358 2.358 0 0 0-.831-.15c-.365 0-.704.084-.98.244l-6.012 3.471c-.442.255-.699.69-.689 1.166l.001.15-6.08 3.487c-.373.199-2.542 1.531-3.29 5.898l-.006.039c-.009.07-.92 7.173-.906 19.875.014 12.62.603 17.116 1.172 21.465l.002.015c.308 2.355 3.475 2.923 3.836 2.98l.034.004c.101.013.204.019.305.019a2.77 2.77 0 0 0 1.396-.392l75.273-43.552c1.811-1.071 2.999-3.423 3.542-6.997 1.186-7.814 1.734-33.096-.301-40.299-.253-.893-.704-1.527-1.343-1.882l-.132-.062-2.085-.789a.973.973 0 0 0-.353-.065' fill='#455A64'/><path d='M128.267 11.565l1.495.434-56.339 32.326' fill='#FFF'/><path d='M74.202 90.545a.5.5 0 0 1-.25-.931l18.437-10.645a.499.499 0 1 1 .499.864L74.451 90.478l-.249.067M75.764 42.654l-.108-.062.046-.171 5.135-2.964.17.045-.045.171-5.135 2.964-.063.017M70.52 90.375V46.421l.063-.036L137.84 7.554v43.954l-.062.036L70.52 90.375zm.25-43.811v43.38l66.821-38.579V7.985L70.77 46.564z' fill='#607D8B'/><path d='M86.986 83.182c-.23.149-.612.384-.849.523l-11.505 6.701c-.237.139-.206.252.068.252h.565c.275 0 .693-.113.93-.252L87.7 83.705c.237-.139.428-.253.425-.256a11.29 11.29 0 0 1-.006-.503c0-.274-.188-.377-.418-.227l-.715.463' fill='#607D8B'/><path d='M75.266 90.782H74.7c-.2 0-.316-.056-.346-.166-.03-.11.043-.217.215-.317l11.505-6.702c.236-.138.615-.371.844-.519l.715-.464a.488.488 0 0 1 .266-.089c.172 0 .345.13.345.421 0 .214.001.363.003.437l.006.004-.004.069c-.003.075-.003.075-.486.356l-11.505 6.702a2.282 2.282 0 0 1-.992.268zm-.6-.25l.034.001h.566c.252 0 .649-.108.866-.234l11.505-6.702c.168-.098.294-.173.361-.214-.004-.084-.004-.218-.004-.437l-.095-.171-.131.049-.714.463c-.232.15-.616.386-.854.525l-11.505 6.702-.029.018z' fill='#607D8B'/><path d='M75.266 89.871H74.7c-.2 0-.316-.056-.346-.166-.03-.11.043-.217.215-.317l11.505-6.702c.258-.151.694-.268.993-.268h.565c.2 0 .316.056.346.166.03.11-.043.217-.215.317l-11.505 6.702a2.282 2.282 0 0 1-.992.268zm-.6-.25l.034.001h.566c.252 0 .649-.107.866-.234l11.505-6.702.03-.018-.035-.001h-.565c-.252 0-.649.108-.867.234l-11.505 6.702-.029.018zM74.37 90.801v-1.247 1.247' fill='#607D8B'/><path d='M68.13 93.901c-.751-.093-1.314-.737-1.439-1.376-.831-4.238-1.151-8.782-1.165-21.352-.015-12.784.897-19.745.897-19.745.711-4.146 2.787-5.192 2.787-5.192l74.859-43.219c.223-.129 2.487-1.584 3.195.923 1.95 6.9 1.488 31.887.275 39.878-.389 2.565-1.248 5.215-3.056 6.283L69.21 93.653c-.382.221-.749.288-1.08.248 0 0-2.781-.441-3.001-2.125-.561-4.281-1.152-8.781-1.167-21.351-.014-12.784.898-19.745.898-19.745.71-4.146 2.787-5.191 2.787-5.191l6.598-3.81.871-.119 6.599-3.83.046-.461L68.13 93.901' fill='#FAFAFA'/><path d='M68.317 94.161l-.215-.013h-.001l-.244-.047c-.719-.156-2.772-.736-2.976-2.292-.568-4.34-1.154-8.813-1.168-21.384-.014-12.654.891-19.707.9-19.777.725-4.231 2.832-5.338 2.922-5.382l6.628-3.827.87-.119 6.446-3.742.034-.334a.248.248 0 0 1 .273-.223.248.248 0 0 1 .223.272l-.059.589-6.752 3.919-.87.118-6.556 3.785c-.031.016-1.99 1.068-2.666 5.018-.007.06-.908 7.086-.894 19.702.014 12.539.597 16.996 1.161 21.305.091.691.689 1.154 1.309 1.452a1.95 1.95 0 0 1-.236-.609c-.781-3.984-1.155-8.202-1.17-21.399-.014-12.653.891-19.707.9-19.777.725-4.231 2.832-5.337 2.922-5.382-.004.001 74.444-42.98 74.846-43.212l.028-.017c.904-.538 1.72-.688 2.36-.433.555.221.949.733 1.172 1.52 2.014 7.128 1.46 32.219.281 39.983-.507 3.341-1.575 5.515-3.175 6.462L69.335 93.869a2.023 2.023 0 0 1-1.018.292zm-.147-.507c.293.036.604-.037.915-.217l75.273-43.551c1.823-1.078 2.602-3.915 2.934-6.106 1.174-7.731 1.731-32.695-.268-39.772-.178-.631-.473-1.032-.876-1.192-.484-.193-1.166-.052-1.921.397l-.034.021-74.858 43.218c-.031.017-1.989 1.069-2.666 5.019-.007.059-.908 7.085-.894 19.702.015 13.155.386 17.351 1.161 21.303.09.461.476.983 1.037 1.139.114.025.185.037.196.039h.001z' fill='#455A64'/><path d='M69.317 68.982c.489-.281.885-.056.885.505 0 .56-.396 1.243-.885 1.525-.488.282-.884.057-.884-.504 0-.56.396-1.243.884-1.526' fill='#FFF'/><path d='M68.92 71.133c-.289 0-.487-.228-.487-.625 0-.56.396-1.243.884-1.526a.812.812 0 0 1 .397-.121c.289 0 .488.229.488.626 0 .56-.396 1.243-.885 1.525a.812.812 0 0 1-.397.121m.794-2.459a.976.976 0 0 0-.49.147c-.548.317-.978 1.058-.978 1.687 0 .486.271.812.674.812a.985.985 0 0 0 .491-.146c.548-.317.978-1.057.978-1.687 0-.486-.272-.813-.675-.813' fill='#8097A2'/><path d='M68.92 70.947c-.271 0-.299-.307-.299-.439 0-.491.361-1.116.79-1.363a.632.632 0 0 1 .303-.096c.272 0 .301.306.301.438 0 .491-.363 1.116-.791 1.364a.629.629 0 0 1-.304.096m.794-2.086a.812.812 0 0 0-.397.121c-.488.283-.884.966-.884 1.526 0 .397.198.625.487.625a.812.812 0 0 0 .397-.121c.489-.282.885-.965.885-1.525 0-.397-.199-.626-.488-.626' fill='#8097A2'/><path d='M69.444 85.35c.264-.152.477-.031.477.272 0 .303-.213.67-.477.822-.263.153-.477.031-.477-.271 0-.302.214-.671.477-.823' fill='#FFF'/><path d='M69.23 86.51c-.156 0-.263-.123-.263-.337 0-.302.214-.671.477-.823a.431.431 0 0 1 .214-.066c.156 0 .263.124.263.338 0 .303-.213.67-.477.822a.431.431 0 0 1-.214.066m.428-1.412c-.1 0-.203.029-.307.09-.32.185-.57.618-.57.985 0 .309.185.524.449.524a.63.63 0 0 0 .308-.09c.32-.185.57-.618.57-.985 0-.309-.185-.524-.45-.524' fill='#8097A2'/><path d='M69.23 86.322l-.076-.149c0-.235.179-.544.384-.661l.12-.041.076.151c0 .234-.179.542-.383.66l-.121.04m.428-1.038a.431.431 0 0 0-.214.066c-.263.152-.477.521-.477.823 0 .214.107.337.263.337a.431.431 0 0 0 .214-.066c.264-.152.477-.519.477-.822 0-.214-.107-.338-.263-.338' fill='#8097A2'/><path d='M139.278 7.769v43.667L72.208 90.16V46.493l67.07-38.724' fill='#455A64'/><path d='M72.083 90.375V46.421l.063-.036 67.257-38.831v43.954l-.062.036-67.258 38.831zm.25-43.811v43.38l66.821-38.579V7.985L72.333 46.564z' fill='#607D8B'/></g><path d='M125.737 88.647l-7.639 3.334V84l-11.459 4.713v8.269L99 100.315l13.369 3.646 13.368-15.314' fill='#455A64'/></g></svg>";
      function RotateInstructions() {
        this.loadIcon_();
        var overlay = document.createElement("div");
        var s = overlay.style;
        s.position = "fixed";
        s.top = 0;
        s.right = 0;
        s.bottom = 0;
        s.left = 0;
        s.backgroundColor = "gray";
        s.fontFamily = "sans-serif";
        s.zIndex = 1e6;
        var img = document.createElement("img");
        img.src = this.icon;
        var s = img.style;
        s.marginLeft = "25%";
        s.marginTop = "25%";
        s.width = "50%";
        overlay.appendChild(img);
        var text = document.createElement("div");
        var s = text.style;
        s.textAlign = "center";
        s.fontSize = "16px";
        s.lineHeight = "24px";
        s.margin = "24px 25%";
        s.width = "50%";
        text.innerHTML = "Place your phone into your Cardboard viewer.";
        overlay.appendChild(text);
        var snackbar = document.createElement("div");
        var s = snackbar.style;
        s.backgroundColor = "#CFD8DC";
        s.position = "fixed";
        s.bottom = 0;
        s.width = "100%";
        s.height = "48px";
        s.padding = "14px 24px";
        s.boxSizing = "border-box";
        s.color = "#656A6B";
        overlay.appendChild(snackbar);
        var snackbarText = document.createElement("div");
        snackbarText.style.float = "left";
        snackbarText.innerHTML = "No Cardboard viewer?";
        var snackbarButton = document.createElement("a");
        snackbarButton.href = "https://www.google.com/get/cardboard/get-cardboard/";
        snackbarButton.innerHTML = "get one";
        snackbarButton.target = "_blank";
        var s = snackbarButton.style;
        s.float = "right";
        s.fontWeight = 600;
        s.textTransform = "uppercase";
        s.borderLeft = "1px solid gray";
        s.paddingLeft = "24px";
        s.textDecoration = "none";
        s.color = "#656A6B";
        snackbar.appendChild(snackbarText);
        snackbar.appendChild(snackbarButton);
        this.overlay = overlay;
        this.text = text;
        this.hide();
      }
      RotateInstructions.prototype.show = function(parent) {
        if (!parent && !this.overlay.parentElement) {
          document.body.appendChild(this.overlay);
        } else if (parent) {
          if (this.overlay.parentElement && this.overlay.parentElement != parent)
            this.overlay.parentElement.removeChild(this.overlay);
          parent.appendChild(this.overlay);
        }
        this.overlay.style.display = "block";
        var img = this.overlay.querySelector("img");
        var s = img.style;
        if (isLandscapeMode()) {
          s.width = "20%";
          s.marginLeft = "40%";
          s.marginTop = "3%";
        } else {
          s.width = "50%";
          s.marginLeft = "25%";
          s.marginTop = "25%";
        }
      };
      RotateInstructions.prototype.hide = function() {
        this.overlay.style.display = "none";
      };
      RotateInstructions.prototype.showTemporarily = function(ms, parent) {
        this.show(parent);
        this.timer = setTimeout(this.hide.bind(this), ms);
      };
      RotateInstructions.prototype.disableShowTemporarily = function() {
        clearTimeout(this.timer);
      };
      RotateInstructions.prototype.update = function() {
        this.disableShowTemporarily();
        if (!isLandscapeMode() && isMobile3()) {
          this.show();
        } else {
          this.hide();
        }
      };
      RotateInstructions.prototype.loadIcon_ = function() {
        this.icon = dataUri("image/svg+xml", rotateInstructionsAsset);
      };
      var DEFAULT_VIEWER = "CardboardV1";
      var VIEWER_KEY = "WEBVR_CARDBOARD_VIEWER";
      var CLASS_NAME = "webvr-polyfill-viewer-selector";
      function ViewerSelector(defaultViewer) {
        try {
          this.selectedKey = localStorage.getItem(VIEWER_KEY);
        } catch (error) {
          console.error("Failed to load viewer profile: %s", error);
        }
        if (!this.selectedKey) {
          this.selectedKey = defaultViewer || DEFAULT_VIEWER;
        }
        this.dialog = this.createDialog_(DeviceInfo.Viewers);
        this.root = null;
        this.onChangeCallbacks_ = [];
      }
      ViewerSelector.prototype.show = function(root) {
        this.root = root;
        root.appendChild(this.dialog);
        var selected = this.dialog.querySelector("#" + this.selectedKey);
        selected.checked = true;
        this.dialog.style.display = "block";
      };
      ViewerSelector.prototype.hide = function() {
        if (this.root && this.root.contains(this.dialog)) {
          this.root.removeChild(this.dialog);
        }
        this.dialog.style.display = "none";
      };
      ViewerSelector.prototype.getCurrentViewer = function() {
        return DeviceInfo.Viewers[this.selectedKey];
      };
      ViewerSelector.prototype.getSelectedKey_ = function() {
        var input = this.dialog.querySelector("input[name=field]:checked");
        if (input) {
          return input.id;
        }
        return null;
      };
      ViewerSelector.prototype.onChange = function(cb) {
        this.onChangeCallbacks_.push(cb);
      };
      ViewerSelector.prototype.fireOnChange_ = function(viewer) {
        for (var i = 0; i < this.onChangeCallbacks_.length; i++) {
          this.onChangeCallbacks_[i](viewer);
        }
      };
      ViewerSelector.prototype.onSave_ = function() {
        this.selectedKey = this.getSelectedKey_();
        if (!this.selectedKey || !DeviceInfo.Viewers[this.selectedKey]) {
          console.error("ViewerSelector.onSave_: this should never happen!");
          return;
        }
        this.fireOnChange_(DeviceInfo.Viewers[this.selectedKey]);
        try {
          localStorage.setItem(VIEWER_KEY, this.selectedKey);
        } catch (error) {
          console.error("Failed to save viewer profile: %s", error);
        }
        this.hide();
      };
      ViewerSelector.prototype.createDialog_ = function(options) {
        var container = document.createElement("div");
        container.classList.add(CLASS_NAME);
        container.style.display = "none";
        var overlay = document.createElement("div");
        var s = overlay.style;
        s.position = "fixed";
        s.left = 0;
        s.top = 0;
        s.width = "100%";
        s.height = "100%";
        s.background = "rgba(0, 0, 0, 0.3)";
        overlay.addEventListener("click", this.hide.bind(this));
        var width2 = 280;
        var dialog = document.createElement("div");
        var s = dialog.style;
        s.boxSizing = "border-box";
        s.position = "fixed";
        s.top = "24px";
        s.left = "50%";
        s.marginLeft = -width2 / 2 + "px";
        s.width = width2 + "px";
        s.padding = "24px";
        s.overflow = "hidden";
        s.background = "#fafafa";
        s.fontFamily = "'Roboto', sans-serif";
        s.boxShadow = "0px 5px 20px #666";
        dialog.appendChild(this.createH1_("Select your viewer"));
        for (var id2 in options) {
          dialog.appendChild(this.createChoice_(id2, options[id2].label));
        }
        dialog.appendChild(this.createButton_("Save", this.onSave_.bind(this)));
        container.appendChild(overlay);
        container.appendChild(dialog);
        return container;
      };
      ViewerSelector.prototype.createH1_ = function(name2) {
        var h1 = document.createElement("h1");
        var s = h1.style;
        s.color = "black";
        s.fontSize = "20px";
        s.fontWeight = "bold";
        s.marginTop = 0;
        s.marginBottom = "24px";
        h1.innerHTML = name2;
        return h1;
      };
      ViewerSelector.prototype.createChoice_ = function(id2, name2) {
        var div2 = document.createElement("div");
        div2.style.marginTop = "8px";
        div2.style.color = "black";
        var input = document.createElement("input");
        input.style.fontSize = "30px";
        input.setAttribute("id", id2);
        input.setAttribute("type", "radio");
        input.setAttribute("value", id2);
        input.setAttribute("name", "field");
        var label = document.createElement("label");
        label.style.marginLeft = "4px";
        label.setAttribute("for", id2);
        label.innerHTML = name2;
        div2.appendChild(input);
        div2.appendChild(label);
        return div2;
      };
      ViewerSelector.prototype.createButton_ = function(label, onclick) {
        var button = document.createElement("button");
        button.innerHTML = label;
        var s = button.style;
        s.float = "right";
        s.textTransform = "uppercase";
        s.color = "#1094f7";
        s.fontSize = "14px";
        s.letterSpacing = 0;
        s.border = 0;
        s.background = "none";
        s.marginTop = "16px";
        button.addEventListener("click", onclick);
        return button;
      };
      var commonjsGlobal = typeof window !== "undefined" ? window : typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : {};
      function unwrapExports(x) {
        return x && x.__esModule && Object.prototype.hasOwnProperty.call(x, "default") ? x["default"] : x;
      }
      function createCommonjsModule(fn, module2) {
        return module2 = { exports: {} }, fn(module2, module2.exports), module2.exports;
      }
      var NoSleep = createCommonjsModule(function(module2, exports2) {
        (function webpackUniversalModuleDefinition(root, factory) {
          module2.exports = factory();
        })(commonjsGlobal, function() {
          return function(modules) {
            var installedModules = {};
            function __webpack_require__(moduleId) {
              if (installedModules[moduleId]) {
                return installedModules[moduleId].exports;
              }
              var module3 = installedModules[moduleId] = {
                i: moduleId,
                l: false,
                exports: {}
              };
              modules[moduleId].call(module3.exports, module3, module3.exports, __webpack_require__);
              module3.l = true;
              return module3.exports;
            }
            __webpack_require__.m = modules;
            __webpack_require__.c = installedModules;
            __webpack_require__.d = function(exports3, name2, getter) {
              if (!__webpack_require__.o(exports3, name2)) {
                Object.defineProperty(exports3, name2, {
                  configurable: false,
                  enumerable: true,
                  get: getter
                });
              }
            };
            __webpack_require__.n = function(module3) {
              var getter = module3 && module3.__esModule ? function getDefault() {
                return module3["default"];
              } : function getModuleExports() {
                return module3;
              };
              __webpack_require__.d(getter, "a", getter);
              return getter;
            };
            __webpack_require__.o = function(object, property) {
              return Object.prototype.hasOwnProperty.call(object, property);
            };
            __webpack_require__.p = "";
            return __webpack_require__(__webpack_require__.s = 0);
          }([
            function(module3, exports3, __webpack_require__) {
              "use strict";
              var _createClass = function() {
                function defineProperties(target, props) {
                  for (var i = 0; i < props.length; i++) {
                    var descriptor = props[i];
                    descriptor.enumerable = descriptor.enumerable || false;
                    descriptor.configurable = true;
                    if ("value" in descriptor)
                      descriptor.writable = true;
                    Object.defineProperty(target, descriptor.key, descriptor);
                  }
                }
                return function(Constructor, protoProps, staticProps) {
                  if (protoProps)
                    defineProperties(Constructor.prototype, protoProps);
                  if (staticProps)
                    defineProperties(Constructor, staticProps);
                  return Constructor;
                };
              }();
              function _classCallCheck(instance, Constructor) {
                if (!(instance instanceof Constructor)) {
                  throw new TypeError("Cannot call a class as a function");
                }
              }
              var mediaFile = __webpack_require__(1);
              var oldIOS = typeof navigator !== "undefined" && parseFloat(("" + (/CPU.*OS ([0-9_]{3,4})[0-9_]{0,1}|(CPU like).*AppleWebKit.*Mobile/i.exec(navigator.userAgent) || [0, ""])[1]).replace("undefined", "3_2").replace("_", ".").replace("_", "")) < 10 && !window.MSStream;
              var NoSleep2 = function() {
                function NoSleep3() {
                  _classCallCheck(this, NoSleep3);
                  if (oldIOS) {
                    this.noSleepTimer = null;
                  } else {
                    this.noSleepVideo = document.createElement("video");
                    this.noSleepVideo.setAttribute("playsinline", "");
                    this.noSleepVideo.setAttribute("src", mediaFile);
                    this.noSleepVideo.addEventListener("timeupdate", function(e2) {
                      if (this.noSleepVideo.currentTime > 0.5) {
                        this.noSleepVideo.currentTime = Math.random();
                      }
                    }.bind(this));
                  }
                }
                _createClass(NoSleep3, [{
                  key: "enable",
                  value: function enable() {
                    if (oldIOS) {
                      this.disable();
                      this.noSleepTimer = window.setInterval(function() {
                        window.location.href = "/";
                        window.setTimeout(window.stop, 0);
                      }, 15e3);
                    } else {
                      this.noSleepVideo.play();
                    }
                  }
                }, {
                  key: "disable",
                  value: function disable() {
                    if (oldIOS) {
                      if (this.noSleepTimer) {
                        window.clearInterval(this.noSleepTimer);
                        this.noSleepTimer = null;
                      }
                    } else {
                      this.noSleepVideo.pause();
                    }
                  }
                }]);
                return NoSleep3;
              }();
              module3.exports = NoSleep2;
            },
            function(module3, exports3, __webpack_require__) {
              "use strict";
              module3.exports = "data:video/mp4;base64,AAAAIGZ0eXBtcDQyAAACAGlzb21pc28yYXZjMW1wNDEAAAAIZnJlZQAACKBtZGF0AAAC8wYF///v3EXpvebZSLeWLNgg2SPu73gyNjQgLSBjb3JlIDE0MiByMjQ3OSBkZDc5YTYxIC0gSC4yNjQvTVBFRy00IEFWQyBjb2RlYyAtIENvcHlsZWZ0IDIwMDMtMjAxNCAtIGh0dHA6Ly93d3cudmlkZW9sYW4ub3JnL3gyNjQuaHRtbCAtIG9wdGlvbnM6IGNhYmFjPTEgcmVmPTEgZGVibG9jaz0xOjA6MCBhbmFseXNlPTB4MToweDExMSBtZT1oZXggc3VibWU9MiBwc3k9MSBwc3lfcmQ9MS4wMDowLjAwIG1peGVkX3JlZj0wIG1lX3JhbmdlPTE2IGNocm9tYV9tZT0xIHRyZWxsaXM9MCA4eDhkY3Q9MCBjcW09MCBkZWFkem9uZT0yMSwxMSBmYXN0X3Bza2lwPTEgY2hyb21hX3FwX29mZnNldD0wIHRocmVhZHM9NiBsb29rYWhlYWRfdGhyZWFkcz0xIHNsaWNlZF90aHJlYWRzPTAgbnI9MCBkZWNpbWF0ZT0xIGludGVybGFjZWQ9MCBibHVyYXlfY29tcGF0PTAgY29uc3RyYWluZWRfaW50cmE9MCBiZnJhbWVzPTMgYl9weXJhbWlkPTIgYl9hZGFwdD0xIGJfYmlhcz0wIGRpcmVjdD0xIHdlaWdodGI9MSBvcGVuX2dvcD0wIHdlaWdodHA9MSBrZXlpbnQ9MzAwIGtleWludF9taW49MzAgc2NlbmVjdXQ9NDAgaW50cmFfcmVmcmVzaD0wIHJjX2xvb2thaGVhZD0xMCByYz1jcmYgbWJ0cmVlPTEgY3JmPTIwLjAgcWNvbXA9MC42MCBxcG1pbj0wIHFwbWF4PTY5IHFwc3RlcD00IHZidl9tYXhyYXRlPTIwMDAwIHZidl9idWZzaXplPTI1MDAwIGNyZl9tYXg9MC4wIG5hbF9ocmQ9bm9uZSBmaWxsZXI9MCBpcF9yYXRpbz0xLjQwIGFxPTE6MS4wMACAAAAAOWWIhAA3//p+C7v8tDDSTjf97w55i3SbRPO4ZY+hkjD5hbkAkL3zpJ6h/LR1CAABzgB1kqqzUorlhQAAAAxBmiQYhn/+qZYADLgAAAAJQZ5CQhX/AAj5IQADQGgcIQADQGgcAAAACQGeYUQn/wALKCEAA0BoHAAAAAkBnmNEJ/8ACykhAANAaBwhAANAaBwAAAANQZpoNExDP/6plgAMuSEAA0BoHAAAAAtBnoZFESwr/wAI+SEAA0BoHCEAA0BoHAAAAAkBnqVEJ/8ACykhAANAaBwAAAAJAZ6nRCf/AAsoIQADQGgcIQADQGgcAAAADUGarDRMQz/+qZYADLghAANAaBwAAAALQZ7KRRUsK/8ACPkhAANAaBwAAAAJAZ7pRCf/AAsoIQADQGgcIQADQGgcAAAACQGe60Qn/wALKCEAA0BoHAAAAA1BmvA0TEM//qmWAAy5IQADQGgcIQADQGgcAAAAC0GfDkUVLCv/AAj5IQADQGgcAAAACQGfLUQn/wALKSEAA0BoHCEAA0BoHAAAAAkBny9EJ/8ACyghAANAaBwAAAANQZs0NExDP/6plgAMuCEAA0BoHAAAAAtBn1JFFSwr/wAI+SEAA0BoHCEAA0BoHAAAAAkBn3FEJ/8ACyghAANAaBwAAAAJAZ9zRCf/AAsoIQADQGgcIQADQGgcAAAADUGbeDRMQz/+qZYADLkhAANAaBwAAAALQZ+WRRUsK/8ACPghAANAaBwhAANAaBwAAAAJAZ+1RCf/AAspIQADQGgcAAAACQGft0Qn/wALKSEAA0BoHCEAA0BoHAAAAA1Bm7w0TEM//qmWAAy4IQADQGgcAAAAC0Gf2kUVLCv/AAj5IQADQGgcAAAACQGf+UQn/wALKCEAA0BoHCEAA0BoHAAAAAkBn/tEJ/8ACykhAANAaBwAAAANQZvgNExDP/6plgAMuSEAA0BoHCEAA0BoHAAAAAtBnh5FFSwr/wAI+CEAA0BoHAAAAAkBnj1EJ/8ACyghAANAaBwhAANAaBwAAAAJAZ4/RCf/AAspIQADQGgcAAAADUGaJDRMQz/+qZYADLghAANAaBwAAAALQZ5CRRUsK/8ACPkhAANAaBwhAANAaBwAAAAJAZ5hRCf/AAsoIQADQGgcAAAACQGeY0Qn/wALKSEAA0BoHCEAA0BoHAAAAA1Bmmg0TEM//qmWAAy5IQADQGgcAAAAC0GehkUVLCv/AAj5IQADQGgcIQADQGgcAAAACQGepUQn/wALKSEAA0BoHAAAAAkBnqdEJ/8ACyghAANAaBwAAAANQZqsNExDP/6plgAMuCEAA0BoHCEAA0BoHAAAAAtBnspFFSwr/wAI+SEAA0BoHAAAAAkBnulEJ/8ACyghAANAaBwhAANAaBwAAAAJAZ7rRCf/AAsoIQADQGgcAAAADUGa8DRMQz/+qZYADLkhAANAaBwhAANAaBwAAAALQZ8ORRUsK/8ACPkhAANAaBwAAAAJAZ8tRCf/AAspIQADQGgcIQADQGgcAAAACQGfL0Qn/wALKCEAA0BoHAAAAA1BmzQ0TEM//qmWAAy4IQADQGgcAAAAC0GfUkUVLCv/AAj5IQADQGgcIQADQGgcAAAACQGfcUQn/wALKCEAA0BoHAAAAAkBn3NEJ/8ACyghAANAaBwhAANAaBwAAAANQZt4NExC//6plgAMuSEAA0BoHAAAAAtBn5ZFFSwr/wAI+CEAA0BoHCEAA0BoHAAAAAkBn7VEJ/8ACykhAANAaBwAAAAJAZ+3RCf/AAspIQADQGgcAAAADUGbuzRMQn/+nhAAYsAhAANAaBwhAANAaBwAAAAJQZ/aQhP/AAspIQADQGgcAAAACQGf+UQn/wALKCEAA0BoHCEAA0BoHCEAA0BoHCEAA0BoHCEAA0BoHCEAA0BoHAAACiFtb292AAAAbG12aGQAAAAA1YCCX9WAgl8AAAPoAAAH/AABAAABAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAGGlvZHMAAAAAEICAgAcAT////v7/AAAF+XRyYWsAAABcdGtoZAAAAAPVgIJf1YCCXwAAAAEAAAAAAAAH0AAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAEAAAAAAygAAAMoAAAAAACRlZHRzAAAAHGVsc3QAAAAAAAAAAQAAB9AAABdwAAEAAAAABXFtZGlhAAAAIG1kaGQAAAAA1YCCX9WAgl8AAV+QAAK/IFXEAAAAAAAtaGRscgAAAAAAAAAAdmlkZQAAAAAAAAAAAAAAAFZpZGVvSGFuZGxlcgAAAAUcbWluZgAAABR2bWhkAAAAAQAAAAAAAAAAAAAAJGRpbmYAAAAcZHJlZgAAAAAAAAABAAAADHVybCAAAAABAAAE3HN0YmwAAACYc3RzZAAAAAAAAAABAAAAiGF2YzEAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAygDKAEgAAABIAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAY//8AAAAyYXZjQwFNQCj/4QAbZ01AKOyho3ySTUBAQFAAAAMAEAAr8gDxgxlgAQAEaO+G8gAAABhzdHRzAAAAAAAAAAEAAAA8AAALuAAAABRzdHNzAAAAAAAAAAEAAAABAAAB8GN0dHMAAAAAAAAAPAAAAAEAABdwAAAAAQAAOpgAAAABAAAXcAAAAAEAAAAAAAAAAQAAC7gAAAABAAA6mAAAAAEAABdwAAAAAQAAAAAAAAABAAALuAAAAAEAADqYAAAAAQAAF3AAAAABAAAAAAAAAAEAAAu4AAAAAQAAOpgAAAABAAAXcAAAAAEAAAAAAAAAAQAAC7gAAAABAAA6mAAAAAEAABdwAAAAAQAAAAAAAAABAAALuAAAAAEAADqYAAAAAQAAF3AAAAABAAAAAAAAAAEAAAu4AAAAAQAAOpgAAAABAAAXcAAAAAEAAAAAAAAAAQAAC7gAAAABAAA6mAAAAAEAABdwAAAAAQAAAAAAAAABAAALuAAAAAEAADqYAAAAAQAAF3AAAAABAAAAAAAAAAEAAAu4AAAAAQAAOpgAAAABAAAXcAAAAAEAAAAAAAAAAQAAC7gAAAABAAA6mAAAAAEAABdwAAAAAQAAAAAAAAABAAALuAAAAAEAADqYAAAAAQAAF3AAAAABAAAAAAAAAAEAAAu4AAAAAQAAOpgAAAABAAAXcAAAAAEAAAAAAAAAAQAAC7gAAAABAAA6mAAAAAEAABdwAAAAAQAAAAAAAAABAAALuAAAAAEAAC7gAAAAAQAAF3AAAAABAAAAAAAAABxzdHNjAAAAAAAAAAEAAAABAAAAAQAAAAEAAAEEc3RzegAAAAAAAAAAAAAAPAAAAzQAAAAQAAAADQAAAA0AAAANAAAAEQAAAA8AAAANAAAADQAAABEAAAAPAAAADQAAAA0AAAARAAAADwAAAA0AAAANAAAAEQAAAA8AAAANAAAADQAAABEAAAAPAAAADQAAAA0AAAARAAAADwAAAA0AAAANAAAAEQAAAA8AAAANAAAADQAAABEAAAAPAAAADQAAAA0AAAARAAAADwAAAA0AAAANAAAAEQAAAA8AAAANAAAADQAAABEAAAAPAAAADQAAAA0AAAARAAAADwAAAA0AAAANAAAAEQAAAA8AAAANAAAADQAAABEAAAANAAAADQAAAQBzdGNvAAAAAAAAADwAAAAwAAADZAAAA3QAAAONAAADoAAAA7kAAAPQAAAD6wAAA/4AAAQXAAAELgAABEMAAARcAAAEbwAABIwAAAShAAAEugAABM0AAATkAAAE/wAABRIAAAUrAAAFQgAABV0AAAVwAAAFiQAABaAAAAW1AAAFzgAABeEAAAX+AAAGEwAABiwAAAY/AAAGVgAABnEAAAaEAAAGnQAABrQAAAbPAAAG4gAABvUAAAcSAAAHJwAAB0AAAAdTAAAHcAAAB4UAAAeeAAAHsQAAB8gAAAfjAAAH9gAACA8AAAgmAAAIQQAACFQAAAhnAAAIhAAACJcAAAMsdHJhawAAAFx0a2hkAAAAA9WAgl/VgIJfAAAAAgAAAAAAAAf8AAAAAAAAAAAAAAABAQAAAAABAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAACsm1kaWEAAAAgbWRoZAAAAADVgIJf1YCCXwAArEQAAWAAVcQAAAAAACdoZGxyAAAAAAAAAABzb3VuAAAAAAAAAAAAAAAAU3RlcmVvAAAAAmNtaW5mAAAAEHNtaGQAAAAAAAAAAAAAACRkaW5mAAAAHGRyZWYAAAAAAAAAAQAAAAx1cmwgAAAAAQAAAidzdGJsAAAAZ3N0c2QAAAAAAAAAAQAAAFdtcDRhAAAAAAAAAAEAAAAAAAAAAAACABAAAAAArEQAAAAAADNlc2RzAAAAAAOAgIAiAAIABICAgBRAFQAAAAADDUAAAAAABYCAgAISEAaAgIABAgAAABhzdHRzAAAAAAAAAAEAAABYAAAEAAAAABxzdHNjAAAAAAAAAAEAAAABAAAAAQAAAAEAAAAUc3RzegAAAAAAAAAGAAAAWAAAAXBzdGNvAAAAAAAAAFgAAAOBAAADhwAAA5oAAAOtAAADswAAA8oAAAPfAAAD5QAAA/gAAAQLAAAEEQAABCgAAAQ9AAAEUAAABFYAAARpAAAEgAAABIYAAASbAAAErgAABLQAAATHAAAE3gAABPMAAAT5AAAFDAAABR8AAAUlAAAFPAAABVEAAAVXAAAFagAABX0AAAWDAAAFmgAABa8AAAXCAAAFyAAABdsAAAXyAAAF+AAABg0AAAYgAAAGJgAABjkAAAZQAAAGZQAABmsAAAZ+AAAGkQAABpcAAAauAAAGwwAABskAAAbcAAAG7wAABwYAAAcMAAAHIQAABzQAAAc6AAAHTQAAB2QAAAdqAAAHfwAAB5IAAAeYAAAHqwAAB8IAAAfXAAAH3QAAB/AAAAgDAAAICQAACCAAAAg1AAAIOwAACE4AAAhhAAAIeAAACH4AAAiRAAAIpAAACKoAAAiwAAAItgAACLwAAAjCAAAAFnVkdGEAAAAObmFtZVN0ZXJlbwAAAHB1ZHRhAAAAaG1ldGEAAAAAAAAAIWhkbHIAAAAAAAAAAG1kaXJhcHBsAAAAAAAAAAAAAAAAO2lsc3QAAAAzqXRvbwAAACtkYXRhAAAAAQAAAABIYW5kQnJha2UgMC4xMC4yIDIwMTUwNjExMDA=";
            }
          ]);
        });
      });
      var NoSleep$1 = unwrapExports(NoSleep);
      var nextDisplayId = 1e3;
      var defaultLeftBounds = [0, 0, 0.5, 1];
      var defaultRightBounds = [0.5, 0, 0.5, 1];
      var raf = window.requestAnimationFrame;
      var caf = window.cancelAnimationFrame;
      function VRFrameData() {
        this.leftProjectionMatrix = new Float32Array(16);
        this.leftViewMatrix = new Float32Array(16);
        this.rightProjectionMatrix = new Float32Array(16);
        this.rightViewMatrix = new Float32Array(16);
        this.pose = null;
      }
      function VRDisplayCapabilities(config2) {
        Object.defineProperties(this, {
          hasPosition: {
            writable: false,
            enumerable: true,
            value: config2.hasPosition
          },
          hasExternalDisplay: {
            writable: false,
            enumerable: true,
            value: config2.hasExternalDisplay
          },
          canPresent: {
            writable: false,
            enumerable: true,
            value: config2.canPresent
          },
          maxLayers: {
            writable: false,
            enumerable: true,
            value: config2.maxLayers
          },
          hasOrientation: {
            enumerable: true,
            get: function get() {
              deprecateWarning("VRDisplayCapabilities.prototype.hasOrientation", "VRDisplay.prototype.getFrameData");
              return config2.hasOrientation;
            }
          }
        });
      }
      function VRDisplay(config2) {
        config2 = config2 || {};
        var USE_WAKELOCK = "wakelock" in config2 ? config2.wakelock : true;
        this.isPolyfilled = true;
        this.displayId = nextDisplayId++;
        this.displayName = "";
        this.depthNear = 0.01;
        this.depthFar = 1e4;
        this.isPresenting = false;
        Object.defineProperty(this, "isConnected", {
          get: function get() {
            deprecateWarning("VRDisplay.prototype.isConnected", "VRDisplayCapabilities.prototype.hasExternalDisplay");
            return false;
          }
        });
        this.capabilities = new VRDisplayCapabilities({
          hasPosition: false,
          hasOrientation: false,
          hasExternalDisplay: false,
          canPresent: false,
          maxLayers: 1
        });
        this.stageParameters = null;
        this.waitingForPresent_ = false;
        this.layer_ = null;
        this.originalParent_ = null;
        this.fullscreenElement_ = null;
        this.fullscreenWrapper_ = null;
        this.fullscreenElementCachedStyle_ = null;
        this.fullscreenEventTarget_ = null;
        this.fullscreenChangeHandler_ = null;
        this.fullscreenErrorHandler_ = null;
        if (USE_WAKELOCK && isMobile3()) {
          this.wakelock_ = new NoSleep$1();
        }
      }
      VRDisplay.prototype.getFrameData = function(frameData) {
        return frameDataFromPose(frameData, this._getPose(), this);
      };
      VRDisplay.prototype.getPose = function() {
        deprecateWarning("VRDisplay.prototype.getPose", "VRDisplay.prototype.getFrameData");
        return this._getPose();
      };
      VRDisplay.prototype.resetPose = function() {
        deprecateWarning("VRDisplay.prototype.resetPose");
        return this._resetPose();
      };
      VRDisplay.prototype.getImmediatePose = function() {
        deprecateWarning("VRDisplay.prototype.getImmediatePose", "VRDisplay.prototype.getFrameData");
        return this._getPose();
      };
      VRDisplay.prototype.requestAnimationFrame = function(callback) {
        return raf(callback);
      };
      VRDisplay.prototype.cancelAnimationFrame = function(id2) {
        return caf(id2);
      };
      VRDisplay.prototype.wrapForFullscreen = function(element) {
        if (isIOS2()) {
          return element;
        }
        if (!this.fullscreenWrapper_) {
          this.fullscreenWrapper_ = document.createElement("div");
          var cssProperties = ["height: " + Math.min(screen.height, screen.width) + "px !important", "top: 0 !important", "left: 0 !important", "right: 0 !important", "border: 0", "margin: 0", "padding: 0", "z-index: 999999 !important", "position: fixed"];
          this.fullscreenWrapper_.setAttribute("style", cssProperties.join("; ") + ";");
          this.fullscreenWrapper_.classList.add("webvr-polyfill-fullscreen-wrapper");
        }
        if (this.fullscreenElement_ == element) {
          return this.fullscreenWrapper_;
        }
        if (this.fullscreenElement_) {
          if (this.originalParent_) {
            this.originalParent_.appendChild(this.fullscreenElement_);
          } else {
            this.fullscreenElement_.parentElement.removeChild(this.fullscreenElement_);
          }
        }
        this.fullscreenElement_ = element;
        this.originalParent_ = element.parentElement;
        if (!this.originalParent_) {
          document.body.appendChild(element);
        }
        if (!this.fullscreenWrapper_.parentElement) {
          var parent = this.fullscreenElement_.parentElement;
          parent.insertBefore(this.fullscreenWrapper_, this.fullscreenElement_);
          parent.removeChild(this.fullscreenElement_);
        }
        this.fullscreenWrapper_.insertBefore(this.fullscreenElement_, this.fullscreenWrapper_.firstChild);
        this.fullscreenElementCachedStyle_ = this.fullscreenElement_.getAttribute("style");
        var self2 = this;
        function applyFullscreenElementStyle() {
          if (!self2.fullscreenElement_) {
            return;
          }
          var cssProperties2 = ["position: absolute", "top: 0", "left: 0", "width: " + Math.max(screen.width, screen.height) + "px", "height: " + Math.min(screen.height, screen.width) + "px", "border: 0", "margin: 0", "padding: 0"];
          self2.fullscreenElement_.setAttribute("style", cssProperties2.join("; ") + ";");
        }
        applyFullscreenElementStyle();
        return this.fullscreenWrapper_;
      };
      VRDisplay.prototype.removeFullscreenWrapper = function() {
        if (!this.fullscreenElement_) {
          return;
        }
        var element = this.fullscreenElement_;
        if (this.fullscreenElementCachedStyle_) {
          element.setAttribute("style", this.fullscreenElementCachedStyle_);
        } else {
          element.removeAttribute("style");
        }
        this.fullscreenElement_ = null;
        this.fullscreenElementCachedStyle_ = null;
        var parent = this.fullscreenWrapper_.parentElement;
        this.fullscreenWrapper_.removeChild(element);
        if (this.originalParent_ === parent) {
          parent.insertBefore(element, this.fullscreenWrapper_);
        } else if (this.originalParent_) {
          this.originalParent_.appendChild(element);
        }
        parent.removeChild(this.fullscreenWrapper_);
        return element;
      };
      VRDisplay.prototype.requestPresent = function(layers) {
        var wasPresenting = this.isPresenting;
        var self2 = this;
        if (!(layers instanceof Array)) {
          deprecateWarning("VRDisplay.prototype.requestPresent with non-array argument", "an array of VRLayers as the first argument");
          layers = [layers];
        }
        return new Promise(function(resolve, reject) {
          if (!self2.capabilities.canPresent) {
            reject(new Error("VRDisplay is not capable of presenting."));
            return;
          }
          if (layers.length == 0 || layers.length > self2.capabilities.maxLayers) {
            reject(new Error("Invalid number of layers."));
            return;
          }
          var incomingLayer = layers[0];
          if (!incomingLayer.source) {
            resolve();
            return;
          }
          var leftBounds = incomingLayer.leftBounds || defaultLeftBounds;
          var rightBounds = incomingLayer.rightBounds || defaultRightBounds;
          if (wasPresenting) {
            var layer = self2.layer_;
            if (layer.source !== incomingLayer.source) {
              layer.source = incomingLayer.source;
            }
            for (var i = 0; i < 4; i++) {
              layer.leftBounds[i] = leftBounds[i];
              layer.rightBounds[i] = rightBounds[i];
            }
            self2.wrapForFullscreen(self2.layer_.source);
            self2.updatePresent_();
            resolve();
            return;
          }
          self2.layer_ = {
            predistorted: incomingLayer.predistorted,
            source: incomingLayer.source,
            leftBounds: leftBounds.slice(0),
            rightBounds: rightBounds.slice(0)
          };
          self2.waitingForPresent_ = false;
          if (self2.layer_ && self2.layer_.source) {
            var fullscreenElement = self2.wrapForFullscreen(self2.layer_.source);
            var onFullscreenChange = function onFullscreenChange2() {
              var actualFullscreenElement = getFullscreenElement();
              self2.isPresenting = fullscreenElement === actualFullscreenElement;
              if (self2.isPresenting) {
                if (screen.orientation && screen.orientation.lock) {
                  screen.orientation.lock("landscape-primary").catch(function(error) {
                    console.error("screen.orientation.lock() failed due to", error.message);
                  });
                }
                self2.waitingForPresent_ = false;
                self2.beginPresent_();
                resolve();
              } else {
                if (screen.orientation && screen.orientation.unlock) {
                  screen.orientation.unlock();
                }
                self2.removeFullscreenWrapper();
                self2.disableWakeLock();
                self2.endPresent_();
                self2.removeFullscreenListeners_();
              }
              self2.fireVRDisplayPresentChange_();
            };
            var onFullscreenError = function onFullscreenError2() {
              if (!self2.waitingForPresent_) {
                return;
              }
              self2.removeFullscreenWrapper();
              self2.removeFullscreenListeners_();
              self2.disableWakeLock();
              self2.waitingForPresent_ = false;
              self2.isPresenting = false;
              reject(new Error("Unable to present."));
            };
            self2.addFullscreenListeners_(fullscreenElement, onFullscreenChange, onFullscreenError);
            if (requestFullscreen(fullscreenElement)) {
              self2.enableWakeLock();
              self2.waitingForPresent_ = true;
            } else if (isIOS2() || isWebViewAndroid()) {
              self2.enableWakeLock();
              self2.isPresenting = true;
              self2.beginPresent_();
              self2.fireVRDisplayPresentChange_();
              resolve();
            }
          }
          if (!self2.waitingForPresent_ && !isIOS2()) {
            exitFullscreen();
            reject(new Error("Unable to present."));
          }
        });
      };
      VRDisplay.prototype.exitPresent = function() {
        var wasPresenting = this.isPresenting;
        var self2 = this;
        this.isPresenting = false;
        this.layer_ = null;
        this.disableWakeLock();
        return new Promise(function(resolve, reject) {
          if (wasPresenting) {
            if (!exitFullscreen() && isIOS2()) {
              self2.endPresent_();
              self2.fireVRDisplayPresentChange_();
            }
            if (isWebViewAndroid()) {
              self2.removeFullscreenWrapper();
              self2.removeFullscreenListeners_();
              self2.endPresent_();
              self2.fireVRDisplayPresentChange_();
            }
            resolve();
          } else {
            reject(new Error("Was not presenting to VRDisplay."));
          }
        });
      };
      VRDisplay.prototype.getLayers = function() {
        if (this.layer_) {
          return [this.layer_];
        }
        return [];
      };
      VRDisplay.prototype.fireVRDisplayPresentChange_ = function() {
        var event = new CustomEvent("vrdisplaypresentchange", { detail: { display: this } });
        window.dispatchEvent(event);
      };
      VRDisplay.prototype.fireVRDisplayConnect_ = function() {
        var event = new CustomEvent("vrdisplayconnect", { detail: { display: this } });
        window.dispatchEvent(event);
      };
      VRDisplay.prototype.addFullscreenListeners_ = function(element, changeHandler, errorHandler) {
        this.removeFullscreenListeners_();
        this.fullscreenEventTarget_ = element;
        this.fullscreenChangeHandler_ = changeHandler;
        this.fullscreenErrorHandler_ = errorHandler;
        if (changeHandler) {
          if (document.fullscreenEnabled) {
            element.addEventListener("fullscreenchange", changeHandler, false);
          } else if (document.webkitFullscreenEnabled) {
            element.addEventListener("webkitfullscreenchange", changeHandler, false);
          } else if (document.mozFullScreenEnabled) {
            document.addEventListener("mozfullscreenchange", changeHandler, false);
          } else if (document.msFullscreenEnabled) {
            element.addEventListener("msfullscreenchange", changeHandler, false);
          }
        }
        if (errorHandler) {
          if (document.fullscreenEnabled) {
            element.addEventListener("fullscreenerror", errorHandler, false);
          } else if (document.webkitFullscreenEnabled) {
            element.addEventListener("webkitfullscreenerror", errorHandler, false);
          } else if (document.mozFullScreenEnabled) {
            document.addEventListener("mozfullscreenerror", errorHandler, false);
          } else if (document.msFullscreenEnabled) {
            element.addEventListener("msfullscreenerror", errorHandler, false);
          }
        }
      };
      VRDisplay.prototype.removeFullscreenListeners_ = function() {
        if (!this.fullscreenEventTarget_)
          return;
        var element = this.fullscreenEventTarget_;
        if (this.fullscreenChangeHandler_) {
          var changeHandler = this.fullscreenChangeHandler_;
          element.removeEventListener("fullscreenchange", changeHandler, false);
          element.removeEventListener("webkitfullscreenchange", changeHandler, false);
          document.removeEventListener("mozfullscreenchange", changeHandler, false);
          element.removeEventListener("msfullscreenchange", changeHandler, false);
        }
        if (this.fullscreenErrorHandler_) {
          var errorHandler = this.fullscreenErrorHandler_;
          element.removeEventListener("fullscreenerror", errorHandler, false);
          element.removeEventListener("webkitfullscreenerror", errorHandler, false);
          document.removeEventListener("mozfullscreenerror", errorHandler, false);
          element.removeEventListener("msfullscreenerror", errorHandler, false);
        }
        this.fullscreenEventTarget_ = null;
        this.fullscreenChangeHandler_ = null;
        this.fullscreenErrorHandler_ = null;
      };
      VRDisplay.prototype.enableWakeLock = function() {
        if (this.wakelock_) {
          this.wakelock_.enable();
        }
      };
      VRDisplay.prototype.disableWakeLock = function() {
        if (this.wakelock_) {
          this.wakelock_.disable();
        }
      };
      VRDisplay.prototype.beginPresent_ = function() {
      };
      VRDisplay.prototype.endPresent_ = function() {
      };
      VRDisplay.prototype.submitFrame = function(pose) {
      };
      VRDisplay.prototype.getEyeParameters = function(whichEye) {
        return null;
      };
      var config = {
        ADDITIONAL_VIEWERS: [],
        DEFAULT_VIEWER: "",
        MOBILE_WAKE_LOCK: true,
        DEBUG: false,
        DPDB_URL: "https://dpdb.webvr.rocks/dpdb.json",
        K_FILTER: 0.98,
        PREDICTION_TIME_S: 0.04,
        CARDBOARD_UI_DISABLED: false,
        ROTATE_INSTRUCTIONS_DISABLED: false,
        YAW_ONLY: false,
        BUFFER_SCALE: 0.5,
        DIRTY_SUBMIT_FRAME_BINDINGS: false
      };
      var Eye = {
        LEFT: "left",
        RIGHT: "right"
      };
      function CardboardVRDisplay2(config$$1) {
        var defaults = extend({}, config);
        config$$1 = extend(defaults, config$$1 || {});
        VRDisplay.call(this, {
          wakelock: config$$1.MOBILE_WAKE_LOCK
        });
        this.config = config$$1;
        this.displayName = "Cardboard VRDisplay";
        this.capabilities = new VRDisplayCapabilities({
          hasPosition: false,
          hasOrientation: true,
          hasExternalDisplay: false,
          canPresent: true,
          maxLayers: 1
        });
        this.stageParameters = null;
        this.bufferScale_ = this.config.BUFFER_SCALE;
        this.poseSensor_ = new PoseSensor(this.config);
        this.distorter_ = null;
        this.cardboardUI_ = null;
        this.dpdb_ = new Dpdb(this.config.DPDB_URL, this.onDeviceParamsUpdated_.bind(this));
        this.deviceInfo_ = new DeviceInfo(this.dpdb_.getDeviceParams(), config$$1.ADDITIONAL_VIEWERS);
        this.viewerSelector_ = new ViewerSelector(config$$1.DEFAULT_VIEWER);
        this.viewerSelector_.onChange(this.onViewerChanged_.bind(this));
        this.deviceInfo_.setViewer(this.viewerSelector_.getCurrentViewer());
        if (!this.config.ROTATE_INSTRUCTIONS_DISABLED) {
          this.rotateInstructions_ = new RotateInstructions();
        }
        if (isIOS2()) {
          window.addEventListener("resize", this.onResize_.bind(this));
        }
      }
      CardboardVRDisplay2.prototype = Object.create(VRDisplay.prototype);
      CardboardVRDisplay2.prototype._getPose = function() {
        return {
          position: null,
          orientation: this.poseSensor_.getOrientation(),
          linearVelocity: null,
          linearAcceleration: null,
          angularVelocity: null,
          angularAcceleration: null
        };
      };
      CardboardVRDisplay2.prototype._resetPose = function() {
        if (this.poseSensor_.resetPose) {
          this.poseSensor_.resetPose();
        }
      };
      CardboardVRDisplay2.prototype._getFieldOfView = function(whichEye) {
        var fieldOfView;
        if (whichEye == Eye.LEFT) {
          fieldOfView = this.deviceInfo_.getFieldOfViewLeftEye();
        } else if (whichEye == Eye.RIGHT) {
          fieldOfView = this.deviceInfo_.getFieldOfViewRightEye();
        } else {
          console.error("Invalid eye provided: %s", whichEye);
          return null;
        }
        return fieldOfView;
      };
      CardboardVRDisplay2.prototype._getEyeOffset = function(whichEye) {
        var offset;
        if (whichEye == Eye.LEFT) {
          offset = [-this.deviceInfo_.viewer.interLensDistance * 0.5, 0, 0];
        } else if (whichEye == Eye.RIGHT) {
          offset = [this.deviceInfo_.viewer.interLensDistance * 0.5, 0, 0];
        } else {
          console.error("Invalid eye provided: %s", whichEye);
          return null;
        }
        return offset;
      };
      CardboardVRDisplay2.prototype.getEyeParameters = function(whichEye) {
        var offset = this._getEyeOffset(whichEye);
        var fieldOfView = this._getFieldOfView(whichEye);
        var eyeParams = {
          offset,
          renderWidth: this.deviceInfo_.device.width * 0.5 * this.bufferScale_,
          renderHeight: this.deviceInfo_.device.height * this.bufferScale_
        };
        Object.defineProperty(eyeParams, "fieldOfView", {
          enumerable: true,
          get: function get() {
            deprecateWarning("VRFieldOfView", "VRFrameData's projection matrices");
            return fieldOfView;
          }
        });
        return eyeParams;
      };
      CardboardVRDisplay2.prototype.onDeviceParamsUpdated_ = function(newParams) {
        if (this.config.DEBUG) {
          console.log("DPDB reported that device params were updated.");
        }
        this.deviceInfo_.updateDeviceParams(newParams);
        if (this.distorter_) {
          this.distorter_.updateDeviceInfo(this.deviceInfo_);
        }
      };
      CardboardVRDisplay2.prototype.updateBounds_ = function() {
        if (this.layer_ && this.distorter_ && (this.layer_.leftBounds || this.layer_.rightBounds)) {
          this.distorter_.setTextureBounds(this.layer_.leftBounds, this.layer_.rightBounds);
        }
      };
      CardboardVRDisplay2.prototype.beginPresent_ = function() {
        var gl = this.layer_.source.getContext("webgl");
        if (!gl)
          gl = this.layer_.source.getContext("experimental-webgl");
        if (!gl)
          gl = this.layer_.source.getContext("webgl2");
        if (!gl)
          return;
        if (this.layer_.predistorted) {
          if (!this.config.CARDBOARD_UI_DISABLED) {
            gl.canvas.width = getScreenWidth() * this.bufferScale_;
            gl.canvas.height = getScreenHeight() * this.bufferScale_;
            this.cardboardUI_ = new CardboardUI(gl);
          }
        } else {
          if (!this.config.CARDBOARD_UI_DISABLED) {
            this.cardboardUI_ = new CardboardUI(gl);
          }
          this.distorter_ = new CardboardDistorter(gl, this.cardboardUI_, this.config.BUFFER_SCALE, this.config.DIRTY_SUBMIT_FRAME_BINDINGS);
          this.distorter_.updateDeviceInfo(this.deviceInfo_);
        }
        if (this.cardboardUI_) {
          this.cardboardUI_.listen(function(e2) {
            this.viewerSelector_.show(this.layer_.source.parentElement);
            e2.stopPropagation();
            e2.preventDefault();
          }.bind(this), function(e2) {
            this.exitPresent();
            e2.stopPropagation();
            e2.preventDefault();
          }.bind(this));
        }
        if (this.rotateInstructions_) {
          if (isLandscapeMode() && isMobile3()) {
            this.rotateInstructions_.showTemporarily(3e3, this.layer_.source.parentElement);
          } else {
            this.rotateInstructions_.update();
          }
        }
        this.orientationHandler = this.onOrientationChange_.bind(this);
        window.addEventListener("orientationchange", this.orientationHandler);
        this.vrdisplaypresentchangeHandler = this.updateBounds_.bind(this);
        window.addEventListener("vrdisplaypresentchange", this.vrdisplaypresentchangeHandler);
        this.fireVRDisplayDeviceParamsChange_();
      };
      CardboardVRDisplay2.prototype.endPresent_ = function() {
        if (this.distorter_) {
          this.distorter_.destroy();
          this.distorter_ = null;
        }
        if (this.cardboardUI_) {
          this.cardboardUI_.destroy();
          this.cardboardUI_ = null;
        }
        if (this.rotateInstructions_) {
          this.rotateInstructions_.hide();
        }
        this.viewerSelector_.hide();
        window.removeEventListener("orientationchange", this.orientationHandler);
        window.removeEventListener("vrdisplaypresentchange", this.vrdisplaypresentchangeHandler);
      };
      CardboardVRDisplay2.prototype.updatePresent_ = function() {
        this.endPresent_();
        this.beginPresent_();
      };
      CardboardVRDisplay2.prototype.submitFrame = function(pose) {
        if (this.distorter_) {
          this.updateBounds_();
          this.distorter_.submitFrame();
        } else if (this.cardboardUI_ && this.layer_) {
          var gl = this.layer_.source.getContext("webgl");
          if (!gl)
            gl = this.layer_.source.getContext("experimental-webgl");
          if (!gl)
            gl = this.layer_.source.getContext("webgl2");
          var canvas = gl.canvas;
          if (canvas.width != this.lastWidth || canvas.height != this.lastHeight) {
            this.cardboardUI_.onResize();
          }
          this.lastWidth = canvas.width;
          this.lastHeight = canvas.height;
          this.cardboardUI_.render();
        }
      };
      CardboardVRDisplay2.prototype.onOrientationChange_ = function(e2) {
        this.viewerSelector_.hide();
        if (this.rotateInstructions_) {
          this.rotateInstructions_.update();
        }
        this.onResize_();
      };
      CardboardVRDisplay2.prototype.onResize_ = function(e2) {
        if (this.layer_) {
          var gl = this.layer_.source.getContext("webgl");
          if (!gl)
            gl = this.layer_.source.getContext("experimental-webgl");
          if (!gl)
            gl = this.layer_.source.getContext("webgl2");
          var cssProperties = [
            "position: absolute",
            "top: 0",
            "left: 0",
            "width: 100vw",
            "height: 100vh",
            "border: 0",
            "margin: 0",
            "padding: 0px",
            "box-sizing: content-box"
          ];
          gl.canvas.setAttribute("style", cssProperties.join("; ") + ";");
          safariCssSizeWorkaround(gl.canvas);
        }
      };
      CardboardVRDisplay2.prototype.onViewerChanged_ = function(viewer) {
        this.deviceInfo_.setViewer(viewer);
        if (this.distorter_) {
          this.distorter_.updateDeviceInfo(this.deviceInfo_);
        }
        this.fireVRDisplayDeviceParamsChange_();
      };
      CardboardVRDisplay2.prototype.fireVRDisplayDeviceParamsChange_ = function() {
        var event = new CustomEvent("vrdisplaydeviceparamschange", {
          detail: {
            vrdisplay: this,
            deviceInfo: this.deviceInfo_
          }
        });
        window.dispatchEvent(event);
      };
      CardboardVRDisplay2.VRFrameData = VRFrameData;
      CardboardVRDisplay2.VRDisplay = VRDisplay;
      return CardboardVRDisplay2;
    });
  }
});

// ../tslib/collections/arrayBinarySearch.ts
function defaultKeySelector(obj2) {
  return obj2;
}
function arrayBinarySearchByKey(arr, itemKey, keySelector) {
  let left2 = 0;
  let right = arr.length;
  let idx = Math.floor((left2 + right) / 2);
  let found = false;
  while (left2 < right && idx < arr.length) {
    const compareTo = arr[idx];
    const compareToKey = isNullOrUndefined(compareTo) ? null : keySelector(compareTo);
    if (isDefined(compareToKey) && itemKey < compareToKey) {
      right = idx;
    } else {
      if (itemKey === compareToKey) {
        found = true;
      }
      left2 = idx + 1;
    }
    idx = Math.floor((left2 + right) / 2);
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
function arrayRemove(arr, value2) {
  const idx = arr.indexOf(value2);
  if (idx > -1) {
    arrayRemoveAt(arr, idx);
    return true;
  }
  return false;
}

// ../tslib/collections/arrayReplace.ts
function arrayReplace(arr, ...items) {
  arr.splice(0, arr.length, ...items);
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
function arraySortByKey(arr, keySelector) {
  const newArr = Array.from(arr);
  arraySortByKeyInPlace(newArr, keySelector);
  return newArr;
}
function arraySortByKeyInPlace(newArr, keySelector) {
  newArr.sort((a, b) => {
    const keyA = keySelector(a);
    const keyB = keySelector(b);
    if (keyA < keyB) {
      return -1;
    } else if (keyA > keyB) {
      return 1;
    } else {
      return 0;
    }
  });
}

// ../tslib/collections/BaseGraphNode.ts
var BaseGraphNode = class {
  constructor(value2) {
    this.value = value2;
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
    const nodes2 = /* @__PURE__ */ new Set();
    const queue = [this];
    while (queue.length > 0) {
      const here = queue.shift();
      if (isDefined(here) && !nodes2.has(here)) {
        nodes2.add(here);
        queue.push(...here._forward);
      }
    }
    return Array.from(nodes2);
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

// ../tslib/collections/mapMap.ts
function mapMap(items, makeID, makeValue) {
  return new Map(items.map((item) => [makeID(item), makeValue(item)]));
}

// ../tslib/collections/makeLookup.ts
function makeLookup(items, makeID) {
  return mapMap(items, makeID, identity);
}

// ../tslib/collections/PriorityList.ts
var PriorityList = class {
  constructor(init) {
    this.items = /* @__PURE__ */ new Map();
    this.defaultItems = new Array();
    if (isDefined(init)) {
      for (const [key, value2] of init) {
        this.add(key, value2);
      }
    }
  }
  add(key, value2) {
    if (isNullOrUndefined(key)) {
      this.defaultItems.push(value2);
    } else {
      let list = this.items.get(key);
      if (isNullOrUndefined(list)) {
        this.items.set(key, list = []);
      }
      list.push(value2);
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
  remove(key, value2) {
    if (isNullOrUndefined(key)) {
      arrayRemove(this.defaultItems, value2);
    } else {
      const list = this.items.get(key);
      if (isDefined(list)) {
        arrayRemove(list, value2);
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

// ../tslib/collections/PriorityMap.ts
var PriorityMap = class {
  constructor(init) {
    this.items = /* @__PURE__ */ new Map();
    if (isDefined(init)) {
      for (const [key1, key2, value2] of init) {
        this.add(key1, key2, value2);
      }
    }
  }
  add(key1, key2, value2) {
    let level1 = this.items.get(key1);
    if (isNullOrUndefined(level1)) {
      this.items.set(key1, level1 = /* @__PURE__ */ new Map());
    }
    level1.set(key2, value2);
    return this;
  }
  *entries() {
    for (const [key1, level1] of this.items) {
      for (const [key2, value2] of level1) {
        yield [key1, key2, value2];
      }
    }
  }
  keys(key1) {
    if (isNullOrUndefined(key1)) {
      return this.items.keys();
    } else {
      return this.items.get(key1).keys();
    }
  }
  *values() {
    for (const level1 of this.items.values()) {
      for (const value2 of level1.values()) {
        yield value2;
      }
    }
  }
  has(key1, key2) {
    return this.items.has(key1) && (isNullOrUndefined(key2) || this.items.get(key1).has(key2));
  }
  get(key1, key2) {
    if (isNullOrUndefined(key2)) {
      return this.items.get(key1);
    } else if (this.items.has(key1)) {
      return this.items.get(key1).get(key2);
    } else {
      return null;
    }
  }
  count(key1) {
    if (this.items.has(key1)) {
      return this.items.get(key1).size;
    }
    return null;
  }
  get size() {
    let size = 0;
    for (const list of this.items.values()) {
      size += list.size;
    }
    return size;
  }
  delete(key1, key2) {
    if (isNullOrUndefined(key2)) {
      return this.items.delete(key1);
    } else if (this.items.has(key1)) {
      return this.items.get(key1).delete(key2);
    } else {
      return false;
    }
  }
  clear() {
    this.items.clear();
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
    this.resolve = (value2) => {
      if (isDefined(this._resolve)) {
        this._resolve(value2);
      }
    };
    this.reject = (reason) => {
      if (isDefined(this._reject)) {
        this._reject(reason);
      }
    };
    this.promise = new Promise((resolve, reject) => {
      this._resolve = (value2) => {
        if (resolveTest(value2)) {
          this._result = value2;
          this._finished = true;
          resolve(value2);
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

// ../tslib/events/once.ts
function targetValidateEvent(target, type2) {
  return "on" + type2 in target;
}
function once(target, resolveEvt, rejectEvtOrTimeout, ...rejectEvts) {
  if (isNullOrUndefined(rejectEvts)) {
    rejectEvts = [];
  }
  let timeout = void 0;
  if (isString(rejectEvtOrTimeout)) {
    rejectEvts.unshift(rejectEvtOrTimeout);
  } else if (isNumber(rejectEvtOrTimeout)) {
    timeout = rejectEvtOrTimeout;
  }
  if (!(target instanceof EventBase)) {
    if (!targetValidateEvent(target, resolveEvt)) {
      throw new Exception(`Target does not have a ${resolveEvt} rejection event`);
    }
    for (const evt of rejectEvts) {
      if (!targetValidateEvent(target, evt)) {
        throw new Exception(`Target does not have a ${evt} rejection event`);
      }
    }
  }
  const task = new Task();
  if (isNumber(timeout)) {
    const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
    task.finally(clearTimeout.bind(globalThis, timeoutHandle));
  }
  const register = (evt, callback) => {
    target.addEventListener(evt, callback);
    task.finally(() => target.removeEventListener(evt, callback));
  };
  const onResolve = (evt) => task.resolve(evt);
  const onReject = (evt) => task.reject(evt);
  register(resolveEvt, onResolve);
  for (const rejectEvt of rejectEvts) {
    register(rejectEvt, onReject);
  }
  return task;
}
function success(task) {
  return task.then(alwaysTrue).catch(alwaysFalse);
}

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

// ../tslib/Exception.ts
var Exception = class extends Error {
  constructor(message, innerError = null) {
    super(message);
    this.innerError = innerError;
  }
};

// ../tslib/flags.ts
function isChrome() {
  return "chrome" in globalThis && !navigator.userAgent.match("CriOS");
}
function isFirefox() {
  return "InstallTrigger" in globalThis;
}
function isMacOS() {
  return /^mac/i.test(navigator.platform);
}
function isIOS() {
  return /iP(ad|hone|od)/.test(navigator.platform) || /Macintosh(.*?) FxiOS(.*?)\//.test(navigator.platform) || isMacOS() && "maxTouchPoints" in navigator && navigator.maxTouchPoints > 2;
}
function isMobileVR() {
  return /Mobile VR/.test(navigator.userAgent) || /Pico Neo 3 Link/.test(navigator.userAgent) || isOculusBrowser;
}
function hasWebXR() {
  return "xr" in navigator && "isSessionSupported" in navigator.xr;
}
function hasWebVR() {
  return "getVRDisplays" in navigator;
}
function hasVR() {
  return hasWebXR() || hasWebVR();
}
function isMobile() {
  return /Android/.test(navigator.userAgent) || /BlackBerry/.test(navigator.userAgent) || /(UC Browser |UCWEB)/.test(navigator.userAgent) || isIOS() || isMobileVR();
}
function isDesktop() {
  return !isMobile();
}
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
function scaleAndAdd(out, a, b, scale4) {
  out[0] = a[0] + b[0] * scale4;
  out[1] = a[1] + b[1] * scale4;
  out[2] = a[2] + b[2] * scale4;
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
  var len3 = x * x + y * y + z * z;
  if (len3 > 0) {
    len3 = 1 / Math.sqrt(len3);
  }
  out[0] = a[0] * len3;
  out[1] = a[1] * len3;
  out[2] = a[2] * len3;
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
function random(out, scale4) {
  scale4 = scale4 || 1;
  var r = RANDOM() * 2 * Math.PI;
  var z = RANDOM() * 2 - 1;
  var zScale = Math.sqrt(1 - z * z) * scale4;
  out[0] = Math.cos(r) * zScale;
  out[1] = Math.sin(r) * zScale;
  out[2] = z * scale4;
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
  return function(a, stride, offset, count2, fn, arg) {
    var i, l;
    if (!stride) {
      stride = 3;
    }
    if (!offset) {
      offset = 0;
    }
    if (count2) {
      l = Math.min(count2 * stride + offset, a.length);
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
function identity(item) {
  return item;
}
function alwaysTrue() {
  return true;
}
function alwaysFalse() {
  return false;
}
function and(a, b) {
  return a && b;
}

// ../tslib/math/angleClamp.ts
var Tau = 2 * Math.PI;
function angleClamp(v) {
  return (v % Tau + Tau) % Tau;
}

// ../tslib/math/clamp.ts
function clamp(v, min3, max3) {
  return Math.min(max3, Math.max(min3, v));
}

// ../tslib/math/deg2rad.ts
function deg2rad(deg) {
  return deg * Math.PI / 180;
}

// ../tslib/math/project.ts
function project(v, min3, max3) {
  const delta3 = max3 - min3;
  if (delta3 === 0) {
    return 0;
  } else {
    return (v - min3) / delta3;
  }
}

// ../tslib/math/unproject.ts
function unproject(v, min3, max3) {
  return v * (max3 - min3) + min3;
}

// ../tslib/math/lerp.ts
function lerp2(a, b, p) {
  return (1 - p) * a + p * b;
}

// ../tslib/math/powerOf2.ts
function nextPowerOf2(v) {
  return Math.pow(2, Math.ceil(Math.log2(v)));
}

// ../tslib/math/rad2deg.ts
function rad2deg(rad) {
  return rad * 180 / Math.PI;
}

// ../tslib/math/truncate.ts
function truncate(v) {
  if (Math.abs(v) > 1e-4) {
    return v;
  }
  return 0;
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
      const end2 = performance.now();
      const delta3 = end2 - this.start;
      const est = this.start - end2 + delta3 * this.weightTotal / soFar;
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

// ../tslib/progress/progressPopper.ts
function progressPopper(progress) {
  return new PoppableParentProgressCallback(progress);
}
var PoppableParentProgressCallback = class extends BaseParentProgressCallback {
  pop(weight) {
    return this.addSubProgress(weight);
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
function isString(obj2) {
  return t(obj2, "string", String);
}
function isBoolean(obj2) {
  return t(obj2, "boolean", Boolean);
}
function isNumber(obj2) {
  return t(obj2, "number", Number);
}
function isGoodNumber(obj2) {
  return isNumber(obj2) && !Number.isNaN(obj2);
}
function isObject(obj2) {
  return isDefined(obj2) && t(obj2, "object", Object);
}
function isDate(obj2) {
  return obj2 instanceof Date;
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

// ../tslib/singleton.ts
function singleton(name2, create8) {
  const box = globalThis;
  let value2 = box[name2];
  if (isNullOrUndefined(value2)) {
    if (isNullOrUndefined(create8)) {
      throw new Error(`No value ${name2} found`);
    }
    value2 = create8();
    box[name2] = value2;
  }
  return value2;
}

// ../tslib/strings/stringRandom.ts
var DEFAULT_CHAR_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZ";
function stringRandom(length4, charSet) {
  if (length4 < 0) {
    throw new Error("Length must be greater than 0");
  }
  if (isNullOrUndefined(charSet)) {
    charSet = DEFAULT_CHAR_SET;
  }
  let str2 = "";
  for (let i = 0; i < length4; ++i) {
    const idx = Math.floor(Math.random() * charSet.length);
    str2 += charSet[idx];
  }
  return str2;
}

// ../tslib/strings/stringToName.ts
function stringToName(...parts) {
  const goodParts = [];
  for (const part of parts) {
    if (isDefined(part) && part.length > 0 && goodParts.indexOf(part) === -1) {
      goodParts.push(part);
    }
  }
  return goodParts.join("-");
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

// ../tslib/timers/SetTimeoutTimer.ts
var SetTimeoutTimer = class extends BaseTimer {
  constructor(targetFrameRate) {
    super(targetFrameRate);
  }
  start() {
    const updater = () => {
      this.timer = setTimeout(updater, this.targetFrameTime);
      this.onTick(performance.now());
    };
    this.timer = setTimeout(updater, this.targetFrameTime);
  }
  stop() {
    if (this.isRunning) {
      clearTimeout(this.timer);
      super.stop();
    }
  }
};

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
  query(name2, value2) {
    this._query.set(name2, value2);
    if (this.refresh()) {
      this._url.searchParams.set(name2, value2);
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

// ../tslib/collections/mapInvert.ts
function mapInvert(map) {
  const mapOut = /* @__PURE__ */ new Map();
  for (const [key, value2] of map) {
    mapOut.set(value2, key);
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
function feet2Meters(feet) {
  return feet / FEET_PER_METER;
}

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

// ../dom/attrs.ts
var Attr = class {
  constructor(key, value2, bySetAttribute, ...tags) {
    this.key = key;
    this.value = value2;
    this.bySetAttribute = bySetAttribute;
    this.tags = tags.map((t2) => t2.toLocaleUpperCase());
    Object.freeze(this);
  }
  tags;
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
function autoPlay(value2) {
  return new Attr("autoplay", value2, false, "audio", "video");
}
function className(value2) {
  return new Attr("className", value2, false);
}
function classList(...values) {
  return new Attr("classList", values, false);
}
function controls(value2) {
  return new Attr("controls", value2, false, "audio", "video");
}
function customData(name2, value2) {
  return new Attr("data-" + name2, value2, false);
}
function htmlHeight(value2) {
  return new Attr("height", value2, false, "canvas", "embed", "iframe", "img", "input", "object", "video");
}
function id(value2) {
  return new Attr("id", value2, false);
}
function loop(value2) {
  return new Attr("loop", value2, false, "audio", "bgsound", "marquee", "video");
}
function max2(value2) {
  return new Attr("max", value2, false, "input", "meter", "progress");
}
function min2(value2) {
  return new Attr("min", value2, false, "input", "meter");
}
function muted(value2) {
  return new Attr("muted", value2, false, "audio", "video");
}
function playsInline(value2) {
  return new Attr("playsInline", value2, false, "audio", "video");
}
function unpackURL(value2) {
  if (value2 instanceof URL) {
    value2 = value2.href;
  }
  return value2;
}
function src(value2) {
  return new Attr("src", unpackURL(value2), false, "audio", "embed", "iframe", "img", "input", "script", "source", "track", "video");
}
function srcObject(value2) {
  return new Attr("srcObject", value2, false, "audio", "video");
}
function step(value2) {
  return new Attr("step", value2, false, "input");
}
function title(value2) {
  return new Attr("title", value2, false);
}
function type(value2) {
  if (!isString(value2)) {
    value2 = value2.value;
  }
  return new Attr("type", value2, false, "button", "input", "command", "embed", "link", "object", "script", "source", "style", "menu");
}
function value(value2) {
  return new Attr("value", value2, false, "button", "data", "input", "li", "meter", "option", "progress", "param");
}
function htmlWidth(value2) {
  return new Attr("width", value2, false, "canvas", "embed", "iframe", "img", "input", "object", "video");
}

// ../dom/css.ts
var CssProp = class {
  constructor(key, value2) {
    this.key = key;
    this.value = value2;
    this.name = key.replace(/[A-Z]/g, (m) => `-${m.toLocaleLowerCase()}`);
  }
  name;
  applyToElement(elem) {
    elem.style[this.key] = this.value;
  }
};
var CssPropSet = class {
  rest;
  constructor(...rest) {
    this.rest = rest;
  }
  applyToElement(elem) {
    for (const prop of this.rest) {
      prop.applyToElement(elem);
    }
  }
};
function styles(...rest) {
  return new CssPropSet(...rest);
}
function backgroundColor(v) {
  return new CssProp("backgroundColor", v);
}
function border(v) {
  return new CssProp("border", v);
}
function borderRadius(v) {
  return new CssProp("borderRadius", v);
}
function boxShadow(v) {
  return new CssProp("boxShadow", v);
}
function columnGap(v) {
  return new CssProp("columnGap", v);
}
function display(v) {
  return new CssProp("display", v);
}
function flexFlow(v) {
  return new CssProp("flexFlow", v);
}
function float(v) {
  return new CssProp("float", v);
}
function fontSize(v) {
  return new CssProp("fontSize", v);
}
function gridArea(vOrRowStart, colStart, rowEnd, colEnd) {
  if (!isString(vOrRowStart)) {
    vOrRowStart = [vOrRowStart, colStart, rowEnd, colEnd].filter(isDefined).join("/");
  }
  return new CssProp("gridArea", vOrRowStart);
}
function gridAutoFlow(v) {
  return new CssProp("gridAutoFlow", v);
}
function gridColumn(vOrColStart, colEnd) {
  if (!isString(vOrColStart)) {
    vOrColStart = [vOrColStart, colEnd].filter(isDefined).join("/");
  }
  return new CssProp("gridColumn", vOrColStart);
}
function gridRow(vOrRowStart, rowEnd) {
  if (!isString(vOrRowStart)) {
    vOrRowStart = [vOrRowStart, rowEnd].filter(isDefined).join("/");
  }
  return new CssProp("gridRow", vOrRowStart);
}
function gridTemplateColumns(v) {
  return new CssProp("gridTemplateColumns", v);
}
function gridTemplateRows(v) {
  return new CssProp("gridTemplateRows", v);
}
function height(v) {
  return new CssProp("height", v);
}
function left(v) {
  return new CssProp("left", v);
}
function margin(v) {
  return new CssProp("margin", v);
}
function marginInlineStart(v) {
  return new CssProp("marginInlineStart", v);
}
function marginLeft(v) {
  return new CssProp("marginLeft", v);
}
function maxHeight(v) {
  return new CssProp("maxHeight", v);
}
function maxWidth(v) {
  return new CssProp("maxWidth", v);
}
function minWidth(v) {
  return new CssProp("minWidth", v);
}
function overflow(v) {
  return new CssProp("overflow", v);
}
function padding(...v) {
  return new CssProp("padding", v.join(" "));
}
function paddingRight(v) {
  return new CssProp("paddingRight", v);
}
function pointerEvents(v) {
  return new CssProp("pointerEvents", v);
}
function position(v) {
  return new CssProp("position", v);
}
function textAlign(v) {
  return new CssProp("textAlign", v);
}
function top(v) {
  return new CssProp("top", v);
}
function touchAction(v) {
  return new CssProp("touchAction", v);
}
function transform(v) {
  return new CssProp("transform", v);
}
function width(v) {
  return new CssProp("width", v);
}
function zIndex(v) {
  return new CssProp("zIndex", isNumber(v) ? v.toFixed(0) : v);
}
function getMonospaceFonts() {
  return "ui-monospace, 'Droid Sans Mono', 'Cascadia Mono', 'Segoe UI Mono', 'Ubuntu Mono', 'Roboto Mono', Menlo, Monaco, Consolas, monospace";
}
var CSSInJSRule = class {
  constructor(selector, props) {
    this.selector = selector;
    this.props = props;
  }
  apply(sheet) {
    const style = this.props.map((prop) => `${prop.name}: ${prop.value};`).join("");
    sheet.insertRule(`${this.selector} {${style}}`, sheet.cssRules.length);
  }
};
function rule(selector, ...props) {
  return new CSSInJSRule(selector, props);
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
function elementSetDisplay(elem, visible, visibleDisplayType = "block") {
  elem = resolveElement(elem);
  elem.style.display = visible ? visibleDisplayType : "none";
}
function elementIsDisplayed(elem) {
  elem = resolveElement(elem);
  return elem.style.display !== "none";
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
function tag(name2, ...rest) {
  let elem = null;
  for (const attr of rest) {
    if (attr instanceof Attr && attr.key === "id") {
      elem = document.getElementById(attr.value);
      break;
    }
  }
  if (elem == null) {
    elem = document.createElement(name2);
  }
  elementApply(elem, ...rest);
  return elem;
}
function isDisableable(obj2) {
  return "disabled" in obj2 && isBoolean(obj2.disabled);
}
function elementClearChildren(elem) {
  elem = resolveElement(elem);
  while (elem.lastChild) {
    elem.lastChild.remove();
  }
}
function elementSetText(elem, text) {
  elem = resolveElement(elem);
  elementClearChildren(elem);
  elem.append(TextNode(text));
}
async function mediaElementCan(type2, elem, prog) {
  if (isDefined(prog)) {
    prog.start();
  }
  const expectedState = type2 === "canplay" ? elem.HAVE_CURRENT_DATA : elem.HAVE_ENOUGH_DATA;
  if (elem.readyState >= expectedState) {
    return true;
  }
  try {
    await once(elem, type2, "error");
    return true;
  } catch (err) {
    console.warn(elem.error, err);
    return false;
  } finally {
    if (isDefined(prog)) {
      prog.end();
    }
  }
}
function mediaElementCanPlay(elem, prog) {
  return mediaElementCan("canplay", elem, prog);
}
function mediaElementCanPlayThrough(elem, prog) {
  return mediaElementCan("canplaythrough", elem, prog);
}
function Audio(...rest) {
  return tag("audio", ...rest);
}
function ButtonRaw(...rest) {
  return tag("button", ...rest);
}
function Button(...rest) {
  return ButtonRaw(...rest, type("button"));
}
function ButtonPrimary(...rest) {
  return Button(...rest, classList("btn", "btn-primary"));
}
function ButtonSecondary(...rest) {
  return Button(...rest, classList("btn", "btn-secondary"));
}
function Canvas(...rest) {
  return tag("canvas", ...rest);
}
function DD(...rest) {
  return tag("dd", ...rest);
}
function Div(...rest) {
  return tag("div", ...rest);
}
function DL(...rest) {
  return tag("dl", ...rest);
}
function DT(...rest) {
  return tag("dt", ...rest);
}
function H1(...rest) {
  return tag("h1", ...rest);
}
function H2(...rest) {
  return tag("h2", ...rest);
}
function Img(...rest) {
  return tag("img", ...rest);
}
function Input(...rest) {
  return tag("input", ...rest);
}
function Label(...rest) {
  return tag("label", ...rest);
}
function Meter(...rest) {
  return tag("meter", ...rest);
}
function Option(...rest) {
  return tag("option", ...rest);
}
function Select(...rest) {
  return tag("select", ...rest);
}
function Video(...rest) {
  return tag("video", ...rest);
}
function InputCheckbox(...rest) {
  return Input(type("checkbox"), ...rest);
}
function InputNumber(...rest) {
  return Input(type("number"), ...rest);
}
function InputRange(...rest) {
  return Input(type("range"), ...rest);
}
function TextNode(txt) {
  return document.createTextNode(txt);
}
function Style(...rest) {
  let elem = document.createElement("style");
  document.head.append(elem);
  for (let x of rest) {
    x.apply(elem.sheet);
  }
  return elem;
}
function BackgroundAudio(autoplay, mute, looping, ...rest) {
  return Audio(playsInline(true), controls(false), muted(mute), autoPlay(autoplay), loop(looping), styles(display("none")), ...rest);
}

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
function createOffscreenCanvas(width2, height2) {
  return new OffscreenCanvas(width2, height2);
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

// ../graphics2d/CanvasImage.ts
var CanvasImage = class extends TypedEventBase {
  constructor(width2, height2, options) {
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
    this._canvas = createUICanvas(width2, height2);
    this._g = this.canvas.getContext("2d");
    if (isHTMLCanvas(this._canvas)) {
      this.element = this._canvas;
    }
  }
  fillRect(color, x, y, width2, height2, margin2) {
    this.g.fillStyle = color;
    this.g.fillRect(x + margin2, y + margin2, width2 - 2 * margin2, height2 - 2 * margin2);
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

// ../graphics2d/ArtificialHorizon.ts
var ArtificialHorizon = class extends CanvasImage {
  _pitch = 0;
  _heading = 0;
  constructor() {
    super(128, 128);
    this.redraw();
  }
  get pitch() {
    return this._pitch;
  }
  set pitch(v) {
    if (v !== this.pitch) {
      this._pitch = v;
      this.redraw();
    }
  }
  get heading() {
    return this._heading;
  }
  set heading(v) {
    if (v !== this.heading) {
      this._heading = v;
      this.redraw();
    }
  }
  setPitchAndHeading(pitch, heading) {
    if (pitch !== this.pitch || heading !== this.heading) {
      this._pitch = pitch;
      this._heading = heading;
      this.redraw();
    }
  }
  onRedraw() {
    const a = deg2rad(this.pitch);
    const b = deg2rad(this.heading - 180);
    const p = 5;
    const w = this.canvas.width - 2 * p;
    const h = this.canvas.height - 2 * p;
    const hw = 0.5 * w;
    const hh = 0.5 * h;
    const y = Math.sin(a);
    const g = this.g;
    g.save();
    {
      g.clearRect(0, 0, this.canvas.width, this.canvas.height);
      g.translate(p, p);
      g.scale(hw, hh);
      g.translate(1, 1);
      g.fillStyle = "#808080";
      g.beginPath();
      g.arc(0, 0, 1, 0, 2 * Math.PI);
      g.fill();
      g.fillStyle = "#d0d0d0";
      g.beginPath();
      g.arc(0, 0, 1, 0, Math.PI, true);
      g.fill();
      g.save();
      {
        g.scale(1, Math.abs(y));
        if (y < 0) {
          g.fillStyle = "#808080";
        }
        g.beginPath();
        g.arc(0, 0, 1, 0, Math.PI, y < 0);
        g.fill();
      }
      g.restore();
      g.save();
      {
        g.shadowColor = "#404040";
        g.shadowBlur = 4;
        g.shadowOffsetX = 3;
        g.shadowOffsetY = 3;
        g.rotate(b);
        g.fillStyle = "#ff0000";
        g.beginPath();
        g.moveTo(-0.1, 0);
        g.lineTo(0, 0.667);
        g.lineTo(0.1, 0);
        g.closePath();
        g.fill();
        g.fillStyle = "#ffffff";
        g.beginPath();
        g.moveTo(-0.1, 0);
        g.lineTo(0, -0.667);
        g.lineTo(0.1, 0);
        g.closePath();
        g.fill();
      }
      g.restore();
      g.beginPath();
      g.strokeStyle = "#000000";
      g.lineWidth = 0.1;
      g.arc(0, 0, 1, 0, 2 * Math.PI);
      g.stroke();
    }
    g.restore();
    return true;
  }
};

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

// ../graphics2d/BatteryImage.ts
function isBatteryNavigator(nav) {
  return "getBattery" in nav;
}
var chargeLabels = [
  "",
  "N/A",
  "charging"
];
var BatteryImage = class extends CanvasImage {
  battery = null;
  lastChargeDirection = null;
  lastLevel = null;
  chargeDirection = 0;
  level = 0.5;
  constructor() {
    super(256, 128);
    if (isBatteryNavigator(navigator)) {
      this.readBattery(navigator);
    } else {
      this.redraw();
    }
  }
  onRedraw() {
    if (this.battery) {
      this.chargeDirection = this.battery.charging ? 1 : -1;
      this.level = this.battery.level;
    } else {
      this.level += 0.1;
      if (this.level > 1) {
        this.level = 0;
      }
    }
    const directionChanged = this.chargeDirection !== this.lastChargeDirection;
    const levelChanged = this.level !== this.lastLevel;
    if (!directionChanged && !levelChanged) {
      return false;
    }
    this.lastChargeDirection = this.chargeDirection;
    this.lastLevel = this.level;
    const levelColor = this.level < 0.1 ? "red" : "#ccc";
    const padding2 = 7;
    const scale4 = 0.7;
    const invScale = (1 - scale4) / 2;
    const bodyWidth = this.canvas.width - 2 * padding2;
    const width2 = bodyWidth - 4 * padding2;
    const height2 = this.canvas.height - 4 * padding2;
    const midX = bodyWidth / 2;
    const midY = this.canvas.height / 2;
    const label = chargeLabels[this.chargeDirection + 1];
    this.g.clearRect(0, 0, bodyWidth, this.canvas.height);
    this.g.save();
    this.g.translate(invScale * this.canvas.width, invScale * this.canvas.height);
    this.g.globalAlpha = 0.75;
    this.g.scale(scale4, scale4);
    this.fillRect("#ccc", 0, 0, bodyWidth, this.canvas.height, 0);
    this.fillRect("#ccc", bodyWidth, midY - 2 * padding2 - 10, padding2 + 10, 4 * padding2 + 20, 0);
    this.g.clearRect(padding2, padding2, bodyWidth - 2 * padding2, this.canvas.height - 2 * padding2);
    this.fillRect("black", padding2, padding2, bodyWidth - 2 * padding2, this.canvas.height - 2 * padding2, 0);
    this.g.clearRect(2 * padding2, 2 * padding2, width2 * this.level, height2);
    this.fillRect(levelColor, 2 * padding2, 2 * padding2, width2 * this.level, height2, 0);
    this.g.fillStyle = "white";
    this.g.strokeStyle = "black";
    this.g.lineWidth = 4;
    this.g.textBaseline = "middle";
    this.g.font = makeFont({
      fontSize: height2 / 2,
      fontFamily: "Lato"
    });
    this.drawText(label, midX, midY, "center");
    this.g.restore();
    return true;
  }
  async readBattery(navigator2) {
    const redraw = this.redraw.bind(this);
    redraw();
    this.battery = await navigator2.getBattery();
    this.battery.addEventListener("chargingchange", redraw);
    this.battery.addEventListener("levelchange", redraw);
    setInterval(redraw, 1e3);
    redraw();
  }
};
__publicField(BatteryImage, "isAvailable", isBatteryNavigator(navigator));

// ../graphics2d/TextImage.ts
var TextImage = class extends CanvasImage {
  constructor(options) {
    super(10, 10, options);
    this.trueWidth = null;
    this.trueHeight = null;
    this.trueFontSize = null;
    this.dx = null;
    this._minWidth = null;
    this._maxWidth = null;
    this._minHeight = null;
    this._maxHeight = null;
    this._freezeDimensions = false;
    this._dimensionsFrozen = false;
    this._bgFillColor = null;
    this._bgStrokeColor = null;
    this._bgStrokeSize = null;
    this._textStrokeColor = null;
    this._textStrokeSize = null;
    this._textFillColor = "black";
    this._textDirection = "horizontal";
    this._wrapWords = true;
    this._fontStyle = "normal";
    this._fontVariant = "normal";
    this._fontWeight = "normal";
    this._fontFamily = "sans-serif";
    this._fontSize = 20;
    this._value = null;
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
  split(value2) {
    if (this.wrapWords) {
      return value2.split(" ").join("\n").replace(/\r\n/, "\n").split("\n");
    } else {
      return value2.replace(/\r\n/, "\n").split("\n");
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

// ../graphics2d/ClockImage.ts
var ClockImage = class extends TextImage {
  constructor() {
    super({
      textFillColor: "#ffffff",
      textStrokeColor: "rgba(0, 0, 0, 0.25)",
      textStrokeSize: 0.05,
      fontFamily: getMonospaceFonts(),
      fontSize: 20,
      minHeight: 1,
      maxHeight: 1,
      padding: 0.3,
      wrapWords: false,
      freezeDimensions: true
    });
    const updater = this.update.bind(this);
    setInterval(updater, 500);
    updater();
  }
  _fps = null;
  get fps() {
    return this._fps;
  }
  set fps(v) {
    if (v !== this.fps) {
      this._fps = v;
      this.update();
    }
  }
  lastLen = 0;
  update() {
    const time = new Date();
    let value2 = time.toLocaleTimeString();
    if (this.fps !== null) {
      value2 += ` ${Math.round(this.fps).toFixed(0)}hz`;
    }
    if (value2.length !== this.lastLen) {
      this.lastLen = value2.length;
      this.unfreeze();
    }
    this.value = value2;
  }
};

// ../dom/onUserGesture.ts
var gestures = [
  "change",
  "click",
  "contextmenu",
  "dblclick",
  "mouseup",
  "pointerup",
  "reset",
  "submit",
  "touchend"
];
function onUserGesture(callback, test) {
  const realTest = test || (async () => true);
  const check = async (evt) => {
    if (evt.isTrusted && await realTest()) {
      for (const gesture of gestures) {
        window.removeEventListener(gesture, check);
      }
      await callback();
    }
  };
  for (const gesture of gestures) {
    window.addEventListener(gesture, check);
  }
}

// ../audio/nodes.ts
var hasAudioContext = "AudioContext" in globalThis;
var hasAudioListener = hasAudioContext && "AudioListener" in globalThis;
var hasOldAudioListener = hasAudioListener && "setPosition" in AudioListener.prototype;
var hasNewAudioListener = hasAudioListener && "positionX" in AudioListener.prototype;
var canCaptureStream = isFunction(HTMLMediaElement.prototype.captureStream) || isFunction(HTMLMediaElement.prototype.mozCaptureStream);
function isWrappedAudioNode(value2) {
  return isDefined(value2) && value2.node instanceof AudioNode;
}
function isErsatzAudioNode(value2) {
  return isDefined(value2) && value2.input instanceof AudioNode && value2.output instanceof AudioNode;
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
function nameVertex(name2, v) {
  names.set(v, name2);
}
function removeVertex(v) {
  names.delete(v);
  if (isAudioNode(v)) {
    disconnect(v);
  }
}
function chain(...nodes2) {
  for (let i = 1; i < nodes2.length; ++i) {
    connect(nodes2[i - 1], nodes2[i]);
  }
}
function connect(left2, right) {
  const a = resolveOutput(left2);
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
function disconnect(left2, right) {
  const a = resolveOutput(left2);
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
  const nodes2 = /* @__PURE__ */ new Map();
  function maybeAdd(node) {
    if (!nodes2.has(node)) {
      nodes2.set(node, new GraphNode(node));
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
    const branch = nodes2.get(parent);
    for (const child of children) {
      if (nodes2.has(child)) {
        branch.connectTo(nodes2.get(child));
      }
    }
  }
  return Array.from(nodes2.values());
}
globalThis.getAudioGraph = getAudioGraph;
async function audioReady(audioCtx) {
  nameVertex("speakers", audioCtx.destination);
  if (audioCtx.state !== "running") {
    if (audioCtx.state === "closed") {
      await audioCtx.resume();
    } else if (audioCtx.state === "suspended") {
      const stateChange = once(audioCtx, "statechange");
      onUserGesture(() => audioCtx.resume());
      await stateChange;
    } else {
      assertNever(audioCtx.state);
    }
  }
}
function initAudio(name2, left2, ...rest) {
  nameVertex(name2, left2);
  for (const right of rest) {
    if (isDefined(right)) {
      connect(left2, right);
    }
  }
  return left2;
}
function Analyser(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new AnalyserNode(audioCtx, options), ...rest);
}
function BiquadFilter(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new BiquadFilterNode(audioCtx, options), ...rest);
}
function Delay(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new DelayNode(audioCtx, options), ...rest);
}
function DynamicsCompressor(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new DynamicsCompressorNode(audioCtx, options), ...rest);
}
function Gain(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new GainNode(audioCtx, options), ...rest);
}
function MediaElementSource(name2, audioCtx, mediaElement, ...rest) {
  return initAudio(name2, audioCtx.createMediaElementSource(mediaElement), ...rest);
}
function MediaStreamDestination(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new MediaStreamAudioDestinationNode(audioCtx, options), ...rest);
}
function MediaStreamSource(name2, audioCtx, mediaStream, ...rest) {
  return initAudio(name2, audioCtx.createMediaStreamSource(mediaStream), ...rest);
}
function Panner(name2, audioCtx, options, ...rest) {
  return initAudio(name2, new PannerNode(audioCtx, options), ...rest);
}

// ../audio/Pose.ts
var Pose = class {
  constructor() {
    this.t = 0;
    this.p = vec3_exports.create();
    this.f = vec3_exports.set(vec3_exports.create(), 0, 0, -1);
    this.u = vec3_exports.set(vec3_exports.create(), 0, 1, 0);
    this.o = vec3_exports.create();
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

// ../audio/BaseAudioElement.ts
var BaseAudioElement = class extends TypedEventBase {
  constructor(id2, audioCtx, spatializer) {
    super();
    this.id = id2;
    this.audioCtx = audioCtx;
    this.spatializer = spatializer;
    this.pose = new Pose();
    this.disposed = false;
    this.volumeControl = Gain(`volume-control-${this.id}`, audioCtx);
  }
  get volume() {
    return this.volumeControl.gain.value;
  }
  set volume(v) {
    this.volumeControl.gain.value = v;
  }
  dispose() {
    if (!this.disposed) {
      this.onDisposing();
      this.disposed = true;
    }
  }
  onDisposing() {
    removeVertex(this.volumeControl);
    if (isDisposable(this.spatializer)) {
      this.spatializer.dispose();
    }
  }
  audioTick(t2) {
    this.spatializer.setPose(this.pose, t2);
  }
};

// ../audio/BaseSpatializer.ts
var BaseSpatializer = class {
  constructor(id2) {
    this.id = id2;
    this.minDistance = 1;
    this.maxDistance = 10;
    this.algorithm = "inverse";
  }
  setAudioProperties(minDistance, maxDistance, algorithm) {
    this.minDistance = minDistance;
    this.maxDistance = maxDistance;
    this.algorithm = algorithm;
  }
};

// ../audio/sources/spatializers/BaseEmitter.ts
var BaseEmitter = class extends BaseSpatializer {
  constructor() {
    super(...arguments);
    this.disposed = false;
  }
  dispose() {
    if (!this.disposed) {
      this.onDisposing();
      this.disposed = true;
    }
  }
  onDisposing() {
    removeVertex(this.input);
  }
};

// ../audio/sources/spatializers/NoSpatializationNode.ts
var nodes = singleton("Juniper:Audio:Destinations:Spatializers:NoSpatializationNode:nodes", () => /* @__PURE__ */ new WeakMap());
var NoSpatializationNode = class extends BaseEmitter {
  static instance(audioCtx) {
    if (!nodes.has(audioCtx)) {
      const id2 = `no-spatialization-hook-${stringRandom(8)}`;
      nodes.set(audioCtx, new NoSpatializationNode(id2, audioCtx));
    }
    return nodes.get(audioCtx);
  }
  constructor(id2, audioCtx) {
    super(id2);
    this.input = this.output = Gain(this.id, audioCtx);
    Object.seal(this);
  }
  setPose(_loc, _t) {
  }
};

// ../audio/destinations/AudioDestination.ts
var AudioDestination = class extends BaseAudioElement {
  constructor(audioCtx, _trueDestination, listener) {
    super("final", audioCtx, listener);
    this._trueDestination = _trueDestination;
    this._remoteUserInput = Gain("remote-user-input", this.audioCtx, null, this._spatializedInput = Gain("spatialized-input", this.audioCtx, null, this.volumeControl));
    this._nonSpatializedInput = Gain("non-spatialized-input", this.audioCtx, null, this.volumeControl);
    connect(this.volumeControl, this._trueDestination);
    connect(NoSpatializationNode.instance(this.audioCtx), this.nonSpatializedInput);
  }
  onDisposing() {
    removeVertex(this._remoteUserInput);
    removeVertex(this._spatializedInput);
    removeVertex(this._nonSpatializedInput);
    super.onDisposing();
  }
  get remoteUserInput() {
    return this._remoteUserInput;
  }
  get spatializedInput() {
    return this._spatializedInput;
  }
  get nonSpatializedInput() {
    return this._nonSpatializedInput;
  }
  get trueDestination() {
    return this._trueDestination;
  }
};

// ../audio/destinations/spatializers/BaseListener.ts
var BaseListener = class extends BaseSpatializer {
  constructor(audioCtx) {
    super("listener");
    this.audioCtx = audioCtx;
  }
  get listener() {
    return this.audioCtx.listener;
  }
};

// ../audio/destinations/spatializers/WebAudioListenerNew.ts
var WebAudioListenerNew = class extends BaseListener {
  constructor(audioCtx) {
    super(audioCtx);
    if (!hasNewAudioListener) {
      throw new Error("Latest WebAudio Listener API is not supported");
    }
    Object.seal(this);
  }
  setPose(loc, t2) {
    const { p, f, u } = loc;
    this.listener.positionX.setValueAtTime(p[0], t2);
    this.listener.positionY.setValueAtTime(p[1], t2);
    this.listener.positionZ.setValueAtTime(p[2], t2);
    this.listener.forwardX.setValueAtTime(f[0], t2);
    this.listener.forwardY.setValueAtTime(f[1], t2);
    this.listener.forwardZ.setValueAtTime(f[2], t2);
    this.listener.upX.setValueAtTime(u[0], t2);
    this.listener.upY.setValueAtTime(u[1], t2);
    this.listener.upZ.setValueAtTime(u[2], t2);
  }
};

// ../audio/destinations/spatializers/WebAudioListenerOld.ts
var WebAudioListenerOld = class extends BaseListener {
  constructor(audioCtx) {
    super(audioCtx);
    if (!hasOldAudioListener) {
      throw new Error("WebAudio Listener API is not supported");
    }
    Object.seal(this);
  }
  setPose(loc, _t) {
    const { p, f, u } = loc;
    this.listener.setPosition(p[0], p[1], p[2]);
    this.listener.setOrientation(f[0], f[1], f[2], u[0], u[1], u[2]);
  }
};

// ../audio/DeviceManager.ts
var canChangeAudioOutput = isFunction(HTMLAudioElement.prototype.setSinkId);
function filterDeviceDuplicates(devices) {
  const filtered = [];
  for (let i = 0; i < devices.length; ++i) {
    const a = devices[i];
    let found = false;
    for (let j = 0; j < filtered.length && !found; ++j) {
      const b = filtered[j];
      found = a.kind === b.kind && b.label.indexOf(a.label) > 0;
    }
    if (!found) {
      filtered.push(a);
    }
  }
  return filtered;
}
var DeviceManagerAudioOutputChangedEvent = class extends TypedEvent {
  constructor(device) {
    super("audiooutputchanged");
    this.device = device;
  }
};
var DeviceManagerAudioInputChangedEvent = class extends TypedEvent {
  constructor(audio) {
    super("audioinputchanged");
    this.audio = audio;
  }
};
var DeviceManagerVideoInputChangedEvent = class extends TypedEvent {
  constructor(video2) {
    super("videoinputchanged");
    this.video = video2;
  }
};
var PREFERRED_AUDIO_OUTPUT_ID_KEY = "calla:preferredAudioOutputID";
var PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";
var PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";
var DeviceManager = class extends TypedEventBase {
  constructor(element, needsVideoDevice = false) {
    super();
    this.element = element;
    this.needsVideoDevice = needsVideoDevice;
    this._hasAudioPermission = false;
    this._hasVideoPermission = false;
    this._currentStream = null;
    this.ready = this.start();
    Object.seal(this);
  }
  get hasAudioPermission() {
    return this._hasAudioPermission;
  }
  get hasVideoPermission() {
    return this._hasVideoPermission;
  }
  get currentStream() {
    return this._currentStream;
  }
  set currentStream(v) {
    if (v !== this.currentStream) {
      if (isDefined(this.currentStream) && this.currentStream.active) {
        for (const track of this.currentStream.getTracks()) {
          track.stop();
        }
      }
      this._currentStream = v;
    }
  }
  async start() {
    if (canChangeAudioOutput) {
      const device = await this.getPreferredAudioOutput();
      if (device) {
        await this.setAudioOutputDevice(device);
      }
    }
  }
  async startPreferredAudioInput() {
    const device = await this.getPreferredAudioInput();
    if (device) {
      await this.setAudioInputDevice(device);
      this.currentStream = await this.startStream();
    }
  }
  async startPreferredVideoInput() {
    const device = await this.getPreferredVideoInput();
    if (device) {
      await this.setVideoInputDevice(device);
      this.currentStream = await this.startStream();
    }
  }
  get preferredAudioOutputID() {
    if (!canChangeAudioOutput) {
      return null;
    }
    return localStorage.getItem(PREFERRED_AUDIO_OUTPUT_ID_KEY);
  }
  get preferredAudioInputID() {
    return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
  }
  get preferredVideoInputID() {
    return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
  }
  async getAudioOutputDevices(filterDuplicates = false) {
    if (!canChangeAudioOutput) {
      return [];
    }
    const devices = await this.getAvailableDevices(filterDuplicates);
    return devices && devices.audioOutput || [];
  }
  async getAudioInputDevices(filterDuplicates = false) {
    const devices = await this.getAvailableDevices(filterDuplicates);
    return devices && devices.audioInput || [];
  }
  async getVideoInputDevices(filterDuplicates = false) {
    const devices = await this.getAvailableDevices(filterDuplicates);
    return devices && devices.videoInput || [];
  }
  async getAudioOutputDevice() {
    if (!canChangeAudioOutput) {
      return null;
    }
    const curId = this.element && this.element.sinkId;
    if (isNullOrUndefined(curId)) {
      return null;
    }
    const devices = await this.getAudioOutputDevices(), device = arrayScan(devices, (d) => d.deviceId === curId);
    return device;
  }
  async getAudioInputDevice() {
    if (isNullOrUndefined(this.currentStream) || !this.currentStream.active) {
      return null;
    }
    const curTracks = this.currentStream.getAudioTracks();
    if (curTracks.length === 0) {
      return null;
    }
    const testTrack = curTracks[0];
    const devices = await this.getAudioInputDevices();
    const device = arrayScan(devices, (d) => testTrack.label === d.label);
    return device;
  }
  async getVideoInputDevice() {
    if (isNullOrUndefined(this.currentStream) || !this.currentStream.active) {
      return null;
    }
    const curTracks = this.currentStream.getVideoTracks();
    if (curTracks.length === 0) {
      return null;
    }
    const testTrack = curTracks[0];
    const devices = await this.getVideoInputDevices();
    const device = arrayScan(devices, (d) => testTrack.label === d.label);
    return device;
  }
  async getPreferredAudioOutput() {
    if (!canChangeAudioOutput) {
      return null;
    }
    const devices = await this.getAudioOutputDevices();
    const device = arrayScan(devices, (d) => d.deviceId === this.preferredAudioOutputID, (d) => d.deviceId === "default", (d) => d.deviceId.length > 0);
    return device;
  }
  async getPreferredAudioInput() {
    const devices = await this.getAudioInputDevices();
    const device = arrayScan(devices, (d) => d.deviceId === this.preferredAudioInputID, (d) => d.deviceId === "default", (d) => d.deviceId.length > 0);
    return device;
  }
  async getPreferredVideoInput() {
    const devices = await this.getVideoInputDevices();
    const device = arrayScan(devices, (d) => d.deviceId === this.preferredVideoInputID, (d) => this.needsVideoDevice && d.deviceId.length > 0);
    return device;
  }
  async setAudioOutputDevice(device) {
    if (canChangeAudioOutput) {
      if (isDefined(device) && device.kind !== "audiooutput") {
        throw new Error(`Device is not an audio output device. Was: ${device.kind}. Label: ${device.label}`);
      }
      localStorage.setItem(PREFERRED_AUDIO_OUTPUT_ID_KEY, device && device.deviceId || null);
      const curDevice = this.element;
      const curDeviceID = curDevice && curDevice.sinkId;
      if (this.preferredAudioOutputID !== curDeviceID) {
        if (isDefined(this.preferredAudioOutputID)) {
          await this.element.setSinkId(this.preferredAudioOutputID);
        }
        this.dispatchEvent(new DeviceManagerAudioOutputChangedEvent(device));
      }
    }
  }
  async setAudioInputDevice(device) {
    if (isDefined(device) && device.kind !== "audioinput") {
      throw new Error(`Device is not an audio input device. Was: ${device.kind}. Label: ${device.label}`);
    }
    localStorage.setItem(PREFERRED_AUDIO_INPUT_ID_KEY, device && device.deviceId || null);
    const curAudio = await this.getAudioInputDevice();
    const curAudioID = curAudio && curAudio.deviceId;
    if (this.preferredAudioInputID !== curAudioID) {
      this.dispatchEvent(new DeviceManagerAudioInputChangedEvent(device));
    }
  }
  async setVideoInputDevice(device) {
    if (isDefined(device) && device.kind !== "videoinput") {
      throw new Error(`Device is not an video input device. Was: ${device.kind}. Label: ${device.label}`);
    }
    localStorage.setItem(PREFERRED_VIDEO_INPUT_ID_KEY, device && device.deviceId || null);
    const curVideo = await this.getVideoInputDevice();
    const curVideoID = curVideo && curVideo.deviceId;
    if (this.preferredVideoInputID !== curVideoID) {
      this.dispatchEvent(new DeviceManagerVideoInputChangedEvent(device));
    }
  }
  async getAvailableDevices(filterDuplicates = false) {
    let devices = await this.getDevices();
    if (filterDuplicates) {
      devices = filterDeviceDuplicates(devices);
    }
    return {
      audioOutput: canChangeAudioOutput ? devices.filter((d) => d.kind === "audiooutput") : [],
      audioInput: devices.filter((d) => d.kind === "audioinput"),
      videoInput: devices.filter((d) => d.kind === "videoinput")
    };
  }
  async getDevices() {
    let devices = null;
    let testStream = null;
    for (let i = 0; i < 3; ++i) {
      devices = await navigator.mediaDevices.enumerateDevices();
      for (const device of devices) {
        if (device.deviceId.length > 0) {
          if (!this.hasAudioPermission) {
            this._hasAudioPermission = device.kind === "audioinput" && device.label.length > 0;
          }
          if (this.needsVideoDevice && !this.hasVideoPermission) {
            this._hasVideoPermission = device.kind === "videoinput" && device.label.length > 0;
          }
        }
      }
      if (this.hasAudioPermission && (!this.needsVideoDevice || this.hasVideoPermission)) {
        break;
      }
      try {
        if (!this.hasAudioPermission || this.needsVideoDevice && !this.hasVideoPermission) {
          testStream = await this.startStream();
        }
      } catch (exp) {
        console.warn(exp);
      }
    }
    if (testStream) {
      for (const track of testStream.getTracks()) {
        track.stop();
      }
    }
    devices = arraySortByKey(devices || [], (d) => d.label);
    return devices;
  }
  startStream() {
    return navigator.mediaDevices.getUserMedia({
      audio: this.preferredAudioInputID && { deviceId: this.preferredAudioInputID } || true,
      video: this.needsVideoDevice && (this.preferredVideoInputID && { deviceId: this.preferredVideoInputID } || true) || false
    });
  }
  async getMediaPermissions() {
    await this.getDevices();
    return {
      audio: this.hasAudioPermission,
      video: this.hasVideoPermission
    };
  }
};

// ../audio/effects/EchoEffect.ts
function* fibGen(a, b, count2 = -1) {
  let i = 0;
  while (count2 < 0 || i < count2) {
    yield a;
    ++i;
    const c = a + b;
    a = b;
    b = c;
  }
}
function EchoEffect(name2, audioCtx, connectTo) {
  return new EchoEffectNode(name2, audioCtx, 3, connectTo);
}
var EchoEffectLayerNode = class {
  constructor(name2, audioCtx, volume, delay, connectTo) {
    this.input = Gain(`${name2}-input`, audioCtx, {
      gain: volume
    }, this.output = Delay(`${name2}-delay`, audioCtx, {
      maxDelayTime: delay,
      delayTime: delay
    }, connectTo));
  }
  dispose() {
    removeVertex(this.input);
    removeVertex(this.output);
  }
};
var EchoEffectNode = class {
  constructor(name2, audioCtx, numLayers, connectTo) {
    this.output = Gain(`${name2}-output`, audioCtx, null, connectTo);
    const delays = Array.from(fibGen(1, 2, numLayers));
    this.layers = delays.map((delay) => new EchoEffectLayerNode(`${name2}-echo-layer-${delay}`, audioCtx, 0.5 / delay, delay, this.output));
    this.input = Gain(`${name2}-input`, audioCtx, null, ...this.layers);
  }
  dispose() {
    removeVertex(this.input);
    for (const layer of this.layers) {
      layer.dispose();
    }
    removeVertex(this.output);
    arrayClear(this.layers);
  }
};

// ../audio/effects/RadioEffect.ts
function RadioEffect(name2, audioCtx, connectTo) {
  return new RadioEffectNode(name2, audioCtx, connectTo);
}
var RadioEffectNode = class {
  constructor(name2, audioCtx, connectTo) {
    this.node = BiquadFilter(`${name2}-biquad-filter`, audioCtx, {
      type: "bandpass",
      frequency: 2500,
      Q: 4.5
    }, connectTo);
  }
  dispose() {
    removeVertex(this.node);
  }
};

// ../audio/effects/WallEffect.ts
function WallEffect(name2, audioCtx, connectTo) {
  return new WallEffectNode(name2, audioCtx, connectTo);
}
var WallEffectNode = class {
  constructor(name2, audioCtx, connectTo) {
    this.node = BiquadFilter(`${name2}-biquad-filter`, audioCtx, {
      type: "bandpass",
      frequency: 400,
      Q: 4.5
    }, connectTo);
  }
  dispose() {
    removeVertex(this.node);
  }
};

// ../audio/effects/index.ts
var effectStore = /* @__PURE__ */ new Map([
  ["Radio", RadioEffect],
  ["Wall", WallEffect],
  ["Echo", EchoEffect]
]);

// ../audio/sources/BaseAudioSource.ts
var AudioSourceAddedEvent = class extends TypedEvent {
  constructor(source) {
    super("sourceadded");
    this.source = source;
  }
};
var BaseAudioSource = class extends BaseAudioElement {
  constructor(id2, audioCtx, spatializer, ...effectNames) {
    super(id2, audioCtx, spatializer);
    this.source = null;
    this.effects = new Array();
    this._connected = false;
    this.setEffects(...effectNames);
  }
  onDisposing() {
    for (const effect of this.effects) {
      if (isDisposable(effect)) {
        effect.dispose();
      }
    }
    arrayClear(this.effects);
    super.onDisposing();
  }
  setEffects(...effectNames) {
    disconnect(this.volumeControl);
    for (const effect of this.effects) {
      if (isDisposable(effect)) {
        effect.dispose();
      }
    }
    arrayClear(this.effects);
    for (const effectName of effectNames) {
      if (isDefined(effectName)) {
        const effect = effectStore.get(effectName);
        if (isDefined(effect)) {
          this.effects.push(effect(`${effectName}-${this.id}`, this.audioCtx));
        }
      }
    }
    chain(this.volumeControl, ...this.effects, this.spatializer);
  }
  get spatialized() {
    return !(this.spatializer instanceof NoSpatializationNode);
  }
  get input() {
    return this.source;
  }
  get connected() {
    return this._connected;
  }
  connect() {
    if (!this.connected) {
      connect(this.source, this.volumeControl);
    }
  }
  disconnect() {
    if (this.connected) {
      disconnect(this.source, this.volumeControl);
    }
  }
  set input(v) {
    if (v !== this.input) {
      if (this.source) {
        removeVertex(this.source);
      }
      if (v) {
        this.source = v;
        this.connect();
        this.dispatchEvent(new AudioSourceAddedEvent(this.source));
      }
    }
  }
  get output() {
    return this.volumeControl;
  }
};

// ../audio/sources/IPlayable.ts
var MediaElementSourceEvent = class extends TypedEvent {
  constructor(type2, source) {
    super(type2);
    this.source = source;
  }
};
var MediaElementSourceLoadedEvent = class extends MediaElementSourceEvent {
  constructor(source) {
    super("loaded", source);
  }
};
var MediaElementSourceErroredEvent = class extends MediaElementSourceEvent {
  constructor(source, error) {
    super("errored", source);
    this.error = error;
  }
};
var MediaElementSourcePlayedEvent = class extends MediaElementSourceEvent {
  constructor(source) {
    super("played", source);
  }
};
var MediaElementSourcePausedEvent = class extends MediaElementSourceEvent {
  constructor(source) {
    super("paused", source);
  }
};
var MediaElementSourceStoppedEvent = class extends MediaElementSourceEvent {
  constructor(source) {
    super("stopped", source);
  }
};
var MediaElementSourceProgressEvent = class extends MediaElementSourceEvent {
  constructor(source) {
    super("progress", source);
    this.value = 0;
    this.total = 0;
  }
};

// ../audio/sources/AudioElementSource.ts
var elementRefCounts = /* @__PURE__ */ new WeakMap();
var nodeRefCounts = /* @__PURE__ */ new WeakMap();
function count(source, delta3) {
  const nodeCount = (nodeRefCounts.get(source) || 0) + delta3;
  nodeRefCounts.set(source, nodeCount);
  const elem = source.mediaElement;
  const elementCount = (elementRefCounts.get(elem) || 0) + delta3;
  elementRefCounts.set(elem, elementCount);
  return elementCount;
}
function inc(source) {
  count(source, 1);
}
function dec(source) {
  count(source, -1);
  if (nodeRefCounts.get(source) === 0) {
    nodeRefCounts.delete(source);
    removeVertex(source);
  }
  if (elementRefCounts.get(source.mediaElement) === 0) {
    elementRefCounts.delete(source.mediaElement);
    if (source.mediaElement.isConnected) {
      source.mediaElement.remove();
    }
  }
}
var AudioElementSource = class extends BaseAudioSource {
  constructor(id2, audioCtx, source, randomize, spatializer, ...effectNames) {
    super(id2, audioCtx, spatializer, ...effectNames);
    this.randomize = randomize;
    inc(this.input = source);
    this.audio = source.mediaElement;
    this.disconnect();
    this.loadEvt = new MediaElementSourceLoadedEvent(this);
    this.playEvt = new MediaElementSourcePlayedEvent(this);
    this.pauseEvt = new MediaElementSourcePausedEvent(this);
    this.stopEvt = new MediaElementSourceStoppedEvent(this);
    this.progEvt = new MediaElementSourceProgressEvent(this);
    const halt = (evt) => {
      this.disconnect();
      if (this.audio.currentTime === 0 || evt.type === "ended") {
        this.dispatchEvent(this.stopEvt);
      } else {
        this.dispatchEvent(this.pauseEvt);
      }
    };
    this.audio.addEventListener("ended", halt);
    this.audio.addEventListener("pause", halt);
    this.audio.addEventListener("play", () => {
      this.connect();
      if (this.randomize && this.audio.loop && this.audio.duration > 1) {
        const startTime = this.audio.duration * Math.random();
        this.audio.currentTime = startTime;
      }
      this.dispatchEvent(this.playEvt);
    });
    this.audio.addEventListener("timeupdate", () => {
      this.progEvt.value = this.audio.currentTime;
      this.progEvt.total = this.audio.duration;
      this.dispatchEvent(this.progEvt);
    });
    mediaElementCanPlay(this.audio).then((success2) => this.dispatchEvent(success2 ? this.loadEvt : new MediaElementSourceErroredEvent(this, this.audio.error)));
  }
  get playbackState() {
    if (this.audio.error) {
      return "errored";
    }
    if (this.audio.ended || this.audio.paused && this.audio.currentTime === 0) {
      return "stopped";
    }
    if (this.audio.paused) {
      return "paused";
    }
    return "playing";
  }
  play() {
    return this.audio.play();
  }
  async playThrough() {
    const endTask = once(this, "stopped");
    await this.play();
    await endTask;
  }
  pause() {
    this.audio.pause();
  }
  stop() {
    this.audio.currentTime = 0;
    this.pause();
  }
  restart() {
    this.stop();
    return this.play();
  }
  onDisposing() {
    this.disconnect();
    dec(this.input);
    super.onDisposing();
  }
};

// ../audio/sources/AudioStreamSource.ts
var AudioStreamSource = class extends BaseAudioSource {
  constructor(id2, audioCtx, spatializer, ...effectNames) {
    super(id2, audioCtx, spatializer, ...effectNames);
  }
};

// ../audio/sources/spatializers/BaseWebAudioPanner.ts
var BaseWebAudioPanner = class extends BaseEmitter {
  constructor(id2, audioCtx) {
    super(id2);
    this.lpx = 0;
    this.lpy = 0;
    this.lpz = 0;
    this.lox = 0;
    this.loy = 0;
    this.loz = 0;
    this.input = this.output = this.panner = Panner(this.id, audioCtx, {
      panningModel: "HRTF",
      distanceModel: "inverse",
      coneInnerAngle: 360,
      coneOuterAngle: 0,
      coneOuterGain: 0
    });
  }
  setAudioProperties(minDistance, maxDistance, algorithm) {
    super.setAudioProperties(minDistance, maxDistance, algorithm);
    this.panner.refDistance = this.minDistance;
    this.panner.distanceModel = algorithm;
    if (this.maxDistance <= 0) {
      this.panner.rolloffFactor = Infinity;
    } else {
      this.panner.rolloffFactor = 1 / this.maxDistance;
    }
  }
  setPose(loc, t2) {
    const { p, f } = loc;
    const [px, py, pz] = p;
    const [ox, oy, oz] = f;
    if (px !== this.lpx || py !== this.lpy || pz !== this.lpz) {
      this.lpx = px;
      this.lpy = py;
      this.lpz = pz;
      this.setPosition(px, py, pz, t2);
    }
    if (ox !== this.lox || oy !== this.loy || oz !== this.loz) {
      this.lox = ox;
      this.loy = oy;
      this.loz = oz;
      this.setOrientation(ox, oy, oz, t2);
    }
  }
};

// ../audio/sources/spatializers/WebAudioPannerNew.ts
var WebAudioPannerNew = class extends BaseWebAudioPanner {
  constructor(id2, audioCtx) {
    super(id2, audioCtx);
    Object.seal(this);
  }
  setPosition(x, y, z, t2) {
    this.panner.positionX.setValueAtTime(x, t2);
    this.panner.positionY.setValueAtTime(y, t2);
    this.panner.positionZ.setValueAtTime(z, t2);
  }
  setOrientation(x, y, z, t2) {
    this.panner.orientationX.setValueAtTime(-x, t2);
    this.panner.orientationY.setValueAtTime(-y, t2);
    this.panner.orientationZ.setValueAtTime(-z, t2);
  }
};

// ../audio/sources/spatializers/WebAudioPannerOld.ts
var WebAudioPannerOld = class extends BaseWebAudioPanner {
  constructor(id2, audioCtx) {
    super(id2, audioCtx);
    Object.seal(this);
  }
  setPosition(x, y, z, _t) {
    this.panner.setPosition(x, y, z);
  }
  setOrientation(x, y, z, _t) {
    this.panner.setOrientation(x, y, z);
  }
};

// ../audio/AudioManager.ts
if (!("AudioContext" in globalThis) && "webkitAudioContext" in globalThis) {
  globalThis.AudioContext = globalThis.webkitAudioContext;
}
if (!("OfflineAudioContext" in globalThis) && "webkitOfflineAudioContext" in globalThis) {
  globalThis.OfflineAudioContext = globalThis.webkitOfflineAudioContext;
}
var USE_HEADPHONES_KEY = "juniper::useHeadphones";
var useHeadphonesToggledEvt = new TypedEvent("useheadphonestoggled");
var hasStreamSources = "createMediaStreamSource" in AudioContext.prototype;
var useElementSourceForUsers = !hasStreamSources;
var AudioManager = class extends TypedEventBase {
  sortedUserIDs = new Array();
  users = /* @__PURE__ */ new Map();
  clips = /* @__PURE__ */ new Map();
  clipPaths = /* @__PURE__ */ new Map();
  pathSources = /* @__PURE__ */ new Map();
  pathCounts = /* @__PURE__ */ new Map();
  localFilter;
  localCompressor;
  _minDistance = 1;
  _maxDistance = 10;
  _offsetRadius = 0;
  _useHeadphones = false;
  _algorithm = "inverse";
  get algorithm() {
    return this._algorithm;
  }
  element = null;
  audioDestination = null;
  devices;
  input;
  localAutoControlledGain;
  output;
  audioCtx;
  ready;
  localUserID = null;
  constructor(defaultLocalUserID) {
    super();
    this.audioCtx = new AudioContext();
    let destination = null;
    if (canChangeAudioOutput) {
      destination = MediaStreamDestination("final-destination", this.audioCtx);
      this.element = Audio(id("Audio-Device-Manager"), playsInline(true), autoPlay(true), srcObject(destination.stream), styles(display("none")));
      elementApply(document.body, this);
    } else {
      destination = this.audioCtx.destination;
    }
    this.devices = new DeviceManager(this.element);
    this.input = Gain("local-mic-user-gain", this.audioCtx, null, this.localAutoControlledGain = Gain("local-mic-auto-gain", this.audioCtx, null, this.localFilter = BiquadFilter("local-mic-filter", this.audioCtx, {
      type: "bandpass",
      frequency: 1500,
      Q: 0.25
    }, this.localCompressor = DynamicsCompressor("local-mic-compressor", this.audioCtx, {
      threshold: -15,
      knee: 40,
      ratio: 17
    }, this.output = MediaStreamDestination("local-mic-destination", this.audioCtx)))));
    this.audioDestination = new AudioDestination(this.audioCtx, destination, hasNewAudioListener ? new WebAudioListenerNew(this.audioCtx) : new WebAudioListenerOld(this.audioCtx));
    NoSpatializationNode.instance(this.audioCtx).setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);
    this.setLocalUserID(defaultLocalUserID);
    const useHeadphones = localStorage.getItem(USE_HEADPHONES_KEY);
    if (isDefined(useHeadphones)) {
      this._useHeadphones = useHeadphones === "true";
    } else {
      this._useHeadphones = isMobileVR();
    }
    this.ready = this.start();
    Object.seal(this);
  }
  get useHeadphones() {
    return this._useHeadphones;
  }
  set useHeadphones(v) {
    if (v !== this.useHeadphones) {
      this._useHeadphones = v;
      localStorage.setItem(USE_HEADPHONES_KEY, this.useHeadphones.toString());
      this.dispatchEvent(useHeadphonesToggledEvt);
    }
  }
  dispose() {
    for (const userID of this.sortedUserIDs) {
      this.removeUser(userID);
    }
    for (const clipID of this.clips.keys()) {
      this.removeClip(clipID);
    }
    disconnect(this.input);
    disconnect(this.localAutoControlledGain);
    disconnect(this.localFilter);
    disconnect(this.localCompressor);
    disconnect(this.output);
    this.audioDestination.dispose();
    this.audioCtx.suspend();
  }
  async start() {
    await audioReady(this.audioCtx);
    if (this.element) {
      await this.element.play();
    }
    await this.devices.ready;
  }
  get offsetRadius() {
    return this._offsetRadius;
  }
  set offsetRadius(v) {
    this._offsetRadius = v;
    this.updateUserOffsets();
  }
  get filter() {
    return this.localFilter;
  }
  get filterFrequency() {
    return this.localFilter.frequency.value;
  }
  set filterFrequency(v) {
    this.localFilter.frequency.value = v;
  }
  get compressor() {
    return this.localCompressor;
  }
  get isReady() {
    return this.audioCtx.state === "running";
  }
  get currentTime() {
    return this.audioCtx.currentTime;
  }
  update() {
    const t2 = this.currentTime;
    this.audioDestination.audioTick(t2);
    for (const clip of this.clips.values()) {
      clip.audioTick(t2);
    }
    for (const user of this.users.values()) {
      user.audioTick(t2);
    }
  }
  counter = 0;
  newSpatializer(id2, spatialize, isRemoteStream) {
    if (spatialize) {
      const destination = isRemoteStream ? this.audioDestination.remoteUserInput : this.audioDestination.spatializedInput;
      const slug = `spatializer-${++this.counter}-${id2}`;
      const spatializer = hasNewAudioListener ? new WebAudioPannerNew(slug + "-new-wa", this.audioCtx) : new WebAudioPannerOld(id2 + "-old-wa", this.audioCtx);
      connect(spatializer, destination);
      return spatializer;
    } else {
      return NoSpatializationNode.instance(this.audioCtx);
    }
  }
  createSpatializer(id2, spatialize, isRemoteStream) {
    const spatializer = this.newSpatializer(id2, spatialize, isRemoteStream);
    spatializer.setAudioProperties(this._minDistance, this._maxDistance, this._algorithm);
    return spatializer;
  }
  createUser(userID, userName) {
    if (!this.users.has(userID)) {
      const id2 = stringToName(userName, userID);
      const spatializer = this.createSpatializer(id2, true, true);
      const user = new AudioStreamSource(id2, this.audioCtx, spatializer);
      this.users.set(userID, user);
      arraySortedInsert(this.sortedUserIDs, userID);
      this.updateUserOffsets();
    }
    return this.users.get(userID);
  }
  setLocalUserID(id2) {
    if (this.audioDestination) {
      arrayRemove(this.sortedUserIDs, this.localUserID);
      this.localUserID = id2;
      arraySortedInsert(this.sortedUserIDs, this.localUserID);
      this.updateUserOffsets();
    }
    return this.audioDestination;
  }
  loadBasicClip(id2, path, vol, prog) {
    return this.loadClip(id2, path, false, false, false, false, vol, [], prog);
  }
  createBasicClip(id2, element, vol) {
    return this.createClip(id2, element, false, false, false, vol, []);
  }
  async loadClip(id2, path, looping, autoPlaying, spatialize, randomize, vol, effectNames, prog) {
    if (isNullOrUndefined(path) || isString(path) && path.length === 0) {
      throw new Error("No clip source path provided");
    }
    if (isDefined(prog)) {
      prog.start(path);
    }
    const source = await this.getSourceTask(id2, path, path, looping, autoPlaying, prog);
    const clip = this.makeClip(source, id2, path, spatialize, randomize, autoPlaying, vol, ...effectNames);
    this.clips.set(id2, clip);
    if (isDefined(prog)) {
      prog.end(path);
    }
    return clip;
  }
  createClip(id2, element, autoPlaying, spatialize, randomize, vol, effectNames) {
    if (isNullOrUndefined(element) || isString(element) && element.length === 0) {
      throw new Error("No clip source path provided");
    }
    const curPath = element.currentSrc;
    const source = this.createSourceFromElement(id2, element);
    const clip = this.makeClip(source, id2, curPath, spatialize, randomize, autoPlaying, vol, ...effectNames);
    this.clips.set(id2, clip);
    return clip;
  }
  getSourceTask(id2, curPath, path, looping, autoPlaying, prog) {
    this.clipPaths.set(id2, curPath);
    let sourceTask = this.pathSources.get(curPath);
    if (isDefined(sourceTask)) {
      this.pathCounts.set(curPath, this.pathCounts.get(curPath) + 1);
    } else {
      sourceTask = this.createSourceFromFile(id2, path, looping, autoPlaying, prog);
      this.pathSources.set(curPath, sourceTask);
      this.pathCounts.set(curPath, 1);
    }
    return sourceTask;
  }
  async createSourceFromFile(id2, path, looping, autoPlaying, prog) {
    if (isDefined(prog)) {
      prog.start(id2);
    }
    const elem = BackgroundAudio(autoPlaying, false, looping, src(path));
    if (!await mediaElementCanPlay(elem)) {
      throw elem.error;
    }
    if (isDefined(prog)) {
      prog.end(id2);
    }
    return this.createSourceFromElement(id2, elem);
  }
  createSourceFromStream(id2, stream) {
    const elem = BackgroundAudio(true, !useElementSourceForUsers, false, srcObject(stream));
    if (useElementSourceForUsers) {
      return this.createSourceFromElement(id2, elem);
    } else {
      return MediaStreamSource(stringToName("media-stream-source", id2, stream.id), this.audioCtx, stream);
    }
  }
  createSourceFromElement(id2, elem) {
    return MediaElementSource(stringToName("media-element-source", id2), this.audioCtx, elem);
  }
  makeClip(source, id2, path, spatialize, randomize, autoPlaying, vol, ...effectNames) {
    const nodeID = stringToName(id2, path);
    const spatializer = this.createSpatializer(nodeID, spatialize, false);
    const clip = new AudioElementSource(stringToName("audio-clip-element", nodeID), this.audioCtx, source, randomize, spatializer, ...effectNames);
    if (autoPlaying) {
      clip.play();
    }
    clip.volume = vol;
    return clip;
  }
  hasClip(id2) {
    return this.clips.has(id2);
  }
  async playClip(id2) {
    if (this.isReady && this.hasClip(id2)) {
      const clip = this.clips.get(id2);
      await clip.play();
    }
  }
  async playClipThrough(id2) {
    if (this.isReady && this.hasClip(id2)) {
      const clip = this.clips.get(id2);
      await clip.playThrough();
    }
  }
  stopClip(id2) {
    if (this.isReady && this.hasClip(id2)) {
      const clip = this.clips.get(id2);
      clip.stop();
    }
  }
  getUser(userID) {
    return this.users.get(userID);
  }
  getClip(id2) {
    return this.clips.get(id2);
  }
  removeSource(sources, id2) {
    const source = sources.get(id2);
    if (isDefined(source)) {
      sources.delete(id2);
      source.dispose();
    }
    return source;
  }
  removeUser(userID) {
    const user = this.removeSource(this.users, userID);
    if (isDefined(user.input)) {
      user.input = null;
    }
    arrayRemove(this.sortedUserIDs, userID);
    this.updateUserOffsets();
  }
  removeClip(id2) {
    const path = this.clipPaths.get(id2);
    this.pathCounts.set(path, this.pathCounts.get(path) - 1);
    if (this.pathCounts.get(path) === 0) {
      this.pathCounts.delete(path);
      this.pathSources.delete(path);
    }
    return this.removeSource(this.clips, id2);
  }
  setUserStream(userID, userName, stream) {
    if (this.users.has(userID)) {
      const user = this.users.get(userID);
      if (stream) {
        user.input = this.createSourceFromStream(stringToName(userName, userID), stream);
      } else if (isDefined(user.input)) {
        user.input = null;
      }
    }
  }
  updateUserOffsets() {
    if (this.offsetRadius > 0) {
      const idx = this.sortedUserIDs.indexOf(this.localUserID);
      const dAngle = 2 * Math.PI / this.sortedUserIDs.length;
      const localAngle = (idx + 1) * dAngle;
      const dx = this.offsetRadius * Math.sin(localAngle);
      const dy = this.offsetRadius * (Math.cos(localAngle) - 1);
      for (let i = 0; i < this.sortedUserIDs.length; ++i) {
        const id2 = this.sortedUserIDs[i];
        const angle3 = (i + 1) * dAngle;
        const x = this.offsetRadius * Math.sin(angle3) - dx;
        const z = this.offsetRadius * (Math.cos(angle3) - 1) - dy;
        this.setUserOffset(id2, x, 0, z);
      }
    }
  }
  setAudioProperties(minDistance, maxDistance, algorithm) {
    this._minDistance = minDistance;
    this._maxDistance = maxDistance;
    this._algorithm = algorithm;
    NoSpatializationNode.instance(this.audioCtx).setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);
    for (const user of this.users.values()) {
      user.spatializer.setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);
    }
    for (const clip of this.clips.values()) {
      clip.spatializer.setAudioProperties(clip.spatializer.minDistance, clip.spatializer.maxDistance, this.algorithm);
    }
  }
  withPose(sources, id2, poseCallback) {
    const source = sources.get(id2);
    let pose = null;
    if (source) {
      pose = source.pose;
    } else if (id2 === this.localUserID) {
      pose = this.audioDestination.pose;
    }
    if (!pose) {
      return null;
    }
    return poseCallback(pose);
  }
  withUser(id2, poseCallback) {
    return this.withPose(this.users, id2, poseCallback);
  }
  setUserOffset(id2, x, y, z) {
    this.withUser(id2, (pose) => {
      pose.setOffset(x, y, z);
    });
  }
  getUserOffset(id2) {
    return this.withUser(id2, (pose) => pose.o);
  }
  setUserPose(id2, px, py, pz, fx, fy, fz, ux, uy, uz) {
    this.withUser(id2, (pose) => {
      pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
    });
  }
  withClip(id2, poseCallback) {
    return this.withPose(this.clips, id2, poseCallback);
  }
  setClipPosition(id2, x, y, z) {
    this.withClip(id2, (pose) => {
      pose.setPosition(x, y, z);
    });
  }
  setClipOrientation(id2, fx, fy, fz, ux, uy, uz) {
    this.withClip(id2, (pose) => {
      pose.setOrientation(fx, fy, fz, ux, uy, uz);
    });
  }
  setClipPose(id2, px, py, pz, fx, fy, fz, ux, uy, uz) {
    this.withClip(id2, (pose) => {
      pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
    });
  }
};

// ../audio/sources/IPlayer.ts
var MediaPlayerEvent = class extends MediaElementSourceEvent {
  constructor(type2, source) {
    super(type2, source);
  }
};
var MediaPlayerLoadingEvent = class extends MediaPlayerEvent {
  constructor(source) {
    super("loading", source);
  }
};

// ../audio/sources/AudioPlayer.ts
var AudioPlayer = class extends BaseAudioSource {
  loadingEvt;
  loadEvt;
  playEvt;
  pauseEvt;
  stopEvt;
  progEvt;
  onError;
  onPlay;
  onCanPlay;
  onWaiting;
  onPause;
  onTimeUpdate;
  _data = null;
  get data() {
    return this._data;
  }
  _loaded = false;
  get loaded() {
    return this._loaded;
  }
  element;
  audioSource;
  get title() {
    return this.element.title;
  }
  setTitle(v) {
    this.element.title = v;
  }
  sourcesByURL = /* @__PURE__ */ new Map();
  sources = new Array();
  potatoes = new Array();
  constructor(audioCtx) {
    super("JuniperAudioPlayer", audioCtx, NoSpatializationNode.instance(audioCtx));
    this.element = Audio(playsInline(true), autoPlay(false), loop(false), controls(true));
    this.input = MediaElementSource("JuniperAudioPlayer-Input", audioCtx, this.element);
    this.loadingEvt = new MediaPlayerLoadingEvent(this);
    this.loadEvt = new MediaElementSourceLoadedEvent(this);
    this.playEvt = new MediaElementSourcePlayedEvent(this);
    this.pauseEvt = new MediaElementSourcePausedEvent(this);
    this.stopEvt = new MediaElementSourceStoppedEvent(this);
    this.progEvt = new MediaElementSourceProgressEvent(this);
    this.onPlay = async () => {
      this.connect();
      this.dispatchEvent(this.playEvt);
    };
    this.onPause = (evt) => {
      this.disconnect();
      if (this.element.currentTime === 0 || evt.type === "ended") {
        this.dispatchEvent(this.stopEvt);
      } else {
        this.dispatchEvent(this.pauseEvt);
      }
    };
    this.onTimeUpdate = async () => {
      this.progEvt.value = this.element.currentTime;
      this.progEvt.total = this.element.duration;
      this.dispatchEvent(this.progEvt);
    };
    this.onError = () => this.loadAudio();
    this.element.addEventListener("play", this.onPlay);
    this.element.addEventListener("pause", this.onPause);
    this.element.addEventListener("ended", this.onPause);
    this.element.addEventListener("error", this.onError);
    this.element.addEventListener("waiting", this.onWaiting);
    this.element.addEventListener("canplay", this.onCanPlay);
    this.element.addEventListener("timeupdate", this.onTimeUpdate);
    Object.assign(window, { audioPlayer: this });
  }
  get hasAudio() {
    const source = this.sourcesByURL.get(this.element.src);
    return isDefined(source) && source.acodec !== "none" || isDefined(this.element.audioTracks) && this.element.audioTracks.length > 0 || isDefined(this.element.webkitAudioDecodedByteCount) && this.element.webkitAudioDecodedByteCount > 0 || isDefined(this.element.mozHasAudio) && this.element.mozHasAudio;
  }
  onDisposing() {
    super.onDisposing();
    this.clear();
    removeVertex(this.input);
    removeVertex(this.audioSource);
    this.element.removeEventListener("play", this.onPlay);
    this.element.removeEventListener("pause", this.onPause);
    this.element.removeEventListener("ended", this.onPause);
    this.element.removeEventListener("error", this.onError);
    this.element.removeEventListener("waiting", this.onWaiting);
    this.element.removeEventListener("canplay", this.onCanPlay);
    this.element.removeEventListener("timeupdate", this.onTimeUpdate);
  }
  clear() {
    this.stop();
    this.sourcesByURL.clear();
    arrayClear(this.sources);
    arrayClear(this.potatoes);
    this.element.src = "";
    this._data = null;
    this._loaded = false;
  }
  async load(data, prog) {
    this.clear();
    this._data = data;
    if (isString(data)) {
      this.setTitle(data);
      this.potatoes.push(data);
    } else {
      this.setTitle(data.title);
      arraySortByKeyInPlace(data.audios, (f) => -f.resolution);
      arrayReplace(this.sources, ...data.audios);
    }
    for (const audio of this.sources) {
      this.sourcesByURL.set(audio.url, audio);
    }
    if (!this.hasSources) {
      throw new Error("No audio sources");
    }
    this.dispatchEvent(this.loadingEvt);
    await this.loadAudio(prog);
    if (!this.hasSources) {
      throw new Error("No audio sources");
    }
    this._loaded = true;
    this.dispatchEvent(this.loadEvt);
    return this;
  }
  async getMediaCapabilities(source) {
    const config = {
      type: "file",
      audio: {
        contentType: source.contentType,
        bitrate: source.abr * 1024,
        samplerate: source.asr
      }
    };
    try {
      return await navigator.mediaCapabilities.decodingInfo(config);
    } catch {
      return {
        supported: true,
        powerEfficient: false,
        smooth: false,
        configuration: config
      };
    }
  }
  get hasSources() {
    return this.sources.length > 0 || this.potatoes.length > 0;
  }
  async loadAudio(prog) {
    if (isDefined(prog)) {
      prog.start();
    }
    this.element.removeEventListener("error", this.onError);
    while (this.hasSources) {
      let url = null;
      const source = this.sources.shift();
      if (isDefined(source)) {
        const caps = await this.getMediaCapabilities(source);
        if (!caps.smooth || !caps.powerEfficient) {
          this.potatoes.push(source.url);
          continue;
        } else {
          url = source.url;
        }
      } else {
        url = this.potatoes.shift();
      }
      this.element.src = url;
      this.element.load();
      if (await mediaElementCanPlayThrough(this.element)) {
        if (isDefined(source)) {
          this.sources.unshift(source);
        } else {
          this.potatoes.unshift(url);
        }
        this.element.addEventListener("error", this.onError);
        if (isDefined(prog)) {
          prog.end();
        }
        return;
      }
    }
  }
  get playbackState() {
    if (isNullOrUndefined(this.data)) {
      return "empty";
    }
    if (!this.loaded) {
      return "loading";
    }
    if (this.element.error) {
      return "errored";
    }
    if (this.element.ended || this.element.paused && this.element.currentTime === 0) {
      return "stopped";
    }
    if (this.element.paused) {
      return "paused";
    }
    return "playing";
  }
  play() {
    return this.element.play();
  }
  async playThrough() {
    const endTask = once(this, "stopped");
    await this.play();
    await endTask;
  }
  pause() {
    this.element.pause();
  }
  stop() {
    this.pause();
    this.element.currentTime = 0;
  }
  restart() {
    this.stop();
    return this.play();
  }
};

// ../webrtc/constants.ts
var DEFAULT_LOCAL_USER_ID = "local-user";

// ../threejs/ButtonFactory.ts
async function loadIcon(fetcher, setName, iconName, iconPath, popper) {
  const { content } = await fetcher.get(iconPath).progress(popper.pop()).image();
  return [
    setName,
    iconName,
    content
  ];
}
var ButtonFactory = class {
  constructor(fetcher, imagePaths, padding2) {
    this.fetcher = fetcher;
    this.imagePaths = imagePaths;
    this.padding = padding2;
    this.uvDescrips = new PriorityMap();
    this.geoms = new PriorityMap();
    this.canvas = null;
    this.texture = null;
    this.enabledMaterial = null;
    this.disabledMaterial = null;
    this.readyTask = new Task();
  }
  async load(prog) {
    const popper = progressPopper(prog);
    const imageSets = new PriorityMap(await Promise.all(Array.from(this.imagePaths.entries()).map(([setName, iconName, path]) => loadIcon(this.fetcher, setName, iconName, path, popper))));
    const images = Array.from(imageSets.values());
    const iconWidth = Math.max(...images.map((img) => img.width));
    const iconHeight = Math.max(...images.map((img) => img.height));
    const area = iconWidth * iconHeight * images.length;
    const squareDim = Math.sqrt(area);
    const cols = Math.floor(squareDim / iconWidth);
    const rows = Math.ceil(images.length / cols);
    const width2 = cols * iconWidth;
    const height2 = rows * iconHeight;
    const canvWidth = nextPowerOf2(width2);
    const canvHeight = nextPowerOf2(height2);
    const widthRatio = width2 / canvWidth;
    const heightRatio = height2 / canvHeight;
    const du = iconWidth / canvWidth;
    const dv = iconHeight / canvHeight;
    this.canvas = createUICanvas(canvWidth, canvHeight);
    const g = this.canvas.getContext("2d");
    g.fillStyle = "#1e4388";
    g.fillRect(0, 0, canvWidth, canvHeight);
    let i = 0;
    for (const [setName, imgName, img] of imageSets.entries()) {
      const c = i % cols;
      const r = (i - c) / cols;
      const u = widthRatio * (c * iconWidth / width2);
      const v = heightRatio * (1 - r / rows) - dv;
      const x = c * iconWidth;
      const y = r * iconHeight + canvHeight - height2;
      const w = iconWidth - 2 * this.padding;
      const h = iconHeight - 2 * this.padding;
      g.drawImage(img, 0, 0, img.width, img.height, x + this.padding, y + this.padding, w, h);
      this.uvDescrips.add(setName, imgName, { u, v, du, dv });
      ++i;
    }
    this.texture = new THREE.CanvasTexture(this.canvas);
    this.enabledMaterial = new THREE.MeshBasicMaterial({
      map: this.texture
    });
    this.enabledMaterial.needsUpdate = true;
    this.disabledMaterial = new THREE.MeshBasicMaterial({
      map: this.texture,
      transparent: true,
      opacity: 0.5
    });
    this.disabledMaterial.needsUpdate = true;
    this.readyTask.resolve();
  }
  getSets() {
    return Array.from(this.imagePaths.keys());
  }
  getIcons(setName) {
    if (!this.imagePaths.has(setName)) {
      throw new Exception(`Button set ${setName} does not exist`);
    }
    return Array.from(this.imagePaths.get(setName).keys());
  }
  async getMaterial(enabled) {
    await this.readyTask;
    return enabled ? this.enabledMaterial : this.disabledMaterial;
  }
  async getGeometry(setName, iconName) {
    await this.readyTask;
    const uvSet = this.uvDescrips.get(setName);
    const uv = uvSet && uvSet.get(iconName);
    if (!uvSet || !uv) {
      throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
    }
    let geom2 = this.geoms.get(setName, iconName);
    if (!geom2) {
      geom2 = new THREE.PlaneBufferGeometry(1, 1, 1, 1);
      geom2.name = `Geometry:${setName}/${iconName}`;
      this.geoms.add(setName, iconName, geom2);
      const uvBuffer = geom2.getAttribute("uv");
      for (let i = 0; i < uvBuffer.count; ++i) {
        const u = uvBuffer.getX(i) * uv.du + uv.u;
        const v = uvBuffer.getY(i) * uv.dv + uv.v;
        uvBuffer.setX(i, u);
        uvBuffer.setY(i, v);
      }
    }
    return geom2;
  }
  async getMesh(setName, iconName, enabled) {
    const geom2 = await this.getGeometry(setName, iconName);
    const mesh = new THREE.Mesh(geom2, enabled ? this.enabledMaterial : this.disabledMaterial);
    mesh.name = `Mesh:${setName}/${iconName}`;
    return mesh;
  }
  async getGeometryAndMaterials(setName, iconName) {
    const [geometry, enabledMaterial, disabledMaterial] = await Promise.all([
      this.getGeometry(setName, iconName),
      this.getMaterial(true),
      this.getMaterial(false)
    ]);
    return {
      geometry,
      enabledMaterial,
      disabledMaterial
    };
  }
  getImageSrc(setName, iconName) {
    const imageSet = this.imagePaths.get(setName);
    const imgSrc = imageSet && imageSet.get(iconName);
    if (!imageSet || !imgSrc) {
      throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
    }
    return imgSrc;
  }
  getImageElement(setName, iconName) {
    return Img(title(setName + " " + iconName), src(this.getImageSrc(setName, iconName)));
  }
};

// ../graphics2d/animation/Animator.ts
var Animator = class {
  animations = new Array();
  update(dt) {
    dt = 1e-3 * dt;
    for (const animation of this.animations) {
      animation(dt);
    }
  }
  clear() {
    arrayClear(this.animations);
  }
  start(delay, duration, update) {
    let time = -delay;
    update(0);
    const animationComplete = new Task();
    this.animations.push((dt) => {
      time += dt / duration;
      if (time >= 1) {
        update(1);
        animationComplete.resolve();
      } else if (time >= 0) {
        update(time);
      }
    });
    return animationComplete;
  }
};

// ../graphics2d/animation/tween.ts
function bump(t2, k) {
  var a = t2 * Math.PI;
  return 0.5 * (1 - Math.cos(a)) - k * Math.sin(2 * a);
}
function jump(t2, k) {
  var a = (t2 - 0.5) * Math.PI;
  return t2 * t2 + k * Math.cos(a);
}

// ../widgets/DialogBox.ts
Style(rule(".dialog, .dialog-container", position("fixed")), rule(".dialog", top(0), left(0), width("100%"), height("100%"), backgroundColor("rgba(0, 0, 0, 0.5)"), zIndex(100)), rule(".dialog-container", top("50%"), left("50%"), maxWidth("100%"), maxHeight("100%"), transform("translateX(-50%) translateY(-50%)"), backgroundColor("white"), boxShadow("rgba(0,0,0,0.5) 0px 5px 30px"), display("grid"), gridTemplateColumns("2em auto 2em"), gridTemplateRows("auto 1fr auto 2em")), rule(".dialog .title-bar", gridArea(1, 1, 2, -1), padding("0.25em")), rule(".dialog-content", gridArea(2, 2, -4, -2), overflow("auto")), rule(".dialog-controls", gridArea(-2, 2, -3, -2)), rule(".dialog .confirm-button", float("right")), rule(".dialog h1, .dialog h2, .dialog h3, .dialog h4, .dialog h5, .dialog h6", textAlign("left")), rule(".dialog select", maxWidth("10em")));
var DialogBox = class {
  element;
  subEventer = new TypedEventBase();
  _title;
  titleElement;
  container;
  contentArea;
  confirmButton;
  cancelButton;
  constructor(title2) {
    this.element = Div(className("dialog"), customData("dialogname", title2), styles(display("none")), this.container = Div(className("dialog-container"), this.titleElement = H1(className("title-bar"), title2), this.contentArea = Div(className("dialog-content")), Div(className("dialog-controls"), this.confirmButton = ButtonPrimary("Confirm", classList("confirm-button")), this.cancelButton = ButtonSecondary("Cancel", classList("cancel-button")))));
    this.confirmButton.addEventListener("click", () => this.subEventer.dispatchEvent(new TypedEvent("confirm")));
    this.cancelButton.addEventListener("click", () => this.subEventer.dispatchEvent(new TypedEvent("cancel")));
    elementApply(document.body, this);
  }
  get title() {
    return this._title;
  }
  set title(v) {
    elementSetText(this.titleElement, this._title = v);
  }
  async onShowing() {
  }
  onShown() {
  }
  async onConfirm() {
  }
  onCancel() {
  }
  async onClosing() {
  }
  onClosed() {
  }
  show(v) {
    elementSetDisplay(this, v);
  }
  get isOpen() {
    return elementIsDisplayed(this);
  }
  async showDialog() {
    await this.onShowing();
    this.show(true);
    this.onShown();
    const confirming = once(this.subEventer, "confirm", "cancel");
    const confirmed = await success(confirming);
    if (confirmed) {
      await this.onConfirm();
    } else {
      this.onCancel();
    }
    await this.onClosing();
    this.show(false);
    this.onClosed();
    return confirmed;
  }
};

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
function isQuaternion(obj2) {
  return isDefined(obj2) && obj2.isQuaternion;
}
function isEuler(obj2) {
  return isDefined(obj2) && obj2.isEuler;
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
function obj(name2, ...rest) {
  const obj2 = new THREE.Object3D();
  obj2.name = name2;
  objGraph(obj2, ...rest);
  return obj2;
}
function objectSetEnabled(obj2, enabled) {
  obj2 = objectResolve(obj2);
  if (isDisableable(obj2)) {
    obj2.disabled = !enabled;
  }
}

// ../threejs/animation/scaleOnHover.ts
var scaledItems = singleton("Juniper:ScaledItems", () => /* @__PURE__ */ new Map());
var start = 1;
var end = 1.1;
var timeScale = 5e-3;
var ScaleState = class {
  constructor(obj2) {
    this.obj = obj2;
    this.base = obj2.scale.clone();
    this.p = 0;
    this.dir = 0;
    this.running = false;
    this.wasDisabled = this.disabled;
    this.onEnter = () => this.run(1);
    this.onExit = () => this.run(-1);
    this.obj.addEventListener("enter", this.onEnter);
    this.obj.addEventListener("exit", this.onExit);
  }
  get enabled() {
    return !this.obj.disabled;
  }
  get disabled() {
    return !this.enabled;
  }
  run(d) {
    if (this.enabled || (d === -1 || this.p > 0)) {
      this.dir = d;
      this.running = true;
    }
  }
  updateScaling(dt) {
    if (this.disabled !== this.wasDisabled) {
      this.wasDisabled = this.disabled;
      if (this.disabled) {
        this.onExit();
      }
    }
    if (this.running) {
      this.p += this.dir * dt;
      if (this.dir > 0 && this.p >= 1 || this.dir < 0 && this.p < 0) {
        this.p = Math.max(0, Math.min(1, this.p));
        this.running = false;
      }
      const q = bump(this.p, 1.1);
      this.obj.scale.copy(this.base).multiplyScalar(q * (end - start) + start);
    }
  }
  dispose() {
    this.obj.removeEventListener("enter", this.onEnter);
    this.obj.removeEventListener("exit", this.onExit);
  }
};
function updateScalings(dt) {
  dt *= timeScale;
  for (const state of scaledItems.values()) {
    state.updateScaling(dt);
  }
}
function removeScaledObj(obj2) {
  const state = scaledItems.get(obj2);
  if (state) {
    scaledItems.delete(obj2);
    state.dispose();
  }
}
function scaleOnHover(obj2) {
  scaledItems.set(obj2, new ScaleState(obj2));
}

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
        set: function(value2) {
          this.uniforms.diffuse.value = value2;
        }
      },
      worldUnits: {
        enumerable: true,
        get: function() {
          return "WORLD_UNITS" in this.defines;
        },
        set: function(value2) {
          if (value2 === true) {
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
        set: function(value2) {
          this.uniforms.linewidth.value = value2;
        }
      },
      dashed: {
        enumerable: true,
        get: function() {
          return Boolean("USE_DASH" in this.defines);
        },
        set(value2) {
          if (Boolean(value2) !== Boolean("USE_DASH" in this.defines)) {
            this.needsUpdate = true;
          }
          if (value2 === true) {
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
        set: function(value2) {
          this.uniforms.dashScale.value = value2;
        }
      },
      dashSize: {
        enumerable: true,
        get: function() {
          return this.uniforms.dashSize.value;
        },
        set: function(value2) {
          this.uniforms.dashSize.value = value2;
        }
      },
      dashOffset: {
        enumerable: true,
        get: function() {
          return this.uniforms.dashOffset.value;
        },
        set: function(value2) {
          this.uniforms.dashOffset.value = value2;
        }
      },
      gapSize: {
        enumerable: true,
        get: function() {
          return this.uniforms.gapSize.value;
        },
        set: function(value2) {
          this.uniforms.gapSize.value = value2;
        }
      },
      opacity: {
        enumerable: true,
        get: function() {
          return this.uniforms.opacity.value;
        },
        set: function(value2) {
          this.uniforms.opacity.value = value2;
        }
      },
      resolution: {
        enumerable: true,
        get: function() {
          return this.uniforms.resolution.value;
        },
        set: function(value2) {
          this.uniforms.resolution.value.copy(value2);
        }
      },
      alphaToCoverage: {
        enumerable: true,
        get: function() {
          return Boolean("USE_ALPHA_TO_COVERAGE" in this.defines);
        },
        set: function(value2) {
          if (Boolean(value2) !== Boolean("USE_ALPHA_TO_COVERAGE" in this.defines)) {
            this.needsUpdate = true;
          }
          if (value2 === true) {
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
function del(obj2, name2) {
  if (name2 in obj2) {
    delete obj2[name2];
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
var black = 0;
var grey = 12632256;
var white = 16777215;
function solidTransparentBlack(opacity) {
  return solidTransparent({ color: black, opacity });
}
var litGrey = /* @__PURE__ */ lit({ color: grey });
var litWhite = /* @__PURE__ */ lit({ color: white });

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
var PlaneCollider = class extends BasePlane {
  constructor(sx, sy) {
    super(sx, sy, solidTransparentBlack(0), true);
    this.visible = false;
  }
};

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
var M = new THREE.Matrix4();
var P = new THREE.Vector3();
function objectGetRelativePose(ref, obj2, position2, quaternion, scale4) {
  M.copy(ref.matrixWorld).invert().multiply(obj2.matrixWorld).decompose(P, quaternion, scale4);
  position2.set(P.x, P.y, P.z, 1);
}

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
var Q = new THREE.Quaternion();
var S = new THREE.Vector3();
var copyCounter = 0;
var Image2DMesh = class extends THREE.Object3D {
  constructor(env, name2, isStatic, materialOrOptions = null) {
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
      this.setEnvAndName(env, name2);
      let material = isMeshBasicMaterial(materialOrOptions) ? materialOrOptions : solidTransparent(Object.assign({}, materialOrOptions, { name: this.name }));
      this.mesh = new TexturedMesh(plane, material);
      this.add(this.mesh);
    }
  }
  dispose() {
    cleanup(this.layer);
  }
  setEnvAndName(env, name2) {
    this.env = env;
    this.name = name2;
    this.tryWebXRLayers &&= this.env && this.env.hasXRCompositionLayers;
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this.setEnvAndName(source.env, source.name + ++copyCounter);
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
          objectGetRelativePose(this.env.stage, this.mesh, P2, Q, S);
          this.lastMatrixWorld.copy(this.matrixWorld);
          const transform2 = new XRRigidTransform(P2, Q);
          const width2 = S.x / 2;
          const height2 = S.y / 2;
          const layout = this.stereoLayoutName === "mono" ? "mono" : this.stereoLayoutName === "left-right" || this.stereoLayoutName === "right-left" ? "stereo-left-right" : "stereo-top-bottom";
          if (isVideo) {
            const invertStereo = this.stereoLayoutName === "right-left" || this.stereoLayoutName === "bottom-top";
            this.layer = this.env.xrMediaBinding.createQuadLayer(this.mesh.material.map.image, {
              space,
              layout,
              invertStereo,
              transform: transform2,
              width: width2,
              height: height2
            });
          } else {
            this.layer = this.env.xrBinding.createQuadLayer({
              space,
              layout,
              textureType: "texture",
              isStatic: this.isStatic,
              viewPixelWidth: this.mesh.imageWidth,
              viewPixelHeight: this.mesh.imageHeight,
              transform: transform2,
              width: width2,
              height: height2
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
          objectGetRelativePose(this.env.stage, this.mesh, P2, Q, S);
          this.lastMatrixWorld.copy(this.matrixWorld);
          this.layer.transform = new XRRigidTransform(P2, Q);
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
  constructor(env, name2, materialOptions) {
    super(env, name2, false, materialOptions);
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

// ../threejs/TextMeshLabel.ts
var TextMeshLabel = class extends THREE.Object3D {
  constructor(fetcher, env, name2, value2, textImageOptions) {
    super();
    this.fetcher = fetcher;
    this.env = env;
    this._disabled = false;
    if (isDefined(value2)) {
      this.name = name2;
      textImageOptions = Object.assign({
        textFillColor: "#ffffff",
        fontFamily: "Segoe UI Emoji",
        fontSize: 20,
        minHeight: 0.25,
        maxHeight: 0.25
      }, textImageOptions, {
        value: value2
      });
      this.image = new TextImage(textImageOptions);
      const id2 = stringRandom(16);
      this.enabledImage = this.createImage(`${id2}-enabled`, 1);
      this.disabledImage = this.createImage(`${id2}-disabled`, 0.5);
      this.disabledImage.visible = false;
      this.add(this.enabledImage, this.disabledImage);
    }
  }
  createImage(id2, opacity) {
    const image2 = new TextMesh(this.env, `text-${id2}`, {
      side: THREE.FrontSide,
      opacity
    });
    image2.textImage = this.image;
    return image2;
  }
  get disabled() {
    return this._disabled;
  }
  set disabled(v) {
    if (v !== this.disabled) {
      this._disabled = v;
      this.enabledImage.visible = !v;
      this.disabledImage.visible = v;
    }
  }
};

// ../threejs/TextMeshButton.ts
var TextMeshButton = class extends TextMeshLabel {
  constructor(fetcher, env, name2, value2, textImageOptions) {
    super(fetcher, env, name2, value2, textImageOptions);
    this.collider = null;
    this.isClickable = true;
    if (isDefined(value2)) {
      this.image.addEventListener("redrawn", () => {
        this.collider.scale.x = this.image.width;
        this.collider.scale.y = this.image.height;
      });
      this.collider = new PlaneCollider(this.image.width, this.image.height);
      this.collider.name = `collider-${this.name}`;
      this.add(this.collider);
      scaleOnHover(this);
    }
  }
};

// ../threejs/ConfirmationDialog.ts
var baseTextStyle = {
  bgStrokeColor: "#000000",
  bgStrokeSize: 0.04,
  wrapWords: false,
  textFillColor: "#ffffff",
  scale: 150
};
var textButtonStyle = Object.assign({}, baseTextStyle, {
  padding: {
    left: 0.1,
    right: 0.1,
    top: 0.025,
    bottom: 0.025
  },
  fontSize: 20,
  minHeight: 0.5,
  maxHeight: 0.5
});
var confirmButton3DStyle = Object.assign({}, textButtonStyle, {
  bgFillColor: "#0078d7"
});
var cancelButton3DStyle = Object.assign({}, textButtonStyle, {
  bgFillColor: "#780000"
});
var textLabelStyle = Object.assign({}, baseTextStyle, {
  bgFillColor: "#ffffff",
  textFillColor: "#000000",
  padding: {
    top: 0.1,
    left: 0.1,
    bottom: 0.4,
    right: 0.1
  },
  fontSize: 25,
  minHeight: 1,
  maxHeight: 1
});
var JUMP_FACTOR = 0.9;
function newStyle(baseStyle, fontFamily) {
  return Object.assign({}, baseStyle, { fontFamily });
}
var ConfirmationDialog = class extends DialogBox {
  constructor(env, fontFamily) {
    super("Confirm action");
    this.env = env;
    this.object = new THREE.Object3D();
    this.name = "ConfirmationDialog";
    this.root = new THREE.Object3D();
    this.animator = new Animator();
    this.confirmButton.innerText = "Yes";
    this.cancelButton.innerText = "No";
    this.mesh = new TextMeshLabel(this.env.fetcher, this.env, "confirmationDialogLabel", "", newStyle(textLabelStyle, fontFamily));
    this.confirmButton3D = new TextMeshButton(this.env.fetcher, this.env, "confirmationDialogConfirmButton", "Yes", newStyle(confirmButton3DStyle, fontFamily));
    this.confirmButton3D.addEventListener("click", () => this.confirmButton.click());
    this.confirmButton3D.position.set(1, -0.5, 0.5);
    this.cancelButton3D = new TextMeshButton(this.env.fetcher, this.env, "confirmationDialogCancelButton", "No", newStyle(cancelButton3DStyle, fontFamily));
    this.cancelButton3D.addEventListener("click", () => this.cancelButton.click());
    this.cancelButton3D.position.set(2, -0.5, 0.5);
    elementApply(this.container, styles(maxWidth("calc(100% - 2em)"), width("max-content")));
    elementApply(this.contentArea, styles(fontSize("18pt"), textAlign("center"), padding("1em")));
    objGraph(this, objGraph(this.root, this.mesh, this.confirmButton3D, this.cancelButton3D));
    objectSetVisible(this.root, false);
  }
  get visible() {
    return elementIsDisplayed(this);
  }
  set visible(visible) {
    elementSetDisplay(this, visible, "inline-block");
    this.mesh.visible = visible;
  }
  update(dt) {
    this.animator.update(dt);
  }
  async showHide(a, b) {
    await this.animator.start(0, 0.25, (t2) => {
      const scale4 = jump(a + b * t2, JUMP_FACTOR);
      this.root.scale.set(scale4, scale4, 0.01);
    });
    this.animator.clear();
  }
  get use3D() {
    return this.env.renderer.xr.isPresenting || this.env.testSpaceLayout;
  }
  async onShowing() {
    await super.onShowing();
    if (this.use3D) {
      this.root.visible = true;
      await this.showHide(0, 1);
    }
  }
  onShown() {
    if (this.use3D) {
      this.element.style.display = "none";
    }
  }
  async onClosing() {
    if (this.use3D) {
      await this.showHide(1, -1);
      this.root.visible = false;
    }
    await super.onClosing();
  }
  prompt(title2, message) {
    this.title = title2;
    elementSetText(this.contentArea, message);
    this.mesh.image.value = message;
    return this.showDialog();
  }
};

// ../threejs/isVisible.ts
function isVisible(obj2) {
  while (obj2 != null) {
    if (!obj2.visible) {
      return false;
    }
    obj2 = obj2.parent;
  }
  return true;
}

// ../threejs/eventSystem/InteractiveObject3D.ts
function isCollider(obj2) {
  return isObject3D(obj2) && isBoolean(obj2.isCollider) && isDefined(obj2.parent);
}
function isInteractiveHit(hit) {
  return isDefined(hit) && isCollider(hit.object);
}
function isObjVisible(hit) {
  return isDefined(hit) && isCollider(hit.object) && (isInteractiveObject3D(hit.object) && isVisible(hit.object) || isInteractiveObject3D(hit.object.parent) && isVisible(hit.object.parent));
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

// ../threejs/eventSystem/InteractionAudio.ts
function makeClipName(type2, isDisabled2) {
  if (type2 === "click" && isDisabled2) {
    type2 = "error";
  }
  return `InteractionAudio-${type2}`;
}
var InteractionAudio = class {
  constructor(audio, eventSys) {
    this.audio = audio;
    this.eventSys = eventSys;
    this.enabled = true;
    const playClip = (evt) => {
      const obj2 = evt.object;
      if (this.enabled && isInteractiveObject3D(obj2) && obj2.isClickable) {
        const clipName = makeClipName(evt.type, obj2.disabled);
        if (this.audio.hasClip(clipName)) {
          const { x, y, z } = evt.point;
          this.audio.setClipPosition(clipName, x, y, z);
          this.audio.playClip(clipName);
        }
      }
    };
    this.eventSys.addEventListener("enter", playClip);
    this.eventSys.addEventListener("exit", playClip);
    this.eventSys.addEventListener("click", playClip);
  }
  async load(type2, path, volume, prog) {
    return await this.audio.loadClip(makeClipName(type2, false), path, false, false, true, false, volume, [], prog);
  }
};

// ../threejs/ScreenMode.ts
var ScreenMode = /* @__PURE__ */ ((ScreenMode2) => {
  ScreenMode2["None"] = "None";
  ScreenMode2["Fullscreen"] = "Fullscreen";
  ScreenMode2["VR"] = "VR";
  ScreenMode2["AR"] = "AR";
  return ScreenMode2;
})(ScreenMode || {});

// ../threejs/ScreenUI.ts
Style(rule("#controls", position("absolute"), left(0), top(0), width("100%"), height("100%")), rule("#controls", display("grid"), fontSize("20pt"), gridTemplateRows("auto 1fr auto")), rule("#controls, #controls *", pointerEvents("none")), rule("#controls canvas", height("58px")), rule("#controls > .row", display("grid"), margin("10px 5px"), gridTemplateColumns("repeat(2, auto)")), rule("#controls > .row.top", gridRow(1)), rule("#controls > .row.middle", gridRow(2, -2)), rule("#controls > .row.bottom", gridRow(-2)), rule("#controls > .row > .cell", display("flex")), rule("#controls > .row > .cell.left", gridColumn(1)), rule("#controls > .row > .cell.right", gridColumn(-2), flexFlow("row-reverse")), rule("#controls > .row > .cell > .btn", borderRadius(0), backgroundColor("#1e4388"), height("58px !important"), width("58px"), padding("0.25em"), margin("0 5px"), pointerEvents("initial")), rule("#controls .btn-primary img", height("calc(100% - 0.5em)")));
var ScreenUI = class {
  constructor() {
    this.element = Div(id("controls"), Div(className("row top"), this.topRowLeft = Div(className("cell left")), this.topRowRight = Div(className("cell right"))), Div(className("row middle"), this.middleRowLeft = Div(className("cell left")), this.middleRowRight = Div(className("cell right"))), Div(className("row bottom"), this.bottomRowLeft = Div(className("cell left")), this.bottomRowRight = Div(className("cell right"))));
    this.cells = [
      [this.topRowLeft, this.topRowRight],
      [this.middleRowLeft, this.middleRowRight],
      [this.bottomRowLeft, this.bottomRowRight]
    ];
  }
};

// ../threejs/SpaceUI.ts
var radius = 1.25;
var dAngleH = deg2rad(30);
var dAngleV = deg2rad(32);
var headPos = new THREE.Vector3(0, 0, 0);
var SpaceUI = class extends THREE.Object3D {
  constructor() {
    super();
    this.name = "SpaceUI";
    this.position.y = -0.25;
  }
  addItem(child, position2) {
    child = objectResolve(child);
    objGraph(this, child);
    this.add(child);
    child.position.set(radius * Math.sin(position2.x * dAngleH), radius * Math.sin(position2.y * dAngleV), -radius * Math.cos(position2.x * dAngleH));
    child.scale.set(position2.scale, position2.scale, 1);
    for (const child2 of this.children) {
      child2.lookAt(headPos);
    }
  }
};

// ../mediatypes/util.ts
var typePattern = /([^\/]+)\/(.+)/;
var subTypePattern = /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/;
var MediaType = class {
  constructor(_type, _fullSubType, extensions) {
    this._type = _type;
    this._fullSubType = _fullSubType;
    this._primaryExtension = null;
    this.depMessage = null;
    const parameters = /* @__PURE__ */ new Map();
    this._parameters = parameters;
    const subTypeParts = this._fullSubType.match(subTypePattern);
    this._tree = subTypeParts[1];
    this._subType = subTypeParts[2];
    this._suffix = subTypeParts[3];
    const paramStr = subTypeParts[4];
    this._value = this._fullValue = this._type + "/";
    if (isDefined(this._tree)) {
      this._value = this._fullValue += this._tree + ".";
    }
    this._value = this._fullValue += this._subType;
    if (isDefined(this._suffix)) {
      this._value = this._fullValue += "+" + this._suffix;
    }
    if (isDefined(paramStr)) {
      const pairs = paramStr.split(";").map((p) => p.trim()).filter((p) => p.length > 0).map((p) => p.split("="));
      for (const [key, ...values] of pairs) {
        const value2 = values.join("=");
        parameters.set(key, value2);
        const slug = `; ${key}=${value2}`;
        this._fullValue += slug;
        if (key !== "q") {
          this._value += slug;
        }
      }
    }
    this._extensions = extensions || [];
    this._primaryExtension = this._extensions[0] || null;
  }
  static parse(value2) {
    if (!value2) {
      return null;
    }
    const match = value2.match(typePattern);
    if (!match) {
      return null;
    }
    const type2 = match[1];
    const subType = match[2];
    return new MediaType(type2, subType);
  }
  deprecate(message) {
    this.depMessage = message;
    return this;
  }
  check() {
    if (isDefined(this.depMessage)) {
      console.warn(`${this._value} is deprecated ${this.depMessage}`);
    }
  }
  matches(value2) {
    if (isNullOrUndefined(value2)) {
      return false;
    }
    if (this.typeName === "*" && this.subTypeName === "*") {
      return true;
    }
    let typeName = null;
    let subTypeName = null;
    if (isString(value2)) {
      const match = value2.match(typePattern);
      if (!match) {
        return false;
      }
      typeName = match[1];
      subTypeName = match[2];
    } else {
      typeName = value2.typeName;
      subTypeName = value2._fullSubType;
    }
    return this.typeName === typeName && (this._fullSubType === "*" || this._fullSubType === subTypeName);
  }
  withParameter(key, value2) {
    const newSubType = `${this._fullSubType}; ${key}=${value2}`;
    return new MediaType(this.typeName, newSubType, this.extensions);
  }
  get typeName() {
    this.check();
    return this._type;
  }
  get tree() {
    this.check();
    return this._tree;
  }
  get suffix() {
    return this._suffix;
  }
  get subTypeName() {
    this.check();
    return this._subType;
  }
  get value() {
    this.check();
    return this._value;
  }
  __getValueUnsafe() {
    return this._value;
  }
  get fullValue() {
    this.check();
    return this._fullValue;
  }
  get parameters() {
    this.check();
    return this._parameters;
  }
  get extensions() {
    this.check();
    return this._extensions;
  }
  __getExtensionsUnsafe() {
    return this._extensions;
  }
  get primaryExtension() {
    this.check();
    return this._primaryExtension;
  }
  toString() {
    if (this.parameters.get("q") === "1") {
      return this.value;
    } else {
      return this.fullValue;
    }
  }
  addExtension(fileName) {
    if (!fileName) {
      throw new Error("File name is not defined");
    }
    if (this.primaryExtension) {
      const idx = fileName.lastIndexOf(".");
      if (idx > -1) {
        const currentExtension = fileName.substring(idx + 1);
        ;
        if (this.extensions.indexOf(currentExtension) > -1) {
          fileName = fileName.substring(0, idx);
        }
      }
      fileName = `${fileName}.${this.primaryExtension}`;
    }
    return fileName;
  }
};
function create2(group2, value2, ...extensions) {
  return new MediaType(group2, value2, extensions);
}
function specialize(group2) {
  return create2.bind(null, group2);
}

// ../mediatypes/image.ts
var image = /* @__PURE__ */ specialize("image");
var Image_Vendor_Google_StreetView_Pano = image("vnd.google.streetview.pano");

// ../mediatypes/video.ts
var video = /* @__PURE__ */ specialize("video");
var Video_Vendor_Mpeg_Dash_Mpd = video("vnd.mpeg.dash.mpd", "mpd");

// ../video/data.ts
function isVideoRecord(obj2) {
  return isString(obj2.vcodec);
}

// ../video/BaseVideoPlayer.ts
var BaseVideoPlayer = class extends BaseAudioSource {
  loadingEvt;
  loadEvt;
  playEvt;
  pauseEvt;
  stopEvt;
  progEvt;
  onPlay;
  onSeeked;
  onCanPlay;
  onWaiting;
  onPause;
  onTimeUpdate = null;
  wasUsingAudioElement = false;
  _data = null;
  get data() {
    return this._data;
  }
  _loaded = false;
  get loaded() {
    return this._loaded;
  }
  video;
  audio;
  videoSource;
  audioSource;
  get title() {
    return this.video.title;
  }
  setTitle(v) {
    this.video.title = v;
    this.audio.title = v;
  }
  onError = /* @__PURE__ */ new Map();
  sourcesByURL = /* @__PURE__ */ new Map();
  sources = new PriorityList();
  potatoes = new PriorityList();
  constructor(audioCtx) {
    super("JuniperVideoPlayer", audioCtx, NoSpatializationNode.instance(audioCtx));
    this.video = this.createMediaElement(Video, controls(true));
    this.audio = this.createMediaElement(Audio, controls(false));
    this.input = Gain("JuniperVideoPlayer-combiner", audioCtx);
    this.videoSource = MediaElementSource("JuniperVideoPlayer-VideoNode", audioCtx, this.video, this.input);
    this.audioSource = MediaElementSource("JuniperVideoPlayer-AudioNode", audioCtx, this.audio, this.input);
    this.loadingEvt = new MediaPlayerLoadingEvent(this);
    this.loadEvt = new MediaElementSourceLoadedEvent(this);
    this.playEvt = new MediaElementSourcePlayedEvent(this);
    this.pauseEvt = new MediaElementSourcePausedEvent(this);
    this.stopEvt = new MediaElementSourceStoppedEvent(this);
    this.progEvt = new MediaElementSourceProgressEvent(this);
    this.onSeeked = () => {
      if (this.useAudioElement) {
        this.audio.currentTime = this.video.currentTime;
      }
    };
    this.onPlay = async () => {
      this.connect();
      this.onSeeked();
      if (this.useAudioElement) {
        await this.audio.play();
      }
      this.dispatchEvent(this.playEvt);
    };
    this.onPause = (evt) => {
      this.disconnect();
      if (this.useAudioElement) {
        this.onSeeked();
        this.audio.pause();
      }
      if (this.video.currentTime === 0 || evt.type === "ended") {
        this.dispatchEvent(this.stopEvt);
      } else {
        this.dispatchEvent(this.pauseEvt);
      }
    };
    let wasWaiting = false;
    this.onWaiting = () => {
      if (this.useAudioElement) {
        wasWaiting = true;
        this.audio.pause();
      }
    };
    this.onCanPlay = async () => {
      if (this.useAudioElement && wasWaiting) {
        await this.audio.play();
        wasWaiting = false;
      }
    };
    this.wasUsingAudioElement = false;
    this.onTimeUpdate = async () => {
      const quality = this.video.getVideoPlaybackQuality();
      if (quality.totalVideoFrames === 0) {
        const onError = this.onError.get(this.video);
        if (isDefined(onError)) {
          await onError();
        }
      } else if (this.useAudioElement) {
        this.wasUsingAudioElement = false;
        const delta3 = this.video.currentTime - this.audio.currentTime;
        if (Math.abs(delta3) > 0.25) {
          this.audio.currentTime = this.video.currentTime;
        }
      } else if (!this.wasUsingAudioElement) {
        this.wasUsingAudioElement = true;
        this.audio.pause();
      }
      this.progEvt.value = this.video.currentTime;
      this.progEvt.total = this.video.duration;
      this.dispatchEvent(this.progEvt);
    };
    this.video.addEventListener("seeked", this.onSeeked);
    this.video.addEventListener("play", this.onPlay);
    this.video.addEventListener("pause", this.onPause);
    this.video.addEventListener("ended", this.onPause);
    this.video.addEventListener("waiting", this.onWaiting);
    this.video.addEventListener("canplay", this.onCanPlay);
    this.video.addEventListener("timeupdate", this.onTimeUpdate);
    Object.assign(window, { videoPlayer: this });
  }
  elementHasAudio(elem) {
    const source = this.sourcesByURL.get(elem.src);
    return isDefined(source) && source.acodec !== "none" || isDefined(elem.audioTracks) && elem.audioTracks.length > 0 || isDefined(elem.webkitAudioDecodedByteCount) && elem.webkitAudioDecodedByteCount > 0 || isDefined(elem.mozHasAudio) && elem.mozHasAudio;
  }
  get useAudioElement() {
    return !this.elementHasAudio(this.video) && this.elementHasAudio(this.audio);
  }
  onDisposing() {
    super.onDisposing();
    this.clear();
    removeVertex(this.input);
    removeVertex(this.videoSource);
    removeVertex(this.audioSource);
    this.video.removeEventListener("seeked", this.onSeeked);
    this.video.removeEventListener("play", this.onPlay);
    this.video.removeEventListener("pause", this.onPause);
    this.video.removeEventListener("ended", this.onPause);
    this.video.removeEventListener("waiting", this.onWaiting);
    this.video.removeEventListener("canplay", this.onCanPlay);
    this.video.removeEventListener("timeupdate", this.onTimeUpdate);
  }
  clear() {
    this.stop();
    for (const [elem, onError] of this.onError) {
      elem.removeEventListener("error", onError);
    }
    this.onError.clear();
    this.sourcesByURL.clear();
    this.sources.clear();
    this.potatoes.clear();
    this.video.src = "";
    this.audio.src = "";
    this.wasUsingAudioElement = false;
    this._data = null;
    this._loaded = false;
  }
  async load(data, prog) {
    this.clear();
    this._data = data;
    if (isString(data)) {
      this.setTitle(data);
      this.potatoes.add(this.video, data);
    } else {
      this.setTitle(data.title);
      this.fillSources(this.video, data.videos);
      this.fillSources(this.audio, data.audios);
    }
    if (!this.hasSources(this.video)) {
      throw new Error("No video sources found");
    }
    this.dispatchEvent(this.loadingEvt);
    await progressTasks(prog, (prog2) => this.loadMediaElement(this.audio, prog2), (prog2) => this.loadMediaElement(this.video, prog2));
    if (!this.hasSources(this.video)) {
      throw new Error("No video playable sources");
    }
    this._loaded = true;
    this.dispatchEvent(this.loadEvt);
    return this;
  }
  fillSources(elem, formats) {
    arraySortByKeyInPlace(formats, (f) => -f.resolution);
    for (const format of formats) {
      if (!Video_Vendor_Mpeg_Dash_Mpd.matches(format.contentType)) {
        this.sources.add(elem, format);
        this.sourcesByURL.set(format.url, format);
      }
    }
  }
  createMediaElement(MediaElement, ...rest) {
    return MediaElement(playsInline(true), autoPlay(false), loop(false), ...rest);
  }
  async getMediaCapabilities(source) {
    const config = {
      type: "file"
    };
    if (isVideoRecord(source)) {
      config.video = {
        contentType: source.contentType,
        bitrate: source.vbr * 1024,
        framerate: source.fps,
        width: source.width,
        height: source.height
      };
    } else if (source.acodec !== "none") {
      config.audio = {
        contentType: source.contentType,
        bitrate: source.abr * 1024,
        samplerate: source.asr
      };
    }
    try {
      return await navigator.mediaCapabilities.decodingInfo(config);
    } catch {
      return {
        supported: true,
        powerEfficient: false,
        smooth: false,
        configuration: config
      };
    }
  }
  hasSources(elem) {
    return this.sources.get(elem).length > 0 || this.potatoes.count(elem) > 0;
  }
  async loadMediaElement(elem, prog) {
    if (isDefined(prog)) {
      prog.start();
    }
    if (this.onError.has(elem)) {
      elem.removeEventListener("error", this.onError.get(elem));
      this.onError.delete(elem);
    }
    while (this.hasSources(elem)) {
      let url = null;
      const source = this.sources.get(elem).shift();
      if (isDefined(source)) {
        const caps = await this.getMediaCapabilities(source);
        if (!caps.smooth || !caps.powerEfficient) {
          this.potatoes.add(elem, source.url);
          continue;
        } else {
          url = source.url;
        }
      } else {
        url = this.potatoes.get(elem).shift();
      }
      elem.src = url;
      elem.load();
      if (await mediaElementCanPlayThrough(elem)) {
        if (isDefined(source)) {
          this.sources.get(elem).unshift(source);
        } else {
          this.potatoes.get(elem).unshift(url);
        }
        const onError = () => this.loadMediaElement(elem, prog);
        elem.addEventListener("error", onError);
        this.onError.set(elem, onError);
        this.wasUsingAudioElement = this.wasUsingAudioElement;
        if (isDefined(prog)) {
          prog.end();
        }
        return;
      }
    }
  }
  get width() {
    return this.video.videoWidth;
  }
  get height() {
    return this.video.videoHeight;
  }
  get playbackState() {
    if (isNullOrUndefined(this.data)) {
      return "empty";
    }
    if (!this.loaded) {
      return "loading";
    }
    if (this.video.error) {
      return "errored";
    }
    if (this.video.ended || this.video.paused && this.video.currentTime === 0) {
      return "stopped";
    }
    if (this.video.paused) {
      return "paused";
    }
    return "playing";
  }
  play() {
    return this.video.play();
  }
  async playThrough() {
    const endTask = once(this, "stopped");
    await this.play();
    await endTask;
  }
  pause() {
    this.video.pause();
  }
  stop() {
    this.pause();
    this.video.currentTime = 0;
  }
  restart() {
    this.stop();
    return this.play();
  }
};

// ../threejs/CustomGeometry.ts
function normalizeQuad(quad) {
  return [
    normalizeTriangle([quad[1], quad[0], quad[2]]),
    normalizeTriangle([quad[2], quad[0], quad[3]])
  ];
}
function normalizeQuads(quads) {
  return quads.map(normalizeQuad).flat();
}
var A2 = new THREE.Vector3();
var B = new THREE.Vector3();
var C = new THREE.Vector3();
function normalizeTriangle(tria) {
  const positions = [
    [tria[0][0], tria[0][1], tria[0][2]],
    [tria[1][0], tria[1][1], tria[1][2]],
    [tria[2][0], tria[2][1], tria[2][2]]
  ];
  const uvs = [
    [tria[0][3], tria[0][4]],
    [tria[1][3], tria[1][4]],
    [tria[2][3], tria[2][4]]
  ];
  C.fromArray(positions[0]);
  A2.fromArray(positions[1]).sub(C);
  C.fromArray(positions[2]);
  B.fromArray(positions[1]).sub(C);
  A2.cross(B);
  const normal = A2.toArray();
  return {
    positions,
    uvs,
    normal
  };
}
function createGeometry(nFaces) {
  const positions = nFaces.map((f) => f.positions).flat(2);
  const uvs = nFaces.map((f) => f.uvs).flat(2);
  const normals = nFaces.flatMap((f) => f.normal);
  const geom2 = new THREE.BufferGeometry();
  geom2.setAttribute("position", new THREE.BufferAttribute(new Float32Array(positions), 3, false));
  geom2.setAttribute("uv", new THREE.BufferAttribute(new Float32Array(uvs), 2, false));
  geom2.setAttribute("normal", new THREE.BufferAttribute(new Float32Array(normals), 3, true));
  return geom2;
}
function createQuadGeometry(...quads) {
  const faces = normalizeQuads(quads);
  return createGeometry(faces);
}
function createEACGeometry(subDivs, ...quads) {
  let remappingQuads = mapEACSubdivision(quads);
  for (let i = 0; i < subDivs; ++i) {
    remappingQuads = subdivide(remappingQuads);
  }
  quads = unmapEACSubdivision(remappingQuads);
  const faces = normalizeQuads(quads);
  return createGeometry(faces);
}
function mapEACSubdivision(quads) {
  return quads.map((quad) => {
    let minU = Number.MAX_VALUE;
    let maxU = Number.MIN_VALUE;
    let minV = Number.MAX_VALUE;
    let maxV = Number.MIN_VALUE;
    for (const vert of quad) {
      const u = vert[3];
      const v = vert[4];
      minU = Math.min(minU, u);
      maxU = Math.max(maxU, u);
      minV = Math.min(minV, v);
      maxV = Math.max(maxV, v);
    }
    const minUV = [minU, minV];
    const deltaUV = [maxU - minU, maxV - minV];
    return {
      minUV,
      deltaUV,
      verts: [
        mapEACSubdivVert(minUV, deltaUV, quad[0]),
        mapEACSubdivVert(minUV, deltaUV, quad[1]),
        mapEACSubdivVert(minUV, deltaUV, quad[2]),
        mapEACSubdivVert(minUV, deltaUV, quad[3])
      ]
    };
  });
}
function mapEACSubdivVert(minUV, deltaUV, vert) {
  return {
    pos: [vert[0], vert[1], vert[2]],
    uv: [vert[3], vert[4]],
    pUV: [
      (vert[3] - minUV[0]) / deltaUV[0],
      (vert[4] - minUV[1]) / deltaUV[1]
    ]
  };
}
function unmapEACSubdivision(quadsx) {
  return quadsx.map((quadx) => [
    unmapEACSubdivVert(quadx, 0),
    unmapEACSubdivVert(quadx, 1),
    unmapEACSubdivVert(quadx, 2),
    unmapEACSubdivVert(quadx, 3)
  ]);
}
function unmapEACSubdivVert(quadx, i) {
  const vert = quadx.verts[i];
  return [
    vert.pos[0],
    vert.pos[1],
    vert.pos[2],
    vert.uv[0],
    vert.uv[1]
  ];
}
function subdivide(quadsx) {
  return quadsx.flatMap((quadx) => {
    const midU1 = midpoint(quadx, quadx.verts[0], quadx.verts[1]);
    const midU2 = midpoint(quadx, quadx.verts[2], quadx.verts[3]);
    const midV1 = midpoint(quadx, quadx.verts[0], quadx.verts[3]);
    const midV2 = midpoint(quadx, quadx.verts[1], quadx.verts[2]);
    const mid = midpoint(quadx, midU1, midU2);
    return [{
      minUV: quadx.minUV,
      deltaUV: quadx.deltaUV,
      verts: [quadx.verts[0], midU1, mid, midV1]
    }, {
      minUV: quadx.minUV,
      deltaUV: quadx.deltaUV,
      verts: [midU1, quadx.verts[1], midV2, mid]
    }, {
      minUV: quadx.minUV,
      deltaUV: quadx.deltaUV,
      verts: [mid, midV2, quadx.verts[2], midU2]
    }, {
      minUV: quadx.minUV,
      deltaUV: quadx.deltaUV,
      verts: [midV1, mid, midU2, quadx.verts[3]]
    }];
  });
}
function midpoint(quadx, from, to) {
  const dx = to.pos[0] - from.pos[0];
  const dy = to.pos[1] - from.pos[1];
  const dz = to.pos[2] - from.pos[2];
  const x = from.pos[0] + 0.5 * dx;
  const y = from.pos[1] + 0.5 * dy;
  const z = from.pos[2] + 0.5 * dz;
  const dpu = to.pUV[0] - from.pUV[0];
  const dpv = to.pUV[1] - from.pUV[1];
  const pu = from.pUV[0] + 0.5 * dpu;
  const pv = from.pUV[1] + 0.5 * dpv;
  const mu = mapEACUV(pu - 0.5) + 0.5;
  const mv = mapEACUV(pv - 0.5) + 0.5;
  const u = mu * quadx.deltaUV[0] + quadx.minUV[0];
  const v = mv * quadx.deltaUV[1] + quadx.minUV[1];
  return {
    pos: [x, y, z],
    pUV: [pu, pv],
    uv: [u, v]
  };
}
function mapEACUV(uv) {
  return 2 * Math.atan(2 * uv) / Math.PI;
}

// ../threejs/VideoPlayer3D.ts
var VideoPlayer3D = class extends BaseVideoPlayer {
  constructor(layerMgr, audioCtx) {
    super(audioCtx);
    this.material = solidTransparent({ name: "videoPlayer-material" });
    this.vidMeshes = [];
    for (let i = 0; i < 2; ++i) {
      const vidMesh = new Image2DMesh(layerMgr, `videoPlayer-view${i + 1}`, false, this.material);
      vidMesh.mesh.setImage(this.video);
      vidMesh.renderOrder = 4;
      if (i > 0) {
        vidMesh.mesh.layers.disable(0);
      } else {
        vidMesh.mesh.layers.enable(0);
      }
      this.vidMeshes.push(vidMesh);
    }
    this.object = obj("videoPlayer", ...this.vidMeshes);
  }
  onDisposing() {
    super.onDisposing();
    cleanup(this.object);
    arrayClear(this.vidMeshes);
  }
  update(dt, frame) {
    for (const vidMesh of this.vidMeshes) {
      vidMesh.update(dt, frame);
    }
  }
  isSupported(encoding, layout) {
    return layout.split("-").map((name2) => GeomPacks.has(encoding, name2)).reduce(and, true);
  }
  setStereoParameters(encoding, layout) {
    if (!this.isSupported(encoding, layout)) {
      throw new Error(`Not supported [encoding: ${encoding}, layout: ${layout}]`);
    }
    for (let i = 0; i < this.vidMeshes.length; ++i) {
      const vidMesh = this.vidMeshes[i];
      vidMesh.webXRLayersEnabled = false;
      vidMesh.mesh.layers.disable(1);
      vidMesh.mesh.layers.disable(2);
      if (layout === "left-right" || layout === "top-bottom") {
        vidMesh.mesh.layers.enable(this.vidMeshes.length - i);
      } else if (layout !== "mono") {
        vidMesh.mesh.layers.enable(i + 1);
      }
    }
    const aspect = this.height / this.width;
    if (encoding !== "N/A") {
      this.vidMeshes[0].scale.set(100, 100, 100);
    } else if (layout === "mono") {
      this.vidMeshes[0].scale.set(1, aspect, 1);
    } else if (layout === "left-right" || layout === "right-left") {
      this.vidMeshes[0].scale.set(1, 2 * aspect, 1);
    } else {
      this.vidMeshes[0].scale.set(1, 0.5 * aspect, 1);
    }
    for (let i = 1; i < this.vidMeshes.length; ++i) {
      this.vidMeshes[i].scale.copy(this.vidMeshes[0].scale);
    }
    const names2 = layout.split("-");
    for (let i = 0; i < names2.length; ++i) {
      const name2 = names2[i];
      const geom2 = GeomPacks.get(encoding, name2);
      const vidMesh = this.vidMeshes[i];
      vidMesh.webXRLayersEnabled = true;
      vidMesh.visible = true;
      if (vidMesh.mesh.geometry !== geom2) {
        vidMesh.mesh.geometry.dispose();
        vidMesh.mesh.geometry = geom2;
      }
    }
  }
};
var PlaneGeom_Mono = createQuadGeometry([
  [-1 / 2, 1 / 2, 0, 0, 1],
  [1 / 2, 1 / 2, 0, 1, 1],
  [1 / 2, -1 / 2, 0, 1, 0],
  [-1 / 2, -1 / 2, 0, 0, 0]
]);
var PlaneDef_Left = [
  [-1 / 2, 1 / 2, 0, 0, 1],
  [1 / 2, 1 / 2, 0, 0.5, 1],
  [1 / 2, -1 / 2, 0, 0.5, 0],
  [-1 / 2, -1 / 2, 0, 0, 0]
];
var PlanDef_Right = [
  [-1 / 2, 1 / 2, 0, 0.5, 1],
  [1 / 2, 1 / 2, 0, 1, 1],
  [1 / 2, -1 / 2, 0, 1, 0],
  [-1 / 2, -1 / 2, 0, 0.5, 0]
];
var CubeStripDef_Mono = [[
  [-1 / 2, 1 / 2, -1 / 2, 1 / 3, 1],
  [1 / 2, 1 / 2, -1 / 2, 2 / 3, 1],
  [1 / 2, -1 / 2, -1 / 2, 2 / 3, 1 / 2],
  [-1 / 2, -1 / 2, -1 / 2, 1 / 3, 1 / 2]
], [
  [1 / 2, 1 / 2, -1 / 2, 2 / 3, 1],
  [1 / 2, 1 / 2, 1 / 2, 1, 1],
  [1 / 2, -1 / 2, 1 / 2, 1, 1 / 2],
  [1 / 2, -1 / 2, -1 / 2, 2 / 3, 1 / 2]
], [
  [-1 / 2, 1 / 2, 1 / 2, 0, 1],
  [-1 / 2, 1 / 2, -1 / 2, 1 / 3, 1],
  [-1 / 2, -1 / 2, -1 / 2, 1 / 3, 1 / 2],
  [-1 / 2, -1 / 2, 1 / 2, 0, 1 / 2]
], [
  [1 / 2, 1 / 2, 1 / 2, 2 / 3, 1 / 2],
  [-1 / 2, 1 / 2, 1 / 2, 2 / 3, 0],
  [-1 / 2, -1 / 2, 1 / 2, 1 / 3, 0],
  [1 / 2, -1 / 2, 1 / 2, 1 / 3, 1 / 2]
], [
  [1 / 2, 1 / 2, -1 / 2, 1, 1 / 2],
  [-1 / 2, 1 / 2, -1 / 2, 1, 0],
  [-1 / 2, 1 / 2, 1 / 2, 2 / 3, 0],
  [1 / 2, 1 / 2, 1 / 2, 2 / 3, 1 / 2]
], [
  [1 / 2, -1 / 2, 1 / 2, 1 / 3, 1 / 2],
  [-1 / 2, -1 / 2, 1 / 2, 1 / 3, 0],
  [-1 / 2, -1 / 2, -1 / 2, 0, 0],
  [1 / 2, -1 / 2, -1 / 2, 0, 1 / 2]
]];
var CubeStripDef_Left = [[
  [-1 / 2, 1 / 2, -1 / 2, 0, 1 / 3],
  [1 / 2, 1 / 2, -1 / 2, 0, 2 / 3],
  [1 / 2, -1 / 2, -1 / 2, 1 / 4, 2 / 3],
  [-1 / 2, -1 / 2, -1 / 2, 1 / 4, 1 / 3]
], [
  [1 / 2, 1 / 2, -1 / 2, 0, 2 / 3],
  [1 / 2, 1 / 2, 1 / 2, 0, 1],
  [1 / 2, -1 / 2, 1 / 2, 1 / 4, 1],
  [1 / 2, -1 / 2, -1 / 2, 1 / 4, 2 / 3]
], [
  [-1 / 2, 1 / 2, 1 / 2, 0, 0],
  [-1 / 2, 1 / 2, -1 / 2, 0, 1 / 3],
  [-1 / 2, -1 / 2, -1 / 2, 1 / 4, 1 / 3],
  [-1 / 2, -1 / 2, 1 / 2, 1 / 4, 0]
], [
  [1 / 2, 1 / 2, 1 / 2, 1 / 4, 2 / 3],
  [-1 / 2, 1 / 2, 1 / 2, 1 / 2, 2 / 3],
  [-1 / 2, -1 / 2, 1 / 2, 1 / 2, 1 / 3],
  [1 / 2, -1 / 2, 1 / 2, 1 / 4, 1 / 3]
], [
  [1 / 2, 1 / 2, -1 / 2, 1 / 4, 1],
  [-1 / 2, 1 / 2, -1 / 2, 1 / 2, 1],
  [-1 / 2, 1 / 2, 1 / 2, 1 / 2, 2 / 3],
  [1 / 2, 1 / 2, 1 / 2, 1 / 4, 2 / 3]
], [
  [1 / 2, -1 / 2, 1 / 2, 1 / 4, 1 / 3],
  [-1 / 2, -1 / 2, 1 / 2, 1 / 2, 1 / 3],
  [-1 / 2, -1 / 2, -1 / 2, 1 / 2, 0],
  [1 / 2, -1 / 2, -1 / 2, 1 / 4, 0]
]];
var CubeStripDef_Right = [[
  [-1 / 2, 1 / 2, -1 / 2, 1 / 2, 1 / 3],
  [1 / 2, 1 / 2, -1 / 2, 1 / 2, 2 / 3],
  [1 / 2, -1 / 2, -1 / 2, 3 / 4, 2 / 3],
  [-1 / 2, -1 / 2, -1 / 2, 3 / 4, 1 / 3]
], [
  [1 / 2, 1 / 2, -1 / 2, 1 / 2, 2 / 3],
  [1 / 2, 1 / 2, 1 / 2, 1 / 2, 1],
  [1 / 2, -1 / 2, 1 / 2, 3 / 4, 1],
  [1 / 2, -1 / 2, -1 / 2, 3 / 4, 2 / 3]
], [
  [-1 / 2, 1 / 2, 1 / 2, 1 / 2, 0],
  [-1 / 2, 1 / 2, -1 / 2, 1 / 2, 1 / 3],
  [-1 / 2, -1 / 2, -1 / 2, 3 / 4, 1 / 3],
  [-1 / 2, -1 / 2, 1 / 2, 3 / 4, 0]
], [
  [1 / 2, 1 / 2, 1 / 2, 3 / 4, 2 / 3],
  [-1 / 2, 1 / 2, 1 / 2, 1, 2 / 3],
  [-1 / 2, -1 / 2, 1 / 2, 1, 1 / 3],
  [1 / 2, -1 / 2, 1 / 2, 3 / 4, 1 / 3]
], [
  [1 / 2, 1 / 2, -1 / 2, 3 / 4, 1],
  [-1 / 2, 1 / 2, -1 / 2, 1, 1],
  [-1 / 2, 1 / 2, 1 / 2, 1, 2 / 3],
  [1 / 2, 1 / 2, 1 / 2, 3 / 4, 2 / 3]
], [
  [1 / 2, -1 / 2, 1 / 2, 3 / 4, 1 / 3],
  [-1 / 2, -1 / 2, 1 / 2, 1, 1 / 3],
  [-1 / 2, -1 / 2, -1 / 2, 1, 0],
  [1 / 2, -1 / 2, -1 / 2, 3 / 4, 0]
]];
var PlaneGeom_Left = createQuadGeometry(PlaneDef_Left);
var PlaneGeom_Right = createQuadGeometry(PlanDef_Right);
var PlaneGeom_Top = createQuadGeometry(rotQuad(PlaneDef_Left));
var PlaneGeom_Bottom = createQuadGeometry(rotQuad(PlanDef_Right));
var CubeStripDef_Top = rot(CubeStripDef_Left);
var CubeStripDef_Bottom = rot(CubeStripDef_Right);
var CubeStripGeom_Mono = createQuadGeometry(...CubeStripDef_Mono);
var CubeStripGeom_Left = createQuadGeometry(...CubeStripDef_Left);
var CubeStripGeom_Right = createQuadGeometry(...CubeStripDef_Right);
var CubeStripGeom_Top = createQuadGeometry(...CubeStripDef_Top);
var CubeStripGeom_Bottom = createQuadGeometry(...CubeStripDef_Bottom);
var EACSubDivisions = 4;
var EACGeom_Mono = createEACGeometry(EACSubDivisions, ...CubeStripDef_Mono);
var EACGeom_Left = createEACGeometry(EACSubDivisions, ...CubeStripDef_Left);
var EACGeom_Right = createEACGeometry(EACSubDivisions, ...CubeStripDef_Right);
var EACGeom_Top = createEACGeometry(EACSubDivisions, ...CubeStripDef_Top);
var EACGeom_Bottom = createEACGeometry(EACSubDivisions, ...CubeStripDef_Bottom);
var GeomPacks = new PriorityMap([
  ["N/A", "mono", PlaneGeom_Mono],
  ["N/A", "left", PlaneGeom_Left],
  ["N/A", "right", PlaneGeom_Right],
  ["N/A", "top", PlaneGeom_Top],
  ["N/A", "bottom", PlaneGeom_Bottom],
  ["Cubemap Strips", "mono", CubeStripGeom_Mono],
  ["Cubemap Strips", "left", CubeStripGeom_Left],
  ["Cubemap Strips", "right", CubeStripGeom_Right],
  ["Cubemap Strips", "top", CubeStripGeom_Top],
  ["Cubemap Strips", "bottom", CubeStripGeom_Bottom],
  ["Equi-Angular Cubemap (YouTube)", "mono", EACGeom_Mono],
  ["Equi-Angular Cubemap (YouTube)", "left", EACGeom_Left],
  ["Equi-Angular Cubemap (YouTube)", "right", EACGeom_Right],
  ["Equi-Angular Cubemap (YouTube)", "top", EACGeom_Top],
  ["Equi-Angular Cubemap (YouTube)", "bottom", EACGeom_Bottom]
]);
function rotVert(vert) {
  return [vert[0], vert[1], vert[2], vert[4], 1 - vert[3]];
}
function rotQuad(quad) {
  return [
    rotVert(quad[0]),
    rotVert(quad[1]),
    rotVert(quad[2]),
    rotVert(quad[3])
  ];
}
function rot(def) {
  return def.map(rotQuad);
}

// ../threejs/Collider.ts
var Collider = class extends THREE.Mesh {
  constructor(geometry) {
    super(geometry, solidTransparentBlack(0));
    this.isCollider = true;
    this.visible = false;
  }
};

// ../threejs/MeshLabel.ts
var MeshLabel = class extends THREE.Object3D {
  constructor(name2, geometry, enabledMaterial, disabledMaterial, size) {
    super();
    this._disabled = false;
    const id2 = stringRandom(16);
    this.name = name2 + id2;
    this.enabledMesh = this.createMesh(`${this.name}-enabled`, geometry, enabledMaterial);
    this.disabledMesh = this.createMesh(`${this.name}-disabled`, geometry, disabledMaterial);
    this.disabledMesh.visible = false;
    this.size = size;
    this.add(this.enabledMesh, this.disabledMesh);
  }
  get size() {
    return this.enabledMesh.scale.x;
  }
  set size(v) {
    this.enabledMesh.scale.setScalar(v);
    this.disabledMesh.scale.setScalar(v);
  }
  createMesh(id2, geometry, material) {
    const mesh = new THREE.Mesh(geometry, material);
    mesh.name = "Mesh-" + id2;
    return mesh;
  }
  get disabled() {
    return this._disabled;
  }
  set disabled(v) {
    if (v !== this.disabled) {
      this._disabled = v;
      this.enabledMesh.visible = !v;
      this.disabledMesh.visible = v;
    }
  }
};

// ../threejs/MeshButton.ts
var MeshButton = class extends MeshLabel {
  constructor(name2, geometry, enabledMaterial, disabledMaterial, size) {
    super(name2, geometry, enabledMaterial, disabledMaterial, size);
    this.isDraggable = false;
    this.isClickable = true;
    this.collider = new Collider(geometry);
    this.collider.name = `Collider-${this.name}`;
    this.size = size;
    this.add(this.collider);
    scaleOnHover(this);
  }
  set size(v) {
    super.size = v;
    if (this.collider) {
      this.collider.scale.setScalar(v);
    }
  }
};

// ../threejs/widgets/ButtonImageWidget.ts
var ButtonImageWidget = class {
  constructor(buttons, setName, iconName) {
    this.mesh = null;
    this.element = ButtonPrimary(title(iconName), buttons.getImageElement(setName, iconName));
    this.object = obj(`${name}-button`);
    this.load(buttons, setName, iconName);
  }
  async load(buttons, setName, iconName) {
    const { geometry, enabledMaterial, disabledMaterial } = await buttons.getGeometryAndMaterials(setName, iconName);
    this.mesh = new MeshButton(iconName, geometry, enabledMaterial, disabledMaterial, 0.2);
    this.object.add(this.mesh);
    this.mesh.visible = this.visible;
    this.mesh.addEventListener("click", () => {
      this.element.click();
    });
  }
  get name() {
    return this.object.name;
  }
  addEventListener(type2, listener, options) {
    this.element.addEventListener(type2, listener, options);
  }
  dispatchEvent(event) {
    return this.element.dispatchEvent(event);
  }
  removeEventListener(type2, callback, options) {
    this.element.removeEventListener(type2, callback, options);
  }
  click() {
    this.element.click();
  }
  get visible() {
    return elementIsDisplayed(this);
  }
  set visible(visible) {
    elementSetDisplay(this, visible, "inline-block");
    if (this.mesh) {
      this.mesh.visible = visible;
    }
  }
};

// ../threejs/widgets/CanvasImageMesh.ts
var CanvasImageMesh = class extends Image2DMesh {
  constructor(env, name2, image2) {
    super(env, name2, false);
    this.image = image2;
    if (this.mesh) {
      this.setImage(image2);
    }
  }
  get object() {
    return this;
  }
  setImage(image2) {
    this.mesh.setImage(image2.canvas);
    this.mesh.objectHeight = 0.1;
    this.mesh.updateTexture();
    image2.addEventListener("redrawn", () => this.mesh.updateTexture());
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this.setImage(source.image);
    return this;
  }
  get element() {
    if (isHTMLCanvas(this.image.canvas)) {
      return this.image.canvas;
    } else {
      return null;
    }
  }
  get isVisible() {
    return elementIsDisplayed(this);
  }
  set isVisible(v) {
    elementSetDisplay(this, v, "inline-block");
    objectSetVisible(this, v);
    objectSetVisible(this.mesh, v);
    this.image.visible = v;
  }
};

// ../dom/elementSetClass.ts
function elementSetClass(elem, enabled, className2) {
  const canEnable = isDefined(className2);
  const hasEnabled = canEnable && elem.classList.contains(className2);
  if (canEnable && hasEnabled !== enabled) {
    if (enabled) {
      elem.classList.add(className2);
    } else {
      elem.classList.remove(className2);
    }
  }
}

// ../dom/buttonSetEnabled.ts
var types = [
  "danger",
  "dark",
  "info",
  "light",
  "primary",
  "secondary",
  "success",
  "warning"
];
function buttonSetEnabled(button, enabled, btnType, label) {
  if (isErsatzElement(button)) {
    button = button.element;
  }
  if (isString(btnType)) {
    for (const type2 of types) {
      elementSetClass(button, enabled && type2 === btnType, `btn-${type2}`);
      elementSetClass(button, !enabled && type2 === btnType, `btn-outline-${type2}`);
    }
  }
  if (isDisableable(button)) {
    button.disabled = !enabled;
  }
  if (label) {
    elementSetText(button, label);
  }
}

// ../threejs/widgets/ToggleButton.ts
var ToggleButton = class {
  constructor(buttons, setName, activeName, inactiveName) {
    this.buttons = buttons;
    this.setName = setName;
    this.activeName = activeName;
    this.inactiveName = inactiveName;
    this.enterButton = null;
    this.exitButton = null;
    this._isAvailable = true;
    this._isEnabled = true;
    this._isVisible = true;
    this._isActive = false;
    this.element = ButtonPrimary(this.btnImage = Img());
    this.object = obj(`${this.setName}-button`);
    this.load();
  }
  get name() {
    return this.object.name;
  }
  addEventListener(type2, listener, options) {
    this.element.addEventListener(type2, listener, options);
  }
  dispatchEvent(event) {
    return this.element.dispatchEvent(event);
  }
  removeEventListener(type2, callback, options) {
    this.element.removeEventListener(type2, callback, options);
  }
  click() {
    this.element.click();
  }
  async load() {
    const [activate, deactivate] = await Promise.all([
      this.buttons.getGeometryAndMaterials(this.setName, this.activeName),
      this.buttons.getGeometryAndMaterials(this.setName, this.inactiveName)
    ]);
    objGraph(this.object, this.enterButton = new MeshButton(`${this.setName}-activate-button`, activate.geometry, activate.enabledMaterial, activate.disabledMaterial, 0.2), this.exitButton = new MeshButton(`${this.setName}-deactivate-button`, deactivate.geometry, deactivate.enabledMaterial, deactivate.disabledMaterial, 0.2));
    this.enterButton.addEventListener("click", () => this.element.click());
    this.exitButton.addEventListener("click", () => this.element.click());
    this.refreshState();
  }
  get mesh() {
    return this.active ? this.enterButton : this.exitButton;
  }
  get available() {
    return this._isAvailable;
  }
  set available(v) {
    this._isAvailable = v;
    this.refreshState();
  }
  get active() {
    return this._isActive;
  }
  set active(v) {
    this._isActive = v;
    this.refreshState();
  }
  get enabled() {
    return this._isEnabled;
  }
  set enabled(v) {
    this._isEnabled = v;
    this.refreshState();
  }
  get visible() {
    return this._isVisible;
  }
  set visible(v) {
    this._isVisible = v;
    this.refreshState();
  }
  refreshState() {
    const type2 = this.active ? this.inactiveName : this.activeName;
    const text = `${type2} ${this.setName}`;
    this.element.title = this.btnImage.title = text;
    this.btnImage.src = this.buttons.getImageSrc(this.setName, type2);
    buttonSetEnabled(this, this.available && this.visible && this.enabled, "primary");
    elementSetDisplay(this, this.available && this.visible, "inline-block");
    if (this.enterButton && this.exitButton) {
      objectSetEnabled(this, this.available && this.visible && this.enabled);
      const visible = objectSetVisible(this.object, this.available && this.visible);
      objectSetVisible(this.enterButton, visible && !this.active);
      objectSetVisible(this.exitButton, visible && this.active);
    }
  }
};

// ../threejs/widgets/ScreenModeToggleButton.ts
var ScreenModeToggleButton = class extends ToggleButton {
  constructor(buttons, mode) {
    const name2 = ScreenMode[mode];
    super(buttons, name2.toLowerCase(), "enter", "exit");
    this.mode = mode;
  }
};

// ../threejs/widgets/widgets.ts
function widgetSetEnabled(obj2, enabled, buttonType) {
  if (obj2.element instanceof HTMLButtonElement) {
    buttonSetEnabled(obj2, enabled, buttonType);
  }
  objectSetEnabled(obj2, enabled);
}

// ../threejs/environment/ApplicationLoader.ts
var ApplicationLoaderEvent = class extends TypedEvent {
  constructor(type2, appName) {
    super(type2);
    this.appName = appName;
  }
};
var ApplicationLoaderAppLoadingEvent = class extends ApplicationLoaderEvent {
  constructor(appName, appLoadParams) {
    super("apploading", appName);
    this.appLoadParams = appLoadParams;
    this.preLoadTask = null;
  }
};
var ApplicationLoaderAppLoadedEvent = class extends ApplicationLoaderEvent {
  constructor(appName, app) {
    super("apploaded", appName);
    this.app = app;
  }
};
var ApplicationLoaderAppShownEvent = class extends ApplicationLoaderEvent {
  constructor(appName, app) {
    super("appshown", appName);
    this.app = app;
  }
};
var ApplicationLoadRequest = class {
  constructor(loader, name2) {
    this.loader = loader;
    this.name = name2;
    this.params = /* @__PURE__ */ new Map();
  }
  param(name2, value2) {
    this.params.set(name2, value2);
    return this;
  }
  load(prog) {
    return this.loader(this.name, this.params, prog);
  }
};
var ApplicationLoader = class extends TypedEventBase {
  constructor(env, JS_EXT) {
    super();
    this.env = env;
    this.JS_EXT = JS_EXT;
    this.loadedModules = /* @__PURE__ */ new Map();
    this.loadingApps = /* @__PURE__ */ new Map();
    this.currentApps = /* @__PURE__ */ new Map();
    this.cacheBustString = null;
  }
  [Symbol.iterator]() {
    return this.currentApps.values();
  }
  isLoaded(name2) {
    return this.currentApps.has(name2);
  }
  get(name2) {
    return this.currentApps.get(name2);
  }
  async loadAppConstructor(name2, prog) {
    if (!this.loadedModules.has(name2)) {
      let url = `/js/${name2}/index${this.JS_EXT}`;
      if (isDefined(this.cacheBustString)) {
        url += "#" + this.cacheBustString;
      }
      const task = this.env.fetcher.get(url).progress(prog).module();
      this.loadedModules.set(name2, task);
    } else if (isDefined(prog)) {
      prog.end();
    }
    const { default: AppConstructor } = await this.loadedModules.get(name2);
    return AppConstructor;
  }
  app(name2) {
    return new ApplicationLoadRequest(this.loadApp.bind(this), name2);
  }
  loadApp(name2, paramsOrProg, prog) {
    let params = null;
    if (paramsOrProg instanceof Map) {
      params = paramsOrProg;
    } else {
      prog = paramsOrProg;
    }
    prog = prog || this.env.loadingBar;
    const evt = new ApplicationLoaderAppLoadingEvent(name2, params);
    this.dispatchEvent(evt);
    const preLoadTask = evt.preLoadTask || Promise.resolve();
    if (!this.loadingApps.has(name2)) {
      const progs = progressPopper(prog);
      const appTask2 = preLoadTask.then(() => this.loadAppInstance(this.env, name2, params, progs.pop(10)));
      this.loadingApps.set(name2, appTask2);
      prog = progs.pop(1);
    }
    const appTask = this.loadingApps.get(name2);
    return appTask.then(async (app) => {
      await app.show(prog);
      this.dispatchEvent(new ApplicationLoaderAppShownEvent(name2, app));
      return app;
    });
  }
  async loadAppInstance(env, name2, params, prog) {
    if (!this.currentApps.has(name2)) {
      const [appLoad, assetLoad] = progressSplitWeighted(prog, [1, 10]);
      const App = await this.loadAppConstructor(name2, appLoad);
      const app = new App(env);
      app.addEventListener("quit", () => this.unloadApp(name2));
      if (isDefined(params)) {
        await app.init(params);
      }
      await app.load(assetLoad);
      this.currentApps.set(name2, app);
      this.dispatchEvent(new ApplicationLoaderAppLoadedEvent(name2, app));
    }
    if (isDefined(prog)) {
      prog.end();
    }
    return this.currentApps.get(name2);
  }
  unloadApp(name2) {
    const app = this.currentApps.get(name2);
    app.clearEventListeners();
    app.dispose();
    this.currentApps.delete(name2);
    this.loadingApps.delete(name2);
  }
};

// ../threejs/animation/lookAngles.ts
var D = new THREE.Vector3();
function getLookHeading(dir) {
  D.copy(dir);
  D.y = 0;
  D.normalize();
  return angleClamp(Math.atan2(D.x, D.z));
}
function getLookPitch(dir) {
  return angleClamp(Math.asin(dir.y));
}

// ../threejs/animation/BodyFollower.ts
var targetPos = new THREE.Vector3();
var targetAngle = 0;
var dPos = new THREE.Vector3();
var curPos = new THREE.Vector3();
var curDir = new THREE.Vector3();
var dQuat = new THREE.Quaternion();
var curAngle = 0;
var copyCounter2 = 0;
function minRotAngle(to, from) {
  const a = to - from;
  const b = a + 2 * Math.PI;
  const c = a - 2 * Math.PI;
  const A3 = Math.abs(a);
  const B3 = Math.abs(b);
  const C2 = Math.abs(c);
  if (A3 < B3 && A3 < C2) {
    return a;
  } else if (B3 < C2) {
    return b;
  } else {
    return c;
  }
}
var BodyFollower = class extends THREE.Object3D {
  constructor(name2, minDistance, minAngle, heightOffset, speed = 1) {
    super();
    this.minDistance = minDistance;
    this.heightOffset = heightOffset;
    this.speed = speed;
    this.name = name2;
    this.lerp = this.minAngle > 0 || this.minDistance > 0;
    this.maxDistance = this.minDistance * 5;
    this.minAngle = deg2rad(minAngle);
    this.maxAngle = Math.PI - this.minAngle;
    Object.seal(this);
  }
  copy(source, recursive = true) {
    super.copy(source, recursive);
    this.name = source.name + ++copyCounter2;
    this.lerp = source.lerp;
    this.maxDistance = source.maxDistance;
    this.minAngle = source.minAngle;
    this.maxAngle = source.maxAngle;
    return this;
  }
  update(height2, position2, angle3, dt) {
    dt *= 1e-3;
    this.clampTo(this.lerp, height2, position2, this.minDistance, this.maxDistance, angle3, this.minAngle, this.maxAngle, dt);
  }
  reset(height2, position2, angle3) {
    this.clampTo(false, height2, position2, 0, 0, angle3, 0, 0, 0);
  }
  clampTo(lerp4, height2, position2, minDistance, maxDistance, angle3, minAngle, maxAngle, dt) {
    targetPos.copy(position2);
    targetPos.y -= this.heightOffset * height2;
    targetAngle = angle3;
    this.getWorldPosition(curPos);
    this.getWorldDirection(curDir);
    curAngle = getLookHeading(curDir);
    dQuat.identity();
    let setPos = !lerp4;
    let setRot = !lerp4;
    if (lerp4) {
      const dist3 = dPos.copy(targetPos).sub(curPos).length();
      if (minDistance < dist3) {
        if (dist3 < maxDistance) {
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

// ../dom/evts.ts
function isModifierless(evt) {
  return !(evt.shiftKey || evt.altKey || evt.ctrlKey || evt.metaKey);
}
var HtmlEvt = class {
  constructor(name2, callback, opts) {
    this.name = name2;
    this.callback = callback;
    if (!isFunction(callback)) {
      throw new Error("A function instance is required for this parameter");
    }
    this.opts = opts;
    Object.freeze(this);
  }
  opts;
  applyToElement(elem) {
    this.add(elem);
  }
  add(elem) {
    elem.addEventListener(this.name, this.callback, this.opts);
  }
  remove(elem) {
    elem.removeEventListener(this.name, this.callback);
  }
};
function onClick(callback, opts) {
  return new HtmlEvt("click", callback, opts);
}
function onInput(callback, opts) {
  return new HtmlEvt("input", callback, opts);
}

// ../event-system/AvatarMovedEvent.ts
var AvatarMovedEvent = class extends TypedEvent {
  px = 0;
  py = 0;
  pz = 0;
  fx = 0;
  fy = 0;
  fz = 0;
  ux = 0;
  uy = 0;
  uz = 0;
  height = 0;
  name = 0 /* LocalUser */;
  constructor() {
    super("avatarmoved");
  }
  set(px, py, pz, fx, fy, fz, ux, uy, uz, height2) {
    this.px = px;
    this.py = py;
    this.pz = pz;
    this.fx = fx;
    this.fy = fy;
    this.fz = fz;
    this.ux = ux;
    this.uy = uy;
    this.uz = uz;
    this.height = height2;
  }
};

// ../threejs/resolveCamera.ts
function resolveCamera(renderer, camera) {
  if (renderer.xr.isPresenting) {
    return renderer.xr.getCamera();
  } else {
    return camera;
  }
}

// ../threejs/setRightUpFwdPosFromMatrix.ts
function setRightUpFwdPosFromMatrix(matrix, R2, U3, F2, P4) {
  const m = matrix.elements;
  R2.set(m[0], m[1], m[2]);
  U3.set(m[4], m[5], m[6]);
  F2.set(-m[8], -m[9], -m[10]);
  P4.set(m[12], m[13], m[14]);
  R2.normalize();
  U3.normalize();
  F2.normalize();
}

// ../threejs/AvatarLocal.ts
var MOUSE_SENSITIVITY_SCALE = 100;
var TOUCH_SENSITIVITY_SCALE = 50;
var GAMEPAD_SENSITIVITY_SCALE = 1;
var MOTIONCONTROLLER_STICK_SENSITIVITY_SCALE = Math.PI / 3;
var B2 = new THREE.Vector3(0, 0, 1);
var R = new THREE.Vector3();
var F = new THREE.Vector3();
var U = new THREE.Vector3();
var P3 = new THREE.Vector3();
var M2 = new THREE.Matrix4();
var E = new THREE.Euler();
var Q1 = new THREE.Quaternion();
var Q2 = new THREE.Quaternion();
var Q3 = new THREE.Quaternion(-Math.sqrt(0.5), 0, 0, Math.sqrt(0.5));
var motion = new THREE.Vector3();
var deltaQuat = new THREE.Quaternion();
var nextFlick = new THREE.Vector3();
var rotStage = new THREE.Matrix4();
var userMovedEvt = new AvatarMovedEvent();
function isPermissionedDeviceOrientationEvent(obj2) {
  return obj2 === DeviceOrientationEvent && "requestPermission" in obj2 && isFunction(obj2.requestPermission);
}
var AvatarLocal = class extends TypedEventBase {
  constructor(renderer, camera, fader, defaultAvatarHeight) {
    super();
    this.renderer = renderer;
    this.camera = camera;
    this.controlMode = "none" /* None */;
    this.requiredMouseButton = /* @__PURE__ */ new Map([
      ["mousedrag" /* MouseDrag */, 1 /* Mouse0 */],
      ["touchswipe" /* Touch */, 1 /* Mouse0 */]
    ]);
    this._heading = 0;
    this._pitch = 0;
    this._roll = 0;
    this.fwrd = false;
    this.back = false;
    this.left = false;
    this.rgth = false;
    this.up = false;
    this.down = false;
    this.grow = false;
    this.shrk = false;
    this.viewEuler = new THREE.Euler();
    this.move = new THREE.Vector3();
    this.followers = new Array();
    this.worldPos = new THREE.Vector3();
    this._worldHeading = 0;
    this._worldPitch = 0;
    this.evtSys = null;
    this.requiredTouchCount = 1;
    this.disableHorizontal = false;
    this.disableVertical = false;
    this.invertHorizontal = false;
    this.invertVertical = true;
    this.minimumX = -85 * Math.PI / 180;
    this.maximumX = 85 * Math.PI / 180;
    this.target = new THREE.Quaternion(0, 0, 0, 1);
    this.edgeFactor = 1 / 3;
    this.accelerationX = 2;
    this.accelerationY = 2;
    this.speedX = 3;
    this.speedY = 2;
    this.deviceQ = new THREE.Quaternion().identity();
    this.u = 0;
    this.v = 0;
    this.du = 0;
    this.dv = 0;
    this._keyboardControlEnabled = false;
    this.deviceOrientation = null;
    this.screenOrientation = 0;
    this.alphaOffset = 0;
    this.onDeviceOrientationChangeEvent = null;
    this.onScreenOrientationChangeEvent = null;
    this.motionEnabled = false;
    this._height = defaultAvatarHeight;
    this.head = obj("Head", fader);
    this.onKeyDown = (evt) => {
      const ok = isModifierless(evt);
      if (evt.key === "w")
        this.fwrd = ok;
      if (evt.key === "s")
        this.back = ok;
      if (evt.key === "a")
        this.left = ok;
      if (evt.key === "d")
        this.rgth = ok;
      if (evt.key === "e")
        this.up = ok;
      if (evt.key === "q")
        this.down = ok;
      if (evt.key === "r")
        this.grow = ok;
      if (evt.key === "f")
        this.shrk = ok;
    };
    this.onKeyUp = (evt) => {
      if (evt.key === "w")
        this.fwrd = false;
      if (evt.key === "s")
        this.back = false;
      if (evt.key === "a")
        this.left = false;
      if (evt.key === "d")
        this.rgth = false;
      if (evt.key === "e")
        this.up = false;
      if (evt.key === "q")
        this.down = false;
      if (evt.key === "r")
        this.grow = false;
      if (evt.key === "f")
        this.shrk = false;
    };
    this.keyboardControlEnabled = true;
    if (isMobileVR()) {
      this.controlMode = "motioncontroller" /* MotionControllerStick */;
    } else if (matchMedia("(pointer: coarse)").matches) {
      this.controlMode = "touchswipe" /* Touch */;
    } else if (matchMedia("(pointer: fine)").matches) {
      this.controlMode = "mousedrag" /* MouseDrag */;
    }
    if (globalThis.isSecureContext && isMobile() && !isMobileVR()) {
      this.onDeviceOrientationChangeEvent = (event) => {
        this.deviceOrientation = event;
        if (event && (event.alpha || event.beta || event.gamma) && this.motionEnabled) {
          this.controlMode = "magicwindow" /* MagicWindow */;
        }
      };
      this.onScreenOrientationChangeEvent = () => {
        if (!isString(globalThis.orientation)) {
          this.screenOrientation = globalThis.orientation || 0;
        }
      };
      this.startMotionControl();
    }
  }
  get worldHeading() {
    return this._worldHeading;
  }
  get worldPitch() {
    return this._worldPitch;
  }
  get height() {
    return this.head.position.y;
  }
  get object() {
    return this.head;
  }
  get stage() {
    return this.head.parent;
  }
  onFlick(direction) {
    nextFlick.set(MOTIONCONTROLLER_STICK_SENSITIVITY_SCALE * direction, 0, 0);
  }
  get keyboardControlEnabled() {
    return this._keyboardControlEnabled;
  }
  set keyboardControlEnabled(v) {
    if (this._keyboardControlEnabled !== v) {
      this._keyboardControlEnabled = v;
      if (this._keyboardControlEnabled) {
        globalThis.addEventListener("keydown", this.onKeyDown);
        globalThis.addEventListener("keyup", this.onKeyUp);
      } else {
        globalThis.removeEventListener("keydown", this.onKeyDown);
        globalThis.removeEventListener("keyup", this.onKeyUp);
      }
    }
  }
  addFollower(follower) {
    this.followers.push(follower);
  }
  onDown(evt) {
    if (evt.pointer.enabled) {
      this.setMode(evt);
    }
  }
  onMove(evt) {
    if (evt.pointer.enabled) {
      this.setMode(evt);
      if (evt.pointer.canMoveView && this.checkMode(this.controlMode, evt)) {
        this.u = evt.pointer.state.u;
        this.v = evt.pointer.state.v;
        this.du = evt.pointer.state.du;
        this.dv = evt.pointer.state.dv;
      }
    }
  }
  setMode(evt) {
    if (evt.pointer.type === "mouse") {
      if (evt.pointer.draggedHit) {
        this.controlMode = "mouseedge" /* MouseScreenEdge */;
      } else {
        this.controlMode = "mousedrag" /* MouseDrag */;
      }
    } else if (evt.pointer.type === "touch" || evt.pointer.type === "pen") {
      this.controlMode = "touchswipe" /* Touch */;
    } else if (evt.pointer.type === "gamepad") {
      this.controlMode = "gamepad" /* Gamepad */;
    } else if (evt.pointer.type === "hand") {
      this.controlMode = "motioncontroller" /* MotionControllerStick */;
    } else {
      this.controlMode = "none" /* None */;
    }
  }
  checkMode(mode, evt) {
    return mode !== "none" /* None */ && this.gestureSatisfied(mode, evt) && this.dragSatisfied(mode, evt);
  }
  gestureSatisfied(mode, evt) {
    const button = this.requiredMouseButton.get(mode);
    if (isNullOrUndefined(button)) {
      return mode === "mouseedge" /* MouseScreenEdge */ || mode === "touchswipe" /* Touch */ || mode === "gamepad" /* Gamepad */;
    } else {
      return evt.pointer.state.buttons === button;
    }
  }
  dragSatisfied(mode, evt) {
    return !this.requiredMouseButton.has(mode) || this.requiredMouseButton.get(mode) == 0 /* None */ || evt.pointer.state.dragging;
  }
  get name() {
    return this.object.name;
  }
  set name(v) {
    this.object.name = v;
  }
  get heading() {
    return this._heading;
  }
  setHeading(angle3) {
    this._heading = angleClamp(angle3);
  }
  get pitch() {
    return this._pitch;
  }
  setPitch(x, minX, maxX) {
    this._pitch = angleClamp(x + Math.PI) - Math.PI;
    this._pitch = clamp(this._pitch, minX, maxX);
  }
  get roll() {
    return this._roll;
  }
  setRoll(z) {
    this._roll = angleClamp(z);
  }
  setHeadingImmediate(heading) {
    this.setHeading(heading);
    this.updateOrientation();
    this.resetFollowers();
  }
  setOrientationImmediate(heading, pitch) {
    this.setHeading(heading);
    this._pitch = angleClamp(pitch);
    this.updateOrientation();
  }
  update(dt) {
    dt *= 1e-3;
    if (this.controlMode === "magicwindow" /* MagicWindow */) {
      const device = this.deviceOrientation;
      if (device) {
        const alpha = device.alpha ? deg2rad(device.alpha) + this.alphaOffset : 0;
        const beta2 = device.beta ? deg2rad(device.beta) : 0;
        const gamma = device.gamma ? deg2rad(device.gamma) : 0;
        const orient = this.screenOrientation ? deg2rad(this.screenOrientation) : 0;
        E.set(beta2, alpha, -gamma, "YXZ");
        Q2.setFromAxisAngle(B2, -orient);
        this.deviceQ.setFromEuler(E).multiply(Q3).multiply(Q2);
      }
    } else if (this.controlMode !== "none" /* None */) {
      const startPitch = this.pitch;
      const startHeading = this.heading;
      const dQuat2 = this.orientationDelta(this.controlMode, this.disableVertical, dt);
      this.rotateView(dQuat2, this.minimumX, this.maximumX);
      if (this.evtSys) {
        const viewChanged = startPitch !== this.pitch || startHeading !== this.heading;
        if (viewChanged && this.controlMode === "mouseedge" /* MouseScreenEdge */) {
          this.evtSys.recheckPointers();
        }
      }
    }
    if (this.fwrd || this.back || this.left || this.rgth || this.up || this.down) {
      Q1.setFromAxisAngle(this.stage.up, this.worldHeading);
      const dx = (this.left ? -1 : 0) + (this.rgth ? 1 : 0);
      const dy = (this.down ? -1 : 0) + (this.up ? 1 : 0);
      const dz = (this.fwrd ? -1 : 0) + (this.back ? 1 : 0);
      this.move.set(dx, dy, dz);
      const d = this.move.length();
      if (d > 0) {
        this.move.multiplyScalar(dt / d).applyQuaternion(Q1);
        this.stage.position.add(this.move);
      }
    }
    if (this.grow || this.shrk) {
      const dy = (this.shrk ? -1 : 0) + (this.grow ? 1 : 0);
      this._height += dy * dt;
      this._height = clamp(this._height, 1, 2);
    }
    this.updateOrientation();
    userMovedEvt.set(P3.x, P3.y, P3.z, F.x, F.y, F.z, U.x, U.y, U.z, this.height);
    this.dispatchEvent(userMovedEvt);
  }
  rotateView(dQuat2, minX = -Math.PI, maxX = Math.PI) {
    this.viewEuler.setFromQuaternion(dQuat2, "YXZ");
    let { x, y } = this.viewEuler;
    this.setHeading(this.heading + y);
    this.setPitch(this.pitch + x, minX, maxX);
    this.setRoll(0);
  }
  updateOrientation() {
    const cam = resolveCamera(this.renderer, this.camera);
    rotStage.makeRotationY(this._heading);
    this.stage.matrix.makeTranslation(this.stage.position.x, this.stage.position.y, this.stage.position.z).multiply(rotStage);
    this.stage.matrix.decompose(this.stage.position, this.stage.quaternion, this.stage.scale);
    if (this.renderer.xr.isPresenting) {
      M2.copy(this.stage.matrixWorld).invert();
      this.head.position.copy(cam.position).applyMatrix4(M2);
      this.head.quaternion.copy(this.stage.quaternion).invert().multiply(cam.quaternion);
    } else {
      this.head.position.set(0, this._height, 0);
      E.set(this._pitch, 0, this._roll, "XYZ");
      this.head.quaternion.setFromEuler(E).premultiply(this.deviceQ);
    }
    this.camera.position.copy(this.head.position);
    this.camera.quaternion.copy(this.head.quaternion);
    this.head.getWorldPosition(this.worldPos);
    this.head.getWorldDirection(F);
    this._worldHeading = getLookHeading(F);
    this._worldPitch = getLookPitch(F);
    setRightUpFwdPosFromMatrix(this.head.matrixWorld, R, U, F, P3);
  }
  reset() {
    this.stage.position.set(0, 0, 0);
    this.setHeadingImmediate(0);
  }
  resetFollowers() {
    for (const follower of this.followers) {
      follower.reset(this.height, this.worldPos, this.worldHeading);
    }
  }
  orientationDelta(mode, disableVertical, dt) {
    var move = this.pointerMovement(mode);
    if (this.controlMode === "mousedrag" /* MouseDrag */ || this.controlMode === "touchswipe" /* Touch */) {
      const factor = Math.pow(0.95, 100 * dt);
      this.du = truncate(factor * this.du);
      this.dv = truncate(factor * this.dv);
    }
    if (disableVertical) {
      move.x = 0;
    } else if (this.invertVertical) {
      move.x *= -1;
    }
    if (this.disableHorizontal) {
      move.y = 0;
    } else if (this.invertHorizontal) {
      move.y *= -1;
    }
    if (mode !== "motioncontroller" /* MotionControllerStick */) {
      move.multiplyScalar(dt);
    }
    E.set(move.y, move.x, 0, "YXZ");
    deltaQuat.setFromEuler(E);
    return deltaQuat;
  }
  pointerMovement(mode) {
    switch (mode) {
      case "mousedrag" /* MouseDrag */:
        return this.getAxialMovement(MOUSE_SENSITIVITY_SCALE);
      case "mousefirstperson" /* MouseFPS */:
        return this.getAxialMovement(MOUSE_SENSITIVITY_SCALE);
      case "touchswipe" /* Touch */:
        return this.getAxialMovement(TOUCH_SENSITIVITY_SCALE);
      case "gamepad" /* Gamepad */:
        return this.getAxialMovement(GAMEPAD_SENSITIVITY_SCALE);
      case "mouseedge" /* MouseScreenEdge */:
        return this.getRadiusMovement();
      case "motioncontroller" /* MotionControllerStick */:
        motion.copy(nextFlick);
        nextFlick.set(0, 0, 0);
        return motion;
      case "none" /* None */:
      case "magicwindow" /* MagicWindow */:
        return motion.set(0, 0, 0);
      default:
        assertNever(mode);
    }
  }
  getAxialMovement(sense) {
    motion.set(-sense * this.du, sense * this.dv, 0);
    return motion;
  }
  getRadiusMovement() {
    motion.set(this.scaleRadialComponent(this.u, this.speedX, this.accelerationX), this.scaleRadialComponent(-this.v, this.speedY, this.accelerationY), 0);
    return motion;
  }
  scaleRadialComponent(n2, dn, ddn) {
    const absN = Math.abs(n2);
    return Math.sign(n2) * Math.pow(Math.max(0, absN - this.edgeFactor) / (1 - this.edgeFactor), ddn) * dn;
  }
  async getPermission() {
    if (!("DeviceOrientationEvent" in globalThis)) {
      return "not-supported";
    }
    if (isPermissionedDeviceOrientationEvent(DeviceOrientationEvent)) {
      return await DeviceOrientationEvent.requestPermission();
    }
    return "granted";
  }
  async startMotionControl() {
    if (!this.motionEnabled) {
      this.onScreenOrientationChangeEvent();
      const permission = await this.getPermission();
      this.motionEnabled = permission === "granted";
      if (this.motionEnabled) {
        globalThis.addEventListener("orientationchange", this.onScreenOrientationChangeEvent);
        globalThis.addEventListener("deviceorientation", this.onDeviceOrientationChangeEvent);
      }
    }
  }
  stopMotionControl() {
    if (this.motionEnabled) {
      globalThis.removeEventListener("orientationchange", this.onScreenOrientationChangeEvent);
      globalThis.removeEventListener("deviceorientation", this.onDeviceOrientationChangeEvent);
      this.motionEnabled = false;
    }
  }
  dispose() {
    this.stopMotionControl();
  }
};

// ../threejs/CameraFOVControl.ts
var CameraControl = class {
  constructor(camera) {
    this.camera = camera;
    this.fovZoomEnabled = true;
    this.minFOV = 15;
    this.maxFOV = 120;
    this.dz = 0;
  }
  onMove(evt) {
    if (evt.pointer.enabled && evt.pointer.canMoveView && evt.pointer.state.dz !== 0) {
      this.dz = evt.pointer.state.dz;
    }
  }
  get fov() {
    return this.camera.fov;
  }
  set fov(v) {
    if (v !== this.fov) {
      this.camera.fov = v;
      this.camera.updateProjectionMatrix();
    }
  }
  update(dt) {
    if (this.fovZoomEnabled && Math.abs(this.dz) > 0) {
      const factor = Math.pow(0.95, 5 * dt);
      this.dz = truncate(factor * this.dz);
      this.fov = clamp(this.camera.fov - this.dz, this.minFOV, this.maxFOV);
    }
  }
};

// ../threejs/layers.ts
var FOREGROUND = 0;
var PURGATORY = 3;
function deepSetLayer(obj2, level) {
  obj2 = objectResolve(obj2);
  obj2.traverse((o) => o.layers.set(level));
}
function deepEnableLayer(obj2, level) {
  obj2 = objectResolve(obj2);
  obj2.traverse((o) => o.layers.enable(level));
}

// ../threejs/eventSystem/BaseCursor.ts
var T = new THREE.Vector3();
var V = new THREE.Vector3();
var Q4 = new THREE.Quaternion();
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
      hit.object.getWorldQuaternion(Q4);
      T.copy(hit.face.normal).applyQuaternion(Q4);
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

// ../threejs/eventSystem/Cursor3D.ts
var Cursor3D = class extends BaseCursor {
  constructor(cursorSystem) {
    super();
    this.cursorSystem = null;
    this.object = new THREE.Object3D();
    this.cursorSystem = cursorSystem;
  }
  add(name2, obj2) {
    this.object.add(obj2);
    deepEnableLayer(obj2, PURGATORY);
    obj2.visible = name2 === "default";
  }
  get position() {
    return this.object.position;
  }
  get style() {
    for (const child of this.object.children) {
      if (child.visible) {
        return child.name;
      }
    }
    return null;
  }
  set style(v) {
    for (const child of this.object.children) {
      child.visible = child.name === v;
    }
    if (this.style == null && this.object.children.length > 0) {
      const defaultCursor = arrayScan(this.object.children, (child) => child.name === "default", (child) => child != null);
      if (defaultCursor != null) {
        defaultCursor.visible = true;
      }
    }
    if (this.cursorSystem) {
      this.cursorSystem.style = "none";
    }
  }
  get visible() {
    return objectIsVisible(this);
  }
  set visible(v) {
    objectSetVisible(this, v);
  }
  lookAt(v) {
    this.object.lookAt(v);
  }
  clone() {
    const obj2 = new Cursor3D();
    for (const child of this.object.children) {
      obj2.add(child.name, child.clone());
    }
    return obj2;
  }
};

// ../event-system/ObjectMovedEvent.ts
var ObjectMovedEvent = class extends TypedEvent {
  constructor(name2 = null) {
    super("objectMoved");
    this.name = name2;
  }
  px = 0;
  py = 0;
  pz = 0;
  fx = 0;
  fy = 0;
  fz = 0;
  ux = 0;
  uy = 0;
  uz = 0;
  set(px, py, pz, fx, fy, fz, ux, uy, uz) {
    this.px = px;
    this.py = py;
    this.pz = pz;
    this.fx = fx;
    this.fy = fy;
    this.fz = fz;
    this.ux = ux;
    this.uy = uy;
    this.uz = uz;
  }
};

// ../threejs/eventSystem/resolveObj.ts
function resolveObj(hit) {
  if (!hit || !isCollider(hit.object)) {
    return null;
  }
  let obj2 = hit.object;
  while (isDefined(obj2) && !isInteractiveObject3D(obj2)) {
    obj2 = obj2.parent;
  }
  return obj2;
}

// ../threejs/eventSystem/EventSystemEvent.ts
var EventSystemEvent = class extends TypedEvent {
  constructor(type2, pointer) {
    super(type2);
    this.pointer = pointer;
    this._hit = null;
    this._point = null;
    this._distance = Number.POSITIVE_INFINITY;
    this._object = null;
    Object.seal(this);
  }
  get hit() {
    return this._hit;
  }
  set hit(v) {
    if (v !== this.hit) {
      this._hit = v;
      this._point = null;
      this._distance = Number.POSITIVE_INFINITY;
      this._object = null;
      if (v) {
        this._point = v.point;
        this._distance = v.distance;
        this._object = resolveObj(v);
      }
    }
  }
  get object() {
    return this._object;
  }
  get point() {
    return this._point;
  }
  get distance() {
    return this._distance;
  }
  to3(altHit) {
    return new EventSystemThreeJSEvent(this.type, this.pointer, altHit, this.pointer.state.buttons);
  }
};
var EventSystemThreeJSEvent = class {
  constructor(type2, pointer, hit, buttons) {
    this.type = type2;
    this.pointer = pointer;
    this.hit = hit;
    this.buttons = buttons;
    this._point = this.hit && this.hit.point;
    this._distance = this.hit && this.hit.distance || Number.POSITIVE_INFINITY;
    this._object = resolveObj(this.hit);
  }
  get object() {
    return this._object;
  }
  get point() {
    return this._point;
  }
  get distance() {
    return this._distance;
  }
};

// ../widgets/EventedGamepad.ts
var GamepadButtonEvent = class extends TypedEvent {
  constructor(type2, button) {
    super(type2);
    this.button = button;
  }
};
var GamepadButtonUpEvent = class extends GamepadButtonEvent {
  constructor(button) {
    super("gamepadbuttonup", button);
  }
};
var GamepadButtonDownEvent = class extends GamepadButtonEvent {
  constructor(button) {
    super("gamepadbuttondown", button);
  }
};
var GamepadAxisEvent = class extends TypedEvent {
  constructor(type2, axis, value2) {
    super(type2);
    this.axis = axis;
    this.value = value2;
  }
};
var GamepadAxisMaxedEvent = class extends GamepadAxisEvent {
  constructor(axis, value2) {
    super("gamepadaxismaxed", axis, value2);
  }
};
var EventedGamepad = class extends TypedEventBase {
  constructor(pad) {
    super();
    this.pad = pad;
    this._isStick = (a) => a % 2 === 0 && a < pad.axes.length - 1;
    for (let b = 0; b < pad.buttons.length; ++b) {
      this.btnDownEvts[b] = new GamepadButtonDownEvent(b);
      this.btnUpEvts[b] = new GamepadButtonUpEvent(b);
      this.wasPressed[b] = false;
    }
    for (let a = 0; a < pad.axes.length; ++a) {
      this.axisMaxEvts[a] = new GamepadAxisMaxedEvent(a, 0);
      this.wasAxisMaxed[a] = false;
      if (this._isStick(a)) {
        this.sticks[a / 2] = { x: 0, y: 0 };
      }
      this.lastAxisValues[a] = pad.axes[a];
    }
    Object.seal(this);
  }
  lastAxisValues = new Array();
  _isStick;
  btnDownEvts = new Array();
  btnUpEvts = new Array();
  wasPressed = new Array();
  axisThresholdMax = 0.9;
  axisThresholdMin = 0.1;
  axisMaxEvts = new Array();
  wasAxisMaxed = new Array();
  sticks = new Array();
  setPad(pad) {
    this.pad = pad;
    this.update();
  }
  get id() {
    return this.pad.id;
  }
  get index() {
    return this.pad.index;
  }
  get connected() {
    return this.pad.connected;
  }
  get mapping() {
    return this.pad.mapping;
  }
  get timestamp() {
    return this.pad.timestamp;
  }
  get hand() {
    return this.pad.hand;
  }
  get pose() {
    return this.pad.pose;
  }
  get buttons() {
    return this.pad.buttons;
  }
  get axes() {
    return this.pad.axes;
  }
  get hapticActuators() {
    return this.pad.hapticActuators;
  }
  update() {
    for (let b = 0; b < this.pad.buttons.length; ++b) {
      const wasPressed = this.wasPressed[b];
      const pressed = this.pad.buttons[b].pressed;
      if (pressed !== wasPressed) {
        this.wasPressed[b] = pressed;
        this.dispatchEvent((pressed ? this.btnDownEvts : this.btnUpEvts)[b]);
      }
    }
    for (let a = 0; a < this.pad.axes.length; ++a) {
      const wasMaxed = this.wasAxisMaxed[a];
      const val = this.pad.axes[a];
      const dir = Math.sign(val);
      const mag = Math.abs(val);
      const maxed = mag >= this.axisThresholdMax;
      const mined = mag <= this.axisThresholdMin;
      const correctedVal = dir * (maxed ? 1 : mined ? 0 : mag);
      if (maxed && !wasMaxed) {
        this.axisMaxEvts[a].value = correctedVal;
        this.dispatchEvent(this.axisMaxEvts[a]);
      }
      this.wasAxisMaxed[a] = maxed;
      this.lastAxisValues[a] = correctedVal;
    }
    for (let a = 0; a < this.axes.length - 1; a += 2) {
      const stick = this.sticks[a / 2];
      stick.x = this.axes[a];
      stick.y = this.axes[a + 1];
    }
  }
};

// ../threejs/examples/loaders/GLTFLoader.js
var GLTFLoader = class extends THREE.Loader {
  constructor(manager) {
    super(manager);
    this.dracoLoader = null;
    this.ktx2Loader = null;
    this.meshoptDecoder = null;
    this.pluginCallbacks = [];
    this.register(function(parser) {
      return new GLTFMaterialsClearcoatExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFTextureBasisUExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFTextureWebPExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFMaterialsSheenExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFMaterialsTransmissionExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFMaterialsVolumeExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFMaterialsIorExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFMaterialsSpecularExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFLightsExtension(parser);
    });
    this.register(function(parser) {
      return new GLTFMeshoptCompression(parser);
    });
  }
  load(url, onLoad, onProgress, onError) {
    const scope = this;
    let resourcePath;
    if (this.resourcePath !== "") {
      resourcePath = this.resourcePath;
    } else if (this.path !== "") {
      resourcePath = this.path;
    } else {
      resourcePath = THREE.LoaderUtils.extractUrlBase(url);
    }
    this.manager.itemStart(url);
    const _onError = function(e2) {
      if (onError) {
        onError(e2);
      } else {
        console.error(e2);
      }
      scope.manager.itemError(url);
      scope.manager.itemEnd(url);
    };
    const loader = new THREE.FileLoader(this.manager);
    loader.setPath(this.path);
    loader.setResponseType("arraybuffer");
    loader.setRequestHeader(this.requestHeader);
    loader.setWithCredentials(this.withCredentials);
    loader.load(url, function(data) {
      try {
        scope.parse(data, resourcePath, function(gltf) {
          onLoad(gltf);
          scope.manager.itemEnd(url);
        }, _onError);
      } catch (e2) {
        _onError(e2);
      }
    }, onProgress, _onError);
  }
  setDRACOLoader(dracoLoader) {
    this.dracoLoader = dracoLoader;
    return this;
  }
  setDDSLoader() {
    throw new Error('THREE.GLTFLoader: "MSFT_texture_dds" no longer supported. Please update to "KHR_texture_basisu".');
  }
  setKTX2Loader(ktx2Loader) {
    this.ktx2Loader = ktx2Loader;
    return this;
  }
  setMeshoptDecoder(meshoptDecoder) {
    this.meshoptDecoder = meshoptDecoder;
    return this;
  }
  register(callback) {
    if (this.pluginCallbacks.indexOf(callback) === -1) {
      this.pluginCallbacks.push(callback);
    }
    return this;
  }
  unregister(callback) {
    if (this.pluginCallbacks.indexOf(callback) !== -1) {
      this.pluginCallbacks.splice(this.pluginCallbacks.indexOf(callback), 1);
    }
    return this;
  }
  parse(data, path, onLoad, onError) {
    let content;
    const extensions = {};
    const plugins = {};
    if (typeof data === "string") {
      content = data;
    } else {
      const magic = THREE.LoaderUtils.decodeText(new Uint8Array(data, 0, 4));
      if (magic === BINARY_EXTENSION_HEADER_MAGIC) {
        try {
          extensions[EXTENSIONS.KHR_BINARY_GLTF] = new GLTFBinaryExtension(data);
        } catch (error) {
          if (onError)
            onError(error);
          return;
        }
        content = extensions[EXTENSIONS.KHR_BINARY_GLTF].content;
      } else {
        content = THREE.LoaderUtils.decodeText(new Uint8Array(data));
      }
    }
    const json = JSON.parse(content);
    if (json.asset === void 0 || json.asset.version[0] < 2) {
      if (onError)
        onError(new Error("THREE.GLTFLoader: Unsupported asset. glTF versions >=2.0 are supported."));
      return;
    }
    const parser = new GLTFParser(json, {
      path: path || this.resourcePath || "",
      crossOrigin: this.crossOrigin,
      requestHeader: this.requestHeader,
      manager: this.manager,
      ktx2Loader: this.ktx2Loader,
      meshoptDecoder: this.meshoptDecoder
    });
    parser.fileLoader.setRequestHeader(this.requestHeader);
    for (let i = 0; i < this.pluginCallbacks.length; i++) {
      const plugin = this.pluginCallbacks[i](parser);
      plugins[plugin.name] = plugin;
      extensions[plugin.name] = true;
    }
    if (json.extensionsUsed) {
      for (let i = 0; i < json.extensionsUsed.length; ++i) {
        const extensionName = json.extensionsUsed[i];
        const extensionsRequired = json.extensionsRequired || [];
        switch (extensionName) {
          case EXTENSIONS.KHR_MATERIALS_UNLIT:
            extensions[extensionName] = new GLTFMaterialsUnlitExtension();
            break;
          case EXTENSIONS.KHR_MATERIALS_PBR_SPECULAR_GLOSSINESS:
            extensions[extensionName] = new GLTFMaterialsPbrSpecularGlossinessExtension();
            break;
          case EXTENSIONS.KHR_DRACO_MESH_COMPRESSION:
            extensions[extensionName] = new GLTFDracoMeshCompressionExtension(json, this.dracoLoader);
            break;
          case EXTENSIONS.KHR_TEXTURE_TRANSFORM:
            extensions[extensionName] = new GLTFTextureTransformExtension();
            break;
          case EXTENSIONS.KHR_MESH_QUANTIZATION:
            extensions[extensionName] = new GLTFMeshQuantizationExtension();
            break;
          default:
            if (extensionsRequired.indexOf(extensionName) >= 0 && plugins[extensionName] === void 0) {
              console.warn('THREE.GLTFLoader: Unknown extension "' + extensionName + '".');
            }
        }
      }
    }
    parser.setExtensions(extensions);
    parser.setPlugins(plugins);
    parser.parse(onLoad, onError);
  }
  parseAsync(data, path) {
    const scope = this;
    return new Promise(function(resolve, reject) {
      scope.parse(data, path, resolve, reject);
    });
  }
};
function GLTFRegistry() {
  let objects = {};
  return {
    get: function(key) {
      return objects[key];
    },
    add: function(key, object) {
      objects[key] = object;
    },
    remove: function(key) {
      delete objects[key];
    },
    removeAll: function() {
      objects = {};
    }
  };
}
var EXTENSIONS = {
  KHR_BINARY_GLTF: "KHR_binary_glTF",
  KHR_DRACO_MESH_COMPRESSION: "KHR_draco_mesh_compression",
  KHR_LIGHTS_PUNCTUAL: "KHR_lights_punctual",
  KHR_MATERIALS_CLEARCOAT: "KHR_materials_clearcoat",
  KHR_MATERIALS_IOR: "KHR_materials_ior",
  KHR_MATERIALS_PBR_SPECULAR_GLOSSINESS: "KHR_materials_pbrSpecularGlossiness",
  KHR_MATERIALS_SHEEN: "KHR_materials_sheen",
  KHR_MATERIALS_SPECULAR: "KHR_materials_specular",
  KHR_MATERIALS_TRANSMISSION: "KHR_materials_transmission",
  KHR_MATERIALS_UNLIT: "KHR_materials_unlit",
  KHR_MATERIALS_VOLUME: "KHR_materials_volume",
  KHR_TEXTURE_BASISU: "KHR_texture_basisu",
  KHR_TEXTURE_TRANSFORM: "KHR_texture_transform",
  KHR_MESH_QUANTIZATION: "KHR_mesh_quantization",
  EXT_TEXTURE_WEBP: "EXT_texture_webp",
  EXT_MESHOPT_COMPRESSION: "EXT_meshopt_compression"
};
var GLTFLightsExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_LIGHTS_PUNCTUAL;
    this.cache = { refs: {}, uses: {} };
  }
  _markDefs() {
    const parser = this.parser;
    const nodeDefs = this.parser.json.nodes || [];
    for (let nodeIndex = 0, nodeLength = nodeDefs.length; nodeIndex < nodeLength; nodeIndex++) {
      const nodeDef = nodeDefs[nodeIndex];
      if (nodeDef.extensions && nodeDef.extensions[this.name] && nodeDef.extensions[this.name].light !== void 0) {
        parser._addNodeRef(this.cache, nodeDef.extensions[this.name].light);
      }
    }
  }
  _loadLight(lightIndex) {
    const parser = this.parser;
    const cacheKey = "light:" + lightIndex;
    let dependency = parser.cache.get(cacheKey);
    if (dependency)
      return dependency;
    const json = parser.json;
    const extensions = json.extensions && json.extensions[this.name] || {};
    const lightDefs = extensions.lights || [];
    const lightDef = lightDefs[lightIndex];
    let lightNode;
    const color = new THREE.Color(16777215);
    if (lightDef.color !== void 0)
      color.fromArray(lightDef.color);
    const range = lightDef.range !== void 0 ? lightDef.range : 0;
    switch (lightDef.type) {
      case "directional":
        lightNode = new THREE.DirectionalLight(color);
        lightNode.target.position.set(0, 0, -1);
        lightNode.add(lightNode.target);
        break;
      case "point":
        lightNode = new THREE.PointLight(color);
        lightNode.distance = range;
        break;
      case "spot":
        lightNode = new THREE.SpotLight(color);
        lightNode.distance = range;
        lightDef.spot = lightDef.spot || {};
        lightDef.spot.innerConeAngle = lightDef.spot.innerConeAngle !== void 0 ? lightDef.spot.innerConeAngle : 0;
        lightDef.spot.outerConeAngle = lightDef.spot.outerConeAngle !== void 0 ? lightDef.spot.outerConeAngle : Math.PI / 4;
        lightNode.angle = lightDef.spot.outerConeAngle;
        lightNode.penumbra = 1 - lightDef.spot.innerConeAngle / lightDef.spot.outerConeAngle;
        lightNode.target.position.set(0, 0, -1);
        lightNode.add(lightNode.target);
        break;
      default:
        throw new Error("THREE.GLTFLoader: Unexpected light type: " + lightDef.type);
    }
    lightNode.position.set(0, 0, 0);
    lightNode.decay = 2;
    if (lightDef.intensity !== void 0)
      lightNode.intensity = lightDef.intensity;
    lightNode.name = parser.createUniqueName(lightDef.name || "light_" + lightIndex);
    dependency = Promise.resolve(lightNode);
    parser.cache.add(cacheKey, dependency);
    return dependency;
  }
  createNodeAttachment(nodeIndex) {
    const self2 = this;
    const parser = this.parser;
    const json = parser.json;
    const nodeDef = json.nodes[nodeIndex];
    const lightDef = nodeDef.extensions && nodeDef.extensions[this.name] || {};
    const lightIndex = lightDef.light;
    if (lightIndex === void 0)
      return null;
    return this._loadLight(lightIndex).then(function(light) {
      return parser._getNodeRef(self2.cache, lightIndex, light);
    });
  }
};
var GLTFMaterialsUnlitExtension = class {
  constructor() {
    this.name = EXTENSIONS.KHR_MATERIALS_UNLIT;
  }
  getMaterialType() {
    return THREE.MeshBasicMaterial;
  }
  extendParams(materialParams, materialDef, parser) {
    const pending = [];
    materialParams.color = new THREE.Color(1, 1, 1);
    materialParams.opacity = 1;
    const metallicRoughness = materialDef.pbrMetallicRoughness;
    if (metallicRoughness) {
      if (Array.isArray(metallicRoughness.baseColorFactor)) {
        const array = metallicRoughness.baseColorFactor;
        materialParams.color.fromArray(array);
        materialParams.opacity = array[3];
      }
      if (metallicRoughness.baseColorTexture !== void 0) {
        pending.push(parser.assignTexture(materialParams, "map", metallicRoughness.baseColorTexture));
      }
    }
    return Promise.all(pending);
  }
};
var GLTFMaterialsClearcoatExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_MATERIALS_CLEARCOAT;
  }
  getMaterialType(materialIndex) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name])
      return null;
    return THREE.MeshPhysicalMaterial;
  }
  extendMaterialParams(materialIndex, materialParams) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name]) {
      return Promise.resolve();
    }
    const pending = [];
    const extension = materialDef.extensions[this.name];
    if (extension.clearcoatFactor !== void 0) {
      materialParams.clearcoat = extension.clearcoatFactor;
    }
    if (extension.clearcoatTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "clearcoatMap", extension.clearcoatTexture));
    }
    if (extension.clearcoatRoughnessFactor !== void 0) {
      materialParams.clearcoatRoughness = extension.clearcoatRoughnessFactor;
    }
    if (extension.clearcoatRoughnessTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "clearcoatRoughnessMap", extension.clearcoatRoughnessTexture));
    }
    if (extension.clearcoatNormalTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "clearcoatNormalMap", extension.clearcoatNormalTexture));
      if (extension.clearcoatNormalTexture.scale !== void 0) {
        const scale4 = extension.clearcoatNormalTexture.scale;
        materialParams.clearcoatNormalScale = new THREE.Vector2(scale4, scale4);
      }
    }
    return Promise.all(pending);
  }
};
var GLTFMaterialsSheenExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_MATERIALS_SHEEN;
  }
  getMaterialType(materialIndex) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name])
      return null;
    return THREE.MeshPhysicalMaterial;
  }
  extendMaterialParams(materialIndex, materialParams) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name]) {
      return Promise.resolve();
    }
    const pending = [];
    materialParams.sheenColor = new THREE.Color(0, 0, 0);
    materialParams.sheenRoughness = 0;
    materialParams.sheen = 1;
    const extension = materialDef.extensions[this.name];
    if (extension.sheenColorFactor !== void 0) {
      materialParams.sheenColor.fromArray(extension.sheenColorFactor);
    }
    if (extension.sheenRoughnessFactor !== void 0) {
      materialParams.sheenRoughness = extension.sheenRoughnessFactor;
    }
    if (extension.sheenColorTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "sheenColorMap", extension.sheenColorTexture));
    }
    if (extension.sheenRoughnessTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "sheenRoughnessMap", extension.sheenRoughnessTexture));
    }
    return Promise.all(pending);
  }
};
var GLTFMaterialsTransmissionExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_MATERIALS_TRANSMISSION;
  }
  getMaterialType(materialIndex) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name])
      return null;
    return THREE.MeshPhysicalMaterial;
  }
  extendMaterialParams(materialIndex, materialParams) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name]) {
      return Promise.resolve();
    }
    const pending = [];
    const extension = materialDef.extensions[this.name];
    if (extension.transmissionFactor !== void 0) {
      materialParams.transmission = extension.transmissionFactor;
    }
    if (extension.transmissionTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "transmissionMap", extension.transmissionTexture));
    }
    return Promise.all(pending);
  }
};
var GLTFMaterialsVolumeExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_MATERIALS_VOLUME;
  }
  getMaterialType(materialIndex) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name])
      return null;
    return THREE.MeshPhysicalMaterial;
  }
  extendMaterialParams(materialIndex, materialParams) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name]) {
      return Promise.resolve();
    }
    const pending = [];
    const extension = materialDef.extensions[this.name];
    materialParams.thickness = extension.thicknessFactor !== void 0 ? extension.thicknessFactor : 0;
    if (extension.thicknessTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "thicknessMap", extension.thicknessTexture));
    }
    materialParams.attenuationDistance = extension.attenuationDistance || 0;
    const colorArray = extension.attenuationColor || [1, 1, 1];
    materialParams.attenuationColor = new THREE.Color(colorArray[0], colorArray[1], colorArray[2]);
    return Promise.all(pending);
  }
};
var GLTFMaterialsIorExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_MATERIALS_IOR;
  }
  getMaterialType(materialIndex) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name])
      return null;
    return THREE.MeshPhysicalMaterial;
  }
  extendMaterialParams(materialIndex, materialParams) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name]) {
      return Promise.resolve();
    }
    const extension = materialDef.extensions[this.name];
    materialParams.ior = extension.ior !== void 0 ? extension.ior : 1.5;
    return Promise.resolve();
  }
};
var GLTFMaterialsSpecularExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_MATERIALS_SPECULAR;
  }
  getMaterialType(materialIndex) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name])
      return null;
    return THREE.MeshPhysicalMaterial;
  }
  extendMaterialParams(materialIndex, materialParams) {
    const parser = this.parser;
    const materialDef = parser.json.materials[materialIndex];
    if (!materialDef.extensions || !materialDef.extensions[this.name]) {
      return Promise.resolve();
    }
    const pending = [];
    const extension = materialDef.extensions[this.name];
    materialParams.specularIntensity = extension.specularFactor !== void 0 ? extension.specularFactor : 1;
    if (extension.specularTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "specularIntensityMap", extension.specularTexture));
    }
    const colorArray = extension.specularColorFactor || [1, 1, 1];
    materialParams.specularColor = new THREE.Color(colorArray[0], colorArray[1], colorArray[2]);
    if (extension.specularColorTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "specularColorMap", extension.specularColorTexture).then(function(texture) {
        texture.encoding = THREE.sRGBEncoding;
      }));
    }
    return Promise.all(pending);
  }
};
var GLTFTextureBasisUExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.KHR_TEXTURE_BASISU;
  }
  loadTexture(textureIndex) {
    const parser = this.parser;
    const json = parser.json;
    const textureDef = json.textures[textureIndex];
    if (!textureDef.extensions || !textureDef.extensions[this.name]) {
      return null;
    }
    const extension = textureDef.extensions[this.name];
    const source = json.images[extension.source];
    const loader = parser.options.ktx2Loader;
    if (!loader) {
      if (json.extensionsRequired && json.extensionsRequired.indexOf(this.name) >= 0) {
        throw new Error("THREE.GLTFLoader: setKTX2Loader must be called before loading KTX2 textures");
      } else {
        return null;
      }
    }
    return parser.loadTextureImage(textureIndex, source, loader);
  }
};
var GLTFTextureWebPExtension = class {
  constructor(parser) {
    this.parser = parser;
    this.name = EXTENSIONS.EXT_TEXTURE_WEBP;
    this.isSupported = null;
  }
  loadTexture(textureIndex) {
    const name2 = this.name;
    const parser = this.parser;
    const json = parser.json;
    const textureDef = json.textures[textureIndex];
    if (!textureDef.extensions || !textureDef.extensions[name2]) {
      return null;
    }
    const extension = textureDef.extensions[name2];
    const source = json.images[extension.source];
    let loader = parser.textureLoader;
    if (source.uri) {
      const handler = parser.options.manager.getHandler(source.uri);
      if (handler !== null)
        loader = handler;
    }
    return this.detectSupport().then(function(isSupported) {
      if (isSupported)
        return parser.loadTextureImage(textureIndex, source, loader);
      if (json.extensionsRequired && json.extensionsRequired.indexOf(name2) >= 0) {
        throw new Error("THREE.GLTFLoader: WebP required by asset but unsupported.");
      }
      return parser.loadTexture(textureIndex);
    });
  }
  detectSupport() {
    if (!this.isSupported) {
      this.isSupported = new Promise(function(resolve) {
        const image2 = new Image();
        image2.src = "data:image/webp;base64,UklGRiIAAABXRUJQVlA4IBYAAAAwAQCdASoBAAEADsD+JaQAA3AAAAAA";
        image2.onload = image2.onerror = function() {
          resolve(image2.height === 1);
        };
      });
    }
    return this.isSupported;
  }
};
var GLTFMeshoptCompression = class {
  constructor(parser) {
    this.name = EXTENSIONS.EXT_MESHOPT_COMPRESSION;
    this.parser = parser;
  }
  loadBufferView(index) {
    const json = this.parser.json;
    const bufferView = json.bufferViews[index];
    if (bufferView.extensions && bufferView.extensions[this.name]) {
      const extensionDef = bufferView.extensions[this.name];
      const buffer = this.parser.getDependency("buffer", extensionDef.buffer);
      const decoder = this.parser.options.meshoptDecoder;
      if (!decoder || !decoder.supported) {
        if (json.extensionsRequired && json.extensionsRequired.indexOf(this.name) >= 0) {
          throw new Error("THREE.GLTFLoader: setMeshoptDecoder must be called before loading compressed files");
        } else {
          return null;
        }
      }
      return Promise.all([buffer, decoder.ready]).then(function(res) {
        const byteOffset = extensionDef.byteOffset || 0;
        const byteLength = extensionDef.byteLength || 0;
        const count2 = extensionDef.count;
        const stride = extensionDef.byteStride;
        const result = new ArrayBuffer(count2 * stride);
        const source = new Uint8Array(res[0], byteOffset, byteLength);
        decoder.decodeGltfBuffer(new Uint8Array(result), count2, stride, source, extensionDef.mode, extensionDef.filter);
        return result;
      });
    } else {
      return null;
    }
  }
};
var BINARY_EXTENSION_HEADER_MAGIC = "glTF";
var BINARY_EXTENSION_HEADER_LENGTH = 12;
var BINARY_EXTENSION_CHUNK_TYPES = { JSON: 1313821514, BIN: 5130562 };
var GLTFBinaryExtension = class {
  constructor(data) {
    this.name = EXTENSIONS.KHR_BINARY_GLTF;
    this.content = null;
    this.body = null;
    const headerView = new DataView(data, 0, BINARY_EXTENSION_HEADER_LENGTH);
    this.header = {
      magic: THREE.LoaderUtils.decodeText(new Uint8Array(data.slice(0, 4))),
      version: headerView.getUint32(4, true),
      length: headerView.getUint32(8, true)
    };
    if (this.header.magic !== BINARY_EXTENSION_HEADER_MAGIC) {
      throw new Error("THREE.GLTFLoader: Unsupported glTF-Binary header.");
    } else if (this.header.version < 2) {
      throw new Error("THREE.GLTFLoader: Legacy binary file detected.");
    }
    const chunkContentsLength = this.header.length - BINARY_EXTENSION_HEADER_LENGTH;
    const chunkView = new DataView(data, BINARY_EXTENSION_HEADER_LENGTH);
    let chunkIndex = 0;
    while (chunkIndex < chunkContentsLength) {
      const chunkLength = chunkView.getUint32(chunkIndex, true);
      chunkIndex += 4;
      const chunkType = chunkView.getUint32(chunkIndex, true);
      chunkIndex += 4;
      if (chunkType === BINARY_EXTENSION_CHUNK_TYPES.JSON) {
        const contentArray = new Uint8Array(data, BINARY_EXTENSION_HEADER_LENGTH + chunkIndex, chunkLength);
        this.content = THREE.LoaderUtils.decodeText(contentArray);
      } else if (chunkType === BINARY_EXTENSION_CHUNK_TYPES.BIN) {
        const byteOffset = BINARY_EXTENSION_HEADER_LENGTH + chunkIndex;
        this.body = data.slice(byteOffset, byteOffset + chunkLength);
      }
      chunkIndex += chunkLength;
    }
    if (this.content === null) {
      throw new Error("THREE.GLTFLoader: JSON content not found.");
    }
  }
};
var GLTFDracoMeshCompressionExtension = class {
  constructor(json, dracoLoader) {
    if (!dracoLoader) {
      throw new Error("THREE.GLTFLoader: No DRACOLoader instance provided.");
    }
    this.name = EXTENSIONS.KHR_DRACO_MESH_COMPRESSION;
    this.json = json;
    this.dracoLoader = dracoLoader;
    this.dracoLoader.preload();
  }
  decodePrimitive(primitive, parser) {
    const json = this.json;
    const dracoLoader = this.dracoLoader;
    const bufferViewIndex = primitive.extensions[this.name].bufferView;
    const gltfAttributeMap = primitive.extensions[this.name].attributes;
    const threeAttributeMap = {};
    const attributeNormalizedMap = {};
    const attributeTypeMap = {};
    for (const attributeName in gltfAttributeMap) {
      const threeAttributeName = ATTRIBUTES[attributeName] || attributeName.toLowerCase();
      threeAttributeMap[threeAttributeName] = gltfAttributeMap[attributeName];
    }
    for (const attributeName in primitive.attributes) {
      const threeAttributeName = ATTRIBUTES[attributeName] || attributeName.toLowerCase();
      if (gltfAttributeMap[attributeName] !== void 0) {
        const accessorDef = json.accessors[primitive.attributes[attributeName]];
        const componentType = WEBGL_COMPONENT_TYPES[accessorDef.componentType];
        attributeTypeMap[threeAttributeName] = componentType;
        attributeNormalizedMap[threeAttributeName] = accessorDef.normalized === true;
      }
    }
    return parser.getDependency("bufferView", bufferViewIndex).then(function(bufferView) {
      return new Promise(function(resolve) {
        dracoLoader.decodeDracoFile(bufferView, function(geometry) {
          for (const attributeName in geometry.attributes) {
            const attribute = geometry.attributes[attributeName];
            const normalized = attributeNormalizedMap[attributeName];
            if (normalized !== void 0)
              attribute.normalized = normalized;
          }
          resolve(geometry);
        }, threeAttributeMap, attributeTypeMap);
      });
    });
  }
};
var GLTFTextureTransformExtension = class {
  constructor() {
    this.name = EXTENSIONS.KHR_TEXTURE_TRANSFORM;
  }
  extendTexture(texture, transform2) {
    if (transform2.texCoord !== void 0) {
      console.warn('THREE.GLTFLoader: Custom UV sets in "' + this.name + '" extension not yet supported.');
    }
    if (transform2.offset === void 0 && transform2.rotation === void 0 && transform2.scale === void 0) {
      return texture;
    }
    texture = texture.clone();
    if (transform2.offset !== void 0) {
      texture.offset.fromArray(transform2.offset);
    }
    if (transform2.rotation !== void 0) {
      texture.rotation = transform2.rotation;
    }
    if (transform2.scale !== void 0) {
      texture.repeat.fromArray(transform2.scale);
    }
    texture.needsUpdate = true;
    return texture;
  }
};
var GLTFMeshStandardSGMaterial = class extends THREE.MeshStandardMaterial {
  constructor(params) {
    super();
    this.isGLTFSpecularGlossinessMaterial = true;
    const specularMapParsFragmentChunk = [
      "#ifdef USE_SPECULARMAP",
      "	uniform sampler2D specularMap;",
      "#endif"
    ].join("\n");
    const glossinessMapParsFragmentChunk = [
      "#ifdef USE_GLOSSINESSMAP",
      "	uniform sampler2D glossinessMap;",
      "#endif"
    ].join("\n");
    const specularMapFragmentChunk = [
      "vec3 specularFactor = specular;",
      "#ifdef USE_SPECULARMAP",
      "	vec4 texelSpecular = texture2D( specularMap, vUv );",
      "	// reads channel RGB, compatible with a glTF Specular-Glossiness (RGBA) texture",
      "	specularFactor *= texelSpecular.rgb;",
      "#endif"
    ].join("\n");
    const glossinessMapFragmentChunk = [
      "float glossinessFactor = glossiness;",
      "#ifdef USE_GLOSSINESSMAP",
      "	vec4 texelGlossiness = texture2D( glossinessMap, vUv );",
      "	// reads channel A, compatible with a glTF Specular-Glossiness (RGBA) texture",
      "	glossinessFactor *= texelGlossiness.a;",
      "#endif"
    ].join("\n");
    const lightPhysicalFragmentChunk = [
      "PhysicalMaterial material;",
      "material.diffuseColor = diffuseColor.rgb * ( 1. - max( specularFactor.r, max( specularFactor.g, specularFactor.b ) ) );",
      "vec3 dxy = max( abs( dFdx( geometryNormal ) ), abs( dFdy( geometryNormal ) ) );",
      "float geometryRoughness = max( max( dxy.x, dxy.y ), dxy.z );",
      "material.roughness = max( 1.0 - glossinessFactor, 0.0525 ); // 0.0525 corresponds to the base mip of a 256 cubemap.",
      "material.roughness += geometryRoughness;",
      "material.roughness = min( material.roughness, 1.0 );",
      "material.specularColor = specularFactor;"
    ].join("\n");
    const uniforms = {
      specular: { value: new THREE.Color().setHex(16777215) },
      glossiness: { value: 1 },
      specularMap: { value: null },
      glossinessMap: { value: null }
    };
    this._extraUniforms = uniforms;
    this.onBeforeCompile = function(shader) {
      for (const uniformName in uniforms) {
        shader.uniforms[uniformName] = uniforms[uniformName];
      }
      shader.fragmentShader = shader.fragmentShader.replace("uniform float roughness;", "uniform vec3 specular;").replace("uniform float metalness;", "uniform float glossiness;").replace("#include <roughnessmap_pars_fragment>", specularMapParsFragmentChunk).replace("#include <metalnessmap_pars_fragment>", glossinessMapParsFragmentChunk).replace("#include <roughnessmap_fragment>", specularMapFragmentChunk).replace("#include <metalnessmap_fragment>", glossinessMapFragmentChunk).replace("#include <lights_physical_fragment>", lightPhysicalFragmentChunk);
    };
    Object.defineProperties(this, {
      specular: {
        get: function() {
          return uniforms.specular.value;
        },
        set: function(v) {
          uniforms.specular.value = v;
        }
      },
      specularMap: {
        get: function() {
          return uniforms.specularMap.value;
        },
        set: function(v) {
          uniforms.specularMap.value = v;
          if (v) {
            this.defines.USE_SPECULARMAP = "";
          } else {
            delete this.defines.USE_SPECULARMAP;
          }
        }
      },
      glossiness: {
        get: function() {
          return uniforms.glossiness.value;
        },
        set: function(v) {
          uniforms.glossiness.value = v;
        }
      },
      glossinessMap: {
        get: function() {
          return uniforms.glossinessMap.value;
        },
        set: function(v) {
          uniforms.glossinessMap.value = v;
          if (v) {
            this.defines.USE_GLOSSINESSMAP = "";
            this.defines.USE_UV = "";
          } else {
            delete this.defines.USE_GLOSSINESSMAP;
            delete this.defines.USE_UV;
          }
        }
      }
    });
    delete this.metalness;
    delete this.roughness;
    delete this.metalnessMap;
    delete this.roughnessMap;
    this.setValues(params);
  }
  copy(source) {
    super.copy(source);
    this.specularMap = source.specularMap;
    this.specular.copy(source.specular);
    this.glossinessMap = source.glossinessMap;
    this.glossiness = source.glossiness;
    delete this.metalness;
    delete this.roughness;
    delete this.metalnessMap;
    delete this.roughnessMap;
    return this;
  }
};
var GLTFMaterialsPbrSpecularGlossinessExtension = class {
  constructor() {
    this.name = EXTENSIONS.KHR_MATERIALS_PBR_SPECULAR_GLOSSINESS;
    this.specularGlossinessParams = [
      "color",
      "map",
      "lightMap",
      "lightMapIntensity",
      "aoMap",
      "aoMapIntensity",
      "emissive",
      "emissiveIntensity",
      "emissiveMap",
      "bumpMap",
      "bumpScale",
      "normalMap",
      "normalMapType",
      "displacementMap",
      "displacementScale",
      "displacementBias",
      "specularMap",
      "specular",
      "glossinessMap",
      "glossiness",
      "alphaMap",
      "envMap",
      "envMapIntensity",
      "refractionRatio"
    ];
  }
  getMaterialType() {
    return GLTFMeshStandardSGMaterial;
  }
  extendParams(materialParams, materialDef, parser) {
    const pbrSpecularGlossiness = materialDef.extensions[this.name];
    materialParams.color = new THREE.Color(1, 1, 1);
    materialParams.opacity = 1;
    const pending = [];
    if (Array.isArray(pbrSpecularGlossiness.diffuseFactor)) {
      const array = pbrSpecularGlossiness.diffuseFactor;
      materialParams.color.fromArray(array);
      materialParams.opacity = array[3];
    }
    if (pbrSpecularGlossiness.diffuseTexture !== void 0) {
      pending.push(parser.assignTexture(materialParams, "map", pbrSpecularGlossiness.diffuseTexture));
    }
    materialParams.emissive = new THREE.Color(0, 0, 0);
    materialParams.glossiness = pbrSpecularGlossiness.glossinessFactor !== void 0 ? pbrSpecularGlossiness.glossinessFactor : 1;
    materialParams.specular = new THREE.Color(1, 1, 1);
    if (Array.isArray(pbrSpecularGlossiness.specularFactor)) {
      materialParams.specular.fromArray(pbrSpecularGlossiness.specularFactor);
    }
    if (pbrSpecularGlossiness.specularGlossinessTexture !== void 0) {
      const specGlossMapDef = pbrSpecularGlossiness.specularGlossinessTexture;
      pending.push(parser.assignTexture(materialParams, "glossinessMap", specGlossMapDef));
      pending.push(parser.assignTexture(materialParams, "specularMap", specGlossMapDef));
    }
    return Promise.all(pending);
  }
  createMaterial(materialParams) {
    const material = new GLTFMeshStandardSGMaterial(materialParams);
    material.fog = true;
    material.color = materialParams.color;
    material.map = materialParams.map === void 0 ? null : materialParams.map;
    material.lightMap = null;
    material.lightMapIntensity = 1;
    material.aoMap = materialParams.aoMap === void 0 ? null : materialParams.aoMap;
    material.aoMapIntensity = 1;
    material.emissive = materialParams.emissive;
    material.emissiveIntensity = 1;
    material.emissiveMap = materialParams.emissiveMap === void 0 ? null : materialParams.emissiveMap;
    material.bumpMap = materialParams.bumpMap === void 0 ? null : materialParams.bumpMap;
    material.bumpScale = 1;
    material.normalMap = materialParams.normalMap === void 0 ? null : materialParams.normalMap;
    material.normalMapType = THREE.TangentSpaceNormalMap;
    if (materialParams.normalScale)
      material.normalScale = materialParams.normalScale;
    material.displacementMap = null;
    material.displacementScale = 1;
    material.displacementBias = 0;
    material.specularMap = materialParams.specularMap === void 0 ? null : materialParams.specularMap;
    material.specular = materialParams.specular;
    material.glossinessMap = materialParams.glossinessMap === void 0 ? null : materialParams.glossinessMap;
    material.glossiness = materialParams.glossiness;
    material.alphaMap = null;
    material.envMap = materialParams.envMap === void 0 ? null : materialParams.envMap;
    material.envMapIntensity = 1;
    material.refractionRatio = 0.98;
    return material;
  }
};
var GLTFMeshQuantizationExtension = class {
  constructor() {
    this.name = EXTENSIONS.KHR_MESH_QUANTIZATION;
  }
};
var GLTFCubicSplineInterpolant = class extends THREE.Interpolant {
  constructor(parameterPositions, sampleValues, sampleSize, resultBuffer) {
    super(parameterPositions, sampleValues, sampleSize, resultBuffer);
  }
  copySampleValue_(index) {
    const result = this.resultBuffer, values = this.sampleValues, valueSize = this.valueSize, offset = index * valueSize * 3 + valueSize;
    for (let i = 0; i !== valueSize; i++) {
      result[i] = values[offset + i];
    }
    return result;
  }
};
GLTFCubicSplineInterpolant.prototype.beforeStart_ = GLTFCubicSplineInterpolant.prototype.copySampleValue_;
GLTFCubicSplineInterpolant.prototype.afterEnd_ = GLTFCubicSplineInterpolant.prototype.copySampleValue_;
GLTFCubicSplineInterpolant.prototype.interpolate_ = function(i1, t0, t2, t1) {
  const result = this.resultBuffer;
  const values = this.sampleValues;
  const stride = this.valueSize;
  const stride2 = stride * 2;
  const stride3 = stride * 3;
  const td = t1 - t0;
  const p = (t2 - t0) / td;
  const pp = p * p;
  const ppp = pp * p;
  const offset1 = i1 * stride3;
  const offset0 = offset1 - stride3;
  const s2 = -2 * ppp + 3 * pp;
  const s3 = ppp - pp;
  const s0 = 1 - s2;
  const s1 = s3 - pp + p;
  for (let i = 0; i !== stride; i++) {
    const p0 = values[offset0 + i + stride];
    const m0 = values[offset0 + i + stride2] * td;
    const p1 = values[offset1 + i + stride];
    const m1 = values[offset1 + i] * td;
    result[i] = s0 * p0 + s1 * m0 + s2 * p1 + s3 * m1;
  }
  return result;
};
var _q = new THREE.Quaternion();
var GLTFCubicSplineQuaternionInterpolant = class extends GLTFCubicSplineInterpolant {
  interpolate_(i1, t0, t2, t1) {
    const result = super.interpolate_(i1, t0, t2, t1);
    _q.fromArray(result).normalize().toArray(result);
    return result;
  }
};
var WEBGL_CONSTANTS = {
  FLOAT: 5126,
  FLOAT_MAT3: 35675,
  FLOAT_MAT4: 35676,
  FLOAT_VEC2: 35664,
  FLOAT_VEC3: 35665,
  FLOAT_VEC4: 35666,
  LINEAR: 9729,
  REPEAT: 10497,
  SAMPLER_2D: 35678,
  POINTS: 0,
  LINES: 1,
  LINE_LOOP: 2,
  LINE_STRIP: 3,
  TRIANGLES: 4,
  TRIANGLE_STRIP: 5,
  TRIANGLE_FAN: 6,
  UNSIGNED_BYTE: 5121,
  UNSIGNED_SHORT: 5123
};
var WEBGL_COMPONENT_TYPES = {
  5120: Int8Array,
  5121: Uint8Array,
  5122: Int16Array,
  5123: Uint16Array,
  5125: Uint32Array,
  5126: Float32Array
};
var WEBGL_FILTERS = {
  9728: THREE.NearestFilter,
  9729: THREE.LinearFilter,
  9984: THREE.NearestMipmapNearestFilter,
  9985: THREE.LinearMipmapNearestFilter,
  9986: THREE.NearestMipmapLinearFilter,
  9987: THREE.LinearMipmapLinearFilter
};
var WEBGL_WRAPPINGS = {
  33071: THREE.ClampToEdgeWrapping,
  33648: THREE.MirroredRepeatWrapping,
  10497: THREE.RepeatWrapping
};
var WEBGL_TYPE_SIZES = {
  "SCALAR": 1,
  "VEC2": 2,
  "VEC3": 3,
  "VEC4": 4,
  "MAT2": 4,
  "MAT3": 9,
  "MAT4": 16
};
var ATTRIBUTES = {
  POSITION: "position",
  NORMAL: "normal",
  TANGENT: "tangent",
  TEXCOORD_0: "uv",
  TEXCOORD_1: "uv2",
  COLOR_0: "color",
  WEIGHTS_0: "skinWeight",
  JOINTS_0: "skinIndex"
};
var PATH_PROPERTIES = {
  scale: "scale",
  translation: "position",
  rotation: "quaternion",
  weights: "morphTargetInfluences"
};
var INTERPOLATION = {
  CUBICSPLINE: void 0,
  LINEAR: THREE.InterpolateLinear,
  STEP: THREE.InterpolateDiscrete
};
var ALPHA_MODES = {
  OPAQUE: "OPAQUE",
  MASK: "MASK",
  BLEND: "BLEND"
};
function createDefaultMaterial(cache) {
  if (cache["DefaultMaterial"] === void 0) {
    cache["DefaultMaterial"] = new THREE.MeshStandardMaterial({
      color: 16777215,
      emissive: 0,
      metalness: 1,
      roughness: 1,
      transparent: false,
      depthTest: true,
      side: THREE.FrontSide
    });
  }
  return cache["DefaultMaterial"];
}
function addUnknownExtensionsToUserData(knownExtensions, object, objectDef) {
  for (const name2 in objectDef.extensions) {
    if (knownExtensions[name2] === void 0) {
      object.userData.gltfExtensions = object.userData.gltfExtensions || {};
      object.userData.gltfExtensions[name2] = objectDef.extensions[name2];
    }
  }
}
function assignExtrasToUserData(object, gltfDef) {
  if (gltfDef.extras !== void 0) {
    if (typeof gltfDef.extras === "object") {
      Object.assign(object.userData, gltfDef.extras);
    } else {
      console.warn("THREE.GLTFLoader: Ignoring primitive type .extras, " + gltfDef.extras);
    }
  }
}
function addMorphTargets(geometry, targets, parser) {
  let hasMorphPosition = false;
  let hasMorphNormal = false;
  for (let i = 0, il = targets.length; i < il; i++) {
    const target = targets[i];
    if (target.POSITION !== void 0)
      hasMorphPosition = true;
    if (target.NORMAL !== void 0)
      hasMorphNormal = true;
    if (hasMorphPosition && hasMorphNormal)
      break;
  }
  if (!hasMorphPosition && !hasMorphNormal)
    return Promise.resolve(geometry);
  const pendingPositionAccessors = [];
  const pendingNormalAccessors = [];
  for (let i = 0, il = targets.length; i < il; i++) {
    const target = targets[i];
    if (hasMorphPosition) {
      const pendingAccessor = target.POSITION !== void 0 ? parser.getDependency("accessor", target.POSITION) : geometry.attributes.position;
      pendingPositionAccessors.push(pendingAccessor);
    }
    if (hasMorphNormal) {
      const pendingAccessor = target.NORMAL !== void 0 ? parser.getDependency("accessor", target.NORMAL) : geometry.attributes.normal;
      pendingNormalAccessors.push(pendingAccessor);
    }
  }
  return Promise.all([
    Promise.all(pendingPositionAccessors),
    Promise.all(pendingNormalAccessors)
  ]).then(function(accessors) {
    const morphPositions = accessors[0];
    const morphNormals = accessors[1];
    if (hasMorphPosition)
      geometry.morphAttributes.position = morphPositions;
    if (hasMorphNormal)
      geometry.morphAttributes.normal = morphNormals;
    geometry.morphTargetsRelative = true;
    return geometry;
  });
}
function updateMorphTargets(mesh, meshDef) {
  mesh.updateMorphTargets();
  if (meshDef.weights !== void 0) {
    for (let i = 0, il = meshDef.weights.length; i < il; i++) {
      mesh.morphTargetInfluences[i] = meshDef.weights[i];
    }
  }
  if (meshDef.extras && Array.isArray(meshDef.extras.targetNames)) {
    const targetNames = meshDef.extras.targetNames;
    if (mesh.morphTargetInfluences.length === targetNames.length) {
      mesh.morphTargetDictionary = {};
      for (let i = 0, il = targetNames.length; i < il; i++) {
        mesh.morphTargetDictionary[targetNames[i]] = i;
      }
    } else {
      console.warn("THREE.GLTFLoader: Invalid extras.targetNames length. Ignoring names.");
    }
  }
}
function createPrimitiveKey(primitiveDef) {
  const dracoExtension = primitiveDef.extensions && primitiveDef.extensions[EXTENSIONS.KHR_DRACO_MESH_COMPRESSION];
  let geometryKey;
  if (dracoExtension) {
    geometryKey = "draco:" + dracoExtension.bufferView + ":" + dracoExtension.indices + ":" + createAttributesKey(dracoExtension.attributes);
  } else {
    geometryKey = primitiveDef.indices + ":" + createAttributesKey(primitiveDef.attributes) + ":" + primitiveDef.mode;
  }
  return geometryKey;
}
function createAttributesKey(attributes) {
  let attributesKey = "";
  const keys = Object.keys(attributes).sort();
  for (let i = 0, il = keys.length; i < il; i++) {
    attributesKey += keys[i] + ":" + attributes[keys[i]] + ";";
  }
  return attributesKey;
}
function getNormalizedComponentScale(constructor) {
  switch (constructor) {
    case Int8Array:
      return 1 / 127;
    case Uint8Array:
      return 1 / 255;
    case Int16Array:
      return 1 / 32767;
    case Uint16Array:
      return 1 / 65535;
    default:
      throw new Error("THREE.GLTFLoader: Unsupported normalized accessor component type.");
  }
}
var GLTFParser = class {
  constructor(json = {}, options = {}) {
    this.json = json;
    this.extensions = {};
    this.plugins = {};
    this.options = options;
    this.cache = new GLTFRegistry();
    this.associations = /* @__PURE__ */ new Map();
    this.primitiveCache = {};
    this.meshCache = { refs: {}, uses: {} };
    this.cameraCache = { refs: {}, uses: {} };
    this.lightCache = { refs: {}, uses: {} };
    this.textureCache = {};
    this.nodeNamesUsed = {};
    if (typeof createImageBitmap !== "undefined" && /Firefox|^((?!chrome|android).)*safari/i.test(navigator.userAgent) === false) {
      this.textureLoader = new THREE.ImageBitmapLoader(this.options.manager);
    } else {
      this.textureLoader = new THREE.TextureLoader(this.options.manager);
    }
    this.textureLoader.setCrossOrigin(this.options.crossOrigin);
    this.textureLoader.setRequestHeader(this.options.requestHeader);
    this.fileLoader = new THREE.FileLoader(this.options.manager);
    this.fileLoader.setResponseType("arraybuffer");
    if (this.options.crossOrigin === "use-credentials") {
      this.fileLoader.setWithCredentials(true);
    }
  }
  setExtensions(extensions) {
    this.extensions = extensions;
  }
  setPlugins(plugins) {
    this.plugins = plugins;
  }
  parse(onLoad, onError) {
    const parser = this;
    const json = this.json;
    const extensions = this.extensions;
    this.cache.removeAll();
    this._invokeAll(function(ext) {
      return ext._markDefs && ext._markDefs();
    });
    Promise.all(this._invokeAll(function(ext) {
      return ext.beforeRoot && ext.beforeRoot();
    })).then(function() {
      return Promise.all([
        parser.getDependencies("scene"),
        parser.getDependencies("animation"),
        parser.getDependencies("camera")
      ]);
    }).then(function(dependencies) {
      const result = {
        scene: dependencies[0][json.scene || 0],
        scenes: dependencies[0],
        animations: dependencies[1],
        cameras: dependencies[2],
        asset: json.asset,
        parser,
        userData: {}
      };
      addUnknownExtensionsToUserData(extensions, result, json);
      assignExtrasToUserData(result, json);
      Promise.all(parser._invokeAll(function(ext) {
        return ext.afterRoot && ext.afterRoot(result);
      })).then(function() {
        onLoad(result);
      });
    }).catch(onError);
  }
  _markDefs() {
    const nodeDefs = this.json.nodes || [];
    const skinDefs = this.json.skins || [];
    const meshDefs = this.json.meshes || [];
    for (let skinIndex = 0, skinLength = skinDefs.length; skinIndex < skinLength; skinIndex++) {
      const joints = skinDefs[skinIndex].joints;
      for (let i = 0, il = joints.length; i < il; i++) {
        nodeDefs[joints[i]].isBone = true;
      }
    }
    for (let nodeIndex = 0, nodeLength = nodeDefs.length; nodeIndex < nodeLength; nodeIndex++) {
      const nodeDef = nodeDefs[nodeIndex];
      if (nodeDef.mesh !== void 0) {
        this._addNodeRef(this.meshCache, nodeDef.mesh);
        if (nodeDef.skin !== void 0) {
          meshDefs[nodeDef.mesh].isSkinnedMesh = true;
        }
      }
      if (nodeDef.camera !== void 0) {
        this._addNodeRef(this.cameraCache, nodeDef.camera);
      }
    }
  }
  _addNodeRef(cache, index) {
    if (index === void 0)
      return;
    if (cache.refs[index] === void 0) {
      cache.refs[index] = cache.uses[index] = 0;
    }
    cache.refs[index]++;
  }
  _getNodeRef(cache, index, object) {
    if (cache.refs[index] <= 1)
      return object;
    const ref = object.clone();
    const updateMappings = (original, clone5) => {
      const mappings = this.associations.get(original);
      if (mappings != null) {
        this.associations.set(clone5, mappings);
      }
      for (const [i, child] of original.children.entries()) {
        updateMappings(child, clone5.children[i]);
      }
    };
    updateMappings(object, ref);
    ref.name += "_instance_" + cache.uses[index]++;
    return ref;
  }
  _invokeOne(func) {
    const extensions = Object.values(this.plugins);
    extensions.push(this);
    for (let i = 0; i < extensions.length; i++) {
      const result = func(extensions[i]);
      if (result)
        return result;
    }
    return null;
  }
  _invokeAll(func) {
    const extensions = Object.values(this.plugins);
    extensions.unshift(this);
    const pending = [];
    for (let i = 0; i < extensions.length; i++) {
      const result = func(extensions[i]);
      if (result)
        pending.push(result);
    }
    return pending;
  }
  getDependency(type2, index) {
    const cacheKey = type2 + ":" + index;
    let dependency = this.cache.get(cacheKey);
    if (!dependency) {
      switch (type2) {
        case "scene":
          dependency = this.loadScene(index);
          break;
        case "node":
          dependency = this.loadNode(index);
          break;
        case "mesh":
          dependency = this._invokeOne(function(ext) {
            return ext.loadMesh && ext.loadMesh(index);
          });
          break;
        case "accessor":
          dependency = this.loadAccessor(index);
          break;
        case "bufferView":
          dependency = this._invokeOne(function(ext) {
            return ext.loadBufferView && ext.loadBufferView(index);
          });
          break;
        case "buffer":
          dependency = this.loadBuffer(index);
          break;
        case "material":
          dependency = this._invokeOne(function(ext) {
            return ext.loadMaterial && ext.loadMaterial(index);
          });
          break;
        case "texture":
          dependency = this._invokeOne(function(ext) {
            return ext.loadTexture && ext.loadTexture(index);
          });
          break;
        case "skin":
          dependency = this.loadSkin(index);
          break;
        case "animation":
          dependency = this.loadAnimation(index);
          break;
        case "camera":
          dependency = this.loadCamera(index);
          break;
        default:
          throw new Error("Unknown type: " + type2);
      }
      this.cache.add(cacheKey, dependency);
    }
    return dependency;
  }
  getDependencies(type2) {
    let dependencies = this.cache.get(type2);
    if (!dependencies) {
      const parser = this;
      const defs = this.json[type2 + (type2 === "mesh" ? "es" : "s")] || [];
      dependencies = Promise.all(defs.map(function(def, index) {
        return parser.getDependency(type2, index);
      }));
      this.cache.add(type2, dependencies);
    }
    return dependencies;
  }
  loadBuffer(bufferIndex) {
    const bufferDef = this.json.buffers[bufferIndex];
    const loader = this.fileLoader;
    if (bufferDef.type && bufferDef.type !== "arraybuffer") {
      throw new Error("THREE.GLTFLoader: " + bufferDef.type + " buffer type is not supported.");
    }
    if (bufferDef.uri === void 0 && bufferIndex === 0) {
      return Promise.resolve(this.extensions[EXTENSIONS.KHR_BINARY_GLTF].body);
    }
    const options = this.options;
    return new Promise(function(resolve, reject) {
      loader.load(THREE.LoaderUtils.resolveURL(bufferDef.uri, options.path), resolve, void 0, function() {
        reject(new Error('THREE.GLTFLoader: Failed to load buffer "' + bufferDef.uri + '".'));
      });
    });
  }
  loadBufferView(bufferViewIndex) {
    const bufferViewDef = this.json.bufferViews[bufferViewIndex];
    return this.getDependency("buffer", bufferViewDef.buffer).then(function(buffer) {
      const byteLength = bufferViewDef.byteLength || 0;
      const byteOffset = bufferViewDef.byteOffset || 0;
      return buffer.slice(byteOffset, byteOffset + byteLength);
    });
  }
  loadAccessor(accessorIndex) {
    const parser = this;
    const json = this.json;
    const accessorDef = this.json.accessors[accessorIndex];
    if (accessorDef.bufferView === void 0 && accessorDef.sparse === void 0) {
      return Promise.resolve(null);
    }
    const pendingBufferViews = [];
    if (accessorDef.bufferView !== void 0) {
      pendingBufferViews.push(this.getDependency("bufferView", accessorDef.bufferView));
    } else {
      pendingBufferViews.push(null);
    }
    if (accessorDef.sparse !== void 0) {
      pendingBufferViews.push(this.getDependency("bufferView", accessorDef.sparse.indices.bufferView));
      pendingBufferViews.push(this.getDependency("bufferView", accessorDef.sparse.values.bufferView));
    }
    return Promise.all(pendingBufferViews).then(function(bufferViews) {
      const bufferView = bufferViews[0];
      const itemSize = WEBGL_TYPE_SIZES[accessorDef.type];
      const TypedArray = WEBGL_COMPONENT_TYPES[accessorDef.componentType];
      const elementBytes = TypedArray.BYTES_PER_ELEMENT;
      const itemBytes = elementBytes * itemSize;
      const byteOffset = accessorDef.byteOffset || 0;
      const byteStride = accessorDef.bufferView !== void 0 ? json.bufferViews[accessorDef.bufferView].byteStride : void 0;
      const normalized = accessorDef.normalized === true;
      let array, bufferAttribute;
      if (byteStride && byteStride !== itemBytes) {
        const ibSlice = Math.floor(byteOffset / byteStride);
        const ibCacheKey = "THREE.InterleavedBuffer:" + accessorDef.bufferView + ":" + accessorDef.componentType + ":" + ibSlice + ":" + accessorDef.count;
        let ib = parser.cache.get(ibCacheKey);
        if (!ib) {
          array = new TypedArray(bufferView, ibSlice * byteStride, accessorDef.count * byteStride / elementBytes);
          ib = new THREE.InterleavedBuffer(array, byteStride / elementBytes);
          parser.cache.add(ibCacheKey, ib);
        }
        bufferAttribute = new THREE.InterleavedBufferAttribute(ib, itemSize, byteOffset % byteStride / elementBytes, normalized);
      } else {
        if (bufferView === null) {
          array = new TypedArray(accessorDef.count * itemSize);
        } else {
          array = new TypedArray(bufferView, byteOffset, accessorDef.count * itemSize);
        }
        bufferAttribute = new THREE.BufferAttribute(array, itemSize, normalized);
      }
      if (accessorDef.sparse !== void 0) {
        const itemSizeIndices = WEBGL_TYPE_SIZES.SCALAR;
        const TypedArrayIndices = WEBGL_COMPONENT_TYPES[accessorDef.sparse.indices.componentType];
        const byteOffsetIndices = accessorDef.sparse.indices.byteOffset || 0;
        const byteOffsetValues = accessorDef.sparse.values.byteOffset || 0;
        const sparseIndices = new TypedArrayIndices(bufferViews[1], byteOffsetIndices, accessorDef.sparse.count * itemSizeIndices);
        const sparseValues = new TypedArray(bufferViews[2], byteOffsetValues, accessorDef.sparse.count * itemSize);
        if (bufferView !== null) {
          bufferAttribute = new THREE.BufferAttribute(bufferAttribute.array.slice(), bufferAttribute.itemSize, bufferAttribute.normalized);
        }
        for (let i = 0, il = sparseIndices.length; i < il; i++) {
          const index = sparseIndices[i];
          bufferAttribute.setX(index, sparseValues[i * itemSize]);
          if (itemSize >= 2)
            bufferAttribute.setY(index, sparseValues[i * itemSize + 1]);
          if (itemSize >= 3)
            bufferAttribute.setZ(index, sparseValues[i * itemSize + 2]);
          if (itemSize >= 4)
            bufferAttribute.setW(index, sparseValues[i * itemSize + 3]);
          if (itemSize >= 5)
            throw new Error("THREE.GLTFLoader: Unsupported itemSize in sparse THREE.BufferAttribute.");
        }
      }
      return bufferAttribute;
    });
  }
  loadTexture(textureIndex) {
    const json = this.json;
    const options = this.options;
    const textureDef = json.textures[textureIndex];
    const source = json.images[textureDef.source];
    let loader = this.textureLoader;
    if (source.uri) {
      const handler = options.manager.getHandler(source.uri);
      if (handler !== null)
        loader = handler;
    }
    return this.loadTextureImage(textureIndex, source, loader);
  }
  loadTextureImage(textureIndex, source, loader) {
    const parser = this;
    const json = this.json;
    const options = this.options;
    const textureDef = json.textures[textureIndex];
    const cacheKey = (source.uri || source.bufferView) + ":" + textureDef.sampler;
    if (this.textureCache[cacheKey]) {
      return this.textureCache[cacheKey];
    }
    const URL2 = self.URL || self.webkitURL;
    let sourceURI = source.uri || "";
    let isObjectURL = false;
    if (source.bufferView !== void 0) {
      sourceURI = parser.getDependency("bufferView", source.bufferView).then(function(bufferView) {
        isObjectURL = true;
        const blob = new Blob([bufferView], { type: source.mimeType });
        sourceURI = URL2.createObjectURL(blob);
        return sourceURI;
      });
    } else if (source.uri === void 0) {
      throw new Error("THREE.GLTFLoader: Image " + textureIndex + " is missing URI and bufferView");
    }
    const promise = Promise.resolve(sourceURI).then(function(sourceURI2) {
      return new Promise(function(resolve, reject) {
        let onLoad = resolve;
        if (loader.isImageBitmapLoader === true) {
          onLoad = function(imageBitmap) {
            const texture = new THREE.Texture(imageBitmap);
            texture.needsUpdate = true;
            resolve(texture);
          };
        }
        loader.load(THREE.LoaderUtils.resolveURL(sourceURI2, options.path), onLoad, void 0, reject);
      });
    }).then(function(texture) {
      if (isObjectURL === true) {
        URL2.revokeObjectURL(sourceURI);
      }
      texture.flipY = false;
      if (textureDef.name)
        texture.name = textureDef.name;
      const samplers = json.samplers || {};
      const sampler = samplers[textureDef.sampler] || {};
      texture.magFilter = WEBGL_FILTERS[sampler.magFilter] || THREE.LinearFilter;
      texture.minFilter = WEBGL_FILTERS[sampler.minFilter] || THREE.LinearMipmapLinearFilter;
      texture.wrapS = WEBGL_WRAPPINGS[sampler.wrapS] || THREE.RepeatWrapping;
      texture.wrapT = WEBGL_WRAPPINGS[sampler.wrapT] || THREE.RepeatWrapping;
      parser.associations.set(texture, { textures: textureIndex });
      return texture;
    }).catch(function() {
      console.error("THREE.GLTFLoader: Couldn't load texture", sourceURI);
      return null;
    });
    this.textureCache[cacheKey] = promise;
    return promise;
  }
  assignTexture(materialParams, mapName, mapDef) {
    const parser = this;
    return this.getDependency("texture", mapDef.index).then(function(texture) {
      if (mapDef.texCoord !== void 0 && mapDef.texCoord != 0 && !(mapName === "aoMap" && mapDef.texCoord == 1)) {
        console.warn("THREE.GLTFLoader: Custom UV set " + mapDef.texCoord + " for texture " + mapName + " not yet supported.");
      }
      if (parser.extensions[EXTENSIONS.KHR_TEXTURE_TRANSFORM]) {
        const transform2 = mapDef.extensions !== void 0 ? mapDef.extensions[EXTENSIONS.KHR_TEXTURE_TRANSFORM] : void 0;
        if (transform2) {
          const gltfReference = parser.associations.get(texture);
          texture = parser.extensions[EXTENSIONS.KHR_TEXTURE_TRANSFORM].extendTexture(texture, transform2);
          parser.associations.set(texture, gltfReference);
        }
      }
      materialParams[mapName] = texture;
      return texture;
    });
  }
  assignFinalMaterial(mesh) {
    const geometry = mesh.geometry;
    let material = mesh.material;
    const useDerivativeTangents = geometry.attributes.tangent === void 0;
    const useVertexColors = geometry.attributes.color !== void 0;
    const useFlatShading = geometry.attributes.normal === void 0;
    if (mesh.isPoints) {
      const cacheKey = "THREE.PointsMaterial:" + material.uuid;
      let pointsMaterial = this.cache.get(cacheKey);
      if (!pointsMaterial) {
        pointsMaterial = new THREE.PointsMaterial();
        THREE.Material.prototype.copy.call(pointsMaterial, material);
        pointsMaterial.color.copy(material.color);
        pointsMaterial.map = material.map;
        pointsMaterial.sizeAttenuation = false;
        this.cache.add(cacheKey, pointsMaterial);
      }
      material = pointsMaterial;
    } else if (mesh.isLine) {
      const cacheKey = "THREE.LineBasicMaterial:" + material.uuid;
      let lineMaterial = this.cache.get(cacheKey);
      if (!lineMaterial) {
        lineMaterial = new THREE.LineBasicMaterial();
        THREE.Material.prototype.copy.call(lineMaterial, material);
        lineMaterial.color.copy(material.color);
        this.cache.add(cacheKey, lineMaterial);
      }
      material = lineMaterial;
    }
    if (useDerivativeTangents || useVertexColors || useFlatShading) {
      let cacheKey = "ClonedMaterial:" + material.uuid + ":";
      if (material.isGLTFSpecularGlossinessMaterial)
        cacheKey += "specular-glossiness:";
      if (useDerivativeTangents)
        cacheKey += "derivative-tangents:";
      if (useVertexColors)
        cacheKey += "vertex-colors:";
      if (useFlatShading)
        cacheKey += "flat-shading:";
      let cachedMaterial = this.cache.get(cacheKey);
      if (!cachedMaterial) {
        cachedMaterial = material.clone();
        if (useVertexColors)
          cachedMaterial.vertexColors = true;
        if (useFlatShading)
          cachedMaterial.flatShading = true;
        if (useDerivativeTangents) {
          if (cachedMaterial.normalScale)
            cachedMaterial.normalScale.y *= -1;
          if (cachedMaterial.clearcoatNormalScale)
            cachedMaterial.clearcoatNormalScale.y *= -1;
        }
        this.cache.add(cacheKey, cachedMaterial);
        this.associations.set(cachedMaterial, this.associations.get(material));
      }
      material = cachedMaterial;
    }
    if (material.aoMap && geometry.attributes.uv2 === void 0 && geometry.attributes.uv !== void 0) {
      geometry.setAttribute("uv2", geometry.attributes.uv);
    }
    mesh.material = material;
  }
  getMaterialType() {
    return THREE.MeshStandardMaterial;
  }
  loadMaterial(materialIndex) {
    const parser = this;
    const json = this.json;
    const extensions = this.extensions;
    const materialDef = json.materials[materialIndex];
    let materialType;
    const materialParams = {};
    const materialExtensions = materialDef.extensions || {};
    const pending = [];
    if (materialExtensions[EXTENSIONS.KHR_MATERIALS_PBR_SPECULAR_GLOSSINESS]) {
      const sgExtension = extensions[EXTENSIONS.KHR_MATERIALS_PBR_SPECULAR_GLOSSINESS];
      materialType = sgExtension.getMaterialType();
      pending.push(sgExtension.extendParams(materialParams, materialDef, parser));
    } else if (materialExtensions[EXTENSIONS.KHR_MATERIALS_UNLIT]) {
      const kmuExtension = extensions[EXTENSIONS.KHR_MATERIALS_UNLIT];
      materialType = kmuExtension.getMaterialType();
      pending.push(kmuExtension.extendParams(materialParams, materialDef, parser));
    } else {
      const metallicRoughness = materialDef.pbrMetallicRoughness || {};
      materialParams.color = new THREE.Color(1, 1, 1);
      materialParams.opacity = 1;
      if (Array.isArray(metallicRoughness.baseColorFactor)) {
        const array = metallicRoughness.baseColorFactor;
        materialParams.color.fromArray(array);
        materialParams.opacity = array[3];
      }
      if (metallicRoughness.baseColorTexture !== void 0) {
        pending.push(parser.assignTexture(materialParams, "map", metallicRoughness.baseColorTexture));
      }
      materialParams.metalness = metallicRoughness.metallicFactor !== void 0 ? metallicRoughness.metallicFactor : 1;
      materialParams.roughness = metallicRoughness.roughnessFactor !== void 0 ? metallicRoughness.roughnessFactor : 1;
      if (metallicRoughness.metallicRoughnessTexture !== void 0) {
        pending.push(parser.assignTexture(materialParams, "metalnessMap", metallicRoughness.metallicRoughnessTexture));
        pending.push(parser.assignTexture(materialParams, "roughnessMap", metallicRoughness.metallicRoughnessTexture));
      }
      materialType = this._invokeOne(function(ext) {
        return ext.getMaterialType && ext.getMaterialType(materialIndex);
      });
      pending.push(Promise.all(this._invokeAll(function(ext) {
        return ext.extendMaterialParams && ext.extendMaterialParams(materialIndex, materialParams);
      })));
    }
    if (materialDef.doubleSided === true) {
      materialParams.side = THREE.DoubleSide;
    }
    const alphaMode = materialDef.alphaMode || ALPHA_MODES.OPAQUE;
    if (alphaMode === ALPHA_MODES.BLEND) {
      materialParams.transparent = true;
      materialParams.depthWrite = false;
    } else {
      materialParams.transparent = false;
      if (alphaMode === ALPHA_MODES.MASK) {
        materialParams.alphaTest = materialDef.alphaCutoff !== void 0 ? materialDef.alphaCutoff : 0.5;
      }
    }
    if (materialDef.normalTexture !== void 0 && materialType !== THREE.MeshBasicMaterial) {
      pending.push(parser.assignTexture(materialParams, "normalMap", materialDef.normalTexture));
      materialParams.normalScale = new THREE.Vector2(1, 1);
      if (materialDef.normalTexture.scale !== void 0) {
        const scale4 = materialDef.normalTexture.scale;
        materialParams.normalScale.set(scale4, scale4);
      }
    }
    if (materialDef.occlusionTexture !== void 0 && materialType !== THREE.MeshBasicMaterial) {
      pending.push(parser.assignTexture(materialParams, "aoMap", materialDef.occlusionTexture));
      if (materialDef.occlusionTexture.strength !== void 0) {
        materialParams.aoMapIntensity = materialDef.occlusionTexture.strength;
      }
    }
    if (materialDef.emissiveFactor !== void 0 && materialType !== THREE.MeshBasicMaterial) {
      materialParams.emissive = new THREE.Color().fromArray(materialDef.emissiveFactor);
    }
    if (materialDef.emissiveTexture !== void 0 && materialType !== THREE.MeshBasicMaterial) {
      pending.push(parser.assignTexture(materialParams, "emissiveMap", materialDef.emissiveTexture));
    }
    return Promise.all(pending).then(function() {
      let material;
      if (materialType === GLTFMeshStandardSGMaterial) {
        material = extensions[EXTENSIONS.KHR_MATERIALS_PBR_SPECULAR_GLOSSINESS].createMaterial(materialParams);
      } else {
        material = new materialType(materialParams);
      }
      if (materialDef.name)
        material.name = materialDef.name;
      if (material.map)
        material.map.encoding = THREE.sRGBEncoding;
      if (material.emissiveMap)
        material.emissiveMap.encoding = THREE.sRGBEncoding;
      assignExtrasToUserData(material, materialDef);
      parser.associations.set(material, { materials: materialIndex });
      if (materialDef.extensions)
        addUnknownExtensionsToUserData(extensions, material, materialDef);
      return material;
    });
  }
  createUniqueName(originalName) {
    const sanitizedName = THREE.PropertyBinding.sanitizeNodeName(originalName || "");
    let name2 = sanitizedName;
    for (let i = 1; this.nodeNamesUsed[name2]; ++i) {
      name2 = sanitizedName + "_" + i;
    }
    this.nodeNamesUsed[name2] = true;
    return name2;
  }
  loadGeometries(primitives) {
    const parser = this;
    const extensions = this.extensions;
    const cache = this.primitiveCache;
    function createDracoPrimitive(primitive) {
      return extensions[EXTENSIONS.KHR_DRACO_MESH_COMPRESSION].decodePrimitive(primitive, parser).then(function(geometry) {
        return addPrimitiveAttributes(geometry, primitive, parser);
      });
    }
    const pending = [];
    for (let i = 0, il = primitives.length; i < il; i++) {
      const primitive = primitives[i];
      const cacheKey = createPrimitiveKey(primitive);
      const cached = cache[cacheKey];
      if (cached) {
        pending.push(cached.promise);
      } else {
        let geometryPromise;
        if (primitive.extensions && primitive.extensions[EXTENSIONS.KHR_DRACO_MESH_COMPRESSION]) {
          geometryPromise = createDracoPrimitive(primitive);
        } else {
          geometryPromise = addPrimitiveAttributes(new THREE.BufferGeometry(), primitive, parser);
        }
        cache[cacheKey] = { primitive, promise: geometryPromise };
        pending.push(geometryPromise);
      }
    }
    return Promise.all(pending);
  }
  loadMesh(meshIndex) {
    const parser = this;
    const json = this.json;
    const extensions = this.extensions;
    const meshDef = json.meshes[meshIndex];
    const primitives = meshDef.primitives;
    const pending = [];
    for (let i = 0, il = primitives.length; i < il; i++) {
      const material = primitives[i].material === void 0 ? createDefaultMaterial(this.cache) : this.getDependency("material", primitives[i].material);
      pending.push(material);
    }
    pending.push(parser.loadGeometries(primitives));
    return Promise.all(pending).then(function(results) {
      const materials2 = results.slice(0, results.length - 1);
      const geometries = results[results.length - 1];
      const meshes = [];
      for (let i = 0, il = geometries.length; i < il; i++) {
        const geometry = geometries[i];
        const primitive = primitives[i];
        let mesh;
        const material = materials2[i];
        if (primitive.mode === WEBGL_CONSTANTS.TRIANGLES || primitive.mode === WEBGL_CONSTANTS.TRIANGLE_STRIP || primitive.mode === WEBGL_CONSTANTS.TRIANGLE_FAN || primitive.mode === void 0) {
          mesh = meshDef.isSkinnedMesh === true ? new THREE.SkinnedMesh(geometry, material) : new THREE.Mesh(geometry, material);
          if (mesh.isSkinnedMesh === true && !mesh.geometry.attributes.skinWeight.normalized) {
            mesh.normalizeSkinWeights();
          }
          if (primitive.mode === WEBGL_CONSTANTS.TRIANGLE_STRIP) {
            mesh.geometry = toTrianglesDrawMode(mesh.geometry, THREE.TriangleStripDrawMode);
          } else if (primitive.mode === WEBGL_CONSTANTS.TRIANGLE_FAN) {
            mesh.geometry = toTrianglesDrawMode(mesh.geometry, THREE.TriangleFanDrawMode);
          }
        } else if (primitive.mode === WEBGL_CONSTANTS.LINES) {
          mesh = new THREE.LineSegments(geometry, material);
        } else if (primitive.mode === WEBGL_CONSTANTS.LINE_STRIP) {
          mesh = new THREE.Line(geometry, material);
        } else if (primitive.mode === WEBGL_CONSTANTS.LINE_LOOP) {
          mesh = new THREE.LineLoop(geometry, material);
        } else if (primitive.mode === WEBGL_CONSTANTS.POINTS) {
          mesh = new THREE.Points(geometry, material);
        } else {
          throw new Error("THREE.GLTFLoader: Primitive mode unsupported: " + primitive.mode);
        }
        if (Object.keys(mesh.geometry.morphAttributes).length > 0) {
          updateMorphTargets(mesh, meshDef);
        }
        mesh.name = parser.createUniqueName(meshDef.name || "mesh_" + meshIndex);
        assignExtrasToUserData(mesh, meshDef);
        if (primitive.extensions)
          addUnknownExtensionsToUserData(extensions, mesh, primitive);
        parser.assignFinalMaterial(mesh);
        meshes.push(mesh);
      }
      for (let i = 0, il = meshes.length; i < il; i++) {
        parser.associations.set(meshes[i], {
          meshes: meshIndex,
          primitives: i
        });
      }
      if (meshes.length === 1) {
        return meshes[0];
      }
      const group2 = new THREE.Group();
      parser.associations.set(group2, { meshes: meshIndex });
      for (let i = 0, il = meshes.length; i < il; i++) {
        group2.add(meshes[i]);
      }
      return group2;
    });
  }
  loadCamera(cameraIndex) {
    let camera;
    const cameraDef = this.json.cameras[cameraIndex];
    const params = cameraDef[cameraDef.type];
    if (!params) {
      console.warn("THREE.GLTFLoader: Missing camera parameters.");
      return;
    }
    if (cameraDef.type === "perspective") {
      camera = new THREE.PerspectiveCamera(THREE.MathUtils.radToDeg(params.yfov), params.aspectRatio || 1, params.znear || 1, params.zfar || 2e6);
    } else if (cameraDef.type === "orthographic") {
      camera = new THREE.OrthographicCamera(-params.xmag, params.xmag, params.ymag, -params.ymag, params.znear, params.zfar);
    }
    if (cameraDef.name)
      camera.name = this.createUniqueName(cameraDef.name);
    assignExtrasToUserData(camera, cameraDef);
    return Promise.resolve(camera);
  }
  loadSkin(skinIndex) {
    const skinDef = this.json.skins[skinIndex];
    const skinEntry = { joints: skinDef.joints };
    if (skinDef.inverseBindMatrices === void 0) {
      return Promise.resolve(skinEntry);
    }
    return this.getDependency("accessor", skinDef.inverseBindMatrices).then(function(accessor) {
      skinEntry.inverseBindMatrices = accessor;
      return skinEntry;
    });
  }
  loadAnimation(animationIndex) {
    const json = this.json;
    const animationDef = json.animations[animationIndex];
    const pendingNodes = [];
    const pendingInputAccessors = [];
    const pendingOutputAccessors = [];
    const pendingSamplers = [];
    const pendingTargets = [];
    for (let i = 0, il = animationDef.channels.length; i < il; i++) {
      const channel = animationDef.channels[i];
      const sampler = animationDef.samplers[channel.sampler];
      const target = channel.target;
      const name2 = target.node !== void 0 ? target.node : target.id;
      const input = animationDef.parameters !== void 0 ? animationDef.parameters[sampler.input] : sampler.input;
      const output = animationDef.parameters !== void 0 ? animationDef.parameters[sampler.output] : sampler.output;
      pendingNodes.push(this.getDependency("node", name2));
      pendingInputAccessors.push(this.getDependency("accessor", input));
      pendingOutputAccessors.push(this.getDependency("accessor", output));
      pendingSamplers.push(sampler);
      pendingTargets.push(target);
    }
    return Promise.all([
      Promise.all(pendingNodes),
      Promise.all(pendingInputAccessors),
      Promise.all(pendingOutputAccessors),
      Promise.all(pendingSamplers),
      Promise.all(pendingTargets)
    ]).then(function(dependencies) {
      const nodes2 = dependencies[0];
      const inputAccessors = dependencies[1];
      const outputAccessors = dependencies[2];
      const samplers = dependencies[3];
      const targets = dependencies[4];
      const tracks = [];
      for (let i = 0, il = nodes2.length; i < il; i++) {
        const node = nodes2[i];
        const inputAccessor = inputAccessors[i];
        const outputAccessor = outputAccessors[i];
        const sampler = samplers[i];
        const target = targets[i];
        if (node === void 0)
          continue;
        node.updateMatrix();
        node.matrixAutoUpdate = true;
        let TypedKeyframeTrack;
        switch (PATH_PROPERTIES[target.path]) {
          case PATH_PROPERTIES.weights:
            TypedKeyframeTrack = THREE.NumberKeyframeTrack;
            break;
          case PATH_PROPERTIES.rotation:
            TypedKeyframeTrack = THREE.QuaternionKeyframeTrack;
            break;
          case PATH_PROPERTIES.position:
          case PATH_PROPERTIES.scale:
          default:
            TypedKeyframeTrack = THREE.VectorKeyframeTrack;
            break;
        }
        const targetName = node.name ? node.name : node.uuid;
        const interpolation = sampler.interpolation !== void 0 ? INTERPOLATION[sampler.interpolation] : THREE.InterpolateLinear;
        const targetNames = [];
        if (PATH_PROPERTIES[target.path] === PATH_PROPERTIES.weights) {
          node.traverse(function(object) {
            if (object.morphTargetInfluences) {
              targetNames.push(object.name ? object.name : object.uuid);
            }
          });
        } else {
          targetNames.push(targetName);
        }
        let outputArray = outputAccessor.array;
        if (outputAccessor.normalized) {
          const scale4 = getNormalizedComponentScale(outputArray.constructor);
          const scaled = new Float32Array(outputArray.length);
          for (let j = 0, jl = outputArray.length; j < jl; j++) {
            scaled[j] = outputArray[j] * scale4;
          }
          outputArray = scaled;
        }
        for (let j = 0, jl = targetNames.length; j < jl; j++) {
          const track = new TypedKeyframeTrack(targetNames[j] + "." + PATH_PROPERTIES[target.path], inputAccessor.array, outputArray, interpolation);
          if (sampler.interpolation === "CUBICSPLINE") {
            track.createInterpolant = function InterpolantFactoryMethodGLTFCubicSpline(result) {
              const interpolantType = this instanceof THREE.QuaternionKeyframeTrack ? GLTFCubicSplineQuaternionInterpolant : GLTFCubicSplineInterpolant;
              return new interpolantType(this.times, this.values, this.getValueSize() / 3, result);
            };
            track.createInterpolant.isInterpolantFactoryMethodGLTFCubicSpline = true;
          }
          tracks.push(track);
        }
      }
      const name2 = animationDef.name ? animationDef.name : "animation_" + animationIndex;
      return new THREE.AnimationClip(name2, void 0, tracks);
    });
  }
  createNodeMesh(nodeIndex) {
    const json = this.json;
    const parser = this;
    const nodeDef = json.nodes[nodeIndex];
    if (nodeDef.mesh === void 0)
      return null;
    return parser.getDependency("mesh", nodeDef.mesh).then(function(mesh) {
      const node = parser._getNodeRef(parser.meshCache, nodeDef.mesh, mesh);
      if (nodeDef.weights !== void 0) {
        node.traverse(function(o) {
          if (!o.isMesh)
            return;
          for (let i = 0, il = nodeDef.weights.length; i < il; i++) {
            o.morphTargetInfluences[i] = nodeDef.weights[i];
          }
        });
      }
      return node;
    });
  }
  loadNode(nodeIndex) {
    const json = this.json;
    const extensions = this.extensions;
    const parser = this;
    const nodeDef = json.nodes[nodeIndex];
    const nodeName = nodeDef.name ? parser.createUniqueName(nodeDef.name) : "";
    return function() {
      const pending = [];
      const meshPromise = parser._invokeOne(function(ext) {
        return ext.createNodeMesh && ext.createNodeMesh(nodeIndex);
      });
      if (meshPromise) {
        pending.push(meshPromise);
      }
      if (nodeDef.camera !== void 0) {
        pending.push(parser.getDependency("camera", nodeDef.camera).then(function(camera) {
          return parser._getNodeRef(parser.cameraCache, nodeDef.camera, camera);
        }));
      }
      parser._invokeAll(function(ext) {
        return ext.createNodeAttachment && ext.createNodeAttachment(nodeIndex);
      }).forEach(function(promise) {
        pending.push(promise);
      });
      return Promise.all(pending);
    }().then(function(objects) {
      let node;
      if (nodeDef.isBone === true) {
        node = new THREE.Bone();
      } else if (objects.length > 1) {
        node = new THREE.Group();
      } else if (objects.length === 1) {
        node = objects[0];
      } else {
        node = new THREE.Object3D();
      }
      if (node !== objects[0]) {
        for (let i = 0, il = objects.length; i < il; i++) {
          node.add(objects[i]);
        }
      }
      if (nodeDef.name) {
        node.userData.name = nodeDef.name;
        node.name = nodeName;
      }
      assignExtrasToUserData(node, nodeDef);
      if (nodeDef.extensions)
        addUnknownExtensionsToUserData(extensions, node, nodeDef);
      if (nodeDef.matrix !== void 0) {
        const matrix = new THREE.Matrix4();
        matrix.fromArray(nodeDef.matrix);
        node.applyMatrix4(matrix);
      } else {
        if (nodeDef.translation !== void 0) {
          node.position.fromArray(nodeDef.translation);
        }
        if (nodeDef.rotation !== void 0) {
          node.quaternion.fromArray(nodeDef.rotation);
        }
        if (nodeDef.scale !== void 0) {
          node.scale.fromArray(nodeDef.scale);
        }
      }
      if (!parser.associations.has(node)) {
        parser.associations.set(node, {});
      }
      parser.associations.get(node).nodes = nodeIndex;
      return node;
    });
  }
  loadScene(sceneIndex) {
    const json = this.json;
    const extensions = this.extensions;
    const sceneDef = this.json.scenes[sceneIndex];
    const parser = this;
    const scene = new THREE.Group();
    if (sceneDef.name)
      scene.name = parser.createUniqueName(sceneDef.name);
    assignExtrasToUserData(scene, sceneDef);
    if (sceneDef.extensions)
      addUnknownExtensionsToUserData(extensions, scene, sceneDef);
    const nodeIds = sceneDef.nodes || [];
    const pending = [];
    for (let i = 0, il = nodeIds.length; i < il; i++) {
      pending.push(buildNodeHierarchy(nodeIds[i], scene, json, parser));
    }
    return Promise.all(pending).then(function() {
      const reduceAssociations = (node) => {
        const reducedAssociations = /* @__PURE__ */ new Map();
        for (const [key, value2] of parser.associations) {
          if (key instanceof THREE.Material || key instanceof THREE.Texture) {
            reducedAssociations.set(key, value2);
          }
        }
        node.traverse((node2) => {
          const mappings = parser.associations.get(node2);
          if (mappings != null) {
            reducedAssociations.set(node2, mappings);
          }
        });
        return reducedAssociations;
      };
      parser.associations = reduceAssociations(scene);
      return scene;
    });
  }
};
function buildNodeHierarchy(nodeId, parentObject, json, parser) {
  const nodeDef = json.nodes[nodeId];
  return parser.getDependency("node", nodeId).then(function(node) {
    if (nodeDef.skin === void 0)
      return node;
    let skinEntry;
    return parser.getDependency("skin", nodeDef.skin).then(function(skin) {
      skinEntry = skin;
      const pendingJoints = [];
      for (let i = 0, il = skinEntry.joints.length; i < il; i++) {
        pendingJoints.push(parser.getDependency("node", skinEntry.joints[i]));
      }
      return Promise.all(pendingJoints);
    }).then(function(jointNodes) {
      node.traverse(function(mesh) {
        if (!mesh.isMesh)
          return;
        const bones = [];
        const boneInverses = [];
        for (let j = 0, jl = jointNodes.length; j < jl; j++) {
          const jointNode = jointNodes[j];
          if (jointNode) {
            bones.push(jointNode);
            const mat = new THREE.Matrix4();
            if (skinEntry.inverseBindMatrices !== void 0) {
              mat.fromArray(skinEntry.inverseBindMatrices.array, j * 16);
            }
            boneInverses.push(mat);
          } else {
            console.warn('THREE.GLTFLoader: Joint "%s" could not be found.', skinEntry.joints[j]);
          }
        }
        mesh.bind(new THREE.Skeleton(bones, boneInverses), mesh.matrixWorld);
      });
      return node;
    });
  }).then(function(node) {
    parentObject.add(node);
    const pending = [];
    if (nodeDef.children) {
      const children = nodeDef.children;
      for (let i = 0, il = children.length; i < il; i++) {
        const child = children[i];
        pending.push(buildNodeHierarchy(child, node, json, parser));
      }
    }
    return Promise.all(pending);
  });
}
function computeBounds(geometry, primitiveDef, parser) {
  const attributes = primitiveDef.attributes;
  const box = new THREE.Box3();
  if (attributes.POSITION !== void 0) {
    const accessor = parser.json.accessors[attributes.POSITION];
    const min3 = accessor.min;
    const max3 = accessor.max;
    if (min3 !== void 0 && max3 !== void 0) {
      box.set(new THREE.Vector3(min3[0], min3[1], min3[2]), new THREE.Vector3(max3[0], max3[1], max3[2]));
      if (accessor.normalized) {
        const boxScale = getNormalizedComponentScale(WEBGL_COMPONENT_TYPES[accessor.componentType]);
        box.min.multiplyScalar(boxScale);
        box.max.multiplyScalar(boxScale);
      }
    } else {
      console.warn("THREE.GLTFLoader: Missing min/max properties for accessor POSITION.");
      return;
    }
  } else {
    return;
  }
  const targets = primitiveDef.targets;
  if (targets !== void 0) {
    const maxDisplacement = new THREE.Vector3();
    const vector = new THREE.Vector3();
    for (let i = 0, il = targets.length; i < il; i++) {
      const target = targets[i];
      if (target.POSITION !== void 0) {
        const accessor = parser.json.accessors[target.POSITION];
        const min3 = accessor.min;
        const max3 = accessor.max;
        if (min3 !== void 0 && max3 !== void 0) {
          vector.setX(Math.max(Math.abs(min3[0]), Math.abs(max3[0])));
          vector.setY(Math.max(Math.abs(min3[1]), Math.abs(max3[1])));
          vector.setZ(Math.max(Math.abs(min3[2]), Math.abs(max3[2])));
          if (accessor.normalized) {
            const boxScale = getNormalizedComponentScale(WEBGL_COMPONENT_TYPES[accessor.componentType]);
            vector.multiplyScalar(boxScale);
          }
          maxDisplacement.max(vector);
        } else {
          console.warn("THREE.GLTFLoader: Missing min/max properties for accessor POSITION.");
        }
      }
    }
    box.expandByVector(maxDisplacement);
  }
  geometry.boundingBox = box;
  const sphere = new THREE.Sphere();
  box.getCenter(sphere.center);
  sphere.radius = box.min.distanceTo(box.max) / 2;
  geometry.boundingSphere = sphere;
}
function addPrimitiveAttributes(geometry, primitiveDef, parser) {
  const attributes = primitiveDef.attributes;
  const pending = [];
  function assignAttributeAccessor(accessorIndex, attributeName) {
    return parser.getDependency("accessor", accessorIndex).then(function(accessor) {
      geometry.setAttribute(attributeName, accessor);
    });
  }
  for (const gltfAttributeName in attributes) {
    const threeAttributeName = ATTRIBUTES[gltfAttributeName] || gltfAttributeName.toLowerCase();
    if (threeAttributeName in geometry.attributes)
      continue;
    pending.push(assignAttributeAccessor(attributes[gltfAttributeName], threeAttributeName));
  }
  if (primitiveDef.indices !== void 0 && !geometry.index) {
    const accessor = parser.getDependency("accessor", primitiveDef.indices).then(function(accessor2) {
      geometry.setIndex(accessor2);
    });
    pending.push(accessor);
  }
  assignExtrasToUserData(geometry, primitiveDef);
  computeBounds(geometry, primitiveDef, parser);
  return Promise.all(pending).then(function() {
    return primitiveDef.targets !== void 0 ? addMorphTargets(geometry, primitiveDef.targets, parser) : geometry;
  });
}
function toTrianglesDrawMode(geometry, drawMode) {
  let index = geometry.getIndex();
  if (index === null) {
    const indices = [];
    const position2 = geometry.getAttribute("position");
    if (position2 !== void 0) {
      for (let i = 0; i < position2.count; i++) {
        indices.push(i);
      }
      geometry.setIndex(indices);
      index = geometry.getIndex();
    } else {
      console.error("THREE.GLTFLoader.toTrianglesDrawMode(): Undefined position attribute. Processing not possible.");
      return geometry;
    }
  }
  const numberOfTriangles = index.count - 2;
  const newIndices = [];
  if (drawMode === THREE.TriangleFanDrawMode) {
    for (let i = 1; i <= numberOfTriangles; i++) {
      newIndices.push(index.getX(0));
      newIndices.push(index.getX(i));
      newIndices.push(index.getX(i + 1));
    }
  } else {
    for (let i = 0; i < numberOfTriangles; i++) {
      if (i % 2 === 0) {
        newIndices.push(index.getX(i));
        newIndices.push(index.getX(i + 1));
        newIndices.push(index.getX(i + 2));
      } else {
        newIndices.push(index.getX(i + 2));
        newIndices.push(index.getX(i + 1));
        newIndices.push(index.getX(i));
      }
    }
  }
  if (newIndices.length / 3 !== numberOfTriangles) {
    console.error("THREE.GLTFLoader.toTrianglesDrawMode(): Unable to generate correct amount of triangles.");
  }
  const newGeometry = geometry.clone();
  newGeometry.setIndex(newIndices);
  return newGeometry;
}

// ../threejs/examples/webxr/motion-controllers.module.js
var Constants = {
  Handedness: Object.freeze({
    NONE: "none",
    LEFT: "left",
    RIGHT: "right"
  }),
  ComponentState: Object.freeze({
    DEFAULT: "default",
    TOUCHED: "touched",
    PRESSED: "pressed"
  }),
  ComponentProperty: Object.freeze({
    BUTTON: "button",
    X_AXIS: "xAxis",
    Y_AXIS: "yAxis",
    STATE: "state"
  }),
  ComponentType: Object.freeze({
    TRIGGER: "trigger",
    SQUEEZE: "squeeze",
    TOUCHPAD: "touchpad",
    THUMBSTICK: "thumbstick",
    BUTTON: "button"
  }),
  ButtonTouchThreshold: 0.05,
  AxisTouchThreshold: 0.1,
  VisualResponseProperty: Object.freeze({
    TRANSFORM: "transform",
    VISIBILITY: "visibility"
  })
};
async function fetchJsonFile(path) {
  const response = await fetch(path);
  if (!response.ok) {
    throw new Error(response.statusText);
  } else {
    return response.json();
  }
}
async function fetchProfilesList(basePath) {
  if (!basePath) {
    throw new Error("No basePath supplied");
  }
  const profileListFileName = "profilesList.json";
  const profilesList = await fetchJsonFile(`${basePath}/${profileListFileName}`);
  return profilesList;
}
async function fetchProfile(xrInputSource, basePath, defaultProfile = null, getAssetPath = true) {
  if (!xrInputSource) {
    throw new Error("No xrInputSource supplied");
  }
  if (!basePath) {
    throw new Error("No basePath supplied");
  }
  const supportedProfilesList = await fetchProfilesList(basePath);
  let match;
  xrInputSource.profiles.some((profileId) => {
    const supportedProfile = supportedProfilesList[profileId];
    if (supportedProfile) {
      match = {
        profileId,
        profilePath: `${basePath}/${supportedProfile.path}`,
        deprecated: !!supportedProfile.deprecated
      };
    }
    return !!match;
  });
  if (!match) {
    if (!defaultProfile) {
      throw new Error("No matching profile name found");
    }
    const supportedProfile = supportedProfilesList[defaultProfile];
    if (!supportedProfile) {
      throw new Error(`No matching profile name found and default profile "${defaultProfile}" missing.`);
    }
    match = {
      profileId: defaultProfile,
      profilePath: `${basePath}/${supportedProfile.path}`,
      deprecated: !!supportedProfile.deprecated
    };
  }
  const profile = await fetchJsonFile(match.profilePath);
  let assetPath;
  if (getAssetPath) {
    let layout;
    if (xrInputSource.handedness === "any") {
      layout = profile.layouts[Object.keys(profile.layouts)[0]];
    } else {
      layout = profile.layouts[xrInputSource.handedness];
    }
    if (!layout) {
      throw new Error(`No matching handedness, ${xrInputSource.handedness}, in profile ${match.profileId}`);
    }
    if (layout.assetPath) {
      assetPath = match.profilePath.replace("profile.json", layout.assetPath);
    }
  }
  return { profile, assetPath };
}
var defaultComponentValues = {
  xAxis: 0,
  yAxis: 0,
  button: 0,
  state: Constants.ComponentState.DEFAULT
};
function normalizeAxes(x = 0, y = 0) {
  let xAxis = x;
  let yAxis = y;
  const hypotenuse = Math.sqrt(x * x + y * y);
  if (hypotenuse > 1) {
    const theta = Math.atan2(y, x);
    xAxis = Math.cos(theta);
    yAxis = Math.sin(theta);
  }
  const result = {
    normalizedXAxis: xAxis * 0.5 + 0.5,
    normalizedYAxis: yAxis * 0.5 + 0.5
  };
  return result;
}
var VisualResponse = class {
  constructor(visualResponseDescription) {
    this.componentProperty = visualResponseDescription.componentProperty;
    this.states = visualResponseDescription.states;
    this.valueNodeName = visualResponseDescription.valueNodeName;
    this.valueNodeProperty = visualResponseDescription.valueNodeProperty;
    if (this.valueNodeProperty === Constants.VisualResponseProperty.TRANSFORM) {
      this.minNodeName = visualResponseDescription.minNodeName;
      this.maxNodeName = visualResponseDescription.maxNodeName;
    }
    this.value = 0;
    this.updateFromComponent(defaultComponentValues);
  }
  updateFromComponent({
    xAxis,
    yAxis,
    button,
    state
  }) {
    const { normalizedXAxis, normalizedYAxis } = normalizeAxes(xAxis, yAxis);
    switch (this.componentProperty) {
      case Constants.ComponentProperty.X_AXIS:
        this.value = this.states.includes(state) ? normalizedXAxis : 0.5;
        break;
      case Constants.ComponentProperty.Y_AXIS:
        this.value = this.states.includes(state) ? normalizedYAxis : 0.5;
        break;
      case Constants.ComponentProperty.BUTTON:
        this.value = this.states.includes(state) ? button : 0;
        break;
      case Constants.ComponentProperty.STATE:
        if (this.valueNodeProperty === Constants.VisualResponseProperty.VISIBILITY) {
          this.value = this.states.includes(state);
        } else {
          this.value = this.states.includes(state) ? 1 : 0;
        }
        break;
      default:
        throw new Error(`Unexpected visualResponse componentProperty ${this.componentProperty}`);
    }
  }
};
var Component = class {
  constructor(componentId, componentDescription) {
    if (!componentId || !componentDescription || !componentDescription.visualResponses || !componentDescription.gamepadIndices || Object.keys(componentDescription.gamepadIndices).length === 0) {
      throw new Error("Invalid arguments supplied");
    }
    this.id = componentId;
    this.type = componentDescription.type;
    this.rootNodeName = componentDescription.rootNodeName;
    this.touchPointNodeName = componentDescription.touchPointNodeName;
    this.visualResponses = {};
    Object.keys(componentDescription.visualResponses).forEach((responseName) => {
      const visualResponse = new VisualResponse(componentDescription.visualResponses[responseName]);
      this.visualResponses[responseName] = visualResponse;
    });
    this.gamepadIndices = Object.assign({}, componentDescription.gamepadIndices);
    this.values = {
      state: Constants.ComponentState.DEFAULT,
      button: this.gamepadIndices.button !== void 0 ? 0 : void 0,
      xAxis: this.gamepadIndices.xAxis !== void 0 ? 0 : void 0,
      yAxis: this.gamepadIndices.yAxis !== void 0 ? 0 : void 0
    };
  }
  get data() {
    const data = { id: this.id, ...this.values };
    return data;
  }
  updateFromGamepad(gamepad) {
    this.values.state = Constants.ComponentState.DEFAULT;
    if (this.gamepadIndices.button !== void 0 && gamepad.buttons.length > this.gamepadIndices.button) {
      const gamepadButton = gamepad.buttons[this.gamepadIndices.button];
      this.values.button = gamepadButton.value;
      this.values.button = this.values.button < 0 ? 0 : this.values.button;
      this.values.button = this.values.button > 1 ? 1 : this.values.button;
      if (gamepadButton.pressed || this.values.button === 1) {
        this.values.state = Constants.ComponentState.PRESSED;
      } else if (gamepadButton.touched || this.values.button > Constants.ButtonTouchThreshold) {
        this.values.state = Constants.ComponentState.TOUCHED;
      }
    }
    if (this.gamepadIndices.xAxis !== void 0 && gamepad.axes.length > this.gamepadIndices.xAxis) {
      this.values.xAxis = gamepad.axes[this.gamepadIndices.xAxis];
      this.values.xAxis = this.values.xAxis < -1 ? -1 : this.values.xAxis;
      this.values.xAxis = this.values.xAxis > 1 ? 1 : this.values.xAxis;
      if (this.values.state === Constants.ComponentState.DEFAULT && Math.abs(this.values.xAxis) > Constants.AxisTouchThreshold) {
        this.values.state = Constants.ComponentState.TOUCHED;
      }
    }
    if (this.gamepadIndices.yAxis !== void 0 && gamepad.axes.length > this.gamepadIndices.yAxis) {
      this.values.yAxis = gamepad.axes[this.gamepadIndices.yAxis];
      this.values.yAxis = this.values.yAxis < -1 ? -1 : this.values.yAxis;
      this.values.yAxis = this.values.yAxis > 1 ? 1 : this.values.yAxis;
      if (this.values.state === Constants.ComponentState.DEFAULT && Math.abs(this.values.yAxis) > Constants.AxisTouchThreshold) {
        this.values.state = Constants.ComponentState.TOUCHED;
      }
    }
    Object.values(this.visualResponses).forEach((visualResponse) => {
      visualResponse.updateFromComponent(this.values);
    });
  }
};
var MotionController = class {
  constructor(xrInputSource, profile, assetUrl) {
    if (!xrInputSource) {
      throw new Error("No xrInputSource supplied");
    }
    if (!profile) {
      throw new Error("No profile supplied");
    }
    this.xrInputSource = xrInputSource;
    this.assetUrl = assetUrl;
    this.id = profile.profileId;
    this.layoutDescription = profile.layouts[xrInputSource.handedness];
    this.components = {};
    Object.keys(this.layoutDescription.components).forEach((componentId) => {
      const componentDescription = this.layoutDescription.components[componentId];
      this.components[componentId] = new Component(componentId, componentDescription);
    });
    this.updateFromGamepad();
  }
  get gripSpace() {
    return this.xrInputSource.gripSpace;
  }
  get targetRaySpace() {
    return this.xrInputSource.targetRaySpace;
  }
  get data() {
    const data = [];
    Object.values(this.components).forEach((component) => {
      data.push(component.data);
    });
    return data;
  }
  updateFromGamepad() {
    Object.values(this.components).forEach((component) => {
      component.updateFromGamepad(this.xrInputSource.gamepad);
    });
  }
};

// ../threejs/examples/webxr/XRControllerModelFactory.js
var DEFAULT_PROFILES_PATH = "https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets@1.0/dist/profiles";
var DEFAULT_PROFILE = "generic-trigger";
var XRControllerModel = class extends THREE.Object3D {
  constructor() {
    super();
    this.motionController = null;
    this.envMap = null;
  }
  setEnvironmentMap(envMap) {
    if (this.envMap == envMap) {
      return this;
    }
    this.envMap = envMap;
    this.traverse((child) => {
      if (child.isMesh) {
        child.material.envMap = this.envMap;
        child.material.needsUpdate = true;
      }
    });
    return this;
  }
  updateMatrixWorld(force) {
    super.updateMatrixWorld(force);
    if (!this.motionController)
      return;
    this.motionController.updateFromGamepad();
    Object.values(this.motionController.components).forEach((component) => {
      Object.values(component.visualResponses).forEach((visualResponse) => {
        const { valueNode, minNode, maxNode, value: value2, valueNodeProperty } = visualResponse;
        if (!valueNode)
          return;
        if (valueNodeProperty === Constants.VisualResponseProperty.VISIBILITY) {
          valueNode.visible = value2;
        } else if (valueNodeProperty === Constants.VisualResponseProperty.TRANSFORM) {
          valueNode.quaternion.slerpQuaternions(minNode.quaternion, maxNode.quaternion, value2);
          valueNode.position.lerpVectors(minNode.position, maxNode.position, value2);
        }
      });
    });
  }
};
function findNodes(motionController, scene) {
  Object.values(motionController.components).forEach((component) => {
    const { type: type2, touchPointNodeName, visualResponses } = component;
    if (type2 === Constants.ComponentType.TOUCHPAD) {
      component.touchPointNode = scene.getObjectByName(touchPointNodeName);
      if (component.touchPointNode) {
        const sphereGeometry = new THREE.SphereGeometry(1e-3);
        const material = new THREE.MeshBasicMaterial({ color: 255 });
        const sphere = new THREE.Mesh(sphereGeometry, material);
        component.touchPointNode.add(sphere);
      } else {
        console.warn(`Could not find touch dot, ${component.touchPointNodeName}, in touchpad component ${component.id}`);
      }
    }
    Object.values(visualResponses).forEach((visualResponse) => {
      const { valueNodeName, minNodeName, maxNodeName, valueNodeProperty } = visualResponse;
      if (valueNodeProperty === Constants.VisualResponseProperty.TRANSFORM) {
        visualResponse.minNode = scene.getObjectByName(minNodeName);
        visualResponse.maxNode = scene.getObjectByName(maxNodeName);
        if (!visualResponse.minNode) {
          console.warn(`Could not find ${minNodeName} in the model`);
          return;
        }
        if (!visualResponse.maxNode) {
          console.warn(`Could not find ${maxNodeName} in the model`);
          return;
        }
      }
      visualResponse.valueNode = scene.getObjectByName(valueNodeName);
      if (!visualResponse.valueNode) {
        console.warn(`Could not find ${valueNodeName} in the model`);
      }
    });
  });
}
function addAssetSceneToControllerModel(controllerModel, scene) {
  findNodes(controllerModel.motionController, scene);
  if (controllerModel.envMap) {
    scene.traverse((child) => {
      if (child.isMesh) {
        child.material.envMap = controllerModel.envMap;
        child.material.needsUpdate = true;
      }
    });
  }
  controllerModel.add(scene);
}
var XRControllerModelFactory = class {
  constructor(gltfLoader = null) {
    this.gltfLoader = gltfLoader;
    this.path = DEFAULT_PROFILES_PATH;
    this._assetCache = {};
    if (!this.gltfLoader) {
      this.gltfLoader = new GLTFLoader();
    }
  }
  createControllerModel(controller) {
    const controllerModel = new XRControllerModel();
    let scene = null;
    controller.addEventListener("connected", (event) => {
      const xrInputSource = event.data;
      if (xrInputSource.targetRayMode !== "tracked-pointer" || !xrInputSource.gamepad)
        return;
      fetchProfile(xrInputSource, this.path, DEFAULT_PROFILE).then(({ profile, assetPath }) => {
        controllerModel.motionController = new MotionController(xrInputSource, profile, assetPath);
        const cachedAsset = this._assetCache[controllerModel.motionController.assetUrl];
        if (cachedAsset) {
          scene = cachedAsset.scene.clone();
          addAssetSceneToControllerModel(controllerModel, scene);
        } else {
          if (!this.gltfLoader) {
            throw new Error("GLTFLoader not set.");
          }
          this.gltfLoader.setPath("");
          this.gltfLoader.load(controllerModel.motionController.assetUrl, (asset) => {
            this._assetCache[controllerModel.motionController.assetUrl] = asset;
            scene = asset.scene.clone();
            addAssetSceneToControllerModel(controllerModel, scene);
          }, null, () => {
            throw new Error(`Asset ${controllerModel.motionController.assetUrl} missing or malformed.`);
          });
        }
      }).catch((err) => {
        console.warn(err);
      });
    });
    controller.addEventListener("disconnected", () => {
      controllerModel.motionController = null;
      controllerModel.remove(scene);
      scene = null;
    });
    return controllerModel;
  }
};

// ../threejs/examples/webxr/XRHandPrimitiveModel.js
var _matrix = new THREE.Matrix4();
var _vector = new THREE.Vector3();
var _oculusBrowserV14CorrectionRight = new THREE.Matrix4().identity();
var _oculusBrowserV14CorrectionLeft = new THREE.Matrix4().identity();
if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
  _oculusBrowserV14CorrectionRight.makeRotationY(Math.PI / 2);
  _oculusBrowserV14CorrectionLeft.makeRotationY(-Math.PI / 2);
}
var XRHandPrimitiveModel = class {
  constructor(handModel, controller, path, handedness, options) {
    this.controller = controller;
    this.handModel = handModel;
    this.envMap = null;
    this.oculusBrowserV14Correction = handedness === "left" ? _oculusBrowserV14CorrectionLeft : _oculusBrowserV14CorrectionRight;
    let geometry;
    if (!options || !options.primitive || options.primitive === "sphere") {
      geometry = new THREE.SphereGeometry(1, 10, 10);
    } else if (options.primitive === "box") {
      geometry = new THREE.BoxGeometry(1, 1, 1);
    } else if (options.primitive === "bone") {
      geometry = new THREE.CylinderGeometry(0.5, 0.75, 2.25, 10, 1).rotateX(-Math.PI / 2);
    }
    const material = new THREE.MeshStandardMaterial();
    this.handMesh = new THREE.InstancedMesh(geometry, material, 30);
    this.handMesh.instanceMatrix.setUsage(THREE.DynamicDrawUsage);
    this.handMesh.castShadow = true;
    this.handMesh.receiveShadow = true;
    this.handModel.add(this.handMesh);
    this.joints = [
      "wrist",
      "thumb-metacarpal",
      "thumb-phalanx-proximal",
      "thumb-phalanx-distal",
      "thumb-tip",
      "index-finger-metacarpal",
      "index-finger-phalanx-proximal",
      "index-finger-phalanx-intermediate",
      "index-finger-phalanx-distal",
      "index-finger-tip",
      "middle-finger-metacarpal",
      "middle-finger-phalanx-proximal",
      "middle-finger-phalanx-intermediate",
      "middle-finger-phalanx-distal",
      "middle-finger-tip",
      "ring-finger-metacarpal",
      "ring-finger-phalanx-proximal",
      "ring-finger-phalanx-intermediate",
      "ring-finger-phalanx-distal",
      "ring-finger-tip",
      "pinky-finger-metacarpal",
      "pinky-finger-phalanx-proximal",
      "pinky-finger-phalanx-intermediate",
      "pinky-finger-phalanx-distal",
      "pinky-finger-tip"
    ];
  }
  updateMesh() {
    const defaultRadius = 8e-3;
    const joints = this.controller.joints;
    let count2 = 0;
    for (let i = 0; i < this.joints.length; i++) {
      const joint = joints[this.joints[i]];
      if (joint.visible) {
        _vector.setScalar(joint.jointRadius || defaultRadius);
        _matrix.compose(joint.position, joint.quaternion, _vector);
        _matrix.multiply(this.oculusBrowserV14Correction);
        this.handMesh.setMatrixAt(i, _matrix);
        count2++;
      }
    }
    this.handMesh.count = count2;
    this.handMesh.instanceMatrix.needsUpdate = true;
  }
};

// ../threejs/examples/webxr/XRHandMeshModel.js
var _oculusBrowserV14CorrectionRight2 = new THREE.Quaternion().identity();
var _oculusBrowserV14CorrectionLeft2 = new THREE.Quaternion().identity();
if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
  _oculusBrowserV14CorrectionRight2.setFromAxisAngle({ x: 0, y: 1, z: 0 }, Math.PI / 2);
  _oculusBrowserV14CorrectionLeft2.setFromAxisAngle({ x: 1, y: 0, z: 0 }, Math.PI).premultiply(_oculusBrowserV14CorrectionRight2);
}
var DEFAULT_HAND_PROFILE_PATH = "https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets@1.0/dist/profiles/generic-hand/";
var XRHandMeshModel = class {
  constructor(handModel, controller, path, handedness) {
    this.controller = controller;
    this.handModel = handModel;
    this.oculusBrowserV14Correction = handedness === "left" ? _oculusBrowserV14CorrectionLeft2 : _oculusBrowserV14CorrectionRight2;
    this.bones = [];
    const loader = new GLTFLoader();
    loader.setPath(path || DEFAULT_HAND_PROFILE_PATH);
    loader.load(`${handedness}.glb`, (gltf) => {
      const object = gltf.scene.children[0];
      this.handModel.add(object);
      const mesh = object.getObjectByProperty("type", "SkinnedMesh");
      mesh.frustumCulled = false;
      mesh.castShadow = true;
      mesh.receiveShadow = true;
      const joints = [
        "wrist",
        "thumb-metacarpal",
        "thumb-phalanx-proximal",
        "thumb-phalanx-distal",
        "thumb-tip",
        "index-finger-metacarpal",
        "index-finger-phalanx-proximal",
        "index-finger-phalanx-intermediate",
        "index-finger-phalanx-distal",
        "index-finger-tip",
        "middle-finger-metacarpal",
        "middle-finger-phalanx-proximal",
        "middle-finger-phalanx-intermediate",
        "middle-finger-phalanx-distal",
        "middle-finger-tip",
        "ring-finger-metacarpal",
        "ring-finger-phalanx-proximal",
        "ring-finger-phalanx-intermediate",
        "ring-finger-phalanx-distal",
        "ring-finger-tip",
        "pinky-finger-metacarpal",
        "pinky-finger-phalanx-proximal",
        "pinky-finger-phalanx-intermediate",
        "pinky-finger-phalanx-distal",
        "pinky-finger-tip"
      ];
      joints.forEach((jointName) => {
        const bone = object.getObjectByName(jointName);
        if (bone !== void 0) {
          bone.jointName = jointName;
        } else {
          console.warn(`Couldn't find ${jointName} in ${handedness} hand mesh`);
        }
        this.bones.push(bone);
      });
    });
  }
  updateMesh() {
    const XRJoints = this.controller.joints;
    for (let i = 0; i < this.bones.length; i++) {
      const bone = this.bones[i];
      if (bone) {
        const XRJoint = XRJoints[bone.jointName];
        if (XRJoint.visible) {
          const position2 = XRJoint.position;
          if (bone) {
            bone.position.copy(position2);
            bone.quaternion.copy(XRJoint.quaternion).multiply(this.oculusBrowserV14Correction);
          }
        }
      }
    }
  }
};

// ../threejs/examples/webxr/XRHandModelFactory.js
var XRHandModel = class extends THREE.Object3D {
  constructor(controller) {
    super();
    this.controller = controller;
    this.motionController = null;
    this.envMap = null;
    this.mesh = null;
  }
  updateMatrixWorld(force) {
    super.updateMatrixWorld(force);
    if (this.motionController) {
      this.motionController.updateMesh();
    }
  }
};
var XRHandModelFactory = class {
  constructor() {
    this.path = null;
  }
  setPath(path) {
    this.path = path;
    return this;
  }
  createHandModel(controller, profile) {
    const handModel = new XRHandModel(controller);
    controller.addEventListener("connected", (event) => {
      const xrInputSource = event.data;
      if (xrInputSource.hand && !handModel.motionController) {
        handModel.xrInputSource = xrInputSource;
        if (profile === void 0 || profile === "spheres") {
          handModel.motionController = new XRHandPrimitiveModel(handModel, controller, this.path, xrInputSource.handedness, { primitive: "sphere" });
        } else if (profile === "boxes") {
          handModel.motionController = new XRHandPrimitiveModel(handModel, controller, this.path, xrInputSource.handedness, { primitive: "box" });
        } else if (profile === "bones") {
          handModel.motionController = new XRHandPrimitiveModel(handModel, controller, this.path, xrInputSource.handedness, { primitive: "bone" });
        } else if (profile === "mesh") {
          handModel.motionController = new XRHandMeshModel(handModel, controller, this.path, xrInputSource.handedness);
        }
      }
    });
    controller.addEventListener("disconnected", () => {
    });
    return handModel;
  }
};

// ../event-system/PointerState.ts
var PointerState = class {
  buttons = 0;
  moveDistance = 0;
  dragDistance = 0;
  x = 0;
  y = 0;
  dx = 0;
  dy = 0;
  dz = 0;
  u = 0;
  v = 0;
  du = 0;
  dv = 0;
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
    this.x = ptr.x;
    this.y = ptr.y;
    this.dx = ptr.dx;
    this.dy = ptr.dy;
    this.dz = ptr.dz;
    this.u = ptr.u;
    this.v = ptr.v;
    this.du = ptr.du;
    this.dv = ptr.dv;
    this.canClick = ptr.canClick;
    this.dragging = ptr.dragging;
    this.ctrl = ptr.ctrl;
    this.alt = ptr.alt;
    this.shift = ptr.shift;
    this.meta = ptr.meta;
  }
  read(evt) {
    this.buttons = evt.buttons;
    this.x = evt.offsetX;
    this.y = evt.offsetY;
    this.dx = evt.movementX;
    this.dy = evt.movementY;
    this.dz = 0;
    this.ctrl = evt.ctrlKey;
    this.alt = evt.altKey;
    this.shift = evt.shiftKey;
    this.meta = evt.metaKey;
  }
};

// ../threejs/setGeometryUVsForCubemaps.ts
function setGeometryUVsForCubemaps(geom2) {
  const positions = geom2.attributes.position;
  const normals = geom2.attributes.normal;
  const uvs = geom2.attributes.uv;
  for (let n2 = 0; n2 < normals.count; ++n2) {
    const _x = n2 * normals.itemSize, _y = n2 * normals.itemSize + 1, _z = n2 * normals.itemSize + 2, nx = normals.array[_x], ny = normals.array[_y], nz = normals.array[_z], _nx_ = Math.abs(nx), _ny_ = Math.abs(ny), _nz_ = Math.abs(nz), px = positions.array[_x], py = positions.array[_y], pz = positions.array[_z], _px_ = Math.abs(px), _py_ = Math.abs(py), _pz_ = Math.abs(pz), _u = n2 * uvs.itemSize, _v = n2 * uvs.itemSize + 1;
    let u = uvs.array[_u], v = uvs.array[_v], largest = 0, mx = _nx_, max3 = _px_;
    if (_ny_ > mx) {
      largest = 1;
      mx = _ny_;
      max3 = _py_;
    }
    if (_nz_ > mx) {
      largest = 2;
      mx = _nz_;
      max3 = _pz_;
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
    u = (u / max3 + 1) / 8;
    v = (v / max3 + 1) / 6;
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
  constructor(type2, name2, evtSys, cursor) {
    this.type = type2;
    this.name = name2;
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
        objGraph;
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
  recheck() {
  }
  setEventState(type2) {
    this.evtSys.checkPointer(this, type2);
  }
  updateCursor(avatarHeadPos, curHit, defaultDistance) {
    if (this.cursor) {
      this.cursor.update(avatarHeadPos, curHit, defaultDistance, this.canMoveView, this.state, this.origin, this.direction);
    }
  }
  lastStateUpdate(updater) {
    if (this.lastState) {
      this.lastState.copy(this.state);
      updater();
      this.state.dragDistance = this.lastState.dragDistance;
    } else {
      updater();
      this.lastState = new PointerState();
      this.lastState.copy(this.state);
    }
  }
  onZoom(dz) {
    this.lastStateUpdate(() => this.state.dz = dz);
    this.setEventState("move");
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
        if (this.lastState.buttons === this.state.buttons) {
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
    const wasDragging = this.state.dragging;
    this.state.dragging = true;
    if (!wasDragging) {
      this.setEventState("dragstart");
    }
    this.state.canClick = false;
    this.setEventState("drag");
  }
  onPointerUp() {
    if (this.state.canClick) {
      const lastButtons = this.state.buttons;
      this.state.buttons = this.lastState.buttons;
      this.setEventState("click");
      this.state.buttons = lastButtons;
    }
    this.setEventState("up");
    this.state.dragDistance = 0;
    const wasDragging = this.state.dragging;
    this.state.dragging = false;
    if (wasDragging) {
      this.setEventState("dragend");
    }
  }
};

// ../threejs/examples/lines/LineSegmentsGeometry.js
var _box = new THREE.Box3();
var _vector2 = new THREE.Vector3();
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
    const start2 = this.attributes.instanceStart;
    const end2 = this.attributes.instanceEnd;
    if (start2 !== void 0) {
      start2.applyMatrix4(matrix);
      end2.applyMatrix4(matrix);
      start2.needsUpdate = true;
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
    const start2 = this.attributes.instanceStart;
    const end2 = this.attributes.instanceEnd;
    if (start2 !== void 0 && end2 !== void 0) {
      this.boundingBox.setFromBufferAttribute(start2);
      _box.setFromBufferAttribute(end2);
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
    const start2 = this.attributes.instanceStart;
    const end2 = this.attributes.instanceEnd;
    if (start2 !== void 0 && end2 !== void 0) {
      const center = this.boundingSphere.center;
      this.boundingBox.getCenter(center);
      let maxRadiusSq = 0;
      for (let i = 0, il = start2.count; i < il; i++) {
        _vector2.fromBufferAttribute(start2, i);
        maxRadiusSq = Math.max(maxRadiusSq, center.distanceToSquared(_vector2));
        _vector2.fromBufferAttribute(end2, i);
        maxRadiusSq = Math.max(maxRadiusSq, center.distanceToSquared(_vector2));
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
    var length4 = array.length - 3;
    var points = new Float32Array(2 * length4);
    for (var i = 0; i < length4; i += 3) {
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
    var length4 = array.length - 3;
    var colors = new Float32Array(2 * length4);
    for (var i = 0; i < length4; i += 3) {
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

// ../threejs/eventSystem/PointerHand.ts
var mcModelFactory = new XRControllerModelFactory();
var handModelFactory = new XRHandModelFactory();
var riftSCorrection = new THREE.Matrix4().makeRotationX(-7 * Math.PI / 9);
var newOrigin = new THREE.Vector3();
var newDirection = new THREE.Vector3();
var delta2 = new THREE.Vector3();
var buttonIndices = new PriorityMap([
  ["left", 1 /* Primary */, 0],
  ["left", 2 /* Secondary */, 1],
  ["left", 3 /* Menu */, 2],
  ["left", 4 /* Info */, 3],
  ["right", 1 /* Primary */, 0],
  ["right", 2 /* Secondary */, 1],
  ["right", 3 /* Menu */, 2],
  ["right", 4 /* Info */, 3]
]);
var pointerNames = /* @__PURE__ */ new Map([
  ["none", 15 /* MotionController */],
  ["left", 16 /* MotionControllerLeft */],
  ["right", 17 /* MotionControllerRight */]
]);
var PointerHand = class extends BasePointer {
  constructor(evtSys, renderer, index) {
    super("hand", 15 /* MotionController */, evtSys, new CursorColor());
    this.renderer = renderer;
    this.laser = new Laser(white, 2e-3);
    this.object = new THREE.Object3D();
    this._handedness = "none";
    this._isHand = false;
    this.inputSource = null;
    this._gamepad = null;
    this.useHaptics = true;
    this.object.add(this.controller = this.renderer.xr.getController(index), this.grip = this.renderer.xr.getControllerGrip(index), this.hand = this.renderer.xr.getHand(index));
    if (isDesktop() && isChrome() && !isOculusBrowser) {
      let maybeOculusRiftS = false;
      this.controller.traverse((child) => {
        const key = child.name.toLocaleLowerCase();
        if (key.indexOf("oculus") >= 0) {
          maybeOculusRiftS = true;
        }
      });
      if (maybeOculusRiftS) {
        this.laser.matrix.copy(riftSCorrection);
      }
    }
    this.controller.add(this.laser);
    this.grip.add(mcModelFactory.createControllerModel(this.controller));
    this.hand.add(handModelFactory.createHandModel(this.hand, "mesh"));
    this.onAxisMaxed = (evt) => {
      if (evt.axis === 2) {
        this.evtSys.onFlick(evt.value);
      }
    };
    this.controller.addEventListener("connected", () => {
      const session = this.renderer.xr.getSession();
      this.inputSource = session.inputSources[index];
      this.setGamepad(this.inputSource.gamepad);
      this._isHand = isDefined(this.inputSource.hand);
      this._handedness = this.inputSource.handedness;
      this.name = pointerNames.get(this.handedness);
      this.updateCursorSide();
      this.grip.visible = !this.isHand;
      this.controller.visible = !this.isHand;
      this.hand.visible = this.isHand;
      this.enabled = true;
      this.isActive = true;
      this.evtSys.onConnected(this);
    });
    this.controller.addEventListener("disconnected", () => {
      this.inputSource = null;
      this.setGamepad(null);
      this._isHand = false;
      this._handedness = "none";
      this.name = pointerNames.get(this.handedness);
      this.updateCursorSide();
      this.grip.visible = false;
      this.controller.visible = false;
      this.hand.visible = false;
      this.enabled = false;
      this.isActive = false;
      this.evtSys.onDisconnected(this);
      this.isActive = false;
    });
    const buttonDown = (btn) => {
      this.updateState();
      this.state.buttons = this.state.buttons | btn;
      this.onPointerDown();
    };
    const buttonUp = (btn) => {
      this.updateState();
      this.state.buttons = this.state.buttons & ~btn;
      this.onPointerUp();
    };
    this.controller.addEventListener("selectstart", () => buttonDown(1 /* Mouse0 */));
    this.controller.addEventListener("selectend", () => buttonUp(1 /* Mouse0 */));
    this.controller.addEventListener("squeezestart", () => buttonDown(2 /* Mouse1 */));
    this.controller.addEventListener("squeezeend", () => buttonUp(2 /* Mouse1 */));
  }
  vibrate() {
    this._vibrate();
  }
  async _vibrate() {
    if (this._gamepad && this.useHaptics) {
      try {
        await Promise.all(this._gamepad.hapticActuators.map((actuator) => actuator.pulse(0.25, 125)));
      } catch {
        this.useHaptics = false;
      }
    }
  }
  setGamepad(pad) {
    if (isDefined(this._gamepad) && isNullOrUndefined(pad)) {
      this._gamepad.clearEventListeners();
      this._gamepad = null;
    }
    if (isDefined(pad)) {
      if (isDefined(this._gamepad)) {
        this._gamepad.setPad(pad);
      } else {
        this._gamepad = new EventedGamepad(pad);
        this._gamepad.addEventListener("gamepadaxismaxed", this.onAxisMaxed);
      }
    }
  }
  get gamepad() {
    return this._gamepad;
  }
  get gamepadId() {
    return this._gamepad && this._gamepad.id;
  }
  get handedness() {
    return this._handedness;
  }
  get isHand() {
    return this._isHand;
  }
  get cursor() {
    return super.cursor;
  }
  set cursor(v) {
    super.cursor = v;
    this.updateCursorSide();
  }
  updateCursorSide() {
    const obj2 = this.cursor.object;
    if (obj2) {
      const sx = this.handedness === "left" ? -1 : 1;
      obj2.scale.set(sx, 1, 1);
    }
  }
  update() {
    if (this.enabled) {
      this.updateState();
      this.onPointerMove();
    }
  }
  updateState() {
    this.lastStateUpdate(() => {
      this.laser.getWorldPosition(newOrigin);
      this.laser.getWorldDirection(newDirection).multiplyScalar(-1);
      delta2.copy(this.origin).add(this.direction);
      this.origin.lerp(newOrigin, 0.9);
      this.direction.lerp(newDirection, 0.9).normalize();
      delta2.sub(this.origin).sub(this.direction);
      this.state.moveDistance += 1e-3 * delta2.length();
      if (isDefined(this._gamepad) && isDefined(this.inputSource)) {
        this.setGamepad(this.inputSource.gamepad);
      }
    });
  }
  isPressed(button) {
    if (!this._gamepad || !buttonIndices.has(this.handedness) || !buttonIndices.get(this.handedness).has(button)) {
      return false;
    }
    const index = buttonIndices.get(this.handedness).get(button);
    return this._gamepad.buttons[index].pressed;
  }
};

// ../threejs/eventSystem/BaseScreenPointer.ts
var BaseScreenPointer = class extends BasePointer {
  constructor(type2, name2, evtSys, renderer, camera, cursor) {
    super(type2, name2, evtSys, cursor);
    this.renderer = renderer;
    this.camera = camera;
    this.id = null;
    this.moveOnUpdate = false;
    const onPointerDown = (evt) => {
      if (this.checkEvent(evt)) {
        this.readEvent(evt);
        this.onPointerDown();
      }
    };
    this.element = this.renderer.domElement;
    this.element.addEventListener("pointerdown", onPointerDown);
    const onPointerMove = (evt) => {
      if (this.checkEvent(evt)) {
        this.readEvent(evt);
        this.onPointerMove();
      }
    };
    this.element.addEventListener("pointermove", onPointerMove);
    const onPointerUp = (evt) => {
      if (this.checkEvent(evt)) {
        this.readEvent(evt);
        this.onPointerUp();
      }
    };
    this.element.addEventListener("pointerup", onPointerUp);
  }
  get isTracking() {
    return this.id != null;
  }
  checkEvent(evt) {
    return evt.pointerType === this.type && evt.pointerId === this.id;
  }
  readEvent(evt) {
    if (this.checkEvent(evt)) {
      this.lastStateUpdate(() => this.state.read(evt));
      if (evt.type === "pointermove") {
        if (document.pointerLockElement) {
          const { x, y } = this.state;
          this.state.x = clamp(this.lastState.x + this.state.dx, 0, this.element.clientWidth);
          this.state.y = clamp(this.lastState.y + this.state.dy, 0, this.element.clientHeight);
          this.state.dx = this.state.x - x;
          this.state.dy = this.state.y - y;
        } else {
          this.state.dx = this.state.x - this.lastState.x;
          this.state.dy = this.state.y - this.lastState.y;
        }
      }
      this.state.moveDistance = Math.sqrt(this.state.dx * this.state.dx + this.state.dy * this.state.dy);
      this.state.u = unproject(project(this.state.x, 0, this.element.clientWidth), -1, 1);
      this.state.v = unproject(project(this.state.y, 0, this.element.clientHeight), -1, 1);
      this.state.du = 2 * this.state.dx / this.element.clientWidth;
      this.state.dv = 2 * this.state.dy / this.element.clientHeight;
    }
  }
  recheck() {
    this.moveOnUpdate = true;
    super.recheck();
  }
  update() {
    const cam = resolveCamera(this.renderer, this.camera);
    this.origin.setFromMatrixPosition(cam.matrixWorld);
    this.direction.set(this.state.u, -this.state.v, 0.5).unproject(cam).sub(this.origin).normalize();
    if (this.moveOnUpdate) {
      this.onPointerMove();
    }
  }
  onPointerMove() {
    this.moveOnUpdate = false;
    super.onPointerMove();
  }
  isPressed(button) {
    const mask = 1 << button;
    return this.state.buttons === mask;
  }
};

// ../threejs/eventSystem/PointerMouse.ts
var PointerMouse = class extends BaseScreenPointer {
  constructor(evtSys, renderer, camera) {
    const onPrep = (evt) => {
      if (evt.pointerType === "mouse" && this.id == null) {
        this.id = evt.pointerId;
      }
    };
    const unPrep = (evt) => {
      if (evt.pointerType === "mouse" && this.id != null) {
        this.id = null;
      }
    };
    const element = renderer.domElement;
    element.addEventListener("pointerdown", onPrep);
    element.addEventListener("pointermove", onPrep);
    super("mouse", 1 /* Mouse */, evtSys, renderer, camera, new CursorXRMouse(renderer));
    this.element.addEventListener("wheel", (evt) => {
      evt.preventDefault();
      const dz = -evt.deltaY * 0.5;
      this.onZoom(dz);
    }, { passive: false });
    this.element.addEventListener("contextmenu", (evt) => {
      evt.preventDefault();
    });
    this.element.addEventListener("pointerup", unPrep);
    this.element.addEventListener("pointercancel", unPrep);
    this.allowPointerLock = false;
    this.element.addEventListener("click", () => {
      if (this.allowPointerLock && !this.isPointerLocked) {
        this.lockPointer();
      }
    });
    this.canMoveView = true;
    Object.seal(this);
  }
  lockPointer() {
    this.element.requestPointerLock();
  }
  unlockPointer() {
    document.exitPointerLock();
  }
  get isPointerLocked() {
    return document.pointerLockElement != null;
  }
};

// ../threejs/eventSystem/PointerSingleTouch.ts
var touches = new Array();
var onPrePointerDown = (evt) => {
  if (evt.pointerType === "touch") {
    const pressCount = countPresses();
    let touch = arrayScan(touches, (t2) => t2.id === evt.pointerId);
    if (!touch) {
      touch = arrayScan(touches, (t2) => t2.id == null);
      touch.id = evt.pointerId;
      touch.state.buttons = 1 << pressCount;
    }
  }
};
var onPostPointerUp = (evt) => {
  if (evt.pointerType === "touch") {
    let touch = arrayScan(touches, (t2) => t2.id === evt.pointerId);
    if (touch) {
      touch.id = null;
    }
  }
};
function countPresses() {
  let count2 = 0;
  for (const touch of touches) {
    if (touch.isTracking) {
      ++count2;
    }
  }
  return count2;
}
var pointerNames2 = /* @__PURE__ */ new Map([
  [0, 3 /* Touch0 */],
  [1, 4 /* Touch1 */],
  [2, 5 /* Touch2 */],
  [3, 6 /* Touch3 */],
  [4, 7 /* Touch4 */],
  [5, 8 /* Touch5 */],
  [6, 9 /* Touch6 */],
  [7, 10 /* Touch7 */],
  [8, 11 /* Touch8 */],
  [9, 12 /* Touch9 */],
  [10, 13 /* Touch10 */]
]);
var PointerSingleTouch = class extends BaseScreenPointer {
  constructor(evtSys, renderer, idx, camera, parent) {
    if (touches.length === 0) {
      renderer.domElement.addEventListener("pointerdown", onPrePointerDown);
    }
    super("touch", pointerNames2.get(idx), evtSys, renderer, camera, null);
    this.parent = null;
    if (touches.length === 0) {
      renderer.domElement.addEventListener("pointerup", onPostPointerUp);
    }
    this.parent = parent;
    this.canMoveView = true;
    Object.seal(this);
    touches.push(this);
  }
  setEventState(type2) {
    super.setEventState(type2);
    if (this.parent) {
      if (type2 === "down") {
        this.parent.onDown();
      } else if (type2 === "up") {
        this.parent.onUp();
      } else if (type2 === "move") {
        this.parent.onMove();
      }
    }
  }
  readEvent(evt) {
    if (this.checkEvent(evt)) {
      const lastButtons = this.state.buttons;
      super.readEvent(evt);
      this.state.buttons = lastButtons;
    }
  }
};

// ../threejs/eventSystem/PointerMultiTouch.ts
function dist2(a, b) {
  const dx = b.x - a.x;
  const dy = b.y - a.y;
  return Math.sqrt(dx * dx + dy * dy);
}
var PointerMultiTouch = class extends BasePointer {
  constructor(evtSys, renderer, camera) {
    super("touch", 14 /* Touches */, evtSys, null);
    this.renderer = renderer;
    this.camera = camera;
    this.touches = new Array(10);
    this.lastPinchDist = 0;
    this.lastPressCount = 0;
    for (let i = 0; i < this.touches.length; ++i) {
      this.touches[i] = new PointerSingleTouch(evtSys, renderer, i, camera, this);
    }
    this.canMoveView = true;
    Object.seal(this);
  }
  onDown() {
    this.updateState();
    this.onPointerDown();
  }
  onUp() {
    this.updateState();
    this.onPointerUp();
  }
  onMove() {
    this.updateState();
    this.onPointerMove();
    const pressCount = countPresses();
    if (pressCount === 2) {
      let a = null;
      let b = null;
      for (const touch of this.touches) {
        if (touch.isTracking) {
          if (!a) {
            a = touch;
          } else if (!b) {
            b = touch;
            break;
          }
        }
      }
      const pinchDist = dist2(a.state, b.state);
      const dz = (pinchDist - this.lastPinchDist) * 2.5;
      if (this.lastPressCount === 2) {
        this.onZoom(dz);
      }
      this.lastPinchDist = pinchDist;
    }
    this.lastPressCount = pressCount;
  }
  vibrate() {
    navigator.vibrate(125);
  }
  updateState() {
    this.lastStateUpdate(() => {
      this.state.buttons = 0;
      this.state.alt = false;
      this.state.ctrl = false;
      this.state.shift = false;
      this.state.meta = false;
      this.state.canClick = false;
      this.state.dragging = false;
      this.state.dragDistance = 0;
      this.state.moveDistance = 0;
      this.state.x = 0;
      this.state.y = 0;
      this.state.dx = 0;
      this.state.dy = 0;
      this.state.u = 0;
      this.state.v = 0;
      this.state.du = 0;
      this.state.dv = 0;
      const K = 1 / countPresses();
      for (const touch of this.touches) {
        if (touch.isTracking) {
          this.state.buttons = Math.max(this.state.buttons, touch.state.buttons);
          this.state.alt = this.state.alt || touch.state.alt;
          this.state.ctrl = this.state.ctrl || touch.state.ctrl;
          this.state.shift = this.state.shift || touch.state.shift;
          this.state.meta = this.state.meta || touch.state.meta;
          this.state.canClick = this.state.canClick || touch.state.canClick;
          this.state.dragging = this.state.dragging || touch.state.dragging;
          this.state.dragDistance += K * touch.state.dragDistance;
          this.state.moveDistance += K * touch.state.moveDistance;
          this.state.x += K * touch.state.x;
          this.state.y += K * touch.state.y;
          this.state.dx += K * touch.state.dx;
          this.state.dy += K * touch.state.dy;
          this.state.u += K * touch.state.u;
          this.state.v += K * touch.state.v;
          this.state.du += K * touch.state.du;
          this.state.dv += K * touch.state.dv;
        }
      }
    });
  }
  get canMoveView() {
    return super.canMoveView;
  }
  set canMoveView(v) {
    if (this.touches) {
      for (const touch of this.touches) {
        touch.canMoveView = v;
      }
      super.canMoveView = v;
    }
  }
  get enabled() {
    return super.enabled;
  }
  set enabled(v) {
    if (this.touches) {
      for (const touch of this.touches) {
        touch.enabled = v;
      }
    }
    super.enabled = v;
  }
  update() {
    const cam = resolveCamera(this.renderer, this.camera);
    this.origin.setFromMatrixPosition(cam.matrixWorld);
    this.direction.set(this.state.u, -this.state.v, 0.5).unproject(cam).sub(this.origin).normalize();
  }
  isPressed(button) {
    for (const touch of this.touches) {
      if (touch.isPressed(button)) {
        return true;
      }
    }
    return false;
  }
};

// ../threejs/eventSystem/PointerPen.ts
var PointerPen = class extends BaseScreenPointer {
  constructor(evtSys, renderer, camera) {
    const onPrep = (evt) => {
      if (evt.pointerType === "pen" && this.id == null) {
        this.id = evt.pointerId;
      }
    };
    const unPrep = (evt) => {
      if (evt.pointerType === "pen" && this.id != null) {
        this.id = null;
      }
    };
    const element = renderer.domElement;
    element.addEventListener("pointerdown", onPrep);
    element.addEventListener("pointermove", onPrep);
    super("pen", 2 /* Pen */, evtSys, renderer, camera, new CursorXRMouse(renderer));
    element.addEventListener("pointerup", unPrep);
    element.addEventListener("pointercancel", unPrep);
    this.canMoveView = true;
    Object.seal(this);
  }
};

// ../threejs/eventSystem/EventSystem.ts
function correctHit(hit, pointer) {
  if (isDefined(hit)) {
    hit.point.copy(pointer.direction).multiplyScalar(hit.distance).add(pointer.origin);
  }
}
var EventSystem = class extends TypedEventBase {
  constructor(env) {
    super();
    this.env = env;
    this.raycaster = new THREE.Raycaster();
    this.hands = new Array();
    this.localPointerMovedEvt = new ObjectMovedEvent();
    this.hits = new Array();
    this.infoPressed = false;
    this.menuPressed = false;
    this.pointerEvents = /* @__PURE__ */ new Map();
    this.raycaster.camera = this.env.camera;
    this.raycaster.layers.set(FOREGROUND);
    this.mouse = new PointerMouse(this, this.env.renderer, this.env.camera);
    this.pen = new PointerPen(this, this.env.renderer, this.env.camera);
    this.touches = new PointerMultiTouch(this, this.env.renderer, this.env.camera);
    for (let i = 0; i < 2; ++i) {
      this.hands[i] = new PointerHand(this, this.env.renderer, i);
    }
    this.pointers = [
      this.mouse,
      this.pen,
      this.touches,
      ...this.hands
    ];
    for (const pointer of this.pointers) {
      if (pointer.cursor) {
        objGraph(this.env.stage, pointer.cursor);
      }
    }
    window.addEventListener("keydown", (evt) => {
      const ok = isModifierless(evt);
      if (evt.key === "/")
        this.infoPressed = ok;
      if (evt.key === "ContextMenu")
        this.menuPressed = ok;
    });
    window.addEventListener("keyup", (evt) => {
      if (evt.key === "/")
        this.infoPressed = false;
      if (evt.key === "ContextMenu")
        this.menuPressed = false;
    });
    this.env.avatar.evtSys = this;
    this.checkXRMouse();
  }
  onFlick(direction) {
    this.env.avatar.onFlick(direction);
  }
  onConnected(_hand) {
    this.checkXRMouse();
  }
  onDisconnected(_hand) {
    this.checkXRMouse();
  }
  checkXRMouse() {
    let count2 = 0;
    for (const hand of this.hands.values()) {
      if (hand.enabled) {
        ++count2;
      }
    }
    const enableScreenPointers = count2 === 0;
    this.mouse.enabled = enableScreenPointers;
    this.pen.enabled = enableScreenPointers;
    this.touches.enabled = enableScreenPointers;
  }
  checkPointer(pointer, eventType) {
    pointer.isActive = true;
    this.fireRay(pointer);
    const { curHit, hoveredHit, pressedHit, draggedHit } = pointer;
    const curObj = resolveObj(curHit);
    const hoveredObj = resolveObj(hoveredHit);
    const pressedObj = resolveObj(pressedHit);
    const draggedObj = resolveObj(draggedHit);
    if (eventType === "move" || eventType === "drag") {
      correctHit(hoveredHit, pointer);
      correctHit(pressedHit, pointer);
      correctHit(draggedHit, pointer);
    }
    switch (eventType) {
      case "move":
        {
          const moveEvt = this.getEvent(pointer, "move", curHit);
          this.env.avatar.onMove(moveEvt);
          this.env.cameraControl.onMove(moveEvt);
          if (isDefined(draggedHit)) {
            draggedObj.dispatchEvent(moveEvt.to3(draggedHit));
          } else if (isDefined(pressedHit)) {
            pressedObj.dispatchEvent(moveEvt.to3(pressedHit));
          } else if (pointer.state.buttons === 0) {
            this.checkExit(curHit, hoveredHit, pointer);
            this.checkEnter(curHit, hoveredHit, pointer);
            if (curObj) {
              curObj.dispatchEvent(moveEvt.to3(curHit));
            }
          }
          this.localPointerMovedEvt.name = pointer.name;
          this.localPointerMovedEvt.set(pointer.origin.x, pointer.origin.y, pointer.origin.z, pointer.direction.x, pointer.direction.y, pointer.direction.z, 0, 1, 0);
          this.dispatchEvent(this.localPointerMovedEvt);
        }
        break;
      case "down":
        {
          const downEvt = this.getEvent(pointer, "down", curHit);
          this.env.avatar.onDown(downEvt);
          if (isClickable(hoveredHit) || isDraggable(hoveredHit)) {
            pointer.pressedHit = hoveredHit;
            hoveredObj.dispatchEvent(downEvt.to3(hoveredHit));
          }
        }
        break;
      case "up":
        {
          const upEvt = this.getEvent(pointer, "up", curHit);
          this.dispatchEvent(upEvt);
          if (pointer.state.buttons === 0) {
            if (isDefined(pressedHit)) {
              pointer.pressedHit = null;
              pressedObj.dispatchEvent(upEvt.to3(pressedHit));
            }
            this.checkExit(curHit, hoveredHit, pointer);
            this.checkEnter(curHit, hoveredHit, pointer);
          }
        }
        break;
      case "click":
        {
          const clickEvt = this.getEvent(pointer, "click", curHit);
          this.dispatchEvent(clickEvt);
          if (isClickable(curHit)) {
            pointer.vibrate();
            curObj.dispatchEvent(clickEvt.to3(curHit));
          }
        }
        break;
      case "dragstart":
        {
          const dragStartEvt = this.getEvent(pointer, "dragstart", pressedHit, curHit);
          this.dispatchEvent(dragStartEvt);
          if (isDefined(pressedHit)) {
            pointer.draggedHit = pressedHit;
            pressedObj.dispatchEvent(dragStartEvt.to3(pressedHit));
          }
        }
        break;
      case "drag":
        {
          const dragEvt = this.getEvent(pointer, "drag", draggedHit, curHit);
          this.dispatchEvent(dragEvt);
          if (isDefined(draggedHit)) {
            draggedObj.dispatchEvent(dragEvt.to3(draggedHit));
          }
        }
        break;
      case "dragcancel":
        {
          const dragCancelEvt = this.getEvent(pointer, "dragcancel", draggedHit, curHit);
          this.dispatchEvent(dragCancelEvt);
          if (isDefined(draggedHit)) {
            pointer.draggedHit = null;
            draggedObj.dispatchEvent(dragCancelEvt.to3(draggedHit));
          }
        }
        break;
      case "dragend":
        {
          const dragEndEvt = this.getEvent(pointer, "dragend", draggedHit, curHit);
          this.dispatchEvent(dragEndEvt);
          if (isDefined(draggedHit)) {
            pointer.draggedHit = null;
            draggedObj.dispatchEvent(dragEndEvt.to3(draggedHit));
          }
        }
        break;
      default:
        assertNever(eventType);
    }
    pointer.updateCursor(this.env.avatar.worldPos, draggedHit || pressedHit || hoveredHit || curHit, 2);
  }
  getEvent(pointer, type2, ...hits) {
    if (!this.pointerEvents.has(pointer)) {
      const evts = /* @__PURE__ */ new Map([
        ["move", new EventSystemEvent("move", pointer)],
        ["enter", new EventSystemEvent("enter", pointer)],
        ["exit", new EventSystemEvent("exit", pointer)],
        ["up", new EventSystemEvent("up", pointer)],
        ["down", new EventSystemEvent("down", pointer)],
        ["click", new EventSystemEvent("click", pointer)],
        ["dragstart", new EventSystemEvent("dragstart", pointer)],
        ["drag", new EventSystemEvent("drag", pointer)],
        ["dragend", new EventSystemEvent("dragend", pointer)]
      ]);
      this.pointerEvents.set(pointer, evts);
    }
    const pointerEvents2 = this.pointerEvents.get(pointer);
    const evt = pointerEvents2.get(type2);
    if (hits.length > 0) {
      evt.hit = arrayScan(hits, isDefined);
      const lastHit = arrayScan(hits, (h) => isDefined(h) && h !== evt.hit);
      if (isDefined(lastHit)) {
        evt.hit.uv = lastHit.uv;
      }
    }
    return evt;
  }
  checkExit(curHit, hoveredHit, pointer) {
    const curObj = resolveObj(curHit);
    const hoveredObj = resolveObj(hoveredHit);
    if (curObj !== hoveredObj && isDefined(hoveredObj)) {
      pointer.hoveredHit = null;
      const exitEvt = this.getEvent(pointer, "exit", hoveredHit);
      this.dispatchEvent(exitEvt);
      hoveredObj.dispatchEvent(exitEvt.to3(hoveredHit));
    }
  }
  checkEnter(curHit, hoveredHit, pointer) {
    const curObj = resolveObj(curHit);
    const hoveredObj = resolveObj(hoveredHit);
    if (curObj !== hoveredObj && isDefined(curHit)) {
      pointer.hoveredHit = curHit;
      const enterEvt = this.getEvent(pointer, "enter", curHit);
      this.dispatchEvent(enterEvt);
      curObj.dispatchEvent(enterEvt.to3(curHit));
    }
  }
  refreshCursors() {
    for (const pointer of this.pointers) {
      if (pointer.cursor) {
        pointer.cursor = this.env.cursor3D.clone();
      }
    }
  }
  fireRay(pointer) {
    arrayClear(this.hits);
    this.raycaster.ray.origin.copy(pointer.origin);
    this.raycaster.ray.direction.copy(pointer.direction);
    this.raycaster.intersectObject(this.env.scene, true, this.hits);
    pointer.curHit = null;
    let minDist = Number.MAX_VALUE;
    for (const hit of this.hits) {
      if (isInteractiveHit(hit) && isObjVisible(hit) && hit.distance < minDist) {
        pointer.curHit = hit;
        minDist = hit.distance;
      }
    }
    if (pointer.curHit && pointer.hoveredHit && pointer.curHit.object === pointer.hoveredHit.object) {
      pointer.hoveredHit = pointer.curHit;
    }
  }
  update() {
    for (const pointer of this.pointers) {
      if (pointer.needsUpdate) {
        pointer.update();
      }
    }
  }
  recheckPointers() {
    for (const pointer of this.pointers) {
      if (pointer.needsUpdate) {
        pointer.recheck();
      }
    }
  }
  isPressed(button) {
    for (const pointer of this.pointers) {
      if (pointer.isPressed(button)) {
        return true;
      }
    }
    return button === 3 /* Menu */ && this.menuPressed || button === 4 /* Info */ && this.infoPressed;
  }
};

// ../threejs/Fader.ts
var completeEvt = new TypedEvent("fadecomplete");
var Fader = class extends TypedEventBase {
  constructor(name2, t2 = 0.15) {
    super();
    this.opacity = 1;
    this.direction = 0;
    this.material = solidTransparent({
      name: "FaderMaterial",
      color: 0,
      side: THREE.BackSide
    });
    this.object = new THREE.Mesh(cube, this.material);
    this.object.name = name2;
    this.object.renderOrder = Number.MAX_VALUE;
    this.speed = 1 / t2;
    this.object.layers.enableAll();
  }
  async fadeOut() {
    if (this.direction != 1) {
      this.direction = 1;
      await once(this, "fadecomplete");
    }
  }
  async fadeIn() {
    if (this.direction != -1) {
      this.direction = -1;
      await once(this, "fadecomplete");
    }
  }
  update(dt) {
    if (this.direction !== 0) {
      const dOpacity = this.direction * this.speed * dt / 1e3;
      if (0 <= this.opacity && this.opacity <= 1) {
        this.opacity += dOpacity;
      }
      if (this.direction === 1 && this.opacity >= 1 || this.direction === -1 && this.opacity <= 0) {
        this.opacity = clamp(this.opacity, 0, 1);
        this.direction = 0;
        this.dispatchEvent(completeEvt);
      }
    }
    this.material.opacity = this.opacity;
    this.material.transparent = this.opacity < 1;
    this.material.needsUpdate = true;
  }
};

// ../threejs/LoadingBar.ts
function chrome(x, y, z, w, h, d) {
  const chromeMesh = new Cube(w, h, d, litWhite);
  chromeMesh.position.set(x, y, z);
  return chromeMesh;
}
var velocity = 0.1;
var LoadingBar = class extends BaseProgress {
  constructor() {
    super();
    this.value = 0;
    this.targetValue = 0;
    this.object = new THREE.Object3D();
    this._enabled = true;
    this.valueBar = new Cube(0, 1, 1, litGrey);
    this.valueBar.scale.set(0, 1, 1);
    const valueBarContainer = new THREE.Object3D();
    valueBarContainer.scale.set(1, 0.1, 0.1);
    objGraph(this, objGraph(valueBarContainer, this.valueBar), chrome(-0.5, 0, -0.05, 0.01, 0.1, 0.01), chrome(-0.5, 0, 0.05, 0.01, 0.1, 0.01), chrome(0.5, 0, -0.05, 0.01, 0.1, 0.01), chrome(0.5, 0, 0.05, 0.01, 0.1, 0.01), chrome(-0.5, -0.05, 0, 0.01, 0.01, 0.1), chrome(0.5, -0.05, 0, 0.01, 0.01, 0.1), chrome(-0.5, 0.05, 0, 0.01, 0.01, 0.1), chrome(0.5, 0.05, 0, 0.01, 0.01, 0.1), chrome(0, -0.05, -0.05, 1, 0.01, 0.01), chrome(0, 0.05, -0.05, 1, 0.01, 0.01), chrome(0, -0.05, 0.05, 1, 0.01, 0.01), chrome(0, 0.05, 0.05, 1, 0.01, 0.01));
    deepSetLayer(this, PURGATORY);
  }
  get enabled() {
    return this._enabled;
  }
  set enabled(v) {
    if (v !== this._enabled) {
      this._enabled = v;
      objectSetVisible(this, objectIsVisible(this) || this.enabled);
    }
  }
  report(soFar, total, msg) {
    super.report(soFar, total, msg);
    this.targetValue = this.p;
  }
  update(dt) {
    if (this.object.parent.visible) {
      this.value = Math.min(this.targetValue, this.value + velocity * dt);
      this.valueBar.scale.set(this.value, 1, 1);
      this.valueBar.position.x = this.value / 2 - 0.5;
      objectSetVisible(this, this.enabled && this.value > 0);
    }
  }
};

// ../dom/fullscreen.ts
if (!hasFullscreenAPI()) {
  const Elm = Element.prototype;
  const Doc = Document.prototype;
  if ("webkitRequestFullscreen" in Elm) {
    Elm.requestFullscreen = Elm.webkitRequestFullscreen;
    Doc.exitFullscreen = Doc.webkitRequestFullscreen;
    Object.defineProperties(Doc, {
      "fullscreenEnabled": {
        get: function() {
          return this.webkitFullscreenEnabled;
        }
      },
      "fullscreenElement": {
        get: function() {
          return this.webkitFullscreenElement;
        }
      }
    });
  } else if ("mozRequestFullScreen" in Elm) {
    Elm.requestFullscreen = Elm.mozRequestFullScreen;
    Doc.exitFullscreen = Doc.mozCancelFullScreen;
    Object.defineProperties(Doc, {
      "fullscreenEnabled": {
        get: function() {
          return this.mozFullScreenEnabled;
        }
      },
      "fullscreenElement": {
        get: function() {
          return this.mozFullScreenElement;
        }
      }
    });
  } else if ("msRequestFullscreen" in Elm) {
    Elm.requestFullscreen = Elm.msRequestFullscreen;
    Doc.exitFullscreen = Doc.msExitFullscreen;
    Object.defineProperties(Doc, {
      "fullscreenEnabled": {
        get: function() {
          return this.msFullscreenEnabled;
        }
      },
      "fullscreenElement": {
        get: function() {
          return this.msFullscreenElement;
        }
      }
    });
  }
}
function hasFullscreenAPI() {
  return "requestFullscreen" in HTMLElement.prototype;
}

// ../../node_modules/webxr-polyfill/src/lib/global.js
var _global = typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {};
var global_default = _global;

// ../../node_modules/webxr-polyfill/src/lib/EventTarget.js
var PRIVATE = Symbol("@@webxr-polyfill/EventTarget");
var EventTarget = class {
  constructor() {
    this[PRIVATE] = {
      listeners: /* @__PURE__ */ new Map()
    };
  }
  addEventListener(type2, listener) {
    if (typeof type2 !== "string") {
      throw new Error("`type` must be a string");
    }
    if (typeof listener !== "function") {
      throw new Error("`listener` must be a function");
    }
    const typedListeners = this[PRIVATE].listeners.get(type2) || [];
    typedListeners.push(listener);
    this[PRIVATE].listeners.set(type2, typedListeners);
  }
  removeEventListener(type2, listener) {
    if (typeof type2 !== "string") {
      throw new Error("`type` must be a string");
    }
    if (typeof listener !== "function") {
      throw new Error("`listener` must be a function");
    }
    const typedListeners = this[PRIVATE].listeners.get(type2) || [];
    for (let i = typedListeners.length; i >= 0; i--) {
      if (typedListeners[i] === listener) {
        typedListeners.pop();
      }
    }
  }
  dispatchEvent(type2, event) {
    const typedListeners = this[PRIVATE].listeners.get(type2) || [];
    const queue = [];
    for (let i = 0; i < typedListeners.length; i++) {
      queue[i] = typedListeners[i];
    }
    for (let listener of queue) {
      listener(event);
    }
    if (typeof this[`on${type2}`] === "function") {
      this[`on${type2}`](event);
    }
  }
};

// ../../node_modules/webxr-polyfill/node_modules/gl-matrix/src/gl-matrix/common.js
var EPSILON2 = 1e-6;
var ARRAY_TYPE2 = typeof Float32Array !== "undefined" ? Float32Array : Array;
var degree2 = Math.PI / 180;

// ../../node_modules/webxr-polyfill/node_modules/gl-matrix/src/gl-matrix/mat4.js
function create3() {
  let out = new ARRAY_TYPE2(16);
  if (ARRAY_TYPE2 != Float32Array) {
    out[1] = 0;
    out[2] = 0;
    out[3] = 0;
    out[4] = 0;
    out[6] = 0;
    out[7] = 0;
    out[8] = 0;
    out[9] = 0;
    out[11] = 0;
    out[12] = 0;
    out[13] = 0;
    out[14] = 0;
  }
  out[0] = 1;
  out[5] = 1;
  out[10] = 1;
  out[15] = 1;
  return out;
}
function copy2(out, a) {
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  out[3] = a[3];
  out[4] = a[4];
  out[5] = a[5];
  out[6] = a[6];
  out[7] = a[7];
  out[8] = a[8];
  out[9] = a[9];
  out[10] = a[10];
  out[11] = a[11];
  out[12] = a[12];
  out[13] = a[13];
  out[14] = a[14];
  out[15] = a[15];
  return out;
}
function identity2(out) {
  out[0] = 1;
  out[1] = 0;
  out[2] = 0;
  out[3] = 0;
  out[4] = 0;
  out[5] = 1;
  out[6] = 0;
  out[7] = 0;
  out[8] = 0;
  out[9] = 0;
  out[10] = 1;
  out[11] = 0;
  out[12] = 0;
  out[13] = 0;
  out[14] = 0;
  out[15] = 1;
  return out;
}
function invert(out, a) {
  let a00 = a[0], a01 = a[1], a02 = a[2], a03 = a[3];
  let a10 = a[4], a11 = a[5], a12 = a[6], a13 = a[7];
  let a20 = a[8], a21 = a[9], a22 = a[10], a23 = a[11];
  let a30 = a[12], a31 = a[13], a32 = a[14], a33 = a[15];
  let b00 = a00 * a11 - a01 * a10;
  let b01 = a00 * a12 - a02 * a10;
  let b02 = a00 * a13 - a03 * a10;
  let b03 = a01 * a12 - a02 * a11;
  let b04 = a01 * a13 - a03 * a11;
  let b05 = a02 * a13 - a03 * a12;
  let b06 = a20 * a31 - a21 * a30;
  let b07 = a20 * a32 - a22 * a30;
  let b08 = a20 * a33 - a23 * a30;
  let b09 = a21 * a32 - a22 * a31;
  let b10 = a21 * a33 - a23 * a31;
  let b11 = a22 * a33 - a23 * a32;
  let det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
  if (!det) {
    return null;
  }
  det = 1 / det;
  out[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
  out[1] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
  out[2] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
  out[3] = (a22 * b04 - a21 * b05 - a23 * b03) * det;
  out[4] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
  out[5] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
  out[6] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
  out[7] = (a20 * b05 - a22 * b02 + a23 * b01) * det;
  out[8] = (a10 * b10 - a11 * b08 + a13 * b06) * det;
  out[9] = (a01 * b08 - a00 * b10 - a03 * b06) * det;
  out[10] = (a30 * b04 - a31 * b02 + a33 * b00) * det;
  out[11] = (a21 * b02 - a20 * b04 - a23 * b00) * det;
  out[12] = (a11 * b07 - a10 * b09 - a12 * b06) * det;
  out[13] = (a00 * b09 - a01 * b07 + a02 * b06) * det;
  out[14] = (a31 * b01 - a30 * b03 - a32 * b00) * det;
  out[15] = (a20 * b03 - a21 * b01 + a22 * b00) * det;
  return out;
}
function multiply2(out, a, b) {
  let a00 = a[0], a01 = a[1], a02 = a[2], a03 = a[3];
  let a10 = a[4], a11 = a[5], a12 = a[6], a13 = a[7];
  let a20 = a[8], a21 = a[9], a22 = a[10], a23 = a[11];
  let a30 = a[12], a31 = a[13], a32 = a[14], a33 = a[15];
  let b0 = b[0], b1 = b[1], b2 = b[2], b3 = b[3];
  out[0] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
  out[1] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
  out[2] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
  out[3] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
  b0 = b[4];
  b1 = b[5];
  b2 = b[6];
  b3 = b[7];
  out[4] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
  out[5] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
  out[6] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
  out[7] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
  b0 = b[8];
  b1 = b[9];
  b2 = b[10];
  b3 = b[11];
  out[8] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
  out[9] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
  out[10] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
  out[11] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
  b0 = b[12];
  b1 = b[13];
  b2 = b[14];
  b3 = b[15];
  out[12] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
  out[13] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
  out[14] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
  out[15] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
  return out;
}
function fromRotationTranslation(out, q, v) {
  let x = q[0], y = q[1], z = q[2], w = q[3];
  let x2 = x + x;
  let y2 = y + y;
  let z2 = z + z;
  let xx = x * x2;
  let xy = x * y2;
  let xz = x * z2;
  let yy = y * y2;
  let yz = y * z2;
  let zz = z * z2;
  let wx = w * x2;
  let wy = w * y2;
  let wz = w * z2;
  out[0] = 1 - (yy + zz);
  out[1] = xy + wz;
  out[2] = xz - wy;
  out[3] = 0;
  out[4] = xy - wz;
  out[5] = 1 - (xx + zz);
  out[6] = yz + wx;
  out[7] = 0;
  out[8] = xz + wy;
  out[9] = yz - wx;
  out[10] = 1 - (xx + yy);
  out[11] = 0;
  out[12] = v[0];
  out[13] = v[1];
  out[14] = v[2];
  out[15] = 1;
  return out;
}
function getTranslation(out, mat) {
  out[0] = mat[12];
  out[1] = mat[13];
  out[2] = mat[14];
  return out;
}
function getRotation(out, mat) {
  let trace = mat[0] + mat[5] + mat[10];
  let S2 = 0;
  if (trace > 0) {
    S2 = Math.sqrt(trace + 1) * 2;
    out[3] = 0.25 * S2;
    out[0] = (mat[6] - mat[9]) / S2;
    out[1] = (mat[8] - mat[2]) / S2;
    out[2] = (mat[1] - mat[4]) / S2;
  } else if (mat[0] > mat[5] && mat[0] > mat[10]) {
    S2 = Math.sqrt(1 + mat[0] - mat[5] - mat[10]) * 2;
    out[3] = (mat[6] - mat[9]) / S2;
    out[0] = 0.25 * S2;
    out[1] = (mat[1] + mat[4]) / S2;
    out[2] = (mat[8] + mat[2]) / S2;
  } else if (mat[5] > mat[10]) {
    S2 = Math.sqrt(1 + mat[5] - mat[0] - mat[10]) * 2;
    out[3] = (mat[8] - mat[2]) / S2;
    out[0] = (mat[1] + mat[4]) / S2;
    out[1] = 0.25 * S2;
    out[2] = (mat[6] + mat[9]) / S2;
  } else {
    S2 = Math.sqrt(1 + mat[10] - mat[0] - mat[5]) * 2;
    out[3] = (mat[1] - mat[4]) / S2;
    out[0] = (mat[8] + mat[2]) / S2;
    out[1] = (mat[6] + mat[9]) / S2;
    out[2] = 0.25 * S2;
  }
  return out;
}
function perspective(out, fovy, aspect, near, far) {
  let f = 1 / Math.tan(fovy / 2), nf;
  out[0] = f / aspect;
  out[1] = 0;
  out[2] = 0;
  out[3] = 0;
  out[4] = 0;
  out[5] = f;
  out[6] = 0;
  out[7] = 0;
  out[8] = 0;
  out[9] = 0;
  out[11] = -1;
  out[12] = 0;
  out[13] = 0;
  out[15] = 0;
  if (far != null && far !== Infinity) {
    nf = 1 / (near - far);
    out[10] = (far + near) * nf;
    out[14] = 2 * far * near * nf;
  } else {
    out[10] = -1;
    out[14] = -2 * near;
  }
  return out;
}

// ../../node_modules/webxr-polyfill/node_modules/gl-matrix/src/gl-matrix/vec3.js
function create4() {
  let out = new ARRAY_TYPE2(3);
  if (ARRAY_TYPE2 != Float32Array) {
    out[0] = 0;
    out[1] = 0;
    out[2] = 0;
  }
  return out;
}
function clone2(a) {
  var out = new ARRAY_TYPE2(3);
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  return out;
}
function length2(a) {
  let x = a[0];
  let y = a[1];
  let z = a[2];
  return Math.sqrt(x * x + y * y + z * z);
}
function fromValues2(x, y, z) {
  let out = new ARRAY_TYPE2(3);
  out[0] = x;
  out[1] = y;
  out[2] = z;
  return out;
}
function copy3(out, a) {
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  return out;
}
function add2(out, a, b) {
  out[0] = a[0] + b[0];
  out[1] = a[1] + b[1];
  out[2] = a[2] + b[2];
  return out;
}
function scale2(out, a, b) {
  out[0] = a[0] * b;
  out[1] = a[1] * b;
  out[2] = a[2] * b;
  return out;
}
function normalize2(out, a) {
  let x = a[0];
  let y = a[1];
  let z = a[2];
  let len3 = x * x + y * y + z * z;
  if (len3 > 0) {
    len3 = 1 / Math.sqrt(len3);
    out[0] = a[0] * len3;
    out[1] = a[1] * len3;
    out[2] = a[2] * len3;
  }
  return out;
}
function dot2(a, b) {
  return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
}
function cross2(out, a, b) {
  let ax = a[0], ay = a[1], az = a[2];
  let bx = b[0], by = b[1], bz = b[2];
  out[0] = ay * bz - az * by;
  out[1] = az * bx - ax * bz;
  out[2] = ax * by - ay * bx;
  return out;
}
function transformQuat2(out, a, q) {
  let qx = q[0], qy = q[1], qz = q[2], qw = q[3];
  let x = a[0], y = a[1], z = a[2];
  let uvx = qy * z - qz * y, uvy = qz * x - qx * z, uvz = qx * y - qy * x;
  let uuvx = qy * uvz - qz * uvy, uuvy = qz * uvx - qx * uvz, uuvz = qx * uvy - qy * uvx;
  let w2 = qw * 2;
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
function angle2(a, b) {
  let tempA = fromValues2(a[0], a[1], a[2]);
  let tempB = fromValues2(b[0], b[1], b[2]);
  normalize2(tempA, tempA);
  normalize2(tempB, tempB);
  let cosine = dot2(tempA, tempB);
  if (cosine > 1) {
    return 0;
  } else if (cosine < -1) {
    return Math.PI;
  } else {
    return Math.acos(cosine);
  }
}
var len2 = length2;
var forEach2 = function() {
  let vec = create4();
  return function(a, stride, offset, count2, fn, arg) {
    let i, l;
    if (!stride) {
      stride = 3;
    }
    if (!offset) {
      offset = 0;
    }
    if (count2) {
      l = Math.min(count2 * stride + offset, a.length);
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

// ../../node_modules/webxr-polyfill/node_modules/gl-matrix/src/gl-matrix/mat3.js
function create5() {
  let out = new ARRAY_TYPE2(9);
  if (ARRAY_TYPE2 != Float32Array) {
    out[1] = 0;
    out[2] = 0;
    out[3] = 0;
    out[5] = 0;
    out[6] = 0;
    out[7] = 0;
  }
  out[0] = 1;
  out[4] = 1;
  out[8] = 1;
  return out;
}

// ../../node_modules/webxr-polyfill/node_modules/gl-matrix/src/gl-matrix/vec4.js
function create6() {
  let out = new ARRAY_TYPE2(4);
  if (ARRAY_TYPE2 != Float32Array) {
    out[0] = 0;
    out[1] = 0;
    out[2] = 0;
    out[3] = 0;
  }
  return out;
}
function clone3(a) {
  let out = new ARRAY_TYPE2(4);
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  out[3] = a[3];
  return out;
}
function fromValues3(x, y, z, w) {
  let out = new ARRAY_TYPE2(4);
  out[0] = x;
  out[1] = y;
  out[2] = z;
  out[3] = w;
  return out;
}
function copy4(out, a) {
  out[0] = a[0];
  out[1] = a[1];
  out[2] = a[2];
  out[3] = a[3];
  return out;
}
function normalize3(out, a) {
  let x = a[0];
  let y = a[1];
  let z = a[2];
  let w = a[3];
  let len3 = x * x + y * y + z * z + w * w;
  if (len3 > 0) {
    len3 = 1 / Math.sqrt(len3);
    out[0] = x * len3;
    out[1] = y * len3;
    out[2] = z * len3;
    out[3] = w * len3;
  }
  return out;
}
var forEach3 = function() {
  let vec = create6();
  return function(a, stride, offset, count2, fn, arg) {
    let i, l;
    if (!stride) {
      stride = 4;
    }
    if (!offset) {
      offset = 0;
    }
    if (count2) {
      l = Math.min(count2 * stride + offset, a.length);
    } else {
      l = a.length;
    }
    for (i = offset; i < l; i += stride) {
      vec[0] = a[i];
      vec[1] = a[i + 1];
      vec[2] = a[i + 2];
      vec[3] = a[i + 3];
      fn(vec, vec, arg);
      a[i] = vec[0];
      a[i + 1] = vec[1];
      a[i + 2] = vec[2];
      a[i + 3] = vec[3];
    }
    return a;
  };
}();

// ../../node_modules/webxr-polyfill/node_modules/gl-matrix/src/gl-matrix/quat.js
function create7() {
  let out = new ARRAY_TYPE2(4);
  if (ARRAY_TYPE2 != Float32Array) {
    out[0] = 0;
    out[1] = 0;
    out[2] = 0;
  }
  out[3] = 1;
  return out;
}
function setAxisAngle(out, axis, rad) {
  rad = rad * 0.5;
  let s = Math.sin(rad);
  out[0] = s * axis[0];
  out[1] = s * axis[1];
  out[2] = s * axis[2];
  out[3] = Math.cos(rad);
  return out;
}
function multiply3(out, a, b) {
  let ax = a[0], ay = a[1], az = a[2], aw = a[3];
  let bx = b[0], by = b[1], bz = b[2], bw = b[3];
  out[0] = ax * bw + aw * bx + ay * bz - az * by;
  out[1] = ay * bw + aw * by + az * bx - ax * bz;
  out[2] = az * bw + aw * bz + ax * by - ay * bx;
  out[3] = aw * bw - ax * bx - ay * by - az * bz;
  return out;
}
function slerp(out, a, b, t2) {
  let ax = a[0], ay = a[1], az = a[2], aw = a[3];
  let bx = b[0], by = b[1], bz = b[2], bw = b[3];
  let omega, cosom, sinom, scale0, scale1;
  cosom = ax * bx + ay * by + az * bz + aw * bw;
  if (cosom < 0) {
    cosom = -cosom;
    bx = -bx;
    by = -by;
    bz = -bz;
    bw = -bw;
  }
  if (1 - cosom > EPSILON2) {
    omega = Math.acos(cosom);
    sinom = Math.sin(omega);
    scale0 = Math.sin((1 - t2) * omega) / sinom;
    scale1 = Math.sin(t2 * omega) / sinom;
  } else {
    scale0 = 1 - t2;
    scale1 = t2;
  }
  out[0] = scale0 * ax + scale1 * bx;
  out[1] = scale0 * ay + scale1 * by;
  out[2] = scale0 * az + scale1 * bz;
  out[3] = scale0 * aw + scale1 * bw;
  return out;
}
function invert2(out, a) {
  let a0 = a[0], a1 = a[1], a2 = a[2], a3 = a[3];
  let dot4 = a0 * a0 + a1 * a1 + a2 * a2 + a3 * a3;
  let invDot = dot4 ? 1 / dot4 : 0;
  out[0] = -a0 * invDot;
  out[1] = -a1 * invDot;
  out[2] = -a2 * invDot;
  out[3] = a3 * invDot;
  return out;
}
function fromMat3(out, m) {
  let fTrace = m[0] + m[4] + m[8];
  let fRoot;
  if (fTrace > 0) {
    fRoot = Math.sqrt(fTrace + 1);
    out[3] = 0.5 * fRoot;
    fRoot = 0.5 / fRoot;
    out[0] = (m[5] - m[7]) * fRoot;
    out[1] = (m[6] - m[2]) * fRoot;
    out[2] = (m[1] - m[3]) * fRoot;
  } else {
    let i = 0;
    if (m[4] > m[0])
      i = 1;
    if (m[8] > m[i * 3 + i])
      i = 2;
    let j = (i + 1) % 3;
    let k = (i + 2) % 3;
    fRoot = Math.sqrt(m[i * 3 + i] - m[j * 3 + j] - m[k * 3 + k] + 1);
    out[i] = 0.5 * fRoot;
    fRoot = 0.5 / fRoot;
    out[3] = (m[j * 3 + k] - m[k * 3 + j]) * fRoot;
    out[j] = (m[j * 3 + i] + m[i * 3 + j]) * fRoot;
    out[k] = (m[k * 3 + i] + m[i * 3 + k]) * fRoot;
  }
  return out;
}
function fromEuler(out, x, y, z) {
  let halfToRad = 0.5 * Math.PI / 180;
  x *= halfToRad;
  y *= halfToRad;
  z *= halfToRad;
  let sx = Math.sin(x);
  let cx = Math.cos(x);
  let sy = Math.sin(y);
  let cy = Math.cos(y);
  let sz = Math.sin(z);
  let cz = Math.cos(z);
  out[0] = sx * cy * cz - cx * sy * sz;
  out[1] = cx * sy * cz + sx * cy * sz;
  out[2] = cx * cy * sz - sx * sy * cz;
  out[3] = cx * cy * cz + sx * sy * sz;
  return out;
}
var clone4 = clone3;
var fromValues4 = fromValues3;
var copy5 = copy4;
var normalize4 = normalize3;
var rotationTo = function() {
  let tmpvec3 = create4();
  let xUnitVec3 = fromValues2(1, 0, 0);
  let yUnitVec3 = fromValues2(0, 1, 0);
  return function(out, a, b) {
    let dot4 = dot2(a, b);
    if (dot4 < -0.999999) {
      cross2(tmpvec3, xUnitVec3, a);
      if (len2(tmpvec3) < 1e-6)
        cross2(tmpvec3, yUnitVec3, a);
      normalize2(tmpvec3, tmpvec3);
      setAxisAngle(out, tmpvec3, Math.PI);
      return out;
    } else if (dot4 > 0.999999) {
      out[0] = 0;
      out[1] = 0;
      out[2] = 0;
      out[3] = 1;
      return out;
    } else {
      cross2(tmpvec3, a, b);
      out[0] = tmpvec3[0];
      out[1] = tmpvec3[1];
      out[2] = tmpvec3[2];
      out[3] = 1 + dot4;
      return normalize4(out, out);
    }
  };
}();
var sqlerp = function() {
  let temp1 = create7();
  let temp2 = create7();
  return function(out, a, b, c, d, t2) {
    slerp(temp1, a, d, t2);
    slerp(temp2, b, c, t2);
    slerp(out, temp1, temp2, 2 * t2 * (1 - t2));
    return out;
  };
}();
var setAxes = function() {
  let matr = create5();
  return function(out, view, right, up) {
    matr[0] = right[0];
    matr[3] = right[1];
    matr[6] = right[2];
    matr[1] = up[0];
    matr[4] = up[1];
    matr[7] = up[2];
    matr[2] = -view[0];
    matr[5] = -view[1];
    matr[8] = -view[2];
    return normalize4(out, fromMat3(out, matr));
  };
}();

// ../../node_modules/webxr-polyfill/src/api/XRRigidTransform.js
var PRIVATE2 = Symbol("@@webxr-polyfill/XRRigidTransform");
var XRRigidTransform2 = class {
  constructor() {
    this[PRIVATE2] = {
      matrix: null,
      position: null,
      orientation: null,
      inverse: null
    };
    if (arguments.length === 0) {
      this[PRIVATE2].matrix = identity2(new Float32Array(16));
    } else if (arguments.length === 1) {
      if (arguments[0] instanceof Float32Array) {
        this[PRIVATE2].matrix = arguments[0];
      } else {
        this[PRIVATE2].position = this._getPoint(arguments[0]);
        this[PRIVATE2].orientation = DOMPointReadOnly.fromPoint({
          x: 0,
          y: 0,
          z: 0,
          w: 1
        });
      }
    } else if (arguments.length === 2) {
      this[PRIVATE2].position = this._getPoint(arguments[0]);
      this[PRIVATE2].orientation = this._getPoint(arguments[1]);
    } else {
      throw new Error("Too many arguments!");
    }
    if (this[PRIVATE2].matrix) {
      let position2 = create4();
      getTranslation(position2, this[PRIVATE2].matrix);
      this[PRIVATE2].position = DOMPointReadOnly.fromPoint({
        x: position2[0],
        y: position2[1],
        z: position2[2]
      });
      let orientation = create7();
      getRotation(orientation, this[PRIVATE2].matrix);
      this[PRIVATE2].orientation = DOMPointReadOnly.fromPoint({
        x: orientation[0],
        y: orientation[1],
        z: orientation[2],
        w: orientation[3]
      });
    } else {
      this[PRIVATE2].matrix = identity2(new Float32Array(16));
      fromRotationTranslation(this[PRIVATE2].matrix, fromValues4(this[PRIVATE2].orientation.x, this[PRIVATE2].orientation.y, this[PRIVATE2].orientation.z, this[PRIVATE2].orientation.w), fromValues2(this[PRIVATE2].position.x, this[PRIVATE2].position.y, this[PRIVATE2].position.z));
    }
  }
  _getPoint(arg) {
    if (arg instanceof DOMPointReadOnly) {
      return arg;
    }
    return DOMPointReadOnly.fromPoint(arg);
  }
  get matrix() {
    return this[PRIVATE2].matrix;
  }
  get position() {
    return this[PRIVATE2].position;
  }
  get orientation() {
    return this[PRIVATE2].orientation;
  }
  get inverse() {
    if (this[PRIVATE2].inverse === null) {
      let invMatrix = identity2(new Float32Array(16));
      invert(invMatrix, this[PRIVATE2].matrix);
      this[PRIVATE2].inverse = new XRRigidTransform2(invMatrix);
      this[PRIVATE2].inverse[PRIVATE2].inverse = this;
    }
    return this[PRIVATE2].inverse;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRSpace.js
var PRIVATE3 = Symbol("@@webxr-polyfill/XRSpace");
var XRSpace = class {
  constructor(specialType = null, inputSource = null) {
    this[PRIVATE3] = {
      specialType,
      inputSource,
      baseMatrix: null,
      inverseBaseMatrix: null,
      lastFrameId: -1
    };
  }
  get _specialType() {
    return this[PRIVATE3].specialType;
  }
  get _inputSource() {
    return this[PRIVATE3].inputSource;
  }
  _ensurePoseUpdated(device, frameId) {
    if (frameId == this[PRIVATE3].lastFrameId)
      return;
    this[PRIVATE3].lastFrameId = frameId;
    this._onPoseUpdate(device);
  }
  _onPoseUpdate(device) {
    if (this[PRIVATE3].specialType == "viewer") {
      this._baseMatrix = device.getBasePoseMatrix();
    }
  }
  set _baseMatrix(matrix) {
    this[PRIVATE3].baseMatrix = matrix;
    this[PRIVATE3].inverseBaseMatrix = null;
  }
  get _baseMatrix() {
    if (!this[PRIVATE3].baseMatrix) {
      if (this[PRIVATE3].inverseBaseMatrix) {
        this[PRIVATE3].baseMatrix = new Float32Array(16);
        invert(this[PRIVATE3].baseMatrix, this[PRIVATE3].inverseBaseMatrix);
      }
    }
    return this[PRIVATE3].baseMatrix;
  }
  set _inverseBaseMatrix(matrix) {
    this[PRIVATE3].inverseBaseMatrix = matrix;
    this[PRIVATE3].baseMatrix = null;
  }
  get _inverseBaseMatrix() {
    if (!this[PRIVATE3].inverseBaseMatrix) {
      if (this[PRIVATE3].baseMatrix) {
        this[PRIVATE3].inverseBaseMatrix = new Float32Array(16);
        invert(this[PRIVATE3].inverseBaseMatrix, this[PRIVATE3].baseMatrix);
      }
    }
    return this[PRIVATE3].inverseBaseMatrix;
  }
  _getSpaceRelativeTransform(space) {
    if (!this._inverseBaseMatrix || !space._baseMatrix) {
      return null;
    }
    let out = new Float32Array(16);
    multiply2(out, this._inverseBaseMatrix, space._baseMatrix);
    return new XRRigidTransform2(out);
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRReferenceSpace.js
var DEFAULT_EMULATION_HEIGHT = 1.6;
var PRIVATE4 = Symbol("@@webxr-polyfill/XRReferenceSpace");
var XRReferenceSpaceTypes = [
  "viewer",
  "local",
  "local-floor",
  "bounded-floor",
  "unbounded"
];
function isFloor(type2) {
  return type2 === "bounded-floor" || type2 === "local-floor";
}
var XRReferenceSpace = class extends XRSpace {
  constructor(type2, transform2 = null) {
    if (!XRReferenceSpaceTypes.includes(type2)) {
      throw new Error(`XRReferenceSpaceType must be one of ${XRReferenceSpaceTypes}`);
    }
    super(type2);
    if (type2 === "bounded-floor" && !transform2) {
      throw new Error(`XRReferenceSpace cannot use 'bounded-floor' type if the platform does not provide the floor level`);
    }
    if (isFloor(type2) && !transform2) {
      transform2 = identity2(new Float32Array(16));
      transform2[13] = DEFAULT_EMULATION_HEIGHT;
    }
    this._inverseBaseMatrix = transform2 || identity2(new Float32Array(16));
    this[PRIVATE4] = {
      type: type2,
      transform: transform2,
      originOffset: identity2(new Float32Array(16))
    };
  }
  _transformBasePoseMatrix(out, pose) {
    multiply2(out, this._inverseBaseMatrix, pose);
  }
  _originOffsetMatrix() {
    return this[PRIVATE4].originOffset;
  }
  _adjustForOriginOffset(transformMatrix) {
    let inverseOriginOffsetMatrix = new Float32Array(16);
    invert(inverseOriginOffsetMatrix, this[PRIVATE4].originOffset);
    multiply2(transformMatrix, inverseOriginOffsetMatrix, transformMatrix);
  }
  _getSpaceRelativeTransform(space) {
    let transform2 = super._getSpaceRelativeTransform(space);
    this._adjustForOriginOffset(transform2.matrix);
    return new XRRigidTransform(transform2.matrix);
  }
  getOffsetReferenceSpace(additionalOffset) {
    let newSpace = new XRReferenceSpace(this[PRIVATE4].type, this[PRIVATE4].transform, this[PRIVATE4].bounds);
    multiply2(newSpace[PRIVATE4].originOffset, this[PRIVATE4].originOffset, additionalOffset.matrix);
    return newSpace;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRSystem.js
var PRIVATE5 = Symbol("@@webxr-polyfill/XR");
var XRSessionModes = ["inline", "immersive-vr", "immersive-ar"];
var DEFAULT_SESSION_OPTIONS = {
  "inline": {
    requiredFeatures: ["viewer"],
    optionalFeatures: []
  },
  "immersive-vr": {
    requiredFeatures: ["viewer", "local"],
    optionalFeatures: []
  },
  "immersive-ar": {
    requiredFeatures: ["viewer", "local"],
    optionalFeatures: []
  }
};
var POLYFILL_REQUEST_SESSION_ERROR = `Polyfill Error: Must call navigator.xr.isSessionSupported() with any XRSessionMode
or navigator.xr.requestSession('inline') prior to requesting an immersive
session. This is a limitation specific to the WebXR Polyfill and does not apply
to native implementations of the API.`;
var XRSystem = class extends EventTarget {
  constructor(devicePromise) {
    super();
    this[PRIVATE5] = {
      device: null,
      devicePromise,
      immersiveSession: null,
      inlineSessions: /* @__PURE__ */ new Set()
    };
    devicePromise.then((device) => {
      this[PRIVATE5].device = device;
    });
  }
  async isSessionSupported(mode) {
    if (!this[PRIVATE5].device) {
      await this[PRIVATE5].devicePromise;
    }
    if (mode != "inline") {
      return Promise.resolve(this[PRIVATE5].device.isSessionSupported(mode));
    }
    return Promise.resolve(true);
  }
  async requestSession(mode, options) {
    if (!this[PRIVATE5].device) {
      if (mode != "inline") {
        throw new Error(POLYFILL_REQUEST_SESSION_ERROR);
      } else {
        await this[PRIVATE5].devicePromise;
      }
    }
    if (!XRSessionModes.includes(mode)) {
      throw new TypeError(`The provided value '${mode}' is not a valid enum value of type XRSessionMode`);
    }
    const defaultOptions = DEFAULT_SESSION_OPTIONS[mode];
    const requiredFeatures = defaultOptions.requiredFeatures.concat(options && options.requiredFeatures ? options.requiredFeatures : []);
    const optionalFeatures = defaultOptions.optionalFeatures.concat(options && options.optionalFeatures ? options.optionalFeatures : []);
    const enabledFeatures = /* @__PURE__ */ new Set();
    let requirementsFailed = false;
    for (let feature of requiredFeatures) {
      if (!this[PRIVATE5].device.isFeatureSupported(feature)) {
        console.error(`The required feature '${feature}' is not supported`);
        requirementsFailed = true;
      } else {
        enabledFeatures.add(feature);
      }
    }
    if (requirementsFailed) {
      throw new DOMException("Session does not support some required features", "NotSupportedError");
    }
    for (let feature of optionalFeatures) {
      if (!this[PRIVATE5].device.isFeatureSupported(feature)) {
        console.log(`The optional feature '${feature}' is not supported`);
      } else {
        enabledFeatures.add(feature);
      }
    }
    const sessionId = await this[PRIVATE5].device.requestSession(mode, enabledFeatures);
    const session = new XRSession(this[PRIVATE5].device, mode, sessionId);
    if (mode == "inline") {
      this[PRIVATE5].inlineSessions.add(session);
    } else {
      this[PRIVATE5].immersiveSession = session;
    }
    const onSessionEnd = () => {
      if (mode == "inline") {
        this[PRIVATE5].inlineSessions.delete(session);
      } else {
        this[PRIVATE5].immersiveSession = null;
      }
      session.removeEventListener("end", onSessionEnd);
    };
    session.addEventListener("end", onSessionEnd);
    return session;
  }
};

// ../../node_modules/webxr-polyfill/src/lib/now.js
var now;
if ("performance" in global_default === false) {
  let startTime = Date.now();
  now = () => Date.now() - startTime;
} else {
  now = () => performance.now();
}
var now_default = now;

// ../../node_modules/webxr-polyfill/src/api/XRPose.js
var PRIVATE6 = Symbol("@@webxr-polyfill/XRPose");
var XRPose2 = class {
  constructor(transform2, emulatedPosition) {
    this[PRIVATE6] = {
      transform: transform2,
      emulatedPosition
    };
  }
  get transform() {
    return this[PRIVATE6].transform;
  }
  get emulatedPosition() {
    return this[PRIVATE6].emulatedPosition;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRViewerPose.js
var PRIVATE7 = Symbol("@@webxr-polyfill/XRViewerPose");
var XRViewerPose = class extends XRPose2 {
  constructor(transform2, views, emulatedPosition = false) {
    super(transform2, emulatedPosition);
    this[PRIVATE7] = {
      views
    };
  }
  get views() {
    return this[PRIVATE7].views;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRViewport.js
var PRIVATE8 = Symbol("@@webxr-polyfill/XRViewport");
var XRViewport = class {
  constructor(target) {
    this[PRIVATE8] = { target };
  }
  get x() {
    return this[PRIVATE8].target.x;
  }
  get y() {
    return this[PRIVATE8].target.y;
  }
  get width() {
    return this[PRIVATE8].target.width;
  }
  get height() {
    return this[PRIVATE8].target.height;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRView.js
var XREyes = ["left", "right", "none"];
var PRIVATE9 = Symbol("@@webxr-polyfill/XRView");
var XRView = class {
  constructor(device, transform2, eye, sessionId) {
    if (!XREyes.includes(eye)) {
      throw new Error(`XREye must be one of: ${XREyes}`);
    }
    const temp = /* @__PURE__ */ Object.create(null);
    const viewport = new XRViewport(temp);
    this[PRIVATE9] = {
      device,
      eye,
      viewport,
      temp,
      sessionId,
      transform: transform2
    };
  }
  get eye() {
    return this[PRIVATE9].eye;
  }
  get projectionMatrix() {
    return this[PRIVATE9].device.getProjectionMatrix(this.eye);
  }
  get transform() {
    return this[PRIVATE9].transform;
  }
  _getViewport(layer) {
    if (this[PRIVATE9].device.getViewport(this[PRIVATE9].sessionId, this.eye, layer, this[PRIVATE9].temp)) {
      return this[PRIVATE9].viewport;
    }
    return void 0;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRFrame.js
var PRIVATE11 = Symbol("@@webxr-polyfill/XRFrame");
var NON_ACTIVE_MSG = "XRFrame access outside the callback that produced it is invalid.";
var NON_ANIMFRAME_MSG = "getViewerPose can only be called on XRFrame objects passed to XRSession.requestAnimationFrame callbacks.";
var NEXT_FRAME_ID = 0;
var XRFrame = class {
  constructor(device, session, sessionId) {
    this[PRIVATE11] = {
      id: ++NEXT_FRAME_ID,
      active: false,
      animationFrame: false,
      device,
      session,
      sessionId
    };
  }
  get session() {
    return this[PRIVATE11].session;
  }
  getViewerPose(referenceSpace) {
    if (!this[PRIVATE11].animationFrame) {
      throw new DOMException(NON_ANIMFRAME_MSG, "InvalidStateError");
    }
    if (!this[PRIVATE11].active) {
      throw new DOMException(NON_ACTIVE_MSG, "InvalidStateError");
    }
    const device = this[PRIVATE11].device;
    const session = this[PRIVATE11].session;
    session[PRIVATE10].viewerSpace._ensurePoseUpdated(device, this[PRIVATE11].id);
    referenceSpace._ensurePoseUpdated(device, this[PRIVATE11].id);
    let viewerTransform = referenceSpace._getSpaceRelativeTransform(session[PRIVATE10].viewerSpace);
    const views = [];
    for (let viewSpace of session[PRIVATE10].viewSpaces) {
      viewSpace._ensurePoseUpdated(device, this[PRIVATE11].id);
      let viewTransform = referenceSpace._getSpaceRelativeTransform(viewSpace);
      let view = new XRView(device, viewTransform, viewSpace.eye, this[PRIVATE11].sessionId);
      views.push(view);
    }
    let viewerPose = new XRViewerPose(viewerTransform, views, false);
    return viewerPose;
  }
  getPose(space, baseSpace) {
    if (!this[PRIVATE11].active) {
      throw new DOMException(NON_ACTIVE_MSG, "InvalidStateError");
    }
    const device = this[PRIVATE11].device;
    if (space._specialType === "target-ray" || space._specialType === "grip") {
      return device.getInputPose(space._inputSource, baseSpace, space._specialType);
    } else {
      space._ensurePoseUpdated(device, this[PRIVATE11].id);
      baseSpace._ensurePoseUpdated(device, this[PRIVATE11].id);
      let transform2 = baseSpace._getSpaceRelativeTransform(space);
      if (!transform2) {
        return null;
      }
      return new XRPose(transform2, false);
    }
    return null;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRRenderState.js
var PRIVATE12 = Symbol("@@webxr-polyfill/XRRenderState");
var XRRenderStateInit = Object.freeze({
  depthNear: 0.1,
  depthFar: 1e3,
  inlineVerticalFieldOfView: null,
  baseLayer: null
});
var XRRenderState = class {
  constructor(stateInit = {}) {
    const config = Object.assign({}, XRRenderStateInit, stateInit);
    this[PRIVATE12] = { config };
  }
  get depthNear() {
    return this[PRIVATE12].config.depthNear;
  }
  get depthFar() {
    return this[PRIVATE12].config.depthFar;
  }
  get inlineVerticalFieldOfView() {
    return this[PRIVATE12].config.inlineVerticalFieldOfView;
  }
  get baseLayer() {
    return this[PRIVATE12].config.baseLayer;
  }
};

// ../../node_modules/webxr-polyfill/src/constants.js
var POLYFILLED_XR_COMPATIBLE = Symbol("@@webxr-polyfill/polyfilled-xr-compatible");
var XR_COMPATIBLE = Symbol("@@webxr-polyfill/xr-compatible");

// ../../node_modules/webxr-polyfill/src/api/XRWebGLLayer.js
var PRIVATE13 = Symbol("@@webxr-polyfill/XRWebGLLayer");
var XRWebGLLayerInit = Object.freeze({
  antialias: true,
  depth: false,
  stencil: false,
  alpha: true,
  multiview: false,
  ignoreDepthValues: false,
  framebufferScaleFactor: 1
});
var XRWebGLLayer2 = class {
  constructor(session, context, layerInit = {}) {
    const config = Object.assign({}, XRWebGLLayerInit, layerInit);
    if (!(session instanceof XRSession2)) {
      throw new Error("session must be a XRSession");
    }
    if (session.ended) {
      throw new Error(`InvalidStateError`);
    }
    if (context[POLYFILLED_XR_COMPATIBLE]) {
      if (context[XR_COMPATIBLE] !== true) {
        throw new Error(`InvalidStateError`);
      }
    }
    const framebuffer = context.getParameter(context.FRAMEBUFFER_BINDING);
    this[PRIVATE13] = {
      context,
      config,
      framebuffer,
      session
    };
  }
  get context() {
    return this[PRIVATE13].context;
  }
  get antialias() {
    return this[PRIVATE13].config.antialias;
  }
  get ignoreDepthValues() {
    return true;
  }
  get framebuffer() {
    return this[PRIVATE13].framebuffer;
  }
  get framebufferWidth() {
    return this[PRIVATE13].context.drawingBufferWidth;
  }
  get framebufferHeight() {
    return this[PRIVATE13].context.drawingBufferHeight;
  }
  get _session() {
    return this[PRIVATE13].session;
  }
  getViewport(view) {
    return view._getViewport(this);
  }
  static getNativeFramebufferScaleFactor(session) {
    if (!session) {
      throw new TypeError("getNativeFramebufferScaleFactor must be passed a session.");
    }
    if (session[PRIVATE10].ended) {
      return 0;
    }
    return 1;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRInputSourceEvent.js
var PRIVATE14 = Symbol("@@webxr-polyfill/XRInputSourceEvent");
var XRInputSourceEvent = class extends Event {
  constructor(type2, eventInitDict) {
    super(type2, eventInitDict);
    this[PRIVATE14] = {
      frame: eventInitDict.frame,
      inputSource: eventInitDict.inputSource
    };
    Object.setPrototypeOf(this, XRInputSourceEvent.prototype);
  }
  get frame() {
    return this[PRIVATE14].frame;
  }
  get inputSource() {
    return this[PRIVATE14].inputSource;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRSessionEvent.js
var PRIVATE15 = Symbol("@@webxr-polyfill/XRSessionEvent");
var XRSessionEvent = class extends Event {
  constructor(type2, eventInitDict) {
    super(type2, eventInitDict);
    this[PRIVATE15] = {
      session: eventInitDict.session
    };
    Object.setPrototypeOf(this, XRSessionEvent.prototype);
  }
  get session() {
    return this[PRIVATE15].session;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRInputSourcesChangeEvent.js
var PRIVATE16 = Symbol("@@webxr-polyfill/XRInputSourcesChangeEvent");
var XRInputSourcesChangeEvent = class extends Event {
  constructor(type2, eventInitDict) {
    super(type2, eventInitDict);
    this[PRIVATE16] = {
      session: eventInitDict.session,
      added: eventInitDict.added,
      removed: eventInitDict.removed
    };
    Object.setPrototypeOf(this, XRInputSourcesChangeEvent.prototype);
  }
  get session() {
    return this[PRIVATE16].session;
  }
  get added() {
    return this[PRIVATE16].added;
  }
  get removed() {
    return this[PRIVATE16].removed;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRSession.js
var PRIVATE10 = Symbol("@@webxr-polyfill/XRSession");
var XRViewSpace = class extends XRSpace {
  constructor(eye) {
    super(eye);
  }
  get eye() {
    return this._specialType;
  }
  _onPoseUpdate(device) {
    this._inverseBaseMatrix = device.getBaseViewMatrix(this._specialType);
  }
};
var XRSession2 = class extends EventTarget {
  constructor(device, mode, id2) {
    super();
    let immersive = mode != "inline";
    let initialRenderState = new XRRenderState({
      inlineVerticalFieldOfView: immersive ? null : Math.PI * 0.5
    });
    this[PRIVATE10] = {
      device,
      mode,
      immersive,
      ended: false,
      suspended: false,
      frameCallbacks: [],
      currentFrameCallbacks: null,
      frameHandle: 0,
      deviceFrameHandle: null,
      id: id2,
      activeRenderState: initialRenderState,
      pendingRenderState: null,
      viewerSpace: new XRReferenceSpace("viewer"),
      viewSpaces: [],
      currentInputSources: []
    };
    if (immersive) {
      this[PRIVATE10].viewSpaces.push(new XRViewSpace("left"), new XRViewSpace("right"));
    } else {
      this[PRIVATE10].viewSpaces.push(new XRViewSpace("none"));
    }
    this[PRIVATE10].onDeviceFrame = () => {
      if (this[PRIVATE10].ended || this[PRIVATE10].suspended) {
        return;
      }
      this[PRIVATE10].deviceFrameHandle = null;
      this[PRIVATE10].startDeviceFrameLoop();
      if (this[PRIVATE10].pendingRenderState !== null) {
        this[PRIVATE10].activeRenderState = new XRRenderState(this[PRIVATE10].pendingRenderState);
        this[PRIVATE10].pendingRenderState = null;
        if (this[PRIVATE10].activeRenderState.baseLayer) {
          this[PRIVATE10].device.onBaseLayerSet(this[PRIVATE10].id, this[PRIVATE10].activeRenderState.baseLayer);
        }
      }
      if (this[PRIVATE10].activeRenderState.baseLayer === null) {
        return;
      }
      const frame = new XRFrame(device, this, this[PRIVATE10].id);
      const callbacks = this[PRIVATE10].currentFrameCallbacks = this[PRIVATE10].frameCallbacks;
      this[PRIVATE10].frameCallbacks = [];
      frame[PRIVATE11].active = true;
      frame[PRIVATE11].animationFrame = true;
      this[PRIVATE10].device.onFrameStart(this[PRIVATE10].id, this[PRIVATE10].activeRenderState);
      this._checkInputSourcesChange();
      const rightNow = now_default();
      for (let i = 0; i < callbacks.length; i++) {
        try {
          if (!callbacks[i].cancelled && typeof callbacks[i].callback === "function") {
            callbacks[i].callback(rightNow, frame);
          }
        } catch (err) {
          console.error(err);
        }
      }
      this[PRIVATE10].currentFrameCallbacks = null;
      frame[PRIVATE11].active = false;
      this[PRIVATE10].device.onFrameEnd(this[PRIVATE10].id);
    };
    this[PRIVATE10].startDeviceFrameLoop = () => {
      if (this[PRIVATE10].deviceFrameHandle === null) {
        this[PRIVATE10].deviceFrameHandle = this[PRIVATE10].device.requestAnimationFrame(this[PRIVATE10].onDeviceFrame);
      }
    };
    this[PRIVATE10].stopDeviceFrameLoop = () => {
      const handle = this[PRIVATE10].deviceFrameHandle;
      if (handle !== null) {
        this[PRIVATE10].device.cancelAnimationFrame(handle);
        this[PRIVATE10].deviceFrameHandle = null;
      }
    };
    this[PRIVATE10].onPresentationEnd = (sessionId) => {
      if (sessionId !== this[PRIVATE10].id) {
        this[PRIVATE10].suspended = false;
        this[PRIVATE10].startDeviceFrameLoop();
        this.dispatchEvent("focus", { session: this });
        return;
      }
      this[PRIVATE10].ended = true;
      this[PRIVATE10].stopDeviceFrameLoop();
      device.removeEventListener("@@webxr-polyfill/vr-present-end", this[PRIVATE10].onPresentationEnd);
      device.removeEventListener("@@webxr-polyfill/vr-present-start", this[PRIVATE10].onPresentationStart);
      device.removeEventListener("@@webxr-polyfill/input-select-start", this[PRIVATE10].onSelectStart);
      device.removeEventListener("@@webxr-polyfill/input-select-end", this[PRIVATE10].onSelectEnd);
      this.dispatchEvent("end", new XRSessionEvent("end", { session: this }));
    };
    device.addEventListener("@@webxr-polyfill/vr-present-end", this[PRIVATE10].onPresentationEnd);
    this[PRIVATE10].onPresentationStart = (sessionId) => {
      if (sessionId === this[PRIVATE10].id) {
        return;
      }
      this[PRIVATE10].suspended = true;
      this[PRIVATE10].stopDeviceFrameLoop();
      this.dispatchEvent("blur", { session: this });
    };
    device.addEventListener("@@webxr-polyfill/vr-present-start", this[PRIVATE10].onPresentationStart);
    this[PRIVATE10].onSelectStart = (evt) => {
      if (evt.sessionId !== this[PRIVATE10].id) {
        return;
      }
      this[PRIVATE10].dispatchInputSourceEvent("selectstart", evt.inputSource);
    };
    device.addEventListener("@@webxr-polyfill/input-select-start", this[PRIVATE10].onSelectStart);
    this[PRIVATE10].onSelectEnd = (evt) => {
      if (evt.sessionId !== this[PRIVATE10].id) {
        return;
      }
      this[PRIVATE10].dispatchInputSourceEvent("selectend", evt.inputSource);
      this[PRIVATE10].dispatchInputSourceEvent("select", evt.inputSource);
    };
    device.addEventListener("@@webxr-polyfill/input-select-end", this[PRIVATE10].onSelectEnd);
    this[PRIVATE10].onSqueezeStart = (evt) => {
      if (evt.sessionId !== this[PRIVATE10].id) {
        return;
      }
      this[PRIVATE10].dispatchInputSourceEvent("squeezestart", evt.inputSource);
    };
    device.addEventListener("@@webxr-polyfill/input-squeeze-start", this[PRIVATE10].onSqueezeStart);
    this[PRIVATE10].onSqueezeEnd = (evt) => {
      if (evt.sessionId !== this[PRIVATE10].id) {
        return;
      }
      this[PRIVATE10].dispatchInputSourceEvent("squeezeend", evt.inputSource);
      this[PRIVATE10].dispatchInputSourceEvent("squeeze", evt.inputSource);
    };
    device.addEventListener("@@webxr-polyfill/input-squeeze-end", this[PRIVATE10].onSqueezeEnd);
    this[PRIVATE10].dispatchInputSourceEvent = (type2, inputSource) => {
      const frame = new XRFrame(device, this, this[PRIVATE10].id);
      const event = new XRInputSourceEvent(type2, { frame, inputSource });
      frame[PRIVATE11].active = true;
      this.dispatchEvent(type2, event);
      frame[PRIVATE11].active = false;
    };
    this[PRIVATE10].startDeviceFrameLoop();
    this.onblur = void 0;
    this.onfocus = void 0;
    this.onresetpose = void 0;
    this.onend = void 0;
    this.onselect = void 0;
    this.onselectstart = void 0;
    this.onselectend = void 0;
  }
  get renderState() {
    return this[PRIVATE10].activeRenderState;
  }
  get environmentBlendMode() {
    return this[PRIVATE10].device.environmentBlendMode || "opaque";
  }
  async requestReferenceSpace(type2) {
    if (this[PRIVATE10].ended) {
      return;
    }
    if (!XRReferenceSpaceTypes.includes(type2)) {
      throw new TypeError(`XRReferenceSpaceType must be one of ${XRReferenceSpaceTypes}`);
    }
    if (!this[PRIVATE10].device.doesSessionSupportReferenceSpace(this[PRIVATE10].id, type2)) {
      throw new DOMException(`The ${type2} reference space is not supported by this session.`, "NotSupportedError");
    }
    if (type2 === "viewer") {
      return this[PRIVATE10].viewerSpace;
    }
    let transform2 = await this[PRIVATE10].device.requestFrameOfReferenceTransform(type2);
    if (type2 === "bounded-floor") {
      if (!transform2) {
        throw new DOMException(`${type2} XRReferenceSpace not supported by this device.`, "NotSupportedError");
      }
      let bounds = this[PRIVATE10].device.requestStageBounds();
      if (!bounds) {
        throw new DOMException(`${type2} XRReferenceSpace not supported by this device.`, "NotSupportedError");
      }
      throw new DOMException(`The WebXR polyfill does not support the ${type2} reference space yet.`, "NotSupportedError");
    }
    return new XRReferenceSpace(type2, transform2);
  }
  requestAnimationFrame(callback) {
    if (this[PRIVATE10].ended) {
      return;
    }
    const handle = ++this[PRIVATE10].frameHandle;
    this[PRIVATE10].frameCallbacks.push({
      handle,
      callback,
      cancelled: false
    });
    return handle;
  }
  cancelAnimationFrame(handle) {
    let callbacks = this[PRIVATE10].frameCallbacks;
    let index = callbacks.findIndex((d) => d && d.handle === handle);
    if (index > -1) {
      callbacks[index].cancelled = true;
      callbacks.splice(index, 1);
    }
    callbacks = this[PRIVATE10].currentFrameCallbacks;
    if (callbacks) {
      index = callbacks.findIndex((d) => d && d.handle === handle);
      if (index > -1) {
        callbacks[index].cancelled = true;
      }
    }
  }
  get inputSources() {
    return this[PRIVATE10].device.getInputSources();
  }
  async end() {
    if (this[PRIVATE10].ended) {
      return;
    }
    if (this[PRIVATE10].immersive) {
      this[PRIVATE10].ended = true;
      this[PRIVATE10].device.removeEventListener("@@webxr-polyfill/vr-present-start", this[PRIVATE10].onPresentationStart);
      this[PRIVATE10].device.removeEventListener("@@webxr-polyfill/vr-present-end", this[PRIVATE10].onPresentationEnd);
      this[PRIVATE10].device.removeEventListener("@@webxr-polyfill/input-select-start", this[PRIVATE10].onSelectStart);
      this[PRIVATE10].device.removeEventListener("@@webxr-polyfill/input-select-end", this[PRIVATE10].onSelectEnd);
      this.dispatchEvent("end", new XRSessionEvent("end", { session: this }));
    }
    this[PRIVATE10].stopDeviceFrameLoop();
    return this[PRIVATE10].device.endSession(this[PRIVATE10].id);
  }
  updateRenderState(newState) {
    if (this[PRIVATE10].ended) {
      const message = "Can't call updateRenderState on an XRSession that has already ended.";
      throw new Error(message);
    }
    if (newState.baseLayer && newState.baseLayer._session !== this) {
      const message = "Called updateRenderState with a base layer that was created by a different session.";
      throw new Error(message);
    }
    const fovSet = newState.inlineVerticalFieldOfView !== null && newState.inlineVerticalFieldOfView !== void 0;
    if (fovSet) {
      if (this[PRIVATE10].immersive) {
        const message = "inlineVerticalFieldOfView must not be set for an XRRenderState passed to updateRenderState for an immersive session.";
        throw new Error(message);
      } else {
        newState.inlineVerticalFieldOfView = Math.min(3.13, Math.max(0.01, newState.inlineVerticalFieldOfView));
      }
    }
    if (this[PRIVATE10].pendingRenderState === null) {
      const activeRenderState = this[PRIVATE10].activeRenderState;
      this[PRIVATE10].pendingRenderState = {
        depthNear: activeRenderState.depthNear,
        depthFar: activeRenderState.depthFar,
        inlineVerticalFieldOfView: activeRenderState.inlineVerticalFieldOfView,
        baseLayer: activeRenderState.baseLayer
      };
    }
    Object.assign(this[PRIVATE10].pendingRenderState, newState);
  }
  _checkInputSourcesChange() {
    const added = [];
    const removed = [];
    const newInputSources = this.inputSources;
    const oldInputSources = this[PRIVATE10].currentInputSources;
    for (const newInputSource of newInputSources) {
      if (!oldInputSources.includes(newInputSource)) {
        added.push(newInputSource);
      }
    }
    for (const oldInputSource of oldInputSources) {
      if (!newInputSources.includes(oldInputSource)) {
        removed.push(oldInputSource);
      }
    }
    if (added.length > 0 || removed.length > 0) {
      this.dispatchEvent("inputsourceschange", new XRInputSourcesChangeEvent("inputsourceschange", {
        session: this,
        added,
        removed
      }));
    }
    this[PRIVATE10].currentInputSources.length = 0;
    for (const newInputSource of newInputSources) {
      this[PRIVATE10].currentInputSources.push(newInputSource);
    }
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRInputSource.js
var PRIVATE17 = Symbol("@@webxr-polyfill/XRInputSource");
var XRInputSource = class {
  constructor(impl) {
    this[PRIVATE17] = {
      impl,
      gripSpace: new XRSpace("grip", this),
      targetRaySpace: new XRSpace("target-ray", this)
    };
  }
  get handedness() {
    return this[PRIVATE17].impl.handedness;
  }
  get targetRayMode() {
    return this[PRIVATE17].impl.targetRayMode;
  }
  get gripSpace() {
    let mode = this[PRIVATE17].impl.targetRayMode;
    if (mode === "gaze" || mode === "screen") {
      return null;
    }
    return this[PRIVATE17].gripSpace;
  }
  get targetRaySpace() {
    return this[PRIVATE17].targetRaySpace;
  }
  get profiles() {
    return this[PRIVATE17].impl.profiles;
  }
  get gamepad() {
    return this[PRIVATE17].impl.gamepad;
  }
};

// ../../node_modules/webxr-polyfill/src/api/XRReferenceSpaceEvent.js
var PRIVATE18 = Symbol("@@webxr-polyfill/XRReferenceSpaceEvent");
var XRReferenceSpaceEvent = class extends Event {
  constructor(type2, eventInitDict) {
    super(type2, eventInitDict);
    this[PRIVATE18] = {
      referenceSpace: eventInitDict.referenceSpace,
      transform: eventInitDict.transform || null
    };
    Object.setPrototypeOf(this, XRReferenceSpaceEvent.prototype);
  }
  get referenceSpace() {
    return this[PRIVATE18].referenceSpace;
  }
  get transform() {
    return this[PRIVATE18].transform;
  }
};

// ../../node_modules/webxr-polyfill/src/api/index.js
var api_default = {
  XRSystem,
  XRSession: XRSession2,
  XRSessionEvent,
  XRFrame,
  XRView,
  XRViewport,
  XRViewerPose,
  XRWebGLLayer: XRWebGLLayer2,
  XRSpace,
  XRReferenceSpace,
  XRReferenceSpaceEvent,
  XRInputSource,
  XRInputSourceEvent,
  XRInputSourcesChangeEvent,
  XRRenderState,
  XRRigidTransform: XRRigidTransform2,
  XRPose: XRPose2
};

// ../../node_modules/webxr-polyfill/src/polyfill-globals.js
var polyfillMakeXRCompatible = (Context) => {
  if (typeof Context.prototype.makeXRCompatible === "function") {
    return false;
  }
  Context.prototype.makeXRCompatible = function() {
    this[XR_COMPATIBLE] = true;
    return Promise.resolve();
  };
  return true;
};
var polyfillGetContext = (Canvas2) => {
  const getContext = Canvas2.prototype.getContext;
  Canvas2.prototype.getContext = function(contextType, glAttribs) {
    const ctx = getContext.call(this, contextType, glAttribs);
    if (ctx) {
      ctx[POLYFILLED_XR_COMPATIBLE] = true;
      if (glAttribs && "xrCompatible" in glAttribs) {
        ctx[XR_COMPATIBLE] = glAttribs.xrCompatible;
      }
    }
    return ctx;
  };
};

// ../../node_modules/webxr-polyfill/src/utils.js
var isImageBitmapSupported = (global2) => !!(global2.ImageBitmapRenderingContext && global2.createImageBitmap);
var isMobile2 = (global2) => {
  var check = false;
  (function(a) {
    if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4)))
      check = true;
  })(global2.navigator.userAgent || global2.navigator.vendor || global2.opera);
  return check;
};
var applyCanvasStylesForMinimalRendering = (canvas) => {
  canvas.style.display = "block";
  canvas.style.position = "absolute";
  canvas.style.width = canvas.style.height = "1px";
  canvas.style.top = canvas.style.left = "0px";
};

// ../../node_modules/webxr-polyfill/src/devices/CardboardXRDevice.js
var import_cardboard_vr_display = __toESM(require_cardboard_vr_display());

// ../../node_modules/webxr-polyfill/src/devices/XRDevice.js
var XRDevice = class extends EventTarget {
  constructor(global2) {
    super();
    this.global = global2;
    this.onWindowResize = this.onWindowResize.bind(this);
    this.global.window.addEventListener("resize", this.onWindowResize);
    this.environmentBlendMode = "opaque";
  }
  onBaseLayerSet(sessionId, layer) {
    throw new Error("Not implemented");
  }
  isSessionSupported(mode) {
    throw new Error("Not implemented");
  }
  isFeatureSupported(featureDescriptor) {
    throw new Error("Not implemented");
  }
  async requestSession(mode, enabledFeatures) {
    throw new Error("Not implemented");
  }
  requestAnimationFrame(callback) {
    throw new Error("Not implemented");
  }
  onFrameStart(sessionId) {
    throw new Error("Not implemented");
  }
  onFrameEnd(sessionId) {
    throw new Error("Not implemented");
  }
  doesSessionSupportReferenceSpace(sessionId, type2) {
    throw new Error("Not implemented");
  }
  requestStageBounds() {
    throw new Error("Not implemented");
  }
  async requestFrameOfReferenceTransform(type2, options) {
    return void 0;
  }
  cancelAnimationFrame(handle) {
    throw new Error("Not implemented");
  }
  endSession(sessionId) {
    throw new Error("Not implemented");
  }
  getViewport(sessionId, eye, layer, target) {
    throw new Error("Not implemented");
  }
  getProjectionMatrix(eye) {
    throw new Error("Not implemented");
  }
  getBasePoseMatrix() {
    throw new Error("Not implemented");
  }
  getBaseViewMatrix(eye) {
    throw new Error("Not implemented");
  }
  getInputSources() {
    throw new Error("Not implemented");
  }
  getInputPose(inputSource, coordinateSystem, poseType) {
    throw new Error("Not implemented");
  }
  onWindowResize() {
    this.onWindowResize();
  }
};

// ../../node_modules/webxr-polyfill/src/devices/GamepadMappings.js
var daydream = {
  mapping: "",
  profiles: ["google-daydream", "generic-trigger-touchpad"],
  buttons: {
    length: 3,
    0: null,
    1: null,
    2: 0
  }
};
var viveFocus = {
  mapping: "xr-standard",
  profiles: ["htc-vive-focus", "generic-trigger-touchpad"],
  buttons: {
    length: 3,
    0: 1,
    1: null,
    2: 0
  }
};
var oculusGo = {
  mapping: "xr-standard",
  profiles: ["oculus-go", "generic-trigger-touchpad"],
  buttons: {
    length: 3,
    0: 1,
    1: null,
    2: 0
  },
  gripTransform: {
    orientation: [Math.PI * 0.11, 0, 0, 1]
  }
};
var oculusTouch = {
  mapping: "xr-standard",
  displayProfiles: {
    "Oculus Quest": ["oculus-touch-v2", "oculus-touch", "generic-trigger-squeeze-thumbstick"]
  },
  profiles: ["oculus-touch", "generic-trigger-squeeze-thumbstick"],
  axes: {
    length: 4,
    0: null,
    1: null,
    2: 0,
    3: 1
  },
  buttons: {
    length: 7,
    0: 1,
    1: 2,
    2: null,
    3: 0,
    4: 3,
    5: 4,
    6: null
  },
  gripTransform: {
    position: [0, -0.02, 0.04, 1],
    orientation: [Math.PI * 0.11, 0, 0, 1]
  }
};
var openVr = {
  mapping: "xr-standard",
  profiles: ["htc-vive", "generic-trigger-squeeze-touchpad"],
  displayProfiles: {
    "HTC Vive": ["htc-vive", "generic-trigger-squeeze-touchpad"],
    "HTC Vive DVT": ["htc-vive", "generic-trigger-squeeze-touchpad"],
    "Valve Index": ["valve-index", "generic-trigger-squeeze-touchpad-thumbstick"]
  },
  buttons: {
    length: 3,
    0: 1,
    1: 2,
    2: 0
  },
  gripTransform: {
    position: [0, 0, 0.05, 1]
  },
  targetRayTransform: {
    orientation: [Math.PI * -0.08, 0, 0, 1]
  },
  userAgentOverrides: {
    "Firefox": {
      axes: {
        invert: [1, 3]
      }
    }
  }
};
var samsungGearVR = {
  mapping: "xr-standard",
  profiles: ["samsung-gearvr", "generic-trigger-touchpad"],
  buttons: {
    length: 3,
    0: 1,
    1: null,
    2: 0
  },
  gripTransform: {
    orientation: [Math.PI * 0.11, 0, 0, 1]
  }
};
var samsungOdyssey = {
  mapping: "xr-standard",
  profiles: ["samsung-odyssey", "microsoft-mixed-reality", "generic-trigger-squeeze-touchpad-thumbstick"],
  buttons: {
    length: 4,
    0: 1,
    1: 0,
    2: 2,
    3: 4
  },
  gripTransform: {
    position: [0, -0.02, 0.04, 1],
    orientation: [Math.PI * 0.11, 0, 0, 1]
  }
};
var windowsMixedReality = {
  mapping: "xr-standard",
  profiles: ["microsoft-mixed-reality", "generic-trigger-squeeze-touchpad-thumbstick"],
  buttons: {
    length: 4,
    0: 1,
    1: 0,
    2: 2,
    3: 4
  },
  gripTransform: {
    position: [0, -0.02, 0.04, 1],
    orientation: [Math.PI * 0.11, 0, 0, 1]
  }
};
var GamepadMappings = {
  "Daydream Controller": daydream,
  "Gear VR Controller": samsungGearVR,
  "HTC Vive Focus Controller": viveFocus,
  "Oculus Go Controller": oculusGo,
  "Oculus Touch (Right)": oculusTouch,
  "Oculus Touch (Left)": oculusTouch,
  "OpenVR Gamepad": openVr,
  "Spatial Controller (Spatial Interaction Source) 045E-065A": windowsMixedReality,
  "Spatial Controller (Spatial Interaction Source) 045E-065D": samsungOdyssey,
  "Windows Mixed Reality (Right)": windowsMixedReality,
  "Windows Mixed Reality (Left)": windowsMixedReality
};
var GamepadMappings_default = GamepadMappings;

// ../../node_modules/webxr-polyfill/src/lib/OrientationArmModel.js
var HEAD_ELBOW_OFFSET_RIGHTHANDED = fromValues2(0.155, -0.465, -0.15);
var HEAD_ELBOW_OFFSET_LEFTHANDED = fromValues2(-0.155, -0.465, -0.15);
var ELBOW_WRIST_OFFSET = fromValues2(0, 0, -0.25);
var WRIST_CONTROLLER_OFFSET = fromValues2(0, 0, 0.05);
var ARM_EXTENSION_OFFSET = fromValues2(-0.08, 0.14, 0.08);
var ELBOW_BEND_RATIO = 0.4;
var EXTENSION_RATIO_WEIGHT = 0.4;
var MIN_ANGULAR_SPEED = 0.61;
var MIN_ANGLE_DELTA = 0.175;
var MIN_EXTENSION_COS = 0.12;
var MAX_EXTENSION_COS = 0.87;
var RAD_TO_DEG = 180 / Math.PI;
function eulerFromQuaternion(out, q, order) {
  function clamp2(value2, min3, max3) {
    return value2 < min3 ? min3 : value2 > max3 ? max3 : value2;
  }
  var sqx = q[0] * q[0];
  var sqy = q[1] * q[1];
  var sqz = q[2] * q[2];
  var sqw = q[3] * q[3];
  if (order === "XYZ") {
    out[0] = Math.atan2(2 * (q[0] * q[3] - q[1] * q[2]), sqw - sqx - sqy + sqz);
    out[1] = Math.asin(clamp2(2 * (q[0] * q[2] + q[1] * q[3]), -1, 1));
    out[2] = Math.atan2(2 * (q[2] * q[3] - q[0] * q[1]), sqw + sqx - sqy - sqz);
  } else if (order === "YXZ") {
    out[0] = Math.asin(clamp2(2 * (q[0] * q[3] - q[1] * q[2]), -1, 1));
    out[1] = Math.atan2(2 * (q[0] * q[2] + q[1] * q[3]), sqw - sqx - sqy + sqz);
    out[2] = Math.atan2(2 * (q[0] * q[1] + q[2] * q[3]), sqw - sqx + sqy - sqz);
  } else if (order === "ZXY") {
    out[0] = Math.asin(clamp2(2 * (q[0] * q[3] + q[1] * q[2]), -1, 1));
    out[1] = Math.atan2(2 * (q[1] * q[3] - q[2] * q[0]), sqw - sqx - sqy + sqz);
    out[2] = Math.atan2(2 * (q[2] * q[3] - q[0] * q[1]), sqw - sqx + sqy - sqz);
  } else if (order === "ZYX") {
    out[0] = Math.atan2(2 * (q[0] * q[3] + q[2] * q[1]), sqw - sqx - sqy + sqz);
    out[1] = Math.asin(clamp2(2 * (q[1] * q[3] - q[0] * q[2]), -1, 1));
    out[2] = Math.atan2(2 * (q[0] * q[1] + q[2] * q[3]), sqw + sqx - sqy - sqz);
  } else if (order === "YZX") {
    out[0] = Math.atan2(2 * (q[0] * q[3] - q[2] * q[1]), sqw - sqx + sqy - sqz);
    out[1] = Math.atan2(2 * (q[1] * q[3] - q[0] * q[2]), sqw + sqx - sqy - sqz);
    out[2] = Math.asin(clamp2(2 * (q[0] * q[1] + q[2] * q[3]), -1, 1));
  } else if (order === "XZY") {
    out[0] = Math.atan2(2 * (q[0] * q[3] + q[1] * q[2]), sqw - sqx + sqy - sqz);
    out[1] = Math.atan2(2 * (q[0] * q[2] + q[1] * q[3]), sqw + sqx - sqy - sqz);
    out[2] = Math.asin(clamp2(2 * (q[2] * q[3] - q[0] * q[1]), -1, 1));
  } else {
    console.log("No order given for quaternion to euler conversion.");
    return;
  }
}
var OrientationArmModel = class {
  constructor() {
    this.hand = "right";
    this.headElbowOffset = HEAD_ELBOW_OFFSET_RIGHTHANDED;
    this.controllerQ = create7();
    this.lastControllerQ = create7();
    this.headQ = create7();
    this.headPos = create4();
    this.elbowPos = create4();
    this.wristPos = create4();
    this.time = null;
    this.lastTime = null;
    this.rootQ = create7();
    this.position = create4();
  }
  setHandedness(hand) {
    if (this.hand != hand) {
      this.hand = hand;
      if (this.hand == "left") {
        this.headElbowOffset = HEAD_ELBOW_OFFSET_LEFTHANDED;
      } else {
        this.headElbowOffset = HEAD_ELBOW_OFFSET_RIGHTHANDED;
      }
    }
  }
  update(controllerOrientation, headPoseMatrix) {
    this.time = now_default();
    if (controllerOrientation) {
      copy5(this.lastControllerQ, this.controllerQ);
      copy5(this.controllerQ, controllerOrientation);
    }
    if (headPoseMatrix) {
      getTranslation(this.headPos, headPoseMatrix);
      getRotation(this.headQ, headPoseMatrix);
    }
    let headYawQ = this.getHeadYawOrientation_();
    let angleDelta = this.quatAngle_(this.lastControllerQ, this.controllerQ);
    let timeDelta = (this.time - this.lastTime) / 1e3;
    let controllerAngularSpeed = angleDelta / timeDelta;
    if (controllerAngularSpeed > MIN_ANGULAR_SPEED) {
      slerp(this.rootQ, this.rootQ, headYawQ, Math.min(angleDelta / MIN_ANGLE_DELTA, 1));
    } else {
      copy5(this.rootQ, headYawQ);
    }
    let controllerForward = fromValues2(0, 0, -1);
    transformQuat2(controllerForward, controllerForward, this.controllerQ);
    let controllerDotY = dot2(controllerForward, [0, 1, 0]);
    let extensionRatio = this.clamp_((controllerDotY - MIN_EXTENSION_COS) / MAX_EXTENSION_COS, 0, 1);
    let controllerCameraQ = clone4(this.rootQ);
    invert2(controllerCameraQ, controllerCameraQ);
    multiply3(controllerCameraQ, controllerCameraQ, this.controllerQ);
    let elbowPos = this.elbowPos;
    copy3(elbowPos, this.headPos);
    add2(elbowPos, elbowPos, this.headElbowOffset);
    let elbowOffset = clone2(ARM_EXTENSION_OFFSET);
    scale2(elbowOffset, elbowOffset, extensionRatio);
    add2(elbowPos, elbowPos, elbowOffset);
    let totalAngle = this.quatAngle_(controllerCameraQ, create7());
    let totalAngleDeg = totalAngle * RAD_TO_DEG;
    let lerpSuppression = 1 - Math.pow(totalAngleDeg / 180, 4);
    sssss;
    let elbowRatio = ELBOW_BEND_RATIO;
    let wristRatio = 1 - ELBOW_BEND_RATIO;
    let lerpValue = lerpSuppression * (elbowRatio + wristRatio * extensionRatio * EXTENSION_RATIO_WEIGHT);
    let wristQ = create7();
    slerp(wristQ, wristQ, controllerCameraQ, lerpValue);
    let invWristQ = invert2(create7(), wristQ);
    let elbowQ = clone4(controllerCameraQ);
    multiply3(elbowQ, elbowQ, invWristQ);
    let wristPos = this.wristPos;
    copy3(wristPos, WRIST_CONTROLLER_OFFSET);
    transformQuat2(wristPos, wristPos, wristQ);
    add2(wristPos, wristPos, ELBOW_WRIST_OFFSET);
    transformQuat2(wristPos, wristPos, elbowQ);
    add2(wristPos, wristPos, elbowPos);
    let offset = clone2(ARM_EXTENSION_OFFSET);
    scale2(offset, offset, extensionRatio);
    add2(this.position, this.wristPos, offset);
    transformQuat2(this.position, this.position, this.rootQ);
    this.lastTime = this.time;
  }
  getPosition() {
    return this.position;
  }
  getHeadYawOrientation_() {
    let headEuler = create4();
    eulerFromQuaternion(headEuler, this.headQ, "YXZ");
    let destinationQ = fromEuler(create7(), 0, headEuler[1] * RAD_TO_DEG, 0);
    return destinationQ;
  }
  clamp_(value2, min3, max3) {
    return Math.min(Math.max(value2, min3), max3);
  }
  quatAngle_(q1, q2) {
    let vec1 = [0, 0, -1];
    let vec2 = [0, 0, -1];
    transformQuat2(vec1, vec1, q1);
    transformQuat2(vec2, vec2, q2);
    return angle2(vec1, vec2);
  }
};

// ../../node_modules/webxr-polyfill/src/devices/GamepadXRInputSource.js
var PRIVATE19 = Symbol("@@webxr-polyfill/XRRemappedGamepad");
var PLACEHOLDER_BUTTON = { pressed: false, touched: false, value: 0 };
Object.freeze(PLACEHOLDER_BUTTON);
var XRRemappedGamepad = class {
  constructor(gamepad, display2, map) {
    if (!map) {
      map = {};
    }
    if (map.userAgentOverrides) {
      for (let agent in map.userAgentOverrides) {
        if (navigator.userAgent.includes(agent)) {
          let override = map.userAgentOverrides[agent];
          for (let key in override) {
            if (key in map) {
              Object.assign(map[key], override[key]);
            } else {
              map[key] = override[key];
            }
          }
          break;
        }
      }
    }
    let axes = new Array(map.axes && map.axes.length ? map.axes.length : gamepad.axes.length);
    let buttons = new Array(map.buttons && map.buttons.length ? map.buttons.length : gamepad.buttons.length);
    let gripTransform = null;
    if (map.gripTransform) {
      let orientation = map.gripTransform.orientation || [0, 0, 0, 1];
      gripTransform = create3();
      fromRotationTranslation(gripTransform, normalize4(orientation, orientation), map.gripTransform.position || [0, 0, 0]);
    }
    let targetRayTransform = null;
    if (map.targetRayTransform) {
      let orientation = map.targetRayTransform.orientation || [0, 0, 0, 1];
      targetRayTransform = create3();
      fromRotationTranslation(targetRayTransform, normalize4(orientation, orientation), map.targetRayTransform.position || [0, 0, 0]);
    }
    let profiles = map.profiles;
    if (map.displayProfiles) {
      if (display2.displayName in map.displayProfiles) {
        profiles = map.displayProfiles[display2.displayName];
      }
    }
    this[PRIVATE19] = {
      gamepad,
      map,
      profiles: profiles || [gamepad.id],
      mapping: map.mapping || gamepad.mapping,
      axes,
      buttons,
      gripTransform,
      targetRayTransform
    };
    this._update();
  }
  _update() {
    let gamepad = this[PRIVATE19].gamepad;
    let map = this[PRIVATE19].map;
    let axes = this[PRIVATE19].axes;
    for (let i = 0; i < axes.length; ++i) {
      if (map.axes && i in map.axes) {
        if (map.axes[i] === null) {
          axes[i] = 0;
        } else {
          axes[i] = gamepad.axes[map.axes[i]];
        }
      } else {
        axes[i] = gamepad.axes[i];
      }
    }
    if (map.axes && map.axes.invert) {
      for (let axis of map.axes.invert) {
        if (axis < axes.length) {
          axes[axis] *= -1;
        }
      }
    }
    let buttons = this[PRIVATE19].buttons;
    for (let i = 0; i < buttons.length; ++i) {
      if (map.buttons && i in map.buttons) {
        if (map.buttons[i] === null) {
          buttons[i] = PLACEHOLDER_BUTTON;
        } else {
          buttons[i] = gamepad.buttons[map.buttons[i]];
        }
      } else {
        buttons[i] = gamepad.buttons[i];
      }
    }
  }
  get id() {
    return "";
  }
  get _profiles() {
    return this[PRIVATE19].profiles;
  }
  get index() {
    return -1;
  }
  get connected() {
    return this[PRIVATE19].gamepad.connected;
  }
  get timestamp() {
    return this[PRIVATE19].gamepad.timestamp;
  }
  get mapping() {
    return this[PRIVATE19].mapping;
  }
  get axes() {
    return this[PRIVATE19].axes;
  }
  get buttons() {
    return this[PRIVATE19].buttons;
  }
  get hapticActuators() {
    return this[PRIVATE19].gamepad.hapticActuators;
  }
};
var GamepadXRInputSource = class {
  constructor(polyfill, display2, primaryButtonIndex = 0, primarySqueezeButtonIndex = -1) {
    this.polyfill = polyfill;
    this.display = display2;
    this.nativeGamepad = null;
    this.gamepad = null;
    this.inputSource = new XRInputSource(this);
    this.lastPosition = create4();
    this.emulatedPosition = false;
    this.basePoseMatrix = create3();
    this.outputMatrix = create3();
    this.primaryButtonIndex = primaryButtonIndex;
    this.primaryActionPressed = false;
    this.primarySqueezeButtonIndex = primarySqueezeButtonIndex;
    this.primarySqueezeActionPressed = false;
    this.handedness = "";
    this.targetRayMode = "gaze";
    this.armModel = null;
  }
  get profiles() {
    return this.gamepad ? this.gamepad._profiles : [];
  }
  updateFromGamepad(gamepad) {
    if (this.nativeGamepad !== gamepad) {
      this.nativeGamepad = gamepad;
      if (gamepad) {
        this.gamepad = new XRRemappedGamepad(gamepad, this.display, GamepadMappings_default[gamepad.id]);
      } else {
        this.gamepad = null;
      }
    }
    this.handedness = gamepad.hand === "" ? "none" : gamepad.hand;
    if (this.gamepad) {
      this.gamepad._update();
    }
    if (gamepad.pose) {
      this.targetRayMode = "tracked-pointer";
      this.emulatedPosition = !gamepad.pose.hasPosition;
    } else if (gamepad.hand === "") {
      this.targetRayMode = "gaze";
      this.emulatedPosition = false;
    }
  }
  updateBasePoseMatrix() {
    if (this.nativeGamepad && this.nativeGamepad.pose) {
      let pose = this.nativeGamepad.pose;
      let position2 = pose.position;
      let orientation = pose.orientation;
      if (!position2 && !orientation) {
        return;
      }
      if (!position2) {
        if (!pose.hasPosition) {
          if (!this.armModel) {
            this.armModel = new OrientationArmModel();
          }
          this.armModel.setHandedness(this.nativeGamepad.hand);
          this.armModel.update(orientation, this.polyfill.getBasePoseMatrix());
          position2 = this.armModel.getPosition();
        } else {
          position2 = this.lastPosition;
        }
      } else {
        this.lastPosition[0] = position2[0];
        this.lastPosition[1] = position2[1];
        this.lastPosition[2] = position2[2];
      }
      fromRotationTranslation(this.basePoseMatrix, orientation, position2);
    } else {
      copy2(this.basePoseMatrix, this.polyfill.getBasePoseMatrix());
    }
    return this.basePoseMatrix;
  }
  getXRPose(coordinateSystem, poseType) {
    this.updateBasePoseMatrix();
    switch (poseType) {
      case "target-ray":
        coordinateSystem._transformBasePoseMatrix(this.outputMatrix, this.basePoseMatrix);
        if (this.gamepad && this.gamepad[PRIVATE19].targetRayTransform) {
          multiply2(this.outputMatrix, this.outputMatrix, this.gamepad[PRIVATE19].targetRayTransform);
        }
        break;
      case "grip":
        if (!this.nativeGamepad || !this.nativeGamepad.pose) {
          return null;
        }
        coordinateSystem._transformBasePoseMatrix(this.outputMatrix, this.basePoseMatrix);
        if (this.gamepad && this.gamepad[PRIVATE19].gripTransform) {
          multiply2(this.outputMatrix, this.outputMatrix, this.gamepad[PRIVATE19].gripTransform);
        }
        break;
      default:
        return null;
    }
    coordinateSystem._adjustForOriginOffset(this.outputMatrix);
    return new XRPose(new XRRigidTransform(this.outputMatrix), this.emulatedPosition);
  }
};

// ../../node_modules/webxr-polyfill/src/devices/WebVRDevice.js
var PRIVATE20 = Symbol("@@webxr-polyfill/WebVRDevice");
var TEST_ENV = false;
var EXTRA_PRESENTATION_ATTRIBUTES = {
  highRefreshRate: true
};
var PRIMARY_BUTTON_MAP = {
  oculus: 1,
  openvr: 1,
  "spatial controller (spatial interaction source)": 1
};
var SESSION_ID = 0;
var Session = class {
  constructor(mode, enabledFeatures, polyfillOptions = {}) {
    this.mode = mode;
    this.enabledFeatures = enabledFeatures;
    this.outputContext = null;
    this.immersive = mode == "immersive-vr" || mode == "immersive-ar";
    this.ended = null;
    this.baseLayer = null;
    this.id = ++SESSION_ID;
    this.modifiedCanvasLayer = false;
    if (this.outputContext && !TEST_ENV) {
      const renderContextType = polyfillOptions.renderContextType || "2d";
      this.renderContext = this.outputContext.canvas.getContext(renderContextType);
    }
  }
};
var WebVRDevice = class extends XRDevice {
  constructor(global2, display2) {
    const { canPresent } = display2.capabilities;
    super(global2);
    this.display = display2;
    this.frame = new global2.VRFrameData();
    this.sessions = /* @__PURE__ */ new Map();
    this.immersiveSession = null;
    this.canPresent = canPresent;
    this.baseModelMatrix = create3();
    this.gamepadInputSources = {};
    this.tempVec3 = new Float32Array(3);
    this.onVRDisplayPresentChange = this.onVRDisplayPresentChange.bind(this);
    global2.window.addEventListener("vrdisplaypresentchange", this.onVRDisplayPresentChange);
    this.CAN_USE_GAMEPAD = global2.navigator && "getGamepads" in global2.navigator;
    this.HAS_BITMAP_SUPPORT = isImageBitmapSupported(global2);
  }
  get depthNear() {
    return this.display.depthNear;
  }
  set depthNear(val) {
    this.display.depthNear = val;
  }
  get depthFar() {
    return this.display.depthFar;
  }
  set depthFar(val) {
    this.display.depthFar = val;
  }
  onBaseLayerSet(sessionId, layer) {
    const session = this.sessions.get(sessionId);
    const canvas = layer.context.canvas;
    if (session.immersive) {
      const left2 = this.display.getEyeParameters("left");
      const right = this.display.getEyeParameters("right");
      canvas.width = Math.max(left2.renderWidth, right.renderWidth) * 2;
      canvas.height = Math.max(left2.renderHeight, right.renderHeight);
      this.display.requestPresent([{
        source: canvas,
        attributes: EXTRA_PRESENTATION_ATTRIBUTES
      }]).then(() => {
        if (!TEST_ENV && !this.global.document.body.contains(canvas)) {
          session.modifiedCanvasLayer = true;
          this.global.document.body.appendChild(canvas);
          applyCanvasStylesForMinimalRendering(canvas);
        }
        session.baseLayer = layer;
      });
    } else {
      session.baseLayer = layer;
    }
  }
  isSessionSupported(mode) {
    if (mode == "immersive-ar") {
      return false;
    }
    if (mode == "immersive-vr" && this.canPresent === false) {
      return false;
    }
    return true;
  }
  isFeatureSupported(featureDescriptor) {
    switch (featureDescriptor) {
      case "viewer":
        return true;
      case "local":
        return true;
      case "local-floor":
        return true;
      case "bounded":
        return false;
      case "unbounded":
        return false;
      default:
        return false;
    }
  }
  async requestSession(mode, enabledFeatures) {
    if (!this.isSessionSupported(mode)) {
      return Promise.reject();
    }
    let immersive = mode == "immersive-vr";
    if (immersive) {
      const canvas = this.global.document.createElement("canvas");
      if (!TEST_ENV) {
        const ctx = canvas.getContext("webgl");
      }
      await this.display.requestPresent([{
        source: canvas,
        attributes: EXTRA_PRESENTATION_ATTRIBUTES
      }]);
    }
    const session = new Session(mode, enabledFeatures, {
      renderContextType: this.HAS_BITMAP_SUPPORT ? "bitmaprenderer" : "2d"
    });
    this.sessions.set(session.id, session);
    if (immersive) {
      this.immersiveSession = session;
      this.dispatchEvent("@@webxr-polyfill/vr-present-start", session.id);
    }
    return Promise.resolve(session.id);
  }
  requestAnimationFrame(callback) {
    return this.display.requestAnimationFrame(callback);
  }
  getPrimaryButtonIndex(gamepad) {
    let primaryButton = 0;
    let name2 = gamepad.id.toLowerCase();
    for (let key in PRIMARY_BUTTON_MAP) {
      if (name2.includes(key)) {
        primaryButton = PRIMARY_BUTTON_MAP[key];
        break;
      }
    }
    return Math.min(primaryButton, gamepad.buttons.length - 1);
  }
  onFrameStart(sessionId, renderState) {
    this.display.depthNear = renderState.depthNear;
    this.display.depthFar = renderState.depthFar;
    this.display.getFrameData(this.frame);
    const session = this.sessions.get(sessionId);
    if (session.immersive && this.CAN_USE_GAMEPAD) {
      let prevInputSources = this.gamepadInputSources;
      this.gamepadInputSources = {};
      let gamepads = this.global.navigator.getGamepads();
      for (let i = 0; i < gamepads.length; ++i) {
        let gamepad = gamepads[i];
        if (gamepad && gamepad.displayId > 0) {
          let inputSourceImpl = prevInputSources[i];
          if (!inputSourceImpl) {
            inputSourceImpl = new GamepadXRInputSource(this, this.display, this.getPrimaryButtonIndex(gamepad));
          }
          inputSourceImpl.updateFromGamepad(gamepad);
          this.gamepadInputSources[i] = inputSourceImpl;
          if (inputSourceImpl.primaryButtonIndex != -1) {
            let primaryActionPressed = gamepad.buttons[inputSourceImpl.primaryButtonIndex].pressed;
            if (primaryActionPressed && !inputSourceImpl.primaryActionPressed) {
              this.dispatchEvent("@@webxr-polyfill/input-select-start", { sessionId: session.id, inputSource: inputSourceImpl.inputSource });
            } else if (!primaryActionPressed && inputSourceImpl.primaryActionPressed) {
              this.dispatchEvent("@@webxr-polyfill/input-select-end", { sessionId: session.id, inputSource: inputSourceImpl.inputSource });
            }
            inputSourceImpl.primaryActionPressed = primaryActionPressed;
          }
          if (inputSourceImpl.primarySqueezeButtonIndex != -1) {
            let primarySqueezeActionPressed = gamepad.buttons[inputSourceImpl.primarySqueezeButtonIndex].pressed;
            if (primarySqueezeActionPressed && !inputSourceImpl.primarySqueezeActionPressed) {
              this.dispatchEvent("@@webxr-polyfill/input-squeeze-start", { sessionId: session.id, inputSource: inputSourceImpl.inputSource });
            } else if (!primarySqueezeActionPressed && inputSourceImpl.primarySqueezeActionPressed) {
              this.dispatchEvent("@@webxr-polyfill/input-squeeze-end", { sessionId: session.id, inputSource: inputSourceImpl.inputSource });
            }
            inputSourceImpl.primarySqueezeActionPressed = primarySqueezeActionPressed;
          }
        }
      }
    }
    if (TEST_ENV) {
      return;
    }
    if (!session.immersive && session.baseLayer) {
      const canvas = session.baseLayer.context.canvas;
      perspective(this.frame.leftProjectionMatrix, renderState.inlineVerticalFieldOfView, canvas.width / canvas.height, renderState.depthNear, renderState.depthFar);
    }
  }
  onFrameEnd(sessionId) {
    const session = this.sessions.get(sessionId);
    if (session.ended || !session.baseLayer) {
      return;
    }
    if (session.outputContext && !(session.immersive && !this.display.capabilities.hasExternalDisplay)) {
      const mirroring = session.immersive && this.display.capabilities.hasExternalDisplay;
      const iCanvas = session.baseLayer.context.canvas;
      const iWidth = mirroring ? iCanvas.width / 2 : iCanvas.width;
      const iHeight = iCanvas.height;
      if (!TEST_ENV) {
        const oCanvas = session.outputContext.canvas;
        const oWidth = oCanvas.width;
        const oHeight = oCanvas.height;
        const renderContext = session.renderContext;
        if (this.HAS_BITMAP_SUPPORT) {
          if (iCanvas.transferToImageBitmap) {
            renderContext.transferFromImageBitmap(iCanvas.transferToImageBitmap());
          } else {
            this.global.createImageBitmap(iCanvas, 0, 0, iWidth, iHeight, {
              resizeWidth: oWidth,
              resizeHeight: oHeight
            }).then((bitmap) => renderContext.transferFromImageBitmap(bitmap));
          }
        } else {
          renderContext.drawImage(iCanvas, 0, 0, iWidth, iHeight, 0, 0, oWidth, oHeight);
        }
      }
    }
    if (session.immersive && session.baseLayer) {
      this.display.submitFrame();
    }
  }
  cancelAnimationFrame(handle) {
    this.display.cancelAnimationFrame(handle);
  }
  async endSession(sessionId) {
    const session = this.sessions.get(sessionId);
    if (session.ended) {
      return;
    }
    if (session.immersive) {
      return this.display.exitPresent();
    } else {
      session.ended = true;
    }
  }
  doesSessionSupportReferenceSpace(sessionId, type2) {
    const session = this.sessions.get(sessionId);
    if (session.ended) {
      return false;
    }
    return session.enabledFeatures.has(type2);
  }
  requestStageBounds() {
    if (this.display.stageParameters) {
      const width2 = this.display.stageParameters.sizeX;
      const depth = this.display.stageParameters.sizeZ;
      const data = [];
      data.push(-width2 / 2);
      data.push(-depth / 2);
      data.push(width2 / 2);
      data.push(-depth / 2);
      data.push(width2 / 2);
      data.push(depth / 2);
      data.push(-width2 / 2);
      data.push(depth / 2);
      return data;
    }
    return null;
  }
  async requestFrameOfReferenceTransform(type2, options) {
    if ((type2 === "local-floor" || type2 === "bounded-floor") && this.display.stageParameters && this.display.stageParameters.sittingToStandingTransform) {
      return this.display.stageParameters.sittingToStandingTransform;
    }
    return null;
  }
  getProjectionMatrix(eye) {
    if (eye === "left") {
      return this.frame.leftProjectionMatrix;
    } else if (eye === "right") {
      return this.frame.rightProjectionMatrix;
    } else if (eye === "none") {
      return this.frame.leftProjectionMatrix;
    } else {
      throw new Error(`eye must be of type 'left' or 'right'`);
    }
  }
  getViewport(sessionId, eye, layer, target) {
    const session = this.sessions.get(sessionId);
    const { width: width2, height: height2 } = layer.context.canvas;
    if (!session.immersive) {
      target.x = target.y = 0;
      target.width = width2;
      target.height = height2;
      return true;
    }
    if (eye === "left" || eye === "none") {
      target.x = 0;
    } else if (eye === "right") {
      target.x = width2 / 2;
    } else {
      return false;
    }
    target.y = 0;
    target.width = width2 / 2;
    target.height = height2;
    return true;
  }
  getBasePoseMatrix() {
    let { position: position2, orientation } = this.frame.pose;
    if (!position2 && !orientation) {
      return this.baseModelMatrix;
    }
    if (!position2) {
      position2 = this.tempVec3;
      position2[0] = position2[1] = position2[2] = 0;
    }
    fromRotationTranslation(this.baseModelMatrix, orientation, position2);
    return this.baseModelMatrix;
  }
  getBaseViewMatrix(eye) {
    if (eye === "left" || eye === "none") {
      return this.frame.leftViewMatrix;
    } else if (eye === "right") {
      return this.frame.rightViewMatrix;
    } else {
      throw new Error(`eye must be of type 'left' or 'right'`);
    }
  }
  getInputSources() {
    let inputSources = [];
    for (let i in this.gamepadInputSources) {
      inputSources.push(this.gamepadInputSources[i].inputSource);
    }
    return inputSources;
  }
  getInputPose(inputSource, coordinateSystem, poseType) {
    if (!coordinateSystem) {
      return null;
    }
    for (let i in this.gamepadInputSources) {
      let inputSourceImpl = this.gamepadInputSources[i];
      if (inputSourceImpl.inputSource === inputSource) {
        return inputSourceImpl.getXRPose(coordinateSystem, poseType);
      }
    }
    return null;
  }
  onWindowResize() {
  }
  onVRDisplayPresentChange(e2) {
    if (!this.display.isPresenting) {
      this.sessions.forEach((session) => {
        if (session.immersive && !session.ended) {
          if (session.modifiedCanvasLayer) {
            const canvas = session.baseLayer.context.canvas;
            document.body.removeChild(canvas);
            canvas.setAttribute("style", "");
          }
          if (this.immersiveSession === session) {
            this.immersiveSession = null;
          }
          this.dispatchEvent("@@webxr-polyfill/vr-present-end", session.id);
        }
      });
    }
  }
};

// ../../node_modules/webxr-polyfill/src/devices/CardboardXRDevice.js
var CardboardXRDevice = class extends WebVRDevice {
  constructor(global2, cardboardConfig) {
    const display2 = new import_cardboard_vr_display.default(cardboardConfig || {});
    super(global2, display2);
    this.display = display2;
    this.frame = {
      rightViewMatrix: new Float32Array(16),
      leftViewMatrix: new Float32Array(16),
      rightProjectionMatrix: new Float32Array(16),
      leftProjectionMatrix: new Float32Array(16),
      pose: null,
      timestamp: null
    };
  }
};

// ../../node_modules/webxr-polyfill/src/devices/InlineDevice.js
var TEST_ENV2 = false;
var SESSION_ID2 = 0;
var Session2 = class {
  constructor(mode, enabledFeatures) {
    this.mode = mode;
    this.enabledFeatures = enabledFeatures;
    this.ended = null;
    this.baseLayer = null;
    this.id = ++SESSION_ID2;
  }
};
var InlineDevice = class extends XRDevice {
  constructor(global2) {
    super(global2);
    this.sessions = /* @__PURE__ */ new Map();
    this.projectionMatrix = create3();
    this.identityMatrix = create3();
  }
  onBaseLayerSet(sessionId, layer) {
    const session = this.sessions.get(sessionId);
    session.baseLayer = layer;
  }
  isSessionSupported(mode) {
    return mode == "inline";
  }
  isFeatureSupported(featureDescriptor) {
    switch (featureDescriptor) {
      case "viewer":
        return true;
      default:
        return false;
    }
  }
  async requestSession(mode, enabledFeatures) {
    if (!this.isSessionSupported(mode)) {
      return Promise.reject();
    }
    const session = new Session2(mode, enabledFeatures);
    this.sessions.set(session.id, session);
    return Promise.resolve(session.id);
  }
  requestAnimationFrame(callback) {
    return window.requestAnimationFrame(callback);
  }
  cancelAnimationFrame(handle) {
    window.cancelAnimationFrame(handle);
  }
  onFrameStart(sessionId, renderState) {
    if (TEST_ENV2) {
      return;
    }
    const session = this.sessions.get(sessionId);
    if (session.baseLayer) {
      const canvas = session.baseLayer.context.canvas;
      perspective(this.projectionMatrix, renderState.inlineVerticalFieldOfView, canvas.width / canvas.height, renderState.depthNear, renderState.depthFar);
    }
  }
  onFrameEnd(sessionId) {
  }
  async endSession(sessionId) {
    const session = this.sessions.get(sessionId);
    session.ended = true;
  }
  doesSessionSupportReferenceSpace(sessionId, type2) {
    const session = this.sessions.get(sessionId);
    if (session.ended) {
      return false;
    }
    return session.enabledFeatures.has(type2);
  }
  requestStageBounds() {
    return null;
  }
  async requestFrameOfReferenceTransform(type2, options) {
    return null;
  }
  getProjectionMatrix(eye) {
    return this.projectionMatrix;
  }
  getViewport(sessionId, eye, layer, target) {
    const session = this.sessions.get(sessionId);
    const { width: width2, height: height2 } = layer.context.canvas;
    target.x = target.y = 0;
    target.width = width2;
    target.height = height2;
    return true;
  }
  getBasePoseMatrix() {
    return this.identityMatrix;
  }
  getBaseViewMatrix(eye) {
    return this.identityMatrix;
  }
  getInputSources() {
    return [];
  }
  getInputPose(inputSource, coordinateSystem, poseType) {
    return null;
  }
  onWindowResize() {
  }
};

// ../../node_modules/webxr-polyfill/src/devices.js
var getWebVRDevice = async function(global2) {
  let device = null;
  if ("getVRDisplays" in global2.navigator) {
    try {
      const displays = await global2.navigator.getVRDisplays();
      if (displays && displays.length) {
        device = new WebVRDevice(global2, displays[0]);
      }
    } catch (e2) {
    }
  }
  return device;
};
var requestXRDevice = async function(global2, config) {
  if (config.webvr) {
    let xr = await getWebVRDevice(global2);
    if (xr) {
      return xr;
    }
  }
  let mobile = isMobile2(global2);
  if (mobile && config.cardboard || !mobile && config.allowCardboardOnDesktop) {
    if (!global2.VRFrameData) {
      global2.VRFrameData = function() {
        this.rightViewMatrix = new Float32Array(16);
        this.leftViewMatrix = new Float32Array(16);
        this.rightProjectionMatrix = new Float32Array(16);
        this.leftProjectionMatrix = new Float32Array(16);
        this.pose = null;
      };
    }
    return new CardboardXRDevice(global2, config.cardboardConfig);
  }
  return new InlineDevice(global2);
};

// ../../node_modules/webxr-polyfill/src/WebXRPolyfill.js
var CONFIG_DEFAULTS = {
  global: global_default,
  webvr: true,
  cardboard: true,
  cardboardConfig: null,
  allowCardboardOnDesktop: false
};
var partials = ["navigator", "HTMLCanvasElement", "WebGLRenderingContext"];
var WebXRPolyfill = class {
  constructor(config = {}) {
    this.config = Object.freeze(Object.assign({}, CONFIG_DEFAULTS, config));
    this.global = this.config.global;
    this.nativeWebXR = "xr" in this.global.navigator;
    this.injected = false;
    if (!this.nativeWebXR) {
      this._injectPolyfill(this.global);
    } else {
      this._injectCompatibilityShims(this.global);
    }
  }
  _injectPolyfill(global2) {
    if (!partials.every((iface) => !!global2[iface])) {
      throw new Error(`Global must have the following attributes : ${partials}`);
    }
    for (const className2 of Object.keys(api_default)) {
      if (global2[className2] !== void 0) {
        console.warn(`${className2} already defined on global.`);
      } else {
        global2[className2] = api_default[className2];
      }
    }
    if (true) {
      const polyfilledCtx = polyfillMakeXRCompatible(global2.WebGLRenderingContext);
      if (polyfilledCtx) {
        polyfillGetContext(global2.HTMLCanvasElement);
        if (global2.OffscreenCanvas) {
          polyfillGetContext(global2.OffscreenCanvas);
        }
        if (global2.WebGL2RenderingContext) {
          polyfillMakeXRCompatible(global2.WebGL2RenderingContext);
        }
        if (!window.isSecureContext) {
          console.warn(`WebXR Polyfill Warning:
This page is not running in a secure context (https:// or localhost)!
This means that although the page may be able to use the WebXR Polyfill it will
not be able to use native WebXR implementations, and as such will not be able to
access dedicated VR or AR hardware, and will not be able to take advantage of
any performance improvements a native WebXR implementation may offer. Please
host this content on a secure origin for the best user experience.
`);
        }
      }
    }
    this.injected = true;
    this._patchNavigatorXR();
  }
  _patchNavigatorXR() {
    let devicePromise = requestXRDevice(this.global, this.config);
    this.xr = new api_default.XRSystem(devicePromise);
    Object.defineProperty(this.global.navigator, "xr", {
      value: this.xr,
      configurable: true
    });
  }
  _injectCompatibilityShims(global2) {
    if (!partials.every((iface) => !!global2[iface])) {
      throw new Error(`Global must have the following attributes : ${partials}`);
    }
    if (global2.navigator.xr && "supportsSession" in global2.navigator.xr && !("isSessionSupported" in global2.navigator.xr)) {
      let originalSupportsSession = global2.navigator.xr.supportsSession;
      global2.navigator.xr.isSessionSupported = function(mode) {
        return originalSupportsSession.call(this, mode).then(() => {
          return true;
        }).catch(() => {
          return false;
        });
      };
      global2.navigator.xr.supportsSession = function(mode) {
        console.warn("navigator.xr.supportsSession() is deprecated. Please call navigator.xr.isSessionSupported() instead and check the boolean value returned when the promise resolves.");
        return originalSupportsSession.call(this, mode);
      };
    }
  }
};

// ../threejs/ScreenControl.ts
if (!navigator.xr) {
  console.info("Polyfilling WebXR API");
  new WebXRPolyfill();
}
var XRSessionToggleEvent = class extends TypedEvent {
  constructor(type2, mode, session, referenceSpaceType, sessionMode) {
    super(type2);
    this.mode = mode;
    this.session = session;
    this.referenceSpaceType = referenceSpaceType;
    this.sessionMode = sessionMode;
  }
};
var XRSessionStartedEvent = class extends XRSessionToggleEvent {
  constructor(mode, session, referenceSpaceType, sessionMode) {
    super("sessionstarted", mode, session, referenceSpaceType, sessionMode);
  }
};
var XRSessionStoppedEvent = class extends XRSessionToggleEvent {
  constructor(mode, session, referenceSpaceType, sessionMode) {
    super("sessionstopped", mode, session, referenceSpaceType, sessionMode);
  }
};
var xrModes = /* @__PURE__ */ new Map([
  ["VR" /* VR */, {
    referenceSpaceType: "local-floor",
    sessionMode: "immersive-vr"
  }],
  ["AR" /* AR */, {
    referenceSpaceType: "local-floor",
    sessionMode: "immersive-ar"
  }]
]);
var ScreenControl = class extends TypedEventBase {
  constructor(renderer, camera, fullscreenElement, enableFullResolution) {
    super();
    this.renderer = renderer;
    this.camera = camera;
    this.fullscreenElement = fullscreenElement;
    this.enableFullResolution = enableFullResolution;
    this._currentMode = "None" /* None */;
    this.buttons = /* @__PURE__ */ new Map();
    this.currentSession = null;
    this.screenUI = null;
    this.wasVisible = /* @__PURE__ */ new Map();
    this.lastWidth = 0;
    this.lastHeight = 0;
    this.addEventListener("sessionstarted", (evt) => {
      if (evt.sessionMode === "inline") {
        this.camera.fov = evt.session.renderState.inlineVerticalFieldOfView * 180 / Math.PI;
      }
    });
    this.addEventListener("sessionstopped", (evt) => {
      if (evt.sessionMode === "inline") {
        this.camera.fov = 50;
      }
    });
    this.renderer.xr.addEventListener("sessionstart", () => this.onSessionStarted());
    this.renderer.xr.addEventListener("sessionend", () => this.onSessionEnded());
    this.refresh();
  }
  setUI(screenUI, fullscreenButton, vrButton, arButton) {
    this.screenUI = screenUI;
    this.buttons.set(fullscreenButton.mode, fullscreenButton);
    this.buttons.set(vrButton.mode, vrButton);
    if (arButton) {
      this.buttons.set(arButton.mode, arButton);
      arButton.available = hasWebXR();
    }
    for (const button of this.buttons.values()) {
      this.wasVisible.set(button, button.visible);
      button.addEventListener("click", this.toggleMode.bind(this, button.mode));
    }
    fullscreenButton.available = !isMobileVR() && hasFullscreenAPI();
    vrButton.available = hasVR();
  }
  get visible() {
    return elementIsDisplayed(this.renderer.domElement);
  }
  set visible(v) {
    elementSetDisplay(this.renderer.domElement, v);
    if (this.screenUI) {
      elementSetDisplay(this.screenUI, v, "grid");
    }
  }
  get currentMode() {
    return this._currentMode;
  }
  resize() {
    if (!this.renderer.xr.isPresenting) {
      const width2 = this.renderer.domElement.clientWidth;
      const height2 = this.renderer.domElement.clientHeight;
      if (width2 > 0 && height2 > 0 && (width2 !== this.lastWidth || height2 !== this.lastHeight)) {
        this.renderer.setPixelRatio(devicePixelRatio);
        this.renderer.setSize(width2, height2, false);
        this.camera.aspect = width2 / height2;
        this.camera.updateProjectionMatrix();
        this.lastWidth = width2;
        this.lastHeight = height2;
      }
    }
  }
  getMetrics() {
    const width2 = this.renderer.domElement.clientWidth;
    const height2 = this.renderer.domElement.clientHeight;
    const pixelRatio = this.renderer.getPixelRatio();
    const fov = this.camera.fov;
    return { width: width2, height: height2, pixelRatio, fov };
  }
  setMetrics(width2, height2, pixelRatio, fov) {
    this.renderer.setPixelRatio(pixelRatio);
    this.renderer.setSize(width2, height2, false);
    this.camera.aspect = width2 / height2;
    this.camera.fov = fov;
    this.camera.updateProjectionMatrix();
  }
  async refresh() {
    await Promise.all(Array.from(this.buttons.values()).filter((btn) => btn.available && btn.mode !== "Fullscreen" /* Fullscreen */).map(async (btn) => {
      const xrMode = xrModes.get(btn.mode);
      btn.available = isDefined(xrMode);
      if (btn.available) {
        const typeSupported = navigator.xr && await navigator.xr.isSessionSupported(xrMode.sessionMode);
        const webVROverride = !hasWebXR() && hasWebVR() && xrMode.sessionMode === "immersive-vr" && xrMode.referenceSpaceType === "local-floor";
        btn.available = typeSupported || webVROverride;
      }
    }));
  }
  async toggleMode(mode) {
    if (mode === "Fullscreen" /* Fullscreen */) {
      await this.toggleFullscreen();
    } else if (mode !== "None" /* None */) {
      await this.toggleXR(mode);
    }
  }
  async start(mode) {
    if (mode !== this.currentMode) {
      await this.toggleMode(this.currentMode);
      await this.toggleMode(mode);
    }
  }
  async stop() {
    await this.toggleMode(this.currentMode);
  }
  get isFullscreen() {
    return document.fullscreen;
  }
  async startFullscreen() {
    if (!this.isFullscreen) {
      await this.fullscreenElement.requestFullscreen({
        navigationUI: "show"
      });
      this.setActive("Fullscreen" /* Fullscreen */);
      this.dispatchEvent(new XRSessionStartedEvent("Fullscreen" /* Fullscreen */, null, null, null));
    }
  }
  async stopFullscreen() {
    if (this.isFullscreen) {
      await document.exitFullscreen();
      this.setActive("None" /* None */);
      this.dispatchEvent(new XRSessionStoppedEvent("Fullscreen" /* Fullscreen */, null, null, null));
    }
  }
  async toggleFullscreen() {
    if (this.isFullscreen) {
      await this.stopFullscreen();
    } else {
      await this.startFullscreen();
    }
  }
  async toggleXR(mode) {
    const xrMode = xrModes.get(mode);
    if (isDefined(xrMode)) {
      if (this.currentSession) {
        this.currentSession.end();
      } else if (navigator.xr) {
        this.camera.position.set(0, 0, 0);
        this.camera.quaternion.identity();
        try {
          const session = await navigator.xr.requestSession(xrMode.sessionMode, {
            optionalFeatures: [
              "local-floor",
              "bounded-floor",
              "high-refresh-rate",
              "hand-tracking",
              "layers"
            ]
          });
          this.setActive(mode);
          this.currentSession = session;
          this.renderer.xr.setReferenceSpaceType(xrMode.referenceSpaceType);
          if (this.enableFullResolution && "XRWebGLLayer" in window && "getNativeFramebufferScaleFactor" in XRWebGLLayer) {
            const size = XRWebGLLayer.getNativeFramebufferScaleFactor(session);
            this.renderer.xr.setFramebufferScaleFactor(size);
          }
          this.renderer.xr.setSession(session);
        } catch (exp) {
          console.error(`Couldn't start session type '${xrMode.sessionMode}'. Reason: ${exp && exp.message || exp || "UNKNOWN"}`);
        }
      }
    }
  }
  onSessionStarted() {
    const mode = this.currentMode;
    const xrMode = xrModes.get(this.currentMode);
    const session = this.currentSession;
    if (session.supportedFrameRates) {
      const max3 = Math.max(...session.supportedFrameRates);
      console.log("Changing framerate to", max3);
      session.updateTargetFrameRate(max3);
    }
    this.dispatchEvent(new XRSessionStartedEvent(mode, session, xrMode.referenceSpaceType, xrMode.sessionMode));
  }
  onSessionEnded() {
    const mode = this.currentMode;
    const xrMode = xrModes.get(this.currentMode);
    const session = this.currentSession;
    this.currentSession = null;
    this.renderer.xr.setSession(null);
    this.setActive("None" /* None */);
    this.dispatchEvent(new XRSessionStoppedEvent(mode, session, xrMode.referenceSpaceType, xrMode.sessionMode));
  }
  setActive(mode) {
    for (const button of this.buttons.values()) {
      button.active = button.mode === mode;
      button.visible = this.wasVisible.get(button) && (mode === "None" /* None */ || button.mode === mode || mode === "Fullscreen" /* Fullscreen */);
    }
    this._currentMode = mode;
  }
};

// ../threejs/Skybox.ts
var U2 = new THREE.Vector3(0, 1, 0);
var FACE_SIZE = 2048;
var FACE_SIZE_HALF = FACE_SIZE / 2;
var FACES = [
  1,
  0,
  2,
  3,
  4,
  5
];
var CUBEMAP_PATTERN = {
  rows: 3,
  columns: 4,
  indices: [
    [-1 /* None */, 2 /* Up */, -1 /* None */, -1 /* None */],
    [0 /* Left */, 5 /* Front */, 1 /* Right */, 4 /* Back */],
    [-1 /* None */, 3 /* Down */, -1 /* None */, -1 /* None */]
  ],
  rotations: [
    [0, Math.PI, 0, 0],
    [0, 0, 0, 0],
    [0, Math.PI, 0, 0]
  ]
};
var black2 = new THREE.Color(0);
var Skybox = class {
  constructor(env) {
    this.env = env;
    this.rt = new THREE.WebGLCubeRenderTarget(FACE_SIZE);
    this.rtScene = new THREE.Scene();
    this.rtCamera = new THREE.CubeCamera(0.01, 10, this.rt);
    this._rotation = new THREE.Quaternion();
    this.layerRotation = new THREE.Quaternion().identity();
    this.stageRotation = new THREE.Quaternion().identity();
    this.images = null;
    this.canvases = new Array(6);
    this.contexts = new Array(6);
    this.curImagePath = null;
    this.layer = null;
    this.wasVisible = false;
    this.wasWebXRLayerAvailable = null;
    this.stageHeading = 0;
    this.rotationNeedsUpdate = false;
    this.imageNeedsUpdate = false;
    this.webXRLayerEnabled = true;
    this.visible = true;
    this.webXRLayerEnabled &&= this.env.hasXRCompositionLayers;
    this.env.scene.background = black2;
    for (let i = 0; i < this.canvases.length; ++i) {
      const f = this.canvases[i] = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
      this.contexts[i] = f.getContext("2d");
    }
    for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
      const indices = CUBEMAP_PATTERN.indices[row];
      const rotations = CUBEMAP_PATTERN.rotations[row];
      for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
        const i = indices[column];
        if (i > -1) {
          const g = this.contexts[i];
          const rotation = rotations[column];
          if (rotation > 0) {
            if (rotation % 2 === 0) {
              g.translate(FACE_SIZE_HALF, FACE_SIZE_HALF);
            } else {
              g.translate(FACE_SIZE_HALF, FACE_SIZE_HALF);
            }
            g.rotate(rotation);
            g.translate(-FACE_SIZE_HALF, -FACE_SIZE_HALF);
          }
        }
      }
    }
    this.rt.texture.name = "SkyboxOutput";
    this.rtScene.add(this.rtCamera);
    this.flipped = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
    this.flipper = this.flipped.getContext("2d");
    this.flipper.fillStyle = black2.getHexString();
    this.flipper.scale(-1, 1);
    this.flipper.translate(-FACE_SIZE, 0);
    this.setImages("", this.canvases);
    Object.seal(this);
  }
  setImage(imageID, image2) {
    if (imageID !== this.curImagePath) {
      const width2 = image2.width / CUBEMAP_PATTERN.columns;
      const height2 = image2.height / CUBEMAP_PATTERN.rows;
      for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
        const indices = CUBEMAP_PATTERN.indices[row];
        for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
          const i = indices[column];
          if (i > -1) {
            const g = this.contexts[i];
            g.drawImage(image2, column * width2, row * height2, width2, height2, 0, 0, FACE_SIZE, FACE_SIZE);
          }
        }
      }
      this.setImages(imageID, this.canvases);
    }
  }
  setImages(imageID, images) {
    if (imageID !== this.curImagePath || images !== this.images) {
      this.curImagePath = imageID;
      if (images !== this.images) {
        if (isDefined(this.cube)) {
          cleanup(this.cube);
        }
        this.images = images;
        this.rtScene.background = this.cube = new THREE.CubeTexture(this.images);
        this.cube.name = "SkyboxInput";
      }
    }
    this.updateImages();
  }
  updateImages() {
    this.cube.needsUpdate = true;
    this.imageNeedsUpdate = true;
  }
  get rotation() {
    return this._rotation;
  }
  set rotation(rotation) {
    const { x, y, z, w } = this._rotation;
    if (isQuaternion(rotation)) {
      this._rotation.copy(rotation);
    } else if (isEuler(rotation)) {
      this._rotation.setFromEuler(rotation);
    } else if (isArray(rotation)) {
      if (rotation.length === 4 && isNumber(rotation[0]) && isNumber(rotation[1]) && isNumber(rotation[2]) && isNumber(rotation[3])) {
        this._rotation.fromArray(rotation);
      } else {
        throw new Error("Skybox rotation was not a valid array format. Needs an array of 4 numbers.");
      }
    } else if (isGoodNumber(rotation)) {
      this._rotation.setFromAxisAngle(U2, rotation);
    } else {
      if (isDefined(rotation)) {
        console.warn("Skybox rotation must be a THREE.Quaternion, THREE.Euler, number[] (representing a Quaternion), or a number (representing rotation about the Y-axis).");
      }
      this._rotation.identity();
    }
    this.rotationNeedsUpdate = this._rotation.x !== x || this._rotation.y !== y || this._rotation.z !== z || this._rotation.w !== w;
  }
  update(frame) {
    if (this.cube) {
      const isWebXRLayerAvailable = this.webXRLayerEnabled && this.env.renderer.xr.isPresenting && isDefined(frame) && isDefined(this.env.xrBinding);
      if (isWebXRLayerAvailable !== this.wasWebXRLayerAvailable) {
        if (isWebXRLayerAvailable) {
          const space = this.env.renderer.xr.getReferenceSpace();
          this.layer = this.env.xrBinding.createCubeLayer({
            space,
            layout: "mono",
            isStatic: false,
            viewPixelWidth: FACE_SIZE,
            viewPixelHeight: FACE_SIZE
          });
          this.env.addWebXRLayer(this.layer, Number.MAX_VALUE);
        } else if (this.layer) {
          this.env.removeWebXRLayer(this.layer);
          this.layer.destroy();
          this.layer = null;
        }
        this.imageNeedsUpdate = this.rotationNeedsUpdate = true;
      }
      this.env.scene.background = this.layer ? null : this.visible ? this.rt.texture : black2;
      if (this.layer) {
        if (this.visible !== this.wasVisible || this.layer.needsRedraw) {
          this.imageNeedsUpdate = true;
        }
        if (this.env.avatar.heading !== this.stageHeading) {
          this.rotationNeedsUpdate = true;
          this.stageHeading = this.env.avatar.heading;
          this.stageRotation.setFromAxisAngle(U2, this.env.avatar.heading);
        }
      } else {
        this.rotationNeedsUpdate = this.imageNeedsUpdate = this.imageNeedsUpdate || this.rotationNeedsUpdate;
      }
      if (this.rotationNeedsUpdate) {
        this.layerRotation.copy(this.rotation).invert();
        if (this.layer) {
          this.layerRotation.multiply(this.stageRotation);
          this.layer.orientation = new DOMPointReadOnly(this.layerRotation.x, this.layerRotation.y, this.layerRotation.z, this.layerRotation.w);
        } else {
          this.rtCamera.quaternion.copy(this.layerRotation);
        }
      }
      if (this.imageNeedsUpdate) {
        if (this.layer) {
          const gl = this.env.renderer.getContext();
          const gLayer = this.env.xrBinding.getSubImage(this.layer, frame);
          const imgs = this.cube.images;
          this.flipper.fillRect(0, 0, FACE_SIZE, FACE_SIZE);
          gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
          gl.bindTexture(gl.TEXTURE_CUBE_MAP, gLayer.colorTexture);
          for (let i = 0; i < imgs.length; ++i) {
            if (this.visible) {
              const img = imgs[FACES[i]];
              this.flipper.drawImage(img, 0, 0, img.width, img.height, 0, 0, FACE_SIZE, FACE_SIZE);
            }
            gl.texSubImage2D(gl.TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, 0, 0, gl.RGBA, gl.UNSIGNED_BYTE, this.flipped);
          }
          gl.bindTexture(gl.TEXTURE_CUBE_MAP, null);
        } else {
          this.rtCamera.update(this.env.renderer, this.rtScene);
        }
      }
      this.imageNeedsUpdate = false;
      this.rotationNeedsUpdate = false;
      this.wasVisible = this.visible;
      this.wasWebXRLayerAvailable = isWebXRLayerAvailable;
    }
  }
};

// ../threejs/environment/XRTimer.ts
var XRTimerTickEvent = class extends BaseTimerTickEvent {
  constructor() {
    super();
    this.frame = null;
    Object.seal(this);
  }
  set(t2, dt, frame) {
    super.set(t2, dt);
    this.frame = frame;
  }
};
var XRTimer = class {
  constructor(renderer) {
    this.renderer = renderer;
    this.tickHandlers = new Array();
    this.lt = -1;
    this._isRunning = false;
    const tickEvt = new XRTimerTickEvent();
    let dt = 0;
    this._onTick = (t2, frame) => {
      if (this.lt >= 0) {
        dt = t2 - this.lt;
        tickEvt.set(t2, dt, frame);
        this.tick(tickEvt);
      }
      this.lt = t2;
    };
  }
  get isRunning() {
    return this._isRunning;
  }
  restart() {
    this.stop();
    this.start();
  }
  addTickHandler(onTick) {
    this.tickHandlers.push(onTick);
  }
  removeTickHandler(onTick) {
    arrayRemove(this.tickHandlers, onTick);
  }
  setAnimationLoop(loop2) {
    this.renderer.setAnimationLoop(loop2);
    this._isRunning = isDefined(loop2);
  }
  start() {
    if (!this.isRunning) {
      this.setAnimationLoop(this._onTick);
    }
  }
  stop() {
    if (this.isRunning) {
      this.setAnimationLoop(null);
      this.lt = -1;
    }
  }
  tick(evt) {
    for (const handler of this.tickHandlers) {
      handler(evt);
    }
  }
};

// ../threejs/environment/BaseEnvironment.ts
var spectator = new THREE.PerspectiveCamera();
var lastViewport = new THREE.Vector4();
var curViewport = new THREE.Vector4();
var gridWidth = 15;
var gridSize = feet2Meters(gridWidth);
Style(rule("#frontBuffer", position("absolute"), left(0), top(0), width("100%"), height("100%"), margin(0), padding(0), border(0), touchAction("none")));
var BaseEnvironment = class extends TypedEventBase {
  constructor(canvas, fetcher, defaultAvatarHeight, enableFullResolution) {
    super();
    this.fetcher = fetcher;
    this.defaultAvatarHeight = defaultAvatarHeight;
    this.layers = new Array();
    this.layerSortOrder = /* @__PURE__ */ new Map();
    this.fadeDepth = 0;
    this.cursor3D = new Cursor3D();
    this.camera = new THREE.PerspectiveCamera(50, 1, 0.01, 1e3);
    this.scene = new THREE.Scene();
    this.stage = obj("Stage");
    this.ambient = new THREE.AmbientLight(16777215, 0.5);
    this.sun = new THREE.DirectionalLight(16777215, 0.75);
    this.ground = new THREE.GridHelper(gridSize, gridWidth, 12632256, 8421504);
    this.foreground = obj("Foreground");
    this.loadingBar = new LoadingBar();
    this._xrBinding = null;
    this._xrMediaBinding = null;
    this._hasXRMediaLayers = null;
    this._hasXRCompositionLayers = null;
    if (isHTMLCanvas(canvas)) {
      canvas.style.backgroundColor = "black";
    }
    this.renderer = new THREE.WebGLRenderer({
      canvas,
      powerPreference: "high-performance",
      precision: "lowp",
      antialias: true,
      alpha: true,
      premultipliedAlpha: true,
      depth: true,
      logarithmicDepthBuffer: true,
      stencil: false,
      preserveDrawingBuffer: false
    });
    this.renderer.domElement.tabIndex = 1;
    this.cameraControl = new CameraControl(this.camera);
    this.screenControl = new ScreenControl(this.renderer, this.camera, this.renderer.domElement.parentElement, enableFullResolution);
    this.fader = new Fader("ViewFader");
    this.worldUISpace = new BodyFollower("WorldUISpace", 0.2, 20, 0.125);
    this.avatar = new AvatarLocal(this.renderer, this.camera, this.fader, defaultAvatarHeight);
    this.eventSystem = new EventSystem(this);
    this.skybox = new Skybox(this);
    this.timer = new XRTimer(this.renderer);
    this.renderer.xr.enabled = true;
    this.sun.name = "Sun";
    this.sun.position.set(0, 1, 1);
    this.sun.lookAt(0, 0, 0);
    this.sun.layers.enableAll();
    const showGround = () => {
      this.ground.visible = this.renderer.xr.isPresenting;
    };
    this.screenControl.addEventListener("sessionstarted", showGround);
    this.screenControl.addEventListener("sessionstopped", showGround);
    showGround();
    this.ambient.name = "Fill";
    this.ambient.layers.enableAll();
    this.loadingBar.object.name = "MainLoadingBar";
    this.loadingBar.object.position.set(0, -0.25, -2);
    this.scene.layers.enableAll();
    this.avatar.addFollower(this.worldUISpace);
    this.timer.addTickHandler((evt) => this.update(evt));
    objGraph(this.scene, this.sun, this.ambient, objGraph(this.stage, this.ground, this.camera, this.avatar, ...this.eventSystem.hands), this.foreground, objGraph(this.worldUISpace, this.loadingBar));
    this.timer.start();
    globalThis.env = this;
  }
  get gl() {
    return this.renderer.getContext();
  }
  get referenceSpace() {
    return this.renderer.xr.getReferenceSpace();
  }
  update(evt) {
    if (this.screenControl.visible) {
      const session = this.xrSession;
      this._xrBinding = this.renderer.xr.getBinding();
      if (this.hasXRMediaLayers && this._xrMediaBinding === null === this.renderer.xr.isPresenting) {
        if (this._xrMediaBinding === null && isDefined(session)) {
          this._xrMediaBinding = new XRMediaBinding(session);
          console.log("Media binding created");
        } else {
          this._xrMediaBinding = null;
        }
      }
      const baseLayer = session && this.renderer.xr.getBaseLayer();
      if (baseLayer !== this.baseLayer) {
        if (isDefined(this.baseLayer)) {
          this.removeWebXRLayer(this.baseLayer);
          this.baseLayer = null;
        }
        if (isDefined(baseLayer)) {
          this.baseLayer = baseLayer;
          this.addWebXRLayer(baseLayer, 0);
        }
      }
      this.screenControl.resize();
      this.eventSystem.update();
      this.cameraControl.update(evt.dt);
      this.avatar.update(evt.dt);
      this.worldUISpace.update(this.avatar.height, this.avatar.worldPos, this.avatar.worldHeading, evt.dt);
      this.fader.update(evt.dt);
      updateScalings(evt.dt);
      this.loadingBar.update(evt.sdt);
      this.preRender(evt);
      this.skybox.update(evt.frame);
      const cam = resolveCamera(this.renderer, this.camera);
      if (cam !== this.camera) {
        const vrCam = cam;
        vrCam.layers.mask = this.camera.layers.mask;
        for (let i = 0; i < vrCam.cameras.length; ++i) {
          const subCam = vrCam.cameras[i];
          subCam.layers.mask = this.camera.layers.mask;
          subCam.layers.enable(i + 1);
          vrCam.layers.enable(i + 1);
        }
      }
      this.renderer.clear();
      this.renderer.render(this.scene, this.camera);
      if (!this.renderer.xr.isPresenting) {
        lastViewport.copy(curViewport);
        this.renderer.getViewport(curViewport);
      } else if (isDesktop() && !isFirefox()) {
        spectator.projectionMatrix.copy(this.camera.projectionMatrix);
        spectator.position.copy(cam.position);
        spectator.quaternion.copy(cam.quaternion);
        const curRT = this.renderer.getRenderTarget();
        this.renderer.xr.isPresenting = false;
        this.renderer.setRenderTarget(null);
        this.renderer.setViewport(lastViewport);
        this.renderer.clear();
        this.renderer.render(this.scene, spectator);
        this.renderer.setViewport(curViewport);
        this.renderer.setRenderTarget(curRT);
        this.renderer.xr.isPresenting = true;
      }
    }
  }
  preRender(_evt) {
  }
  async onQuitting() {
    this.dispatchEvent(new TypedEvent("quitting"));
    window.location.href = "/";
  }
  get hasAlpha() {
    return this.renderer.getContextAttributes().alpha;
  }
  get xrSession() {
    return this.renderer.xr.getSession();
  }
  get xrBinding() {
    return this._xrBinding;
  }
  get xrMediaBinding() {
    return this._xrMediaBinding;
  }
  get isReadyForLayers() {
    return this.hasAlpha && (!isOculusBrowser || oculusBrowserVersion.major >= 15);
  }
  get hasXRMediaLayers() {
    if (this._hasXRMediaLayers === null) {
      this._hasXRMediaLayers = this.isReadyForLayers && "XRMediaBinding" in globalThis && isFunction(XRMediaBinding.prototype.createQuadLayer);
    }
    return this._hasXRMediaLayers;
  }
  get hasXRCompositionLayers() {
    if (this._hasXRCompositionLayers === null) {
      this._hasXRCompositionLayers = this.isReadyForLayers && "XRWebGLBinding" in globalThis && isFunction(XRWebGLBinding.prototype.createCubeLayer);
    }
    return this._hasXRCompositionLayers;
  }
  addWebXRLayer(layer, sortOrder) {
    this.layers.push(layer);
    this.layerSortOrder.set(layer, sortOrder);
    arraySortByKeyInPlace(this.layers, (l) => -this.layerSortOrder.get(l));
    this.updateLayers();
  }
  removeWebXRLayer(layer) {
    this.layerSortOrder.delete(layer);
    arrayRemove(this.layers, layer);
    this.updateLayers();
  }
  updateLayers() {
    const session = this.xrSession;
    if (isDefined(session)) {
      session.updateRenderState({
        layers: this.layers
      });
    }
  }
  clearScene() {
    this.dispatchEvent(new TypedEvent("sceneclearing"));
    cleanup(this.foreground);
    this.dispatchEvent(new TypedEvent("scenecleared"));
  }
  async fadeOut() {
    ++this.fadeDepth;
    if (this.fadeDepth === 1) {
      await this.fader.fadeOut();
      this.skybox.visible = false;
      this.camera.layers.set(PURGATORY);
      this.loadingBar.start();
      await this.fader.fadeIn();
    }
  }
  async fadeIn() {
    --this.fadeDepth;
    if (this.fadeDepth === 0) {
      await this.fader.fadeOut();
      this.camera.layers.set(FOREGROUND);
      this.skybox.visible = true;
      await this.fader.fadeIn();
    }
  }
  async loadModel(path, prog) {
    const loader = new GLTFLoader();
    const model = await loader.loadAsync(path, (evt) => {
      prog.report(evt.loaded, evt.total, path);
    });
    model.scene.traverse((m) => {
      if (isMesh(m)) {
        m.isCollider = true;
        const material = m.material;
        material.side = THREE.FrontSide;
        material.needsUpdate = true;
      }
    });
    return model.scene;
  }
  async load3DCursor(path, prog) {
    const model = await this.loadModel(path, prog);
    const children = model.children.slice(0);
    for (const child of children) {
      this.cursor3D.add(child.name, child);
    }
    this.eventSystem.refreshCursors();
    this.dispatchEvent(new TypedEvent("newcursorloaded"));
  }
  async load(prog) {
    await this.load3DCursor("/models/Cursors.glb", prog);
  }
};

// ../webrtc/ActivityDetector.ts
var ActivityDetector = class {
  constructor(name2, audioCtx) {
    this.name = name2;
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

// ../widgets/InputRangeWithNumber.ts
Style(rule(".input-range-with-number", display("grid"), gridAutoFlow("column"), columnGap("5px"), gridTemplateColumns("1fr auto")));
var InputRangeWithNumberElement = class extends TypedEventBase {
  element;
  rangeInput;
  numberInput;
  constructor(...rest) {
    super();
    this.element = Div(className("input-range-with-number"), this.rangeInput = InputRange(...rest), this.numberInput = InputNumber());
    this.numberInput.min = this.rangeInput.min;
    this.numberInput.max = this.rangeInput.max;
    this.numberInput.step = this.rangeInput.step;
    this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
    this.numberInput.disabled = this.rangeInput.disabled;
    this.numberInput.placeholder = this.rangeInput.placeholder;
    this.numberInput.addEventListener("input", () => {
      this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber;
      this.rangeInput.dispatchEvent(new Event("input"));
    });
    this.rangeInput.addEventListener("input", () => {
      this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
      this.dispatchEvent(new TypedEvent("input"));
    });
  }
  get value() {
    return this.rangeInput.value;
  }
  set value(v) {
    this.rangeInput.value = this.numberInput.value = v;
  }
  get valueAsNumber() {
    return this.rangeInput.valueAsNumber;
  }
  set valueAsNumber(v) {
    this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber = v;
  }
  get disabled() {
    return this.rangeInput.disabled;
  }
  set disabled(v) {
    this.rangeInput.disabled = this.numberInput.disabled = v;
  }
};
function InputRangeWithNumber(...rest) {
  return new InputRangeWithNumberElement(...rest);
}

// ../widgets/PropertyList.ts
var PropertyGroup = class {
  constructor(name2, ...properties) {
    this.name = name2;
    this.properties = properties;
  }
  properties;
};
function group(name2, ...properties) {
  return new PropertyGroup(name2, ...properties);
}
Style(rule("dl", display("grid"), gridAutoFlow("row"), gridTemplateColumns("auto 1fr"), margin("1em")), rule("dt", gridColumn(1), textAlign("right"), paddingRight("1em")), rule("dd", textAlign("left"), gridColumn(2), marginInlineStart("0")), rule("dl > span, dl > div", gridColumn(1, 3)), rule("dl .alert", width("20em")));
var DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);
var PropertyList = class {
  element;
  rowGroups = /* @__PURE__ */ new Map();
  constructor(...rest) {
    this.element = DL(...this.createElements(rest));
  }
  append(...rest) {
    elementApply(this.element, ...this.createElements(rest));
  }
  createElements(rest) {
    return rest.flatMap((entry) => this.createGroups(entry).flatMap(identity));
  }
  createGroups(entry) {
    let name2 = DEFAULT_PROPERTY_GROUP;
    const group2 = new Array();
    if (entry instanceof PropertyGroup) {
      name2 = entry.name;
      group2.push(...entry.properties.map((e2) => this.createRow(e2)));
    } else {
      group2.push(this.createRow(entry));
    }
    if (!this.rowGroups.has(name2)) {
      this.rowGroups.set(name2, []);
    }
    this.rowGroups.get(name2).push(...group2);
    return group2;
  }
  createRow(entry) {
    if (isArray(entry)) {
      const [labelText, ...fields] = entry;
      const label = Label(labelText);
      for (const field of fields) {
        if (field instanceof HTMLInputElement || field instanceof HTMLTextAreaElement || field instanceof HTMLSelectElement) {
          if (field.id.length === 0) {
            field.id = stringRandom(10);
          }
          label.htmlFor = field.id;
          break;
        }
      }
      return [
        DT(label),
        DD(...fields)
      ];
    } else if (isString(entry) || isNumber(entry) || isBoolean(entry) || isDate(entry)) {
      return [
        Div(H2(entry))
      ];
    } else {
      return [
        entry
      ];
    }
  }
  setGroupVisible(id2, v) {
    const rows = this.rowGroups.get(id2);
    if (rows) {
      for (const row of rows) {
        for (const elem of row) {
          if (isErsatzElements(elem)) {
            for (const e2 of elem.elements) {
              elementSetDisplay(e2, v);
            }
          } else if (isErsatzElement(elem) || elem instanceof HTMLElement) {
            elementSetDisplay(elem, v);
          }
        }
      }
    }
  }
};

// ../threejs/environment/DeviceDialog.ts
var MIC_GROUP = "micFields" + stringRandom(8);
var DeviceDialog = class extends DialogBox {
  constructor(env) {
    super("Configure devices");
    this.env = env;
    this.micLookup = null;
    this.spkrLookup = null;
    this.spkrVolumeControl = null;
    this.ready = null;
    this.speakers = null;
    this.timer = new SetTimeoutTimer(30);
    this._showMic = true;
    this.cancelButton.style.display = "none";
    const clipLoaded = this.env.audio.loadBasicClip("test-audio", "/audio/test-clip.mp3", 0.5);
    elementApply(this.container, styles(minWidth("max-content")));
    elementApply(this.contentArea, this.properties = new PropertyList(group(MIC_GROUP, "Input", [
      "Microphones",
      this.microphones = Select(onInput(async () => {
        const deviceId = this.microphones.value;
        const device = this.micLookup.get(deviceId);
        await this.env.audio.devices.setAudioInputDevice(device);
      }))
    ], [
      "Input level",
      this.micScenario = Meter()
    ], ["Volume", this.micVolumeControl = InputRangeWithNumber(min2(0), max2(100), step(1), value(0), onInput(() => {
      env.audio.input.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
    }))], "Output")));
    this.env.audio.devices.addEventListener("audioinputchanged", (evt) => {
      this.microphones.value = evt.audio && evt.audio.deviceId || "";
    });
    if (canChangeAudioOutput) {
      this.properties.append([
        "Speakers",
        this.speakers = Select(onInput(async () => {
          const deviceId = this.speakers.value;
          const device = this.spkrLookup.get(deviceId);
          await this.env.audio.devices.setAudioOutputDevice(device);
        }))
      ]);
      this.env.audio.devices.addEventListener("audiooutputchanged", (evt) => {
        this.speakers.value = evt.device && evt.device.deviceId || "";
      });
    }
    this.properties.append([
      "Using headphones",
      this.useHeadphones = InputCheckbox(onInput(() => {
        this.env.audio.useHeadphones = this.useHeadphones.checked;
        elementSetDisplay(this.headphoneWarning, !this.env.audio.useHeadphones, "inline-block");
      })),
      this.testSpkrButton = ButtonSecondary("Test", title("Test audio"), styles(marginLeft("0.5em")), onClick(async () => {
        buttonSetEnabled(this.testSpkrButton, false, "secondary");
        await clipLoaded;
        await this.env.audio.playClipThrough("test-audio");
        buttonSetEnabled(this.testSpkrButton, true, "secondary");
      }))
    ], this.headphoneWarning = Div(className("alert alert-warning"), "\u{1F3A7}\u{1F399}\uFE0F This site has a voice chat feature. Voice chat is best experienced using headphones."), [
      "Volume",
      this.spkrVolumeControl = InputRangeWithNumber(min2(0), max2(100), step(1), value(0), onInput(() => env.audio.audioDestination.volume = this.spkrVolumeControl.valueAsNumber / 100))
    ]);
    this.activity = new ActivityDetector("device-settings-dialog-activity", this.env.audio.audioCtx);
    this.timer.addTickHandler(() => {
      this.micScenario.value = this.activity.level;
    });
  }
  async load() {
    await this.env.audio.ready;
    await this.env.audio.devices.ready;
    const mics = await this.env.audio.devices.getAudioInputDevices();
    this.micLookup = makeLookup(mics, (m) => m.deviceId);
    elementApply(this.microphones, Option(value(""), "NONE"), ...mics.map((device) => Option(value(device.deviceId), device.label)));
    if (canChangeAudioOutput) {
      const spkrs = await this.env.audio.devices.getAudioOutputDevices();
      this.spkrLookup = makeLookup(spkrs, (device) => device.deviceId);
      elementApply(this.speakers, ...spkrs.map((device) => Option(value(device.deviceId), device.label)));
    }
    connect(this.env.audio.input, this.activity);
  }
  async onShowing() {
    if (this.ready === null) {
      this.ready = this.load();
    }
    await this.ready;
    const curMic = await this.env.audio.devices.getAudioInputDevice();
    this.microphones.value = curMic && curMic.deviceId || "";
    this.micVolumeControl.valueAsNumber = this.env.audio.input.gain.value * 100;
    if (canChangeAudioOutput) {
      const curSpker = await this.env.audio.devices.getAudioOutputDevice();
      this.speakers.value = curSpker && curSpker.deviceId || "";
    }
    this.spkrVolumeControl.valueAsNumber = this.env.audio.audioDestination.volume * 100;
    this.useHeadphones.checked = this.env.audio.useHeadphones;
    elementSetDisplay(this.headphoneWarning, !this.env.audio.useHeadphones, "inline-block");
    this.timer.start();
    await super.onShowing();
  }
  onClosed() {
    this.timer.stop();
    super.onClosed();
  }
  get showMic() {
    return this._showMic;
  }
  set showMic(v) {
    if (v !== this.showMic) {
      this._showMic = v;
      this.properties.setGroupVisible(MIC_GROUP, this.showMic);
    }
  }
};

// ../threejs/environment/Environment.ts
var EnvironmentRoomJoinedEvent = class extends TypedEvent {
  constructor(roomName) {
    super("roomjoined");
    this.roomName = roomName;
  }
};
var Environment = class extends BaseEnvironment {
  audio;
  interactionAudio;
  xrUI;
  screenUISpace = new ScreenUI();
  confirmationDialog;
  compassImage;
  clockImage;
  batteryImage;
  settingsButton;
  muteMicButton;
  muteEnvAudioButton;
  quitButton;
  lobbyButton;
  vrButton;
  fullscreenButton;
  devicesDialog;
  apps;
  uiButtons;
  audioPlayer;
  videoPlayer;
  envAudioToggleEvt = new TypedEvent("environmentaudiotoggled");
  _currentRoom = null;
  get currentRoom() {
    return this._currentRoom;
  }
  DEBUG;
  constructor(canvas, fetcher, dialogFontFamily, uiImagePaths, defaultAvatarHeight, enableFullResolution, options) {
    super(canvas, fetcher, defaultAvatarHeight, enableFullResolution);
    this.compassImage = new CanvasImageMesh(this, "Horizon", new ArtificialHorizon());
    this.compassImage.mesh.renderOrder = 5;
    this.clockImage = new CanvasImageMesh(this, "Clock", new ClockImage());
    this.clockImage.mesh.renderOrder = 5;
    options = options || {};
    const JS_EXT = options.JS_EXT || ".js";
    this.DEBUG = options.DEBUG || false;
    this.apps = new ApplicationLoader(this, JS_EXT);
    this.apps.addEventListener("apploading", (evt) => {
      evt.preLoadTask = this.fadeOut().then(() => {
        this.clearScene();
        this.avatar.reset();
      });
    });
    this.apps.addEventListener("apploaded", (evt) => {
      evt.app.addEventListener("joinroom", (evt2) => {
        if (evt2.roomName !== this._currentRoom) {
          this._currentRoom = evt2.roomName;
          this.dispatchEvent(new EnvironmentRoomJoinedEvent(evt2.roomName));
        }
      });
    });
    this.apps.addEventListener("appshown", async (evt) => {
      this.lobbyButton.visible = evt.appName !== "menu";
      await this.fadeIn();
    });
    this.audio = new AudioManager(DEFAULT_LOCAL_USER_ID);
    this.audio.setAudioProperties(1, 4, "exponential");
    this.audioPlayer = new AudioPlayer(this.audio.audioCtx);
    this.videoPlayer = new VideoPlayer3D(this, this.audio.audioCtx);
    this.videoPlayer.object.visible = false;
    this.interactionAudio = new InteractionAudio(this.audio, this.eventSystem);
    this.confirmationDialog = new ConfirmationDialog(this, dialogFontFamily);
    this.devicesDialog = new DeviceDialog(this);
    elementApply(this.renderer.domElement.parentElement, this.screenUISpace, this.confirmationDialog, this.devicesDialog);
    this.uiButtons = new ButtonFactory(this.fetcher, uiImagePaths, 20);
    this.settingsButton = new ButtonImageWidget(this.uiButtons, "ui", "settings");
    this.quitButton = new ButtonImageWidget(this.uiButtons, "ui", "quit");
    this.lobbyButton = new ButtonImageWidget(this.uiButtons, "ui", "lobby");
    this.muteMicButton = new ToggleButton(this.uiButtons, "microphone", "mute", "unmute");
    this.muteEnvAudioButton = new ToggleButton(this.uiButtons, "environment-audio", "mute", "unmute");
    this.vrButton = new ScreenModeToggleButton(this.uiButtons, "VR" /* VR */);
    this.fullscreenButton = new ScreenModeToggleButton(this.uiButtons, "Fullscreen" /* Fullscreen */);
    this.xrUI = new SpaceUI();
    this.xrUI.addItem(this.clockImage, { x: -1, y: 1, scale: 1 });
    this.xrUI.addItem(this.quitButton, { x: 1, y: 1, scale: 0.5 });
    this.xrUI.addItem(this.confirmationDialog, { x: 0, y: 0, scale: 0.25 });
    this.xrUI.addItem(this.settingsButton, { x: -1, y: -1, scale: 0.5 });
    this.xrUI.addItem(this.muteMicButton, { x: -0.84, y: -1, scale: 0.5 });
    this.xrUI.addItem(this.muteEnvAudioButton, { x: -0.68, y: -1, scale: 0.5 });
    this.xrUI.addItem(this.lobbyButton, { x: -0.473, y: -1, scale: 0.5 });
    this.xrUI.addItem(this.vrButton, { x: 1, y: -1, scale: 0.5 });
    this.xrUI.addItem(this.fullscreenButton, { x: 1, y: -1, scale: 0.5 });
    this.worldUISpace.add(this.xrUI);
    elementApply(this.screenUISpace.topRowLeft, this.compassImage, this.clockImage);
    elementApply(this.screenUISpace.topRowRight, this.quitButton);
    elementApply(this.screenUISpace.bottomRowLeft, this.settingsButton, this.muteMicButton, this.muteEnvAudioButton, this.lobbyButton);
    elementApply(this.screenUISpace.bottomRowRight, this.fullscreenButton, this.vrButton);
    if (BatteryImage.isAvailable && isMobile()) {
      this.batteryImage = new CanvasImageMesh(this, "Battery", new BatteryImage());
      this.xrUI.addItem(this.batteryImage, { x: 0.75, y: -1, scale: 1 });
      elementApply(this.screenUISpace.topRowRight, this.batteryImage);
    }
    this.vrButton.visible = isDesktop() && hasVR() || isMobileVR();
    this.lobbyButton.visible = false;
    this.muteMicButton.visible = false;
    this.screenControl.setUI(this.screenUISpace, this.fullscreenButton, this.vrButton);
    this.refreshSpaceUI();
    this.quitButton.addEventListener("click", () => this.withConfirmation("Confirm quit", "Are you sure you want to quit?", async () => {
      if (this.renderer.xr.isPresenting) {
        this.screenControl.stop();
      }
      await this.onQuitting();
    }));
    this.lobbyButton.addEventListener("click", () => this.withConfirmation("Confirm return to lobby", "Are you sure you want to return to the lobby?", () => this.dispatchEvent(new TypedEvent("home"))));
    this.settingsButton.addEventListener("click", async () => {
      const mode = this.screenControl.currentMode;
      const wasPresenting = this.renderer.xr.isPresenting;
      if (wasPresenting) {
        await this.screenControl.stop();
      }
      await this.devicesDialog.showDialog();
      if (wasPresenting) {
        await this.screenControl.start(mode);
      }
    });
    const onSessionChange = () => this.refreshSpaceUI();
    this.screenControl.addEventListener("sessionstarted", onSessionChange);
    this.screenControl.addEventListener("sessionstopped", onSessionChange);
    this.muteEnvAudioButton.addEventListener("click", () => {
      this.muteEnvAudioButton.active = !this.muteEnvAudioButton.active;
      this.dispatchEvent(this.envAudioToggleEvt);
    });
    this.avatar.addEventListener("avatarmoved", (evt) => this.audio.setUserPose(this.audio.localUserID, evt.px, evt.py, evt.pz, evt.fx, evt.fy, evt.fz, evt.ux, evt.uy, evt.uz));
  }
  refreshSpaceUI() {
    this.xrUI.visible = this.renderer.xr.isPresenting || this.testSpaceLayout;
    this.clockImage.isVisible = this.xrUI.visible || this.DEBUG;
  }
  _testSpaceLayout = false;
  get testSpaceLayout() {
    return this._testSpaceLayout;
  }
  set testSpaceLayout(v) {
    if (v !== this.testSpaceLayout) {
      this._testSpaceLayout = v;
      this.refreshSpaceUI();
    }
  }
  countTick = 0;
  fpses = new Array();
  avgFPS = 0;
  preRender(evt) {
    super.preRender(evt);
    this.audio.update();
    this.videoPlayer.update(evt.dt, evt.frame);
    this.compassImage.image.setPitchAndHeading(rad2deg(this.avatar.worldPitch), rad2deg(this.avatar.worldHeading));
    if (this.DEBUG) {
      const fps = Math.round(evt.fps);
      this.avgFPS += fps / 100;
      this.fpses.push(fps);
      if (this.fpses.length > 100) {
        const fps2 = this.fpses.shift();
        this.avgFPS -= fps2 / 100;
      }
      if (++this.countTick % 100 === 0) {
        this.clockImage.image.fps = this.avgFPS;
      }
    }
    this.confirmationDialog.update(evt.dt);
    for (const app of this.apps) {
      app.update(evt);
    }
  }
  get environmentAudioMuted() {
    return this.muteEnvAudioButton.active;
  }
  async withConfirmation(title2, msg, act) {
    this.onConfirmationShowing(true);
    if (await this.confirmationDialog.prompt(title2, msg)) {
      act();
    }
    this.onConfirmationShowing(false);
  }
  onConfirmationShowing(showing) {
    widgetSetEnabled(this.quitButton, !showing, "primary");
    widgetSetEnabled(this.lobbyButton, !showing, "primary");
  }
  async load(prog) {
    await progressTasks(prog, (prog2) => super.load(prog2), (prog2) => this.uiButtons.load(prog2), (prog2) => this.audio.loadBasicClip("footsteps", "/audio/TransitionFootstepAudio.mp3", 0.5, prog2), (prog2) => this.interactionAudio.load("enter", "/audio/basic_enter.mp3", 0.25, prog2), (prog2) => this.interactionAudio.load("exit", "/audio/basic_exit.mp3", 0.25, prog2), (prog2) => this.interactionAudio.load("error", "/audio/basic_error.mp3", 0.25, prog2), (prog2) => this.interactionAudio.load("click", "/audio/vintage_radio_button_pressed.mp3", 1, prog2));
  }
};

// src/index.ts
var src_default = Environment;
export {
  src_default as default
};
//# sourceMappingURL=index.js.map
