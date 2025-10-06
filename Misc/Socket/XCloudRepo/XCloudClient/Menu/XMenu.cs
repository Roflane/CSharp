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
                 ※Delete Directory (dd)
                 ※Rename Directory (dr)
                 ※Upload File (fu)
                 ※Download File (fd)
                 ※Delete File (frm)
                 ※Rename File (fr)
                 ※Copy File (fc)
                 """ + "\n");
    }
}