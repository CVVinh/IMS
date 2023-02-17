using Microsoft.Extensions.Hosting;
using ServiceStack;

namespace BE.Helpers
{
	public static class FilesHelper
	{
		public class FileActive
		{
			public string pdf { get; set; }
			public string doc { get; set; }
		}
		public static bool IsFile(IFormFile postedFile)
		{
			var config = new ConfigurationBuilder()
			   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			   .AddJsonFile("appsettings.json").Build();
			var IamgeActive = config.GetSection(nameof(FileActive)).Get<FileActive>();
			var path = Path.GetExtension(postedFile.FileName).ToLower();
			if (path != IamgeActive.pdf && path != IamgeActive.doc)
			{
				return false;
			}
			return true;
		}
		public static string UploadFileAndReturnPath(IFormFile file, string root, string childFolder, bool saveInWwwRoot = true )
		{
			//var root = saveInWwwRoot ? _host.WebRootPath : _host.ContentRootPath;
			var filename = Path.GetFileNameWithoutExtension(file.FileName)
							+ DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff")
							+ Path.GetExtension(file.FileName);
			if (!Directory.Exists(root + childFolder))
			{
				Directory.CreateDirectory(root + childFolder);
			}
			var relativePath = childFolder + filename;
			var path = root + relativePath;
			var x = new FileStream(path, FileMode.Create);
			file.CopyTo(x);
			x.Dispose();
			GC.Collect();
			return relativePath;
		}
		public static string GetUniqueFileName(string fileName)
		{
			fileName = Path.GetFileName(fileName);
			return Path.GetFileNameWithoutExtension(fileName)
				+ "_" + Guid.NewGuid().ToString().Substring(0, 4)
				+ Path.GetExtension(fileName);
		}
	}
}
