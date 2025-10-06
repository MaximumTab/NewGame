using UnityEngine;

public class treeantTEST : MonoBehaviour
{
    Animator animator;
    
    void Start() {
        animator = GetComponent<Animator>();
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("animation is activating"); 
            animator.SetBool("IsWall", true);
        } else {
            animator.SetBool("IsWall", false);
        }
    }
}
