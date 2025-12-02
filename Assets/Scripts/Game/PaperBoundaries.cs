using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBoundaries : MonoBehaviour
{
    [SerializeField] private List<GameObject> GrabbablePapers = new List<GameObject>();

    private Vector3[] paperResetPositions = new Vector3[8]; //Array of paper position coordinates
    private Quaternion[] paperResetRotations = new Quaternion[8]; //Array of paper position rotations

    void Awake()
    {
        for (int i = 0; i < GrabbablePapers.Count; i++)
        {
            paperResetPositions[i] = GrabbablePapers[i].transform.position;
            paperResetRotations[i] = GrabbablePapers[i].transform.rotation;
        }
    }

    void Update()
    {
        for (int i = 0; i < GrabbablePapers.Count; i++)
        {
            if (GrabbablePapers[i].transform.position.y < paperResetPositions[i].y - 0.6)
            {
                GrabbablePapers[i].transform.position = paperResetPositions[i];
                GrabbablePapers[i].transform.rotation = paperResetRotations[i];
            }
        }
    }
}
