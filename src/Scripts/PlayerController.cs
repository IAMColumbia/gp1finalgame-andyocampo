using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MouseInput input;
    Tile tile;
    private static Tile lastTileSelected = null; //the tile that the player will move
    public bool isPlaying;

    private void Awake()
    {
        input = GetComponent<MouseInput>();
        isPlaying = false;
    }

    void Update()
    {
        if(isPlaying)
        {
            TileController();
        }
    }

    /// <summary>
    /// Controls the tile movements
    /// </summary>
    private void TileController()
    {
        if (input.objectHit)
        {
            tile = input.objectClicked.GetComponent<Tile>();

            if (tile.GetComponent<SpriteRenderer>().sprite == MatchManager.Instance.matchedSprite || TileManager.Instance.isRefilling)
            { return; }

            if (tile.IsSelected) //if current tile is selected, deselect on click
            {
                DeselectTile(tile);
            }
            else
            {
                if (lastTileSelected == null) //if no tile has been selected, make this tile the first tile selected on click
                {
                    SelectTile(tile);
                }
                else
                {
                    tile.CalculateNeighbors(); //calculates neighbors of new chosen tile
                    if (tile.neighboringTiles.Contains(lastTileSelected)) //if last tile selected is a neighbor of the new chosen tile
                    {
                        SwapTiles(tile, lastTileSelected);
                        StartCoroutine(MatchManager.Instance.FindAllMatches());//checks if tile movement was match
                        DeselectTile(lastTileSelected);
                    }
                    else
                    {
                        DeselectTile(lastTileSelected);
                        SelectTile(tile);
                    }
                }
            }
        }
    }

    //makes tile selected and sets to last tile selected.
    public void SelectTile(Tile tile)
    {
        tile.IsSelected = true;
        tile.ApplySelectedColor();
        lastTileSelected = tile; //selected tile
    }

    //makes tile not selected and sets last tile selected to null.
    public void DeselectTile(Tile tile)
    {
        tile.IsSelected = false;
        tile.RemoveSelectedColor();
        lastTileSelected = null;
        tile.neighboringTiles.Clear();
    }

    //swaps sprites of two tiles
    public void SwapTiles(Tile tile, Tile otherTile)
    {
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        SpriteRenderer otherRenderer = otherTile.GetComponent<SpriteRenderer>();
        if (renderer.sprite == otherRenderer.sprite)
        { return; }

        Sprite newSprite = otherRenderer.sprite;
        otherRenderer.sprite = renderer.sprite;
        renderer.sprite = newSprite;
        GameManager.movesRemaining--;
    }
}
