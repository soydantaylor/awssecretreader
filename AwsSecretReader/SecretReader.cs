using System;
using System.Collections.Generic;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AwsSecretReader
{
	public class SecretReader : ISecretReader
	{
		private string _region => System.Environment.GetEnvironmentVariable("region") ?? "us-east-1";
		private static string Environment => System.Environment.GetEnvironmentVariable("ENVIRONMENT_PATH") ?? "develop";
		private string _serviceName => System.Environment.GetEnvironmentVariable("APPLICATION_NAME") ?? "my-app";
		private string ParameterPath =>
			System.Environment.GetEnvironmentVariable("parameter-path") ?? $"/{_serviceName}/{Environment}/settings/";
		private Dictionary<string, string> Parameters { get; set; }
		
		public SecretReader()
		{
			var parameters = new List<Parameter>();
			Parameters = new Dictionary<string, string>();
			try
			{
				using (var client = new AmazonSimpleSystemsManagementClient())
				{
					var req = new GetParametersByPathRequest
					{
						Path = ParameterPath,
						Recursive = true,
						WithDecryption = true
					};
					string nextToken;
					do
					{
						var result = client.GetParametersByPathAsync(req).Result;
						Console.WriteLine($"api result parameters count: {result.Parameters.Count}");
						parameters.AddRange(result.Parameters);
						nextToken = result.NextToken;
					
					} while (!string.IsNullOrEmpty(nextToken));

					Console.WriteLine($"{parameters.Count} found");
				
					foreach (var p in parameters)
					{
						var name = p.Name.Replace(ParameterPath, string.Empty);
						Console.WriteLine($"found {name}");
						var value = p.Value;
						Parameters.Add(name, value);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

		}
		public string GetParameter(string parameterName)
		{
			try
			{
				return Parameters[parameterName];
			}
			catch (Exception e)
			{
				throw new Exception($"{e.Message}\n{parameterName} not found in parameter dictionary check: {ParameterPath}{parameterName} is in SSM Parameter Store.");
			}
		}

		public string Region()
		{
			return _region;
		}

		public string GetEnvironment()
		{
			return Environment;
		}

		public string AppName()
		{
			return _serviceName;
		}
	}
}