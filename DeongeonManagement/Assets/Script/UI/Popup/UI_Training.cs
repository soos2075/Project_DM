using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Training : UI_PopUp
{
    enum Contents
    {
        LayoutGroup,
        Return,
        ResumeCount,
        Confirm,
    }

    UI_Content[] contentData;
    public int resumeCount;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(Contents));
        contentData = new UI_Content[Main.Instance.monsters.Length];

        resumeCount = Main.Instance.TrainingCount + 1;
        ResumeCountUpdate();
        GetObject((int)Contents.Return).AddUIEvent((data) => ClosePopUp());
        GetObject((int)Contents.Confirm).AddUIEvent((data) => TrainingConfirm());
    }


    void Start()
    {
        Init();
        GenerateContents();
        FillContent();
    }

    void Update()
    {
        
    }


    public void ResumeCountUpdate()
    {
        resumeCount--;
        GetObject((int)Contents.ResumeCount).GetComponent<TextMeshProUGUI>().text = $"선택 가능 횟수 : {resumeCount}";
    }



    void GenerateContents()
    {
        for (int i = 0; i < Main.Instance.monsters.Length; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Content", GetObject((int)Contents.LayoutGroup).transform);
            contentData[i] = content.GetComponent<UI_Content>();
        }
    }
    void FillContent()
    {
        for (int i = 0; i < contentData.Length; i++)
        {
            if (Main.Instance.monsters[i] != null)
            {
                contentData[i].transform.GetChild(0).GetComponent<Image>().sprite = Main.Instance.monsters[i].Sprite;
                contentData[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    $"이름 : {Main.Instance.monsters[i].name}\n" +
                    $"HP : {Main.Instance.monsters[i].HP}\n" +
                    $"LV : {Main.Instance.monsters[i].LV}";
                contentData[i].State = UI_Content.ContentState.Possible;
            }
            else
            {
                contentData[i].State = UI_Content.ContentState.Nothing;
            }
        }
    }



    void TrainingConfirm()
    {
        Debug.Log("선택된 몬스터들 훈련진행");
    }

}
