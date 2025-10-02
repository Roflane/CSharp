public class XCloudCore(string login) {
    private readonly string _rootDir = $"Users/{login}";
    
    public string[] DirectoryViewRoot() {
        if (string.IsNullOrEmpty(login)) return [];
        string[] dirs = Directory.GetDirectories(_rootDir);
        return dirs.Select(dir => dir.Replace($"{_rootDir}", "")).ToArray();
    }
    
    public bool DirectoryCreate(string dir) {
        string targetDir = $"{_rootDir}/{dir}";
        
        if (string.IsNullOrEmpty(dir) || 
            Directory.Exists(targetDir)) return false;
        
        Directory.CreateDirectory(targetDir);
        return true;
    }
    
    public bool DirectoryDelete(string dir) {
        string targetDir = $"{_rootDir}/{dir}";
        
        if (string.IsNullOrEmpty(dir) || 
            !Directory.Exists(targetDir)) return false;
        
        Directory.Delete(targetDir);
        return true;
    }
    
    public bool DirectoryRename(string dir, string newDirName) {
        string targetDir =  $"{_rootDir}/{dir}";
        
        if (string.IsNullOrEmpty(dir) || 
            !Directory.Exists(targetDir)) return false;

        Directory.Move(targetDir, $"{_rootDir}/{newDirName}");
        return true;
    }
    
    public async Task<bool> FileUpload(string dir, string fileName, byte[] fileBuffer) {
        try {
            string targetDir = $"{_rootDir}/{dir}/{fileName}";

            string? directory = Path.GetDirectoryName(targetDir);
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) {
                return false;
            }
            
            await File.WriteAllBytesAsync(targetDir, fileBuffer);
            return true;
        }
        catch { return false; }
    }
    
    public async Task<bool> FileDownload(string dir, byte[] fileBuffer) {
        try {
            string targetDir = $"{_rootDir}/{dir}";
            string? directory = Path.GetDirectoryName(targetDir);
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) {
                return false;
            }

            await File.WriteAllBytesAsync(targetDir, fileBuffer);
            return true;
        }
        catch { return false; }
    }
    
    public bool FileDelete(string dir) {
        string targetDir = $"{_rootDir}/{dir}";
        
        if (string.IsNullOrEmpty(targetDir) || 
            !Directory.Exists(dir)) return false;

        File.Delete(targetDir);
        return true;
    }

    public bool FileRename(string dir, string newDir) {
        string targetDir = $"{_rootDir}/{dir}";
        
        if (string.IsNullOrEmpty(targetDir) || 
            !Directory.Exists(dir)) return false;

        File.Move(dir, newDir);
        return true;
    } 
}