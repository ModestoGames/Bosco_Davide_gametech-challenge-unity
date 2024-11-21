using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorUIItem : UIItem
{
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _defaultVisibility;

    private int _parameterId;

    private void Start()
    {
        _parameterId = Animator.StringToHash(Strings.AnimatorKeys.Visible);
        _animator.SetBool(_parameterId, _defaultVisibility);

        if (_defaultVisibility)
            _animator.Play(Strings.AnimatorKeys.Show, 0, 1.0f);
        else
            _animator.Play(Strings.AnimatorKeys.Hide, 0, 1.0f);
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
