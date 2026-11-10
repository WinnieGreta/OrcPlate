using System.Collections.Generic;

namespace Gameplay.NumberPlate.Model
{
    public interface IPlatePiece
    {
        float Width { get; }
        bool IsLeftEdgeOpen { get; }
        bool IsRightEdgeOpen { get; }
        string Reading { get; }
        IReadOnlyList<SubPlate> SubPlates { get; }

        bool CanCutAt(float localX);
        (IPlatePiece left, IPlatePiece right) CutAt(float localX);
        void Flip();
    }
}