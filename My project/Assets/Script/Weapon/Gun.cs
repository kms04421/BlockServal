using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : WeaponBase
{
    public float range = 100f; //사거리
    public float damage = 10f; //데미지
    public ParticleSystem muzzleFlash; //총구 불꽃

    private void Start()
    {
   
        base.Start();  // 부모 클래스의 Start() 실행 (카메라, 총구 위치 등 기본 초기화)
        
    }

    public override void Attack()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
        {
            HandleHit(hit);
        }
    }
     private void HandleHit(RaycastHit hit)
    {
        Vector3Int hitPosition = WorldPositionHelper.GetIntBlockPosition(hit.point);
        TerrainChunk terrainChunk = TerrainGenerator.chunks[WorldPositionHelper.WorldToChunkPos(hit.transform.position)];
        
        if (terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z] == BlockType.Air) 
            return;
        terrainChunk.blocks[hitPosition.x, hitPosition.y, hitPosition.z] = BlockType.Air;
        terrainChunk.BuildMesh(); //  UpdateChunks();
    }
}
