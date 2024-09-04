using System.Collections.Generic;
using UnityEngine;

public interface IGraph
{
    List<Vector3Int> GetNeighbours(Vector3Int cell);
}
