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
		[Fact]
		public void TestGetRealParameter()
		{
			const string expectedValue = "this is a test";
			var secretReader = SecretReader.Instance;
			var fetchedValue = secretReader.GetParameter("testingThing");
			Assert.NotNull(fetchedValue);
			Assert.Equal(expectedValue, fetchedValue);

		}
	}
}