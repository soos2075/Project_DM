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

    //enum Contents
    //{
    //    Title,
    //    Content,
    //}

    enum TMP_Texts
    {
        Title,

        NeedRank,
        NeedMana,
        NeedGold,
    }


    public override void Init()
    {
        SetCanvasWorldSpace();

        //Bind<TextMeshProUGUI>(typeof(Contents));
        Bind<TextMeshProUGUI>(typeof(TMP_Texts));

        //GetTMP(((int)Contents.Content)).text = $"{UserData.Instance.LocaleText("필요")} {UserData.Instance.LocaleText("Mana")} : {NeedMana}\n" +
        //    $"{UserData.Instance.LocaleText("필요")} {UserData.Instance.LocaleText("Gold")} : {NeedGold}\n" +
        //    $"{UserData.Instance.LocaleText("필요")} {UserData.Instance.LocaleText("Rank")} : {NeedLv}";



        GetTMP(((int)TMP_Texts.Title)).text = $"{UserData.Instance.LocaleText("던전 확장")}";
        GetTMP(((int)TMP_Texts.NeedRank)).text = $"{(Define.DungeonRank)NeedLv}";
        GetTMP(((int)TMP_Texts.NeedMana)).text = $"{NeedMana}";
        GetTMP(((int)TMP_Texts.NeedGold)).text = $"{NeedGold}";


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

        if (NeedGold <= Main.Instance.Player_Gold && NeedMana <= Main.Instance.Player_Mana && NeedLv <= Main.Instance.DungeonRank)
        {
            var ui = Managers.UI.ShowPopUp<UI_Confirm>();
            ui.SetText(UserData.Instance.LocaleText("Confirm_Expansion"), () => StartCoroutine(WaitForAnswer(ui)));
        }
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        Main.Instance.CurrentDay.SubtractGold(NeedGold, Main.DayResult.EventType.Etc);
        Main.Instance.CurrentDay.SubtractMana(NeedMana, Main.DayResult.EventType.Etc);
        //Main.Instance.Player_AP--;

        Main.Instance.Basement_Expansion();
        FindObjectOfType<UI_Management>().DungeonExpansion();

        //Managers.UI.ClosePopUp(confirm);

        yield return null;
        yield return null;

        //? 4층 확장시 4층으로 전이진이 옮겨지는 이벤트
        Managers.Dialogue.ShowDialogueUI(DialogueName.Expansion_4, Main.Instance.Player);
        //Managers.UI.ClosePopUp(confirm);
        Managers.Resource.Destroy(gameObject);
        //yield return null;
        //yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);
    }
}
