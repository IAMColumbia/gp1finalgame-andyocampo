using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchState {Default, Matched}
public class Tile : MonoBehaviour
{
    private int column, row;
    private bool isSelected;
    public bool IsSelected
    { get { return isSelected; } set { isSelected = value; } }

    private MatchState matchState;
    public MatchState MatchState 
    { get { return matchState; } set { matchState = value; } }

    public List<Tile> neighboringTiles;
    private SpriteRenderer spriteRenderer;

    private Color colorWhenSelected = new Color(.5f, .5f, .5f, 1); //color that is given to sprite when selected

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        matchState = MatchState.Default;
    }

    private void Start()
    {
        neighboringTiles = new List<Tile>();
        column = (int)this.transform.position.x;
        row = (int)this.transform.position.y;
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

    /// <summary>
    /// Calculates the adjacent tiles next to the current tile.
    /// </summary>
    public void CalculateNeighbors()
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
}
