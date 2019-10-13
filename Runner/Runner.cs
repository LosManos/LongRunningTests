using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RunnerTest")]
namespace Runner
{
    public class Runner
    {
        private readonly Suite _suite = new Suite();
        private readonly List<Type> _caseClasses = new List<Type>();
        internal readonly List<object> CaseInstances = new List<object>();
        private readonly Action setupsFinished;
        private readonly Action testsFinished;
        private readonly Action tearDownsFinished;

        public Runner(
            Action setupsFinished, 
            Action testsFinished, 
            Action tearDownsFinished)
        {
            this.setupsFinished = setupsFinished;
            this.testsFinished = testsFinished;
            this.tearDownsFinished = tearDownsFinished;
        }

        public void AddCase(Type caseClass)
        {
            _caseClasses.Add(caseClass);
        }

        public void Start()
        {
            // Create instances of all CaseClasses.
            foreach (var caseClass in _caseClasses)
            {
                CaseInstances.Add(Activator.CreateInstance(caseClass));
            }

            var tasks = ExecuteAllSetups();
            Task.WaitAll(tasks.ToArray());
            setupsFinished();

            tasks = ExecuteAllTests();
            Task.WaitAll(tasks.ToArray());
            testsFinished();

            tasks = ExecuteAllTearDowns();
            Task.WaitAll(tasks.ToArray());
            tearDownsFinished();
        }

        private IEnumerable<Task> ExecuteAllTearDowns()
        {
            var tasks = new List<Task>();
            foreach (var instance in CaseInstances)
            {

                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<CaseTearDownAttribute>(instance).Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private IEnumerable<Task> ExecuteAllTests()
        {
            var tasks = new List<Task>();
            foreach (var instance in CaseInstances)
            {

                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<CaseTestAttribute>(instance).Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private IEnumerable<Task> ExecuteAllSetups()
        {
            var tasks = new List<Task>();
            foreach (var instance in CaseInstances)
            {

                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<CaseSetupAttribute>(instance).Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private static System.Reflection.MethodInfo GetMethodByAttribute<TAttribute>(
            object instance) where TAttribute:Attribute
        {
            return instance.GetType()
                  .GetMethods()
                  .Where(m => m.GetCustomAttributes(typeof(TAttribute), false).Length >= 1)
                  .Single();
        }

        private Task StartSetup(Case @case)
        {
            return StartTask(@case.Setup);
        }

        private Task StartTask(Action action)
        {
            var task = new Task(() =>
            {
                action();
            });
            task.Start();
            return task;
        }
    }

    public class Suite
    {
        public IList<Case> Cases;

        public IEnumerable<Action> Setups {
            get {
                foreach( var @case in Cases)
                {
                    yield return @case.Setup;
                }
            }
        }
        
        public Suite()
        {
            Cases = new List<Case>();
        }
    }

    public class Case
    {
        public readonly Action Setup;
        public readonly Action Test;
        public readonly Action TearDown;
        public Case(Action setup, Action test, Action tearDown)
        {
            Setup = setup;
            Test = test;
            TearDown = tearDown;
        }
    }
}
