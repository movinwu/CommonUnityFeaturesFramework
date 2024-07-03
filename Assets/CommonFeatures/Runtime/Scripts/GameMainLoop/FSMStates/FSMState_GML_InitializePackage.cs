using CommonFeatures.FSM;
using CommonFeatures.Log;
using CommonFeatures.Resource;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-��ʼ����
    /// </summary>
    public class FSMState_GML_InitializePackage : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //��ʼ������
            CommonLog.Resource("��ʼ��ʼ����Դ��");

            var blackboard = this.FSM.GetBlackboard<GameMainLoopBlackboard>();

            // ������Դ������
            var package = YooAssets.TryGetPackage(blackboard.PackageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(blackboard.PackageName);
            }

            blackboard.Package = package;

            InitializationOperation initializationOperation = null;
            //�༭������ģʽ
            if (blackboard.PlayMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(blackboard.DefaultBuildPipeline, blackboard.PackageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // ��������ģʽ
            else if (blackboard.PlayMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.DecryptionServices = new FileStreamDecryption();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // ��������ģʽ
            else if (blackboard.PlayMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new HostPlayModeParameters();
                createParameters.DecryptionServices = new FileStreamDecryption();
                createParameters.BuildinQueryServices = new GameQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // WebGL����ģʽ
            else if (blackboard.PlayMode == EPlayMode.WebPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new WebPlayModeParameters();
                createParameters.DecryptionServices = new FileStreamDecryption();
                createParameters.BuildinQueryServices = new GameQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            await UniTask.WaitUntil(() => initializationOperation.IsDone);

            // �����ʼ��ʧ�ܵ�����ʾ����
            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                CommonLog.ResourceError($"{initializationOperation.Error}");
            }
            else
            {
                var version = initializationOperation.PackageVersion;
                CommonLog.Resource($"Init resource package version : {version}");
                this.FSM.ChangeState<FSMState_GML_UpdatePackageVersion>();
            }
        }

        /// <summary>
        /// ��ȡ��Դ��������ַ
        /// </summary>
        private string GetHostServerURL()
        {
            //string hostServerIP = "http://10.0.2.2"; //��׿ģ������ַ
            string hostServerIP = "http://127.0.0.1";
            string appVersion = "v1.0";

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/CDN/WebGL/{appVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
        }
    }
}
