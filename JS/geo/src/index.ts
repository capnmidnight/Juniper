import { arrayClear, debounce, debounceRAF } from "@juniper-lib/util";
import { D, G, HeightSvg, HtmlRender, ID, OnClick, OnPointerCancel, OnPointerDown, OnPointerMove, OnPointerUp, OnWheel, Path, TitleSvg, ViewBox, WidthSvg, isModifierless } from "@juniper-lib/dom";
import * as d3 from "d3";
import type { Feature, FeatureCollection, MultiPolygon, Polygon } from "geojson";

export type GEOJSONS = FeatureCollection<Polygon | MultiPolygon>;
export type GEOJSON = Feature<Polygon | MultiPolygon>;


export type GEOJSONCallback<T = string> = (v: GEOJSON) => T;

export class GeoMapPathSelectedEvent extends Event {
    constructor(public readonly pathId: string) {
        super("pathselected")
    }
}

// https://github.com/d3/d3-geo
export class GeoMap extends EventTarget {
    readonly #getRegionId: GEOJSONCallback;
    readonly #getRegionName: GEOJSONCallback;

    readonly #g = G();
    readonly #paths = new Map<string, SVGPathElement>();
    readonly #features = new Map<string, GEOJSON>();
    readonly #markers = new Array<SVGElement>();
    readonly #tags = new Map<SVGPathElement, Array<string>>();

    readonly #render: VoidFunction;
    readonly #projection: d3.GeoProjection;
    readonly #resizer: ResizeObserver;
    readonly #resize: VoidFunction;

    #lng = 0;
    #dy = 0;
    #s = 1;
    #clicked = false;
    #createPath: d3.GeoPath<any, d3.GeoPermissibleObjects>;

    constructor(public readonly svg: SVGSVGElement,
        movable: boolean,
        getRegionId: GEOJSONCallback,
        getRegionName: GEOJSONCallback
    ) {
        super();

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
            HtmlRender(this.svg,
                OnPointerDown(evt => {
                    if (isModifierless(evt) && evt.buttons === 1) {
                        this.#clicked = true;
                    }
                }),
                OnPointerCancel(() => this.#clicked = false),
                OnPointerUp(() => this.#clicked = false),
                OnPointerMove(evt => {
                    if (this.#clicked) {
                        evt.preventDefault();
                        evt.stopPropagation();
                        this.#lng += evt.movementX / (5 * this.#s);
                        this.#dy += Math.sqrt(this.#s) * evt.movementY / 100;
                        this.#render();
                    }
                }),
                OnWheel(evt => {
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

            HtmlRender(this.svg,
                WidthSvg(width.toFixed()),
                HeightSvg(height.toFixed()),
                ViewBox(`${size.x} ${size.y} ${size.width} ${size.height}`)
            );
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

    #addFeature(feature: GEOJSON) {
        const id = this.#getRegionId(feature);
        const path = Path(
            ID(id),
            D(this.#createPath(feature)),
            TitleSvg(this.#getRegionName(feature)),
            OnClick(() => {
                this.selectPath(id);
                this.dispatchEvent(new GeoMapPathSelectedEvent(id));
            })
        );

        this.#features.set(id, feature);
        this.#paths.set(id, path);
        return path;
    }

    addFeature(feature: GEOJSON) {
        const path = this.#addFeature(feature);
        this.#g.append(path);
        this.#resize();
    }

    getFeature(pathId: string) {
        return this.#features.get(pathId);
    }

    getCentroid(pathId: string | GEOJSON) {
        const feature = (typeof pathId === "string")
            ? this.getFeature(pathId)
            : pathId;
        return this.#createPath.centroid(feature);
    }

    setGeoJsonFile(geoJsonFile: GEOJSONS) {
        this.#g.innerHTML = "";
        this.#paths.clear();
        this.#features.clear();

        const paths = geoJsonFile.features.map(f => this.#addFeature(f));
        this.#g.append(...paths);

        this.#resize();
    }

    async loadGeoJsonFile(path: string | URL) {
        const request = fetch(path)
        const response = await request;
        if (response.ok) {
            const geoJsonFile = await response.json() as GEOJSONS;
            this.setGeoJsonFile(geoJsonFile);
        }
    }

    getPath(pathId: string) {
        return this.#paths.get(pathId);
    }

    selectPath(pathId: string) {
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

    tag(pathId: string, className: string) {
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

    addMarker(marker: SVGElement) {
        this.#markers.push(marker);
        this.#g.append(marker);
    }
}
