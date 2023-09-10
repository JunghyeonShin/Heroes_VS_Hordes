using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionTilemap : MonoBehaviour
{
    public HeroController HeroController { get; set; }

    private const float TILE_MOVE_SIZE = 88f;
    private const int MOVE_RIGHT = 0;
    private const int MOVE_UP = 0;
    private const string TAG_MAP_COLLISION_AREA = "MapCollisionArea";

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(TAG_MAP_COLLISION_AREA))
        {
            var differentX = Mathf.Abs(HeroController.transform.position.x - transform.position.x);
            var differentY = Mathf.Abs(HeroController.transform.position.y - transform.position.y);

            var moveDirectionX = HeroController.InputVec.x >= MOVE_RIGHT ? 1 : -1;
            var moveDirectionY = HeroController.InputVec.y >= MOVE_UP ? 1 : -1;

            if (differentX > differentY)
                transform.Translate(Vector3.right * moveDirectionX * TILE_MOVE_SIZE);
            else if (differentX < differentY)
                transform.Translate(Vector3.up * moveDirectionY * TILE_MOVE_SIZE);
            else
                transform.Translate(moveDirectionX * TILE_MOVE_SIZE, moveDirectionY * TILE_MOVE_SIZE, 0f);
        }
    }
}
