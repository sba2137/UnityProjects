using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Action CheckRoom = delegate { };

    private bool _isMoving;

    private Coroutine _moveCoroutine;

    #region Commands

    public void FollowPath(List<Node> path)
    {
        _moveCoroutine = StartCoroutine(Move(path));
    }

    public void StopFollowingPath()
    {
        StopCoroutine(_moveCoroutine);
    }

    #endregion

    #region Enemy Logic

    private IEnumerator Move(List<Node> path)
    {
        _isMoving = true;

        for (int i = 0; i < path.Count; i++)
        {
            while (Vector2.Distance(gameObject.transform.position, new Vector2(path[i].X, path[i].Y)) > 0.1f)
            {
                gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, new Vector2(path[i].X, path[i].Y), StaticConfig.EnemyMovementSpeed * Time.deltaTime);

                yield return new WaitForSeconds(0.01f);
            }
        }

        _isMoving = false;

        StartCoroutine(TargetRoomCheck());
    }

    private IEnumerator TargetRoomCheck()
    {
        while (_isMoving == false)
        {
            yield return new WaitForSeconds(StaticConfig.EnemyWaypointGuardTime);

            CheckRoom();
        }
    }

    #endregion
}

