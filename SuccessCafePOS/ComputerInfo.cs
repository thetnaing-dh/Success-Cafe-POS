using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace SuccessCafePOS
{
    public static class ComputerInfo
    {
        public static string GetProductKey()
        {
            return GetHash("CPU >> " + CpuId() + "\nBIOS >> " + BiosId() + "\nBASE >> " + BaseId());
        }
        public static string GetProductID()
        {         
            return BiosId();
        }
        private static string GetHash(string s)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes = new ASCIIEncoding().GetBytes(s);
            return GetHexString(mD5CryptoServiceProvider.ComputeHash(bytes));
        }
        private static string GetHexString(byte[] bt)
        {
            string text = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte num = bt[i];
                int num2 = num & 0xF;
                int num3 = (num >> 4) & 0xF;
                text = ((num3 <= 9) ? (text + num3) : (text + (char)(num3 - 10 + 65)));
                text = ((num2 <= 9) ? (text + num2) : (text + (char)(num2 - 10 + 65)));
                if (i + 1 != bt.Length && (i + 1) % 2 == 0)
                {
                    text += "-";
                }
            }

            return text;
        }
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string text = "";
            foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
            {
                if (text == "")
                {
                    try
                    {
                        text = instance[wmiProperty].ToString();
                        return text;
                    }
                    catch
                    {
                    }
                }
            }
            return text;
        }
        private static string CpuId()
        {
            string text = identifier("Win32_Processor", "ProcessorId");

            return text;
        }
        private static string BiosId()
        {
            return identifier("Win32_ComputerSystemProduct", "UUID");
        }
        private static string BaseId()
        {
            return identifier("Win32_OperatingSystem", "SerialNumber");
        }
    }
}
