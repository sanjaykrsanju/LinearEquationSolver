using System;
using System.Text;
using LinearEquationSolver.Helper;

namespace LinearEquationSolver
{
    enum LinearEquationSolverStatus
    {
        Success,
        IllConditioned,
        Singular,
    };

    /// <summary>
    /// This class solves systems of linear equations.
    /// </summary>
    class LinearEquationSolver
    {
         public static readonly double s_smallFloatingPointValue = 5.69E-14;

        public static LinearEquationSolverStatus Solve(int numberOfEquations,
                                                       Sparse2DMatrix<int, int, double> aMatrix,
                                                       SparseArray<int, double> bVector,
                                                       SparseArray<int, double> xVector)
        {
            //----------------------------------------------------------
            // Matrix a_matrix is copied into working matrix aMatrixCopy.
            //----------------------------------------------------------

            Sparse2DMatrix<int, int, double> aMatrixCopy = new Sparse2DMatrix<int, int, double>(aMatrix);

            //----------------------------------------------------------
            // The maximum value rowMaximumVector[i], i = 0 to n - 1
            // is stored
            //----------------------------------------------------------

            SparseArray<int, double> rowMaximumVector = new SparseArray<int, double>();

            int i = 0;
            for (i = 0; i < numberOfEquations; i++)
            {
                double temp = 0.0;

                for (int j = 0; j < numberOfEquations; j++)
                {
                    double test = Math.Abs(aMatrix[i, j]);

                    if (test > temp)
                    {
                        temp = test;
                    }
                }

                rowMaximumVector[i] = temp;

                //----------------------------------------------------------
                // Test for singular matrix.
                //----------------------------------------------------------

                if (temp == 0.0)
                {
                    return LinearEquationSolverStatus.Singular;
                }
            }

            //----------------------------------------------------------
            // The r'th column of "l", the r'th pivotal position r', and
            // the r'th row of "u" are determined.
            //----------------------------------------------------------

            SparseArray<int, int> pivotRowArray = new SparseArray<int, int>();

            for (int r = 0; r < numberOfEquations; r++)
            {
                double maximumValue = 0.0;
                int rowMaximumValueIndex = r;



                double temp;

                for (i = r; i < numberOfEquations; i++)
                {
                    temp = aMatrixCopy[i, r];

                    for (int j = 0; j < r; j++)
                    {
                        temp = temp - aMatrixCopy[i, j] * aMatrixCopy[j, r];
                    }

                    aMatrixCopy[i, r] = temp;

                    double test = Math.Abs(temp / rowMaximumVector[i]);

                    if (test > maximumValue)
                    {
                        maximumValue = test;
                        rowMaximumValueIndex = i;
                    }
                }

                //----------------------------------------------------------
                // Test for matrix which is singular to working precision.
                //----------------------------------------------------------

                if (maximumValue == 0.0)
                {
                    return LinearEquationSolverStatus.IllConditioned;
                }

                //----------------------------------------------------------
                // "rowMaximumValueIndex" = r' and "pivotRowArray[r]"
                // is the pivotal row.
                //----------------------------------------------------------

                rowMaximumVector[rowMaximumValueIndex] = rowMaximumVector[r];
                pivotRowArray[r] = rowMaximumValueIndex;

                //----------------------------------------------------------
                // Rows "r" and "pivotRowArray[r]" are exchanged.
                //----------------------------------------------------------

                for (i = 0; i < numberOfEquations; i++)
                {
                    temp = aMatrixCopy[r, i];
                    aMatrixCopy[r, i] = aMatrixCopy[rowMaximumValueIndex, i];
                    aMatrixCopy[rowMaximumValueIndex, i] = temp;
                }


                for (i = r + 1; i < numberOfEquations; i++)
                {
                    temp = aMatrixCopy[r, i];

                    for (int j = 0; j < r; j++)
                    {
                        temp = temp - aMatrixCopy[r, j] * aMatrixCopy[j, i];
                    }

                    aMatrixCopy[r, i] = temp / aMatrixCopy[r, r];
                }
            }



            SparseArray<int, double> residualVector = new SparseArray<int, double>();

            for (i = 0; i < numberOfEquations; i++)
            {
                xVector[i] = 0.0;
                residualVector[i] = bVector[i];
            }

            //----------------------------------------------------------
            // The iteration counter is initialized.
            //----------------------------------------------------------

            int iteration = 0;
            bool notConvergedFlag = true;

            do
            {
                //----------------------------------------------------------
                // The forward substitution is performed and the solution
                // of l y = p r is calculated where p r is the current
                // residual after interchanges.
                //----------------------------------------------------------

                for (i = 0; i < numberOfEquations; i++)
                {
                    int pivotRowIndex = pivotRowArray[i];
                    double temp = residualVector[pivotRowIndex];
                    residualVector[pivotRowIndex] = residualVector[i];

                    for (int j = 0; j < i; j++)
                    {
                        temp = temp - aMatrixCopy[i, j] * residualVector[j];
                    }

                    residualVector[i] = temp / aMatrixCopy[i, i];
                }

                //----------------------------------------------------------
                // The back substitution is performed and the solution of
                // u e = y is calculated. The current correction is stored
                // in variable residualVector.
                //----------------------------------------------------------

                for (i = numberOfEquations - 1; i >= 0; i--)
                {
                    double temp = residualVector[i];

                    for (int j = i + 1; j < numberOfEquations; j++)
                    {
                        temp = temp - aMatrixCopy[i, j] * residualVector[j];
                    }

                    residualVector[i] = temp;
                }

                //----------------------------------------------------------
                // The norm of the error in the residuals and the norm of
                // the present solution vector are calculated.
                //----------------------------------------------------------

                double normOfX = 0.0;
                double normOfError = 0.0;

                for (i = 0; i < numberOfEquations; i++)
                {
                    double test = Math.Abs(xVector[i]);

                    if (test > normOfX)
                    {
                        normOfX = test;
                    }

                    test = Math.Abs(residualVector[i]);

                    if (test > normOfError)
                    {
                        normOfError = test;
                    }
                }

                //----------------------------------------------------------
                // If iteration is zero then skip this section because
                // no correction exists on the first iteration.
                //----------------------------------------------------------

                if (iteration != 0)
                {
                    //------------------------------------------------------
                    // Test for matrix which is singular to working
                    // precision.
                    //------------------------------------------------------

                    if ((iteration == 1) && (normOfError / normOfX > 0.5))
                    {
                        return LinearEquationSolverStatus.IllConditioned;
                    }


                    notConvergedFlag = normOfError / normOfX >= s_smallFloatingPointValue;

#if DEBUGCODE
                    double normRatioForDebug = normOfError / normOfX;
#endif
                }

                //----------------------------------------------------------
                // The corrections (residuals) are added to the
                // solution vector.
                //----------------------------------------------------------

                for (i = 0; i < numberOfEquations; i++)
                {
                    xVector[i] = xVector[i] + residualVector[i];
                }

                //----------------------------------------------------------
                // The new residuals corresponding to the solution vector
                // are calculated.
                //----------------------------------------------------------

                for (i = 0; i < numberOfEquations; i++)
                {
                    double temp = bVector[i];

                    for (int j = 0; j < numberOfEquations; j++)
                    {
                        temp = temp - aMatrix[i, j] * xVector[j];
                    }

                    residualVector[i] = temp;
                }



                iteration++;
            }
            while (notConvergedFlag);

            return LinearEquationSolverStatus.Success;
        }
    }
}
