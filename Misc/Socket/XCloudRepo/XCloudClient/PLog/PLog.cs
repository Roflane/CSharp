using XCloudClient.Enums;


public static class PLog {
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
}