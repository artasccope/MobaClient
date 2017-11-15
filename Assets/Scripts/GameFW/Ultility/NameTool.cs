using System.Text;

namespace GameFW.Ultility
{
    /// <summary>
    /// 名字方法
    /// </summary>
    public class NameTool
    {
        /// <summary>
        /// 将bundle name和asset name合成一个string
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string GetUniqueAssetStr(string bundleName, string assetName)
        {
            return new StringBuilder(bundleName).Append(' ').Append(assetName).ToString();
        }
        /// <summary>
        /// 从合并的string中得到bundle name
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public static string GetBundleName(string prefabName)
        {
            return prefabName.Split(' ')[0];
        }

        /// <summary>
        /// 得到去掉后缀的prefab名字
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public static string GetRealName(string prefabName)
        {
            string[] nameCompose = prefabName.Split(' ', '.');
            if (nameCompose.Length > 1)
                return nameCompose[nameCompose.Length - 2];
            else
            {
                return prefabName;
            }
        }
    }
}
