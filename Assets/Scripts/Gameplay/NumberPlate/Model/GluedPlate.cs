using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay.NumberPlate.Model
{
    public class GluedPlate : IPlatePiece
    {
        public float Width { get; }
        public bool IsLeftEdgeOpen => SubPlates[0].IsLeftEdgeOpen;
        public bool IsRightEdgeOpen => SubPlates[^1].IsRightEdgeOpen;
        public string Reading => string.Concat(SubPlates.Select(x => x.Reading));
        public IReadOnlyList<SubPlate> SubPlates { get; private set; }

        public GluedPlate(IReadOnlyList<SubPlate> subPlates)
        {
            if (subPlates == null)
                throw new ArgumentNullException(nameof(subPlates));
            if (subPlates.Count == 0)
                throw new ArgumentException("A glued plate needs at least one subplate", nameof(subPlates));

            SubPlates = subPlates;
            Width = subPlates.Sum(x => x.Width);
        }

        public bool CanCutAt(float localX)
        {
            var (index, subLocalX) = LocateCut(localX);
            if (index != -1)
            {
                return SubPlates[index].CanCutAt(subLocalX);
            }
            return false;
        }

        public (GluedPlate left, GluedPlate right) CutAt(float localX)
        {
            if (!CanCutAt(localX))
                throw new ArgumentOutOfRangeException(nameof(localX), "Argument out of bounds");

            var (index, subLocalX) = LocateCut(localX);
            var (leftCutSubPlate, rightCutSubPlate) = SubPlates[index].CutAt(subLocalX);
            List<SubPlate> leftList = new List<SubPlate>();
            List<SubPlate> rightList = new List<SubPlate>();
            leftList.AddRange(SubPlates.Take(index));
            leftList.Add(leftCutSubPlate);
            rightList.Add(rightCutSubPlate);
            rightList.AddRange(SubPlates.Skip(index + 1));
            return( new GluedPlate(leftList), new GluedPlate(rightList));
        }

        (IPlatePiece left, IPlatePiece right) IPlatePiece.CutAt(float localX) => CutAt(localX);

        public void Flip()
        {
            SubPlates = SubPlates.Reverse().ToArray();
            foreach (var sp in SubPlates) sp.Flip();
        }

        private (int index, float subLocalX) LocateCut(float localX)
        {
            for (int i = 0; i < SubPlates.Count; i++)
            {
                if (localX <= SubPlates[i].Width)
                {
                    return (i, localX);
                }
                localX -= SubPlates[i].Width;
            }
            return (-1, -1);
        }
    }
}