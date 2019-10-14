using FluentAssertions;
using System.Linq;
using Xunit;

namespace RunnerTest
{
    public class RunAllInCaseTest
    {
        Runner.Runner sut;
        bool _setupsHaveBeenRun = false;
        bool _testsHaveBeenRun = false;
        bool _tearDownsHaveBeenRun = false;

        [Fact]
        public void RunAllInCase()
        {
            sut = new Runner.Runner(
                OnSetupsFinished,
                OnTestsFinished,
                OnTearDownsFinished,
                () => { },
                () => { });

            sut.AddCase(
                typeof(RunAllInCaseCase));

            //  Act.
            sut.Start();

            //  Assert.
            new[] { _setupsHaveBeenRun, _testsHaveBeenRun, _tearDownsHaveBeenRun }.Should()
                .AllBeEquivalentTo(true);
        }

        private void OnSetupsFinished()
        {
            var instance = (RunAllInCaseCase)sut.CaseInstances.Single();
            _setupsHaveBeenRun = instance.SetupHasBeenRun;
        }

        private void OnTestsFinished()
        {
            var instance = (RunAllInCaseCase)sut.CaseInstances.Single();
            _testsHaveBeenRun = instance.TestHasBeenRun;
        }

        private void OnTearDownsFinished()
        {
            var instance = (RunAllInCaseCase)sut.CaseInstances.Single();
            _tearDownsHaveBeenRun = instance.TearDownHasBeenRun;
        }
    }
}
