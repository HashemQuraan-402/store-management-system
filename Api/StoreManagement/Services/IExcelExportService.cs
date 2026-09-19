using StoreManagement.Models.Entities;

namespace StoreManagement.Services
{
    public interface IExcelExportService
    {
        byte[] ExportWarehousesWithItems(List<Warehouse> warehouses);
    }
}
