using UnityEngine;

public abstract class BaseCandy : MonoBehaviour
{
    public enum CandyColor
    {
        Red,
        Yellow,
        Green,
        Blue,
        Orange,
        Purple
    }

    public enum CandyType
    {
        Regualar,
        Spencial
    }
    protected int _currentX;
    protected int _currentY;
    [SerializeField] protected CandyColor _CandyColor;
    [SerializeField] protected CandyType _candyType;
    public virtual void Init(int x, int y, CandyColor dotColor, CandyType candyType = CandyType.Regualar)
    {
        _currentX = x;
        _currentY = y;
        _CandyColor = dotColor;
        _candyType = candyType;
    }
    public virtual void Init(int x, int y, CandyColor CandyColor,  CandyType candyType = CandyType.Regualar, bool isHorizontal = false)
    {
        _currentX = x;
        _currentY = y;
        _CandyColor = CandyColor;
        _candyType = candyType;
    }

    #region Logic
    void OnMouseDown()
    {
        Match3.Instance.HandleOnMouseDown(new Vector2Int(_currentX, _currentY));
    }

    void OnMouseUp()
    {
        Vector3 mousePosition3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mousePosition2 = new Vector2Int((int)mousePosition3.x, (int)mousePosition3.y);
        Match3.Instance.HandleOnMouseUp(mousePosition2);
    }

    public virtual void OnMatch()
    {   
        Match3.Instance.CandyTiles[_currentX, _currentY] = null;
        ActivateEffect();
    }
    public virtual void ActivateEffect()
    {

    }
    #endregion


    #region Get Set
    public CandyColor GetCurrentCandyColor()
    {
        return _CandyColor;
    }
    public Vector2Int GetCurrentPos()
    {
        return new Vector2Int(_currentX, _currentY);
    }

    public void SetCandyColor(CandyColor CandyColor)
    {
        _CandyColor = CandyColor;
    }

    public void SetCurrentPos(int x, int y)
    {
        _currentX = x;
        _currentY = y;
    }
    #endregion 




}