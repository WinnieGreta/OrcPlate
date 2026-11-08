using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay.NumberPlate.Model
{
    public class PlatePiece
    {
        public bool IsLeftEdgeOpen, IsRightEdgeOpen;
        public readonly SourcePlate Source;
        public readonly float LeftSourceX, RightSourceX;
        public bool IsUpright { get; private set; }
        public float Width => RightSourceX - LeftSourceX;
        public const float MinWidth = 0.1f;
        
        public PlatePiece(SourcePlate source, float leftX, float rightX, bool isUpright, bool isLeftEdgeOpen = false, bool isRightEdgeOpen = false)
        {
            Source = source;
            LeftSourceX = leftX;
            RightSourceX = rightX;
            IsUpright = isUpright;
            IsLeftEdgeOpen = isLeftEdgeOpen;
            IsRightEdgeOpen = isRightEdgeOpen;
        }

        public bool CanCutAt(float localX) => localX > MinWidth && localX < Width - MinWidth;

        public (PlatePiece left, PlatePiece right) CutAt(float localX)
        {
            if (!CanCutAt(localX))
            {
                throw new ArgumentOutOfRangeException("localX", "Argument out of bounds");
            }
            float sourceX = ToSource(localX);
            var leftPiece = new PlatePiece(Source, LeftSourceX, sourceX, IsUpright, IsLeftEdgeOpen, true);
            var rightPiece = new PlatePiece(Source, sourceX, RightSourceX, IsUpright, true, IsRightEdgeOpen);
            return IsUpright ? (leftPiece, rightPiece) : (rightPiece, leftPiece);
        }

        private float ToSource(float localX) => IsUpright ? LeftSourceX + localX : RightSourceX - localX;

        public void Flip()
        {
            IsUpright = !IsUpright;
        }

        public IEnumerable<(Symbol Symbol, bool IsIntact)> VisibleSymbols()
        {
            var symbolsOnPiece = Source.Symbols.Where(x => x.LeftX < RightSourceX && x.RightX > LeftSourceX);
            if (!IsUpright)
                symbolsOnPiece = symbolsOnPiece.Reverse();
            foreach (var s in symbolsOnPiece)
            {
                yield return (s, s.LeftX >= LeftSourceX && s.RightX <= RightSourceX);
            }
        }

        public string Reading =>
            string.Concat(VisibleSymbols().Select(x => !x.IsIntact ? '?' : x.Symbol.Read(IsUpright)));

    }
}