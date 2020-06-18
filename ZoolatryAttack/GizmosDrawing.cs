using UnityEngine;

public static class UtilitiesZoolatry 
{

    public static void DrawGizmoDisk(this Transform t,float radius)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f,0.2f,0.2f,0.5f); //this is gray, could be anything
        Gizmos.matrix = Matrix4x4.TRS(t.position,t.rotation,new Vector3(1,.01f,1));
        Gizmos.DrawSphere(Vector3.zero,radius);
        Gizmos.matrix = oldMatrix;
    }

  
}
