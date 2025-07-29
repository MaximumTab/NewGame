using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<GameObject> Neighbours;
    public bool PauseLink;
    [SerializeField] private bool Active;
    
    private float sphereRadius = 1.0f;
    private Color sphereColorActive = Color.blue;
    private Color sphereColorInactive = Color.red;
    private Color lineColorActive = Color.green;
    private Color lineColorInactive = Color.red;
    private void OnDrawGizmos()
    {
        foreach (GameObject Neighbour in Neighbours)
        {
            Node NNode=Neighbour.GetComponent<Node>();
            if (!NNode.Neighbours.Contains(gameObject)&&!PauseLink)
            {
                NNode.Neighbours.Add(gameObject);
            }
        }

        Gizmos.color = Active?sphereColorActive:sphereColorInactive;
        Gizmos.DrawSphere(transform.position,sphereRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Active?lineColorActive:lineColorInactive;
        foreach(GameObject Neighbour in Neighbours)
        {
            Gizmos.DrawLine(Neighbour.transform.position, transform.position);
            
        }
    }
}
