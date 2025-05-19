using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IUsable
{
    public Camera cam; // 플레이어 카메라
    public float range = 100f;
    public float damage = 10f;
    public ParticleSystem muzzleFlash;
    private ChunkPos chunkPos;
  
    public void Use()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 화면 중앙
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {       
            chunkPos = WorldPositionHelper.WorldToChunkPos(hit.transform.position);           
            Vector3Int hitPosition = WorldPositionHelper.GetIntBlockPosition(hit.point);
            TerrainChunk terrainChunk = TerrainGenerator.chunks[chunkPos];
            if (terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z] == BlockType.Air)
                return;
            Debug.Log($"{terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z]}");          
           
            terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z] = BlockType.Air;
            TerrainGenerator.chunks[chunkPos].BuildMesh();
            

        }


    }
}
