using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform playerBox;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Transform shootingPoint;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float gravity = -9.81f;

    public int minStepsBetweenShots = 500;
    public int damage = 100;
    public int range = 30;

    private Vector3 movementInput;
    private Vector3 velocity;
    private bool shotAvailable = false;
    private int stepsUntilShotIsAvailable = 0;
    private int score;

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

        if (controller.isGrounded)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
        }

        controller.Move(moveVector * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
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

        var layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var direction = transform.forward;

        Debug.DrawRay(shootingPoint.position, direction * range, Color.red, 2f);


        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 50f, layerMask))
        {
            Debug.DrawRay(shootingPoint.position, direction * range, Color.green, 2f);
            hit.transform.GetComponent<Enemy>().GetShot(damage, this);
        }

        shotAvailable = false;
        stepsUntilShotIsAvailable = minStepsBetweenShots;
    }

    public void RegisterKill() {
        score += 100;
    }

}
