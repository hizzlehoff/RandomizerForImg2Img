using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    GridItem[,,] grid;

    public int xSize = 0;
    public int ySize = 0;
    public int zSize = 0;

    public Grid(int xSize, int ySize, int zSize)
    {
        InitGrid(xSize, ySize, zSize);
    }

    public void InitGrid(int xSize, int ySize, int zSize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.zSize = zSize;

        grid = new GridItem[xSize, ySize, zSize];

        for (int z = 0; z < zSize; z++) {
            for (int y = 0; y < ySize; y++) {
                for (int x = 0; x < xSize; x++) {
                    grid[x, y, z] = new GridItem(false);
                }
            }
        }
    }

    public void RandomizeGrid(float threshold, bool mirrorGrid)
    {
        for (int z = 0; z < zSize; z++) {
            for (int y = 0; y < ySize; y++) {
                for (int x = 0; x < xSize ; x++) {
                    grid[x, y, z].isOccupied = (Random.Range(0f, 1f) > threshold) ? true : false;
                }
            }
        }

        if (mirrorGrid) MirrorGrid();
    }

    public void RandomWalksGrid(int walksMin, int walksMax, int lengthMin, int lengthMax, bool mirrorGrid)
    {
        Vector3[] positions = RandomWalks.GetWalks(walksMin, walksMax, lengthMin, lengthMax, xSize, ySize, zSize);

        for (int i = 0; i < positions.Length; i++) {

            int x = (int)positions[i].x;
            int y = (int)positions[i].y;
            int z = (int)positions[i].z;

            if (x >= 0 && x < xSize && y >= 0 && y < ySize && z >= 0 && z < zSize) {
                grid[x, y, z].isOccupied = true;
            }
        }

        if (mirrorGrid) MirrorGrid();
    }

    void MirrorGrid()
    {
        for (int z = 0; z < zSize; z++) {
            for (int y = 0; y < ySize; y++) {
                for (int x = 0; x < (int) xSize / 2; x++) {
                    grid[xSize - x - 1, y, z].isOccupied = grid[x, y, z].isOccupied;
                }
            }
        }
    }

    public bool IsOccupied(int x, int y, int z)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || z < 0 || z >= zSize) return false;
        return grid[x, y, z].isOccupied;
    }

    public Transform GetTransform(int x, int y, int z) {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || z < 0 || z >= zSize) return null;
        return grid[x, y, z].t;
    }

    public int GetType(int x, int y, int z)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || z < 0 || z >= zSize) return 0;
        return (int) grid[x, y, z].type;
    }

    public Vector3 GetOrientation(int x, int y, int z)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || z < 0 || z >= zSize) return Vector3.zero;
        return grid[x, y, z].orientation;
    }

    public void SetTransform(int x, int y, int z, Transform t, Vector3 r)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || z < 0 || z >= zSize) return;
        grid[x, y, z].t = t;

        grid[x, y, z].t.position = new Vector3(x, y, z);
        grid[x, y, z].t.eulerAngles = r;
    }

    public void SetTypes()
    {
        for (int z = 0; z < zSize; z++) {
            for (int y = 0; y < ySize; y++) {
                for (int x = 0; x < xSize; x++) {
                    SetType(x, y, z);
                }
            }
        }
    }

    public void SetType(int x, int y, int z)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || z < 0 || z >= zSize) return;

        bool[] activeSides = GetActiveSides(x, y, z);

        switch (activeSides[0], activeSides[1], activeSides[2], activeSides[3], activeSides[4], activeSides[5]) {

            // 4 adjacent sides = side.
            case (true, false, true, true, true, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 180, 0);
                break;
            case (true, true, false, true, true, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 90, 0);
                break;
            case (true, true, true, false, true, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 0, 0);
                break;
            case (true, true, true, true, false, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, -90, 0);
                break;

            // 4 adjacent sides = side.
            case (false, false, true, true, true, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(180, 0, 0);
                break;
            case (false, true, false, true, true, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(180, -90, 0);
                break;
            case (false, true, true, false, true, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(180, 180, 0);
                break;
            case (false, true, true, true, false, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(180, 90, 0);
                break;

            // 2 adjacent sides = side.
            case ( true, true, false, false, false, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = Vector3.zero;
                break;
            case (true, false, true, false, false, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 270, 0);
                break;
            case (true, false, false, true, false, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 180, 0);
                break;
            case (true, false, false, false, true, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 90, 0);
                break;

            // 2 adjacent sides = side.
            case (false, true, false, false, false, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(90, 0, 0);
                break;
            case (false, false, true, false, false, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(90, 270, 0);
                break;
            case (false, false, false, true, false, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(90, 180, 0);
                break;
            case (false, false, false, false, true, true):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(90, 90, 0);
                break;

            // 2 adjacent sides = side.
            case (false, true, true, false, false, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 0, 90);
                break;
            case (false, false, true, true, false, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 270, 90);
                break;
            case (false, false, false, true, true, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 180, 90);
                break;
            case (false, true, false, false, true, false):
                grid[x, y, z].type = GridItemType.Side;
                grid[x, y, z].orientation = new Vector3(0, 90, 90);
                break;

            // 3 adjacent sides = corner.
            case (true, true, true, false, false, false):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(0, 0, 0);
                break;
            case (true, false, true, true, false, false):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(0, 270, 0);
                break;
            case (true, false, false, true, true, false):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(0, 180, 0);
                break;
            case (true, true, false, false, true, false):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(0, 90, 0);
                break;

            // 3 adjacent sides = corner.
            case (false, true, true, false, false, true):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(180, 90, 0);
                break;
            case (false, false, true, true, false, true):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(180, 0, 0);
                break;
            case (false, false, false, true, true, true):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(180, -90, 0);
                break;
            case (false, true, false, false, true, true):
                grid[x, y, z].type = GridItemType.Corner;
                grid[x, y, z].orientation = new Vector3(180, 180, 0);
                break;

            // 1 adjacent sides = end.
            case (true, false, false, false, false, false):
                grid[x, y, z].type = GridItemType.End;
                grid[x, y, z].orientation = new Vector3(-90, 0, 0);
                break;
            case (false, false, false, false, false, true):
                grid[x, y, z].type = GridItemType.End;
                grid[x, y, z].orientation = new Vector3(90, 0, 0);
                break;
            case (false, true, false, false, false, false):
                grid[x, y, z].type = GridItemType.End;
                grid[x, y, z].orientation = new Vector3(0, 0, 0);
                break;
            case (false, false, true, false, false, false):
                grid[x, y, z].type = GridItemType.End;
                grid[x, y, z].orientation = new Vector3(0, -90, 0);
                break;
            case (false, false, false, true, false, false):
                grid[x, y, z].type = GridItemType.End;
                grid[x, y, z].orientation = new Vector3(0, 180, 0);
                break;
            case (false, false, false, false, true, false):
                grid[x, y, z].type = GridItemType.End;
                grid[x, y, z].orientation = new Vector3(0, 90, 0);
                break;

            case (false, false, false, false, false, false):
                grid[x, y, z].type = GridItemType.Floater;
                grid[x, y, z].orientation = new Vector3(0, 0, 0);
                break;

            default:
                grid[x, y, z].type = GridItemType.Center;
                grid[x, y, z].orientation = Vector3.zero;
                break;
        }
    }


    bool[] GetActiveSides(int x, int y, int z)
    {
        bool[] activeSides = new bool[6];

        // Bottom neighbor.
        if (IsOccupied(x, y - 1, z)) activeSides[0] = true;

        // Middle 4 neighbors.
        if (IsOccupied(x, y, z - 1)) activeSides[1] = true;
        if (IsOccupied(x - 1, y, z)) activeSides[4] = true;
        if (IsOccupied(x + 1, y, z)) activeSides[2] = true;
        if (IsOccupied(x, y, z + 1)) activeSides[3] = true;

        // Top neighbor.
        if (IsOccupied(x, y + 1, z)) activeSides[5] = true;

        return activeSides;
    }

    int ConvertToInteger(bool[] binaryArray)
    {
        int result = 0;
        int power = 1;

        for (int i = 0; i < binaryArray.Length; i++) {
            if (binaryArray[i]) {
                result += power;
            }
            power *= 2;
        }
        return result;
    }
}
