import { fadeOut } from "@juniper-lib/dom";
export function makeAlerts() {
    const alerts = document.querySelectorAll(".alert");
    for (const alert of alerts) {
        if (alert.style.display !== "none") {
            fadeOut(alert, 2, 0.5);
        }
    }
}
//# sourceMappingURL=makeAlerts.js.map