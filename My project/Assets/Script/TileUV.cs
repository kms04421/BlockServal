using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUV 
{
    Vector2[] uvs;

    public Vector2[] GetUvs() => uvs;
  
    public TileUV(Tile tile)
    {
        uvs = GetTextureRect((int)tile);
    }
    public Vector2[] GetTextureRect(int i) // 스프라이트 아틀라스에서 가져온 텍스쳐 uv설정
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
    public static Dictionary<Tile, TileUV> tiles = new Dictionary<Tile, TileUV>()
    {
        {Tile.Grass,new TileUV(Tile.Grass)},
        {Tile.Dirt,new TileUV(Tile.Dirt)},
        {Tile.GrassSide,new TileUV(Tile.GrassSide)}
    };
}
public enum Tile {GrassSide, Grass, Dirt}