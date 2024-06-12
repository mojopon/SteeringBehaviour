using System.Collections.Generic;
using UnityEngine;

public class SteeringAlgorithm
{
    private float[] currentWeightMap = new float[Directions.AllDirections.Count];
    private int previousDirection = -1;

    // ���ς��g���đΏۂɍł��߂Â���ړ������ɋ߂����ɏd�݂Â�������
    public void ApplySeek(Vector2 self, Vector2 target, float intensity = 1f)
    {
        var weightMap = CalculateSeek(self, target, intensity);
        ApplyWeightMap(weightMap);
    }

    private float[] CalculateSeek(Vector2 self, Vector2 target, float intensity)
    {
        var weightMap = new float[Directions.AllDirections.Count];
        Vector2 displacement = target - self;
        for (int i = 0; i < weightMap.Length; i++)
        {
            float dot = Vector2.Dot(displacement.normalized, Directions.AllDirections[i]);
            dot = (dot + 1) * 0.5f;
            weightMap[i] = dot * intensity;
        }

        return weightMap;
    }

    // �Ώۂ��痣���ړ�������D��ŏd�݂Â�������
    public void ApplyFlee(Vector2 self, Vector2 target, float intensity = 1f)
    {
        var displacement = target - self;
        var opposite = self - displacement;
        var weights = CalculateSeek(self, opposite, intensity);
        ApplyWeightMap(weights);
    }

    // �Ώۂɕ��s�ړ�����悤�Ȉړ������̏d�݂Â�������
    public void ApplyStrife(Vector2 self, Vector2 target, float intensity = 0.1f)
    {
        var weightMap = new float[Directions.AllDirections.Count];
        Vector2 displacement = target - self;
        for (int i = 0; i < weightMap.Length; i++)
        {
            float dot = Vector2.Dot(displacement.normalized, Directions.AllDirections[i]);
            var modifier = 1.0f - Mathf.Pow(Mathf.Abs(dot + 0.25f), 2.0f);
            var result = (dot + 1) * 0.5f * modifier * intensity;
            weightMap[i] = result;
        }

        ApplyWeightMap(weightMap);
    }

    // 1�t���[���O�̈ړ����������Ƃ����d�݂Â�������
    public void ApplyBias(Vector2 self, int preferedDirection, float intensity = 0.3f)
    {
        if (preferedDirection != -1)
        {
            var displacement = Directions.AllDirections[preferedDirection];
            ApplySeek(self, self + displacement, intensity);
        }
    }

    public void ApplyBias(Vector2 self, float intensity = 0.3f)
    {
        ApplyBias(self, previousDirection, intensity);
    }

    // ��Q�����悯��d�݂Â����s��
    public void ApplyCollisionAvoidance(int[] blockedDirections)
    {
        foreach (var directionNumber in blockedDirections)
        {
            currentWeightMap[directionNumber] = Mathf.NegativeInfinity;
        }
    }

    // Neighbor(���L����)���痣���悤�Ȉړ����s�����߂̏d�݂Â����s��
    public void ApplySeparation(Vector2 self, List<Vector2> neighbors, float separationRange, float intensity = 8f)
    {
        if (neighbors.Count == 0 || 0 >= intensity) return;

        Vector2 away = Vector2.zero;


        foreach (var neighbor in neighbors)
        {
            var displacement = self - neighbor;
            var distance = Vector2.Distance(self, neighbor);

            if (0.05f > distance)
            {
                away += Directions.AllDirections[Random.Range(0, Directions.AllDirections.Count)];
            }
            else if (separationRange > distance)
            {
                var factor = 1.0f - (distance / separationRange);
                away += displacement.normalized * factor;
            }
        }

        if (away == Vector2.zero)
        {
            return;
        }
        else
        {
            ApplySeek(self, self + away, intensity);
        }
    }

    // �d�݂�K�p
    private void ApplyWeightMap(float[] weightMap)
    {
        if (weightMap.Length == 0) return;

        for (int i = 0; i < weightMap.Length; i++)
        {
            if (weightMap[i] == Mathf.NegativeInfinity)
            {
                continue;
            }

            if (weightMap[i] == Mathf.NegativeInfinity)
            {
                currentWeightMap[i] = Mathf.NegativeInfinity;
            }
            else
            {
                currentWeightMap[i] += weightMap[i];
            }
        }
    }

    // �d�݃}�b�v��������
    public void ClearCurrentWeightMap()
    {
        currentWeightMap = new float[Directions.AllDirections.Count];
    }

    // �ł��d�݂̑傫���ړ�������Ԃ�
    public Vector2 GetMovement()
    {
        int desiredDirection = GetDesiredDirectionNumber();

        if (desiredDirection != -1)
        {
            previousDirection = desiredDirection;
            return Directions.AllDirections[desiredDirection];
        }
        else
        {
            previousDirection = -1;
            return Vector2.zero;
        }
    }

    public int GetDesiredDirectionNumber()
    {
        int desiredDirection = -1;
        float max = 0f;
        for (int i = 0; i < currentWeightMap.Length; i++)
        {
            if (currentWeightMap[i] > max)
            {
                max = currentWeightMap[i];
                desiredDirection = i;
            }
        }

        return desiredDirection;
    }

    public float[] GetWeightMap()
    {
        return currentWeightMap;
    }
}

// 16�����̃x�N�g��
public static class Directions
{
    public static List<Vector2> AllDirections = new List<Vector2>{
            new Vector2(0,1).normalized,
            (new Vector2(0,1).normalized + new Vector2(1,1).normalized).normalized,
            new Vector2(1,1).normalized,
            (new Vector2(1,1).normalized + new Vector2(1,0).normalized).normalized,
            new Vector2(1,0).normalized,
            (new Vector2(1,0).normalized + new Vector2(1,-1).normalized).normalized,
            new Vector2(1,-1).normalized,
            (new Vector2(1,-1).normalized + new Vector2(0,-1).normalized).normalized,
            new Vector2(0,-1).normalized,
            (new Vector2(0,-1).normalized + new Vector2(-1,-1).normalized).normalized,
            new Vector2(-1,-1).normalized,
            (new Vector2(-1,-1).normalized + new Vector2(-1,0).normalized).normalized,
            new Vector2(-1,0).normalized,
            (new Vector2(-1,0).normalized + new Vector2(-1,1).normalized).normalized,
            new Vector2(-1,1).normalized,
            (new Vector2(-1,1).normalized + new Vector2(0,1).normalized).normalized,
        };
}
