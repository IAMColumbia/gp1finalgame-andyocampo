using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int column, row;
    private bool isSelected;
    public bool isMatched;
    List<Tile> neighboringTiles = new List<Tile>();

    private SpriteRenderer spriteRenderer;
    private static Tile lastTileSelected = null; //the tile that the player will move

    private Color colorWhenSelected = new Color(.5f, .5f, .5f, 1); //color that is given to sprite when selected

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isMatched = false;
    }

    private void Start()
    {
        neighboringTiles = new List<Tile>();
        column = (int)this.transform.position.x;
        row = (int)this.transform.position.y;
    }

    private void Update()
    {
        if(isMatched)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.25f);
        }
        if(isSelected == true)
        {
            Debug.DrawRay((Vector2)this.transform.position, Vector2.up, Color.black);
            Debug.DrawRay((Vector2)this.transform.position, Vector2.down, Color.magenta);
            Debug.DrawRay((Vector2)this.transform.position, Vector2.left, Color.red);
            Debug.DrawRay((Vector2)this.transform.position, Vector2.right, Color.green);
        }
    }

    //applies selected color
    public void ApplySelectedColor()
    {
        spriteRenderer.color = colorWhenSelected;
    }

    //removes selected color
    public void RemoveSelectedColor()
    {
        spriteRenderer.color = Color.white;
    }

    //makes tile selected and sets to current tile selected.
    public void SelectTile()
    {
        isSelected = true;
        ApplySelectedColor();
        lastTileSelected = gameObject.GetComponent<Tile>(); //selected tile
        Debug.Log($"Current tile is {lastTileSelected.name}");
    }

    //makes tile not selected and sets current tile selected to null.
    public void DeselectTile()
    {
        isSelected = false;
        RemoveSelectedColor();
        lastTileSelected = null;
        neighboringTiles.Clear();
    }

    //calculates which neighbors are next to new chosen tile
    private void CalculateNeighbors()
    {
        if (this.column > 0)
            neighboringTiles.Add(TileManager.Instance.allTiles[column - 1, row]);
        if (this.column < TileManager.Instance.xWidth - 1)
            neighboringTiles.Add(TileManager.Instance.allTiles[column + 1, row]);
        if (this.row > 0)
            neighboringTiles.Add(TileManager.Instance.allTiles[column, row - 1]);
        if (this.row < TileManager.Instance.yHeight - 1)
            neighboringTiles.Add(TileManager.Instance.allTiles[column, row + 1]);
    }

    //swaps sprites
    public void SwapTiles(SpriteRenderer otherTileSprite)
    {
        if (spriteRenderer.sprite == otherTileSprite.sprite)
        { return; }

        Sprite newSprite = otherTileSprite.sprite;
        otherTileSprite.sprite = spriteRenderer.sprite;
        spriteRenderer.sprite = newSprite;
    }

    //when left mouse button is pressed over tile (will be moved to its own classed eventually)
    private void OnMouseDown()
    {
        if (spriteRenderer.sprite == null)
        { return; }

        if (isSelected) //if current tile is selected, deselect on click
        {
            DeselectTile();
        }
        else
        {
            if (lastTileSelected == null) //if no tile has been selected, make this tile the first tile selected on click
            {
                SelectTile();
                foreach (Tile go in neighboringTiles)
                {
                    Debug.Log(go.name);
                }
            }
            else
            {
                CalculateNeighbors(); //calculates neighbors of new chosen tile
                if (neighboringTiles.Contains(lastTileSelected)) //if last tile selected is a neighbor of the new chosen tile
                {
                    SwapTiles(lastTileSelected.spriteRenderer);
                    MatchManager.Instance.FindMatches(); //checks if tile movement was match
                    lastTileSelected.DeselectTile();
                }
                else
                {
                    lastTileSelected.DeselectTile();
                    SelectTile();
                }
            }
        }
    }
}
