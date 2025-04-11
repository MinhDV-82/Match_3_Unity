using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using CandyColor = BaseCandy.CandyColor;
using CandyType = BaseCandy.CandyType;
public class SpecialCandyManager : MonoBehaviour
{
    public List<StrippedCandy> PrefabSpritedCandys;
    public List<BoomCandy> PrefabBoomCandys;

    [SerializeField] Match3 _match3;
    void Awake()
    {
        _match3 = FindFirstObjectByType<Match3>();
    }
    public bool HaveSpencialCandy(int countCandyRow, int countCandyCol)
    {
        if (countCandyRow > countCandyCol)
        {
            CommonUtils.Swap(ref countCandyRow, ref countCandyCol);
        }

        return IsBoomCandy(countCandyRow, countCandyCol) || IsSpritedCandy(countCandyRow, countCandyCol);
    }

    public BaseCandy GetSpencialCandy(int x, int y, Vector2Int rangeRow, Vector2Int rangeCol, int countCandyRow, int countCandyCol, CandyColor candyColor)
    {
        BaseCandy spencialCandy = null;

        if (IsSpritedCandy(countCandyRow, countCandyCol))
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
        else if (IsBoomCandy(countCandyRow, countCandyCol))
        {
            if (countCandyRow > countCandyCol)
            {
                CommonUtils.Swap(ref countCandyRow, ref countCandyCol);
            }
            
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

        return spencialCandy;
    }

    private StrippedCandy CreateSpritedCandy(int x, int y, CandyColor candyColor, bool isHorizontal)
    {
        StrippedCandy prefabStrippedCandy = GetPrefabStrippedCandyByColor(candyColor);
        StrippedCandy strippedCandy = Instantiate(prefabStrippedCandy, new Vector2(x, y), Quaternion.identity);
        strippedCandy.transform.name = "SpriteCandy" + x + "" + y;
        strippedCandy.Init(x, y, candyColor, CandyType.Spencial, isHorizontal);

        return strippedCandy;
    }

    private BoomCandy CreateBoomCandy(int x, int y, CandyColor candyColor)
    {
        BoomCandy prefabBoomCandt = GetPrefabBoomCandyByColor(candyColor);
        BoomCandy boomCandy = Instantiate(prefabBoomCandt, new Vector2(x, y), Quaternion.identity);
        boomCandy.transform.name = "SpriteCandy" + x + "" + y;
        boomCandy.Init(x, y, candyColor, CandyType.Spencial);

        return boomCandy;
    }

    private BoomCandy GetPrefabBoomCandyByColor(CandyColor candyColor) {
        foreach (BoomCandy boomCandy in PrefabBoomCandys) {
            if (boomCandy.GetCurrentCandyColor().Equals(candyColor)) {
                return boomCandy;
            }
        }
        return PrefabBoomCandys[0];
    }
    private StrippedCandy GetPrefabStrippedCandyByColor(CandyColor candyColor) {
        foreach (StrippedCandy strippedCandy in PrefabSpritedCandys) {
            if (strippedCandy.GetCurrentCandyColor().Equals(candyColor)) {
                return strippedCandy;
            }
        }
        return PrefabSpritedCandys[0];
    }



    private bool IsSpritedCandy(int countCandyRow, int countCandyCol)
    {
        return countCandyRow == 4 && countCandyCol == 0 || countCandyCol == 4 && countCandyRow == 0;
    }
    private bool IsHorizontal(int countCandyRow, int countCandyCol)
    {
        return countCandyRow >= 4 ;
    }
    private bool IsBoomCandy(int countCandyRow, int countCandyCol)
    {
        return countCandyRow >= 3 && countCandyCol >= 3;
    }

}