using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Guild_NPC", menuName = "SO_Guild_NPC")]
public class SO_Guild_NPC : ScriptableObject
{

    // 고유 이름
    [Header("LabelName")]
    public string LabelName;

    // 고유 ID (진행중인 대화가 없을 때 돌아갈 번호)
    [Header("Index")]
    public int Original_Index;


    [Header("ActiveDay")]
    public Guild_DayOption DayOption;


    // 퀘스트가 있다면 더해줄 번호
    [Header("현재 퀘스트")]
    public List<int> InstanceQuestList = new List<int>();

    // 선택지 제공 리스트
    [Header("옵션")]
    public List<int> OptionList = new List<int>();


    // 완료목록

}
