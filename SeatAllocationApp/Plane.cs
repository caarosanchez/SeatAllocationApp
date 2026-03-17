using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatAllocationApp
{
    /// <summary>
    /// Represents the airplane and its seating zones.
    /// Tracks zone weights for balancing.
    /// </summary>
    internal class Plane
    {
        public int FrontLeftWeight { get; set; }
        public int FrontRightWeight { get; set; }
        public int AftLeftWeight { get; set; }
        public int AftRightWeight { get; set; }

        public int TotalWeight => FrontLeftWeight + FrontRightWeight + AftLeftWeight + AftRightWeight;
        public int TargetZoneWeight => TotalWeight / 4;

        public List<Seat> Seats { get; set; } = new List<Seat>();

        // Initializes all seats according to predetermined airplane configuration
        public void InitializeSeats()
        {
            char[] seatLetters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K' };

            for (int row = 1; row <= 10; row++)
            {
                foreach (var letter in seatLetters)
                {
                    Seats.Add(new Seat
                    {
                        SeatNumber = $"{row}{letter}",
                        IsOccupied = false,
                        IsAisle = letter == 'C' || letter == 'D' || letter == 'G' || letter == 'H',
                        Zone = SetZone(row, letter)
                    });
                }
            }
        }

        // Returns the zone for each seat based on airplane configuration
        public string SetZone(int row, char letter)
        {
            if (row <= 5 && (letter == 'A' || letter == 'B' || letter == 'C' || letter == 'D' || letter == 'E'))
            { return "FrontLeft"; }
            if (row <= 5 && (letter == 'F' || letter == 'G' || letter == 'H' || letter == 'J' || letter == 'K'))
            { return "FrontRight"; }
            if (row > 5 && (letter == 'A' || letter == 'B' || letter == 'C' || letter == 'D' || letter == 'E'))
            { return "AftLeft"; }
            if (row > 5 && (letter == 'F' || letter == 'G' || letter == 'H' || letter == 'J' || letter == 'K'))
            { return "AftRight"; }
            return "AftRight"; // Default case, should not reach here
        }

        // Updates the wight of a zone after assigning a passenger
        public void UpdateZoneWeight(string zone, int paxWeight)
        {
            switch (zone)
            {
                case "FrontLeft":
                    FrontLeftWeight += paxWeight;
                    break;
                case "FrontRight":
                    FrontRightWeight += paxWeight;
                    break;
                case "AftLeft":
                    AftLeftWeight += paxWeight;
                    break;
                case "AftRight":
                    AftRightWeight += paxWeight;
                    break;
                default:
                    Console.WriteLine($"Could not add weight {paxWeight} for seat in zone '{zone}'. Default action: ignored.");
                    break;
            }
        }

        // Returns the zone with the lowest current weight
        public string GetLightestZone()
        {
            var zoneWeights = new Dictionary<string, int>
            {
                { "FrontLeft", FrontLeftWeight },
                { "FrontRight", FrontRightWeight },
                { "AftLeft", AftLeftWeight },
                { "AftRight", AftRightWeight }
            };
            return zoneWeights.OrderBy(z => z.Value).First().Key;
        }

        // Checks if all zones are balanced within tolerance
        public bool IsBalanced(int tolerancePercent = 5)
        {
            int tolerance = TargetZoneWeight * tolerancePercent / 100;

            var zoneWeights = new Dictionary<string, int>
            {
                {"FrontLeft", FrontLeftWeight },
                {"FrontRight", FrontRightWeight },
                {"AftLeft", AftLeftWeight },
                {"AftRight", AftRightWeight }
            };

            foreach (var weight in zoneWeights.Values)
            {
                if (weight < TargetZoneWeight - tolerance || weight > TargetZoneWeight + tolerance)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
