using UnityEngine;

public class TheWallBehaviour : EnemyBehaviour
{
    private float DamageReduction;
    public override void AlwaysRun()
    {
        base.AlwaysRun();
        if (Lives == 0 && PercHp < 0.9f)
        {
            DamageReduction = PercHp;
        }
        else if(Lives==0)
        {
            DamageReduction = 0.9f;
        }
    }

    public override void TakeDamage(float DamageTaken)
    {
        if (Lives == 0)
        {
            Hp -= DamageTaken;
        }
        else
        {
            Hp -= DamageTaken * (1 - DamageReduction);
        }
    }
}
