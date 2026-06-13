using System.ComponentModel.DataAnnotations;

namespace movie_watchlist_blazor.Models
{
    public class Review
    {
        // Database model that specifies the different info that is needed if a user is wanting to leave a review
        // On a movie that they've specified.
        [Key]
        public long ReviewId { get; set; }
        [Required]
        [StringLength(1000)]
        public string ReviewTitle { get; set; } = string.Empty;
        [Required]
        [StringLength(5000)]
        public string ReviewContent { get; set; } = string.Empty;
        [Required]
        public int MovieRating { get; set; }

        [Required]
        public int MovieId {get; set; }


        // Reviewer/User Details
        [Required]
        public int CreatedBy {get; set;}
        public int? ModifiedBy { get; set; }

        // Posting Dates
        [Required]
        public DateTime CreateDate {get; set;} = DateTime.UtcNow;
        public DateTime ModifyDate {get; set;} = DateTime.UtcNow;

        //An option to not delete reviews but keep them on file and filter them out
        public bool IsDeleted {get; set;} 
    }
}