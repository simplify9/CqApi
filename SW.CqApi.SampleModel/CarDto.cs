using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SW.CqApi.SampleModel
{
    public class CarOwner {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    public class CarDto
    {
        [Required]
        public string PlateNumber { get; set; }
        public string Owner { get; set; } = "RandomOwner";
        public string Brand { get; set; }
        public int[] OwnerIds { get; set; }
        public List<string> PreviousOwners { get; set; }
        public CarOwner CarOwner { get; set; }
    }
}
