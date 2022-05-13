using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform playerBox;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform bullet;
    [Space]
    [SerializeField] private float speed;

    public int minStepsBetweenShots = 500;
    public int damage = 100;
    public float range = 30;

    private Vector3 movementInput;
    private bool shotAvailable = false;
    private int stepsUntilShotIsAvailable = 0;
    private int score;
    private Vector3 respawnPosition;

    private void Start()
    {
        respawnPosition = transform.position;        
    }

    private void Update()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.Mouse0))
            Shoot();
    }

    private void FixedUpdate()
    {
        if (!shotAvailable)
        {
            stepsUntilShotIsAvailable--;

            if (stepsUntilShotIsAvailable <= 0)
                shotAvailable = true;
        }

        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = playerBox.TransformDirection(movementInput);

        controller.Move(moveVector * speed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        Vector3 objectPos = UnityEngine.Camera.main.WorldToScreenPoint(transform.position);

        mousePos.x = objectPos.x - mousePos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, angle - 90, 0));
    }

    private void Shoot()
    {
        if (!shotAvailable)
            return;

        var layerMask = LayerMask.GetMask("Player", "Wall", "Enemy");
        var direction = transform.forward;

        Transform bulletTransform = Instantiate(bullet, shootingPoint.position, Quaternion.identity);
        bulletTransform.GetComponent<Bullet>().Setup(direction);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 50f, layerMask))
        {
            if (hit.transform.CompareTag("enemy"))
            {
                Debug.DrawRay(shootingPoint.position, direction * range, Color.green, 2f);
                hit.transform.GetComponent<Enemy>().GetShot(damage, this);
            }
            else
            {
                Debug.DrawRay(shootingPoint.position, direction * range, Color.red, 2f);
            }
        }
        

        shotAvailable = false;
        stepsUntilShotIsAvailable = minStepsBetweenShots;
    }

    public void RegisterKill() {
        score += 100;
    }

    public void Respawn()
    {
        Debug.Log("respawn");
        transform.localPosition = respawnPosition;
    }

}
