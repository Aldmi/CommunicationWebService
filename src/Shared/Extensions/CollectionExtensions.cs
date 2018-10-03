using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            using (var iter = source.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var chunk = new T[size];
                    var count = 1;
                    chunk[0] = iter.Current;
                    for (int i = 1; i < size && iter.MoveNext(); i++)
                    {
                        chunk[i] = iter.Current;
                        count++;
                    }
                    if (count < size)
                    {
                        Array.Resize(ref chunk, count);
                    }
                    yield return chunk;
                }
            }
        }


        public static string ArrayByteToString(this IEnumerable<byte> source, string format)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in source)
            {
                stringBuilder.Append(item.ToString(format));
            }
            return stringBuilder.ToString();
        }
    }
}