using System.Xml.Linq;

namespace Juniper
{
    public class SsmlDocument
    {
        public string Text { get; set; }
        public string VoiceName { get; set; }

        private string _style;
        public string Style
        {
            get => _style;
            set => _style = value
                ?.Trim()
                ?.ToLowerInvariant();
        }

        public bool HasStyle =>
            !string.IsNullOrEmpty(Style)
            && Style != "none";

        public override string ToString()
        {
            var xml = XNamespace.Xml;
            var xmlns = XNamespace.Xmlns;
            XNamespace ssml = "http://www.w3.org/2001/10/synthesis";
            XNamespace mstts = "https://www.w3.org/2001/mstts";

            var speak = new XElement(
                ssml + "speak",
                new XAttribute("version", "1.0"),
                new XAttribute(xmlns + "mstts", mstts),
                new XAttribute(xml + "lang", "en-US"));

            var voice = new XElement(ssml + "voice",
                new XAttribute("name", VoiceName));

            speak.Add(voice);

            if (HasStyle)
            {
                var style = new XElement(
                    mstts + "express-as",
                    new XAttribute("style", Style),
                    new XText(Text));
                voice.Add(style);
            }
            else
            {
                voice.Add(new XText(Text));
            }
            
            return speak.ToString();
        }
    }
}
