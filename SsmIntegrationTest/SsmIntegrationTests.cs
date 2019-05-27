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
			var value = "this is a test";
			var secretReader = new SecretReader();
			var param = secretReader.GetParameter("testingThing");
			Assert.NotNull(param);
			Assert.Equal(value, param);

		}
	}
}