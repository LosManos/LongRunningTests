using FakeControllers;
using Runner;
using System.Linq;
using Xunit;

namespace CustomerControllerTest
{
    public class CanLoad
    {
        [CaseSetup]
        public void MySetup()
        {
        }

        [CaseTest]
        public void MyTest()
        {
            var sut = new CustomerController();

            //  Act.
            var res = sut.LoadAll();

            //  Assert.
            Assert.Equal(4, res.Count());
        }

        [CaseTearDown]
        public void MyTearDown()
        {
        }
    }
}
