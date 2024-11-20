using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorUIItem : UIItem
{
    [SerializeField] private Animator _animator;

    private int _parameterId;

    private void Start()
    {
        _parameterId = Animator.StringToHash(Strings.AnimatorKeys.Visible);
    }

    public override void PerformShowAction()
    {
        _animator.SetBool(_parameterId, true);
    }

    public override void PerformHideAction()
    {
        _animator.SetBool(_parameterId, false);
    }
}
