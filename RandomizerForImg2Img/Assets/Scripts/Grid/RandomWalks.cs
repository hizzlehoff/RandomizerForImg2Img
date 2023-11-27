using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RandomWalks
{
   public static Vector3[] GetWalks(int walksMin, int walksMax, int lengthMin, int lengthMax, int xSize, int ySize, int zSize)
   {
        List<Vector3> positions = new List<Vector3>();
        for (int j = 0; j< Random.Range(walksMin, walksMax); j++) {
            Vector3 position = new Vector3(Random.Range(0, xSize), Random.Range(0, ySize), Random.Range(0, zSize));
            positions.Add(position);

            Vector3 newPosition = position;

            for (int i=0;i<Random.Range(lengthMin, lengthMax); i++) {
                int randomDirections = Random.Range(0, 6);

                switch (randomDirections) {
                    case 0:
                        newPosition = position + new Vector3(0, -1, 0);
                        break;
                    case 1:
                        newPosition = position + new Vector3(0, 0, -1);
                        break;
                    case 2:
                        newPosition = position + new Vector3(1, 0, 0);
                        break;
                    case 3:
                        newPosition = position + new Vector3(0, 0, 1);
                        break;
                    case 4:
                        newPosition = position + new Vector3(-1, 0, 0);
                        break;
                    case 5:
                        newPosition = position + new Vector3(0, 1, 0);
                        break;
                    default:
                        newPosition = position + new Vector3(0, 0, 0);
                        break;
                }

                if (newPosition.x >= 0 && newPosition.x < xSize && newPosition.y >= 0 && newPosition.y < ySize && newPosition.z >= 0 && newPosition.z < zSize) {
                    positions.Add(newPosition);
                    position = newPosition;
                }
            }
        }
        return positions.ToArray();
    }
}
