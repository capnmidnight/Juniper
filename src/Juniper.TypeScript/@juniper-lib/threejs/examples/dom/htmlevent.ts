export function htmlevent(element: HTMLElement, type: string, x: number, y: number) {

    const mouseEventInit = {
        clientX: (x * element.offsetWidth) + element.offsetLeft,
        clientY: (y * element.offsetHeight) + element.offsetTop,
        view: element.ownerDocument.defaultView
    };

    window.dispatchEvent(new MouseEvent(type, mouseEventInit));

    const rect = element.getBoundingClientRect();

    x = x * rect.width + rect.left;
    y = y * rect.height + rect.top;

    function traverse(node: ChildNode) {

        if (node.nodeType !== Node.TEXT_NODE && node.nodeType !== Node.COMMENT_NODE) {
            const element = node as unknown as HTMLElement;
            const rect = element.getBoundingClientRect();

            if (x > rect.left && x < rect.right && y > rect.top && y < rect.bottom) {

                element.dispatchEvent(new MouseEvent(type, mouseEventInit));

                if (element instanceof HTMLInputElement && element.type === 'range' && (type === 'mousedown' || type === 'click')) {

                    const min = parseFloat(element.min);
                    const max = parseFloat(element.max);
                    const width = rect.width;
                    const offsetX = x - rect.x;
                    const proportion = offsetX / width;
                    element.valueAsNumber = min + (max - min) * proportion;
                    element.dispatchEvent(new InputEvent('input', { bubbles: true }));

                }

            }

            for (let i = 0; i < element.childNodes.length; i++) {

                traverse(element.childNodes[i]);

            }

        }

    }

    traverse(element);

}
