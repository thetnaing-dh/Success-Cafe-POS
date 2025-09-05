using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;


public class DirectPrint
{
    public void PrintToPrinter(LocalReport report, string printername = null)
    {
        PageSettings pageSettings = new PageSettings();
        pageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;
        pageSettings.Landscape = report.GetDefaultPageSettings().IsLandscape;
        pageSettings.Margins = report.GetDefaultPageSettings().Margins;
        Print(report, pageSettings, printername);
    }
    private void Print(LocalReport report, PageSettings pageSettings, string printername = null)
    {
       string deviceInfo =
            $@"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>{pageSettings.PaperSize.Width * 100}in</PageWidth>
                    <PageHeight>{pageSettings.PaperSize.Height * 100}in</PageHeight>
                    <MarginTop>{pageSettings.Margins.Top * 100}in</MarginTop>
                    <MarginLeft>{pageSettings.Margins.Left * 100}in</MarginLeft>
                    <MarginRight>{pageSettings.Margins.Right * 100}in</MarginRight>
                    <MarginBottom>{pageSettings.Margins.Bottom * 100}in</MarginBottom>
                </DeviceInfo>";
        Warning[] warnings;
        var streams = new List<Stream>();
        var pageIndex = 0;
        report.Render("Image", deviceInfo,
            (name, fileNameExtension, encoding, mimeType, willSeek) =>
            {
                MemoryStream stream = new MemoryStream();
                streams.Add(stream);
                return stream;
            }, out warnings);
        foreach (Stream stream in streams)
            stream.Position = 0;
        if (streams == null || streams.Count == 0)
            throw new Exception("No stream to print.");
        // Print the report using a PrintDocument
        using (PrintDocument printDocument = new PrintDocument())
        {
            if (printername != null)
            {
                printDocument.DefaultPageSettings = pageSettings;
                printDocument.PrinterSettings.PrinterName = printername;
            }
            else
            {
                printDocument.DefaultPageSettings = pageSettings; //use default printer
            }
            //PrinterSettings.PrinterName = printerName;


            if (!printDocument.PrinterSettings.IsValid)
                throw new Exception("Can't find the default printer.");
            else
            {
                printDocument.PrintPage += (sender, e) =>
                {
                    Metafile pageImage = new Metafile(streams[pageIndex]);
                    Rectangle adjustedRect = new Rectangle(e.PageBounds.Left - (int)e.PageSettings.HardMarginX, e.PageBounds.Top - (int)e.PageSettings.HardMarginY, e.PageBounds.Width, e.PageBounds.Height);
                    e.Graphics.FillRectangle(Brushes.Transparent, adjustedRect);
                    e.Graphics.DrawImage(pageImage, adjustedRect);
                    pageIndex++;
                    e.HasMorePages = (pageIndex < streams.Count);
                  //  e.Graphics.DrawRectangle(Pens.Red, adjustedRect);
                };
                printDocument.EndPrint += (Sender, e) =>
                {
                    if (streams != null)
                    {
                        foreach (Stream stream in streams)
                            stream.Close();
                        streams = null;
                    }
                };
                printDocument.Print();
            }
        }
    }
}