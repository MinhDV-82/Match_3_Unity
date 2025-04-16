using System.Collections.Generic;
using UnityEngine;
using CandyColor = BaseCandy.CandyColor;
public enum Direction
{
    Up,
    Right,
    Down,
    Left
}
public class Match3 : MonoBehaviour
{
    // public ArrayLayout DepTraiLayout;
    public static Match3 Instance => instance;
    private static Match3 instance;
    [SerializeField] StripedCandy spritedCandy;

    public enum State
    {
        Falling,
        Filling,
        Full
    }
    public int Width = 9;
    public int Height = 10;
    public GameObject TileBackground;
    public List<Candy> prefabCandys;

    public GameObject[,] BackGroundTiles;
    public BaseCandy[,] CandyTiles;
    System.Random random = new System.Random();
    private float _tileSize = 1f;

    private bool _isMatch = false;

    [Header("Logic Move")]
    [SerializeField] State _state = State.Full;
    private bool _isProcessingInput = false;
    [SerializeField] private TouchController _touchController;
    [SerializeField] private SpecialCandyManager _specialCandyManager;

    void Awake()
    {
        Application.targetFrameRate = 90;
        QualitySettings.vSyncCount = 0;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        // Init HandleCombo
        CandyCombo.InitMatch3(this);

        LoadTouchController();
        LoadSpecialCandyManager();
    }

    void LoadTouchController()
    {
        if (_touchController != null) return;
        _touchController = GameObject.FindFirstObjectByType<TouchController>();
    }

    void LoadSpecialCandyManager()
    {
        if (_specialCandyManager != null) return;
        _specialCandyManager = GameObject.FindFirstObjectByType<SpecialCandyManager>();
    }

    void Start()
    {
        BackGroundTiles = new GameObject[Width, Height];
        CandyTiles = new BaseCandy[Width, Height];

        InitializeBoard();
    }
    private void InitializeBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 pos = new Vector3(i * _tileSize, j * _tileSize, 0);

                BackGroundTiles[i, j] = Instantiate(TileBackground, pos, Quaternion.identity);
                BackGroundTiles[i, j].transform.parent = transform;

                CandyTiles[i, j] = CreateRandomCandy(pos);
            }
        }
        HandleMatch();
    }

    public BaseCandy CreateRandomCandy(Vector3 pos)
    {
        //! Tao cac candy binh thuong
        int randomIndexCandy = random.Next(0, 4);
        Candy candy = Instantiate(prefabCandys[randomIndexCandy], pos, Quaternion.identity);
        candy.transform.parent = transform;
        candy.transform.name = "x " + pos.x + "y " + pos.y;

        candy.Init((int)pos.x, (int)pos.y, prefabCandys[randomIndexCandy].GetCurrentCandyColor(), BaseCandy.CandyType.Regualar);
        return candy;
    }


    public bool IsValidCandy(BaseCandy a)
    {
        return a != null;
    }

    public Vector2Int GetRangeMatchInRow(int x, int y)
    {
        if (CandyTiles[x, y] == null)
        {
            Debug.Log(x + " " + y + "tile is null");
            return new Vector2Int(-1, -1);
        }

       CandyColor candyColor = CandyTiles[x, y].GetCurrentCandyColor();

        int top = x;
        int bottom = x;

        while (bottom >= 0 && IsValidPosition(bottom - 1, y) && IsValidCandy(CandyTiles[bottom - 1, y]) && CandyTiles[bottom - 1, y].GetCurrentCandyColor().Equals(candyColor))
        {
            bottom--;
        }

        while (top < Width && IsValidPosition(top + 1, y) && IsValidCandy(CandyTiles[top + 1, y]) && CandyTiles[top + 1, y].GetCurrentCandyColor().Equals(candyColor))
        {
            top++;
        }

        return new Vector2Int(bottom, top);
    }
    public int CountMatchInRow(int x, int y)
    {
        Vector2Int index = GetRangeMatchInRow(x, y);
        int count = index.y - index.x + 1;

        return count;
    }
    private bool CheckRow(int x, int y)
    {
        return CountMatchInRow(x, y) >= 3;
    }
    public Vector2Int GetRangeMatchInCol(int x, int y)
    {
        if (CandyTiles[x, y] == null)
        {
            Debug.Log(x + " " + y + "tile is null");
            return new Vector2Int(-1, -1);
        }

       CandyColor candyColor = CandyTiles[x, y].GetCurrentCandyColor();

        int bottom = y;
        int top = y;

        while (bottom >= 0 && IsValidPosition(x, bottom - 1) && IsValidCandy(CandyTiles[x, bottom - 1]) && CandyTiles[x, bottom - 1].GetCurrentCandyColor().Equals(candyColor))
        {
            bottom--;
        }

        while (top < Height && IsValidPosition(x, top + 1) && IsValidCandy(CandyTiles[x, top + 1]) && CandyTiles[x, top + 1].GetCurrentCandyColor().Equals(candyColor))
        {
            top++;
        }

        return new Vector2Int(bottom, top);
    }
    public int CountMatchInCol(int x, int y)
    {
        Vector2Int index = GetRangeMatchInCol(x, y);
        int count = index.y - index.x + 1;
        return count;
    }
    private bool CheckCol(int x, int y)
    {
        return CountMatchInCol(x, y) >= 3;
    }
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
    public bool CanMatch(int x, int y)
    {
        if (CheckRow(x, y) || CheckCol(x, y)) return true;
        return false;
    }

    public void HandleTouchMatch(BaseCandy a, BaseCandy b)
    {
        if (!_state.Equals(State.Full))
        {
            return;
        }
        _isMatch = false;

        if (a != null) HandleCandyTouchMatch(a);
        if (b != null) HandleCandyTouchMatch(b);

        if (_isMatch)
            _state = State.Falling;
    }
    private void HandleCandyTouchMatch(BaseCandy baseCandy)
    {
        Vector2Int baseCandyPos = baseCandy.GetCurrentPos();
        if (CheckRow(baseCandyPos.x, baseCandyPos.y) || CheckCol(baseCandyPos.x, baseCandyPos.y))
        {
            _isMatch = true;

            Vector2Int rangeRow = GetRangeMatchInRow(baseCandyPos.x, baseCandyPos.y);
            Vector2Int rangeCol = GetRangeMatchInCol(baseCandyPos.x, baseCandyPos.y);
            int countMatchInRow = CountMatchInRow(baseCandyPos.x, baseCandyPos.y);
            int countMatchInCol = CountMatchInCol(baseCandyPos.x, baseCandyPos.y);

           CandyColor candyColor = CommonUtils.GetCandyByColor(baseCandy);

            if (_specialCandyManager.HaveSpencialCandy(baseCandyPos.x, baseCandyPos.y, rangeRow, rangeCol, countMatchInRow, countMatchInCol))
            {
                CandyTiles[baseCandyPos.x, baseCandyPos.y] = _specialCandyManager.GetSpencialCandy(baseCandyPos.x, baseCandyPos.y, rangeRow, rangeCol, countMatchInRow, countMatchInCol, candyColor);
            }
            else
            {
                if (CheckRow(baseCandyPos.x, baseCandyPos.y))
                    for (int i = rangeRow.x; i <= rangeRow.y; i++)
                    {
                        if (CandyTiles[i, baseCandyPos.y] != null) CandyTiles[i, baseCandyPos.y].OnMatch();
                    }
                else
                    for (int j = rangeCol.x; j <= rangeCol.y; j++)
                    {
                        if (CandyTiles[baseCandyPos.x, j] != null) CandyTiles[baseCandyPos.x, j].OnMatch();
                    }
            }
        }

    }

    public void HandleMatch()
    {
        if (!_state.Equals(State.Full))
        {
            return;
        }
        _isMatch = false;

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (!IsValidCandy(CandyTiles[i, j])) continue;
                HandleCandyTouchMatch(CandyTiles[i, j]);
            }
        }

        if (_isMatch)
            _state = State.Falling;

    }

    void OnDrawGizmos()
    {
        float size = 1f;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Gizmos.color = Color.white;
                Vector3 pos = transform.position + new Vector3(size * i, size * j, 0);
                Gizmos.DrawWireCube(pos, new Vector3(size, size, 0));
            }
        }
    }
    public void HandleOnMouseDown(Vector2Int firstTouchPosition)
    {
        if (!_state.Equals(State.Full) || _isProcessingInput)
            return;

        _touchController.SetFirstPosition(firstTouchPosition);
    }

    public void HandleOnMouseUp(Vector2Int lastTouchPosition)
    {
        if (!_state.Equals(State.Full) || _isProcessingInput)
            return;

        _isProcessingInput = true;
        _touchController.SetLastTouchPosition(lastTouchPosition);
        StartCoroutine(_touchController.HandleTouchMove());
    }

    public State GetCurrentState()
    {
        return _state;
    }
    public void SetCurrentState(State state)
    {
        _state = state;
    }
    public void CompleteMove()
    {
        _isProcessingInput = false;
    }
    public void SetMatch(bool isMatch)
    {
        _isMatch = isMatch;
    }
    public void ApplyFalling() {
        _state = State.Falling;
    }
}
