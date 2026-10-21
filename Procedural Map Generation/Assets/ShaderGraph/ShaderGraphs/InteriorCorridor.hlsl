#ifndef INTERIOR_CORRIDOR_INCLUDED
#define INTERIOR_CORRIDOR_INCLUDED

void InteriorSetup(float3 ViewDirWorldSpace, float2 UV, out float3 right, out float3 up, out float3 intoWall, out float3 ro, out float3 rd)
{
    right    = normalize(UNITY_MATRIX_M._m00_m10_m20);
    up       = normalize(UNITY_MATRIX_M._m01_m11_m21);
    intoWall = normalize(UNITY_MATRIX_M._m02_m12_m22);

    float2 doorSize = float2(length(UNITY_MATRIX_M._m00_m10_m20), length(UNITY_MATRIX_M._m01_m11_m21));

    float3 camToSurface = -normalize(ViewDirWorldSpace);
    rd = float3(dot(camToSurface, right),dot(camToSurface, up),dot(camToSurface, intoWall));
    ro = float3((UV - 0.5) * doorSize, 0.0);   // start on the opening
}

void InteriorCorridor_float( float3 ViewDirWorldSpace,float2 UV,float3 RoomSize,float RoomOffsetY,out float2 HitUV,out float3 NormalRoomSpace,out float Dist,out float3 NormalWorldSpace)
{
    float3 right, up, intoWall, ro, rd;
    InteriorSetup(ViewDirWorldSpace, UV, right, up, intoWall, ro, rd);

    float2 roomHalfSize = RoomSize.xy * 0.5;
    float roomDepth = RoomSize.z;
    float floorY   = -roomHalfSize.y + RoomOffsetY;
    float ceilingY =  roomHalfSize.y + RoomOffsetY;

    float safeDirX = rd.x >= 0 ? max(rd.x, 1e-5) : min(rd.x, -1e-5);
    float safeDirY = rd.y >= 0 ? max(rd.y, 1e-5) : min(rd.y, -1e-5);
    float safeDirZ = max(rd.z, 1e-5);


    float tSideWall  = ((rd.x >= 0 ? roomHalfSize.x : -roomHalfSize.x) - ro.x) / safeDirX;
    float tFloorCeil = ((rd.y >= 0 ? ceilingY : floorY) - ro.y) / safeDirY;
    float tBackWall  = roomDepth / safeDirZ;

    float tNearest = min(min(tSideWall, tFloorCeil), tBackWall);
    float3 hitPos = ro + rd * tNearest;

    if (tBackWall <= tSideWall && tBackWall <= tFloorCeil)   // back wall
    {
        HitUV = hitPos.xy;
        NormalRoomSpace = float3(0, 0, -1);
    }
    else if (tSideWall <= tFloorCeil)                        // side wall
    {
        HitUV = hitPos.zy;
        NormalRoomSpace = float3(rd.x >= 0 ? -1 : 1, 0, 0);
    }
    else                                                     // floor or ceiling
    {
        HitUV = hitPos.xz;
        NormalRoomSpace = float3(0, rd.y >= 0 ? -1 : 1, 0);
    }
    NormalWorldSpace = NormalRoomSpace.x * right + NormalRoomSpace.y * up + NormalRoomSpace.z * intoWall;
    Dist = tNearest;
}

void InteriorPipe_float(float3 ViewDirWorldSpace,float2 UV,float3 RoomSize,float RoomOffsetY,out float2 HitUV,out float3 NormalRoomSpace, out float Dist,out float3 NormalWorldSpace,out float Mask)
{
    float3 right, up, intoWall, ro, rd;
    InteriorSetup(ViewDirWorldSpace, UV, right, up, intoWall, ro, rd);

    float radiusX = max(RoomSize.x * 0.5, 1e-4);
    float radiusY = max(RoomSize.y * 0.5, 1e-4);
    float roomDepth = RoomSize.z;
    
    float2 radii = float2(radiusX, radiusY);
    float2 originCircle = float2(ro.x, ro.y - RoomOffsetY) / radii;
    float2 dirCircle    = rd.xy / radii;

    float distFromAxis = length(originCircle);   // 1.0 = exactly on the pipe wall
    Mask = 1.0 - step(1.0, distFromAxis);        // 1 inside the ellipse, 0 in the corners
    if (distFromAxis > 1.0)
    {
        originCircle /= distFromAxis;
        ro.xy = originCircle * radii + float2(0.0, RoomOffsetY);
    }
    
    float A = max(dot(dirCircle, dirCircle), 1e-8);
    float B = dot(originCircle, dirCircle);
    float C = dot(originCircle, originCircle) - 1.0;   // <= 0: we start inside the pipe
    
    float tPipeWall = (-B + sqrt(max(B * B - A * C, 0.0))) / A;
    float tBackCap = roomDepth / max(rd.z, 1e-5);
    float tNearest = min(tPipeWall, tBackCap);
    float3 hitPos = ro + rd * tNearest;

    if (tBackCap <= tPipeWall)               // flat back cap
    {
        HitUV = hitPos.xy;
        NormalRoomSpace = float3(0, 0, -1);
    }
    else                                     // curved wall
    {
        float2 radialPos = float2(hitPos.x, hitPos.y - RoomOffsetY);
        float angle = atan2(radialPos.x, radialPos.y);

        HitUV = float2(angle * (radiusX + radiusY) * 0.5, hitPos.z);
        
        NormalRoomSpace = -normalize(float3(radialPos.x / (radiusX * radiusX),radialPos.y / (radiusY * radiusY), 0.0));
    }
    NormalWorldSpace = NormalRoomSpace.x * right + NormalRoomSpace.y * up + NormalRoomSpace.z * intoWall;
    Dist = tNearest;
}
#endif
