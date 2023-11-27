using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GridItemType {
    Center = 0,
    Side = 1,
    Corner = 2,
    End = 3,
    Floater = 4,
}

public class GridItem
{
    public bool isOccupied = false;
    public Transform t;

    public GridItemType type = GridItemType.Center;
    public Vector3 orientation = Vector3.zero;

    public GridItem(bool isOccupied)
    {
        this.isOccupied = isOccupied;
    }
}
