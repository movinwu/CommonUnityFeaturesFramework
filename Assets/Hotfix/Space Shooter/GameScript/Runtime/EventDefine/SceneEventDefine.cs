﻿using CommonFeatures;
using CommonFeatures.Event;

public class SceneEventDefine
{
    public class ChangeToHomeScene : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new ChangeToHomeScene();
            CommonFeaturesManager.Event.SendMessage(msg);
        }
    }

    public class ChangeToBattleScene : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new ChangeToBattleScene();
            CommonFeaturesManager.Event.SendMessage(msg);
        }
    }
}