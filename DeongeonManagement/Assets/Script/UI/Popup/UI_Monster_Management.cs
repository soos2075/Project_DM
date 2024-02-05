using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Monster_Management : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Panels
    {
        //? Panel취소이벤트는 base.Init이 하는중
        //Panel,
        SubPanel,
        GridPanel,
        ButtonPanel,
    }

    enum Texts
    {
        Lv,
        Name,
        Status,
        State,
        FloorData,
    }
    enum Etc
    {
        Profile,
    }

    enum Buttons
    {
        Training,
        Placement,
        Release,
        Recover,
        Return,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        AddRightClickCloseAllEvent();

        Bind<Image>(typeof(Panels));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(Etc));
        Bind<Button>(typeof(Buttons));

        CreateMonsterBox();
        TextClear();


        switch (Type)
        {
            case UI_Type.Management:
                Init_Management();
                break;

            case UI_Type.Placement:
                Init_Placement();
                break;
        }
    }


    public enum UI_Type
    {
        Management,
        Placement,
    }
    public UI_Type Type;


    void Init_Management()
    {
        //? 몬스터를 누르면 그 때 각종 버튼이 나타나고 누를 수 있게됨. 그전까진 의미없음
        GetTMP((int)Texts.FloorData).text = $"남은 행동력 : {Main.Instance.Player_AP}";
    }
    void Init_Placement()
    {
        //? Main.Instance.CurrentFloor이 결정되있는 상태 = 즉 바로 배치로 들어감
        //GetObject((int)Objects.ResumeCount).GetComponent<TextMeshProUGUI>().text = $"배치 가능 횟수 : {Main.Instance.CurrentFloor.MaxMonsterSize}";
        GetTMP((int)Texts.FloorData).text = $"{Main.Instance.CurrentFloor.Name_KR}\n배치된 몬스터 : {Main.Instance.CurrentFloor.monsterList.Count}\n" +
            $"추가 배치가능 몬스터 : {Main.Instance.CurrentFloor.MaxMonsterSize}";
        ButtonClear();
    }





    void CreateMonsterBox()
    {
        childList = new List<UI_MonsterBox>();

        for (int i = 0; i < GameManager.Monster.Monsters.Length; i++)
        {
            var obj = Managers.Resource.Instantiate("UI/PopUp/Element/MonsterBox", GetImage(((int)Panels.GridPanel)).transform);

            var box = obj.GetComponent<UI_MonsterBox>();
            box.monster = GameManager.Monster.Monsters[i];
            box.parent = this;
            childList.Add(box);
        }
    }

    public UI_MonsterBox Current { get; private set; }
    List<UI_MonsterBox> childList;

    public void ShowDetail(UI_MonsterBox selected)
    {
        SelectedPanel(selected);
        if (selected.monster == null)
        {
            TextClear();
            ButtonClear();
            return;
        }

        if (Type == UI_Type.Placement)
        {
            PlacementOne(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID));
            return;
        }

        AddButtonEvent();


        GetTMP(((int)Texts.Lv)).text = $"Lv.{selected.monster.LV}";
        GetTMP(((int)Texts.Name)).text = selected.monster.Name_KR;


        GetTMP(((int)Texts.Status)).text = $"HP : {selected.monster.HP} / {selected.monster.HP_Max} \n";
        GetTMP(((int)Texts.Status)).text += $"ATK : {selected.monster.ATK} \tDEF : {selected.monster.DEF} \n" +
            $"AGI : {selected.monster.AGI} \tLUK : {selected.monster.LUK}";

        GetTMP(((int)Texts.State)).text = "진화조건 : ???";

        GetObject(((int)Etc.Profile)).GetComponent<Image>().sprite = selected.monster.Data.sprite;
    }

    void TextClear()
    {
        GetTMP(((int)Texts.Lv)).text = "";
        GetTMP(((int)Texts.Name)).text = "";
        GetTMP(((int)Texts.Status)).text = "";
        GetTMP(((int)Texts.State)).text = "";
        GetObject(((int)Etc.Profile)).GetComponent<Image>().sprite = Managers.Sprite.GetSprite("Nothing");
    }

    void SelectedPanel(UI_MonsterBox selected)
    {
        selected.ChangePanelColor(Define.Color_Yellow);
        Current = selected;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Define.Color_Dark);
        }
    }







    #region Buttons
    void ButtonClear()
    {
        for (int i = 0; i < Enum.GetNames(typeof(Buttons)).Length; i++)
        {
            GetButton(i).gameObject.SetActive(false);
        }
    }

    void Init_ButtonEvent()
    {
        for (int i = 0; i < Enum.GetNames(typeof(Buttons)).Length; i++)
        {
            GetButton(i).gameObject.SetActive(true);
            GetButton(i).gameObject.RemoveUIEventAll();
            GetButton(i).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        }
    }
    IEnumerator RefreshAll()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("창 새로고침");
        GetTMP((int)Texts.FloorData).text = $"남은 행동력 : {Main.Instance.Player_AP}";
        ShowDetail(Current);
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ShowContents();
        }
        AddButtonEvent();
    }

    void AddButtonEvent()
    {
        if (Current == null || Current.monster == null) return;

        Init_ButtonEvent();

        switch (Current.monster.State)
        {
            case Monster.MonsterState.Standby:
                GetButton(((int)Buttons.Return)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Placement)).gameObject.
                    AddUIEvent((data) => PlacementEvent(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID)));

                GetButton(((int)Buttons.Training)).gameObject.
                    AddUIEvent((data) => Current.monster.Training());

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));
                break;



            case Monster.MonsterState.Placement:
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Release)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Return)).gameObject.
                    AddUIEvent((data) => Current.monster.MonsterOutFloor());

                GetButton(((int)Buttons.Training)).gameObject.
                    AddUIEvent((data) => Current.monster.Training());
                break;



            case Monster.MonsterState.Injury:
                GetButton(((int)Buttons.Training)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Return)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Recover)).gameObject.
                    AddUIEvent((data) => Current.monster.Recover());

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));
                break;
        }
    }





    void PlacementOne(Vector2Int[] vector2Ints, Action action)
    {
        Managers.UI.PausePopUp(this);
        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;
        Main.Instance.CurrentFloor.UI_Floor.ShowTile();
    }


    void PlacementEvent(Vector2Int[] vector2Ints, Action action)
    {
        if (Current == null || Current.monster == null) return;

        if (Current.monster.State == Monster.MonsterState.Placement)
        {
            Current.monster.MonsterOutFloor();
        }

        StartCoroutine(ShowAllFloor(vector2Ints, action));
    }
    IEnumerator ShowAllFloor(Vector2Int[] vector2Ints, Action action)
    {
        Managers.UI.PausePopUp(this);
        Managers.UI.ShowPopUpAlone<UI_DungeonPlacement>();

        yield return new WaitForEndOfFrame();
        PanelDisable();

        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;
        for (int i = 0; i < Main.Instance.Floor.Length; i++)
        {
            if (Main.Instance.Floor[i].MaxMonsterSize > 0)
            {
                Main.Instance.Floor[i].UI_Floor.ShowTile();
            }
        }

    }
    public void PanelDisable()
    {
        var floorList = FindObjectsOfType<UI_Floor>();
        foreach (var item in floorList)
        {
            item.GetComponent<Image>().enabled = false;
        }
    }



    bool Create(int monsterID, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        Main.Instance.CurrentFloor = tile.floor;

        foreach (var item in boundary)
        {
            Vector2Int delta = tile.index + item;
            BasementTile temp = null;
            if (tile.floor.TileMap.TryGetValue(delta, out temp))
            {
                var obj = GameManager.Monster.Monsters[monsterID];
                GameManager.Placement.PlacementConfirm(obj, new PlacementInfo(tile.floor, temp));

                obj.PlacementInfo.Place_Floor.MaxMonsterSize--;
                obj.State = Monster.MonsterState.Placement;
            }
        }
        return true;
    }

    void CreateAll(int monsterID)
    {
        if (Create(monsterID, Main.Instance.CurrentBoundary))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    void CreateOver()
    {
        Debug.Log($"{Current.monster.Name_KR}(이)가 {Main.Instance.CurrentFloor.Name_KR}에 배치되었습니다");
        ResetAction();
        Managers.UI.CloseAll();
    }
    void ResetAction()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
        Main.Instance.CurrentFloor = null;
    }

    
    #endregion
}
