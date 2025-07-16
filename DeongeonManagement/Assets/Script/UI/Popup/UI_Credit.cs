using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Credit : UI_PopUp
{
    void Start()
    {
        Init();
    }

    private void LateUpdate()
    {
        //Debug.Log(stateInfo.normalizedTime);

        if (isOver) return;

        if (Input.anyKey)
        {
            //anim.speed = 2.0f;
            Time.timeScale = 2;
        }
        else
        {
            //anim.speed = 1.0f;
            Time.timeScale = 1;
        }


        //stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //if (stateInfo.normalizedTime >= 1.0f)
        //{
        //    Close_Credit();
        //}

        if (moveCor == null)
        {
            Close_Credit();
            return;
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close_Credit();
            return;
        }
    }


    enum Objects
    {
        Panel_Main,
        MoveBox,
        Credit_End,
    }

    public GameObject element_Text;
    public GameObject element_Image;
    public GameObject element_Black;

    public Sprite unity_logo;



    Animator anim;
    AnimatorStateInfo stateInfo;

    bool isOver;
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Objects));

        SoundManager.Instance.PlaySound("BGM/_Credit_Noname", Define.AudioType.BGM);

        //anim = GetComponentInChildren<Animator>();
        //stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        GetObject((int)Objects.Panel_Main).AddUIEvent((data) => Close_Credit(), Define.UIEvent.RightClick);

        movingBox = GetObject((int)Objects.MoveBox).GetComponent<RectTransform>();
        moveCor = StartCoroutine(Play_Credit());

        Setting_Credit();
    }

    void Close_Credit()
    {
        //SoundManager.Instance.PlaySound("BGM/_Title_Arcade", Define.AudioType.BGM);
        SoundManager.Instance.Play_Main_Default();
        isOver = true;
        ClosePopUp();
    }


    RectTransform movingBox;
    Coroutine moveCor;



    void Setting_Credit()
    {
        Add_Blank(new Vector2Int(1200, 250));

        //Add_Text($"<b>Novice Dungeon Master", 72);

        Add_Blank(new Vector2Int(1200, 100));

        Add_Text("<b>Developed By", 72);
        Add_Text("LazyCnD", 64);
        Add_Blank(new Vector2Int(1200, 50));

        Add_Text("<b>Programming ", 72);
        Add_Text("LazyCnD", 64);
        Add_Blank(new Vector2Int(1200, 50));

        Add_Text("<b>Art & Graphics", 72);
        Add_Text("QU4RTER \nBongbong", 64);
        Add_Blank(new Vector2Int(1200, 50));


        Add_Text("<b> Music", 72);
        Add_Text("Homi", 64);
        Add_Blank(new Vector2Int(1200, 50));

        Add_Text("<b>Localization", 72);
        Add_Text("Claude\nDeepL\nMasa K", 64);
        Add_Blank(new Vector2Int(1200, 50));

        Add_Text("<b>Playtesting", 72);
        Add_Text("RedRay \n ssaco \n ryumirai \n 다피네 \n Stove Indie \nJHJ \nJade ", 64);
        Add_Blank(new Vector2Int(1200, 50));


        Add_Text("<b>Special Thanks", 72);
        Add_Text("RedRay \n QU4RTER \n 다피네 \nJHJ \n yupi \n MasaK  \nAHN \nKSK \n ", 64);
        Add_Blank(new Vector2Int(1200, 200));

        Add_Image(unity_logo, new Vector2Int(1200, 200));
    }

    enum Alignment
    {
        Center,
        Left,
        Right,
    }

    void Add_Blank(Vector2Int _size)
    {
        var blk = Instantiate(element_Black, movingBox.transform);
        blk.GetComponent<RectTransform>().sizeDelta = _size;
    }

    void Add_Image(Sprite _sprite, Vector2Int _size)
    {
        var img = Instantiate(element_Image, movingBox.transform);
        img.GetComponent<Image>().sprite = _sprite;
        img.GetComponent<RectTransform>().sizeDelta = _size;
    }



    void Add_Text(string _main, int _size, Alignment alignOption = Alignment.Center)
    {
        var txt = Instantiate(element_Text, movingBox.transform);
        txt.GetComponent<TextMeshProUGUI>().text = _main;
        txt.GetComponent<TextMeshProUGUI>().fontSize = _size;
    }


    IEnumerator Play_Credit()
    {

        yield return new WaitForSecondsRealtime(1);

        //? 마지막에 유니티 로고 가운데올쯤에 끝내야되니까 일정값을 빼줌
        float h_size = movingBox.sizeDelta.y - 460;

        float current_h_pos = 0;

        while (current_h_pos <= h_size)
        {
            yield return null;
            current_h_pos += Time.deltaTime * 70;
            movingBox.localPosition = new Vector3(0, current_h_pos, 0);
        }

        yield return new WaitForSecondsRealtime(3.0f);

        GetObject((int)Objects.MoveBox).SetActive(false);
        GetObject((int)Objects.Credit_End).SetActive(true);

        yield return new WaitForSecondsRealtime(5.0f);

        Debug.Log("재생종료");
        moveCor = null;
    }

}
