using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Data;
using StoreManagement.Models.Dtos;
using StoreManagement.Models.Entities;
using StoreManagement.Services;
using System.Security.Claims;

namespace StoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly StoreDbContext dbContext;
        private readonly IExcelExportService excel;

        public WarehousesController(StoreDbContext dbContext, IExcelExportService excel)
        {
            this.dbContext = dbContext;
            this.excel = excel;
        }

        //review this later
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        /* every controller have property called User => The currently authenticated user.
           and the User object contain claims , 
           find first cliam with this type 
           ! find first value could retrun null but int.parse dont accept null so we put !
         */

        // GET /api/Warehouses
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<WarehouseListDto>>> GetAllWarehouses()
        {
            var warehouses = await dbContext.Warehouses
                .Include(w => w.CreatedBy)
                .Include(w => w.Items)
                .Where(w => w.CreatedById == CurrentUserId)
                .OrderByDescending(w => w.CreatedDateAndTime)
                .Select(w => new WarehouseListDto
                {
                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.WarehouseName,
                    WarehouseDescription = w.WarehouseDescription,
                    CreatedByName = w.CreatedBy!.UserFullName,
                    CreatedDateAndTime = w.CreatedDateAndTime,
                    ItemsCount = w.Items.Count
                }).ToListAsync();
            return Ok(warehouses);
        }


        // GET /api/Warehouses/{id}
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles ="Manager")]
        public async Task<ActionResult<WarehouseListDto>> GetWarehouseById(int id)
        {
            var warehouse = await dbContext.Warehouses
                                            .Include(w => w.CreatedBy)
                                            .Include(w => w.Items)
                                            .Where(w => w.WarehouseId == id && w.CreatedById == CurrentUserId)
                                            .Select(w => new WarehouseListDto
                                            {
                                                WarehouseId = w.WarehouseId,
                                                WarehouseName= w.WarehouseName,
                                                WarehouseDescription = w.WarehouseDescription,
                                                CreatedByName = w.CreatedBy!.UserFullName,
                                                CreatedDateAndTime= w.CreatedDateAndTime,
                                                ItemsCount = w.Items.Count
                                            }).FirstOrDefaultAsync();
            return warehouse is null ? NotFound() : Ok(warehouse);
        }




        // GET /api/Warehouses/{id}/items
        [HttpGet("{id:int}/items")]//  :int is a constraint says that this value must be int
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ItemDto>> GetWarehouseItems(int id)
        {
            // this way is better than from the comented one because it loads items list data
            var warehouse = await dbContext.Warehouses
                                            .Include(w => w.Items)
                                            .FirstOrDefaultAsync(w =>
                                                w.WarehouseId == id &&
                                                w.CreatedById == CurrentUserId);
            // var warehouse = await dbContext.Warehouses.FirstOrDefaultAsync(w => w.WarehouseId == id);
            if (warehouse is null)
            {
                return NotFound();
            }

            return Ok(warehouse.Items.Select(i => new ItemDto
            {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                ItemDescription = i.ItemDescription,
                Quantity = i.Quantity,
                WarehouseId = i.WarehouseId
            }));
        }



        // POST /api/Warehouses
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<WarehouseListDto>> CreateWarehouse(CreateWarehouseDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //AnySync => does any row satsfiy this condition  ????
            var nameExists = await dbContext.Warehouses
                                    .AnyAsync(w => w.WarehouseName.ToLower() == request.WarehouseName.Trim().ToLower());

            if(nameExists)
            {
                return Conflict(new {message = "Warehouse name must be unique."});
            }

            var warehosue = new Warehouse {
                WarehouseName = request.WarehouseName,
                WarehouseDescription = request.WarehouseDescription,
                CreatedById = CurrentUserId,
                CreatedDateAndTime = DateTime.Now,
                // select ==> take every object in the list and make it to anthor object and return a list 
                // select ==> foreach(var item in request.items)
                Items = request.Items.Select(i => new Item
                {
                    ItemName = i.ItemName.Trim(),
                    ItemDescription = i.ItemDescription,
                    Quantity= i.Quantity
                }).ToList()
            };

            dbContext.Warehouses.Add(warehosue);
            await dbContext.SaveChangesAsync();
            
            var currentUser = await dbContext.Users.FindAsync(CurrentUserId);

            var warehouseDto = new WarehouseListDto { 
                WarehouseId = warehosue.WarehouseId,
                WarehouseName= warehosue.WarehouseName,
                WarehouseDescription = warehosue.WarehouseDescription,
                CreatedByName = currentUser!.UserFullName,
                CreatedDateAndTime= warehosue.CreatedDateAndTime,
                ItemsCount = warehosue.Items.Count,
            };

            return CreatedAtAction(nameof(GetWarehouseById), new { id = warehosue.WarehouseId } , warehouseDto);

        }


        // DELETE /api/Warehouses/{id}
        [HttpDelete]
        [Authorize(Roles ="Manager")]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await dbContext.Warehouses.FirstOrDefaultAsync(
                    w => w.WarehouseId == id && w.CreatedById == CurrentUserId
                );

            if (warehouse is null)
            {
                return NotFound(new { message = "there is no warehouse with this id:" + id });
            }

            var isInUse = await dbContext.SupplyDocuments.AnyAsync( sd => sd.WarehouseId == id);

            if (isInUse)
            {
                return Conflict(new { message = "Cannot delete a warehouse referenced by a existing supply document."});
            }

             dbContext.Warehouses.Remove( warehouse );
            await dbContext.SaveChangesAsync();

            return NoContent();
        }


        //this funtion should be reviwed later !!!

        // GET /api/Warehouses/export
        [HttpGet("export")]
        [Authorize( Roles ="Manager")]
        public async Task<IActionResult> ExportWarehouses()
        {
            var warehouses = await dbContext.Warehouses
                .Include(w => w.CreatedBy)
                .Include(w => w.Items)
                .Where(w => w.CreatedById == CurrentUserId)
                .OrderBy(w => w.WarehouseName)
                .ToListAsync();

            var fileBytes = excel.ExportWarehousesWithItems(warehouses);
            var fileName = $"Warehouses_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
               fileBytes,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               fileName);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // GET /api/Warehouses/dropdown
        [HttpGet("dropdown")]
        public async Task<ActionResult<List<DropListDto>>> GetWarehousesDropdown()
        {
            var warehouses = await dbContext.Warehouses
                .OrderBy(w => w.WarehouseName)
                .Select(w => new DropListDto
            {
                Id = w.WarehouseId, Name = w.WarehouseName,
            }
                ).ToListAsync();

            return Ok( warehouses );
                                
        }


        // GET /api/Warehouses/{id}/items/dropdown
        [HttpGet("{id}/items/dropdown")]
        public async Task<ActionResult<List<DropListDto>>> GetItemsDropDown(int id)
        {
            var warehouse = await dbContext.Warehouses.AnyAsync( w => w.WarehouseId == id);

            if (!warehouse)
            {
                return NotFound(new { message = "warehouse not found." });
            }

            var items = await dbContext.Items
                                .Where(i => i.WarehouseId == id)
                                .OrderBy(i => i.ItemName)
                                .Select(i => new DropListDto
                                {
                                    Id = i.ItemId,Name = i.ItemName
                                })
                                .ToListAsync();

            return Ok(items);
        }

    }
}
