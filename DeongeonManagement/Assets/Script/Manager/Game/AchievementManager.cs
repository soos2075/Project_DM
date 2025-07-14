using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    #region Singleton
    private static AchievementManager _instance;
    public static AchievementManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<AchievementManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@Achievement" };
                _instance = go.AddComponent<AchievementManager>();
            }
            DontDestroyOnLoad(_instance);
        }
    }


    private void Awake()
    {
        Initialize();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion
    void Start()
    {
        Init();
    }



    void Init()
    {
        //? ���� API �� �����ϴ� �Լ�. ���Ῡ�θ� bool ������ �س��� false �� �� �Ʒ� �Լ��� �� ���Ͻ��ѹ����� �ɵ�?
    }


    //? ���̺� �� �� ȣ���ϸ� ���������� ����. �ƴϸ� �ð� ������Ʈ�Ҷ��� ������. �ƴ� �ð� ���⶧ Ȥ�� UI Ŭ���� �� ��� Ÿ�̹��� ����.
    //? �߿��Ѱ� �������� �ϰų� ���ɸ����� ��Ȳ���� ���ϸ� �ɵ�. �� UserData ������Ʈ �� �� ȣ���ص� �״� ����� ����.
    public void AchievementUpdate()
    {
        //? ����api�� ����ƴ���Ȯ�κ��� �ϰ� �ȵ����� ����(��������)

        //? ����üũ / �̹� �޼��� ��������(�̰� ����Ƿ���?)
        if (true)
        {
            //? ���� api���� ���� �޼� �ҷ�����
        }
    }

    //? ������ �����ÿ��� ����� �������� �ʴ� Record�� �ϳ� ������(��¥�� �ƿ� ��Ͽ�) �ƴϸ� �׳� UserData������ �޾Ƽ� ������ �����.
    //? UserData�� CollectionData, ClearData�� �ϴ°� �ϴ��� �´� �� ������. �� ���� �۾��Ұ͵� ����.. �ٵ� ����.. ������Ʈ ������ ���ؼ� ���� ���̺� ������
    //? ���� �� ������ �߻��� ���� �ְڴٸ�, �װ� �� �̸� ������ �ϵ簡 �ƴϸ� ���̺� ���Ը� ���� �� �ʱ�ȭ�ϴ°� �߰��ϸ� �ɰŰ��⵵ ��.
}



//? �������� �۾�. �׷��� ������ �ϴٺ��� 10�� 20�и��� �ڵ����� Ŭ����Ǵ� �������� �ȳִ°� ������.
//? ���� ��� ó������ ���谡�� ����Ʈ�ȴ� �����Ŷ� �׳� �����ϸ� �ڵ����� ������ �׷��͵�.

/* ���� ����Ʈ
 * 
 * ùȸ�� Ŭ���� -                        // 1
 * 
 * ������ Ŭ���� - ���� ���� * 1          //  7
 * 
 * ���̵��� Ŭ���� - ���̵� ���� * 1        // 4
 * 
 * ���� ���÷� - ���� / ���谡 / Ư�� / �ü� / Ư���ü� / ��Ƽ��Ʈ / ����   // 7
 * 
 * 
 * 
 * ���Ѹ�� - 60���� / 100����          // 2
 * 
 * �� 10�� �񺸸� ��� ������              // 1
 * 
 * 
 */

/* ���� ���̵�� (�ϴ� ����)
 * 
 * Ư�� ���� ���� 
 * ��ȭ ���� (��ȭ������ �ֵ� ��)
 * 
 * ù �α⵵ 500/1000/2000 �޼�                // 2
 * ù ���赵 500/1000/2000 �޼�                // 2
 * ù ��ũ �޼� D/C/B/A/S                    // 5
 * 
 * ���谡���� �¸� 100/500/1000            //3
 * ȹ���� ���� 5000/20000/50000          //3
 * ȹ���� ��� 5000/20000/50000          //3
 * ���� �Ʒ�Ƚ�� 10/50/100                //3
 * �⵵ Ƚ��    10/50/100               //3
 * 
 * 
 */