using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RunnerTest")]
namespace Runner
{
    public class Runner
    {
        private readonly IList<Type> _caseClasses = new List<Type>();
        private readonly Action setupsFinished;
        private readonly Action testsFinished;
        private readonly Action tearDownsFinished;
        private readonly Action dependenciesSetupsFinished;
        private readonly Action dependenciesTearDownsFinished;

        internal readonly IList<object> CaseInstances = new List<object>();
        internal readonly IList<object> DependencyInstances = new List<object>();

        public Runner(
            Action setupsFinished,
            Action testsFinished,
            Action tearDownsFinished, 
            Action dependenciesSetupsFinished, 
            Action dependenciesTearDownsFinished)
        {
            this.setupsFinished = setupsFinished;
            this.testsFinished = testsFinished;
            this.tearDownsFinished = tearDownsFinished;
            this.dependenciesSetupsFinished = dependenciesSetupsFinished;
            this.dependenciesTearDownsFinished = dependenciesTearDownsFinished;
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

            // Create instances of all DependencyClasses.
            foreach (var dependencyClass in GetDependencyTypes(CaseInstances))
            {
                DependencyInstances.Add(Activator.CreateInstance(dependencyClass));
            }

            //  Run all dependencies setups.
            var tasks = ExecuteAllDependenciesSetups(DependencyInstances);
            Task.WaitAll(tasks.ToArray());
            dependenciesSetupsFinished();

            //  Run all Setups.
            tasks = ExecuteAllSetups(CaseInstances);
            Task.WaitAll(tasks.ToArray());
            setupsFinished();

            //  Run all Tests.
            tasks = ExecuteAllTests(CaseInstances);
            Task.WaitAll(tasks.ToArray());
            testsFinished();

            //  Run all TearDowns.
            tasks = ExecuteAllTearDowns(CaseInstances);
            Task.WaitAll(tasks.ToArray());
            tearDownsFinished();

            //  Run all dependencies setups.
            tasks = ExecuteAllDependenciesTearDowns(DependencyInstances);
            Task.WaitAll(tasks.ToArray());
            dependenciesTearDownsFinished();
        }

        /// <summary>This method returns a list of all the Dependencies for all Cases.
        /// As per present implementation the dependencies may be out of order.
        /// </summary>
        /// <param name="caseInstances"></param>
        /// <returns></returns>
        private static IEnumerable<Task> ExecuteAllDependenciesSetups(IEnumerable<object> dependencyInstances)
        {
            var tasks = new List<Task>();
            foreach (var instance in dependencyInstances)
            {
                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<DependencySetupAttribute>(instance)
                        .Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private static IEnumerable<Task> ExecuteAllDependenciesTearDowns(IEnumerable<object> dependencyInstances)
        {
            var tasks = new List<Task>();
            foreach (var instance in dependencyInstances)
            {
                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<DependencyTearDownAttribute>(instance)
                        .Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private static IEnumerable<Task> ExecuteAllTearDowns(IEnumerable<object> caseInstances)
        {
            var tasks = new List<Task>();
            foreach (var instance in caseInstances)
            {
                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<CaseTearDownAttribute>(instance)
                        .Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private static IEnumerable<Task> ExecuteAllTests(IEnumerable<object> caseInstances)
        {
            var tasks = new List<Task>();
            foreach (var instance in caseInstances)
            {
                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<CaseTestAttribute>(instance)
                        .Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private static IEnumerable<Task> ExecuteAllSetups(IEnumerable<object> caseInstances)
        {
            var tasks = new List<Task>();
            foreach (var instance in caseInstances)
            {
                var startedTask = StartTask(() =>
                {
                    GetMethodByAttribute<CaseSetupAttribute>(instance)
                        .Invoke(instance, null);
                });
                tasks.Add(startedTask);
            }
            return tasks;
        }

        private static IEnumerable<Type> GetDependencyTypes(IEnumerable<object> caseInstances)
        {
            var ret = new List<Type>();
            foreach (var instance in caseInstances)
            {
                var valueInProperty = GetValueInPropertyByAttribute<CaseDependenciesAttribute, Type[]>(instance);
                if (valueInProperty != default)
                {
                    ret.AddRange(valueInProperty);
                }
            }
            return ret.Distinct();
        }

        private static MethodInfo GetMethodByAttribute<TAttribute>(object instance) where TAttribute : Attribute
        {
            return instance.GetType()
                  .GetMethods()
                  .Where(m => m.GetCustomAttributes(typeof(TAttribute), false).Length >= 1)
                  .Single();
        }

        private static PropertyInfo GetPropertyByAttribute<TAttribute>(object instance) where TAttribute : Attribute
        {
            return instance.GetType()
                  .GetProperties()
                  .Where(m => m.GetCustomAttributes(typeof(TAttribute), false).Length >= 1)
                  .SingleOrDefault();
        }

        /// <summary>This method returns the value of a property with a certain Attribute.
        /// If there is no property with this Attribute found the default of TPropertyType is returned.
        /// </summary>
        /// <typeparam name="TAttribute">The Attribute class.</typeparam>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="instance">The instance of the class where the property resides.</param>
        /// <returns></returns>
        private static TPropertyType GetValueInPropertyByAttribute<TAttribute, TPropertyType>(object instance) where TAttribute : Attribute
        {
            var property = GetPropertyByAttribute<TAttribute>(instance);
            var propertyInfo = GetPropertyByAttribute<CaseDependenciesAttribute>(instance);
            if (propertyInfo != null)
            {
                return (TPropertyType)propertyInfo.GetValue(instance, null);
            }
            return default;
        }

        private static Task StartTask(Action action)
        {
            var task = new Task(() =>
            {
                action();
            });
            task.Start();
            return task;
        }
    }
}
