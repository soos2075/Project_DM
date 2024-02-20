using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)mineralType); } set { mineralType = (MineralType)value; } }

    public override void FacilityInit()
    {
        Type = FacilityType.Mineral;
        Name_prefab = name;

        Init_Mineral();
    }

    public Sprite[] mineralSprites;

    public enum MineralType
    {
        Rock = 0,
        Sand = 1,
        Stone = 2,
        Iron = 3,
        Coal = 4,
        Diamond = 5,
    }
    public MineralType mineralType { get; set; } = MineralType.Rock;

    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;

    void Init_Mineral()
    {

        GetComponentInChildren<SpriteRenderer>().sprite = mineralSprites[(int)mineralType];

        switch (mineralType)
        {
            case MineralType.Rock:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "흔한 바위";
                Detail_KR = "흔한 던전의 바위입니다. 그렇지만 지상의 바위와는 완전히 다른 물질이에요.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 3;
                hp_value = 0;
                break;

            case MineralType.Sand:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 4;
                }

                Name = "모래바위";
                Detail_KR = "쉽게 부서져 모래가 되는 바위에요. 마나를 가지고 있어 쉽게 뭉쳐지고 물과 잘 섞여 쓰임새가 많아요.";
                durationTime = 1;
                ap_value = 1;
                mp_value = 4;
                hp_value = 0;
                break;

            case MineralType.Stone:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "단단한 바위";
                Detail_KR = "단단한 바위입니다. 튼튼해서 부수려면 꽤나 많은 체력이 필요할 거에요. 건축재료로 사용되어 인기가 많습니다.";
                durationTime = 3;
                ap_value = 1;
                mp_value = 3;
                hp_value = 0;
                break;

            case MineralType.Iron:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "마나 철광석";
                Detail_KR = "순도가 높은 철을 포함하고 있는 철광석입니다. 무기를 만들 때도 쓰이고 각종 장신구에도 쓰여요.";
                durationTime = 5;
                ap_value = 2;
                mp_value = 15;
                hp_value = 0;
                break;

            case MineralType.Coal:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "석탄";
                Detail_KR = "불이 잘 붙는 석탄입니다. 마나를 먹은 석탄은 화력도 유지력도 훨씬 길기 때문에 지상에선 고급 연료입니다.";
                durationTime = 3;
                ap_value = 2;
                mp_value = 7;
                hp_value = 0;
                break;

            case MineralType.Diamond:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 2;
                }

                Name = "다이아몬드";
                Detail_KR = "매우 귀한 물질인 다이아몬드입니다. 마나의 순도가 높기때문에 마법무기의 핵심 부분이에요. 매우 단단하기 때문에 캐기도 힘들어요.";
                durationTime = 5;
                ap_value = 3;
                mp_value = 20;
                hp_value = 0;
                break;
        }
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "광석 채굴중...", ap: ap_value, mp: mp_value, hp: hp_value));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] 철거할까요? ");
        StartCoroutine(WaitForAnswer(ui));
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            GameManager.Facility.RemoveFacility(this);
        }
    }

}
