using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Gameplay.NumberPlate.Model.Tests
{
    public class ModelTests
    {
        
        private static IReadOnlyList<Symbol> _symbols1 = new List<Symbol>
        {
            new Symbol('A', 1f, 2f),
            new Symbol('B', 2.5f, 3.5f),
            new Symbol('2', 4f, 5f),
            new Symbol('3', 5.5f, 6.5f),
            new Symbol('4', 7f, 8f),
        };
        
        private static IReadOnlyList<Symbol> _symbols2 = new List<Symbol>()
        {
            new Symbol('M', 1f, 2f),
            new Symbol('M', 2.5f, 3.5f),
            new Symbol('1', 4f, 5f),
            new Symbol('8', 5.5f, 6.5f),
            new Symbol('9', 7f, 8f),
        };
        
        private static IReadOnlyList<Symbol> _symbols3 = new List<Symbol>()
        {
            new Symbol('C', 1f, 2f),
            new Symbol('D', 3f, 4f),
            new Symbol('6', 5f, 6f),
            new Symbol('5', 7f, 8f),
            new Symbol('7', 9f, 9f),
        };

        private SourcePlate _source1 = new SourcePlate(_symbols1, 9f);
        private SourcePlate _source2 = new SourcePlate(_symbols2, 9f);
        private SourcePlate _source3 = new SourcePlate(_symbols3, 11f);

        [Test]
        public void SubPlate_Uncut_ReadsAsSourcePlate()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            Assert.That(subPlate1.Reading, Is.EqualTo(string.Concat(_symbols1.Select(x => x.Character))));
            Assert.That(subPlate2.Reading, Is.EqualTo(string.Concat(_symbols2.Select(x => x.Character))));
        }

        [Test]
        public void SubPlate_CleanCutPieces_ReadingIntact()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            (IPlatePiece platePiece11, IPlatePiece platePiece12) = subPlate1.CutAt(3.75f);
            Assert.That(subPlate1.Reading, Is.EqualTo(string.Concat(platePiece11.Reading, platePiece12.Reading)));
        }

        [Test]
        public void SubPlate_DirtyCutPieces_ReadingBroken()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            (IPlatePiece platePiece11, IPlatePiece platePiece12) = subPlate1.CutAt(3f);
            string reading1 = subPlate1.Reading;
            string reading11 = platePiece11.Reading;
            string reading12 = platePiece12.Reading;
            Assert.That(reading11[^1], Is.EqualTo('?'));
            Assert.That(reading12[0], Is.EqualTo('?'));
            Assert.That(reading11.Length + reading12.Length, Is.EqualTo(reading1.Length + 1));
        }

        [Test]
        public void SubPlate_CutAndFlip_NonRotatableSymbols()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            (IPlatePiece platePiece11, IPlatePiece platePiece12) = subPlate1.CutAt(3.75f);
            string uprightPieceReading1 = platePiece11.Reading;
            string uprightPieceReading2 = platePiece12.Reading;
            platePiece11.Flip();
            platePiece12.Flip();
            Assert.That(platePiece11.Reading, Is.EqualTo("??"));
            Assert.That(platePiece12.Reading, Is.EqualTo("???"));
            platePiece11.Flip();
            platePiece12.Flip();
            Assert.That(platePiece11.Reading, Is.EqualTo(uprightPieceReading1));
            Assert.That(platePiece12.Reading, Is.EqualTo(uprightPieceReading2));
        }

        [Test]
        public void SubPlate_CutAndFlip_RotatableSymbols()
        {
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            (IPlatePiece platePiece21, IPlatePiece platePiece22) = subPlate2.CutAt(3.75f);
            string uprightPieceReading1 = platePiece21.Reading;
            string uprightPieceReading2 = platePiece22.Reading;
            platePiece21.Flip();
            platePiece22.Flip();
            Assert.That(platePiece21.Reading, Is.EqualTo("WW"));
            Assert.That(platePiece22.Reading, Is.EqualTo("681"));
            platePiece21.Flip();
            platePiece22.Flip();
            Assert.That(platePiece21.Reading, Is.EqualTo(uprightPieceReading1)); 
            Assert.That(platePiece22.Reading, Is.EqualTo(uprightPieceReading2));
        }

        [Test]
        public void SubPlate_FlipAndCut_NonRotatableSymbols()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            subPlate1.Flip();
            (IPlatePiece platePiece11, IPlatePiece platePiece12) = subPlate1.CutAt(3.75f);
            Assert.That(platePiece11.Reading, Is.EqualTo("??")); 
            Assert.That(platePiece12.Reading, Is.EqualTo("???"));
            platePiece11.Flip();
            platePiece12.Flip();
            Assert.That(platePiece11.Reading, Is.EqualTo("34"));
            Assert.That(platePiece12.Reading, Is.EqualTo("AB2"));
        }

        [Test]
        public void SubPlate_FlipAndCut_RotatableSymbols()
        {
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            subPlate2.Flip();
            (IPlatePiece platePiece21, IPlatePiece platePiece22) = subPlate2.CutAt(3.75f);
            Assert.That(platePiece21.Reading, Is.EqualTo("68"));
            Assert.That(platePiece22.Reading, Is.EqualTo("1WW"));
            platePiece21.Flip();
            platePiece22.Flip();
            Assert.That(platePiece21.Reading, Is.EqualTo("89"));
            Assert.That(platePiece22.Reading, Is.EqualTo("MM1"));
        }

        [Test]
        public void SubPlate_CanCutAtCheck()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            Assert.That(!subPlate1.CanCutAt(-3f));
            Assert.That(!subPlate1.CanCutAt(0));
            Assert.That(!subPlate1.CanCutAt(0.01f));
            Assert.That(!subPlate1.CanCutAt(subPlate1.Width));
            Assert.That(!subPlate1.CanCutAt(subPlate1.Width - 0.01f));
            Assert.That(!subPlate1.CanCutAt(subPlate1.Width + 10f));
            Assert.That(subPlate1.CanCutAt(subPlate1.Width / 2));
        }

        [Test]
        public void GluedPiece_Uncut_ReadsAsConcatSubPlates()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source1, 0, _source2.Width, true);
            SubPlate subPlate3 = new SubPlate(_source1, 0, _source3.Width, true);

            List<SubPlate> subPlates1 = new List<SubPlate>();
            subPlates1.Add(subPlate1);
            subPlates1.Add(subPlate2);
            subPlates1.Add(subPlate3);

            GluedPlate gluedPlate1 = new GluedPlate(subPlates1);
            
            Assert.That(gluedPlate1.Width, Is.EqualTo(subPlate1.Width + subPlate2.Width + subPlate3.Width));
            Assert.That(gluedPlate1.Reading, Is.EqualTo(subPlate1.Reading + subPlate2.Reading + subPlate3.Reading));

            List<SubPlate> subPlates2 = new List<SubPlate>();
            subPlates2.Add(subPlate1);
            subPlate2.Flip();
            subPlates2.Add(subPlate2);
            subPlates2.Add(subPlate3);

            GluedPlate gluedPlate2 = new GluedPlate(subPlates2);
            
            Assert.That(gluedPlate2.Width, Is.EqualTo(subPlate1.Width + subPlate2.Width + subPlate3.Width));
            Assert.That(gluedPlate2.Reading, Is.EqualTo(subPlate1.Reading + subPlate2.Reading + subPlate3.Reading));

        }

        [Test]
        public void GluedPiece_CanCutAtCheck()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            SubPlate subPlate3 = new SubPlate(_source3, 0, _source3.Width, true);

            List<SubPlate> subPlates1 = new List<SubPlate>();
            subPlates1.Add(subPlate1);
            subPlates1.Add(subPlate2);
            subPlates1.Add(subPlate3);

            GluedPlate gluedPlate1 = new GluedPlate(subPlates1);
            
            Assert.That(!gluedPlate1.CanCutAt(-3f));
            Assert.That(!gluedPlate1.CanCutAt(0));
            Assert.That(!gluedPlate1.CanCutAt(subPlate1.Width));
            Assert.That(!gluedPlate1.CanCutAt(subPlate1.Width + subPlate2.Width));
            Assert.That(!gluedPlate1.CanCutAt(subPlate1.Width + subPlate2.Width - 0.01f));
            Assert.That(!gluedPlate1.CanCutAt(subPlate1.Width + subPlate2.Width + 0.01f));
            Assert.That(!gluedPlate1.CanCutAt(subPlate1.Width + subPlate2.Width + subPlate3.Width));
            Assert.That(!gluedPlate1.CanCutAt(subPlate1.Width + subPlate2.Width + subPlate3.Width + 3f));
            
            Assert.That(gluedPlate1.CanCutAt(subPlate1.Width / 2));
            Assert.That(gluedPlate1.CanCutAt( subPlate1.Width + subPlate2.Width / 2));
            Assert.That(gluedPlate1.CanCutAt( subPlate1.Width + subPlate2.Width + subPlate3.Width / 2));
        }

        [Test]
        public void GluedPlate_CleanCut_ReadsAsSubPlatePieces()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            SubPlate subPlate3 = new SubPlate(_source3, 0, _source3.Width, true);

            List<SubPlate> subPlates1 = new List<SubPlate>();
            subPlates1.Add(subPlate1);
            subPlates1.Add(subPlate2);
            subPlates1.Add(subPlate3);

            GluedPlate gluedPlate1 = new GluedPlate(subPlates1);

            (GluedPlate platePiece1, GluedPlate platePiece2) = gluedPlate1.CutAt(subPlate1.Width + 3.75f);
            
            Assert.That(platePiece1.Width, Is.EqualTo(subPlate1.Width + 3.75f));
            Assert.That(platePiece2.Width, Is.EqualTo(gluedPlate1.Width - subPlate1.Width - 3.75f));
            
            Assert.That(platePiece1.IsLeftEdgeOpen, Is.EqualTo(subPlate1.IsLeftEdgeOpen));
            Assert.That(platePiece1.IsRightEdgeOpen);
            Assert.That(platePiece2.IsLeftEdgeOpen);
            Assert.That(platePiece2.IsRightEdgeOpen, Is.EqualTo(subPlate3.IsRightEdgeOpen));
            
            Assert.That(platePiece1.SubPlates[^1].Reading[^1], Is.EqualTo('M'));
            Assert.That(platePiece2.SubPlates[0].Reading[0], Is.EqualTo('1'));
            
            Assert.That(string.Concat(platePiece1.Reading, platePiece2.Reading), Is.EqualTo(gluedPlate1.Reading));

        }
        
        [Test]
        public void GluedPlate_DirtyCut_ReadsAsSubPlatePieces()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            SubPlate subPlate3 = new SubPlate(_source3, 0, _source3.Width, true);

            List<SubPlate> subPlates1 = new List<SubPlate>();
            subPlates1.Add(subPlate1);
            subPlates1.Add(subPlate2);
            subPlates1.Add(subPlate3);

            GluedPlate gluedPlate1 = new GluedPlate(subPlates1);

            (GluedPlate platePiece1, GluedPlate platePiece2) = gluedPlate1.CutAt(subPlate1.Width + 3f);
            
            Assert.That(platePiece1.Width, Is.EqualTo(subPlate1.Width + 3f));
            Assert.That(platePiece2.Width, Is.EqualTo(gluedPlate1.Width - subPlate1.Width - 3f));
            
            Assert.That(platePiece1.IsLeftEdgeOpen, Is.EqualTo(subPlate1.IsLeftEdgeOpen));
            Assert.That(platePiece1.IsRightEdgeOpen);
            Assert.That(platePiece2.IsLeftEdgeOpen);
            Assert.That(platePiece2.IsRightEdgeOpen, Is.EqualTo(subPlate3.IsRightEdgeOpen));
            
            Assert.That(platePiece1.SubPlates[^1].Reading[^1], Is.EqualTo('?'));
            Assert.That(platePiece2.SubPlates[0].Reading[0], Is.EqualTo('?'));
            
            Assert.That(string.Concat(platePiece1.Reading, platePiece2.Reading), !Is.EqualTo(gluedPlate1.Reading));

        }
    }
}