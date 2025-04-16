
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using CandyType = BaseCandy.CandyType;
public static class CandyCombo
{
    private static readonly Dictionary<(CandyType, CandyType), Func<BaseCandy, BaseCandy, IEnumerator>> comboActions = new()
    {
        // ! ColorBoom
        {
        (CandyType.ColorBoom, CandyType.ColorBoom),
        (candyA, candyB) => InteractionColorBoomWithColorBoom(candyA, candyB)
        },
        {
        (CandyType.ColorBoom, CandyType.Boom),
        (candyA, candyB) => InteractionColorBoomWithBoom(candyA, candyB)
        },
        {
        (CandyType.ColorBoom, CandyType.Striped),
        (candyA, candyB) => InteractionColorBoomWithStriped(candyA, candyB)
        },
        {
        (CandyType.ColorBoom, CandyType.Regualar),
        (candyA, candyB) =>InteractionColorBoomWithRegular(candyA, candyB)
        },
        //! Boom 
        {
        (CandyType.Boom, CandyType.Boom),
        (candyA, candyB) => InteractionBoomWithBoom(candyA, candyB)
        },
        {
        (CandyType.Boom, CandyType.Striped),
        (candyA, candyB) => InteractionBoomWithStriped(candyA, candyB)
        },
        {
        (CandyType.Boom, CandyType.Regualar),
        (candyA, candyB) => InteractionBoomWithRegular(candyA, candyB)
        },
        //! Striped 
        {
        (CandyType.Striped, CandyType.Striped),
        (candyA, candyB) => InteractionStripedWithStriped(candyA, candyB)
        },
        {
        (CandyType.Striped, CandyType.Regualar),
        (candyA, candyB) => InteractionStripedWithRegular(candyA, candyB)
        },
    };



    private static Match3 _match3;
    public static void InitMatch3(Match3 match3)
    {
        _match3 = match3;
    }

    public static IEnumerator HandleCombo(MonoBehaviour context, BaseCandy candyA, BaseCandy candyB)
    {
        var key = GetSortedKey(candyA, candyB);
        if (comboActions.TryGetValue(key, out var action))
        {
            yield return context.StartCoroutine(action.Invoke(candyA, candyB));
        }
        else
        {
            Debug.LogWarning("Khong co combo " + candyA.GetCurrentType() + " " + candyB.GetCurrentType());
        }
        yield break;
    }

    public static IEnumerator InteractionBase(BaseCandy candyA, BaseCandy candyB)
    {
        // ! Lay gia tri cua pos dot hien tai roi di chuyen 
        Vector2Int aPos = candyA.GetCurrentPos();
        Vector2Int bPos = candyB.GetCurrentPos();

        candyA.transform.DOMove(new Vector3(bPos.x, bPos.y, 0), 0.15f).SetEase(Ease.Linear);
        candyB.transform.DOMove(new Vector3(aPos.x, aPos.y, 0), 0.15f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(.2f);

        _match3.CandyTiles[aPos.x, aPos.y].SetCurrentPos(bPos.x, bPos.y);
        _match3.CandyTiles[bPos.x, bPos.y].SetCurrentPos(aPos.x, aPos.y);

        CommonUtils.Swap(ref _match3.CandyTiles[aPos.x, aPos.y], ref _match3.CandyTiles[bPos.x, bPos.y]);
    }

    private static (CandyType, CandyType) GetSortedKey(BaseCandy candyA, BaseCandy candyB)
    {
        int GetPriority(CandyType c) =>
        c is CandyType.ColorBoom ? 4 :
        c is CandyType.Boom ? 3 :
        c is CandyType.Striped ? 2 :
        c is CandyType.Regualar ? 1 : 0;
        var types = new[] { candyA.GetCurrentType(), candyB.GetCurrentType() }
                    .OrderByDescending(t => GetPriority(t))
                    .ToArray();
        return (types[0], types[1]);
    }

    #region Interaction ColorBoom

    private static IEnumerator InteractionColorBoomWithColorBoom(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        _match3.HandleTouchMatch(candyA, candyB);
        _match3.ApplyFalling();

        yield break;
    }
    private static IEnumerator InteractionColorBoomWithBoom(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        _match3.HandleTouchMatch(candyA, candyB);
        _match3.ApplyFalling();

        yield break;
    }
    private static IEnumerator InteractionColorBoomWithStriped(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        _match3.HandleTouchMatch(candyA, candyB);
        _match3.ApplyFalling();

        yield break;
    }
    private static IEnumerator InteractionColorBoomWithRegular(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        _match3.HandleTouchMatch(candyA, candyB);
        _match3.ApplyFalling();

        yield break;
    }
    #endregion 

    #region  Interaction Boom
    private static IEnumerator InteractionBoomWithBoom(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        candyA.OnMatch();
        _match3.HandleTouchMatch(candyA, candyB);
        _match3.ApplyFalling();
        yield break;
    }
    private static IEnumerator InteractionBoomWithStriped(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        candyA.OnMatch();

        _match3.ApplyFalling();
        _match3.HandleTouchMatch(candyA, candyB);
        
        yield break;

    }
    private static IEnumerator InteractionBoomWithRegular(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        candyA.OnMatch();
        _match3.HandleTouchMatch(candyA, candyB);

        yield break;

    }

    #endregion 

    #region  Interaction Striped
    private static IEnumerator InteractionStripedWithStriped(BaseCandy candyA, BaseCandy candyB)
    {
        yield return InteractionBase(candyA, candyB);
        candyA.OnMatch();
        if (candyB != null) candyB.OnMatch();
        _match3.HandleTouchMatch(candyA, candyB);
        _match3.ApplyFalling();
        
        yield break;
    }

    private static IEnumerator InteractionStripedWithRegular(BaseCandy candyA, BaseCandy candyB)
    {
        _match3.HandleTouchMatch(candyA, candyB);

        yield break;
    }
    #endregion

}