using XCloudRepo.Configs;

public static class XRegValidator {
    public static EDataStatus CheckData(string login, string password) {
        if (login.Length > XRegistrationConfig.MaxLoginLength) {
            return EDataStatus.LoginLengthExceeded;
        }
        if (password.Length > XRegistrationConfig.MaxPasswordLength) {
            return EDataStatus.LoginLengthExceeded;
        }
        
        return EDataStatus.Success;
    }
}