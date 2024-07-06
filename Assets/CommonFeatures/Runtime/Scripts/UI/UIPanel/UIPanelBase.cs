using CommonFeatures.Localization;
using CommonFeatures.Utility;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI界面基类
    /// </summary>
    public class UIPanelBase : MonoBehaviour
    {
        public async UniTask Init()
        {
            this.gameObject.SetActive(false);

            await OnInit();
        }

        protected virtual UniTask OnInit() 
        { 
            return UniTask.CompletedTask;
        }

        public async UniTask Show()
        {
            PanelScreenFit().Forget();
            this.gameObject.SetActive(true);

            //遍历找到所有的自动化多语言组件
            var autoLocalizations = this.transform.FindComponents<AutoLocalization>();
            for (int i = 0; i < autoLocalizations.Count; i++)
            {
                autoLocalizations[i].OnShow();
            }

            await OnShow();
        }

        protected virtual UniTask OnShow()
        {
            return UniTask.CompletedTask;
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);

            //遍历找到所有的自动化多语言组件
            var autoLocalizations = this.transform.FindComponents<AutoLocalization>();
            for (int i = 0; i < autoLocalizations.Count; i++)
            {
                autoLocalizations[i].OnHide();
            }

            OnHide();
        }

        protected virtual void OnHide() { }

        public void Release()
        {
            OnRelease();
        }

        protected virtual void OnRelease() { }

        /// <summary>
        /// 界面适配
        /// </summary>
        public UniTask PanelScreenFit()
        {
            var curScreenSize = Screen.safeArea;//适配safeArea//预设尺寸
            var canvasScaler = this.transform.parent.parent.GetComponent<CanvasScaler>();

            //采用Scale With Screen Size方案
            if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                //默认1080 * 1920,实际情况下应该预先设置为界面制作实际尺寸
                canvasScaler.referenceResolution = new Vector2(1080f, 1920f);
            }

            //固定适配权重
            if (canvasScaler.matchWidthOrHeight != 0)
            {
                canvasScaler.matchWidthOrHeight = 0;
            }

            var size = canvasScaler.referenceResolution;
            var radio = size.y / size.x;//预设尺寸高度和宽度比值
            var curRadio = curScreenSize.height / curScreenSize.width;//当前尺寸高度和宽度比值
            size.y = size.y * curRadio / radio;//尺寸

            var scaler = canvasScaler.referenceResolution.x / curScreenSize.width;
            var up = (Screen.height - curScreenSize.yMax) * scaler;//上方间距
            var down = curScreenSize.yMin * scaler;//下方间距
            var left = curScreenSize.xMin * scaler;//左侧间距
            var right = (Screen.width - curScreenSize.xMax) * scaler;//右侧间距
            var pos = new Vector2(left - right, down - up);//位置

            //屏幕尺寸修改
            var rectTrans = this.GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.one * 0.5f;
            rectTrans.anchorMax = Vector2.one * 0.5f;
            rectTrans.pivot = Vector2.one * 0.5f;
            rectTrans.sizeDelta = size;
            rectTrans.anchoredPosition = pos;

            return UniTask.CompletedTask;
        }
    }
}
