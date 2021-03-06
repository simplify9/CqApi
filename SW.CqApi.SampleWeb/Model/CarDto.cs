﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Model
{
    public class CarOwner {
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public int? Age { get; set; }
        public Dictionary<string, int> TestDictInt { get; set; }
        public List<CarOwner> Owners { get; set; }
    }

    public class GenericTest<T1, T2>
    {
        public Type GetT()
        {
            return typeof(T1);
        }
        public T1 inner { get; set; }
        public List<T2> inner2 { get; set; }
    }

    public class CarDto
    {
        [Required]
        public string PlateNumber { get; set; }
        public string Owner { get; set; } = "RandomOwner";
        public string Brand { get; set; }
        public int[] OwnerIds { get; set; }
        [IgnoreMember]
        public List<CarOwner> PreviousOwners { get; set; }
        public GenericTest<int, string> GenericTest { get; set; }
        [IgnoreMember]
        public CarOwner CarOwner { get; set; }
        public Dictionary<string, string> TestDict { get; set; }
    }
}
