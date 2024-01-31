using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusUp : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Images
    {
        Panel,
        ProfilePanel,
        Profile,
    }
    enum Texts
    {
        Lv,
        Name,
        Status,
        State,
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        StartCoroutine(TestFunc());

        //ShowDefault();
        //ShowText();
    }


    [System.Obsolete]
    IEnumerator TestFunc()
    {
        var mon = Managers.Placement.CreatePlacementObject("Monster/Slime", null, Define.PlacementType.Monster);
        Managers.Monster.AddMonster(mon as Monster);
        TargetMonster(mon as Monster);

        yield return new WaitForEndOfFrame();
        ShowDefault();
    }


    Monster monster;

    int before_lv;
    int before_hp;
    int before_atk;
    int before_def;
    int before_agi;
    int before_luk;

    int after_lv;
    int after_hp;
    int after_atk;
    int after_def;
    int after_agi;
    int after_luk;


    string show_lv;
    string show_hp;
    string show_atk;
    string show_def;
    string show_agi;
    string show_luk;
    public void TargetMonster(Monster _monster)
    {
        monster = _monster;

        before_lv = monster.LV;
        before_hp = monster.HP;
        before_atk = monster.ATK;
        before_def = monster.DEF;
        before_agi = monster.AGI;
        before_luk = monster.LUK;
    }

    void AfterStatus()
    {
        after_lv =monster.LV;
        after_hp =monster.HP;
        after_atk = monster.ATK;
        after_def = monster.DEF;
        after_agi = monster.AGI;
        after_luk = monster.LUK;
    }


    void ShowDefault()
    {
        GetImage(((int)Images.Profile)).sprite = monster.Data.sprite;
        GetTMP(((int)Texts.Name)).text = monster.Data.Name;

        GetTMP(((int)Texts.Lv)).text = $"LV.{monster.LV}";
        GetTMP(((int)Texts.Status)).text = $"HP : {monster.HP}/{monster.HP}\n" +
            $"ATK : {monster.ATK} \tDEF : {monster.DEF} \n" +
            $"AGI : {monster.AGI} \tLUK : {monster.LUK}";
    }



    void ShowUpStatus()
    {

    }

}
