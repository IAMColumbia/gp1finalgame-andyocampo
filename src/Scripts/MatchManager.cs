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

    private bool matchFound; //if a match is found return true

    [Tooltip("The sprite tiles change to when matched.")]
    public Sprite matchedSprite;

    public List<Tile> matchesFound; //list of matching tiles

    private void Awake()
    {
        instance = this;
        matchFound = false;
        matchesFound = new List<Tile>();
    }

    /// <summary>
    /// Finds all the matches on the board.
    /// </summary>
    /// <returns>One second wait to calculate matches.</returns>
    public IEnumerator FindAllMatches()
    {
        int width = TileManager.Instance.xWidth;
        int height = TileManager.Instance.yHeight;
        bool foundHorizontal;
        bool foundVertical;

        
        for (int columns = 0; columns < width; columns++)
        {
            for (int rows = 0; rows < height; rows++)
            {
                Tile currentTile = TileManager.Instance.allTiles[columns, rows];
                Sprite currentTileSprite = currentTile.GetComponent<SpriteRenderer>().sprite;
                if (currentTileSprite != matchedSprite)
                {
                    foundHorizontal = CheckHorizontalMatches(width, columns, rows, currentTile, currentTileSprite);
                    foundVertical = CheckVerticalMatches(height, columns, rows, currentTile, currentTileSprite);
                    if (foundHorizontal || foundVertical)
                    {
                        matchFound = true;
                        yield return new WaitForSeconds(1f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Locates horizontal sprites that are equal to the currently moved sprite.
    /// </summary>
    /// <param name="width">The width of the board of tiles.</param>
    /// <param name="column">The column of the current tile.</param>
    /// <param name="row">The row of the current tile.</param>
    /// <param name="currentTile">The current tile.</param>
    /// <param name="currentTileSprite">The sprite of the current tile.</param>
    /// <returns></returns>
    private bool CheckHorizontalMatches(int width, int column, int row, Tile currentTile, Sprite currentTileSprite)
    {
        //search left
        int leftBound = -1;

        while (column + leftBound >= 0 && TileManager.Instance.allTiles[column + leftBound, row].GetComponent<SpriteRenderer>().sprite == currentTileSprite)
        {
            TileManager.Instance.allTiles[column + leftBound, row].MatchState = MatchState.Matched;
            currentTile.MatchState = MatchState.Matched;
            if (!matchesFound.Contains(TileManager.Instance.allTiles[column + leftBound, row]))
            { matchesFound.Add(TileManager.Instance.allTiles[column + leftBound, row]); }

            leftBound--;
        }

        //search right
        int rightBound = 1;
        while (column + rightBound < width && TileManager.Instance.allTiles[column + rightBound, row].GetComponent<SpriteRenderer>().sprite == currentTileSprite)
        {
            TileManager.Instance.allTiles[column + rightBound, row].MatchState = MatchState.Matched;
            currentTile.MatchState = MatchState.Matched;
            if (!matchesFound.Contains(TileManager.Instance.allTiles[column + rightBound, row]))
            { matchesFound.Add(TileManager.Instance.allTiles[column + rightBound, row]); }

            rightBound++;
        }

        rightBound--;
        leftBound++;

        if (rightBound - leftBound >= 2)
        {
            Debug.Log(rightBound - leftBound);
            if(rightBound - leftBound >= 3)
            { GameManager.timeLeft += 3; }
            else { GameManager.timeLeft += 2; }

            matchFound = true;
            if (!matchesFound.Contains(currentTile))
            { matchesFound.Add(currentTile); }

            ClearMatch();
        }
        else
        {
            matchesFound.Clear();
        }

        return matchFound;
    }

    /// <summary>
    /// Locates vertical sprites that are equal to the currently moved sprite.
    /// </summary>
    /// <param name="height">The height of the board of tiles.</param>
    /// <param name="column">The column of the current tile.</param>
    /// <param name="row">The row of the current tile.</param>
    /// <param name="currentTile">The current tile.</param>
    /// <param name="currentTileSprite">The sprite of the current tile.</param>
    /// <returns></returns>
    private bool CheckVerticalMatches(int height, int column, int row, Tile currentTile, Sprite currentTileSprite)
    {
        //search down
        int downBound = -1;
        while (row + downBound >= 0 && TileManager.Instance.allTiles[column, row + downBound].GetComponent<SpriteRenderer>().sprite == currentTileSprite)
        {
            TileManager.Instance.allTiles[column, row + downBound].MatchState = MatchState.Matched;
            currentTile.MatchState = MatchState.Matched;
            if (!matchesFound.Contains(TileManager.Instance.allTiles[column, row + downBound]))
            { matchesFound.Add(TileManager.Instance.allTiles[column, row + downBound]); }

            downBound--;
        }

        //search up
        int upBound = 1;
        while (row + upBound < height && TileManager.Instance.allTiles[column, row + upBound].GetComponent<SpriteRenderer>().sprite == currentTileSprite)
        {
            TileManager.Instance.allTiles[column, row + upBound].MatchState = MatchState.Matched;
            currentTile.MatchState = MatchState.Matched;
            if (!matchesFound.Contains(TileManager.Instance.allTiles[column, row + upBound]))
            { matchesFound.Add(TileManager.Instance.allTiles[column, row + upBound]); }

            upBound++;
        }

        upBound--;
        downBound++;

        if (upBound - downBound >= 2)
        {
            Debug.Log(upBound - downBound);
            if (upBound - downBound >= 3)
            { GameManager.timeLeft += 3; }
            else { GameManager.timeLeft += 2; }

            matchFound = true;
            if (!matchesFound.Contains(currentTile))
            { matchesFound.Add(currentTile); }
            ClearMatch();
        }
        else
        {
            matchesFound.Clear();
        }

        return matchFound;
    }

    /// <summary>
    /// Replaces tile sprites to matched sprite and clears match.
    /// </summary>
    private void ClearMatch()
    {
        for (int i = 0; i < matchesFound.Count; i++)
        {
            matchesFound[i].GetComponent<SpriteRenderer>().sprite = matchedSprite;
            matchesFound[i].MatchState = MatchState.Default;
            //Debug.Log(matchesFound[i].name);
        }
        matchesFound.Clear();
        StartCoroutine(TileManager.Instance.FindMatchedTiles());
    }

    /// <summary>
    /// Clears all matches on board.
    /// </summary>
    /// <param name="tile">Current tile in board.</param>
    public void ClearAllMatches(Tile tile)
    {
        if(tile.GetComponent<SpriteRenderer>().sprite == matchedSprite)
        { return; }

        if (matchFound)
        {
            GameManager.matchGoal--;
            matchFound = false;
            StopCoroutine(TileManager.Instance.FindMatchedTiles());
        }
    }
}