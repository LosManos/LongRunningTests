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

            // Execute all Setups.
            var tasks = new List<Task>();
            foreach (var instance in CaseInstances)
            {

                var startedTask = StartTask(() =>
               {
                   GetMethodByAttribute(instance, typeof(CaseSetupAttribute)).Invoke(instance, null);
               });
                tasks.Add(startedTask);
            }
            Task.WaitAll(tasks.ToArray());
            setupsFinished();

            // Execute all Tests.
            tasks = new List<Task>();
            foreach (var instance in CaseInstances)
            {

                var startedTask = StartTask(() =>
               {
                   GetMethodByAttribute(instance, typeof(CaseTestAttribute)).Invoke(instance, null);
               });
                tasks.Add(startedTask);
            }
            Task.WaitAll(tasks.ToArray());
            testsFinished();

            // Execute all TearDowns.
            tasks = new List<Task>();
            foreach (var instance in CaseInstances)
            {

                var startedTask = StartTask(() =>
               {
                   GetMethodByAttribute(instance, typeof(CaseTearDownAttribute)).Invoke(instance, null);
               });
                tasks.Add(startedTask);
            }
            Task.WaitAll(tasks.ToArray());
            tearDownsFinished();
        }

        private static System.Reflection.MethodInfo GetMethodByAttribute(
            object instance, 
            Type attributeType)
        {
            return instance.GetType()
                  .GetMethods()
                  .Where(m => m.GetCustomAttributes(attributeType, false).Length >= 1)
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
