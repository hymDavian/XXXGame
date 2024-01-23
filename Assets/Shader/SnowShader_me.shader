Shader "Custom/SnowShader_me"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {} //主要显示的贴图纹理
        _Bump ("Bump", 2D) = "bump" {} //凹凸纹理类型

        _Snow ("Snow Level",Range(0,1)) = 0 //积雪覆盖数量
        _SnowColor ("Snow Color",Color) = (1,1,1,1) //积雪颜色
        _SnowDirection ("Snow Direction",Vector) = (0,1,0) //下雪方向
        _SnowDepth ("Snow Depth",Range(0,0.3)) = 0.1 //厚度
        _Wetness ("Wetness",Range(0,0.5)) = 0.3 //过渡时的雪透明度
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        //vertex:vert 表示使用自定义vertex顶点函数，函数名为vert
        #pragma surface surf Lambert vertex:vert 

        //声明并连接Properties里定义的变量
        sampler2D _MainTex;
        sampler2D _Bump; 
        float _Snow;
        float4 _SnowColor;
        float4 _SnowDirection;
        float _SnowDepth;
        float _Wetness;

        //这个结构体就是获取上面2张图的每个像素点的uv坐标
        struct Input{
            float2 uv_MainTex;
            float2 uv_Bump;
            float3 worldNormal;
            INTERNAL_DATA //需要这个变量来计算世界坐标系的法向量
        };

        void surf (Input IN,inout SurfaceOutput o){
            half4 c = tex2D(_MainTex,IN.uv_MainTex);//像素颜色基础值
            o.Normal = UnpackNormal(tex2D(_Bump,IN.uv_Bump));//UnpackNormal获取bump图片的对应uv位置的法向值

            //直接赋值雪色彩的方式
            //dot函数取两向量之间夹角余弦值
            //余弦值越接近1表示向量之间方向越一致，夹角越小   越接近-1表示夹角越大
            //lerp(1,-1,_Snow)表示需求的积雪数量越少，这个插值结果越接近1，也就是所需的最低要求夹角值差越小，越难以满足积雪条件，从而使得_Snow这个值能够控制积雪覆盖程度
            // half3 worldDir = WorldNormalVector(IN, o.Normal); //得到切空间法向值在世界坐标系下真正的法向值
            // if(dot(worldDir,_SnowDirection.xyz) >= lerp(1,-1,_Snow))
            // {
            //     o.Albedo = _SnowColor.rgb;
            // }
            // else
            // {
            //     o.Albedo = c.rgb;
            // }
            // o.Alpha = 1;
            

            //过渡模式
            float difference = dot(WorldNormalVector(IN,o.Normal),_SnowDirection) - lerp(1,-1,_Snow);//当前法向量和降雪向量所成角度 ， 与 需求角度的比例差距值
            difference = saturate(difference/_Wetness); //这个差距再除去湿度，差距越大，湿度越高
            o.Albedo = difference*_SnowColor.rgb + (1-difference) * c;  //雪的颜色带上湿度 +  除去湿度的剩余色彩比例，用于作为原来色彩的占比
            o.Alpha = c.a;
          

            //角度方式直观理解
            /*
            float EulRatio =dot(WorldNormalVector(IN,o.Normal),_SnowDirection); //理解为一个 1->-1的角度插值
            float Eul = lerp(0,180, EulRatio); //法向量和降雪方向所成角度
            float needMaxEul = lerp(0,180,_Snow); //最大允许角度
            if (Eul > needMaxEul ) //所成角度在最大允许角度内
            {
                o.Albedo = _SnowColor.rgb;
            }
            else
            {
                o.Albedo = c.rgb;
            }
            o.Alpha = 1;
            */


        }

        void vert(inout appdata_full v)
        {
            // float4 sn = mul(UNITY_MATRIX_IT_MV,_SnowDirection);//将降雪方向转化到模型局部坐标系下
            if( dot(v.normal,_SnowDirection.xyz) >= lerp(1,-1,(  (1-_Wetness)*_Snow*2)/3) ) //如果下雪方向和这个顶点法向方向所成角度余弦值 大于 降雪量阈值控制的2倍除以3，也就是降雪覆盖量越大，所需的角度差容忍值越接近-1，越能满足需要堆叠雪厚度，但不会超过0.6，也就是最多角度差不会超过316度
            {
                v.vertex.xyz += (_SnowDirection.xyz + v.normal) * _SnowDepth * _Snow;//这个顶点位置沿着法向量升高设定的积雪厚度数
            }
        }

        ENDCG
    }
    FallBack "Diffuse"
}
