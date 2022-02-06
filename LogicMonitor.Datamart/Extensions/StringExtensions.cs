namespace LogicMonitor.Datamart.Extensions;

internal static class StringExtensions
{
	public static string Truncate(this string value, int maxChars)
		=> value.Length <= maxChars ? value : value[..maxChars];
}
