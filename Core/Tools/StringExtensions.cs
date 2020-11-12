using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Tools
{
    public static class StringExtensions
    {
        public static string GetSubstring(this string originalString, string firstPoint, string SecondPoint)
        {
            int firstPointIndex = originalString.IndexOf(firstPoint) + firstPoint.Length;
            return originalString.Substring(firstPointIndex, originalString.IndexOf(SecondPoint) - firstPointIndex);
        }
    }
}