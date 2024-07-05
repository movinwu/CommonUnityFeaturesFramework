using CommonFeatures.Config;
using CommonFeatures.Localization;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 过渡界面，显示公司logo或者动画等
    /// </summary>
    public class UIPanel_Splash : UIPanelBase
    {
        [SerializeField] private AutoLocalization m_Text;

        protected override UniTask OnShow()
        {
            m_Text.AddLocalizationFormat(CommonFeaturesManager.Config.GetConfig<ApplicationConfig>().FullVersion);

            return base.OnShow();
        }
    }
}
