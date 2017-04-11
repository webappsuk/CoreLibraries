using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Text
{
    /// <summary>
    /// Class TextExtensions provides extension methods for dealing with Unicode Text.
    /// </summary>
    public static partial class TextExtensions
    {
        /// <summary>
        /// Performs a binary search on the code points collection.
        /// </summary>
        /// <typeparam name="T">The property enumeration type.</typeparam>
        /// <param name="codePoint">The code point to find.</param>
        /// <param name="codePoints">The code points collection.</param>
        /// <param name="asciiIndex">Index of the last code point range which is less than or equal to the <see cref="AsciiMaxCodePoint"/>.</param>
        /// <param name="notFound">The value to return when the code point was not found.</param>
        /// <returns>The code point property.</returns>
        private static T BinarySearchCodePoints<T>(int codePoint, [NotNull] int[] codePoints, int asciiIndex, T notFound)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            // The binary search searches between two indices.
            int minIndex = 0;
            int maxIndex = codePoints.Length - 1;

            if (codePoint <= AsciiMaxCodePoint)
                // Our first optimisation is to notice that in the majority of use cases we will be searching for ASCII characters,
                // so we can immediately strip the majority of the codePoints set.
                maxIndex = asciiIndex;
            else if (codePoint - AsciiMaxCodePoint < maxIndex - asciiIndex)
            {
                // As the ranges are ordered AND are at least length 1, then the maximum index cannot be greater than the code point itself - 
                // this is a lovely and cheap optimization for relatively low code-points.  We further improve by ignoring the first 'asciiIndex'
                // code points as we already know that codePoint doesn't lie here.
                minIndex = asciiIndex + 1;
                maxIndex = codePoint - AsciiMaxCodePoint + asciiIndex;
            }
            
            do
            {
                // Find midpoint
                int index = (minIndex + maxIndex) >> 1;

                // Calculate actual index in codePoints collection.
                int actualIndex = index * 3;
                int start = codePoints[actualIndex++];

                // Optimization to prevent lookup of end if we already match start.
                if (start == codePoint) return (T)(object)codePoints[actualIndex+1];

                // If start > codePoint then our maximum index is the index before this one.
                if (start > codePoint) maxIndex = index - 1;
                
                // If end < codePoint then our minimum index is the index after this one.
                else if (codePoints[actualIndex++] < codePoint) minIndex = index + 1;

                // If start <= codePoint && end >= codePoint, we're overlapping and we're done
                else return (T)(object)codePoints[actualIndex];
            } while (minIndex <= maxIndex);

            // Didn't find the code point.
            return notFound;
        }
    }
}
