using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Monster : UI_PopUp
{

    enum Objects
    {
        Return,
        Content,
        ResumeCount,
    }

    public int resumeCount { get; set; }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Return).gameObject.AddUIEvent(data => ClosePopUp());
        resumeCount = Main.Instance.CurrentFloor.Size;
    }
    void Start()
    {
        Init();
        GenerateContents();
        ResumeCountUpdate(0);
    }

    public void ResumeCountUpdate(int value)
    {
        resumeCount += value;
        Main.Instance.CurrentFloor.Size = resumeCount;
        GetObject((int)Objects.ResumeCount).GetComponent<TextMeshProUGUI>().text = $"배치 가능 횟수 : {resumeCount}";
    }


    void GenerateContents()
    {
        for (int i = 0; i < Main.Instance.Monsters.Length; i++)
        {
            UI_Placement_Content content = Managers.Resource.Instantiate("UI/PopUp/Element/Placement_Content", GetObject((int)Objects.Content).transform).
                GetComponent<UI_Placement_Content>();

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


}
