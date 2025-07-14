using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public int _OrbLevel;

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
                _OrbLevel = GameManager.Buff.CurrentBuff.Orb_green;
                break;

            case OrbType.blue:
                _OrbLevel = GameManager.Buff.CurrentBuff.Orb_blue;
                break;

            case OrbType.yellow:
                _OrbLevel = GameManager.Buff.CurrentBuff.Orb_yellow;
                break;

            case OrbType.red:
                _OrbLevel = GameManager.Buff.CurrentBuff.Orb_red;
                break;
        }

        if (_OrbLevel > 0)
        {
            isActive = true;
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
        if (UserData.Instance.GameMode == Define.TimeMode.Stop) return true;
        if (Time.timeScale == 0) return true;


        if (Main.Instance.Turn < 5 && UserData.Instance.FileConfig.PlayRounds == 1) return true;

        // PointerEventData를 생성하고 현재 마우스 위치를 설정
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            //Debug.Log("현재 마우스가 올라가 있는 UI: " + results[0].gameObject.name);
            //? 팝업 ui랑 메인 ui 위에선 작동안하도록 변경
            if (results[0].gameObject.GetComponentInParent<UI_Management>(true)) return true;
            if (results[0].gameObject.GetComponentInParent<UI_PopUp>(true))
            {
                if (results[0].gameObject.GetComponentInParent<UI_DungeonPlacement>(true) == null)
                {
                    return true;
                }
            }
        }

        return false;
    }


    #region ShowBox

    UI_TileView view;
    void MoveEvent()
    {
        if (Managers.UI._popupStack.Count > 0) return;
        if (!isActive) return;

        if (view == null)
        {
            view = Managers.UI.ShowPopUpAlone<UI_TileView>();

            string title = "";
            string msg = "";
            switch (_OrbType)
            {
                case OrbType.green:
                    title = UserData.Instance.LocaleText_Tooltip("Orb_G");
                    msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green_tier1")}";
                    if (_OrbLevel > 1)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Green_tier1")}";
                    }
                    if (_OrbLevel > 2)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Green_Detail")}";
                    }
                    break;

                case OrbType.yellow:
                    title = UserData.Instance.LocaleText_Tooltip("Orb_Y");
                    msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow_tier1")}";
                    if (_OrbLevel > 1)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Yellow_tier1")}";
                    }
                    if (_OrbLevel > 2)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Yellow_Detail")}";
                    }
                    break;

                case OrbType.blue:
                    title = UserData.Instance.LocaleText_Tooltip("Orb_B");
                    msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")}";
                    if (_OrbLevel > 1)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")}";
                    }
                    if (_OrbLevel > 2)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")}";
                    }
                    break;

                case OrbType.red:
                    title = UserData.Instance.LocaleText_Tooltip("Orb_R");
                    msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")}";
                    if (_OrbLevel > 1)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")}";
                    }
                    if (_OrbLevel > 2)
                    {
                        msg += $"\n{UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")}";
                    }
                    break;
            }


            view.ViewContents(title, msg);
        }


    }
    void CloseView()
    {
        if (view)
        {
            Managers.UI.ClosePopupPick(view);
            view = null;
        }
    }

    #endregion

    private void OnMouseOver()
    {
        if (Main.Instance.Management == false) return;
        MoveEvent();
        //Debug.Log("무브");
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
        CloseView();
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
        if (isActive && _OrbLevel >= 3)
        {
            //AlreadyActive();
        }
        else
        {
            MouseClickEvent();
        }

    }


    //void AlreadyActive()
    //{
    //    UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
    //    string msg = "";
    //    switch (_OrbType)
    //    {
    //        case OrbType.green:
    //            msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green_Detail")}";
    //            break;

    //        case OrbType.yellow:
    //            msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow_Detail")}";
    //            break;

    //        case OrbType.blue:
    //            msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")}";
    //            break;

    //        case OrbType.red:
    //            msg = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")}";
    //            break;
    //    }
    //    ui.SetText(msg, () => { });
    //}



    void MouseClickEvent()
    {
        UI_Confirm ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        string mainText = "";

        switch (_OrbType)
        {
            case OrbType.green:
                switch (_OrbLevel)
                {
                    case 0:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green")} - 1{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Green_tier1")})";
                        ui.SetText(mainText, () => Confirm(ap: 1, mana: 500, gold: 100));
                        ui.SetMode_Calculation(Define.DungeonRank.F, "500", "100", "1");
                        break;

                    case 1:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green")} - 2{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Green_tier1")})";
                        ui.SetText(mainText, () => Confirm(ap: 1, mana: 500, gold: 100));
                        ui.SetMode_Calculation(Define.DungeonRank.D, "500", "100", "1");
                        break;

                    case 2:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Green")} - 3{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Green_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 3, mana: 1500, gold: 300));
                        ui.SetMode_Calculation(Define.DungeonRank.C, "1500", "300", "3");
                        break;
                }
                break;


            case OrbType.yellow:
                switch (_OrbLevel)
                {
                    case 0:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow")} - 1{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Yellow_tier1")})";
                        ui.SetText(mainText, () => Confirm(ap: 1, mana: 500, gold: 100));
                        ui.SetMode_Calculation(Define.DungeonRank.F, "500", "100", "1");
                        break;

                    case 1:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow")} - 2{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Yellow_tier1")})";
                        ui.SetText(mainText, () => Confirm(ap: 1, mana: 500, gold: 100));
                        ui.SetMode_Calculation(Define.DungeonRank.D, "500", "100", "1");
                        break;

                    case 2:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Yellow")} - 3{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Yellow_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 3, mana: 1500, gold: 300));
                        ui.SetMode_Calculation(Define.DungeonRank.C, "1500", "300", "3");
                        break;
                }
                break;


            case OrbType.blue:
                switch (_OrbLevel)
                {
                    case 0:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue")} - 1{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 2, mana: 1000, gold: 0));
                        ui.SetMode_Calculation(Define.DungeonRank.D, "1000", "0", "2");
                        break;

                    case 1:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue")} - 2{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 3, mana: 1500, gold: 0));
                        ui.SetMode_Calculation(Define.DungeonRank.C, "1500", "0", "3");
                        break;

                    case 2:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Blue")} - 3{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Blue_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 4, mana: 2000, gold: 0));
                        ui.SetMode_Calculation(Define.DungeonRank.B, "2000", "0", "4");
                        break;
                }
                break;


            case OrbType.red:
                switch (_OrbLevel)
                {
                    case 0:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red")} - 1{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 2, mana: 500, gold: 1000));
                        ui.SetMode_Calculation(Define.DungeonRank.D, "500", "1000", "2");
                        break;

                    case 1:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red")} - 2{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 3, mana: 750, gold: 1500));
                        ui.SetMode_Calculation(Define.DungeonRank.C, "750", "1500", "3");
                        break;

                    case 2:
                        mainText = $"{UserData.Instance.LocaleText_Tooltip("Orb_Red")} - 3{UserData.Instance.LocaleText_Tooltip("Orb_Level")}";
                        mainText += $"\n({UserData.Instance.LocaleText_Tooltip("Orb_Red_Detail")})";
                        ui.SetText(mainText, () => Confirm(ap: 4, mana: 1000, gold: 2000));
                        ui.SetMode_Calculation(Define.DungeonRank.B, "1000", "2000", "4");
                        break;
                }
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
                    GameManager.Buff.CurrentBuff.Orb_green += 1;
                    _OrbLevel = GameManager.Buff.CurrentBuff.Orb_green;

                    if (_OrbLevel == 3)
                    {
                        foreach (var item in GameManager.Facility.facilityList)
                        {
                            if (item.GetType() == typeof(Herb))
                            {
                                item.GetComponent<Herb>().Orb_Bonus();
                            }
                        }
                    }
                    break;

                case OrbType.blue:
                    GameManager.Buff.CurrentBuff.Orb_blue += 1;
                    _OrbLevel = GameManager.Buff.CurrentBuff.Orb_blue;
                    break;

                case OrbType.yellow:
                    GameManager.Buff.CurrentBuff.Orb_yellow += 1;
                    _OrbLevel = GameManager.Buff.CurrentBuff.Orb_yellow;

                    if (_OrbLevel == 3)
                    {
                        foreach (var item in GameManager.Facility.facilityList)
                        {
                            if (item.GetType() == typeof(Mineral))
                            {
                                item.GetComponent<Mineral>().Orb_Bonus();
                            }
                        }
                    }
                    break;

                case OrbType.red:
                    GameManager.Buff.CurrentBuff.Orb_red += 1;
                    _OrbLevel = GameManager.Buff.CurrentBuff.Orb_red;
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
