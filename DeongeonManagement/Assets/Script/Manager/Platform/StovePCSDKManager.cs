using Stove.PCSDK.NET;
using System.Collections;
using UnityEngine;

public class StovePCSDKManager : MonoBehaviour
{
    // Setting value filled through 'LoadConfig'
    public const string Env = "live";
    public const string AppKey = "7fc1c44beb31f6b706df1592cc2b4d6fcba63ca9caf37165375664bbfe3f4d20";
    public const string AppSecret = "063eb3d7f548203182409d7ab7a60718e843796ef93262506c8c0c45d30bc1c57374a87c9b9925e51a79c4016db6985b";
    public const string GameId = "NOVICE_IND";
    public StovePCLogLevel LogLevel;
    public string LogPath;

    private StovePCCallback callback;
    private Coroutine runcallbackCoroutine;


#if STOVE_BUILD
    //? 첫 씬에서만 인증 및 처리를 다 할거라면 없어도 되고, 씬이 바뀌어도 계속 해야한다면 필요
    //private void Awake()
    //{
    //    DontDestroyOnLoad(transform.gameObject);
    //}

    private void Start()
    {
        Stove_Init();
    }


    private void OnDestroy()
    {
        if (runcallbackCoroutine != null)
        {
            StopCoroutine(runcallbackCoroutine);
            runcallbackCoroutine = null;
        }

        StovePC.Uninitialize();
    }
#endif

    void Stove_Init()
    {
        StovePCConfig config = new StovePCConfig
        {
            Env = Env,
            AppKey = AppKey,
            AppSecret = AppSecret,
            GameId = GameId,
            LogLevel = this.LogLevel,
            LogPath = this.LogPath
        };

        this.callback = new StovePCCallback
        {
            OnError = new StovePCErrorDelegate(this.OnError),
            OnInitializationComplete = new StovePCInitializationCompleteDelegate(this.OnInitializationComplete),
            OnOwnership = new StovePCOwnershipDelegate(this.OnOwnership)
        };

        StovePCResult sdkResult = StovePC.Initialize(config, this.callback);
        if (StovePCResult.NoError == sdkResult)
        {
            // 초기화 오류가 없어  RunCallback 주기적  호출
            this.runcallbackCoroutine = StartCoroutine(RunCallback(0.5f));
        }
        else
        {
            // 초기화 실패로 게임 종료
            Debug.Log($"Stove Initialize fail : {(int)sdkResult} / code Name : {sdkResult.ToString()}");
            Application.Quit();
        }
    }




    private void OnInitializationComplete()
    {
        Debug.Log("PC SDK initialization success");

        //? 초기화 성공 후 소유권 확인 / 확인결과 테스트에선 OnOwnership이 아예 호출이 안되는듯. (result는 NoError로 잘 뜨긴 함)
        StovePCResult result = StovePC.GetOwnership();
        if (StovePCResult.NoError != result)
        {
            Debug.Log($"Ownership fail : {(int)result} / code Name : {result.ToString()}");
        }
    }

    private void OnError(StovePCError error)
    {
        switch (error.FunctionType)
        {
            case StovePCFunctionType.Initialize:
            case StovePCFunctionType.GetUser:
            case StovePCFunctionType.GetOwnership:
                BeginQuitAppDueToError();
                break;
        }
    }

    private void BeginQuitAppDueToError() //? 아래 주석은 원래 있던 주석
    {
        // 어쩌면 당신은 즉시 앱을 중단하기보다는 사용자에게 앱 중단에 대한 메시지를 보여준 후
        // 사용자 액션(e.g. 종료 버튼 클릭)에 따라 앱을 중단하고 싶어 할지도 모릅니다.
        // 그렇다면 여기에 QuitApplication을 지우고 당신만의 로직을 구현하십시오.
        // 권장하는 필수 사전 작업 오류에 대한 메시지는 아래와 같습니다.
        // 한국어 : 필수 사전 작업이 실패하여 게임을 종료합니다.
        // 그 외 언어 : The required pre-task fails and exits the game.
        Application.Quit();
    }
    private void OnOwnership(StovePCOwnership[] ownerships)
    {
        bool owned = false;

        foreach (var ownership in ownerships)
        {

            //? 추가적으로 ownership.MemberNo 비교도 가능
            // [OwnershipCode] 1: 소유권 획득, 2: 소유권 해제(구매 취소한 경우)
            if (ownership.OwnershipCode != 1)
            {
                continue;
            }

            // [GameCode] 3: BASIC 게임, 4: DEMO
            if (ownership.GameId == GameId && ownership.GameCode == 3)
            {
                owned = true; // 소유권 확인 변수 true로 설정
            }
        }

        if (owned)
        {
            // 소유권 검증이 정상적으로 완료 된 이후 게임진입 로직 작성
            Debug.Log("Success : Stove Ownership");
            ToggleRunCallback_ValueChanged(false);
        }
        else
        {
            // 소유권 검증실패 후 게임을 종료하고 에러메세지 표출 로직 작성
            Debug.Log("Fail : Stove Ownership");
            Application.Quit();
        }
    }




    //? 코루틴 종료 (만약 인증 후 다른 콜백도 받아야되면 호출할 필요 없음. 저는 인증만 되면 필요없어서 끔)
    public void ToggleRunCallback_ValueChanged(bool isOn)
    {
        if (isOn)
        {
            float intervalSeconds = 1f;
            runcallbackCoroutine = StartCoroutine(RunCallback(intervalSeconds));
        }
        else
        {
            if (runcallbackCoroutine != null)
            {
                StopCoroutine(runcallbackCoroutine);
                runcallbackCoroutine = null;
            }
        }
    }



    //? 이 코루틴이 돌고있어야 콜백을 받을 수 있음 (this.callback) , 반대로 인증이 다 끝나서 필요없다면 이 코루틴을 꺼도 됨
    private IEnumerator RunCallback(float intervalSeconds)
    {
        WaitForSeconds wfs = new WaitForSeconds(intervalSeconds);
        while (true)
        {
            Debug.Log("RunCallback");
            StovePC.RunCallback();
            yield return wfs;
        }
    }


}
