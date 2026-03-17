# Seat Allocation App

# Passenger Seat Assignment System

## Overview

This project is a C# console application that processes passenger data from a JSON file and assigns seats based on defined business rules. The application reads input data, applies seat allocation logic, and outputs the updated data into a new JSON file.

---

## Features

- JSON data input and output
- Seat assignment based on business rules
- Clean and maintainable structure
- Separation of concerns
- Basic error handling

---

## Architecture

The project is structured with separation of concerns:

- **Models**
  - - Represent data entities such as Passenger, Plane, and Seat

- **Repository**
  - Handles JSON input/output operations

- **SeatAllocator**
  - Contains the business logic for assigning seats

- **Program**
  - Entry point that orchestrates the workflow

---

## How It Works

1. The application reads passenger data from an input JSON file
2. Seat assignment logic is applied through the `SeatAllocator`
3. The updated passenger list is written to an output JSON file

---

## Input

The application expects a JSON file containing passenger data, including attributes such as name, age, gender, and group associations.

---

## Output

The application generates a JSON file with the same passenger data, enriched with assigned seat information based on the implemented business rules.

---

## Technologies Used

- C#
- .NET
- System.Text.Json

---

## How to Run

1. Clone or download the repository
2. Open the solution in Visual Studio
3. Ensure the input JSON file is placed in the expected directory (e.g., `/Input`)
4. Build and run the project
5. Check the output JSON file in the output directory (e.g., `/Output`)

---

## Notes

- The project focuses on clarity, structure, and maintainability
- Business rules are encapsulated within the `SeatAllocator` class
- Designed as a demonstration of applying logic to structured data

---

## Future Improvements

- Support for different seat layouts
- More advanced seat assignment rules
