using System.IO;

namespace GameFW.Ultility
{
    /// <summary>
    /// 文件常用方法
    /// </summary>
    public class FileTool
    {
        /// <summary>
        /// 复制文件到另一个路径
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFolder"></param>
        public static void CopyFileToDestFolder(string sourceFile, string destFolder) {
            if (!File.Exists(sourceFile))
                return;

            string[] strs = sourceFile.Split('/');
            if (strs.Length < 1)
                return;

            string destFileName = destFolder + '/' + strs[strs.Length - 1];

            if (File.Exists(destFileName))
                File.Delete(destFileName);

            File.Copy(sourceFile, destFileName);
        }
    }
}
