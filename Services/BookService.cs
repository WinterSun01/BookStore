using BookStore.Models.Entities;
using BookStore.Models.ViewModels;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;
using System.Linq;

namespace BookStore.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> SearchAndFilterAsync(
            string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var books = await _bookRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                books = books.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Description.ToLower().Contains(searchTerm) ||
                    (b.Author != null && b.Author.FullName.ToLower().Contains(searchTerm)));
            }

            if (categoryId.HasValue)
                books = books.Where(b => b.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                books = books.Where(b => b.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                books = books.Where(b => b.Price <= maxPrice.Value);

            return books.OrderByDescending(b => b.CreatedAt);
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _bookRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _bookRepository.SearchAsync(searchTerm);
        }

        public async Task AddBookAsync(Book book)
        {
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BookViewModel>> GetAllBooksForAdminAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Price = b.Price,
                Stock = b.Stock,
                ImageUrl = b.ImageUrl,
                AuthorId = b.AuthorId,
                CategoryId = b.CategoryId,
                PublisherId = b.PublisherId
            }).ToList();
        }

        public async Task<BookViewModel?> GetBookForEditAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return null;

            return new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                CurrentImageUrl = book.ImageUrl,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                PublisherId = book.PublisherId
            };
        }

        public async Task AddBookAsync(BookViewModel model)
        {
            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImageUrl = model.ImageUrl,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                PublisherId = model.PublisherId,
                CreatedAt = DateTime.UtcNow
            };

            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(BookViewModel model)
        {
            var book = await _bookRepository.GetByIdAsync(model.Id);
            if (book == null) return;

            book.Title = model.Title;
            book.Description = model.Description;
            book.Price = model.Price;
            book.Stock = model.Stock;
            book.ImageUrl = model.ImageUrl;
            book.AuthorId = model.AuthorId;
            book.CategoryId = model.CategoryId;
            book.PublisherId = model.PublisherId;

            await _bookRepository.UpdateAsync(book);
        }
    }
}