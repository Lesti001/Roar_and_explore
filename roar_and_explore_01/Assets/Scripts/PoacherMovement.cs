using System.Collections.Generic;
using SafariGame;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PoacherMovement : MonoBehaviour
{
    private GameObject target;
    public bool isMoving = true;
    public float speed;
    public float waitTime;
    public float attackSpeed = 10;
    public float lastAttackTime;
    public int attackBcSimSpeed = 1;

    void Awake()
    {
        FindTarget();
    }

    void Update()
    {
        if (GameModel.instance.getSimulationSpeed() <= 0)
        {
            return;
        }

        if (target.gameObject != null)
        {

            Vector3 direction = (target.transform.position - transform.position);
            Vector3 directionNormalized = direction.normalized;
            float distanceToTarget = direction.magnitude;

            // Rotate towards the direction
            float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), Time.deltaTime * 10f);
            
            // move towards target
            // calculate how much you would move this frame
            float moveDistance = speed * Time.deltaTime;

            // If we're going to overshoot, clamp it
            if (moveDistance >= distanceToTarget)
            {
                transform.position = target.transform.position;
            }
            else
            {
                transform.position += directionNormalized * moveDistance;
            }

            if ((target.transform.position - transform.position).magnitude <= 3)
            {
                Debug.Log("killed animal");                
                AnimalManager.Instance.poacherKillsAnimal(target.gameObject);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("target gone, finding a new one"); // sok okbol eltunhet utkozben a target allat, ha van, akkor keressen egy ujat
            FindTarget();
        }
        

        switch (GameModel.instance.getSimulationSpeed())
        {
            case 1:
                speed = 0.5f;
                waitTime = 30;
                attackBcSimSpeed = 1;
                break;
            case 2:
                speed = 12f;
                waitTime = 15;
                attackBcSimSpeed = 2;
                break;
            case 3:
                speed = 84f;
                waitTime = 5;
                attackBcSimSpeed = 3;
                break;
            default:
                break;
        }

    }

    private void FindTarget()
    {
        List<GameObject> a = AnimalManager.Instance.getAllAnimals();
        if (a.Count > 0)
        {
            float min = (a[0].transform.position - transform.position).magnitude; //if yes animal -> target nearest one
            int minInd = 0;
            for (int i = 0; i < a.Count; i++)
            {
                if ((a[i].transform.position - transform.position).magnitude < min)
                {
                    min = (a[i].transform.position - transform.position).magnitude;
                    minInd = i;
                }
            }
            target = a[minInd].gameObject;
        }
        else
        {
            Debug.Log("poacher couldn't find target - leaving");
            Destroy(gameObject);
        }
    }
}