using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    private Transform player;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    private Animator _animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
    }

    public void UpdateAI()
    {
        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    if (Vector3.Distance(transform.position, player.position) < attackRange)
                    {
                        AttackPlayer();
                    }
                    else
                    {
                        MoveTowardsPlayer();
                    }
                }
            }
        }
    }

    void AttackPlayer()
    {
        _animator.SetTrigger("Attack");
        // Add logic to reduce player's health or other attack effects
        Debug.Log("Zombie attacks the player!");
    }

    void MoveTowardsPlayer()
    {
        // Logic to move towards player, if needed
        Debug.Log("Zombie moves towards the player!");
    }
}
