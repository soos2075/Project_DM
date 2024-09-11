using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Orb : MonoBehaviour
{

    public enum OrbType
    {
        green,
        blue,
        yellow,
        red,
    }

    public OrbType _OrbType;
    public bool isActive;

    SpriteResolver Resolver;
    Animator anim;

    private void Start()
    {
        Resolver = GetComponent<SpriteResolver>();
        anim = GetComponent<Animator>();
        Init_State();
    }


    void Init_State()
    {
        switch (_OrbType)
        {
            case OrbType.green:
                if (GameManager.Buff.CurrentBuff.Orb_green > 0) isActive = true;
                break;

            case OrbType.blue:
                if (GameManager.Buff.CurrentBuff.Orb_blue > 0) isActive = true;
                break;

            case OrbType.yellow:
                if (GameManager.Buff.CurrentBuff.Orb_yellow > 0) isActive = true;
                break;

            case OrbType.red:
                if (GameManager.Buff.CurrentBuff.Orb_red > 0) isActive = true;
                break;
        }

        if (isActive)
        {
            anim.enabled = true;
        }
    }



    bool ReturnCheck() //? 이벤트 발생 조건
    {
        if (Main.Instance.Management == false) return true;
        if (Main.Instance.CurrentAction != null) return true;
        if (UserData.Instance.GameMode == Define.GameMode.Stop) return true;
        if (Time.timeScale == 0) return true;


        if (Main.Instance.Turn <= 5) return true;

        return false;
    }


    private void OnMouseEnter()
    {
        EnterReserve = StartCoroutine(Wait_Enter());
    }

    IEnumerator Wait_Enter()
    {
        yield return new WaitUntil(() => ReturnCheck() == false);

        if (isActive)
        {
            //Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active_line");
        }
        else
        {
            Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Inactive_line");
        }
    }

    Coroutine EnterReserve;

    private void OnMouseExit()
    {
        //if (ReturnCheck()) return;
        //Debug.Log(_OrbType + "Exit");

        if (EnterReserve != null)
        {
            StopCoroutine(EnterReserve);
        }
        StartCoroutine(Wait_Exit());
    }

    IEnumerator Wait_Exit()
    {
        yield return new WaitUntil(() => ReturnCheck() == false);

        if (isActive)
        {
            //Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active");
        }
        else
        {
            Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Inactive");
        }
    }



    private void OnMouseUpAsButton()
    {
        if (ReturnCheck()) return;

        //Debug.Log(_OrbType + "Click");
        StartCoroutine(WaitFrame());
    }


    IEnumerator WaitFrame() //? ClearPanel이 타일 이외의 지역 좌클릭 = CloseAll 로 해놨기때문에 1프레임 기다려야함
    {
        yield return new WaitForEndOfFrame();
        if (isActive)
        {
            AlreadyActive();
        }
        else
        {
            MouseClickEvent();
        }

    }


    void AlreadyActive()
    {
        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        string msg = "";
        switch (_OrbType)
        {
            case OrbType.green:
                msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green_Detail")}";
                break;

            case OrbType.yellow:
                msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow_Detail")}";
                break;

            case OrbType.blue:
                msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")}";
                break;

            case OrbType.red:
                msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")}";
                break;
        }
        ui.SetText(msg, () => { });
    }



    void MouseClickEvent()
    {
        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        switch (_OrbType)
        {
            case OrbType.green:
                var green = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green")}\n" +
                    $"({UserData.Instance.LocaleText_Tooltip("Orb_Green_Detail")})";
                ui.SetText(green, () => Confirm(ap: 2, mana: 750, gold: 250));
                ui.SetMode_Calculation((int)Define.DungeonRank.F, "750", "250", "2");
                break;


            case OrbType.yellow:
                var yellow = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow")}\n" +
                    $"({UserData.Instance.LocaleText_Tooltip("Orb_Yellow_Detail")})";
                ui.SetText(yellow, () => Confirm(ap: 2, mana: 750, gold: 250));
                ui.SetMode_Calculation((int)Define.DungeonRank.F, "750", "250", "2");
                break;


            case OrbType.blue:
                var blue = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue")}\n" +
                    $"({UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")})";
                ui.SetText(blue, () => Confirm(mana: 3000, gold: 0, ap: 3));
                ui.SetMode_Calculation((int)Define.DungeonRank.D, "3000", "0", "3");
                break;


            case OrbType.red:
                var red = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red")}\n" +
                    $"({UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")})";
                ui.SetText(red, () => Confirm(mana: 1000, gold: 2000, ap: 4));
                ui.SetMode_Calculation((int)Define.DungeonRank.C, "1000", "2000", "4");
                break;
        }
    }

    void Confirm(int mana, int gold = 0, int lv = 0, int ap = 0)
    {
        if (ConfirmCheck(mana, gold, lv, ap))
        {
            Main.Instance.Player_AP -= ap;
            Main.Instance.CurrentDay.SubtractGold(gold, Main.DayResult.EventType.Etc);
            Main.Instance.CurrentDay.SubtractMana(mana, Main.DayResult.EventType.Etc);

            isActive = true;
            anim.enabled = true;
            switch (_OrbType)
            {
                case OrbType.green:
                    GameManager.Buff.CurrentBuff.Orb_green = 1;
                    foreach (var item in GameManager.Facility.facilityList)
                    {
                        if (item.GetType() == typeof(Herb))
                        {
                            item.GetComponent<Herb>().Orb_Bonus();
                        }
                    }
                    break;

                case OrbType.blue:
                    GameManager.Buff.CurrentBuff.Orb_blue = 1;
                    break;

                case OrbType.yellow:
                    GameManager.Buff.CurrentBuff.Orb_yellow = 1;
                    foreach (var item in GameManager.Facility.facilityList)
                    {
                        if (item.GetType() == typeof(Mineral))
                        {
                            item.GetComponent<Mineral>().Orb_Bonus();
                        }
                    }
                    break;

                case OrbType.red:
                    GameManager.Buff.CurrentBuff.Orb_red = 1;
                    break;
            }
        }
    }

    bool ConfirmCheck(int mana, int gold = 0, int lv = 0, int ap = 0)
    {
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Mana");
            return false;
        }
        if (Main.Instance.Player_Gold < gold)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Gold");
            return false;
        }
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Rank");
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return false;
        }

        return true;
    }


}
