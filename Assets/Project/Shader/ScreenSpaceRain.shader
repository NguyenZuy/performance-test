Shader "Custom/ScreenSpaceRain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RainTex ("Rain Texture", 2D) = "white" {}
        _RainSpeed ("Rain Speed", Range(0, 10)) = 1
        _RainIntensity ("Rain Intensity", Range(0, 1)) = 0.5
        _RainColor ("Rain Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _RainTex;
            float4 _MainTex_ST;
            float _RainSpeed;
            float _RainIntensity;
            float4 _RainColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Calculate rain UV with time-based movement
                float2 rainUV = i.uv;
                rainUV.y += _Time.y * _RainSpeed;
                
                // Sample rain texture
                fixed4 rain = tex2D(_RainTex, rainUV);
                
                // Combine rain with main texture
                col.rgb = lerp(col.rgb, _RainColor.rgb, rain.r * _RainIntensity);
                
                return col;
            }
            ENDCG
        }
    }
}
