namespace AwsSecretReader
{
	public interface ISecretReader
	{
		/// <summary>
		/// Gets a paremeter value based on key name and the values that are set for environment and application name
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		string GetParameter(string parameterName);
		/// <summary>
		/// region to seek the ssm key in.  if the "region" environment variable is set, it will use that.  If it is absent it will default to "us-east-1"
		/// This is useful for using other service SDKs like SQS, etc. 
		/// </summary>
		/// <returns></returns>
		string Region();
		/// <summary>
		/// This is intended to be the second item in your SSM path
		/// /app-name/enviornment/settings/variable-name
		/// </summary>
		/// <returns>
		/// the environment variable that is set in "ENVIRONMENT_PATH"
		/// defaults to "develop"
		/// </returns>
		string GetEnvironment();
		
		/// <summary>
		/// This is intended to be the first part of your SSM key.
		/// /app-name/environment/settings/variable-name
		/// </summary>
		/// <returns>
		/// the environment variable stored in APPLICATION_NAME
		/// defaults to "my-app"
		/// </returns>
		string AppName();
	}
}