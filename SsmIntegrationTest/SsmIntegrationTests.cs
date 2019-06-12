using System;
using AwsSecretReader;
using Xunit;
using Xunit.Abstractions;

namespace SsmIntegrationTest
{
	public class SsmIntegrationTests
	{
		private readonly ITestOutputHelper _helper;
		public SsmIntegrationTests(ITestOutputHelper helper)
		{
			_helper = helper;
		}
		[Theory]
		[InlineData("testingThing", "this is a test")]
		public void TestGetRealParameter(string paramName, string paramValue)
		{
			var secretReader = new SecretHandler();
			var fetchedValue = secretReader.GetParameter(paramName);
			Assert.NotNull(fetchedValue);
			Assert.Equal(paramValue, fetchedValue);
		}

		[Theory]
		[InlineData("testingOtherThing", "another test")]
		public async void TestPutRealParameter(string paramName, string paramValue)
		{
			var secretHandler = new SecretHandler();
			await secretHandler.PutParameter(paramName, paramValue);

			var fetchedValue = secretHandler.GetParameter(paramName);
			Assert.Equal(paramValue, fetchedValue);
		}
	}
}