import { className, id, title } from "@juniper-lib/dom/attrs";
import { backgroundColor, border, cursor, display, height, left, margin, padding, position, rule, textAlign, top, transform, width, zIndex } from "@juniper-lib/dom/css";
import { Div, IFrame, Img, Style } from "@juniper-lib/dom/tags";

Style(
    rule(".thumbnail-view",
        position("fixed"),
        top("50%"),
        left("50%"),
        width("75%"),
        height("80%"),
        transform("translate(-50%, -50%)"),
        border("solid 1px grey"),
        backgroundColor("white"),
        padding("4em"),
        margin("auto"),
        textAlign("center"),
        zIndex(15)),

    rule(".thumbnail",
        cursor("zoom-in")),

    rule(".thumbnail-view > img",
        height("100%")),

    rule(".thumbnail-view > iframe",
        width("100%"),
        height("100%")),

    rule(".img-thumbnail",
        width("2em")));

function showThumbnail(elem: HTMLImageElement, isPhotosphere: boolean) {
    thumbnailView.style.display = "block";
    thumbnailImage.style.display = isPhotosphere ? "none" : "block";
    thumbnailIFrame.style.display = isPhotosphere ? "block" : "none";
    if (isPhotosphere) {
        const id = parseInt(elem.dataset.photosphereid, 10);
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
    const thumbnails = Array.from(document.querySelectorAll<HTMLImageElement>(".thumbnail"));
    for (const thumbnail of thumbnails) {
        if (!existing.has(thumbnail)) {
            thumbnail.addEventListener("click", () => showThumbnail(thumbnail, thumbnail.classList.contains("photosphere")));
            existing.add(thumbnail);
        }
    }
}

const thumbnailViewerID = "33D0371F-B096-473D-AEE3-B17F5392CCEC";

let thumbnailView = document.getElementById(thumbnailViewerID) as HTMLDivElement;
if (!thumbnailView) {
    thumbnailView = Div(
        id(thumbnailViewerID),
        className("thumbnail-view"),
        display("none"),
        Img(title("Thumbnail")),
        IFrame(title("Preview")));
}

const thumbnailImage = thumbnailView.querySelector<HTMLImageElement>("img");
const thumbnailIFrame = thumbnailView.querySelector<HTMLIFrameElement>("iframe");

if (!thumbnailView.parentElement) {
    document.body.append(thumbnailView);
    thumbnailView.addEventListener("pointerleave", hideThumbnail);
    thumbnailView.addEventListener("click", hideThumbnail);
}