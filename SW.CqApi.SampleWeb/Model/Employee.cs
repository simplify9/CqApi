using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SW.CqApi.SampleWeb.Model
{
    public class Employee
    {
        public Employee()
        {
            //Salary = new Money();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public bool Married { get; set; }
        public Money Salary { get; set; }
        public int EmploymentStatus { get; set; }
        public string Country { get; set; }
        public string Photo { get; set; }
        public ICollection<Leave> Leaves { get; set; }

        public static List<Employee> Sample = new List<Employee>
            {
            new Employee()
            {
                Id=1,
                FirstName="Samer",
                LastName="Awajan",
                Gender="M",
                Salary = new Money{Amount=100, Currency="USD" },
                Leaves = new List<Leave>()
                {
                    new Leave {Days=100, Reason="sick" },
                    new Leave {Days=30, Reason="marriage" }
                }
            },
            new Employee() {Id=2, FirstName="Yaser", LastName="Awajan", Gender="M" },
            new Employee() {Id=3, FirstName="Osama", LastName="Awajan", Gender="M" },
            new Employee() {Id=4, FirstName="Ahmad", LastName="Awajan", Gender="M" },

    };

    }



    public class Money
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
    }

    public class Leave
    {
        public int Days { get; set; }
        public string Reason { get; set; }
    }

}
