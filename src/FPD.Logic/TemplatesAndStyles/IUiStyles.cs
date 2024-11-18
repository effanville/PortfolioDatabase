using System.ComponentModel;

namespace Effanville.FPD.Logic.TemplatesAndStyles;

public interface IUiStyles : INotifyPropertyChanged
{
    bool IsLightTheme { get; }
    void UpdateTheme(bool isLightTheme);
}