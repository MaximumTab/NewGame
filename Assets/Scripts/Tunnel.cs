using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class Tunnel : Path
{
    public List<Tunnel> ExitTunnel;
    public List<Tunnel> EnterTunnel;
}

/* [CustomEditor(typeof(Tunnel))]
public class TunnelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Tunnel thisTun = (Tunnel)target;
        foreach (Tunnel exitBud in thisTun.ExitTunnel)
        {
            if (!exitBud.EnterTunnel.Contains(thisTun))
            {
                exitBud.EnterTunnel.Add(thisTun);
            }
        }
        base.OnInspectorGUI();
    }
}
 */