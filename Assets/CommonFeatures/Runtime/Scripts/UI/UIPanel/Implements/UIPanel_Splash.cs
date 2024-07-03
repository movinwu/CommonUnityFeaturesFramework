using CommonFeatures.Config;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ���ɽ��棬��ʾ��˾logo���߶�����
    /// </summary>
    public class UIPanel_Splash : UIPanelBase
    {
        [SerializeField] private TMP_Text m_Text;

        protected override UniTask OnInit()
        {
            m_Text.text = $"{CommonFeaturesManager.Localization.GetMainLocalization("splash_welcome")}\n{string.Format(CommonFeaturesManager.Localization.GetMainLocalization("splash_version_header"), CommonFeaturesManager.Config.GetConfig<ApplicationConfig>().FullVersion)}";

            return base.OnInit();
        }
    }
}
