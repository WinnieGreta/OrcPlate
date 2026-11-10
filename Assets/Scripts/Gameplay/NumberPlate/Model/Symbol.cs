using System.Collections.Generic;

namespace Gameplay.NumberPlate.Model
{
    public readonly struct Symbol
    {
        public readonly char Character;
        public readonly float LeftX, RightX; // drawn shape edges in source plate units

        public Symbol(char character, float leftX, float rightX)
        {
            Character = character;
            LeftX = leftX;
            RightX = rightX;
        }

        private static readonly Dictionary<char, char> rotatedChars = new()
        {
            ['0'] = '0', ['1'] = '1', ['8'] = '8', ['6'] = '9', ['9'] = '6',
            ['I'] = 'I', ['H'] = 'H', ['O'] = 'O', ['S'] = 'S', ['N'] = 'N',
            ['X'] = 'X', ['Z'] = 'Z', ['M'] = 'W', ['W'] = 'M'
        };

        public char Read(bool isUpright) => isUpright ? Character : rotatedChars.TryGetValue(Character, out var c) ? c : '?';
        
    }
}