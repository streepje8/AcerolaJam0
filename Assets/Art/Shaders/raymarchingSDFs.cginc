//Referenced this wonderful page for help on the basics SDF's because i keep forgetting them
// https://iquilezles.org/articles/distfunctions/

//SDFS from iquellez
float sdSphere(float3 p, float s )
{
	return length(p)-s;
}

float sdBox(float3 p, float3 b )
{
	float3 q = abs(p) - b;
	return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float sdRoundBox(float3 p, float3 b, float r )
{
	float3 q = abs(p) - b + r;
	return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
}

float sdBoxFrame(float3 p, float3 b, float e )
{
	p = abs(p  )-b;
	float3 q = abs(p+e)-e;
	return min(min(
		length(max(float3(p.x,q.y,q.z),0.0))+min(max(p.x,max(q.y,q.z)),0.0),
		length(max(float3(q.x,p.y,q.z),0.0))+min(max(q.x,max(p.y,q.z)),0.0)),
		length(max(float3(q.x,q.y,p.z),0.0))+min(max(q.x,max(q.y,p.z)),0.0));
}

float sdTorus(float3 p, float2 t )
{
	float2 q = float2(length(p.xz)-t.x,p.y);
	return length(q)-t.y;
}

float sdCone(float3 p, float2 c, float h )
{
  float q = length(p.xz);
  return max(dot(c.xy,float2(q,p.y)),-h-p.y);
}

float sdOctahedron(float3 p, float s)
{
  p = abs(p);
  return (p.x+p.y+p.z-s)*0.57735027;
}

//Combiners from iquellez
float opUnion( float d1, float d2 )
{
	return min(d1,d2);
}
float opSubtraction( float d1, float d2 )
{
	return max(-d1,d2);
}
float opIntersection( float d1, float d2 )
{
	return max(d1,d2);
}
float opXor(float d1, float d2 )
{
	return max(min(d1,d2),-max(d1,d2));
}

float opSmoothUnion( float d1, float d2, float k )
{
	float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
	return lerp( d2, d1, h ) - k*h*(1.0-h);
}

float opSmoothSubtraction( float d1, float d2, float k )
{
	float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
	return lerp( d2, -d1, h ) + k*h*(1.0-h);
}

float opSmoothIntersection( float d1, float d2, float k )
{
	float h = clamp( 0.5 - 0.5*(d2-d1)/k, 0.0, 1.0 );
	return lerp( d2, d1, h ) + k*h*(1.0-h);
}


//P Affectors
float3 opRepetition(float3 p, float3 s)
{
	float3 q = p - s*round(p/s);
	return q;
}

float3 opLimitedRepetition(float3 p, float s, float3 l)
{
	float3 q = p - s*clamp(round(p/s),-l,l);
	return q;
}

float3 opTwistX(float3 p, float k)
{
	float c = cos(k*p.y);
	float s = sin(k*p.y);
	float2x2 m = float2x2(c,-s,s,c);
	float3 q = float3(p.x,mul(m,p.yz));
	return q;
}

float3 opTwistY(float3 p, float k)
{
	float c = cos(k*p.y);
	float s = sin(k*p.y);
	float2x2 m = float2x2(c,-s,s,c);
	float3 q = float3(mul(m,p.xz),p.y);
	return q;
}

float3 opTwistZ(float3 p, float k)
{
	float c = cos(k*p.y);
	float s = sin(k*p.y);
	float2x2 m = float2x2(c,-s,s,c);
	float3 q = float3(mul(m,p.yx),p.z);
	return q;
}

float3 opCheapBend(float3 p, float k)
{
	float c = cos(k*p.x);
	float s = sin(k*p.x);
	float2x2 m = float2x2(c,-s,s,c);
	float3 q = float3(mul(m,p.xy),p.z);
	return q;
}