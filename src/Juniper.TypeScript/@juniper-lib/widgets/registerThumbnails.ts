import { ClassList, ID, Title_attr } from "@juniper-lib/dom/attrs";
import { display } from "@juniper-lib/dom/css";
import { Div, IFrame, Img, elementApply, elementSetDisplay } from "@juniper-lib/dom/tags";

import "./registerThumbnails.css";

function showThumbnail(elem: HTMLImageElement, isPhotosphere: boolean) {
    elementSetDisplay(thumbnailView, true, "block");
    elementSetDisplay(thumbnailImage, !isPhotosphere, "block");
    elementSetDisplay(thumbnailIFrame, isPhotosphere, "block");
    const match = elem.src.match(/\/file\/(\d+)$/);
    if (isPhotosphere && match) {
        const id = parseInt(match[1], 10);
        thumbnailIFrame.src = `/Editor/PhotosphereViewer?FileID=${id}`;
    }
    else {
        thumbnailImage.src = elem.src;
    }
}

function hideThumbnail() {
    thumbnailView.style.display = "none";
}

const existing = new WeakSet<HTMLImageElement>();

export function registerThumbnails() {
    const thumbnails = document.querySelectorAll<HTMLImageElement>(".thumbnail");
    for (const thumbnail of thumbnails) {
        if (!existing.has(thumbnail)) {
            thumbnail.addEventListener("click", () =>
                showThumbnail(thumbnail, thumbnail.classList.contains("photosphere")));
            existing.add(thumbnail);
        }
    }
}

const thumbnailViewerID = "33D0371F-B096-473D-AEE3-B17F5392CCEC";

let thumbnailView = document.getElementById(thumbnailViewerID) as HTMLDivElement;
if (!thumbnailView) {
    thumbnailView = Div(
        ID(thumbnailViewerID),
        ClassList("thumbnail-view"),
        display("none"),
        Img(Title_attr("Thumbnail")),
        IFrame(Title_attr("Preview")));
}

const thumbnailImage = thumbnailView.querySelector<HTMLImageElement>("img");
const thumbnailIFrame = thumbnailView.querySelector<HTMLIFrameElement>("iframe");

if (!thumbnailView.parentElement) {
    elementApply(document.body, thumbnailView);
    thumbnailView.addEventListener("pointerleave", hideThumbnail);
    thumbnailView.addEventListener("click", hideThumbnail);
}