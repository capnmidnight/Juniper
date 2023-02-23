// MY FRAGMENT SHADER#define THREEZ
uniform float opacity;
uniform sampler2D map;
varying vec2 vUv;
void main(){
  gl_FragColor = texture2D( map, vUv );
#ifdef PREMULTIPLIED_ALPHA
	gl_FragColor.rgb *= gl_FragColor.a;
#endif
  gl_FragColor.a = opacity + 0.1;
}