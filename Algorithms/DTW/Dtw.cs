//(The MIT License)

//Copyright (c) 2012 Darjan Oblak

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the 'Software'), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Algorithms.DTW
{
    public class Dtw
    {
        #region Dtw Parameters 
        /// <summary>
        /// Gestures for comparing
        /// </summary>
        private readonly Gesture x;
        private readonly Gesture y;

        /// <summary>
        /// If Gesture X has more points than Y.
        /// </summary>
        private readonly bool isXLongerOrEqualThanY;

        /// <summary>
        /// Number of points of difference.
        /// </summary>
        private readonly int signalsLengthDifference;

        /// <summary>
        /// Distance type
        ///  |- Normal = 1
        ///  |- Selective = 2
        ///  |- Threshold = 3
        ///  |- SelectiveWithThreshold = 4
        /// </summary>
        private readonly Distances distance;

        /// <summary>
        /// Apply boundary constraint at (1, 1).
        /// </summary>
        private readonly bool boundaryConstraintStart;

        /// <summary>
        /// Apply boundary constraint at (m, n).
        /// </summary>
        private readonly bool boundaryConstraintEnd;

        /// <summary>
        /// If Sakoe-Chiba max shift constraint (side steps) is enabled.
        /// </summary>
        private readonly bool sakoeChibaConstraint;

        /// <summary>
        /// Sakoe-Chiba max shift constraint (side steps).
        /// </summary>
        private readonly int sakoeChibaMaxShift;

        /// <summary>
        /// 
        /// </summary>
        private bool calculated;

        /// <summary>
        /// 
        /// </summary>
        private double[][] distances;

        /// <summary>
        /// I
        /// </summary>
        private double[][] pathCost;

        /// <summary>
        /// If we want to use slope constraint in dtw algorithm
        /// </summary>
        private readonly bool useSlopeConstraint;

        /// <summary>
        /// 
        /// </summary>
        private readonly int slopeMatrixLookbehind = 1;

        /// <summary>
        /// Diagonal steps in local window for calculation. Results in Ikatura paralelogram shaped dtw-candidate space. Use in combination with slopeStepSizeAside parameter. 
        /// </summary>
        private readonly int slopeStepSizeDiagonal;

        /// <summary>
        /// Side steps in local window for calculation. Results in Ikatura paralelogram shaped dtw-candidate space. Use in combination with slopeStepSizeDiagonal parameter. 
        /// </summary>
        private readonly int slopeStepSizeAside;

        /// <summary>
        /// [indexX][indexY][step]
        /// </summary>
        private int[][][] _predecessorStepX;
        private int[][][] _predecessorStepY;
        #endregion


        /// <summary>
        /// Initialize class that performs single variable DTW calculation for given series and settings.
        /// </summary>
        /// <param name="x">Gesture A.</param>
        /// <param name="y">Gesture B.</param>
        /// /// <param name="distanceMeasure">Distance measure used (how distance between skeletons is calculated).</param>
        /// <param name="boundaryConstraintStart">Apply boundary constraint at (1, 1).</param>
        /// <param name="boundaryConstraintEnd">Apply boundary constraint at (m, n).</param>
        /// <param name="slopeStepSizeDiagonal">Diagonal steps in local window for calculation. Results in Itakura paralelogram shaped dtw-candidate space. Use in combination with slopeStepSizeAside parameter. Leave null for no constraint.</param>
        /// <param name="slopeStepSizeAside">Side steps in local window for calculation. Results in Ikatura paralelogram shaped dtw-candidate space. Use in combination with slopeStepSizeDiagonal parameter. Leave null for no constraint.</param>
        /// <param name="sakoeChibaMaxShift">Sakoe-Chiba max shift constraint (side steps). Leave null for no constraint.</param>
        public Dtw(Gesture _x, Gesture _y, DistanceType _distanceType, bool _boundaryConstraintStart = true, bool _boundaryConstraintEnd = true, int? _slopeStepSizeDiagonal = null, int? _slopeStepSizeAside = null, int? _sakoeChibaMaxShift = null)
        {
            x = _x;
            y = _y;
            distance = new Distances(_distanceType);
            boundaryConstraintStart = _boundaryConstraintStart;
            boundaryConstraintEnd = _boundaryConstraintEnd;

            if (x.Length == 0 || y.Length == 0)
                throw new ArgumentException("Both series should have at least one value.");

            if (_sakoeChibaMaxShift != null && _sakoeChibaMaxShift < 0)
                throw new ArgumentException("Sakoe-Chiba max shift value should be positive or null.");

            isXLongerOrEqualThanY = x.Length >= y.Length;
            signalsLengthDifference = Math.Abs(x.Length - y.Length);
            sakoeChibaConstraint = _sakoeChibaMaxShift.HasValue;
            sakoeChibaMaxShift = _sakoeChibaMaxShift.HasValue ? _sakoeChibaMaxShift.Value : int.MaxValue;

            if (_slopeStepSizeAside != null || _slopeStepSizeDiagonal != null)
            {
                if (_slopeStepSizeAside == null || _slopeStepSizeDiagonal == null)
                    throw new ArgumentException("Both values or none for slope constraint must be specified.");

                if (_slopeStepSizeDiagonal < 1)
                    throw new ArgumentException("Diagonal slope constraint parameter must be greater than 0.");

                if (_slopeStepSizeAside < 0)
                    throw new ArgumentException("Diagonal slope constraint parameter must be greater or equal to 0.");

                useSlopeConstraint = true;
                slopeStepSizeAside = _slopeStepSizeAside.Value;
                slopeStepSizeDiagonal = _slopeStepSizeDiagonal.Value;

                slopeMatrixLookbehind = _slopeStepSizeDiagonal.Value + _slopeStepSizeAside.Value;
            }

            //todo: throw error when solution (path from (1, 1) to (m, n) is not even possible due to slope constraints)
        }

        /// <summary>
        /// Initialize class that performs single variable DTW calculation for given series and settings.
        /// </summary>
        /// <param name="x">Gesture A.</param>
        /// <param name="y">Gesture B.</param>
        /// <param name="y">Settings</param>
        public Dtw(Gesture _x, Gesture _y, ConfigurationDTW dtwConf) : this(_x, _y, dtwConf.distance, dtwConf.boundaryConstraintStart, dtwConf.boundaryConstraintEnd, dtwConf.slopeStepSizeDiagonal, dtwConf.slopeStepSizeAside, dtwConf.sakoeChibaMaxShift) { }


        /// <summary>
        /// Initialize arrays
        /// </summary>
        private void InitializeArrays()
        {
            distances = new double[x.Length + slopeMatrixLookbehind][];
            for (int i = 0; i < x.Length + slopeMatrixLookbehind; i++)
                distances[i] = new double[y.Length + slopeMatrixLookbehind];

            pathCost = new double[x.Length + slopeMatrixLookbehind][];
            for (int i = 0; i < x.Length + slopeMatrixLookbehind; i++)
                pathCost[i] = new double[y.Length + slopeMatrixLookbehind];

            _predecessorStepX = new int[x.Length + slopeMatrixLookbehind][][];
            for (int i = 0; i < x.Length + slopeMatrixLookbehind; i++)
                _predecessorStepX[i] = new int[y.Length + slopeMatrixLookbehind][];

            _predecessorStepY = new int[x.Length + slopeMatrixLookbehind][][];
            for (int i = 0; i < x.Length + slopeMatrixLookbehind; i++)
                _predecessorStepY[i] = new int[y.Length + slopeMatrixLookbehind][];
        }

        private void CalculateDistances()
        {
            for (int additionalIndex = 1; additionalIndex <= slopeMatrixLookbehind; additionalIndex++)
            {
                //initialize [x.len - 1 + additionalIndex][all] elements
                for (int i = 0; i < y.Length + slopeMatrixLookbehind; i++)
                    pathCost[x.Length - 1 + additionalIndex][i] = double.PositiveInfinity;

                //initialize [all][y.len - 1 + additionalIndex] elements
                for (int i = 0; i < x.Length + slopeMatrixLookbehind; i++)
                    pathCost[i][y.Length - 1 + additionalIndex] = double.PositiveInfinity;
            }

            double[][] dists = x.Skeletons.Select(skx => y.Skeletons.Select(sky => distance.Distance(skx, sky)).ToArray()).ToArray();

            for (int i = 0; i < dists.Length; i++)
                for (int j = 0; j < dists[i].Length; j++)
                    distances[i][j] = dists[i][j];
        }

        private void CalculateWithoutSlopeConstraint()
        {
            var stepMove0 = new[] { 0 };
            var stepMove1 = new[] { 1 };

            for (int i = x.Length - 1; i >= 0; i--)
            {
                var currentRowDistances = distances[i];
                var currentRowPathCost = pathCost[i];
                var previousRowPathCost = pathCost[i + 1];

                var currentRowPredecessorStepX = _predecessorStepX[i];
                var currentRowPredecessorStepY = _predecessorStepY[i];

                for (int j = y.Length - 1; j >= 0; j--)
                {
                    //Sakoe-Chiba constraint, but make it wider in one dimension when signal lengths are not equal
                    if (sakoeChibaConstraint &&
                        (isXLongerOrEqualThanY
                       ? j > i && j - i > sakoeChibaMaxShift || j < i && i - j > sakoeChibaMaxShift + signalsLengthDifference
                       : j > i && j - i > sakoeChibaMaxShift + signalsLengthDifference || j < i && i - j > sakoeChibaMaxShift))
                    {
                        currentRowPathCost[j] = double.PositiveInfinity;
                        continue;
                    }

                    var diagonalNeighbourCost = previousRowPathCost[j + 1];
                    var xNeighbourCost = previousRowPathCost[j];
                    var yNeighbourCost = currentRowPathCost[j + 1];

                    //on the topright edge, when boundary constrained only assign current distance as path distance to the (m, n) element
                    //on the topright edge, when not boundary constrained, assign current distance as path distance to all edge elements
                    if (double.IsInfinity(diagonalNeighbourCost) && (!boundaryConstraintEnd || i - j == x.Length - y.Length))
                        currentRowPathCost[j] = currentRowDistances[j];
                    else if (diagonalNeighbourCost <= xNeighbourCost && diagonalNeighbourCost <= yNeighbourCost)
                    {
                        currentRowPathCost[j] = diagonalNeighbourCost + currentRowDistances[j];
                        currentRowPredecessorStepX[j] = stepMove1;
                        currentRowPredecessorStepY[j] = stepMove1;
                    }
                    else if (xNeighbourCost <= yNeighbourCost)
                    {
                        currentRowPathCost[j] = xNeighbourCost + currentRowDistances[j];
                        currentRowPredecessorStepX[j] = stepMove1;
                        currentRowPredecessorStepY[j] = stepMove0;
                    }
                    else
                    {
                        currentRowPathCost[j] = yNeighbourCost + currentRowDistances[j];
                        currentRowPredecessorStepX[j] = stepMove0;
                        currentRowPredecessorStepY[j] = stepMove1;
                    }
                }
            }
        }

        private void CalculateWithSlopeLimit()
        {
            //precreated array that contain arrays of steps which are used when stepaside path is the optimal one
            //stepAsideMoves*[0] is empty element, because contents are 1-based, access to elements is faster that way
            var stepAsideMovesHorizontalX = new int[slopeStepSizeAside + 1][];
            var stepAsideMovesHorizontalY = new int[slopeStepSizeAside + 1][];
            var stepAsideMovesVerticalX = new int[slopeStepSizeAside + 1][];
            var stepAsideMovesVerticalY = new int[slopeStepSizeAside + 1][];
            for (int i = 1; i <= slopeStepSizeAside; i++)
            {
                var movesXHorizontal = new List<int>();
                var movesYHorizontal = new List<int>();
                var movesXVertical = new List<int>();
                var movesYVertical = new List<int>();

                //make steps in horizontal/vertical direction
                for (int stepAside = 1; stepAside <= i; stepAside++)
                {
                    movesXHorizontal.Add(1);
                    movesYHorizontal.Add(0);

                    movesXVertical.Add(0);
                    movesYVertical.Add(1);
                }

                //make steps in diagonal direction
                for (int stepForward = 1; stepForward <= slopeStepSizeDiagonal; stepForward++)
                {
                    movesXHorizontal.Add(1);
                    movesYHorizontal.Add(1);

                    movesXVertical.Add(1);
                    movesYVertical.Add(1);
                }

                stepAsideMovesHorizontalX[i] = movesXHorizontal.ToArray();
                stepAsideMovesHorizontalY[i] = movesYHorizontal.ToArray();

                stepAsideMovesVerticalX[i] = movesXVertical.ToArray();
                stepAsideMovesVerticalY[i] = movesYVertical.ToArray();
            }

            var stepMove1 = new[] { 1 };

            for (int i = x.Length - 1; i >= 0; i--)
            {
                var currentRowDistances = distances[i];

                var currentRowPathCost = pathCost[i];
                var previousRowPathCost = pathCost[i + 1];

                var currentRowPredecessorStepX = _predecessorStepX[i];
                var currentRowPredecessorStepY = _predecessorStepY[i];

                for (int j = y.Length - 1; j >= 0; j--)
                {
                    //Sakoe-Chiba constraint, but make it wider in one dimension when signal lengths are not equal
                    if (sakoeChibaConstraint &&
                        (isXLongerOrEqualThanY
                        ? j > i && j - i > sakoeChibaMaxShift || j < i && i - j > sakoeChibaMaxShift + signalsLengthDifference
                        : j > i && j - i > sakoeChibaMaxShift + signalsLengthDifference || j < i && i - j > sakoeChibaMaxShift))
                    {
                        currentRowPathCost[j] = double.PositiveInfinity;
                        continue;
                    }

                    //just initialize lowest cost with diagonal neighbour element
                    var lowestCost = previousRowPathCost[j + 1];
                    var lowestCostStepX = stepMove1;
                    var lowestCostStepY = stepMove1;

                    for (int alternativePathAside = 1; alternativePathAside <= slopeStepSizeAside; alternativePathAside++)
                    {
                        var costHorizontalStepAside = 0.0;
                        var costVerticalStepAside = 0.0;

                        for (int stepAside = 1; stepAside <= alternativePathAside; stepAside++)
                        {
                            costHorizontalStepAside += distances[i + stepAside][j];
                            costVerticalStepAside += distances[i][j + stepAside];
                        }

                        for (int stepForward = 1; stepForward < slopeStepSizeDiagonal; stepForward++)
                        {
                            costHorizontalStepAside += distances[i + alternativePathAside + stepForward][j + stepForward];
                            costVerticalStepAside += distances[i + stepForward][j + alternativePathAside + stepForward];
                        }

                        //at final step, add comulative cost
                        costHorizontalStepAside += pathCost[i + alternativePathAside + slopeStepSizeDiagonal][j + slopeStepSizeDiagonal];

                        //at final step, add comulative cost
                        costVerticalStepAside += pathCost[i + slopeStepSizeDiagonal][j + alternativePathAside + slopeStepSizeDiagonal];

                        //check if currently considered horizontal stepaside is better than the best option found until now
                        if (costHorizontalStepAside < lowestCost)
                        {
                            lowestCost = costHorizontalStepAside;
                            lowestCostStepX = stepAsideMovesHorizontalX[alternativePathAside];
                            lowestCostStepY = stepAsideMovesHorizontalY[alternativePathAside];
                        }

                        //check if currently considered vertical stepaside is better than the best option found until now
                        if (costVerticalStepAside < lowestCost)
                        {
                            lowestCost = costVerticalStepAside;
                            lowestCostStepX = stepAsideMovesVerticalX[alternativePathAside];
                            lowestCostStepY = stepAsideMovesVerticalY[alternativePathAside];
                        }
                    }

                    //on the topright edge, when boundary constrained only assign current distance as path distance to the (m, n) element
                    //on the topright edge, when not boundary constrained, assign current distance as path distance to all edge elements
                    if (double.IsInfinity(lowestCost) && (!boundaryConstraintEnd || i - j == x.Length - y.Length))
                        lowestCost = 0;

                    currentRowPathCost[j] = lowestCost + currentRowDistances[j];
                    currentRowPredecessorStepX[j] = lowestCostStepX;
                    currentRowPredecessorStepY[j] = lowestCostStepY;
                }
            }
        }

        private void Calculate()
        {
            if (!calculated)
            {
                InitializeArrays();
                CalculateDistances();

                if (useSlopeConstraint)
                    CalculateWithSlopeLimit();
                else
                    CalculateWithoutSlopeConstraint();

                calculated = true;
            }
        }

        public double GetCost()
        {
            double cost;
            Calculate();

            if (boundaryConstraintStart)
                cost = pathCost[0][0];
            else
                cost = Math.Min(pathCost[0].Min(), pathCost.Select(y => y[0]).Min());
            
            cost = double.IsInfinity(cost) ? double.MaxValue : cost;

            return cost;
        }

        public Tuple<int, int>[] GetPath()
        {
            Calculate();

            var path = new List<Tuple<int, int>>();
            var indexX = 0;
            var indexY = 0;
            if (!boundaryConstraintStart)
            {
                //find the starting element with lowest cost
                var min = double.PositiveInfinity;
                for (int i = 0; i < Math.Max(x.Length, y.Length); i++)
                {
                    if (i < x.Length && pathCost[i][0] < min)
                    {
                        indexX = i;
                        indexY = 0;
                        min = pathCost[i][0];
                    }
                    if (i < y.Length && pathCost[0][i] < min)
                    {
                        indexX = 0;
                        indexY = i;
                        min = pathCost[0][i];
                    }
                }
            }

            path.Add(new Tuple<int, int>(indexX, indexY));
            while (
                boundaryConstraintEnd
                ? (indexX < x.Length - 1 || indexY < y.Length - 1)
                : (indexX < x.Length - 1 && indexY < y.Length - 1))
            {
                var stepX = _predecessorStepX[indexX][indexY];
                var stepY = _predecessorStepY[indexX][indexY];

                for (int i = 0; i < stepX.Length; i++)
                {
                    indexX += stepX[i];
                    indexY += stepY[i];
                    path.Add(new Tuple<int, int>(indexX, indexY));
                }
            }
            return path.ToArray();
        }

        public double[][] GetDistanceMatrix()
        {
            Calculate();
            return distances;
        }

        public double[][] GetCostMatrix()
        {
            Calculate();
            return pathCost;
        }

    }
}

