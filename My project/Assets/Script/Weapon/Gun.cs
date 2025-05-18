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

            ChunkPos hitChunkPos = WorldPositionHelper.WorldToChunkPos(hit.transform.position);
            if (!chunkPos.Equals(hitChunkPos))
            { 
                chunkPos = hitChunkPos;
            }
            Vector3Int hitPosition = WorldPositionHelper.GetIntBlockPosition(hit.point);
            Debug.Log($"{hit.transform.position.x}, {hit.transform.position.z}");       
            BlockType blocks = TerrainGenerator.chunks[chunkPos].blocks[hitPosition.x, hitPosition.y, hitPosition.z];
            if (blocks != BlockType.Air)
            {           
                Block.blocks[blocks] = new Block(Tile.Dirt);
                TerrainGenerator.chunks[chunkPos].BuildMesh();
            }

        }
        
    }
}
