using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    public Transform player;
    public GameObject terrainChunk;
    FastNoise noise = new FastNoise(); //펄린노이즈를 빠르게 만들어주는 스크립트
    int chunkDist = 1; //플레이어 근방 생성할 청크
    List<ChunkPos> toGenerate = new List<ChunkPos>();
    ChunkPos curChunk = new ChunkPos(-1, -1);
    public static Dictionary<ChunkPos, TerrainChunk> chunks = new Dictionary<ChunkPos, TerrainChunk>();
    void Start()
    {
        LoadChunks();
      
    }
 
    void BuildChunk(int xPos, int zPos)
    {
        TerrainChunk chunk;
        GameObject chunkGO = Instantiate(terrainChunk, new Vector3(xPos, 0, zPos), Quaternion.identity);
        chunk = chunkGO.GetComponent<TerrainChunk>();
        chunkGO.transform.parent = transform;
        for (int x = 0; x < TerrainChunk.chunkWidth+2; x++)
        {
            for (int z = 0; z < TerrainChunk.chunkWidth+2; z++)
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

        //Simplex 노이즈를 사용하여 부드러운 높이 변화를 생성
        //노이즈의 스케일을 조정해서 넓은 언덕과 계곡을 형성
        float simplex1 = noise.GetSimplex(x * 0.8f, z * 0.8f) * 10;

        //(x * 3f, z * 3f) 좀 더 높은 주파수를 사용해서 작은 세부적인 지형 변화를 추가
        //(noise.GetSimplex(x * .3f, z * .3f) + .5f)낮은 주파수의 노이즈를 사용해 높이 변화의 강도를 조정
        float simplex2 = noise.GetSimplex(x * 3f, z * 3f) * 10 * (noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

        //큰 지형 변화(simplex1) + 세부적인 디테일(simplex2) = 최종 높이맵을 기반으로 지형의 높이를 결정
        float heightMap = simplex1 + simplex2;

        //TerrainChunk.chunkHeight * 0.5f = 지형의 기본 높이를 chunkHeight의 중간값으로 설정한값에 heightMap 값을 더해서 최종 높이를 결정.
        float baseLandHeight = TerrainChunk.chunkHeight * 0.5f + heightMap;

        /*   //3d noise for caves and overhangs and such
           float caveNoise1 = noise.GetPerlinFractal(x * 5f, y * 10f, z * 5f); // PerlinFractal을 사용하여 동굴 생성
           float caveMask = noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.3f;  // Simplex 노이즈를 사용해 특정높이에서만 동굴이 생성되도록 설정 하단에 조건존재
        */

        //stone layer heightmap
     /*   float simplexStone1 = noise.GetSimplex(x * 1f, z * 1f) * 10; //x * 1, z * 1 =  저주파 노이즈를 사용하여 기본적인 암석층 형성.
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


      /*  if (caveNoise1 > Mathf.Max(caveMask, .2f)) // 동굴
            blockType = BlockType.Air;*/

        return blockType;

    }
    // 월드생성 
    void LoadChunks(bool instant = false) //true면 바로 생성
    {
        int chunkWidth = TerrainChunk.chunkWidth;
        //현재 플레이어 위치를 기준의 청크값 x,z값 가져옴
        int curChunkPosX = Mathf.FloorToInt(player.position.x / chunkWidth) * chunkWidth; // 플레이어 위치 기준 소수를 내림하여 위치값 설정
        int curChunkPosZ = Mathf.FloorToInt(player.position.z / chunkWidth) * chunkWidth;
      
        //curChunk에 x,z값과 curChunkPosX,curChunkPosZ 값이 일치 하지 않으면 
        if (curChunk.x != curChunkPosX || curChunk.z != curChunkPosZ)
        {
            curChunk.x = curChunkPosX;
            curChunk.z = curChunkPosZ;
            // 플레이어 근처 블럭 생성 
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
    IEnumerator DelayBuildChunks() // 프레임 드랍 방지용 한프레임 내에서 한번에 생성되는거 방지
    {
        while (toGenerate.Count > 0)
        {
            BuildChunk(toGenerate[0].x, toGenerate[0].z);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(0.1f);

        }

    }




}
public struct ChunkPos
{
    public int x;
    public int z;
    public ChunkPos(int x,int z)
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
