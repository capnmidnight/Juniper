using UnityEngine;

namespace Juniper.Input.Pointers
{
    public class UnaryPointerConfiguration : AbstractPointerConfiguration<Unary>
    {
        public UnaryPointerConfiguration()
        {
            AddButton(Unary.One, KeyCode.Mouse0);
        }
    }
}
