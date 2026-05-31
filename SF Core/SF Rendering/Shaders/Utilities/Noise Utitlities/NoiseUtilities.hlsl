#ifdef SF_NOISES_UTILITIES
#else 
#define SF_NOISES_UTILITIES

inline uint Hash_Tchou_2_1_uint(uint2 v)
{
    // ~6 alu (2 mul)
    v.y ^= 1103515245U;
    v.x += v.y;
    v.x *= v.y;
    v.x ^= v.x >> 5u;
    v.x *= 0x27d4eb2du;
    return v.x;
}

inline float Hash_Tchou_2_1_float(float2 i)
{
    uint2 v = (uint2) (int2) round(i);
    uint r= Hash_Tchou_2_1_uint(v);
    return (r >> 8) * (1.0 / float(0x00ffffff));
}

inline float SimpleValueNoise_Deterministic (float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = Hash_Tchou_2_1_float(c0);
    float r1 = Hash_Tchou_2_1_float(c1);
    float r2 = Hash_Tchou_2_1_float(c2);
    float r3 = Hash_Tchou_2_1_float(c3);

    float bottomOfGrid = lerp(r0, r1, f.x);
    float topOfGrid = lerp(r2, r3, f.x);
    float t = lerp(bottomOfGrid, topOfGrid, f.y);
    return t;
}

float SimpleNoise(float2 UV, float Scale)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3-0));
    t += SimpleValueNoise_Deterministic(float2(UV.xy*(Scale/freq)))*amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3-1));
    t += SimpleValueNoise_Deterministic(float2(UV.xy*(Scale/freq)))*amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3-2));
    t += SimpleValueNoise_Deterministic(float2(UV.xy*(Scale/freq)))*amp;

    return t;
}
#endif