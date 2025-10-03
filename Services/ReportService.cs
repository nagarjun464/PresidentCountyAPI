using ClosedXML.Excel;
using PresidentCountyAPI.Models;
using System;

namespace PresidentCountyAPI.Services
{
    public class ReportService
    {
        public byte[] ExportPresidentCountyToExcel(List<PresidentCountyCandidate> data)
        {
            using var workbook = new XLWorkbook();  // using ClosedXML
            var ws = workbook.Worksheets.Add("Report");

            // Add headers
            ws.Cell(1, 1).Value = "State";
            ws.Cell(1, 2).Value = "County";
            ws.Cell(1, 3).Value = "Candidate";
            ws.Cell(1, 4).Value = "Party";
            ws.Cell(1, 5).Value = "Votes";
            ws.Cell(1, 6).Value = "Won";

            // Populate rows
            for (int i = 0; i < data.Count; i++)
            {
                var r = i + 2;
                var c = data[i];
                ws.Cell(r, 1).Value = c.State;
                ws.Cell(r, 2).Value = c.County;
                ws.Cell(r, 3).Value = c.CandidateName;
                ws.Cell(r, 4).Value = c.Party;
                ws.Cell(r, 5).Value = c.TotalVotes;
                ws.Cell(r, 6).Value = c.Won;
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportPresidentCountyToPdf(List<PresidentCountyCandidate> data)
        {
            // One option: create Excel then convert to PDF (if using Syncfusion XlsIO + renderer) 
            // Or use a PDF library directly (QuestPDF, iText, etc.)
            throw new NotImplementedException();
        }
    }

}
