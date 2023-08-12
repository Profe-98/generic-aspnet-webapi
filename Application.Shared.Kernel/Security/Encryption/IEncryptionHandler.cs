using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Application.Shared.Kernel.Security.Encryption
{
    public interface IEncryptionHandler
    {
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes);
        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes);
        public string EncryptText(string input, string password);
        public string DecryptText(string input, string password);
        public string CreateRandomKey(int length);
        public string MD5(string str);
        public Task<string> MD5Async(string str);
        public Task<string> MD5Async(Stream stream);
        public string SHA256(string str);
    }


    public interface ISingletonEncryptionHandler : IEncryptionHandler
    {

    }

    public interface IScopedEncryptionHandler : IEncryptionHandler
    {

    }
}
