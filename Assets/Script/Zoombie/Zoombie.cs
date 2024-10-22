using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
<<<<<<< HEAD
<<<<<<< HEAD
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private string _faceUpStandUpStateName;

    [SerializeField]
    private string _faceDownStandUpStateName;

    [SerializeField]
    private string _faceUpStandUpClipName;

    [SerializeField]
    private string _faceDownStandUpClipName;

    [SerializeField]
    private string _attackTriggerName = "Attack";

    [SerializeField]
    private float _timeToResetBones;

=======
    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }

    private enum ZombieState
    {
        Walking,
        Ragdoll,
        StandingUp,
        ResettingBones,
        Attacking
    }

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private string _faceUpStandUpStateName;

    [SerializeField]
    private string _faceDownStandUpStateName;

    [SerializeField]
    private string _faceUpStandUpClipName;

    [SerializeField]
    private string _faceDownStandUpClipName;

    [SerializeField]
    private string _attackTriggerName = "Attack";

    [SerializeField]
    private float _timeToResetBones;

>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    [SerializeField]
    private float _chaseSpeed = 3f;

    private Rigidbody[] _ragdollRigidbodies;
<<<<<<< HEAD
    private Animator _animator;
    private CharacterController _characterController;
    private Transform _hipsBone;
    private NavMeshAgent _navMeshAgent;
=======
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
>>>>>>> parent of f4a3bda3 (Merge remote-tracking branch 'origin/nghi_zombie')
    private Transform _playerTarget;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private float _timeToWakeUp;
    private Transform _hipsBone;
    private float _elapsedResetBonesTime;

<<<<<<< HEAD
    private ZombieController _zombieController;

=======
    private ZombieState _currentState = ZombieState.Walking;
    private Animator _animator;
    private CharacterController _characterController;
    private float _timeToWakeUp;
    private Transform _hipsBone;
    private NavMeshAgent _navMeshAgent;
    private Transform _playerTarget;
    private BoneTransform[] _faceUpStandUpBoneTransforms;
    private BoneTransform[] _faceDownStandUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;
    private Transform[] _bones;
    private float _elapsedResetBonesTime;
    private bool _isFacingUp;

>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    void Awake()
=======
    private void Awake()
>>>>>>> parent of f4a3bda3 (Merge remote-tracking branch 'origin/nghi_zombie')
    {
        zombieAI = GetComponent<ZombieAI>();
        zombieMovement = GetComponent<ZombieMovement>();
        zombieAnimationManager = GetComponent<ZombieAnimationManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
<<<<<<< HEAD
<<<<<<< HEAD
        _bones = _hipsBone.GetComponentsInChildren<Transform>();

=======

        _bones = _hipsBone.GetComponentsInChildren<Transform>();
>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
        _faceUpStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _faceDownStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _faceUpStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        PopulateAnimationStartBoneTransforms(_faceUpStandUpClipName, _faceUpStandUpBoneTransforms);
        PopulateAnimationStartBoneTransforms(_faceDownStandUpClipName, _faceDownStandUpBoneTransforms);
<<<<<<< HEAD
        DisableRagdoll();

        // Initialize the ZombieController with the necessary components
        _zombieController = new ZombieController(
            _timeToResetBones,
            _bones,
            _faceUpStandUpBoneTransforms,
            _faceDownStandUpBoneTransforms,
            _ragdollBoneTransforms,
            _hipsBone,
            _animator,
            _navMeshAgent,
            _playerTarget,
            _chaseSpeed,
            _attackTriggerName,
            _ragdollRigidbodies,
            _characterController,
            _faceUpStandUpStateName,
            _faceDownStandUpStateName
        );

=======

        DisableRagdoll();
>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    }

    void Update()
    {
<<<<<<< HEAD
        _zombieController.Update();
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        EnableRagdoll();
        Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);
        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        _navMeshAgent.enabled = false; // Disable NavMeshAgent when falling
        _zombieController.TriggerRagdoll();
=======
        switch (_currentState)
        {
            case ZombieState.Walking:
                WalkingBehaviour();
=======
        zombieAnimationManager.SetFacingDirection(_hipsBone.forward.y > 0);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case ZombieState.Walking:
                zombieMovement.Move();
>>>>>>> parent of f4a3bda3 (Merge remote-tracking branch 'origin/nghi_zombie')
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
<<<<<<< HEAD
                break;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        EnableRagdoll();

        Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);
        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);

        _navMeshAgent.enabled = false;  // Tắt NavMeshAgent khi ngã
        _currentState = ZombieState.Ragdoll;
        _timeToWakeUp = Random.Range(5, 10);
    }

    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rigidbody.position, hitPoint);

            if (closestRigidbody == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidbody = rigidbody;
            }
        }

        return closestRigidbody;
    }

    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        _animator.enabled = true;
        _characterController.enabled = true;
        _navMeshAgent.enabled = true; // Bật lại NavMeshAgent khi đứng dậy
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        _animator.enabled = false;
        _characterController.enabled = false;
    }

    private void WalkingBehaviour()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_playerTarget.position);
        _navMeshAgent.speed = _chaseSpeed;

        Vector3 directionToPlayer = (_playerTarget.position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);

        if (Vector3.Distance(transform.position, _playerTarget.position) < 1.5f)
        {
            _animator.SetTrigger(_attackTriggerName);
            _currentState = ZombieState.Attacking;
        }
    }
    private void RagdollBehaviour()
    {
        _timeToWakeUp -= Time.deltaTime;

        if (_timeToWakeUp <= 0)
        {
            _isFacingUp = _hipsBone.forward.y > 0;

            AlignRotationToHips();

            PopulateBoneTransforms(_ragdollBoneTransforms);

            _currentState = ZombieState.ResettingBones;
            _elapsedResetBonesTime = 0;
        }
>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    }

    private void DestroyObstacle(Collider obstacle)
    {
<<<<<<< HEAD
        Destroy(obstacle.gameObject);
=======
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpStateName()) == false)
        {
            _currentState = ZombieState.Walking;
        }
>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    }

    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
<<<<<<< HEAD
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rigidbody.position, hitPoint);
            if (closestRigidbody == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidbody = rigidbody;
            }
        }

        return closestRigidbody;
    }

    private void DisableRagdoll()
=======
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;

        BoneTransform[] standUpBoneTransforms = GetStandUpBoneTransforms();

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _bones[boneIndex].localScale = Vector3.one;
        }

        if (elapsedPercentage >= 1)
        {
            _currentState = ZombieState.StandingUp;
            DisableRagdoll();
            _animator.Play(GetStandUpStateName(), 0, 0);
        }
    }


    private void AttackingBehaviour()
>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }
<<<<<<< HEAD
        _animator.enabled = true;
        _characterController.enabled = true;
        _navMeshAgent.enabled = true; // Re-enable NavMeshAgent when standing up
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        _animator.enabled = false;
        _characterController.enabled = false;
    }

    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

=======

        if (Vector3.Distance(transform.position, _playerTarget.position) < 1.5f)
        {
            _animator.SetTrigger(_attackTriggerName);
        }
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredDirection = _hipsBone.up;

        if (_isFacingUp)
        {
            desiredDirection *= -1;
        }

        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipsPosition;
        _hipsBone.rotation = originalHipsRotation;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;

        Vector3 positionOffset = GetStandUpBoneTransforms()[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPosition;

        
        foreach (Transform bone in _bones)
        {
            bone.localScale = Vector3.one;
        }
    }


    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
    private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransforms(boneTransforms);
=======
>>>>>>> parent of f4a3bda3 (Merge remote-tracking branch 'origin/nghi_zombie')
                break;
        }

        zombieAI.UpdateAI();
    }
<<<<<<< HEAD
<<<<<<< HEAD
=======

    private string GetStandUpStateName()
    {
        return _isFacingUp ? _faceUpStandUpStateName : _faceDownStandUpStateName;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return _isFacingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
    }
>>>>>>> parent of 2c2ac15e (Merge branch 'Nghi_zoombie')
=======

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

>>>>>>> parent of f4a3bda3 (Merge remote-tracking branch 'origin/nghi_zombie')
}
