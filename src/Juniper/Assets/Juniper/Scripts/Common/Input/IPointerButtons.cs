namespace Juniper.Input.Pointers
{
    public interface IPointerButtons<ButtonIDType>
        where ButtonIDType : struct
    {
        string name
        {
            get;
        }

        bool IsConnected
        {
            get;
        }

        bool AnyButtonPressed
        {
            get;
        }

        bool IsButtonDown(ButtonIDType button);

        bool IsButtonPressed(ButtonIDType button);

        bool IsButtonUp(ButtonIDType button);
    }
}
