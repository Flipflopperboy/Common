using System;
using System.Collections.Generic;
using System.Text;



namespace Flip.Common
{
	public static class EnumerableExtensions
	{
		public static string ToSeparatedString<T>(this IEnumerable<T> source, Func<T, string> predicate, string separator)
		{
			StringBuilder builder = new StringBuilder();
			foreach (T item in source)
			{
				string s = predicate(item);
				if (!string.IsNullOrEmpty(s))
				{
					builder.Append(s);
					builder.Append(separator);
				}
			}
			if (builder.Length > separator.Length)
			{
				builder.Remove(builder.Length - separator.Length, separator.Length);
			}
			return builder.ToString();
		}

		public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<int, T> action)
		{
			int index = 0;
			foreach (T item in source)
			{
				action(index, item);
				index++;
			}
		}
	}
}