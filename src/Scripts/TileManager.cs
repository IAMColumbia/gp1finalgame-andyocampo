using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager instance;
    public static TileManager Instance 
    { 
        get
        {
            if (instance == null)
            {
                Debug.LogError("TileManager does not exist!");
            }

            return instance;
        }
    }
    public Tile[,] allTiles;
    public int xWidth;  //amount of columns of tiles
    public int yHeight; //amount of rows of tiles
    public bool isRefilling; //if the board is refilling
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Sprite[] tileSpriteTypes;
    SpriteRenderer newTileRenderer;

    private void Awake()
    {
        instance = this;
        allTiles = new Tile[xWidth, yHeight];
        CreateLevel();
    }

    /// <summary>
    /// Creates a randomized level with no matches.
    /// </summary>
    private void CreateLevel()
    {
        Sprite[] previousTileBelow = new Sprite[xWidth];
        Sprite previousTileLeft = null; 

        for (int y = 0; y < yHeight; y++)
        {
            for (int x = 0; x < xWidth; x++)
            {
                RandomizeTile(x, y);

                //checks whether a match was found during level creation
                List<Sprite> possibleTiles = new List<Sprite>();
                possibleTiles.AddRange(tileSpriteTypes);

                possibleTiles.Remove(previousTileBelow[x]);
                possibleTiles.Remove(previousTileLeft);

                Sprite newSprite = possibleTiles[Random.Range(0, possibleTiles.Count)];
                newTileRenderer.sprite = newSprite;
                previousTileBelow[x] = newSprite;
                previousTileLeft = newSprite;
            }
        }
    }

    /// <summary>
    /// Randomizes tile sprite.
    /// </summary>
    /// <param name="xPos">X position of tile.</param>
    /// <param name="yPos">Y position of tile.</param>
    private void RandomizeTile(int xPos, int yPos)
    {
        Vector2 startPosition = new Vector2(xPos, yPos);
        GameObject tile = Instantiate(tilePrefab, startPosition, Quaternion.identity, this.transform);

        Sprite randomTileSprite = tileSpriteTypes[Random.Range(0, tileSpriteTypes.Length)];
        tile.GetComponent<SpriteRenderer>().sprite = randomTileSprite;
        tile.name = tile.name.Replace("(Clone)", $"({xPos},{yPos})");
        allTiles[xPos, yPos] = tile.GetComponent<Tile>();
        newTileRenderer = tile.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Finds all the tiles in the board that have been matched.
    /// </summary>
    /// <returns>Refill board coroutine</returns>
    public IEnumerator FindMatchedTiles()
    {
        for (int x = 0; x < xWidth; x++)
        {
            for (int y = 0; y < yHeight; y++)
            {
                if (allTiles[x, y].GetComponent<SpriteRenderer>().sprite == MatchManager.Instance.matchedSprite)
                {
                    yield return StartCoroutine(RefillTiles(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < xWidth; x++)
        {
            for (int y = 0; y < yHeight; y++)
            {
                MatchManager.Instance.ClearAllMatches(allTiles[x,y]);
            }
        }
    }

    /// <summary>
    /// Refills tiles that have matching Sprite.
    /// </summary>
    /// <param name="x">X position of tile.</param>
    /// <param name="yStart">Y position from where tile was matched.</param>
    /// <returns></returns>
    private IEnumerator RefillTiles(int x, int yStart)
    {
        StopCoroutine(MatchManager.Instance.FindAllMatches());
        StopCoroutine(FindMatchedTiles());
        isRefilling = true;
        //Debug.Log("is Reflling");
        List<SpriteRenderer> renderer = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < yHeight; y++)
        {
            SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == MatchManager.Instance.matchedSprite)
            {
                nullCount++;
            }
            renderer.Add(spriteRenderer);
        }
        

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(0.3f);
            Debug.Log(renderer.Count);
            if(renderer.Count == 1)
            {
                renderer[0].sprite = GetNewSprite(x, yHeight - 1);
            }
            else
            {
                for (int j = 0; j < renderer.Count - 1; j++)
                {
                    //Debug.Log(j);
                    renderer[j].sprite = renderer[j + 1].sprite;
                    renderer[j + 1].sprite = GetNewSprite(x, yHeight - 1);
                }
            }
        }
        StartCoroutine(MatchManager.Instance.FindAllMatches());
        isRefilling = false;
    }

    /// <summary>
    /// Gets a new sprite to refill.
    /// </summary>
    /// <param name="x">X position of tile.</param>
    /// <param name="y">Y position of tile.</param>
    /// <returns>New sprite.</returns>
    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> tileSprites = new List<Sprite>();
        tileSprites.AddRange(tileSpriteTypes);

        if (x > 0)
        {
            tileSprites.Remove(allTiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xWidth - 1)
        {
            tileSprites.Remove(allTiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            tileSprites.Remove(allTiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return tileSprites[Random.Range(0, tileSprites.Count)];
    }
}