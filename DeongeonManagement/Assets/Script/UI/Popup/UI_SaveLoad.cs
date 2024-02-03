using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    }

    public enum Buttons
    {
        None,
        Save,
        Load,
    }

    public Buttons State;
    Slot SlotIndex;


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

        GetImage((int)Slot.SaveSlot_1).gameObject.AddUIEvent((data) => SlotClickEvent(1));
        GetImage((int)Slot.SaveSlot_2).gameObject.AddUIEvent((data) => SlotClickEvent(2));
        GetImage((int)Slot.SaveSlot_3).gameObject.AddUIEvent((data) => SlotClickEvent(3));

        SlotData();
    }

    public void SetMode(Buttons saveMode)
    {
        State = saveMode;
    }




    void SlotClickEvent(int index)
    {
        switch (State)
        {
            case Buttons.Save:
                Managers.Data.SaveToJson($"DM_Save_{index}", index);
                SlotData();
                break;

            case Buttons.Load:
                //ClosePopUp();
                Managers.Scene.AddLoadAction_OneTime(() => LoadAction(index));
                Managers.Scene.LoadSceneAsync("2_Management");
                break;
        }
    }


    void SlotData()
    {
        if (Managers.Data.SaveFileSearch($"DM_Save_{1}"))
        {
            GetImage((int)Slot.SaveSlot_1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Managers.Data.GetDateToFile($"DM_Save_{1}");
        }


        if (Managers.Data.SaveFileSearch($"DM_Save_{2}"))
        {
            GetImage((int)Slot.SaveSlot_2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Managers.Data.GetDateToFile($"DM_Save_{2}");
        }

        if (Managers.Data.SaveFileSearch($"DM_Save_{3}"))
        {
            GetImage((int)Slot.SaveSlot_3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Managers.Data.GetDateToFile($"DM_Save_{3}");
        }

    }


    void LoadAction(int index)
    {
        Main.Instance.Default_Init();
        Managers.Data.LoadToStorage($"DM_Save_{index}");
    }
}
