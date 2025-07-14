using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private MaterialPropertyBlock propBlock;
    private SpriteRenderer sr;
    private float offset = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        offset += scrollSpeed * Time.unscaledDeltaTime;

        // MaterialPropertyBlock 사용 - 원본 Material 변경 없음
        sr.GetPropertyBlock(propBlock);
        propBlock.SetVector("_MainTex_ST", new Vector4(1, 1, offset, 0));
        propBlock.SetColor("_Color", sr.color);
        sr.SetPropertyBlock(propBlock);
    }


}
