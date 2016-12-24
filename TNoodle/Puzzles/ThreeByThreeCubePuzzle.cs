﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNoodle.Core;
using TNoodle.Solvers;
using TNoodle.min2phase;
using System.Threading;

namespace TNoodle.Puzzles
{
    public class ThreeByThreeCubePuzzle : CubePuzzle
    {
        //private static final Logger l = Logger.getLogger(ThreeByThreeCubePuzzle.class.getName());
        private const int THREE_BY_THREE_MAX_SCRAMBLE_LENGTH = 21;
        private const int THREE_BY_THREE_TIMEMIN = 200; //milliseconds
        private const int THREE_BY_THREE_TIMEOUT = 60 * 1000; //milliseconds

        private ThreadLocal<Search> twoPhaseSearcher = null;
        public ThreeByThreeCubePuzzle() : base(3)
        {
            //string newMinDistance = EnvGetter.getenv("TNOODLE_333_MIN_DISTANCE");
            //if (newMinDistance != null)
            //{
            //wcaMinScrambleDistance = Integer.parseInt(newMinDistance);
            //}
            twoPhaseSearcher = new ThreadLocal<Search>(() => new Search());
            //{
            //protected Search initialValue()
            // {/
            // return new Search();
            //};
            //};
        }

        protected internal override string solveIn(PuzzleState ps, int n)
        {
            return solveIn(ps, n, null, null);
        }

        public string solveIn(PuzzleState ps, int n, string firstAxisRestriction, string lastAxisRestriction)
        {
            CubeState cs = (CubeState)ps;
            if (this.Equals(getSolvedState()))
            {
                // TODO - apparently min2phase can't solve the solved cube
                return "";
            }
            string solution = twoPhaseSearcher.Value.solution(cs.toFaceCube(), n, THREE_BY_THREE_TIMEOUT, 0, 0, firstAxisRestriction, lastAxisRestriction).Trim();
            if ("Error 7".Equals(solution))
            {
                // No solution exists for given depth
                return null;
            }
            else if (solution.StartsWith("Error"))
            {
                // TODO - Not really sure what to do here.
                //l.severe(solution + " while searching for solution to " + cs.toFaceCube());
                //azzert(false);
                return null;
            }
            return solution;
        }

        public PuzzleStateAndGenerator generateRandomMoves(Random r, string firstAxisRestriction, string lastAxisRestriction)
        {
            string randomState = Tools.randomCube(r);
            string scramble = twoPhaseSearcher.Value.solution(randomState, THREE_BY_THREE_MAX_SCRAMBLE_LENGTH, THREE_BY_THREE_TIMEOUT, THREE_BY_THREE_TIMEMIN, Search.INVERSE_SOLUTION, firstAxisRestriction, lastAxisRestriction).Trim();

            AlgorithmBuilder ab = new AlgorithmBuilder(this, AlgorithmBuilder.MergingMode.CANONICALIZE_MOVES);
            //try
            {
                ab.appendAlgorithm(scramble);
            }
            //catch //(InvalidMoveException e)
            {
                //azzert(false, new InvalidScrambleException(scramble, e));
            }
            return ab.getStateAndGenerator();
        }
        public override PuzzleStateAndGenerator generateRandomMoves(Random r)
        {
            return generateRandomMoves(r, null, null);
        }
    }
}
