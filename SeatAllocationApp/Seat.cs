using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatAllocationApp
{
    /// <summary>
    /// Represents a seat in the airplane.
    /// Tracks occupancy status, seat number, type of seat, and the zone it belongs to.
    /// </summary>
    internal class Seat
    {
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public bool IsAisle { get; set; }
        public Passenger? Occupant { get; set; }
        public string Zone { get; set; } = string.Empty;
    }
}
