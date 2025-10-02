namespace XCloudClient.Configs;

public static class XCloudClientConfig {
    public const int MaxFileBufferSize = 1024 * 100 * 1024;
    
    public const string Reserved = "||||";
    public const string Register = "Register";
    public const string Auth = "Auth";
    
    public const string DirectoryViewRoot = "dvr";
    public const string DirectoryCreate = "dc";
    public const string DirectoryDelete = "dd";
    public const string DirectoryRename = "dr";
    
    public const string FileUpload = "fu";
    public const string FileRename = "fr";
    public const string FileCopy = "fc";
    public const string FileDelete = "fd";
}