using UnityEngine;

public class BoomCandy : BaseCandy
{

    public override void Init(int x, int y, CandyColor candyColor, CandyType candyType)
    {
        base.Init(x, y, candyColor, candyType);
    }

    public override void ActivateEffect()
    {
        base.ActivateEffect();
        for (int i = _currentX - 1; i < _currentX + 3; i++) {
            for (int j = _currentY - 1; j < _currentY + 3; j++) {
                // Tranh goi bi de quy
                if (i == _currentX && j == _currentY) continue;

                if (Match3.Instance.IsValidPosition(i, j) && Match3.Instance.CandyTiles[i, j] != null) {
                    Match3.Instance.CandyTiles[i, j].OnMatch();
                }
            }
        }

        Destroy(this.gameObject);
    }
}