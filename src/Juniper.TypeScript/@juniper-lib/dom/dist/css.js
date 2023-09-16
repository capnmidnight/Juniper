import { isArray, isDefined, isNumber, isString } from "@juniper-lib/tslib/dist/typeChecks";
function asInt(v) {
    return isNumber(v) ? v.toFixed(0) : v;
}
export function perc(value) {
    return `${value}%`;
}
export function deg(value) {
    return `${value}deg`;
}
export function rad(value) {
    return `${value}rad`;
}
export function grad(value) {
    return `${value}grad`;
}
export function turn(value) {
    return `${value}turn`;
}
export function cap(value) {
    return `${value}cap`;
}
export function ch(value) {
    return `${value}ch`;
}
export function em(value) {
    return `${value}em`;
}
export function ex(value) {
    return `${value}ex`;
}
export function ic(value) {
    return `${value}ic`;
}
export function lh(value) {
    return `${value}lh`;
}
export function rem(value) {
    return `${value}rem`;
}
export function rlh(value) {
    return `${value}rlh`;
}
export function vh(value) {
    return `${value}vh`;
}
export function vw(value) {
    return `${value}vw`;
}
export function vi(value) {
    return `${value}vi`;
}
export function vb(value) {
    return `${value}vb`;
}
export function vmin(value) {
    return `${value}vmin`;
}
export function vmax(value) {
    return `${value}vmax`;
}
export function px(value) {
    return `${value}px`;
}
export function cm(value) {
    return `${value}cm`;
}
export function mm(value) {
    return `${value}mm`;
}
export function Q(value) {
    return `${value}Q`;
}
export function inch(value) {
    return `${value}in`;
}
export function pc(value) {
    return `${value}pc`;
}
export function pt(value) {
    return `${value}pt`;
}
export function fr(value) {
    return `${value}fr`;
}
export function hash(r, g, b, a) {
    if (isDefined(a)) {
        return `#${r}${g}${b}${a}`;
    }
    else {
        return `#${r}${g}${b}`;
    }
}
export function rgb(...v) {
    return `rgb(${v.join(", ")})`;
}
export function rgba(...v) {
    return `rgba(${v.join(", ")})`;
}
export function hsl(...v) {
    return `hsl(${v.join(", ")})`;
}
export function hsla(...v) {
    return `hsla(${v.join(", ")})`;
}
export function hwb(hue, whiteness, blackness, alpha) {
    if (isDefined(alpha)) {
        return `hwb(${hue} ${whiteness} ${blackness} / ${alpha})`;
    }
    else {
        return `hwb(${hue} ${whiteness} ${blackness})`;
    }
}
export function lch(lightness, chroma, hue, alpha) {
    if (isDefined(alpha)) {
        return `lch(${lightness} ${chroma} ${hue} / ${alpha})`;
    }
    else {
        return `lch(${lightness} ${chroma} ${hue})`;
    }
}
export function lab(lightness, a, b, alpha) {
    if (isDefined(alpha)) {
        return `lab(${lightness} ${a} ${b} / ${alpha})`;
    }
    else {
        return `lab(${lightness} ${a} ${b})`;
    }
}
export function matrix(a, b, c, d, tx, ty) {
    return `matrix(${a}, ${b}, ${c}, ${d}, ${tx}, ${ty})`;
}
export function matrix3d(a1, b1, c1, d1, a2, b2, c2, d2, a3, b3, c3, d3, a4, b4, c4, d4) {
    return `matrix3d(${a1}, ${b1}, ${c1}, ${d1}, ${a2}, ${b2}, ${c2}, ${d2}, ${a3}, ${b3}, ${c3}, ${d3}, ${a4}, ${b4}, ${c4}, ${d4})`;
}
export function perspectiv(v) {
    return `perspective(${v})`;
}
export function rotate(a) {
    return `rotate(${a})`;
}
export function rotate3d(x, y, z, a) {
    return `rotate3d(${x}, ${y}, ${z}, ${a})`;
}
export function rotateX(a) {
    return `rotateX(${a})`;
}
export function rotateY(a) {
    return `rotateY(${a})`;
}
export function rotateZ(a) {
    return `rotateZ(${a})`;
}
export function translate(x, y) {
    return `translate(${x}, ${y})`;
}
export function translate3d(x, y, z) {
    return `translate3d(${x}, ${y}, ${z})`;
}
export function translateX(x) {
    return `translateX(${x})`;
}
export function translateY(y) {
    return `translateY(${y})`;
}
export function translateZ(z) {
    return `translateZ(${z})`;
}
export function scale(x, y) {
    return `scale(${x}, ${y})`;
}
export function scale3d(x, y, z) {
    return `scale3d(${x}, ${y}, ${z})`;
}
export function scaleX(x) {
    return `scaleX(${x})`;
}
export function scaleY(y) {
    return `scaleY(${y})`;
}
export function scaleZ(z) {
    return `scaleZ(${z})`;
}
export function skew(x, y) {
    return `skew(${x}, ${y})`;
}
export function skewX(x) {
    return `skewX(${x})`;
}
export function skewY(y) {
    return `skewY(${y})`;
}
export function repeat(count, expr) {
    return `repeat(${count}, ${expr})`;
}
export function fitContent(len) {
    return `fit-content(${len})`;
}
export function minMax(min, max) {
    return `minmax(${min}, ${max})`;
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
export class Prop {
    constructor(_value) {
        this._value = _value;
    }
    get value() {
        return this._value;
    }
    toString() {
        return this.value;
    }
}
export class PropSet {
    constructor(pre, props, post) {
        this.pre = pre;
        this.props = props;
        this.post = post;
    }
    get value() {
        return this.pre
            + this.props.map(p => p.toString()).join("\n")
            + this.post;
    }
    toString() {
        return this.value;
    }
    applyToSheet(sheet) {
        sheet.insertRule(this.toString(), sheet.cssRules.length);
    }
}
class KeyValueProp extends Prop {
    constructor(_name, sep, value) {
        super(value);
        this._name = _name;
        this.sep = sep;
    }
    get name() {
        return this._name;
    }
    toString() {
        return this.name
            + this.sep
            + this.value
            + ";";
    }
}
class SelectorPropSet extends PropSet {
    constructor(selector, props) {
        super(selector + " {\n", props, "\n}\n");
    }
}
class CssDeclareProp extends KeyValueProp {
    constructor(key, value) {
        super(key, ": ", value);
    }
}
export class CssElementStyleProp extends CssDeclareProp {
    constructor(key, value) {
        super(key.replace(/[A-Z]/g, (m) => `-${m.toLocaleLowerCase()}`), value.toString());
        this.key = key;
        this.priority = "";
    }
    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    applyToElement(elem) {
        elem.style[this.key] = this.value + this.priority;
    }
    important() {
        this.priority = " !important";
        return this;
    }
    get value() {
        return super.value + this.priority;
    }
}
export function isCssElementStyleProp(obj) {
    return obj instanceof CssElementStyleProp;
}
class CssElementStylePropSet extends SelectorPropSet {
    constructor(selector, props) {
        super(selector, props);
    }
}
export function rule(selector, ...props) {
    return new CssElementStylePropSet(selector, props);
}
export function alignItems(v) { return new CssElementStyleProp("alignItems", v); }
export function alignContent(v) { return new CssElementStyleProp("alignContent", v); }
export function alignSelf(v) { return new CssElementStyleProp("alignSelf", v); }
export function all(v) { return new CssElementStyleProp("all", v); }
export function alignmentBaseline(v) { return new CssElementStyleProp("alignmentBaseline", v); }
export function animationDelay(...v) { return new CssElementStyleProp("animationDelay", v.join(", ")); }
export function animationDuration(...v) { return new CssElementStyleProp("animationDuration", v.join(", ")); }
export function animationDirection(...v) { return new CssElementStyleProp("animationDirection", v.join(", ")); }
export function animationFillMode(...v) { return new CssElementStyleProp("animationFillMode", v.join(", ")); }
export function animationIterationCount(...v) { return new CssElementStyleProp("animationIterationCount", v.join(", ")); }
export function animationName(...v) { return new CssElementStyleProp("animationName", v.join(", ")); }
export function animationPlayState(...v) { return new CssElementStyleProp("animationPlayState", v.join(", ")); }
export function animationTimingFunction(...v) { return new CssElementStyleProp("animationTimingFunction", v.join(" ")); }
export function animation(...v) { return new CssElementStyleProp("animation", v.join(" ")); }
export function appearance(v) { return new CssElementStyleProp("appearance", v); }
export function backdropFilter(...v) { return new CssElementStyleProp("backdropFilter", v.join(" ")); }
export function backfaceVisibility(v) { return new CssElementStyleProp("backfaceVisibility", v); }
export function backgroundAttachment(v) { return new CssElementStyleProp("backgroundAttachment", v); }
export function backgroundBlendMode(...v) { return new CssElementStyleProp("backgroundBlendMode", v.join(", ")); }
export function backgroundClip(v) { return new CssElementStyleProp("backgroundClip", v); }
export function backgroundColor(v) { return new CssElementStyleProp("backgroundColor", v); }
export function backgroundImage(...v) { return new CssElementStyleProp("backgroundImage", v.join(", ")); }
export function backgroundOrigin(v) { return new CssElementStyleProp("backgroundOrigin", v); }
export function backgroundPosition(...v) { return new CssElementStyleProp("backgroundPosition", v.join(" ")); }
export function backgroundPositionX(...v) { return new CssElementStyleProp("backgroundPositionX", v.join(" ")); }
export function backgroundPositionY(...v) { return new CssElementStyleProp("backgroundPositionY", v.join(" ")); }
export function backgroundRepeat(...v) { return new CssElementStyleProp("backgroundRepeat", v.join(" ")); }
export function backgroundSize(...v) { return new CssElementStyleProp("backgroundSize", v.join(" ")); }
export function background(...v) { return new CssElementStyleProp("background", v.join(", ")); }
export function baselineShift(v) { return new CssElementStyleProp("baselineShift", v); }
export function blockSize(v) { return new CssElementStyleProp("blockSize", v); }
export function border(v) { return new CssElementStyleProp("border", v); }
export function borderBlockEnd(v) { return new CssElementStyleProp("borderBlockEnd", v); }
export function borderBlockEndColor(v) { return new CssElementStyleProp("borderBlockEndColor", v); }
export function borderBlockEndStyle(v) { return new CssElementStyleProp("borderBlockEndStyle", v); }
export function borderBlockEndWidth(v) { return new CssElementStyleProp("borderBlockEndWidth", v); }
export function borderBlockStart(v) { return new CssElementStyleProp("borderBlockStart", v); }
export function borderBlockStartColor(v) { return new CssElementStyleProp("borderBlockStartColor", v); }
export function borderBlockStartStyle(v) { return new CssElementStyleProp("borderBlockStartStyle", v); }
export function borderBlockStartWidth(v) { return new CssElementStyleProp("borderBlockStartWidth", v); }
export function borderBottom(v) { return new CssElementStyleProp("borderBottom", v); }
export function borderBottomColor(v) { return new CssElementStyleProp("borderBottomColor", v); }
export function borderBottomLeftRadius(...v) { return new CssElementStyleProp("borderBottomLeftRadius", v.join(" ")); }
export function borderBottomRightRadius(...v) { return new CssElementStyleProp("borderBottomRightRadius", v.join(" ")); }
export function borderBottomStyle(v) { return new CssElementStyleProp("borderBottomStyle", v); }
export function borderBottomWidth(v) { return new CssElementStyleProp("borderBottomWidth", v); }
export function borderCollapse(v) { return new CssElementStyleProp("borderCollapse", v); }
export function borderColor(...v) { return new CssElementStyleProp("borderColor", v.join(" ")); }
export function borderImage(v) { return new CssElementStyleProp("borderImage", v); }
export function borderImageOutset(...v) { return new CssElementStyleProp("borderImageOutset", v.join(" ")); }
export function borderImageRepeat(...v) { return new CssElementStyleProp("borderImageRepeat", v.join(" ")); }
export function borderImageSlice(...v) { return new CssElementStyleProp("borderImageSlice", v.join(" ")); }
export function borderImageSource(v) { return new CssElementStyleProp("borderImageSource", v); }
export function borderImageWidth(...v) { return new CssElementStyleProp("borderImageWidth", v.join(" ")); }
export function borderInlineEnd(v) { return new CssElementStyleProp("borderInlineEnd", v); }
export function borderInlineEndColor(v) { return new CssElementStyleProp("borderInlineEndColor", v); }
export function borderInlineEndStyle(v) { return new CssElementStyleProp("borderInlineEndStyle", v); }
export function borderInlineEndWidth(v) { return new CssElementStyleProp("borderInlineEndWidth", v); }
export function borderInlineStart(v) { return new CssElementStyleProp("borderInlineStart", v); }
export function borderInlineStartColor(v) { return new CssElementStyleProp("borderInlineStartColor", v); }
export function borderInlineStartStyle(v) { return new CssElementStyleProp("borderInlineStartStyle", v); }
export function borderInlineStartWidth(v) { return new CssElementStyleProp("borderInlineStartWidth", v); }
export function borderLeft(v) { return new CssElementStyleProp("borderLeft", v); }
export function borderLeftColor(v) { return new CssElementStyleProp("borderLeftColor", v); }
export function borderLeftStyle(v) { return new CssElementStyleProp("borderLeftStyle", v); }
export function borderLeftWidth(v) { return new CssElementStyleProp("borderLeftWidth", v); }
export function borderRadius(...v) {
    let value = null;
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
export function borderRight(v) { return new CssElementStyleProp("borderRight", v); }
export function borderRightColor(v) { return new CssElementStyleProp("borderRightColor", v); }
export function borderRightStyle(v) { return new CssElementStyleProp("borderRightStyle", v); }
export function borderRightWidth(v) { return new CssElementStyleProp("borderRightWidth", v); }
export function borderSpacing(v) { return new CssElementStyleProp("borderSpacing", v); }
export function borderStyle(...v) { return new CssElementStyleProp("borderStyle", v.join(" ")); }
export function borderTop(v) { return new CssElementStyleProp("borderTop", v); }
export function borderTopColor(v) { return new CssElementStyleProp("borderTopColor", v); }
export function borderTopLeftRadius(...v) { return new CssElementStyleProp("borderTopLeftRadius", v.join(" ")); }
export function borderTopRightRadius(...v) { return new CssElementStyleProp("borderTopRightRadius", v.join(" ")); }
export function borderTopStyle(v) { return new CssElementStyleProp("borderTopStyle", v); }
export function borderTopWidth(v) { return new CssElementStyleProp("borderTopWidth", v); }
export function borderWidth(...v) { return new CssElementStyleProp("borderWidth", v.join(" ")); }
export function bottom(v) { return new CssElementStyleProp("bottom", v); }
export function boxShadow(v) { return new CssElementStyleProp("boxShadow", v); }
export function boxSizing(v) { return new CssElementStyleProp("boxSizing", v); }
export function breakAfter(v) { return new CssElementStyleProp("breakAfter", v); }
export function breakBefore(v) { return new CssElementStyleProp("breakBefore", v); }
export function breakInside(v) { return new CssElementStyleProp("breakInside", v); }
export function captionSide(v) { return new CssElementStyleProp("captionSide", v); }
export function caretColor(v) { return new CssElementStyleProp("caretColor", v); }
export function clear(v) { return new CssElementStyleProp("clear", v); }
export function clip(v) { return new CssElementStyleProp("clip", v); }
export function clipPath(v) { return new CssElementStyleProp("clipPath", v); }
export function clipRule(v) { return new CssElementStyleProp("clipRule", v); }
export function color(v) { return new CssElementStyleProp("color", v); }
export function colorInterpolation(v) { return new CssElementStyleProp("colorInterpolation", v); }
export function colorInterpolationFilters(v) { return new CssElementStyleProp("colorInterpolationFilters", v); }
export function colorScheme(v) { return new CssElementStyleProp("colorScheme", v); }
export function columnCount(v) { return new CssElementStyleProp("columnCount", v); }
export function columnFill(v) { return new CssElementStyleProp("columnFill", v); }
export function columnGap(v) { return new CssElementStyleProp("columnGap", v); }
export function gridColumnGap(v) { return new CssElementStyleProp("gridColumnGap", v); }
export function columnRule(v) { return new CssElementStyleProp("columnRule", v); }
export function columnRuleColor(v) { return new CssElementStyleProp("columnRuleColor", v); }
export function columnRuleStyle(v) { return new CssElementStyleProp("columnRuleStyle", v); }
export function columnRuleWidth(v) { return new CssElementStyleProp("columnRuleWidth", v); }
export function columnSpan(v) { return new CssElementStyleProp("columnSpan", v); }
export function columnWidth(v) { return new CssElementStyleProp("columnWidth", v); }
export function columns(v) { return new CssElementStyleProp("columns", v); }
export function contain(v) { return new CssElementStyleProp("contain", v); }
export function counterIncrement(v) { return new CssElementStyleProp("counterIncrement", v); }
export function counterReset(v) { return new CssElementStyleProp("counterReset", v); }
export function cursor(v) { return new CssElementStyleProp("cursor", v); }
export function direction(v) { return new CssElementStyleProp("direction", v); }
export function display(v) { return new CssElementStyleProp("display", v); }
export function dominantBaseline(v) { return new CssElementStyleProp("dominantBaseline", v); }
export function emptyCells(v) { return new CssElementStyleProp("emptyCells", v); }
export function fill(v) { return new CssElementStyleProp("fill", v); }
export function fillOpacity(v) { return new CssElementStyleProp("fillOpacity", v); }
export function fillRule(v) { return new CssElementStyleProp("fillRule", v); }
export function filter(v) { return new CssElementStyleProp("filter", v); }
export function flexBasis(v) { return new CssElementStyleProp("flexBasis", v); }
export function flexDirection(v) { return new CssElementStyleProp("flexDirection", v); }
export function flexWrap(v) { return new CssElementStyleProp("flexWrap", v); }
export function flexFlow(...v) { return new CssElementStyleProp("flexFlow", v.join(" ")); }
export function flex(...v) { return new CssElementStyleProp("flex", v.join(" ")); }
export function flexGrow(v) { return new CssElementStyleProp("flexGrow", v); }
export function flexShrink(v) { return new CssElementStyleProp("flexShrink", v); }
export function float(v) { return new CssElementStyleProp("float", v); }
export function floodColor(v) { return new CssElementStyleProp("floodColor", v); }
export function floodOpacity(v) { return new CssElementStyleProp("floodOpacity", v); }
export function font(v) { return new CssElementStyleProp("font", v); }
export function fontFamily(v) { return new CssElementStyleProp("fontFamily", v); }
export function fontFeatureSettings(v) { return new CssElementStyleProp("fontFeatureSettings", v); }
export function fontKerning(v) { return new CssElementStyleProp("fontKerning", v); }
export function fontOpticalSizing(v) { return new CssElementStyleProp("fontOpticalSizing", v); }
export function fontSize(v) { return new CssElementStyleProp("fontSize", v); }
export function fontStretch(v) { return new CssElementStyleProp("fontStretch", v); }
export function fontStyle(v) { return new CssElementStyleProp("fontStyle", v); }
export function fontVariant(v) { return new CssElementStyleProp("fontVariant", v); }
export function fontVariantCaps(v) { return new CssElementStyleProp("fontVariantCaps", v); }
export function fontVariantEastAsian(v) { return new CssElementStyleProp("fontVariantEastAsian", v); }
export function fontVariantLigatures(v) { return new CssElementStyleProp("fontVariantLigatures", v); }
export function fontVariantNumeric(v) { return new CssElementStyleProp("fontVariantNumeric", v); }
export function fontVariationSettings(v) { return new CssElementStyleProp("fontVariationSettings", v); }
export function fontWeight(v) { return new CssElementStyleProp("fontWeight", v); }
export function gap(...v) { return new CssElementStyleProp("gap", v.join(" ")); }
export function gridGap(...v) { return new CssElementStyleProp("gridGap", v.join(" ")); }
export function grid(v) { return new CssElementStyleProp("grid", v); }
export function gridArea(vOrRowStart, colStart, rowEnd, colEnd) {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, colStart, rowEnd, colEnd]
            .filter(isDefined)
            .join("/");
    }
    return new CssElementStyleProp("gridArea", vOrRowStart);
}
export function gridAutoColumns(v) { return new CssElementStyleProp("gridAutoColumns", v); }
export function gridAutoFlow(v) { return new CssElementStyleProp("gridAutoFlow", v); }
export function gridAutoRows(v) { return new CssElementStyleProp("gridAutoRows", v); }
export function gridColumn(vOrColStart, colEnd) {
    if (!isString(vOrColStart)) {
        vOrColStart = [vOrColStart, colEnd]
            .filter(isDefined)
            .join("/");
    }
    return new CssElementStyleProp("gridColumn", vOrColStart);
}
export function gridColumnEnd(v) { return new CssElementStyleProp("gridColumnEnd", v); }
export function gridColumnStart(v) { return new CssElementStyleProp("gridColumnStart", v); }
export function gridRow(vOrRowStart, rowEnd) {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, rowEnd]
            .filter(isDefined)
            .join("/");
    }
    return new CssElementStyleProp("gridRow", vOrRowStart);
}
export function gridRowEnd(v) { return new CssElementStyleProp("gridRowEnd", v); }
export function gridRowStart(v) { return new CssElementStyleProp("gridRowStart", v); }
export function gridTemplate(v) { return new CssElementStyleProp("gridTemplate", v); }
export function gridTemplateAreas(...v) { return new CssElementStyleProp("gridTemplateAreas", v.map((r) => "\"" + r + "\"").join("\n")); }
export function gridTemplateColumns(...v) { return new CssElementStyleProp("gridTemplateColumns", v.join(" ")); }
export function gridTemplateRows(...v) { return new CssElementStyleProp("gridTemplateRows", v.join(" ")); }
export function height(v) { return new CssElementStyleProp("height", v); }
export function hyphens(v) { return new CssElementStyleProp("hyphens", v); }
export function imageOrientation(v) { return new CssElementStyleProp("imageOrientation", v); }
export function imageRendering(v) { return new CssElementStyleProp("imageRendering", v); }
export function inlineSize(v) { return new CssElementStyleProp("inlineSize", v); }
export function isolation(v) { return new CssElementStyleProp("isolation", v); }
export function justifyContent(v) { return new CssElementStyleProp("justifyContent", v); }
export function justifyItems(v) { return new CssElementStyleProp("justifyItems", v); }
export function justifySelf(v) { return new CssElementStyleProp("justifySelf", v); }
export function left(v) { return new CssElementStyleProp("left", v); }
export function letterSpacing(v) { return new CssElementStyleProp("letterSpacing", v); }
export function lightingColor(v) { return new CssElementStyleProp("lightingColor", v); }
export function lineBreak(v) { return new CssElementStyleProp("lineBreak", v); }
export function lineHeight(v) { return new CssElementStyleProp("lineHeight", v); }
export function listStyle(v) { return new CssElementStyleProp("listStyle", v); }
export function listStyleImage(v) { return new CssElementStyleProp("listStyleImage", v); }
export function listStylePosition(v) { return new CssElementStyleProp("listStylePosition", v); }
export function listStyleType(v) { return new CssElementStyleProp("listStyleType", v); }
export function margin(...v) { return new CssElementStyleProp("margin", v.join(" ")); }
export function marginBlockEnd(v) { return new CssElementStyleProp("marginBlockEnd", v); }
export function marginBlockStart(v) { return new CssElementStyleProp("marginBlockStart", v); }
export function marginBottom(v) { return new CssElementStyleProp("marginBottom", v); }
export function marginInlineEnd(v) { return new CssElementStyleProp("marginInlineEnd", v); }
export function marginInlineStart(v) { return new CssElementStyleProp("marginInlineStart", v); }
export function marginLeft(v) { return new CssElementStyleProp("marginLeft", v); }
export function marginRight(v) { return new CssElementStyleProp("marginRight", v); }
export function marginTop(v) { return new CssElementStyleProp("marginTop", v); }
export function marker(v) { return new CssElementStyleProp("marker", v); }
export function markerEnd(v) { return new CssElementStyleProp("markerEnd", v); }
export function markerMid(v) { return new CssElementStyleProp("markerMid", v); }
export function markerStart(v) { return new CssElementStyleProp("markerStart", v); }
export function mask(v) { return new CssElementStyleProp("mask", v); }
export function maskType(v) { return new CssElementStyleProp("maskType", v); }
export function maxBlockSize(v) { return new CssElementStyleProp("maxBlockSize", v); }
export function maxHeight(v) { return new CssElementStyleProp("maxHeight", v); }
export function maxInlineSize(v) { return new CssElementStyleProp("maxInlineSize", v); }
export function maxWidth(v) { return new CssElementStyleProp("maxWidth", v); }
export function minBlockSize(v) { return new CssElementStyleProp("minBlockSize", v); }
export function minHeight(v) { return new CssElementStyleProp("minHeight", v); }
export function minInlineSize(v) { return new CssElementStyleProp("minInlineSize", v); }
export function minWidth(v) { return new CssElementStyleProp("minWidth", v); }
export function mixBlendMode(v) { return new CssElementStyleProp("mixBlendMode", v); }
export function objectFit(v) { return new CssElementStyleProp("objectFit", v); }
export function objectPosition(v) { return new CssElementStyleProp("objectPosition", v); }
export function offset(v) { return new CssElementStyleProp("offset", v); }
export function offsetDistance(v) { return new CssElementStyleProp("offsetDistance", v); }
export function offsetPath(v) { return new CssElementStyleProp("offsetPath", v); }
export function offsetRotate(v) { return new CssElementStyleProp("offsetRotate", v); }
export function opacity(v) { return new CssElementStyleProp("opacity", v); }
export function order(v) { return new CssElementStyleProp("order", v); }
export function orphans(v) { return new CssElementStyleProp("orphans", v); }
export function outline(v) { return new CssElementStyleProp("outline", v); }
export function outlineColor(v) { return new CssElementStyleProp("outlineColor", v); }
export function outlineOffset(v) { return new CssElementStyleProp("outlineOffset", v); }
export function outlineStyle(v) { return new CssElementStyleProp("outlineStyle", v); }
export function outlineWidth(v) { return new CssElementStyleProp("outlineWidth", v); }
export function overflow(...v) { return new CssElementStyleProp("overflow", v.join(" ")); }
export function overflowX(v) { return new CssElementStyleProp("overflowX", v); }
export function overflowY(v) { return new CssElementStyleProp("overflowY", v); }
export function overflowAnchor(v) { return new CssElementStyleProp("overflowAnchor", v); }
export function overflowWrap(v) { return new CssElementStyleProp("overflowWrap", v); }
export function overscrollBehavior(v) { return new CssElementStyleProp("overscrollBehavior", v); }
export function overscrollBehaviorBlock(v) { return new CssElementStyleProp("overscrollBehaviorBlock", v); }
export function overscrollBehaviorInline(v) { return new CssElementStyleProp("overscrollBehaviorInline", v); }
export function overscrollBehaviorX(v) { return new CssElementStyleProp("overscrollBehaviorX", v); }
export function overscrollBehaviorY(v) { return new CssElementStyleProp("overscrollBehaviorY", v); }
export function padding(...v) { return new CssElementStyleProp("padding", v.join(" ")); }
export function paddingBlockEnd(v) { return new CssElementStyleProp("paddingBlockEnd", v); }
export function paddingBlockStart(v) { return new CssElementStyleProp("paddingBlockStart", v); }
export function paddingBottom(v) { return new CssElementStyleProp("paddingBottom", v); }
export function paddingInlineEnd(v) { return new CssElementStyleProp("paddingInlineEnd", v); }
export function paddingInlineStart(v) { return new CssElementStyleProp("paddingInlineStart", v); }
export function paddingLeft(v) { return new CssElementStyleProp("paddingLeft", v); }
export function paddingRight(v) { return new CssElementStyleProp("paddingRight", v); }
export function paddingTop(v) { return new CssElementStyleProp("paddingTop", v); }
export function pageBreakAfter(v) { return new CssElementStyleProp("pageBreakAfter", v); }
export function pageBreakBefore(v) { return new CssElementStyleProp("pageBreakBefore", v); }
export function pageBreakInside(v) { return new CssElementStyleProp("pageBreakInside", v); }
export function paintOrder(v) { return new CssElementStyleProp("paintOrder", v); }
export function perspective(v) { return new CssElementStyleProp("perspective", v); }
export function perspectiveOrigin(v) { return new CssElementStyleProp("perspectiveOrigin", v); }
export function placeContent(v) { return new CssElementStyleProp("placeContent", v); }
export function placeItems(v) { return new CssElementStyleProp("placeItems", v); }
export function placeSelf(v) { return new CssElementStyleProp("placeSelf", v); }
export function pointerEvents(v) { return new CssElementStyleProp("pointerEvents", v); }
export function position(v) { return new CssElementStyleProp("position", v); }
export function quotes(v) { return new CssElementStyleProp("quotes", v); }
export function resize(v) { return new CssElementStyleProp("resize", v); }
export function right(v) { return new CssElementStyleProp("right", v); }
export function rowGap(v) { return new CssElementStyleProp("rowGap", v); }
export function gridRowGap(v) { return new CssElementStyleProp("gridRowGap", v); }
export function rubyPosition(v) { return new CssElementStyleProp("rubyPosition", v); }
export function scrollBehavior(v) { return new CssElementStyleProp("scrollBehavior", v); }
export function scrollMargin(v) { return new CssElementStyleProp("scrollMargin", v); }
export function scrollMarginBlock(v) { return new CssElementStyleProp("scrollMarginBlock", v); }
export function scrollMarginBlockEnd(v) { return new CssElementStyleProp("scrollMarginBlockEnd", v); }
export function scrollMarginBlockStart(v) { return new CssElementStyleProp("scrollMarginBlockStart", v); }
export function scrollMarginBottom(v) { return new CssElementStyleProp("scrollMarginBottom", v); }
export function scrollMarginInline(v) { return new CssElementStyleProp("scrollMarginInline", v); }
export function scrollMarginInlineEnd(v) { return new CssElementStyleProp("scrollMarginInlineEnd", v); }
export function scrollMarginInlineStart(v) { return new CssElementStyleProp("scrollMarginInlineStart", v); }
export function scrollMarginLeft(v) { return new CssElementStyleProp("scrollMarginLeft", v); }
export function scrollMarginRight(v) { return new CssElementStyleProp("scrollMarginRight", v); }
export function scrollMarginTop(v) { return new CssElementStyleProp("scrollMarginTop", v); }
export function scrollPadding(v) { return new CssElementStyleProp("scrollPadding", v); }
export function scrollPaddingBlock(v) { return new CssElementStyleProp("scrollPaddingBlock", v); }
export function scrollPaddingBlockEnd(v) { return new CssElementStyleProp("scrollPaddingBlockEnd", v); }
export function scrollPaddingBlockStart(v) { return new CssElementStyleProp("scrollPaddingBlockStart", v); }
export function scrollPaddingBottom(v) { return new CssElementStyleProp("scrollPaddingBottom", v); }
export function scrollPaddingInline(v) { return new CssElementStyleProp("scrollPaddingInline", v); }
export function scrollPaddingInlineEnd(v) { return new CssElementStyleProp("scrollPaddingInlineEnd", v); }
export function scrollPaddingInlineStart(v) { return new CssElementStyleProp("scrollPaddingInlineStart", v); }
export function scrollPaddingLeft(v) { return new CssElementStyleProp("scrollPaddingLeft", v); }
export function scrollPaddingRight(v) { return new CssElementStyleProp("scrollPaddingRight", v); }
export function scrollPaddingTop(v) { return new CssElementStyleProp("scrollPaddingTop", v); }
export function scrollSnapAlign(v) { return new CssElementStyleProp("scrollSnapAlign", v); }
export function scrollSnapStop(v) { return new CssElementStyleProp("scrollSnapStop", v); }
export function scrollSnapType(v) { return new CssElementStyleProp("scrollSnapType", v); }
export function shapeImageThreshold(v) { return new CssElementStyleProp("shapeImageThreshold", v); }
export function shapeMargin(v) { return new CssElementStyleProp("shapeMargin", v); }
export function shapeOutside(v) { return new CssElementStyleProp("shapeOutside", v); }
export function shapeRendering(v) { return new CssElementStyleProp("shapeRendering", v); }
export function stopColor(v) { return new CssElementStyleProp("stopColor", v); }
export function stopOpacity(v) { return new CssElementStyleProp("stopOpacity", v); }
export function stroke(v) { return new CssElementStyleProp("stroke", v); }
export function strokeDasharray(v) { return new CssElementStyleProp("strokeDasharray", v); }
export function strokeDashoffset(v) { return new CssElementStyleProp("strokeDashoffset", v); }
export function strokeLinecap(v) { return new CssElementStyleProp("strokeLinecap", v); }
export function strokeLinejoin(v) { return new CssElementStyleProp("strokeLinejoin", v); }
export function strokeMiterlimit(v) { return new CssElementStyleProp("strokeMiterlimit", v); }
export function strokeOpacity(v) { return new CssElementStyleProp("strokeOpacity", v); }
export function strokeWidth(v) { return new CssElementStyleProp("strokeWidth", v); }
export function tabSize(v) { return new CssElementStyleProp("tabSize", v); }
export function tableLayout(v) { return new CssElementStyleProp("tableLayout", v); }
export function textAlign(v) { return new CssElementStyleProp("textAlign", v); }
export function textAlignLast(v) { return new CssElementStyleProp("textAlignLast", v); }
export function textAnchor(v) { return new CssElementStyleProp("textAnchor", v); }
export function textCombineUpright(v) { return new CssElementStyleProp("textCombineUpright", v); }
export function textDecoration(v) { return new CssElementStyleProp("textDecoration", v); }
export function textDecorationColor(v) { return new CssElementStyleProp("textDecorationColor", v); }
export function textDecorationLine(v) { return new CssElementStyleProp("textDecorationLine", v); }
export function textDecorationSkipInk(v) { return new CssElementStyleProp("textDecorationSkipInk", v); }
export function textDecorationStyle(v) { return new CssElementStyleProp("textDecorationStyle", v); }
export function textIndent(v) { return new CssElementStyleProp("textIndent", v); }
export function textOrientation(v) { return new CssElementStyleProp("textOrientation", v); }
export function textOverflow(v) { return new CssElementStyleProp("textOverflow", v); }
export function textRendering(v) { return new CssElementStyleProp("textRendering", v); }
export function textShadow(v) { return new CssElementStyleProp("textShadow", v); }
export function textTransform(v) { return new CssElementStyleProp("textTransform", v); }
export function textUnderlinePosition(v) { return new CssElementStyleProp("textUnderlinePosition", v); }
export function top(v) { return new CssElementStyleProp("top", v); }
export function touchAction(v) { return new CssElementStyleProp("touchAction", v); }
export function transform(...v) { return new CssElementStyleProp("transform", v.join(" ")); }
export function transformBox(v) { return new CssElementStyleProp("transformBox", v); }
export function transformOrigin(...v) { return new CssElementStyleProp("transformOrigin", v.join(" ")); }
export function transformStyle(v) { return new CssElementStyleProp("transformStyle", v); }
export function transition(v) { return new CssElementStyleProp("transition", v); }
export function transitionDelay(v) { return new CssElementStyleProp("transitionDelay", v); }
export function transitionDuration(v) { return new CssElementStyleProp("transitionDuration", v); }
export function transitionProperty(v) { return new CssElementStyleProp("transitionProperty", v); }
export function transitionTimingFunction(v) { return new CssElementStyleProp("transitionTimingFunction", v); }
export function unicodeBidi(v) { return new CssElementStyleProp("unicodeBidi", v); }
export function userSelect(v) { return new CssElementStyleProp("userSelect", v); }
export function verticalAlign(v) { return new CssElementStyleProp("verticalAlign", v); }
export function visibility(v) { return new CssElementStyleProp("visibility", v); }
export function whiteSpace(v) { return new CssElementStyleProp("whiteSpace", v); }
export function widows(v) { return new CssElementStyleProp("widows", asInt(v)); }
export function width(v) { return new CssElementStyleProp("width", v); }
export function willChange(v) { return new CssElementStyleProp("willChange", v); }
export function wordBreak(v) { return new CssElementStyleProp("wordBreak", v); }
export function wordSpacing(v) { return new CssElementStyleProp("wordSpacing", v); }
export function wordWrap(v) { return new CssElementStyleProp("wordWrap", v); }
export function writingMode(v) { return new CssElementStyleProp("writingMode", v); }
export function zIndex(v) { return new CssElementStyleProp("zIndex", asInt(v)); }
class CssRegularAtRuleSet extends PropSet {
    constructor(selector, value) {
        super("@" + selector + " ", [new Prop(value)], ";");
    }
}
export function cssCharset(value) {
    return new CssRegularAtRuleSet("charset", value);
}
export function cssImport(value) {
    return new CssRegularAtRuleSet("import", value);
}
export function cssNamespace(value) {
    return new CssRegularAtRuleSet("namespace", value);
}
class CssColorProfileProp extends CssDeclareProp {
    constructor(name, value) {
        super(name, value);
    }
}
export function colorProfile(name, src, ...props) {
    props.unshift(new CssColorProfileProp("src", src));
    return new SelectorPropSet("@color-profile " + name, props);
}
export function renderingIntent(value) { return new CssColorProfileProp("rendering-intent", value); }
export function components(...names) { return new CssColorProfileProp("components", names.join(", ")); }
class CssCounterStyleProp extends CssDeclareProp {
    constructor(name, value) {
        super(name, value);
    }
}
export function counterStyle(name, ...props) {
    return new SelectorPropSet(`@counter-style ${name}`, props);
}
export function system(value) { return new CssCounterStyleProp("system", value); }
export function negative(value) { return new CssCounterStyleProp("negative", value); }
export function prefix(value) { return new CssCounterStyleProp("prefix", value); }
export function suffix(value) { return new CssCounterStyleProp("suffix", value); }
export function range(...v) { return new CssCounterStyleProp("range", v.join(", ")); }
export function pad(count, symbol) { return new CssCounterStyleProp("pad", `${asInt(count)} ${JSON.stringify(symbol)}`); }
export function fallback(value) { return new CssCounterStyleProp("fallback", value); }
export function symbols(value) { return new CssCounterStyleProp("symbols", value); }
export function additiveSymbols(...v) { return new CssCounterStyleProp("additive-symbols", v.join(", ")); }
export function speakAs(value) { return new CssCounterStyleProp("speak-as", value); }
export function layer(...namesOrRules) {
    const names = [
        ...namesOrRules.filter(isString)
    ];
    const rules = namesOrRules.filter(v => v instanceof PropSet);
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
export function media(query, ...props) {
    return new SelectorPropSet("@media " + query, props);
}
//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@page
//TODO https://developer.mozilla.org/en-US/docs/Web/CSS/@supports
//# sourceMappingURL=css.js.map