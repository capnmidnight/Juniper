import { backgroundColor, border, ClassList, cursor, display, Div, elementSetDisplay, em, height, HtmlRender, ID, IFrame, Img, left, margin, padding, perc, position, rule, SingletonStyleBlob, textAlign, TitleAttr, top, transform, translateFunc, width, zIndex } from "@juniper-lib/dom";

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

    SingletonStyleBlob("Juniper::Widgets::Thumbnails", () => [
        rule(".thumbnail-view",
            position("fixed"),
            top(perc(50)),
            left(perc(50)),
            width(perc(75)),
            height(perc(80)),
            transform(translateFunc(perc(-50), perc(-50))),
            border("solid 1px grey"),
            backgroundColor("white"),
            padding(em(4)),
            margin("auto"),
            textAlign("center"),
            zIndex(150),

            rule(">img",
                height(perc(100))
            ),

            rule(">iframe",
                width(perc(100)),
                height(perc(100))
            )
        ),

        rule(".thumbnail",
            cursor("zoom-in")
        ),

        rule(".img-thumbnail",
            width(em(2))
        )
    ]);

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
        Img(TitleAttr("Thumbnail")),
        IFrame(TitleAttr("Preview")));
}

const thumbnailImage = thumbnailView.querySelector<HTMLImageElement>("img");
const thumbnailIFrame = thumbnailView.querySelector<HTMLIFrameElement>("iframe");

if (!thumbnailView.parentElement) {
    HtmlRender(document.body, thumbnailView);
    thumbnailView.addEventListener("pointerleave", hideThumbnail);
    thumbnailView.addEventListener("click", hideThumbnail);
}