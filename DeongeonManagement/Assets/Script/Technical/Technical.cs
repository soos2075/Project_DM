using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Technical : MonoBehaviour
{
    private void Start()
    {
        Init();
    }


    public virtual void Init()
    {

    }


    protected enum DayType
    {
        Day,
        Night,
    }

    protected void AddTurnEvent(Action<int> action, DayType dayType)
    {

        switch (dayType)
        {
            case DayType.Day:
                Main.Instance.DayActions.Add(action);
                break;

            case DayType.Night:
                Main.Instance.NightActions.Add(action);
                break;

        }
    }

    //? 추가적으로 가져야할 데이터 : 설치된 위치 (UI_Technical) / 마우스 올렸을 때 나타나는 UI에 입력할 정보

    public abstract string Name_KR { get; set; }

    public abstract string Detail { get; set; }

    protected abstract void MainEvent(int day);

    public UI_Technical parent;

}
