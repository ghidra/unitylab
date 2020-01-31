
//float2 rotate(float2 coord, float angle) {
//	return float2x2(cos(angle), -sin(angle), sin(angle), cos(angle)) * coord;
//}

#define PI 3.1415927
#define TWO_PI 6.2831855

float easeOutQuad(float t) {
	return -1.0 * t * (t - 2.0);
}

float easeOutCubic(float t) {
	return (t = t - 1.0) * t * t + 1.0;
}

float easeInCubic(float t) {
	return t * t * t;
}

float easeInOutCubic(float t) {
	if ((t *= 2.0) < 1.0) {
		return 0.5 * t * t * t;
	}
	else {
		return 0.5 * ((t -= 2.0) * t * t + 2.0);
	}
}

float easeInOutExpo(float t) {
	if (t == 0.0 || t == 1.0) {
		return t;
	}
	if ((t *= 2.0) < 1.0) {
		return 0.5 * pow(2.0, 10.0 * (t - 1.0));
	}
	else {
		return 0.5 * (-pow(2.0, -10.0 * (t - 1.0)) + 2.0);
	}
}

float linearstep(float begin, float end, float t) {
	return clamp((t - begin) / (end - begin), 0.0, 1.0);
}

float linearstepUpDown(float upBegin, float upEnd, float downBegin, float downEnd, float t) {
	return linearstep(upBegin, upEnd, t) - linearstep(downBegin, downEnd, t);
}

float stepUpDown(float begin, float end, float t) {
	return step(begin, t) - step(end, t);
}

float clockWipe(float2 p, float t) {
	float a = atan2(-p.x, -p.y);
	float v = (t * TWO_PI > a + PI) ? 1.0 : 0.0;
	return v;
}

float3 twist(float3 pos, float t)
{
	float st = sin(t);
	float ct = cos(t);
	float3 new_pos;
	new_pos.x = pos.x*ct - pos.z*st;
	new_pos.z = pos.x*st + pos.z*ct;
	new_pos.y = pos.y;
	return new_pos;
}

float3 rgb2hsv(float3 c) {
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 c) {
	c = float3(c.x, clamp(c.yz, 0.0, 1.0));
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float3x3 rotationAxisAngle(float3 v, float a)
{
	float si = sin(a);
	float co = cos(a);
	float ic = 1.0 - co;
	return float3x3(v.x*v.x*ic + co, v.y*v.x*ic - si*v.z, v.z*v.x*ic + si*v.y,
		v.x*v.y*ic + si*v.z, v.y*v.y*ic + co, v.z*v.y*ic - si*v.x,
		v.x*v.z*ic - si*v.y, v.y*v.z*ic + si*v.x, v.z*v.z*ic + co);
}
//http://planning.cs.uiuc.edu/node102.html
float3x3 eulerMatrix(float x, float y, float z)
{
	float sx = sin(x*(3.14125/180.0));
	float cx = cos(x*(3.14125/180.0));
	float sy = sin(y*(3.14125/180.0));
	float cy = cos(y*(3.14125/180.0));
	float sz = sin(z*(3.14125/180.0));
	float cz = cos(z*(3.14125/180.0));

	return float3x3(cx*cy, cx*sy*sz-sz*cz, cx*sy*cz+sx*sz,
		sz*cy, sx*sy*sz+cx*cz, sz*sy*cz-cx*sz,
		-sy, cy*sz, cy*cz);
}

float3x3 rotationAlign(in float3 d, in float3 z)
{
	float3 v = cross(z, d);
	float c = dot(z, d);
	float k = (1.0 - c) / (1.0 - c*c);
	return float3x3(
		v.x*v.x*k + c, v.y*v.x*k - v.z, v.z*v.x*k + v.y,
		v.x*v.y*k + v.z, v.y*v.y*k + c, v.z*v.y*k - v.x,
		v.x*v.z*k - v.y, v.y*v.z*k + v.x, v.z*v.z*k + c);
}
float4x4 rotationMatrix4(float3 x, float3 y, float3 z)
{
	///make sure vectors are normalized before hand
	return float4x4(
		x.x,x.y,x.z,0.0,
		y.x,y.y,y.z,0.0,
		z.x,z.y,z.z,0.0,
		0.0,0.0,0.0,1.0);
}
float3x3 rotationMatrix3(float3 x, float3 y, float3 z)
{
	return float3x3(
		x.x,x.y,x.z,
		y.x,y.y,y.z,
		z.x,z.y,z.z);
}

float3x3 getTangentBasis( in float3 tangent_y )
{
	float3 UpVector = float3( 0.001, 1, 0 );///added epsilon to avoid errors when incomming tagent happens to be 0,1,0
	float3 tangent_x = normalize( cross( UpVector, tangent_y ) );
	float3 tangent_z = cross( tangent_y, tangent_x );
	return float3x3( tangent_x, tangent_y, tangent_z );
}

////
float fit(float v, float l1, float h1, float l2, float h2){return l2 + (v - l1) * (h2 - l2) / (h1 - l1);}
float fitClamp(float v, float l1, float h1, float l2, float h2){return clamp(l2 + (v - l1) * (h2 - l2) / (h1 - l1),l2,h2);}
float bias(float t, float b) { return (t / ((((1.0 / b) - 2.0)*(1.0 - t)) + 1.0)); }
float gain(float t,float g) { return lerp(bias(t * 2.0,g)/2.0,bias(t * 2.0 - 1.0,1.0 - g)/2.0 + 0.5,round(t-0.5)); }
float luminance(float3 c){return dot(c, float3(0.2125, 0.7154, 0.0721));}

float remap( float value, float low1, float high1, float low2, float high2 )
{
	return low2 + ( value - low1 ) * ( high2 - low2 ) / ( high1 - low1 );
}

float remap( float value, float low2, float high2 )
{
	return low2 + value * ( high2 - low2 );
}

// IQ utility curve functions http://www.iquilezles.org/www/articles/functions/functions.htm

float almostIdentity( float x, float m, float n )
{
	if( x>m ) return x;

	float a = 2.0*n - m;
	float b = 2.0*m - 3.0*n;
	float t = x / m;

	return ( a*t + b )*t*t + n;
}


float impulse( float k, float x )
{
	float h = k*x;
	return h*exp( 1.0 - h );
}


float cubicPulse( float c, float w, float x )
{
	x = abs( x - c );
	if( x>w ) return 0.0;
	x /= w;
	return 1.0 - x*x*( 3.0 - 2.0*x );
}


float expStep( float x, float k, float n )
{
	return exp( -k*pow( x, n ) );
}


float iq_gain( float x, float k )
{
	float a = 0.5*pow( 2.0*( ( x < 0.5 ) ? x : 1.0 - x ), k );
	return ( x<0.5 ) ? a : 1.0 - a;
}


float parabola( float x, float k )
{
	return pow( 4.0*x*( 1.0 - x ), k );
}

float pcurve( float x, float a, float b )
{
	float k = pow( a + b, a + b ) / ( pow( a, a )*pow( b, b ) );
	return k * pow( x, a ) * pow( 1.0 - x, b );
}

