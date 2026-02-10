#version 330 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uTex;

// Luz basica
uniform vec3 uLightDir;    // Direccion HACIA la luz.
uniform vec3 uLightColor;  // Color de la luz.
uniform vec3 uBaseColor;   // Color base del material (tinte).
uniform float uAmbient;    // Luz ambiente [0...1].

void main()
{
    vec3 N = vec3(0.0, 0.0, 1.0);

    vec3 L = normalize(uLightDir);

    float diff = max(dot(N,L), 0.0);

    float intensity = clamp(uAmbient + diff, 0.0, 1.0);

    vec3 tex = texture(uTex, vUV).rgb;
    vec3 base = tex * uBaseColor;

    vec3 color = base * (uLightColor * intensity);
    FragColor = vec4(color, 1.0);
}