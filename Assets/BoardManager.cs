using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public enum TileType { Dirt, Water, Pasture, Desert, Grain, Rock, Woods }

    public GameObject[] tilePrefab;
    public GameObject treePrefab;
    public GameObject boatPrefab;
    public GameObject cowPrefab;
    public GameObject cactusPrefab;
    public GameObject wheatPrefab;
    public GameObject rocksPrefab;
    public GameObject housePrefab;

    public Text Score;

    public List<GameObject> spawnedTiles = new List<GameObject>();
    Dictionary<TileType, long> bitboards = new Dictionary<TileType, long>();

    void Awake()
    {
        foreach (var t in Enum.GetValues(typeof(TileType)))
        {
            bitboards[(TileType)t] = 0;
        }
    }

    public void CreateBoard()
    {
        DeleteBoard();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var index = UnityEngine.Random.Range(0, tilePrefab.Length);
                var pos = new Vector3(j, 0, i);
                var go = Instantiate(tilePrefab[index], pos, Quaternion.identity);
                go.name = tilePrefab[index].tag + i + j;
                spawnedTiles.Add(go);

                TileType tt;
                if (Enum.TryParse(go.tag, out tt))
                {
                    bitboards[tt] = SetBit(bitboards[tt], i, j);
                    Debug.Log("BB: " + tt + " = " + Convert.ToString(bitboards[tt], 2));
                }
            }
        }

        Debug.Log("Dirt = " + CountBits(bitboards[TileType.Dirt]));
        Debug.Log("Water = " + CountBits(bitboards[TileType.Water]));
        Debug.Log("Pasture = " + CountBits(bitboards[TileType.Pasture]));
        Debug.Log("Desert = " + CountBits(bitboards[TileType.Desert]));
        Debug.Log("Grain = " + CountBits(bitboards[TileType.Grain]));
        Debug.Log("Rock = " + CountBits(bitboards[TileType.Rock]));
        Debug.Log("Woods = " + CountBits(bitboards[TileType.Woods]));

        InvokeRepeating("PlantTree", 0.3f, 0.3f);
        InvokeRepeating("PlaceBoat", 0.3f, 0.3f);
        InvokeRepeating("PlaceCow", 0.3f, 0.3f);
        InvokeRepeating("PlaceCactus", 0.3f, 0.3f);
        InvokeRepeating("PlaceWheat", 0.3f, 0.3f);
        InvokeRepeating("PlaceRocks", 0.3f, 0.3f);
        InvokeRepeating("PlaceCabin", 0.3f, 0.3f);
    }

    void PlantTree() { PlaceThing(TileType.Dirt, treePrefab); }
    void PlaceBoat() { PlaceThing(TileType.Water, boatPrefab); }
    void PlaceCow() { PlaceThing(TileType.Pasture, cowPrefab); }
    void PlaceCactus() { PlaceThing(TileType.Desert, cactusPrefab); }
    void PlaceWheat() { PlaceThing(TileType.Grain, wheatPrefab); }
    void PlaceRocks() { PlaceThing(TileType.Rock, rocksPrefab); }
    void PlaceCabin() { PlaceThing(TileType.Woods, housePrefab); }

    void PlaceThing(TileType tile, GameObject what)
    {
        int r = UnityEngine.Random.Range(0, 8);
        int c = UnityEngine.Random.Range(0, 8);

        if (GetBit(bitboards[tile], r, c))
        {
            int index = r * 8 + c;
            if (index < spawnedTiles.Count)
            {
                if (spawnedTiles[index].transform.childCount == 0)
                {
                    var g = Instantiate(what);
                    g.transform.parent = spawnedTiles[index].transform;
                    g.transform.localPosition = new Vector3(0, 0.5f, 0);
                }
            }
        }
    }

    public void DeleteBoard()
    {
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            var tile = spawnedTiles[i];
            if (tile != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(tile);
                else
                    Destroy(tile);
            }
        }
        spawnedTiles.Clear();

        foreach (TileType t in Enum.GetValues(typeof(TileType)))
        {
            bitboards[t] = 0;
        }
    }

    long SetBit(long board, int r, int c)
    {
        return board | (1L << (r * 8 + c));
    }

    bool GetBit(long board, int r, int c)
    {
        return (board & (1L << (r * 8 + c))) != 0;
    }

    int CountBits(long bb)
    {
        int cnt = 0;
        while (bb != 0)
        {
            bb &= bb - 1;
            cnt++;
        }
        return cnt;
    }
}
