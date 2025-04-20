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




    public enum DataState
    {
        Select,
        Save,
        Load,
    }

    DataState State { get; set; }

    //public Sprite button_Down;
    //public Sprite button_Up;

    //public Sprite slot_Active;
    //public Sprite slot_Inactive;

    enum Slot
    {
        AutoSave,
    }
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

        // 마지막 세이브 슬롯의 인덱스
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
            if (data.isClear) // 클리어 데이터면 걍 몬스터만 고르고 바로 끝 or 나중에 무한모드로 가든지 말든지(도전과제같은거?)
            {
                //UserData.Instance.GameClear(data);
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

                // 마지막 세이브 슬롯의 인덱스
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
                        if (autodata.isClear) // 클리어 데이터면 걍 몬스터만 고르고 바로 끝 or 나중에 무한모드로 가든지 말든지
                        {
                            //UserData.Instance.GameClear(autodata);
                            return;
                        }

                        // 클리어 데이터가 아니면 정상로드
                        Managers.Scene.AddLoadAction_OneTime(() => LoadAction($"AutoSave"));
                        //Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame($"AutoSave"));
                        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
                        //UserData.Instance.GameMode = Define.GameMode.Normal;
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
                    if (data.isClear) // 클리어 데이터면 걍 몬스터만 고르고 바로 끝 or 나중에 무한모드로 가든지 말든지(도전과제같은거?)
                    {
                        //UserData.Instance.GameClear(data);
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
            string diff = "★";
            for (int i = 0; i < data.difficultyLevel; i++)
            {
                diff += "★";
            }

            SaveSlotList[_index - 1].transform.Find("_Slot").GetComponentInChildren<TextMeshProUGUI>().text = 
                $"{UserData.Instance.LocaleText("Slot")} {_index} - {diff}";


            SaveSlotList[_index - 1].transform.Find("_Date").GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.GetLocalDateTime(data.dateTime)}";

            SaveSlotList[_index - 1].transform.Find("_Day").GetComponentInChildren<TextMeshProUGUI>().text = $"{data.mainData.turn}{UserData.Instance.LocaleText("Day")}";
            SaveSlotList[_index - 1].transform.Find("_Rank").GetComponentInChildren<TextMeshProUGUI>().text =
                $"{(Define.DungeonRank)data.mainData.DungeonLV}";

            SaveSlotList[_index - 1].transform.Find("_Pop").GetComponentInChildren<TextMeshProUGUI>().text = $"{data.mainData.FameOfDungeon}";
            SaveSlotList[_index - 1].transform.Find("_Danger").GetComponentInChildren<TextMeshProUGUI>().text = $"{data.mainData.DangerOfDungeon}";

            if (data.isClear)
            {
                SaveSlotList[_index - 1].transform.Find("_Day").GetComponentInChildren<TextMeshProUGUI>().text = $"{data.mainData.turn} : Clear";
            }
        }
        else
        {
            SaveSlotList[_index - 1].transform.Find("_Slot").GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("Slot")} {_index}";
            SaveSlotList[_index - 1].transform.Find("_Date").GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("No Data")}";
            SaveSlotList[_index - 1].transform.Find("_Day").GetComponentInChildren<TextMeshProUGUI>().text = $"";
            SaveSlotList[_index - 1].transform.Find("_Rank").GetComponentInChildren<TextMeshProUGUI>().text = $"";
            SaveSlotList[_index - 1].transform.Find("_Pop").GetComponentInChildren<TextMeshProUGUI>().text = $"";
            SaveSlotList[_index - 1].transform.Find("_Danger").GetComponentInChildren<TextMeshProUGUI>().text = $"";
        }




    }
    void ShowAutoInfo()
    {
        var autodata = Managers.Data.GetData($"AutoSave");
        if (autodata != null)
        {
            string diff = "★";
            for (int i = 0; i < autodata.difficultyLevel; i++)
            {
                diff += "★";
            }

            GetImage(((int)Slot.AutoSave)).transform.Find("_Slot").GetComponentInChildren<TextMeshProUGUI>().text = 
                $"{UserData.Instance.LocaleText("AutoSave")} - {diff}";

            GetImage(((int)Slot.AutoSave)).transform.Find("_Date").GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.GetLocalDateTime(autodata.dateTime)}";

            GetImage(((int)Slot.AutoSave)).transform.Find("_Day").GetComponentInChildren<TextMeshProUGUI>().text = $"{autodata.mainData.turn}{UserData.Instance.LocaleText("Day")}";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Rank").GetComponentInChildren<TextMeshProUGUI>().text =
                $"{(Define.DungeonRank)autodata.mainData.DungeonLV}";

            GetImage(((int)Slot.AutoSave)).transform.Find("_Pop").GetComponentInChildren<TextMeshProUGUI>().text = $"{autodata.mainData.FameOfDungeon}";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Danger").GetComponentInChildren<TextMeshProUGUI>().text = $"{autodata.mainData.DangerOfDungeon}";

            if (autodata.isClear)
            {
                GetImage(((int)Slot.AutoSave)).transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = $"{autodata.mainData.turn} : Clear";
            }
        }
        else
        {
            GetImage(((int)Slot.AutoSave)).transform.Find("_Slot").GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("AutoSave")}";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Date").GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("No Data")}";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Day").GetComponentInChildren<TextMeshProUGUI>().text = $"";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Rank").GetComponentInChildren<TextMeshProUGUI>().text = $"";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Pop").GetComponentInChildren<TextMeshProUGUI>().text = $"";
            GetImage(((int)Slot.AutoSave)).transform.Find("_Danger").GetComponentInChildren<TextMeshProUGUI>().text = $"";
        }

    }


    void LoadAction(int index)
    {
        Main.Instance.Default_Init();
        Managers.Data.LoadGame($"DM_Save_{index}");
        //Managers.Data.LoadGame_ToFile(index);
    }

    void LoadAction(string slotName)
    {
        Main.Instance.Default_Init();
        Managers.Data.LoadGame(slotName);
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
            SaveSlotButtonList[i].gameObject.GetComponent<Image>().color = Color.white;
        }
        SaveSlotButtonList[index].gameObject.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }




    #endregion


    #region UI Pop 기본 열기/닫기 함수
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
