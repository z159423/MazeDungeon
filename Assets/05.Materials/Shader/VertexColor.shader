Shader "Custom/vertexShaderWithStencil"
{
    Properties
    {
        Vector1_51D5406F("Smooth", Float) = 0
    }

        HLSLINCLUDE
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
#define SHADERGRAPH_PREVIEW 1

        CBUFFER_START(UnityPerMaterial)
        float Vector1_51D5406F;
    CBUFFER_END

        struct SurfaceDescriptionInputs
    {
        float4 VertexColor;
    };


    struct SurfaceDescription
    {
        float4 Out_0;
    };

    SurfaceDescription PopulateSurfaceData(SurfaceDescriptionInputs IN)
    {
        SurfaceDescription surface = (SurfaceDescription)0;
        surface.Out_0 = IN.VertexColor;
        return surface;
    }

    struct GraphVertexInput
    {
        float4 vertex : POSITION;
        float4 color : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    GraphVertexInput PopulateVertexData(GraphVertexInput v)
    {
        return v;
    }

    ENDHLSL

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Stencil {
                        Ref 2
                        Comp equal
                        Pass keep
                        ZFail decrWrap
                    }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag



            struct GraphVertexOutput
            {
                float4 position : POSITION;
                float4 VertexColor : COLOR;

            };



            GraphVertexOutput vert(GraphVertexInput v)
            {
                v = PopulateVertexData(v);

                GraphVertexOutput o;
                float3 positionWS = TransformObjectToWorld(v.vertex);
                o.position = TransformWorldToHClip(positionWS);
                float4 VertexColor = v.color;
                o.VertexColor = VertexColor;

                return o;
            }

            float4 frag(GraphVertexOutput IN) : SV_Target
            {
                float4 VertexColor = IN.VertexColor;

                SurfaceDescriptionInputs surfaceInput = (SurfaceDescriptionInputs)0;
                surfaceInput.VertexColor = VertexColor;

                SurfaceDescription surf = PopulateSurfaceData(surfaceInput);
                return all(isfinite(surf.Out_0)) ? half4(surf.Out_0.x, surf.Out_0.y, surf.Out_0.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);

            }
            ENDHLSL
        }
    }
}
