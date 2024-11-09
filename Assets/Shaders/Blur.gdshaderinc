//Kawase算法表现不错
vec4 KawaseBlur(sampler2D sampler, vec2 uv, vec2 texelSize, float pixelOffset)
{
	vec4 o = vec4(0);
	o += texture(sampler, uv + vec2(pixelOffset +0.5, pixelOffset +0.5) * texelSize); 
	o += texture(sampler, uv + vec2(-pixelOffset -0.5, pixelOffset +0.5) * texelSize); 
	o += texture(sampler, uv + vec2(-pixelOffset -0.5, -pixelOffset -0.5) * texelSize); 
	o += texture(sampler, uv + vec2(pixelOffset +0.5, -pixelOffset -0.5) * texelSize); 
	return o / 4.;
}