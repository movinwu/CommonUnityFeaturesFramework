using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.IO;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestDownload : MonoBehaviour
    {
        private async void Start()
        {
            string url1 = "http://education.jnaw.top/lubozhibouploaded/quanjing/1615791915263.mp4";
            string url2 = "http://education.jnaw.top/lubozhibouploaded/quanjing/1629796505018.mp4";

            string savePath = Path.Combine(Application.dataPath, "../TestResource/Download");

            var download1 = await CommonFeaturesManager.Download.InitDownload(url1, savePath);
            download1.downloadedLength.ForEachAsync(x =>
            {
                CommonLog.Log($"下载文件1, 进度 {download1.downloadedLength} / {download1.downloadTotalLength}, 百分比: {(double)download1.downloadedLength / download1.downloadTotalLength * 100d}%");
            }).Forget();
            await CommonFeaturesManager.Download.StartDownload(download1);
            var download2 = await CommonFeaturesManager.Download.InitDownload(url2, savePath);
            download2.downloadedLength.ForEachAsync(x =>
            {
                CommonLog.Log($"下载文件2, 进度 {download2.downloadedLength} / {download2.downloadTotalLength}, 百分比: {(double)download2.downloadedLength / download2.downloadTotalLength * 100d}%");
            }).Forget();
            await CommonFeaturesManager.Download.StartDownload(download2);
        }
    }
}
