using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomEventContent", menuName = "ScriptableObjects/RandomEvent")]
public class SO_RandomEventContent : ScriptableObject
{
    public int ID;

    //? 이벤트 점수
    public int value;

    //? 이벤트 풀
    public RandomEventPool pool;
    //? 이벤트 타입 (지속 / 발동)
    public RandomEventType type;
    //? 좋은, 나쁜, 중립, 특별
    public RandomEventValue eventValue;

    //? 지속시간 (발동형이면 0)
    public int continuousDays;



    //? 이벤트 내용
    public string description;

    //? 활성화 상태
    public bool isActive;

    //? 잠김 상태
    public bool isOpened;

    //? 반복가능 여부
    public bool isRefeat;



    ////? 이 이벤트의 현재 실행 횟수
    //public int currentPlayCount;

    ////? 이 이벤트의 최대 실행 가능 횟수
    //public int maxPlayCount;

    ////? 이 이벤트의 누적 실행 횟수
    //public int accumPlayCount;
}
