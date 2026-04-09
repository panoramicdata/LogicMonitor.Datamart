namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor integration (e.g. webhook, email, ticketing) stored in the datamart.
/// </summary>
public class IntegrationStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// The integration type (e.g. HTTP, Email, ServiceNow).
	/// </summary>
	public string Type { get; set; } = string.Empty;

	/// <summary>
	/// Extra configuration data for the integration.
	/// </summary>
	public string Extra { get; set; } = string.Empty;

	/// <summary>
	/// HTTP headers for acknowledgement requests.
	/// </summary>
	public string? AckHeaders { get; set; }

	/// <summary>
	/// HTTP method for acknowledgement requests.
	/// </summary>
	public string? AckMethod { get; set; }

	/// <summary>
	/// OAuth version for acknowledgement authentication.
	/// </summary>
	public string? AckOAuthVersion { get; set; }

	/// <summary>
	/// OAuth grant type for acknowledgement authentication.
	/// </summary>
	public string? AckOAuthGrantType { get; set; }

	/// <summary>
	/// OAuth access token URL for acknowledgement authentication.
	/// </summary>
	public string? AckOAuthAccessTokenUrl { get; set; }

	/// <summary>
	/// OAuth client identifier for acknowledgement authentication.
	/// </summary>
	public string? AckOAuthClientId { get; set; }

	/// <summary>
	/// OAuth client secret for acknowledgement authentication.
	/// </summary>
	public string? AckOAuthClientSecret { get; set; }

	/// <summary>
	/// OAuth scope for acknowledgement authentication.
	/// </summary>
	public string? AckOAuthScope { get; set; }

	/// <summary>
	/// Password for acknowledgement requests.
	/// </summary>
	public string? AckPassword { get; set; }

	/// <summary>
	/// Payload template for acknowledgement requests.
	/// </summary>
	public string? AckPayload { get; set; }

	/// <summary>
	/// Payload format for acknowledgement requests (e.g. JSON, XML).
	/// </summary>
	public string? AckPayloadFormat { get; set; }

	/// <summary>
	/// URL for acknowledgement requests.
	/// </summary>
	public string? AckUrl { get; set; }

	/// <summary>
	/// Username for acknowledgement requests.
	/// </summary>
	public string? AckUsername { get; set; }

	/// <summary>
	/// The alert data type for acknowledgement requests.
	/// </summary>
	public string? AckAlertDataType { get; set; }

	/// <summary>
	/// The alert data type for the primary notification.
	/// </summary>
	public string? AlertDataType { get; set; }

	/// <summary>
	/// HTTP headers for clear requests.
	/// </summary>
	public string? ClearHeaders { get; set; }

	/// <summary>
	/// HTTP method for clear requests.
	/// </summary>
	public string? ClearMethod { get; set; }

	/// <summary>
	/// OAuth version for clear authentication.
	/// </summary>
	public string? ClearOAuthVersion { get; set; }

	/// <summary>
	/// OAuth grant type for clear authentication.
	/// </summary>
	public string? ClearOAuthGrantType { get; set; }

	/// <summary>
	/// OAuth access token URL for clear authentication.
	/// </summary>
	public string? ClearOAuthAccessTokenUrl { get; set; }

	/// <summary>
	/// OAuth client identifier for clear authentication.
	/// </summary>
	public string? ClearOAuthClientId { get; set; }

	/// <summary>
	/// OAuth client secret for clear authentication.
	/// </summary>
	public string? ClearOAuthClientSecret { get; set; }

	/// <summary>
	/// OAuth scope for clear authentication.
	/// </summary>
	public string? ClearOAuthScope { get; set; }

	/// <summary>
	/// Password for clear requests.
	/// </summary>
	public string? ClearPassword { get; set; }

	/// <summary>
	/// Payload template for clear requests.
	/// </summary>
	public string? ClearPayload { get; set; }

	/// <summary>
	/// Payload format for clear requests.
	/// </summary>
	public string? ClearPayloadFormat { get; set; }

	/// <summary>
	/// URL for clear requests.
	/// </summary>
	public string? ClearUrl { get; set; }

	/// <summary>
	/// Username for clear requests.
	/// </summary>
	public string? ClearUsername { get; set; }

	/// <summary>
	/// The enabled status of the integration.
	/// </summary>
	public string? EnabledStatus { get; set; }

	/// <summary>
	/// HTTP headers for the primary notification request.
	/// </summary>
	public string? Headers { get; set; }

	/// <summary>
	/// HTTP method for the primary notification request.
	/// </summary>
	public string? Method { get; set; }

	/// <summary>
	/// The method used to parse the response.
	/// </summary>
	public string? ParseMethod { get; set; }

	/// <summary>
	/// The expression used to parse the response.
	/// </summary>
	public string? ParseExpression { get; set; }

	/// <summary>
	/// Payload template for the primary notification request.
	/// </summary>
	public string? Payload { get; set; }

	/// <summary>
	/// Payload format for the primary notification request.
	/// </summary>
	public string? PayloadFormat { get; set; }

	/// <summary>
	/// HTTP method for update requests.
	/// </summary>
	public string? UpdateMethod { get; set; }

	/// <summary>
	/// URL for update requests.
	/// </summary>
	public string? UpdateUrl { get; set; }

	/// <summary>
	/// URL for the primary notification request.
	/// </summary>
	public string? Url { get; set; }

	/// <summary>
	/// Username for the primary notification request.
	/// </summary>
	public string? Username { get; set; }

	/// <summary>
	/// OAuth version for primary authentication.
	/// </summary>
	public string? OAuthVersion { get; set; }

	/// <summary>
	/// OAuth grant type for primary authentication.
	/// </summary>
	public string? OAuthGrantType { get; set; }

	/// <summary>
	/// OAuth access token URL for primary authentication.
	/// </summary>
	public string? OAuthAccessTokenUrl { get; set; }

	/// <summary>
	/// OAuth client identifier for primary authentication.
	/// </summary>
	public string? OAuthClientId { get; set; }

	/// <summary>
	/// OAuth client secret for primary authentication.
	/// </summary>
	public string? OAuthClientSecret { get; set; }

	/// <summary>
	/// OAuth scope for primary authentication.
	/// </summary>
	public string? OAuthScope { get; set; }

	/// <summary>
	/// Password for the primary notification request.
	/// </summary>
	public string? Password { get; set; }

	/// <summary>
	/// HTTP headers for update requests.
	/// </summary>
	public string? UpdateHeaders { get; set; }

	/// <summary>
	/// Password for update requests.
	/// </summary>
	public string? UpdatePassword { get; set; }

	/// <summary>
	/// HTTP method for update-data requests.
	/// </summary>
	public string? UpdateDataMethod { get; set; }

	/// <summary>
	/// URL for update-data requests.
	/// </summary>
	public string? UpdateDataUrl { get; set; }

	/// <summary>
	/// Username for update-data requests.
	/// </summary>
	public string? UpdateDataUsername { get; set; }

	/// <summary>
	/// Password for update-data requests.
	/// </summary>
	public string? UpdateDataPassword { get; set; }

	/// <summary>
	/// Payload template for update-data requests.
	/// </summary>
	public string? UpdateDataPayload { get; set; }

	/// <summary>
	/// Payload format for update-data requests.
	/// </summary>
	public string? UpdateDataPayloadFormat { get; set; }

	/// <summary>
	/// HTTP headers for update-data requests.
	/// </summary>
	public string? UpdateDataHeaders { get; set; }

	/// <summary>
	/// Alert data type for update-data requests.
	/// </summary>
	public string? UpdateDataAlertDataType { get; set; }

	/// <summary>
	/// Alert data type for update requests.
	/// </summary>
	public string? UpdateAlertDataType { get; set; }

	/// <summary>
	/// Alert data type for clear requests.
	/// </summary>
	public string? ClearAlertDataType { get; set; }

	/// <summary>
	/// OAuth version for update-data authentication.
	/// </summary>
	public string? UpdateDataOAuthVersion { get; set; }

	/// <summary>
	/// OAuth grant type for update-data authentication.
	/// </summary>
	public string? UpdateDataOAuthGrantType { get; set; }

	/// <summary>
	/// OAuth access token URL for update-data authentication.
	/// </summary>
	public string? UpdateDataOAuthAccessTokenUrl { get; set; }

	/// <summary>
	/// OAuth client identifier for update-data authentication.
	/// </summary>
	public string? UpdateDataOAuthClientId { get; set; }

	/// <summary>
	/// OAuth client secret for update-data authentication.
	/// </summary>
	public string? UpdateDataOAuthClientSecret { get; set; }

	/// <summary>
	/// OAuth scope for update-data authentication.
	/// </summary>
	public string? UpdateDataOAuthScope { get; set; }

	/// <summary>
	/// Payload template for update requests.
	/// </summary>
	public string? UpdatePayload { get; set; }

	/// <summary>
	/// Payload format for update requests.
	/// </summary>
	public string? UpdatePayloadFormat { get; set; }

	/// <summary>
	/// Username for update requests.
	/// </summary>
	public string? UpdateUsername { get; set; }

	/// <summary>
	/// OAuth version for update authentication.
	/// </summary>
	public string? UpdateOAuthVersion { get; set; }

	/// <summary>
	/// OAuth grant type for update authentication.
	/// </summary>
	public string? UpdateOAuthGrantType { get; set; }

	/// <summary>
	/// OAuth access token URL for update authentication.
	/// </summary>
	public string? UpdateOAuthAccessTokenUrl { get; set; }

	/// <summary>
	/// OAuth client identifier for update authentication.
	/// </summary>
	public string? UpdateOAuthClientId { get; set; }

	/// <summary>
	/// OAuth client secret for update authentication.
	/// </summary>
	public string? UpdateOAuthClientSecret { get; set; }

	/// <summary>
	/// OAuth scope for update authentication.
	/// </summary>
	public string? UpdateOAuthScope { get; set; }

	/// <summary>
	/// The email subject for email-type integrations.
	/// </summary>
	public string? Subject { get; set; }

	/// <summary>
	/// The name of the integration.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// The email body for email-type integrations.
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// A description of the integration.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// The notification receivers for the integration.
	/// </summary>
	public string? Receivers { get; set; }

	/// <summary>
	/// The notification sender for the integration.
	/// </summary>
	public string? Sender { get; set; }

	/// <summary>
	/// The time zone identifier for the integration.
	/// </summary>
	public int? Zone { get; set; }

	/// <summary>
	/// The LogicMonitor account identifier.
	/// </summary>
	public int? AccountId { get; set; }

	/// <summary>
	/// The due date/time for ticketing integrations.
	/// </summary>
	public string? DueDateTime { get; set; }

	/// <summary>
	/// The queue identifier for ticketing integrations.
	/// </summary>
	public int? QueueId { get; set; }

	/// <summary>
	/// The ticket priority for warning-level alerts.
	/// </summary>
	public int? WarnPriority { get; set; }

	/// <summary>
	/// The ticket priority for error-level alerts.
	/// </summary>
	public int? ErrorPriority { get; set; }

	/// <summary>
	/// The ticket priority for critical-level alerts.
	/// </summary>
	public int? CriticalPriority { get; set; }

	/// <summary>
	/// The ticket status to set when a new ticket is created.
	/// </summary>
	public int? StatusNewTicket { get; set; }

	/// <summary>
	/// The ticket status to set when a ticket is updated.
	/// </summary>
	public int? StatusUpdateTicket { get; set; }

	/// <summary>
	/// The ticket status to set when a ticket is closed.
	/// </summary>
	public int? StatusCloseTicket { get; set; }

	/// <summary>
	/// The ticket status to set when an alert is acknowledged.
	/// </summary>
	public int? StatusAckTicket { get; set; }
}
