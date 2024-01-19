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
    public int resumeCount { get; set; }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(Contents));
        contentData = new UI_Content[Main.Instance.Monsters.Length];

        resumeCount = Main.Instance.TrainingCount;
        ResumeCountUpdate(0);
        GetObject((int)Contents.Return).AddUIEvent((data) => ClosePopUp());
        GetObject((int)Contents.Confirm).AddUIEvent((data) => TrainingConfirm());
    }


    void Start()
    {
        Init();
        GenerateContents();
    }

    void Update()
    {
        
    }


    public void ResumeCountUpdate(int value)
    {
        resumeCount += value;
        GetObject((int)Contents.ResumeCount).GetComponent<TextMeshProUGUI>().text = $"선택 가능 횟수 : {resumeCount}";
    }



    void GenerateContents()
    {
        for (int i = 0; i < Main.Instance.Monsters.Length; i++)
        {
            UI_Content content = Managers.Resource.Instantiate("UI/PopUp/Element/Content", GetObject((int)Contents.LayoutGroup).transform)
                .GetComponent<UI_Content>();
            contentData[i] = content;
            if (Main.Instance.Monsters[i] != null)
            {
                content.MonsterID = i;
            }
            else
            {
                content.MonsterID = -1;
            }
        }
    }


    void TrainingConfirm()
    {
        for (int i = 0; i < contentData.Length; i++)
        {
            if (contentData[i].State == UI_Content.ContentState.Blue)
            {
                Main.Instance.Monsters[i].Training();
                Main.Instance.TrainingCount--;
                Debug.Log($"{Main.Instance.Monsters[i].name} 훈련진행");
            }
        }
        
        ShowResult();
    }

    void ShowResult()
    {
        ClosePopUp();

        Debug.Log("몬스터 스펙업 된거 애니메이션 진행하거나 팝업창 훈련진행개수만큼 띄워놓고 클릭해서 없애기");
    }

}
