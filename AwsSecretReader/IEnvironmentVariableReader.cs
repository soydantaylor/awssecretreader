namespace AwsSecretReader
{
	/// <summary>
	/// Just an interface to wrap reading env variables for unit testing purposes
	/// </summary>
	public interface IEnvironmentVariableReader
	{
		string GetValue(string key);
	}
}