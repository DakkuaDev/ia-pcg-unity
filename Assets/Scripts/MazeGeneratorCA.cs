using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGeneratorCA : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public bool startRandom = true;
    public float startGenerationWallThreshold = 0.2f;
    public int tunnelsForStartGeneration = 10;
    public int tunnelsSizeForStartGeneration = 5;
    public int numberOfGenerations = 50;
    public int[] bornRule = { 3 };
    public int[] surviveRule = {1,2,3,4,5};

    public int[,] maze = null;

    public GameObject baseWallPrefab;
    public GameObject baseFloorPrefab;
    public int seed = -1;

    public float wallWidth = 3;
    public float wallHeight = 2;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = (int)(Random.value * 100000);
        }
        maze = GenerateMazeData(seed, width, height, numberOfGenerations);
        MatrixToPhysic.GeneratePhysicMaze(maze, transform, baseWallPrefab, baseFloorPrefab, wallWidth, wallHeight);

        var pos = FindStartPosition();
        if (pos.x != -1 && pos.y != -1)
        {
            var player = (GameObject)Instantiate(playerPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
    }

    void PrintMazeData(int[,] m, int generation)
    {
        string data = "Generation: " + generation + "\n";
        for (int y = 0; y <= m.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= m.GetUpperBound(0); ++x)
            {
                if (m[x, y] == 1)
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
    }

    int[,] GenerateMazeData(int s, int w, int h, int generations)
    {
        System.Random r = new System.Random(s);

        int[,] ret = new int[w, h];

        // random start
        if (startRandom)
        {
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    ret[x, y] = r.NextDouble() < startGenerationWallThreshold ? 1 : 0;
                }
            }
        }
        else
        {
            ret = GetComponent<MazeGeneratorWalker>().GenerateMazeData(s, w, h, tunnelsForStartGeneration, tunnelsSizeForStartGeneration);
        }

        PrintMazeData(ret, 0);

        for (int i = 0; i < generations; ++i)
        {
            ret = Evolution(ret);
            PrintMazeData(ret, i+1);
        }

        // fill all edges in map
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                if (x == 0 || y == 0 || x == w-1 || y == h-1)
                {
                    ret[x, y] = 1;
                }
            }
        }

        return ret;
    }

    int NumberOfNeighbours(int posX, int posY, int[,] m)
    {
        int ret = 0;
        for (int y = posY - 1; y <= posY + 1; ++y)
        {
            for (int x = posX - 1; x <= posX + 1; ++x)
            {
                if (y >= 0 && y <= m.GetUpperBound(1) && x >= 0 && x <= m.GetUpperBound(0))
                {
                    // not count own cell
                    if (posX != x || posY != y)
                    {
                        ret += m[x,y];
                    }
                }
            }
        }

        return ret;
    }

    int ApplyRules(int x, int y, int[,] m)
    {
        int neighbours = NumberOfNeighbours(x, y, m);
        if (m[x,y] == 0 && bornRule.Contains(neighbours))
        {
            return 1;
        }
        else if (m[x,y] == 1 && surviveRule.Contains(neighbours))
        {
            return 1;
        }

        return 0;
    }

    int[,] Evolution(int[,] src)
    {
        int[,] ret = new int[src.GetUpperBound(0)+1, src.GetUpperBound(1)+1];
        for (int y = 0; y <= src.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= src.GetUpperBound(0); ++x)
            {
                ret[x,y] = ApplyRules(x,y,src);
            }
        }

        return ret;
    }

    private Vector2 FindStartPosition()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // first empty space is our start goal
                if (maze[x, y] == 0)
                {
                    return new Vector2(x * wallWidth, -y * wallWidth);
                }
            }
        }

        return new Vector2(-1, -1);
    }
}

