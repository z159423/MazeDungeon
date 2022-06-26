// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

    Shader "StencilShader/StencilMaskShader01" {
		
        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
            
			Stencil {
					Ref 2
					Comp always
					Pass replace
				}

			Pass{
				//don't draw color or depth
				Blend Zero One
				ZWrite Off

				CGPROGRAM
				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag

				struct appdata {
					float4 vertex : POSITION;
				};

				struct v2f {
					float4 position : SV_POSITION;
				};

				v2f vert(appdata v) {
					v2f o;
					//calculate the position in clip space to render the object
					o.position = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_TARGET{
					return 0;
				}

				ENDCG
			}
        }
    }
