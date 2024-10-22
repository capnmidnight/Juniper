namespace Juniper.Input;

public interface IKeyEventSource
{
    event EventHandler<KeyChangeEventArgs> Changed;
    event EventHandler<KeyEventArgs> Down;
    event EventHandler<KeyEventArgs> Up;

    bool IsDown(string name);
    float GetValue(string name);
    void DefineAxis(string name, string negative, string positive);
    float GetAxis(string name);
    void Start();
    void Quit();
}
