import {
    CanvasTexture,
    LinearFilter,
    sRGBEncoding,
    Event as ThreeJSEvent
} from 'three';

import { html2canvas } from "./html2canvas";
import { htmlevent } from "./htmlevent";

export class HTMLTexture extends CanvasTexture {

    private scheduleUpdate: number = null;

    private readonly observer: MutationObserver;

    constructor(private readonly dom: HTMLElement) {

        super(html2canvas(dom));

        this.anisotropy = 16;
        this.encoding = sRGBEncoding;
        this.minFilter = LinearFilter;
        this.magFilter = LinearFilter;

        // Create an observer on the DOM, and run html2canvas update in the next loop
        const observer = new MutationObserver(() => {

            if (!this.scheduleUpdate) {

                // ideally should use xr.requestAnimationFrame, here setTimeout to avoid passing the renderer
                this.scheduleUpdate = setTimeout(() => this.update(), 16) as unknown as number;

            }

        });

        const config: MutationObserverInit = {
            attributes: true,
            childList: true,
            subtree: true,
            characterData: true
        };
        observer.observe(dom, config);

        this.observer = observer;

    }

    dispatchDOMEvent(event: ThreeJSEvent) {
        if (event.data) {
            htmlevent(this.dom, event.type, event.data.x, event.data.y);
        }
    }

    update() {
        this.image = html2canvas(this.dom);
        this.needsUpdate = true;
        this.scheduleUpdate = null;
    }

    override dispose() {
        if (this.observer) {
            this.observer.disconnect();
        }

        clearTimeout(this.scheduleUpdate);
        this.scheduleUpdate = null;

        super.dispose();
    }
}