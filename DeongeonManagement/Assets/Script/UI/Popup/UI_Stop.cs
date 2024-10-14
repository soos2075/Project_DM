using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Stop : UI_PopUp
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        
    }


    private void Update()
    {
        Time.timeScale = 0;
    }

    private void LateUpdate()
    {
        if (Managers.UI._popupStack.Count > 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(gameObject);
        }
    }





    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }
}
