using CommonFeatures.Config;
using CommonFeatures.DataTable;
using CommonFeatures.Event;
using CommonFeatures.FSM;
using CommonFeatures.GML;
using CommonFeatures.Localization;
using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.PSM;
using CommonFeatures.Resource;
using CommonFeatures.UI;
using Cysharp.Threading.Tasks;
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
        /// 事件通知
        /// </summary>
        public static CommonFeature_Event Event;

        /// <summary>
        /// 配置
        /// </summary>
        public static CommonFeature_Config Config;

        /// <summary>
        /// 数据表
        /// </summary>
        public static CommonFeature_DataTable DataTable;

        /// <summary>
        /// 网络
        /// </summary>
        public static CommonFeature_Network Network;

        /// <summary>
        /// 有限状态机
        /// </summary>
        public static CommonFeature_FSM FSM;

        /// <summary>
        /// 并行状态机
        /// </summary>
        public static CommonFeature_PSM PSM;

        /// <summary>
        /// 资源管理
        /// </summary>
        public static CommonFeature_Resource Resource;

        /// <summary>
        /// UI
        /// </summary>
        public static CommonFeature_UI UI;

        /// <summary>
        /// 游戏主循环
        /// </summary>
        public static CommonFeature_GML GML;

        /// <summary>
        /// 本地化
        /// </summary>
        public static CommonFeature_Localization Localization;

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
                CommonLog.LogError($"全局有且只能有一个实例,但是在{BelongGameObjectName}和{this.gameObject.name}上重复挂载");
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
                else if ("Net".Equals(child.name))
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
            }

            //正式开始游戏
            if (null == GML)
            {
                CommonLog.LogError("缺少游戏主循环,无法开始游戏");
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
            UI.Release();
        }
    }
}
