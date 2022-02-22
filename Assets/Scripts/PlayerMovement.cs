using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController controller;

	public Transform groundCheck;
	public LayerMask groundMask;

	float speed = 4f;
	float gravity = -9.81f;
	float groundDistance = 0.1f;
	float jumpHeight = 1f;


	Vector3 velocity;
	bool isGrounded;
	
	// Update is called once per frame
	void Update () {
		isGrounded = Physics.CheckSphere (groundCheck.position, groundDistance, groundMask);

		if (isGrounded && velocity.y <0f) {
			velocity.y = -1f;
		}

		float x = Input.GetAxis ("Horizontal");
		float z = Input.GetAxis ("Vertical");

		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move (move * speed * Time.deltaTime);

		if (Input.GetButtonDown ("Jump") && isGrounded) {
			velocity.y = Mathf.Sqrt (-2f * gravity * jumpHeight);
		}

		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
}
