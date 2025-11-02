Shader "Custom/URPAdditiveGlow"
{
    Properties
    {
        _GlowColor ("Glow Color", Color) = (1,0,0,1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _FresnelPower ("Edge Glow", Range(0.5, 5)) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        Blend One One
        ZWrite Off
        Cull Back
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _GlowColor;
                float _GlowIntensity;
                float _FresnelPower;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 viewDirWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = posInputs.positionCS;
                
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);
                
                // Fresnel
                float fresnel = saturate(dot(viewDirWS, normalWS));
                fresnel = pow(fresnel, _FresnelPower);
                
                // Pulse
                float pulse = sin(_Time.y * 3) * 0.3 + 0.7;
                
                return _GlowColor * fresnel * _GlowIntensity * pulse;
            }
            ENDHLSL
        }
    }
}