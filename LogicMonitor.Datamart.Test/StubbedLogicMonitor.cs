using System.Net;
using System.Text;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Replaces the HTTP layer of a client with canned responses, so that no portal is contacted.
/// </summary>
internal static class StubbedLogicMonitor
{
	public static List<string> Install(DatamartClient client, Func<string, string?> responder)
	{
		var requests = new List<string>();
		var field = typeof(LogicMonitor.Api.LogicMonitorClient)
			.GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic)
			?? throw new InvalidOperationException("LogicMonitorClient._client not found: the LogicMonitor.Api package changed shape.");

		field.SetValue(client, new HttpClient(new StubHandler(responder, requests)));
		return requests;
	}

	public static string Page(params string[] itemsJson)
		=> "{\"status\":200,\"errmsg\":\"OK\",\"data\":{\"total\":" + itemsJson.Length + ",\"items\":[" + string.Join(",", itemsJson) + "]}}";

	private sealed class StubHandler(Func<string, string?> responder, List<string> requests) : HttpMessageHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var path = request.RequestUri!.PathAndQuery;
			requests.Add(path);
			var body = responder(path);

			return Task.FromResult(body is null
				? new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("{}", Encoding.UTF8, "application/json") }
				: new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") });
		}
	}
}
