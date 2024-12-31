using UnityEngine;
using System;
using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;
using UnityEngine.UIElements;

public partial class AIScript : MonoBehaviour
{
        protected void ClosestUpperTeamMate(GameObject[] UpperTeammates) {
            int lowestDistanceIndex = 0;
            float smallest = float.MaxValue;
            float[] distanceBetween = new float[UpperTeammates.Length];
            for (int i = 0; i < UpperTeammates.Length; i++)
            {
                distanceBetween[i] = Vector3.Distance(transform.position, UpperTeammates[i].transform.position);

                if (distanceBetween[i] < smallest)
                {
                    smallest = distanceBetween[i];
                    lowestDistanceIndex = i;
                }
            }
            closestUpperTeammate = UpperTeammates[lowestDistanceIndex];
        }

        protected GameObject[] FindTeammates(GameObject[] position){
            teammates = new GameObject[position.Length-1];

            int correctIndexPos = 0;
            //loops through all of the players. If the player it is looking at is unique and is not the player the script is attached to, then the player is added to teammates.
            for (int index = 0; index < position.Length; index++)
            {
                if (position[index] != gameObject)
                {
                    teammates[correctIndexPos] = position[index];
                    correctIndexPos++;
                }
            }
            return teammates;
        }

        private void ClosestTeammate() {
            int lowestDistanceIndex = 0;
            float smallest = float.MaxValue;
            

            for (int i = 0; i < teammates.Length; i++)
            {
                if (teammates[i] != gameObject)
                {
                    float distanceBetween = Vector3.Distance(transform.position, teammates[i].transform.position);

                    if (distanceBetween < smallest)
                    {
                        smallest = distanceBetween;
                        lowestDistanceIndex = i;
                    }

                    _closestTeammate = teammates[lowestDistanceIndex];
                }
            }
        }
        
    }
