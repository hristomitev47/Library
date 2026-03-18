namespace WebLibrary.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public ICollection<BookGenre> BookGenres { get; set; }
    }
}
