using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


namespace SafariGame
{
    public class RangerMovement : MonoBehaviour, IPointerClickHandler
    {
        public Vector3 target;
        public bool isPatroling;
        public bool isMoving;
        public bool isSelected;
        public float speed;
        public float waitTime;
        public float attackSpeed = 10;
        public float lastAttackTime;
        public int attackHit = 50;
        public int attackBcSimSpeed = 1;

        void OnDrawGizmosSelected()
        {
            // Draw a yellow cube at the transform position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(0, 15, 0), new Vector3(72, 33, 1));
        }

        IEnumerator WaitAndMove()
        {
            isMoving = false;
            yield return new WaitForSeconds(waitTime);
            isMoving = true;
            isPatroling = true;
        }

        void Awake()
        {
            target = new Vector3(Random.Range(-37, 37), Random.Range(-3, 33), 0);
        }
        public void Start() 
        {
            AnimalManager.Instance.AddRanger(this.gameObject);
        }

        void Update()
        {
            
            if (Input.GetMouseButtonDown(1) && isSelected == true)
            {
                target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.x = Mathf.Clamp(target.x, -37, 37);
                target.y = Mathf.Clamp(target.y, -3, 33);
                target.z = 0;
                isMoving = true;
                isPatroling = false;

            }
            if (isMoving == false || GameModel.instance.getSimulationSpeed() <= 0)
            {
                return;
            }

            Vector3 direction = (target - transform.position);
            Vector3 directionNormalized = direction.normalized;
            float distanceToTarget = direction.magnitude;

            // Rotate towards the direction
            float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), Time.deltaTime * 10f);

            // Calculate how much you would move this frame
            float moveDistance = speed * Time.deltaTime;

            // Move forward
            if (moveDistance >= distanceToTarget)
            {
                transform.position = target;
            }
            else
            {
                transform.position += (Vector3)(directionNormalized * moveDistance);
            }

            if ((target - transform.position).magnitude <= 0.1)
            {
                if (isPatroling == true)
                {
                    target = new Vector3(Random.Range(-37, 37), Random.Range(-3, 33), 0); //need to ask how big the map is!!
                }
                else
                {
                    StartCoroutine("WaitAndMove");
                }
            }

            switch (GameModel.instance.getSimulationSpeed())
            {
                case 1:
                    speed = 0.75f;
                    waitTime = 30;
                    attackBcSimSpeed = 1;
                    break;
                case 2:
                    speed = 18f;
                    waitTime = 15;
                    attackBcSimSpeed = 2;
                    break;
                case 3:
                    speed = 126f;
                    waitTime = 5;
                    attackBcSimSpeed = 3;
                    break;
                default:
                    break;
            }

            var shootRange = Physics2D.OverlapCircleAll(transform.position, 3);

            foreach (var p in shootRange)
            {
                if (p.CompareTag("Poacher"))
                {
                    AttackPoacher(p.gameObject);
                }
            }
        }

        public void buttonPressed()
        {
            // Debug.Log("CLICKED RANGER");
            isSelected = !isSelected;
            GetComponent<SpriteRenderer>().color = isSelected ? Color.red : Color.green;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Debug.Log("CLICKED RANGER");
            if (eventData.button == 0)
            {
                isSelected = !isSelected;
                GetComponent<SpriteRenderer>().color = isSelected ? Color.red : Color.white;
            }
        }

        public void AttackPoacher(GameObject Poacher)
        {
         
            if(lastAttackTime + (attackSpeed / attackBcSimSpeed) < Time.time)
            {
                isMoving = false;
                Debug.Log("Ranger Attacked");
                lastAttackTime = Time.time;
                if(Random.Range(0, 100) < attackHit)
                {
                    Destroy(Poacher);
                    GameModel.instance.changeMoney(200);
                    Debug.Log("Poacher Dead");
                    isMoving = true;
                }
                else
                {
                    Debug.Log("Ranger Dead");
                    AnimalManager.Instance.RemoveRanger(this.gameObject);
                    GameModel.instance.removeRanger();
                    Destroy(gameObject);
                }
            }

        }
    }
}
