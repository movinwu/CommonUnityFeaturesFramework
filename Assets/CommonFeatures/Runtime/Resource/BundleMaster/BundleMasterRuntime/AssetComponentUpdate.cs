using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using CommonFeatures.Log;

namespace BundleMaster
{
    public static partial class AssetComponent
    {
#if !(Nintendo_Switch || BMWebGL)
        /// <summary>
        /// 检查分包是否需要更新
        /// </summary>
        /// <param name="bundlePackageNames">所有分包的名称以及是否验证文件CRC</param>
        public static async UniTask<UpdateBundleDataInfo> CheckAllBundlePackageUpdate(Dictionary<string, bool> bundlePackageNames)
        {
            UpdateBundleDataInfo updateBundleDataInfo = new UpdateBundleDataInfo();
            if (AssetComponentConfig.AssetLoadMode != EAssetLoadMode.RemoteAB)
            {
                updateBundleDataInfo.NeedUpdate = false;
                if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.LocalAB)
                {
                    AssetComponentConfig.HotfixPath = AssetComponentConfig.LocalBundlePath;
                }
                else
                {
#if !UNITY_EDITOR
                    AssetLogHelper.LogError("AssetLoadMode = AssetLoadMode.Develop 只能在编辑器下运行");
#endif
                }
                return updateBundleDataInfo;
            }
            updateBundleDataInfo.UpdateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            //开始处理每个分包的更新信息
            foreach (var bundlePackageInfo in bundlePackageNames)
            {
                string bundlePackageName = bundlePackageInfo.Key;
                string remoteVersionLog = await GetRemoteBundlePackageVersionLog(bundlePackageName);
                if (remoteVersionLog == null)
                {
                    CommonLog.ResourceError("未找到远程分包: " + bundlePackageName);
                    continue;
                }
                //创建各个分包对应的文件夹
                if (!Directory.Exists(Path.Combine(AssetComponentConfig.HotfixPath, bundlePackageName)))
                {
                    Directory.CreateDirectory(Path.Combine(AssetComponentConfig.HotfixPath, bundlePackageName));
                }
                //获取CRCLog
                string crcLogPath = Path.Combine(AssetComponentConfig.HotfixPath, bundlePackageName, "CRCLog.txt");
                updateBundleDataInfo.PackageCRCDictionary.Add(bundlePackageName, new Dictionary<string, uint>());
                if (File.Exists(crcLogPath))
                {
                    string crcLog;
                    using (StreamReader streamReader = new StreamReader(crcLogPath))
                    {
                        crcLog = await streamReader.ReadToEndAsync();
                    }
                    string[] crcLogData = crcLog.Split('\n');
                    for (int j = 0; j < crcLogData.Length; j++)
                    {
                        string crcLine = crcLogData[j];
                        if (string.IsNullOrWhiteSpace(crcLine))
                        {
                            continue;
                        }
                        string[] info = crcLine.Split('|');
                        if (info.Length != 3)
                        {
                            continue;
                        }
                        if (!uint.TryParse(info[1], out uint crc))
                        {
                            continue;
                        }
                        //如果存在重复就覆盖
                        if (updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].ContainsKey(info[0]))
                        {
                            updateBundleDataInfo.PackageCRCDictionary[bundlePackageName][info[0]] = crc;
                        }
                        else
                        {
                            updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].Add(info[0], crc);
                        }
                            
                    }
                }
                else
                {
                    CreateUpdateLogFile(crcLogPath, null);
                }
                //把CRCLogPath的分包名存起来
                updateBundleDataInfo.PackageCRCFile.Add(bundlePackageName, null);
                //获取本地的VersionLog
                string localVersionLogExistPath = BundleFileExistPath(bundlePackageName, "VersionLogs.txt", true);
                string localVersionLog;
                using (UnityWebRequest webRequest = UnityWebRequest.Get(localVersionLogExistPath))
                {
                    await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                    if (webRequest.result != UnityWebRequest.Result.Success)
#else
                    if (!string.IsNullOrEmpty(webRequest.error))
#endif
                    {
                        localVersionLog = "INIT|0";
                    }
                    else
                    {
                        localVersionLog = webRequest.downloadHandler.text;
                    }
                }

                string[] remoteVersionData = remoteVersionLog.Split('\n');
                string[] localVersionData = localVersionLog.Split('\n');
                if (bundlePackageInfo.Value)
                {
                    await CalcNeedUpdateBundleFileCRC(updateBundleDataInfo, bundlePackageName, remoteVersionData, localVersionData);
                }
                else
                {
                    await CalcNeedUpdateBundle(updateBundleDataInfo, bundlePackageName, remoteVersionData, localVersionData);
                }
            }
            if (updateBundleDataInfo.PackageNeedUpdateBundlesInfos.Count > 0)
            {
                updateBundleDataInfo.NeedUpdate = true;
            }
            else
            {
                //如果不需要更新就清理CRC路径
                updateBundleDataInfo.PackageCRCFile.Clear();
            }
            return updateBundleDataInfo;
        }
        
        /// <summary>
        /// 获取所有需要更新的Bundle的文件(不检查文件CRC)
        /// </summary>
        private static async UniTask CalcNeedUpdateBundle(UpdateBundleDataInfo updateBundleDataInfo, string bundlePackageName, string[] remoteVersionData, string[] localVersionData)
        {
            string[] remoteVersionDataSplits = remoteVersionData[0].Split('|');
            int remoteVersion = int.Parse(remoteVersionDataSplits[1]);
            int localVersion = int.Parse(localVersionData[0].Split('|')[1]);
            updateBundleDataInfo.PackageToVersion.Add(bundlePackageName, new int[2]{localVersion, remoteVersion});
            if (remoteVersionDataSplits[2] == "origin")
            {
                updateBundleDataInfo.PackageToType.Add(bundlePackageName, UpdateBundleDataInfo.PackageType.Origin);
            }
            else
            {
                if (bool.TryParse(remoteVersionDataSplits[2], out bool result))
                {
                    updateBundleDataInfo.PackageToType.Add(bundlePackageName, result ? UpdateBundleDataInfo.PackageType.Encrypt : UpdateBundleDataInfo.PackageType.Normal);
                }
                else
                {
                    updateBundleDataInfo.PackageToType.Add(bundlePackageName, UpdateBundleDataInfo.PackageType.Normal);
                }
            }
            
            if (localVersion > remoteVersion)
            {
                CommonLog.ResourceError("本地版本号优先与远程版本号 " + localVersion + ">" + remoteVersion + "\n"
                + "localBundleTime: " + localVersionData[0].Split('|')[0] + "\n"
                + "remoteBundleTime: " + remoteVersionData[0].Split('|')[0] + "\n"
                + "Note: 发送了版本回退或者忘了累进版本号");
            }
            //判断StreamingAsset下资源
            string streamingAssetLogPath = Path.Combine(AssetComponentConfig.LocalBundlePath, bundlePackageName, "VersionLogs.txt");
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            streamingAssetLogPath = "file://" + streamingAssetLogPath;
#endif
            string streamingLog;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(streamingAssetLogPath))
            {
                await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    streamingLog = "INIT|0";
                }
                else
                {
                    streamingLog = webRequest.downloadHandler.text;
                }
            }
            //添加streaming文件CRC信息
            Dictionary<string, uint> streamingCRC = new Dictionary<string, uint>();
            string[] streamingLogData = streamingLog.Split('\n');
            for (int i = 1; i < streamingLogData.Length; i++)
            {
                string streamingDataLine = streamingLogData[i];
                if (!string.IsNullOrWhiteSpace(streamingDataLine))
                {
                    string[] info = streamingDataLine.Split('|');
                    uint crc = uint.Parse(info[2]);
                    if (!updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].ContainsKey(info[0]))
                    {
                        updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].Add(info[0], crc);
                    }
                    streamingCRC.Add(info[0], crc);
                }
            }
            // //添加本地文件CRC信息
            // for (int i = 1; i < localVersionData.Length; i++)
            // {
            //     string localVersionDataLine = localVersionData[i];
            //     if (!string.IsNullOrWhiteSpace(localVersionDataLine))
            //     {
            //         string[] info = localVersionDataLine.Split('|');
            //         if (updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].ContainsKey(info[0]))
            //         {
            //             updateBundleDataInfo.PackageCRCDictionary[bundlePackageName][info[0]] = uint.Parse(info[2]);
            //         }
            //         else
            //         {
            //             updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].Add(info[0], uint.Parse(info[2]));
            //         }
            //     }
            // }
            //创建最后需要返回的数据
            Dictionary<string, long> needUpdateBundles = new Dictionary<string, long>();
            for (int i = 1; i < remoteVersionData.Length; i++)
            {
                string remoteVersionDataLine = remoteVersionData[i];
                if (!string.IsNullOrWhiteSpace(remoteVersionDataLine))
                {
                    string[] info = remoteVersionDataLine.Split('|');
                    if (updateBundleDataInfo.PackageCRCDictionary[bundlePackageName].TryGetValue(info[0], out uint crc))
                    {
                        if (crc == uint.Parse(info[2]))
                        {
                            if (streamingCRC.TryGetValue(info[0], out uint streamingCrc))
                            {
                                if (crc != streamingCrc)
                                {
                                    if (!File.Exists(PathUnifiedHelper.UnifiedPath(Path.Combine(AssetComponentConfig.HotfixPath, bundlePackageName, info[0]))))
                                    {
                                        needUpdateBundles.Add(info[0], long.Parse(info[1]));
                                    }
                                }
                            }
                            else
                            {
                                if (!File.Exists(PathUnifiedHelper.UnifiedPath(Path.Combine(AssetComponentConfig.HotfixPath, bundlePackageName, info[0]))))
                                {
                                    needUpdateBundles.Add(info[0], long.Parse(info[1]));
                                }
                            }
                            continue;
                        }
                    }
                    needUpdateBundles.Add(info[0], long.Parse(info[1]));
                }
            }
            if (!(needUpdateBundles.Count > 0 || remoteVersionData[0] != localVersionData[0]))
            {
                //说明这个分包不需要更新
                return;
            }
            updateBundleDataInfo.PackageNeedUpdateBundlesInfos.Add(bundlePackageName, needUpdateBundles);
            foreach (long needUpdateBundleSize in needUpdateBundles.Values)
            {
                updateBundleDataInfo.NeedUpdateSize += needUpdateBundleSize;
            }
        }
        
        /// <summary>
        /// 获取所有需要更新的Bundle的文件(计算文件CRC)
        /// </summary>
        private static async UniTask CalcNeedUpdateBundleFileCRC(UpdateBundleDataInfo updateBundleDataInfo, string bundlePackageName, string[] remoteVersionData, string[] localVersionData)
        {
            string[] remoteVersionDataSplits = remoteVersionData[0].Split('|');
            int remoteVersion = int.Parse(remoteVersionDataSplits[1]);
            int localVersion = int.Parse(localVersionData[0].Split('|')[1]);
            updateBundleDataInfo.PackageToVersion.Add(bundlePackageName, new int[2]{localVersion, remoteVersion});
            if (remoteVersionDataSplits[2] == "origin")
            {
                updateBundleDataInfo.PackageToType.Add(bundlePackageName, UpdateBundleDataInfo.PackageType.Origin);
            }
            else
            {
                if (bool.TryParse(remoteVersionDataSplits[2], out bool result))
                {
                    updateBundleDataInfo.PackageToType.Add(bundlePackageName, result ? UpdateBundleDataInfo.PackageType.Encrypt : UpdateBundleDataInfo.PackageType.Normal);
                }
                else
                {
                    updateBundleDataInfo.PackageToType.Add(bundlePackageName, UpdateBundleDataInfo.PackageType.Normal);
                }
            }
            if (localVersion > remoteVersion)
            {
                CommonLog.ResourceError("本地版本号优先与远程版本号 " + localVersion + ">" + remoteVersion + "\n"
                + "localBundleTime: " + localVersionData[0].Split('|')[0] + "\n"
                + "remoteBundleTime: " + remoteVersionData[0].Split('|')[0] + "\n"
                + "Note: 发送了版本回退或者忘了累进版本号");
            }
            //创建最后需要返回的数据
            Dictionary<string, long> needUpdateBundles = new Dictionary<string, long>();
            var checkCount = remoteVersionData.Length - 1;
            Queue<int> loopQueue = new Queue<int>();
            //loopSize依据资源大小调节, 如果资源太大loopSize也调很大, 会撑爆内存, 调大只会节约一点点时间(目的是如果有进度条每个循环更新一下进度条)
            int loopSize = 5;
            for (int i = 0; i < checkCount / loopSize; i++)
            {
                loopQueue.Enqueue(loopSize);
            }
            if (checkCount % loopSize > 0)
            {
                loopQueue.Enqueue(checkCount % loopSize);
            }
            int loop = loopQueue.Count;
            for (int i = 0; i < loop; i++)
            {
                checkCount = loopQueue.Dequeue();
                int count = checkCount;
                UniTask[] allCheckTask = new UniTask[count];
                for (int j = 0; j < count; j++)
                {
                    allCheckTask[j] = CheckFileCRC(remoteVersionData[i * loopSize + j + 1], bundlePackageName, needUpdateBundles);
                }
                await UniTask.WhenAll(allCheckTask);
            }
            
            if (!(needUpdateBundles.Count > 0 || remoteVersionData[0] != localVersionData[0]))
            {
                //说明这个分包不需要更新
                return;
            }
            updateBundleDataInfo.PackageNeedUpdateBundlesInfos.Add(bundlePackageName, needUpdateBundles);
            foreach (long needUpdateBundleSize in needUpdateBundles.Values)
            {
                updateBundleDataInfo.NeedUpdateSize += needUpdateBundleSize;
            }
        }

        private static async UniTask CheckFileCRC(string remoteVersionDataLine, string bundlePackageName, Dictionary<string, long> needUpdateBundles)
        {
            if (!string.IsNullOrWhiteSpace(remoteVersionDataLine))
            {
                string[] info = remoteVersionDataLine.Split('|');
                //如果文件不存在直接加入更新
                string filePath = BundleFileExistPath(bundlePackageName, info[0], true);
                uint fileCRC32 = await VerifyHelper.GetFileCRC32(filePath);
                //判断是否和远程一样, 不一样直接加入更新
                if (uint.Parse(info[2]) != fileCRC32)
                {
                    needUpdateBundles.Add(info[0], long.Parse(info[1]));
                }
            }
        }
        
        /// <summary>
        /// 下载更新
        /// </summary>
        public static async UniTask DownLoadUpdate(UpdateBundleDataInfo updateBundleDataInfo)
        {
            if (AssetComponentConfig.AssetLoadMode != EAssetLoadMode.RemoteAB)
            {
                CommonLog.ResourceError("AssetLoadMode != AssetLoadMode.Build 不需要更新");
                return;
            }
            //打开CRCLog文件
            List<string> bundlePackageNames = updateBundleDataInfo.PackageCRCFile.Keys.ToList();
            foreach (string bundlePackageName in bundlePackageNames)
            {
                string crcLogPath = Path.Combine(AssetComponentConfig.HotfixPath, bundlePackageName, "CRCLog.txt");
                StreamWriter crcStream = new StreamWriter(crcLogPath, true);
                crcStream.AutoFlush = false;
                updateBundleDataInfo.PackageCRCFile[bundlePackageName] = crcStream;
            }
            Dictionary<string, Queue<DownLoadTask>> packageDownLoadTask = new Dictionary<string, Queue<DownLoadTask>>();
            //准备需要下载的内容的初始化信息
            foreach (var packageNeedUpdateBundlesInfo in updateBundleDataInfo.PackageNeedUpdateBundlesInfos)
            {
                Queue<DownLoadTask> downLoadTaskQueue = new Queue<DownLoadTask>();
                string packageName = packageNeedUpdateBundlesInfo.Key;
                packageDownLoadTask.Add(packageName, downLoadTaskQueue);
                string downLoadPackagePath = Path.Combine(AssetComponentConfig.HotfixPath, packageName);
                if (!Directory.Exists(downLoadPackagePath))
                {
                    Directory.CreateDirectory(downLoadPackagePath);
                }
                foreach (var updateBundlesInfo in packageNeedUpdateBundlesInfo.Value)
                {
                    DownLoadTask downLoadTask = new DownLoadTask();
                    downLoadTask.UpdateBundleDataInfo = updateBundleDataInfo;
                    downLoadTask.PackageDownLoadTask = packageDownLoadTask;
                    downLoadTask.PackegName = packageName;
                    downLoadTask.DownLoadPackagePath = downLoadPackagePath;
                    downLoadTask.FileName = updateBundlesInfo.Key;
                    downLoadTask.FileSize = updateBundlesInfo.Value;
                    downLoadTaskQueue.Enqueue(downLoadTask);
                }
            }

            var allTasks = new List<UniTask>();
            //开启下载
            for (int i = 0; i < AssetComponentConfig.MaxDownLoadCount; i++)
            {
                foreach (Queue<DownLoadTask> downLoadTaskQueue in packageDownLoadTask.Values)
                {
                    if (downLoadTaskQueue.Count > 0)
                    {
                        //webGL 不可以用多线程下载器
                        allTasks.Add(downLoadTaskQueue.Dequeue().ThreadDownLoad());
                        break;
                    }
                }
            }
            //将下载进度更新添加到帧循环
            DownLoadAction += updateBundleDataInfo.UpdateProgressAndSpeedCallBack;
           
            await UniTask.WhenAll(allTasks);
            //下载完成关闭CRCLog文件
            foreach (StreamWriter sw in updateBundleDataInfo.PackageCRCFile.Values)
            {
                sw.Close();
                sw.Dispose();
            }
            updateBundleDataInfo.PackageCRCFile.Clear();
            //所有分包都下载完成了就处理分包的Log文件
            foreach (string packageName in updateBundleDataInfo.PackageNeedUpdateBundlesInfos.Keys)
            {
                if (updateBundleDataInfo.Cancel)
                {
                    return;
                }
                if (updateBundleDataInfo.PackageToType[packageName] != UpdateBundleDataInfo.PackageType.Origin)
                {
                    byte[] fileLogsData = await DownloadBundleHelper.DownloadDataByUrl(Path.Combine(AssetComponentConfig.BundleServerUrl, packageName, "FileLogs.txt"));
                    byte[] dependLogsData = await DownloadBundleHelper.DownloadDataByUrl(Path.Combine(AssetComponentConfig.BundleServerUrl, packageName, "DependLogs.txt"));
                    byte[] groupLogsData = await DownloadBundleHelper.DownloadDataByUrl(Path.Combine(AssetComponentConfig.BundleServerUrl, packageName, "GroupLogs.txt"));
                    if (fileLogsData == null || dependLogsData == null || groupLogsData == null)
                    {
                        updateBundleDataInfo.CancelUpdate();
                        CommonLog.ResourceError("获取Log表失败, PackageName: " + packageName);
                        continue;
                    }
                    CreateUpdateLogFile(Path.Combine(AssetComponentConfig.HotfixPath, packageName, "FileLogs.txt"),
                        System.Text.Encoding.UTF8.GetString(fileLogsData));
                    CreateUpdateLogFile(Path.Combine(AssetComponentConfig.HotfixPath, packageName, "DependLogs.txt"),
                        System.Text.Encoding.UTF8.GetString(dependLogsData));
                    CreateUpdateLogFile(Path.Combine(AssetComponentConfig.HotfixPath, packageName, "GroupLogs.txt"),
                        System.Text.Encoding.UTF8.GetString(groupLogsData));
                }
                byte[] versionLogsData = await DownloadBundleHelper.DownloadDataByUrl(Path.Combine(AssetComponentConfig.BundleServerUrl, packageName, "VersionLogs.txt"));
                if (versionLogsData == null)
                {
                    updateBundleDataInfo.CancelUpdate();
                    CommonLog.ResourceError("获取Log表失败, PackageName: " + packageName);
                    continue;
                }
                CreateUpdateLogFile(Path.Combine(AssetComponentConfig.HotfixPath, packageName, "VersionLogs.txt"),
                    System.Text.Encoding.UTF8.GetString(versionLogsData));
            }
            
            updateBundleDataInfo.SmoothProgress = 100;
            DownLoadAction -= updateBundleDataInfo.UpdateProgressAndSpeedCallBack;
            updateBundleDataInfo.FinishCallback?.Invoke();
            CommonLog.ResourceError("下载完成");
        }
#endif 
    }
    
}
