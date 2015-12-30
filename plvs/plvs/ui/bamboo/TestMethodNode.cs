using System.Drawing;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.bamboo {
    internal class TestMethodNode {

        public string Name { get; private set; }
        public Image Icon { get; private set; }
        public string Result { get; private set; }
        public BambooTest Test { get; private set; }

        public TestMethodNode(BambooTest test) {
            Name = test.MethodName;
            Icon = test.Result == BambooTest.TestResult.SUCCESSFUL
                       ? Resources.icn_plan_passed
                       : test.Result == BambooTest.TestResult.FAILED
                             ? Resources.icn_plan_failed
                             : Resources.icn_plan_disabled;
            Result = test.Result.GetStringValue();
            Test = test;
        }
    }
}
