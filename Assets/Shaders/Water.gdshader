shader_type spatial;
render_mode cull_back, depth_draw_always, shadows_disabled;

#include "Noise.gdshaderinc"
#include "Blur.gdshaderinc"

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, repeat_disable, filter_linear_mipmap;
uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, repeat_disable, filter_linear;

varying vec3 _Pos;

group_uniforms Effect;
uniform float DepthEffect : hint_range(0.0, 3.0) = 1.75;
group_uniforms;

group_uniforms Color;
uniform vec3 Color : source_color = vec3(.24,.721,1.);
uniform vec3 DepthColor : source_color = vec3(0,.384,.702);
uniform float Transparency : hint_range(0.0, 1.0) = 0.9;
group_uniforms;

group_uniforms Wave;
uniform float WaveStrength : hint_range(0.0, 1.0) = 0.8;
uniform vec2 WaveScale = vec2(1);
uniform vec4 WaveColor : source_color = vec4(0,0.82,1,0.22);
uniform vec2 WaveSpeed = vec2(0.1,0.12);
group_uniforms;

group_uniforms Blur;
uniform float BlurSize : hint_range(0.0, 3.0) = 1.2;
uniform float BlurStrength : hint_range(0.0, 2.0) = 1.2;
group_uniforms;

void vertex() {
	// Called for every vertex the material is visible on.
	vec4 upos = (MODEL_MATRIX * vec4(VERTEX, 1.0));
	_Pos = upos.xyz/upos.w;
}

float depth_map(float depth){
	return 1. - pow(E,-DepthEffect*depth);
}

void fragment() {
	vec3 color = vec3(0);
	
	//获取位置
	float depth = textureLod(DEPTH_TEXTURE, SCREEN_UV, 0.0).r;
	vec4 upos = INV_VIEW_MATRIX * (INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, depth, 1.0));
	vec3 pixel_pos = upos.xyz/upos.w;
	
	//垂直深度计算
	float depth_value = depth_map(_Pos.y-pixel_pos.y);
	color += mix(Color, DepthColor, depth_value);
	
	//波纹
	vec2 wave_pos = _Pos.xz/WaveScale/5. + TIME*WaveSpeed;
	float wave = cellular_noise(wave_pos, TIME) * WaveStrength;
	color = mix(color, WaveColor.rgb, wave * WaveColor.a);
	
	//模糊&混合
	float blur_range = atan(BlurSize/VERTEX.z);//摄像头距离补正
	vec3 blur = KawaseBlur(SCREEN_TEXTURE, SCREEN_UV, 1./VIEWPORT_SIZE, depth_value*blur_range*BlurStrength*10.).rgb;
	//旧：用mipmaps实现模糊
	//vec3 blur = textureLod(SCREEN_TEXTURE,SCREEN_UV,depth_value*BlurStrength).rgb;
	//混色方式，上面这行是混合，下面那行是乘法（颜色会更深沉，透明度调低效果更佳）
	color = mix(color, blur, Transparency);
	//color = mix(color, vec3(1), Transparency) * blur;
	
	//输出
	ALBEDO = color;
	ROUGHNESS = 0.25 * wave;
	SPECULAR = 0.5 * wave;
	METALLIC = 0.0 * wave;
	//ALPHA = 1. - Transparency;
}