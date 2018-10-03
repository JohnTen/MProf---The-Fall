using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.Platformer
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(BaseGroundDetector))]
	public class PhysicalJumper : BaseCharacterJumper
	{
		protected Rigidbody2D rigidbody;
		protected BaseGroundDetector groundDetector;

		protected override void Jumping()
		{
			if (groundDetector.OnGround)
			{
				var vel = rigidbody.velocity;

				// Concluded from S = Vi * t + 1/2 * a * t^2 and t = (Vf - Vi)/a
				vel.y = Mathf.Sqrt(19.62f * jumpHeight * rigidbody.gravityScale);
				rigidbody.velocity = vel;
			}
		}

		protected void Update()
		{
			if (GetJumpingCommand())
			{
				Jumping();
			}
		}

		protected virtual void Awake()
		{
			rigidbody = GetComponent<Rigidbody2D>();
			groundDetector = GetComponent<BaseGroundDetector>();
		}
	}
}
