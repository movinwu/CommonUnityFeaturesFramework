using CommonFeatures.Config;
using CommonFeatures.DataTable;
using CommonFeatures.Event;
using CommonFeatures.FSM;
using CommonFeatures.GML;
using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.PSM;
using CommonFeatures.Resource;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// CommonFeaturesManager
    /// ͨ�ù��ܹ�����,���й������
    /// <para>ȫ��ֻ����һ���������,��Ҫ�ظ�����</para>
    /// </summary>
    public class CommonFeaturesManager : MonoBehaviour
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
        /// ��Ϸ��ѭ��
        /// </summary>
        public static CommonFeature_GML GML;

        private void Awake()
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
                    Config.Init();
                }
                else if ("DataTable".Equals(child.name))
                {
                    DataTable = child.GetComponent<CommonFeature_DataTable>();
                    DataTable.Init();
                }
                else if ("Net".Equals(child.name))
                {
                    Network = child.GetComponent<CommonFeature_Network>();
                    Network.Init();
                }
                else if ("FSM".Equals(child.name))
                {
                    FSM = child.GetComponent<CommonFeature_FSM>();
                    FSM.Init();
                }
                else if ("PSM".Equals(child.name))
                {
                    PSM = child.GetComponent<CommonFeature_PSM>();
                    PSM.Init();
                }
                else if ("Resource".Equals(child.name))
                {
                    Resource = child.GetComponent<CommonFeature_Resource>();
                    Resource.Init();
                }
                else if ("GML".Equals(child.name))
                {
                    GML = child.GetComponent<CommonFeature_GML>();
                    GML.Init();
                }
                else if ("Event".Equals(child.name))
                {
                    Event = child.GetComponent<CommonFeature_Event>();
                    Event.Init();
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

        private void OnDestroy()
        {
            DataTable.Release();
            Network.Release();
            FSM.Release();
            PSM.Release();
            GML.Release();
            Resource.Release();
            Event.Release();
        }
    }
}
