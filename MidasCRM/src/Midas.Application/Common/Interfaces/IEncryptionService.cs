using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
