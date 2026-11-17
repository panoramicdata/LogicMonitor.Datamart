using System.Text.Json;

namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor Integration API objects to <see cref="IntegrationStoreItem"/>.
/// </summary>
public class IntegrationProfile : Profile
{
	private static readonly JsonSerializerOptions _serializationOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};
	/// <summary>
	/// Initializes a new instance of the <see cref="IntegrationProfile"/> class.
	/// </summary>
	public IntegrationProfile()
	{
		CreateIntegrationToIntegrationStoreItemMap();
		CreateHttpIntegrationToIntegrationStoreItemMap();
		CreateEmailIntegrationToIntegrationStoreItemMap();
		CreateAutoTaskIntegrationToIntegrationStoreItemMap();
	}

	private void CreateIntegrationToIntegrationStoreItemMap()
	{
		CreateMap<Integration, IntegrationStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.AckHeaders, dest => dest.AckMethod, dest => dest.AckOAuthVersion, dest => dest.AckOAuthGrantType,
				dest => dest.AckOAuthAccessTokenUrl, dest => dest.AckOAuthClientId, dest => dest.AckOAuthClientSecret, dest => dest.AckOAuthScope,
				dest => dest.AckPassword, dest => dest.AckPayload, dest => dest.AckPayloadFormat, dest => dest.AckUrl,
				dest => dest.AckUsername, dest => dest.AckAlertDataType, dest => dest.AlertDataType, dest => dest.ClearHeaders,
				dest => dest.ClearMethod, dest => dest.ClearOAuthVersion, dest => dest.ClearOAuthGrantType, dest => dest.ClearOAuthAccessTokenUrl,
				dest => dest.ClearOAuthClientId, dest => dest.ClearOAuthClientSecret, dest => dest.ClearOAuthScope, dest => dest.ClearPassword,
				dest => dest.ClearPayload, dest => dest.ClearPayloadFormat, dest => dest.ClearUrl, dest => dest.ClearUsername,
				dest => dest.EnabledStatus, dest => dest.Headers, dest => dest.Method, dest => dest.ParseMethod,
				dest => dest.ParseExpression, dest => dest.Payload, dest => dest.PayloadFormat, dest => dest.UpdateMethod,
				dest => dest.UpdateUrl, dest => dest.Url, dest => dest.Username, dest => dest.OAuthVersion,
				dest => dest.OAuthGrantType, dest => dest.OAuthAccessTokenUrl, dest => dest.OAuthClientId, dest => dest.OAuthClientSecret,
				dest => dest.OAuthScope, dest => dest.Password, dest => dest.UpdateHeaders, dest => dest.UpdatePassword,
				dest => dest.UpdateDataMethod, dest => dest.UpdateDataUrl, dest => dest.UpdateDataUsername, dest => dest.UpdateDataPassword,
				dest => dest.UpdateDataPayload, dest => dest.UpdateDataPayloadFormat, dest => dest.UpdateDataHeaders, dest => dest.UpdateDataAlertDataType,
				dest => dest.UpdateAlertDataType, dest => dest.ClearAlertDataType, dest => dest.UpdateDataOAuthVersion, dest => dest.UpdateDataOAuthGrantType,
				dest => dest.UpdateDataOAuthAccessTokenUrl, dest => dest.UpdateDataOAuthClientId, dest => dest.UpdateDataOAuthClientSecret, dest => dest.UpdateDataOAuthScope,
				dest => dest.UpdatePayload, dest => dest.UpdatePayloadFormat, dest => dest.UpdateUsername, dest => dest.UpdateOAuthVersion,
				dest => dest.UpdateOAuthGrantType, dest => dest.UpdateOAuthAccessTokenUrl, dest => dest.UpdateOAuthClientId, dest => dest.UpdateOAuthClientSecret,
				dest => dest.UpdateOAuthScope, dest => dest.Subject, dest => dest.Body, dest => dest.Receivers,
				dest => dest.Sender, dest => dest.Zone, dest => dest.AccountId, dest => dest.DueDateTime,
				dest => dest.QueueId, dest => dest.WarnPriority, dest => dest.ErrorPriority, dest => dest.CriticalPriority,
				dest => dest.StatusNewTicket, dest => dest.StatusUpdateTicket, dest => dest.StatusCloseTicket, dest => dest.StatusAckTicket)
			.ForMember(dest => dest.LogicMonitorId, opts => opts.MapFrom(src => src.Id));
	}

	private void CreateHttpIntegrationToIntegrationStoreItemMap()
	{
		CreateMap<HttpIntegration, IntegrationStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.Subject, dest => dest.Name, dest => dest.Body, dest => dest.Description,
				dest => dest.Receivers, dest => dest.Sender, dest => dest.Url, dest => dest.Zone,
				dest => dest.AccountId, dest => dest.DueDateTime, dest => dest.QueueId, dest => dest.WarnPriority,
				dest => dest.ErrorPriority, dest => dest.CriticalPriority, dest => dest.StatusNewTicket, dest => dest.StatusUpdateTicket,
				dest => dest.StatusCloseTicket, dest => dest.StatusAckTicket)
			.ForMember(dest => dest.LogicMonitorId, opts => opts.MapFrom(src => src.Id))
			.ForMember(dest => dest.EnabledStatus, opts => opts.MapFrom(src => JsonSerializer.Serialize(src.EnabledStatus, _serializationOptions)))
			.ForMember(dest => dest.Headers, opts => opts.MapFrom(src => JsonSerializer.Serialize(src.Headers, _serializationOptions)))
			.ForMember(dest => dest.UpdateHeaders, opts => opts.MapFrom(src => JsonSerializer.Serialize(src.UpdateHeaders, _serializationOptions)))
			.ForMember(dest => dest.UpdateDataHeaders, opts => opts.MapFrom(src => JsonSerializer.Serialize(src.UpdateDataHeaders, _serializationOptions)))
			.ForMember(dest => dest.AckHeaders, opts => opts.MapFrom(src => JsonSerializer.Serialize(src.AckHeaders, _serializationOptions)))
			.ForMember(dest => dest.ClearHeaders, opts => opts.MapFrom(src => JsonSerializer.Serialize(src.ClearHeaders, _serializationOptions)));
	}

	private void CreateEmailIntegrationToIntegrationStoreItemMap()
	{
		CreateMap<EmailIntegration, IntegrationStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.Headers, dest => dest.UpdateHeaders, dest => dest.UpdateDataHeaders, dest => dest.AckHeaders,
				dest => dest.ClearHeaders, dest => dest.AckHeaders, dest => dest.AckMethod, dest => dest.AckOAuthVersion,
				dest => dest.AckOAuthGrantType, dest => dest.AckOAuthAccessTokenUrl, dest => dest.AckOAuthClientId, dest => dest.AckOAuthClientSecret,
				dest => dest.AckOAuthScope, dest => dest.AckPassword, dest => dest.AckPayload, dest => dest.AckPayloadFormat,
				dest => dest.AckUrl, dest => dest.AckUsername, dest => dest.AckAlertDataType, dest => dest.AlertDataType,
				dest => dest.ClearHeaders, dest => dest.ClearMethod, dest => dest.ClearOAuthVersion, dest => dest.ClearOAuthGrantType,
				dest => dest.ClearOAuthAccessTokenUrl, dest => dest.ClearOAuthClientId, dest => dest.ClearOAuthClientSecret, dest => dest.ClearOAuthScope,
				dest => dest.ClearPassword, dest => dest.ClearPayload, dest => dest.ClearPayloadFormat, dest => dest.ClearUrl,
				dest => dest.ClearUsername, dest => dest.EnabledStatus, dest => dest.Headers, dest => dest.Method,
				dest => dest.ParseMethod, dest => dest.ParseExpression, dest => dest.Payload, dest => dest.PayloadFormat,
				dest => dest.UpdateMethod, dest => dest.UpdateUrl, dest => dest.Username, dest => dest.OAuthVersion,
				dest => dest.OAuthGrantType, dest => dest.OAuthAccessTokenUrl, dest => dest.OAuthClientId, dest => dest.OAuthClientSecret,
				dest => dest.OAuthScope, dest => dest.Password, dest => dest.UpdateHeaders, dest => dest.UpdatePassword,
				dest => dest.UpdateDataMethod, dest => dest.UpdateDataUrl, dest => dest.UpdateDataUsername, dest => dest.UpdateDataPassword,
				dest => dest.UpdateDataPayload, dest => dest.UpdateDataPayloadFormat, dest => dest.UpdateDataHeaders, dest => dest.UpdateDataAlertDataType,
				dest => dest.UpdateAlertDataType, dest => dest.ClearAlertDataType, dest => dest.UpdateDataOAuthVersion, dest => dest.UpdateDataOAuthGrantType,
				dest => dest.UpdateDataOAuthAccessTokenUrl, dest => dest.UpdateDataOAuthClientId, dest => dest.UpdateDataOAuthClientSecret, dest => dest.UpdateDataOAuthScope,
				dest => dest.UpdatePayload, dest => dest.UpdatePayloadFormat, dest => dest.UpdateUsername, dest => dest.UpdateOAuthVersion,
				dest => dest.UpdateOAuthGrantType, dest => dest.UpdateOAuthAccessTokenUrl, dest => dest.UpdateOAuthClientId, dest => dest.UpdateOAuthClientSecret,
				dest => dest.UpdateOAuthScope, dest => dest.Url, dest => dest.Zone, dest => dest.AccountId,
				dest => dest.DueDateTime, dest => dest.QueueId, dest => dest.WarnPriority, dest => dest.ErrorPriority,
				dest => dest.CriticalPriority, dest => dest.StatusNewTicket, dest => dest.StatusUpdateTicket, dest => dest.StatusCloseTicket,
				dest => dest.StatusAckTicket)
			.ForMember(dest => dest.LogicMonitorId, opts => opts.MapFrom(src => src.Id))
			.ForMember(dest => dest.Subject, opts => opts.MapFrom(src => src.Subject))
			.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
			.ForMember(dest => dest.Body, opts => opts.MapFrom(src => src.Body))
			.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
			.ForMember(dest => dest.Receivers, opts => opts.MapFrom(src => src.Receivers))
			.ForMember(dest => dest.Sender, opts => opts.MapFrom(src => src.Sender));
	}

	private void CreateAutoTaskIntegrationToIntegrationStoreItemMap()
	{
		CreateMap<AutoTaskIntegration, IntegrationStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.Headers, dest => dest.UpdateHeaders, dest => dest.UpdateDataHeaders, dest => dest.AckHeaders,
				dest => dest.ClearHeaders, dest => dest.Subject, dest => dest.Body, dest => dest.Receivers,
				dest => dest.Sender, dest => dest.AckHeaders, dest => dest.AckMethod, dest => dest.AckOAuthVersion,
				dest => dest.AckOAuthGrantType, dest => dest.AckOAuthAccessTokenUrl, dest => dest.AckOAuthClientId, dest => dest.AckOAuthClientSecret,
				dest => dest.AckOAuthScope, dest => dest.AckPassword, dest => dest.AckPayload, dest => dest.AckPayloadFormat,
				dest => dest.AckUrl, dest => dest.AckUsername, dest => dest.AckAlertDataType, dest => dest.AlertDataType,
				dest => dest.ClearHeaders, dest => dest.ClearMethod, dest => dest.ClearOAuthVersion, dest => dest.ClearOAuthGrantType,
				dest => dest.ClearOAuthAccessTokenUrl, dest => dest.ClearOAuthClientId, dest => dest.ClearOAuthClientSecret, dest => dest.ClearOAuthScope,
				dest => dest.ClearPassword, dest => dest.ClearPayload, dest => dest.ClearPayloadFormat, dest => dest.ClearUrl,
				dest => dest.ClearUsername, dest => dest.EnabledStatus, dest => dest.Headers, dest => dest.Method,
				dest => dest.ParseMethod, dest => dest.ParseExpression, dest => dest.Payload, dest => dest.PayloadFormat,
				dest => dest.UpdateMethod, dest => dest.UpdateUrl, dest => dest.Username, dest => dest.OAuthVersion,
				dest => dest.OAuthGrantType, dest => dest.OAuthAccessTokenUrl, dest => dest.OAuthClientId, dest => dest.OAuthClientSecret,
				dest => dest.OAuthScope, dest => dest.Password, dest => dest.UpdateHeaders, dest => dest.UpdatePassword,
				dest => dest.UpdateDataMethod, dest => dest.UpdateDataUrl, dest => dest.UpdateDataUsername, dest => dest.UpdateDataPassword,
				dest => dest.UpdateDataPayload, dest => dest.UpdateDataPayloadFormat, dest => dest.UpdateDataHeaders, dest => dest.UpdateDataAlertDataType,
				dest => dest.UpdateAlertDataType, dest => dest.ClearAlertDataType, dest => dest.UpdateDataOAuthVersion, dest => dest.UpdateDataOAuthGrantType,
				dest => dest.UpdateDataOAuthAccessTokenUrl, dest => dest.UpdateDataOAuthClientId, dest => dest.UpdateDataOAuthClientSecret, dest => dest.UpdateDataOAuthScope,
				dest => dest.UpdatePayload, dest => dest.UpdatePayloadFormat, dest => dest.UpdateUsername, dest => dest.UpdateOAuthVersion,
				dest => dest.UpdateOAuthGrantType, dest => dest.UpdateOAuthAccessTokenUrl, dest => dest.UpdateOAuthClientId, dest => dest.UpdateOAuthClientSecret,
				dest => dest.UpdateOAuthScope, dest => dest.Url, dest => dest.Zone, dest => dest.AccountId,
				dest => dest.DueDateTime, dest => dest.QueueId, dest => dest.WarnPriority, dest => dest.ErrorPriority,
				dest => dest.CriticalPriority, dest => dest.StatusNewTicket, dest => dest.StatusUpdateTicket, dest => dest.StatusCloseTicket,
				dest => dest.StatusAckTicket)
			.ForMember(dest => dest.LogicMonitorId, opts => opts.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
			.ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
			.ForMember(dest => dest.Zone, opts => opts.MapFrom(src => src.Zone))
			.ForMember(dest => dest.AccountId, opts => opts.MapFrom(src => src.AccountId))
			.ForMember(dest => dest.DueDateTime, opts => opts.MapFrom(src => src.DueDateTime))
			.ForMember(dest => dest.QueueId, opts => opts.MapFrom(src => src.QueueId))
			.ForMember(dest => dest.WarnPriority, opts => opts.MapFrom(src => src.WarnPriority))
			.ForMember(dest => dest.ErrorPriority, opts => opts.MapFrom(src => src.ErrorPriority))
			.ForMember(dest => dest.CriticalPriority, opts => opts.MapFrom(src => src.CriticalPriority))
			.ForMember(dest => dest.StatusNewTicket, opts => opts.MapFrom(src => src.StatusNewTicket))
			.ForMember(dest => dest.StatusUpdateTicket, opts => opts.MapFrom(src => src.StatusUpdateTicket))
			.ForMember(dest => dest.StatusCloseTicket, opts => opts.MapFrom(src => src.StatusCloseTicket))
			.ForMember(dest => dest.StatusAckTicket, opts => opts.MapFrom(src => src.StatusAckTicket));
	}
}