using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SW.CqApi.SampleModel
{
    public class CarOwner {
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public int? Age { get; set; }
    }

    public class GenericTest<T>
    {
        public Type GetT()
        {
            return typeof(T);
        }
        public T inner { get; set; }
    }

    public class CarDto
    {
        [Required]
        public string PlateNumber { get; set; }
        public string Owner { get; set; } = "RandomOwner";
        public string Brand { get; set; }
        public int[] OwnerIds { get; set; }
        public List<CarOwner> PreviousOwners { get; set; }
        public GenericTest<int> genericTest { get; set; }
        public CarOwner CarOwner { get; set; }
        public IDictionary<string, IEnumerable<string>> testDictionary { get; set; }
    }
}
