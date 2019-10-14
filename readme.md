# LongRunningTests

Some system tests require long run times and no deterministic time.

Say the system writes to a database which is then replicated to another which is then read from. The database sync is not run in a deterministic manner so one could only guess and set a max time.
Now if there are many such tests they all have a time out. No figure there is a system failure, or it is just plain slow, and these time outs will add upp.

This framework tries to remedy some of this by running all setups and when they are finished all tests, followed by all teardown.
Say a database has to be setup. That should be done only once disregarding how many tests are run. Some tests require the same test data. These should be added only for relevant tests.

## More explanation

Each test is a Case with Setup, Test and Teardown, run in this order.
But no Test is run before all Setups in all Cases are run. In the same way all TearDowns are run after all Tests.

Since several Cases can have the same Dependency, like records in a database or a fake listening service running, each Case can also have a dependency on a Dependency.
If several Cases have dependency on the same Dependency only one such Dependency is used.
This Dependency in turn has DependencySetup and DependencyTearDown. All DependencySetups are run before any Case Setup is run. All DependencyTearDowns are run after all Case Teardowns are run.

### Life cycle
All Dependency Setups are run.
All Case Setups are run.
All Case Tests are run.
All Case TearDowns are run.
All Dependency TearDowns are run.

## Future

Send data from (common) Setup to the test so the tests know what Ids are used for testing. Say the Dependency Setup creates records then the ids should be passed on to the Case for use.

Each test case can contain serveral tests.

Make sure it works with async test methods.