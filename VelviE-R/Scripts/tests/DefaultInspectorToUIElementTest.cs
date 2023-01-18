using UnityEngine;

namespace VIEditor
{
    public class DefaultInspectorToUIElementTest : MonoBehaviour
    {
        public enum TestEnum
        {
            ItemOne,
            ItemTwo
        }
        [SerializeField] protected GameObject bameObjectField;
        [SerializeField] protected float floatField;
        [SerializeField] protected TestEnum testEnum;
        [SerializeField] protected int[] intArrayTest;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}