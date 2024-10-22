import { arrayClear, debounce, debounceRAF } from "@juniper-lib/util";
import { D, G, HeightSvg, HtmlRender, ID, OnClick, OnPointerCancel, OnPointerDown, OnPointerMove, OnPointerUp, OnWheel, Path, TitleSvg, ViewBox, WidthSvg, isModifierless } from "@juniper-lib/dom";
import * as d3 from "d3";
export class GeoMapPathSelectedEvent extends Event {
    constructor(pathId) {
        super("pathselected");
        this.pathId = pathId;
    }
}
// https://github.com/d3/d3-geo
export class GeoMap extends EventTarget {
    #getRegionId;
    #getRegionName;
    #g = G();
    #paths = new Map();
    #features = new Map();
    #markers = new Array();
    #tags = new Map();
    #render;
    #projection;
    #resizer;
    #resize;
    #lng = 0;
    #dy = 0;
    #s = 1;
    #clicked = false;
    #createPath;
    constructor(svg, movable, getRegionId, getRegionName) {
        super();
        this.svg = svg;
        this.#getRegionId = getRegionId;
        this.#getRegionName = getRegionName;
        this.#projection = d3
            .geoMercator()
            .rotate([this.#lng, 0, 0])
            .translate([0, this.#dy])
            .scale(this.#s);
        this.#createPath = d3
            .geoPath()
            .projection(this.#projection);
        this.#render = debounceRAF(this.#_render.bind(this));
        this.#resize = debounce(this.#_resize.bind(this));
        this.svg.append(this.#g);
        if (movable) {
            HtmlRender(this.svg, OnPointerDown(evt => {
                if (isModifierless(evt) && evt.buttons === 1) {
                    this.#clicked = true;
                }
            }), OnPointerCancel(() => this.#clicked = false), OnPointerUp(() => this.#clicked = false), OnPointerMove(evt => {
                if (this.#clicked) {
                    evt.preventDefault();
                    evt.stopPropagation();
                    this.#lng += evt.movementX / (5 * this.#s);
                    this.#dy += Math.sqrt(this.#s) * evt.movementY / 100;
                    this.#render();
                }
            }), OnWheel(evt => {
                evt.preventDefault();
                evt.stopPropagation();
                this.#s = Math.max(1, this.#s - evt.deltaY / 100);
                this.#render();
            }));
        }
        this.#resizer = new ResizeObserver((evts) => {
            for (const evt of evts) {
                if (evt.target == this.svg) {
                    this.#resize();
                }
            }
        });
        this.#resizer.observe(this.svg);
    }
    #_resize() {
        if (this.#features.size > 0) {
            const maxWidth = this.svg.clientWidth;
            const size = this.#g.getBBox();
            const width = maxWidth;
            const height = size.height * maxWidth / size.width;
            HtmlRender(this.svg, WidthSvg(width.toFixed()), HeightSvg(height.toFixed()), ViewBox(`${size.x} ${size.y} ${size.width} ${size.height}`));
        }
    }
    #_render() {
        this.#projection.rotate([this.#lng, 0, 0])
            .translate([0, this.#dy])
            .scale(this.#s);
        this.#createPath = d3.geoPath().projection(this.#projection);
        for (const feature of this.#features.values()) {
            const p = this.#paths.get(this.#getRegionId(feature));
            p.setAttribute("d", this.#createPath(feature));
        }
    }
    #addFeature(feature) {
        const id = this.#getRegionId(feature);
        const path = Path(ID(id), D(this.#createPath(feature)), TitleSvg(this.#getRegionName(feature)), OnClick(() => {
            this.selectPath(id);
            this.dispatchEvent(new GeoMapPathSelectedEvent(id));
        }));
        this.#features.set(id, feature);
        this.#paths.set(id, path);
        return path;
    }
    addFeature(feature) {
        const path = this.#addFeature(feature);
        this.#g.append(path);
        this.#resize();
    }
    getFeature(pathId) {
        return this.#features.get(pathId);
    }
    getCentroid(pathId) {
        const feature = (typeof pathId === "string")
            ? this.getFeature(pathId)
            : pathId;
        return this.#createPath.centroid(feature);
    }
    setGeoJsonFile(geoJsonFile) {
        this.#g.innerHTML = "";
        this.#paths.clear();
        this.#features.clear();
        const paths = geoJsonFile.features.map(f => this.#addFeature(f));
        this.#g.append(...paths);
        this.#resize();
    }
    async loadGeoJsonFile(path) {
        const request = fetch(path);
        const response = await request;
        if (response.ok) {
            const geoJsonFile = await response.json();
            this.setGeoJsonFile(geoJsonFile);
        }
    }
    getPath(pathId) {
        return this.#paths.get(pathId);
    }
    selectPath(pathId) {
        let anySelected = false;
        for (const [id, path] of this.#paths) {
            const selected = pathId === id;
            if (selected) {
                anySelected = true;
            }
            path.classList.toggle("selected", selected);
        }
        if (pathId && !anySelected) {
            console.warn(`Couldn't find path "${pathId}"`);
        }
    }
    clearTags() {
        for (const [path, tags] of this.#tags) {
            for (const tag of tags) {
                path.classList.remove(tag);
            }
        }
        this.#tags.clear();
    }
    tag(pathId, className) {
        const path = this.getPath(pathId);
        if (path) {
            if (!this.#tags.has(path)) {
                this.#tags.set(path, []);
            }
            const tags = this.#tags.get(path);
            tags.push(className);
            path.classList.add(className);
        }
        else {
            console.warn(`Couldn't find path "${pathId}"`);
        }
    }
    clearMarkers() {
        for (const marker of this.#markers) {
            marker.remove();
        }
        arrayClear(this.#markers);
    }
    addMarker(marker) {
        this.#markers.push(marker);
        this.#g.append(marker);
    }
}
//# sourceMappingURL=index.js.map