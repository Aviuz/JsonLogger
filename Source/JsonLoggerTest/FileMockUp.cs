using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLoggerTest
{
    public class FileMockUp
    {
        public const string TestFilePath = "C:/temp/JsonLoggerTestingFile.txt";
        public const string TestFilePathSecondary = "C:/temp/JsonLoggerTestingFileSecondary.txt";

        public static void CleanFile(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
