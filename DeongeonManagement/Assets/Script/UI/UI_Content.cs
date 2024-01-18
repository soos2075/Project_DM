using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Content : UI_Base
{
    enum Contents
    {
        Content,
        Image,
        Textinfo,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
    }

    void Start()
    {
        Init();
        if (State == ContentState.Possible)
        {
            gameObject.AddUIEvent((data) => MonsterChoose());
        }
    }

    void Update()
    {
        
    }



    void MonsterChoose()
    {
        var training = GetComponentInParent<UI_Training>();

        if (training.resumeCount > 0 && State == ContentState.Possible)
        {
            State = ContentState.Chosen;
            training.ResumeCountUpdate(-1);
        }
        else if (State == ContentState.Chosen)
        {
            State = ContentState.Possible;
            training.ResumeCountUpdate(1);
        }
    }



    public enum ContentState
    {
        Nothing,
        Possible,
        Impossible,
        Chosen,
    }

    ContentState _state;
    public ContentState State
    { 
        get { return _state; }
        set 
        { 
            _state = value;
            GetComponent<Image>().color = ColorTint(_state);
        }
    }
    Color32 ColorTint(ContentState _state)
    {
        switch (_state)
        {
            case ContentState.Possible:
                return new Color32(100, 255, 100, 175);

            case ContentState.Impossible:
                return new Color32(255, 100, 100, 175);

            case ContentState.Chosen:
                return new Color32(100, 100, 255, 175);

            default:
                return new Color32(255, 255, 255, 175);
        }
    }

}
