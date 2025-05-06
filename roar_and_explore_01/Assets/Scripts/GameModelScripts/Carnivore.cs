using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace SafariGame
{
    public class Carnivore : Animal
    {
        public Carnivore() : base() { }

        public override void findFood()
        {
            if (isPackLeader)
            {
                List<GameObject> g = AnimalManager.Instance.getGiraffes();
                List<GameObject> z = AnimalManager.Instance.getZebras();
                //THIS IF CHECKS FOR CLOSEST ANIMAL, I KNOW ITS LONG SADGE
                // igazad van, aki nem tud hosszu kodot olvasni, menjen arufeltoltonek! - n
                if (g.Count > 0 && z.Count > 0)
                {
                    if (UnityEngine.Vector2.Distance(g.Last().transform.position, (UnityEngine.Vector2)this.gameObject.transform.position) <= UnityEngine.Vector2.Distance(z.Last().transform.position, (UnityEngine.Vector2)this.gameObject.transform.position))
                    {
                        Hunt(g.Last());
                    }
                    else
                    {
                        Hunt(z.Last());
                    }
                }
                else if (g.Count > 0)
                {
                    Hunt(g.Last());
                }
                else if (z.Count > 0)
                {
                    Hunt(z.Last());
                }
                else
                {
                    if (UnityEngine.Vector2.Distance(roamingTarget, (UnityEngine.Vector2)transform.position) < 0.001f)//Arrived at destination
                    {
                        roamingTarget = GetClampedPosition(roamingTarget + UnityEngine.Random.insideUnitCircle * 5f);
                    }
                    MoveToTarget(roamingTarget);
                }
            }
            else
            {
                MoveToTarget(AnimalManager.Instance.getPackLeaderPosition(this) + positionDifference);
            }
        }
        public void Hunt(GameObject targetP) 
        {
            UnityEngine.Debug.Log("HUNTING");
            moveSpeed = moveSpeed * 1.2f;
            MoveToTarget((UnityEngine.Vector2)targetP.transform.position + positionDifference);
            if (UnityEngine.Vector3.Distance(targetP.transform.position, transform.position) < 1f) 
            {
                UnityEngine.Debug.Log("KILLING ANIMAL");
                AnimalManager.Instance.animalDies(targetP);
                Destroy(targetP);
                if (AnimalManager.Instance.getLions().Contains(this.gameObject))
                {
                    AnimalManager.Instance.feedLions();
                    Debug.Log("FED LIONS");
                }
                else
                {
                    AnimalManager.Instance.feedHyenas();
                    Debug.Log("FED HYENAS");
                }
            }


        }
    }
}