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
        //? �̰� �θ��°����� IronSkin�̶�°� Ȯ���� �ȵǼ� �� �޼����� �ȶ�...
        /// <summary>
        /// ������ ������ 0.25�� ���ϰ� �ݿø��� ���� ������
        /// </summary>
        /// <typeparam name="T">T�� int�����Դϴ�</typeparam>
        /// <param name="current">���� ������ ��������</param>
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
    //? �ƹ�ȿ������
    Nothing = 0,

    //? �ѤѤѤѤѤѤ� �븻Ư�� - ���߿� ����ų� �� �� ����

    //? ������ : HP ���ʽ�
    //ToughnessTrait = 101,


    //? ���׶� Veteran  : ����Ƚ�� 10/15/20 �̻� : �ý��� +1,2,3
    VeteranC = 201,
    VeteranB = 202,
    VeteranA = 203,


    //? ����Ʈ Elite  : �Ʒ�Ƚ�� 5/10/15�� �̻� : �������� ���� ��·� up
    EliteC = 301,
    EliteB = 302,
    EliteA = 303,


    //? �Һ�, Shirking,  : �λ�Ƚ�� 3/4/5ȸ �̻� : �λ�ȸ������ ���� : 10 20 30
    ShirkingC = 401,
    ShirkingB = 402,
    ShirkingA = 403,


    //? ������, Survivability,  : ��ġ �� �λ������ �ʰ� 4/7/10/15���̻� ��� : ü�� ���ʽ� : 15, 30, 50, �λ󹫽�(��0�Ǹ� �� �� ��1)
    SurvivabilityC = 501,
    SurvivabilityB = 502,
    SurvivabilityA = 503,
    SurvivabilityS = 504,


    //? ������ Discreet  : ��ġ���� �ʰ� 3/6/9���̻� ��� : ������ ȹ�� ����ġ +20, 40, 60
    DiscreetC = 601,
    DiscreetB = 602,
    DiscreetA = 603,


    //? ���ں� Ruthless  : �� óġ�� 10/15/20 �̻� : ATK +3,6,9
    RuthlessC = 701,
    RuthlessB = 702,
    RuthlessA = 703,

    




    //? �ѤѤѤѤѤѤ� ����Ư�� - ���Ͱ� �⺻���� ����������

    //? �籸�� : �λ� �� �Ϸ簡 ������ �λ󿡼� ȸ��
    Reconfigure = 1,

    //? Ȱ�� : ������ ���۵� �� ���� �ִ�ü���� 10% ȸ��
    Vitality = 2,

    //? ���� : ���� ����Ʈ�� �� ���� ���������� HP +1
    Predation = 3,

    //? ö�Ǻ� : ������ 25%��ŭ ���õ�����
    IronSkin = 4,

    //? ģ�� : ���� �ִ� �ٸ� ���ָ��� ��� ���� +1
    Friend = 5,

    //? �е� : ���� ���� �� ����� ����ü�� -20%
    Overwhelm = 6,

    //? ������ : ���� �� ���ݸ�ŭ�� �߰�����
    Nimble = 7,

    LifeDrain = 20,




    //? -----------------NPC Ư��
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
