using Microsoft.EntityFrameworkCore;

public class DepartmentRepository
{
    private readonly InstitutionContext _context;

    public DepartmentRepository(InstitutionContext context)
    {
        _context = context;
    }

    // Задание 8: Department → Instructor
    public List<Department> GetDepartmentsWithInstructors()
    {
        return _context.Departments
            .Include(d => d.Instructors)
            .ToList();
    }

    public Department LoadDepartmentInstructorsExplicit(int departmentId)
    {
        var department = _context.Departments.Find(departmentId);
        if (department != null)
        {
            _context.Entry(department)
                .Collection(d => d.Instructors)
                .Load();
        }
        return department;
    }
}