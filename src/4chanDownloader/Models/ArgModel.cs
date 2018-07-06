using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace fourchanDownloader
{
	class ArgModel
	{
		[Option('u', "url", HelpText = "Thread link to download images from; i.e. https://boards.4chan.org/b/123123123", Required = true)]
		public string Url { get; set; }

		[Option('p', "preserve", Default = true, HelpText = "Preserves original filenames instead of 4chan UNIX timestamps", Required = false)]
		public bool PreserveFileName { get; set; }

		[Option('t', "threads", Default = 4, HelpText = "Amount of threads to use for simultaneous downloading", Required = false)]
		public int Threads { get; set; }
	}
}
