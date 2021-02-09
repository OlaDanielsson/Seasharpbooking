﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seasharpbooking.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; } //hur fan skriver man denna kopplingen då den ska hämtas från annan API?
    }
}