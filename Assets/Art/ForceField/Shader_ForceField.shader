Shader "Custom/Shader_ForceField"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _GridSize ("Grid Size", Float) = 1.0
        _PatternScrollSpeed ("Pattern Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
        _FresnelPower ("Fresnel Power", Float) = 2.0
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 1)
        _EmissionTex ("Emission Texture", 2D) = "black" {}
        _EmissionStrength ("Emission Strength", Float) = 1.0
        _MaskHeight ("Mask Height", Float) = 0.5
        _MaskSmoothness ("Mask Smoothness", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            sampler2D _MainTex;
            float _GridSize;
            float2 _PatternScrollSpeed;
            float _FresnelPower;
            fixed4 _FresnelColor;
            sampler2D _EmissionTex;
            float _EmissionStrength;
            float _MaskHeight;
            float _MaskSmoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _GridSize;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scrolling texture
                float2 uv = i.uv + _PatternScrollSpeed * _Time.y;
                fixed4 texColor = tex2D(_MainTex, uv);

                // Fresnel effect with color
                float fresnel = pow(1.0 - dot(i.worldNormal, i.viewDir), _FresnelPower);
                fixed4 fresnelColor = _FresnelColor * fresnel;

                // Emission
                fixed4 emissionColor = tex2D(_EmissionTex, uv) * _EmissionStrength;

                // Mask height with smoothness
                float mask = saturate(1.0 - smoothstep(_MaskHeight - _MaskSmoothness, _MaskHeight + _MaskSmoothness, i.worldPos.y));

                // Combine effects
                fixed4 finalColor = (texColor + fresnelColor + emissionColor) * mask;
                finalColor.a = ((texColor.a * fresnel) + fresnelColor.a) * mask; // Adjust transparency
                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Transparent/Diffuse"
}
