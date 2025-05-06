using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

namespace SafariGame
{
    public class Herbivore : Animal
    {
        public Herbivore() : base() { }

        public override void findFood()
        {
            if (latestFoodLocation != new Vector2(-100, -100))
            {
                MoveToTarget(latestFoodLocation);
                if (Vector2.Distance(latestFoodLocation, (Vector2)transform.position) < 0.04f)
                {
                    if (AnimalManager.Instance.consumeFood(latestFoodLocation))
                    {
                        UnityEngine.Debug.Log("EATING");
                        hunger = 0;
                    }
                    else
                    {
                        UnityEngine.Debug.Log("FOOD DEPLETED, NEED NEW SOURCE");
                        latestFoodLocation = new Vector2(-100, -100);
                    }
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
                MoveToTarget(AnimalManager.Instance.getPackLeaderPosition(this) + positionDifference);
            }
        }
    }
}