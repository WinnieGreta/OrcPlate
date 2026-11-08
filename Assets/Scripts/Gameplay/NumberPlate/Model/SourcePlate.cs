using System.Collections.Generic;

namespace Gameplay.NumberPlate.Model
{
    public class SourcePlate
    {
        public readonly IReadOnlyList<Symbol> Symbols;
        public readonly float Width;

        public SourcePlate(IReadOnlyList<Symbol> symbols, float width)
        {
            Symbols = symbols;
            Width = width;
        }
    }
}