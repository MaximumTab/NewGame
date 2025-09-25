using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyLayout : MonoBehaviour
{
    public DisplayEnemyStuffs DES;
    public EnemyDatabase ED;
    public List<GameObject> ShowObjs;
    public GameObject CopyObj;
    public GameObject ContentHolder;
    public Vector2 PositionChange;
    public void OnClick(int i)
    {
        DES.SelectedInfo = ED.Enemies[i];
    }

    private void CreateIndexes()
    {
        ShowObjs = new List<GameObject>();
        for (int i = 0; i < ED.Enemies.Count; i++)
        {
            GameObject Obj = Instantiate(CopyObj, ContentHolder.transform);
            RectTransform rect=Obj.GetComponent<RectTransform>();
            rect.localPosition=new Vector3((i % 4)*PositionChange.x+50,-Mathf.Floor(i/4f)*PositionChange.y-50);
            if (Obj.TryGetComponent(typeof(EnemyDataIndex), out Component EDI))
            {
                EnemyDataIndex enemyDataIndex=EDI as EnemyDataIndex;
                enemyDataIndex.index = i;
                enemyDataIndex.sprite.sprite = ED.Enemies[i].EnemyPic;
                enemyDataIndex.Layout = gameObject.GetComponent<EnemyLayout>();
            }

            ShowObjs.Add(Obj);
        }
    }

    private void Start()
    {
        CreateIndexes();
    }
}
