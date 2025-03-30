using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainChunk;
    FastNoise noise = new FastNoise(); //�޸������ ������ ������ִ� ��ũ��Ʈ

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

        //Simplex ����� ����Ͽ� �ε巯�� ���� ��ȭ�� ����
        //�������� �������� �����ؼ� ���� ����� ����� ����
        float simplex1 = noise.GetSimplex(x * 0.8f, z * 0.8f) * 10;

        //(x * 3f, z * 3f) �� �� ���� ���ļ��� ����ؼ� ���� �������� ���� ��ȭ�� �߰�
        //(noise.GetSimplex(x * .3f, z * .3f) + .5f)���� ���ļ��� ����� ����� ���� ��ȭ�� ������ ����
        float simplex2 = noise.GetSimplex(x * 3f, z * 3f) * 10 * (noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

        //ū ���� ��ȭ(simplex1) + �������� ������(simplex2) = ���� ���̸��� ������� ������ ���̸� ����
        float heightMap = simplex1 + simplex2;

        //TerrainChunk.chunkHeight * 0.5f = ������ �⺻ ���̸� chunkHeight�� �߰������� �����Ѱ��� heightMap ���� ���ؼ� ���� ���̸� ����.
        float baseLandHeight = TerrainChunk.chunkHeight * 0.5f + heightMap;

        /*   //3d noise for caves and overhangs and such
           float caveNoise1 = noise.GetPerlinFractal(x * 5f, y * 10f, z * 5f); // PerlinFractal�� ����Ͽ� ���� ����
           float caveMask = noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.3f;  // Simplex ����� ����� Ư�����̿����� ������ �����ǵ��� ���� �ϴܿ� ��������
        */

        //stone layer heightmap
     /*   float simplexStone1 = noise.GetSimplex(x * 1f, z * 1f) * 10; //x * 1, z * 1 =  ������ ����� ����Ͽ� �⺻���� �ϼ��� ����.
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


      /*  if (caveNoise1 > Mathf.Max(caveMask, .2f)) // ����
            blockType = BlockType.Air;*/

        return blockType;

    }

}
