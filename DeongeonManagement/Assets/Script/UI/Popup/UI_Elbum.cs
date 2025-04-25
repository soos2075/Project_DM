using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class UI_Elbum : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Objects
    {
        MainPanel,
        EndingBox,
        ShowBox,
        CGBox,
    }

    enum Buttons
    {
        Button_Replay,
        Button_CG,

        Close,
        CG_Close,

        Next,
        Prev,
    }

    enum TMP_Texts
    {
        MainText,
        HintText,

        Text_CG,
    }

    enum Images
    {
        MainImage,
        CG_Main
    }



    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(Objects));
        Bind<Button>(typeof(Buttons));
        Bind<TMPro.TextMeshProUGUI>(typeof(TMP_Texts));
        Bind<Image>(typeof(Images));

        SLA = GetImage((int)Images.MainImage).GetComponent<SpriteLibrary>().spriteLibraryAsset;

        GetButton((int)Buttons.Close).gameObject.AddUIEvent(data => ClosePopUp());
        GetButton((int)Buttons.Button_Replay).gameObject.AddUIEvent(data => Replay_Click());
        GetButton((int)Buttons.Button_CG).gameObject.AddUIEvent(data => CG_Click());

        Init_EndingContents();
        ShowBox_Clear();
        Init_CG_Box();
    }



    void ShowBox_Clear()
    {
        GetImage((int)Images.MainImage).sprite = Managers.Sprite.GetClear();
        GetButton((int)Buttons.Button_Replay).gameObject.SetActive(false);
        GetButton((int)Buttons.Button_CG).gameObject.SetActive(false);

        GetTMP((int)TMP_Texts.MainText).text = $"";
        GetTMP((int)TMP_Texts.HintText).text = $"";
    }




    void Init_EndingContents()
    {
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Ending_Content", GetObject((int)Objects.EndingBox).transform);
            content.GetComponent<UI_Ending_Content>().Set_Data(this, CollectionManager.Instance.GetData_Ending(Endings.Dog));
        }
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Ending_Content", GetObject((int)Objects.EndingBox).transform);
            content.GetComponent<UI_Ending_Content>().Set_Data(this, CollectionManager.Instance.GetData_Ending(Endings.Dragon));
        }
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Ending_Content", GetObject((int)Objects.EndingBox).transform);
            content.GetComponent<UI_Ending_Content>().Set_Data(this, CollectionManager.Instance.GetData_Ending(Endings.Ravi));
        }
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Ending_Content", GetObject((int)Objects.EndingBox).transform);
            content.GetComponent<UI_Ending_Content>().Set_Data(this, CollectionManager.Instance.GetData_Ending(Endings.Cat));
        }
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Ending_Content", GetObject((int)Objects.EndingBox).transform);
            content.GetComponent<UI_Ending_Content>().Set_Data(this, CollectionManager.Instance.GetData_Ending(Endings.Demon));
        }
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Ending_Content", GetObject((int)Objects.EndingBox).transform);
            content.GetComponent<UI_Ending_Content>().Set_Data(this, CollectionManager.Instance.GetData_Ending(Endings.Hero));
        }

        //? 아래는 크레딧이나 이런거 추가해도 댐
    }


    SpriteLibraryAsset SLA;

    SO_Ending Current_Data;

    public void Ending_ClickEvent(SO_Ending data, bool isClear)
    {
        Current_Data = data;
        if (isClear)
        {
            //? 클리어한 데이터 ShowBox
            //Debug.Log("클리어");

            GetButton((int)Buttons.Button_Replay).gameObject.SetActive(true);
            GetButton((int)Buttons.Button_CG).gameObject.SetActive(true);
            GetImage((int)Images.MainImage).sprite = SLA.GetSprite(data.keyName, "Clear");

            GetTMP((int)TMP_Texts.MainText).text = UserData.Instance.LocaleText_NGP($"Clear_{data.keyName}");

            GetTMP((int)TMP_Texts.HintText).text = "";

            var log = UserData.Instance.CurrentPlayerData.GetDataLog((Endings)data.id);
            if (log.difficultyLevel == -1)
            {
                return;
            }

            int clearcount = UserData.Instance.CurrentPlayerData.GetEndingCount(log.endings);
            //CollectionManager.Instance.RoundClearData.endingClearCount.TryGetValue(log.endings, out clearcount);

            string diff = "★";
            for (int i = 0; i < log.difficultyLevel; i++)
            {
                diff += "★";
            }

            GetTMP((int)TMP_Texts.HintText).text = $"{UserData.Instance.LocaleText_NGP("Log_클리어횟수")} : {clearcount}" +
                $"\n{UserData.Instance.LocaleText_NGP("Log_클리어난이도")} : {diff}" +
                $"\n{UserData.Instance.LocaleText_NGP("Log_클리어시간")} : {(int)log.clearTime / 60}m {(int)log.clearTime % 60}s";
        }
        else
        {
            //? 미클리어니까 잠김이미지와 힌트
            //Debug.Log("못깸");

            GetButton((int)Buttons.Button_Replay).gameObject.SetActive(false);
            GetButton((int)Buttons.Button_CG).gameObject.SetActive(false);
            GetImage((int)Images.MainImage).sprite = SLA.GetSprite(data.keyName, "Lock");

            GetTMP((int)TMP_Texts.MainText).text = $"";
            GetTMP((int)TMP_Texts.HintText).text = UserData.Instance.LocaleText_NGP($"Hint_{data.keyName}");
        }
    }

    void Replay_Click()
    {
        UserData.Instance.EndingState = (Endings)Current_Data.id;
        Managers.Scene.LoadSceneAsync(SceneName._7_NewEnding, false);
    }



    

    int current_Num;
    List<Sprite> CurrentScene;

    void Init_CG_Box()
    {
        GetButton((int)Buttons.CG_Close).gameObject.AddUIEvent(data => CG_Close());
        GetButton((int)Buttons.Next).gameObject.AddUIEvent(data => Set_CG(1));
        GetButton((int)Buttons.Prev).gameObject.AddUIEvent(data => Set_CG(-1));
        GetObject((int)Objects.CGBox).SetActive(false);
    }


    void CG_Click()
    {
        if (Current_Data == null) return;

        GetObject((int)Objects.CGBox).SetActive(true);

        CurrentScene = new List<Sprite>();
        foreach (var item in Current_Data.cutSceneList)
        {
            CurrentScene.Add(item.sprite);
        }

        Set_CG(0);
    }

    void CG_Close()
    {
        GetObject((int)Objects.CGBox).SetActive(false);
        current_Num = 0;
    }


    void Set_CG(int changeValue)
    {
        if (CurrentScene == null) return;

        current_Num += changeValue;
        if (current_Num >= CurrentScene.Count)
        {
            current_Num = CurrentScene.Count - 1;
        }
        if (current_Num < 0)
        {
            current_Num = 0;
        }

        GetImage((int)Images.CG_Main).sprite = CurrentScene[current_Num];
        GetTMP((int)TMP_Texts.Text_CG).text = $"{current_Num + 1} / {CurrentScene.Count}";
    }

}
