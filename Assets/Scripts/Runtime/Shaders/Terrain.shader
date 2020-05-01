Shader "Custom/Terrain" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base", 2D) = "white" {}
    }
 
    SubShader {
        Pass {
            Material {
            }
            ColorMaterial AmbientAndDiffuse
            Lighting On
            SetTexture [_MainTex] {
                Combine texture * primary, texture * primary
            }
            SetTexture [_MainTex] {
                constantColor [_Color]
                Combine previous * constant DOUBLE, previous * constant
            } 
        }
    }
 
    Fallback "VertexLit", 1
}
