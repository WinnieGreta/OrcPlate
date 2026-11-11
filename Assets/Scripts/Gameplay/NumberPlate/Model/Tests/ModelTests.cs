using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

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
            new Symbol('7', 9f, 10f),
        };

        private SourcePlate _source1 = new SourcePlate(_symbols1, 9f);
        private SourcePlate _source2 = new SourcePlate(_symbols2, 9f);
        private SourcePlate _source3 = new SourcePlate(_symbols3, 11f);

        [Test]
        public void SubPlate_Uncut_ReadsAsSourcePlate()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            Assert.That(subPlate1.Reading, Is.EqualTo("AB234"));
            Assert.That(subPlate2.Reading, Is.EqualTo("MM189"));

            Assert.Throws<ArgumentNullException>(() => new SubPlate(null, 0f, 10f, true));
            Assert.Throws<ArgumentException>(() => new SubPlate(_source1, 0f, 0.01f, true));
        }

        [Test]
        public void SubPlate_CleanCutPieces_ReadingIntact()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            (IPlatePiece platePiece11, IPlatePiece platePiece12) = subPlate1.CutAt(3.75f);
            Assert.That(subPlate1.Reading, Is.EqualTo("AB234"));
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
        public void GluedPlate_Uncut_ReadsAsConcatSubPlates()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            SubPlate subPlate3 = new SubPlate(_source3, 0, _source3.Width, true);

            List<SubPlate> subPlates1 = new List<SubPlate>();
            subPlates1.Add(subPlate1);
            subPlates1.Add(subPlate2);
            subPlates1.Add(subPlate3);

            GluedPlate gluedPlate1 = new GluedPlate(subPlates1);
            
            Assert.That(gluedPlate1.Width, Is.EqualTo(subPlate1.Width + subPlate2.Width + subPlate3.Width));
            Assert.That(gluedPlate1.Reading, Is.EqualTo("AB234MM189CD657"));

            List<SubPlate> subPlates2 = new List<SubPlate>();
            subPlates2.Add(subPlate1);
            subPlate2.Flip();
            subPlates2.Add(subPlate2);
            subPlates2.Add(subPlate3);

            GluedPlate gluedPlate2 = new GluedPlate(subPlates2);
            
            Assert.That(gluedPlate2.Width, Is.EqualTo(29f));
            Assert.That(gluedPlate2.Reading, Is.EqualTo("AB234681WWCD657"));

        }

        [Test]
        public void GluedPlate_CanCutAtCheck()
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
            
            Assert.That(platePiece1.IsLeftEdgeOpen, Is.EqualTo(false));
            Assert.That(platePiece1.IsRightEdgeOpen, Is.EqualTo(true));
            Assert.That(platePiece2.IsLeftEdgeOpen, Is.EqualTo(true));
            Assert.That(platePiece2.IsRightEdgeOpen, Is.EqualTo(false));
            
            Assert.That(platePiece1.SubPlates[^1].Reading[^1], Is.EqualTo('M'));
            Assert.That(platePiece2.SubPlates[0].Reading[0], Is.EqualTo('1'));
            
            Assert.That(string.Concat(platePiece1.Reading, platePiece2.Reading), Is.EqualTo("AB234MM189CD657"));

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
            
            Assert.That(platePiece1.IsLeftEdgeOpen, Is.EqualTo(false));
            Assert.That(platePiece1.IsRightEdgeOpen, Is.EqualTo(true));
            Assert.That(platePiece2.IsLeftEdgeOpen, Is.EqualTo(true));
            Assert.That(platePiece2.IsRightEdgeOpen, Is.EqualTo(false));
            
            Assert.That(platePiece1.SubPlates[^1].Reading[^1], Is.EqualTo('?'));
            Assert.That(platePiece2.SubPlates[0].Reading[0], Is.EqualTo('?'));
            
            Assert.That(string.Concat(platePiece1.Reading, platePiece2.Reading), Is.EqualTo("AB234M??189CD657"));

        }

        [Test]
        public void GluedPlate_Flip()
        {
            SubPlate subPlate1 = new SubPlate(_source1, 0, _source1.Width, true);
            SubPlate subPlate2 = new SubPlate(_source2, 0, _source2.Width, true);
            SubPlate subPlate3 = new SubPlate(_source3, 0, _source3.Width, true);

            List<SubPlate> subPlates1 = new List<SubPlate>();
            subPlates1.Add(subPlate1);
            subPlates1.Add(subPlate2);
            subPlates1.Add(subPlate3);

            GluedPlate gluedPlate1 = new GluedPlate(subPlates1);
            
            gluedPlate1.Flip();
            Assert.That(gluedPlate1.Reading, Is.EqualTo("??9??681WW?????"));
            
            gluedPlate1.Flip();
            Assert.That(gluedPlate1.Reading, Is.EqualTo("AB234MM189CD657"));

            (GluedPlate gluedPlate11, GluedPlate gluedPlate12) = gluedPlate1.CutAt(14.25f);
            
            gluedPlate11.Flip();
            Assert.That(gluedPlate11.Reading, Is.EqualTo("1WW?????"));
            Assert.That(gluedPlate11.IsLeftEdgeOpen, Is.EqualTo(true));
            Assert.That(gluedPlate11.IsRightEdgeOpen, Is.EqualTo(false));
            
            gluedPlate12.Flip();
            Assert.That(gluedPlate12.Reading, Is.EqualTo("??9??68"));
            Assert.That(gluedPlate12.IsRightEdgeOpen, Is.EqualTo(true));
            Assert.That(gluedPlate12.IsLeftEdgeOpen, Is.EqualTo(false));

            (GluedPlate gluedPlate111, GluedPlate gluedPlate112) = gluedPlate11.CutAt(5f);
            Assert.That(gluedPlate111.Reading, Is.EqualTo("1WW"));
            Assert.That(gluedPlate111.IsLeftEdgeOpen, Is.EqualTo(true));
            Assert.That(gluedPlate111.IsRightEdgeOpen, Is.EqualTo(true));
            
            gluedPlate111.Flip();
            Assert.That(gluedPlate111.Reading, Is.EqualTo("MM1"));
            Assert.That(gluedPlate111.IsLeftEdgeOpen, Is.EqualTo(true));
            Assert.That(gluedPlate111.IsRightEdgeOpen, Is.EqualTo(true));
        }

        [Test]
        public void GluedPlate_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new GluedPlate(null));
            var zeroSubPlates = new List<SubPlate>();
            Assert.Throws<ArgumentException>(() => new GluedPlate(zeroSubPlates));
            var subPlates = new List<SubPlate>()
                { 
                    new SubPlate(_source1, 0, _source1.Width, true), 
                    new SubPlate(_source2, 0, _source2.Width, true) 
                };
            GluedPlate gluedPlate = new GluedPlate(subPlates);
            Assert.That(gluedPlate.Reading, Is.EqualTo("AB234MM189"));
            subPlates.Add(new SubPlate(_source3, 0, _source3.Width, true));
            Assert.That(gluedPlate.Reading, Is.EqualTo("AB234MM189"));
        }

        [Test]
        public void GluedPlate_CanJoin()
        {
            GluedPlate gluedPlate1 = new GluedPlate( new List<SubPlate>() { new SubPlate(_source1, 0, _source1.Width, true) });
            GluedPlate gluedPlate2 = new GluedPlate( new List<SubPlate>() { new SubPlate(_source2, 0, _source2.Width, true) });
            Assert.That(GluedPlate.CanJoin(gluedPlate1, gluedPlate2), Is.EqualTo(false));
            
            GluedPlate gluedPlate3 = new GluedPlate( new List<SubPlate>() { new SubPlate(_source1, 0, _source1.Width, true, false, true) });
            GluedPlate gluedPlate4 = new GluedPlate( new List<SubPlate>() { new SubPlate(_source2, 0, _source2.Width, true, true, false) });
            Assert.That(GluedPlate.CanJoin(gluedPlate3, gluedPlate4), Is.EqualTo(true));
            Assert.That(GluedPlate.Join(gluedPlate3, gluedPlate4).Reading, Is.EqualTo("AB234MM189"));
            Assert.That(GluedPlate.Join(gluedPlate3, gluedPlate4).IsLeftEdgeOpen, Is.EqualTo(gluedPlate3.IsLeftEdgeOpen));
            Assert.That(GluedPlate.Join(gluedPlate3, gluedPlate4).IsRightEdgeOpen, Is.EqualTo(gluedPlate4.IsRightEdgeOpen));
            
            GluedPlate gluedPlate5 = new GluedPlate( new List<SubPlate>() { new SubPlate(_source1, 0, _source1.Width, true, true, true) });
            Assert.Throws<ArgumentException>(() => GluedPlate.Join(gluedPlate5, gluedPlate5));

            GluedPlate gluedPlate6 = GluedPlate.Join(GluedPlate.Join(gluedPlate3, gluedPlate5), gluedPlate4);
            Assert.That(gluedPlate6.SubPlates.Count, Is.EqualTo(3));
            Assert.That(gluedPlate6, Is.TypeOf(typeof(GluedPlate)));

        }
    }
}