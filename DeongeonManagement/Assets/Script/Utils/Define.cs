using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{

    public enum Language
    {
        EN = 0,
        KR = 1,
        JP = 2,
        SCC = 3,
    }

    public enum GameMode
    {
        Normal,
        Stop,
        //X2,
        //X3,
    }





    public enum AudioType
    {
        Effect,
        BGM,
    }


    public enum MouseEvent
    {
        None = 0,
        Click = 1,
        Press = 2,
    }

    public enum UIEvent
    {
        LeftClick,
        RightClick,
        Drag,
        BeginDrag,
        EndDrag,
        Move,
        Enter,
        Exit,
        Down,
        Up,
    }




    public enum DungeonRank
    {
        F = 1,
        D = 2,
        C = 3,
        B = 4,
        A = 5,
        S = 6,
        SS = 7,
        SSS = 8,

        X = 9,
        XX = 10,
        XXX = 11,

        Y = 12,
        YY = 13,
        YYY = 14,

        Z = 15,
        ZZ = 16,
        ZZZ = 17,

        L = 99,
    }

    public enum DungeonFloor
    {
        Egg = 0,
        Floor_1 = 1,
        Floor_2 = 2,
        Floor_3 = 3,
        Floor_4 = 4,
        Floor_5 = 5,
        Floor_6 = 6,
        Floor_7 = 7,
    }





    public enum TileType
    {
        Empty,

        Monster,

        NPC,

        Facility,

        Player_Interaction,

        Non_Interaction,

        //Monster_Standby,
        //Monster_Battle,

        //NPC_Stanby,
        //NPC_Busy,

        //Interaction_Stanby,
        //Interaction_Using,

        //Event_Stanby,
        //Event_Using,

        //Facility,
        //Entrance,
        //Exit,

        //Trap,
        //Using,

        //Special, //? 절대 삭제되지 않아야하는거
    }

    public enum PlaceEvent
    {
        Nothing, 

        Placement, // 이동 / 혼자

        Battle,

        Interaction,    // npc - 이동없이 상호작용
        Event,          // npc - 이동 후 상호작용

        Avoid,          // 피하기 / 상태리셋

        Entrance,
        Exit,

        //Overlap, // 이동 / 겹침
        //Using, // 이동 후 상호작용

        //Using_Portal, // 이동 후 상호작용 후 순간이동

    }

    public enum TextColor
    {
        Bold,
        Italic,

        red,
        green,
        blue,
        yellow,
        white,
        black,

        npc_red,
        monster_green,

        SkyBlue,
        LightGreen,
        LightYellow,

        Plus_Green,
        Plus_Blue,
        Plus_Red,
    }



    #region Animation
    public static readonly int ANIM_Attack = Animator.StringToHash("Attack");
    public static readonly int ANIM_Shot = Animator.StringToHash("Shot");
    public static readonly int ANIM_Jab = Animator.StringToHash("Jab");


    public static readonly int ANIM_Idle = Animator.StringToHash("Idle");
    public static readonly int ANIM_Ready = Animator.StringToHash("Ready");
    public static readonly int ANIM_Running = Animator.StringToHash("Running");


    public static readonly int ANIM_Dead = Animator.StringToHash("Dead");

    public static readonly int ANIM_Interaction = Animator.StringToHash("Interaction");
    public static readonly int ANIM_Trap = Animator.StringToHash("Trap");


    public static readonly int ANIM_Idle_NoWeapon = Animator.StringToHash("Idle_NoWeapon");
    public static readonly int ANIM_Idle_Sit = Animator.StringToHash("Idle_Sit");

    #endregion



    #region Image_Color
    public static readonly Color Color_Green = new Color32(100, 255, 100, 175);
    public static readonly Color Color_Blue = new Color32(100, 100, 255, 175);
    public static readonly Color Color_Red = new Color32(255, 100, 100, 175);
    public static readonly Color Color_White = new Color32(255, 255, 255, 175);

    public static readonly Color Color_Yellow = new Color32(255, 255, 100, 175);
    public static readonly Color Color_Dark = new Color32(50, 50, 50, 175);

    public static readonly Color Color_Alpha_1 = new Color32(255, 255, 255, 235);
    public static readonly Color Color_Alpha_2 = new Color32(255, 255, 255, 215);
    public static readonly Color Color_Alpha_3 = new Color32(255, 255, 255, 195);
    public static readonly Color Color_Alpha_4 = new Color32(255, 255, 255, 175);
    public static readonly Color Color_Alpha_5 = new Color32(255, 255, 255, 155);
    public static readonly Color Color_Alpha_6 = new Color32(255, 255, 255, 100);
    public static readonly Color Color_Alpha_7 = new Color32(255, 255, 255, 50);
    public static readonly Color Color_Alpha_8 = new Color32(255, 255, 255, 25);

    public static readonly Color Color_Gamma_1 = new Color32(235, 235, 235, 255);
    public static readonly Color Color_Gamma_2 = new Color32(215, 215, 215, 255);
    public static readonly Color Color_Gamma_3 = new Color32(190, 190, 190, 255);
    public static readonly Color Color_Gamma_4 = new Color32(170, 170, 170, 255);
    public static readonly Color Color_Gamma_5 = new Color32(150, 150, 150, 255);
    #endregion Image_Color


    #region Boundary

    public enum Boundary //? Util의 GetBoundary에도 추가해줘야함
    {
        //테스트 = 100,

        Boundary_1x1 = 101,
        Boundary_1x2 = 102,
        Boundary_1x3 = 103,

        Boundary_2x1 = 201,
        Boundary_2x2 = 202,
        Boundary_2x3 = 203,

        Boundary_3x1 = 301,
        Boundary_3x2 = 302,
        Boundary_3x3 = 303,
        Boundary_4x4 = 404,
        Boundary_5x5 = 505,

        Boundary_Cross_1 = 1001,
        Boundary_Cross_2 = 1002,
        Boundary_Cross_3 = 1003,
        Boundary_Cross_4 = 1004,

        Boundary_X_1 = 2001,
        Boundary_X_Big = 2002,

        Boundary_Diagonal__Small = 2101,
        Boundary_Diagonal__Big = 2102,

        Boundary_Diagonal_Reverse_Small = 2201,
        Boundary_Diagonal_Reverse_Big = 2202,


        Boundary_V_1 = 2501,

        Boundary_Side_All = 3001,
        Boundary_Side_X = 3002,

        Boundary_Side_Cross = 3003,
        Boundary_Side_Cross_2 = 3004,


        Boundary_Empty_rhombus = 4001,
    }



    //? 짝수의 경우 0,0기준 오른쪽/위쪽 우선함 
    public static readonly Vector2Int[] Boundary_1x1 = { new Vector2Int(0, 0) };

    public static readonly Vector2Int[] Boundary_1x2 = { new Vector2Int(0, 0), new Vector2Int(0, 1) };

    public static readonly Vector2Int[] Boundary_1x3 = { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };


    public static readonly Vector2Int[] Boundary_2x1 = { new Vector2Int(0, 0), new Vector2Int(1, 0) };

    public static readonly Vector2Int[] Boundary_2x2 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1)};

    public static readonly Vector2Int[] Boundary_2x3 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(0, -1), new Vector2Int(1, -1)};



    public static readonly Vector2Int[] Boundary_3x1 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 0)};

    public static readonly Vector2Int[] Boundary_3x2 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1)};

    public static readonly Vector2Int[] Boundary_3x3 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1),
        new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_4x4 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(2, -1),
        new Vector2Int(-1, 2), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2),};

    public static readonly Vector2Int[] Boundary_5x5 = {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-2, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(2, 1), new Vector2Int(-2, 1),
        new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(2, -1), new Vector2Int(-2, -1),
        new Vector2Int(0, -2), new Vector2Int(1, -2), new Vector2Int(-1, -2), new Vector2Int(2, -2), new Vector2Int(-2, -2),
        new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(-1, 2), new Vector2Int(2, 2), new Vector2Int(-2, 2)    };







    public static readonly Vector2Int[] Boundary_Cross_1 = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)};

    public static readonly Vector2Int[] Boundary_Cross_2 = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-2, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(0, 2), new Vector2Int(0, -2)};

    public static readonly Vector2Int[] Boundary_Cross_3 = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-2, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(0, 2), new Vector2Int(0, -2),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_Cross_4 = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-2, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(0, 2), new Vector2Int(0, -2)};




    public static readonly Vector2Int[] Boundary_X_1 = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_X_Big = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1),
        new Vector2Int(2, 2), new Vector2Int(2, -2), new Vector2Int(-2, 2), new Vector2Int(-2, -2)};


    public static readonly Vector2Int[] Boundary_Diagonal__Small = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_Diagonal__Big = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1), new Vector2Int(-1, -1),
        new Vector2Int(2, 2), new Vector2Int(-2, -2)};

    public static readonly Vector2Int[] Boundary_Diagonal_Reverse_Small = {
        new Vector2Int(0, 0),
        new Vector2Int(1, -1), new Vector2Int(-1, 1)};

    public static readonly Vector2Int[] Boundary_Diagonal_Reverse_Big = {
        new Vector2Int(0, 0),
        new Vector2Int(1, -1), new Vector2Int(-1, 1),
        new Vector2Int(2, -2), new Vector2Int(-2, 2)};


    public static readonly Vector2Int[] Boundary_V_1 = {
        new Vector2Int(0, -1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0), 
        new Vector2Int(2, 1), new Vector2Int(-2, 1)};



    public static readonly Vector2Int[] Boundary_Side_All = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_Side_X = {
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_Side_Cross = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)};

    public static readonly Vector2Int[] Boundary_Side_Cross_2 = {
        new Vector2Int(2, 0), new Vector2Int(-2, 0), new Vector2Int(0, 2), new Vector2Int(0, -2) };



    public static readonly Vector2Int[] Boundary_Empty_rhombus = {
        new Vector2Int(2, 0), new Vector2Int(-2, 0), new Vector2Int(0, 2), new Vector2Int(0, -2),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    #endregion Boundary



    #region ColorPalette
    public static readonly string[] HairColors = new string[] {
        "#3D3D3D","#5D5D5D","#858585","#C7CFDD",
        "#5D2C28","#8A4836","#BF6F4A","#E69C69",
        "#F6CA9F","#C64524","#E07438","#FFA214",
        "#891E2B","#C42430","#622461","#93388F",
        "#F389F5","#0098DC","#00CDF9","#657392",
        "#134C4C","#1E6F50","#33984B","#5AC54F",};

    #endregion



    public static readonly string AtoZ = "ABCDEFGHIJKLNMOPQRSTUVWXYZ";

}


class PriorityQueue<T> where T : IComparable<T>
{
    //? 우선순위 큐 구현 : 우선순위 큐란 부모가 항상 자식보다 큰 이진트리를 말함(힙 트리)
    //? 두번째 규칙은 항상 모든 레벨에 노드가 꽉차있어야하고(마지막층 제외), 채울 땐 항상 왼쪽부터

    //? i 노드의 왼쪽 자식은 (2 * i) + 1
    //? i 노드의 오른쪽 자식은 (2 * i) + 2
    //? i 노드의 부모는 (i - 1) / 2 (소수점을 버려서 +2는 상관없음)

    List<T> _list = new List<T>();
    public void Push(T data)
    {
        _list.Add(data);

        int now = _list.Count - 1;

        while (now > 0)
        {
            int parent = (now - 1) / 2;

            if (_list[now].CompareTo(_list[parent]) > 0)
            {
                T temp = _list[parent];
                _list[parent] = _list[now];
                _list[now] = temp;

                now = parent;
            }
            else
                break;
        }
    }

    public T Pop()
    {
        T pop = _list[0];

        int count = _list.Count - 1;
        _list[0] = _list[count];
        _list.RemoveAt(count);
        count--;

        int start = 0;
        while (true)
        {
            int left = 2 * start + 1;  //? 자식 트리 찾기 - 왼쪽
            int right = 2 * start + 2; //? 자식 트리 찾기 - 오른쪽


            int next = start;                //? 값이 바뀌면 저장할 변수

            if (left <= count && _list[next].CompareTo(_list[left]) == -1)      //? 왼쪽이랑 비교 = 높은값 저장 , 자식이 있는지 확인(count보다 작으면 최하위)
            {                   //! *** 여기서 조건 두개중에 앞에부분이 거짓이면 뒷부분을 아예 확인을 안해서 인덱스 에러가 안나는데
                next = left;    //! 조건 순서를 반대로하면 앞의 조건부터 읽기때문에 자식이 없는 상황이면 인덱스에러가뜸
            }
            if (right <= count && _list[next].CompareTo(_list[right]) == -1)     //? 오른쪽이랑 비교 = 기존값과 왼쪽값 비교에서 높은값이랑 비교하기때문에 왼,오,기존 중에 높은값
            {
                next = right;
            }

            if (start == next)  //? 왼쪽 오른쪽 둘다 비교해봤는데 숫자가 그대로라면 바로 탈출
            {
                break;
            }

            T temp = _list[start];
            _list[start] = _list[next];
            _list[next] = temp;

            start = next;
        }

        return pop;
    }


    public int Count
    {
        get { return _list.Count; }
    }
}

struct PQNode : IComparable<PQNode>
{
    public float F;
    public int posX;
    public int posY;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
        {
            return 0;
        }
        return F < other.F ? 1 : -1;  //이부분이 PriorityQueue에서 F가 높은순으로 트리를 만들지 낮은순으로 만들지 결정함
    }
}
