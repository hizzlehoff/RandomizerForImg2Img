Shader "Custom/Gradient" {
    Properties{
        _TopColor("Top Color", Color) = (1,1,1,1)
        _BottomColor("Bottom Color", Color) = (0,0,0,1)
    }

        SubShader{
            Tags {"Queue" = "Overlay" "RenderType" = "Overlay" }
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma exclude_renderers gles xbox360 ps3
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f {
                    float4 pos : POSITION;
                    float4 color : COLOR;
                };

                uniform float4 _TopColor;
                uniform float4 _BottomColor;

                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.color = v.vertex.y > 0 ? _TopColor : _BottomColor;
                    return o;
                }

                fixed4 frag(v2f i) : COLOR {
                    return i.color;
                }
                ENDCG
            }
    }
}