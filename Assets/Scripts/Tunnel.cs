using System;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Tunnel : Path
{
    public List<Tunnel> ExitTunnel;
    public List<Tunnel> EnterTunnel;

    private void Update()
    {
        foreach (Tunnel exitBud in ExitTunnel)
        {
            if (!exitBud.EnterTunnel.Contains(this))
            {
                exitBud.EnterTunnel.Add(this);
            }
        }
    }
}
