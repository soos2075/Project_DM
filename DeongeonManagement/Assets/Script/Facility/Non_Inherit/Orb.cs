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

    private void Start()
    {
        Resolver = GetComponent<SpriteResolver>();
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

        if (isActive) Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active");
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
            Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active_line");
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
            Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active");
        }
        else
        {
            Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Inactive");
        }
    }



    private void OnMouseUpAsButton()
    {
        if (ReturnCheck()) return;

        //? 이미 활성화되있으면 리턴
        if (isActive) return;

        //Debug.Log(_OrbType + "Click");
        StartCoroutine(WaitFrame());
    }


    IEnumerator WaitFrame() //? ClearPanel이 타일 이외의 지역 좌클릭 = CloseAll 로 해놨기때문에 1프레임 기다려야함
    {
        yield return new WaitForEndOfFrame();
        MouseClickEvent();
    }


    void MouseClickEvent()
    {
        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        switch (_OrbType)
        {
            case OrbType.green:
                ui.SetText($"{UserData.Instance.LocaleText_Tooltip("Orb_Green")}\n" +
                    $"<size=25>" +
                    $"(-2{UserData.Instance.LocaleText("AP")}, " +
                    $"-750{UserData.Instance.LocaleText("Mana")}, " +
                    $"-250{UserData.Instance.LocaleText("Gold")})");// +
                    //$"{UserData.Instance.LocaleText("필요")})");

                StartCoroutine(WaitForAnswer(ui, () => Confirm(ap: 2, mana: 750, gold: 250)));
                break;


            case OrbType.yellow:
                ui.SetText($"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow")}\n" +
    $"<size=25>" +
    $"(-2{UserData.Instance.LocaleText("AP")}, " +
    $"-750{UserData.Instance.LocaleText("Mana")}, " +
    $"-250{UserData.Instance.LocaleText("Gold")})");// +
    //$"{UserData.Instance.LocaleText("필요")})");

                StartCoroutine(WaitForAnswer(ui, () => Confirm(ap: 2, mana: 750, gold: 250)));
                break;


            case OrbType.blue:
                ui.SetText($"{UserData.Instance.LocaleText_Tooltip("Orb_Blue")}\n" +
    $"<size=25>" +
    $"(-3{UserData.Instance.LocaleText("AP")}, " +
    $"-2000{UserData.Instance.LocaleText("Mana")}, " +
    $"-1000{UserData.Instance.LocaleText("Gold")})");// +
    //$"{UserData.Instance.LocaleText("필요")})");

                StartCoroutine(WaitForAnswer(ui, () => Confirm(ap: 3, mana: 2000, gold: 1000)));
                break;


            case OrbType.red:
                ui.SetText($"{UserData.Instance.LocaleText_Tooltip("Orb_Red")}\n" +
    $"<size=25>" +
    $"(-4{UserData.Instance.LocaleText("AP")}, " +
    $"-1000{UserData.Instance.LocaleText("Mana")}, " +
    $"-2000{UserData.Instance.LocaleText("Gold")})");// +
    //$"{UserData.Instance.LocaleText("필요")})");

                StartCoroutine(WaitForAnswer(ui, () => Confirm(ap: 4, mana: 1000, gold: 2000)));
                break;
        }
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm, System.Action action)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.UI.ClosePopupPick(confirm);
            action.Invoke();
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
