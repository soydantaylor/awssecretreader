using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using AwsSecretReader;
using Moq;
using Xunit;

namespace SecretReaderTests
{
	public class SecretReaderTest
	{
		public int NumTimesGetParamsByPathCalled { get; set; }
		public SecretReaderTest()
		{
			NumTimesGetParamsByPathCalled = 0;
		}
		
		[Fact]
		public void TestConstructor()
		{
			const string name1 = "name1";
			const string value1 = "value1";
			const string name2 = "name2";
			const string value2 = "value2";


			var ssm = new Mock<IAmazonSimpleSystemsManagement>();
			var envReader = new Mock<IEnvironmentVariableReader>();
			var getSsmThing = new Mock<SsmInjector>();
			getSsmThing.Setup(x => x.GetSsmClient(It.IsAny<string>())).Returns(ssm.Object);
			envReader.Setup(x => x.GetValue(It.IsAny<string>())).Returns("testing");
			
			ssm.Setup(x =>
					x.GetParametersByPathAsync(It.IsAny<GetParametersByPathRequest>(), It.IsAny<CancellationToken>()))
				.Returns<GetParametersByPathRequest, CancellationToken>((request,cancellationToken) => GetParametersByPath(request));
			
			
			var secretReader = new SecretHandler(ssm.Object, envReader.Object, getSsmThing.Object);
			var thing = secretReader.GetParameter(name1);
			var thing2 = secretReader.GetParameter(name2);
			Assert.Equal(value1, thing);
			Assert.Equal(value2, thing2);

		}
		
		

		private async Task<GetParametersByPathResponse> GetParametersByPath(GetParametersByPathRequest request)
		{
			++NumTimesGetParamsByPathCalled;
			var primaryResponse = new GetParametersByPathResponse
			{
				NextToken = "some-token",
				Parameters = new List<Parameter>
				{
					new Parameter
					{
						Name = "/app/someapp/dev/thing/testing/name1",
						Value = "value1"
					}
				}
			};
			
			var secondaryResponse = new GetParametersByPathResponse
            {
            	NextToken = null,
            	Parameters = new List<Parameter>
            	{
            		new Parameter
            		{
            			Name = "/app/someapp/dev/thing/testing/name2",
            			Value = "value2"
            		}
            	}
            };

			var response = primaryResponse;
			
			if (NumTimesGetParamsByPathCalled > 1)
			{
				response = secondaryResponse;
			}
			
			
			return await Task.FromResult(response);
		}
	}
}