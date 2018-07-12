Shader "Projector/Additive" { 
   Properties { 
      _ShadowTex ("Cookie", 2D) = "" { TexGen ObjectLinear } 
      _FalloffTex ("FallOff", 2D) = "" { TexGen ObjectLinear } 
   } 
   Subshader { 
      Pass { 
         ZWrite off 
         Fog { Color (1, 1, 1) } 
         ColorMask RGB 
         Blend One One 
         SetTexture [_ShadowTex] { 
            combine texture, ONE - texture 
            Matrix [_Projector] 
         } 
         SetTexture [_FalloffTex] { 
            constantColor (0,0,0,0) 
            combine previous lerp (texture) constant 
            Matrix [_ProjectorClip] 
         } 
      }
   }
}