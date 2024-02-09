Shader "Custom/FlashWhite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // ��������Ʈ �ؽ���
        _FlashIntensity ("Flash Intensity", Range(0, 1)) = 0 // �÷��� ����
        _ColorTint ("Flash Color", Color) = (1, 1, 1, 1) // �÷��� ����
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _FlashIntensity;
            fixed4 _ColorTint;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // ������ �κ��� ���� ������ �ʰ�, �ؽ��İ� �ִ� �κи� �÷���
                fixed4 finalColor = texColor.a > 0 ? lerp(texColor, _ColorTint, _FlashIntensity) : texColor;
                
                return finalColor;
            }
            ENDCG
        }
    }
}