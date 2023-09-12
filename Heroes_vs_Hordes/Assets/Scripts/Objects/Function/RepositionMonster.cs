using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionMonster : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public HeroController HeroController { get; set; }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Define.TAG_REPOSITION_AREA))
        {
            if (false == _collider.enabled)
                return;

            var repositionVec = HeroController.transform.position + new Vector3(HeroController.InputVec.x * 22f, HeroController.InputVec.y * 22f, 0f);
            transform.position = repositionVec;
        }
    }
}
