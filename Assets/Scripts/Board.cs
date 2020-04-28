using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public Type[,] board;

    public int width, height;

    int[,] checkDeltas = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

    public Board(int width, int height, int mines, int x, int y)
    {
        Initialize(width, height, mines, x, y);
    }

    void Initialize(int width, int height, int mines, int x, int y)
    {
        this.width = width;
        this.height = height;

        board = new Type[height, width];

        for (int y_ = 0; y_ < height; y_++)
        {
            for (int x_ = 0; x_ < width; x_++)
            {
                board[y_, x_] = Type.None;
            }
        }

        PlantMines(mines, x, y);
        MarkNumbers();
    }

    void PlantMines(int mines, int x, int y)
    {
        int mineCount = 0;

        while (mineCount < mines)
        {
            int rx = Random.Range(0, width);
            int ry = Random.Range(0, height);

            bool flag = false;
            for (int i = 0; i < 8; i++)
            {
                int x_ = rx + checkDeltas[i, 0];
                int y_ = ry + checkDeltas[i, 1];

                if (x_ == x || y_ == y)
                {
                    flag = true;
                    break;
                }
            }

            if (GetType(rx, ry) != Type.Mine && !flag)
            {
                board[ry, rx] = Type.Mine;

                mineCount++;
            }
        }
    }

    public void Print()
    {
        string text = "";

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                text += board[y, x] + "\t";
            }
            text += "\n";
        }

        Debug.Log(text);
    }

    void MarkNumbers()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (GetType(x, y) == Type.Mine) continue;

                for(int i = 0; i < 8; i++)
                {
                    int x_ = x + checkDeltas[i, 0];
                    int y_ = y + checkDeltas[i, 1];

                    if (GetType(x_, y_) == Type.Mine)
                    {
                        board[y, x]++;
                    }
                }
            }
        }
    }

    public Type GetType(int x, int y)
    {
        if (x >= width || x < 0 || y >= height || y < 0)
        {
            return Type.Null;
        }

        return board[y, x];
    }
}
