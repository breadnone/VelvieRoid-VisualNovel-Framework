using UnityEngine;

namespace VelvieR
{
    public abstract class LeanAbstract : VBlockCore
    {
        [field: SerializeField, HideInInspector] public GameObject targetobject { get; set; }
        [field: SerializeField, HideInInspector] public float duration { get; set; } = 1f;
        [field: SerializeField, HideInInspector] public Vector3 to { get; set; }
        [field: SerializeField, HideInInspector] public LeanTweenType loopType { get; set; } = LeanTweenType.clamp;
        [field: SerializeField, HideInInspector] public int loopCount { get; set; }
        [field: SerializeField, HideInInspector] public LeanTweenType ease { get; set; } = LeanTweenType.notUsed;
        [field: SerializeField, HideInInspector] public Transform target { get; set; }
        [field: SerializeField, HideInInspector] public bool waitUntilFinished { get; set; }
        [field: SerializeField, HideInInspector] public bool enableOnStart { get; set; }
        [field: SerializeField, HideInInspector] public bool disableOnComplete { get; set; }
    }
}