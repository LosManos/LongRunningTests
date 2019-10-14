using Runner;
using System.Collections.Generic;
using System.Threading;

namespace RunnerTest
{
    public class RunAllInCaseInOrder
    {
        private int counter = 0;
        internal IList<int> SetupsRunCounter = new List<int>();
        internal IList<int> TestsRunCounter = new List<int>();
        internal IList<int> TearDownsRunCounter = new List<int>();

        [CaseSetup]
        public void MySetup()
        {
            SetupsRunCounter.Add(
                Interlocked.Increment(ref counter));
        }

        [CaseTest]
        public void MyTest()
        {
            TestsRunCounter.Add(
                Interlocked.Increment(ref counter));
        }

        [CaseTearDown]
        public void MyTearDown()
        {
            TearDownsRunCounter.Add(
                Interlocked.Increment(ref counter));
        }
    }
}
