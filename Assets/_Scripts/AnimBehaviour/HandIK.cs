using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIK : StateMachineBehaviour
{
   private Transform _leftPoint;
   private Transform _rightPoint;

   public void SetHandPoints(Transform leftPoint, Transform rightPoint)
   {
      _leftPoint = leftPoint;
      _rightPoint = rightPoint;
   }

   public void ClearHandPoints()
   {
      _leftPoint = null;
      _rightPoint = null;
   }

   private void ActivateIK(Animator anim, AvatarIKGoal ikGoal, Transform handPoint)
   {
      if (handPoint is null) return;
      
      anim.SetIKPositionWeight(ikGoal, 1);
      anim.SetIKRotationWeight(ikGoal, 1);
      
      anim.SetIKPosition(ikGoal, handPoint.position);
      anim.SetIKRotation(ikGoal, handPoint.rotation);
   }

   private void DeactivateIK(Animator anim, AvatarIKGoal ikGoal)
   {
      anim.SetIKPositionWeight(ikGoal, 0);
      anim.SetIKRotationWeight(ikGoal, 0);
   }

   public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      ActivateIK(animator, AvatarIKGoal.LeftHand, _leftPoint);
      ActivateIK(animator, AvatarIKGoal.RightHand, _rightPoint);
   }

   public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      DeactivateIK(animator, AvatarIKGoal.LeftHand);
      DeactivateIK(animator, AvatarIKGoal.RightHand);
      // _handPoint = null;
   }
}
