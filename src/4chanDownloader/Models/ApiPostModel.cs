using Newtonsoft.Json;

namespace fourchanDownloader
{
	public class ApiPostModel
	{
		[JsonProperty("no", NullValueHandling = NullValueHandling.Ignore)]
		public string ID { get; set; }

		[JsonProperty("tim", NullValueHandling = NullValueHandling.Ignore)]
		public long? ImageID { get; set; } = 0;

		[JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
		public string Filename { get; set; }

		[JsonProperty("ext", NullValueHandling = NullValueHandling.Ignore)]
		public string Ext { get; set; }

		[JsonProperty("semantic_url", NullValueHandling = NullValueHandling.Ignore)]
		public string ThreadContent { get; set; }

		[JsonProperty("fsize", NullValueHandling = NullValueHandling.Ignore)]
		public int FileSize { get; set; }
	}
}
