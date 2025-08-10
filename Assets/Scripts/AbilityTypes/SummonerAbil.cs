using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon", menuName = "Abilities/Summoning")]
public class SummonerAbil : Ability
{
    public override void UseAbility(GameObject Target, Vector3 Source, float Atk)
    {
        Vector3 RandPos = new Vector3(Random.Range(-Range / 2, Range / 2), 0, Random.Range(-Range / 2, Range / 2));
        Debug.Log("You are using a summoner ability");
        GameObject newProj=Instantiate(Projectile, Source+RandPos, Quaternion.identity);
        if (newProj.GetComponent<EnemyBehaviour>())
        {
            EnemyBehaviour CurEnem= newProj.GetComponent<EnemyBehaviour>();
            CurEnem.Route = new TravelPoints();
            CurEnem.Route.CheckPoints = new List<Paths>();
            EnemyBehaviour ParentBehaviour = Target.GetComponent<EnemyBehaviour>();
            foreach (Paths path in ParentBehaviour.Route.CheckPoints)
            {
                CurEnem.Route.CheckPoints.Add(new Paths(path.Objective));
                CurEnem.Route.CheckPoints.Last().Path = new List<Vector3>();
                foreach (Vector3 Location in path.Path)
                {
                    CurEnem.Route.CheckPoints.Last().Path.Add(Location);
                }
            }
            CurEnem.OnSpawn();
            CurEnem.TunnelLocs = ParentBehaviour.TunnelLocs;
            CurEnem.TunnelTimes = ParentBehaviour.TunnelTimes;
            CurEnem.TunnelIndex = ParentBehaviour.TunnelIndex;
            CurEnem.SpawnSuccess = true;
        }
    }
}
