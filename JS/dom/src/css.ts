import { isArray, isDefined, isNumber, isString, toString } from "@juniper-lib/util";
import { AbstractAppliable } from "./AbstractAppliable";


/**********************************
 * CASCADING STYLE SHEETS
 *********************************/

export function cssVar(name: string): string { return `$var(--${name})`; }
export function perc(value: number): CssPercentage { return `${value}%`; }
export function deg(value: number): CssAngle { return `${value}deg`; }
export function rad(value: number): CssAngle { return `${value}rad`; }
export function grad(value: number): CssAngle { return `${value}grad`; }
export function turn(value: number): CssAngle { return `${value}turn`; }
export function cap(value: number): CssFontRelativeLength { return `${value}cap`; }
export function ch(value: number): CssFontRelativeLength { return `${value}ch`; }
export function em(value: number): CssFontRelativeLength { return `${value}em`; }
export function ex(value: number): CssFontRelativeLength { return `${value}ex`; }
export function ic(value: number): CssFontRelativeLength { return `${value}ic`; }
export function lh(value: number): CssFontRelativeLength { return `${value}lh`; }
export function rem(value: number): CssFontRelativeLength { return `${value}rem`; }
export function rlh(value: number): CssFontRelativeLength { return `${value}rlh`; }
export function vh(value: number): CssViewportPercentageLength { return `${value}vh`; }
export function vw(value: number): CssViewportPercentageLength { return `${value}vw`; }
export function vi(value: number): CssViewportPercentageLength { return `${value}vi`; }
export function vb(value: number): CssViewportPercentageLength { return `${value}vb`; }
export function vmin(value: number): CssViewportPercentageLength { return `${value}vmin`; }
export function vmax(value: number): CssViewportPercentageLength { return `${value}vmax`; }
export function px(value: number): CssAbsoluteLength { return `${value}px`; }
export function cm(value: number): CssAbsoluteLength { return `${value}cm`; }
export function mm(value: number): CssAbsoluteLength { return `${value}mm`; }
export function q(value: number): CssAbsoluteLength { return `${value}Q`; }
export function inch(value: number): CssAbsoluteLength { return `${value}in`; }
export function pc(value: number): CssAbsoluteLength { return `${value}pc`; }
export function pt(value: number): CssAbsoluteLength { return `${value}pt`; }
export function fr(value: number): CssGridFlexValue { return `${value}fr`; }

export function matrix(a: number, b: number, c: number, d: number, tx: number, ty: number) {
    return `matrix(${a}, ${b}, ${c}, ${d}, ${tx}, ${ty})`;
}

export function matrix3d(
    a1: number, b1: number, c1: number, d1: number,
    a2: number, b2: number, c2: number, d2: number,
    a3: number, b3: number, c3: number, d3: number,
    a4: number, b4: number, c4: number, d4: number) {
    return `matrix3d(${a1}, ${b1}, ${c1}, ${d1}, ${a2}, ${b2}, ${c2}, ${d2}, ${a3}, ${b3}, ${c3}, ${d3}, ${a4}, ${b4}, ${c4}, ${d4})`;
}

export function perspectiveValue(v: 0 | string) {
    return `perspective(${v})`;
}

export function rotate(a: 0 | string) {
    return `rotate(${a})`;
}

export function rotate3d(x: number, y: number, z: number, a: 0 | string) {
    return `rotate3d(${x}, ${y}, ${z}, ${a})`;
}

export function rotateX(a: 0 | string) {
    return `rotateX(${a})`;
}

export function rotateY(a: 0 | string) {
    return `rotateY(${a})`;
}

export function rotateZ(a: 0 | string) {
    return `rotateZ(${a})`;
}

export function translateFunc(x: 0 | string, y: 0 | string) {
    return `translate(${x}, ${y})`;
}

export function translate3d(x: 0 | string, y: 0 | string, z: 0 | string) {
    return `translate3d(${x}, ${y}, ${z})`;
}

export function translateX(x: 0 | string) {
    return `translateX(${x})`;
}

export function translateY(y: 0 | string) {
    return `translateY(${y})`;
}

export function translateZ(z: 0 | string) {
    return `translateZ(${z})`;
}

export function scale(x: 0 | string, y: 0 | string) {
    return `scale(${x}, ${y})`;
}

export function scale3d(x: 0 | string, y: 0 | string, z: 0 | string) {
    return `scale3d(${x}, ${y}, ${z})`;
}

export function scaleX(x: 0 | string) {
    return `scaleX(${x})`;
}

export function scaleY(y: 0 | string) {
    return `scaleY(${y})`;
}

export function scaleZ(z: 0 | string) {
    return `scaleZ(${z})`;
}

export function skew(x: 0 | string, y: 0 | string) {
    return `skew(${x}, ${y})`;
}

export function skewX(x: 0 | string) {
    return `skewX(${x})`;
}

export function skewY(y: 0 | string) {
    return `skewY(${y})`;
}

export function repeat(count: number | "auto-fill" | "auto-fit", expr: string) {
    return `repeat(${count}, ${expr})` as CssGridTemplateTrackRepeatValue;
}

export function fitContent(len: string) {
    return `fit-content(${len})`;
}

export function minMax(min: string, max: string) {
    return `minmax(${min}, ${max})`;
}

export function hash(r: number | string, g: number | string, b: number | string, a?: number | string) {
    return "#" + [r, g, b, a].filter(isDefined).join("") as CssColorHashValue;
}

function cssList(name: string, ...args: (number | string)[]) {
    return `${name}(${args.filter(isDefined).join(", ")})`;
}

export function rgb(red: number, green: number, blue: number, alpha?: number) {
    return cssList("rgb", red, green, blue, alpha) as CssColorRGBValue;
}

export function rgba(red: number, green: number, blue: number, alpha: number) {
    return cssList("rgba", red, green, blue, alpha) as CssColorRGBValue;
}

export function hsl(hue: string, saturation: string, lightness: string, alpha?: number | string) {
    return cssList("hsl", hue, saturation, lightness, alpha) as CssColorHSLValue;
}

export function hsla(hue: string, saturation: string, lightness: string, alpha: number | string) {
    return cssList("hsla", hue, saturation, lightness, alpha) as CssColorHSLValue;
}

export function hwb(hue: string, whiteness: string, blackness: string, alpha?: number | string) {
    if (isDefined(alpha)) {
        return `hwb(${hue} ${whiteness} ${blackness} / ${alpha})` as CssColorHWBValue;
    }
    else {
        return `hwb(${hue} ${whiteness} ${blackness})` as CssColorHWBValue;
    }
}

export function lch(lightness: string, chroma: number, hue: string, alpha?: number | string) {
    if (isDefined(alpha)) {
        return `lch(${lightness} ${chroma} ${hue} / ${alpha})` as CssColorLCHValue;
    }
    else {
        return `lch(${lightness} ${chroma} ${hue})` as CssColorLCHValue;
    }
}

export function lab(lightness: string, a: number, b: number, alpha?: number | string) {
    if (isDefined(alpha)) {
        return `lab(${lightness} ${a} ${b} / ${alpha})` as CssColorLabValue;
    }
    else {
        return `lab(${lightness} ${a} ${b})` as CssColorLabValue;;
    }
}

/**
 * A selection of fonts for preferred monospace rendering.
 **/
export function getMonospaceFonts() {
    return "ui-monospace, 'Droid Sans Mono', 'Cascadia Mono', 'Segoe UI Mono', 'Ubuntu Mono', 'Roboto Mono', Menlo, Monaco, Consolas, monospace";
}

/**
 * A selection of fonts for preferred monospace rendering.
 **/
export function getMonospaceFamily() {
    return fontFamily(getMonospaceFonts());
}

/**
 * A selection of fonts that should match whatever the user's operating system normally uses.
 **/
export function getSystemFonts() {
    return "system-ui, -apple-system, '.SFNSText-Regular', 'San Francisco', 'Segoe UI', 'Ubuntu', 'Roboto', 'Noto Sans', 'Droid Sans', sans-serif";
}

/**
 * A selection of fonts that should match whatever the user's operating system normally uses.
 **/
export function getSystemFamily() {
    return fontFamily(getSystemFonts());
}

/**
 * A selection of serif fonts.
 **/
export function getSerifFonts() {
    return "Georgia, Cambria, 'Times New Roman', Times, serif";
}

export function getSerifFamily() {
    return fontFamily(getSerifFonts());
}

export interface ICssProp {
    value: string;
    toString(): string;
}

export class Prop implements ICssProp {
    #value: string;

    constructor(value: string) {
        this.#value = value;
    }

    get value(): string { return this.#value; }

    toString(): string { return this.value; }
}

export class PropSet implements ICssProp {
    #selector: string;
    #pre;
    #props;
    #post;

    constructor(selector: string, pre: string, props: ICssProp[], post: string) {
        this.#selector = selector;
        this.#pre = pre;
        this.#props = props;
        this.#post = post;
    }

    get value(): string {
        return this.selector
            + this.#pre
            + this.#props.map(toString).join("\n    ")
            + this.#post;
    }

    get selector(): string { return this.#selector; }

    get _subProps(): ICssProp[] { return this.#props; }

    toString(): string { return this.value; }
}

class KeyValueProp extends Prop {
    #name;
    #sep;

    constructor(name: string, sep: string, value: string) {
        super(value);

        this.#name = name;
        this.#sep = sep;
    }

    get name() { return this.#name; }

    override toString() {
        return this.name
            + this.#sep
            + this.value
            + ";";
    }
}

class SelectorPropSet extends PropSet {
    #parent: SelectorPropSet = null;

    constructor(selector: string, props: ICssProp[]) {
        super(selector, " {\n    ", props, "\n}\n");
        for (const prop of props) {
            if (prop instanceof SelectorPropSet) {
                prop.nestUnder(this);
            }
        }
    }

    override get selector(): string {
        const superSelector = this.#parent?.selector ?? "";
        if (superSelector.length === 0) {
            return super.selector;
        }

        const superSelectorParts = superSelector.split(",");
        const selectorParts = super.selector.split(",");
        const nestedSelector = selectorParts
            .map(b => superSelectorParts
                .map(a => a + b)
                .join(","))
            .join(",");
        return nestedSelector;
    }

    nestUnder(parent: SelectorPropSet) {
        this.#parent = parent;
    }
}

class CssDeclareProp extends KeyValueProp {
    constructor(key: string, value: string) {
        super(key, ": ", value);
    }
}

interface ExtendedCSSStyleDeclaration extends CSSStyleDeclaration {
    textWrap: string;
    "aspect-ratio": string;
}

export class CssElementStyleProp extends AbstractAppliable<Node> implements ICssProp {
    #name;
    get name() { return this.#name; }

    #key;
    get key() { return this.#key; }

    #value: string;
    get value() { return this.#value + this.#priority; }

    #priority = "";
    important(v: boolean) {
        this.#priority = v ? " !important" : "";
        return this;
    }

    constructor(key: keyof ExtendedCSSStyleDeclaration & string, value: string | number) {
        super();
        this.#name = key.replace(/[A-Z]/g, (m) => `-${m.toLocaleLowerCase()}`);
        this.#key = key;
        this.#value = value.toString();
    }

    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    apply(elem: Node) {
        if (elem instanceof HTMLElement) {
            (elem.style as any)[this.key] = this.value + this.#priority;
        }
    }

    override toString() { return `${this.name}:${this.value};`; }
}

export function rule(selector: string, ...props: (CssElementStyleProp | SelectorPropSet)[]) {
    return new SelectorPropSet(selector, props);
}

function asInt(v: number | string): string {
    return isNumber(v) ? v.toFixed(0) : v;
}

export function alignItems(v: CssGlobalValue | CssJustifyAlignValue) { return new CssElementStyleProp("alignItems", v); }

export function alignContent(v: CssGlobalValue | CssJustifyAlignContentValue) { return new CssElementStyleProp("alignContent", v); }

export function alignSelf(v: CssGlobalValue | CssJustifyAlignSelfValue) { return new CssElementStyleProp("alignSelf", v); }

export function all(v: CssGlobalValue) { return new CssElementStyleProp("all", v); }

export function alignmentBaseline(v: CssAlignmentBaselineValue) { return new CssElementStyleProp("alignmentBaseline", v); }

export function animationDelay(v: CssGlobalValue): CssElementStyleProp;
export function animationDelay(...v: CssTimeValue[]): CssElementStyleProp;
export function animationDelay(...v: string[]) { return new CssElementStyleProp("animationDelay", v.join(", ")); }

export function animationDuration(v: CssGlobalValue): CssElementStyleProp;
export function animationDuration(...v: CssTimeValue[]): CssElementStyleProp;
export function animationDuration(...v: string[]) { return new CssElementStyleProp("animationDuration", v.join(", ")); }

export function animationDirection(v: CssGlobalValue): CssElementStyleProp;
export function animationDirection(...v: CssAnimationDirectionValue[]): CssElementStyleProp;
export function animationDirection(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("animationDirection", v.join(", ")); }

export function animationFillMode(v: CssGlobalValue): CssElementStyleProp;
export function animationFillMode(...v: CssAnimationFillModeValue[]): CssElementStyleProp;
export function animationFillMode(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("animationFillMode", v.join(", ")); }

export function animationIterationCount(v: CssGlobalValue): CssElementStyleProp;
export function animationIterationCount(...v: CssAnimationIterationCountValue[]): CssElementStyleProp;
export function animationIterationCount(...v: (number | string)[]): CssElementStyleProp { return new CssElementStyleProp("animationIterationCount", v.join(", ")); }

export function animationName(v: CssGlobalValue): CssElementStyleProp;
export function animationName(...v: string[]): CssElementStyleProp;
export function animationName(...v: string[]) { return new CssElementStyleProp("animationName", v.join(", ")); }

export function animationPlayState(v: CssGlobalValue): CssElementStyleProp;
export function animationPlayState(...v: CssAnimationPlayStateValue[]): CssElementStyleProp;
export function animationPlayState(...v: string[]) { return new CssElementStyleProp("animationPlayState", v.join(", ")); }

export function animationTimingFunction(v: CssGlobalValue): CssElementStyleProp;
export function animationTimingFunction(...v: CssAnimationTimingFunctionValue[]): CssElementStyleProp;
export function animationTimingFunction(...v: string[]) { return new CssElementStyleProp("animationTimingFunction", v.join(" ")); }

export function animation(v: CssGlobalValue): CssElementStyleProp;
export function animation(...v: (string | number)[]): CssElementStyleProp;
export function animation(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("animation", v.join(" ")); }

export function appearance(v: CssGlobalValue | CssAppearanceValue) { return new CssElementStyleProp("appearance", v); }

export function aspectRatio(v: CssGlobalValue | number | `${number}/${number}`) { return new CssElementStyleProp("aspect-ratio", v.toString()); }

export function backdropFilter(v: "none" | CssGlobalValue): CssElementStyleProp;
export function backdropFilter(...v: CssFilterFunction[]): CssElementStyleProp;
export function backdropFilter(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("backdropFilter" as any, v.join(" ")); }

export function backfaceVisibility(v: CssGlobalValue | CssBackfaceVisibilityValue) { return new CssElementStyleProp("backfaceVisibility", v); }

export function backgroundAttachment(v: CssGlobalValue | CssBackgroundAttachmentValue) { return new CssElementStyleProp("backgroundAttachment", v); }

export function backgroundBlendMode(v: CssGlobalValue): CssElementStyleProp;
export function backgroundBlendMode(...v: CssBlendModeValue[]): CssElementStyleProp;
export function backgroundBlendMode(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("backgroundBlendMode", v.join(", ")); }

export function backgroundClip(v: CssGlobalValue | CssClipValue): CssElementStyleProp { return new CssElementStyleProp("backgroundClip", v); }

export function backgroundColor(v: string) { return new CssElementStyleProp("backgroundColor", v); }

export function backgroundImage(v: CssGlobalValue): CssElementStyleProp;
export function backgroundImage(...v: CssImage[]): CssElementStyleProp;
export function backgroundImage(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("backgroundImage", v.join(", ")); }

export function backgroundOrigin(v: CssGlobalValue | CssBoxType): CssElementStyleProp { return new CssElementStyleProp("backgroundOrigin", v); }

export function backgroundPosition(v: CssGlobalValue): CssElementStyleProp;
export function backgroundPosition(v: CssBackgroundPositionValue): CssElementStyleProp;
export function backgroundPosition(x: CssLengthPercentage, y: CssLengthPercentage): CssElementStyleProp;
export function backgroundPosition(x: CssBackgroundPositionHorizontalKeyword, y: CssBackgroundPositionVerticalValue): CssElementStyleProp;
export function backgroundPosition(y: CssBackgroundPositionVerticalKeyword, x: CssBackgroundPositionHorizontalValue): CssElementStyleProp;
export function backgroundPosition(x: CssBackgroundPositionHorizontalKeyword, y: CssBackgroundPositionVerticalKeyword, yOffset: CssLengthPercentage): CssElementStyleProp;
export function backgroundPosition(x: CssBackgroundPositionHorizontalKeyword, xOffset: CssLengthPercentage, y: CssBackgroundPositionVerticalKeyword): CssElementStyleProp;
export function backgroundPosition(y: CssBackgroundPositionVerticalKeyword, x: CssBackgroundPositionHorizontalKeyword, xOffset: CssLengthPercentage): CssElementStyleProp;
export function backgroundPosition(y: CssBackgroundPositionVerticalKeyword, yOffset: CssLengthPercentage, x: CssBackgroundPositionHorizontalKeyword): CssElementStyleProp;
export function backgroundPosition(x: CssBackgroundPositionHorizontalKeyword, xOffset: CssLengthPercentage, y: CssBackgroundPositionVerticalKeyword, yOffset: CssLengthPercentage): CssElementStyleProp;
export function backgroundPosition(y: CssBackgroundPositionVerticalKeyword, yOffset: CssLengthPercentage, x: CssBackgroundPositionHorizontalKeyword, xOffset: CssLengthPercentage): CssElementStyleProp;
export function backgroundPosition(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundPosition", v.join(" ")); }

export function backgroundPositionX(v: CssGlobalValue): CssElementStyleProp;
export function backgroundPositionX(x: CssBackgroundPositionHorizontalValue): CssElementStyleProp;
export function backgroundPositionX(x: CssBackgroundPositionHorizontalKeyword, xOffset: CssLengthPercentage): CssElementStyleProp;
export function backgroundPositionX(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundPositionX", v.join(" ")); }

export function backgroundPositionY(v: CssGlobalValue): CssElementStyleProp;
export function backgroundPositionY(y: CssBackgroundPositionVerticalValue): CssElementStyleProp;
export function backgroundPositionY(y: CssBackgroundPositionVerticalKeyword, yOffset: CssLengthPercentage): CssElementStyleProp;
export function backgroundPositionY(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundPositionY", v.join(" ")); }

export function backgroundRepeat(v: CssGlobalValue): CssElementStyleProp;
export function backgroundRepeat(v: CssBackgroundRepeatValue): CssElementStyleProp;
export function backgroundRepeat(x: CssBackgroundRepeat, y: CssBackgroundRepeat): CssElementStyleProp;
export function backgroundRepeat(...v: string[]) { return new CssElementStyleProp("backgroundRepeat", v.join(" ")); }

export function backgroundSize(v: CssGlobalValue | CssBackgroundSizeSingleValue): CssElementStyleProp;
export function backgroundSize(x: CssBackgroundSizeValue, y: CssBackgroundSizeValue): CssElementStyleProp;
export function backgroundSize(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundSize", v.join(" ")); }

export function background(v: CssGlobalValue): CssElementStyleProp;
export function background(...v: string[]): CssElementStyleProp;
export function background(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("background", v.join(", ")); }

export function baselineShift(v: string) { return new CssElementStyleProp("baselineShift", v); }

export function blockSize(v: CssGlobalValue | CssSizePropertyValue) { return new CssElementStyleProp("blockSize", v); }

export function border(v: string | 0) { return new CssElementStyleProp("border", v); }

export function borderBlockEnd(v: string) { return new CssElementStyleProp("borderBlockEnd", v); }

export function borderBlockEndColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderBlockEndColor", v); }

export function borderBlockEndStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderBlockEndStyle", v); }

export function borderBlockEndWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderBlockEndWidth", v); }

export function borderBlockStart(v: string) { return new CssElementStyleProp("borderBlockStart", v); }

export function borderBlockStartColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderBlockStartColor", v); }

export function borderBlockStartStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderBlockStartStyle", v); }

export function borderBlockStartWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderBlockStartWidth", v); }

export function borderBottom(v: string) { return new CssElementStyleProp("borderBottom", v); }

export function borderBottomColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderBottomColor", v); }

export function borderBottomLeftRadius(v: CssGlobalValue): CssElementStyleProp;
export function borderBottomLeftRadius(v: CssLengthPercentage): CssElementStyleProp;
export function borderBottomLeftRadius(major: CssLengthPercentage, minor: CssLengthPercentage): CssElementStyleProp;
export function borderBottomLeftRadius(...v: (string | number)[]) { return new CssElementStyleProp("borderBottomLeftRadius", v.join(" ")); }

export function borderBottomRightRadius(v: CssGlobalValue): CssElementStyleProp;
export function borderBottomRightRadius(v: CssLengthPercentage): CssElementStyleProp;
export function borderBottomRightRadius(major: CssLengthPercentage, minor: CssLengthPercentage): CssElementStyleProp;
export function borderBottomRightRadius(...v: (string | number)[]) { return new CssElementStyleProp("borderBottomRightRadius", v.join(" ")); }

export function borderBottomStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderBottomStyle", v); }

export function borderBottomWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderBottomWidth", v); }

export function borderCollapse(v: CssGlobalValue | CssBorderCollapseValue) { return new CssElementStyleProp("borderCollapse", v); }

export function borderColor(v: CssGlobalValue): CssElementStyleProp;
export function borderColor(all: CssColorValue): CssElementStyleProp;
export function borderColor(vertical: CssColorValue, horizontal: CssColorValue): CssElementStyleProp;
export function borderColor(top: CssColorValue, horizontal: CssColorValue, bottom: CssColorValue): CssElementStyleProp;
export function borderColor(top: CssColorValue, right: CssColorValue, bottom: CssColorValue, left: CssColorValue): CssElementStyleProp;
export function borderColor(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("borderColor", v.join(" ")); }

export function borderImage(v: string) { return new CssElementStyleProp("borderImage", v); }

export function borderImageOutset(v: CssGlobalValue): CssElementStyleProp;
export function borderImageOutset(all: CssLength | number): CssElementStyleProp;
export function borderImageOutset(vertical: CssLength | number, horizontal: CssLength | number): CssElementStyleProp;
export function borderImageOutset(top: CssLength | number, horizontal: CssLength | number, bottom: CssLength | number): CssElementStyleProp;
export function borderImageOutset(top: CssLength | number, right: CssLength | number, bottom: CssLength | number, left: CssLength | number): CssElementStyleProp;
export function borderImageOutset(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("borderImageOutset", v.join(" ")); }

export function borderImageRepeat(v: CssGlobalValue): CssElementStyleProp;
export function borderImageRepeat(all: CssBorderRepeatValue): CssElementStyleProp;
export function borderImageRepeat(vertical: CssBorderRepeatValue, horizontal: CssBorderRepeatValue): CssElementStyleProp;
export function borderImageRepeat(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("borderImageRepeat", v.join(" ")); }

export function borderImageSlice(v: CssGlobalValue): CssElementStyleProp;
export function borderImageSlice(all: CssLengthPercentage | "fill"): CssElementStyleProp;
export function borderImageSlice(vertical: CssLengthPercentage | "fill", horizontal: CssLengthPercentage | "fill"): CssElementStyleProp;
export function borderImageSlice(top: CssLengthPercentage | "fill", horizontal: CssLengthPercentage | "fill", bottom: CssLengthPercentage | "fill"): CssElementStyleProp;
export function borderImageSlice(top: CssLengthPercentage | "fill", right: CssLengthPercentage | "fill", bottom: CssLengthPercentage | "fill", left: CssLengthPercentage | "fill"): CssElementStyleProp;
export function borderImageSlice(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("borderImageSlice", v.join(" ")); }

export function borderImageSource(v: CssGlobalValue | CssImage | "none") { return new CssElementStyleProp("borderImageSource", v); }

export function borderImageWidth(v: CssGlobalValue): CssElementStyleProp;
export function borderImageWidth(v: CssLengthPercentage | "auto"): CssElementStyleProp;
export function borderImageWidth(vertical: CssLengthPercentage | "auto", horizontal: CssLengthPercentage | "auto"): CssElementStyleProp;
export function borderImageWidth(top: CssLengthPercentage | "auto", horizontal: CssLengthPercentage | "auto", bottom: CssLengthPercentage | "auto"): CssElementStyleProp;
export function borderImageWidth(top: CssLengthPercentage | "auto", right: CssLengthPercentage | "auto", bottom: CssLengthPercentage | "auto", left: CssLengthPercentage | "auto"): CssElementStyleProp;
export function borderImageWidth(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("borderImageWidth", v.join(" ")); }

export function borderInlineEnd(v: string) { return new CssElementStyleProp("borderInlineEnd", v); }

export function borderInlineEndColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderInlineEndColor", v); }

export function borderInlineEndStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderInlineEndStyle", v); }

export function borderInlineEndWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderInlineEndWidth", v); }

export function borderInlineStart(v: string) { return new CssElementStyleProp("borderInlineStart", v); }

export function borderInlineStartColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderInlineStartColor", v); }

export function borderInlineStartStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderInlineStartStyle", v); }

export function borderInlineStartWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderInlineStartWidth", v); }

export function borderLeft(v: string) { return new CssElementStyleProp("borderLeft", v); }

export function borderLeftColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderLeftColor", v); }

export function borderLeftStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderLeftStyle", v); }

export function borderLeftWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderLeftWidth", v); }

export function borderRadius(v: CssGlobalValue): CssElementStyleProp;
export function borderRadius(all: CssLengthPercentage): CssElementStyleProp;
export function borderRadius(all: [CssLengthPercentage, CssLengthPercentage]): CssElementStyleProp;
export function borderRadius(topLeftBottomRight: CssLengthPercentage, topRightBottomLeft: CssLengthPercentage): CssElementStyleProp;
export function borderRadius(topLeft: CssLengthPercentage, topRightBottomLeft: CssLengthPercentage, bottomRight: CssLengthPercentage): CssElementStyleProp;
export function borderRadius(topLeft: CssLengthPercentage, topRight: CssLengthPercentage, bottomRight: CssLengthPercentage, bottomLeft: CssLengthPercentage): CssElementStyleProp;
export function borderRadius(...v: (string | number | (string | number)[])[]): CssElementStyleProp {
    let value: string = null;
    if (v.length === 0) {
        value = "";
    }
    else if (isArray(v[0]) && isArray(v[1])) {
        const left = v[0].join(" ");
        const right = v[1].join(" ");
        value = `${left} / ${right}`;
    }
    else {
        value = v.join(" ");
    }
    return new CssElementStyleProp("borderRadius", value);
}

export function borderRight(v: string) { return new CssElementStyleProp("borderRight", v); }

export function borderRightColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderRightColor", v); }

export function borderRightStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderRightStyle", v); }

export function borderRightWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderRightWidth", v); }

export function borderSpacing(v: string) { return new CssElementStyleProp("borderSpacing", v); }

export function borderStyle(v: CssGlobalValue): CssElementStyleProp;
export function borderStyle(all: CssBorderStyleValue): CssElementStyleProp;
export function borderStyle(vertical: CssBorderStyleValue, horizontal: CssBorderStyleValue): CssElementStyleProp;
export function borderStyle(top: CssBorderStyleValue, horizontal: CssBorderStyleValue, bottom: CssBorderStyleValue): CssElementStyleProp;
export function borderStyle(top: CssBorderStyleValue, right: CssBorderStyleValue, bottom: CssBorderStyleValue, left: CssBorderStyleValue): CssElementStyleProp;
export function borderStyle(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("borderStyle", v.join(" ")); }

export function borderTop(v: string) { return new CssElementStyleProp("borderTop", v); }

export function borderTopColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("borderTopColor", v); }

export function borderTopLeftRadius(v: CssGlobalValue): CssElementStyleProp;
export function borderTopLeftRadius(v: CssLengthPercentage): CssElementStyleProp;
export function borderTopLeftRadius(major: CssLengthPercentage, minor: CssLengthPercentage): CssElementStyleProp;
export function borderTopLeftRadius(...v: (string | number)[]) { return new CssElementStyleProp("borderTopLeftRadius", v.join(" ")); }

export function borderTopRightRadius(v: CssGlobalValue): CssElementStyleProp;
export function borderTopRightRadius(v: CssLengthPercentage): CssElementStyleProp;
export function borderTopRightRadius(major: CssLengthPercentage, minor: CssLengthPercentage): CssElementStyleProp;
export function borderTopRightRadius(...v: (string | number)[]) { return new CssElementStyleProp("borderTopRightRadius", v.join(" ")); }

export function borderTopStyle(v: CssGlobalValue | CssBorderStyleValue) { return new CssElementStyleProp("borderTopStyle", v); }

export function borderTopWidth(v: CssGlobalValue | CssLengthPercentage | CssWidthKeyword | "auto") { return new CssElementStyleProp("borderTopWidth", v); }

export function borderWidth(v: CssGlobalValue): CssElementStyleProp;
export function borderWidth(all: CssLengthPercentage | CssWidthKeyword | "auto"): CssElementStyleProp;
export function borderWidth(vert: CssLengthPercentage | CssWidthKeyword | "auto", horiz: CssLengthPercentage | CssWidthKeyword | "auto"): CssElementStyleProp;
export function borderWidth(top: CssLengthPercentage | CssWidthKeyword | "auto", horiz: CssLengthPercentage | CssWidthKeyword | "auto", bottom: CssLengthPercentage | CssWidthKeyword | "auto"): CssElementStyleProp;
export function borderWidth(top: CssLengthPercentage | CssWidthKeyword | "auto", right: CssLengthPercentage | CssWidthKeyword | "auto", bottom: CssLengthPercentage | CssWidthKeyword | "auto", left: CssLengthPercentage | CssWidthKeyword | "auto"): CssElementStyleProp;
export function borderWidth(...v: (string | number)[]) { return new CssElementStyleProp("borderWidth", v.join(" ")); }

export function bottom(v: CssGlobalValue | CssElementPositionValue) { return new CssElementStyleProp("bottom", v); }

export function boxShadow(v: string) { return new CssElementStyleProp("boxShadow", v); }

export function boxSizing(v: string) { return new CssElementStyleProp("boxSizing", v); }

export function breakAfter(v: string) { return new CssElementStyleProp("breakAfter", v); }

export function breakBefore(v: string) { return new CssElementStyleProp("breakBefore", v); }

export function breakInside(v: string) { return new CssElementStyleProp("breakInside", v); }

export function captionSide(v: string) { return new CssElementStyleProp("captionSide", v); }

export function caretColor(v: string) { return new CssElementStyleProp("caretColor", v); }

export function clear(v: string) { return new CssElementStyleProp("clear", v); }

export function clip(v: string) { return new CssElementStyleProp("clip", v); }

export function clipPath(v: string) { return new CssElementStyleProp("clipPath", v); }

export function clipRule(v: string) { return new CssElementStyleProp("clipRule", v); }

export function color(v: string): CssElementStyleProp { return new CssElementStyleProp("color", v); }

export function colorInterpolation(v: string) { return new CssElementStyleProp("colorInterpolation", v); }

export function colorInterpolationFilters(v: string) { return new CssElementStyleProp("colorInterpolationFilters", v); }

export function colorScheme(v: string) { return new CssElementStyleProp("colorScheme", v); }

export function columnCount(v: string) { return new CssElementStyleProp("columnCount", v); }

export function columnFill(v: string) { return new CssElementStyleProp("columnFill", v); }

export function columnGap(v: CssGlobalValue | CssLengthPercentage | CssCalcStatement): CssElementStyleProp { return new CssElementStyleProp("columnGap", v); }
export function gridColumnGap(v: CssGlobalValue | CssLengthPercentage | CssCalcStatement) { return new CssElementStyleProp("gridColumnGap", v); }

export function columnRule(v: string) { return new CssElementStyleProp("columnRule", v); }

export function columnRuleColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("columnRuleColor", v); }

export function columnRuleStyle(v: string) { return new CssElementStyleProp("columnRuleStyle", v); }

export function columnRuleWidth(v: CssGlobalValue | CssLength | CssWidthKeyword) { return new CssElementStyleProp("columnRuleWidth", v); }

export function columnSpan(v: number) { return new CssElementStyleProp("columnSpan", v); }

export function columnWidth(v: CssGlobalValue | CssLength | "auto") { return new CssElementStyleProp("columnWidth", v); }

export function columns(v: string) { return new CssElementStyleProp("columns", v); }

export function contain(v: string) { return new CssElementStyleProp("contain", v); }

export function content(v: string) { return new CssElementStyleProp("content", v); }

export function counterIncrement(v: string) { return new CssElementStyleProp("counterIncrement", v); }

export function counterReset(v: string) { return new CssElementStyleProp("counterReset", v); }

export function cssVarDecl(name: string, value: string) { return new CssElementStyleProp("--" + name as any, value); }

export function cursor(v: CssGlobalValue | CssCursorValue) { return new CssElementStyleProp("cursor", v); }

export function direction(v: CssGlobalValue | CssDirectionValue) { return new CssElementStyleProp("direction", v); }

export function display(v: CssGlobalValue | CssDisplayValue) { return new CssElementStyleProp("display", v); }

export function dominantBaseline(v: string) { return new CssElementStyleProp("dominantBaseline", v); }

export function emptyCells(v: string) { return new CssElementStyleProp("emptyCells", v); }

export function fill(v: string) { return new CssElementStyleProp("fill", v); }

export function fillOpacity(v: string) { return new CssElementStyleProp("fillOpacity", v); }

export function fillRule(v: string) { return new CssElementStyleProp("fillRule", v); }

export function filter(v: string) { return new CssElementStyleProp("filter", v); }

export function flexBasis(v: CssGlobalValue | CssFlexBasisValue) { return new CssElementStyleProp("flexBasis", v); }

export function flexDirection(v: CssGlobalValue | CssFlexDirectionValue) { return new CssElementStyleProp("flexDirection", v); }

export function flexWrap(v: CssGlobalValue | CssFlexWrapValue) { return new CssElementStyleProp("flexWrap", v); }

export function flexFlow(v: CssGlobalValue): CssElementStyleProp;
export function flexFlow(v: CssFlexFlowValue): CssElementStyleProp;
export function flexFlow(x: CssFlexFlowValue, y: CssFlexFlowValue): CssElementStyleProp;
export function flexFlow(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("flexFlow", v.join(" ")); }

export function flex(v: CssGlobalValue): CssElementStyleProp;
export function flex(grow: number): CssElementStyleProp;
export function flex(basis: CssFlexBasisValue): CssElementStyleProp;
export function flex(grow: number, shrink: number): CssElementStyleProp;
export function flex(grow: number, basis: CssFlexBasisValue): CssElementStyleProp;
export function flex(grow: number, shrink: number, basis: CssFlexBasisValue): CssElementStyleProp;
export function flex(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("flex", v.join(" ")); }

export function flexGrow(v: CssGlobalValue | number) { return new CssElementStyleProp("flexGrow", v); }

export function flexShrink(v: CssGlobalValue | number) { return new CssElementStyleProp("flexShrink", v); }

export function float(v: CssGlobalValue | CssFloatValue) { return new CssElementStyleProp("float", v); }

export function floodColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("floodColor", v); }

export function floodOpacity(v: string) { return new CssElementStyleProp("floodOpacity", v); }

export function font(v: string) { return new CssElementStyleProp("font", v); }

export function fontFamily(v: string) { return new CssElementStyleProp("fontFamily", v); }

export function fontFeatureSettings(v: string) { return new CssElementStyleProp("fontFeatureSettings", v); }

export function fontKerning(v: string) { return new CssElementStyleProp("fontKerning", v); }

export function fontOpticalSizing(v: string) { return new CssElementStyleProp("fontOpticalSizing", v); }

export function fontSize(v: string) { return new CssElementStyleProp("fontSize", v); }

export function fontStretch(v: string) { return new CssElementStyleProp("fontStretch", v); }

export function fontStyle(v: string) { return new CssElementStyleProp("fontStyle", v); }

export function fontVariant(v: string) { return new CssElementStyleProp("fontVariant", v); }

export function fontVariantCaps(v: string) { return new CssElementStyleProp("fontVariantCaps", v); }

export function fontVariantEastAsian(v: string) { return new CssElementStyleProp("fontVariantEastAsian", v); }

export function fontVariantLigatures(v: string) { return new CssElementStyleProp("fontVariantLigatures", v); }

export function fontVariantNumeric(v: string) { return new CssElementStyleProp("fontVariantNumeric", v); }

export function fontVariationSettings(v: string) { return new CssElementStyleProp("fontVariationSettings", v); }

export function fontWeight(v: string) { return new CssElementStyleProp("fontWeight", v); }

export function gap(v: CssGlobalValue | CssLengthPercentage | CssCalcStatement): CssElementStyleProp;
export function gap(row: CssLengthPercentage | CssCalcStatement, column: CssLengthPercentage | CssCalcStatement): CssElementStyleProp
export function gap(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("gap", v.join(" ")); }
export function gridGap(v: CssGlobalValue | CssLengthPercentage | CssCalcStatement): CssElementStyleProp;
export function gridGap(row: CssLengthPercentage | CssCalcStatement, column: CssLengthPercentage | CssCalcStatement): CssElementStyleProp
export function gridGap(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("gridGap", v.join(" ")); }

export function grid(v: string) { return new CssElementStyleProp("grid", v); }

export function gridArea(v: string): CssElementStyleProp;
export function gridArea(rowStart: number, colStart?: number, rowEnd?: number, colEnd?: number): CssElementStyleProp;
export function gridArea(vOrRowStart: string | number, colStart?: number, rowEnd?: number, colEnd?: number): CssElementStyleProp {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, colStart, rowEnd, colEnd]
            .filter(isDefined)
            .join("/");
    }

    return new CssElementStyleProp("gridArea", vOrRowStart);
}

export function gridAutoColumns(v: string) { return new CssElementStyleProp("gridAutoColumns", v); }

export function gridAutoFlow(v: CssGlobalValue | CssGridAutoFlowValue) { return new CssElementStyleProp("gridAutoFlow", v); }

export function gridAutoRows(v: string) { return new CssElementStyleProp("gridAutoRows", v); }

export function gridColumn(v: string): CssElementStyleProp
export function gridColumn(colStart: number, colEnd?: number): CssElementStyleProp
export function gridColumn(vOrColStart: string | number, colEnd?: number): CssElementStyleProp {
    if (!isString(vOrColStart)) {
        vOrColStart = [vOrColStart, colEnd]
            .filter(isDefined)
            .join("/");
    }
    return new CssElementStyleProp("gridColumn", vOrColStart);
}

export function gridColumnEnd(v: string | number) { return new CssElementStyleProp("gridColumnEnd", v); }

export function gridColumnStart(v: string | number) { return new CssElementStyleProp("gridColumnStart", v); }

export function gridRow(v: string): CssElementStyleProp
export function gridRow(rowStart: number, rowEnd?: number): CssElementStyleProp
export function gridRow(vOrRowStart: string | number, rowEnd?: number): CssElementStyleProp {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, rowEnd]
            .filter(isDefined)
            .join("/");
    }
    return new CssElementStyleProp("gridRow", vOrRowStart);
}

export function gridRowEnd(v: string | number) { return new CssElementStyleProp("gridRowEnd", v); }

export function gridRowStart(v: string | number) { return new CssElementStyleProp("gridRowStart", v); }

export function gridTemplate(v: string) { return new CssElementStyleProp("gridTemplate", v); }

export function gridTemplateAreas(...v: string[]) { return new CssElementStyleProp("gridTemplateAreas", v.map((r) => "\"" + r + "\"").join("\n")); }

export function gridTemplateColumns(v: CssGlobalValue): CssElementStyleProp;
export function gridTemplateColumns(...v: CssGridTemplateTrackValue[]): CssElementStyleProp;
export function gridTemplateColumns(...v: (string | number)[]) { return new CssElementStyleProp("gridTemplateColumns", v.join(" ")); }

export function gridTemplateRows(v: CssGlobalValue): CssElementStyleProp;
export function gridTemplateRows(...v: CssGridTemplateTrackValue[]): CssElementStyleProp;
export function gridTemplateRows(...v: (string | number)[]) { return new CssElementStyleProp("gridTemplateRows", v.join(" ")); }

export function height(v: CssGlobalValue | CssSizePropertyValue) { return new CssElementStyleProp("height", v); }

export function hyphens(v: string) { return new CssElementStyleProp("hyphens", v); }

export function imageOrientation(v: string) { return new CssElementStyleProp("imageOrientation", v); }

export function imageRendering(v: string) { return new CssElementStyleProp("imageRendering", v); }

export function inlineSize(v: string) { return new CssElementStyleProp("inlineSize", v); }

export function inset(v: CssGlobalValue | CssLengthPercentage): CssElementStyleProp;
export function inset(vert: CssLengthPercentage, horiz: CssLengthPercentage): CssElementStyleProp;
export function inset(top: CssLengthPercentage, horiz: CssLengthPercentage, bot: CssLengthPercentage): CssElementStyleProp;
export function inset(top: CssLengthPercentage, right: CssLengthPercentage, bot: CssLengthPercentage, left: CssLengthPercentage): CssElementStyleProp;
export function inset(...v: (CssGlobalValue | CssLengthPercentage)[]) { return new CssElementStyleProp("inset", v.join(" ")); }
export function insetBlock(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("insetBlock", v); }
export function insetBlockEnd(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("insetBlockEnd", v); }
export function insetBlockStart(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("insetBlockStart", v); }
export function insetInline(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("insetInline", v); }
export function insetInlineEnd(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("insetInlineEnd", v); }
export function insetInlineStart(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("insetInlineStart", v); }

export function isolation(v: string) { return new CssElementStyleProp("isolation", v); }

export function justifyContent(v: CssGlobalValue | CssJustifyAlignContentValue) { return new CssElementStyleProp("justifyContent", v); }

export function justifyItems(v: CssGlobalValue | CssJustifyItemsValue) { return new CssElementStyleProp("justifyItems", v); }

export function justifySelf(v: CssGlobalValue | CssJustifyAlignSelfValue) { return new CssElementStyleProp("justifySelf", v); }

export function left(v: CssElementPositionValue) { return new CssElementStyleProp("left", v); }

export function letterSpacing(v: string) { return new CssElementStyleProp("letterSpacing", v); }

export function lightingColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("lightingColor", v); }

export function lineBreak(v: string) { return new CssElementStyleProp("lineBreak", v); }

export function lineHeight(v: string) { return new CssElementStyleProp("lineHeight", v); }

export function listStyle(v: string) { return new CssElementStyleProp("listStyle", v); }

export function listStyleImage(v: string) { return new CssElementStyleProp("listStyleImage", v); }

export function listStylePosition(v: string) { return new CssElementStyleProp("listStylePosition", v); }

export function listStyleType(v: string) { return new CssElementStyleProp("listStyleType", v); }

export function margin(v: CssGlobalValue | CssLengthPercentage | "auto"): CssElementStyleProp;
export function margin(vert: CssLengthPercentage | "auto", horiz: CssLengthPercentage | "auto"): CssElementStyleProp;
export function margin(top: CssLengthPercentage | "auto", horiz: CssLengthPercentage | "auto", bot: CssLengthPercentage | "auto"): CssElementStyleProp;
export function margin(top: CssLengthPercentage | "auto", right: CssLengthPercentage | "auto", bot: CssLengthPercentage | "auto", left: CssLengthPercentage | "auto"): CssElementStyleProp;
export function margin(...v: (number | string)[]) { return new CssElementStyleProp("margin", v.join(" ")); }

export function marginBlockEnd(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginBlockEnd", v); }

export function marginBlockStart(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginBlockStart", v); }

export function marginBottom(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginBottom", v); }

export function marginInlineEnd(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginInlineEnd", v); }

export function marginInlineStart(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginInlineStart", v); }

export function marginLeft(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginLeft", v); }

export function marginRight(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginRight", v); }

export function marginTop(v: CssLengthPercentage | "auto") { return new CssElementStyleProp("marginTop", v); }

export function marker(v: string) { return new CssElementStyleProp("marker", v); }

export function markerEnd(v: string) { return new CssElementStyleProp("markerEnd", v); }

export function markerMid(v: string) { return new CssElementStyleProp("markerMid", v); }

export function markerStart(v: string) { return new CssElementStyleProp("markerStart", v); }

export function mask(v: string) { return new CssElementStyleProp("mask", v); }

export function maskType(v: string) { return new CssElementStyleProp("maskType", v); }

export function maxBlockSize(v: string) { return new CssElementStyleProp("maxBlockSize", v); }

export function maxHeight(v: string | 0) { return new CssElementStyleProp("maxHeight", v); }

export function maxInlineSize(v: string) { return new CssElementStyleProp("maxInlineSize", v); }

export function maxWidth(v: string | 0) { return new CssElementStyleProp("maxWidth", v); }

export function minBlockSize(v: string) { return new CssElementStyleProp("minBlockSize", v); }

export function minHeight(v: string | 0) { return new CssElementStyleProp("minHeight", v); }

export function minInlineSize(v: string) { return new CssElementStyleProp("minInlineSize", v); }

export function minWidth(v: string | 0) { return new CssElementStyleProp("minWidth", v); }

export function mixBlendMode(v: string) { return new CssElementStyleProp("mixBlendMode", v); }

export function objectFit(v: string) { return new CssElementStyleProp("objectFit", v); }

export function objectPosition(v: string) { return new CssElementStyleProp("objectPosition", v); }

export function offset(v: string) { return new CssElementStyleProp("offset", v); }

export function offsetDistance(v: string) { return new CssElementStyleProp("offsetDistance", v); }

export function offsetPath(v: string) { return new CssElementStyleProp("offsetPath", v); }

export function offsetRotate(v: string) { return new CssElementStyleProp("offsetRotate", v); }

export function opacity(v: CssGlobalValue | CssAlpha) { return new CssElementStyleProp("opacity", v); }

export function order(v: string) { return new CssElementStyleProp("order", v); }

export function orphans(v: string) { return new CssElementStyleProp("orphans", v); }

export function outline(v: string) { return new CssElementStyleProp("outline", v); }

export function outlineColor(v: CssGlobalValue | CssColorValue | "invert") { return new CssElementStyleProp("outlineColor", v); }

export function outlineOffset(v: string) { return new CssElementStyleProp("outlineOffset", v); }

export function outlineStyle(v: string) { return new CssElementStyleProp("outlineStyle", v); }

export function outlineWidth(v: string | 0) { return new CssElementStyleProp("outlineWidth", v); }

export function overflow(v: CssGlobalValue | CssOverflowValue): CssElementStyleProp;
export function overflow(x: CssOverflowValue, y: CssOverflowValue): CssElementStyleProp;
export function overflow(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("overflow", v.join(" ")); }

export function overflowX(v: CssGlobalValue | CssOverflowValue) { return new CssElementStyleProp("overflowX", v); }

export function overflowY(v: CssGlobalValue | CssOverflowValue) { return new CssElementStyleProp("overflowY", v); }

export function overflowAnchor(v: string) { return new CssElementStyleProp("overflowAnchor", v); }

export function overflowWrap(v: string) { return new CssElementStyleProp("overflowWrap", v); }

export function overscrollBehavior(v: string) { return new CssElementStyleProp("overscrollBehavior", v); }

export function overscrollBehaviorBlock(v: string) { return new CssElementStyleProp("overscrollBehaviorBlock", v); }

export function overscrollBehaviorInline(v: string) { return new CssElementStyleProp("overscrollBehaviorInline", v); }

export function overscrollBehaviorX(v: string) { return new CssElementStyleProp("overscrollBehaviorX", v); }

export function overscrollBehaviorY(v: string) { return new CssElementStyleProp("overscrollBehaviorY", v); }

export function padding(v: CssGlobalValue | CssLengthPercentage): CssElementStyleProp;
export function padding(vert: CssLengthPercentage, horiz: CssLengthPercentage): CssElementStyleProp;
export function padding(top: CssLengthPercentage, horiz: CssLengthPercentage, bot: CssLengthPercentage): CssElementStyleProp;
export function padding(top: CssLengthPercentage, right: CssLengthPercentage, bot: CssLengthPercentage, left: CssLengthPercentage): CssElementStyleProp;
export function padding(...v: (CssGlobalValue | CssLengthPercentage)[]) { return new CssElementStyleProp("padding", v.join(" ")); }

export function paddingBlockEnd(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingBlockEnd", v); }

export function paddingBlockStart(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingBlockStart", v); }

export function paddingBottom(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingBottom", v); }

export function paddingInlineEnd(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingInlineEnd", v); }

export function paddingInlineStart(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingInlineStart", v); }

export function paddingLeft(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingLeft", v); }

export function paddingRight(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingRight", v); }

export function paddingTop(v: CssGlobalValue | CssLengthPercentage) { return new CssElementStyleProp("paddingTop", v); }

export function pageBreakAfter(v: string) { return new CssElementStyleProp("pageBreakAfter", v); }

export function pageBreakBefore(v: string) { return new CssElementStyleProp("pageBreakBefore", v); }

export function pageBreakInside(v: string) { return new CssElementStyleProp("pageBreakInside", v); }

export function paintOrder(v: string) { return new CssElementStyleProp("paintOrder", v); }

export function perspective(v: string) { return new CssElementStyleProp("perspective", v); }

export function perspectiveOrigin(v: string) { return new CssElementStyleProp("perspectiveOrigin", v); }

export function placeContent(v: string) { return new CssElementStyleProp("placeContent", v); }

export function placeItems(v: string) { return new CssElementStyleProp("placeItems", v); }

export function placeSelf(v: string) { return new CssElementStyleProp("placeSelf", v); }

export function pointerEvents(v: CssGlobalValue | CssPointerEventsValue) { return new CssElementStyleProp("pointerEvents", v); }

export function position(v: CssGlobalValue | CssPositionValue) { return new CssElementStyleProp("position", v); }

export function quotes(v: string) { return new CssElementStyleProp("quotes", v); }

export function resize(v: string) { return new CssElementStyleProp("resize", v); }

export function right(v: CssElementPositionValue) { return new CssElementStyleProp("right", v); }

export function rowGap(v: CssGlobalValue | CssLengthPercentage | CssCalcStatement) { return new CssElementStyleProp("rowGap", v); }
export function gridRowGap(v: CssGlobalValue | CssLengthPercentage | CssCalcStatement) { return new CssElementStyleProp("gridRowGap", v); }

export function rubyPosition(v: string) { return new CssElementStyleProp("rubyPosition", v); }

export function scrollBehavior(v: string) { return new CssElementStyleProp("scrollBehavior", v); }

export function scrollMargin(v: string | 0) { return new CssElementStyleProp("scrollMargin", v); }

export function scrollMarginBlock(v: string) { return new CssElementStyleProp("scrollMarginBlock", v); }

export function scrollMarginBlockEnd(v: string) { return new CssElementStyleProp("scrollMarginBlockEnd", v); }

export function scrollMarginBlockStart(v: string) { return new CssElementStyleProp("scrollMarginBlockStart", v); }

export function scrollMarginBottom(v: string | 0) { return new CssElementStyleProp("scrollMarginBottom", v); }

export function scrollMarginInline(v: string) { return new CssElementStyleProp("scrollMarginInline", v); }

export function scrollMarginInlineEnd(v: string) { return new CssElementStyleProp("scrollMarginInlineEnd", v); }

export function scrollMarginInlineStart(v: string) { return new CssElementStyleProp("scrollMarginInlineStart", v); }

export function scrollMarginLeft(v: string | 0) { return new CssElementStyleProp("scrollMarginLeft", v); }

export function scrollMarginRight(v: string | 0) { return new CssElementStyleProp("scrollMarginRight", v); }

export function scrollMarginTop(v: string | 0) { return new CssElementStyleProp("scrollMarginTop", v); }

export function scrollPadding(v: string | 0) { return new CssElementStyleProp("scrollPadding", v); }

export function scrollPaddingBlock(v: string) { return new CssElementStyleProp("scrollPaddingBlock", v); }

export function scrollPaddingBlockEnd(v: string) { return new CssElementStyleProp("scrollPaddingBlockEnd", v); }

export function scrollPaddingBlockStart(v: string) { return new CssElementStyleProp("scrollPaddingBlockStart", v); }

export function scrollPaddingBottom(v: string | 0) { return new CssElementStyleProp("scrollPaddingBottom", v); }

export function scrollPaddingInline(v: string) { return new CssElementStyleProp("scrollPaddingInline", v); }

export function scrollPaddingInlineEnd(v: string) { return new CssElementStyleProp("scrollPaddingInlineEnd", v); }

export function scrollPaddingInlineStart(v: string) { return new CssElementStyleProp("scrollPaddingInlineStart", v); }

export function scrollPaddingLeft(v: string | 0) { return new CssElementStyleProp("scrollPaddingLeft", v); }

export function scrollPaddingRight(v: string | 0) { return new CssElementStyleProp("scrollPaddingRight", v); }

export function scrollPaddingTop(v: string | 0) { return new CssElementStyleProp("scrollPaddingTop", v); }

export function scrollSnapAlign(v: string) { return new CssElementStyleProp("scrollSnapAlign", v); }

export function scrollSnapStop(v: string) { return new CssElementStyleProp("scrollSnapStop", v); }

export function scrollSnapType(v: string) { return new CssElementStyleProp("scrollSnapType", v); }

export function shapeImageThreshold(v: string) { return new CssElementStyleProp("shapeImageThreshold", v); }

export function shapeMargin(v: string) { return new CssElementStyleProp("shapeMargin", v); }

export function shapeOutside(v: string) { return new CssElementStyleProp("shapeOutside", v); }

export function shapeRendering(v: string) { return new CssElementStyleProp("shapeRendering", v); }

export function stopColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("stopColor", v); }

export function stopOpacity(v: string) { return new CssElementStyleProp("stopOpacity", v); }

export function stroke(v: string) { return new CssElementStyleProp("stroke", v); }

export function strokeDasharray(v: string) { return new CssElementStyleProp("strokeDasharray", v); }

export function strokeDashoffset(v: string) { return new CssElementStyleProp("strokeDashoffset", v); }

export function strokeLinecap(v: string) { return new CssElementStyleProp("strokeLinecap", v); }

export function strokeLinejoin(v: string) { return new CssElementStyleProp("strokeLinejoin", v); }

export function strokeMiterlimit(v: string) { return new CssElementStyleProp("strokeMiterlimit", v); }

export function strokeOpacity(v: string) { return new CssElementStyleProp("strokeOpacity", v); }

export function strokeWidth(v: string | 0) { return new CssElementStyleProp("strokeWidth", v); }

export function tabSize(v: string) { return new CssElementStyleProp("tabSize", v); }

export function tableLayout(v: string) { return new CssElementStyleProp("tableLayout", v); }

export function textAlign(v: CssGlobalValue | CssTextAlignValue) { return new CssElementStyleProp("textAlign", v); }

export function textAlignLast(v: CssGlobalValue | CssTextAlignLastValue) { return new CssElementStyleProp("textAlignLast", v); }

export function textAnchor(v: string) { return new CssElementStyleProp("textAnchor", v); }

export function textCombineUpright(v: string) { return new CssElementStyleProp("textCombineUpright", v); }

export function textDecoration(v: string) { return new CssElementStyleProp("textDecoration", v); }

export function textDecorationColor(v: CssGlobalValue | CssColorValue) { return new CssElementStyleProp("textDecorationColor", v); }

export function textDecorationLine(v: string) { return new CssElementStyleProp("textDecorationLine", v); }

export function textDecorationSkipInk(v: string) { return new CssElementStyleProp("textDecorationSkipInk", v); }

export function textDecorationStyle(v: string) { return new CssElementStyleProp("textDecorationStyle", v); }

export function textIndent(v: string) { return new CssElementStyleProp("textIndent", v); }

export function textOrientation(v: string) { return new CssElementStyleProp("textOrientation", v); }

export function textOverflow(v: CssGlobalValue | CssTextOverflowValue) { return new CssElementStyleProp("textOverflow", v); }

export function textRendering(v: string) { return new CssElementStyleProp("textRendering", v); }

export function textShadow(v: string) { return new CssElementStyleProp("textShadow", v); }

export function textTransform(v: string) { return new CssElementStyleProp("textTransform", v); }

export function textUnderlinePosition(v: string) { return new CssElementStyleProp("textUnderlinePosition", v); }

export function textWrap(v: string) { return new CssElementStyleProp("textWrap", v); }

export function top(v: CssElementPositionValue) { return new CssElementStyleProp("top", v); }

export function touchAction(v: CssGlobalValue | CssTouchActionValue) { return new CssElementStyleProp("touchAction", v); }

export function transform(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("transform", v.join(" ")); }

export function transformBox(v: CssGlobalValue | CssTransformBoxValue) { return new CssElementStyleProp("transformBox", v); }

export function transformOrigin(v: CssGlobalValue): CssElementStyleProp;
export function transformOrigin(v: CssLengthPercentage | CssBackgroundPositionKeyword): CssElementStyleProp;
export function transformOrigin(x: CssLengthPercentage | CssBackgroundPositionHorizontalKeyword, y: CssLengthPercentage | CssBackgroundPositionVerticalKeyword): CssElementStyleProp;
export function transformOrigin(x: CssLengthPercentage | CssBackgroundPositionHorizontalKeyword, y: CssLengthPercentage | CssBackgroundPositionVerticalKeyword, z: CssLength): CssElementStyleProp;
export function transformOrigin(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("transformOrigin", v.join(" ")); }

export function transformStyle(v: CssGlobalValue | CssTransformStyleValue) { return new CssElementStyleProp("transformStyle", v); }

export function transition(v: string) { return new CssElementStyleProp("transition", v); }

export function transitionDelay(v: string | 0) { return new CssElementStyleProp("transitionDelay", v); }

export function transitionDuration(v: string | 0) { return new CssElementStyleProp("transitionDuration", v); }

export function transitionProperty(v: string) { return new CssElementStyleProp("transitionProperty", v); }

export function transitionTimingFunction(v: string) { return new CssElementStyleProp("transitionTimingFunction", v); }

export function translate(v: CssGlobalValue): CssElementStyleProp;
export function translate(v: CssLengthPercentage): CssElementStyleProp;
export function translate(x: CssLengthPercentage, y: CssLengthPercentage): CssElementStyleProp;
export function translate(x: CssLengthPercentage, y: CssLengthPercentage, z: CssLength): CssElementStyleProp;
export function translate(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("translate", v.join(" ")); }


export function unicodeBidi(v: string) { return new CssElementStyleProp("unicodeBidi", v); }

export function userSelect(v: string) { return new CssElementStyleProp("userSelect", v); }

export function verticalAlign(v: CssGlobalValue | CssVerticalAlignValue) { return new CssElementStyleProp("verticalAlign", v); }

export function visibility(v: CssGlobalValue | CssVisiblityValue) { return new CssElementStyleProp("visibility", v); }

export function whiteSpace(v: CssGlobalValue | CssWhiteSpaceValue) { return new CssElementStyleProp("whiteSpace", v); }

export function widows(v: CssGlobalValue | number) { return new CssElementStyleProp("widows", asInt(v)); }

export function width(v: CssGlobalValue | CssSizePropertyValue) { return new CssElementStyleProp("width", v); }

export function willChange(v: string) { return new CssElementStyleProp("willChange", v); }

export function wordBreak(v: string) { return new CssElementStyleProp("wordBreak", v); }

export function wordSpacing(v: string) { return new CssElementStyleProp("wordSpacing", v); }

export function wordWrap(v: CssGlobalValue | CssWordWrapValue) { return new CssElementStyleProp("wordWrap", v); }

export function writingMode(v: CssGlobalValue | CssWritingModeValue) { return new CssElementStyleProp("writingMode", v); }

export function zIndex(v: number) { return new CssElementStyleProp("zIndex", asInt(v)); }

class CssRegularAtRuleSet<T extends CssAtRuleRegular = CssAtRuleRegular> extends PropSet {
    constructor(selector: T, value: string) {
        super("@" + selector, " ", [new Prop(value)], ";");
    }
}

export function cssCharset(value: string) {
    return new CssRegularAtRuleSet("charset", value);
}

export function cssImport(value: string) {
    return new CssRegularAtRuleSet("import", value);
}

export function cssNamespace(value: string) {
    return new CssRegularAtRuleSet("namespace", value);
}

class CssColorProfileProp<T extends CssColorProfilePropName = CssColorProfilePropName> extends CssDeclareProp {
    constructor(name: T, value: string) {
        super(name, value);
    }
}

export function colorProfile(name: CssDashedName, src: string, renderingIntent: CssColorProfileProp<"rendering-intent">): PropSet;
export function colorProfile(name: CssDashedName, src: string, components: CssColorProfileProp<"components">): PropSet;
export function colorProfile(name: CssDashedName, src: string, renderingIntent: CssColorProfileProp<"rendering-intent">, components: CssColorProfileProp<"components">): PropSet;
export function colorProfile(name: CssDashedName, src: string, ...props: CssColorProfileProp[]): PropSet {
    props.unshift(new CssColorProfileProp("src", src));
    return new SelectorPropSet("@color-profile " + name, props);
}

export function renderingIntent(value: CssRenderingIntentValue): CssColorProfileProp { return new CssColorProfileProp("rendering-intent", value); }

export function components(...names: string[]) { return new CssColorProfileProp("components", names.join(", ")); }

class CssCounterStyleProp<T extends CssCounterStylePropName = CssCounterStylePropName> extends CssDeclareProp {
    constructor(name: T, value: string) {
        super(name, value);
    }
}

export function counterStyle(name: string, ...props: CssCounterStyleProp[]): PropSet {
    return new SelectorPropSet(`@counter-style ${name}`, props);
}

export function system(value: CssCounterStyleSystemValue) { return new CssCounterStyleProp("system", value); }

export function negative(value: string) { return new CssCounterStyleProp("negative", value); }

export function prefix(value: string) { return new CssCounterStyleProp("prefix", value); }

export function suffix(value: string) { return new CssCounterStyleProp("suffix", value); }

export function range(value: CssCounterStyleRangeValue | "auto"): CssCounterStyleProp<"range">;
export function range(...v: CssCounterStyleRangeValue[]): CssCounterStyleProp<"range">;
export function range(...v: string[]): CssCounterStyleProp<"range"> { return new CssCounterStyleProp("range", v.join(", ")); }

export function pad(count: number, symbol: string) { return new CssCounterStyleProp("pad", `${asInt(count)} ${JSON.stringify(symbol)}`); }

export function fallback(value: string) { return new CssCounterStyleProp("fallback", value); }

export function symbols(value: string) { return new CssCounterStyleProp("symbols", value); }

export function additiveSymbols(...v: string[]) { return new CssCounterStyleProp("additive-symbols", v.join(", ")); }

export function speakAs(value: string) { return new CssCounterStyleProp("speak-as", value); }

//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@font-face
//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@font-feature-values
//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@keyframes

export function layer(...names: string[]): PropSet;
export function layer(name: string, ...rules: PropSet[]): PropSet;
export function layer(...rules: PropSet[]): PropSet;
export function layer(...namesOrRules: (PropSet | string)[]) {
    const names = [
        ...namesOrRules.filter(isString)
    ];

    const rules = namesOrRules.filter(v => v instanceof PropSet) as PropSet[];

    if (names.length === 0 && rules.length === 0) {
        throw new Error("Layer names and/or rules are not defined.");
    }
    if (names.length > 1 && rules.length > 0) {
        throw new Error("Cannot define multiple layers and rules at the same time");
    }
    else if (names.length === 0) {
        return new SelectorPropSet("@layer", rules);
    }
    else {
        const layerList = names.join(", ");
        if (names.length > 1 || rules.length === 0) {
            return new CssRegularAtRuleSet("layer", layerList);
        }
        else {
            return new SelectorPropSet("@layer " + layerList, rules);
        }
    }
}

//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@media
export function media(query: string, ...props: PropSet[]) {
    return new SelectorPropSet("@media " + query, props);
}

//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@page
//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@supports

export type CssGlobalValue =
    | ""
    | "inherit"
    | "initial"
    | "revert"
    | "revert-layer"
    | "unset";

export type CssFunction<F extends string, T extends string | number> = `${F}(${T})`;

export type CssUrl = CssFunction<"url", string>;

export type CssPercentage = `${number}%`;

export type CssAlpha =
    | number
    | CssPercentage;

export type CssAngle =
    | number
    | `${number}deg`
    | `${number}rad`
    | `${number}grad`
    | `${number}turn`;

export type CssFontRelativeLength =
    | `${number}cap`
    | `${number}ch`
    | `${number}em`
    | `${number}ex`
    | `${number}ic`
    | `${number}lh`
    | `${number}rem`
    | `${number}rlh`;

export type CssViewportPercentageLength =
    | `${number}vh`
    | `${number}vw`
    | `${number}vi`
    | `${number}vb`
    | `${number}vmin`
    | `${number}vmax`;

export type CssAbsoluteLength =
    | `${number}px`
    | `${number}cm`
    | `${number}mm`
    | `${number}Q`
    | `${number}in`
    | `${number}pc`
    | `${number}pt`;

export type CssWidthKeyword =
    | "thin"
    | "medium"
    | "thick";

export type CssLength =
    | CssFontRelativeLength
    | CssViewportPercentageLength
    | CssAbsoluteLength;

export type CssLengthPercentage =
    | CssLength
    | CssPercentage
    | 0
    | "0";

export type CssAtRuleRegular =
    | "charset"
    | "import"
    | "namespace"
    | "layer";

export type CssBorderCollapseValue =
    | "collapse"
    | "separate";

export type CssBorderRepeatValue =
    | "stretch"
    | "repeat"
    | "round"
    | "space";

export type CssBorderStyleValue =
    | "none"
    | "hidden"
    | "dotted"
    | "dashed"
    | "solid"
    | "double"
    | "groove"
    | "ridge"
    | "inset"
    | "outset";

export type CssCalcStatement = CssFunction<"calc", string>;

export type CssColorKeywordValue =
    | "transparent"
    | "currentcolor"
    | "ActiveText"
    | "ButtonBorder"
    | "ButtonFace"
    | "ButtonText"
    | "Canvas"
    | "CanvasText"
    | "Field"
    | "FieldText"
    | "GrayText"
    | "Highlight"
    | "HighlightText"
    | "LinkText"
    | "Mark"
    | "MarkText"
    | "VisitedText"
    | "aqua"
    | "black"
    | "blue"
    | "fuchsia"
    | "gray"
    | "grey"
    | "green"
    | "lime"
    | "maroon"
    | "navy"
    | "olive"
    | "purple"
    | "red"
    | "silver"
    | "teal"
    | "white"
    | "yellow"
    | "aliceblue"
    | "antiquewhite"
    | "aqua"
    | "aquamarine"
    | "azure"
    | "beige"
    | "bisque"
    | "black"
    | "blanchedalmond"
    | "blue"
    | "blueviolet"
    | "brown"
    | "burlywood"
    | "cadetblue"
    | "chartreuse"
    | "chocolate"
    | "coral"
    | "cornflowerblue"
    | "cornsilk"
    | "crimson"
    | "cyan"
    | "darkblue"
    | "darkcyan"
    | "darkgoldenrod"
    | "darkgray"
    | "darkgreen"
    | "darkkhaki"
    | "darkmagenta"
    | "darkolivegreen"
    | "darkorange"
    | "darkorchid"
    | "darkred"
    | "darksalmon"
    | "darkseagreen"
    | "darkslateblue"
    | "darkslategray"
    | "darkturquoise"
    | "darkviolet"
    | "deeppink"
    | "deepskyblue"
    | "dimgray"
    | "dodgerblue"
    | "firebrick"
    | "floralwhite"
    | "forestgreen"
    | "fuchsia"
    | "gainsboro"
    | "ghostwhite"
    | "gold"
    | "goldenrod"
    | "gray"
    | "green"
    | "greenyellow"
    | "honeydew"
    | "hotpink"
    | "indianred"
    | "indigo"
    | "ivory"
    | "khaki"
    | "lavender"
    | "lavenderblush"
    | "lawngreen"
    | "lemonchiffon"
    | "lightblue"
    | "lightcoral"
    | "lightcyan"
    | "lightgoldenrodyellow"
    | "lightgreen"
    | "lightgrey"
    | "lightpink"
    | "lightsalmon"
    | "lightseagreen"
    | "lightskyblue"
    | "lightslategray"
    | "lightsteelblue"
    | "lightyellow"
    | "lime"
    | "limegreen"
    | "linen"
    | "magenta"
    | "maroon"
    | "mediumaquamarine"
    | "mediumblue"
    | "mediumorchid"
    | "mediumpurple"
    | "mediumseagreen"
    | "mediumslateblue"
    | "mediumspringgreen"
    | "mediumturquoise"
    | "mediumvioletred"
    | "midnightblue"
    | "mintcream"
    | "mistyrose"
    | "moccasin"
    | "navajowhite"
    | "navy"
    | "navyblue"
    | "oldlace"
    | "olive"
    | "olivedrab"
    | "orange"
    | "orangered"
    | "orchid"
    | "palegoldenrod"
    | "palegreen"
    | "paleturquoise"
    | "palevioletred"
    | "papayawhip"
    | "peachpuff"
    | "peru"
    | "pink"
    | "plum"
    | "powderblue"
    | "purple"
    | "red"
    | "rosybrown"
    | "royalblue"
    | "saddlebrown"
    | "salmon"
    | "sandybrown"
    | "seagreen"
    | "seashell"
    | "sienna"
    | "silver"
    | "skyblue"
    | "slateblue"
    | "slategray"
    | "snow"
    | "springgreen"
    | "steelblue"
    | "tan"
    | "teal"
    | "thistle"
    | "tomato"
    | "turquoise"
    | "violet"
    | "wheat"
    | "white"
    | "whitesmoke"
    | "yellow"
    | "yellowgreen";

export type HexDigit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" | "a" | "b" | "c" | "d" | "e" | "f" | "A" | "B" | "C" | "D" | "E" | "F";
export type HexNumber = `${HexDigit}${HexDigit}`
export type CssColorHashValue = `#${string}`;

export type CssColorRGBValue = CssFunction<"rgb" | "rgba", string>;
export type CssColorHSLValue = CssFunction<"hsl" | "hsla", string>;
export type CssColorHWBValue = CssFunction<"hwb", string>;
export type CssColorLCHValue = CssFunction<"lch", string>;
export type CssColorLabValue = CssFunction<"lab", string>;

export type CssColorSpaceName =
    | "srgb"
    | "srgb-linear"
    | "display-p3"
    | "a98-rgb"
    | "prophoto-rgb"
    | "rec2020"
    | "xyz"
    | "xyz-d50"
    | "xyz-d65";

export type CssColorFunctionValue = CssFunction<"color", `${CssColorSpaceName} ${string}`>;

export type CssColorValue =
    | CssColorKeywordValue
    | CssColorHashValue
    | CssColorRGBValue
    | CssColorHSLValue
    | CssColorHWBValue
    | CssColorLCHValue
    | CssColorLabValue
    | CssColorFunctionValue;

export type CssColorProfilePropName =
    | "src"
    | "rendering-intent"
    | "components";

export type CssCounterStylePropName =
    | "system"
    | "negative"
    | "prefix"
    | "suffix"
    | "range"
    | "pad"
    | "fallback"
    | "symbols"
    | "additive-symbols"
    | "speak-as";

export type CssCounterStyleSystemValue =
    | "cyclic"
    | "numeric"
    | "alphabetic"
    | "symbolic"
    | "additive"
    | "fixed"
    | `fixed ${number}`
    | `extends ${string}`;

export type CssCounterStyleRangeValue =
    | `${number} ${number}`
    | `${number} infinite`
    | `infinite ${number}`
    | "infinite infinite";

export type CssSizePropertyValue =
    | 0
    | "0"
    | CssLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"
    | CssCalcStatement;

export type CssAlignmentBaselineValue =
    | "auto"
    | "baseline"
    | "before-edge"
    | "text-before-edge"
    | "middle"
    | "central"
    | "after-edge"
    | "text-after-edge"
    | "ideographic"
    | "alphabetic"
    | "hanging"
    | "mathematical"
    | "top"
    | "bottom";

export type CssTimeSecondsValue = `${number}s`;
export type CssTimeMillisecondsValue = `${number}ms`;
export type CssTimeValue = CssTimeSecondsValue | CssTimeMillisecondsValue;

export type CssAnimationDirectionValue =
    | "normal"
    | "reverse"
    | "alternate"
    | "alternate-reverse";

export type CssAnimationFillModeValue =
    | "none"
    | "forwards"
    | "backwards"
    | "both";

export type CssAnimationIterationCountValue =
    | number
    | "infinite";

export type CssAnimationPlayStateValue =
    | "running"
    | "paused";

export type CssAnimationTimingFunctionNamed =
    | "ease"
    | "ease-in"
    | "ease-out"
    | "ease-in-out"
    | "linear"
    | "step-start"
    | "step-end";
export type CssAnimationTimingFunctionCubicBezier = `cubic-bezier(${number}, ${number}, ${number}, ${number})`;
export type CssAnimationTimingFunctionSteps = `steps(${number}, ${CssAnimationTimingFunctionNamed})`;
export type CssAnimationTimingFunctionValue =
    | CssAnimationTimingFunctionNamed
    | CssAnimationTimingFunctionCubicBezier
    | CssAnimationTimingFunctionSteps;

export type CssAppearanceValue =
    | "none"
    | "auto"
    | "menulist-button"
    | "textfield"
    | "button"
    | "checkbox"
    | "listbox"
    | "menulist"
    | "meter"
    | "progress-bar"
    | "push-button"
    | "radio"
    | "searchfield"
    | "slider-horizontal"
    | "square-button"
    | "textarea";

export type CssDropShadowParams =
    | `${CssLength} ${CssLength}`
    | `${CssLength} ${CssLength} ${string}`
    | `${CssLength} ${CssLength} ${CssLength} ${string}`;


export type CssFilterFunction = CssFunction<"blur", CssLength>
    | CssFunction<"brightness", CssAlpha>
    | CssFunction<"contrast", CssAlpha>
    | CssFunction<"drop-shadow", CssDropShadowParams>
    | CssFunction<"grayscale", CssAlpha>
    | CssFunction<"hue-rotate", CssAngle>
    | CssFunction<"invert", CssAlpha>
    | CssFunction<"opacity", CssAlpha>
    | CssFunction<"saturate", CssAlpha>
    | CssFunction<"sepia", CssAlpha>
    | CssUrl;

export type CssBackfaceVisibilityValue =
    | "visible"
    | "hiden";

export type CssBackgroundAttachmentValue =
    | "scroll"
    | "fixed"
    | "local";

export type CssBlendModeValue = "normal"
    | "multiply"
    | "screen"
    | "overlay"
    | "darken"
    | "lighten"
    | "color-dodge"
    | "color-burn"
    | "hard-light"
    | "soft-light"
    | "difference"
    | "exclusion"
    | "hue"
    | "saturation"
    | "color"
    | "luminosity";

export type CssBoxType =
    | "border-box"
    | "padding-box"
    | "content-box";

export type CssClipValue =
    | CssBoxType
    | "text";

export type CssGradient =
    | CssFunction<"linear-gradient", string>
    | CssFunction<"radial-gradient", string>
    | CssFunction<"repeating-linear-gradient", string>
    | CssFunction<"repeating-radial-gradient", string>
    | CssFunction<"conic-gradient", string>;

export type CssImage = CssUrl
    | CssGradient
    | CssFunction<"element", string>
    | CssFunction<"cross-fade", string>
    | CssFunction<"image-set", string>;

export type CssBackgroundPositionHorizontalKeyword =
    | "left"
    | "center"
    | "right";
export type CssBackgroundPositionVerticalKeyword =
    | "top"
    | "center"
    | "bottom";
export type CssBackgroundPositionKeyword =
    | CssBackgroundPositionHorizontalKeyword
    | CssBackgroundPositionVerticalKeyword;
export type CssBackgroundPositionHorizontalValue =
    | CssBackgroundPositionKeyword
    | CssLengthPercentage;
export type CssBackgroundPositionVerticalValue =
    | CssBackgroundPositionKeyword
    | CssLengthPercentage;
export type CssBackgroundPositionValue =
    | CssBackgroundPositionKeyword
    | CssLengthPercentage;

export type CssBackgroundRepeat =
    | "repeat"
    | "space"
    | "round"
    | "no-repeat";

export type CssBackgroundRepeatValue =
    | "repeat-x"
    | "repeat-y"
    | CssBackgroundRepeat;

export type CssBackgroundSizeValue =
    | "auto"
    | CssLengthPercentage;
export type CssBackgroundSizeSingleValue =
    | "contain"
    | "cover"
    | CssBackgroundSizeValue;

export type CssElementPositionValue =
    | "auto"
    | CssLengthPercentage
    | `anchor(${CssBackgroundPositionKeyword})`;

export type CssCursorValue =
    | "auto"
    | "default"
    | "none"
    | "context-menu"
    | "help"
    | "pointer"
    | "progress"
    | "wait"
    | "cell"
    | "crosshair"
    | "text"
    | "vertical-text"
    | "alias"
    | "copy"
    | "move"
    | "no-drop"
    | "not-allowed"
    | "grab"
    | "grabbing"
    | "all-scroll"
    | "col-resize"
    | "row-resize"
    | "n-resize"
    | "e-resize"
    | "s-resize"
    | "w-resize"
    | "ne-resize"
    | "nw-resize"
    | "se-resize"
    | "sw-resize"
    | "ew-resize"
    | "ns-resize"
    | "nesw-resize"
    | "nwse-resize"
    | "zoom-in"
    | "zoom-out";

export type CssDashedName = `--${string}`;

export type CssDirectionValue =
    | "ltr"
    | "rtl";

export type CssDisplayValue =
    | "none"
    | "contents"
    | "block"
    | "inline"
    | "inline-block"
    | "flex"
    | "inline-flex"
    | "inline flex"
    | "block flex"
    | "grid"
    | "inline-grid"
    | "inline grid"
    | "block grid"
    | "flow-root"
    | "inline flow-root"
    | "block flow-root"
    | "block flow"
    | "inline flow"
    | "table"
    | "table-row"
    | "table-row-group"
    | "list-item";

export type CssFlexBasisValue =
    | CssLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"

export type CssFlexDirectionValue =
    | "row"
    | "row-reverse"
    | "column"
    | "column-reverse";

export type CssFlexWrapValue =
    | "nowrap"
    | "wrap"
    | "wrap-reverse";

export type CssFlexFlowValue =
    | CssFlexDirectionValue
    | CssFlexWrapValue;

export type CssFloatValue =
    | "left"
    | "right"
    | "none"
    | "inline-start"
    | "inline-end";

export type CssGridAutoFlowValue =
    | "row"
    | "column"
    | "dense"
    | "row dense"
    | "column dense";

export type CssGridLineName = `[${string}]`;

export type CssGridFlexValue = `${number}fr`;

export type CssGridTemplateTrackSize =
    | CssLengthPercentage
    | CssGridFlexValue
    | "max-content"
    | "min-content"
    | "auto";

export type CssGridTemplateTrackMinMaxValue = CssFunction<"minmax", `${CssGridTemplateTrackSize}, ${CssGridTemplateTrackSize}`>;
export type CssGridTemplateTrackFitContentValue = CssFunction<"fit-content", CssLengthPercentage>;
export type CssGridTemplateTrackRepeatValue = CssFunction<"repeat", `${number | "auto-fill" | "auto-fit"}, ${string}`>;

export type CssGridTemplateTrackValue =
    | "none"
    | CssGridLineName
    | CssGridTemplateTrackSize
    | CssGridTemplateTrackMinMaxValue
    | CssGridTemplateTrackFitContentValue
    | CssGridTemplateTrackRepeatValue
    | "subgrid";

export type CssJustifyAlignValue =
    | "center"
    | "start"
    | "end"
    | "flex-start"
    | "flex-end"
    | "left"
    | "right"
    | "normal"
    | "stretch"
    | "safe center"
    | "unsafe center";

export type CssJustifyAlignContentValue =
    | CssJustifyAlignValue
    | "space-between"
    | "space-around"
    | "space-evenly";

export type CssJustifyAlignItemsContentValue =
    | CssJustifyAlignValue
    | "self-start"
    | "self-end"
    | "baseline"
    | "first baseline"
    | "last baseline";

export type CssJustifyItemsValue =
    | CssJustifyAlignItemsContentValue
    | "legacy right"
    | "legacy left"
    | "legacy center";

export type CssJustifyAlignSelfValue =
    | CssJustifyAlignItemsContentValue
    | "auto"
    | "anchor-center";

export type CssMediaQueryTypeValue =
    | "all"
    | "print"
    | "screen";

export type CssMediaQueryFeatureValue =
    | "any-hover"
    | "any-pointer"
    | "aspect-ratio"
    | "color"
    | "color-gamut"
    | "color-index"
    | "display-mode"
    | "dynamic-range"
    | "forced-colors"
    | "grid"
    | "height"
    | "hover"
    | "inverted-colors"
    | "monochrome"
    | "orientation"
    | "overflow-block"
    | "overflow-inline"
    | "pointer"
    | "prefers-color-scheme"
    | "prefers-contrast"
    | "prefers-reduced-motion"
    | "resolution"
    | "script"
    | "update"
    | "video-dynamic-range"
    | "width";

export type CssMediaQueryOperatorValue =
    | "and"
    | "not"
    | "only"
    | ","
    | "<"
    | "<="
    | ">"
    | ">=";

export type CssOverflowValue =
    | "visible"
    | "hidden"
    | "clip"
    | "scroll"
    | "auto";

export type CssPointerEventsValue =
    | "auto"
    | "none";

export type CssPositionValue =
    | "static"
    | "absolute"
    | "fixed"
    | "relative"
    | "sticky";

export type CssRenderingIntentValue =
    | "relative-colorimetric"
    | "absolute-colorimetric"
    | "perceptual"
    | "saturation";

export type CssTextAlignLastValue =
    | "auto"
    | "start"
    | "end"
    | "left"
    | "right"
    | "center"
    | "justify"
    | "match-parent";

export type CssTextAlignValue =
    | CssTextAlignLastValue
    | "justify-all";

export type CssTextOverflowValue =
    | "clip"
    | "ellipsis";

export type CssTouchActionValue =
    | "auto"
    | "none"
    | "pan-x"
    | "pan-left"
    | "pan-right"
    | "pan-y"
    | "pan-up"
    | "pan-down"
    | "pinch-zoom"
    | "manipulation";


export type CssTransformMatrixValue = `matrix(${number}, ${number}, ${number}, ${number}, ${number}, ${number})`;
export type CssTransformMatrix3DValue = `matrix3d(${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number})`;
export type CssTransformPerspectiveValue = `perspective(${CssLength | "none"})`;
export type CssTransformRotate = `rotate(${CssAngle})`;
export type CSSTransformRotate3D = `rotate3d(${number}, ${number}, ${number}, ${CssAngle})`;
export type CssTransformRotateX = `rotateX(${CssAngle})`;
export type CssTransformRotateY = `rotateY(${CssAngle})`;
export type CssTransformRotateZ = `rotateZ(${CssAngle})`;
export type CssTransformTranslate = `translate(${CssLengthPercentage}, ${CssLengthPercentage})`;
export type CSSTransformTranslate3D = `translate3d(${CssLengthPercentage}, ${CssLengthPercentage}, ${CssLengthPercentage})`;
export type CssTransformTranslateX = `translateX(${CssLengthPercentage})`;
export type CssTransformTranslateY = `translateY(${CssLengthPercentage})`;
export type CssTransformTranslateZ = `translateZ(${CssLengthPercentage})`;
export type CssTransformScale = `scale(${number}, ${number})`;
export type CSSTransformScale3D = `scale3d(${number}, ${number}, ${number})`;
export type CssTransformScaleX = `scaleX(${number})`;
export type CssTransformScaleY = `scaleY(${number})`;
export type CssTransformScaleZ = `scaleZ(${number})`;
export type CssTransformSkew = `skew(${CssAngle}, ${CssAngle})`;
export type CssTransformSkewX = `skewX(${CssAngle})`;
export type CssTransformSkewY = `skewY(${CssAngle})`;

export type CssTransformValue =
    | CssTransformMatrixValue
    | CssTransformMatrix3DValue
    | CssTransformRotate
    | CSSTransformRotate3D
    | CssTransformRotateX
    | CssTransformRotateY
    | CssTransformRotateZ
    | CssTransformTranslate
    | CSSTransformTranslate3D
    | CssTransformTranslateX
    | CssTransformTranslateY
    | CssTransformTranslateZ
    | CssTransformScale
    | CSSTransformScale3D
    | CssTransformScaleX
    | CssTransformScaleY
    | CssTransformScaleZ
    | CssTransformSkew
    | CssTransformSkewX
    | CssTransformSkewY;

export type CssTransformBoxValue =
    | "content-box"
    | "border-box"
    | "fill-box"
    | "stroke-box"
    | "view-box";

export type CssTransformStyleValue =
    | "flat"
    | "preserve-3d";

export type CssVerticalAlignValue =
    | CssLengthPercentage
    | "baseline"
    | "sub"
    | "super"
    | "text-top"
    | "text-bottom"
    | "middle"
    | "top"
    | "bottom";

export type CssVisiblityValue =
    | "visible"
    | "hidden"
    | "collapse";

export type CssWhiteSpaceValue =
    | "normal"
    | "nowrap"
    | "pre"
    | "pre-wrap"
    | "pre-line"
    | "break-spaces";

export type CssWordWrapValue =
    | "normal"
    | "break-word"
    | "anywhere";

export type CssWritingModeValue =
    | "horizontal-tb"
    | "vertical-rl"
    | "vertical-lr"
    | "sideways-rl"
    | "sideways-lr";

export const externalLinkIconSVG_DataURI = "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMiIgaGVpZ2h0PSIxMiIgdmlld0JveD0iMCAwIDEyIDEyIj4KCTx0aXRsZT4KCQlleHRlcm5hbCBsaW5rCgk8L3RpdGxlPgoJPHBhdGggZmlsbD0iIzM2YyIgZD0iTTYgMWg1djVMOC44NiAzLjg1IDQuNyA4IDQgNy4zbDQuMTUtNC4xNnpNMiAzaDJ2MUgydjZoNlY4aDF2MmExIDEgMCAwIDEtMSAxSDJhMSAxIDAgMCAxLTEtMVY0YTEgMSAwIDAgMSAxLTEiLz4KPC9zdmc+";

export function externalLinkRule(...props: (CssElementStyleProp | SelectorPropSet)[]) {
    return rule("a[href][target='_blank']::after",
        display("inline-block"),
        content(`' '`),
        marginLeft("0.33em"),
        width("1em"),
        height("1em"),
        backgroundImage(`url(${externalLinkIconSVG_DataURI})`),
        backgroundRepeat("no-repeat"),
        ...props
    );
}