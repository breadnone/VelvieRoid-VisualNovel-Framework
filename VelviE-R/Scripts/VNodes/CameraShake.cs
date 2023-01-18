using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Camera/ShakeCamera", "Shakes a camera.", VColor.Blue, "Sc")]
    public class CameraShake : VBlockCore
    {
        [SerializeField] public Camera cam;
        [SerializeField] public float power;
        [SerializeField] public float magnitude = 9.5f;
        [SerializeField] public float shakeTime = 0.42f;
        [SerializeField] public float dropOffTime = 1.6f;
        private Vector3 defPos;
        private Quaternion defRot;
        public override void OnVEnter()
        {
            if(cam != null)
            {
                //Taken straight up out of LeanTween docs!
                defPos = cam.gameObject.transform.position;
                defRot = cam.gameObject.transform.rotation;

                float height = Mathf.PerlinNoise(magnitude, 0f)*10f;
                height = height*height * 0.3f;

				power = height*0.2f; // the degrees to shake the camera
				shakeTime = 0.42f; // The period of each shake
				dropOffTime = 1.6f; // How long it takes the shaking to settle down to nothing
				LTDescr shakeTween = LeanTween.rotateAroundLocal( cam.gameObject, Vector3.right, power, shakeTime)
				.setEase( LeanTweenType.easeShake ) // this is a special ease that is good for shaking
				.setLoopClamp()
				.setRepeat(-1);

				// Slow the camera shake down to zero
				LeanTween.value(cam.gameObject, power, 0f, dropOffTime).setOnUpdate( 
					(float val)=>{
						shakeTween.setTo(Vector3.right*val);
					}
				).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>
                {
                    LeanTween.delayedCall(0.1f, ()=>
                    {
                        cam.gameObject.transform.position = defPos;
                        cam.gameObject.transform.rotation = defRot;
                    });
                });
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(cam == null)
            {
                summary += "Camera can't be null!";
            }

            return summary;
        }
    }
}