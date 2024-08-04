using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : Facility, IWall
{
    public override void Init_Personal()
    {
        statueType = (StatueType)CategoryIndex;
    }

    public enum StatueType
    {
        Gold = 3900,
        Mana = 3901,
        Dog = 3902,
        
    }
    public StatueType statueType { get; set; }




    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }


    public override void MouseMoveEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;

        switch (statueType)
        {
            case StatueType.Gold:
                SLA_ObjectManager.Instance.SetLabel("Statue_Gold", "Gold", "Interaction");
                break;

            case StatueType.Mana:
                SLA_ObjectManager.Instance.SetLabel("Statue_Mana", "Mana", "Interaction");
                break;

            case StatueType.Dog:
                SLA_ObjectManager.Instance.SetLabel("Statue_Dog", "Dog", "Interaction");
                break;
        }
    }
    public override void MouseExitEvent()
    {
        if (Main.Instance.Management == false) return;

        switch (statueType)
        {
            case StatueType.Gold:
                SLA_ObjectManager.Instance.SetLabel("Statue_Gold", "Gold", "Entry");
                break;

            case StatueType.Mana:
                SLA_ObjectManager.Instance.SetLabel("Statue_Mana", "Mana", "Entry");
                break;

            case StatueType.Dog:
                SLA_ObjectManager.Instance.SetLabel("Statue_Dog", "Dog", "Entry");
                break;
        }
    }

    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;

        if (Main.Instance.Player_AP <= 0)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return;
        }

        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        switch (statueType)
        {
            case StatueType.Gold:
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_GoldStatue")}\n" +
                    $"<size=25>(1{UserData.Instance.LocaleText("AP")} {UserData.Instance.LocaleText("필요")})");
                StartCoroutine(WaitForAnswer_Gold(ui));
                break;

            case StatueType.Mana:
                ui.SetText($"{UserData.Instance.LocaleText("Confirm_ManaStatue")}\n" +
                    $"<size=25>(1{UserData.Instance.LocaleText("AP")} {UserData.Instance.LocaleText("필요")})");
                StartCoroutine(WaitForAnswer_Mana(ui));
                break;
        }
    }

    IEnumerator WaitForAnswer_Gold(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            int gold = Random.Range(50, 100);
            Main.Instance.Player_AP--;
            Main.Instance.CurrentDay.AddGold(gold, Main.DayResult.EventType.Etc);

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{gold} {UserData.Instance.LocaleText("Message_Get_Gold")}";
        }
    }
    IEnumerator WaitForAnswer_Mana(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            int mana = Random.Range(75, 150);
            Main.Instance.Player_AP--;
            Main.Instance.CurrentDay.AddMana(mana, Main.DayResult.EventType.Etc);

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{mana} {UserData.Instance.LocaleText("Message_Get_Mana")}";
        }
    }

    IEnumerator WaitForAnswer_Dog(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Main.Instance.Player_AP--;
            //? 행동력 이벤트는 추후에 기획에 따라

            Managers.UI.ClosePopupPick(confirm);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();

        }
    }

}
