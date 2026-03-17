using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SeatAllocationApp
{
    /// <summary>
    /// Handles seat assignment logic for all passengers.
    /// Considers special needs, groups, and general passengers while balancing zones.
    /// </summary>
    internal class SeatAllocator
    {
        // Main method to assign all seats
        public void AssignSeats(List<Passenger> passengers, Plane plane)
        {
            // validate IDs
            if (passengers.Any(p => string.IsNullOrWhiteSpace(p.Id)))
               throw new InvalidDataException("One or more passengers missing Id.");

            var dup = passengers.GroupBy(p => p.Id)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .FirstOrDefault();
            if (dup != null)
                throw new InvalidDataException($"Duplicate passenger Id found: {dup}");

            var groups = BuildGroups(passengers); // Build groups from current passenger list

            // Assign seats to passengers
            AssignWCHR(passengers, plane);
            AssignUMNR(passengers, plane);
            AssignGroups(groups, plane);
            AssignFMuslim(passengers, plane);
            AssignGeneral(passengers, plane);

            // Check if any pax has not been assigned a seat
            var unassigned = passengers.Where(p => !p.IsAssigned).ToList();
            if (unassigned.Any())
            {
                Console.WriteLine("Warning: some passengers could not be assigned seats.");
            }
            else
            {
                Console.WriteLine("All passengers successfully assigned.");
            }

            // Check if plane is balanced after seat allocation
            if (plane.IsBalanced())
            {
                Console.WriteLine("Plane zones are balanced within tolerance.");
            }
            else
            {
                Console.WriteLine("Warning: some zones are outside the target weight range.");
            }
        }

        // Allocates a single passenger to a seat
        public void AllocateSeat(Seat seat, Passenger pax, Plane plane)
        {
            if (seat == null || pax == null)
            {
                throw new ArgumentNullException("Seat or Passenger is null. Unable to assign seat.");
            }

            seat.Occupant = pax;
            seat.IsOccupied = true;
            pax.Seat = seat.SeatNumber;
            pax.IsAssigned = true;
            plane.UpdateZoneWeight(seat.Zone, pax.Weight);
        }

        // Builds groups from current passenger list
        private List<List<Passenger>> BuildGroups(List<Passenger> passengers)
        {
            var groups = new List<List<Passenger>>();
            var groupedPax = new HashSet<string>();

            var dict = passengers.ToDictionary(p => p.Id);

            foreach (var pax in passengers)
            {
                if (groupedPax.Contains(pax.Id))
                    continue;
                var group = new List<Passenger>();
                group.Add(pax);
                groupedPax.Add(pax.Id);
                if (pax.Group != null && pax.Group.Count > 0)
                {
                    foreach (var id in pax.Group)
                    {
                        if (!groupedPax.Contains(id) && dict.TryGetValue(id, out var groupMember))
                        {
                            group.Add(groupMember);
                            groupedPax.Add(id);
                        }
                    }
                }
                groups.Add(group);
            }

            Console.WriteLine("Groups built successfully");

            return groups;
        }

        // Assigns seats to WCHR passengers
        public void AssignWCHR(List<Passenger> passengers, Plane plane)
        {
            var wchrPax = passengers.Where(p => p.Special == "WCHR" && !p.IsAssigned).ToList();
            var aisleSeats = plane.Seats.Where(s => s.IsAisle && !s.IsOccupied).ToList();

            foreach (var pax in wchrPax)
            {
                var seat = aisleSeats.FirstOrDefault();

                if (seat != null)
                {
                    AllocateSeat(seat, pax, plane);
                    aisleSeats.Remove(seat);
                }
                else
                {
                    Console.WriteLine($"No available aisle seats for WCHR pax {pax.Id}");
                }
            }

            Console.WriteLine("WCHR passengers assigned successfully.");
        }

        // Assigns seats to UMNR passengers
        public void AssignUMNR(List<Passenger> passengers, Plane plane)
        {
            var umnrPax = passengers.Where(p => p.Special == "UMNR" && !p.IsAssigned).ToList();

            foreach (var umnr in umnrPax)
            {
                var emptySeats = plane.Seats.Where(p => !p.IsOccupied).ToList();

                bool assigned = false;

                foreach (var seat in emptySeats)
                {
                    if (IsSeatSafe(seat, plane.Seats, umnr, "UMNR"))
                    {
                        AllocateSeat(seat, umnr, plane);
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                {
                    Console.WriteLine($"UMNR pax {umnr.Id}: Unable to pre-assign under UMNR restrictions. Assigned during general allocation.");
                }
            }

            Console.WriteLine("UMNR passengers assigned successfully");
        }

        // Assigns seats to Female Muslim passengers
        public void AssignFMuslim(List<Passenger> passengers, Plane plane)
        {
            var fMusPax = passengers.Where(p => p.Special == "Muslim" && p.Gender == 'F' && !p.IsAssigned).ToList();

            foreach (var fMus in fMusPax)
            {
                var emptySeats = plane.Seats.Where(p => !p.IsOccupied).ToList();

                bool assigned = false;

                foreach (var seat in emptySeats)
                {
                    if (IsSeatSafe(seat, plane.Seats, fMus, "FemMuslim"))
                    {
                        AllocateSeat(seat, fMus, plane);
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                {
                    Console.WriteLine($"Female Muslim pax {fMus.Id}: Unable to pre-assign preferred seat. Assigned during general allocation.");
                }
            }

            Console.WriteLine("Female Muslim passengers assigned successfully.");
        }

        // Assigns seats to groups
        public void AssignGroups(List<List<Passenger>> groups, Plane plane)
        {
            foreach (var group in groups)
            {
                var unassigned = group.Where(p => !p.IsAssigned).ToList();
                if (unassigned.Count == 0)
                    continue;

                // Orders zones by weight to prioritize lightest zone allocation
                var zonesByWeight = new List<string> { "FrontLeft", "FrontRight", "AftLeft", "AftRight" }
                    .OrderBy(z =>
                        z == "FrontLeft" ? plane.FrontLeftWeight :
                        z == "FrontRight" ? plane.FrontRightWeight :
                        z == "AftLeft" ? plane.AftLeftWeight :
                        plane.AftRightWeight);

                foreach (var pax in unassigned)
                {
                    if (!plane.Seats.Any(s => !s.IsOccupied))
                    {
                        Console.WriteLine("No more seats available. Cannot assign remaining passengers.");
                        return;
                    }

                    bool assigned = false;

                    foreach (var zone in zonesByWeight)
                    {
                        var seat = plane.Seats.FirstOrDefault(s => !s.IsOccupied && s.Zone == zone);

                        if (seat != null)
                        {
                            AllocateSeat(seat, pax, plane);
                            assigned = true;
                            break;
                        }
                    }

                    if (!assigned)
                    {
                        Console.WriteLine($"Unable to assign seat for passenger {pax.Id}.");
                    }
                }
            }

            Console.WriteLine("Groups assigned successfully");
        }

        // Assigns remaining passengers while balancing zones
        public void AssignGeneral(List<Passenger> passengers, Plane plane)
        {
            var unassigned = passengers.Where(p => !p.IsAssigned).ToList();
            if (unassigned.Count == 0)
            {
                Console.WriteLine("Seat allocation has been completed.");
                return;
            }

            foreach (var pax in unassigned)
            {
                if (!plane.Seats.Any(s => !s.IsOccupied))
                {
                    Console.WriteLine($"No more seats available. Cannot assign pax {pax.Id}");
                    return;
                }

                bool assigned = false;

                var zonesByWeight = new List<string> { "FrontLeft", "FrontRight", "AftLeft", "AftRight" }
                    .OrderBy(z =>
                        z == "FrontLeft" ? plane.FrontLeftWeight :
                        z == "FrontRight" ? plane.FrontRightWeight :
                        z == "AftLeft" ? plane.AftLeftWeight :
                        plane.AftRightWeight);

                foreach (var zone in zonesByWeight)
                {
                    var seat = plane.Seats.FirstOrDefault(s => !s.IsOccupied && s.Zone == zone);

                    if (seat != null)
                    {
                        AllocateSeat(seat, pax, plane);
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                {
                    Console.WriteLine($"Unable to assign seat for passenger {pax.Id}.");
                }
            }

            Console.WriteLine("Remaining passengers assigned successfully");
        }

        // Checks whether a seat is safe and valid for the specified passenger based on restrictions and rules
        public bool IsSeatSafe(Seat seat, List<Seat> allSeats, Passenger pax, string restriction)
        {
            char[] seatColumns = ['A', 'B', 'C', '-', 'D', 'E', 'F', '-', 'G', 'H', 'J', 'K']; //'-' represents aisle

            string seatRow = seat.SeatNumber[..^1];
            char seatCol = seat.SeatNumber[^1];

            int index = Array.IndexOf(seatColumns, seatCol);

            Seat? leftSeat = null;
            Seat? rightSeat = null;

            bool leftSafe = true;
            bool rightSafe = true;

            if (index > 0 && seatColumns[index - 1] != '-')
            {
                leftSeat = allSeats.FirstOrDefault(s => s.SeatNumber == seatRow + seatColumns[index - 1]);

                if (leftSeat != null && leftSeat.IsOccupied)
                {
                    var neighborPax = leftSeat.Occupant!;
                    if (restriction == "UMNR" && neighborPax.Special != "UMNR" && neighborPax.Gender == 'M')
                        leftSafe = false;
                    else if (restriction == "FemMuslim" && neighborPax.Gender == 'M' && !pax.Group.Contains(neighborPax.Id))
                    {
                        leftSafe = false;
                    }
                }
            }

            if (index < seatColumns.Length - 1 && seatColumns[index + 1] != '-')
            {
                rightSeat = allSeats.FirstOrDefault(s => s.SeatNumber == seatRow + seatColumns[index + 1]);
                if (rightSeat != null && rightSeat.IsOccupied)
                {
                    var neighborPax = rightSeat.Occupant!;
                    if (restriction == "UMNR" && neighborPax.Special != "UMNR" && neighborPax.Gender == 'M')
                        rightSafe = false;
                    else if (restriction == "FemMuslim" && neighborPax.Gender == 'M' && !pax.Group.Contains(neighborPax.Id))
                    {
                        rightSafe = false;
                    }
                }
            }

            return leftSafe && rightSafe;
        }
    }
}
