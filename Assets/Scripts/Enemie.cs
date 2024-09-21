using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour
{
    public enum EnemyType { SmallGoblin, BigGoblin }
    public EnemyType enemyType;

    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;

    public Animator AnimatorController;
    public NavMeshAgent Agent;

    public GameObject smallerEnemyPrefab;

    private float lastAttackTime = 0;
    private bool isDead = false;
    private Player player;

    private void Start()
    {
        SceneManager.Instance.AddEnemie(this);
        if (Agent != null)
        {
            Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        }
        player = FindAnyObjectByType<Player>();
        if (enemyType == EnemyType.BigGoblin)
        {
            Hp *= 2;
        }
    }
    private void Update()
    {
        if (isDead)
        {
            return;
        }
        if (Hp <= 0)
        {
            Die();
            if (Agent != null && Agent.isOnNavMesh)
            {
                Agent.isStopped = true;
            }
            return;
        }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);

        if (distance <= AttackRange)
        {
            if (Agent != null && Agent.isOnNavMesh)
            {
                Agent.isStopped = true;
            }
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                SceneManager.Instance.Player.Hp -= Damage;
                AnimatorController.SetTrigger("Attack");
            }
        }
        else
        {
            if (Agent != null && Agent.isOnNavMesh)
            {
                Agent.SetDestination(SceneManager.Instance.Player.transform.position);
            }
        }
        AnimatorController.SetFloat("Speed", Agent != null ? Agent.speed : 0);
    }
    private void Die()
    {
        player.Hp += 2;
        SceneManager.Instance.RemoveEnemie(this);
        isDead = true;
        AnimatorController.SetTrigger("Die");
        if (enemyType == EnemyType.BigGoblin)
        {
            SpawnSmallerEnemies();
        }
        Destroy(gameObject, 2f); 
    }
    private void SpawnSmallerEnemies()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * 0.5f); 
            spawnPosition.y = transform.position.y; 
            GameObject smallerEnemy = Instantiate(smallerEnemyPrefab, spawnPosition, Quaternion.identity);
            NavMeshAgent smallerEnemyAgent = smallerEnemy.GetComponent<NavMeshAgent>();
            if (smallerEnemyAgent != null)
            {
                smallerEnemyAgent.SetDestination(SceneManager.Instance.Player.transform.position);
            }
        }
    }
}

