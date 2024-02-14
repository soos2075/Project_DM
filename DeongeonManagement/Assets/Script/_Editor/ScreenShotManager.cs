using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotManager : MonoBehaviour
{
    void Start()
    {
        
    }

    //string tempName = "temp_";
    int indexer = 0;

    public void ScreenShot(string screenName)
    {
        StartCoroutine(TakeScreenShot_Target(screenName));
    }

    private IEnumerator TakeScreenShot_Target(string screenName = "temp")
    {
        int realX = debugWorldX + (Screen.width / 2);
        int realY = debugWorldY + (Screen.height / 2);

        int sizeX = Mathf.Abs(debugWorldX) * 2;
        int sizeY = Mathf.Abs(debugWorldY) * 2;

        // 한 프레임을 기다립니다.
        yield return new WaitForEndOfFrame();

        string FolderPath = $"{Application.dataPath}/ScreenShot";

        Texture2D screenTex = new Texture2D(sizeX, sizeY, TextureFormat.RGB24, false);

        Rect area = new Rect(realX, realY, sizeX, sizeY);
        screenTex.ReadPixels(area, 0, 0);
        screenTex.Apply();

        // 폴더가 존재하지 않으면 새로 생성
        if (Directory.Exists(FolderPath) == false)
        {
            Directory.CreateDirectory(FolderPath);
        }

        // 스크린샷 저장
        File.WriteAllBytes($"{FolderPath}/{screenName}_{indexer}.png", screenTex.EncodeToPNG());

        Destroy(screenTex);
        Debug.Log(FolderPath + "스샷 저장");
        indexer++;
    }
    //private IEnumerator TakeScreenShot()
    //{
    //    // 한 프레임을 기다립니다.
    //    yield return new WaitForEndOfFrame();

    //    string FolderPath = $"{Application.dataPath}/ScreenShot";

    //    Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

    //    Rect area = new Rect(0f, 0f, Screen.width, Screen.height);
    //    screenTex.ReadPixels(area, 0, 0);
    //    screenTex.Apply();

    //    // 폴더가 존재하지 않으면 새로 생성
    //    if (Directory.Exists(FolderPath) == false)
    //    {
    //        Directory.CreateDirectory(FolderPath);
    //    }

    //    // 스크린샷 저장
    //    File.WriteAllBytes($"{FolderPath}/{tempName}{indexer}.png", screenTex.EncodeToPNG());

    //    Destroy(screenTex);

    //    Debug.Log(FolderPath + "스샷 저장");

    //    indexer++;
    //}


    public int debugWorldX;
    public int debugWorldY;

    void Update()
    {
        float posX = ((float)debugWorldX / ((float)Screen.width / 2)) * ((float)Screen.width / (float)Screen.height);
        Debug.DrawRay(new Vector3(posX * Camera.main.orthographicSize, -Screen.height, 0), Vector3.up * Screen.height * 2, Color.blue);
        Debug.DrawRay(new Vector3(-posX * Camera.main.orthographicSize, -Screen.height, 0), Vector3.up * Screen.height * 2, Color.blue);

        float posY = ((float)debugWorldY / ((float)Screen.height / 2));
        Debug.DrawRay(new Vector3(-Screen.width, posY * Camera.main.orthographicSize, 0), Vector3.right * Screen.width * 2, Color.red);
        Debug.DrawRay(new Vector3(-Screen.width, -posY * Camera.main.orthographicSize, 0), Vector3.right * Screen.width * 2, Color.red);
    }
}
