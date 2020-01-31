using UnityEngine.EventSystems;

namespace Juniper.Events
{
    /// <summary>
    /// Like a click event, but taking a lot longer to fulfill.
    /// </summary>
    public interface ILongPressHandler : IEventSystemHandler
    {
        /// <summary>
        /// In the process of registering a Long Press event,
        /// the receiving control has the option to update its
        /// visual representationi based on how long the user
        /// has been pressing it.
        /// </summary>
        /// <param name="evt"></param>
        void OnLongPressUpdate(PointerEventData evt);

        /// <summary>
        /// This method executes when the user presses on a
        /// control and does not release before the "long-press timeout"
        /// occurs. The "long-press timeout" is defined in
        /// <see cref="Input.Pointers.MappedButton{ButtonIDType}.THRESHOLD_LONG_PRESS"/>.
        /// </summary>
        /// <param name="evt"></param>
        void OnLongPress(PointerEventData evt);
    }
}
