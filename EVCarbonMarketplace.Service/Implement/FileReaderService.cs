using EVCarbonMarketplace.Model.Payload.Response.TelemetryRecord;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class FileReaderService : IFileReaderService
    {
        
  
        public async Task<List<TelemetryRecordResponse>> ReadTelemetryFileAsync(IFormFile file)
        {
            var records = new List<TelemetryRecordResponse>();

            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string line;
                bool isHeader = true;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (isHeader) { isHeader = false; continue; }

                    var parts = line.Split(',');

                    var record = new TelemetryRecordResponse
                    {
                        LoggedAt = DateTime.Parse(parts[0]),
                        Odometer = decimal.Parse(parts[1]),
                        DistanceTravelled = decimal.Parse(parts[2]),
                        EnergyConsumed = decimal.Parse(parts[3]),
                        BatteryLevel = decimal.Parse(parts[4])
                    };

                    records.Add(record);
                }
            }

            return records;
        }
    }
}
