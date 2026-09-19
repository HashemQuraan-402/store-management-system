using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Data;
using StoreManagement.Models.Dtos;
using StoreManagement.Models.Entities;
using System.Security.Claims;

namespace StoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplyDocumentController : ControllerBase
    {


        private int currentUserID => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private readonly StoreDbContext dbContext;

        public SupplyDocumentController(StoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private bool IsMangaer => User.IsInRole(UserType.Manager.ToString());




        // GET /api/SupplyDocument
        [HttpGet]
        public async Task<ActionResult<List<SupplyDocumentListDto>>> GetSupplyDocuments()
        {
            var query = dbContext.SupplyDocuments
                                       .Include(s => s.CreatedBy)
                                       .Include(s => s.Warehouse)
                                       .Include(s => s.Item)
                                       .AsQueryable();

            query = IsMangaer ? query.Where(s => s.Warehouse!.CreatedById == currentUserID) : query.Where(s => s.CreatedById == currentUserID );


            var documents = await query
                .OrderByDescending(s => s.CreatedDateAndTime)
                .Select(
                    s => new SupplyDocumentListDto
                    {
                        SupplyDocumentId = s.SupplyDocumentId,
                        SupplyDocumentName = s.SupplyDocumentName,
                        supplyDocumentSubject = s.SupplyDocumentSubject,
                        CreatedByName = s.CreatedBy!.UserFullName,
                        createdDate= s.CreatedDateAndTime,
                        WarehouseName = s.Warehouse!.WarehouseName,
                        ItemName = s.Item!.ItemName,
                        status = s.documentStatus.ToString()
                    }
                ).ToListAsync();
            return Ok( documents );
        }


        // GET /api/SupplyDocument/{id}
        [HttpGet("{id:int}")]
        
        public async Task<ActionResult<SupplyDocumentListDto>> GetSupplyDocumentById(int id)
        {
            var document = await dbContext.SupplyDocuments
                .Include(s => s.CreatedBy)
                .Include(s => s.Warehouse)
                .Include(s => s.Item)
                .FirstOrDefaultAsync(s => s.SupplyDocumentId == id && s.CreatedById == currentUserID);

            if( document is null)
            {
                return NotFound(new { message = "there is no Supply Document with this id."});
            }

            return Ok( new SupplyDocumentListDto
            {
                SupplyDocumentId = document.SupplyDocumentId,
                SupplyDocumentName = document.SupplyDocumentName,
                supplyDocumentSubject = document.SupplyDocumentSubject,
                CreatedByName = document.CreatedBy!.UserFullName,
                createdDate= document.CreatedDateAndTime,
                WarehouseName = document.Warehouse!.WarehouseName,
                ItemName = document.Item!.ItemName,
                status = document.documentStatus.ToString()
            } );
        }


        // POST /api/SupplyDocument
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<SupplyDocumentListDto>> CreateSupplyDocument(CreateSupplyDocumentDto request)
        {
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var item = await dbContext.Items
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.ItemId == request.ItemId && i.WarehouseId == request.WarehouseId);

            if (item is null)
            {
                return BadRequest(new { message = "the selected item does not belong to any warehouse." });
            }

            var document = new SupplyDocument
            {
                SupplyDocumentName = request.SupplyDocumentName,
                SupplyDocumentSubject = request.SupplyDocumentSubject,
                CreatedById = currentUserID,
                CreatedDateAndTime = DateTime.Now,
                WarehouseId = request.WarehouseId,
                ItemId = request.ItemId,
                documentStatus = DocumentStatus.Pending
            };

            dbContext.SupplyDocuments.Add( document );
            await dbContext.SaveChangesAsync();

            var currentUser = await dbContext.Users.FindAsync(currentUserID);

            return CreatedAtAction(nameof(GetSupplyDocumentById), new { id = document.SupplyDocumentId }, 
                new SupplyDocumentListDto { 
                    SupplyDocumentId = document.SupplyDocumentId,
                    SupplyDocumentName = document.SupplyDocumentName,
                    supplyDocumentSubject = document.SupplyDocumentSubject,
                    CreatedByName = document.CreatedBy!.UserFullName,
                    createdDate = document.CreatedDateAndTime,
                    WarehouseName = document.Warehouse!.WarehouseName,
                    ItemName = document.Item!.ItemName,
                    status = document.documentStatus.ToString()
                });


        }

        // DELETE /api/SupplyDocument/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles ="Employee")]
        public async Task<IActionResult> DeleteSupplyDocument(int id)
        {
            var document = await dbContext.SupplyDocuments.FirstOrDefaultAsync(s => s.SupplyDocumentId == id && s.CreatedById == currentUserID);
            if(document is null)
            {
                return NotFound(new {message = "there is no supply document with this id."});
            }

            if(document.documentStatus != DocumentStatus.Pending)
            {
                return Conflict(new {message = "only supply document with pending status can be deleted."});
            }

            dbContext.SupplyDocuments.Remove(document);
            await dbContext.SaveChangesAsync();

            return NoContent();

        }

        // PUT /api/SupplyDocument/{id}/approve
        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            return await SetStatus(id, DocumentStatus.Approved);
        }

        // PUT /api/SupplyDocument/{id}/decline
        [HttpPut("{id:int}/decline")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Decline(int id)
        {
            return await SetStatus(id, DocumentStatus.Declined);
        }

        // SetStatus method for put endpoint

        private async Task<IActionResult> SetStatus(int id, DocumentStatus status)
        {
            var document = await dbContext.SupplyDocuments
                .FirstOrDefaultAsync(s => s.SupplyDocumentId == id);

            if (document is null)
            {
                return NotFound(new { message = "Supply document not found." });
            }

            if(document.documentStatus != DocumentStatus.Pending)
            {
                return Conflict(new { message = "Only a pending supply document can be approved or declined." });
            }

            document.documentStatus = status;
            await dbContext.SaveChangesAsync();

            return Ok(new { message = $"Supply Document {status}", status = status.ToString()});
        }


    }
}
