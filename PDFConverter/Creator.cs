using DTO;
using iText.IO.Font;
using iText.IO.Image;
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
        public static void CreateReport(IndicatorStatisticsDTO model, string dirPath)
        {
            var FONT = Path.Combine(Environment.CurrentDirectory, @"font", fontArial);
            var IMG = String.Empty;

            if(model.TypeDTO == 0)
            {
                IMG = Path.Combine(dirPath, "imageIndicator.png");
            }

            using PdfWriter pdfWriter = new PdfWriter(Path.Combine(dirPath, string.Concat(model.NameDTO, ".pdf")));
            using PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            using Document document = new Document(pdfDocument);
            pdfDocument.SetTagged();

            if(model.TypeDTO == 0)
            {
                Table table = new Table(new float[] { 50, 150, 50 });
                PdfFont f1 = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                table.AddHeaderCell(new Cell().Add(new Paragraph("Дата")).SetFont(f1).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Значение")).SetFont(f1).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Ед. изм.")).SetFont(f1).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                foreach (var item in model.ItemsDTO)
                {
                    table.AddCell(new Paragraph(item.ResearchDateDTO.ToShortDateString()).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Paragraph(item.ValueDTO.ToString()).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Paragraph(item.UnitDTO).SetFont(f1).SetTextAlignment(TextAlignment.CENTER));
                }
                document.Add(new Paragraph($"Пациент - {model.PatientNameDTO}").SetFont(f1));
                document.Add(new Paragraph($"График изменения - {model.NameDTO}").SetFont(f1));
                document.Add(new Image(ImageDataFactory.Create(IMG)));
                document.Add(new Paragraph($"Таблица данных - {model.NameDTO}").SetFont(f1));
                document.Add(table.SetHorizontalAlignment(HorizontalAlignment.CENTER));
                document.Flush();

                document.Close();
            }
            else
            {
                Table table = new Table(new float[] { 50, 150});
                PdfFont f1 = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                table.AddHeaderCell(new Cell().Add(new Paragraph("Дата")).SetFont(f1).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Значение")).SetFont(f1).SetHorizontalAlignment(HorizontalAlignment.CENTER));
                
                foreach (var item in model.ItemsDTO)
                {
                    var valueToPrint = String.Empty;
                    if(item.ValueDTO == 0)
                    {
                        valueToPrint = "Не обнаружено";
                    }
                    else
                    {
                        valueToPrint = "ВЫЯВЛЕНО";
                    }
                    table.AddCell(new Paragraph(item.ResearchDateDTO.ToShortDateString()).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Paragraph(valueToPrint).SetFont(f1).SetTextAlignment(TextAlignment.CENTER));
                }
                document.Add(new Paragraph($"Пациент - {model.PatientNameDTO}").SetFont(f1));
                document.Add(new Paragraph($"Статистика изменения - {model.NameDTO}").SetFont(f1));
                document.Add(new Paragraph($"Таблица данных - {model.NameDTO}").SetFont(f1));
                document.Add(table.SetHorizontalAlignment(HorizontalAlignment.CENTER));
                document.Flush();

                document.Close();
            }
            
        }
    }
}
