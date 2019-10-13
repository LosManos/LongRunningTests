using FluentAssertions;
using System.Linq;
using Xunit;

namespace RunnerTest
{
    public partial class RunnerTests
    {
        Runner.Runner runner;
        bool _setupsHaveBeenRun = false;
        bool _testsHaveBeenRun = false;
        bool _tearDownsHaveBeenRun = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void OnSetupsFinished()
        {
            var instance = (RunAllInCaseCase)runner.CaseInstances.Single();
            _setupsHaveBeenRun = instance.SetupHasBeenRun;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void OnTestsFinished()
        {
            var instance = (RunAllInCaseCase)runner.CaseInstances.Single();
            _testsHaveBeenRun = instance.TestHasBeenRun;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void OnTearDownsFinished()
        {
            var instance = (RunAllInCaseCase)runner.CaseInstances.Single();
            _tearDownsHaveBeenRun = instance.TearDownHasBeenRun;
        }

        [Fact]
        public void RunAllInCase()
        {
            runner = new Runner.Runner(
                OnSetupsFinished, 
                OnTestsFinished, 
                OnTearDownsFinished);

            runner.AddCase(
                typeof(RunAllInCaseCase));
            runner.Start();

            //  Assert.
            new[] { _setupsHaveBeenRun, _testsHaveBeenRun, _tearDownsHaveBeenRun }.Should()
                .AllBeEquivalentTo(true);
        }
    }
}
