using CommonFeatures.Config;
using CommonFeatures.DataTable;
using CommonFeatures.FSM;
using CommonFeatures.GML;
using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.PSM;
using CommonFeatures.Timer;
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
        /// ����
        /// </summary>
        public static CommonConfig Config;

        /// <summary>
        /// ���ݱ�
        /// </summary>
        public static CommonFeature_DataTable DataTable;

        /// <summary>
        /// ����
        /// </summary>
        public static CommonFeature_Download Download;

        /// <summary>
        /// Http����
        /// </summary>
        public static CommonFeature_Http Http;

        /// <summary>
        /// ����
        /// </summary>
        public static CommonFeature_Net Net;

        /// <summary>
        /// ʱ��
        /// </summary>
        public static CommonFeature_Timer Timer;

        /// <summary>
        /// ����״̬��
        /// </summary>
        public static CommonFeature_FSM FSM;

        /// <summary>
        /// ����״̬��
        /// </summary>
        public static CommonFeature_PSM PSM;

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
                if ("DataTable".Equals(child.name))
                {
                    DataTable = child.GetComponent<CommonFeature_DataTable>();
                    DataTable.Init();
                }
                else if ("Download".Equals(child.name))
                {
                    Download = child.GetComponent<CommonFeature_Download>();
                    Download.Init();
                }
                else if ("Http".Equals(child.name))
                {
                    Http = child.GetComponent<CommonFeature_Http>();
                    Http.Init();
                }
                else if ("Net".Equals(child.name))
                {
                    Net = child.GetComponent<CommonFeature_Net>();
                    Net.Init();
                }
                else if ("Timer".Equals(child.name))
                {
                    Timer = child.GetComponent<CommonFeature_Timer>();
                    Timer.Init();
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
                else if ("GML".Equals(child.name))
                {
                    GML = child.GetComponent<CommonFeature_GML>();
                    GML.Init();
                }
            }

            //��ʽ��ʼ��Ϸ
            if (null == GML)
            {
                CommonLog.LogError("ȱ����Ϸ��ѭ��,�޷���ʼ��Ϸ");
            }
            else
            {
                GML.StartGame();
            }
        }

        private void Update()
        {
            DataTable.Tick();
            Download.Tick();
            Http.Tick();
            Net.Tick();
            Timer.Tick();
            FSM.Tick();
        }

        private void OnDestroy()
        {
            DataTable.Release();
            Download.Release();
            Http.Release();
            Net.Release();
            Timer.Release();
            FSM.Release();
        }
    }
}
