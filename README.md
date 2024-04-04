# fourchanDownloader [![CodeFactor](https://www.codefactor.io/repository/github/solwynn/fourchandownloader/badge)](https://www.codefactor.io/repository/github/solwynn/fourchandownloader) 
A program I wrote in a few hours to download every image from a 4chan thread, with multi-thread support.
  
# Downloads  
[Download the latest fourchanDownloader at the Releases page](https://github.com/solwynn/fourchanDownloader/releases)

# Usage  
fourchanDownloader is a command-line tool and has 3 command-line options:  
-u, --url (Required) - the thread link to download images from  
-p, --preserve (Optional, Default: True) - whether or not to preserve original file names or use the 4chan UNIX timestamp to save files  
-t, --threads (Optional, Default: 4) - maximum amount of threads to use for simultaneous file downloading  
  
If you wanted to keep the timestamped filenames, use 5 threads for downloading, and download the thread "121221221", you would do as follows:   
```
./fourchanDownloader.exe -u https://boards.4chan.org/b/thread/121221221 -p false -t 5
```  

For basic usage, you only need to use the url parameter.  
```
./fourchanDownloader.exe -u https://boards.4chan.org/b/thread/121221221
```

# Building from Source  
Open the solution in src/ and Restore NuGet Packages, then Build as Release.

# License  
This software is released under the MIT software license.