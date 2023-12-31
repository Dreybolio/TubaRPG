using UnityEngine;
public enum ModelDirection
{
    FORWARD,
    BACKWARD,
    LEFT,
    RIGHT,
}
public class CharacterModel : MonoBehaviour
{
    [SerializeField] private GameObject spriteForward;
    [SerializeField] private GameObject spriteBackward;
    [SerializeField] private RuntimeAnimatorController battleController;
    [SerializeField] private RuntimeAnimatorController overworldController;
    [SerializeField] private bool leftIsDefault;
    private Animator animator;

    private readonly Vector3 NORMAL = new(1, 1, 1);
    private readonly Vector3 INVERTED = new(-1, 1, 1);
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetAsBattleModel()
    {
        animator.runtimeAnimatorController = battleController;
    }
    public void SetAsOverworldModel()
    {
        animator.runtimeAnimatorController = overworldController;
    }
    public void SetDirection(ModelDirection dir)
    {
        switch (dir)
        {
            case ModelDirection.FORWARD:
                if (spriteForward != null) { spriteForward.SetActive(true); }
                if (spriteBackward != null) { spriteBackward.SetActive(false); }
                break;
            case ModelDirection.BACKWARD:
                if (spriteForward != null) { spriteForward.SetActive(false); }
                if (spriteBackward != null) { spriteBackward.SetActive(true); }
                break;
            case ModelDirection.LEFT:
                if(leftIsDefault) { transform.localScale = NORMAL; }
                else { transform.localScale = INVERTED; }
                break;
            case ModelDirection.RIGHT:
                if (leftIsDefault) { transform.localScale = INVERTED; }
                else { transform.localScale = NORMAL; }
                break;
        }
    }
}
