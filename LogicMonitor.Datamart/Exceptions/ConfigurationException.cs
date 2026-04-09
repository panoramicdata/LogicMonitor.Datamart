namespace LogicMonitor.Datamart.Exceptions;

/// <summary>
/// Exception thrown when the datamart configuration is invalid.
/// </summary>
public class ConfigurationException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
	/// </summary>
	public ConfigurationException() : base()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigurationException"/> class with a message.
	/// </summary>
	/// <param name="message">The error message.</param>
	public ConfigurationException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigurationException"/> class with a message and inner exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="innerException">The inner exception.</param>
	public ConfigurationException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
