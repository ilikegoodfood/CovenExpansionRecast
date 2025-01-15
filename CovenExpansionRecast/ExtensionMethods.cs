using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public static class ExtensionMethods
    {
        public static bool StartsWithVowel(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            string vowels = "aeiou";
            if (vowels.IndexOf(Char.ToLowerInvariant(str[0])) == -1)
            {
                return false;
            }

            return true;
        }
    }
}
