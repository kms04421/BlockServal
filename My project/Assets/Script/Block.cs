using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Tile top, side, bottom;
    public TileUV topTile, sideTile, bottmTile;
    
    public Block(Tile top)
    {
        this.top = top;
        this.side = top;
        this.bottom = top;
        SetTileUVs();
    }
    public Block(Tile top,Tile side,Tile bottom)
    {
        this.top = top;
        this.side = side;
        this.bottom = bottom;
        SetTileUVs();
    }
    public void SetTileUVs()
    {
        topTile = TileUV.tiles[top];
        sideTile = TileUV.tiles[side];
        bottmTile = TileUV.tiles[bottom];
    }

    public static Dictionary<BlockType, Block> blocks = new Dictionary<BlockType, Block>()
    {
        {BlockType.Grass,new Block(Tile.Grass,Tile.GrassSide,Tile.Dirt)},
        {BlockType.Dirt,new Block(Tile.Dirt) }

    };
}
public enum BlockType { Air, Grass, Dirt }