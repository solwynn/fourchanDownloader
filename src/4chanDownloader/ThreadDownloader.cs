using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace fourchanDownloader
{
	public class ThreadDownloader
	{
		private string dirName;
		private bool dirInit = false;
		public bool PreserveFileName { get; }
		public Uri Url { get; }
		public int Threads;
		private static HttpClient client = new HttpClient();
		
		static ThreadDownloader()
		{
			client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("*");
			client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
			client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
		}

		public ThreadDownloader(string _Url, bool _PreserveFileName, int _Threads)
		{
			Url = new Uri(_Url);

			PreserveFileName = _PreserveFileName;
			Threads = _Threads;
		}

		private static string CleanFileName(string fileName)
		{
			return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
		}

		private void DirCreate(ApiPostModel OriginalPost)
		{
			if (!Directory.Exists("Downloads"))
			{
				Directory.CreateDirectory("Downloads");
			}

			if (!dirInit)
			{
				if (OriginalPost.ThreadContent != null)
				{
					dirName = CleanFileName(OriginalPost.ThreadContent);
					Directory.CreateDirectory($"Downloads/{dirName}");
				}
				else
				{
					Directory.CreateDirectory($"Downloads/{OriginalPost.ID}");
				}
				dirInit = true;
			}
			Console.WriteLine($"Directory initialized: {dirName}");
		}

		async private Task<string> GetThreadJson()
		{
			string content;
			Uri JsonURL = new Uri($"{Url.Scheme}://a.4cdn.org/{Url.Segments[1]}thread/{Url.Segments[3]}.json");
			HttpResponseMessage responseMsg = (await client.GetAsync(JsonURL)).EnsureSuccessStatusCode();
			content = await responseMsg.Content.ReadAsStringAsync();
			return content.Substring(9, content.Length - 9).TrimEnd('}');
		}

		async private Task<ApiPostModel[]> GetPosts()
		{
			return JsonConvert.DeserializeObject<ApiPostModel[]>(await GetThreadJson());
		}

		async public Task DownloadImage(ApiPostModel post)
		{
			string saveLoc = $"Downloads/{dirName}/{post.Filename}.{post.Ext}";
			Uri ImageUrl = new Uri($"{Url.Scheme}://is2.4chan.org/{Url.Segments[1]}{post.ImageID}{post.Ext}");
			HttpResponseMessage responseMsg;

			if (!PreserveFileName) saveLoc = $"Downloads/{dirName}/{post.ImageID}.{post.Ext}";

			try
			{
				if (!File.Exists(saveLoc))
				{
					Console.WriteLine($"Downloading: {ImageUrl}...");
					responseMsg = (await client.GetAsync(ImageUrl)).EnsureSuccessStatusCode();
					File.WriteAllBytes(saveLoc, await responseMsg.Content.ReadAsByteArrayAsync());
				}
				else
				{
					Console.WriteLine("File exists, skipping...");
				}	
			}
			catch (Exception err)
			{
				Console.WriteLine(err);
			}

		}

		async public Task DownloadThread()
		{
			ApiPostModel[] posts = await GetPosts();
			DirCreate(posts[0]);

			Parallel.ForEach(posts,
				new ParallelOptions { MaxDegreeOfParallelism = Threads },
				(_post) =>
				{
					if (_post.ImageID != 0)
					{
						DownloadImage(_post).Wait();
					}
				});
		}
	}
}
