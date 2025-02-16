using EasyWoWMacro.Business.Models;

namespace EasyWoWMacro.Business.Services.Interface;

public interface IMacroParser
{
    Macro ParseMacro(string? input);
}