export function printMatrixElements(pre, m) {
    const rows = Math.floor(Math.sqrt(m.length));
    const cols = m.length / rows;
    let s = pre + "\n";
    for (let y = 0; y < rows; ++y) {
        s += "| ";
        for (let x = 0; x < cols; ++x) {
            s += m[y * cols + x].toFixed(2);
            if (x < cols - 1) {
                s += ", ";
            }
        }
        s += " |\n";
    }
    console.log(s);
}
export function printMatrix(pre, m) {
    printMatrixElements(pre, m.elements);
}
//# sourceMappingURL=printMatrix.js.map