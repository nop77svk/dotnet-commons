namespace NoP77svk.Text
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class StringExt
    {
        public static int IndexOfWhiteSpace(this string self, int startIndex)
        {
            int result = -1;

            for (int ixWhiteSpace = startIndex; ixWhiteSpace < self.Length; ixWhiteSpace++)
            {
                if (char.IsWhiteSpace(self[ixWhiteSpace]))
                {
                    result = ixWhiteSpace;
                    break;
                }
            }

            return result;
        }

        public static int IndexOfWhiteSpace(this string self)
        {
            return self.IndexOfWhiteSpace(0);
        }

        public static int LastIndexOfWhiteSpace(this string self, int startIndex)
        {
            int result = -1;

            for (int ixWhiteSpace = startIndex; ixWhiteSpace >= 0; ixWhiteSpace--)
            {
                if (char.IsWhiteSpace(self[ixWhiteSpace]))
                {
                    result = ixWhiteSpace;
                    break;
                }
            }

            return result;
        }

        public static int LastIndexOfWhiteSpace(this string self)
        {
            return self.LastIndexOfWhiteSpace(self.Length - 1);
        }
    }
}
