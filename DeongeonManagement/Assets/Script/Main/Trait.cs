using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait
{
    #region Normal Trait

    public class Nothing : ITrait_Value
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
    //public class ToughnessTrait : ITrait
    //{
    //    public TraitGroup ID { get; } = TraitGroup.ToughnessTrait;
    //    public int ApplyHP(int current)
    //    {
    //        return current *= 200;
    //    }
    //    public int ApplyHP_Max(int current)
    //    {
    //        return 0;
    //    }
    //    public int ApplyATK(int current)
    //    {
    //        return 0;
    //    }
    //    public int ApplyDEF(int current)
    //    {
    //        return 0;
    //    }
    //    public int ApplyAGI(int current)
    //    {
    //        return 0;
    //    }
    //    public int ApplyLUK(int current)
    //    {
    //        return 0;
    //    }

    //    public void DoSomething()
    //    {

    //    }
    //    public int GetSomething<T>(T current)
    //    {
    //        return 0;
    //    }

    //    public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
    //    {
    //        return current as T1;
    //    }
    //}

    public class EliteC : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.EliteC;
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
    public class EliteB : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.EliteB;
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
    public class EliteA : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.EliteA;
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

    public class VeteranC : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.VeteranC;
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
    public class VeteranB : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.VeteranB;
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
    public class VeteranA : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.VeteranA;
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

    public class ShirkingC : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.ShirkingC;
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
    public class ShirkingB : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.ShirkingB;
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
    public class ShirkingA : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.ShirkingA;
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

    public class SurvivabilityC : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.SurvivabilityC;
        public int ApplyHP(int current)
        {
            return 15;
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
    public class SurvivabilityB : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.SurvivabilityB;
        public int ApplyHP(int current)
        {
            return 30;
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
    public class SurvivabilityA : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.SurvivabilityA;
        public int ApplyHP(int current)
        {
            return 50;
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
    public class SurvivabilityS : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.SurvivabilityS;
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

    public class DiscreetC : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.DiscreetC;
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
    public class DiscreetB : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.DiscreetB;
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
    public class DiscreetA : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.DiscreetA;
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

    public class RuthlessC : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.RuthlessC;
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
            return 3;
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
    public class RuthlessB : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.RuthlessB;
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
            return 6;
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
    public class RuthlessA : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.RuthlessA;
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
            return 9;
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
    }
    public class Vitality : ITrait_Value
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
    public class Predation : ITrait_Value
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
    public class IronSkin : ITrait_Value
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
    public class Friend : ITrait_Value
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
    public class Overwhelm : ITrait_Value
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
    public class Nimble : ITrait_Value
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

    public class LifeDrain : ITrait_Value
    {
        public TraitGroup ID { get; } = TraitGroup.LifeDrain;
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
                int damage = (int)(object)current;
                int drain = Mathf.RoundToInt(damage * 0.25f);
                return drain;
            }
            return 0;
        }

        public T1 GetSomething<T1, T2>(T2 current) where T1 : UnityEngine.Object where T2 : UnityEngine.Object
        {
            return current as T1;
        }
    }

    #endregion



    #region NPCTrait_NonChangeStat
    public class Herbalism : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Herbalism;
    }
    public class Mineralogy : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Mineralogy;
    }
    public class Collector : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Collector;
    }


    public class Weed : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Weed;
    }
    public class Stone : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Stone;
    }

    public class Militant : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Militant;
    }
    public class Civilian : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Civilian;
    }
    public class Trample : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Trample;
    }

    public class Swiftness : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Swiftness;
    }


    #endregion



    #region Hunting:Target
    public class Hunting_Slime : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Hunting_Slime;
    }

    public class Hunting_Golem : ITrait
    {
        public TraitGroup ID { get; } = TraitGroup.Hunting_Golem;
    }

    #endregion
}

public interface ITrait
{
    TraitGroup ID { get; }
}

public interface ITrait_Value : ITrait
{
    //TraitGroup ID { get; }
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

public enum TraitRating
{
    Normal = 0,
    Original = 1,
    Battle = 2,
    Facility_Value = 3,
}

public enum TraitGroup
{
    //? 아무효과없음
    Nothing = 0,

    //? ㅡㅡㅡㅡㅡㅡㅡ 노말특성 - 나중에 생기거나 할 수 있음

    //? 강인함 : HP 보너스
    //ToughnessTrait = 101,


    //? 베테랑 Veteran  : 전투횟수 10/15/20 이상 : 올스탯 +1,2,3
    VeteranC = 201,
    VeteranB = 202,
    VeteranA = 203,


    //? 엘리트 Elite  : 훈련횟수 5/10/15번 이상 : 레벨업시 스탯 상승률 up
    EliteC = 301,
    EliteB = 302,
    EliteA = 303,


    //? 꾀병, Shirking,  : 부상횟수 3/4/5회 이상 : 부상회복마나 감소 : 10 20 30
    ShirkingC = 401,
    ShirkingB = 402,
    ShirkingA = 403,


    //? 생존력, Survivability,  : 배치 후 부상당하지 않고 4/7/10/15일이상 경과 : 체력 보너스 : 15, 30, 50, 부상무시(피0되면 퇴각 후 피1)
    SurvivabilityC = 501,
    SurvivabilityB = 502,
    SurvivabilityA = 503,
    SurvivabilityS = 504,


    //? 신중함 Discreet  : 배치하지 않고 3/6/9일이상 경과 : 전투시 획득 경험치 +20, 40, 60
    DiscreetC = 601,
    DiscreetB = 602,
    DiscreetA = 603,


    //? 무자비 Ruthless  : 적 처치수 10/15/20 이상 : ATK +3,6,9
    RuthlessC = 701,
    RuthlessB = 702,
    RuthlessA = 703,

    




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

    LifeDrain = 20,




    //? -----------------NPC 특성
    //? Bonus
    Herbalism = 7000,
    Mineralogy,
    Collector,

    //? Weak
    Weed = 8000,
    Stone,


    //? Special(Category)
    Militant = 9000,
    Civilian,
    Trample,
    Swiftness,


    //? Hunting
    Hunting_Slime = 9900,
    Hunting_Golem,
}
