using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
//using Newtonsoft.Json;

namespace AwsSecretReader
{
	public class SecretReader
	{
		private readonly string _region;
		private const string _keyPattern = @"(?<key>[^\/]+$)";
		private readonly string _parameterPath;
		private Dictionary<string, string> Parameters { get; set; }
		private readonly IAmazonSimpleSystemsManagement _client;
		private IEnvironmentVariableReader _envreader;
		private readonly SsmInjector _injector;
		
		private static Lazy<SecretReader> _lazy = new Lazy<SecretReader>(() => new SecretReader());

		public static SecretReader Instance => _lazy.Value;
		
		/// <summary>
		/// Pulls all of the SSM parameters for a given region and path.  This package requires running with a role
		/// or that AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY are set as environment variables.
		///
		/// Additionally,
		/// DEFAULT_AWS_REGION (us-east-1 is assumed if none provided)
		/// and SSM_PARAMETER_PATH ("/" is assumed if none provided)
		/// should be set as environment variables
		/// </summary>
		public SecretReader()
		{
			_envreader = new EnvironmentVariableReader();
			_region = _envreader.GetValue("DEFAULT_AWS_REGION") ?? "us-east-1";
			_parameterPath = _envreader.GetValue("SSM_PARAMETER_PATH");

			_injector = new SsmInjector();
			
			Environment.SetEnvironmentVariable("DEFAULT_AWS_REGION", _region);
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
						Path = _parameterPath ?? "/testing",
						Recursive = true,
						WithDecryption = true
					};
					
					
					string nextToken;
					do
					{
						var result = client.GetParametersByPathAsync(req).Result;
						parameters.AddRange(result.Parameters);
						nextToken = result.NextToken;
					
					} while (!string.IsNullOrEmpty(nextToken));

					Console.WriteLine($"found {parameters.Count} params");
					foreach (var p in parameters)
					{
						
						
						var name = p.Name.Replace(_parameterPath ?? "*******", string.Empty);
						var key = Regex.Match(p.Value, _keyPattern).Groups["key"].Value;
						Console.WriteLine($"found {name}");
						var value = p.Value;
						Parameters.Add(key, value);
						
					}

					foreach (var key in Parameters.Keys)
					{
						Console.WriteLine($"{key} = {Parameters[key]}");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);//just log it on the console for now.
				
				throw;
			}
		}

		/// <summary>
		/// For testing only
		/// </summary>
		/// <param name="ssm"></param>
		/// <param name="variableReader"></param>
		/// <param name="injector"></param>
		public SecretReader(IAmazonSimpleSystemsManagement ssm, IEnvironmentVariableReader variableReader, SsmInjector injector)
		{
			Console.WriteLine("this constructor should only be used for unit testing.  It was not designed for production use.");
			_client = ssm;
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
			return !Parameters.ContainsKey(parameterName) 
				? $"{parameterName} not found in parameter dictionary check: {_parameterPath}{parameterName} is in SSM Parameter Store." 
				: Parameters[parameterName];
		}
	}
}