using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RunnerTest
{
    public class RunAllInCaseInOrderTest
    {
        Runner.Runner sut;
        IList<int>  _setupsRunCounter = new List<int>();
        IList<int> _testsRunCounter = new List<int>();
        IList<int>  _tearDownsRunCounter = new List<int>();

        [Fact]
        public void RunAllInCaseInOrder()
        {
            sut = new Runner.Runner(
                OnSetupsFinished,
                OnTestsFinished,
                OnTearDownsFinished,
                () => { },
                () => { });

            sut.AddCase(
                typeof(RunAllInCaseInOrder));

            //  Act.
            sut.Start();

            //  Assert.
            new[] { _setupsRunCounter.Count(), _testsRunCounter.Count(), _tearDownsRunCounter.Count() }.Should()
                .AllBeEquivalentTo(1,
                because: "Each of Setup, Test and TearDown should only have been run once.");
            _setupsRunCounter.Max().Should()
                .BeLessThan(_testsRunCounter.Min(),
                because: "Setup should be run before Test.");
            _testsRunCounter.Max().Should()
                .BeLessThan(_tearDownsRunCounter.Min(),
                because: "Test should be run before TearDown.");
        }

        private void OnSetupsFinished()
        {
            var instance = (RunAllInCaseInOrder)sut.CaseInstances.Single();
            _setupsRunCounter = instance.SetupsRunCounter;
        }

        private void OnTestsFinished()
        {
            var instance = (RunAllInCaseInOrder)sut.CaseInstances.Single();
            _testsRunCounter = instance.TestsRunCounter;
        }

        private void OnTearDownsFinished()
        {
            var instance = (RunAllInCaseInOrder)sut.CaseInstances.Single();
            _tearDownsRunCounter = instance.TearDownsRunCounter;
        }
    }
}
