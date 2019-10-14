using Runner;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RunnerTest
{
    internal class Common
    {
        internal static int Counter = 0;
    }

    public class CommonDepency
    {
        internal IList<int> SetupCounters = new List<int>();
        internal IList<int> TearDownCounter = new List<int>();

        [DependencySetup]
        public void Setup()
        {
            SetupCounters.Add(
                Interlocked.Increment(ref Common.Counter)
            );
        }

        [DependencyTearDown]
        public void Teardown()
        {
            TearDownCounter.Add(
                Interlocked.Increment(ref Common.Counter)
            );
        }
    }

    public class UseCommonSetupAndTearDownOne
    {
        internal IList<int> SetupCounters = new List<int>();
        internal IList<int> TearDownCounters = new List<int>();

        [CaseDependencies]
        public Type[] Dependencies => new[]{
            typeof(CommonDepency)
        };

        [CaseSetup]
        public void MySetup()
        {
            SetupCounters.Add(
                Interlocked.Increment(ref Common.Counter)
            );
        }

        [CaseTest]
        public void MyTest()
        {
        }

        [CaseTearDown]
        public void MyTearDown()
        {
            TearDownCounters.Add(
                Interlocked.Increment(ref Common.Counter)
            );
        }
    }

    public class UseCommonSetupAndTearDownTwo
    {
        internal IList<int> SetupCounters = new List<int>();
        internal IList<int> TearDownCounters = new List<int>();

        [CaseDependencies]
        public Type[] Dependencies => new[]{
            typeof(CommonDepency)
        };

        [CaseSetup]
        public void MySetup()
        {
            SetupCounters.Add(
                Interlocked.Increment(ref Common.Counter)
            );
        }

        [CaseTest]
        public void MyTest()
        {
        }

        [CaseTearDown]
        public void MyTearDown()
        {
            TearDownCounters.Add(
                Interlocked.Increment(ref Common.Counter)
            );
        }
    }
}
