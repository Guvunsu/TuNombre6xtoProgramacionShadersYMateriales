#version 330 core

in vec2 vUV;
in mat3 vTBN;

uniform sampler2D uDiffuse;
uniform sampler2D uNormalMap;

uniform vec3 uLightDir;
uniform vec3 uLightColor;
uniform float uAmbient;

out vec4 FragColor;

void main()
{
    vec3 normalTex = texture(uNormalMap, vUV).rgb;
    normalTex = normalTex * 2.0 - 1.0;

    vec3 N = normalize(vTBN * normalTex);

    float diff = max(dot(N, normalize(-uLightDir)), 0.0);

    vec3 color = texture(uDiffuse, vUV).rgb;

    vec3 lighting = color * (uAmbient + diff * uLightColor);

    FragColor = vec4(lighting, 1.0);
}
