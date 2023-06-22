/*using Microsoft.AspNetCore.Mvc;
using static OnlineLibraryClient.Controllers.HomeController;

namespace OnlineLibraryClient.Views.Home
{
    public class _ResultListPartial
    {
        [HttpPost]
        public async Task<IActionResult> Rate([FromBody] RatingRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            // Extract the book ID and rating from the request
            int bookId = request.BookId;
            int rating = request.Rating;

            // TODO: Process the rating and update it in your data store or database

            // Return the updated rating as a response
            var response = new RatingRequestModel
            {
                BookId = bookId,
                Rating = rating
            };

            return Ok(response);
        }
    }
}
*/