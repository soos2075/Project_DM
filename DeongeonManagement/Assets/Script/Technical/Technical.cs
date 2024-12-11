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


    //? �� �� �ɷ��� �ߵ��ƴ��� ����. ��� ������ ������ �˷��൵ ������. ���� ��� ī������ �� �ϴ� �����.
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

    //? �߰������� �������� ������ : ��ġ�� ��ġ (UI_Technical) / ���콺 �÷��� �� ��Ÿ���� UI�� �Է��� ����

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
