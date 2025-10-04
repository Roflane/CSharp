public class XCloudCore(string login) {
    public string RootDir => $"C:/XCloud/{login}/";
    
    public bool DirectoryExists(string folder) {
        return Directory.Exists($"{RootDir}/{folder}");
    }

    public string[] DirectoryViewRoot() {
        string[] dirs = Directory.GetDirectories(RootDir);
        return dirs.Select(dir => dir.Replace($"{RootDir}", "")).ToArray();
    }
    
    public bool DirectoryCreate(string dir) {
        string targetDir = $"{RootDir}/{dir}";
        
        if (string.IsNullOrEmpty(dir) || 
            Directory.Exists(targetDir)) return false;
        
        Directory.CreateDirectory(targetDir);
        return true;
    }
    
    public bool DirectoryDelete(string dir) {
        string targetDir = $"{RootDir}/{dir}";
        
        if (string.IsNullOrEmpty(dir) || 
            !Directory.Exists(targetDir)) return false;
        
        Directory.Delete(targetDir);
        return true;
    }
    
    public bool DirectoryRename(string dir, string newDirName) {
        string targetDir =  $"{RootDir}/{dir}";
        
        if (string.IsNullOrEmpty(dir) || 
            !Directory.Exists(targetDir)) return false;

        Directory.Move(targetDir, $"{RootDir}/{newDirName}");
        return true;
    }
    
    public async Task<bool> FileUpload(string dir, string fileName, byte[] fileBuffer) {
        try {
            string targetDir = $"{RootDir}/{dir}/{fileName}";

            string? directory = Path.GetDirectoryName(targetDir);
            if (string.IsNullOrEmpty(directory)) 
                return false;
            
            await File.WriteAllBytesAsync(targetDir, fileBuffer);
            return true;
        }
        catch { return false; }
    }
    
    public async Task<bool> FileDownload(string dir, byte[] fileBuffer) {
        try {
            string targetDir = $"{RootDir}/{dir}";
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
        string targetDir = $"{RootDir}/{dir}";
        
        if (string.IsNullOrEmpty(targetDir) || 
            !Directory.Exists(dir)) return false;

        File.Delete(targetDir);
        return true;
    }

    public bool FileRename(string dir, string newDir) {
        string targetDir = $"{RootDir}/{dir}";
        
        if (string.IsNullOrEmpty(targetDir) || 
            !Directory.Exists(dir)) return false;

        File.Move(dir, newDir);
        return true;
    } 
}