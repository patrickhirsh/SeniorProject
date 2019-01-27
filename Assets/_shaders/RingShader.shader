// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RingShader"
{
	 Properties {
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
     _OriginX ("PosX Origin", Range(0,1)) = 0.5
     _OriginY ("PosY Origin", Range(0,1)) = 0.5
     
     _LineThickness ("Line thickness", Range(0,1)) = 0.5
     _Radius ("Radius", Range(0,1)) = 0.5
     _MaxRadius ("Max Radius", Range(0,1)) = 0.25
     _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
     _TransColor ("Transparent Color", Color) = (0.0, 0.0, 0.0, 0.0)
     _Speed ("Speed", Float) = 1.0
 }
 
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 100
     
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha 
     
     Pass {  
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fog
             
             #include "UnityCG.cginc"
 
             struct appdata_t {
                 float4 vertex : POSITION;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f {
                 float4 vertex : SV_POSITION;
                 half2 texcoord : TEXCOORD0;
                 UNITY_FOG_COORDS(1)
             };
 
             sampler2D _MainTex;
             float4 _MainTex_ST;
             
             
             float4 _Color;
             float4 _TransColor;
             
             float _OriginX;
             float _OriginY;
             
             float _LineThickness;
             float _Radius;
             float _MaxRadius;
             float _Speed;
             
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                 UNITY_TRANSFER_FOG(o,o.vertex);
                 return o;
             }
             
             fixed4 frag (v2f i) : SV_Target
             {
                 fixed4 col = tex2D(_MainTex, i.texcoord);
                 UNITY_APPLY_FOG(i.fogCoord, col);
         
                 float xdist = _OriginX - i.texcoord.x;
                 float ydist = _OriginY - i.texcoord.y;
               
                 _Radius += (_Time[0] * _Speed) % _MaxRadius;
                
                 if(_Radius >= _MaxRadius)
                    _Radius = 0;
               
                 float distanceToCenter = (xdist * xdist + ydist * ydist);
                 
                 if(distanceToCenter > _Radius - _LineThickness && distanceToCenter < _Radius + _LineThickness)
                 {
                    col = _Color;
                 }
                 else
                 {
                    col = _TransColor;
                 }
                 return col;
             }
         ENDCG
     }
 }
 
 }