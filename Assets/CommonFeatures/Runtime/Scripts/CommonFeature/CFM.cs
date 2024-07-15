using CommonFeatures.Config;
using CommonFeatures.DataTable;
using CommonFeatures.Event;
using CommonFeatures.FSM;
using CommonFeatures.GML;
using CommonFeatures.Localization;
using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.Pool;
using CommonFeatures.PSM;
using CommonFeatures.Resource;
using CommonFeatures.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// CommonFeaturesManager
    /// <para>ͨ�ù��ܹ�����,���й������</para>
    /// <para>ȫ��ֻ����һ���������,��Ҫ�ظ�����</para>
    /// </summary>
    public class CFM : MonoBehaviour
    {
        /// <summary>
        /// ��������������
        /// </summary>
        private static string BelongGameObjectName = string.Empty;

        /// <summary>
        /// �¼�֪ͨ
        /// </summary>
        public static CommonFeature_Event Event;

        /// <summary>
        /// ����
        /// </summary>
        public static CommonFeature_Config Config;

        /// <summary>
        /// ���ݱ�
        /// </summary>
        public static CommonFeature_DataTable DataTable;

        /// <summary>
        /// ����
        /// </summary>
        public static CommonFeature_Network Network;

        /// <summary>
        /// ����״̬��
        /// </summary>
        public static CommonFeature_FSM FSM;

        /// <summary>
        /// ����״̬��
        /// </summary>
        public static CommonFeature_PSM PSM;

        /// <summary>
        /// ��Դ����
        /// </summary>
        public static CommonFeature_Resource Resource;

        /// <summary>
        /// UI
        /// </summary>
        public static CommonFeature_UI UI;

        /// <summary>
        /// ��Ϸ��ѭ��
        /// </summary>
        public static CommonFeature_GML GML;

        /// <summary>
        /// ���ػ�
        /// </summary>
        public static CommonFeature_Localization Localization;

        /// <summary>
        /// ���ó�
        /// </summary>
        public static CommonFeature_ReferencePool ReferencePool;

        /// <summary>
        /// ���建���
        /// </summary>
        public static CommonFeature_GameObjectPool GameObjectPool;

        private void Awake()
        {
            AsyncAwake().Forget();
        }

        private async UniTask AsyncAwake()
        {
            if (string.IsNullOrEmpty(BelongGameObjectName))
            {
                BelongGameObjectName = this.gameObject.name;
            }
            else
            {
                CommonLog.LogError($"ȫ������ֻ����һ��ʵ��,������{BelongGameObjectName}��{this.gameObject.name}���ظ�����");
                return;
            }

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i);
                if ("Config".Equals(child.name))
                {
                    Config = child.GetComponent<CommonFeature_Config>();
                    await Config.Init();
                }
                else if ("DataTable".Equals(child.name))
                {
                    DataTable = child.GetComponent<CommonFeature_DataTable>();
                    await DataTable.Init();
                }
                else if ("Network".Equals(child.name))
                {
                    Network = child.GetComponent<CommonFeature_Network>();
                    await Network.Init();
                }
                else if ("FSM".Equals(child.name))
                {
                    FSM = child.GetComponent<CommonFeature_FSM>();
                    await FSM.Init();
                }
                else if ("PSM".Equals(child.name))
                {
                    PSM = child.GetComponent<CommonFeature_PSM>();
                    await PSM.Init();
                }
                else if ("Resource".Equals(child.name))
                {
                    Resource = child.GetComponent<CommonFeature_Resource>();
                    await Resource.Init();
                }
                else if ("GML".Equals(child.name))
                {
                    GML = child.GetComponent<CommonFeature_GML>();
                    await GML.Init();
                }
                else if ("Event".Equals(child.name))
                {
                    Event = child.GetComponent<CommonFeature_Event>();
                    await Event.Init();
                }
                else if ("UI".Equals(child.name))
                {
                    UI = child.GetComponent<CommonFeature_UI>();
                    await UI.Init();
                }
                else if ("Localization".Equals(child.name))
                {
                    Localization = child.GetComponent<CommonFeature_Localization>();
                    await Localization.Init();
                }
                else if ("ReferencePool".Equals(child.name))
                {
                    ReferencePool = child.GetComponent<CommonFeature_ReferencePool>();
                    await ReferencePool.Init();
                }
                else if ("GameObjectPool".Equals(child.name))
                {
                    GameObjectPool = child.GetComponent<CommonFeature_GameObjectPool>();
                    await GameObjectPool.Init();
                }
            }

            //��ʽ��ʼ��Ϸ
            if (null == GML)
            {
                CommonLog.LogError("ȱ����Ϸ��ѭ��,�޷���ʼ��Ϸ");
            }
            else
            {
                GML.StartGame().Forget();
            }
        }

        private void Update()
        {
            ReferencePool.OnUpdate();
            GameObjectPool.OnUpdate();
            Event.OnUpdate();
            Config.OnUpdate();
            DataTable.OnUpdate();
            Network.OnUpdate();
            FSM.OnUpdate();
            PSM.OnUpdate();
            Localization.OnUpdate();
            Resource.OnUpdate();
            UI.OnUpdate();
            GML.OnUpdate();
        }

        private void OnDestroy()
        {
            ReferencePool.Release();
            GameObjectPool.Release();
            Event.Release();
            Config.Release();
            DataTable.Release();
            Network.Release();
            FSM.Release();
            PSM.Release();
            Localization.Release();
            Resource.Release();
            UI.Release();
            GML.Release();
        }
    }
}
