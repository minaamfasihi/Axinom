namespace Axinom.DataManagement.Services.Decryption {
    public interface IDecryptor {
        string Decrypt(byte[] str);
    }
}