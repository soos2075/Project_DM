using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UI_SaveLoad : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Slot
    {
        SaveSlot_1,
        SaveSlot_2,
        SaveSlot_3,
        SaveSlot_4,
        SaveSlot_5,
        SaveSlot_6,
    }

    public enum Buttons
    {
        None,
        Save,
        Load,
    }

    public Buttons State;


    public override void Init()
    {
        base.Init();


        Bind<Image>(typeof(Slot));
        Bind<Button>(typeof(Buttons));

        if (State != Buttons.None)
        {
            GetButton(((int)Buttons.Save)).gameObject.SetActive(false);
            GetButton(((int)Buttons.Load)).gameObject.SetActive(false);
        }

        GetButton(((int)Buttons.Save)).gameObject.AddUIEvent((data) => State = Buttons.Save);
        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent((data) => State = Buttons.Load);



        for (int i = 0; i < Enum.GetNames(typeof(Slot)).Length; i++)
        {
            GetImage(i).gameObject.AddUIEvent((data) => SlotClickEvent(data.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex()));
        }

        ShowDataInfo();
    }

    public void SetMode(Buttons saveMode)
    {
        State = saveMode;
    }



    void SlotClickEvent(int index)
    {
        index += 1;
        switch (State)
        {
            case Buttons.Save:
                Managers.Data.SaveToJson($"DM_Save_{index}", index);
                ShowDataInfo();
                break;

            case Buttons.Load:
                if (Managers.Data.GetData($"DM_Save_{index}") == null)
                {
                    return;
                }
                //ClosePopUp();
                Managers.Scene.AddLoadAction_OneTime(() => LoadAction(index));
                Managers.Scene.LoadSceneAsync("2_Management");
                break;
        }
    }


    void ShowDataInfo()
    {
        for (int i = 1; i <= 6; i++)
        {
            var data = Managers.Data.GetData($"DM_Save_{i}");

            if (data != null)
            {
                GetImage(i - 1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{data.turn}일차";
                GetImage(i - 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{data.dateTime}";
                GetImage(i - 1).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"유명도 : {data.FameOfDungeon} / 위험도 : {data.DangerOfDungeon}";
            }
            else
            {
                GetImage(i - 1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"데이터 없음";
                GetImage(i - 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"";
                GetImage(i - 1).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"";
            }
        }
    }


    void LoadAction(int index)
    {
        Main.Instance.Default_Init();
        Managers.Data.LoadGame($"DM_Save_{index}");
    }
}
