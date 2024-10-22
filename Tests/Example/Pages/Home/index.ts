import { isDefined, isGoodNumber } from "@juniper-lib/util";
import { CedrusDataAPI } from "@juniper-lib/cedrus";
import { P } from "@juniper-lib/dom";
import { getCurrentUserInfo } from "../../getCurrentUserInfo";

const currentUser = getCurrentUserInfo();

if (currentUser.roles.has("Admin")) {
    document.body.addEventListener("click", async (evt) => {
        if (evt.target instanceof HTMLButtonElement) {
            const userIdStr = evt.target.dataset.userId;
            if (isDefined(userIdStr)) {
                const userId = parseFloat(userIdStr);
                if (isGoodNumber(userId)) {
                    const ds = await CedrusDataAPI.dataSourceTask;
                    await ds.grantUserAccess(userId);
                    const parent = evt.target.parentElement.parentElement;
                    evt.target.parentElement.remove();
                    if (parent.childElementCount === 0) {
                        parent.replaceWith(P("None"));
                    }
                }
            }
        }
    });
}