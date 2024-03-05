Shader "Unlit/SpaceShipShader"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Mask", 2D) = "white" {}
        [NoScaleOffset]_TEXTURE("Texture", 2D) = "white" {}
        [HDR]_ShipColor("Ship Color", Color) = (1,1,1,1)
        [HDR]_WindowColor("Window Color", Color) = (1,1,1,1)
        [HDR]_EngineColor("Engine Color", Color) = (1,1,1,1)
        [HDR]_OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Range(0.0, 1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float3 normal : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _TEXTURE;
            float4 _ShipColor;
            float4 _WindowColor;
            float4 _EngineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mask = tex2D(_MainTex, i.uv);
                fixed4 col = 0;
                float lightVal = dot(i.normal, _WorldSpaceLightPos0) * 0.5 + 0.5;
                lightVal = floor(lightVal * 10) / 10;
                col += round(mask.r) * _ShipColor; //tex2D(_TEXTURE, i.uv).r
                col += round(mask.g) * tex2D(_TEXTURE, i.uv).g * _WindowColor;
                float offsetA = tex2D(_TEXTURE, i.uv + float2(_Time.x,-_Time.x)).r;
                float offsetB = tex2D(_TEXTURE, i.uv + float2(_Time.x,-_Time.x) * 0.5).r;
                col += round(mask.b) * (tex2D(_TEXTURE, i.uv + float2(offsetA,offsetB)).b * _EngineColor);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * lightVal;
            }
            ENDCG
        }

        //Outline based on https://www.videopoetics.com/tutorials/pixel-perfect-outline-shaders-unity/
        Pass {

            Cull Front

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            half _OutlineWidth;

            float4 vert(float4 position : POSITION, float3 normal : NORMAL) : SV_POSITION
            {
                float4 clipPosition = UnityObjectToClipPos(position);
                float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, normal));

                float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * 10.0f * clipPosition.w * 2;
                clipPosition.xy += offset;

                return clipPosition;
            }

            half4 _OutlineColor;

            half4 frag() : SV_TARGET
            {
                return _OutlineColor;
            }

            ENDCG

        }
    }
}
