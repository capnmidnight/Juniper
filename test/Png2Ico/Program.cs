using Juniper.Imaging.Ico;

var here = new DirectoryInfo(Directory.GetCurrentDirectory());
var images = new[]
{
    here.Touch("foxglove_32.png"),
    here.Touch("foxglove_64.png"),
    here.Touch("foxglove_128.png"),
    here.Touch("foxglove_256.png"),
};

Ico.Concatenate(here.Touch("foxglove.ico"), images);