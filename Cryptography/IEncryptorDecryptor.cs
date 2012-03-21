namespace WebApplications.Utilities.Cryptography
{
    public interface IEncryptorDecryptor
    {
        string Encrypt(string input);
        string Decrypt(string input, out bool isLatestKey);
        bool TryDecrypt(string input, out string result, out bool? isLatestKey);
    }
}