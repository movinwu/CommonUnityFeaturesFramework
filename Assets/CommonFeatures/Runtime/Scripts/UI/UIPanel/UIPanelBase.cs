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
    /// UI�������
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

            //�����ҵ����е��Զ������������
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

            //�����ҵ����е��Զ������������
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
        /// ��������
        /// </summary>
        /// <param name="referenceResolution">Ԥ��ߴ�</param>
        public void PanelScreenFit(Vector2 referenceResolution)
        {
            var curScreenSize = Screen.safeArea;//����safeArea//Ԥ��ߴ�

            var size = referenceResolution;
            var radio = size.y / size.x;//Ԥ��ߴ�߶ȺͿ�ȱ�ֵ
            var curRadio = curScreenSize.height / curScreenSize.width;//��ǰ�ߴ�߶ȺͿ�ȱ�ֵ
            size.y = size.y * curRadio / radio;//�ߴ�

            var scaler = referenceResolution.x / curScreenSize.width;
            var up = (Screen.height - curScreenSize.yMax) * scaler;//�Ϸ����
            var down = curScreenSize.yMin * scaler;//�·����
            var left = curScreenSize.xMin * scaler;//�����
            var right = (Screen.width - curScreenSize.xMax) * scaler;//�Ҳ���
            var pos = new Vector2(left - right, down - up);//λ��

            //��Ļ�ߴ��޸�
            var rectTrans = this.GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.one * 0.5f;
            rectTrans.anchorMax = Vector2.one * 0.5f;
            rectTrans.pivot = Vector2.one * 0.5f;
            rectTrans.sizeDelta = size;
            rectTrans.anchoredPosition = pos;
        }
    }
}
