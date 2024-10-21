using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationManager : MonoBehaviour
{
    [SerializeField]
    private string _faceUpStandUpStateName;
    [SerializeField]
    private string _faceDownStandUpStateName;
    [SerializeField]
    private string _faceUpStandUpClipName;
    [SerializeField]
    private string _faceDownStandUpClipName;
    [SerializeField]
    public float _timeToResetBones;
    [SerializeField]
    public string _attackTriggerName = "Attack";

    private Animator _animator;
    private Transform _hipsBone;
    public BoneTransform[] _faceUpStandUpBoneTransforms;
    public BoneTransform[] _faceDownStandUpBoneTransforms;
    public BoneTransform[] _ragdollBoneTransforms;
    public Transform[] _bones;
    private bool _isFacingUp;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component is missing!");
            return;
        }

        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _bones = _hipsBone.GetComponentsInChildren<Transform>();

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
    }

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
                break;
            }
        }
        transform.position = positionBeforeSampling;
    }

    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

    public string GetStandUpStateName()
    {
        return _isFacingUp ? _faceUpStandUpStateName : _faceDownStandUpStateName;
    }

    public BoneTransform[] GetStandUpBoneTransforms()
    {
        return _isFacingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
    }

    public void SetFacingDirection(bool isFacingUp)
    {
        _isFacingUp = isFacingUp;
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        EnableRagdoll();
        Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);
        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;
        foreach (var rigidbody in GetComponentsInChildren<Rigidbody>())
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

    private void EnableRagdoll()
    {
        foreach (var rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = false;
        }
        _animator.enabled = false;
    }

    public void DisableRagdoll()
    {
        foreach (var rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = true;
        }
        _animator.enabled = true;
    }
}
