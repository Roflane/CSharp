using XCloudClient.Configs;
using XCloudClient.Enums;


public static class PLog {
    public static bool AccoundData(string login, string password) {
        if (login.Length > XRegistrationConfig.MaxDataLength) {
            Log.Red($"Exceeded maximum length of {XRegistrationConfig.MaxDataLength} for login\n");
            return false;
        }
        if (password.Length > XRegistrationConfig.MaxDataLength + 1) {
            Log.Red($"Exceeded maximum length of {XRegistrationConfig.MaxDataLength} for password\n");
            return false;
        }
        return true;
    }
    
    public static void FileSize(EResponseCode status) {
        switch (status) {
            case EResponseCode.FileSizeOk:
                Log.Green("File upload request sent.\nPress any key to continue", true);
                break;
            case EResponseCode.FileSizeOverflow:
                Log.Red("Response: file size overflow.\nPress any key to continue", true);
                break;
        }
    }
    
    public static void FileDelete(EResponseCode status) {
        switch (status) {
            case EResponseCode.FileExists:
                Log.Green("File delete successfully.", true);
                break;
            case EResponseCode.FileNotExists:
                Log.Red("File delete unsuccessfully.", true);
                break;
        }
    }
    
    public static void FileDownload(EResponseCode status, long totalReceived, long remoteFileSize) {
        if (status == EResponseCode.FileTransferComplete && totalReceived == remoteFileSize) {
            Log.Green("File download successfully.", true);
        }
        else Log.Red(
                $"File download incomplete or failed. (Received {totalReceived}/{remoteFileSize} bytes, Code: {status})",
                true
                );
    }

    public static void FileRename(EResponseCode status) {
        if (status == EResponseCode.FileExists) {
            Log.Green("File renamed successfully.", true);
        }
        else Log.Red("File renamed unsuccessfully.", true);
    }
    
    public static void FileCopy(EResponseCode status) {
        if (status == EResponseCode.FileExists) {
            Log.Green("File copied successfully.", true);
        }
        else Log.Red("File copied unsuccessfully.", true);
    }
}