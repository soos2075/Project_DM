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


    public SO_Technical Data { get; set; }

    public abstract int InstanceDate { get; set; }
    public abstract int Cycle { get; set; }
    public Action<int> MainAction { get; set; }


    //? 몇 번 능력이 발동됐는지 정보. 사실 쓸데는 없지만 알려줘도 좋을듯. 신전 기부 카운팅할 때 일단 써야함.
    public int InteractionCounter { get; set; }

    protected enum DayType
    {
        Day,
        Night,
    }

    protected void AddTurnEvent(Action<int> _action, DayType dayType)
    {
        switch (dayType)
        {
            case DayType.Day:
                Main.Instance.DayActions.Add(_action);
                break;

            case DayType.Night:
                Main.Instance.NightActions.Add(_action);
                break;

        }
    }
    protected void RemoveTurnEvent(Action<int> _action, DayType dayType)
    {
        switch (dayType)
        {
            case DayType.Day:
                Main.Instance.DayActions.Remove(_action);
                break;

            case DayType.Night:
                Main.Instance.NightActions.Remove(_action);
                break;

        }
    }

    //? 추가적으로 가져야할 데이터 : 설치된 위치 (UI_Technical) / 마우스 올렸을 때 나타나는 UI에 입력할 정보

    protected abstract void MainEvent(int day);
    public abstract void RemoveTechnical();

    public TechnicalFloor parent;


    public void AddCollectionPoint()
    {
        var collection = CollectionManager.Instance.Get_Collection(Data);
        if (collection != null)
        {
            collection.AddPoint();
        }
    }
}
