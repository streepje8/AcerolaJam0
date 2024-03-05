Shader "Unlit/EnvironmentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _AberrationATex("AberrationATex", 2D) = "white" {}
        _AberrationBTex("AberrationBTex", 2D) = "white" {}
        _AberrationCTex("AberrationCTex", 2D) = "white" {}
        _AberrationDTex("AberrationDTex", 2D) = "white" {}
        _AberrationETex("AberrationETex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CULL FRONT
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"
            #include "RaymarchingSDFs.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 hitPos : TEXCOORD1;
            };

            struct fo //frag output
            {
                fixed4 color : SV_Target;
                float depth : SV_Depth;
            };

            struct Aberration
            {
                float3 position;
                float3 scale;
                int id;
            };


            StructuredBuffer<Aberration> _Aberrations;
            int _AberrationCount;
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.hitPos = worldPos.xyz / worldPos.w;
                return o;
            }


            //Aberration Textures
            sampler2D _AberrationATex;
            sampler2D _AberrationBTex;
            sampler2D _AberrationCTex;
            sampler2D _AberrationDTex;
            sampler2D _AberrationETex;
            
            //Aberrations
            float AberrationA(float3 p)
            {
                p = opTwistY(p, sin(_Time.y) + 2);
                return sdOctahedron(p,1);
            }

            float AberrationB(float3 p)
            {
                float boundingSphere = sdSphere(p, 2);
                if(boundingSphere > 1) return boundingSphere;
                float dstA = sdSphere(p - float3(sin(_Time.y),sin(_Time.z * 0.25),sin(_Time.y * 2)), 0.2);
                float dstB = sdSphere(p - float3(sin(_Time.y * 0.25),sin(_Time.y * 2),sin(_Time.z)), 0.2);
                float dstC = sdSphere(p, 0.4);
                float combinedDst = opSmoothUnion(dstA,opSmoothUnion(dstB,dstC,1.5),1.5);
                return combinedDst;
            }
            
            float AberrationC(float3 p)
            {
                float boundingSphere = sdSphere(p, 2);
                if(boundingSphere > 1) return boundingSphere;
                float dstA = sdSphere(p, 1.5);
                p = opTwistZ(p, sin(_Time.y));
                p = opTwistX(p, sin((_Time.y + 1.5) * 2));
                p = opTwistY(p, sin(_Time.y * 4));
                return opSmoothSubtraction(dstA,sdBox(p,float3(1.5,1.5,1.5)),1);
            }

            float AberrationD(float3 p)
            {
                float boundingSphere = sdSphere(p, 2);
                if(boundingSphere > 1) return boundingSphere;
                float dstA = sdSphere(p - float3(sin(_Time.y),sin(_Time.z * 0.25),sin(_Time.y * 2)), 0.2);
                float dstB = sdSphere(p - float3(sin(_Time.y * 0.25),sin(_Time.y * 2),sin(_Time.z)), 0.2);
                float dstC = sdSphere(p, 0.4);
                float combinedDst = opSmoothUnion(dstA,opSmoothUnion(dstB,dstC,1.5),1.5);
                return combinedDst;
            }

            float AberrationE(float3 p)
            {
                float boundingSphere = sdSphere(p, 2);
                if(boundingSphere > 1) return boundingSphere;
                float dstA = sdSphere(p - float3(sin(_Time.y),sin(_Time.z * 0.25),sin(_Time.y * 2)), 0.2);
                float dstB = sdSphere(p - float3(sin(_Time.y * 0.25),sin(_Time.y * 2),sin(_Time.z)), 0.2);
                float dstC = sdSphere(p, 0.4);
                float combinedDst = opSmoothUnion(dstA,opSmoothUnion(dstB,dstC,1.5),1.5);
                return combinedDst;
            }
            
            float2 SceneSDF(float3 p)
            {
                float dst = 10000;
                int returnAberrationType = -1;
                for(int i = 0; i < _AberrationCount; i++)
                {
                    float scale = _Aberrations[i].scale * 0.5;
                    float3 localP = (p - _Aberrations[i].position) / scale;
                    float oldDst = dst;
                    switch (_Aberrations[i].id)
                    {
                        case 0: dst = min(dst,AberrationA(localP) * scale); break;
                        case 1: dst = min(dst,AberrationB(localP) * scale); break;
                        case 2: dst = min(dst,AberrationC(localP) * scale); break;
                        case 3: dst = min(dst,AberrationD(localP) * scale); break;
                        case 4: dst = min(dst,AberrationE(localP) * scale); break;
                        default: break;
                    }
                    if(dst != oldDst) returnAberrationType = _Aberrations[i].id;
                }
                return float2(dst, returnAberrationType);
            }

            float3 SceneNormal(float3 p)
            {
                float epsilon = 0.0001;
                float centerDistance = SceneSDF(p);
                float xDistance = SceneSDF(p + float3(epsilon, 0, 0));
                float yDistance = SceneSDF(p + float3(0, epsilon, 0));
                float zDistance = SceneSDF(p + float3(0, 0, epsilon));
                return normalize((float3(xDistance, yDistance, zDistance) - centerDistance) / epsilon);
            }

            //Found this epic function at https://forum.unity.com/threads/how-to-render-object-with-just-a-depth-texture.718607/#:~:text=Code%20(CSharp)%3A-,float%20LinearToDepth(float%20linearDepth),%C2%A0%20%C2%A0%20%C2%A0%20%C2%A0%20%C2%A0%20%C2%A0%20%7D,-Another%20option%3A
            float LinearToDepth(float linearDepth)
            {
                return (1.0 - _ZBufferParams.w * linearDepth) / (linearDepth * _ZBufferParams.z);
            }

            fo frag (v2f i)
            {
                fo o;
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.hitPos - rayOrigin);
                fixed4 col = 0;

                //Add Stars
                float noise = snoise(rayDir * 100) * 0.5 + 0.5;
                col += (noise > 0.9);
                
                float travelled = 0;
                int stepCount = 0;
                bool hit = false;
                float dotNL = 0;
                int hitType = -1;
                for(int i = 0; i < 500; i++)
                {
                    float3 p = rayOrigin + rayDir * travelled;
                    float2 dt = SceneSDF(p);
                    float d = dt.x;
                    if(d < 0.001f)
                    {
                        hitType = int(dt.y);
                        float3 normal = SceneNormal(p);
                        dotNL = dot(normal, _WorldSpaceLightPos0) * 0.5 + 0.5;
                        float finalDepth = LinearToDepth(distance(p, rayOrigin + rayDir * 0.02));
                        o.depth = finalDepth;
                        hit = true;
                        break;
                    }
                    if(d >= 1000) {
                        stepCount = i;
                        break;
                    }
                    travelled += d;
                    stepCount++;
                }
                switch(hitType)
                {
                    case 0: col.rgb = tex2D(_AberrationATex, float2(dotNL, 0.5)).rgb; break;
                    case 1: col.rgb = tex2D(_AberrationBTex, float2(dotNL, 0.5)).rgb; break;
                    case 2: col.rgb = tex2D(_AberrationCTex, float2(dotNL, 0.5)).rgb; break;
                    case 3: col.rgb = tex2D(_AberrationDTex, float2(dotNL, 0.5)).rgb; break;
                    case 4: col.rgb = tex2D(_AberrationETex, float2(dotNL, 0.5)).rgb; break;
                    default: break;
                }
                float glow = stepCount / 30.0f;
                col.rgb += pow(clamp(glow,0,1),2) * 0; //TODO: FIX THIS SHIT LATER
                o.color = col;
                if(!hit) o.depth = LinearToDepth(900);
                return o;
            }
            ENDCG
        }
    }
}
