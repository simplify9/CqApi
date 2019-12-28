using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SW.CqApi.SampleModel
{
    public class CarDto
    {
        [Required]
        public string PlateNumber { get; set; }
        public string Owner { get; set; }
        public string Brand { get; set; }
    }
}
