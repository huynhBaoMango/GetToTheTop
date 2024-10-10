using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Zoombie : MonoBehaviour
{
   
    private enum ZoombieState
    {
        Walking,
        Ragdoll
    }
    [SerializeField]
    private Camera _Camera;
    private Rigidbody[] _ragdollRigibodies;
    private ZoombieState _currentState = ZoombieState.Walking;
    private CharacterController characterController;
    private Animator _animator;
    void Awake()
    {
        _ragdollRigibodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        DisabeleRagdoll();
    }
    void Update()
    {
        switch(_currentState)
        {
            case ZoombieState.Walking:
                WalkingBehaviour(); 
                break;
            case ZoombieState.Ragdoll
                : RagdollBehaviour(); 
                break;
        }    
    }
    public void TriggerRagdoll(Vector3 force,Vector3 hitPoint)
    {
        EnableRagdoll();
        Rigidbody hitRigibody = _ragdollRigibodies.OrderBy(Rigidbody => Vector3.Distance(Rigidbody.position,hitPoint)).First();
        hitRigibody.AddForceAtPosition(force, hitPoint,ForceMode.Impulse);
        _currentState = ZoombieState.Ragdoll;
    }    
    private void DisabeleRagdoll()
    {
        foreach (var rigibody in _ragdollRigibodies)
        {
            rigibody.isKinematic = true;
        }
        _animator.enabled = true;
        characterController.enabled = true;
    }
    private void EnableRagdoll()
    {
        foreach (var rigibody in _ragdollRigibodies)
        {
            rigibody.isKinematic = false;
        }
        _animator.enabled = false;
        characterController.enabled = false;
    }
    private void WalkingBehaviour()
    {
        Vector3 direction = _Camera.transform.position - transform.position;
        direction.y = 0;
        direction = direction.normalized;

        
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

        
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 20 * Time.deltaTime); 

        
    }

    private void RagdollBehaviour()
    {

    }    
}