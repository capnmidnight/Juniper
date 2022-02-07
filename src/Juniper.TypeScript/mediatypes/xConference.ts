import { specialize } from "./util";

const xConference = specialize("xconference");

export const anyXConference = xConference("*");
export const XConference_XCooltalk = xConference("x-cooltalk", "ice");
