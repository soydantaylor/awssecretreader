using System;
using System.Collections.Generic;
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
		[Fact]
		public void TestConstructor()
		{
			var name1 = "name1";
			var value1 = "value1";
			
			
			var ssm = new Mock<IAmazonSimpleSystemsManagement>();
			var envReader = new Mock<IEnvironmentVariableReader>();
			var getSsmThing = new Mock<SsmInjector>();
			getSsmThing.Setup(x => x.GetSsmClient(It.IsAny<string>())).Returns(ssm.Object);
			envReader.Setup(x => x.GetValue(It.IsAny<string>())).Returns("testing");
			
			ssm.Setup(x =>
					x.GetParametersByPathAsync(It.IsAny<GetParametersByPathRequest>(), It.IsAny<CancellationToken>()))
				.Returns<GetParametersByPathRequest, CancellationToken>((request,cancellationToken) => GetParametersByPath(request));
			
			
			var secretReader = new SecretReader(ssm.Object, envReader.Object, getSsmThing.Object);
			var thing = secretReader.GetParameter(name1);
			Assert.Equal(value1, thing);

		}
		
		

		private async Task<GetParametersByPathResponse> GetParametersByPath(GetParametersByPathRequest request)
		{
			var getParametersByPathResponse = new GetParametersByPathResponse
			{
				NextToken = null,
				Parameters = new List<Parameter>
				{
					new Parameter
					{
						Name = "/app/someapp/dev/thing/testing/name1",
						Value = "value1"
					}
				}
			};
			return await Task.FromResult(getParametersByPathResponse);
		}
	}
}