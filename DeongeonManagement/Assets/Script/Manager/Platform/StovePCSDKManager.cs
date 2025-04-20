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
    //? ù �������� ���� �� ó���� �� �ҰŶ�� ��� �ǰ�, ���� �ٲ� ��� �ؾ��Ѵٸ� �ʿ�
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
            // �ʱ�ȭ ������ ����  RunCallback �ֱ���  ȣ��
            this.runcallbackCoroutine = StartCoroutine(RunCallback(0.5f));
        }
        else
        {
            // �ʱ�ȭ ���з� ���� ����
            Debug.Log($"Stove Initialize fail : {(int)sdkResult} / code Name : {sdkResult.ToString()}");
            Application.Quit();
        }
    }




    private void OnInitializationComplete()
    {
        Debug.Log("PC SDK initialization success");

        //? �ʱ�ȭ ���� �� ������ Ȯ�� / Ȯ�ΰ�� �׽�Ʈ���� OnOwnership�� �ƿ� ȣ���� �ȵǴµ�. (result�� NoError�� �� �߱� ��)
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

    private void BeginQuitAppDueToError() //? �Ʒ� �ּ��� ���� �ִ� �ּ�
    {
        // ��¼�� ����� ��� ���� �ߴ��ϱ⺸�ٴ� ����ڿ��� �� �ߴܿ� ���� �޽����� ������ ��
        // ����� �׼�(e.g. ���� ��ư Ŭ��)�� ���� ���� �ߴ��ϰ� �;� ������ �𸨴ϴ�.
        // �׷��ٸ� ���⿡ QuitApplication�� ����� ��Ÿ��� ������ �����Ͻʽÿ�.
        // �����ϴ� �ʼ� ���� �۾� ������ ���� �޽����� �Ʒ��� �����ϴ�.
        // �ѱ��� : �ʼ� ���� �۾��� �����Ͽ� ������ �����մϴ�.
        // �� �� ��� : The required pre-task fails and exits the game.
        Application.Quit();
    }
    private void OnOwnership(StovePCOwnership[] ownerships)
    {
        bool owned = false;

        foreach (var ownership in ownerships)
        {

            //? �߰������� ownership.MemberNo �񱳵� ����
            // [OwnershipCode] 1: ������ ȹ��, 2: ������ ����(���� ����� ���)
            if (ownership.OwnershipCode != 1)
            {
                continue;
            }

            // [GameCode] 3: BASIC ����, 4: DEMO
            if (ownership.GameId == GameId && ownership.GameCode == 3)
            {
                owned = true; // ������ Ȯ�� ���� true�� ����
            }
        }

        if (owned)
        {
            // ������ ������ ���������� �Ϸ� �� ���� �������� ���� �ۼ�
            Debug.Log("Success : Stove Ownership");
            ToggleRunCallback_ValueChanged(false);
        }
        else
        {
            // ������ �������� �� ������ �����ϰ� �����޼��� ǥ�� ���� �ۼ�
            Debug.Log("Fail : Stove Ownership");
            Application.Quit();
        }
    }




    //? �ڷ�ƾ ���� (���� ���� �� �ٸ� �ݹ鵵 �޾ƾߵǸ� ȣ���� �ʿ� ����. ���� ������ �Ǹ� �ʿ��� ��)
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



    //? �� �ڷ�ƾ�� �����־�� �ݹ��� ���� �� ���� (this.callback) , �ݴ�� ������ �� ������ �ʿ���ٸ� �� �ڷ�ƾ�� ���� ��
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
