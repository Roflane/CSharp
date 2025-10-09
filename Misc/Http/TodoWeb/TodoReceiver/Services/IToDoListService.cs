using ToDoListWebApplication.Models;

namespace ToDoListWebApplication.Services;

//// CRUD
// CREATE - POST
// READ - GET
// UPDATE - PUT/PATCH
// DELETE - DELETE

internal interface IToDoListService
{
    Task<IEnumerable<ToDoItem>> GetAll(); // GET
    Task<ToDoItem?> GetById(int id); // GET
    Task Add(ToDoItem item); // POST
    Task Update(ToDoItem newItem); // PUT
    Task Delete(int id); // DELETE
}