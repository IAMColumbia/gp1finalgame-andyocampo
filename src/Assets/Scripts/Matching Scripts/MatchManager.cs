using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private static MatchManager instance;
    public static MatchManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("MatchManager does not exist!");
            }

            return instance;
        }
    }

    private bool matchFound;

    private void Awake()
    {
        instance = this;
        matchFound = false;
    }

    public List<GameObject> matchesFound = new List<GameObject>();

    //starts coroutine when tiles are swapped
    public void FindMatches()
    {
        StartCoroutine(FindAllMatches());
    }

    //finds all matches
    private IEnumerator FindAllMatches()
    {
        int width = TileManager.Instance.xWidth;
        int height = TileManager.Instance.yHeight;

        yield return new WaitForSeconds(0.1f);
        for (int columns = 0; columns < width; columns++)
        {
            for (int rows = 0; rows < height; rows++)
            {
                GameObject currentTile = TileManager.Instance.allTiles[columns, rows].gameObject;
                Sprite currentTileSprite = currentTile.GetComponent<SpriteRenderer>().sprite;
                if (currentTile != null)
                {
                    CheckHorizontalMatches(width, columns, rows, currentTile, currentTileSprite);
                    CheckVerticalMatches(height, columns, rows, currentTile, currentTileSprite);
                }
            }
        }
    }

    //finds horizontal matches
    private void CheckHorizontalMatches(int width, int column, int row, GameObject currentTile, Sprite currentTileSprite)
    {
        if ((column > 0) && (column < width - 1))
        {
            GameObject leftTile = TileManager.Instance.allTiles[column - 1, row].gameObject;
            Sprite leftTileSprite = leftTile.GetComponent<SpriteRenderer>().sprite;
            GameObject rightTile = TileManager.Instance.allTiles[column + 1, row].gameObject;
            Sprite rightTileSprite = rightTile.GetComponent<SpriteRenderer>().sprite;

            if (leftTile != null && rightTile != null)
            {
                if (leftTileSprite == currentTileSprite && rightTileSprite == currentTileSprite)
                {
                    matchFound = true;
                    leftTile.GetComponent<Tile>().isMatched = true;
                    rightTile.GetComponent<Tile>().isMatched = true;
                    currentTile.GetComponent<Tile>().isMatched = true;

                    if (!matchesFound.Contains(leftTile))
                    { matchesFound.Add(leftTile); }

                    if (!matchesFound.Contains(rightTile))
                    { matchesFound.Add(rightTile); }

                    if (!matchesFound.Contains(currentTile))
                    { matchesFound.Add(currentTile); }

                    ClearMatch();
                }
            }
        }
    }

    //finds vertical Matches
    private void CheckVerticalMatches(int height, int column, int row, GameObject currentTile, Sprite currentTileSprite)
    {
        if ((row > 0) && (row < height - 1))
        {
            GameObject upTile = TileManager.Instance.allTiles[column, row + 1].gameObject;
            Sprite upTileSprite = upTile.GetComponent<SpriteRenderer>().sprite;
            GameObject downTile = TileManager.Instance.allTiles[column, row - 1].gameObject;
            Sprite downTileSprite = downTile.GetComponent<SpriteRenderer>().sprite;

            if (upTile != null && downTile != null)
            {
                if (upTileSprite == currentTileSprite && downTileSprite == currentTileSprite)
                {
                    matchFound = true;
                    upTile.GetComponent<Tile>().isMatched = true;
                    downTile.GetComponent<Tile>().isMatched = true;
                    currentTile.GetComponent<Tile>().isMatched = true;

                    if (!matchesFound.Contains(upTile))
                    { matchesFound.Add(upTile); }

                    if (!matchesFound.Contains(downTile))
                    { matchesFound.Add(downTile); }

                    if (!matchesFound.Contains(currentTile))
                    { matchesFound.Add(currentTile); }

                    ClearMatch();
                }
            }
        }
    }

    //clears match by setting matched sprites to null
    private void ClearMatch()
    {
        for (int i = 0; i < matchesFound.Count; i++)
        {
            matchesFound[i].GetComponent<SpriteRenderer>().sprite = null;
            matchesFound[i].GetComponent<Tile>().isMatched = false;
            Debug.Log(matchesFound[i].name);
        }
        matchesFound.Clear();
        StartCoroutine(TileManager.Instance.FindNullTiles());
    }

    //goes through the entire board to check if any other matches have been made
    public void ClearAllMatches()
    {
        if (matchFound)
        {
            matchFound = false;
            StopCoroutine(TileManager.Instance.FindNullTiles());
            StartCoroutine(TileManager.Instance.FindNullTiles());
        }
    }
}