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
        //? 스팀 API 와 연결하는 함수. 연결여부를 bool 값으로 해놓고 false 일 땐 아래 함수를 걍 리턴시켜버려도 될듯?
    }


    //? 세이브 할 때 호출하면 되지않을까 싶음. 아니면 시간 업데이트할때도 괜찮고. 아님 시간 멈출때 혹은 UI 클릭할 때 등등 타이밍은 많다.
    //? 중요한건 매프레임 하거나 렉걸릴만한 상황에만 안하면 될듯. 걍 UserData 업데이트 될 때 호출해도 그닥 상관은 없음.
    public void AchievementUpdate()
    {
        //? 스팀api랑 연결됐는지확인부터 하고 안됐으면 ㅌㅌ(오프라인)

        //? 조건체크 / 이미 달성한 업적인지(이건 없어도되려나?)
        if (true)
        {
            //? 스팀 api에서 업적 달성 불러오기
        }
    }

    //? 데이터 삭제시에도 절대로 삭제되지 않는 Record를 하나 만들지(진짜로 아예 기록용) 아니면 그냥 UserData에서만 받아서 할지는 고민중.
    //? UserData와 CollectionData, ClearData로 하는게 일단은 맞는 것 같긴함. 더 따로 작업할것도 없고.. 근데 흐음.. 업데이트 등으로 인해서 기존 세이브 슬롯을
    //? 못쓸 때 문제가 발생할 수도 있겠다만, 그건 뭐 미리 공지를 하든가 아니면 세이브 슬롯만 삭제 및 초기화하는걸 추가하면 될거같기도 함.
}



//? 업적관련 작업. 그런데 게임을 하다보면 10분 20분만에 자동으로 클리어되는 업적들은 안넣는게 좋을듯.
//? 예를 들어 처음으로 모험가를 쓰러트렸다 같은거랑 그냥 진행하면 자동으로 깨지는 그런것들.

/* 업적 리스트
 * 
 * 첫회차 클리어 -                        // 1
 * 
 * 엔딩별 클리어 - 엔딩 개수 * 1          //  7
 * 
 * 난이도별 클리어 - 난이도 개수 * 1        // 4
 * 
 * 도감 올컬렉 - 유닛 / 모험가 / 특성 / 시설 / 특수시설 / 아티팩트 / 엔딩   // 7
 * 
 * 
 * 
 * 무한모드 - 60일차 / 100일차          // 2
 * 
 * 제 10대 비보를 모두 모으기              // 1
 * 
 * 
 */

/* 업적 아이디어 (일단 보류)
 * 
 * 특수 동료 영입 
 * 진화 몬스터 (진화가능한 애들 다)
 * 
 * 첫 인기도 500/1000/2000 달성                // 2
 * 첫 위험도 500/1000/2000 달성                // 2
 * 첫 랭크 달성 D/C/B/A/S                    // 5
 * 
 * 모험가에게 승리 100/500/1000            //3
 * 획득한 마나 5000/20000/50000          //3
 * 획득한 골드 5000/20000/50000          //3
 * 유닛 훈련횟수 10/50/100                //3
 * 기도 횟수    10/50/100               //3
 * 
 * 
 */