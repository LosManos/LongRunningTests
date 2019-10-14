# LongRunningTests

Some system tests require long run times and no deterministic time.

Say the system writes to a database which is then replicated to another which is then read from. The database sync is not run in a deterministic manner so one could only guess and set a max time.
Now if there are many such tests they all have a time out. No figure there is a system failure, or it is just plain slow, and these time outs will add upp.

This framework tries to remedy some of this by running all setups and when they are finished all tests, followed by all teardown.
Say a database has to be setup. That should be done only once disregarding how many tests are run. Some tests require the same test data. These should be added only for relevant tests.

## Future

Common Setup and Teardown, like a  dependency, that has to be run once.

Send data from (common) Setup to the test so the tests know what Ids are used for testing.
