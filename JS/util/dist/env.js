import { identity } from "./filters";
const loc = /* @__PURE__ */ (() => new URL(document.location.href))();
export const DEVELOPMENT = /* @__PURE__ */ (() => /^https?:\/\/(?:localhost|127\.0\.0\.1)/.test(loc.href))();
export const STAGING = /* @__PURE__ */ (() => !DEVELOPMENT
    && (/\/Test( |%20)Bed\//.test(loc.href)
        || /\/development\//.test(loc.href)))();
export const PRODUCTION = /* @__PURE__ */ (() => !DEVELOPMENT
    && !STAGING)();
export const DEBUG = /* @__PURE__ */ (() => DEVELOPMENT
    && !loc.searchParams.has("RELEASE")
    || loc.searchParams.has("DEBUG"))();
export const TEST = /* @__PURE__ */ (() => loc.searchParams.has("TEST"))();
export const RELEASE = /* @__PURE__ */ (() => !DEBUG
    || loc.searchParams.has("RELEASE"))();
export const ENV = /* @__PURE__ */ (() => [
    DEVELOPMENT && "development",
    STAGING && "staging",
    PRODUCTION && "production",
    DEBUG && "debug",
    TEST && "test",
    RELEASE && "release"
].filter(identity)
    .join("-"))();
//# sourceMappingURL=env.js.map