using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomEventContent", menuName = "ScriptableObjects/RandomEvent")]
public class SO_RandomEventContent : ScriptableObject
{
    public int ID;

    //? �̺�Ʈ ����
    public int value;

    //? �̺�Ʈ Ǯ
    public RandomEventPool pool;
    //? �̺�Ʈ Ÿ�� (���� / �ߵ�)
    public RandomEventType type;
    //? ����, ����, �߸�, Ư��
    public RandomEventValue eventValue;

    //? ���ӽð� (�ߵ����̸� 0)
    public int continuousDays;



    //? �̺�Ʈ ����
    public string description;

    //? Ȱ��ȭ ����
    public bool isActive;

    //? ��� ����
    public bool isOpened;

    //? �ݺ����� ����
    public bool isRefeat;



    ////? �� �̺�Ʈ�� ���� ���� Ƚ��
    //public int currentPlayCount;

    ////? �� �̺�Ʈ�� �ִ� ���� ���� Ƚ��
    //public int maxPlayCount;

    ////? �� �̺�Ʈ�� ���� ���� Ƚ��
    //public int accumPlayCount;
}
