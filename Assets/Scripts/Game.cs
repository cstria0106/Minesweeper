using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance
    {
        get
        {
            return FindObjectOfType<Game>();
        }
    }

    public Board board;
    public Block[,] blocks;

    public GameObject blockPrefab;

    int[,] checkDeltas = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

    int width = 10;
    int height = 10;
    int mines = 9;

    int opennedBlocks = 0;

    bool started = false;
    public bool gameOver = false;

    public void Load(int width, int height, int mine)
    {
        StartCoroutine(LoadRoutine(width, height, mine));
    }

    IEnumerator LoadRoutine(int width, int height, int mine)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game");

        while (!async.isDone)
            yield return new WaitForEndOfFrame();

        Game.Instance.Initialize(width, height, mine);
    }

    public void Initialize(int width, int height, int mine)
    {
        this.width = width;
        this.height = height;
        this.mines = mine;

        GenerateBlocks();
        CameraManager.Instance.InitializeSize(width, height);
    }

    public void StartGame(int x, int y)
    {
        started = true;

        board = new Board(width, height, mines, x, y);

        UpdateBlocks();
        
        OpenBlock(x, y);

        Camera.main.GetComponent<CameraShaker>().Shake(-0.5f, 0.5f, 0.5f);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void GenerateBlocks()
    {
        blocks = new Block[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                blocks[y, x] = Instantiate(blockPrefab).GetComponent<Block>();
                blocks[y, x].Initialize((x + y) % 2, Type.None);

                Vector3 pos = new Vector3();
                pos.x = x * 0.64f - ((width - 1) * 0.64f) / 2f;
                pos.y = -y * 0.64f + ((height - 1) * 0.64f) / 2f;

                blocks[y, x].transform.position = pos;
                blocks[y, x].x = x;
                blocks[y, x].y = y;

                blocks[y, x].transform.SetParent(gameObject.transform);
            }
        }
    }

    void InitializeBlocks()
    {
        blocks = new Block[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                blocks[y, x] = Instantiate(blockPrefab).GetComponent<Block>();
                blocks[y, x].Initialize((x + y) % 2, board.GetType(x, y));

                Vector3 pos = new Vector3();
                pos.x = x * 0.64f - ((width - 1) * 0.64f) / 2f;
                pos.y = -y * 0.64f + ((height - 1) * 0.64f) / 2f;

                blocks[y, x].transform.position = pos;
                blocks[y, x].x = x;
                blocks[y, x].y = y;

                blocks[y, x].transform.SetParent(gameObject.transform);
            }
        }
    }

    void UpdateBlocks()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                blocks[y, x].Initialize((x + y) % 2, board.GetType(x, y));
            }
        }
    }

    public bool OpenBlock(int x, int y, bool sound = false)
    {
        if (!started)
        {
            if (blocks[y, x].flagged)
                return false;

            SoundManager.Instance.PlayOpenSound();
            StartGame(x, y);
        }

        if (x >= board.width || x < 0 || y >= board.height || y < 0)
        {
            return false;
        }

        if(blocks[y, x].opened || blocks[y, x].flagged)
        {
            return false;
        }

        if (sound) SoundManager.Instance.PlayOpenSound();

        if (blocks[y, x].Open())
            opennedBlocks++;

        if (opennedBlocks == width * height - mines)
            Clear();

        if(board.GetType(x, y) == Type.None)
        {
            for (int i = 0; i < 8; i++)
            {
                int x_ = x + checkDeltas[i, 0];
                int y_ = y + checkDeltas[i, 1];

                OpenBlock(x_, y_);
            }
        }

        if(board.GetType(x, y) == Type.Mine)
        {
            GameOver(x, y);
        }

        return true;
    }

    public bool OpenAround(int x, int y)
    {
        if (board.GetType(x, y) <= Type.None || board.GetType(x, y) > Type.Mine)
            return false;

        if (!blocks[y, x].opened)
            return false;

        int flagCount = 0;

        for (int i = 0; i < 8; i++)
        {
            int x_ = x + checkDeltas[i, 0];
            int y_ = y + checkDeltas[i, 1];

            if (x_ >= width || x_ < 0 || y_ >= height || y_ < 0)
            {
                continue;
            }

            if (blocks[y_, x_].flagged) flagCount++;
        }

        if (flagCount == (int)board.GetType(x, y))
        {
            bool flag = false;
            for (int i = 0; i < 8; i++)
            {
                int x_ = x + checkDeltas[i, 0];
                int y_ = y + checkDeltas[i, 1];

                if (OpenBlock(x_, y_))
                    flag = true;
            }

            if(flag) SoundManager.Instance.PlayOpenSound();
        }

        return true;
    }

    public void HoverAround(int x, int y)
    {
        for (int i = 0; i < 8; i++)
        {
            int x_ = x + checkDeltas[i, 0];
            int y_ = y + checkDeltas[i, 1];

            if (x_ >= width || x_ < 0 || y_ >= height || y_ < 0)
            {
                continue;
            }

            if (blocks[y_, x_].flagged)
                continue;

            blocks[y_, x_].hovered = true;
        }

    }

    public void GameOver(int x, int y)
    {
        gameOver = true;
        StartCoroutine(GameOverRoutine(x, y));
    }

    IEnumerator GameOverRoutine(int x, int y)
    {
        blocks[y, x].burnt = true;

        if (board.GetType(x, y) == Type.Mine)
        {
            if (blocks[y, x].flagged) blocks[y, x].Flag();
            blocks[y, x].Open();
        }

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 8; i++)
        {
            int x_ = x + checkDeltas[i, 0];
            int y_ = y + checkDeltas[i, 1];

            if (x_ < 0 || x_ >= width || y_ < 0 || y_ >= height) continue;

            if(!blocks[y_, x_].burnt)
                StartCoroutine(GameOverRoutine(x_, y_));
        }
    }

    public void Clear()
    {

    }
}
