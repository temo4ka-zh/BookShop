using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookShop
{
    public sealed class DataRepository
    {
        public List<Author> Authors { get; private set; }
        public List<Genre> Genres { get; private set; }
        public List<Book> Books { get; private set; }

        public DataRepository()
        {
            Authors = new List<Author>();
            Genres = new List<Genre>();
            Books = new List<Book>();
        }

        public void Load()
        {
            Authors.Clear();
            Genres.Clear();
            Books.Clear();

            string dataDirectory = GetDataDirectory();

            Authors.AddRange(ReadSimpleList(Path.Combine(dataDirectory, "Authors.txt"))
                .Select(item => new Author { Id = item.Id, Name = item.Name }));

            Genres.AddRange(ReadSimpleList(Path.Combine(dataDirectory, "Genres.txt"))
                .Select(item => new Genre { Id = item.Id, Name = item.Name }));

            Books.AddRange(ReadBooks(Path.Combine(dataDirectory, "Books.txt")));
        }

        public string GetAuthorName(int authorId)
        {
            Author author = Authors.FirstOrDefault(item => item.Id == authorId);
            return author == null ? "Неизвестный автор" : author.Name;
        }

        public string GetGenreName(int genreId)
        {
            Genre genre = Genres.FirstOrDefault(item => item.Id == genreId);
            return genre == null ? "Неизвестный жанр" : genre.Name;
        }

        private static string GetDataDirectory()
        {
            string applicationData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (Directory.Exists(applicationData))
            {
                return applicationData;
            }

            string projectData = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (Directory.Exists(projectData))
            {
                return projectData;
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private static IEnumerable<SimpleItem> ReadSimpleList(string path)
        {
            EnsureFileExists(path);

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                int dotIndex = line.IndexOf('.');
                if (dotIndex <= 0)
                {
                    throw new FormatException("Строка '" + line + "' в файле '" + Path.GetFileName(path) + "' имеет неверный формат.");
                }

                int id = int.Parse(line.Substring(0, dotIndex).Trim());
                string name = line.Substring(dotIndex + 1).Trim();

                yield return new SimpleItem { Id = id, Name = name };
            }
        }

        private static IEnumerable<Book> ReadBooks(string path)
        {
            EnsureFileExists(path);

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                int dotIndex = line.IndexOf('.');
                if (dotIndex <= 0)
                {
                    throw new FormatException("Строка '" + line + "' в файле '" + Path.GetFileName(path) + "' имеет неверный формат.");
                }

                int id = int.Parse(line.Substring(0, dotIndex).Trim());
                string bookData = line.Substring(dotIndex + 1).Trim();
                string[] parts = bookData.Split(',');

                if (parts.Length != 4)
                {
                    throw new FormatException("Строка '" + line + "' в файле '" + Path.GetFileName(path) + "' имеет неверный формат.");
                }

                yield return new Book
                {
                    Id = id,
                    Title = parts[0].Trim(),
                    AuthorId = int.Parse(parts[1].Trim()),
                    GenreId = int.Parse(parts[2].Trim()),
                    HasDiscount = bool.Parse(parts[3].Trim())
                };
            }
        }

        private static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Не найден файл данных: " + path, path);
            }
        }

        private sealed class SimpleItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
