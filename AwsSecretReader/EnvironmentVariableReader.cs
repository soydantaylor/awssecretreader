using System;

namespace AwsSecretReader
{
	public class EnvironmentVariableReader : IEnvironmentVariableReader
	{
		public string GetValue(string key)
		{
			return Environment.GetEnvironmentVariable(key);
		}
	}
}