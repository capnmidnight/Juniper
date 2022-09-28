export function htmlevent(element, event, x, y) {

    const mouseEventInit = {
        clientX: (x * element.offsetWidth) + element.offsetLeft,
        clientY: (y * element.offsetHeight) + element.offsetTop,
        view: element.ownerDocument.defaultView
    };

    window.dispatchEvent(new MouseEvent(event, mouseEventInit));

    const rect = element.getBoundingClientRect();

    x = x * rect.width + rect.left;
    y = y * rect.height + rect.top;

    function traverse(element) {

        if (element.nodeType !== Node.TEXT_NODE && element.nodeType !== Node.COMMENT_NODE) {

            const rect = element.getBoundingClientRect();

            if (x > rect.left && x < rect.right && y > rect.top && y < rect.bottom) {

                element.dispatchEvent(new MouseEvent(event, mouseEventInit));

                if (element instanceof HTMLInputElement && element.type === 'range' && (event === 'mousedown' || event === 'click')) {

                    const [min, max] = ['min', 'max'].map(property => parseFloat(element[property]));

                    const width = rect.width;
                    const offsetX = x - rect.x;
                    const proportion = offsetX / width;
                    element.value = min + (max - min) * proportion;
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
