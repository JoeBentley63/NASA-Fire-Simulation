
using System;

namespace Kinesense.Interfaces.Enum
{
    [Flags]
    public enum OrdinalDirection
    {
        Up = 1,
        UpRight = 2,
        Right = 4,
        DownRight = 8,
        Down = 16,
        DownLeft = 32,
        Left = 64,
        UpLeft = 128
    } ;

    public enum OrdinalDirectionBasic
    {
        Up,        
        Right,
        Down,
        Left
    } ;
}
