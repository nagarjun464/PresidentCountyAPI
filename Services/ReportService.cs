using ClosedXML.Excel;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PresidentCountyAPI.Models;

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

        public byte[] ExportPresidentCountyToPdf(List<PresidentCountyCandidate> candidates)
        {
            try
            {
                // ✅ Use a memory stream to store the PDF
                using (var ms = new MemoryStream())
                {
                    PdfDocument document = new PdfDocument();
                    document.Info.Title = "President County Report";

                    // ✅ Font setup
                    GlobalFontSettings.FontResolver = new CustomFontResolver();
                    XFont headerFont = new XFont("DejaVuSans", 14, XFontStyleEx.Bold);
                    XFont normalFont = new XFont("DejaVuSans", 10, XFontStyleEx.Regular);

                    // ✅ Page setup
                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    double margin = 40;
                    double y = margin;
                    double lineHeight = 20;
                    double pageHeight = page.Height - margin;

                    // ✅ Title
                    gfx.DrawString("President County Report",
                        headerFont, XBrushes.Black,
                        new XRect(0, y, page.Width, lineHeight),
                        XStringFormats.TopCenter);

                    y += lineHeight * 2;

                    // ✅ Table headers
                    gfx.DrawString("State", normalFont, XBrushes.Black, new XRect(margin, y, 80, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString("County", normalFont, XBrushes.Black, new XRect(margin + 80, y, 120, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString("Candidate", normalFont, XBrushes.Black, new XRect(margin + 200, y, 120, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString("Party", normalFont, XBrushes.Black, new XRect(margin + 310, y, 60, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString("Votes", normalFont, XBrushes.Black, new XRect(margin + 380, y, 60, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString("Won", normalFont, XBrushes.Black, new XRect(margin + 470, y, 60, lineHeight), XStringFormats.TopLeft);

                    y += lineHeight;

                    // ✅ Draw a horizontal line
                    gfx.DrawLine(XPens.Black, margin, y, page.Width - margin, y);
                    y += 5;

                    // ✅ Loop through candidates (with pagination)
                    foreach (var c in candidates)
                    {
                        // Add a new page if needed
                        if (y + lineHeight > pageHeight)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            y = margin;

                            gfx.DrawString("President County Report (continued)",
                                headerFont, XBrushes.Black,
                                new XRect(0, y, page.Width, lineHeight),
                                XStringFormats.TopCenter);

                            y += lineHeight * 2;
                        }

                        gfx.DrawString(c.State ?? "", normalFont, XBrushes.Black, new XRect(margin, y, 80, lineHeight), XStringFormats.TopLeft);
                        gfx.DrawString(c.County ?? "", normalFont, XBrushes.Black, new XRect(margin + 80, y, 120, lineHeight), XStringFormats.TopLeft);
                        gfx.DrawString(c.CandidateName ?? "", normalFont, XBrushes.Black, new XRect(margin + 200, y, 120, lineHeight), XStringFormats.TopLeft);
                        gfx.DrawString(c.Party ?? "", normalFont, XBrushes.Black, new XRect(margin + 310, y, 60, lineHeight), XStringFormats.TopLeft);
                        gfx.DrawString(c.TotalVotes ?? "", normalFont, XBrushes.Black, new XRect(margin + 380, y, 60, lineHeight), XStringFormats.TopLeft);
                        gfx.DrawString(c.Won ?? "", normalFont, XBrushes.Black, new XRect(margin + 470, y, 60, lineHeight), XStringFormats.TopLeft);

                        y += lineHeight;
                    }

                    // ✅ Save PDF to memory
                    document.Save(ms, false);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                var message = $"PDF export failed: {ex.Message}";
                return System.Text.Encoding.UTF8.GetBytes(message);
            }
        }

    }
}
