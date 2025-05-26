using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BattleManager : MonoBehaviour
{
    #region singleton
    private static BattleManager _instance;
    public static BattleManager Instance { get { Init(); return _instance; } }

    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<BattleManager>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@BattleManager");
                _instance = go.AddComponent<BattleManager>();
                DontDestroyOnLoad(go);
            }
        }
    }
    #endregion


    void Start()
    {
        Init_BattlePosList();
    }


    public void TurnStart()
    {
        BattleCount = 10;
        BattleList.Clear();
    }

    public Material flash_Monster;
    public Material flash_NPC;

    public SpriteLibraryAsset Battle_SLA;


    public int BattleCount { get; set; } = 10;
    public List<BattleField> BattleList = new List<BattleField>();


    public Transform[] FloorBase;

    public Coroutine ShowBattleField(NPC _npc, Monster _monster, out BattleField.BattleResult result)
    {
        int slotIndex = 0;
        Vector3 bfPos = SetPos_Field(_monster.PlacementInfo, out slotIndex);


        var bf = Managers.Resource.Instantiate("Battle/BattleField").GetComponent<BattleField>();
        bf.sort = BattleCount;
        bf.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = BattleCount;
        BattleCount += 2;
        bf.transform.position = bfPos;
        bf.floorIndex = _monster.PlacementInfo.Place_Floor.FloorIndex;
        bf.slotIndex = slotIndex;

        var line = Managers.Resource.Instantiate("Battle/Line").GetComponent<LineRenderer>();
        line.SetPosition(0, _monster.transform.position);
        line.SetPosition(1, bfPos);

        SetFieldColor(bf, line, _monster, _npc);

        bf.SetHPBar(_npc.B_HP, _npc.B_HP_Max, _monster.B_HP, _monster.B_HP_Max);


        result = bf.Battle(_npc, _monster, flash_NPC, flash_Monster);
        BattleList.Add(bf);
        return StartCoroutine(Battle(bf, line));
    }

    void SetFieldColor(BattleField field, LineRenderer line, Monster monster, NPC npc)
    {
        var floor = monster.PlacementInfo.Place_Floor.FloorIndex;
        //Debug.Log(floor + "@@");

        //? 배틀필드 배경이미지
        if (floor > 0)
        {
            field.sprite_BG.sprite = Battle_SLA.GetSprite("Field", $"{floor}");
        }
        else
        {
            field.sprite_BG.sprite = Battle_SLA.GetSprite("Field", "Entry");
        }


        var npcType = npc.GetType();

        if (npcType == typeof(NPC_Normal) && npc.TraitCheck(TraitGroup.Civilian))
        {
            field.sprite_border.sprite = Battle_SLA.GetSprite("Frame", "1");
            field.sprite_Icon.sprite = Battle_SLA.GetSprite("Icon", "1");

            Color color = new Color32(187, 227, 36, 255);
            line.startColor = color;
            line.endColor = color;
        }
        else if (npcType == typeof(NPC_Normal))
        {
            field.sprite_border.sprite = Battle_SLA.GetSprite("Frame", "2");
            field.sprite_Icon.sprite = Battle_SLA.GetSprite("Icon", "2");

            Color color = new Color32(35, 145, 255, 255);
            line.startColor = color;
            line.endColor = color;
        }
        else if (npcType == typeof(NPC_MainEvent))
        {
            field.sprite_border.sprite = Battle_SLA.GetSprite("Frame", "3");
            field.sprite_Icon.sprite = Battle_SLA.GetSprite("Icon", "3");

            Color color = new Color32(255, 54, 67, 255);
            line.startColor = color;
            line.endColor = color;
        }
        else if (npcType == typeof(NPC_SubEvent))
        {
            field.sprite_border.sprite = Battle_SLA.GetSprite("Frame", "6");
            field.sprite_Icon.sprite = Battle_SLA.GetSprite("Icon", "6");

            Color color = new Color32(246, 183, 72, 255);
            line.startColor = color;
            line.endColor = color;
        }
        else if (npcType == typeof(NPC_Hunter))
        {
            field.sprite_border.sprite = Battle_SLA.GetSprite("Frame", "5");
            field.sprite_Icon.sprite = Battle_SLA.GetSprite("Icon", "5");

            Color color = new Color32(246, 183, 72, 255);
            line.startColor = color;
            line.endColor = color;
        }
        else if (npcType == typeof(NPC_Unique) || npcType == typeof(NPC_RandomEvent))
        {
            field.sprite_border.sprite = Battle_SLA.GetSprite("Frame", "4");
            field.sprite_Icon.sprite = Battle_SLA.GetSprite("Icon", "4");

            Color color = new Color32(204, 106, 255, 255);
            line.startColor = color;
            line.endColor = color;
        }
    }



    IEnumerator Battle(BattleField _field, LineRenderer _line)
    {
        yield return _field.BattlePlay();
        RemoveCor(_field);
        Managers.Resource.Destroy(_field.gameObject);
        Managers.Resource.Destroy(_line.gameObject);
    }



    public void RemoveCor(BattleField _field)
    {
        AddPos_Field((Define.DungeonFloor)_field.floorIndex, _field.slotIndex);
        BattleList.Remove(_field);
    }

    Vector3 SetPos_Field(PlacementInfo info, out int slotNumber)
    {
        bool[] checkList = floor_egg;
        Vector3[] posList = Floor_Egg_Battle_Pos;

        switch ((Define.DungeonFloor)info.Place_Floor.FloorIndex)
        {
            case Define.DungeonFloor.Egg:
                checkList = floor_egg;
                posList = Floor_Egg_Battle_Pos;
                break;

            case Define.DungeonFloor.Floor_1:
                checkList = floor_1;
                posList = Floor_1_Battle_Pos;
                break;

            case Define.DungeonFloor.Floor_2:
                checkList = floor_2;
                posList = Floor_2_Battle_Pos;
                break;

            case Define.DungeonFloor.Floor_3:
                checkList = floor_3;
                posList = Floor_3_Battle_Pos;
                break;

            case Define.DungeonFloor.Floor_4:
                checkList = floor_4;
                posList = Floor_4_Battle_Pos;
                break;

            case Define.DungeonFloor.Floor_5:
                checkList = floor_5;
                posList = Floor_5_Battle_Pos;
                break;
            //case Define.DungeonFloor.Floor_6:
            //    break;
            //case Define.DungeonFloor.Floor_7:
            //    break;
        }

        int nearPick = GetNearField(info, checkList, posList);
        //int ranPick = GetRandomField(checkList);

        if (nearPick == -1) //? -1이면 모든 슬롯이 꽉찼으니 적당히 랜덤값
        {
            float valueX;
            float valueY;
            do
            {// -10 ~ 10 사이의 값 중 랜덤 추출
                valueX = Random.Range(-10, 11);
            } while (valueX > -5 && valueX < 5); // -4~4의 범위를 제외
            do
            {// -10 ~ 10 사이의 값 중 랜덤 추출
                valueY = Random.Range(-10, 11);
            } while (valueY > -5 && valueY < 5); // -4~4의 범위를 제외

            slotNumber = -1;
            return FloorBase[info.Place_Floor.FloorIndex].position + new Vector3(valueX, valueY, 0);
        }

        slotNumber = nearPick;
        checkList[nearPick] = true;
        return posList[nearPick] + FloorBase[info.Place_Floor.FloorIndex].position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
    }


    int GetNearField(PlacementInfo info, bool[] checklist, Vector3[] posList)
    {
        Vector3 originPos = info.Place_Tile.worldPosition;

        int index = -1;
        float distance = int.MaxValue;

        for (int i = 0; i < checklist.Length; i++)
        {
            if (checklist[i] == false)
            {
                float tempDis = Vector3.Distance(originPos, posList[i] + (info.Place_Floor.transform.position));
                if (tempDis < distance)
                {
                    index = i;
                    distance = tempDis;
                }
            }
        }

        return index;
    }

    int GetRandomField(bool[] target)
    {
        int ranValue = Random.Range(0, target.Length);
        int counter = 0;
        while (counter < 100)
        {
            if (!target[ranValue])
            {
                return ranValue;
            }
            counter++;
        }
        return -1;
    }


    void AddPos_Field(Define.DungeonFloor floorIndex, int slotIndex)
    {
        if (slotIndex == -1)
        {
            return;
        }

        switch (floorIndex)
        {
            case Define.DungeonFloor.Egg:
                floor_egg[slotIndex] = false;
                break;
            case Define.DungeonFloor.Floor_1:
                floor_1[slotIndex] = false;
                break;
            case Define.DungeonFloor.Floor_2:
                floor_2[slotIndex] = false;
                break;
            case Define.DungeonFloor.Floor_3:
                floor_3[slotIndex] = false;
                break;
            case Define.DungeonFloor.Floor_4:
                floor_4[slotIndex] = false;
                break;
            case Define.DungeonFloor.Floor_5:
                floor_5[slotIndex] = false;
                break;
            //case Define.DungeonFloor.Floor_6:
            //    break;
            //case Define.DungeonFloor.Floor_7:
            //    break;
        }
    }




    bool[] floor_egg; 
    bool[] floor_1 ;
    bool[] floor_2 ;
    bool[] floor_3 ;
    bool[] floor_4 ;
    bool[] floor_5;

    Vector3[] Floor_Egg_Battle_Pos;
    Vector3[] Floor_1_Battle_Pos;
    Vector3[] Floor_2_Battle_Pos;
    Vector3[] Floor_3_Battle_Pos;
    Vector3[] Floor_4_Battle_Pos;
    Vector3[] Floor_5_Battle_Pos;


    void Init_BattlePosList()
    {
        Floor_Egg_Battle_Pos = Init_PosList(0);
        Floor_1_Battle_Pos = Init_PosList(1);
        Floor_2_Battle_Pos = Init_PosList(2);
        Floor_3_Battle_Pos = Init_PosList(3);
        Floor_4_Battle_Pos = Init_PosList(4);
        Floor_5_Battle_Pos = Init_PosList(5);

        floor_egg = new bool[Floor_Egg_Battle_Pos.Length];
        floor_1 = new bool[Floor_1_Battle_Pos.Length];
        floor_2 = new bool[Floor_2_Battle_Pos.Length];
        floor_3 = new bool[Floor_3_Battle_Pos.Length];
        floor_4 = new bool[Floor_4_Battle_Pos.Length];
        floor_5 = new bool[Floor_5_Battle_Pos.Length];
    }


    Vector3[] Init_PosList(int floorIndex)
    {
        var list = FloorBase[floorIndex].GetComponent<BasementFloor>().battlePos;

        Vector3[] posArray = new Vector3[list.Count];
        for (int i = 0; i < posArray.Length; i++)
        {
            posArray[i] = list[i].localPosition;
        }
        return posArray;
    }



    //static readonly Vector3[] Floor_Egg_Battle_Pos = new Vector3[]
    //{
    //    new Vector3(6, 1, 0), new Vector3(0, 1, 0), new Vector3(-6, 1, 0), new Vector3(-12, 1, 0),
    //    new Vector3(-11, -3, 0), new Vector3(-11, -7, 0),
    //    new Vector3(6, -11, 0), new Vector3(0, -11, 0), new Vector3(-6, -11, 0), new Vector3(-12,-11 , 0)
    //};

    //static readonly Vector3[] Floor_1_Battle_Pos = new Vector3[]
    //{
    //    new Vector3(-4, 5, 0), new Vector3(-10, 5, 0), new Vector3(6, 5, 0), new Vector3(12, 5, 0),
    //    new Vector3(-13, 0.5f, 0), new Vector3(-13, -3.5f, 0), new Vector3(12, 0.5f, 0), new Vector3(14, -3.5f, 0),
    //    new Vector3(0, -7, 0), new Vector3(6, -7, 0), new Vector3(-6, -7, 0), new Vector3(12, -7, 0), new Vector3(-12, -7, 0),
    //};

    //static readonly Vector3[] Floor_2_Battle_Pos = new Vector3[]
    //{
    //    new Vector3(0, 5.5f, 0), new Vector3(6, 5.5f, 0), new Vector3(-6, 5.5f, 0),
    //    new Vector3(-9, 1.5f, 0), new Vector3(-9, -2.5f, 0),
    //    new Vector3(9.5f, -2.5f, 0), new Vector3(9.5f, 1.5f, 0),
    //    new Vector3(-3, -4.5f, 0), new Vector3(3, -4.5f, 0),
    //};

    //static readonly Vector3[] Floor_3_Battle_Pos = new Vector3[]
    //{
    //    new Vector3(3, 4f, 0), new Vector3(3, 8f, 0), new Vector3(-4, 7f, 0),
    //    new Vector3(-13, 2f, 0), new Vector3(-11.5f, 5.5f, 0), new Vector3(-19f, 4f, 0),
    //    new Vector3(4.5f, -4f, 0), new Vector3(12f, -4f, 0), new Vector3(14f, 0f, 0),
    //    new Vector3(16f, 9f, 0), new Vector3(12f, 13f, 0), new Vector3(18f, 12.5f, 0),
    //    new Vector3(-24.5f, 0, 0), new Vector3(-24.5f, -4, 0),
    //    new Vector3(-18, -7, 0), new Vector3(-11, -7, 0), new Vector3(-4, -7, 0), new Vector3(18.5f, 4.5f, 0),
    //};

    //static readonly Vector3[] Floor_4_Battle_Pos = new Vector3[]
    //{
    //    new Vector3(-3, 0, 0), new Vector3(4, 0, 0), new Vector3(4, 4, 0), new Vector3(-3, 4, 0), new Vector3(4, 8, 0), new Vector3(-3, 8, 0),
    //    new Vector3(-10, 8, 0), new Vector3(-20, 7, 0), new Vector3(-13.5f, -1, 0), new Vector3(-20, 0, 0), new Vector3(-12.5f, -8, 0),
    //    new Vector3(14, -5, 0), new Vector3(14.5f, -1, 0),
    //    new Vector3(23.5f, 0, 0), new Vector3(23.5f, 4, 0), new Vector3(23.5f, 8, 0), 
    //    new Vector3(16, 8, 0), new Vector3(10, 5, 0), new Vector3(9, 9, 0),
    //    new Vector3(-5, -11, 0), new Vector3(2, -11, 0), new Vector3(9, -11, 0), new Vector3(16, -11, 0), new Vector3(23, -11, 0),
    //};

    //static readonly Vector3[] Floor_5_Battle_Pos = new Vector3[]
    //{
    //    new Vector3(0, 11, 0), new Vector3(7, 11, 0), new Vector3(14, 11, 0), new Vector3(21, 11, 0),
    //    new Vector3(2.5f, -3, 0), new Vector3(-7, 11, 0), new Vector3(-14, 11, 0), new Vector3(-21, 11, 0),
    //    new Vector3(8, 0, 0), new Vector3(15, -1, 0), new Vector3(15, -5, 0),
    //    new Vector3(3, -10, 0), new Vector3(-9.5f, -4.5f, 0), 
    //    new Vector3(-18, 3.5f, 0), new Vector3(-10, 3.5f, 0), new Vector3(4, 4f, 0), new Vector3(12, 5, 0),
    //    new Vector3(-7, -9, 0),
    //    new Vector3(-21, -12, 0), new Vector3(-14, -12, 0), new Vector3(10, -12, 0), new Vector3(17, -12, 0),
    //    new Vector3(23.5f, 4.5f, 0), new Vector3(-27.5f, 3f, 0), new Vector3(-27.5f, -1f, 0), new Vector3(-27.5f, -5f, 0),
    //    new Vector3(-27.5f, -9f, 0),
    //};
}



