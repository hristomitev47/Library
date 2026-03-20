namespace WebLibrary.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public int PublishedYear { get; set; }
        public int CopiesTotal { get; set; }
        public string? ImagePath { get; set; }
        public string? Description { get; set; }
        public ICollection<BookAuthor>? BookAuthors { get; set; }
        public ICollection<BookGenre>? BookGenres { get; set; }
        public ICollection<Loan>? Loans { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
