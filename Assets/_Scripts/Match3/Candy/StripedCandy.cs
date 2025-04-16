using UnityEngine;

public class StripedCandy : BaseCandy
{
    [SerializeField] Sprite _horizontalSprite;
    [SerializeField] Sprite _verticalSprite;
    [SerializeField] SpriteRenderer _spriteRenderer;

    private bool _isHorizontal = false;

    public override void Init(int x, int y, CandyColor candyColor, CandyType candyType, bool isHorizontal)
    {
        base.Init(x, y, candyColor, candyType, isHorizontal);
        if (_isHorizontal)
        {
            _spriteRenderer.sprite = _horizontalSprite;
        }
        else
        {
            _spriteRenderer.sprite = _verticalSprite;
        }
    }

    public override void ActivateEffect()
    {
        base.ActivateEffect();
        if (_isHorizontal)
        {
            for (int i = 0; i < Match3.Instance.Width; i++)
            {


                if (Match3.Instance.CandyTiles[i, _currentY] == null)
                {
                    EffectManager.Instance.PlayEffect(EffectType.NormalMatch, i, _currentY);
                    continue;
                }
                if (i == _currentX) continue;

                Match3.Instance.CandyTiles[i, _currentY].OnMatch();
            }
        }
        else
        {
            for (int j = 0; j < Match3.Instance.Height; j++)
            {
                if (Match3.Instance.CandyTiles[_currentX, j] == null) continue;
                if (j == _currentY) continue;

                Match3.Instance.CandyTiles[_currentX, j].OnMatch();
            }
        }

        Destroy(this.gameObject);
    }
}