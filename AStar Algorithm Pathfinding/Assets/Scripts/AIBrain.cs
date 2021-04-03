using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIBrain : MonoBehaviour
{
    [Header("Room Settings")]

    public List<Room> RoomsList;

    [Header("Enemy Settings")]

    [SerializeField] private Enemy _enemy;

    [SerializeField] private Transform _enemyTargetTransform;

    [Header("Tilemap Settings")]

    [SerializeField] private Tilemap _floorTilemap;

    [SerializeField] private Tilemap _collisionsTilemap;

    [System.Serializable]
    public struct Room
    {
        public string RoomName;

        public Transform RoomWaypoint;
    }

    private Pathfinding _pathfinding;

    private void Awake()
    {
        _pathfinding = new Pathfinding();
        _pathfinding.Configure(_floorTilemap, _collisionsTilemap);

        _enemy.CheckRoom += IsEnemyInThePlayerRoom;

        MoveToWaypoint(GetClosestRoom(_enemyTargetTransform).RoomWaypoint);
    }

    #region Commands

    private void MoveToWaypoint(Transform waypoint)
    {
        _enemy.FollowPath(_pathfinding.FindObject(new Point((int)waypoint.position.x, (int)waypoint.position.y), new Point((int)_enemy.transform.position.x, (int)_enemy.transform.position.y)));
    }

    #endregion

    #region Utility

    private Room GetClosestRoom(Transform entityPosition)
    {
        if (RoomsList.Count != 0)
        {
            Room closestRoom = RoomsList[0];

            foreach (Room room in RoomsList)
            {
                if (Vector2.Distance(closestRoom.RoomWaypoint.position, entityPosition.position) > Vector2.Distance(room.RoomWaypoint.position, entityPosition.position))
                {
                    closestRoom = room;
                }
            }

            return closestRoom;
        }

        return default;
    }

    private void IsEnemyInThePlayerRoom()
    {
        if (GetClosestRoom(_enemy.transform).RoomName != GetClosestRoom(_enemyTargetTransform).RoomName)
        {
            MoveToWaypoint(GetClosestRoom(_enemyTargetTransform).RoomWaypoint);
        }
    }

    #endregion
}
