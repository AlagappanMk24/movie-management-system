namespace ReviewService.Domain.Enums
{
    /// <summary>
    /// Specifies categories for classifying movie reviews.
    /// </summary>
    public enum ReviewCategory
    {
        /// <summary>Casual watch suitable for passing time.</summary>
        Timepass = 1,

        /// <summary>Not recommended; can be skipped.</summary>
        Skip = 2,

        /// <summary>Recommended watch; definitely go for it.</summary>
        GoForIt = 3,

        /// <summary>Exceptional masterclass; absolute perfection.</summary>
        Perfection = 4
    }
}