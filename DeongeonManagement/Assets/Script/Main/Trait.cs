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
    //? �ƹ�ȿ������
    Nothing = 0,



    //? �ѤѤѤѤѤѤ� �븻Ư�� - ���߿� ����ų� �� �� ����

    //? ������ : HP ���ʽ�
    ToughnessTrait = 101,


    //? �������ʽ� : ����Ƚ���� ���� ���Ⱥ��ʽ� / �ش� Ư���� �ϳ��� ���� �� ����
    //? ������ : ����Ƚ�� 20���̻� : ��� ���� +1
    //? ���׶� : ����Ƚ�� 50���̻� : ��� ���� +2
    //? ������ : ����Ƚ�� 100���̻� : ��� ���� +3





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



}
