using System.Collections.Generic;
using System.Numerics;

public class Candy : BaseCandy
{
    
    public override void ActivateEffect()
    {
        base.ActivateEffect();
        EffectManager.Instance.PlayEffect(EffectType.NormalMatch,transform);
        
        Destroy(this.gameObject);
    }
}