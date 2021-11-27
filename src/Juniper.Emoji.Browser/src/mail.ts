import { EmojiGroup } from "./EmojiGroup";
import { outboxTray, inboxTray, packageBox, eMail, incomingEnvelope, envelopeWithArrow, closedMailboxWithLoweredFlag, closedMailboxWithRaisedFlag, openMailboxWithRaisedFlag, openMailboxWithLoweredFlag, postbox, postalHorn } from ".";


export const mail = /*@__PURE__*/ new EmojiGroup(
    "Mail", "Mail",
    outboxTray,
    inboxTray,
    packageBox,
    eMail,
    incomingEnvelope,
    envelopeWithArrow,
    closedMailboxWithLoweredFlag,
    closedMailboxWithRaisedFlag,
    openMailboxWithRaisedFlag,
    openMailboxWithLoweredFlag,
    postbox,
    postalHorn);
