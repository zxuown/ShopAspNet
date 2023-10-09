using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ShopAspNet.Models.LiqPaySDK;

public class Notify
{
	[JsonPropertyName("data")]
	[JsonProperty("data")]
	public string Data{ get; set; }

	[JsonPropertyName("signature")]
	[JsonProperty("signature")]
	public string Signature { get; set; }
}
