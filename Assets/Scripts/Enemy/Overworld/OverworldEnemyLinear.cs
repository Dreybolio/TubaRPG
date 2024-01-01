using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyLinear : OverworldEnemyBase
{
    protected override void DoMovement()
    {
        Gravity();
        GroundedCheck();
        Move();
        CheckForTurnAround();
        ApplyVelocity();
    }

    protected void Gravity()
    {

        if (_grounded)
        {
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravityScale * Time.deltaTime;
        }

    }
    protected void GroundedCheck()
    {
        _grounded = Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }
    protected void Move()
    {
        Vector2 moveInput;
        if (_chasingPlayer)
        {
            moveInput = new Vector2(hero.position.x, hero.position.z) - new Vector2(transform.position.x, transform.position.z);
        }
        else
        {
            if(!(Vector3.Distance(transform.position, startPos) < 0.25f))
            {
                moveInput = new Vector2(startPos.x, startPos.z) - new Vector2(transform.position.x, transform.position.z);
            }
            else
            {
                moveInput = Vector2.zero;
            }
        }
        moveInput.Normalize();
        Vector2 targetVector = moveInput * speed;
        Vector2 currentVector = new(controller.velocity.x, controller.velocity.z);

        // Set goal speed

        if (Vector2.Distance(currentVector, targetVector) < -0.1f || Vector2.Distance(currentVector, targetVector) > 0.1f)
        {
            _speed = Vector2.Lerp(currentVector, targetVector, Time.deltaTime * acceleration);
            _speed = Vector2.ClampMagnitude(_speed, speed);
            _speed.Set(Mathf.Round(_speed.x * 1000f) / 1000f, Mathf.Round(_speed.y * 1000f) / 1000f);
        }
        else
        {
            _speed = targetVector;
        }

        //Animation
        // Return 0 - 1 on speed
        animator.SetFloat(_animSpeed_F, _speed.magnitude / speed);
    }
    protected void CheckForTurnAround()
    {
        if (_facingRight && _speed.x < 0f)
        {
            model.SetDirection(ModelDirection.LEFT);
            _facingRight = false;
        }
        else if (!_facingRight && _speed.x > 0f)
        {
            model.SetDirection(ModelDirection.RIGHT);
            _facingRight = true;
        }

        if (_facingForward && _speed.y > 0.5f)
        {
            model.SetDirection(ModelDirection.BACKWARD);
            _facingForward = false;
        }
        else if ((!_facingForward && _speed.y < 0f) || (!_facingForward && _speed.y <= 0.5f && _speed.x != 0f))
        {
            model.SetDirection(ModelDirection.FORWARD);
            _facingForward = true;
        }
    }
    protected void ApplyVelocity()
    {
        controller.Move(new Vector3(_speed.x, _verticalVelocity, _speed.y) * Time.deltaTime);
    }
}
