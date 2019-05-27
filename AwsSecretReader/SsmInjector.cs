using System;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AwsSecretReader
{
	public class SsmInjector
	{
		public virtual IAmazonSimpleSystemsManagement GetSsmClient(string region)
		{

			var rep = RegionEndpoint.USEast1;
			switch (region)
			{
				case "us-east-2":
					rep = RegionEndpoint.USEast2;
					break;
				case "us-east-1":
					rep = RegionEndpoint.USEast1;
					break;
				case "us-west-1":
					rep = RegionEndpoint.USWest1;
					break;
				case "us-west-2":
					rep = RegionEndpoint.USWest2;
					break;
				case "ap-south-1":
					rep = RegionEndpoint.APSouth1;
					break;
				case "ap-northeast-2":
					rep = RegionEndpoint.APNortheast2;
					break;
				case "ap-southeast-1":
					rep = RegionEndpoint.APSoutheast1;
					break;
				case "ap-southeast-2":
					rep = RegionEndpoint.APSoutheast2;
					break;
				case "ap-northeast-1":
					rep = RegionEndpoint.APNortheast1;
					break;
				case "ca-central-1":
					rep = RegionEndpoint.CACentral1;
					break;
				case "cn-north-1":
					rep = RegionEndpoint.CNNorth1;
					break;
				case "cn-northwest-1":
					rep = RegionEndpoint.CNNorthWest1;
					break;
				case "eu-central-1":
					rep = RegionEndpoint.EUCentral1;
					break;
				case "eu-west-1":
					rep = RegionEndpoint.USEast1;
					break;
				case "eu-west-2":
					rep = RegionEndpoint.EUWest2;
					break;
				case "eu-west-3":
					rep = RegionEndpoint.EUWest3;
					break;
				case "sa-east-1":
					rep = RegionEndpoint.SAEast1;
					break;
				case "us-gov-east-1":
				case "us-gov-west-1":
					throw new InvalidParametersException("government regions not supported");
				default:
					throw new InvalidParametersException($"region {region} not supported");
					
			}
			
			return new AmazonSimpleSystemsManagementClient(rep);
		}
	}
}