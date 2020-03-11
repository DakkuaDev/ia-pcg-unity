using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixToPhysic
{
    public static void GeneratePhysicMaze(int[,] m, Transform parent, GameObject baseWallPrefab, GameObject baseFloorPrefab, float wallWidth, float wallHeight)
    {
        int w = m.GetUpperBound(0);
        int h = m.GetUpperBound(1);

        for (int y = 0; y <= h; ++y)
        {
            for (int x = 0; x <= w; ++x)
            {
                if (m[x, y] == 1)
                {
                    var wall = (GameObject)GameObject.Instantiate(baseWallPrefab, new Vector3(x * wallWidth, 0, -y * wallWidth), Quaternion.identity);
                    wall.transform.SetParent(parent);
                    wall.name = "Wall" + x + "_" + y;
                }
                else
                {
                    var floor = (GameObject)GameObject.Instantiate(baseFloorPrefab, new Vector3(x * wallWidth, -wallHeight * .5f, -y * wallWidth), Quaternion.identity);
                    floor.transform.SetParent(parent);
                    floor.name = "Floor" + x + "_" + y;
                }
            }
        }
    }
}
