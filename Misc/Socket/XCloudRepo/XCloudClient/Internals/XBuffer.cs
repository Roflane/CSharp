using XCloudClient.Enums;

namespace XCloudClient.Internals;

public class XBuffer {
    public byte[] StatusBuffer { get; set; } = new byte[sizeof(EResponseCode)];
}