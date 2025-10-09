using Microsoft.EntityFrameworkCore;
using ToDoListWebApplication.Models;
using ToDoListWebApplication.Todo;

namespace ToDoListWebApplication.Services {
    internal sealed class ToDoListService : IToDoListService {
        private readonly ToDoContext _context;

        public ToDoListService(ToDoContext context) {
            _context = context;
        }

        public async Task<IEnumerable<ToDoItem>> GetAll() {
            return await _context.ToDoItems.ToListAsync();
        }

        public async Task<ToDoItem?> GetById(int id) {
            return await _context.ToDoItems.FindAsync(id);
        }

        public async Task Add(ToDoItem item) {
            _context.ToDoItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ToDoItem item) {
            _context.ToDoItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id) {
            var item = await _context.ToDoItems.FindAsync(id);
            if (item != null)
            {
                _context.ToDoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}