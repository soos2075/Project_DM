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
        AutoSave,
    }

    public enum DataState
    {
        Select,
        Save,
        Load,
    }

    DataState State { get; set; }

    public Sprite button_Down;
    public Sprite button_Up;

    public Sprite slot_Active;
    public Sprite slot_Inactive;

    enum GameObjects
    {
        IndexBox,
        MainPanel,

        NoTouch,
        Close,
    }


    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Slot));
        Bind<GameObject>(typeof(GameObjects));

        GetObject(((int)GameObjects.Close)).gameObject.AddUIEvent((data) => ClosePopUp());


        GetObject((int)GameObjects.MainPanel).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject((int)GameObjects.NoTouch).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);


        Init_SaveSlot();

        GetImage((int)Slot.AutoSave).gameObject.AddUIEvent((data) => SlotClickEvent(0, DataState.Load));

        //for (int i = 0; i < Enum.GetNames(typeof(Slot)).Length; i++)
        //{
        //    GetImage(i).gameObject.AddUIEvent((data) => SlotClickEvent(data.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex()));
        //}

        //ShowDataInfo();
        ShowAutoInfo();
    }

    public void SetMode(DataState saveMode)
    {
        State = saveMode;
    }


    //void SaveButton()
    //{
    //    State = Buttons.Save;
    //    GetButton(((int)Buttons.Save)).GetComponent<Image>().sprite = button_Down;
    //    GetButton(((int)Buttons.Save)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 18, 0, 0);

    //    GetButton(((int)Buttons.Load)).GetComponent<Image>().sprite = button_Up;
    //    GetButton(((int)Buttons.Load)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 5, 0, 0);
    //}
    //void LoadButton()
    //{
    //    State = Buttons.Load;
    //    GetButton(((int)Buttons.Load)).GetComponent<Image>().sprite = button_Down;
    //    GetButton(((int)Buttons.Load)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 18, 0, 0);

    //    GetButton(((int)Buttons.Save)).GetComponent<Image>().sprite = button_Up;
    //    GetButton(((int)Buttons.Save)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 5, 0, 0);
    //}


    void Show_SaveOption(int index)
    {
        var confirm = Managers.UI.ShowPopUpAlone<UI_SaveLoad_Confirm>();
        confirm.SetAction($"{UserData.Instance.LocaleText("Slot")} {index}", () => SaveDataConfirm(index), () => LoadDataConfirm(index));
    }


    void SaveDataConfirm(int index)
    {
        Managers.Data.SaveAndAddFile($"DM_Save_{index}", index);
        UserData.Instance.SetData(PrefsKey.SaveTimes, UserData.Instance.GetDataInt(PrefsKey.SaveTimes) + 1);

        ShowDataInfo(index);

        SoundManager.Instance.PlaySound("SFX/Save");
        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = UserData.Instance.LocaleText("Message_Saved");

        // ������ ���̺� ������ �ε���
        UserData.Instance.SetData(PrefsKey.LastSaveSlotIndex, (index - 1) / 6);
    }
    void LoadDataConfirm(int index)
    {
        var data = Managers.Data.GetData($"DM_Save_{index}");
        if (data == null)
        {
            return;
        }
        else
        {
            if (data.isClear) // Ŭ���� �����͸� �� ���͸� ���� �ٷ� �� or ���߿� ���Ѹ��� ������ ������(��������������?)
            {
                UserData.Instance.GameClear(data);
                return;
            }

            Managers.Scene.AddLoadAction_OneTime(() => LoadAction(index));
            Managers.Scene.LoadSceneAsync(SceneName._2_Management);
        }
    }


    void SlotClick(int index)
    {
        switch (State)
        {
            case DataState.Select:
                var data = Managers.Data.GetData($"DM_Save_{index}");
                if (data == null)
                {
                    SlotClickEvent(index, DataState.Save);
                }
                else
                {
                    Show_SaveOption(index);
                }
                break;

            case DataState.Save:
                SlotClickEvent(index, DataState.Save);
                break;

            case DataState.Load:
                SlotClickEvent(index, DataState.Load);
                break;
        }
    }




    void SlotClickEvent(int index, DataState _currentState)
    {
        //index += 1;
        switch (_currentState)
        {
            case DataState.Save:
                if (index == 0)
                {
                    return;
                }
                if (EventManager.Instance.Temp_saveData != null)
                {
                    Managers.Data.SaveAndAddFile(EventManager.Instance.Temp_saveData, $"DM_Save_{index}", index);
                }
                else
                {
                    Managers.Data.SaveAndAddFile($"DM_Save_{index}", index);
                    UserData.Instance.SetData(PrefsKey.SaveTimes, UserData.Instance.GetDataInt(PrefsKey.SaveTimes) + 1);
                }

                ShowDataInfo(index);

                SoundManager.Instance.PlaySound("SFX/Save");
                var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
                msg.Message = UserData.Instance.LocaleText("Message_Saved");

                // ������ ���̺� ������ �ε���
                UserData.Instance.SetData(PrefsKey.LastSaveSlotIndex, (index - 1) / 6);
                break;

            case DataState.Load:
                if (index == 0)
                {
                    var autodata = Managers.Data.GetData($"AutoSave");
                    if (autodata == null)
                    {
                        return;
                    }
                    else
                    {
                        if (autodata.isClear) // Ŭ���� �����͸� �� ���͸� ���� �ٷ� �� or ���߿� ���Ѹ��� ������ ������
                        {
                            UserData.Instance.GameClear(autodata);
                            return;
                        }

                        // Ŭ���� �����Ͱ� �ƴϸ� ����ε�
                        Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Default_Init());
                        Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame($"AutoSave"));
                        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
                        UserData.Instance.GameMode = Define.GameMode.Normal;
                        return;
                    }
                }


                var data = Managers.Data.GetData($"DM_Save_{index}");
                if (data == null)
                {
                    return;
                }
                else
                {
                    if (data.isClear) // Ŭ���� �����͸� �� ���͸� ���� �ٷ� �� or ���߿� ���Ѹ��� ������ ������(��������������?)
                    {
                        UserData.Instance.GameClear(data);
                        return;
                    }

                    Managers.Scene.AddLoadAction_OneTime(() => LoadAction(index));
                    Managers.Scene.LoadSceneAsync(SceneName._2_Management);
                }

                break;
        }
    }


    void ShowDataInfo(int _index)
    {
        var data = Managers.Data.GetData($"DM_Save_{_index}");

        if (data != null)
        {
            if (data.isClear)
            {
                SaveSlotList[_index - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("Slot")} {_index}";
                SaveSlotList[_index - 1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.GetLocalDateTime(data.dateTime)}";
                SaveSlotList[_index - 1].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    $"Clear : " + $"{data.endgins.ToString()}";
            }
            else
            {
                SaveSlotList[_index - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("Slot")} {_index}";
                SaveSlotList[_index - 1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.GetLocalDateTime(data.dateTime)}";
                SaveSlotList[_index - 1].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    $"{data.turn}{UserData.Instance.LocaleText("Day")}\n" +
                    $"\t{UserData.Instance.LocaleText("Popularity")} : {data.FameOfDungeon}\n" +
                    $"\t{UserData.Instance.LocaleText("Danger")} : {data.DangerOfDungeon}";
            }
        }
        else
        {
            SaveSlotList[_index - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("Slot")} {_index}";
            SaveSlotList[_index - 1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"";
            SaveSlotList[_index - 1].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("No Data")}";
        }




    }
    void ShowAutoInfo()
    {
        var autodata = Managers.Data.GetData($"AutoSave");
        if (autodata != null)
        {
            if (autodata.isClear)
            {
                GetImage(((int)Slot.AutoSave)).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("AutoSave")}";
                GetImage(((int)Slot.AutoSave)).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.GetLocalDateTime(autodata.dateTime)}";
                GetImage(((int)Slot.AutoSave)).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    $"Clear : " + $"{autodata.endgins.ToString()}";
            }
            else
            {
                GetImage(((int)Slot.AutoSave)).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("AutoSave")}";
                GetImage(((int)Slot.AutoSave)).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.GetLocalDateTime(autodata.dateTime)}";
                GetImage(((int)Slot.AutoSave)).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    $"{autodata.turn}{UserData.Instance.LocaleText("Day")}\n" +
                    $"\t{UserData.Instance.LocaleText("Popularity")} : {autodata.FameOfDungeon}\n" +
                    $"\t{UserData.Instance.LocaleText("Danger")} : {autodata.DangerOfDungeon}";
            }
        }
        else
        {
            GetImage(((int)Slot.AutoSave)).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("AutoSave")}";
            GetImage(((int)Slot.AutoSave)).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"";
            GetImage(((int)Slot.AutoSave)).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("No Data")}";
        }

    }


    void LoadAction(int index)
    {
        Main.Instance.Default_Init();
        Managers.Data.LoadGame($"DM_Save_{index}");
        //Managers.Data.LoadGame_ToFile(index);
    }



    #region SaveSlot

    List<Button> SaveSlotButtonList = new List<Button>();
    List<GameObject> SaveSlotBoxList = new List<GameObject>();
    List<GameObject> SaveSlotList = new List<GameObject>();
    void Init_SaveSlot()
    {
        var box = GetObject((int)GameObjects.IndexBox).transform;

        for (int i = 0; i < box.childCount; i++)
        {
            var button = box.GetChild(i).gameObject.GetComponent<Button>();
            button.gameObject.AddUIEvent((data) => SelectSlotBox(data.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex()));
            SaveSlotButtonList.Add(button);

            var slot = Managers.Resource.Instantiate("UI/PopUp/Element/SaveSlot", GetObject((int)GameObjects.MainPanel).transform);
            SaveSlotBoxList.Add(slot);

            for (int j = 0; j < slot.transform.childCount; j++)
            {
                GameObject obj = slot.transform.GetChild(j).gameObject;
                obj.name = $"SaveSlot_{(i * 6) + (j + 1)}";

                obj.AddUIEvent((data) =>
                {
                    var str = data.pointerCurrentRaycast.gameObject.name.Substring(data.pointerCurrentRaycast.gameObject.name.IndexOf("_") + 1);
                    //Debug.Log(str);
                    //SlotClickEvent(int.Parse(str));
                    SlotClick(int.Parse(str));
                });
                SaveSlotList.Add(obj);
            }
        }

        SelectSlotBox(UserData.Instance.GetDataInt(PrefsKey.LastSaveSlotIndex, 0));
    }

    void SelectSlotBox(int index)
    {
        ButtonColor(index);

        for (int i = 0; i < SaveSlotBoxList.Count; i++)
        {
            SaveSlotBoxList[i].gameObject.SetActive(false);
        }
        SaveSlotBoxList[index].gameObject.SetActive(true);

        for (int i = 0; i < SaveSlotBoxList[index].transform.childCount; i++)
        {
            ShowDataInfo((index * 6) + i + 1);
        }
    }

    void ButtonColor(int index)
    {
        for (int i = 0; i < SaveSlotButtonList.Count; i++)
        {
            SaveSlotButtonList[i].gameObject.GetComponent<Image>().sprite = slot_Inactive;
        }
        SaveSlotButtonList[index].gameObject.GetComponent<Image>().sprite = slot_Active;
    }




    #endregion


    #region UI Pop �⺻ ����/�ݱ� �Լ�
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

    #endregion
}
