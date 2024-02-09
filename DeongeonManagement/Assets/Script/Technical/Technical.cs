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


    public TechnicalData Data { get; set; }

    public abstract int InstanceDate { get; set; }
    public abstract int Cycle { get; set; }


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

    //? �߰������� �������� ������ : ��ġ�� ��ġ (UI_Technical) / ���콺 �÷��� �� ��Ÿ���� UI�� �Է��� ����

    protected abstract void MainEvent(int day);

    public TechnicalFloor parent;

}
