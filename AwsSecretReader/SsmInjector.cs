using System;
using Amazon;
using Amazon.Internal;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AwsSecretReader
{
	/// <summary>
	/// Just useful for testing.  Not needed for production.
	/// </summary>
	public class SsmInjector
	{
		public virtual IAmazonSimpleSystemsManagement GetSsmClient(string region)
		{
			return new AmazonSimpleSystemsManagementClient(RegionEndpoint.GetBySystemName(region));
		}
	}
}