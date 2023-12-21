using CsvHelper;
using shared;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT05
{
    public enum ChallengeDifficulty
    {
        easy,
        medium,
        hard,
        complete,
        none
    }


    public class LevelData
    {
        public Level Name { get; set; } = Level.None;

        public ChallengeDifficulty GameDifficulty { get; set; } = ChallengeDifficulty.none;
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public string Correct { get; set; } = "";
        public string Wrong1 { get; set; } = "";
        public string Wrong2 { get; set; } = "";
        public string Wrong3 { get; set; } = "";
        public string Wrong4 { get; set; } = "";
        public string Wrong5 { get; set; } = "";

        public string GetWrongSequence(int id)
        {
            if ( id == -1) { id = 0; } // TODO: id should be random

            if ( id == 0 ) { return Wrong1; }
            if (id == 1) { return Wrong2; }
            if (id == 2) { return Wrong3; }
            if (id == 3) { return Wrong4; }
            return Wrong5;
        }
    }

    public sealed class LevelDatabase
    {
        const string FILE_LOCATION = "rna.csv";

        private static readonly LevelDatabase _instance = new LevelDatabase();

        public static LevelDatabase Instance
        {
            get { return _instance; }
        }

        Dictionary<Level, LevelData> _levelDatabase = new Dictionary<Level, LevelData>();

        public void Initialise()
        {
            LoadDataFromCSV();
        }

        public List<Level> GetLevelsOfDifficulty( ChallengeDifficulty targetDifficulty)
        {
            List<Level> levels = new List<Level>();

            foreach(var p in _levelDatabase )
            {
                if (p.Value.GameDifficulty == targetDifficulty)
                {
                    levels.Add(p.Key);
                }
            }

            return levels;
        }

        public Level GetRandomLevelOfDifficulty( ChallengeDifficulty targetDifficulty )
        {
            var levels = GetLevelsOfDifficulty( targetDifficulty );

            if (levels.Count == 0) return Level.None;

            return levels.GetRandomElement();
        }

        public List<NucleotideEnum> GetCorrectNucleotides( Level level)
        {
            var levelData = _levelDatabase[level];
            return ConvertStringToList_Nucleotides( levelData.Correct );
        }

        List<NucleotideEnum> ConvertStringToList_Nucleotides(string input)
        {
            List<NucleotideEnum> results = new List<NucleotideEnum>();

            foreach (char c in input)
            {
                var nucleotideEnum = GetEnumFromChar(c);
                if (nucleotideEnum != NucleotideEnum.None)
                {
                    results.Add(nucleotideEnum);
                }
                else
                {
                    int error=0;
                    ++error;
                }
            }

            return results;
        }

        private NucleotideEnum GetEnumFromChar(char c)
        {
            if (c == 'A') return NucleotideEnum.A;

            if (c == 'C') return NucleotideEnum.C;

            if (c == 'G') return NucleotideEnum.G;

            if (c == 'T') return NucleotideEnum.T;

            return NucleotideEnum.None;
        }

        public List<NucleotideEnum> GetWrongNucleotides(Level level, int id = -1)
        {
            

            var levelData = _levelDatabase[level];

            string nucleotideSequence = levelData.GetWrongSequence(id);
            return ConvertStringToList_Nucleotides( nucleotideSequence );
        }

        private void LoadDataFromCSV()
        {
            var locationPath = Path.Combine(Directory.GetCurrentDirectory(), FILE_LOCATION);
            using (var reader = new StreamReader(locationPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<LevelData>();

                foreach (var r in records)
                {
                    _levelDatabase.Add(r.Name, r);
                }
            }
        }

        public string GetDescription(Level level)
        {
            return _levelDatabase[level].Description;
        }
    }
}
