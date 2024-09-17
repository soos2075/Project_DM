using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterBox_Clear : UI_Base
{
    void Start()
    {
        Init();
    }



    public Save_MonsterData monster;
    public UI_Ending_Monster parent;

    enum Contents
    {
        BG,
        Line,
        Sprite,
        Name,
        State,
        Lv,
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        ShowContents();
        gameObject.AddUIEvent((data) => Selected());
    }


    void Selected()
    {
        if (monster != null)
        {
            Debug.Log("���õ� - ���ö���¡ �ؾ���!!");
            var ui = Managers.UI.ShowPopUp<UI_Confirm>();
            ui.SetText($"�� ���͸� ���������?(���ö���¡ �ؾ���)", () => TakeMonster());
        }
    }

    void TakeMonster()
    {
        UserData.Instance.SelectedMonster = monster;
        Managers.UI.ClosePopupPick(parent);
    }



    public void ShowContents()
    {
        if (monster == null)
        {
            Clear();
            return;
        }

        GetObject(((int)Contents.Sprite)).GetComponent<Image>().sprite = 
            Managers.Sprite.Get_SLA(SpriteManager.Library.Monster, monster.categoryName, monster.labelName);
        GetObject(((int)Contents.Name)).GetComponent<TextMeshProUGUI>().text = monster.savedName;
        GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = "";
        GetObject(((int)Contents.Lv)).GetComponent<TextMeshProUGUI>().text = $"Lv.{monster.LV}";
    }

    void Clear()
    {
        GetObject(((int)Contents.Sprite)).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        GetObject(((int)Contents.Name)).GetComponent<TextMeshProUGUI>().text = "";
        GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = "";
        GetObject(((int)Contents.Lv)).GetComponent<TextMeshProUGUI>().text = "";
    }






    //private void OnEnable()
    //{
    //    if (GetObject(((int)Contents.BG)) != null)
    //    {
    //        ShowContents();
    //    }
    //}


}
