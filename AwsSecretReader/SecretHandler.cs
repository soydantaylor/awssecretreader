using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AwsSecretReader
{
	public class SecretHandler : ISecretHandler
	{
		private readonly string _region;
		private const string _keyPattern = @"(?<key>[^\/]+$)";
		private readonly string _parameterPath;
		private Dictionary<string, string> Parameters { get; set; }
		private IEnvironmentVariableReader _envreader;
		private readonly SsmInjector _injector;

		private const string SSM_PARAM_NOT_FOUND = "";

		/// <summary>
		/// Pulls all of the SSM parameters for a given region and path.  This package requires running with a role
		/// or that AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY are set as environment variables.
		///
		/// Additionally,
		/// DEFAULT_AWS_REGION (us-east-1 is assumed if none provided)
		/// and SSM_PARAMETER_PATH ("/" is assumed if none provided)
		/// should be set as environment variables
		/// </summary>
		public SecretHandler()
		{
			_envreader = new EnvironmentVariableReader();

			_region = _envreader.GetValue("DEFAULT_AWS_REGION") ?? "us-east-1";
			_parameterPath = _envreader.GetValue("SSM_PARAMETER_PATH");
			_injector = new SsmInjector();
			Initialize();
		}

		/// <summary>
		/// Can be overridden by a mocking framework, but shouldn't be overridden for production purposes.
		/// </summary>
		private void Initialize()
		{
			var parameters = new List<Parameter>();
			Parameters = new Dictionary<string, string>();
			try
			{
				using (var client = _injector.GetSsmClient(_region))
				{
					var req = new GetParametersByPathRequest
					{
						Path = _parameterPath ?? "/",
						Recursive = true,
						WithDecryption = true
					};

					do
					{
						var result = client.GetParametersByPathAsync(req).Result;
						req.NextToken = result.NextToken;
						parameters.AddRange(result.Parameters);
					} while (!string.IsNullOrEmpty(req.NextToken));

					foreach (var p in parameters)
					{
						var key = Regex.Match(p.Name, _keyPattern).Groups["key"].Value;
						var value = p.Value;
						Parameters.Add(key, value);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"could not get params {e}"); //just log it on the console for now.
				throw;
			}
		}

		/// <summary>
		/// For testing only
		/// </summary>
		/// <param name="ssm"></param>
		/// <param name="variableReader"></param>
		/// <param name="injector"></param>
		public SecretHandler(IAmazonSimpleSystemsManagement ssm, IEnvironmentVariableReader variableReader,
			SsmInjector injector)
		{
			Console.WriteLine(
				"this constructor should only be used for unit testing.  It was not designed for production use.");
			_envreader = variableReader;
			_injector = injector;
			Initialize();
		}

		/// <summary>
		/// Finds a string parameter value at the path provided to the constructor.  If the parameter is not found, an error message will be returned.
		/// </summary>
		/// <param name="parameterName">The name of the parameter key</param>
		/// <returns>The value of the key/value pair requested by its key</returns>
		public string GetParameter(string parameterName)
		{
			var envVariable = Environment.GetEnvironmentVariable(parameterName);

			//if it's in the env variable, log it and use it
			if (!string.IsNullOrWhiteSpace(envVariable))
			{
				Console.WriteLine($"found {parameterName} in environment variable.  Overriding SSM value");
				return envVariable;
			}

			//if it's not, check SSM, and use it if it's there.
			if (Parameters.ContainsKey(parameterName))
			{
				return Parameters[parameterName];
			}

			throw new Exception(SSM_PARAM_NOT_FOUND);


			// return !Parameters.ContainsKey(parameterName) 
			// 	? $"{parameterName} not found in parameter dictionary check: {_parameterPath}/{parameterName} is in SSM Parameter Store." 
			// 	: Parameters[parameterName];
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="secure"></param>
		public async Task PutParameter(string name, string value, bool secure = true)
		{
			using (var client = _injector.GetSsmClient(_region))
			{
				var fullName = $"{_parameterPath}/{name}";
				Console.WriteLine(fullName);
				var putRequest = new PutParameterRequest
				{
					Name = fullName, Overwrite = true, Value = value,
					Type = secure ? ParameterType.SecureString : ParameterType.String
				};

				var putResponse = await client.PutParameterAsync(putRequest);
				if (putResponse.HttpStatusCode != HttpStatusCode.OK)
				{
					throw new Exception("unable to put the parameter");
				}
			}
		}
	}
}