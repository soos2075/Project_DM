using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Herb : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)herbType); } set { herbType = (HerbType)value; } }

    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Interaction;
        Name_prefab = name;

        Init_Herb();
    }

    public enum HerbType
    {
        Low = 0,
        Pumpkin = 1,
        High = 2,
    }
    public HerbType herbType { get; set; } = HerbType.Low;

    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;


    void Init_Herb()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(herbType.ToString(), "Entry");

        switch (herbType)
        {
            case HerbType.Low:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 1;
                }
                Name = "하급 약초";
                Detail_KR = "흔한 약초입니다. 소량이지만 마나를 머금고 있고 던전에서만 얻을 수 있어 인기는 많습니다.";
                durationTime = 3;
                ap_value = 1;
                mp_value = 9;
                hp_value = 0;
                break;

            case HerbType.Pumpkin:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }
                Name = "마나 펌킨";
                Detail_KR = "마나를 지닌 호박입니다. 최고급 식재료로 사용되며 기타 여러 합성에도 사용됩니다.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 12;
                hp_value = 0;
                break;

            case HerbType.High:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }
                Name = "고급 약초";
                Detail_KR = "꽤 귀한 약초입니다. 고급 포션의 재료로 사용된다고 하네요.";
                durationTime = 3;
                ap_value = 1;
                mp_value = 35;
                hp_value = 0;
                break;
        }
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            int changeMP = mp_value;

            if (npc.GetType() == typeof(Herbalist))
            {
                changeMP = mp_value;
            }
            else if(npc.GetType() == typeof(Miner))
            {
                changeMP = (int)(mp_value * 0.5f);
            }
            else if (npc.GetType() == typeof(Adventurer))
            {
                changeMP = (int)(mp_value * 0.3f);
            }
            else
            {
                changeMP = (int)(mp_value * 0.3f);
            }


            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "약초 채집중...", ap: ap_value, mp: changeMP, hp: hp_value));
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
