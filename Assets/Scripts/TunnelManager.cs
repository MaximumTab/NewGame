using System.Collections;
using UnityEngine;

public static class TunnelManager
{
    public class CoTunManager: MonoBehaviour { }


    //Variable reference for the class
    private static CoTunManager CoTunManag;

    public static void GoingThrough(GameObject passenger,Vector3 exitPoint, float travelTime)
    {
        passenger.SetActive(false);
        if (!CoTunManag)
        {
            GameObject Coman = new GameObject("CoTunnelManager");
            CoTunManag=Coman.AddComponent<CoTunManager>();
        }

        CoTunManag.StartCoroutine(Through(passenger, exitPoint, travelTime));
    }

    private static IEnumerator Through(GameObject passenger, Vector3 exitPoint, float travelTime)
    {
        yield return new WaitForSeconds(travelTime);
        passenger.SetActive(true);
        passenger.transform.position = exitPoint+new Vector3(0,0.2f,0);
    }
}
