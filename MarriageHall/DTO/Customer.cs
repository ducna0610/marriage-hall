﻿using MarriageHall.DTO.Enums;

namespace MarriageHall.DTO
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public GenderEnum Gender { get; set; }
    }
}
