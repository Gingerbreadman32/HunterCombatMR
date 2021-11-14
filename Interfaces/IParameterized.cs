using System;

namespace HunterCombatMR.Interfaces
{
    public interface IParameterized
    {
        Type ParameterType { get; }
        int RequiredParameters { get; }
    }
}