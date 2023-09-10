using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject[] _tilemaps;

    private readonly Vector3[] TILE_POSITIONS = new Vector3[]
    {
        new Vector3( 22f,  22f, 0f),
        new Vector3(-22f,  22f, 0f),
        new Vector3(-22f, -22f, 0f),
        new Vector3( 22f, -22f, 0f)
    };

    private void OnDisable()
    {
        for (int ii = 0; ii < _tilemaps.Length; ++ii)
            _tilemaps[ii].transform.position = TILE_POSITIONS[ii];
    }
}
