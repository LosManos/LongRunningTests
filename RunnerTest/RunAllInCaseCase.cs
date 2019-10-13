using Runner;

namespace RunnerTest
{
    public partial class RunnerTests
    {
        public class RunAllInCaseCase
        {
            internal bool SetupHasBeenRun = false;
            internal bool TestHasBeenRun = false;
            internal bool TearDownHasBeenRun = false;

            [CaseSetup]
            public void MySetup()
            {
                SetupHasBeenRun = true;
            }

            [CaseTest]
            public void MyTest()
            {
                TestHasBeenRun = true;
            }

            [CaseTearDown]
            public void MyTearDown()
            {
                TearDownHasBeenRun = true;
            }
        }
    }
}
