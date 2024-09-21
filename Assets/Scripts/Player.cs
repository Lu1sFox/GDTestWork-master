using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Button doubleButton;
    [SerializeField] private Image cooldownBar;
    [SerializeField] private PlayerMovement playerMovement;

    public float Hp;
    public float Damage;
    public float AttackSpeed;
    public float AttackRange = 2f;
    public float doubleAttackCooldown = 2f;

    private bool isDead = false;
    private bool attackReady = true;
    private bool doubleAttackReady = true;
    private bool isProcessingQueue = false;
    public Animator AnimatorController;
    private Queue<System.Action> attackQueue = new Queue<System.Action>();

    private void Update()
    {
        if (isDead || Hp <= 0)
        {
            if (isDead) return;
            Die();
            return;
        }

        UpdateButtonState();
        UpdateCooldownBar();

        if (attackReady && attackQueue.Count > 0)
        {
            if (!isProcessingQueue)
            {
                StartCoroutine(ProcessAttackQueue());
            }
        }
    }
    private void UpdateButtonState()
    {
        bool enemiesInRange = AreEnemiesInRange();
        doubleButton.interactable = enemiesInRange && doubleAttackReady;
    }
    private void UpdateCooldownBar()
    {
        if (!doubleAttackReady && cooldownBar != null)
        {
            float cooldownProgress = (Time.time - lastDoubleAttackTime) / doubleAttackCooldown;
            cooldownBar.fillAmount = Mathf.Clamp01(1 - cooldownProgress);
        }
    }
    private bool AreEnemiesInRange()
    {
        foreach (var enemy in SceneManager.Instance.Enemies)
        {
            if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= AttackRange)
            {
                return true;
            }
        }
        return false;
    }
    private Enemie FindClosestEnemy()
    {
        Enemie closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (var enemy in SceneManager.Instance.Enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }
    public void DoubleAttack()
    {
        if (isProcessingQueue || !doubleAttackReady) return;

        Enemie closestEnemy = FindClosestEnemy();
        if (CanAttack(closestEnemy))
        {
            StartCoroutine(DoubleAttackCooldownCoroutine());
            LookAtEnemy(closestEnemy);
            closestEnemy.Hp -= Damage * 2;
            AnimatorController.SetTrigger("Double attack");
        }
    }

    private bool CanAttack(Enemie enemy) =>
        enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= AttackRange;

    public void Attack() =>
        attackQueue.Enqueue(() => PerformAttack());

    private void PerformAttack()
    {
        Enemie closestEnemy = FindClosestEnemy();
        if (CanAttack(closestEnemy))
        {
            LookAtEnemy(closestEnemy);
            closestEnemy.Hp -= Damage;
        }
        AnimatorController.SetTrigger("Attack");
    }
    private void LookAtEnemy(Enemie enemy)
    {
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    private void Die()
    {
        playerMovement.enabled = false;
        StopAllCoroutines();
        doubleButton.interactable = false;
        isDead = true;
        AnimatorController.SetTrigger("Die");
        SceneManager.Instance.GameOver();
    }
    private IEnumerator ProcessAttackQueue()    
    {
        isProcessingQueue = true;
        attackReady = false;
        while (attackQueue.Count > 0)
        {
            attackQueue.Dequeue().Invoke();
            yield return new WaitForSeconds(AttackSpeed);
        }
        attackReady = true;
        isProcessingQueue = false;
    }
    private float lastDoubleAttackTime;
    private IEnumerator DoubleAttackCooldownCoroutine()
    {
        doubleAttackReady = false;
        lastDoubleAttackTime = Time.time;
        yield return new WaitForSeconds(doubleAttackCooldown);
        doubleAttackReady = true;
    }
}
