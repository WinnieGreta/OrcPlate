using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay.NumberPlate.Model
{
    public class SubPlate : IPlatePiece
    {
        public SourcePlate Source { get; }
        public float LeftSourceX { get; }
        public float RightSourceX { get; }
        private readonly bool _isLeftSourceEdgeOpen, _isRightSourceEdgeOpen;
        public bool IsUpright { get; private set; }
        public bool IsLeftEdgeOpen => IsUpright ? _isLeftSourceEdgeOpen : _isRightSourceEdgeOpen;
        public bool IsRightEdgeOpen => IsUpright ? _isRightSourceEdgeOpen : _isLeftSourceEdgeOpen;
        public float Width => RightSourceX - LeftSourceX;
        public IReadOnlyList<SubPlate> SubPlates { get; }
        public const float MinCutWidth = 0.1f;
        
        public SubPlate(SourcePlate source, float leftX, float rightX, bool isUpright, bool isLeftSourceEdgeOpen = false, bool isRightSourceEdgeOpen = false)
        {
            if (source == null)
                throw new ArgumentNullException("Subplate must have source plate");
            
            if (leftX < 0 || rightX <= leftX + MinCutWidth)
                throw new ArgumentException($"Subplate width must be greater than minimum {MinCutWidth}");
            
            Source = source;
            LeftSourceX = leftX;
            RightSourceX = rightX;
            IsUpright = isUpright;
            _isLeftSourceEdgeOpen = isLeftSourceEdgeOpen;
            _isRightSourceEdgeOpen = isRightSourceEdgeOpen;
            SubPlates = new[] { this };
        }

        public bool CanCutAt(float localX) => localX > MinCutWidth && localX < Width - MinCutWidth;

        public (SubPlate left, SubPlate right) CutAt(float localX)
        {
            if (!CanCutAt(localX))
                throw new ArgumentOutOfRangeException(nameof(localX), "Argument out of bounds");
            
            float sourceX = ToSource(localX);
            var leftPiece = new SubPlate(Source, LeftSourceX, sourceX, IsUpright, _isLeftSourceEdgeOpen, true);
            var rightPiece = new SubPlate(Source, sourceX, RightSourceX, IsUpright, true, _isRightSourceEdgeOpen);
            return IsUpright ? (leftPiece, rightPiece) : (rightPiece, leftPiece);
        }

        (IPlatePiece left, IPlatePiece right) IPlatePiece.CutAt(float localX) => CutAt(localX);

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