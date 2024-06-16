using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine.Networking;

namespace BundleMaster
{
#if BMWebGL
    public static partial class AssetComponent
    {
        /// <summary>
        /// WebGL获取Bundle信息文件的路径
        /// </summary>
        internal static string BundleFileExistPath_WebGL(string bundlePackageName, string fileName)
        {
            string path = Path.Combine(AssetComponentConfig.LocalBundlePath, bundlePackageName, fileName);
            return path;
        }

        internal static async UniTask<string> LoadWebGLFileText(string filePath)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(filePath))
            {
                await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.Success)
#else
                if (string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    string str = webRequest.downloadHandler.text;
                    return str;
                }
                else
                {
                    AssetLogHelper.LogError("WebGL初始化分包未找到要读取的文件: \t" + filePath);
                    return "";
                }
                
            }
        }
        
        
    }
#endif
}
