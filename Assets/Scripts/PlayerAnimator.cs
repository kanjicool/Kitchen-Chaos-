using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    [SerializeField] private Player player;
    private Animator animator;

    private void Awake()
    {

        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsWalking",player.IsWalking());
    }
}
