export function getColumnIndex(element) {
    let column = element;
    while (column
        && column.tagName !== "TD"
        && column.tagName !== "TH") {
        column = column.parentElement;
    }
    if (column) {
        const columnRow = column.parentElement;
        let columnIndex = 0;
        for (const child of columnRow.children) {
            if (child === column) {
                return columnIndex;
            }
            if (child instanceof HTMLTableCellElement) {
                columnIndex += child.colSpan;
            }
        }
    }
    return -1;
}
//# sourceMappingURL=getColumnIndex.js.map