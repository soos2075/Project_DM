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
        GetObject((int)Contents.ResumeCount).GetComponent<TextMeshProUGUI>().text = $"���� ���� Ƚ�� : {resumeCount}";
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
                Debug.Log($"{Main.Instance.Monsters[i].name} �Ʒ�����");
            }
        }
        
        ShowResult();
    }

    void ShowResult()
    {
        ClosePopUp();

        Debug.Log("���� ����� �Ȱ� �ִϸ��̼� �����ϰų� �˾�â �Ʒ����ళ����ŭ ������� Ŭ���ؼ� ���ֱ�");
    }

}
