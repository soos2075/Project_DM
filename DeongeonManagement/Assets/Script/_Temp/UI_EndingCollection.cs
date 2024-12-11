using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndingCollection : UI_Base
{
    void Start()
    {
        Init();
    }



    enum Buttons
    {
        Ravi,
        Dog,
        Cat,
        Demon,
        Dragon,
        Hero,
    }



    public override void Init()
    {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Ravi).gameObject.AddUIEvent(data => Replay_Ending(Endings.Ravi));
        GetButton((int)Buttons.Demon).gameObject.AddUIEvent(data => Replay_Ending(Endings.Demon));
        GetButton((int)Buttons.Hero).gameObject.AddUIEvent(data => Replay_Ending(Endings.Hero));

        GetButton((int)Buttons.Cat).gameObject.AddUIEvent(data => Replay_Ending(Endings.Cat));
        GetButton((int)Buttons.Dog).gameObject.AddUIEvent(data => Replay_Ending(Endings.Dog));
        GetButton((int)Buttons.Dragon).gameObject.AddUIEvent(data => Replay_Ending(Endings.Dragon));
    }


    void Replay_Ending(Endings endings)
    {
        UserData.Instance.EndingState = endings;
        Managers.Scene.LoadSceneAsync(SceneName._7_NewEnding, false);
    }
}
