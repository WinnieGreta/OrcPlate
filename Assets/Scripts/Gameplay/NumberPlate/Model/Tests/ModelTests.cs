using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Zenject;

namespace Gameplay.NumberPlate.Model.Tests
{
    public class ModelTests : ZenjectUnitTestFixture
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

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

        private SourcePlate _source1 = new SourcePlate(_symbols1, 9f);
        private SourcePlate _source2 = new SourcePlate(_symbols2, 9f);

        [Test]
        public void PlatePiece_UncutReadsAsSourcePlate()
        {
            PlatePiece platePiece1 = new PlatePiece(_source1, 0, _source1.Width, true);
            PlatePiece platePiece2 = new PlatePiece(_source2, 0, _source1.Width, true);
            Assert.That(platePiece1.Reading.Equals(string.Concat(_symbols1.Select(x => x.Character))));
            Assert.That(platePiece2.Reading.Equals(string.Concat(_symbols2.Select(x => x.Character))));
        }

        [Test]
        public void PlatePiece_CleanCutPieces_ReadingIntact()
        {
            PlatePiece platePiece1 = new PlatePiece(_source1, 0, _source1.Width, true);
            (PlatePiece platePiece11, PlatePiece platePiece12) = platePiece1.CutAt(3.75f);
            Assert.That(platePiece1.Reading.Equals(string.Concat(platePiece11.Reading, platePiece12.Reading)));
        }

        [Test]
        public void PlatePiece_DirtyCutPieces_ReadingBroken()
        {
            PlatePiece platePiece1 = new PlatePiece(_source1, 0, _source1.Width, true);
            (PlatePiece platePiece11, PlatePiece platePiece12) = platePiece1.CutAt(3f);
            string reading1 = platePiece1.Reading;
            string reading11 = platePiece11.Reading;
            string reading12 = platePiece12.Reading;
            Assert.That(reading11[^1] == '?' && reading12[0] == '?');
            Assert.That(reading11.Length + reading12.Length == reading1.Length + 1);
        }

        [Test]
        public void PlatePiece_CutAndFlip_NonRotatableSymbols()
        {
            PlatePiece platePiece1 = new PlatePiece(_source1, 0, _source1.Width, true);
            (PlatePiece platePiece11, PlatePiece platePiece12) = platePiece1.CutAt(3.75f);
            string uprightPieceReading1 = platePiece11.Reading;
            string uprightPieceReading2 = platePiece12.Reading;
            platePiece11.Flip();
            platePiece12.Flip();
            Assert.That(platePiece11.Reading == "??" && platePiece12.Reading == "???");
            platePiece11.Flip();
            platePiece12.Flip();
            Assert.That(platePiece11.Reading == uprightPieceReading1 && platePiece12.Reading == uprightPieceReading2);
        }

        [Test]
        public void PlatePiece_CutAndFlip_RotatableSymbols()
        {
            PlatePiece platePiece2 = new PlatePiece(_source2, 0, _source2.Width, true);
            (PlatePiece platePiece21, PlatePiece platePiece22) = platePiece2.CutAt(3.75f);
            string uprightPieceReading1 = platePiece21.Reading;
            string uprightPieceReading2 = platePiece22.Reading;
            platePiece21.Flip();
            platePiece22.Flip();
            Assert.That(platePiece21.Reading == "WW" && platePiece22.Reading == "681");
            platePiece21.Flip();
            platePiece22.Flip();
            Assert.That(platePiece21.Reading == uprightPieceReading1 && platePiece22.Reading == uprightPieceReading2);
        }

        [Test]
        public void PlatePiece_FlipAndCut_NonRotatableSymbols()
        {
            PlatePiece platePiece1 = new PlatePiece(_source1, 0, _source1.Width, true);
            platePiece1.Flip();
            (PlatePiece platePiece11, PlatePiece platePiece12) = platePiece1.CutAt(3.75f);
            Assert.That(platePiece11.Reading == "??" && platePiece12.Reading == "???");
            platePiece11.Flip();
            platePiece12.Flip();
            Assert.That(platePiece11.Reading == "34" && platePiece12.Reading == "AB2");
        }

        [Test]
        public void PlatePiece_FlipAndCut_RotatableSymbols()
        {
            PlatePiece platePiece2 = new PlatePiece(_source2, 0, _source2.Width, true);
            platePiece2.Flip();
            (PlatePiece platePiece21, PlatePiece platePiece22) = platePiece2.CutAt(3.75f);
            Assert.That(platePiece21.Reading == "68" && platePiece22.Reading == "1WW");
            platePiece21.Flip();
            platePiece22.Flip();
            Assert.That(platePiece21.Reading == "89" && platePiece22.Reading == "MM1");
        }

        [Test]
        public void PlatePiece_CanCutAtCheck()
        {
            PlatePiece platePiece1 = new PlatePiece(_source1, 0, _source1.Width, true);
            Assert.That(!platePiece1.CanCutAt(-3f));
            Assert.That(!platePiece1.CanCutAt(0));
            Assert.That(!platePiece1.CanCutAt(0.01f));
            Assert.That(!platePiece1.CanCutAt(platePiece1.Width));
        }
    }
}