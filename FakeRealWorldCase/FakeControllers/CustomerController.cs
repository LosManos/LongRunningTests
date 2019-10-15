using System.Collections;
using System.Collections.Generic;

namespace FakeControllers
{
    public class CustomerController
    {
        public IEnumerable<Customer> LoadAll()
        {
            return new[]
            {
                new Customer{Id=1, Name="Alfa"},
                new Customer{Id=2, Name="Bravo"},
                new Customer{Id=3, Name="Charlie"},
                new Customer{Id=4, Name="Delta"},
            };
        }
    }
}
