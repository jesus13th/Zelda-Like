using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform tTarget;

    [Header("Health")]
    [SerializeField] private int health = 3;
    private bool isDead = false;

    [Header("Velocity")]
    [SerializeField] private Vector2 velocityCurrent;
    [SerializeField] private float velocityTarget;
    private Vector2 velocityRef;
    [SerializeField] private float velocitySmooth;
    [SerializeField] private string parameterVelocityX;

    [Header("Attack")]
    [SerializeField] private float chaseRange = 3;
    private float distance;

    private void Update() {
        distance = Vector2.Distance(tTarget.position, transform.position);
        Vector2 direction;
        if (distance < chaseRange) {
            direction = (tTarget.position - transform.position).normalized;
        } else {
            direction = Vector2.zero;
        }
        velocityCurrent = Vector2.SmoothDamp(velocityCurrent, velocityTarget * direction, ref velocityRef, velocitySmooth);

    }
    private void FixedUpdate() {
        if(canDamage && !isDead)
            rb.velocity = velocityCurrent;
    }
    private void LateUpdate() {
        if (Mathf.Abs(rb.velocity.magnitude) > 0.1f) {
            animator.SetFloat(parameterVelocityX, rb.velocity.x);
            animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.magnitude) / velocityTarget);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
    private bool canDamage = true;
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player") && canDamage) {
            Debug.Log("Damage");
            StartCoroutine(DamageTarget(tTarget.GetComponent<CharacterMovement>()));
        }
    }
    private IEnumerator DamageTarget(CharacterMovement t) {
        t.ApplyDamage();
        canDamage = false;
        yield return new WaitForSeconds(2f);
        canDamage = true;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Bullet")) {
            ApplyDamage();
            Destroy(collision.gameObject);
        }
    }
    public void ApplyDamage() {
        if (isDead)
            return;

        health--;
        if (health < 0) {
            StartCoroutine(DeadCoroutine());
        }
        animator.SetTrigger("Damage");
    }
    private IEnumerator DeadCoroutine() {
        animator.SetBool("IsDead", isDead = true);
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
