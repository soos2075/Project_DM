using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CheatInput : UI_PopUp
{
    void Start()
    {
        Init();
    }



    enum InputField
    {
        Input_Day,
        Input_Mana,
        Input_Gold,
        Input_AP,
        Input_Popular,
        Input_Danger,
        Input_Rank,
    }

    enum Buttons
    {
        Confirm,
        Close,
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(InputField));
        Bind<Button>(typeof(Buttons));


        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent((data) => ConfirmButton());
        GetButton((int)Buttons.Close).gameObject.AddUIEvent((data) => Close());
    }


    void Close()
    {
        gameObject.SetActive(false);
    }


    void ConfirmButton()
    {
        if (Managers.Scene.GetCurrentScene() != SceneName._2_Management)
        {
            return;
        }


        //for (int i = 0; i < Enum.GetNames(typeof(InputField)).Length; i++)
        //{
        //    var input = GetObject(i).GetComponent<TMP_InputField>();
        //    Debug.Log($"{(InputField)i} : {input.text}");
        //}


        {
            var input = GetObject((int)InputField.Input_Day).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var day = int.Parse(input);
                //day = Mathf.Clamp(day, 0, 29);

                Main.Instance.Turn = day;

                Main.Instance.DayList = new List<Main.DayResult>();
                for (int i = 0; i < day; i++)
                {
                    Main.Instance.DayList.Add(new Main.DayResult());
                }

                Main.Instance.CurrentDay = new Main.DayResult();
            }
        }

        {
            var input = GetObject((int)InputField.Input_Mana).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var mana = int.Parse(input);
                Main.Instance.CurrentDay.AddMana(mana, Main.DayResult.EventType.Etc);
            }
        }

        {
            var input = GetObject((int)InputField.Input_Gold).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var value = int.Parse(input);
                Main.Instance.CurrentDay.AddGold(value, Main.DayResult.EventType.Etc);
            }
        }

        {
            var input = GetObject((int)InputField.Input_Danger).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var value = int.Parse(input);
                Main.Instance.CurrentDay.AddDanger(value);
            }
        }

        {
            var input = GetObject((int)InputField.Input_Popular).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var value = int.Parse(input);
                Main.Instance.CurrentDay.AddPop(value);
            }
        }

        {
            var input = GetObject((int)InputField.Input_AP).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var value = int.Parse(input);
                Main.Instance.Player_AP = (value);
            }
        }

        {
            var input = GetObject((int)InputField.Input_Rank).GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(input) == false)
            {
                var value = int.Parse(input);
                Main.Instance.DungeonRank = (value);
            }
        }

    }
}
