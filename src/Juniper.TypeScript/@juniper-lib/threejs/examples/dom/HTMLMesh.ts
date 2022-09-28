import {
    Mesh,
    MeshBasicMaterial,
    PlaneGeometry,
    Event as ThreeJSEvent
} from "three";


import { HTMLTexture } from "./HTMLTexture";

export class HTMLMesh extends Mesh<PlaneGeometry, MeshBasicMaterial> {

    private readonly texture: HTMLTexture;
    private readonly onEvent: (event: ThreeJSEvent) => void;

    constructor(dom: HTMLElement) {

        const texture = new HTMLTexture(dom);
        const geometry = new PlaneGeometry(texture.image.width * 0.001, texture.image.height * 0.001);
        const material = new MeshBasicMaterial({ map: texture, toneMapped: false, transparent: true });

        super(geometry, material);

        this.texture = texture;
        this.geometry = geometry;
        this.material = material;

        this.onEvent = (event: ThreeJSEvent) =>
            texture.dispatchDOMEvent(event);

        this.addEventListener("mousedown", this.onEvent);
        this.addEventListener("mousemove", this.onEvent);
        this.addEventListener("mouseup", this.onEvent);
        this.addEventListener("click", this.onEvent);
    }



    dispose() {
        this.removeEventListener("mousedown", this.onEvent);
        this.removeEventListener("mousemove", this.onEvent);
        this.removeEventListener("mouseup", this.onEvent);
        this.removeEventListener("click", this.onEvent);

        this.geometry.dispose();
        this.material.dispose();
        this.texture.dispose();
    }
}
