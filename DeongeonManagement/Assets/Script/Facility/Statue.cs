using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : Facility
{
    public override void Init_Personal()
    {
        statueType = (StatueType)OptionIndex;
    }

    public enum StatueType
    {
        Gold = 3900,
        Mana = 3901,
    }
    public StatueType statueType { get; set; }




    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }

    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        if (Main.Instance.Player_AP <= 0)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.GetLocaleText("Message_No_AP");
            return;
        }

        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        switch (statueType)
        {
            case StatueType.Gold:
                ui.SetText(UserData.Instance.GetLocaleText("Confirm_GoldStatue"));
                StartCoroutine(WaitForAnswer_Gold(ui));
                break;

            case StatueType.Mana:
                ui.SetText(UserData.Instance.GetLocaleText("Confirm_ManaStatue"));
                StartCoroutine(WaitForAnswer_Mana(ui));
                break;
        }
    }

    IEnumerator WaitForAnswer_Gold(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            int gold = Random.Range(25, 100);
            Main.Instance.Player_AP--;
            Main.Instance.CurrentDay.AddGold(gold);

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{gold} {UserData.Instance.GetLocaleText("Message_Get_Gold")}";
        }
    }
    IEnumerator WaitForAnswer_Mana(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            int mana = Random.Range(50, 200);
            Main.Instance.Player_AP--;
            Main.Instance.CurrentDay.AddMana(mana);

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{mana} {UserData.Instance.GetLocaleText("Message_Get_Mana")}";
        }
    }


}
