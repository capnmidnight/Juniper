import { ID, Input } from "@juniper-lib/dom";

export function getCurrentUserInfo() {
    const roles = new Set((Input(ID("currentUserRoles"))?.value ?? "").split(","));
    const id = parseFloat(Input(ID("currentUserId")).value ?? "-1");
    return { id, roles }
}