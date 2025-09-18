using EVCarbonMarketplace.Model.Payload.Response.TelemetryRecord;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IFileReaderService
    {
        Task<List<TelemetryRecordResponse>> ReadTelemetryFileAsync(IFormFile file);

    }
}
