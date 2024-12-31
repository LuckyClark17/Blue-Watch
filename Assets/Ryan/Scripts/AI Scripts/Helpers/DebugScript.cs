using System;
using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;
using UnityEngine.UIElements;
public partial class AIScript : MonoBehaviour {
        protected void DrawLines()
    {
//        Debug.DrawLine(transform.position, closestUpperTeammate.transform.position, Color.red);
        if (_closestTeammate != null)
            Debug.DrawLine(transform.position, _closestTeammate.transform.position, Color.yellow);
    }
}