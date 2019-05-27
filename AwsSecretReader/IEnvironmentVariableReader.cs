namespace AwsSecretReader
{
	public interface IEnvironmentVariableReader
	{
		string GetValue(string key);
	}
}