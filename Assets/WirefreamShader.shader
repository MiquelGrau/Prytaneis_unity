Shader "Custom/WireframeShader"
{
    Properties
    {
        _LineWidth("Line Width", Range(0,0.1)) = 0.005
    }

    SubShader
    {
        Pass
        {
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite On
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                uint vertexID : SV_VertexID; // Afegeix aquesta línia
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 barycentric : TEXCOORD0;
            };

            float _LineWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.barycentric = float3(0, 0, 0);
                if (v.vertexID % 3 == 0) o.barycentric.x = 1;
                if (v.vertexID % 3 == 1) o.barycentric.y = 1;
                if (v.vertexID % 3 == 2) o.barycentric.z = 1;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float minDist = min(min(i.barycentric.x, i.barycentric.y), i.barycentric.z);
                if (minDist > _LineWidth)
                {
                    discard;
                }
                return half4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
