using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatAllocationApp
{
    /// <summary>
    /// Represents a passenger with personal information and seat asignment details.
    /// Stores special needs, group membership, weight, and assigned seat.
    /// </summary>
    internal class Passenger
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<string> Group { get; set; } = new List<string>();
        public int Weight { get; set; }
        public char Gender { get; set; }
        public string Special { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;

        public bool IsAssigned { get; set; }
    }
}
