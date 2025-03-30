using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainChunk;
    FastNoise noise = new FastNoise(); //펄린노이즈를 빠르게 만들어주는 스크립트

    void Start()
    {
        BulidChunk(10,10);
    }

    void BulidChunk(int xPos, int zPos)
    {
        TerrainChunk chunk;
        GameObject chunkGO = Instantiate(terrainChunk, new Vector3(xPos, 0, zPos), Quaternion.identity);
        chunk = chunkGO.GetComponent<TerrainChunk>();
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

}
