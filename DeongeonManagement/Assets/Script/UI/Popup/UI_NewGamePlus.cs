using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_NewGamePlus : UI_PopUp
{
    private void Start()
    {
        Init_EventData();
        Init();
    }



    enum Contents
    {
        NoTouch,
    }


    enum Texts
    {
        ClearPoint,
        Record,
    }

    public enum Panel
    {
        ModePanel,
        DifficultyPanel,
        BuffPanel,
        StatuePanel,
        UnitPanel,
        ArtifactPanel,
    }

    enum Btn
    {
        NextBtn,
        PrevBtn,
        GameStart,
    }


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Contents));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Panel));
        Bind<Button>(typeof(Btn));


        Init_Mode();
        Init_Difficulty();
        Init_Buff();
        Init_Statue();
        Init_Unit();
        Init_Artifact();

        GetButton((int)Btn.NextBtn).gameObject.AddUIEvent(data => CurrentPage++);
        GetButton((int)Btn.PrevBtn).gameObject.AddUIEvent(data => CurrentPage--);

        CurrentPage = 0;
        CurrentPoint = UserData.Instance.CurrentPlayerData.GetClearPoint();
        Init_Record();


        GetButton((int)Btn.GameStart).gameObject.AddUIEvent(data => GameStart());
    }


    void Init_Record()
    {
        var data = UserData.Instance.CurrentPlayerData;

        GetTMP(((int)Texts.Record)).GetComponent<TextMeshProUGUI>().text =
            $"{UserData.Instance.LocaleText_NGP("Log_클리어횟수")} : {data.GetClearCount()}" +
            $"\n{UserData.Instance.LocaleText_NGP("Log_엔딩")} : {data.EndingClearNumber()} / {Enum.GetValues(typeof(Endings)).Length}" +
            $"\n{UserData.Instance.LocaleText_NGP("Log_클리어난이도")} : {Util.GetDiffStar(data.GetHighestDifficultyLevel())}" +
            $"\n{UserData.Instance.LocaleText_NGP("Log_최고턴")} : {data.GetHighestTurn()}";
    }




    void GameStart()
    {
        UserData.Instance.FileConfig.Difficulty = (Define.DifficultyLevel)currentSelectDifficulty;
        UserData.Instance.FileConfig.GameMode = (Define.ModeSelect)currentSelectMode;

        if ((Define.ModeSelect)currentSelectMode == Define.ModeSelect.Endless)
        {
            UserData.Instance.FileConfig.Difficulty = Define.DifficultyLevel.Normal;
        }

        foreach (var item in bonusList)
        {
            UserData.Instance.FileConfig.SetBoolValue(item.ToString(), true);
        }
        foreach (var item in statueList)
        {
            UserData.Instance.FileConfig.SetBoolValue(item.ToString(), true);
        }

        Managers.UI.ClosePopUp();
    }




    #region SO_Data_ 필요한 툴팁 텍스트만 DataManager에서 직접 가져오기
    public string GetArtifactData(ArtifactLabel label)
    {
        string[] datas = Managers.Data.GetTextData_Artifact((int)label);

        if (datas == null)
        {
            Debug.Log($"{label} : CSV Data Not Exist");
            return "";
        }

        return datas[2];
    }
    public string GetArtifactLabel(ArtifactLabel label)
    {
        string[] datas = Managers.Data.GetTextData_Artifact((int)label);

        if (datas == null)
        {
            Debug.Log($"{label} : CSV Data Not Exist");
            return "";
        }

        return datas[0];
    }

    public string GetStatueData(int id)
    {
        string[] datas = Managers.Data.GetTextData_Object(id);

        if (datas == null)
        {
            Debug.Log($"{id} : CSV Data Not Exist");
            return "";
        }

        return datas[1];
    }


    #endregion





    public Sprite defaultBtn;
    public Sprite selectBtn;


    public int currentPoint;
    public int CurrentPoint
    {
        get { return currentPoint; }
        set
        {
            currentPoint = value;
            Update_Point();
        }
    }

    void Update_Point()
    {
        GetTMP(((int)Texts.ClearPoint)).GetComponent<TextMeshProUGUI>().text =
            $"Clear Point : {currentPoint}P";
    }


    public int currentPage = 0;
    public int CurrentPage 
    { 
        get { return currentPage; } 
        set 
        {
            currentPage = value; 
            Update_PageBtn(); 
        } 
    }



    void Update_PageBtn()
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(Panel)).Length; i++)
        {
            GetImage(i).gameObject.SetActive(false);
        }

        int pageIndex = System.Enum.GetNames(typeof(Panel)).Length - 1;

        if (currentPage <= 0)
        {
            currentPage = 0;
            GetButton((int)Btn.NextBtn).gameObject.SetActive(true);
            GetButton((int)Btn.PrevBtn).gameObject.SetActive(false);
        }
        else if (currentPage >= pageIndex)
        {
            currentPage = pageIndex;
            GetButton((int)Btn.NextBtn).gameObject.SetActive(false);
            GetButton((int)Btn.PrevBtn).gameObject.SetActive(true);
        }
        else //if (0 < currentPage && currentPage < pageIndex)
        {
            GetButton((int)Btn.NextBtn).gameObject.SetActive(true);
            GetButton((int)Btn.PrevBtn).gameObject.SetActive(true);
        }

        GetImage(currentPage).gameObject.SetActive(true);
    }

    public int currentSelectMode = 0;

    void Init_Mode()
    {
        var parent = GetImage((int)Panel.ModePanel).GetComponentInChildren<GridLayoutGroup>();
        var btn = parent.GetComponentsInChildren<Button>();

        btn[0].gameObject.AddUIEvent(data => Mode_Select(0));
        btn[1].gameObject.AddUIEvent(data => Mode_Select(1));

        Mode_Select(0);

        //? 툴팁 등록
        {
            var tool = btn[0].gameObject.GetOrAddComponent<UI_Tooltip>();
            tool.SetTooltipContents("", UserData.Instance.LocaleText_NGP("스토리모드_설명"), UI_TooltipBox.ShowPosition.RightDown);
            tool.SetFontSize(_contentSize: 30);
        }
        {
            var tool = btn[1].gameObject.GetOrAddComponent<UI_Tooltip>();
            tool.SetTooltipContents("", UserData.Instance.LocaleText_NGP("무한모드_설명"), UI_TooltipBox.ShowPosition.RightDown);
            tool.SetFontSize(_contentSize: 30);
        }
    }

    void Mode_Select(int _mode)
    {
        var parent = GetImage((int)Panel.ModePanel).GetComponentInChildren<GridLayoutGroup>();
        var btn = parent.GetComponentsInChildren<Button>();

        foreach (var item in btn)
        {
            item.image.sprite = defaultBtn;
        }

        currentSelectMode = _mode;
        btn[_mode].image.sprite = selectBtn;
    }



    public int currentSelectDifficulty = 0;
    void Init_Difficulty()
    {
        var parent = GetImage((int)Panel.DifficultyPanel).GetComponentInChildren<GridLayoutGroup>();
        var btn = parent.GetComponentsInChildren<Button>();

        btn[0].gameObject.AddUIEvent(data => DifficultSelect(0));
        btn[1].gameObject.AddUIEvent(data => DifficultSelect(1));
        btn[2].gameObject.AddUIEvent(data => DifficultSelect(2));
        btn[3].gameObject.AddUIEvent(data => DifficultSelect(3));
        //btn[4].gameObject.AddUIEvent(data => DifficultSelect(4));

        foreach (var item in btn)
        {
            item.gameObject.SetActive(false);
        }

        int currentMaxLevel = UserData.Instance.CurrentPlayerData.GetHighestDifficultyLevel();
        for (int i = 0; i < currentMaxLevel + 2; i++)
        {
            if (i > 3) break;

            btn[i].gameObject.SetActive(true);
        }

        DifficultSelect(Mathf.Clamp(currentMaxLevel, 0, 3));
    }

    void DifficultSelect(int diff)
    {
        var parent = GetImage((int)Panel.DifficultyPanel).GetComponentInChildren<GridLayoutGroup>();
        var btn = parent.GetComponentsInChildren<Button>();

        foreach (var item in btn)
        {
            item.image.sprite = defaultBtn;
        }

        currentSelectDifficulty = diff;
        btn[diff].image.sprite = selectBtn;
    }


    void Init_Buff()
    {
        Add_Content(Panel.BuffPanel, Bonus.Buff_ApBonusOne);
        Add_Content(Panel.BuffPanel, Bonus.Buff_ApBonusTwo);
        Add_Content(Panel.BuffPanel, Bonus.Buff_PopBonus);
        Add_Content(Panel.BuffPanel, Bonus.Buff_DangerBonus);
        Add_Content(Panel.BuffPanel, Bonus.Buff_ManaBonus);
        Add_Content(Panel.BuffPanel, Bonus.Buff_ManaBonus1000);
        Add_Content(Panel.BuffPanel, Bonus.Buff_GoldBonus);
        Add_Content(Panel.BuffPanel, Bonus.Buff_GoldBonus1000);

        Add_Content(Panel.BuffPanel, Bonus.Buff_Starting_4F);
        Add_Content(Panel.BuffPanel, Bonus.Buff_Starting_Facility);
    }

    void Init_Statue()
    {
        statueList.Add(Bonus.Statue_Mana);
        statueList.Add(Bonus.Statue_Gold);

        Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Gold, 3901);
        Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Mana, 3902);

        ////? 테스트용으로 임시 전부 개방
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dog);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Ravi);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dragon);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Cat);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Demon);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Hero);


        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Dog))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dog, 3903);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Ravi))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Ravi, 3905);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Dragon))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dragon, 3904);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Cat))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Cat, 3906);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Demon))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Demon, 3907);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Hero))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Hero, 3908);
        }
    }

    void Init_Unit()
    {
        if (CollectionManager.Instance.Get_Collection_Monster("BloodySlime").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_BloodySlime);
        }
        if (CollectionManager.Instance.Get_Collection_Monster("FlameGolem").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_FlameGolem);
        }
        if (CollectionManager.Instance.Get_Collection_Monster("Pixie").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Pixie);
        }
        if (CollectionManager.Instance.Get_Collection_Monster("Salinu").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Salinu);
        }
        if (CollectionManager.Instance.Get_Collection_Monster("HellHound").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_HellHound);
        }
        if (CollectionManager.Instance.Get_Collection_Monster("Griffin").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Griffin);
        }
        if (CollectionManager.Instance.Get_Collection_Monster("Lilith").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Lilith);
        }


        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Cat))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Rena);
        }


        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Ravi))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Ravi);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Demon))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Lievil);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Hero))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Rideer);
        }

    }

    void Init_Artifact()
    {
        Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Pop, ArtifactLabel.OrbOfPopularity);
        Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Danger, ArtifactLabel.OrbOfDanger);

        if (UserData.Instance.CurrentPlayerData.GetClearCount() >= 2)
        {
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_DownDanger, ArtifactLabel.MarbleOfReassurance);
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_DownPop, ArtifactLabel.MarbleOfOblivion);
        }
        

        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Demon))
        {
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Decay, ArtifactLabel.TouchOfDecay);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Hero))
        {
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Hero, ArtifactLabel.ProofOfHero);
        }

        if (UserData.Instance.CurrentPlayerData.GetHighestDifficultyLevel() >= 1)
        {
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Lv_1, ArtifactLabel.LvBook_1);
        }
        if (UserData.Instance.CurrentPlayerData.GetHighestDifficultyLevel() >= 2)
        {
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Lv_2, ArtifactLabel.LvBook_2);
        }
        if (UserData.Instance.CurrentPlayerData.GetHighestDifficultyLevel() >= 3)
        {
            Add_Content_Artifact(Panel.ArtifactPanel, Bonus.Arti_Lv_3, ArtifactLabel.LvBook_3);
        }
    }


    void Add_Content_Artifact(Panel _panel, Bonus _label, ArtifactLabel arti)
    {
        var parent = GetImage((int)_panel).GetComponentInChildren<GridLayoutGroup>();

        var obj = Managers.Resource.Instantiate("UI/PopUp/Element/NewGameBonusElement", parent.transform);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{GetArtifactLabel(arti)} <b>({BonusDict[_label].point}P)</b>";
            //$"{UserData.Instance.LocaleText_NGP($"{_label}")} <b>({BonusDict[_label].point}P)</b>";

        //? AddEvent - ContentClick
        obj.AddUIEvent(data => ContentClick(_label, obj.GetComponent<Button>()));

        //? 툴팁 등록
        var tool = obj.GetOrAddComponent<UI_Tooltip>();
        tool.SetTooltipContents("", GetArtifactData(arti), UI_TooltipBox.ShowPosition.RightDown);
        tool.SetFontSize(_contentSize: 30);

        //? 등록할 때 껐다 켜서 이미지 최신화
        ContentClick(_label, obj.GetComponent<Button>());
        ContentClick(_label, obj.GetComponent<Button>());
    }




    void Add_Content(Panel _panel, Bonus _label)
    {
        var parent = GetImage((int)_panel).GetComponentInChildren<GridLayoutGroup>();

        var obj = Managers.Resource.Instantiate("UI/PopUp/Element/NewGameBonusElement", parent.transform);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText_NGP($"{_label}")} <b>({BonusDict[_label].point}P)</b>";

        //? AddEvent - ContentClick
        obj.AddUIEvent(data => ContentClick(_label, obj.GetComponent<Button>()));

        //? 등록할 때 껐다 켜서 이미지 최신화
        ContentClick(_label, obj.GetComponent<Button>());
        ContentClick(_label, obj.GetComponent<Button>());
    }
    void Add_Content_Statue(Panel _panel, Bonus _label, int _id)
    {
        var parent = GetImage((int)_panel).GetComponentInChildren<GridLayoutGroup>();

        var obj = Managers.Resource.Instantiate("UI/PopUp/Element/NewGameBonusElement", parent.transform);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText_NGP($"{_label}")} <b>({BonusDict[_label].point}P)</b>";

        //? AddEvent - ContentClick
        obj.AddUIEvent(data => ContentClick_Statue(_label, obj.GetComponent<Button>()));

        //? 툴팁 등록
        var tool = obj.GetOrAddComponent<UI_Tooltip>();
        tool.SetTooltipContents("", GetStatueData(_id), UI_TooltipBox.ShowPosition.RightDown);
        tool.SetFontSize(_contentSize: 30);

        //? 등록할 때 껐다 켜서 이미지 최신화
        ContentClick_Statue(_label, obj.GetComponent<Button>());
        ContentClick_Statue(_label, obj.GetComponent<Button>());
    }



    void ContentClick(Bonus _label, Button _btn)
    {
        //? 이미 활성화 상태면 비활성화 시키고 포인트 롤백 / BonusList에서 제거
        if (bonusList.Contains(_label))
        {
            _btn.image.sprite = defaultBtn;
            CurrentPoint += BonusDict[_label].point;
            bonusList.Remove(_label);
        }
        else //? 비활성화라면 남은 포인트 확인 / 남은 포인트가 필요 포인트보다 높다면 활성화 시키고 포인트 마이너스 / Bonus리스트에 추가
        {
            if (CurrentPoint >= BonusDict[_label].point)
            {
                _btn.image.sprite = selectBtn;
                CurrentPoint -= BonusDict[_label].point;
                bonusList.Add(_label);
            }
        }
    }

    void ContentClick_Statue(Bonus _label, Button _btn)
    {
        //? 이미 활성화 상태면 비활성화 시키고 포인트 롤백 / BonusList에서 제거
        if (statueList.Contains(_label))
        {
            _btn.image.sprite = defaultBtn;
            CurrentPoint += BonusDict[_label].point;
            statueList.Remove(_label);
        }
        else //? 비활성화라면 남은 포인트 확인 / 남은 포인트가 필요 포인트보다 높다면 활성화 시키고 포인트 마이너스 / Bonus리스트에 추가
        {
            if (CurrentPoint >= BonusDict[_label].point && statueList.Count < 5)
            {
                _btn.image.sprite = selectBtn;
                CurrentPoint -= BonusDict[_label].point;
                statueList.Add(_label);
            }
        }
    }


    //? 조각상은 5개로 제한해야되니까 리스트 따로 만들고 마지막에 추가하든지 해
    List<Bonus> statueList = new List<Bonus>();

    //? 얘는 일반 리스트
    List<Bonus> bonusList = new List<Bonus>();


    //? 모든 Enum Data가 들어가있어야함
    Dictionary<Bonus, BtnEvent> BonusDict = new Dictionary<Bonus, BtnEvent>();

    public class BtnEvent
    {
        public Panel panel;
        public Bonus bonus;
        public int point;

        public BtnEvent(Panel _panel, Bonus _bonus, int _point)
        {
            panel = _panel;
            bonus = _bonus;
            point = _point;
        }
    }

    void Init_EventData()
    {
        //? 시작효과
        BonusDict.Add(Bonus.Buff_ApBonusOne, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ApBonusOne, 15));
        BonusDict.Add(Bonus.Buff_ApBonusTwo, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ApBonusTwo, 30));
        BonusDict.Add(Bonus.Buff_PopBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_PopBonus, 5));
        BonusDict.Add(Bonus.Buff_DangerBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_DangerBonus, 5));
        BonusDict.Add(Bonus.Buff_ManaBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ManaBonus, 2));
        BonusDict.Add(Bonus.Buff_GoldBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_GoldBonus, 4));

        BonusDict.Add(Bonus.Buff_ManaBonus1000, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ManaBonus1000, 6));
        BonusDict.Add(Bonus.Buff_GoldBonus1000, new BtnEvent(Panel.BuffPanel, Bonus.Buff_GoldBonus1000, 12));

        BonusDict.Add(Bonus.Buff_Starting_4F, new BtnEvent(Panel.BuffPanel, Bonus.Buff_Starting_4F, 10));
        BonusDict.Add(Bonus.Buff_Starting_Facility, new BtnEvent(Panel.BuffPanel, Bonus.Buff_Starting_Facility, 10));



        //? 조각상
        BonusDict.Add(Bonus.Statue_Mana, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Mana, 0));
        BonusDict.Add(Bonus.Statue_Gold, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Gold, 0));
        BonusDict.Add(Bonus.Statue_Dog, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Dog, 5));
        BonusDict.Add(Bonus.Statue_Cat, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Cat, 7));
        BonusDict.Add(Bonus.Statue_Dragon, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Dragon, 7));
        BonusDict.Add(Bonus.Statue_Ravi, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Ravi, 5));
        BonusDict.Add(Bonus.Statue_Demon, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Demon, 10));
        BonusDict.Add(Bonus.Statue_Hero, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Hero, 10));


        //? 유닛
        BonusDict.Add(Bonus.Unit_BloodySlime, new BtnEvent(Panel.UnitPanel, Bonus.Unit_BloodySlime, 5));
        BonusDict.Add(Bonus.Unit_FlameGolem, new BtnEvent(Panel.UnitPanel, Bonus.Unit_FlameGolem, 5));

        BonusDict.Add(Bonus.Unit_Pixie, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Pixie, 10));
        BonusDict.Add(Bonus.Unit_HellHound, new BtnEvent(Panel.UnitPanel, Bonus.Unit_HellHound, 10));

        BonusDict.Add(Bonus.Unit_Salinu, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Salinu, 12));
        BonusDict.Add(Bonus.Unit_Griffin, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Griffin, 15));
        BonusDict.Add(Bonus.Unit_Lilith, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Lilith, 13));


        BonusDict.Add(Bonus.Unit_Rena, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Rena, 10));

        BonusDict.Add(Bonus.Unit_Ravi, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Ravi, 15));
        BonusDict.Add(Bonus.Unit_Lievil, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Lievil, 15));
        BonusDict.Add(Bonus.Unit_Rideer, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Rideer, 15));



        //? artifact
        BonusDict.Add(Bonus.Arti_Hero, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Hero, 15));
        BonusDict.Add(Bonus.Arti_Decay, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Decay, 20));
        BonusDict.Add(Bonus.Arti_Pop, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Pop, 5));
        BonusDict.Add(Bonus.Arti_Danger, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Danger, 5));
        BonusDict.Add(Bonus.Arti_DownDanger, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_DownDanger,7));
        BonusDict.Add(Bonus.Arti_DownPop, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_DownPop, 7));

        BonusDict.Add(Bonus.Arti_Lv_1, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Lv_1, 5));
        BonusDict.Add(Bonus.Arti_Lv_2, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Lv_2, 10));
        BonusDict.Add(Bonus.Arti_Lv_3, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Lv_3, 15));
    }



    public enum Bonus //? UserData에 아래리스트 그대로 bool값이 존재해야함
    {
        //? 조각상
        Statue_Mana,
        Statue_Gold,
        Statue_Dog,
        Statue_Cat,
        Statue_Dragon,
        Statue_Ravi,
        Statue_Demon,
        Statue_Hero,

        //? 고유효과
        Buff_ApBonusOne,
        Buff_ApBonusTwo,
        Buff_PopBonus,
        Buff_DangerBonus,
        Buff_ManaBonus,
        Buff_GoldBonus,
        Buff_ManaBonus1000,
        Buff_GoldBonus1000,

        Buff_Starting_4F,
        Buff_Starting_Facility,


        //? 유닛
        Unit_BloodySlime,
        Unit_FlameGolem,
        Unit_Pixie,
        Unit_Salinu,
        Unit_HellHound,
        Unit_Griffin,
        Unit_Lilith,
        Unit_Rena,

        Unit_Ravi,
        Unit_Lievil,
        Unit_Rideer,

        //? 아티팩트
        Arti_Hero,
        Arti_Decay,
        Arti_Pop,
        Arti_Danger,

        Arti_DownDanger,
        Arti_DownPop,

        Arti_Lv_1,
        Arti_Lv_2,
        Arti_Lv_3,
    }


}
