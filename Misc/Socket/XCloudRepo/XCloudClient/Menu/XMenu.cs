namespace XCloudClient.Menu;

public static class XMenu {
    public static void PrintEnterOptions() {
        Log.Blue("""
                 ---Options---
                 ※Register
                 ※Auth 
                 """ + "\n");
    }

    public static void PrintCloudOptions() {
        Log.Blue("""
                 ---Options---
                 ※View Root Directory (dvr)
                 ※Create Directory (dc)
                 ※Delete Directory (dc)
                 ※Rename Directory (dr)
                 ※Upload File (fu)
                 """ + "\n");
    }
}