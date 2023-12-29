using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RallyServer
{
    internal class ScoreTable
    {
        public static List<ScoreNote> scoreNotes;

        static ScoreTable()
        {
            scoreNotes = new List<ScoreNote>();
            var filename = "scores.txt";
            if (File.Exists(filename))
                scoreNotes = File.ReadAllLines(filename)
                    .Select(x => x.Split())
                    .Select(x => new ScoreNote
                    {
                        Nick = x.First(),
                        Score = float.Parse(x.Last())
                    }).ToList();
        }

        public static void SaveScores()
        {
            var filename = "scores.txt";
            File.WriteAllLines(filename, scoreNotes
                .Select(x => $"{x.Nick} {x.Score}").ToArray());
        }

        public static void Add(string nick, float score)
        {
            var note = new ScoreNote 
            { 
                Nick = nick, 
                Score = score 
            };
            scoreNotes.Add(note);
        }

        public static List<ScoreNote> GetScores() => scoreNotes;
    }
}
