using UnityEngine;

public class SpecialEgg : Facility
{

    public enum EggType
    {
        Lv_1 = 2901,
        Lv_2 = 2902,
        Lv_3 = 2903,

        Dog = 2910,
        Dragon = 2911,
        Slime = 2912,
    }


    public EggType Egg { get; set; }

    public void SetEggData(SO_Facility SO_data)
    {
        //EventType = SO_data.Type;
        Name = SO_data.labelName.SetTextColorTag(Define.TextColor.SkyBlue);
        Detail_KR = SO_data.detail;

        Egg = (EggType)SO_data.id;
        InteractionOfTimes = SO_data.interactionOfTimes;
    }



    public override void Init_Personal()
    {
        //InteractionOfTimes = 10000;
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = true;
        isClearable = false;
    }






    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 3, UserData.Instance.LocaleText("Event_Egg"), ap: 0, mp: 0, hp: 0));

            Managers.UI.ClearAndShowPopUp<UI_GameOver>();
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }
}
