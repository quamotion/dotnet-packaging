using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.IO
{
    internal class FileNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null && y != null)
            {
                return -1;
            }

            if (y == null && x != null)
            {
                return 1;
            }

            for (int i = 0; i < x.Length; i++)
            {
                if (i >= y.Length)
                {
                    return 1;
                }

                var xChar = InverseLowerAndUpper(x[i]);
                var yChar = InverseLowerAndUpper(y[i]);

                // Swap lowercase & upper case

                if (xChar != yChar)
                {
                    return xChar.CompareTo(yChar);
                }
            }

            return 0;
        }

        private byte InverseLowerAndUpper(char c)
        {/*
            if (char.IsLower(c))
            {
                return (byte)char.ToUpper(c);
            }

            if (char.IsUpper(c))
            {
                return (byte)char.ToLower(c);
            }*/

            return (byte)c;
        }
    }
}
