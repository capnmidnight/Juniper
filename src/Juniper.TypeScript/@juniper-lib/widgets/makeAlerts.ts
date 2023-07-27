import { fadeOut } from "@juniper-lib/dom/fadeOut";

export function makeAlerts() {
    const alerts = document.querySelectorAll<HTMLElement>(".alert");
    for(const alert of alerts) {
        if (alert.style.display !== "none") {
            fadeOut(alert, 2, 0.5);
        }
    }
}
