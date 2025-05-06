using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using SafariGame;

public class JeepScript : MonoBehaviour
{
    private List<Vector2> path;
    private int nodesVisited;
    private Vector2 targetNode;
    private int passengerCount;
    private int ticketPrice;
    private float moveSpeed;
    private float reachThreshold = 0.5f;
    
    private HashSet<GameObject> lionsSeen = new HashSet<GameObject>();
    private HashSet<GameObject> hyenasSeen = new HashSet<GameObject>();
    private HashSet<GameObject> giraffesSeen = new HashSet<GameObject>();
    private HashSet<GameObject> zebrasSeen = new HashSet<GameObject>();
    private float detectionRange = 10f;
    private double score;

    // for editor testing
    private void OnDrawGizmosSelected()
    {
        // Set the color
        Gizmos.color = Color.yellow;

        // Draw a wireframe circle at the Jeep's position
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    public void Start()
    {
        AnimalManager.Instance.AddJeep(this.gameObject);
    }

    public void SetParameters(List<Vector2> pt, int pc, int tp)
    {
        List<Vector2> reversedList = new List<Vector2>(pt);
        List<Vector2> finalPath = new List<Vector2>(pt);
        reversedList.Reverse();
        finalPath.AddRange(reversedList);
        path = finalPath;

        nodesVisited = 0;
        passengerCount = pc;
        ticketPrice = tp;
        targetNode = path.First();
    }

    private void CheckForAnimals()
    {
        if (nodesVisited > (path.Count / 2)) { return; }

        foreach (GameObject l in AnimalManager.Instance.getLions())
            { if (Vector2.Distance(transform.position, l.transform.position) < detectionRange) { lionsSeen.Add(l);    } }
        foreach (GameObject h in AnimalManager.Instance.getHyenas())
            { if (Vector2.Distance(transform.position, h.transform.position) < detectionRange) { hyenasSeen.Add(h);   } }
        foreach (GameObject g in AnimalManager.Instance.getGiraffes())
            { if (Vector2.Distance(transform.position, g.transform.position) < detectionRange) { giraffesSeen.Add(g); } }
        foreach (GameObject z in AnimalManager.Instance.getZebras())
            { if (Vector2.Distance(transform.position, z.transform.position) < detectionRange) { zebrasSeen.Add(z);   } }
    }

    private void Halfway()
    {
        // the first of every type gives more score (variety is important)
        score = (lionsSeen.Count    > 0 ? lionsSeen.Count    + 5 : 0) +
                (hyenasSeen.Count   > 0 ? hyenasSeen.Count   + 5 : 0) +
                (giraffesSeen.Count > 0 ? giraffesSeen.Count + 5 : 0) +
                (zebrasSeen.Count   > 0 ? zebrasSeen.Count   + 5 : 0);

        // checks if it's worth the ticket price
        if      ( 20 <= ticketPrice && ticketPrice <   80) { score -=  7; }
        else if ( 80 <= ticketPrice && ticketPrice <  160) { score -= 14; }
        else if (160 <= ticketPrice && ticketPrice <= 200) { score -= 24; }

        // changes weight of score
        Debug.Log("individual trip score: " + score + " / 4 x " + passengerCount);
        score = Math.Ceiling(score /= 4);
        score *= passengerCount;
    }

    private void Update()
    {
        switch (GameModel.instance.getSimulationSpeed())
        {
            case 0:
                moveSpeed = 0f;
                break;
            case 1:
                moveSpeed = 1.5f;
                break;
            case 2:
                moveSpeed = 36f;
                break;
            case 3:
                moveSpeed = 168f;
                break;

        }

        if (nodesVisited < path.Count - 1)
        {
            CheckForAnimals();
            if (nodesVisited == (path.Count / 2)) { Halfway(); }

            targetNode = path[nodesVisited + 1];

            Vector2 currentPosition = transform.position;
            Vector2 direction = (targetNode - currentPosition);
            Vector2 directionNormalized = direction.normalized;
            float distanceToTarget = direction.magnitude;

            // rotate towards target
            float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), Time.deltaTime * 10f);

            // move towards target

            // calculate how much you would move this frame
            float moveDistance = moveSpeed * Time.deltaTime;

            // If we're going to overshoot, clamp it
            if (moveDistance >= distanceToTarget)
            {
                transform.position = targetNode;
            }
            else
            {
                transform.position += (Vector3)(directionNormalized * moveDistance);
            }

            // transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, targetNode) < reachThreshold) { nodesVisited++; }
        }
        else
        {
            GameModel.instance.jeepUsage(false);
            GameModel.instance.changeVisitorRating((int)score);
            AnimalManager.Instance.RemoveJeep(this.gameObject);
            Destroy(gameObject);
        }
    }
}