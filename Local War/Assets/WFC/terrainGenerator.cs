using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * TODO:
 * getNextTile
 * placeTiles
*/

public class terrainGenerator : MonoBehaviour
{
    public GameObject[] tileModules;

    public int xSize = 8;
    public int ySize = 8;
    public int zSize = 8;

    public Vector3 worldOrigin;
    public Vector3 worldScale;

    private List<PrimitiveTile> importedTiles = new List<PrimitiveTile>();
    private int[, , ,] tiles;

    System.Random rng = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        importTiles();

        if ((xSize == 0) || (ySize == 0) || (zSize == 0))
        {
            xSize = 8;
            ySize = 8;
            zSize = 8;
        }

        for (int t = 0; t < importedTiles.Count; t++)
        {
            print("Tile #" + t.ToString() + ", or " + importedTiles[t].Module.name);
            print("    +x: " + tiles[t, 2, 1, 1].ToString() + " (" + importedTiles[tiles[t, 2, 1, 1]].Module.name + ")");
            print("    -x: " + tiles[t, 0, 1, 1].ToString() + " (" + importedTiles[tiles[t, 0, 1, 1]].Module.name + ")");
            print("    +y: " + tiles[t, 1, 2, 1].ToString() + " (" + importedTiles[tiles[t, 1, 2, 1]].Module.name + ")");
            print("    -y: " + tiles[t, 1, 0, 1].ToString() + " (" + importedTiles[tiles[t, 1, 0, 1]].Module.name + ")");
            print("    +z: " + tiles[t, 1, 1, 2].ToString() + " (" + importedTiles[tiles[t, 1, 1, 2]].Module.name + ")");
            print("    -z: " + tiles[t, 1, 1, 0].ToString() + " (" + importedTiles[tiles[t, 1, 1, 0]].Module.name + ")");
        }

        generateTerrain(new int[] { xSize, ySize, zSize }, 3);
    }
    
    private class PrimitiveTile : IComparable<PrimitiveTile>
    {
        public PrimitiveTile(int[] adjs, int[] rots, int id, bool rotatable, int rotation, GameObject module, bool bottomAllow, bool topAllow, bool sideAllow, int frequency)
        {
            Adjs = adjs;
            Rots = rots;

            Id = id;
            Rotation = rotation;

            Module = module;

            Rotatable = rotatable;
            BottomAllow = bottomAllow;
            TopAllow = topAllow;
            SideAllow = sideAllow;

            Frequency = frequency;
        }

        public int[] Adjs { get; set; }
        public int[] Rots { get; set; }

        public int Id { get; set; }
        public int Rotation { get; set; }

        public GameObject Module { get; set; }

        public bool Rotatable { get; set; }
        public bool BottomAllow { get; set; }
        public bool TopAllow { get; set; }
        public bool SideAllow { get; set; }

        public int Frequency { get; set; }

        public PrimitiveTile Duplicate()
        {
            return (PrimitiveTile)this.MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public int CompareTo(PrimitiveTile other)
        {
            return Id.CompareTo(other.Id);
        }
    }

    void generateTerrain(int[] dims, int backtrackSteps)
    {
        int currentTilemap = 1;
        int oldestTilemap = 0;

        int tileCount = importedTiles.Count;

        int[] nextTileLocation = new int[3] { dims[0] / 2, dims[1] / 2, dims[2] / 2 };
        bool[] nextTile = new bool[tileCount];
        bool[] inverseTile = new bool[tileCount];

        bool[][,,,] tileMaps = new bool[backtrackSteps][,,,];

        for (int i = 0; i < backtrackSteps; i++)
        {
            tileMaps[i] = new bool[dims[0], dims[1], dims[2], tileCount];
        }

        tileMaps[0] = initializeTilemap();

        tileMaps[1] = (bool[,,,])tileMaps[0].Clone();

        nextTile[39] = true;

        tileMaps[0] = propagateChange(tileMaps[0], nextTile, nextTileLocation);

        while (true)
        {
            //Debug.Log("Copying tilemap");
            
            tileMaps[currentTilemap] = (bool[,,,])tileMaps[previousTilemapIdx(currentTilemap)].Clone(); //suspicious
            

            //Debug.Log("Finished");

            //Debug.Log("Getting next tile");
            getNextTile();
            //Debug.Log("Finished");

            if (nextTile.Length == 0)
            {
                print("Finished generating terrain.");
                break;
            }

            //Debug.Log("Propagating change");
            propagateChange(tileMaps[currentTilemap], nextTile, nextTileLocation); // PROBLEMS HERE
            //Debug.Log("Finished");

            if (!tilemapValid(tileMaps[currentTilemap]))
            {
                //Debug.Log("Got bad tilemap; backtracking");
                print("Got bad tilemap.");
                //Debug.Log("Copying tilemap");
                tileMaps[currentTilemap] = (bool[,,,])tileMaps[previousTilemapIdx(currentTilemap)].Clone();
                //Debug.Log("Finished");

                for (int i = 0;i < tileCount; i++)
                {
                    inverseTile[i] = tileMaps[currentTilemap][nextTileLocation[0], nextTileLocation[1], nextTileLocation[2], i];
                }

                inverseTile[Array.IndexOf(nextTile, true)] = false;

                //Debug.Log("Propagating change");
                propagateChange(tileMaps[currentTilemap], inverseTile, nextTileLocation); // PROBLEMS HERE
                //Debug.Log("Finished");

                if (!tilemapValid(tileMaps[currentTilemap]))
                {
                    print("Backtracking.");

                    if (oldestTilemap == previousTilemapIdx(currentTilemap))
                    {
                        break;
                    }
                    else
                    {
                        currentTilemap = previousTilemapIdx(currentTilemap);
                    }
                }
                else
                {
                    currentTilemap = (currentTilemap + 1) % backtrackSteps;

                    if (oldestTilemap == currentTilemap)
                    {
                        oldestTilemap = (currentTilemap + 1) % backtrackSteps;
                    }
                }
            }
            else
            {
                currentTilemap = (currentTilemap + 1) % backtrackSteps;

                if (oldestTilemap == currentTilemap)
                {
                    oldestTilemap = (currentTilemap + 1) % backtrackSteps;
                }
            }
        }

        placeTiles(tileMaps[currentTilemap]);
        print(tilemapValid(tileMaps[currentTilemap]));
        print(tilemapComplete(tileMaps[currentTilemap]));

        // => entropyAtPos(bool[,,,] tm, int[] pos)
        int entropyAtPos(bool[,,,] tm, int[] pos)
        {
            int entropy = 0;

            for (int i = 0; i < tileCount; i++)
            {
                if (tm[pos[0], pos[1], pos[2], i])
                {
                    entropy++;
                }
            }
            return entropy;
        }

        // Good
        bool listContains1DArray(List<int[]> L, int[] A)
        {
            bool elementFound = false;

            for (int i = 0; i < L.Count; i++)
            {
                elementFound = true;

                for (int e = 0; e < A.Length; e++)
                {
                    if (L[i][e] != A[e])
                    {
                        elementFound = false;
                        break;
                    }
                }

                if (elementFound)
                {
                    return true;
                }
            }

            return false;
        }

        // => bool[,,,] propagateChange(bool[,,,] tm, bool[] tile, int[] location, oldTm = null)
        bool[,,,] propagateChange(bool[,,,] tilemap, bool[] tile, int[] location)
        {
            List<int[]> getNeighbors(int[] pos)
            {
                

                List<int[]> returnTiles = new List<int[]>();
                List<int[]> invalidTiles = new List<int[]>();

                for (int d = 0; d < 3; d++)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        returnTiles.Add(new int[] { pos[0], pos[1], pos[2] });
                        returnTiles[returnTiles.Count - 1][d] += i;
                    }
                }

                foreach (int[] currentNeighbor in returnTiles)
                {
                    //x
                    if (currentNeighbor[0] >= dims[0] || currentNeighbor[0] < 0)
                    {
                        invalidTiles.Add(currentNeighbor);
                    }

                    //y
                    else if (currentNeighbor[1] >= dims[1] || currentNeighbor[1] < 0)
                    {
                        invalidTiles.Add(currentNeighbor);
                    }

                    //z
                    else if (currentNeighbor[2] >= dims[2] || currentNeighbor[2] < 0)
                    {
                        invalidTiles.Add(currentNeighbor);
                    }
                }

                foreach (int[] invalidTile in invalidTiles)
                {
                    returnTiles.Remove(invalidTile); // suspicious
                }

                

                return returnTiles;
            }

            bool[] getPossibleTiles(bool[,,,] tm, int[] pos, bool exclusive=false) 
            {
                
                //Debug.Log("In getPossibleTiles");
                bool[] possibleTiles = new bool[tileCount];

                bool isPossible = false;

                // Check whether or not each tile type is possible
                for (int tileID = 0; tileID < tileCount; tileID++)
                {
                    if (exclusive && !tm[pos[0], pos[1], pos[2], tileID])
                    {
                        continue;
                    }

                    isPossible = true;
                    
                    // Check this tile type, given this tile's neighbors.
                    foreach (int[] neighbor in getNeighbors(pos))
                    {
                        // If the tile referenced by the current tile type is not possible...
                        if (!tm[neighbor[0], neighbor[1], neighbor[2], tiles[tileID, neighbor[0] - pos[0] + 1, neighbor[1] - pos[1] + 1, neighbor[2] - pos[2] + 1]])
                        {
                            isPossible = false;

                            // check if it is referenced by a tile in the neighbor that is possible.
                            // Loop through all tile types in this neighbor
                            for (int t = 0; t < tileCount; t++)
                            {
                                // If the tile is possible...
                                if (tm[neighbor[0], neighbor[1], neighbor[2], t])
                                {
                                    // check if it references the current tile type.
                                    if (tileID == tiles[t, pos[0] - neighbor[0] + 1, pos[1] - neighbor[1] + 1, pos[2] - neighbor[2] + 1])
                                    {
                                        // If it is, this tile is possible.
                                        isPossible = true;
                                        break;
                                    }
                                }
                            }

                            // If the tile wasn't possible, don't bother checking for all of the other neighbors.
                            if (!isPossible)
                            {
                                break;
                            }
                        }
                    }

                    if (isPossible)
                    {
                        possibleTiles[tileID] = true;
                    }
                }

                return possibleTiles;
            }

            bool[,,,] lastTm = (bool[,,,])tilemap.Clone();

            bool[] newTile = new bool[tileCount];

            bool allFalse = false;

            // The set of tiles whose entropy has changed, and their neighbors must be computed.
            List<int[]> tileSet = new List<int[]>();
            tileSet.Add(location);

            List<int[]> tileBuffer = new List<int[]>();

            for (int i = 0; i < tileCount; i++)
            {
                tilemap[location[0], location[1], location[2], i] = tile[i];
            }

            //float startTime = Time.realtimeSinceStartup;

            while (tileSet.Count != 0)
            {
                tileBuffer.Clear();

                // Loop through tiles being worked on
                foreach (int[] i in tileSet)
                {
                    // Look at each of their neighbors
                    foreach (int[] neighbor in getNeighbors(i))
                    {
                        

                        // Find the possible tiles for this neighbor
                        newTile = getPossibleTiles(tilemap, neighbor);

                        

                        allFalse = true;

                        for (int t = 0; t < tileCount; t++)
                        {

                            tilemap[neighbor[0], neighbor[1], neighbor[2], t] = newTile[t];

                            if (tilemap[neighbor[0], neighbor[1], neighbor[2], t])
                            {
                                allFalse = false;
                            }

                            //tilemap[neighbor[0], neighbor[1], neighbor[2], t] = newTile[t];
                            // Set the tilemap location equal to the possible tiles
                        }

                        if (allFalse)
                        {
                            //print("WARNING: Found bad tile during propagation on iteration " + iter.ToString());
                            return tilemap;
                        }


                        // If nothing has changed, don't bother computing its neighbors.
                        for (int t = 0; t < tileCount; t++)
                        {
                            if (tilemap[neighbor[0], neighbor[1], neighbor[2], t] != lastTm[neighbor[0], neighbor[1], neighbor[2], t])
                            {
                                if (!listContains1DArray(tileBuffer, neighbor))
                                {
                                    tileBuffer.Add(neighbor);
                                }
                                break;
                            }
                        }
                    }
                }
                //print("Iteration " + iter.ToString() + " tilemap is " + tilemapValid(tileMaps[currentTilemap]).ToString());

                tileSet.Clear();

                for (int i = 0; i < tileBuffer.Count; i++)
                {
                    tileSet.Add(tileBuffer[i]);
                }

                lastTm = (bool[,,,])tilemap.Clone();
            }
            // While there are still uncomputed neighbors

            return tilemap;
        }

        // => tilemapValid(bool[,,,] tm)
        bool tilemapValid(bool[,,,] tm)
        {
            bool allFalse = false;
            //Loop through every tile in the tilemap; if you reach a tile where every possible tile is False, return False
            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        allFalse = true;

                        for (int t = 0; t < tileCount; t++)
                        {
                            
                            if (tm[x, y, z, t])
                            {
                                allFalse = false;
                                break;
                            }
                        }

                        if (allFalse)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        int weightedRandomChoice(List<int> items, List<int> freqs)
        {
            if (freqs.Count != items.Count)
            {
                throw new Exception("aaaaaa");
            }

            bool allZero = true;

            foreach (int n in freqs)
            {
                if (n != 0)
                {
                    allZero = false;
                    break;
                }
            }

            if (allZero)
            {
                return items[rng.Next(items.Count)];
            }
            else
            {
                int currentIdx = 0;

                while (currentIdx < items.Count)
                {
                    if (freqs[currentIdx] == 0)
                    {
                        freqs.RemoveAt(currentIdx);
                        items.RemoveAt(currentIdx);
                    }
                    else
                    {
                        currentIdx += 1;
                    }
                }
            }

            int freqSum = 0;

            foreach (int f in freqs)
            {
                freqSum += f;
            }

            int randChoice = rng.Next(freqSum);
            int currentSum = 0;

            for (int i = 0; i < freqs.Count; i++)
            {
                if (randChoice >= currentSum && randChoice < (currentSum + freqs[i]))
                {
                    return items[i];
                }
                else
                {
                    currentSum += freqs[i];
                }
            }

            return items[0];
        }

        // int[] getNextTile(bool[,,,] tm)
        // The first three ints are the position, and the last is the type.
        void getNextTile()
        {
            //print("Getting next tile...");
            List<int> possibleTiles = new List<int>();

            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        if (entropyAtPos(tileMaps[currentTilemap], new int[] { x, y, z }) == 1)
                        {
                            continue;
                        }

                        for (int t = 0; t < tileCount; t++)
                        {
                            if (tileMaps[currentTilemap][x, y, z, t])
                            {
                                if (!possibleTiles.Contains(t))
                                {
                                    possibleTiles.Add(t);
                                }
                            }
                        }
                    }
                }
            }

            if (possibleTiles.Count == 0)
            {
                nextTile = new bool[0];
                return;
            }

            List<int> tileFrequencies = new List<int>();

            foreach (int i in possibleTiles)
            {
                tileFrequencies.Add(importedTiles[i].Frequency);
            }

            int chosenTile = weightedRandomChoice(possibleTiles, tileFrequencies);

            nextTile = new bool[tileCount];
            nextTile[weightedRandomChoice(possibleTiles, tileFrequencies)] = true;

            int minimumTileEntropy = tileCount;
            int currentTileEntropy = 0;

            List<int[]> possibleLocations = new List<int[]>();

            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        if (tileMaps[currentTilemap][x, y, z, chosenTile])
                        {
                            currentTileEntropy = entropyAtPos(tileMaps[currentTilemap], new int[] { x, y, z });

                            if (currentTileEntropy > 1)
                            {
                                if ((currentTileEntropy == minimumTileEntropy))
                                {
                                    possibleLocations.Add(new int[] { x, y, z });
                                }
                                else if (currentTileEntropy < minimumTileEntropy)
                                {
                                    possibleLocations.Clear();
                                    minimumTileEntropy = currentTileEntropy;
                                    possibleLocations.Add(new int[] { x, y, z });
                                }
                            }
                        }
                    }
                }
            }

            nextTileLocation = possibleLocations[rng.Next(possibleLocations.Count)];
            //print("Next tile is " + chosenTileIdx.ToString() + " at location [" + nextTileLocation[0].ToString() + ", " + nextTileLocation[1].ToString() + ", " + nextTileLocation[2].ToString() + "]");
        }

        bool tilemapComplete(bool[,,,] tm)
        {
            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        if (entropyAtPos(tm, new int[] { x, y, z }) != 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        int previousTilemapIdx(int currentTilemapIdx)
        {
            if (currentTilemapIdx == 0)
            {
                return (backtrackSteps - 1);
            }

            else
            {
                return (currentTilemapIdx - 1);
            }
        }

        // bool[,,,] initializeTilemap()
        bool[,,,] initializeTilemap()
        {
            bool [,,,] tmOut = new bool[dims[0], dims[1], dims[2], tileCount];

            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        for (int t = 0; t < tileCount; t++)
                        {
                            tmOut[x, y, z, t] = true;
                        }
                    }
                }
            }

            // Get top, bottom, and edge tiles
            // Get bottom edge and top edge tiles
            // Propagate top and bottom
            // Propagate edges
            // Propagate other edges

            bool[] topTile = new bool[tileCount];
            bool[] bottomTile = new bool[tileCount];
            bool[] edgeTile = new bool[tileCount];

            for (int t = 0; t < tileCount; t++)
            {
                topTile[t] = importedTiles[t].TopAllow;
                bottomTile[t] = importedTiles[t].BottomAllow;
                edgeTile[t] = importedTiles[t].SideAllow;
            }

            print("t");
            
            // Top & bottom
            for (int x = 0; x < dims[0]; x++)
            {
                for (int z = 0; z < dims[2]; z++)
                {
                    bool[] nextTopTile = intersectTiles(topTile, getTileAtPos(tmOut, new int[] { x, dims[1] - 1, z }));
                    tmOut = propagateChange(tmOut, nextTopTile, new int[] { x, dims[1] - 1, z });

                    bool[] nextBottomTile = intersectTiles(bottomTile, getTileAtPos(tmOut, new int[] { x, 0, z }));
                    tmOut = propagateChange(tmOut, nextBottomTile, new int[] { x, 0, z });
                }
            }

            bool[] floorTile = new bool[tileCount];

            for (int t = 0; t < tileCount; t++)
            {
                if (importedTiles[t].Module.name == "Floor")
                {
                    floorTile[t] = true;
                    break;
                }
            }

            /*/

            for (int x = 0; x < dims[0]; x++)
            {
                tmOut = propagateChange(tmOut, floorTile, new int[] { x, dims[1] / 2, 0 });
                tmOut = propagateChange(tmOut, floorTile, new int[] { x, dims[1] / 2, dims[2] - 1 });
            }

            for (int z = 0; z < dims[0]; z++)
            {
                tmOut = propagateChange(tmOut, floorTile, new int[] { 0, dims[1] / 2, z });
                tmOut = propagateChange(tmOut, floorTile, new int[] { dims[0] - 1, dims[1] / 2, z });
            }

            /*/
            //Sides
            
            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    tmOut = propagateChange(tmOut, intersectTiles(edgeTile, getTileAtPos(tmOut, new int[] { x, y, 0 })), new int[] { x, y, 0 });
                    tmOut = propagateChange(tmOut, intersectTiles(edgeTile, getTileAtPos(tmOut, new int[] { x, y, dims[2] - 1 })), new int[] { x, y, dims[2] - 1 });
                }
            }

            for (int z = 0; z < dims[2]; z++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    tmOut = propagateChange(tmOut, intersectTiles(edgeTile, getTileAtPos(tmOut, new int[] { 0, y, z })), new int[] { 0, y, z });
                    tmOut = propagateChange(tmOut, intersectTiles(edgeTile, getTileAtPos(tmOut, new int[] { dims[0] - 1, y, z })), new int[] { dims[0] - 1, y, z });
                }
            }
            
            bool[] possibleTiles = new bool[tileCount];

            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        for (int t = 0; t < tileCount; t++)
                        {
                            if (tmOut[x, y, z, t])
                            {
                                possibleTiles[t] = true;
                            }
                        }
                    }
                }
            }

            for (int t = 0; t < tileCount; t++)
            {
                if (!possibleTiles[t])
                {
                    print("Tile #" + t.ToString() + " is impossible.");
                }
            }
            print("Initialization success: ");
            print(tilemapValid(tmOut).ToString());
            if (!tilemapValid(tmOut))
            {
                throw new Exception("AAAAAAAAAAAAAAAAAAAAA WHY MUYST THYIS HAPEEN");
            }

            return tmOut;
        }

        bool[] intersectTiles(bool[] tile1, bool[] tile2)
        {
            bool[] tileOut = new bool[tileCount];

            for (int t = 0; t < tileCount; t++)
            {
                tileOut[t] = tile1[t] && tile2[t];
            }

            return tileOut;
        }

        bool[] getTileAtPos(bool[,,,] tm, int[] location)
        {
            bool[] tileOut = new bool[tileCount];

            for (int t = 0; t < tileCount; t++)
            {
                tileOut[t] = tm[location[0], location[1], location[2], t];
            }

            return tileOut;
        }

        void placeTiles(bool[,,,] tm)
        {

            for (int x = 0; x < dims[0]; x++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int z = 0; z < dims[2]; z++)
                    {
                        if (entropyAtPos(tm, new int[] { x, y, z }) == 1)
                        {
                            for (int t = 0; t < tileCount; t++)
                            {
                                if (tm[x, y, z, t])
                                {
                                    Quaternion mRot = Quaternion.Euler(0, 90 * importedTiles[t].Rotation, 0);
                                    Instantiate(importedTiles[t].Module, new Vector3(x * worldScale.x + worldOrigin.x, y * worldScale.y + worldOrigin.y, z * worldScale.z + worldOrigin.z), mRot);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void importTiles()
    {
        int[] shiftLinearAdjRules(int[] adjs)
        {
            // [+x, -x, +y, -y, +z, -z] [0, 1, 2, 3, 4, 5]
            //      ||
            //      \/
            // [+z, -z, +y, -y, -x, +x] [5, 4, 2, 3, 1, 0]
            return new int[] { adjs[4], adjs[5], adjs[2], adjs[3], adjs[1], adjs[0] };
            //return new int[] { adjs[5], adjs[4], adjs[2], adjs[3], adjs[0], adjs[1] };
        }

        int findTileIndex(List<PrimitiveTile> tileList, int id)
        {
            for (int i = 0;i < tileList.Count; i++)
            {
                if (id == tileList[i].Id)
                {
                    return i;
                }
            }

            return -1;
        }

        
        List<int> rotatableTiles = new List<int>();
        List<int> nonrotatableTiles = new List<int>();

        tileModule tmData;

        for (int i = 0;i < tileModules.Length; i++)
        {
            tmData = tileModules[i].GetComponent<tileModule>();

            importedTiles.Add(new PrimitiveTile(
                new int[] { Array.IndexOf(tileModules, tmData.POSITIVE_X_TILE),
                            Array.IndexOf(tileModules, tmData.NEGATIVE_X_TILE),
                            Array.IndexOf(tileModules, tmData.POSITIVE_Y_TILE),
                            Array.IndexOf(tileModules, tmData.NEGATIVE_Y_TILE),
                            Array.IndexOf(tileModules, tmData.POSITIVE_Z_TILE),
                            Array.IndexOf(tileModules, tmData.NEGATIVE_Z_TILE)},

                new int[] { tmData.POSITIVE_X_ROTATION,
                            tmData.NEGATIVE_X_ROTATION,
                            tmData.POSITIVE_Y_ROTATION,
                            tmData.NEGATIVE_Y_ROTATION,
                            tmData.POSITIVE_Z_ROTATION,
                            tmData.NEGATIVE_Z_ROTATION},
                0,
                tmData.Rotatable,
                0,
                tileModules[i],
                tmData.AllowOnFloor,
                tmData.AllowOnCeiling,
                tmData.AllowOnEdges,
                tmData.Frequency));

            importedTiles[i].Module.transform.localScale = worldScale;

            if (tmData.Rotatable)
            {
                rotatableTiles.Add(i);
            }
            else
            {
                nonrotatableTiles.Add(i);
            }
        }

        for (int i = 0;i < rotatableTiles.Count; i++)
        {
            importedTiles[rotatableTiles[i]].Id = 4 * i;
        }

        for (int i = 0;i < nonrotatableTiles.Count; i++)
        {
            importedTiles[nonrotatableTiles[i]].Id = 4 * rotatableTiles.Count + i;
        }

        for (int i = 0; i < importedTiles.Count; i++)
        {
            for (int adjRule = 0; adjRule < importedTiles[i].Adjs.Length; adjRule++)
            {
                importedTiles[i].Adjs[adjRule] = importedTiles[importedTiles[i].Adjs[adjRule]].Id;
            }
        }

        List<PrimitiveTile> newTiles = new List<PrimitiveTile>();

        for (int i = 0; i < importedTiles.Count; i++)
        {
            if (importedTiles[i].Rotatable)
            {
                PrimitiveTile newTile = importedTiles[i].Duplicate();
                for (int tile = 0; tile < 3; tile++)
                {
                    newTile.Adjs = shiftLinearAdjRules(newTile.Adjs);
                    newTile.Rots = shiftLinearAdjRules(newTile.Rots);
                    newTile.Id++;
                    newTile.Rotation++;

                    for (int rot = 0; rot < newTile.Rots.Length; rot++)
                    {
                        newTile.Rots[rot] = (newTile.Rots[rot] + 1) % 4;
                    }

                    newTiles.Add(newTile.Duplicate());
                }
            }
        }

        for (int i = 0; i < newTiles.Count; i++)
        {
            importedTiles.Add(newTiles[i]);
        }

        for (int i = 0; i < importedTiles.Count; i++)
        {
            //if (importedTiles[i].Rotatable)
            //{
            for (int neighbor = 0; neighbor < importedTiles[i].Adjs.Length; neighbor++)
            {
                if (importedTiles[findTileIndex(importedTiles, importedTiles[i].Adjs[neighbor])].Rotatable)
                {
                    importedTiles[i].Adjs[neighbor] += importedTiles[i].Rots[neighbor];
                }
            }
            //}
        }

        tiles = new int[importedTiles.Count, 3, 3, 3];

        for (int i = 0; i < importedTiles.Count; i++)
        {
            tiles[importedTiles[i].Id, 2, 1, 1] = importedTiles[i].Adjs[0];// +x
            tiles[importedTiles[i].Id, 0, 1, 1] = importedTiles[i].Adjs[1];// -x
            tiles[importedTiles[i].Id, 1, 2, 1] = importedTiles[i].Adjs[2];// +y
            tiles[importedTiles[i].Id, 1, 0, 1] = importedTiles[i].Adjs[3];// -y
            tiles[importedTiles[i].Id, 1, 1, 2] = importedTiles[i].Adjs[4];// +z
            tiles[importedTiles[i].Id, 1, 1, 0] = importedTiles[i].Adjs[5];// -z
        }

        importedTiles.Sort();

        print("Finished importing tiles");
    }
}