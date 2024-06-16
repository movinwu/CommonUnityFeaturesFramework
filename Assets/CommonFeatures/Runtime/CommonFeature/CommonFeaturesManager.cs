using CommonFeatures.Config;
using CommonFeatures.DataTable;
using CommonFeatures.FSM;
using CommonFeatures.GML;
using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.PSM;
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
        public static CommonFeature_Net Net;

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
                else if ("Net".Equals(child.name))
                {
                    Net = child.GetComponent<CommonFeature_Net>();
                    Net.Init();
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

        private void OnDestroy()
        {
            DataTable.Release();
            Net.Release();
            FSM.Release();
            PSM.Release();
            GML.Release();
        }
    }
}
