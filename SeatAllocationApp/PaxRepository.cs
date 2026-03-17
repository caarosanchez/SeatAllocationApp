using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeatAllocationApp
{
    /// <summary>
    /// Handles loading and saving data to/from JSON file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="outputDir"></param>
    internal class PaxRepository(string filePath, string outputDir = "Output")
    {
        private readonly string filePath = filePath;
        private readonly string outputDir = outputDir;

        // Loads pax from JSON file
        public List<Passenger> LoadFromJson()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Input file was not found.");
            }

            string json = File.ReadAllText(filePath);

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Passenger>>(json, options)
                     ?? throw new JsonException("No passenger data found in file.");
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Failed to deserialize passenger data: {ex.Message}");
            }
        }

        // Saves seat allocation into a new JSON file
        public void SaveToJson(List<Passenger> passengers)
        {
            if (!Directory.Exists(outputDir))
            {
                // Creates Output folder
                Directory.CreateDirectory(outputDir);
            }
            string outputPath = Path.Combine(outputDir, "allocated_passengers.json");
            string outputJson = JsonSerializer.Serialize(passengers, new JsonSerializerOptions { WriteIndented = true });
            try
            {
                File.WriteAllText(outputPath, outputJson);
                Console.WriteLine($"Saved passengers seat allocation to: {outputPath}");
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to write output file: {ex.Message}");
            }
        }
    }
}
