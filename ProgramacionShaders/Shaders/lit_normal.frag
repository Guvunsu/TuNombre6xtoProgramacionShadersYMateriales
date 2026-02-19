#version 330 core
in vec2 vUV;
in mat3 vTBN;

out vec4 FragColor;

uniform sampler2D uDiffuse;
uniform sampler2D uNormalMap;

uniform vec3 uLightDir;
uniform vec3 uLightColor;
uniform vec3 uBaseColor;
uniform float uAmbient;

void main()
{
    vec3 albedo = texture(uDiffuse, vUV).rgb * uBaseColor;

    vec3 nTex = texture(uNormalMap, vUV).xyz;
    vec3 nTangent = normalize(nTex * 2.0 - 1.0);

    // Si se ve invertido:
    // nTangent.y *= -1.0;

    vec3 N = normalize(vTBN * nTangent);
    vec3 L = normalize(uLightDir);

    float diff = max(dot(N, L), 0.0);
    float intensity = clamp(uAmbient + diff, 0.0, 1.0);

    vec3 color = albedo * (uLightColor * intensity);
    FragColor = vec4(color, 1.0);
}
