using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models.Entities;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Title.Contains(searchTerm) ||
                            b.Author.FullName.Contains(searchTerm) ||
                            b.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }
    }
}