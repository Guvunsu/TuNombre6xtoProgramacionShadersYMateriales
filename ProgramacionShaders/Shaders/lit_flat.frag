#version 330 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uTex;
 
//luz basica 

uniform vec3 uLightDir;   //Direccion hacia la luz
uniform vec3 uLightColor; //color de la luz
uniform  vec3 uBaseColor; //color base del material (tinte)
uniform float uAmbient;   //lus ambiental [0.....1]

void main (){
    //para pruebas, definimos una normal que apunta hacia Z+
    vec3 N = vec3 (0.0,0.0,1.0);

    //uLightDir direccion de la cara hacia la luz 
    vec3 = L =normalize (uLightDir);

    //Difusion: Cuanta luz pega,segun el angulo
    float diff  = max (dot(N,L),0.0);

    //intensidad final con ambiente 
    float intensity = clamp(uAmbient + diff , 0.0 , 1.0);

    //Color del material = textura*tinte 
    vec3 tex = texture (uTex,vUV).rgb;
    vec3 base = tex * uBaseColor;

    //iluminacion 
    vec3 color = base * (uLightColor * intensity);

    FragColor = vec4(color,1.0);
}