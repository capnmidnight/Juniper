import { isDefined, isNumber, isString } from "@juniper-lib/tslib/typeChecks";
import { IElementAppliable } from "./tags";

function asInt(v: number | string): string {
    return isNumber(v) ? v.toFixed(0) : v;
}

export function px(value: number): CSSAbsoluteLength {
    return `${value}px`;
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
    return "system-ui, -apple-system, '.SFNSText-Regular', 'San Francisco', 'Segoe UI', 'Ubuntu', 'Roboto', 'Noto Sans' 'Droid Sans', sans-serif";
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

export type CSSPropName = Exclude<keyof CSSStyleDeclaration,
    "length"
    | "parentRule"
    | "getPropertyPriority"
    | "getPropertyValue"
    | "item"
    | "removeProperty"
    | "setProperty"> & string;

export class Prop {
    constructor(private readonly _value: string) {

    }

    get value() {
        return this._value;
    }

    toString() {
        return this.value;
    }
}

export class PropSet<T extends Prop = Prop> {
    constructor(
        private readonly pre: string,
        private readonly props: (T | PropSet<T>)[],
        private readonly post: string) {

    }

    get value(): string {
        return this.pre
            + this.props.map(p => p.toString()).join("\n")
            + this.post;
    }

    toString() {
        return this.value;
    }

    applyToSheet(sheet: CSSStyleSheet) {
        sheet.insertRule(this.toString(), sheet.cssRules.length);
    }
}

class KeyValueProp extends Prop {
    constructor(
        private readonly _name: string,
        private readonly sep: string,
        value: string) {
        super(value);
    }

    get name() {
        return this._name;
    }

    override toString() {
        return this.name
            + this.sep
            + this.value
            + ";";
    }
}

class SelectorPropSet<T extends Prop = Prop> extends PropSet<T> {
    constructor(selector: string, props: (T | PropSet<T>)[]) {
        super(selector + " {\n", props, "\n}\n");
    }
}

class CssDeclareProp
    extends KeyValueProp {
    constructor(name: string, value: string) {
        super(name, ": ", value);
    }
}

export class CssElementStyleProp<K extends CSSPropName = CSSPropName>
    extends CssDeclareProp
    implements IElementAppliable {
    private priority = "";

    constructor(
        public readonly key: K,
        value: string | number) {
        super(key.replace(/[A-Z]/g, (m) => `-${m.toLocaleLowerCase()}`), value.toString());
    }

    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    applyToElement(elem: HTMLElement) {
        (elem.style as any)[this.key] = this.value + this.priority;
    }

    important(): this {
        this.priority = " !important";
        return this;
    }

    override get value(): string {
        return super.value + this.priority;
    }
}

class CssElementStylePropSet extends SelectorPropSet<CssElementStyleProp> {
    constructor(selector: string, props: CssElementStyleProp[]) {
        super(selector, props);
    }
}

export function rule(selector: string, ...props: CssElementStyleProp[]) {
    return new CssElementStylePropSet(selector, props);
}

export function alignItems(v: CSSGlobalValues | CSSAlignItemsValue) { return new CssElementStyleProp("alignItems", v); }

export function alignContent(v: CSSGlobalValues | CSSAlignContentValue) { return new CssElementStyleProp("alignContent", v); }

export function alignSelf(v: CSSGlobalValues | CSSAlignSelfValue) { return new CssElementStyleProp("alignSelf", v); }

export function all(v: CSSGlobalValues) { return new CssElementStyleProp("all", v); }

export function alignmentBaseline(v: CSSAlignmentBaselineValues) { return new CssElementStyleProp("alignmentBaseline", v); }

export function animationDelay(v: CSSGlobalValues): CssElementStyleProp;
export function animationDelay(...v: CSSTimeValue[]): CssElementStyleProp;
export function animationDelay(...v: string[]) { return new CssElementStyleProp("animationDelay", v.join(", ")); }

export function animationDuration(v: CSSGlobalValues): CssElementStyleProp;
export function animationDuration(...v: CSSTimeValue[]): CssElementStyleProp;
export function animationDuration(...v: string[]) { return new CssElementStyleProp("animationDuration", v.join(", ")); }

export function animationDirection(v: CSSGlobalValues): CssElementStyleProp;
export function animationDirection(...v: CSSAnimationDirectionValue[]): CssElementStyleProp;
export function animationDirection(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("animationDirection", v.join(", ")); }

export function animationFillMode(v: CSSGlobalValues): CssElementStyleProp;
export function animationFillMode(...v: CSSAnimationFillModeValue[]): CssElementStyleProp;
export function animationFillMode(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("animationFillMode", v.join(", ")); }

export function animationIterationCount(v: CSSGlobalValues): CssElementStyleProp;
export function animationIterationCount(...v: CSSAnimationIterationCountValue[]): CssElementStyleProp;
export function animationIterationCount(...v: (number | string)[]): CssElementStyleProp { return new CssElementStyleProp("animationIterationCount", v.join(", ")); }

export function animationName(v: CSSGlobalValues): CssElementStyleProp;
export function animationName(...v: string[]): CssElementStyleProp;
export function animationName(...v: string[]) { return new CssElementStyleProp("animationName", v.join(", ")); }

export function animationPlayState(v: CSSGlobalValues): CssElementStyleProp;
export function animationPlayState(...v: CSSAnimationPlayStateValue[]): CssElementStyleProp;
export function animationPlayState(...v: string[]) { return new CssElementStyleProp("animationPlayState", v.join(", ")); }

export function animationTimingFunction(v: CSSGlobalValues): CssElementStyleProp;
export function animationTimingFunction(...v: CSSAnimationTimingFunctionValue[]): CssElementStyleProp;
export function animationTimingFunction(...v: string[]) { return new CssElementStyleProp("animationTimingFunction", v.join(' ')); }

export function animation(v: CSSGlobalValues): CssElementStyleProp;
export function animation(...v: (string | number)[]): CssElementStyleProp;
export function animation(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("animation", v.join(" ")); }

export function appearance(v: CSSGlobalValues | CSSAppearanceValue) { return new CssElementStyleProp("appearance", v); }

export function backdropFilter(v: "none" | CSSGlobalValues): CssElementStyleProp;
export function backdropFilter(...v: CSSFilterFunction[]): CssElementStyleProp;
export function backdropFilter(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("backdropFilter" as any, v.join(' ')); }

export function backfaceVisibility(v: CSSGlobalValues | CSSBackfaceVisibilityValue) { return new CssElementStyleProp("backfaceVisibility", v); }

export function backgroundAttachment(v: CSSGlobalValues | CSSBackgroundAttachmentValue) { return new CssElementStyleProp("backgroundAttachment", v); }

export function backgroundBlendMode(v: CSSGlobalValues): CssElementStyleProp;
export function backgroundBlendMode(...v: CSSBlendModeValue[]): CssElementStyleProp;
export function backgroundBlendMode(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("backgroundBlendMode", v.join(", ")); }

export function backgroundClip(v: CSSGlobalValues | CSSClipValue): CssElementStyleProp { return new CssElementStyleProp("backgroundClip", v); }

export function backgroundColor(v: CSSGlobalValues | CSSColorValue) { return new CssElementStyleProp("backgroundColor", v); }

export function backgroundImage(v: CSSGlobalValues): CssElementStyleProp;
export function backgroundImage(...v: CSSImage[]): CssElementStyleProp;
export function backgroundImage(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("backgroundImage", v.join(", ")); }

export function backgroundOrigin(v: CSSGlobalValues | CSSBoxType): CssElementStyleProp { return new CssElementStyleProp("backgroundOrigin", v); }

export function backgroundPosition(v: CSSGlobalValues): CssElementStyleProp;
export function backgroundPosition(v: CSSPositionValue): CssElementStyleProp;
export function backgroundPosition(x: CSSLengthPercentage, y: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPosition(x: CSSPositionHorizontalKeyword, y: CSSPositionVerticalValue): CssElementStyleProp;
export function backgroundPosition(y: CSSPositionVerticalKeyword, x: CSSPositionHorizontalValue): CssElementStyleProp;
export function backgroundPosition(x: CSSPositionHorizontalKeyword, y: CSSPositionVerticalKeyword, yOffset: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPosition(x: CSSPositionHorizontalKeyword, xOffset: CSSLengthPercentage, y: CSSPositionVerticalKeyword): CssElementStyleProp;
export function backgroundPosition(y: CSSPositionVerticalKeyword, x: CSSPositionHorizontalKeyword, xOffset: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPosition(y: CSSPositionVerticalKeyword, yOffset: CSSLengthPercentage, x: CSSPositionHorizontalKeyword): CssElementStyleProp;
export function backgroundPosition(x: CSSPositionHorizontalKeyword, xOffset: CSSLengthPercentage, y: CSSPositionVerticalKeyword, yOffset: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPosition(y: CSSPositionVerticalKeyword, yOffset: CSSLengthPercentage, x: CSSPositionHorizontalKeyword, xOffset: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPosition(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundPosition", v.join(" ")); }

export function backgroundPositionX(v: CSSGlobalValues): CssElementStyleProp;
export function backgroundPositionX(x: CSSPositionHorizontalValue): CssElementStyleProp;
export function backgroundPositionX(x: CSSPositionHorizontalKeyword, xOffset: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPositionX(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundPositionX", v.join(" ")); }

export function backgroundPositionY(v: CSSGlobalValues): CssElementStyleProp;
export function backgroundPositionY(y: CSSPositionVerticalValue): CssElementStyleProp;
export function backgroundPositionY(y: CSSPositionVerticalKeyword, yOffset: CSSLengthPercentage): CssElementStyleProp;
export function backgroundPositionY(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundPositionY", v.join(" ")); }

export function backgroundRepeat(v: CSSGlobalValues): CssElementStyleProp;
export function backgroundRepeat(v: CSSBackgroundRepeatValue): CssElementStyleProp;
export function backgroundRepeat(x: CSSBackgroundRepeat, y: CSSBackgroundRepeat): CssElementStyleProp;
export function backgroundRepeat(...v: string[]) { return new CssElementStyleProp("backgroundRepeat", v.join(" ")); }

export function backgroundSize(v: CSSGlobalValues | CSSBackgroundSizeSingleValue): CssElementStyleProp;
export function backgroundSize(x: CSSBackgroundSizeValue, y: CSSBackgroundSizeValue): CssElementStyleProp;
export function backgroundSize(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("backgroundSize", v.join(" ")); }

export function background(v: CSSGlobalValues): CssElementStyleProp;
export function background(...v: string[]): CssElementStyleProp;
export function background(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("background", v.join(", ")); }

export function baselineShift(v: string) { return new CssElementStyleProp("baselineShift", v); }

export function blockSize(v: string) { return new CssElementStyleProp("blockSize", v); }

export function border(v: string | 0) { return new CssElementStyleProp("border", v); }

export function borderBlockEnd(v: string) { return new CssElementStyleProp("borderBlockEnd", v); }

export function borderBlockEndColor(v: string) { return new CssElementStyleProp("borderBlockEndColor", v); }

export function borderBlockEndStyle(v: string) { return new CssElementStyleProp("borderBlockEndStyle", v); }

export function borderBlockEndWidth(v: string) { return new CssElementStyleProp("borderBlockEndWidth", v); }

export function borderBlockStart(v: string) { return new CssElementStyleProp("borderBlockStart", v); }

export function borderBlockStartColor(v: string) { return new CssElementStyleProp("borderBlockStartColor", v); }

export function borderBlockStartStyle(v: string) { return new CssElementStyleProp("borderBlockStartStyle", v); }

export function borderBlockStartWidth(v: string) { return new CssElementStyleProp("borderBlockStartWidth", v); }

export function borderBottom(v: string) { return new CssElementStyleProp("borderBottom", v); }

export function borderBottomColor(v: string) { return new CssElementStyleProp("borderBottomColor", v); }

export function borderBottomLeftRadius(v: string) { return new CssElementStyleProp("borderBottomLeftRadius", v); }

export function borderBottomRightRadius(v: string) { return new CssElementStyleProp("borderBottomRightRadius", v); }

export function borderBottomStyle(v: string) { return new CssElementStyleProp("borderBottomStyle", v); }

export function borderBottomWidth(v: string) { return new CssElementStyleProp("borderBottomWidth", v); }

export function borderCollapse(v: string) { return new CssElementStyleProp("borderCollapse", v); }

export function borderColor(v: string) { return new CssElementStyleProp("borderColor", v); }

export function borderImage(v: string) { return new CssElementStyleProp("borderImage", v); }

export function borderImageOutset(v: string) { return new CssElementStyleProp("borderImageOutset", v); }

export function borderImageRepeat(v: string) { return new CssElementStyleProp("borderImageRepeat", v); }

export function borderImageSlice(v: string) { return new CssElementStyleProp("borderImageSlice", v); }

export function borderImageSource(v: string) { return new CssElementStyleProp("borderImageSource", v); }

export function borderImageWidth(v: string) { return new CssElementStyleProp("borderImageWidth", v); }

export function borderInlineEnd(v: string) { return new CssElementStyleProp("borderInlineEnd", v); }

export function borderInlineEndColor(v: string) { return new CssElementStyleProp("borderInlineEndColor", v); }

export function borderInlineEndStyle(v: string) { return new CssElementStyleProp("borderInlineEndStyle", v); }

export function borderInlineEndWidth(v: string) { return new CssElementStyleProp("borderInlineEndWidth", v); }

export function borderInlineStart(v: string) { return new CssElementStyleProp("borderInlineStart", v); }

export function borderInlineStartColor(v: string) { return new CssElementStyleProp("borderInlineStartColor", v); }

export function borderInlineStartStyle(v: string) { return new CssElementStyleProp("borderInlineStartStyle", v); }

export function borderInlineStartWidth(v: string) { return new CssElementStyleProp("borderInlineStartWidth", v); }

export function borderLeft(v: string) { return new CssElementStyleProp("borderLeft", v); }

export function borderLeftColor(v: string) { return new CssElementStyleProp("borderLeftColor", v); }

export function borderLeftStyle(v: string) { return new CssElementStyleProp("borderLeftStyle", v); }

export function borderLeftWidth(v: string) { return new CssElementStyleProp("borderLeftWidth", v); }

export function borderRadius(v: CSSGlobalValues): CssElementStyleProp;
export function borderRadius(v: CSSLengthPercentage): CssElementStyleProp;
export function borderRadius(vertical: CSSLengthPercentage, horizontal: CSSLengthPercentage): CssElementStyleProp;
export function borderRadius(top: CSSLengthPercentage, right: CSSLengthPercentage, bottom: CSSLengthPercentage, left: CSSLengthPercentage): CssElementStyleProp;
export function borderRadius(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("borderRadius", v.join(" ")); }

export function borderRight(v: string) { return new CssElementStyleProp("borderRight", v); }

export function borderRightColor(v: string) { return new CssElementStyleProp("borderRightColor", v); }

export function borderRightStyle(v: string) { return new CssElementStyleProp("borderRightStyle", v); }

export function borderRightWidth(v: string) { return new CssElementStyleProp("borderRightWidth", v); }

export function borderSpacing(v: string) { return new CssElementStyleProp("borderSpacing", v); }

export function borderStyle(v: string) { return new CssElementStyleProp("borderStyle", v); }

export function borderTop(v: string) { return new CssElementStyleProp("borderTop", v); }

export function borderTopColor(v: string) { return new CssElementStyleProp("borderTopColor", v); }

export function borderTopLeftRadius(v: string) { return new CssElementStyleProp("borderTopLeftRadius", v); }

export function borderTopRightRadius(v: string) { return new CssElementStyleProp("borderTopRightRadius", v); }

export function borderTopStyle(v: string) { return new CssElementStyleProp("borderTopStyle", v); }

export function borderTopWidth(v: string) { return new CssElementStyleProp("borderTopWidth", v); }

export function borderWidth(v: string | 0) { return new CssElementStyleProp("borderWidth", v); }

export function bottom(v: CSSGlobalValues | CSSElementPositionValue) { return new CssElementStyleProp("bottom", v); }

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

export function color(v: CSSGlobalValues): CssElementStyleProp;
export function color(v: CSSColorValue): CssElementStyleProp;
export function color(v: string): CssElementStyleProp { return new CssElementStyleProp("color", v); }

export function colorInterpolation(v: string) { return new CssElementStyleProp("colorInterpolation", v); }

export function colorInterpolationFilters(v: string) { return new CssElementStyleProp("colorInterpolationFilters", v); }

export function colorScheme(v: string) { return new CssElementStyleProp("colorScheme", v); }

export function columnCount(v: string) { return new CssElementStyleProp("columnCount", v); }

export function columnFill(v: string) { return new CssElementStyleProp("columnFill", v); }

export function columnGap(v: string) { return new CssElementStyleProp("columnGap", v); }

export function columnRule(v: string) { return new CssElementStyleProp("columnRule", v); }

export function columnRuleColor(v: string) { return new CssElementStyleProp("columnRuleColor", v); }

export function columnRuleStyle(v: string) { return new CssElementStyleProp("columnRuleStyle", v); }

export function columnRuleWidth(v: string) { return new CssElementStyleProp("columnRuleWidth", v); }

export function columnSpan(v: string) { return new CssElementStyleProp("columnSpan", v); }

export function columnWidth(v: string) { return new CssElementStyleProp("columnWidth", v); }

export function columns(v: string) { return new CssElementStyleProp("columns", v); }

export function contain(v: string) { return new CssElementStyleProp("contain", v); }

export function counterIncrement(v: string) { return new CssElementStyleProp("counterIncrement", v); }

export function counterReset(v: string) { return new CssElementStyleProp("counterReset", v); }

export function cursor(v: CSSGlobalValues | CSSCursorValue) { return new CssElementStyleProp("cursor", v); }

export function direction(v: CSSGlobalValues | CSSDirectionValues) { return new CssElementStyleProp("direction", v); }

export function display(v: CSSImportant<CSSGlobalValues | CSSDisplayValues>) { return new CssElementStyleProp("display", v); }

export function dominantBaseline(v: string) { return new CssElementStyleProp("dominantBaseline", v); }

export function emptyCells(v: string) { return new CssElementStyleProp("emptyCells", v); }

export function fill(v: string) { return new CssElementStyleProp("fill", v); }

export function fillOpacity(v: string) { return new CssElementStyleProp("fillOpacity", v); }

export function fillRule(v: string) { return new CssElementStyleProp("fillRule", v); }

export function filter(v: string) { return new CssElementStyleProp("filter", v); }

export function flexBasis(v: CSSGlobalValues | CSSFlexBasisValues) { return new CssElementStyleProp("flexBasis", v); }

export function flexDirection(v: CSSGlobalValues | CSSFlexDirectionValues) { return new CssElementStyleProp("flexDirection", v); }

export function flexWrap(v: CSSGlobalValues | CSSFlexWrapValues) { return new CssElementStyleProp("flexWrap", v); }

export function flexFlow(v: CSSGlobalValues): CssElementStyleProp;
export function flexFlow(v: CSSFlexFlowValues): CssElementStyleProp;
export function flexFlow(x: CSSFlexFlowValues, y: CSSFlexFlowValues): CssElementStyleProp;
export function flexFlow(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("flexFlow", v.join(' ')); }

export function flex(v: CSSGlobalValues): CssElementStyleProp;
export function flex(grow: number): CssElementStyleProp;
export function flex(basis: CSSFlexBasisValues): CssElementStyleProp;
export function flex(grow: number, shrink: number): CssElementStyleProp;
export function flex(grow: number, basis: CSSFlexBasisValues): CssElementStyleProp;
export function flex(grow: number, shrink: number, basis: CSSFlexBasisValues): CssElementStyleProp;
export function flex(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("flex", v.join(' ')); }

export function flexGrow(v: CSSGlobalValues | number) { return new CssElementStyleProp("flexGrow", v); }

export function flexShrink(v: CSSGlobalValues | number) { return new CssElementStyleProp("flexShrink", v); }

export function float(v: CSSGlobalValues | CSSFloatValues) { return new CssElementStyleProp("float", v); }

export function floodColor(v: string) { return new CssElementStyleProp("floodColor", v); }

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

export function gap(v: string) { return new CssElementStyleProp("gap", v); }

export function grid(v: string) { return new CssElementStyleProp("grid", v); }

export function gridArea(v: string): CssElementStyleProp;
export function gridArea(rowStart: number, colStart?: number, rowEnd?: number, colEnd?: number): CssElementStyleProp;
export function gridArea(vOrRowStart: string | number, colStart?: number, rowEnd?: number, colEnd?: number): CssElementStyleProp {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, colStart, rowEnd, colEnd]
            .filter(isDefined)
            .join('/');
    }

    return new CssElementStyleProp("gridArea", vOrRowStart);
}

export function gridAutoColumns(v: string) { return new CssElementStyleProp("gridAutoColumns", v); }

export function gridAutoFlow(v: CSSGlobalValues | CSSGridAutoFlowValues) { return new CssElementStyleProp("gridAutoFlow", v); }

export function gridAutoRows(v: string) { return new CssElementStyleProp("gridAutoRows", v); }

export function gridColumn(v: string): CssElementStyleProp
export function gridColumn(colStart: number, colEnd?: number): CssElementStyleProp
export function gridColumn(vOrColStart: string | number, colEnd?: number): CssElementStyleProp {
    if (!isString(vOrColStart)) {
        vOrColStart = [vOrColStart, colEnd]
            .filter(isDefined)
            .join('/');
    }
    return new CssElementStyleProp("gridColumn", vOrColStart);
}

export function gridColumnEnd(v: string | number) { return new CssElementStyleProp("gridColumnEnd", v); }

export function gridColumnGap(v: CSSLengthPercentage) { return new CssElementStyleProp("gridColumnGap", v); }

export function gridColumnStart(v: string | number) { return new CssElementStyleProp("gridColumnStart", v); }

export function gridGap(v: CSSLengthPercentage) { return new CssElementStyleProp("gridGap", v); }

export function gridRow(v: string): CssElementStyleProp
export function gridRow(rowStart: number, rowEnd?: number): CssElementStyleProp
export function gridRow(vOrRowStart: string | number, rowEnd?: number): CssElementStyleProp {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, rowEnd]
            .filter(isDefined)
            .join('/');
    }
    return new CssElementStyleProp("gridRow", vOrRowStart);
}

export function gridRowEnd(v: string | number) { return new CssElementStyleProp("gridRowEnd", v); }

export function gridRowGap(v: CSSLengthPercentage) { return new CssElementStyleProp("gridRowGap", v); }

export function gridRowStart(v: string | number) { return new CssElementStyleProp("gridRowStart", v); }

export function gridTemplate(v: string) { return new CssElementStyleProp("gridTemplate", v); }

export function gridTemplateAreas(...v: string[]) { return new CssElementStyleProp("gridTemplateAreas", v.map((r) => '"' + r + '"').join('\n')); }

export function gridTemplateColumns(v: CSSGlobalValues): CssElementStyleProp;
export function gridTemplateColumns(...v: CSSGridTemplateTrackValues[]): CssElementStyleProp;
export function gridTemplateColumns(...v: (string | number)[]) { return new CssElementStyleProp("gridTemplateColumns", v.join(" ")); }

export function gridTemplateRows(v: CSSGlobalValues): CssElementStyleProp;
export function gridTemplateRows(...v: CSSGridTemplateTrackValues[]): CssElementStyleProp;
export function gridTemplateRows(...v: (string | number)[]) { return new CssElementStyleProp("gridTemplateRows", v.join(" ")); }

export function height(v: CSSImportant<CSSGlobalValues | CSSSizePropertyValue>) { return new CssElementStyleProp("height", v); }

export function hyphens(v: string) { return new CssElementStyleProp("hyphens", v); }

export function imageOrientation(v: string) { return new CssElementStyleProp("imageOrientation", v); }

export function imageRendering(v: string) { return new CssElementStyleProp("imageRendering", v); }

export function inlineSize(v: string) { return new CssElementStyleProp("inlineSize", v); }

export function isolation(v: string) { return new CssElementStyleProp("isolation", v); }

export function justifyContent(v: string) { return new CssElementStyleProp("justifyContent", v); }

export function justifyItems(v: string) { return new CssElementStyleProp("justifyItems", v); }

export function justifySelf(v: string) { return new CssElementStyleProp("justifySelf", v); }

export function left(v: CSSElementPositionValue) { return new CssElementStyleProp("left", v); }

export function letterSpacing(v: string) { return new CssElementStyleProp("letterSpacing", v); }

export function lightingColor(v: string) { return new CssElementStyleProp("lightingColor", v); }

export function lineBreak(v: string) { return new CssElementStyleProp("lineBreak", v); }

export function lineHeight(v: string) { return new CssElementStyleProp("lineHeight", v); }

export function listStyle(v: string) { return new CssElementStyleProp("listStyle", v); }

export function listStyleImage(v: string) { return new CssElementStyleProp("listStyleImage", v); }

export function listStylePosition(v: string) { return new CssElementStyleProp("listStylePosition", v); }

export function listStyleType(v: string) { return new CssElementStyleProp("listStyleType", v); }

export function margin(v: CSSGlobalValues | CSSLengthPercentageAuto): CssElementStyleProp;
export function margin(vert: CSSLengthPercentageAuto, horiz: CSSLengthPercentageAuto): CssElementStyleProp;
export function margin(top: CSSLengthPercentageAuto, horiz: CSSLengthPercentageAuto, bot: CSSLengthPercentageAuto): CssElementStyleProp;
export function margin(top: CSSLengthPercentageAuto, right: CSSLengthPercentageAuto, bot: CSSLengthPercentageAuto, left: CSSLengthPercentageAuto): CssElementStyleProp;
export function margin(...v: (number | string)[]) { return new CssElementStyleProp("margin", v.join(" ")); }

export function marginBlockEnd(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginBlockEnd", v); }

export function marginBlockStart(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginBlockStart", v); }

export function marginBottom(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginBottom", v); }

export function marginInlineEnd(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginInlineEnd", v); }

export function marginInlineStart(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginInlineStart", v); }

export function marginLeft(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginLeft", v); }

export function marginRight(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginRight", v); }

export function marginTop(v: CSSLengthPercentageAuto) { return new CssElementStyleProp("marginTop", v); }

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

export function opacity(v: CSSGlobalValues | CSSNumberPercentage) { return new CssElementStyleProp("opacity", v); }

export function order(v: string) { return new CssElementStyleProp("order", v); }

export function orphans(v: string) { return new CssElementStyleProp("orphans", v); }

export function outline(v: string) { return new CssElementStyleProp("outline", v); }

export function outlineColor(v: string) { return new CssElementStyleProp("outlineColor", v); }

export function outlineOffset(v: string) { return new CssElementStyleProp("outlineOffset", v); }

export function outlineStyle(v: string) { return new CssElementStyleProp("outlineStyle", v); }

export function outlineWidth(v: string | 0) { return new CssElementStyleProp("outlineWidth", v); }

export function overflow(v: CSSGlobalValues | CSSOverflowValues): CssElementStyleProp;
export function overflow(x: CSSOverflowValues, y: CSSOverflowValues): CssElementStyleProp;
export function overflow(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("overflow", v.join(" ")); }

export function overflowX(v: CSSGlobalValues | CSSOverflowValues) { return new CssElementStyleProp("overflowX", v); }

export function overflowY(v: CSSGlobalValues | CSSOverflowValues) { return new CssElementStyleProp("overflowY", v); }

export function overflowAnchor(v: string) { return new CssElementStyleProp("overflowAnchor", v); }

export function overflowWrap(v: string) { return new CssElementStyleProp("overflowWrap", v); }

export function overscrollBehavior(v: string) { return new CssElementStyleProp("overscrollBehavior", v); }

export function overscrollBehaviorBlock(v: string) { return new CssElementStyleProp("overscrollBehaviorBlock", v); }

export function overscrollBehaviorInline(v: string) { return new CssElementStyleProp("overscrollBehaviorInline", v); }

export function overscrollBehaviorX(v: string) { return new CssElementStyleProp("overscrollBehaviorX", v); }

export function overscrollBehaviorY(v: string) { return new CssElementStyleProp("overscrollBehaviorY", v); }

export function padding(v: CSSGlobalValues | CSSLengthPercentage): CssElementStyleProp;
export function padding(vert: CSSLengthPercentage, horiz: CSSLengthPercentage): CssElementStyleProp;
export function padding(top: CSSLengthPercentage, horiz: CSSLengthPercentage, bot: CSSLengthPercentage): CssElementStyleProp;
export function padding(top: CSSLengthPercentage, right: CSSLengthPercentage, bot: CSSLengthPercentage, left: CSSLengthPercentage): CssElementStyleProp;
export function padding(...v: (CSSGlobalValues | CSSLengthPercentage)[]) { return new CssElementStyleProp("padding", v.join(" ")); }

export function paddingBlockEnd(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingBlockEnd", v); }

export function paddingBlockStart(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingBlockStart", v); }

export function paddingBottom(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingBottom", v); }

export function paddingInlineEnd(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingInlineEnd", v); }

export function paddingInlineStart(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingInlineStart", v); }

export function paddingLeft(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingLeft", v); }

export function paddingRight(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingRight", v); }

export function paddingTop(v: CSSGlobalValues | CSSLengthPercentage) { return new CssElementStyleProp("paddingTop", v); }

export function pageBreakAfter(v: string) { return new CssElementStyleProp("pageBreakAfter", v); }

export function pageBreakBefore(v: string) { return new CssElementStyleProp("pageBreakBefore", v); }

export function pageBreakInside(v: string) { return new CssElementStyleProp("pageBreakInside", v); }

export function paintOrder(v: string) { return new CssElementStyleProp("paintOrder", v); }

export function perspective(v: string) { return new CssElementStyleProp("perspective", v); }

export function perspectiveOrigin(v: string) { return new CssElementStyleProp("perspectiveOrigin", v); }

export function placeContent(v: string) { return new CssElementStyleProp("placeContent", v); }

export function placeItems(v: string) { return new CssElementStyleProp("placeItems", v); }

export function placeSelf(v: string) { return new CssElementStyleProp("placeSelf", v); }

export function pointerEvents(v: CSSGlobalValues | CSSPointerEventsValue) { return new CssElementStyleProp("pointerEvents", v); }

export function position(v: CSSImportant<CSSGlobalValues | CSSPositionValues>) { return new CssElementStyleProp("position", v); }

export function quotes(v: string) { return new CssElementStyleProp("quotes", v); }

export function resize(v: string) { return new CssElementStyleProp("resize", v); }

export function right(v: CSSElementPositionValue) { return new CssElementStyleProp("right", v); }

export function rowGap(v: string | 0) { return new CssElementStyleProp("rowGap", v); }

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

export function stopColor(v: string) { return new CssElementStyleProp("stopColor", v); }

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

export function textAlign(v: CSSGlobalValues | CSSTextAlignValues) { return new CssElementStyleProp("textAlign", v); }

export function textAlignLast(v: CSSGlobalValues | CSSTextAlignLastValues) { return new CssElementStyleProp("textAlignLast", v); }

export function textAnchor(v: string) { return new CssElementStyleProp("textAnchor", v); }

export function textCombineUpright(v: string) { return new CssElementStyleProp("textCombineUpright", v); }

export function textDecoration(v: string) { return new CssElementStyleProp("textDecoration", v); }

export function textDecorationColor(v: string) { return new CssElementStyleProp("textDecorationColor", v); }

export function textDecorationLine(v: string) { return new CssElementStyleProp("textDecorationLine", v); }

export function textDecorationSkipInk(v: string) { return new CssElementStyleProp("textDecorationSkipInk", v); }

export function textDecorationStyle(v: string) { return new CssElementStyleProp("textDecorationStyle", v); }

export function textIndent(v: string) { return new CssElementStyleProp("textIndent", v); }

export function textOrientation(v: string) { return new CssElementStyleProp("textOrientation", v); }

export function textOverflow(v: CSSGlobalValues | CSSTextOverflowValues) { return new CssElementStyleProp("textOverflow", v); }

export function textRendering(v: string) { return new CssElementStyleProp("textRendering", v); }

export function textShadow(v: string) { return new CssElementStyleProp("textShadow", v); }

export function textTransform(v: string) { return new CssElementStyleProp("textTransform", v); }

export function textUnderlinePosition(v: string) { return new CssElementStyleProp("textUnderlinePosition", v); }

export function top(v: CSSElementPositionValue) { return new CssElementStyleProp("top", v); }

export function touchAction(v: CSSImportant<CSSGlobalValues | CSSTouchActionValues>) { return new CssElementStyleProp("touchAction", v); }

export function transform(v: CSSGlobalValues): CssElementStyleProp;
export function transform(perspective: CSSTransformPerspectiveValue, ...rest: CSSTransformValue[]): CssElementStyleProp;
export function transform(...v: CSSTransformValue[]): CssElementStyleProp;
export function transform(...v: string[]): CssElementStyleProp { return new CssElementStyleProp("transform", v.join(" ")); }

export function transformBox(v: CSSGlobalValues | CSSTransformBoxValue) { return new CssElementStyleProp("transformBox", v); }

export function transformOrigin(v: CSSGlobalValues): CssElementStyleProp;
export function transformOrigin(v: CSSLengthPercentage | CSSPositionKeyword): CssElementStyleProp;
export function transformOrigin(x: CSSLengthPercentage | CSSPositionHorizontalKeyword, y: CSSLengthPercentage | CSSPositionVerticalKeyword): CssElementStyleProp;
export function transformOrigin(x: CSSLengthPercentage | CSSPositionHorizontalKeyword, y: CSSLengthPercentage | CSSPositionVerticalKeyword, z: CSSLength): CssElementStyleProp;
export function transformOrigin(...v: (string | number)[]): CssElementStyleProp { return new CssElementStyleProp("transformOrigin", v.join(" ")); }

export function transformStyle(v: CSSGlobalValues | CSSTransformStyleValue) { return new CssElementStyleProp("transformStyle", v); }

export function transition(v: string) { return new CssElementStyleProp("transition", v); }

export function transitionDelay(v: string | 0) { return new CssElementStyleProp("transitionDelay", v); }

export function transitionDuration(v: string | 0) { return new CssElementStyleProp("transitionDuration", v); }

export function transitionProperty(v: string) { return new CssElementStyleProp("transitionProperty", v); }

export function transitionTimingFunction(v: string) { return new CssElementStyleProp("transitionTimingFunction", v); }

export function unicodeBidi(v: string) { return new CssElementStyleProp("unicodeBidi", v); }

export function userSelect(v: string) { return new CssElementStyleProp("userSelect", v); }

export function verticalAlign(v: CSSGlobalValues | CSSVerticalAlignValues) { return new CssElementStyleProp("verticalAlign", v); }

export function visibility(v: CSSGlobalValues | CSSVisiblityValues) { return new CssElementStyleProp("visibility", v); }

export function whiteSpace(v: CSSGlobalValues | CSSWhiteSpaceValues) { return new CssElementStyleProp("whiteSpace", v); }

export function widows(v: CSSGlobalValues | number) { return new CssElementStyleProp("widows", asInt(v)); }

export function width(v: CSSImportant<CSSGlobalValues | CSSSizePropertyValue>) { return new CssElementStyleProp("width", v); }

export function willChange(v: string) { return new CssElementStyleProp("willChange", v); }

export function wordBreak(v: string) { return new CssElementStyleProp("wordBreak", v); }

export function wordSpacing(v: string) { return new CssElementStyleProp("wordSpacing", v); }

export function wordWrap(v: string) { return new CssElementStyleProp("wordWrap", v); }

export function writingMode(v: CSSGlobalValues | CSSWritingModeValues) { return new CssElementStyleProp("writingMode", v); }

export function zIndex(v: CSSImportant<number>) { return new CssElementStyleProp("zIndex", asInt(v)); }

class CssRegularAtRuleSet<T extends CSSAtRuleRegular = CSSAtRuleRegular> extends PropSet<Prop> {
    constructor(selector: T, value: string) {
        super("@" + selector + " ", [new Prop(value)], ";");
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

class CssColorProfileProp<T extends CSSColorProfilePropName = CSSColorProfilePropName> extends CssDeclareProp {
    constructor(name: T, value: string) {
        super(name, value);
    }
}

export function colorProfile(name: CSSDashedName, src: string, renderingIntent: CssColorProfileProp<"rendering-intent">): PropSet;
export function colorProfile(name: CSSDashedName, src: string, components: CssColorProfileProp<"components">): PropSet;
export function colorProfile(name: CSSDashedName, src: string, renderingIntent: CssColorProfileProp<"rendering-intent">, components: CssColorProfileProp<"components">): PropSet;
export function colorProfile(name: CSSDashedName, src: string, ...props: CssColorProfileProp[]): PropSet {
    props.unshift(new CssColorProfileProp("src", src));
    return new SelectorPropSet("@color-profile " + name, props);
}

export function renderingIntent(value: CSSRenderingIntentValue): CssColorProfileProp { return new CssColorProfileProp("rendering-intent", value); }

export function components(...names: string[]) { return new CssColorProfileProp("components", names.join(", ")); }

class CssCounterStyleProp<T extends CSSCounterStylePropName = CSSCounterStylePropName> extends CssDeclareProp {
    constructor(name: T, value: string) {
        super(name, value);
    }
}

export function counterStyle(name: string, ...props: CssCounterStyleProp[]): PropSet {
    return new SelectorPropSet(`@counter-style ${name}`, props);
}

export function system(value: CSSCounterStyleSystemValue) { return new CssCounterStyleProp("system", value); }

export function negative(value: string) { return new CssCounterStyleProp("negative", value); }

export function prefix(value: string) { return new CssCounterStyleProp("prefix", value); }

export function suffix(value: string) { return new CssCounterStyleProp("suffix", value); }

export function range(value: "auto" | CSSCounterStyleRangeValue): CssCounterStyleProp<"range">;
export function range(...v: CSSCounterStyleRangeValue[]): CssCounterStyleProp<"range">;
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
        throw new Error("Cannot define multiple layers and rules at the same time")
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
