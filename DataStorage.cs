using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CheapestFlights
{
    internal class DataStorage
    {
        private List<FlightData> flightDataList = new List<FlightData>();
        private string excelFilePath = "flightData.xlsx";

        public void StoreFlightData(string date, string departureCity, string destination, string time, string price, bool isReturnFlight = false)
        {
            var flightData = new FlightData
            {
                Date = date,
                Departure = departureCity,
                Destination = destination,
                Time = time,
                Price = price
            };

            // Check if the flight data already exists in the list
            if (!flightDataList.Exists(fd => fd.Date == date && fd.Departure == departureCity && fd.Destination == destination && fd.Time == time && fd.Price == price))
            {
                flightDataList.Add(flightData);
            }

            WriteDataToExcelFile(flightDataList, excelFilePath, isReturnFlight);
        }

        public void ClearFlightDataList()
        {
            flightDataList.Clear();
        }

        public void WriteDataToExcelFile(List<FlightData> flightDataList, string excelFilePath, bool isReturnFlight)
        {
            var file = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(file))
            {
                string sheetName = isReturnFlight ? "ReturnFlights" : "DepartingFlights";

                // Try to get the worksheet; if it doesn't exist, create a new one
                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add(sheetName);

                    // If it's a new worksheet, you might want to set up headers here
                    worksheet.Cells[1, 1].Value = "Date";          // Column A
                    worksheet.Cells[1, 2].Value = "Departure";     // Column B
                    worksheet.Cells[1, 3].Value = "Destination";   // Column C
                    worksheet.Cells[1, 4].Value = "Time";          // Column D
                    worksheet.Cells[1, 5].Value = "Price";         // Column E
                }

                int lastRow = worksheet.Dimension == null ? 1 : worksheet.Dimension.End.Row;

                for (int i = 0; i < flightDataList.Count; i++)
                {
                    // Starting the data from the last empty row
                    var row = lastRow + i + 1;

                    // Update the necessary columns in the worksheet
                    worksheet.Cells[row, 1].Value = flightDataList[i].Date;         // Column A
                    worksheet.Cells[row, 2].Value = flightDataList[i].Departure;    // Column B
                    worksheet.Cells[row, 3].Value = flightDataList[i].Destination;  // Column C
                    worksheet.Cells[row, 4].Value = flightDataList[i].Time;         // Column D
                    worksheet.Cells[row, 5].Value = flightDataList[i].Price;        // Column E
                }

                package.Save();
            }
        }

        public void WriteDataToFile(List<string> data, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            // Append all records to the string builder
            foreach (var record in data)
            {
                fileContent.AppendLine(record);
            }

            // Explicitly use UTF-8 encoding
            File.WriteAllText(filePath, fileContent.ToString(), Encoding.UTF8);
            Console.WriteLine($"Written data to {filePath}");
        }
    }
}
