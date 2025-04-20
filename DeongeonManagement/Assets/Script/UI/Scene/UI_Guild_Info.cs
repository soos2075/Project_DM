using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Guild_Info : UI_Base
{

    enum Texts
    {
        TMP_Mana,
        TMP_Gold,
        TMP_Pop,
        TMP_Danger,
    }
    enum Images
    {
        MainUI,
        Image_Day_NX,
        Image_Day_XN,
        Image_Rank,
    }

    enum Objects
    {
        AP,
    }


    void Start()
    {
        Init();
    }
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(Objects));

        saveData = Managers.Data.GetData("Temp_GuildSave");

        AP_Refresh();
    }

    DataManager.SaveData saveData;


    private void LateUpdate()
    {
        Texts_Refresh();
    }


    public void Texts_Refresh()
    {
        int tens = saveData.mainData.turn / 10;
        int ones = saveData.mainData.turn % 10;

        

        GetImage((int)Images.Image_Day_NX).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Number", $"{tens}");
        GetImage((int)Images.Image_Day_XN).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Number", $"{ones}");

        GetImage((int)Images.Image_Rank).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Rank",
            $"{(Define.DungeonRank)saveData.mainData.DungeonLV}");


        GetTMP(((int)Texts.TMP_Mana)).text = $"{saveData.mainData.Player_Mana}";
        GetTMP(((int)Texts.TMP_Gold)).text = $"{saveData.mainData.Player_Gold}";
        GetTMP(((int)Texts.TMP_Pop)).text = $"{saveData.mainData.FameOfDungeon}";
        GetTMP(((int)Texts.TMP_Danger)).text = $"{saveData.mainData.DangerOfDungeon}";
    }

    public void AP_Refresh()
    {
        if (GetObject(((int)Objects.AP)) == null) return;

        var pos = GetObject(((int)Objects.AP)).transform;

        for (int i = pos.childCount - 1; i >= 0; i--)
        {
            Destroy(pos.GetChild(i).gameObject);
        }

        for (int i = 0; i < saveData.mainData.Player_AP; i++)
        {
            Managers.Resource.Instantiate("UI/PopUp/Element/behaviour", pos);
        }
    }
}
