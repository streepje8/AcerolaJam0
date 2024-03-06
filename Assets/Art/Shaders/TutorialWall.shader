Shader "Unlit/TutorialWall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color("Color", Color) = (1,1,1,1)
        _NoiseMultiplier("Noise Multiplier", Range(0, 1)) = 0.5
        _NoiseScale("Noise Scale", Range(0, 1)) = 0.5
        [MaterialToggle]_DEBUG("DEBUG", Int) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 hitPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float3 _PlayerPos;
            float _NoiseScale;
            float _NoiseMultiplier;
            int _DEBUG;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.hitPos = worldPos.xyz / worldPos.w;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, ((abs(i.normal.y) > 0.5) ? i.hitPos.xz : ((abs(i.normal.x) > 0) ? i.hitPos.yz : i.hitPos.xy)) / 10.0f); //So i can strech the walls haha
                col *= _Color;
                float noise = snoise(float4(i.hitPos,_Time.y) * _NoiseScale);
                col.a = 1 - clamp(pow(distance(i.hitPos,_PlayerPos) / 5 + (noise * _NoiseMultiplier),2), 0, 1);
                if(_DEBUG)
                {
                    col = dot(i.normal, _WorldSpaceLightPos0) * 0.5 + 0.5;
                    col.a = 0.5;
                }
                return col;
            }
            ENDCG
        }
    }
}
