using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private ZombieAI zombieAI;
    private ZombieMovement zombieMovement;
    private ZombieAnimationManager zombieAnimationManager;

    void Awake()
    {
        zombieAI = GetComponent<ZombieAI>();
        zombieMovement = GetComponent<ZombieMovement>();
        zombieAnimationManager = GetComponent<ZombieAnimationManager>();
    }

    void Update()
    {
        zombieAI.UpdateAI();
        zombieMovement.Move();
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        zombieAnimationManager.TriggerRagdoll(force, hitPoint);
    }
}
