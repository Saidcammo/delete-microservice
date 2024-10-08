using ListingService.Data;
using ListingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly ListingDbContext database;
        private readonly MessageService messageService;

        public ListingController(ListingDbContext database,MessageService messageService)
        {
            this.database = database;
            this.messageService = messageService;
        }

        // POST: api/listing
        [HttpPost]
        public async Task<IActionResult> PostListing([FromBody] Listing listing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var listing1 = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                UserId = listing.UserId,
                CreatedAt = DateTime.Now,
            };

            listing.Id = 0;

            database.Listings.Add(listing);
            await database.SaveChangesAsync();
            messageService.NotifyListingCreation(listing1);
            messageService.SendLoggingActions($"Ad: {listing.Title} Created by UserID: {listing.UserId}");
            return CreatedAtAction("PostListing", new { id = listing.Id }, listing);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteListing([FromBody] ListingDeleteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

           
            var listing = await database.Listings.FindAsync(request.ListingId);
            if (listing == null)
            {
                return NotFound(new { message = "Listing not found." }); 
            }

            
            database.Listings.Remove(listing);
            await database.SaveChangesAsync();

            var listingDto = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                UserId = listing.UserId,
                CreatedAt = listing.CreatedAt,
            };

            // Skicka meddelandet via RabbitMQ för att informera andra mikroservicar om raderingen
            messageService.NotifyListingDelete(listingDto);

            messageService.SendLoggingActions($"Ad: {listing.Title} Deleted by UserID: {listing.UserId}");

            return Ok(new { message = "Listing deleted successfully." });
          
        }
    }


}





public class ListingDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ListingDeleteRequest
{
    public int ListingId { get; set; }
}
