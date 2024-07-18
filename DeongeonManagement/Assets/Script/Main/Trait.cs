using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait
{
    #region Normal Trait

    public class Nothing : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Nothing;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }


    }
    public class ToughnessTrait : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.ToughnessTrait;
        public int ApplyHP(int current)
        {
            return current *= 200;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }



    #endregion


    #region Original Trait
    public class Reconfigure : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Reconfigure;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }
    public class Vitality : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Vitality;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            if (current is int)
            {
                int hp = (int)(object)current;

                int recorverHP = Mathf.RoundToInt(hp * 0.1f);

                return recorverHP;
            }
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }
    public class Predation : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Predation;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }
    public class IronSkin : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.IronSkin;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        //? 이게 부르는곳에서 IronSkin이라는게 확정이 안되서 이 메세지가 안뜸...
        /// <summary>
        /// 방어력을 넣으면 0.25를 곱하고 반올림한 값을 돌려줌
        /// </summary>
        /// <typeparam name="T">T는 int형식입니다</typeparam>
        /// <param name="current">현재 방어력을 넣으세요</param>
        /// <returns>Mathf.RoundToInt(DEF * 0.25f)</returns>
        public int GetSomething<T>(T current)
        {
            if (current is int)
            {
                int def = (int)(object)current;

                int trueDamage = Mathf.RoundToInt(def * 0.25f);

                return trueDamage;
            }

            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }
    public class Friend : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Friend;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }
    public class Overwhelm : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Overwhelm;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }
    public class Nimble : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Nimble;
        public int ApplyHP(int current)
        {
            return 0;
        }
        public int ApplyHP_Max(int current)
        {
            return 0;
        }
        public int ApplyATK(int current)
        {
            return 0;
        }
        public int ApplyDEF(int current)
        {
            return 0;
        }
        public int ApplyAGI(int current)
        {
            return 0;
        }
        public int ApplyLUK(int current)
        {
            return 0;
        }

        public void DoSomething()
        {

        }
        public int GetSomething<T>(T current)
        {
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }

    #endregion
}

public interface ITrait
{
    TraitGroup ID { get; }
    int ApplyHP(int current);
    int ApplyHP_Max(int current);
    int ApplyATK(int current);
    int ApplyDEF(int current);
    int ApplyAGI(int current);
    int ApplyLUK(int current);

    void DoSomething();

    int GetSomething<T>(T current);

    public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object;
}

public enum TraitGroup
{
    //? 아무효과없음
    Nothing = 0,



    //? ㅡㅡㅡㅡㅡㅡㅡ 노말특성 - 나중에 생기거나 할 수 있음

    //? 강인함 : HP 보너스
    ToughnessTrait = 101,


    //? 전투보너스 : 전투횟수에 따라 스탯보너스 / 해당 특성은 하나만 가질 수 있음
    //? 숙련자 : 전투횟수 20번이상 : 모든 스탯 +1
    //? 베테랑 : 전투횟수 50번이상 : 모든 스탯 +2
    //? 전투광 : 전투횟수 100번이상 : 모든 스탯 +3





    //? ㅡㅡㅡㅡㅡㅡㅡ 고유특성 - 몬스터가 기본으로 가지고있음

    //? 재구성 : 부상 후 하루가 지나면 부상에서 회복
    Reconfigure = 1,

    //? 활력 : 전투가 시작될 때 마다 최대체력의 10% 회복
    Vitality = 2,

    //? 포식 : 적을 쓰러트릴 때 마다 영구적으로 HP +1
    Predation = 3,

    //? 철피부 : 방어력의 25%만큼 방어무시데미지
    IronSkin = 4,

    //? 친구 : 층에 있는 다른 유닛마다 모든 스탯 +1
    Friend = 5,

    //? 압도 : 전투 시작 시 상대의 현재체력 -20%
    Overwhelm = 6,

    //? 날렵함 : 공격 시 절반만큼의 추가공격
    Nimble = 7,



}
