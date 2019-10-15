using Xunit;

namespace CustomerControllerTest
{
    public class CustomerControllerTest
    {
        [Fact]
        public void Test1()
        {
            var runner = new Runner.Runner(
                () => { },
                () => { },
                () => { },
                () => { },
                () => { });

            runner.AddCase(typeof(CanLoad));

            runner.Start();
        }
    }
}
