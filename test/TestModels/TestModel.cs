using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TestModels
{

    public class Person : IInterface
    {
        public string Name { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }
        public Address Address { get; set; }

        public void CreateOrder(Order order, Address billingAddress)
        {
            //TODO: implement
        }
    }
 
    public class Order
    {
        public int Qty { get; set; }
    }
 
    public class Address
    {
        public int StreetNr { get; set; }
        public string Street { get; set; }
    }

    [CustomTest]
    public class TestModel : Base
    {
        public int Id { get; set; }
        public string President { get; protected set; }
        public string Spy { get; private set; }

        [PickMe]
        public string MyString { get; set; }

        public NestedModel Nested { get; set; }
        public DateTime ClosedAt { get; set; }
    }
 
    public class NestedModel
    {
        public string Deep { get; set; }
    }

    public class Base
    {
        public DateTime CreatedAt { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class CustomTestAttribute : Attribute
    {
    }
}
