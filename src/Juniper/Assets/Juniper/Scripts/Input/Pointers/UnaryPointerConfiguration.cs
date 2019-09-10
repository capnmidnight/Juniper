using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public class UnaryPointerConfiguration : AbstractPointerConfiguration<Unary>
    {
        public UnaryPointerConfiguration()
        {
            AddButton(Unary.One, InputButton.Left);
        }
    }
}
