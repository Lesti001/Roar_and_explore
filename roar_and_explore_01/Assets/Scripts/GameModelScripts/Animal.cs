using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

namespace SafariGame
{
    public abstract class Animal : MonoBehaviour, IPointerClickHandler
    {
        protected int age;
        protected int hunger;
        protected int thirst;
        protected int lust;
        private int ticksForHunger;
        protected bool isChiped = false;

        protected Vector2 positionDifference;
        protected Vector2 latestFoodLocation = new Vector2(-100f, -100f);
        protected Vector2 latestWaterLocation = new Vector2(-100f, -100f);
        protected Vector2 roamingTarget;
        protected Vector2 originalPosition;

        private int latestTime;
        private int oldTime;
        protected float moveSpeed = 0.25f;

        private GameObject gotRanger;

        protected bool isPackLeader;
        public void setPackLeader(bool s) { isPackLeader = s; }
        public virtual void findFood() { }
        public int getLust() => lust;
        public void setLust(int l)
        {
            lust = l;
        }
        public int getAge() => age;
        public int getHunger() => hunger;
        public int getThirst() => thirst;
        public bool GetIsChiped() => isChiped;
        public bool getIsPackLeader() => isPackLeader;
        public void feeedAnimal() { hunger = 0; }


        public void Start()
        {
            this.age = 0;
            this.hunger = 1;
            this.thirst = 1;
            AnimalManager.Instance.addAnimal(this);
            latestTime = GameModel.instance.getTimeOfDay();
            oldTime = GameModel.instance.getTimeOfDay();
            positionDifference = UnityEngine.Random.insideUnitCircle * 3f;
            originalPosition = transform.position;
            roamingTarget = GetClampedPosition(originalPosition + (UnityEngine.Random.insideUnitCircle * 10f));

        }

        public void Update()
        {
            if (GameModel.instance.getSimulationSpeed() <= 0 || GameModel.instance.getNightTime()) return; // Este nem éhezik szomjazik meg mozog
            switch (GameModel.instance.getSimulationSpeed()) // ranger 3szoros poacher 2szeres ha valtoztatunk rajta azokon is kell -j 
            {
                case 1:
                    moveSpeed = 0.25f;
                    ticksForHunger += 1;
                    break;
                case 2:
                    moveSpeed = 6f;
                    ticksForHunger += 24;
                    break;
                case 3:
                    moveSpeed = 42f;
                    ticksForHunger += 168;
                    break;
                default:
                    break;
            }

            //AFTER EATING THE ANIMAL IS SLUGISH
            if (hunger == 0 && thirst < 4)
            {
                moveSpeed = moveSpeed * 0.05f;
            }

            if (thirst > 5 && latestWaterLocation != new Vector2(-100, -100))
            {
                Debug.Log("Animal is thirsty, going towards known oasis");
                MoveToTarget(latestWaterLocation);
                if (Vector2.Distance(latestWaterLocation, (Vector2)transform.position) < 0.04f)
                {
                    Debug.Log("DRINKING");
                    thirst = 0;
                }
            }
            else if (hunger > 4)
            {
                Debug.Log("HUNGRY");
                findFood();
                if (hunger > 40) 
                {
                    this.die();
                }
            }
            else if (isPackLeader)//IS LEADER AND ROAMING
            {
                if (Vector2.Distance(roamingTarget, (Vector2)transform.position) < 0.001f)//Arrived at destination
                {
                    roamingTarget = GetClampedPosition(roamingTarget + UnityEngine.Random.insideUnitCircle * 5f);
                }
                MoveToTarget(roamingTarget);
            }
            else//FOLLOWS LEADER
            {
                MoveToTarget(GetClampedPosition(AnimalManager.Instance.getPackLeaderPosition(this) + positionDifference));
            }

            if (thirst > 30)
            {
                Debug.Log("Animal is passed away of thirst");
                this.die();
                return;
            }
            if (hunger > 40)
            {
                Debug.Log("Animal is passed away of Hunger");
                this.die();
                return;
            }

            //LOOKING FOR WATER AND FOOD
            if (latestWaterLocation == new Vector2(-100, -100))//LOOKS FOR WATER
            {
                foreach (var item in AnimalManager.Instance.getWaterSources())
                {
                    //ANIMAL SPOTS WATER
                    if (Mathf.Abs(this.gameObject.transform.position.x - item.x) < 5f && Mathf.Abs(this.gameObject.transform.position.y - item.y) < 5f)
                    {
                        latestWaterLocation = item;
                        Debug.Log("FOUND WATER");
                    }
                }
            }
            if (latestFoodLocation == new Vector2(-100, -100))
            {
                foreach (var item in AnimalManager.Instance.getFoodSources())
                {
                    //ANIMAL SPOTS FOOD
                    if (Mathf.Abs(this.gameObject.transform.position.x - item.Item1.x) < 5f && Mathf.Abs(this.gameObject.transform.position.y - item.Item1.y) < 5f)
                    {
                        latestFoodLocation = item.Item1;
                        UnityEngine.Debug.Log("FOUND FOOD");
                    }
                }
            }

            

            //BALANCE THESE NUMBERS LATER
            if (ticksForHunger >= 15000)
            {
                if (age > 10)//ANIMAL IS ADULT
                {
                    hunger++;
                    thirst ++;
                    if (hunger < 10 && thirst < 10)
                    {
                        lust++;
                    }
                }                
                hunger++;
                thirst += 2;
                age++;                
                if (age > 80)
                {
                    this.die();
                }
                ticksForHunger = 0;
            }

            //changing difference from packleader for better movement in pack
            oldTime = latestTime;
            latestTime = GameModel.instance.getTimeOfDay();
            if (oldTime != latestTime)
            {
                positionDifference = UnityEngine.Random.insideUnitCircle * 3f;
            }

            if (gotRanger != null)
            {
                gotRanger.GetComponent<RangerMovement>().target = transform.position;
            }
        }
        public void die()
        {
            AnimalManager.Instance.animalDies(this.gameObject);
            Destroy(this.gameObject);
        }
        protected void MoveToTarget(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, -angle);
            }
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        private void OnMouseDown()
        {
            Debug.Log("Object clicked: " + gameObject.name);
            if (isChiped == false)
            {
                AnimalManager.Instance.animalIsClicked(this);
            }
        }
        protected Vector2 GetClampedPosition(Vector2 position)
        {
            // Clamp the position within boundaries
            float clampedX = Mathf.Clamp(position.x, -37, 37);
            float clampedY = Mathf.Clamp(position.y, 0, 33);
            return new Vector2(clampedX, clampedY);
        }
        public void GettingChip() 
        {
            isChiped = true;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            gotRanger = GameModel.instance.isRangerSelected(transform.position);
            if (gotRanger != null)
            {
                StartCoroutine("WaitAndDie");
            }
        }
        IEnumerator WaitAndDie()
        {
            while ((gotRanger.transform.position - transform.position).magnitude >= 1)            
            {
                yield return new WaitForSeconds(0.1f);
            }
            GameModel.instance.changeMoney(200);
            AnimalManager.Instance.poacherKillsAnimal(gameObject);
        }

    }
}