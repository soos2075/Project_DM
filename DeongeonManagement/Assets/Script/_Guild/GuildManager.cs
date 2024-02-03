using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : MonoBehaviour
{
    #region Singleton
    private static GuildManager _instance;
    public static GuildManager Instance { get { Init(); return _instance; } }

    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GuildManager>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@GuildManager");
                _instance = go.AddComponent<GuildManager>();
                DontDestroyOnLoad(go);
            }
        }
    }
    #endregion

    private void Awake()
    {
        Init();
    }


    void Start()
    {
        myInt++;
        Debug.Log(myInt + myName);
    }

    public int myInt = 3;

    public string myName = "����";


    //? ��� ���� �̱����̾����. �����͸� GameManager���� �����������
    //? DontDestroy�� �ʿ䰡 �������� �ָ���. ���ư� �� SceneLoaded�� ü���� �ɾ������ �����͸� �������ְ� �˾Ƽ� �μ����������� �ʹ�.
    //? chatgpt���� ����ϱ� DontDestroy����� ��� ���µ�. �װ� ����� Scene��带 addictive�� �ϸ� ������� ��ε� �����ʰ� �ߺ��ؼ� �ҷ���
    //? �ٵ� �̰� �翬�� ��������� �ƴ�. �������� ������Ʈ�� ���� �����ִ°Ŵϱ�.

    //? �׷��ϱ� �� ����Ʈ�����ϰ� �״��������� ������ �Ѱ��ְ� �����ϸ� �ɵ�. �ٸ� ����Ʈ���� �ѹ� �ɸ��� ��Ұ� �ȵǼ� �޸𸮰� ����ؼ� ��������.
    //? ������ �� ���� �ٲ�� �ٽ� ��Ƴ�. �׷��ϱ� �׳� ���� ���������� ����ѹ� ���������ʹ� ��� �������ѳ��� �� ����� ������?
    //? �׳� ���湮�Ҷ����� �ʱ�ȭ �ѹ� �����ָ� �׸���.



}
