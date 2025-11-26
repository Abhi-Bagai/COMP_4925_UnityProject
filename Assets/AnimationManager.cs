using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        animator.SetBool("IsWalkingUp", Input.GetKey(KeyCode.W));
        animator.SetBool("IsWalkingDown", Input.GetKey(KeyCode.S));
        animator.SetBool("IsWalkingLeft", Input.GetKey(KeyCode.A));
        animator.SetBool("IsWalkingRight", Input.GetKey(KeyCode.D));
    }
}
