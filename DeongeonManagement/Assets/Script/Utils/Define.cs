using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{

    public enum Language
    {
        Eng = 0,
        Kor = 1,

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
        Move,
        Enter,
        Exit,
    }


    public enum TileType
    {
        Empty,

        Monster,
        NPC,
        Facility,
        Entrance,
        Exit,

        //Trap,
        Using,
    }

    public enum PlacementType
    {
        Facility,
        Monster,
        NPC,
    }

    public enum PlaceEvent
    {
        Nothing, 

        Placement, // 이동 / 혼자
        Battle,
        Interaction, // 이동없이 상호작용
        Entrance,
        Exit,
        Avoid, // 상태 리셋
        Overlap, // 이동 / 겹침
        Using, // 이동 후 상호작용

        Using_Portal, // 이동 후 상호작용 후 순간이동
        Event, // 이동 후 상호작용 후 State = Return
    }

    public enum TextColor
    {
        red,
        green,
        blue,
        yellow,
        white,
        black,

        npc,
        monster,

        SkyBlue,
        LightGreen,
        LightYellow,
    }



    #region Animation
    public static readonly int ANIM_attack = Animator.StringToHash("attack");
    public static readonly int ANIM_idle = Animator.StringToHash("idle");
    public static readonly int ANIM_dead = Animator.StringToHash("dead");

    #endregion



    #region Image_Color
    public static readonly Color Color_Green = new Color32(100, 255, 100, 175);
    public static readonly Color Color_Blue = new Color32(100, 100, 255, 175);
    public static readonly Color Color_Red = new Color32(255, 100, 100, 175);
    public static readonly Color Color_White = new Color32(255, 255, 255, 175);
    public static readonly Color Color_Gray = new Color32(155, 155, 155, 255);

    public static readonly Color Color_Yellow = new Color32(255, 255, 100, 175);
    public static readonly Color Color_Dark = new Color32(50, 50, 50, 175);
    #endregion Image_Color


    #region Boundary
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



    public static readonly Vector2Int[] Boundary_X_1 = {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};



    public static readonly Vector2Int[] Boundary_Side_All = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_Side_X = {
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    public static readonly Vector2Int[] Boundary_Side_Cross = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)};
    #endregion Boundary



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
