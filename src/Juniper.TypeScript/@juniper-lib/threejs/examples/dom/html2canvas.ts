import { Color } from 'three';

const canvases = new WeakMap();

interface Clip {
    x: number;
    y: number;
    width: number;
    height: number;
}

class Clipper {

    private readonly clips = new Array<Clip>();
    private isClipping = false;

    constructor(private readonly context: CanvasRenderingContext2D) {
    }



    private doClip() {

        if (this.isClipping) {

            this.isClipping = false;
            this.context.restore();

        }

        if (this.clips.length === 0) return;

        let minX = - Infinity, minY = - Infinity;
        let maxX = Infinity, maxY = Infinity;

        for (let i = 0; i < this.clips.length; i++) {

            const clip = this.clips[i];

            minX = Math.max(minX, clip.x);
            minY = Math.max(minY, clip.y);
            maxX = Math.min(maxX, clip.x + clip.width);
            maxY = Math.min(maxY, clip.y + clip.height);

        }

        this.context.save();
        this.context.beginPath();
        this.context.rect(minX, minY, maxX - minX, maxY - minY);
        this.context.clip();

        this.isClipping = true;

    }



    add(clip: Clip) {

        this.clips.push(clip);
        this.doClip();

    }

    remove() {
        this.clips.pop();
        this.doClip();
    }
}

type BorderName =
    "borderTop"
    | "borderBottom"
    | "borderLeft"
    | "borderRight";


export function html2canvas(element: HTMLElement): HTMLCanvasElement {

    const range = document.createRange();
    const color = new Color();

    function drawText(style: CSSStyleDeclaration, x: number, y: number, string: string) {

        if (string !== '') {

            if (style.textTransform === 'uppercase') {

                string = string.toUpperCase();

            }

            context.font = style.fontWeight + ' ' + style.fontSize + ' ' + style.fontFamily;
            context.textBaseline = 'top';
            context.fillStyle = style.color;
            context.fillText(string, x, y + parseFloat(style.fontSize) * 0.1);

        }

    }

    function buildRectPath(x: number, y: number, w: number, h: number, r: number) {

        if (w < 2 * r) r = w / 2;
        if (h < 2 * r) r = h / 2;

        context.beginPath();
        context.moveTo(x + r, y);
        context.arcTo(x + w, y, x + w, y + h, r);
        context.arcTo(x + w, y + h, x, y + h, r);
        context.arcTo(x, y + h, x, y, r);
        context.arcTo(x, y, x + w, y, r);
        context.closePath();

    }

    function drawBorder(style: CSSStyleDeclaration, which: BorderName, x: number, y: number, width: number, height: number) {

        const borderWidth = style[`${which}Width`];
        const borderStyle = style[`${which}Style`];
        const borderColor = style[`${which}Color`];

        if (borderWidth !== '0px' && borderStyle !== 'none' && borderColor !== 'transparent' && borderColor !== 'rgba(0, 0, 0, 0)') {

            context.strokeStyle = borderColor;
            context.lineWidth = parseFloat(borderWidth);
            context.beginPath();
            context.moveTo(x, y);
            context.lineTo(x + width, y + height);
            context.stroke();

        }

    }

    function drawElement(node: ChildNode, style: CSSStyleDeclaration = null) {

        let x = 0, y = 0, width = 0, height = 0;

        if (node.nodeType === Node.COMMENT_NODE) {

            return;

        } else if (node.nodeType === Node.TEXT_NODE) {

            // text

            range.selectNode(node);

            const rect = range.getBoundingClientRect();

            x = rect.left - offset.left - 0.5;
            y = rect.top - offset.top - 0.5;
            width = rect.width;
            height = rect.height;

            drawText(style, x, y, node.nodeValue.trim());

        } else if (node instanceof HTMLCanvasElement) {

            // Canvas element
            if (node.style.display === 'none') return;

            context.save();
            const dpr = window.devicePixelRatio;
            context.scale(1 / dpr, 1 / dpr);
            context.drawImage(node, 0, 0);
            context.restore();

        } else {
            const element = node as unknown as HTMLElement;
            if (element.style.display === 'none') return;

            const rect = element.getBoundingClientRect();

            x = rect.left - offset.left - 0.5;
            y = rect.top - offset.top - 0.5;
            width = rect.width;
            height = rect.height;

            style = window.getComputedStyle(element);

            // Get the border of the element used for fill and border

            buildRectPath(x, y, width, height, parseFloat(style.borderRadius));

            const backgroundColor = style.backgroundColor;

            if (backgroundColor !== 'transparent' && backgroundColor !== 'rgba(0, 0, 0, 0)') {

                context.fillStyle = backgroundColor;
                context.fill();

            }

            // If all the borders match then stroke the round rectangle

            const borders = ['borderTop', 'borderLeft', 'borderBottom', 'borderRight'] as Array<BorderName>;

            let match = true;
            let prevBorder = null;

            for (const border of borders) {

                if (prevBorder !== null) {

                    match = (style[`${border}Width`] === style[`${prevBorder}Width`]) &&
                        (style[`${border}Color`] === style[`${prevBorder}Color`]) &&
                        (style[`${border}Style`] === style[`${prevBorder}Style`]);

                }

                if (match === false) break;

                prevBorder = border;

            }

            if (match === true) {

                // They all match so stroke the rectangle from before allows for border-radius

                const width = parseFloat(style.borderTopWidth);

                if (style.borderTopWidth !== '0px' && style.borderTopStyle !== 'none' && style.borderTopColor !== 'transparent' && style.borderTopColor !== 'rgba(0, 0, 0, 0)') {

                    context.strokeStyle = style.borderTopColor;
                    context.lineWidth = width;
                    context.stroke();

                }

            } else {

                // Otherwise draw individual borders

                drawBorder(style, 'borderTop', x, y, width, 0);
                drawBorder(style, 'borderLeft', x, y, 0, height);
                drawBorder(style, 'borderBottom', x, y + height, width, 0);
                drawBorder(style, 'borderRight', x + width, y, 0, height);

            }

            if (node instanceof HTMLInputElement) {

                let accentColor = style.accentColor;

                if (accentColor === undefined || accentColor === 'auto') accentColor = style.color;

                color.set(accentColor);

                const luminance = Math.sqrt(0.299 * (color.r ** 2) + 0.587 * (color.g ** 2) + 0.114 * (color.b ** 2));
                const accentTextColor = luminance < 0.5 ? 'white' : '#111111';

                if (node.type === 'radio') {

                    buildRectPath(x, y, width, height, height);

                    context.fillStyle = 'white';
                    context.strokeStyle = accentColor;
                    context.lineWidth = 1;
                    context.fill();
                    context.stroke();

                    if (node.checked) {

                        buildRectPath(x + 2, y + 2, width - 4, height - 4, height);

                        context.fillStyle = accentColor;
                        context.strokeStyle = accentTextColor;
                        context.lineWidth = 2;
                        context.fill();
                        context.stroke();

                    }

                }

                if (node.type === 'checkbox') {

                    buildRectPath(x, y, width, height, 2);

                    context.fillStyle = node.checked ? accentColor : 'white';
                    context.strokeStyle = node.checked ? accentTextColor : accentColor;
                    context.lineWidth = 1;
                    context.stroke();
                    context.fill();

                    if (node.checked) {

                        const currentTextAlign = context.textAlign;

                        context.textAlign = 'center';

                        const properties = {
                            color: accentTextColor,
                            fontFamily: style.fontFamily,
                            fontSize: height + 'px',
                            fontWeight: 'bold'
                        } as CSSStyleDeclaration;

                        drawText(properties, x + (width / 2), y, '✔');

                        context.textAlign = currentTextAlign;

                    }

                }

                if (node.type === 'range') {

                    const min = parseFloat(node.min);
                    const max = parseFloat(node.max);
                    const value = node.valueAsNumber;
                    const position = ((value - min) / (max - min)) * (width - height);

                    buildRectPath(x, y + (height / 4), width, height / 2, height / 4);
                    context.fillStyle = accentTextColor;
                    context.strokeStyle = accentColor;
                    context.lineWidth = 1;
                    context.fill();
                    context.stroke();

                    buildRectPath(x, y + (height / 4), position + (height / 2), height / 2, height / 4);
                    context.fillStyle = accentColor;
                    context.fill();

                    buildRectPath(x + position, y, height, height, height / 2);
                    context.fillStyle = accentColor;
                    context.fill();

                }

                if (node.type === 'color' || node.type === 'text' || node.type === 'number') {

                    clipper.add({ x: x, y: y, width: width, height: height });

                    drawText(style, x + parseInt(style.paddingLeft), y + parseInt(style.paddingTop), node.value);

                    clipper.remove();

                }

            }

        }

        /*
        // debug
        context.strokeStyle = '#' + Math.random().toString( 16 ).slice( - 3 );
        context.strokeRect( x - 0.5, y - 0.5, width + 1, height + 1 );
        */

        const isClipping = style.overflow === 'auto' || style.overflow === 'hidden';

        if (isClipping) clipper.add({ x: x, y: y, width: width, height: height });

        for (let i = 0; i < node.childNodes.length; i++) {

            drawElement(node.childNodes[i], style);

        }

        if (isClipping) clipper.remove();

    }

    const offset = element.getBoundingClientRect();

    let canvas;

    if (canvases.has(element)) {

        canvas = canvases.get(element);

    } else {

        canvas = document.createElement('canvas');
        canvas.width = offset.width;
        canvas.height = offset.height;

    }

    const context = canvas.getContext('2d'/*, { alpha: false }*/);

    const clipper = new Clipper(context);

    // console.time( 'drawElement' );

    drawElement(element);

    // console.timeEnd( 'drawElement' );

    return canvas;

}