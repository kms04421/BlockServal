using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : WeaponBase
{
    public float range = 100f; //사거리
    public float damage = 10f; //데미지   
    public ParticleSystem muzzleFlash; //총구 불꽃

    public override void Attack()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
        {
            switch (hit.collider.gameObject.layer)
            {
                case LayerName.Monster:
                    HandleMonsterHit(hit);
                    break;
                    
                case LayerName.Ground:
                    HandleGroundHit(hit);
                    break;
                    
                default:
                    Debug.Log($"Hit unhandled layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                    break;
            }
        }
    }
     private void HandleGroundHit(RaycastHit hit)
    {
        Vector3Int hitPosition = WorldPositionHelper.GetIntBlockPosition(hit.point);
        TerrainChunk terrainChunk = TerrainGenerator.chunks[WorldPositionHelper.WorldToChunkPos(hit.transform.position)];
        
        if (terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z] == BlockType.Air) 
            return;
        terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z] = BlockType.Air;
        terrainChunk.BuildMesh(); //  UpdateChunks();
    }
    private void HandleMonsterHit(RaycastHit hit)
    {
        // 몬스터 피격 처리
        Monster monster = hit.transform.root.GetComponent<Monster>();
        Debug.Log("Hit "+monster.name);
        if (monster != null)
        {
            monster.Hit(GetDamage());
        }
    }

}
