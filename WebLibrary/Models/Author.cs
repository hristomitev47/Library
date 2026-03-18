namespace WebLibrary.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; }

        public ICollection<BookAuthor>? BookAuthors { get; set; }
    }
}
