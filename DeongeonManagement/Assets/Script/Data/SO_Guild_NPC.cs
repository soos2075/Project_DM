using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Guild_NPC", menuName = "SO_Guild_NPC")]
public class SO_Guild_NPC : ScriptableObject
{

    // ���� �̸�
    [Header("LabelName")]
    public string LabelName;

    // ���� ID (�������� ��ȭ�� ���� �� ���ư� ��ȣ)
    [Header("Index")]
    public int Original_Index;


    [Header("ActiveDay")]
    public Guild_DayOption DayOption;


    // ����Ʈ�� �ִٸ� ������ ��ȣ
    [Header("���� ����Ʈ")]
    public List<int> InstanceQuestList = new List<int>();

    // ������ ���� ����Ʈ
    [Header("�ɼ�")]
    public List<int> OptionList = new List<int>();


    // �Ϸ���

}
