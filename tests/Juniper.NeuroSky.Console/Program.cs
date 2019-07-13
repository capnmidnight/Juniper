using static System.Console;

namespace Juniper.NeuroSky.Console
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            WriteLine($"MindWave version {MindWaveAdapter.Version}");
            using (var device = MindWaveAdapter.FindAdapter())
            {
                if (device == null)
                {
                    WriteLine("No device available");
                }
                else
                {
                    WriteLine(
$@"Connected:

Mains Frequency: {device.MainsFrequency}
Baud Rate:       {device.SerialBaudRate}");

                    string lastOutput = null;
                    while (true)
                    {
                        var packetsRead = device.ReadPackets(1);
                        if (packetsRead > 0)
                        {
                            var output =
$@"Packets Read:  {packetsRead}
Battery:        {device.Battery}
Signal:         {device.PoorSignal}
Attention:      {device.Attention}
Meditation:     {device.Meditation}
Delta:          {device.Delta}
Theta:          {device.Theta}
Alpha1:         {device.Alpha1}
Alpha2:         {device.Alpha2}
Beta1:          {device.Beta1}
Beta2:          {device.Beta2}
Gamma1:         {device.Gamma1}
Gamma2:         {device.Gamma2}
";

                            if (output != lastOutput)
                            {
                                lastOutput = output;
                                WriteLine(output);
                            }
                        }
                    }
                }
            }
        }
    }
}
