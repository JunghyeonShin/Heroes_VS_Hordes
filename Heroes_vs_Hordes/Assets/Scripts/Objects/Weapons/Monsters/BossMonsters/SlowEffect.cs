using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEffect : MonoBehaviour
{
    private float angle;

    private const float ROTATE_SPEED = 45f;

    private void FixedUpdate()
    {
        angle += ROTATE_SPEED * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
