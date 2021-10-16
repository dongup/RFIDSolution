using FastMember;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RFIDSolution.Server.Utils;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Inventory;
using RFIDSolution.Shared.Models.ProductInout;
using RFIDSolution.Shared.Models.Products;
using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ApiControllerBase
    {
        public ReportsController(AppDbContext _context, IWebHostEnvironment env) : base(_context, webEnv: env)
        {

        }
        private string rootPath => _env.WebRootPath;

        protected string FileNameBuilder(string fileName, string fromDate = "", string toDate = "", bool takeAll = true)
        {
            string dateInfo = $"{fromDate} - {toDate}";
            fileName = takeAll ? fileName : $"{fileName} {dateInfo}";
            fileName += ".xlsx";
            return fileName;
        }

        protected IActionResult Download(string path)
        {
            string relativePath = path.RelativePath();
            return Ok(relativePath);
        }

        protected string CreateExcel(DataTable data, string template,
            string fileName, int rowStart,
            bool fillHeader = false, string sheetName = "Sheet1",
            bool formatNumber = true, bool keepFormat = false, bool convertNumber = true)
        {
            string templatePath = Path.Combine(rootPath, "ExcelTemplates", template);
            string outPutPath = Path.Combine(rootPath, "ExcelResults", fileName);

            ExcelHelper excel = new ExcelHelper(templatePath);
            XSSFWorkbook wb = excel.hssfworkbook;
            ICellStyle style = wb.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;

            ICellStyle numberStyle = wb.CreateCellStyle();
            numberStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.0");
            numberStyle.BorderBottom = BorderStyle.Thin;
            numberStyle.BorderTop = BorderStyle.Thin;
            numberStyle.BorderRight = BorderStyle.Thin;
            numberStyle.BorderLeft = BorderStyle.Thin;


            ICellStyle linkStyle = wb.CreateCellStyle();
            linkStyle.BorderBottom = BorderStyle.Thin;
            linkStyle.BorderTop = BorderStyle.Thin;
            linkStyle.BorderRight = BorderStyle.Thin;
            linkStyle.BorderLeft = BorderStyle.Thin;
            linkStyle.FillForegroundColor = IndexedColors.Blue.Index;
            IFont font = wb.CreateFont();
            font.Underline = FontUnderlineType.Single;
            font.Color = HSSFColor.Blue.Index;
            linkStyle.SetFont(font);

            ICellStyle warningStyle = wb.CreateCellStyle();
            warningStyle.BorderBottom = BorderStyle.Thin;
            warningStyle.BorderTop = BorderStyle.Thin;
            warningStyle.BorderRight = BorderStyle.Thin;
            warningStyle.BorderLeft = BorderStyle.Thin;
            warningStyle.FillBackgroundColor = IndexedColors.Yellow.Index;
            warningStyle.FillPattern = FillPattern.ThinBackwardDiagonals;
            warningStyle.FillForegroundColor = IndexedColors.White.Index;

            ISheet sheet1 = wb.GetSheet(sheetName);
            if (sheet1 == null)
            {
                throw new Exception("There is no such sheet " + sheetName + " exist in the template!");
            }

            for (int i = 0; i < data.Rows.Count; i++)
            {
                //LogConsole($"Added row {i} to excel.");
                var row = data.Rows[i];
                for (int j = 0; j < row.ItemArray.Count(); j++)
                {
                    var collumn = data.Columns[j];
                    //Nếu cần tạo cột động thì fillHeader = true
                    if (fillHeader)
                    {
                        var header = sheet1.GetRow(0) ?? sheet1.CreateRow(0);
                        var headerCell = header.GetCell(j) ?? header.CreateCell(j);
                        if (string.IsNullOrEmpty(headerCell.StringCellValue))
                        {
                            ICellStyle headerStyle = wb.CreateCellStyle();
                            headerStyle.WrapText = true;
                            headerCell.CellStyle = headerStyle;
                            headerCell.SetCellValue(collumn.ColumnName);
                        }
                    }

                    int index = i + rowStart;
                    var excelRow = sheet1.GetRow(index) ?? sheet1.CreateRow(index);
                    var excelCell = excelRow.GetCell(j) ?? excelRow.CreateCell(j);
                    //Console.WriteLine();
                    if (excelCell != null)
                    {
                        if (!keepFormat)
                        {
                            excelCell.CellStyle = style;
                        }
                        string value = row.ItemArray[j].ToString().ToString(new CultureInfo("en-US"));
                        //Console.WriteLine(value + " | ");
                        bool isWarning = false;

                        if (value.Contains(';'))
                        {
                            var arr = value.Split(';');
                            value = arr[0];
                            isWarning = arr[1] == "1" ? true : false;
                        }

                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.DefaultThreadCurrentCulture, out double num))
                        {
                            if (!convertNumber)
                            {
                                excelCell.SetCellValue(num);
                                excelCell.SetCellType(CellType.Numeric);
                            }
                            else
                            {
                                string val = $"VALUE(\"{num}\")";
                                excelCell.SetCellFormula(val);
                                excelCell.SetCellType(CellType.Formula);
                                excelCell.CellStyle = numberStyle;
                            }
                        }
                        else
                        {
                            if (value.Contains("<br/>"))
                            {
                                value = value.Replace("<br/>", Environment.NewLine);
                                value = value.Replace("<b>", "");
                                value = value.Replace("</b>", "");
                            }

                        }

                        if (isWarning)
                        {
                            excelCell.CellStyle = warningStyle;
                        }
                        excelCell.SetCellValue(value);
                    }
                }

                //Console.WriteLine();
            }

            sheet1.ForceFormulaRecalculation = true;

            excel.WriteToFile(outPutPath);
            return outPutPath;
        }

        protected void updateExcelFile(string filePath, string value, int row, int collum, string sheetName = "Sheet1")
        {
            ExcelHelper excel = new ExcelHelper(Path.Combine(rootPath, filePath));
            XSSFWorkbook wb = excel.hssfworkbook;
            ISheet sheet1 = wb.GetSheet(sheetName);
            if (sheet1 == null)
            {
                throw new Exception("There is no such sheet " + sheetName + " exist in the template!");
            }

            var excelRow = sheet1.GetRow(row) ?? sheet1.CreateRow(row);
            var excelCell = excelRow.GetCell(collum) ?? excelRow.CreateCell(collum);

            excelCell.SetCellValue(value);
            excel.SaveChange();
        }

        protected void setCellComment(ISheet sheet, IWorkbook workbook, ICell cell, string commentstring)
        {
            // Create the drawing patriarch (top level container for all shapes including cell comments)
            IDrawing patriarch = (XSSFDrawing)sheet.CreateDrawingPatriarch();

            // Client anchor defines size and position of the comment in the worksheet
            IComment comment = patriarch.CreateCellComment(new XSSFClientAnchor(0, 0, 0, 0, 2, 1, 4, 4));

            // Set comment author
            comment.Author = "Hệ thống";

            // Set text in the comment
            comment.String = new XSSFRichTextString($"{comment.Author}:{Environment.NewLine} {commentstring}");

            // If you want the author displayed in bold on top like in Excel
            // The author will be displayed in the status bar when on mouse over the commented cell
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            comment.String.ApplyFont(0, comment.Author.Length, font);

            // Set comment visible
            comment.Visible = false;

            // Assign comment to a cell
            cell.CellComment = comment;
        }

        [HttpGet("inboundreport")]
        public async Task<IActionResult> GetInboudReport(string fromDate, string toDate)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();
            var rspns = new ResponseModel<List<ProductModel>>();

            var query = _context.PRODUCT
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderByDescending(x => x.CREATED_DATE)
                .Select(x => new
                {
                    ID = x.PRODUCT_ID,
                    SKU = x.PRODUCT_CODE,
                    ModelName = x.Model.MODEL_NAME,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    Size = x.PRODUCT_SIZE,
                    DevStyleName = x.DEV_NAME,
                    Season = x.PRODUCT_SEASON,
                    Stage = x.PRODUCT_STAGE,
                    Location = x.PRODUCT_LOCATION,
                    POC = x.PRODUCT_POC,
                    LR = x.LR.GetDescription(),
                    REMARKS = x.PRODUCT_REMARKS,
                    TimeIn = x.CREATED_DATE.ToVNString()
                }).ToList();

            DataTable dataExcel = new DataTable();
            using (var reader = ObjectReader.Create(query, "ID", "SKU", "ModelName", "Category", "ColorWay", "Size", "DevStyleName", "Season", "Stage", "Location", "POC", "LR", "TimeIn", "REMARKS"))
            {
                dataExcel.Load(reader);
            }

            string fileName = FileNameBuilder("INBOUD REPORT", takeAll: false);
            string file = CreateExcel(dataExcel, "InboudTemplate.xlsx", fileName, 6, false, "Sheet1");
            updateExcelFile(file, $"{DateTime.Now.ToVNString()}", 3, 10);

            return Download(file);
        }

        [HttpGet("inboundreportdata")]
        public async Task<ResponseModel<PaginationResponse<ProductModel>>> GetInboudReportData(string fromDate, string toDate, int pageItem, int pageIndex)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();
            var rspns = new ResponseModel<PaginationResponse<ProductModel>>();

            var query = _context.PRODUCT
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderByDescending(x => x.CREATED_DATE)
                .Select(x => new ProductModel()
                {
                    ID = x.PRODUCT_ID,
                    SKU = x.PRODUCT_CODE,
                    ModelName = x.Model.MODEL_NAME,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    Size = x.PRODUCT_SIZE,
                    DevStyleName = x.DEV_NAME,
                    Season = x.PRODUCT_SEASON,
                    Stage = x.PRODUCT_STAGE,
                    Location = x.PRODUCT_LOCATION,
                    POC = x.PRODUCT_POC,
                    LRStr = x.LR.GetDescription(),
                    CreatedDate = x.CREATED_DATE,
                    Remarks = x.PRODUCT_REMARKS,
                }).AsQueryable();

            return rspns.Succeed(new PaginationResponse<ProductModel>(query, pageItem, pageIndex));
        }

        [HttpGet("stockreport")]
        public async Task<IActionResult> GetStockReport(string fromDate, string toDate)
        {
            var rspns = new ResponseModel<List<ProductModel>>();
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var query = _context.PRODUCT
                .Where(x => x.CREATED_DATE <= dToDate)
                .OrderByDescending(x => x.PRODUCT_CODE)
                .ThenBy(x => x.CREATED_DATE)
                .Select(x => new ProductModel()
                {
                    ID = x.PRODUCT_ID,
                    SKU = x.PRODUCT_CODE,
                    ModelName = x.Model.MODEL_NAME,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    Size = x.PRODUCT_SIZE,
                    DevStyleName = x.DEV_NAME,
                    Season = x.PRODUCT_SEASON,
                    Stage = x.PRODUCT_STAGE,
                    Location = x.PRODUCT_LOCATION,
                    POC = x.PRODUCT_POC,
                    LRStr = x.LR.GetDescription(),
                    ProductStatus = x.PRODUCT_STATUS,
                    ReturnTime = x.TransferDetails
                        .Where(a => a.TRANSFER_TIME <= dToDate)
                        .OrderByDescending(a => a.TRANSFER_TIME)
                        .Select(x => x.RETURN_TIME)
                        .FirstOrDefault(),
                    Remarks = x.PRODUCT_REMARKS
                }).ToList();

            foreach (var item in query)
            {
                if (item.ReturnTime == null) continue;
                DateTime returnTime = (DateTime)item.ReturnTime;

                if (returnTime <= dToDate)
                {
                    item.ProductStatus = ProductStatus.Available;
                }
                else
                {
                    item.ProductStatus = ProductStatus.Transfered;
                }
            }

            DataTable dataExcel = new DataTable();
            using (var reader = ObjectReader.Create(query, "ID", "SKU", "ModelName", "Category", "ColorWay", "Size", "DevStyleName", "Season", "Stage", "Location", "POC", "LRStr", "ProductStatusStr", "Remarks"))
            {
                dataExcel.Load(reader);
            }

            string fileName = FileNameBuilder("STOCK REPORT", takeAll: false);
            string file = CreateExcel(dataExcel, "StockTemplate.xlsx", fileName, 6, false, "Sheet1");
            updateExcelFile(file, $"{DateTime.Now.ToVNString()}", 3, 10);

            return Download(file);
        }

        [HttpGet("stockreportdata")]
        public async Task<ResponseModel<PaginationResponse<ProductModel>>> GetStockReportData(string fromDate, string toDate, int pageItem, int pageIndex)
        {
            var rspns = new ResponseModel<PaginationResponse<ProductModel>>();
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var query = _context.PRODUCT
                .Include(x => x.TransferDetails)
                .Where(x => x.CREATED_DATE <= dToDate)
                 .OrderByDescending(x => x.PRODUCT_CODE)
                .ThenBy(x => x.CREATED_DATE)
                .Select(x => new ProductModel()
                {
                    ID = x.PRODUCT_ID,
                    SKU = x.PRODUCT_CODE,
                    ModelName = x.Model.MODEL_NAME,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    Size = x.PRODUCT_SIZE,
                    DevStyleName = x.DEV_NAME,
                    Season = x.PRODUCT_SEASON,
                    Stage = x.PRODUCT_STAGE,
                    Location = x.PRODUCT_LOCATION,
                    POC = x.PRODUCT_POC,
                    LRStr = x.LR.GetDescription(),
                    ProductStatus = x.PRODUCT_STATUS,
                    ReturnTime = x.TransferDetails
                        .Where(a => a.TRANSFER_TIME <= dToDate)
                        .OrderByDescending(a => a.TRANSFER_TIME)
                        .Select(x => x.RETURN_TIME)
                        .FirstOrDefault(),
                    CreatedDate = x.CREATED_DATE,
                    Remarks = x.PRODUCT_REMARKS
                }).AsQueryable();

            var result = new PaginationResponse<ProductModel>(query, pageItem, pageIndex);
            foreach(var item in result.Data)
            {
                if (item.ReturnTime == null) continue;
                DateTime returnTime = (DateTime)item.ReturnTime;

                if (returnTime <= dToDate)
                {
                    item.ProductStatus = ProductStatus.Available;
                }
                else
                {
                    item.ProductStatus = ProductStatus.Transfered;
                }
            }

            return rspns.Succeed(result);
        }

        [HttpGet("transferreport")]
        public async Task<IActionResult> GetOutboundReport(string fromDate, string toDate)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var rspns = new ResponseModel<PaginationResponse<ProductTransferModel>>();

            var query = _context.PRODUCT_TRANSFER_DTL
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderBy(x => x.CREATED_DATE)
                .Select(x => new ProductTransferModel()
                {
                    ProductId = x.PRODUCT_ID,
                    SKU = x.Product.PRODUCT_CODE,
                    ModelName = x.Product.Model.MODEL_NAME,
                    ProductCategory = x.Product.PRODUCT_CATEGORY,
                    ColorWay = x.Product.COLOR_NAME,
                    Size = x.Product.PRODUCT_SIZE,
                    EPC = x.Product.EPC,
                    TRANSFER_BY = x.TRANSFER_BY,
                    TRANSFER_REASON = x.Transfer.TRANSFER_REASON,
                    TRANSFER_TO = x.Transfer.TRANSFER_TO,
                    TRANSFER_NOTE = x.TRANSFER_NOTE,
                    TRANSFER_TIME = x.TRANSFER_TIME,
                    STATUS_STR = x.STATUS.GetDescription(),
                    RETURN_BY = x.RETURN_BY,
                    RETURN_NOTE = x.RETURN_NOTE,
                    RETURN_TIME = x.RETURN_TIME,
                }).AsQueryable();

            DataTable dataExcel = new DataTable();
            using (var reader = ObjectReader.Create(query, "ProductId", "SKU", "ModelName", "ProductCategory", "EPC", "TRANSFER_BY"
                , "TRANSFER_TO", "TRANSFER_REASON", "TRANSFER_NOTE", "TRANSFER_TIME", "STATUS_STR", "RETURN_BY", "RETURN_TIME", "RETURN_NOTE"))
            {
                dataExcel.Load(reader);
            }

            string fileName = FileNameBuilder("TRANSFER REPORT ", dFromDate.ToString("dd-MM-yyyy"), dToDate.ToString("dd-MM-yyyy"), false);
            string file = CreateExcel(dataExcel, "TransferTemplate.xlsx", fileName, 6, false, "Sheet1");
            updateExcelFile(file, $"{dFromDate.ToShortVNString()} - {dToDate.ToShortVNString()}", 3, 2);
            updateExcelFile(file, $"{DateTime.Now.ToVNString()}", 3, 10);

            return Download(file);
        }

        [HttpGet("transferreportdata")]
        public async Task<ResponseModel<PaginationResponse<ProductTransferModel>>> GetOutboundReportData(string fromDate, string toDate, int pageItem, int pageIndex)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var rspns = new ResponseModel<PaginationResponse<ProductTransferModel>>();

            var query = _context.PRODUCT_TRANSFER_DTL
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderBy(x => x.CREATED_DATE)
                .Select(x => new ProductTransferModel()
                {
                    ProductId = x.PRODUCT_ID,
                    SKU = x.Product.PRODUCT_CODE,
                    ProductCategory = x.Product.PRODUCT_CATEGORY,
                    ColorWay = x.Product.COLOR_NAME,
                    Size = x.Product.PRODUCT_SIZE,
                    EPC = x.Product.EPC,
                    ModelName = x.Product.Model.MODEL_NAME,
                    TRANSFER_BY = x.TRANSFER_BY,
                    TRANSFER_REASON = x.Transfer.TRANSFER_REASON,
                    TRANSFER_TO = x.Transfer.TRANSFER_TO,
                    TRANSFER_NOTE = x.TRANSFER_NOTE,
                    TRANSFER_TIME = x.TRANSFER_TIME,
                    STATUS = x.STATUS,
                    RETURN_BY = x.RETURN_BY,
                    RETURN_NOTE = x.RETURN_NOTE,
                    RETURN_TIME = x.RETURN_TIME,
                }).AsQueryable();

            return rspns.Succeed(new PaginationResponse<ProductTransferModel>(query, pageItem, pageIndex));
        }

        [HttpGet("inventoryreport")]
        public async Task<IActionResult> GetInventoryReport(string fromDate, string toDate)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var rspns = new ResponseModel<PaginationResponse<ProductTransferModel>>();

            var query = _context.INVENTORY
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderBy(x => x.CREATED_DATE)
                .Select(x => new InventoryModel()
                {
                    INVENTORY_ID = x.INVENTORY_ID,
                    INVENTORY_NAME = x.INVENTORY_NAME,
                    CREATED_DATE = x.CREATED_DATE.ToVNString(),
                    CREATED_USER = x.CREATED_USER,
                    TOTAL = x.InventoryDetails.Count,
                    TOTAL_FOUND = x.InventoryDetails.Where(a => a.STATUS == InventoryProductStatus.Found).Count(),
                    INVENTORY_STATUS = x.INVENTORY_STATUS.GetDescription(),
                    COMPLETE_USER = x.COMPLETE_USER,
                    COMPLETE_DATE = x.COMPLETE_DATE.ToVNString(),
                    NOTE = x.NOTE,
                }).AsQueryable();

            DataTable dataExcel = new DataTable();
            using (var reader = ObjectReader.Create(query, "INVENTORY_ID", "INVENTORY_NAME", "CREATED_DATE", "CREATED_USER", "TOTAL", "TOTAL_FOUND"
                , "INVENTORY_STATUS", "COMPLETE_USER", "COMPLETE_DATE", "NOTE"))
            {
                dataExcel.Load(reader);
            }

            string fileName = FileNameBuilder("INVENTORY REPORT ", dFromDate.ToString("dd-MM-yyyy"), dToDate.ToString("dd-MM-yyyy"), false);
            string file = CreateExcel(dataExcel, "InventoryTemplate.xlsx", fileName, 6, false, "Sheet1");
            updateExcelFile(file, $"{dFromDate.ToShortVNString()} - {dToDate.ToShortVNString()}", 3, 2);
            updateExcelFile(file, $"{DateTime.Now.ToVNString()}", 3, 10);

            return Download(file);
        }

        [HttpGet("inventoryreportdata")]
        public async Task<ResponseModel<PaginationResponse<InventoryModel>>> GetInventoryReportData(string fromDate, string toDate, int pageItem, int pageIndex)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var rspns = new ResponseModel<PaginationResponse<InventoryModel>>();

            var query = _context.INVENTORY
                 .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                 .OrderBy(x => x.CREATED_DATE)
                 .Select(x => new InventoryModel()
                 {
                     INVENTORY_ID = x.INVENTORY_ID,
                     INVENTORY_NAME = x.INVENTORY_NAME,
                     CREATED_DATE = x.CREATED_DATE.ToVNString(),
                     CREATED_USER = x.CREATED_USER,
                     TOTAL = x.InventoryDetails.Count,
                     TOTAL_FOUND = x.InventoryDetails.Where(a => a.STATUS == InventoryProductStatus.Found).Count(),
                     INVENTORY_STATUS = x.INVENTORY_STATUS.GetDescription(),
                     COMPLETE_USER = x.COMPLETE_USER,
                     COMPLETE_DATE = x.COMPLETE_DATE.ToVNString(),
                     NOTE = x.NOTE,
                 }).AsQueryable();

            return rspns.Succeed(new PaginationResponse<InventoryModel>(query, pageItem, pageIndex));
        }

        [HttpGet("warningreport")]
        public async Task<IActionResult> GetWarningReport(string fromDate, string toDate)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var rspns = new ResponseModel<PaginationResponse<ProductAlertModel>>();

            var query = _context.PRODUCT_ALERT
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderBy(x => x.CREATED_DATE)
                .Select(x => new ProductAlertModel()
                {
                    ALERT_ID = x.ALERT_ID,
                    PRODUCT_ID = x.PRODUCT_ID,
                    SKU = x.Product.PRODUCT_CODE,
                    MODEL_NAME = x.Product.Model.MODEL_NAME,
                    CATEGORY = x.Product.PRODUCT_CATEGORY,
                    EPC = x.Product.EPC,
                    WARNING_TIME = x.CREATED_DATE.ToVNString(),
                    ALERT_CONF_REASON = x.ALERT_CONF_REASON,
                    ALERT_CONF_USER = x.ALERT_CONF_USER,
                    ALERT_CONF_TIME = x.ALERT_CONF_TIME,
                    TotalWarningSecond = x.ALERT_CONF_TIME == null? 0 : (int)((DateTime)x.ALERT_CONF_TIME - x.CREATED_DATE).TotalSeconds,
                }).AsQueryable();

            DataTable dataExcel = new DataTable();
            using (var reader = ObjectReader.Create(query, "ALERT_ID", "PRODUCT_ID", "SKU", "MODEL_NAME", "CATEGORY", "EPC"
                , "WARNING_TIME", "ALERT_CONF_REASON", "ALERT_CONF_USER", "ALERT_CONF_TIME", "TotalWarningSecond"))
            {
                dataExcel.Load(reader);
            }

            string fileName = FileNameBuilder("WARNING REPORT ", dFromDate.ToString("dd-MM-yyyy"), dToDate.ToString("dd-MM-yyyy"), false);
            string file = CreateExcel(dataExcel, "AlertTemplate.xlsx", fileName, 6, false, "Sheet1");
            updateExcelFile(file, $"{dFromDate.ToShortVNString()} - {dToDate.ToShortVNString()}", 3, 2);
            updateExcelFile(file, $"{DateTime.Now.ToVNString()}", 3, 10);

            return Download(file);
        }

        [HttpGet("warningreportdata")]
        public async Task<ResponseModel<PaginationResponse<ProductAlertModel>>> GetWarningReportData(string fromDate, string toDate, int pageItem, int pageIndex)
        {
            DateTime dFromDate = fromDate.ToDateTime();
            DateTime dToDate = toDate.ToDateTime();

            var rspns = new ResponseModel<PaginationResponse<ProductAlertModel>>();

            var query = _context.PRODUCT_ALERT
                .Where(x => (x.CREATED_DATE >= dFromDate && x.CREATED_DATE <= dToDate))
                .OrderBy(x => x.CREATED_DATE)
                .Select(x => new ProductAlertModel()
                {
                    ALERT_ID = x.ALERT_ID,
                    PRODUCT_ID = x.PRODUCT_ID,
                    SKU = x.Product.PRODUCT_CODE,
                    MODEL_NAME = x.Product.Model.MODEL_NAME,
                    CATEGORY = x.Product.PRODUCT_CATEGORY,
                    EPC = x.Product.EPC,
                    WARNING_TIME = x.CREATED_DATE.ToVNString(),
                    ALERT_CONF_REASON = x.ALERT_CONF_REASON,
                    ALERT_CONF_USER = x.ALERT_CONF_USER,
                    ALERT_CONF_TIME = x.ALERT_CONF_TIME,
                    TotalWarningSecond = x.ALERT_CONF_TIME == null ? 0 : (int)((DateTime)x.ALERT_CONF_TIME - x.CREATED_DATE).TotalSeconds,
                }).AsQueryable();

            return rspns.Succeed(new PaginationResponse<ProductAlertModel>(query, pageItem, pageIndex));
        }
    }
}
