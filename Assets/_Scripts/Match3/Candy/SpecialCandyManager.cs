using System.Collections.Generic;
using UnityEngine;
using CandyColor = BaseCandy.CandyColor;
using CandyType = BaseCandy.CandyType;
public class SpecialCandyManager : MonoBehaviour
{
    public List<StripedCandy> PrefabSpritedCandys;
    public List<BoomCandy> PrefabBoomCandys;

    [SerializeField] Match3 _match3;
    void Awake()
    {
        _match3 = FindFirstObjectByType<Match3>();
    }
    public bool HaveSpencialCandy(int x, int y, Vector2Int rangeRow, Vector2Int rangeCol, int countCandyRow, int countCandyCol)
    {
        return IsBoomCandy(x, y, rangeRow, rangeCol) || IsSpritedCandy(countCandyRow, countCandyCol);
    }

    public BaseCandy GetSpencialCandy(int x, int y, Vector2Int rangeRow, Vector2Int rangeCol, int countCandyRow, int countCandyCol, CandyColor candyColor)
    {
        BaseCandy spencialCandy = null;

        if (IsBoomCandy(x, y, rangeRow, rangeCol))
        {
            List<Vector2Int> range = GetRangeBoom(x, y, rangeRow, rangeCol);
            rangeRow = range[0];
            rangeCol = range[1];

            for (int i = rangeRow.x; i <= rangeRow.y; i++)
            {
                if (_match3.CandyTiles[i, y] != null) _match3.CandyTiles[i, y].OnMatch();
            }
            for (int j = rangeCol.x; j <= rangeCol.y; j++)
            {
                if (_match3.CandyTiles[x, j] != null) _match3.CandyTiles[x, j].OnMatch();
            }

            spencialCandy = CreateBoomCandy(x, y, candyColor);
        }
        else if (IsSpritedCandy(countCandyRow, countCandyCol))
        {
            bool isHorizontal = IsHorizontal(countCandyRow, countCandyCol);
            if (isHorizontal)
            {
                for (int i = rangeRow.x; i <= rangeRow.y; i++)
                {
                    if (_match3.CandyTiles[i, y] != null) _match3.CandyTiles[i, y].OnMatch();
                }
            }
            else
            {
                for (int j = rangeCol.x; j <= rangeCol.y; j++)
                {
                    if (_match3.CandyTiles[x, j] != null) _match3.CandyTiles[x, j].OnMatch();
                }
            }
            spencialCandy = CreateSpritedCandy(x, y, candyColor, isHorizontal);
        }

        return spencialCandy;
    }

    public StripedCandy CreateSpritedCandy(int x, int y, CandyColor candyColor, bool isHorizontal)
    {
        StripedCandy prefabStripedCandy = GetPrefabStripedCandyByColor(candyColor);
        StripedCandy StripedCandy = Instantiate(prefabStripedCandy, new Vector2(x, y), Quaternion.identity);
        StripedCandy.transform.name = "SpriteCandy" + x + "" + y;
        StripedCandy.transform.parent = _match3.transform;
        StripedCandy.Init(x, y, candyColor, CandyType.Striped, isHorizontal);

        return StripedCandy;
    }

    public BoomCandy CreateBoomCandy(int x, int y, CandyColor candyColor)
    {
        BoomCandy prefabBoomCandt = GetPrefabBoomCandyByColor(candyColor);
        BoomCandy boomCandy = Instantiate(prefabBoomCandt, new Vector2(x, y), Quaternion.identity);
        boomCandy.transform.name = "SpriteCandy" + x + "" + y;
        boomCandy.transform.parent = _match3.transform;
        boomCandy.Init(x, y, candyColor, CandyType.Boom);

        return boomCandy;
    }

    private BoomCandy GetPrefabBoomCandyByColor(CandyColor candyColor)
    {
        foreach (BoomCandy boomCandy in PrefabBoomCandys)
        {
            if (boomCandy.GetCurrentCandyColor().Equals(candyColor))
            {
                return boomCandy;
            }
        }
        return PrefabBoomCandys[0];
    }
    private StripedCandy GetPrefabStripedCandyByColor(CandyColor candyColor)
    {
        foreach (StripedCandy StripedCandy in PrefabSpritedCandys)
        {
            if (StripedCandy.GetCurrentCandyColor().Equals(candyColor))
            {
                return StripedCandy;
            }
        }
        return PrefabSpritedCandys[0];
    }



    private bool IsSpritedCandy(int countCandyRow, int countCandyCol)
    {
        return countCandyRow >= 4 || countCandyCol >= 4;
    }
    private bool IsHorizontal(int countCandyRow, int countCandyCol)
    {
        return countCandyRow >= 4;
    }
    private List<Vector2Int> GetRangeBoom(int x, int y, Vector2Int rangeRow, Vector2Int rangeCol)
    {
        List<Vector2Int> range = new();
        int CountInRow = _match3.CountMatchInRow(x, y);
        int CountInCol = _match3.CountMatchInCol(x, y);

        if (CountInRow >= 3 && CountInCol >= 3)
        {
            range.Add(rangeRow);
            range.Add(rangeCol);

            return range;
        }

        for (int i = rangeRow.x; i <= rangeRow.y; i++)
        {
            int countInCol = _match3.CountMatchInCol(i, y);
            if (CountInRow >= 3 && countInCol >= 3)
            {
                range.Add(rangeRow);
                range.Add(_match3.GetRangeMatchInCol(i, y));

                return range;
            }
        }
        for (int j = rangeCol.x; j <= rangeCol.y; j++)
        {
            int countInRow = _match3.CountMatchInRow(x, j);
            if (countInRow >= 3 && CountInCol >= 3)
            {
                range.Add(_match3.GetRangeMatchInRow(x, j));
                range.Add(rangeCol);
                return range;
            }
        }

        return range;
    }
    private bool IsBoomCandy(int x, int y, Vector2Int rangeRow, Vector2Int rangeCol)
    {
        return GetRangeBoom(x, y, rangeRow, rangeCol).Count > 0;
    }

}