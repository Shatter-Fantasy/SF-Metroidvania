#ifdef SF_SDF
#else 
#define SF_SDF

// Unity Shape Equivalents
float RoundedRectangle(float2 UV, float Width, float Height, float Radius)
{
    Radius = max(min(min(abs(Radius * 2), abs(Width)), abs(Height)), 1e-5);
    float2 uv = abs(UV * 2 - 1) - float2(Width, Height) + Radius;
    float d = length(max(0, uv)) / Radius;
    return saturate((1 - d) / fwidth(d));
}
#endif
