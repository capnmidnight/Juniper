if (!hasFullscreenAPI()) {
    const Elm = Element.prototype;
    const Doc = Document.prototype;
    if ("webkitRequestFullscreen" in Elm) {
        Elm.requestFullscreen = Elm.webkitRequestFullscreen;
        Doc.exitFullscreen = Doc.webkitRequestFullscreen;
        Object.defineProperties(Doc, {
            "fullscreenEnabled": {
                get: function () {
                    return this.webkitFullscreenEnabled;
                }
            },
            "fullscreenElement": {
                get: function () {
                    return this.webkitFullscreenElement;
                }
            }
        });
    }
    else if ("mozRequestFullScreen" in Elm) {
        Elm.requestFullscreen = Elm.mozRequestFullScreen;
        Doc.exitFullscreen = Doc.mozCancelFullScreen;
        Object.defineProperties(Doc, {
            "fullscreenEnabled": {
                get: function () {
                    return this.mozFullScreenEnabled;
                }
            },
            "fullscreenElement": {
                get: function () {
                    return this.mozFullScreenElement;
                }
            }
        });
    }
    else if ("msRequestFullscreen" in Elm) {
        Elm.requestFullscreen = Elm.msRequestFullscreen;
        Doc.exitFullscreen = Doc.msExitFullscreen;
        Object.defineProperties(Doc, {
            "fullscreenEnabled": {
                get: function () {
                    return this.msFullscreenEnabled;
                }
            },
            "fullscreenElement": {
                get: function () {
                    return this.msFullscreenElement;
                }
            }
        });
    }
}
export function hasFullscreenAPI() {
    return "requestFullscreen" in document.documentElement;
}
