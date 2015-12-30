using Atlassian.plvs.attributes;

namespace Atlassian.plvs.api.bamboo {
    public class BambooTest {

        public enum TestResult {
            [StringValue("Successful")]
            [ColorValue("#339933")]
            SUCCESSFUL,
            [StringValue("Failed")]
            [ColorValue("#ff0000")]
            FAILED,
            [StringValue("Unknown")]
            [ColorValue("#808080")]
            UNKNOWN
        }

        public string ClassName { get; private set; }
        public string MethodName { get; private set; }
        public TestResult Result { get; private set; }

        public BambooTest(string className, string methodName, TestResult result) {
            ClassName = className;
            MethodName = methodName;
            Result = result;
        }
    }
}
