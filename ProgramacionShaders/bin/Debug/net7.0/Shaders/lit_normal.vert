#version 330 core
layout(location=0) in vec3 aPos;
layout(location=1) in vec3 aNormal;
layout(location=2) in vec3 aTangent;
layout(location=3) in vec2 aUV;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProj;

out vec2 vUV;
out mat3 vTBN;

void main()
{
    vUV = aUV;

    vec3 N = normalize(mat3(uModel) * aNormal);
    vec3 T = normalize(mat3(uModel) * aTangent);
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);

    vTBN = mat3(T, B, N);

    vec4 world = uModel * vec4(aPos, 1.0);
    gl_Position = uProj * uView * world;
}
