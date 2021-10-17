using DTO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFConverter
{
    public class Creator
    {
        public static string fontArial = "arial.ttf";
        public static void CreateReport(IndicatorStatisticsDTO model, string filePath)
        {
            var FONT = Path.Combine(Environment.CurrentDirectory, @"font", fontArial);
            using PdfWriter pdfWriter = new PdfWriter(filePath);
            using PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            using Document document = new Document(pdfDocument);
            pdfDocument.SetTagged();
            Table table = new Table(new float[] { 50, 150, 50 });
            PdfFont f1 = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            table.AddHeaderCell(new Cell().Add(new Paragraph("Дата")).SetFont(f1).SetTextAlignment(TextAlignment.CENTER));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Значение")).SetFont(f1).SetTextAlignment(TextAlignment.CENTER));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ед. изм.")).SetFont(f1).SetTextAlignment(TextAlignment.CENTER));
            foreach (var item in model.ItemsDTO)
            {
                table.AddCell(new Paragraph(item.ResearchDateDTO.ToShortDateString()).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Paragraph(item.ValueDTO.ToString()).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Paragraph(item.UnitDTO).SetFont(f1).SetTextAlignment(TextAlignment.CENTER));
            }
            document.Add(new Paragraph($"Пациент - {model.PatientNameDTO}"));
            document.Add(new Paragraph($"Таблица данных - {model.NameDTO}").SetFont(f1));
            
            document.Add(table);
            document.Flush();
            
            document.Close();
        }
    }
}
