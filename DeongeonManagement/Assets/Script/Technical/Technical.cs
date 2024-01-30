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

    //? �߰������� �������� ������ : ��ġ�� ��ġ (UI_Technical) / ���콺 �÷��� �� ��Ÿ���� UI�� �Է��� ����

    public abstract string Name_KR { get; set; }

    public abstract string Detail { get; set; }

    protected abstract void MainEvent(int day);

    public UI_Technical parent;

}
