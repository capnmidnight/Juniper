export function getWidth(img) {
    if (img instanceof HTMLVideoElement) {
        return img.videoWidth;
    }
    else if (img instanceof VideoFrame) {
        return img.displayWidth;
    }
    else {
        return img.width;
    }
}
export function getHeight(img) {
    if (img instanceof HTMLVideoElement) {
        return img.videoHeight;
    }
    else if (img instanceof VideoFrame) {
        return img.displayHeight;
    }
    else {
        return img.height;
    }
}
//# sourceMappingURL=images.js.map