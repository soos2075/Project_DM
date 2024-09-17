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
        //Status,
        State,

        Status_HP,
        Status_ATK,
        Status_DEF,
        Status_AGI,
        Status_LUK,
    }


    public string StateText { get; set; }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        //StartCoroutine(TestFunc());

        GetImage(((int)Images.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage(((int)Images.ProfilePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);

        //? monster는 이 컴포넌트를 생성하고 바로 TargetMonster를 호출해서 넣어줘야함

        if (monster == null)
        {
            ClosePopUp();
            return;
        }

        StartCoroutine(WaitFrame());

        SoundManager.Instance.PlaySound($"SFX/LevelUp");
    }


    IEnumerator WaitFrame()
    {
        yield return new WaitForEndOfFrame();
        AfterStatus();
        ShowDefault();
        ShowUpStatus();

        if (string.IsNullOrEmpty(StateText) == false)
        {
            GetTMP(((int)Texts.State)).text = StateText;
        }
    }

    void ShowDefault()
    {
        GetImage(((int)Images.Profile)).sprite = 
            Managers.Sprite.Get_SLA(SpriteManager.Library.Monster, monster.Data.SLA_category, monster.Data.SLA_label);
        GetTMP(((int)Texts.Name)).text = monster.Data.labelName;
    }
    void ShowUpStatus()
    {
        GetTMP(((int)Texts.Lv)).text = $"LV.{show_lv}";

        //GetTMP(((int)Texts.Status)).text = $"HP\t{show_hp}\n" +
        //    $"ATK\t{show_atk} \tDEF\t{show_def} \n" +
        //    $"AGI\t{show_agi} \tLUK\t{show_luk}";

        GetTMP(((int)Texts.Status_HP)).text = $"{show_hp}";
        GetTMP(((int)Texts.Status_ATK)).text = $"{show_atk}";
        GetTMP(((int)Texts.Status_DEF)).text = $"{show_def}";
        GetTMP(((int)Texts.Status_AGI)).text = $"{show_agi}";
        GetTMP(((int)Texts.Status_LUK)).text = $"{show_luk}";
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
        before_hp = monster.HP_Max;
        before_atk = monster.ATK;
        before_def = monster.DEF;
        before_agi = monster.AGI;
        before_luk = monster.LUK;

        show_lv = monster.LV.ToString();
        show_hp = monster.HP_Max.ToString();
        show_atk = monster.ATK.ToString();
        show_def = monster.DEF.ToString();
        show_agi = monster.AGI.ToString();
        show_luk = monster.LUK.ToString();
    }

    public void TargetMonster(Monster _monster, int _lv, int _hpMax, int _atk, int _def, int _agi, int _luk)
    {
        monster = _monster;

        before_lv = _lv;
        before_hp = _hpMax;
        before_atk = _atk;
        before_def = _def;
        before_agi = _agi;
        before_luk = _luk;

        show_lv = _lv.ToString();
        show_hp = _hpMax.ToString();
        show_atk = _atk.ToString();
        show_def = _def.ToString();
        show_agi = _agi.ToString();
        show_luk = _luk.ToString();
    }

    void AfterStatus()
    {
        after_lv = monster.LV;
        after_hp = monster.HP_Max;
        after_atk = monster.ATK;
        after_def = monster.DEF;
        after_agi = monster.AGI;
        after_luk = monster.LUK;

        StatusComparison();
    }

    void StatusComparison()
    {
        if (before_lv < after_lv)
        {
            show_lv = $"{after_lv}".SetTextColorTag(Define.TextColor.Plus_Green);
            show_lv += $" ▲{after_lv - before_lv}".SetTextColorTag(Define.TextColor.Plus_Blue);
        }
        else if (before_lv > after_lv)
        {
            show_lv = $"{after_lv}".SetTextColorTag(Define.TextColor.npc_red);
            show_lv += $" ▼{Mathf.Abs(after_lv - before_lv)}".SetTextColorTag(Define.TextColor.yellow);
        }


        if (before_hp < after_hp)
        {
            show_hp = $"{after_hp}".SetTextColorTag(Define.TextColor.Plus_Green);
            show_hp += $" ▲{after_hp - before_hp}".SetTextColorTag(Define.TextColor.Plus_Blue);
        }
        else if (before_hp > after_hp)
        {
            show_hp = $"{after_hp}".SetTextColorTag(Define.TextColor.npc_red);
            show_hp += $" ▼{Mathf.Abs(after_hp - before_hp)}".SetTextColorTag(Define.TextColor.yellow);
        }


        if (before_atk < after_atk)
        {
            show_atk = $"{after_atk}".SetTextColorTag(Define.TextColor.Plus_Green);
            show_atk += $" ▲{after_atk - before_atk}".SetTextColorTag(Define.TextColor.Plus_Blue);
        }
        else if (before_atk > after_atk)
        {
            show_atk = $"{after_atk}".SetTextColorTag(Define.TextColor.npc_red);
            show_atk += $" ▼{Mathf.Abs(after_atk - before_atk)}".SetTextColorTag(Define.TextColor.yellow);
        }


        if (before_def < after_def)
        {
            show_def = $"{after_def}".SetTextColorTag(Define.TextColor.Plus_Green);
            show_def += $" ▲{after_def - before_def}".SetTextColorTag(Define.TextColor.Plus_Blue);
        }
        else if (before_def > after_def)
        {
            show_def = $"{after_def}".SetTextColorTag(Define.TextColor.npc_red);
            show_def += $" ▼{Mathf.Abs(after_def - before_def)}".SetTextColorTag(Define.TextColor.yellow);
        }


        if (before_agi < after_agi)
        {
            show_agi = $"{after_agi}".SetTextColorTag(Define.TextColor.Plus_Green);
            show_agi += $" ▲{after_agi - before_agi}".SetTextColorTag(Define.TextColor.Plus_Blue);
        }
        else if (before_agi > after_agi)
        {
            show_agi = $"{after_agi}".SetTextColorTag(Define.TextColor.npc_red);
            show_agi += $" ▼{Mathf.Abs(after_agi - before_agi)}".SetTextColorTag(Define.TextColor.yellow);
        }


        if (before_luk < after_luk)
        {
            show_luk = $"{after_luk}".SetTextColorTag(Define.TextColor.Plus_Green);
            show_luk += $" ▲{after_luk - before_luk}".SetTextColorTag(Define.TextColor.Plus_Blue);
        }
        else if (before_luk > after_luk)
        {
            show_luk = $"{after_luk}".SetTextColorTag(Define.TextColor.npc_red);
            show_luk += $" ▼{Mathf.Abs(after_luk - before_luk)}".SetTextColorTag(Define.TextColor.yellow);
        }
    }



    public override bool EscapeKeyAction()
    {
        return true;
    }


    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }

}
