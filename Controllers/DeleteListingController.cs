using Microsoft.AspNetCore.Mvc;

public class DeleteListingController: ControllerBase
{
    [Route("api")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly MessageService messageService;

        public ListingController(MessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpPost]
        [Route("deletewithid")]
        public IActionResult DeleteWithId([FromBody]string id)
        {
            System.Console.WriteLine("deleting " + id);
            messageService.NotifyListingDelete(id);
            messageService.SendLoggingActions("Trying to delete ad with Id: " + id);
            return Ok("Trying to delete: " + id);
        }
    }
}

