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
        /// <summary>
        /// 是否正在显示
        /// </summary>
        private bool m_Showing;

        public async UniTask Init()
        {
            m_Showing = false;

            await OnInit();
        }

        protected virtual async UniTask OnInit() { }

        public async UniTask Show()
        {
            m_Showing = true;
            PanelFit().Forget();

            await OnShow();
        }

        protected virtual async UniTask OnShow() { }

        public void Hide()
        {
            m_Showing = false;

            OnHide();
        }

        protected virtual void OnHide() { }

        public void Release()
        {
            m_Showing = false;

            OnRelease();
        }

        protected virtual void OnRelease() { }

        /// <summary>
        /// 界面适配
        /// </summary>
        private async UniTask PanelFit()
        {
            var preScreenSize = Rect.zero;//缓存的上一帧屏幕尺寸
            while (m_Showing)
            {
                //当屏幕适配所需参数发生变化时
                var curScreenSize = Screen.safeArea;//适配safeArea
                if (!preScreenSize.Equals(curScreenSize))
                {
                    preScreenSize = curScreenSize;

                    //预设尺寸
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

                    var pos = curScreenSize.center;//位置
                    pos = pos * canvasScaler.referenceResolution.x / curScreenSize.width;
                    pos = pos - size / 2;

                    //屏幕尺寸修改
                    var rectTrans = this.GetComponent<RectTransform>();
                    rectTrans.anchorMin = Vector2.one * 0.5f;
                    rectTrans.anchorMax = Vector2.one * 0.5f;
                    rectTrans.pivot = Vector2.one * 0.5f;
                    rectTrans.sizeDelta = size;
                    rectTrans.anchoredPosition = pos;
                }

                //等待下一帧运行
                await UniTask.NextFrame(PlayerLoopTiming.PreLateUpdate);
            }
        }
    }
}
