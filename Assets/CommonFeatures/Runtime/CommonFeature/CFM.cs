using CommonFeatures.Config;
using CommonFeatures.DataTable;
using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.Singleton;
using CommonFeatures.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// CommonFeaturesManager
    /// ͨ�ù��ܹ�����,���й������
    /// <para>ȫ��ֻ����һ���������,��Ҫ�ظ�����</para>
    /// </summary>
    public class CFM : MonoBehaviour
    {
        /// <summary>
        /// ��������������
        /// </summary>
        private static string BelongGameObjectName = string.Empty;

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
            }
        }

        private void Update()
        {
            Config.Tick();
            DataTable.Tick();
            Download.Tick();
            Http.Tick();
            Net.Tick();
            Timer.Tick();
        }

        private void OnDestroy()
        {
            Config.Release();
            DataTable.Release();
            Download.Release();
            Http.Release();
            Net.Release();
            Timer.Release();
        }
    }
}
