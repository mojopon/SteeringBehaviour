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

        // �d�݂̃��Z�b�g
        algorithm.ClearCurrentWeightMap();

        var self = transform.position;
        var target = targetTransform.position;
        
        // ��Q��������ꍇ�͂悯��d�݂Â����s��
        // ��Q���̌��m��Detector�ōs���A�u���b�N���ꂽ�����̏���SteeringAlgorithm�ɓn��
        var blockedDirections = detector.DetectAllCollisionsWithCircleCast(self, CircleCastSize, CircleCastRange);
        algorithm.ApplyCollisionAvoidance(blockedDirections.ToArray());

        var distance = Vector2.Distance(self, target);
        // �Ώۂ܂ł̋�����PursueDistanceMax���傫���ꍇ�́A�Ώۂɋ߂Â����Ώۂ��牡�ɕ��s�ړ�����d�݂Â����s��
        if (distance > PursueDistanceMax)
        {
            algorithm.ApplySeek(self, target);
            algorithm.ApplyStrife(self, target, StrifeFactor);
        }
        // PursueDistanceMin��苗�����߂��ꍇ�́A�Ώۂ��痣���d�݂Â����s��
        else if (PursueDistanceMin > distance)
        {
            algorithm.ApplyFlee(self, target);
        }
        // ����ȊO�͑Ώۂ��牡�ɕ��s�ړ�����d�݂Â����s��
        else
        {
            algorithm.ApplyStrife(self, target, StrifeFactor);
        }

        // �O��ړ������ړ����������ɏd�݂Â����s���i�X�^�b�N�̖h�~�j
        algorithm.ApplyBias(self);

        // �ߋ����̑��L�������痣���d�݂Â����s��
        var neighbors = detector.DetectNeighborsWithOverlapCircle(self, SeparationRange);
        algorithm.ApplySeparation(self, neighbors, SeparationRange);

        // �ŏI�I�Ɋ���o���ꂽ�ړ��������Z�b�g
        SetMove(algorithm.GetMovement());

        // �d�݂Â��}�b�v�̃M�Y����`�悷��i�d�݂̉����j
        gizmoDrawer.SetData(algorithm.GetWeightMap());
    }

    // Rigidbody2d�ł̈ړ���FixedUpdate�ɍs��
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
