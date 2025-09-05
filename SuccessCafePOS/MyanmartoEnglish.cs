using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuccessCafePOS
{
    public class MyanmartoEnglish
    {
        public string convert_text(string input)
        {
            char[] result = input.ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] >= '\u1040' && result[i] <= '\u1049')
                {
                    result[i] = (char)(result[i] - '\u1040' + '0');
                }
            }
            return new string(result);
        }
    }
}
