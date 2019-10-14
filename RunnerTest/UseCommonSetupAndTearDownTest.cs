using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RunnerTest
{
    /// <summary>This test validates that if several Cases has dependency on the same Setup it is run once.
    /// </summary>
    public class UseCommonSetupAndTearDownTest
    {
        Runner.Runner runner;
        readonly List<int> _testSetupsCounters = new List<int>();
        readonly List<int> _testTearDownsCounters = new List<int>();
        IEnumerable<int> _dependenciesSetupsCounters = new List<int>();
        IEnumerable<int> _dependenciesTearDownsCounters = new List<int>();

        [Fact]
        public void UseCommonSetupAndTearDown()
        {
            runner = new Runner.Runner(
                OnSetupsFinished,
                () => { },
                OnTearDownsFinished,
                OnDependenciesSetupsFinished,
                OnDependenciesTearDownsFinished);

            runner.AddCase(
                typeof(UseCommonSetupAndTearDownOne));
            runner.AddCase(
                typeof(UseCommonSetupAndTearDownTwo));

            //  Act.
            runner.Start();

            //  Assert.
            new[] { _dependenciesSetupsCounters.Count(), _dependenciesTearDownsCounters.Count() }.Should()
                .AllBeEquivalentTo(1,
                because: "There should be just one call to each of the dependencies setups and teardowns.");
            _dependenciesSetupsCounters.Max().Should()
                .BeLessThan(_dependenciesTearDownsCounters.Min(),
                because: "All dependency Setups should be run before teh dependency Teardowns.");

            _dependenciesSetupsCounters.Max().Should()
                .BeLessThan(_testSetupsCounters.Min(),
                because: "All dependency setups must be run before all test setups.");
            _testTearDownsCounters.Max().Should()
                .BeLessThan(_dependenciesTearDownsCounters.Min(),
                because: "All tests must have been teared down before all dependency teardowns.");
        }

        private void OnSetupsFinished()
        {
            foreach( var instance in runner.CaseInstances)
            {
                switch (instance)
                {
                    case UseCommonSetupAndTearDownOne inst:
                        _testSetupsCounters.AddRange(inst.SetupCounters);
                        break;
                    case UseCommonSetupAndTearDownTwo inst:
                        _testSetupsCounters.AddRange(inst.SetupCounters);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("NotAKnownParameter", "There was a Setup in a class not anticipated.");
                }
            }
        }

        private void OnTearDownsFinished()
        {
            foreach (var instance in runner.CaseInstances)
            {
                switch (instance)
                {
                    case UseCommonSetupAndTearDownOne inst:
                        _testTearDownsCounters.AddRange(inst.TearDownCounters);
                        break;
                    case UseCommonSetupAndTearDownTwo inst:
                        _testTearDownsCounters.AddRange(inst.TearDownCounters);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("NotAKnownParameter", "There was a TearDown in a class not anticipated.");
                }
            }
        }

        private void OnDependenciesSetupsFinished()
        {
            var instance = (CommonDepency)runner.DependencyInstances.Single();
            _dependenciesSetupsCounters = instance.SetupCounters;
        }

        private void OnDependenciesTearDownsFinished()
        {
            var instance = (CommonDepency)runner.DependencyInstances.Single();
            _dependenciesTearDownsCounters = instance.TearDownCounter;
        }
    }
}
