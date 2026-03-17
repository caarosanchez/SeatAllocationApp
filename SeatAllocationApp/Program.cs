using System;
using System.Text.Json;

namespace SeatAllocationApp
{
    /// <summary>
    /// Entry point of the Seat Allocation program.
    /// Orchestrates the overall flow: load pax, initialize planes, assign seats, and save results.
    /// </summary>

    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Load pax data from JSON
                var repository = new PaxRepository(@"Input\dataset.json"); 
                var passengers = repository.LoadFromJson();
                Console.WriteLine("Data loaded successfully.");

                //Initialize airplane seats
                var plane = new Plane();
                plane.InitializeSeats();

                //Assign seats to pax
                var allocator = new SeatAllocator();
                allocator.AssignSeats(passengers, plane);

                //Save assigned seats to new JSON file
                repository.SaveToJson(passengers);
            }
            catch (Exception ex)
            {
                //Display any errors encountered
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
