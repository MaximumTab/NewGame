using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Entities/EnemyStats")]
public class EnemyStats : EntityStats
{
    public EnemyType Type;
    public int ObjectiveLives=1;
    public int Lives = 1;
    public List<float> MaxHpMod=new List<float>();
    public List<float> AtkMod=new List<float>();
    public List<float> SpdMod=new List<float>();
    public List<float> AtkspdMod=new List<float>();
    public List<float> DownTime=new List<float>();
    public List<MoveType> StageAmounts;
    public List<Vector3> StationaryHBoxSize;
    public List<Vector3> StationaryHBoxCenter;
    public List<float> StationaryAliveTime=new List<float>();
    
    public void CompFix<T>(int ImportantComp, List<T> ArrayChange, T DefValue)
    {
        if (ImportantComp < ArrayChange.Count)
        {
            ArrayChange.Remove(ArrayChange.Last());
        }
        else if(ImportantComp!=ArrayChange.Count)
        {
            ArrayChange.Add(DefValue);
        }
    }

    public void fixArrays()
    {
        CompFix(Lives,MaxHpMod,1);
        CompFix(Lives,AtkMod,1);
        CompFix(Lives,SpdMod,1);
        CompFix(Lives,AtkspdMod,1);
        CompFix(Lives,StageAmounts,EnemyStats.MoveType.Moving);
    }

    public enum EnemyType
    {
        Basic,
        Elite,
        Boss
    }

    public enum MoveType
    {
        Moving,
        Stationary
    }
}

/* [CustomEditor(typeof(EnemyStats))]
public class EnemyStatEditor : Editor
{
    public void CompFix<T>(int ImportantComp, List<T> ArrayChange, T DefValue)
    {
        if (ImportantComp < ArrayChange.Count)
        {
            ArrayChange.Remove(ArrayChange.Last());
        }
        else if(ImportantComp!=ArrayChange.Count)
        {
            ArrayChange.Add(DefValue);
        }
    }

    public override void OnInspectorGUI()
    {
        EnemyStats EStats = (EnemyStats)target;
        try
        {
            if (EStats.Lives < 1)
            {
                EStats.Lives = 1;
            }

            CompFix(EStats.Lives, EStats.MaxHpMod, 1);
            CompFix(EStats.Lives, EStats.AtkMod, 1);
            CompFix(EStats.Lives, EStats.SpdMod, 1);
            CompFix(EStats.Lives, EStats.AtkspdMod, 1);
            CompFix(EStats.Lives, EStats.StageAmounts, EnemyStats.MoveType.Moving);

            int allStation = EStats.StageAmounts.FindAll(type => type == EnemyStats.MoveType.Stationary).Count;
            CompFix(allStation, EStats.StationaryHBoxCenter, Vector3.zero);
            CompFix(allStation, EStats.StationaryHBoxSize, Vector3.one);
            CompFix(allStation, EStats.StationaryAliveTime, 30);
            CompFix(EStats.Lives - 1, EStats.DownTime, 0);
        }
        catch
        {
        }

        base.OnInspectorGUI();
    }
}
 */