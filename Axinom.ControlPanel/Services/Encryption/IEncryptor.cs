namespace Axinom.ControlPanel.Services.Encryption {
    public interface IEncryptor {
        byte[] Encrypt(string str);
    }
}