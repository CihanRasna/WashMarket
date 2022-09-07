using UnityEngine;

namespace GameplayScripts
{
    public class Player : Actor
    {
        public void Move(Vector2 direction)
        {
            var myTransform = transform;
            var targetForward = myTransform.forward;
            targetForward.x += direction.x;
            targetForward.z += direction.y;
            myTransform.forward = targetForward;
            myTransform.position += targetForward * (speed * direction.magnitude * Time.deltaTime);
        }
    }
}