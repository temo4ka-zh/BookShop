namespace BookShop
{
    public sealed class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Author()
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public sealed class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Genre()
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public sealed class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public bool HasDiscount { get; set; }

        public Book()
        {
            Title = string.Empty;
        }
    }

    public sealed class BookGridRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string GenreName { get; set; }
        public string DiscountText { get; set; }

        public BookGridRow()
        {
            Title = string.Empty;
            AuthorName = string.Empty;
            GenreName = string.Empty;
            DiscountText = string.Empty;
        }
    }
}
