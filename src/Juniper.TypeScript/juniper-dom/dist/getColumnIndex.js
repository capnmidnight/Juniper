export function getColumnIndex(element) {
    let column = element;
    while (column
        && column.tagName !== "TD"
        && column.tagName !== "TH") {
        column = column.parentElement;
    }
    if (!column) {
        return -1;
    }
    const columnRow = column.parentElement;
    let columnIndex = -1;
    for (let c = 0; c < columnRow.children.length; ++c) {
        if (columnRow.children[c] === column) {
            columnIndex = c;
        }
    }
    return columnIndex;
}
