shader_type spatial;

uniform float alpha;

void fragment() {
	ALBEDO = vec3(255, 255, 0);
	ALPHA = alpha;
}