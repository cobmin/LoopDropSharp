using LoopDropSharp.Models;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using OfficeOpenXml;
using System.Threading.Tasks;
using OfficeOpenXml.Style;
using System.Drawing;
using OfficeOpenXml.Table;
using System.Data.SqlTypes;
using OfficeOpenXml.Drawing.Chart;

namespace LoopDropSharp.Helpers
{
    public class ExcelFile
    {
        public static async Task CreateExcelFile(LoopExchange loopExchangeData, List<LoopPhunksHolderInformation> nftInformationAndOwner)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileNameLocation = new FileInfo(@".\LoopPhunksDataCOOL.xlsx");

            await SaveExcelFile(loopExchangeData, nftInformationAndOwner, fileNameLocation);
        }

        private static async Task SaveExcelFile(LoopExchange loopExchangeData, List<LoopPhunksHolderInformation> nftInformationAndOwner, FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("Items");
                CreateHeader(ws, loopExchangeData, nftInformationAndOwner);

                //ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Row(1).Style.Font.Size = 24;
                //ws.Row(1).Style.Font.Color.SetColor(Color.Blue);

                //ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Row(2).Style.Font.Bold = true;
                //ws.Column(3).Width = 20;

                //var pic = ws.Drawings.AddPicture("MyPhoto", new FileInfo("C:\\Code\\JokerBurnMeme.jpg"));
                //pic.SetPosition(2, 0, 1, 0);

                var range = ws.Cells["A6"].LoadFromCollection(nftInformationAndOwner.Select(x => x.loopPhunksHolderAndAmount), true, TableStyles.Medium1);
                range = ws.Cells["C6"].LoadFromCollection(nftInformationAndOwner.Select(x => x.loopPhunksInformation), true, TableStyles.Medium1);

                ws.Cells.AutoFitColumns();
                //ws.Column(3).Width = 20;

                var loopPhunksHolderAndTotalAmount = new List<LoopPhunksHolderAndAmount>();

                foreach (var data in nftInformationAndOwner)
                {
                    if (!loopPhunksHolderAndTotalAmount.Any(x => x.owner == data.loopPhunksHolderAndAmount.owner))
                    {
                        loopPhunksHolderAndTotalAmount.Add(new LoopPhunksHolderAndAmount()
                        {
                            owner = data.loopPhunksHolderAndAmount.owner,
                            amount = data.loopPhunksHolderAndAmount.amount

                        });
                    }
                    else
                    {
                        loopPhunksHolderAndTotalAmount.Find(x => x.owner == data.loopPhunksHolderAndAmount.owner).amount++;
                    }
                }
                var chartTest = new LoopPhunksOwnerDistribution();
                var loopPhunksHolderAmountAndPercentage = new List<LoopPhunksHolderAmountAndPercentage>();

                foreach (var data in loopPhunksHolderAndTotalAmount)
                {
                    loopPhunksHolderAmountAndPercentage.Add(new LoopPhunksHolderAmountAndPercentage()
                    {
                        owner = data.owner,
                        amount = data.amount,
                        percentageOwned = Decimal.Round((Convert.ToDecimal(data.amount) / Convert.ToDecimal(loopExchangeData.count)) * 100, 2)
                    });
                    if (data.amount == 1)
                    {
                        chartTest.amount1++;
                    }
                    else if (data.amount >= 2 && data.amount <= 5)
                    {
                        chartTest.amount2To5++;
                    }
                    else if (data.amount >= 6 && data.amount <= 10)
                    {
                        chartTest.amount6To10++;
                    }
                    else if (data.amount >= 11 && data.amount <= 19)
                    {
                        chartTest.amount11To19++;
                    }
                    else if (data.amount >= 20)
                    {
                        chartTest.amount20Plus++;
                    }
                }

                var ws2 = package.Workbook.Worksheets.Add("Analytics");
                CreateHeader(ws2, loopExchangeData, nftInformationAndOwner);
                range = ws2.Cells["A6"].LoadFromCollection(loopPhunksHolderAmountAndPercentage.OrderByDescending(x=>x.amount), true, TableStyles.Medium1);
                ws2.Cells["E6"].Value = $"Owner Distribution [{nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count()}]";
                ws2.Cells["E7"].Value = "1 item";
                ws2.Cells["E8"].Value = "2-5 items";
                ws2.Cells["E9"].Value = "6-10 items";
                ws2.Cells["E10"].Value = "11-19 items";
                ws2.Cells["E11"].Value ="20+ items";
                ws2.Cells["F7"].Value = chartTest.amount1;
                ws2.Cells["F8"].Value = chartTest.amount2To5;
                ws2.Cells["F9"].Value = chartTest.amount6To10;
                ws2.Cells["F10"].Value = chartTest.amount11To19;
                ws2.Cells["F11"].Value = chartTest.amount20Plus;
                ws2.Cells["G7"].Value = $"{Decimal.Round((Convert.ToDecimal(chartTest.amount1)/ Convert.ToDecimal(nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count()))*100, 2)}%";
                ws2.Cells["G8"].Value = $"{Decimal.Round((Convert.ToDecimal(chartTest.amount2To5)/ Convert.ToDecimal(nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count())) * 100, 2)}%";
                ws2.Cells["G9"].Value = $"{Decimal.Round((Convert.ToDecimal(chartTest.amount6To10)/ Convert.ToDecimal(nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count())) * 100, 2)}%";
                ws2.Cells["G10"].Value = $"{Decimal.Round((Convert.ToDecimal(chartTest.amount11To19)/ Convert.ToDecimal(nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count())) * 100, 2)}%";
                ws2.Cells["G11"].Value = $"{Decimal.Round((Convert.ToDecimal(chartTest.amount20Plus)/ Convert.ToDecimal(nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count())) * 100, 2)}%";

                ws2.Cells.AutoFitColumns();

                //Add a column chart
                //var chart = ws2.Drawings.AddBarChart("column3dChart", eBarChartType.ColumnClustered3D);
                //var serie = chart.Series.Add(ws2.Cells[7, 2, 11, 2]);
                //serie.Header = "Order Value";
                //chart.SetPosition(0, 0, 6, 0);
                //chart.SetSize(600, 200);
                //chart.Title.Text = "Owner Distribution";

                await package.SaveAsync();
            }
        }

        private static void CreateHeader(ExcelWorksheet ws, LoopExchange loopExchangeData, List<LoopPhunksHolderInformation> nftInformationAndOwner)
        {
            ws.Cells["A1"].Value = loopExchangeData.name;
            ws.Row(1).Style.Font.Size = 24;
            ws.Row(1).Style.Font.Bold = true;

            ExcelRange inLine = ws.Cells["A2"];
            inLine.IsRichText = true;
            ExcelRichText text1 = inLine.RichText.Add("By ");
            ExcelRichText text2 = inLine.RichText.Add(loopExchangeData.minterDomain);
            text2.Bold = true;
            ws.Row(2).Style.Font.Size = 16;

            inLine = ws.Cells["A3"];
            inLine.IsRichText = true;
            text1 = inLine.RichText.Add("Items ");
            text2 = inLine.RichText.Add(loopExchangeData.count.ToString());
            text2.Bold = true;
            ExcelRichText text3 = inLine.RichText.Add(" · ");
            text1 = inLine.RichText.Add("Created ");
            text1.Bold = false;
            text2 = inLine.RichText.Add("Oct 2022");
            text2.Bold = true;
            text3 = inLine.RichText.Add(" · ");
            text1 = inLine.RichText.Add("Creator Fee ");
            text1.Bold = false;
            text2 = inLine.RichText.Add("0%");
            text2.Bold = true;

            ws.Cells["A4"].Value = $"{loopExchangeData.description}";

            inLine = ws.Cells["A5"];
            inLine.IsRichText = true;
            text1 = inLine.RichText.Add($"{Decimal.Round(decimal.Parse(loopExchangeData.volume1) / 1000000000000000000000m)}K LRC ");
            text1.Bold = true;
            text2 = inLine.RichText.Add("total volume · ");
            text2.Bold = false;
            text1 = inLine.RichText.Add($"{decimal.Parse(loopExchangeData.floorPrice1) / 1000000000000000000m} LRC ");
            text1.Bold = true;
            text2 = inLine.RichText.Add("floor price · ");
            text2.Bold = false;
            text1 = inLine.RichText.Add($"{nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count()} ");
            text1.Bold = true;
            text2 = inLine.RichText.Add("owners · ");
            text2.Bold = false;
            text1 = inLine.RichText.Add($"{Decimal.Round(Convert.ToDecimal(nftInformationAndOwner.GroupBy(x => x.loopPhunksHolderAndAmount.owner).Select(g => g.First()).Count()) / Convert.ToDecimal(loopExchangeData.count) * 100)}% ");
            text1.Bold = true;
            text2 = inLine.RichText.Add("unique owners");
            text2.Bold = false;

            ws.Cells["A1:C1"].Merge = true;
            ws.Cells["A2:B2"].Merge = true;
            ws.Cells["A3:E3"].Merge = true;
            ws.Cells["A4:N4"].Merge = true;
            ws.Cells["A5:G5"].Merge = true;
        }

        public static async Task CreateExcelFileImxCollection(List<Result> collectionHolders)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileNameLocation = new FileInfo(@".\ImxCollectionHolders.xlsx");

            await SaveExcelFileImxCollection(collectionHolders, fileNameLocation);
        }

        private static async Task SaveExcelFileImxCollection(List<Result> collectionHolders, FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("Items");

                var range = ws.Cells["A1"].LoadFromCollection(collectionHolders, true, TableStyles.Medium1);
                //range = ws.Cells["C6"].LoadFromCollection(collectionHolders.Select(x => x.loopPhunksInformation), true, TableStyles.Medium1);

                ws.Cells.AutoFitColumns();

                await package.SaveAsync();
            }
        }

        public static async Task CreateExcelFileImxMints(List<MintResult> collectionHolders)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileNameLocation = new FileInfo(@".\ImxCollectionMints.xlsx");

            await SaveExcelFileImxMints(collectionHolders, fileNameLocation);
        }

        private static async Task SaveExcelFileImxMints(List<MintResult> collectionHolders, FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("Items");
                var range = ws.Cells["A1"].LoadFromCollection(collectionHolders, true, TableStyles.Medium1);
                range = ws.Cells["F1"].LoadFromCollection(collectionHolders.Select(x => x.token.data.token_id), true, TableStyles.Medium1);

                ws.Cells.AutoFitColumns();
                await package.SaveAsync();
            }
        }

        public static async Task CreateExcelFileImxCollectionTransfers(List<TransferResult> collectionHolders)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileNameLocation = new FileInfo(@".\ImxCollectionTransfers.xlsx");

            await SaveExcelFileImxTransfers(collectionHolders, fileNameLocation);
        }

        private static async Task SaveExcelFileImxTransfers(List<TransferResult> collectionHolders, FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("Items");
                var range = ws.Cells["A1"].LoadFromCollection(collectionHolders, true, TableStyles.Medium1);

                ws.Cells.AutoFitColumns();
                await package.SaveAsync();
            }
        }

        public static async Task CreateExcelFileImxWalletBalance(List<ImxUserEth> walletAddresses)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileNameLocation = new FileInfo(@".\ImxWalletBalances.xlsx");

            await SaveExcelFileImxWalletBalance(walletAddresses, fileNameLocation);

            Console.WriteLine($"Finished. Your file can be found {AppDomain.CurrentDomain.BaseDirectory}\nImxHolders.xlsx");
        }

        private static async Task SaveExcelFileImxWalletBalance(List<ImxUserEth> walletAddresses, FileInfo file)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("Items");
                var range = ws.Cells["A1"].LoadFromCollection(walletAddresses, true, TableStyles.Medium1);

                ws.Cells.AutoFitColumns();
                await package.SaveAsync();
            }
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }
    }
}
