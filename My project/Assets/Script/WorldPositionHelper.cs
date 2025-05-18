using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class WorldPositionHelper
{
    public static Vector3Int GetChunkPosition(Vector3 pos)
    {
        Vector3Int Pos = new Vector3Int((Mathf.FloorToInt(pos.x / 16) * 16), Mathf.FloorToInt(pos.y) - 1, Mathf.FloorToInt(pos.z / 16) * 16);
        return Pos;
    }

    public static Vector3Int GetIntBlockPosition(Vector3 pos)
    {
        int chunkStartX = Mathf.FloorToInt(pos.x / 16f) * 16;
        int chunkStartZ = Mathf.FloorToInt(pos.z / 16f) * 16;

        int localX = Mathf.FloorToInt(pos.x) - chunkStartX + 1;
        int localY = Mathf.FloorToInt(pos.y);
        int localZ = Mathf.FloorToInt(pos.z) - chunkStartZ + 1;

        return new Vector3Int(localX, localY, localZ);
    }

    public static Vector3Int LocalToWorld(Vector3Int blockPostion, Vector3Int chunkWorldPos)
    {
        return new Vector3Int(
            chunkWorldPos.x + blockPostion.x - 1,
            blockPostion.y,
            chunkWorldPos.z + blockPostion.z - 1
        );
    }
    public static Vector3Int LocalToWorld(Vector3Int blockPostion, ChunkPos chunkWorldPos)
    {
        return new Vector3Int(
            chunkWorldPos.x + blockPostion.x - 1,
            blockPostion.y,
            chunkWorldPos.z + blockPostion.z - 1
        );
    }
    public static Vector3Int Vector3ToVector3Int(Vector3 vector)
    {
        return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
    }
    public static ChunkPos WorldToChunkPos(Vector3 worldPos)
    {
        
        int x = Mathf.FloorToInt(worldPos.x );
        int z = Mathf.FloorToInt(worldPos.z );
        return new ChunkPos(x, z);
    }
}
