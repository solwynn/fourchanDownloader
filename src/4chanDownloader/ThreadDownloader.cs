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
		private static HttpClient Client = new HttpClient();
		private string DirName;
		private bool DirInit = false;
		public bool PreserveFileName { get; }
		public int Threads;
		public Uri Url { get; }

		// thank you to Michael Minton (https://stackoverflow.com/users/1488979/michael-minton) from stackoverflow
		// https://stackoverflow.com/questions/146134/how-to-remove-illegal-characters-from-path-and-filenames
		private static string CleanFileName(string fileName)
		{
			return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
		}

		static ThreadDownloader()
		{
			Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("application/json");
			Client.DefaultRequestHeaders.UserAgent.ParseAdd("fcD (https://github.com/willowsnow/fourchanDownloader/)");
		}

		public ThreadDownloader(string url, bool preserveFileName, int threads)
		{
			PreserveFileName = preserveFileName;
			Threads = threads;
			Url = new Uri(url);
		}

		private void DirCreate(ApiPostModel originalPost)
		{
			if (!Directory.Exists("Downloads"))
			{
				Directory.CreateDirectory("Downloads");
			}

			if (!DirInit)
			{
				if (originalPost.ThreadContent != null)
				{
					DirName = CleanFileName(originalPost.ThreadContent);
					Directory.CreateDirectory($"Downloads/{DirName}");
				}
				else
				{
					Directory.CreateDirectory($"Downloads/{originalPost.ID}");
				}

				DirInit = true;
			}

			Console.WriteLine($"Directory initialized: {DirName}");
		}

		async private Task<string> GetThreadJson()
		{
			Uri JsonURL = new Uri($"{Url.Scheme}://a.4cdn.org/{Url.Segments[1]}thread/{Url.Segments[3]}.json");
			HttpResponseMessage responseMsg = (await Client.GetAsync(JsonURL)).EnsureSuccessStatusCode();
			string content = await responseMsg.Content.ReadAsStringAsync();
			return content.Substring(9, content.Length - 9).TrimEnd('}');
		}

		async private Task<ApiPostModel[]> GetPosts()
		{
			return JsonConvert.DeserializeObject<ApiPostModel[]>(await GetThreadJson());
		}

		async public Task DownloadImage(ApiPostModel post)
		{
			Uri ImageUrl = new Uri($"{Url.Scheme}://is2.4chan.org/{Url.Segments[1]}{post.ImageID}{post.Ext}");
			HttpResponseMessage responseMsg;
			string saveLoc = $"Downloads/{DirName}/{post.Filename}.{post.Ext}";

			if (!PreserveFileName) saveLoc = $"Downloads/{DirName}/{post.ImageID}.{post.Ext}";

			try
			{
				if (!File.Exists(saveLoc))
				{
					Console.WriteLine($"Downloading: {ImageUrl}...");
					responseMsg = (await Client.GetAsync(ImageUrl)).EnsureSuccessStatusCode();
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
