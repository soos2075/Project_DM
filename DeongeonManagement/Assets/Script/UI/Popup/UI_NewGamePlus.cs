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
        ClearPoint
    }

    public enum Panel
    {
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



        Init_Difficulty();
        Init_Buff();
        Init_Statue();
        Init_Unit();
        Init_Artifact();

        GetButton((int)Btn.NextBtn).gameObject.AddUIEvent(data => CurrentPage++);
        GetButton((int)Btn.PrevBtn).gameObject.AddUIEvent(data => CurrentPage--);

        CurrentPage = 0;
        CurrentPoint = CollectionManager.Instance.RoundClearData.GetClearPoint();

        GetButton((int)Btn.GameStart).gameObject.AddUIEvent(data => GameStart());
    }




    void GameStart()
    {
        UserData.Instance.FileConfig.Difficulty = (Define.DifficultyLevel)currentSelectDifficulty;

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


        switch (currentPage)
        {
            case 0:
                GetButton((int)Btn.NextBtn).gameObject.SetActive(true);
                GetButton((int)Btn.PrevBtn).gameObject.SetActive(false);
                GetImage((int)Panel.DifficultyPanel).gameObject.SetActive(true);
                break;

            case 1:
                GetButton((int)Btn.NextBtn).gameObject.SetActive(true);
                GetButton((int)Btn.PrevBtn).gameObject.SetActive(true);
                GetImage((int)Panel.BuffPanel).gameObject.SetActive(true);
                break;

            case 2:
                GetButton((int)Btn.NextBtn).gameObject.SetActive(true);
                GetButton((int)Btn.PrevBtn).gameObject.SetActive(true);
                GetImage((int)Panel.StatuePanel).gameObject.SetActive(true);
                break;

            case 3:
                GetButton((int)Btn.NextBtn).gameObject.SetActive(true);
                GetButton((int)Btn.PrevBtn).gameObject.SetActive(true);
                GetImage((int)Panel.UnitPanel).gameObject.SetActive(true);
                break;

            case 4:
                GetButton((int)Btn.NextBtn).gameObject.SetActive(false);
                GetButton((int)Btn.PrevBtn).gameObject.SetActive(true);
                GetImage((int)Panel.ArtifactPanel).gameObject.SetActive(true);
                break;
        }
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
        btn[4].gameObject.AddUIEvent(data => DifficultSelect(4));

        foreach (var item in btn)
        {
            item.gameObject.SetActive(false);
        }

        int currentMaxLevel = CollectionManager.Instance.RoundClearData.highestDifficultyLevel;
        for (int i = 0; i < currentMaxLevel + 2; i++)
        {
            if (i > 4) break;

            btn[i].gameObject.SetActive(true);
        }

        DifficultSelect(currentMaxLevel);
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
        Add_Content(Panel.BuffPanel, Bonus.Buff_GoldBonus);

    }

    void Init_Statue()
    {
        statueList.Add(Bonus.Statue_Mana);
        statueList.Add(Bonus.Statue_Gold);

        Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Gold);
        Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Mana);

        ////? 테스트용으로 임시 전부 개방
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dog);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Ravi);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dragon);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Cat);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Demon);
        //Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Hero);


        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Dog))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dog);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Ravi))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Ravi);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Dragon))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Dragon);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Cat))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Cat);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Demon))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Demon);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Hero))
        {
            Add_Content_Statue(Panel.StatuePanel, Bonus.Statue_Hero);
        }
    }

    void Init_Unit()
    {
        if (CollectionManager.Instance.Get_Collection_KeyName<SO_Monster>("BloodySlime").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_BloodySlime);
        }
        if (CollectionManager.Instance.Get_Collection_KeyName<SO_Monster>("FlameGolem").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_FlameGolem);
        }
        if (CollectionManager.Instance.Get_Collection_KeyName<SO_Monster>("Salinu").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Salinu);
        }
        if (CollectionManager.Instance.Get_Collection_KeyName<SO_Monster>("HellHound").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_HellHound);
        }
        if (CollectionManager.Instance.Get_Collection_KeyName<SO_Monster>("Griffin").info.isRegist)
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Griffin);
        }



        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Ravi))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Ravi);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Demon))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Lievil);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Hero))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Rideer);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Cat))
        {
            Add_Content(Panel.UnitPanel, Bonus.Unit_Rena);
        }

    }

    void Init_Artifact()
    {
        //? 임시로 전체 개방

        Add_Content(Panel.ArtifactPanel, Bonus.Arti_Pop);
        Add_Content(Panel.ArtifactPanel, Bonus.Arti_Danger);

        if (CollectionManager.Instance.RoundClearData.clearCounter >= 2)
        {
            Add_Content(Panel.ArtifactPanel, Bonus.Arti_DownDanger);
            Add_Content(Panel.ArtifactPanel, Bonus.Arti_DownPop);
        }

        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Demon))
        {
            Add_Content(Panel.ArtifactPanel, Bonus.Arti_Decay);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Hero))
        {
            Add_Content(Panel.ArtifactPanel, Bonus.Arti_Hero);
        }
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
    void Add_Content_Statue(Panel _panel, Bonus _label)
    {
        var parent = GetImage((int)_panel).GetComponentInChildren<GridLayoutGroup>();

        var obj = Managers.Resource.Instantiate("UI/PopUp/Element/NewGameBonusElement", parent.transform);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText_NGP($"{_label}")} <b>({BonusDict[_label].point}P)</b>";

        //? AddEvent - ContentClick
        obj.AddUIEvent(data => ContentClick_Statue(_label, obj.GetComponent<Button>()));

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
        BonusDict.Add(Bonus.Buff_ApBonusOne, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ApBonusOne, 15));
        BonusDict.Add(Bonus.Buff_ApBonusTwo, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ApBonusTwo, 30));
        BonusDict.Add(Bonus.Buff_PopBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_PopBonus, 5));
        BonusDict.Add(Bonus.Buff_DangerBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_DangerBonus, 5));
        BonusDict.Add(Bonus.Buff_ManaBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_ManaBonus, 2));
        BonusDict.Add(Bonus.Buff_GoldBonus, new BtnEvent(Panel.BuffPanel, Bonus.Buff_GoldBonus, 3));


        BonusDict.Add(Bonus.Statue_Mana, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Mana, 0));
        BonusDict.Add(Bonus.Statue_Gold, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Gold, 0));
        BonusDict.Add(Bonus.Statue_Dog, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Dog, 5));
        BonusDict.Add(Bonus.Statue_Cat, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Cat, 7));
        BonusDict.Add(Bonus.Statue_Dragon, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Dragon, 7));
        BonusDict.Add(Bonus.Statue_Ravi, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Ravi, 5));
        BonusDict.Add(Bonus.Statue_Demon, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Demon, 10));
        BonusDict.Add(Bonus.Statue_Hero, new BtnEvent(Panel.StatuePanel, Bonus.Statue_Hero, 10));



        BonusDict.Add(Bonus.Unit_BloodySlime, new BtnEvent(Panel.UnitPanel, Bonus.Unit_BloodySlime, 3));
        BonusDict.Add(Bonus.Unit_FlameGolem, new BtnEvent(Panel.UnitPanel, Bonus.Unit_FlameGolem, 3));
        BonusDict.Add(Bonus.Unit_HellHound, new BtnEvent(Panel.UnitPanel, Bonus.Unit_HellHound, 6));
        BonusDict.Add(Bonus.Unit_Salinu, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Salinu, 9));
        BonusDict.Add(Bonus.Unit_Griffin, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Griffin, 9));


        BonusDict.Add(Bonus.Unit_Rena, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Rena, 7));

        BonusDict.Add(Bonus.Unit_Ravi, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Ravi, 15));
        BonusDict.Add(Bonus.Unit_Lievil, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Lievil, 15));
        BonusDict.Add(Bonus.Unit_Rideer, new BtnEvent(Panel.UnitPanel, Bonus.Unit_Rideer, 15));



        //? artifact
        BonusDict.Add(Bonus.Arti_Hero, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Hero, 10));
        BonusDict.Add(Bonus.Arti_Decay, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Decay, 15));
        BonusDict.Add(Bonus.Arti_Pop, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Pop, 3));
        BonusDict.Add(Bonus.Arti_Danger, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_Danger, 3));
        BonusDict.Add(Bonus.Arti_DownDanger, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_DownDanger,6));
        BonusDict.Add(Bonus.Arti_DownPop, new BtnEvent(Panel.ArtifactPanel, Bonus.Arti_DownPop, 6));
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

        //? 유닛
        Unit_BloodySlime,
        Unit_FlameGolem,
        Unit_Salinu,
        Unit_HellHound,
        Unit_Griffin,
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
    }

}
