using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Expansion_Floor : UI_Base, IWorldSpaceUI
{
    void Start()
    {
        Init();
    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, false);
    }

    enum Contents
    {
        Title,
        Content,
    }


    public override void Init()
    {
        SetCanvasWorldSpace();

        Bind<TextMeshProUGUI>(typeof(Contents));

        GetTMP(((int)Contents.Content)).text = $"필요 마나 : {NeedMana}\n필요 골드 : {NeedGold}\n필요 레벨 : {NeedLv}";

        gameObject.AddUIEvent((data) => ExpansionCheck());
    }


    public int FloorIndex { get; set; }
    public int NeedGold;
    public int NeedMana;
    public int NeedLv;


    public void SetContents(int floor, int gold, int mana, int lv)
    {
        FloorIndex = floor;
        NeedGold = gold;
        NeedMana = mana;
        NeedLv = lv;
    }


    void ExpansionCheck()
    {
        if (!Main.Instance.Management)
        {
            return;
        }

        if (NeedGold <= Main.Instance.Player_Gold && NeedMana <= Main.Instance.Player_Mana && NeedLv <= Main.Instance.DungeonRank && Main.Instance.Player_AP > 0)
        {
            var ui = Managers.UI.ShowPopUp<UI_Confirm>();
            ui.SetText(UserData.Instance.GetLocaleText("Confirm_Expansion"));
            StartCoroutine(WaitForAnswer(ui));
        }
    }

   
    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Main.Instance.CurrentDay.SubtractGold(NeedGold);
            Main.Instance.CurrentDay.SubtractMana(NeedMana);
            Main.Instance.Player_AP--;

            Main.Instance.Basement_Expansion();
            FindObjectOfType<UI_Management>().DungeonExpansion();
            Managers.Resource.Destroy(gameObject);
        }
        //else if (confirm.GetAnswer() == UI_Confirm.State.No)
        //{

        //}
    }
}
