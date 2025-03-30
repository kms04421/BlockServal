using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public const int chunkWidth = 16;
    public const int chunkHeight = 64;
    public BlockType[,,] blocks = new BlockType[chunkWidth + 2, chunkHeight, chunkWidth + 2];

    public void BuildMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int x = 1; x < chunkWidth + 1; x++)
            for (int z = 1; z < chunkWidth + 1; z++)
                for (int y = 0; y < chunkHeight; y++)
                {
                    int numFaces = 0; // 랜더링할 면의 갯수
                    if (blocks[x, y, z] != BlockType.Air)
                    {
                    
                        Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                        //각면이 공기인곳 찾아서 추가 
                        //위
                        if (y < chunkHeight - 1 && blocks[x, y + 1, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            numFaces++;

                            uvs.AddRange(GetTextureRect(1));
                        }
                        //아래
                        if (y > 0 && blocks[x, y - 1, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(2));
                        }


                        //오른쪽
                        if (blocks[x + 1, y, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        //앞
                        if (blocks[x, y, z - 1] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        //뒤
                        if (blocks[x, y, z + 1] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        //왼쪽
                        if (blocks[x - 1, y, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        int tl = verts.Count - 4 * numFaces; //한면당 verts를 4개씩 추가하니까 verts.Count - 4 * numFaces를 할경우 가장 처음 추가한 정점값나옴
                        for (int i = 0; i < numFaces; i++) //한면당 정점이 4씩 증가함 그면에 맟춰 삼각형 두개 배열값 생성해서 tris추가 
                        {
                            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                         
                        }
                    }

                }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
       
            // 텍스처 적용
        GetComponent<MeshRenderer>().material.mainTexture = SpriteAtlasManager.GetBlockSprite(0).texture;
        
    }
    private Vector2[] GetTextureRect(int i)
    {
        Sprite blockSprite = SpriteAtlasManager.GetBlockSprite(i);
        Rect textureRect = blockSprite.textureRect; // 상단 스프라이트
        Texture tex = blockSprite.texture;
        // 텍스처의 UV 좌표 계산
        float uvXMin = textureRect.x / tex.width;
        float uvXMax = (textureRect.x + textureRect.width) / tex.width;
        float uvYMin = textureRect.y / tex.height;
        float uvYMax = (textureRect.y + textureRect.height) / tex.height;

        // UV 좌표 반환
        return new Vector2[]
        {
           new Vector2(uvXMax, uvYMin), // 좌상
        new Vector2(uvXMax, uvYMax), // 우상
        new Vector2(uvXMin, uvYMax), // 우하
        new Vector2(uvXMin, uvYMin)  // 좌하
        };
    }

}
