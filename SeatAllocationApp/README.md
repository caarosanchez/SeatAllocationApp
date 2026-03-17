# Seat Allocation Program

## How to Run

1. Ensure you have .NET installed (version 8.0 or later).
2. Make sure the input JSON file (`dataset.json`) is inside the `Input` folder.  
3. Open the solution `SeatAllocationApp.sln` in Visual Studio (or your preferred IDE).  
4. Build the solution to restore dependencies and compile the project.  
5. Run the project. The seat allocation process will execute, and the results will be saved in the `Output` folder (creates it if it does not exist).  
6. Console messages provide information about the allocation process and any passengers who could not be assigned seats.

---

## Key Assumptions

- Passengers with special needs (WCHR and UMNR) are assigned **first**, before considering zone weight and group seating.  
- Female Muslim passengers are assigned with their group first. It is assumed that manual adjustments are possible if male passengers from other groups end up adjacent.  
- Zone balancing uses a tolerance percentage which is set in the code and can be adjusted if needed.
- Groups are seated together when possible; if seats are insufficient, some group members may be assigned separately.  
- Input JSON contains unique passenger IDs. Duplicate IDs may cause errors.  
- Seat zones are predefined and fixed: FrontLeft, FrontRight, AftLeft, AftRight.  

---

## Known Limitations

- Group seating cannot guarantee complete adjacency if insufficient seats are available in the same zone.  
- Female Muslim passengers are assigned with their group first, which may not fully prevent males from other groups from being adjacent. Manual adjustments are assumed possible. 
- Program does not provide a graphical interface; allocation results are only available in the JSON output and console messages.  
- The program assumes the input data is correctly formatted; invalid JSON or missing fields may cause errors.  
- Tolerance for zone weight balancing is approximate; perfect balance cannot always be guaranteed.

---

## Notes

- The program prioritizes safety and accessibility requirements first, followed by group and weight balancing.  
- Output JSON structure mirrors input JSON with updated seat assignments.
