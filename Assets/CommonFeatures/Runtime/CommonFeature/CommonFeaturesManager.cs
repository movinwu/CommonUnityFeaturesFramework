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
    /// 通用功能管理器,所有功能入口
    /// <para>全局只能有一个功能入口,不要重复挂载</para>
    /// </summary>
    public class CommonFeaturesManager : MonoBehaviour
    {
        /// <summary>
        /// 所属的物体名称
        /// </summary>
        private static string BelongGameObjectName = string.Empty;

        /// <summary>
        /// 配置
        /// </summary>
        public static CommonConfig Config;

        /// <summary>
        /// 数据表
        /// </summary>
        public static CommonFeature_DataTable DataTable;

        /// <summary>
        /// 网络
        /// </summary>
        public static CommonFeature_Net Net;

        /// <summary>
        /// 有限状态机
        /// </summary>
        public static CommonFeature_FSM FSM;

        /// <summary>
        /// 并行状态机
        /// </summary>
        public static CommonFeature_PSM PSM;

        /// <summary>
        /// 游戏主循环
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
                CommonLog.LogError($"全局有且只能有一个实例,但是在{BelongGameObjectName}和{this.gameObject.name}上重复挂载");
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

            //正式开始游戏
            if (null == GML)
            {
                CommonLog.LogError("缺少游戏主循环,无法开始游戏");
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
