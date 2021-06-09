namespace Subnautica_Mod_Manager
{
	internal static class FileUtils
	{
		public static void ExtractArchive(string from, string to)
		{
			if (System.IO.Path.GetExtension(from) == ".zip")
			{
				System.IO.Compression.ZipFile.ExtractToDirectory(from, to);
			}
			else if (System.IO.Path.GetExtension(from) == ".7z")
			{
				string zPath = "7za.exe"; //add to proj and set CopyToOuputDir
				try
				{
					System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
					{
						WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
						FileName = zPath,
						UseShellExecute = true,
						Arguments = $"x \"{from}\" -y -o\"{to}\""
					}).WaitForExit();
				}
				catch (System.Exception)
				{
					//handle error
				}
			}
		}
	}
}
