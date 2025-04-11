using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] StrippedCandy spritedCandy;

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
        Application.targetFrameRate = 90; // Hoặc 30, 90 tuỳ ý
        QualitySettings.vSyncCount = 0;   // Tắt VSync để targetFrameRate có hiệu lực
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

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
        //! Tao cac dot binh thuong
        int randomIndexCandy = random.Next(0, 3);
        Candy dot = Instantiate(prefabCandys[randomIndexCandy], pos, Quaternion.identity);
        dot.transform.parent = transform;
        dot.transform.name = "x " + pos.x + "y " + pos.y;

        dot.Init((int)pos.x, (int)pos.y, prefabCandys[randomIndexCandy].GetCurrentCandyColor(), BaseCandy.CandyType.Regualar);

        return dot;
    }

  
    public bool IsValidCandy(BaseCandy a)
    {
        return a != null;
    }

    private Vector2Int GetIndexMatchInRow(int x, int y)
    {
        if (CandyTiles[x, y] == null)
        {
            Debug.Log(x + " " + y + "tile is null");
            return new Vector2Int(-1, -1);
        }

        BaseCandy.CandyColor dotType = CandyTiles[x, y].GetCurrentCandyColor();

        int top = x;
        int bottom = x;

        while (bottom >= 0 && IsValidPosition(bottom - 1, y) && IsValidCandy(CandyTiles[bottom - 1, y]) && CandyTiles[bottom - 1, y].GetCurrentCandyColor().Equals(dotType))
        {
            bottom--;
        }

        while (top < Width && IsValidPosition(top + 1, y) && IsValidCandy(CandyTiles[top + 1, y]) && CandyTiles[top + 1, y].GetCurrentCandyColor().Equals(dotType))
        {
            top++;
        }

        return new Vector2Int(bottom, top);
    }
    private int CountMatchInRow(int x, int y)
    {
        Vector2Int index = GetIndexMatchInRow(x, y);
        int count = index.y - index.x + 1;

        return count;
    }
    private bool CheckRow(int x, int y)
    {
        return CountMatchInRow(x, y) >= 3;
    }
    private Vector2Int GetIndexMatchInCol(int x, int y)
    {
        if (CandyTiles[x, y] == null)
        {
            Debug.Log(x + " " + y + "tile is null");
            return new Vector2Int(-1, -1);
        }

        BaseCandy.CandyColor dotType = CandyTiles[x, y].GetCurrentCandyColor();

        int bottom = y;
        int top = y;

        while (bottom >= 0 && IsValidPosition(x, bottom - 1) && IsValidCandy(CandyTiles[x, bottom - 1]) && CandyTiles[x, bottom - 1].GetCurrentCandyColor().Equals(dotType))
        {
            bottom--;
        }

        while (top < Height && IsValidPosition(x, top + 1) && IsValidCandy(CandyTiles[x, top + 1]) && CandyTiles[x, top + 1].GetCurrentCandyColor().Equals(dotType))
        {
            top++;
        }

        return new Vector2Int(bottom, top);
    }
    private int CountMatchInCol(int x, int y)
    {
        Vector2Int index = GetIndexMatchInCol(x, y);
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
        if (!_state.Equals(State.Full) || _isProcessingInput)
        {
            return;
        }
        _isMatch = false;

        HandleCandyTouchMatch(a);
        HandleCandyTouchMatch(b);

        if (_isMatch)
            _state = State.Falling;
    }
    private void HandleCandyTouchMatch(BaseCandy BaseCandy)
    {

        Vector2Int BaseCandyPos = BaseCandy.GetCurrentPos();
        if (CheckRow(BaseCandyPos.x, BaseCandyPos.y) || CheckCol(BaseCandyPos.x, BaseCandyPos.y))
        {
            _isMatch = true;

            Vector2Int rangeRow = GetIndexMatchInRow(BaseCandyPos.x, BaseCandyPos.y);
            Vector2Int rangeCol = GetIndexMatchInCol(BaseCandyPos.x, BaseCandyPos.y);
            int countMatchInRow = CountMatchInRow(BaseCandyPos.x, BaseCandyPos.y);
            int countMatchInCol = CountMatchInCol(BaseCandyPos.x, BaseCandyPos.y);

        
         Debug.Log("Count" + countMatchInRow + "  " + countMatchInCol);
         
            BaseCandy.CandyColor candyColor = CommonUtils.GetCandyByColor(BaseCandy);

            if (_specialCandyManager.HaveSpencialCandy(countMatchInRow, countMatchInCol))
            {
                CandyTiles[BaseCandyPos.x, BaseCandyPos.y] = _specialCandyManager.GetSpencialCandy(BaseCandyPos.x, BaseCandyPos.y, rangeRow, rangeCol, countMatchInRow, countMatchInCol, candyColor);
            }
            else
            {
                if (CheckRow(BaseCandyPos.x, BaseCandyPos.y))
                    for (int i = rangeRow.x; i <= rangeRow.y; i++)
                    {
                        if (CandyTiles[i, BaseCandyPos.y] != null) CandyTiles[i, BaseCandyPos.y].OnMatch();
                    }
                else
                    for (int j = rangeCol.x; j <= rangeCol.y; j++)
                    {
                        if (CandyTiles[BaseCandyPos.x, j] != null) CandyTiles[BaseCandyPos.x, j].OnMatch();
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
}
