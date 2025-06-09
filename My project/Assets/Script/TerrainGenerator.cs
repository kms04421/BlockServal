using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : Singleton<TerrainGenerator>
{
  
    public GameObject terrainChunk;
    FastNoise noise = new FastNoise(); //FastNoise 
    public static int chunkDist = 5; //Chunk 생성거리
    List<ChunkPos> toGenerate = new List<ChunkPos>();
    ChunkPos curChunk = new ChunkPos(-1, -1);
    public static Dictionary<ChunkPos, TerrainChunk> chunks = new Dictionary<ChunkPos, TerrainChunk>();


    void BuildChunk(int xPos, int zPos)
    {
        TerrainChunk chunk;
        GameObject chunkGO = Instantiate(terrainChunk, new Vector3(xPos, 0, zPos), Quaternion.identity);
        chunk = chunkGO.GetComponent<TerrainChunk>();
        chunkGO.transform.parent = transform;
        for (int x = 0; x < TerrainChunk.chunkWidth + 2; x++)
        {
            for (int z = 0; z < TerrainChunk.chunkWidth + 2; z++)
            {
                for (int y = 0; y < TerrainChunk.chunkHeight; y++)
                {
                    chunk.blocks[x, y, z] = GetBlockType(xPos + x - 1, y, zPos + z - 1);
                    //Debug.Log(chunk.blocks[x, y, z]);

                }
            }
        }
        chunk.BuildMesh();
        chunks.Add(new ChunkPos(xPos, zPos), chunk);

    }
    BlockType GetBlockType(int x, int y, int z)
    {

        //Debug.Log(noise.GetSimplex(x, z));

        // Simplex 노이즈를 사용하여 부드러운 지형 변화를 생성
        // 전체적인 지형의 기복을 만드는 기본 노이즈
        float simplex1 = noise.GetSimplex(x * 0.8f, z * 0.8f) * 10;

        
     // (x * 3f, z * 3f)로 더 작은 스케일의 노이즈를 추가해서 세부 디테일한 지형 변화를 추가
     // (noise.GetSimplex(x * .3f, z * .3f) + .5f)으로 스케일의 강도를 조절해 지형 변화의 강도를 조절
        float simplex2 = noise.GetSimplex(x * 3f, z * 3f) * 10 * (noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

        // 기본 노이즈(simplex1) + 세부 디테일 노이즈(simplex2) = 최종 높이 맵
        float heightMap = simplex1 + simplex2;

        //TerrainChunk.chunkHeight * 0.5f = 기본 높이 chunkHeight에 추가
        float baseLandHeight = TerrainChunk.chunkHeight * 0.5f + heightMap;

        /*   //3d noise for caves and overhangs and such
           float caveNoise1 = noise.GetPerlinFractal(x * 5f, y * 10f, z * 5f); // PerlinFractal
           float caveMask = noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.3f;  // Simplex 
        */

        //stone layer heightmap
        /*   float simplexStone1 = noise.GetSimplex(x * 1f, z * 1f) * 10; //x * 1, z * 1 =  
           float simplexStone2 = (noise.GetSimplex(x * 5f, z * 5f) + .5f) * 20 * (noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

           float stoneHeightMap = simplexStone1 + simplexStone2;
           float baseStoneHeight = TerrainChunk.chunkHeight * .25f + stoneHeightMap;
   */


        BlockType blockType = BlockType.Air;

        //under the surface, dirt block
        if (y <= baseLandHeight)
        {
            blockType = BlockType.Dirt;

            //just on the surface, use a grass type
            if (y > baseLandHeight - 1)
                blockType = BlockType.Grass;

        }


        /*  if (caveNoise1 > Mathf.Max(caveMask, .2f)) // 
              blockType = BlockType.Air;*/

        return blockType;

    }
    // ������� 
    public void LoadChunks(Transform player, bool instant = false) //true   
    {
        int chunkWidth = TerrainChunk.chunkWidth;
        //  
        int curChunkPosX = Mathf.FloorToInt(player.position.x / chunkWidth) * chunkWidth; 
        int curChunkPosZ = Mathf.FloorToInt(player.position.z / chunkWidth) * chunkWidth;

        //curChunk x,z curChunkPosX,curChunkPosZ 
        if (curChunk.x != curChunkPosX || curChunk.z != curChunkPosZ)
        {
            curChunk.x = curChunkPosX;
            curChunk.z = curChunkPosZ;
            for (int i = curChunkPosX - chunkWidth * chunkDist; i <= curChunkPosX + chunkWidth * chunkDist; i += chunkWidth)
                for (int j = curChunkPosZ - chunkWidth * chunkDist; j <= curChunkPosZ + chunkWidth * chunkDist; j += chunkWidth)
                {
                    ChunkPos cp = new ChunkPos(i, j);
                    if (!chunks.ContainsKey(cp) && !toGenerate.Contains(cp))
                    {
                        if (instant)
                        {
                            BuildChunk(i, j);
                        }
                        else
                        {
                            toGenerate.Add(cp);
                        }
                    }
                }
            StartCoroutine(DelayBuildChunks());
        }

    }
    IEnumerator DelayBuildChunks() // 
    {
        while (toGenerate.Count > 0)
        {
            BuildChunk(toGenerate[0].x, toGenerate[0].z);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(0.01f);

        }

    }




}
public struct ChunkPos
{
    public int x;
    public int z;
    public ChunkPos(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public override bool Equals(object obj)
    {
        if (obj is ChunkPos other)
        {
            return this.x == other.x && this.z == other.z;
        }
        return false;
    }
}
