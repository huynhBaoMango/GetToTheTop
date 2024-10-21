using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private enum ZombieState
    {
        Walking,
        Ragdoll,
        StandingUp,
        ResettingBones,
        Attacking
    }

    public ZombieAI zombieAI;
    public ZombieMovement zombieMovement;
    public ZombieAnimationManager zombieAnimationManager;
    [SerializeField]
    private float _chaseSpeed = 3f;
    private ZombieState _currentState = ZombieState.Walking;
    private Transform _playerTarget;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private float _timeToWakeUp;
    private Transform _hipsBone;
    private float _elapsedResetBonesTime;

    private void Awake()
    {
        zombieAI = GetComponent<ZombieAI>();
        zombieMovement = GetComponent<ZombieMovement>();
        zombieAnimationManager = GetComponent<ZombieAnimationManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        zombieAnimationManager.SetFacingDirection(_hipsBone.forward.y > 0);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case ZombieState.Walking:
                zombieMovement.Move();
                break;
            case ZombieState.Ragdoll:
                RagdollBehaviour();
                break;
            case ZombieState.StandingUp:
                StandingUpBehaviour();
                break;
            case ZombieState.ResettingBones:
                ResettingBonesBehaviour();
                break;
            case ZombieState.Attacking:
                AttackingBehaviour();
                break;
        }

        zombieAI.UpdateAI();
    }

    private void RagdollBehaviour()
    {
        _timeToWakeUp -= Time.deltaTime;
        if (_timeToWakeUp <= 0)
        {
            _currentState = ZombieState.ResettingBones;
            _elapsedResetBonesTime = 0;
        }
    }

    private void StandingUpBehaviour()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(zombieAnimationManager.GetStandUpStateName()) == false)
        {
            _currentState = ZombieState.Walking;
        }
    }

    private void ResettingBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / zombieAnimationManager._timeToResetBones;
        BoneTransform[] standUpBoneTransforms = zombieAnimationManager.GetStandUpBoneTransforms();

        for (int boneIndex = 0; boneIndex < zombieAnimationManager._bones.Length; boneIndex++)
        {
            zombieAnimationManager._bones[boneIndex].localPosition = Vector3.Lerp(
                zombieAnimationManager._ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);
            zombieAnimationManager._bones[boneIndex].localRotation = Quaternion.Lerp(
                zombieAnimationManager._ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        if (elapsedPercentage >= 1)
        {
            _currentState = ZombieState.StandingUp;
            zombieAnimationManager.DisableRagdoll();
            _animator.Play(zombieAnimationManager.GetStandUpStateName(), 0, 0);
        }
    }

    private void AttackingBehaviour()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            _currentState = ZombieState.Walking;
            _navMeshAgent.isStopped = false;
        }
        if (Vector3.Distance(transform.position, _playerTarget.position) < 1.5f)
        {
            _animator.SetTrigger(zombieAnimationManager._attackTriggerName);
        }
    }

}
