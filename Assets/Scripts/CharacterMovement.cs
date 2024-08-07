using System.Collections;

using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Health")]
    [SerializeField] private int health;
    [SerializeField] private bool isDead = false;

    [Header("Velocity")]
    private Vector2 axis;
    [SerializeField] private Vector2 velocityCurrent;
    [SerializeField] private float velocityTarget;
    private Vector2 velocityRef;
    [SerializeField] private float velocitySmooth;
    [SerializeField] private string parameterVelocityX;
    [SerializeField] private string parameterVelocityY;

    [Header("Weapon")]
    private Vector2 direction;
    [ShowIf("canShot"), SerializeField] private KeyCode shotKey = KeyCode.Z;
    [SerializeField] private bool canShot;
    [ShowIf("canShot"), SerializeField] private Transform tSpanwBullets;
    [ShowIf("canShot"), SerializeField] private GameObject pBullet;
    [ShowIf("canShot"), SerializeField] private float bulletForce;
    [ShowIf("canShot"), SerializeField] private float delay;
    [ShowIf("canShot"), SerializeField] private float duration;

    private float timer = 0;
    private void Start() {
        HealthSystem.Instance.CurrentHealth = health;
    }
    void Update() {
        axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        velocityCurrent = Vector2.SmoothDamp(velocityCurrent, velocityTarget * axis, ref velocityRef, velocitySmooth);

        if (canShot) {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(shotKey) && timer > delay) {
                var angle = Mathf.Round((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90) / 90f) * 90f;
                var arrow = Instantiate(pBullet, tSpanwBullets.position, Quaternion.Euler(0, 0, angle), null);
                arrow.GetComponent<Rigidbody2D>().AddForce(arrow.transform.up * bulletForce);
                Destroy(arrow, duration);
            }
        }
    }
    private void FixedUpdate() {
        rb.velocity = velocityCurrent;
    }
    private void LateUpdate() {
        if (Mathf.Abs(rb.velocity.magnitude) > 0.1f) {
            direction = rb.velocity.normalized;
            animator.SetFloat(parameterVelocityX, rb.velocity.x);
            animator.SetFloat(parameterVelocityY, rb.velocity.y);
            animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.magnitude) / velocityTarget);
        }
    }
    public void ApplyDamage() {
        if (isDead)
            return;

        health--;
        HealthSystem.Instance.CurrentHealth = health;
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