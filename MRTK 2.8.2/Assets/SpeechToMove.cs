using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

[RequireComponent(typeof(SolverHandler))]
[RequireComponent(typeof(HandConstraintPalmUp))]
[RequireComponent(typeof(SpeechInputHandler))]
public class SpeechToMove : MonoBehaviour
{
    SolverHandler handTracker;
    HandConstraintPalmUp constraintPalmUp;
    SpeechInputHandler speechInputHandler;

    [SerializeField] float travelSpeed = 0.5f;
    [SerializeField] float distanceThreshhold =0.1f;

    private bool isPalmUp = false;
    private Vector3 handPosition;

    // Start is called before the first frame update
    void Start()
    {
        handTracker = GetComponent<SolverHandler>();
        SetHandTrackerProperties();

        constraintPalmUp = GetComponent<HandConstraintPalmUp>();
        constraintPalmUp.OnHandActivate.AddListener(() => SetPalmUp(true));
        constraintPalmUp.OnHandDeactivate.AddListener(() => SetPalmUp(false));
        constraintPalmUp.UpdateLinkedTransform = true;

        speechInputHandler = GetComponent<SpeechInputHandler>();
        speechInputHandler.AddResponse("Julie", () => StartToMove());
    }
    // Update is called once per frame
    void Update()
    {
        handPosition = handTracker.TransformTarget.position;
    }

    public void StartToMove()
    {
        if (!isPalmUp)
        {
            Debug.Log("Your palm is not placed correctly!");
            return;
        }

        constraintPalmUp.UpdateLinkedTransform = false;
        StartCoroutine(CoStartToMove());
    }

    private IEnumerator CoStartToMove()
    {
        Debug.Log("Start Moving...");
        while (!(Vector3.Distance(transform.position, handPosition) <= distanceThreshhold))
        {
            transform.position = Vector3.Lerp(transform.position, handPosition, travelSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Stop Moving");
        constraintPalmUp.UpdateLinkedTransform = true;
    }

    private void SetPalmUp(bool palmUp)
    {
        isPalmUp = palmUp;
    }

    private void SetHandTrackerProperties()
    {
        handTracker.TrackedTargetType = TrackedObjectType.HandJoint;
        handTracker.TrackedHandedness = Handedness.Both;
        handTracker.TrackedHandJoint = TrackedHandJoint.Palm;
        handTracker.UpdateSolvers = true;
    }
}
