using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Reflection;

namespace ProcessingService.RetrieveLogic
{
    public class BookQueryParameters
    {
        public string? Title { get; set; } = null;
        public string? Publisher { get; set; } = null;
        public float? MinRating { get; set; } = null;
        public float? MaxRating { get; set; } = null;
        public string? IdentifierCode { get; set; } = null;
        public float? MinPrice { get; set; } = null;
        public float? MaxPrice { get; set; } = null;
        public int? MaxPageCount { get; set; } = null;
        public int? MinPageCount { get; set; } = null;
        public string? Domain { get; set; } = null;
        public string? Author { get; set; } = null;
        public DateTime? MinPublishDate { get; set; } = null;
        public DateTime? MaxPublishDate { get; set; } = null;
        public DateTime? MinRetrievedDate { get; set; } = null;
        public DateTime? MaxRetrievedDate { get; set; } = null;
        public bool WithSeveralLinks { get; set; } = false;
        public bool WithDiscount { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
       
