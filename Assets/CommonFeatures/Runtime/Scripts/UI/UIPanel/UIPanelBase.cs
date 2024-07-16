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
            PanelScreenFit(CFM.UI.GetCanvasReferenceResolution());
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

        public virtual UniTask OnUpdate()
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
        /// <param name="referenceResolution">预设尺寸</param>
        public void PanelScreenFit(Vector2 referenceResolution)
        {
            var curScreenSize = Screen.safeArea;//适配safeArea//预设尺寸

            var size = referenceResolution;
            var radio = size.y / size.x;//预设尺寸高度和宽度比值
            var curRadio = curScreenSize.height / curScreenSize.width;//当前尺寸高度和宽度比值
            size.y = size.y * curRadio / radio;//尺寸

            var scaler = referenceResolution.x / curScreenSize.width;
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
        }
    }
}
