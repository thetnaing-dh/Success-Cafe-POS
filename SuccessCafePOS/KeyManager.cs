using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SuccessCafePOS
{
    public class KeyManager
    {
        private string EncryptionKey = string.Empty;

        public KeyManager(string encryptionKey)
        {
            EncryptionKey = encryptionKey;
        }

        public bool DisassembleKey(string ProductKey, ref KeyValuesClass KeyValues)
        {
            CspParameters cspParameters = new CspParameters();
            cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
            new RSACryptoServiceProvider(1024, cspParameters);
            try
            {
                if (string.IsNullOrEmpty(ProductKey))
                {
                    throw new ArgumentNullException("Product Key is null or empty.");
                }

                byte[] buffer = Base32Converter.FromBase32String(ProductKey);
                byte[] buffer2 = new byte[2];
                byte[] buffer3 = new byte[1];
                byte[] array = new byte[16];
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    memoryStream.Read(array, 0, 8);
                    memoryStream.Read(buffer2, 0, 1);
                    memoryStream.Read(buffer3, 0, 1);
                    memoryStream.Read(array, 8, array.Length - 8);
                    memoryStream.ToArray();
                }

                byte[] rgbKey = new byte[32]
                {
                    9, 192, 133, 135, 96, 254, 70, 21, 34, 88,
                    251, 164, 153, 21, 202, 129, 146, 199, 146, 21,
                    169, 72, 3, 36, 231, 22, 209, 188, 118, 36,
                    48, 194
                };
                byte[] rgbIV = new byte[16]
                {
                    148, 5, 58, 123, 59, 115, 65, 151, 197, 88,
                    86, 179, 206, 85, 34, 76
                };
                byte[] buffer4 = new RijndaelManaged().CreateDecryptor(rgbKey, rgbIV).TransformFinalBlock(array, 0, array.Length);
                byte[] array2 = new byte[2];
                byte[] array3 = new byte[2];
                byte[] array4 = new byte[2];
                byte[] array5 = new byte[2];
                byte[] array6 = new byte[2];
                byte[] array7 = new byte[4];
                byte[] buffer5 = new byte[2];
                byte[] array8 = new byte[2];
                using (MemoryStream memoryStream2 = new MemoryStream(buffer4))
                {
                    memoryStream2.Read(array2, 0, 1);
                    memoryStream2.Read(array3, 0, 1);
                    memoryStream2.Read(array4, 0, 1);
                    memoryStream2.Read(array5, 0, 1);
                    memoryStream2.Read(array6, 0, 1);
                    memoryStream2.Read(array7, 0, 4);
                    memoryStream2.Read(buffer5, 0, 1);
                    memoryStream2.Read(array8, 0, 1);
                }

                KeyValuesClass keyValuesClass = new KeyValuesClass();
                keyValuesClass.Header = (byte)BitConverter.ToInt16(array2, 0);
                keyValuesClass.Footer = (byte)BitConverter.ToInt16(array8, 0);
                keyValuesClass.ProductCode = (byte)BitConverter.ToInt16(array3, 0);
                keyValuesClass.Version = (byte)BitConverter.ToInt16(array4, 0);
                keyValuesClass.Edition = (Edition)BitConverter.ToInt16(array5, 0);
                keyValuesClass.Type = (byte)BitConverter.ToInt16(array6, 0);
                string text = BitConverter.ToUInt32(array7, 0).ToString().PadLeft(8, '0');
                keyValuesClass.Expiration = new DateTime(Convert.ToInt16(text.Substring(4, 4)), Convert.ToInt16(text.Substring(2, 2)), Convert.ToInt16(text.Substring(0, 2)));
                KeyValues = keyValuesClass;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidKey(ref string ProductKey)
        {
            using (new RSACryptoServiceProvider())
            {
                try
                {
                    if (string.IsNullOrEmpty(ProductKey))
                    {
                        throw new ArgumentNullException("Product Key is null or empty.");
                    }

                    byte[] buffer = Base32Converter.FromBase32String(ProductKey);
                    byte[] buffer2 = new byte[1];
                    byte[] buffer3 = new byte[1];
                    byte[] array = new byte[16];
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        memoryStream.Read(array, 0, 8);
                        memoryStream.Read(buffer2, 0, 1);
                        memoryStream.Read(buffer3, 0, 1);
                        memoryStream.Read(array, 8, array.Length - 8);
                        memoryStream.ToArray();
                    }

                    byte[] rgbKey = new byte[32]
                    {
                        9, 192, 133, 135, 96, 254, 70, 21, 34, 88,
                        251, 164, 153, 21, 202, 129, 146, 199, 146, 21,
                        169, 72, 3, 36, 231, 22, 209, 188, 118, 36,
                        48, 194
                    };
                    byte[] rgbIV = new byte[16]
                    {
                        148, 5, 58, 123, 59, 115, 65, 151, 197, 88,
                        86, 179, 206, 85, 34, 76
                    };
                    new RijndaelManaged().CreateDecryptor(rgbKey, rgbIV).TransformFinalBlock(array, 0, array.Length);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
