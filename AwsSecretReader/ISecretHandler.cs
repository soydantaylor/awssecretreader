using System.Threading.Tasks;

namespace AwsSecretReader
{
	public interface ISecretHandler
	{
		/// <summary>
		/// Finds a string parameter value at the path provided to the constructor.  If the parameter is not found, an error message will be returned.
		/// </summary>
		/// <param name="parameterName">The name of the parameter key</param>
		/// <returns>The value of the key/value pair requested by its key</returns>
		string GetParameter(string parameterName);

		/// <summary>
		/// Adds a parameter to the parameter store.  Its name will be appended to whatever is in SSM_PARAMETER_PATH
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="secure"></param>
		Task PutParameter(string name, string value, bool secure = true);
	}
}