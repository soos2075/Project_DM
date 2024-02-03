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

    public string myName = "응애";


    //? 얘는 단일 싱글톤이어야함. 데이터를 GameManager에게 전달해줘야함
    //? DontDestroy일 필요가 있을지는 애매함. 돌아갈 때 SceneLoaded에 체인을 걸어놓으면 데이터를 전달해주고 알아서 부셔지지않을까 싶다.
    //? chatgpt한테 물어보니까 DontDestroy말고는 방법 없는듯. 그거 말고는 Scene모드를 addictive로 하면 현재씬이 언로드 되지않고 중복해서 불러옴
    //? 근데 이건 당연히 좋은방법은 아님. 이전씬꺼 오브젝트가 몽땅 남아있는거니까.

    //? 그러니까 걍 돈디스트로이하고 그다음씬가서 데이터 넘겨주고 삭제하면 될듯. 다만 돈디스트로이 한번 걸린건 취소가 안되서 메모리가 계속해서 남아있음.
    //? 심지어 또 씬이 바뀌면 다시 살아남. 그러니까 그냥 굳이 죽이지말고 길드한번 가고나서부터는 계속 유지시켜놔도 별 상관은 없을듯?
    //? 그냥 길드방문할때마다 초기화 한번 때려주면 그만임.



}
