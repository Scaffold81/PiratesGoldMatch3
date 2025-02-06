public class GetEnumBase : EnumProvider
{
    private void OnEnable()
    {
        Invoke(nameof(Get), 0.1f);
    }

    private void Get()
    {
        EnumSelection(SelectedEnumType);
    }
}
