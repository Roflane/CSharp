namespace ZipArchiver.Managers;

public static class ArchiveManager {
    private static int _activeArchives = 0;

    public static int ActiveArchives {
        get => _activeArchives;
    }

    public static void AddArchive() {
        Interlocked.Increment(ref _activeArchives);
    }

    public static void RemoveArchive() {
        Interlocked.Decrement(ref _activeArchives);
    }
}
