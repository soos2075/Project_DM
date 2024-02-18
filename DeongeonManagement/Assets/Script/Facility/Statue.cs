using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.PlayerEvent;
        InteractionOfTimes = 10000;
        Name_prefab = name;

        Init_TypeSelect();
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }



    public enum StatueType
    {
        Gold,
        Mana,
    }
    public StatueType statueType { get; set; }

    void Init_TypeSelect()
    {
        switch (statueType)
        {
            case StatueType.Gold:
                Name = "금빛 천사상";
                Detail_KR = "황금으로 만들어진 천사상입니다. 기도를 올리면 소량의 골드를 얻습니다.";
                break;

            case StatueType.Mana:
                Name = "은빛 천사상";
                Detail_KR = "순은으로 만들어진 천사상입니다. 기도를 올리면 소량의 마나를 얻습니다.";
                break;
        }
    }

    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        if (Main.Instance.Player_AP <= 0)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "행동력이 부족합니다";
            return;
        }

        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        switch (statueType)
        {
            case StatueType.Gold:
                ui.SetText("금빛 조각상에게 기도할까요?");
                StartCoroutine(WaitForAnswer_Gold(ui));
                break;

            case StatueType.Mana:
                ui.SetText("은빛 조각상에게 기도할까요?");
                StartCoroutine(WaitForAnswer_Mana(ui));
                break;
        }
    }

    IEnumerator WaitForAnswer_Gold(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            int gold = Random.Range(5, 16) * (Main.Instance.Turn + 1);
            Main.Instance.Player_AP--;
            Main.Instance.ClickEvent_Gold(gold);

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"여신상에 기도를 올려 {gold} 골드를 얻었습니다.";
        }
    }
    IEnumerator WaitForAnswer_Mana(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            int mana = Random.Range(10, 31) * (Main.Instance.Turn + 1);
            Main.Instance.Player_AP--;
            Main.Instance.ClickEvent_Mana(mana);

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"여신상에 기도를 올려 {mana} 마나를 얻었습니다.";
        }
    }


}
