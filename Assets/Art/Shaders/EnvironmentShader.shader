Shader "Unlit/EnvironmentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
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

            //Referenced this wonderful page for help on the basics SDF's because i keep forgetting them
            // https://iquilezles.org/articles/distfunctions/

            //SDFS from iquellez
            float sdSphere(float3 p, float s )
            {
                return length(p)-s;
            }

            float sdBox(float3 p, float3 b )
            {
                float3 q = abs(p) - b;
                return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
            }

            float sdRoundBox(float3 p, float3 b, float r )
            {
                float3 q = abs(p) - b + r;
                return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
            }

            float sdBoxFrame(float3 p, float3 b, float e )
            {
                p = abs(p  )-b;
                float3 q = abs(p+e)-e;
                return min(min(
                    length(max(float3(p.x,q.y,q.z),0.0))+min(max(p.x,max(q.y,q.z)),0.0),
                    length(max(float3(q.x,p.y,q.z),0.0))+min(max(q.x,max(p.y,q.z)),0.0)),
                    length(max(float3(q.x,q.y,p.z),0.0))+min(max(q.x,max(q.y,p.z)),0.0));
            }

            float sdTorus(float3 p, float2 t )
            {
                float2 q = float2(length(p.xz)-t.x,p.y);
                return length(q)-t.y;
            }

            float sdCone(float3 p, float2 c, float h )
            {
              float q = length(p.xz);
              return max(dot(c.xy,float2(q,p.y)),-h-p.y);
            }

            float sdOctahedron(float3 p, float s)
            {
              p = abs(p);
              return (p.x+p.y+p.z-s)*0.57735027;
            }

            //Combiners from iquellez
            float opUnion( float d1, float d2 )
            {
                return min(d1,d2);
            }
            float opSubtraction( float d1, float d2 )
            {
                return max(-d1,d2);
            }
            float opIntersection( float d1, float d2 )
            {
                return max(d1,d2);
            }
            float opXor(float d1, float d2 )
            {
                return max(min(d1,d2),-max(d1,d2));
            }
            
            float opSmoothUnion( float d1, float d2, float k )
            {
                float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
                return lerp( d2, d1, h ) - k*h*(1.0-h);
            }

            float opSmoothSubtraction( float d1, float d2, float k )
            {
                float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
                return lerp( d2, -d1, h ) + k*h*(1.0-h);
            }

            float opSmoothIntersection( float d1, float d2, float k )
            {
                float h = clamp( 0.5 - 0.5*(d2-d1)/k, 0.0, 1.0 );
                return lerp( d2, d1, h ) + k*h*(1.0-h);
            }
            
            
            //Aberrations

            float AberrationA(float3 p)
            {
                return sdSphere(p,1);
            }

            float AberrationB(float3 p)
            {
                return sdOctahedron(p,2);
            }
            
            float SceneSDF(float3 p)
            {
                float dst = 10000;
                for(int i = 0; i < _AberrationCount; i++)
                {
                    float scale = _Aberrations[i].scale * 0.5;
                    float3 localP = (p - _Aberrations[i].position) / scale;
                    switch (_Aberrations[i].id)
                    {
                        case 0: dst = min(dst,AberrationA(localP) * scale); break;
                        case 1: dst = min(dst,AberrationB(localP) * scale); break;
                        default: break;
                    }
                }
                return dst;
            }

            float3 SceneNormal(float3 p)
            {
                float epsilon = 0.00001;
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
                for(int i = 0; i < 500; i++)
                {
                    float3 p = rayOrigin + rayDir * travelled;
                    float d = SceneSDF(p);
                    if(d < 0.001f)
                    {
                        float3 normal = SceneNormal(p);
                        
                        col.rgb = dot(normal, _WorldSpaceLightPos0) * 0.5 + 0.5;
                        o.depth = LinearToDepth(distance(p, rayOrigin));
                        hit = true;
                        break;
                    }
                    if(d >= 1000) {
                        stepCount = i;
                        break;
                    }
                    travelled += d;
                }
                o.color = col;
                if(!hit) o.depth = LinearToDepth(900);
                return o;
            }
            ENDCG
        }
    }
}
