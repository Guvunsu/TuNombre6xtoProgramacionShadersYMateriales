#version 330 core 

layout (location -0 ) in vec2 aPos;
in vec3 aColor;

out vec3 Color;

void main(){
    FragColor = vec4 (vColor, 1.0);
}