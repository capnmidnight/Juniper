/**
 * An enumeration of values for the states of test runs.
 **/
export var TestStates;
(function (TestStates) {
    TestStates[TestStates["found"] = 0] = "found";
    TestStates[TestStates["started"] = 1] = "started";
    TestStates[TestStates["succeeded"] = 2] = "succeeded";
    TestStates[TestStates["failed"] = 4] = "failed";
    TestStates[TestStates["completed"] = 8] = "completed";
})(TestStates || (TestStates = {}));
//# sourceMappingURL=TestStates.js.map