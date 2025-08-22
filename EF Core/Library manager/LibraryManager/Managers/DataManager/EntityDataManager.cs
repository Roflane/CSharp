using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using LibraryManager.Library;

namespace LibraryManager.Managers.DataManager;

public class EntityDataManager<T> : INotifyPropertyChanged where T : class {
    private string? _searchText;
    private LibraryContext _dbContext;
    private List<T> _entityList = new();
    private List<T> _filteredEntity = new();

    public EntityDataManager(LibraryContext dbContext, string? searchText) {
        _dbContext = dbContext;
        _searchText = searchText;
    }
    
    public List<T> EntityList => _entityList;
    
    public List<T> FilteredEntityList {
        get => _filteredEntity;
        set {
            _filteredEntity = value;
            OnPropertyChanged(nameof(FilteredEntityList));
        }
    }
    
    public void LoadEntity() {
        var dbSetProperty = _dbContext.GetType().GetProperties()
            .FirstOrDefault(p => p.PropertyType == typeof(DbSet<T>));
            
        if (dbSetProperty != null) {
            var dbSet = dbSetProperty.GetValue(_dbContext) as DbSet<T>;
            if (dbSet != null) _entityList = dbSet.ToList();
            FilterEntities();
        }
    }
    
    public void FilterEntities() {
        if (string.IsNullOrWhiteSpace(_searchText)) {
            FilteredEntityList = _entityList;
            return;
        }
        
        var stringProperties = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string));
            
        FilteredEntityList = _entityList.Where(entity =>
            stringProperties.Any(prop => {
                var value = prop.GetValue(entity) as string;
                return value != null && value.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
            })
        ).ToList();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}