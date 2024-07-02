using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI�������
    /// </summary>
    public class UIPanelBase : MonoBehaviour
    {
        /// <summary>
        /// �Ƿ�������ʾ
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
        /// ��������
        /// </summary>
        private async UniTask PanelFit()
        {
            var preScreenSize = Rect.zero;//�������һ֡��Ļ�ߴ�
            while (m_Showing)
            {
                //����Ļ����������������仯ʱ
                var curScreenSize = Screen.safeArea;//����safeArea
                if (!preScreenSize.Equals(curScreenSize))
                {
                    preScreenSize = curScreenSize;

                    //Ԥ��ߴ�
                    var canvasScaler = this.transform.parent.parent.GetComponent<CanvasScaler>();

                    //����Scale With Screen Size����
                    if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                    {
                        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        //Ĭ��1080 * 1920,ʵ�������Ӧ��Ԥ������Ϊ��������ʵ�ʳߴ�
                        canvasScaler.referenceResolution = new Vector2(1080f, 1920f);
                    }

                    //�̶�����Ȩ��
                    if (canvasScaler.matchWidthOrHeight != 0)
                    {
                        canvasScaler.matchWidthOrHeight = 0;
                    }

                    var size = canvasScaler.referenceResolution;
                    var radio = size.y / size.x;//Ԥ��ߴ�߶ȺͿ�ȱ�ֵ
                    var curRadio = curScreenSize.height / curScreenSize.width;//��ǰ�ߴ�߶ȺͿ�ȱ�ֵ
                    size.y = size.y * curRadio / radio;//�ߴ�

                    var pos = curScreenSize.center;//λ��
                    pos = pos * canvasScaler.referenceResolution.x / curScreenSize.width;
                    pos = pos - size / 2;

                    //��Ļ�ߴ��޸�
                    var rectTrans = this.GetComponent<RectTransform>();
                    rectTrans.anchorMin = Vector2.one * 0.5f;
                    rectTrans.anchorMax = Vector2.one * 0.5f;
                    rectTrans.pivot = Vector2.one * 0.5f;
                    rectTrans.sizeDelta = size;
                    rectTrans.anchoredPosition = pos;
                }

                //�ȴ���һ֡����
                await UniTask.NextFrame(PlayerLoopTiming.PreLateUpdate);
            }
        }
    }
}
