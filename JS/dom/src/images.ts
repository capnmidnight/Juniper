export function getWidth(img: TexImageSource) {
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

export function getHeight(img: TexImageSource) {
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