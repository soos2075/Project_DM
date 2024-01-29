using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StateBar : UI_Base, IWorldSpaceUI
{
    void Start()
    {
        SetCanvasWorldSpace();
        Init();
    }
    void Update()
    {
        UpdateState();
    }
    void LateUpdate()
    {
        transform.position = npc.transform.position + new Vector3(0, -0.3f, 0);
    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace);
    }


    enum State
    {
        hp,
        mp,
        ap,
    }


    NPC npc;

    public override void Init()
    {
        Bind<Image>(typeof(State));
        npc = GetComponentInParent<NPC>();
        hp_origin = npc.HP;
        mp_origin = npc.Mana;
        ap_origin = npc.ActionPoint;
    }


    float hp_origin;
    float mp_origin;
    float ap_origin;

    void UpdateState()
    {
        GetImage(((int)State.hp)).fillAmount = npc.HP / hp_origin;
        GetImage(((int)State.mp)).fillAmount = npc.Mana / mp_origin;
        GetImage(((int)State.ap)).fillAmount = npc.ActionPoint / ap_origin;
    }




}
