using UnityEngine;
using UnityEngine.UI;

public class UI_Return : UI_Base
{
    void Start()
    {
        Init();
    }

    enum Buttons
    {
        Return,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Buttons));

    }




}
