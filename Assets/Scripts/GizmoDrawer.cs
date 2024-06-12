using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
    [SerializeField]
    private bool drawGizmos = true;

    private float[] weightMap = new float[0];

    public void SetData(float[] weightMap) 
    {
        this.weightMap = weightMap;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (!drawGizmos) return;
            if (weightMap.Length == 0) return;

            Gizmos.color = Color.white;
            if (weightMap.Length == Directions.AllDirections.Count)
            {
                for (int i = 0; i < Directions.AllDirections.Count; i++)
                {
                    if (0 > weightMap[i]) continue;

                    Gizmos.DrawRay(transform.position, Directions.AllDirections[i] * weightMap[i] * 2);
                }
            }
        }

        weightMap = new float[0];
    }
}
