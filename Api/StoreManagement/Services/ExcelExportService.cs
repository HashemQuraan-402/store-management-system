using StoreManagement.Models.Entities;
using ClosedXML.Excel;
using System.Drawing;

namespace StoreManagement.Services
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] ExportWarehousesWithItems(List<Warehouse> warehouses)
        {
            using var workbook = new XLWorkbook();

            // ---- Sheet 1: Warehouses ----
            var wsWarehouses = workbook.Worksheets.Add("Warehouses");
            wsWarehouses.Cell(1, 1).Value = "Warehouse Name";
            wsWarehouses.Cell(1, 2).Value = "Description";
            wsWarehouses.Cell(1, 3).Value = "Created By";
            wsWarehouses.Cell(1, 4).Value = "Created Date";
            wsWarehouses.Cell(1, 5).Value = "Items Count";
            var headerRow1 = wsWarehouses.Row(1);
            headerRow1.Style.Font.Bold = true;
            headerRow1.Style.Fill.BackgroundColor = XLColor.LightGray;

            var row = 2;
            foreach (var wh in warehouses)
            {
                wsWarehouses.Cell(row, 1).Value = wh.WarehouseName;
                wsWarehouses.Cell(row, 2).Value = wh.WarehouseDescription ?? string.Empty;
                wsWarehouses.Cell(row, 3).Value = wh.CreatedBy?.UserFullName ?? string.Empty;
                wsWarehouses.Cell(row, 4).Value = wh.CreatedDateAndTime;
                wsWarehouses.Cell(row, 4).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                wsWarehouses.Cell(row, 5).Value = wh.Items.Count;
                row++;
            }
            wsWarehouses.Columns().AdjustToContents();

            // ---- Sheet 2: Items (per warehouse) ----
            var wsItems = workbook.Worksheets.Add("Items");
            wsItems.Cell(1, 1).Value = "Warehouse Name";
            wsItems.Cell(1, 2).Value = "Item Name";
            wsItems.Cell(1, 3).Value = "Item Description";
            wsItems.Cell(1, 4).Value = "Quantity";
            var headerRow2 = wsItems.Row(1);
            headerRow2.Style.Font.Bold = true;
            headerRow2.Style.Fill.BackgroundColor = XLColor.LightGray;

            row = 2;
            foreach (var wh in warehouses)
            {
                foreach (var item in wh.Items)
                {
                    wsItems.Cell(row, 1).Value = wh.WarehouseName;
                    wsItems.Cell(row, 2).Value = item.ItemName;
                    wsItems.Cell(row, 3).Value = item.ItemDescription ?? string.Empty;
                    wsItems.Cell(row, 4).Value = item.Quantity;
                    row++;
                }
            }
            wsItems.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
