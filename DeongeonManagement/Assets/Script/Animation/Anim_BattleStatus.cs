using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_BattleStatus : MonoBehaviour
{
    void Start()
    {

    }

    void LateUpdate()
    {
        //? X 반전 취소
        transform.localScale = transform.parent.localScale;
    }

    //? HP ATK DEF AGI LUK 순서대로 위치할 포지션
    Vector2[] Preset = new Vector2[] { new Vector2(-0.5f, -1), new Vector2(0, -1), new Vector2(0.5f, -1),
     new Vector2(0, -1.5f), new Vector2(0.5f, -1.5f)};

    List<GameObject> ActiveEffectList = new List<GameObject>();


    public void Update_BattleStatus(BattleStatusLabel _label, Dictionary<BattleStatusLabel, int> _data)
    {
        //? 반대되는 버프/디버프로 상쇄가 될 때
        if (_label != ReverseStatus(_label))
        {
            int originValue = _data[_label];
            int reverseValue = _data[ReverseStatus(_label)];

            if (originValue - reverseValue == 0)
            {
                //? 둘다 지워
                Remove_BS(_label);
                Remove_BS(ReverseStatus(_label));
            }
            else if (originValue - reverseValue > 0)
            {
                //? 정버프
                Remove_BS(ReverseStatus(_label));
                Add_BS(_label);
            }
            else if (originValue - reverseValue < 0)
            {
                //? 역버프
                Remove_BS(_label);
                Add_BS(ReverseStatus(_label));
            }
        }
        else
        {
            //? 스폐셜 타입은 상쇄가 없으니까 그냥 +면 Add
            int value = _data[_label];
            if (value > 0)
            {
                Add_BS(_label);
            }
            else
            {
                Remove_BS(_label);
            }
        }

        Arrange_List();
    }

    void Add_BS(BattleStatusLabel _label)
    {
        foreach (var item in ActiveEffectList)
        {
            if (item.name == $"{_label}")
            {
                return;
            }
        }
        var newAnim = Managers.Resource.Instantiate($"Effect/BattleStatus/{_label}", transform);
        ActiveEffectList.Add(newAnim);
    }

    void Remove_BS(BattleStatusLabel _label)
    {
        foreach (var item in ActiveEffectList)
        {
            if (item.name == $"{_label}")
            {
                ActiveEffectList.Remove(item);
                Managers.Resource.Destroy(item);
                break;
            }
        }
    }

    void Arrange_List()
    {
        int count = 0;

        foreach (var item in ActiveEffectList)
        {
            if (item.name == $"{BattleStatusLabel.Robust}" || item.name == $"{BattleStatusLabel.Wound}" ||
                item.name == $"{BattleStatusLabel.Sharp}" || item.name == $"{BattleStatusLabel.Wither}" ||
                item.name == $"{BattleStatusLabel.Guard}" || item.name == $"{BattleStatusLabel.Corrode}" ||
                item.name == $"{BattleStatusLabel.Haste}" || item.name == $"{BattleStatusLabel.Slow}" ||
                item.name == $"{BattleStatusLabel.Chance}" || item.name == $"{BattleStatusLabel.Jinx}")
            {
                count++;
            }
        }

        if (count <= 3)
        {
            foreach (var item in ActiveEffectList)
            {
                if (item.name == $"{BattleStatusLabel.Chance}" || item.name == $"{BattleStatusLabel.Jinx}")
                {
                    count -= 1;
                    item.transform.localPosition = Preset[count];
                }
            }
            foreach (var item in ActiveEffectList)
            {
                if (item.name == $"{BattleStatusLabel.Haste}" || item.name == $"{BattleStatusLabel.Slow}")
                {
                    count -= 1;
                    item.transform.localPosition = Preset[count];
                }
            }
            foreach (var item in ActiveEffectList)
            {
                if (item.name == $"{BattleStatusLabel.Guard}" || item.name == $"{BattleStatusLabel.Corrode}")
                {
                    count -= 1;
                    item.transform.localPosition = Preset[count];
                }
            }
            foreach (var item in ActiveEffectList)
            {
                if (item.name == $"{BattleStatusLabel.Sharp}" || item.name == $"{BattleStatusLabel.Wither}")
                {
                    count -= 1;
                    item.transform.localPosition = Preset[count];
                }
            }
            foreach (var item in ActiveEffectList)
            {
                if (item.name == $"{BattleStatusLabel.Robust}" || item.name == $"{BattleStatusLabel.Wound}")
                {
                    count -= 1;
                    item.transform.localPosition = Preset[count];
                }
            }
        }
        else
        {
            foreach (var item in ActiveEffectList)
            {
                if (item.name == $"{BattleStatusLabel.Robust}" || item.name == $"{BattleStatusLabel.Wound}")
                {
                    item.transform.localPosition = Preset[0];
                }
                if (item.name == $"{BattleStatusLabel.Sharp}" || item.name == $"{BattleStatusLabel.Wither}")
                {
                    item.transform.localPosition = Preset[1];
                }
                if (item.name == $"{BattleStatusLabel.Guard}" || item.name == $"{BattleStatusLabel.Corrode}")
                {
                    item.transform.localPosition = Preset[2];
                }
                if (item.name == $"{BattleStatusLabel.Haste}" || item.name == $"{BattleStatusLabel.Slow}")
                {
                    item.transform.localPosition = Preset[3];
                }
                if (item.name == $"{BattleStatusLabel.Chance}" || item.name == $"{BattleStatusLabel.Jinx}")
                {
                    item.transform.localPosition = Preset[4];
                }
            }
        }

    }

    public void Anim_Reset()
    {
        foreach (var item in ActiveEffectList)
        {
            Managers.Resource.Destroy(item.gameObject);
        }
        ActiveEffectList.Clear();
    }

    BattleStatusLabel ReverseStatus(BattleStatusLabel _label)
    {
        switch (_label)
        {
            case BattleStatusLabel.Empower:
                return BattleStatusLabel.Weaken;

            case BattleStatusLabel.Vigor:
                return BattleStatusLabel.Fatigue;

            case BattleStatusLabel.Blessing:
                return BattleStatusLabel.Decay;

            case BattleStatusLabel.Sharp:
                return BattleStatusLabel.Wither;

            case BattleStatusLabel.Guard:
                return BattleStatusLabel.Corrode;

            case BattleStatusLabel.Haste:
                return BattleStatusLabel.Slow;

            case BattleStatusLabel.Chance:
                return BattleStatusLabel.Jinx;

            case BattleStatusLabel.Robust:
                return BattleStatusLabel.Wound;

            case BattleStatusLabel.Weaken:
                return BattleStatusLabel.Empower;

            case BattleStatusLabel.Fatigue:
                return BattleStatusLabel.Vigor;

            case BattleStatusLabel.Decay:
                return BattleStatusLabel.Blessing;

            case BattleStatusLabel.Wither:
                return BattleStatusLabel.Sharp;

            case BattleStatusLabel.Corrode:
                return BattleStatusLabel.Guard;

            case BattleStatusLabel.Slow:
                return BattleStatusLabel.Haste;

            case BattleStatusLabel.Jinx:
                return BattleStatusLabel.Chance;

            case BattleStatusLabel.Wound:
                return BattleStatusLabel.Robust;


            default:
                return _label;
        }
    }

}
