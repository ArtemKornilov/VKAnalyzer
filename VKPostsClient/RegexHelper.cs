using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VKPostsClient
{
	public static class RegexHelper
	{
		public static int GetAge(string text)
		{
			var ageRegex = new Regex(@"(\d{2}\s(лет|год[a-ю]*))|([А-Ю]\d{2})|((девушка|парень)[,]?\s?\d{2})");

			var age = ageRegex.Match(text).Value
						.Where(m => char.IsDigit(m))
						.Aggregate(string.Empty, (res, s) => res += s);
			
			return  Convert.ToInt32(age == string.Empty ? "0" : age);
		}

		public static string GetLocations(string text)
		{
			var locationRegex = new Regex(@"((?<=#pz)|(?<=#пз))[а-яА-яa-zA-z]*");
			var locations = locationRegex.Matches(text)
						.Select(s => s.Value.Trim('_').ToLower())
						.Aggregate(string.Empty, (res, s) => res += s + " ");

			return locations;

		}
	}
}
