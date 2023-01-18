using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Animation/VBlendShape", "Lerps SkinnedMeshRenderer's blendShapes.", VColor.Green01, "Ps")]
    public class VBlendShape : VBlockCore
    {
        [SerializeField] public SkinnedMeshRenderer vmesh;
        [SerializeField] public List<VBlendShapes> values = new List<VBlendShapes>();
        [SerializeField] public bool waitUntilFinished = false;
        [SerializeField] public bool cancelPrevious = false;
        [SerializeField] public bool forceCompletePrevious = false;
        public override void OnVEnter()
        {
            if(cancelPrevious || forceCompletePrevious)
            {
                if(LeanTween.isTweening(vmesh.gameObject))
                {
                    LeanTween.cancel(vmesh.gameObject, forceCompletePrevious);
                }
            }

            if(values.Count > 0)
            {
                for(int i = 0; i < values.Count; i++)
                {
                    if(values[i] == null)
                        continue;

                    if(values[i].zeroedPrevValue)
                    {
                        vmesh.SetBlendShapeWeight(values[i].blendShapeindex, 0f);
                    }

                    var getIndex = vmesh.sharedMesh.GetBlendShapeIndex(values[i].blendName);
                    var getWeight = vmesh.GetBlendShapeWeight(getIndex);

                    LeanTween.value(vmesh.gameObject, getWeight, values[i].blendValue, values[i].duration).setOnUpdate((float val)=>
                    {
                        vmesh.SetBlendShapeWeight(getIndex, val);

                    }).setOnComplete(()=>
                    {
                        var idx = i;

                        if(idx == values.Count - 1 && waitUntilFinished)
                        {
                            OnVContinue();
                        }
                    });
                }
            }
        }
        public override void OnVExit()
        {
            if(!waitUntilFinished)
                OnVContinue();
        }

    }
}