using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private Detector detector;
    [SerializeField]
    private GizmoDrawer gizmoDrawer;

    public float Speed = 1.5f;
    public float PursueOffset = 0.25f;
    public float PursueDistanceMax = 1.2f;
    public float PursueDistanceMin = 0.8f;
    public float CircleCastSize = 0.25f;
    public float CircleCastRange = 1f;
    public float StrifeFactor = 0.1f;
    public float BiasStrength = 0.1f;
    public float SeparationRange = 0.5f;

    private Rigidbody2D rb2d;
    private SteeringAlgorithm algorithm = new SteeringAlgorithm();
    private Vector2 movement;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // 重みのリセット
        algorithm.ClearCurrentWeightMap();

        var self = transform.position;
        var target = targetTransform.position;
        
        // 障害物がある場合はよける重みづけを行う
        // 障害物の検知はDetectorで行い、ブロックされた方向の情報をSteeringAlgorithmに渡す
        var blockedDirections = detector.DetectAllCollisionsWithCircleCast(self, CircleCastSize, CircleCastRange);
        algorithm.ApplyCollisionAvoidance(blockedDirections.ToArray());

        var distance = Vector2.Distance(self, target);
        // 対象までの距離がPursueDistanceMaxより大きい場合は、対象に近づく＆対象から横に平行移動する重みづけを行う
        if (distance > PursueDistanceMax)
        {
            algorithm.ApplySeek(self, target);
            algorithm.ApplyStrife(self, target, StrifeFactor);
        }
        // PursueDistanceMinより距離が近い場合は、対象から離れる重みづけを行う
        else if (PursueDistanceMin > distance)
        {
            algorithm.ApplyFlee(self, target);
        }
        // それ以外は対象から横に並行移動する重みづけを行う
        else
        {
            algorithm.ApplyStrife(self, target, StrifeFactor);
        }

        // 前回移動した移動方向を軸に重みづけを行う（スタックの防止）
        algorithm.ApplyBias(self);

        // 近距離の他キャラから離れる重みづけを行う
        var neighbors = detector.DetectNeighborsWithOverlapCircle(self, SeparationRange);
        algorithm.ApplySeparation(self, neighbors, SeparationRange);

        // 最終的に割り出された移動方向をセット
        SetMove(algorithm.GetMovement());

        // 重みづけマップのギズモを描画する（重みの可視化）
        gizmoDrawer.SetData(algorithm.GetWeightMap());
    }

    // Rigidbody2dでの移動はFixedUpdateに行う
    private void FixedUpdate()
    {
        Move();
    }

    private void SetMove(Vector2 direction)
    {
        movement = direction;
    }

    private void Move() 
    {
        if (movement == Vector2.zero) return;

        rb2d.MovePosition(transform.position + ((Vector3)movement * Time.fixedDeltaTime * Speed));
        movement = Vector2.zero;
    }
}
