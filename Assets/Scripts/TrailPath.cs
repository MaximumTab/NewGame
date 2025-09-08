using UnityEngine;

public class TrailPath : EnemyBehaviour
{
    public override void ReachedCheckPoint()
    {
        CheckProg++;
        if (Route.CheckPoints.Count != CheckProg)
        {
            NewRoute();
        }else if(!Leaked)
        {
            Leaked = true;
            rb.linearVelocity = Vector3.zero;
            //GameManager.LoseLife(((EnemyStats)entityStats).ObjectiveLives);
            Destroy(CoEntMan.gameObject);
            Destroy(gameObject);
        }
    }

    public override void CreateCollider()
    {
    }

    public override void CreateHpBar()
    {
    }
}
