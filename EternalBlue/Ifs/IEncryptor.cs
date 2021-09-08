using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Ifs
{
    public interface IEncryptor
    {
        public string Decrypt(string text);
        public string Encrypt(string text);
    }
}
