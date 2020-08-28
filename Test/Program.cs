using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TNoodle.Puzzles;

namespace Test
{
    public class SolutionResponse
    {
        public string Solution { get; set; }
    }
    public class Program
    {
        private static string[] ALG_POW = new[] { "", "2", "'" };

        private static int move_id(string move)
        {
            switch (move)
            {
                case "U":
                    return 0;
                case "R":
                    return 1;
                case "F":
                    return 2;
                case "D":
                    return 3;
                case "L":
                    return 4;
                case "B":
                    return 5;
                case "u":
                    return 6;
                case "r":
                    return 7;
                case "f":
                    return 8;
                case "d":
                    return 9;
                case "l":
                    return 10;
                case "b":
                    return 11;
                case "E":
                    return 12;
                case "M":
                    return 13;
                case "S":
                    return 14;
                case "x":
                    return 15;
                case "y":
                    return 16;
                case "z":
                    return 17;
            }
            return -1;
        }

        // Returns the power of a move with given suffix
        public static int move_pow(string chr)
        {
            switch (chr)
            {
                case "2":
                    return 1;
                case "'":
                    return 2;
                case "3":
                    return 2;
            }
            return 0;
        }
        public static string invert_alg(string alg)
        {
            string inverse = "";
            int pow = 0;
            int i = alg.Length - 1;
            while (i >= 0)
            {
                int move = move_id(alg.Substring(i, 1));
                if (move != -1)
                {
                    inverse += alg.Substring(i, 1) + ALG_POW[2 - pow] + " ";
                    pow = 0;
                }
                else
                    pow = move_pow(alg.Substring(i, 1));
                i--;
            }

            return inverse;
        }

        public static async Task Main()
        {
            var httpClient = new HttpClient();
            List<string> generatedScrambles = new List<string>();

            while (generatedScrambles.Count < 900)
            {
                System.Random rand = new System.Random();
                var threeByThree = new ThreeByThreeCubePuzzle();

                var scramble = threeByThree.GenerateWcaScramble(rand);
                var cubeState = scramble.State as CubePuzzle.CubeState;
                var facelets = cubeState.ToFaceCube();

                var response = await httpClient.GetAsync($"https://kewb.boehs.net/cube/solve/{facelets}");

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                var solution = JsonConvert.DeserializeObject<SolutionResponse>(responseString);

                var finalAlgorithm = invert_alg(solution.Solution);

                generatedScrambles.Add(finalAlgorithm);
            }

            System.IO.File.WriteAllText("scrambles.dat", string.Join("\r\n", generatedScrambles));
        }
    }
}
