
using System.Collections.Generic;
using UnityEngine;


public class FillBoard : BaseComponetMatch3
{
    private float _gravityAcceleration = 9f;
    private float _maxGravityAcceleration = 18f;
    private class FallingTileInfo
    {
        public BaseCandy PCandy;
        public int SourceX, SourceY;
        public int DownY;
        public int TargetX, TargetY;
        public float Veclocity;
        public bool IsLanded;
    }

    private List<FallingTileInfo> _fallingCandys = new();
    private bool[,] _fallingTiles;

    void Start()
    {
        _fallingTiles = new bool[match3.Width + 1, match3.Height + 1];
    }
    void Update()
    {
        switch (match3.GetCurrentState())
        {
            case Match3.State.Falling:
                if (IsDoneFalling())
                {
                    match3.SetCurrentState(Match3.State.Full);
                    match3.HandleMatch();
                }
                else
                    ApplyGravityEffect();
                break;
        }
    }
    private void ApplyGravityEffect()
    {
        IdentifyFallingPieces();
        UpdateFallingPieces();
    }
    private void IdentifyFallingPieces()
    {
        _fallingCandys.RemoveAll(tile => tile.IsLanded);

        for (int i = 0; i < match3.Width; i++)
        {
            for (int j = 1; j < match3.Height; j++)
            {
                if (match3.CandyTiles[i, j] == null) continue;
                int lowestEmptyY = j;
                while (lowestEmptyY >= 0 && match3.IsValidPosition(i, lowestEmptyY - 1) && match3.CandyTiles[i, lowestEmptyY - 1] == null)
                {
                    lowestEmptyY--;
                }


                if (lowestEmptyY < j && !_fallingCandys.Exists(t => t.PCandy == match3.CandyTiles[i, j]))
                {
                    CommonUtils.Swap(ref match3.CandyTiles[i, j], ref match3.CandyTiles[i, lowestEmptyY]);

                    var newFalling = new FallingTileInfo
                    {
                        PCandy = match3.CandyTiles[i, lowestEmptyY],
                        SourceX = i,
                        SourceY = j,
                        DownY = j - 1,
                        TargetX = i,
                        TargetY = lowestEmptyY,
                        Veclocity = 0f,
                        IsLanded = false
                    };

                    _fallingCandys.Add(newFalling);
                }
            }
        }

        for (int i = 0; i < match3.Width; i++)
        {
            for (int j = 0; j < match3.Height; j++)
            {
                if (match3.CandyTiles[i, j] == null)
                {
                    Vector3 startPos = new Vector3(i, match3.Height, 0);
                    BaseCandy newCandy = match3.CreateRandomCandy(startPos);

                    newCandy.gameObject.SetActive(false);
                    match3.CandyTiles[i, j] = newCandy;

                    var newFalling = new FallingTileInfo
                    {
                        PCandy = newCandy,
                        SourceX = i,
                        SourceY = match3.Height,
                        DownY = match3.Height - 1,
                        TargetX = i,
                        TargetY = j,
                        Veclocity = 0f,
                        IsLanded = false,
                    };

                    _fallingCandys.Add(newFalling);
                }
            }

        }
    }

    private void UpdateFallingPieces()
    {
        foreach (var dotInfo in _fallingCandys)
        {
            if (dotInfo.IsLanded) continue;

            if (_fallingTiles[dotInfo.SourceX, dotInfo.DownY] && dotInfo.SourceY != dotInfo.DownY) //! Nhan biet candy khong thuoc o do 
            {
                continue;
            }
            if (!_fallingTiles[dotInfo.SourceX, dotInfo.DownY]) //! Lan dau init du lieu danh dau chu quyen
            {
                _fallingTiles[dotInfo.SourceX, dotInfo.DownY] = true;
                dotInfo.SourceY--;
            }

            Vector3 currentPos = dotInfo.PCandy.transform.position;

            dotInfo.Veclocity += _gravityAcceleration * Time.deltaTime;
            dotInfo.Veclocity = Mathf.Min(dotInfo.Veclocity, _maxGravityAcceleration);

            float newY = currentPos.y - dotInfo.Veclocity * Time.deltaTime;
            float targetY = dotInfo.TargetY;

            if (Mathf.Abs(newY - dotInfo.DownY) <= 0.4f)
            {
                dotInfo.PCandy.gameObject.SetActive(true);
            }

            if (Mathf.Abs(newY - dotInfo.DownY) <= 0.2f) //! Neu den duoc o do
            {
                _fallingTiles[dotInfo.SourceX, dotInfo.DownY] = false;
                dotInfo.DownY--;
            }

            if (Mathf.Abs(targetY - newY) <= 0.2f)
            {
                dotInfo.PCandy.transform.position = new Vector3(dotInfo.TargetX, dotInfo.TargetY, 0);
                dotInfo.PCandy.SetCurrentPos(dotInfo.TargetX, dotInfo.TargetY);

                dotInfo.IsLanded = true;

            }
            else
            {
                dotInfo.PCandy.transform.position = new Vector3(dotInfo.TargetX, newY, 0);
            }
        }
    }
    private bool IsDoneFalling()
    {
        if (_fallingCandys.Count > 0 && _fallingCandys.Exists(t => !t.IsLanded))
            return false;

        for (int i = 0; i < match3.Width; i++)
        {
            for (int j = match3.Height - 1; j >= 0; j--)
            {
                if (match3.CandyTiles[i, j] == null) return false; // Neu bang chua full
                if (match3.IsValidPosition(i, j - 1) && match3.CandyTiles[i, j] && match3.CandyTiles[i, j - 1] == null) return false;
            }
        }

        for (int i = 0; i < match3.Width + 1; i++)
        {
            for (int j = 0; j < match3.Height + 1; j++)
                _fallingTiles[i, j] = false;
        }

        return true;
    }
    // private void FillFullBoard()
    // {
    //     bool hasEmptySpaces = false;

    //     for (int i = 0; i < match3.Width; i++)
    //     {
    //         for (int j = 0; j < match3.Height; j++)
    //         {
    //             if (match3.CandyTiles[i, j] == null)
    //             {
    //                 hasEmptySpaces = true;

    //                 Vector3 startPos = new Vector3(i, match3.Height, 0);
    //                 BaseCandy newCandy = match3.CreateRandomCandy(startPos);

    //                 newCandy.gameObject.SetActive(false);
    //                 match3.CandyTiles[i, j] = newCandy;

    //                 var newFalling = new FallingTileInfo
    //                 {
    //                     PCandy = newCandy,
    //                     SourceX = i,
    //                     SourceY = match3.Height,
    //                     DownY = match3.Height - 1,
    //                     TargetX = i,
    //                     TargetY = j,
    //                     IsLanded = false,
    //                 };

    //                 _fallingCandys.Add(newFalling);
    //             }
    //         }

    //     }

    //     if (hasEmptySpaces)
    //     {
    //         match3.SetCurrentState(Match3.State.Falling);
    //     }
    //     else
    //     {
    //         match3.SetCurrentState(Match3.State.Full);
    //     }

    // }

}