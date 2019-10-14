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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void OnSetupsFinished()
        {
            var instance = (RunAllInCaseCase)sut.CaseInstances.Single();
            _setupsHaveBeenRun = instance.SetupHasBeenRun;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void OnTestsFinished()
        {
            var instance = (RunAllInCaseCase)sut.CaseInstances.Single();
            _testsHaveBeenRun = instance.TestHasBeenRun;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1013:Public method should be marked as test", Justification = "<Pending>")]
        public void OnTearDownsFinished()
        {
            var instance = (RunAllInCaseCase)sut.CaseInstances.Single();
            _tearDownsHaveBeenRun = instance.TearDownHasBeenRun;
        }

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
    }
}
