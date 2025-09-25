using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Databases/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyInfo> Enemies;
}
[Serializable]
public class EnemyInfo
{
    public GameObject EnemyPref;
    public Sprite EnemyPic;
    public string Description;
}