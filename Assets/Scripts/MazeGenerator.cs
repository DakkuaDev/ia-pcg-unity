using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float wallProbability = 0.2f;
    public int[,] maze = null;
    public GameObject baseWallPrefab;
    public GameObject baseFloorPrefab;

    public float wallWidth = 3;
    public float wallHeight = 2;

    // Start is called before the first frame update
    void Start()
    {
        maze = GenerateMazeData(width, height);

        string data = "";
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (maze[x,y] == 1)
                {
                    data += "#";
                }
                else
                {
                    data += ".";
                }
            }
            data += "\n";
        }
        Debug.Log(data);

        GeneratePhysicMaze(maze);
    }

    int[,] GenerateMazeData(int w, int h)
    {
        int [,] ret = new int[w, h];
        
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                if (y == 0 || y == h-1 || x == 0 || x == w-1)
                {
                    ret[x,y] = 1;
                }
                else if ((y%2) == 0 && (x%2) == 0)
                {
                    if (Random.value > wallProbability)
                    {
                        ret[x,y] = 1;

                        int a = Random.value < 0.5f ? 0 : (Random.value < 0.5 ? 1 : -1);
                        int b = a != 0 ? 0 : (Random.value < 0.5f ? 1 : -1);
                        ret[x+a, y+b] = 1;
                    }
                }
            }
        }

        return ret;
    }

    public void GeneratePhysicMaze(int[,] m)
    {
        int w = m.GetUpperBound(0);
        int h = m.GetUpperBound(1);

        for (int y = 0; y <= h; ++y)
        {
            for (int x = 0; x <= w; ++x)
            {
                if (m[x,y] == 1)
                {
                    var wall = (GameObject) Instantiate(baseWallPrefab, new Vector3(x * wallWidth, 0, -y * wallWidth), Quaternion.identity);
                    wall.transform.SetParent(transform);
                    wall.name = "Wall" + x +"_" + y;
                }
                else
                {
                    var floor = (GameObject) Instantiate(baseFloorPrefab, new Vector3(x * wallWidth, -wallHeight*.5f, -y * wallWidth), Quaternion.identity);
                    floor.transform.SetParent(transform);
                    floor.name = "Floor" + x +"_" + y;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
