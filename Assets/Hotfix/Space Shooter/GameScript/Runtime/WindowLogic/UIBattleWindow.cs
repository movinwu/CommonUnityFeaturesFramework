using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonFeatures.Event;
using CommonFeatures;

public class UIBattleWindow : MonoBehaviour
{
    private GameObject _overView;
    private Text _scoreLabel;

    private void Awake()
    {
        _overView = this.transform.Find("OverView").gameObject;
        _scoreLabel = this.transform.Find("ScoreView/Score").GetComponent<Text>();
        _scoreLabel.text = "Score : 0";

        var restartBtn = this.transform.Find("OverView/Restart").GetComponent<Button>();
        restartBtn.onClick.AddListener(OnClickRestartBtn);

        var homeBtn = this.transform.Find("OverView/Home").GetComponent<Button>();
        homeBtn.onClick.AddListener(OnClickHomeBtn);

        CommonFeaturesManager.Event.AddListener<BattleEventDefine.ScoreChange>(OnHandleEventMessage);
        CommonFeaturesManager.Event.AddListener<BattleEventDefine.GameOver>(OnHandleEventMessage);
    }
    private void OnDestroy()
    {
        CommonFeaturesManager.Event.RemoveListener<BattleEventDefine.ScoreChange>(OnHandleEventMessage);
        CommonFeaturesManager.Event.RemoveListener<BattleEventDefine.GameOver>(OnHandleEventMessage);
    }

    private void OnClickRestartBtn()
    {
        SceneEventDefine.ChangeToBattleScene.SendEventMessage();
    }
    private void OnClickHomeBtn()
    {
        SceneEventDefine.ChangeToHomeScene.SendEventMessage();
    }
    private void OnHandleEventMessage(IEventMessage message)
    {
        if(message is BattleEventDefine.ScoreChange)
        {
            var msg = message as BattleEventDefine.ScoreChange;
            _scoreLabel.text = $"Score : {msg.CurrentScores}";
        }
        else if(message is BattleEventDefine.GameOver)
        {
            _overView.SetActive(true);
        }
    }
}