using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinearEquationSolver.Helper
{

    public enum LinearEquationParserStatus
    {
        Success,
        SuccessNoEquation,
        ErrorIllegalEquation,
        ErrorNoEqualSign,
        ErrorMultipleEqualSigns,
        ErrorNoTermBeforeEqualSign,
        ErrorNoTermAfterEqualSign,
        ErrorNoTermEncountered,
        ErrorNoVariableInEquation,
        ErrorMultipleDecimalPoints,
        ErrorTooManyDigits,
        ErrorMissingExponent,
        ErrorIllegalExponent,
    }

    internal enum LinearEquationParserState
    {
        ParseTerm,
        ParseOperator
    };

    /// <summary>
    /// This class provides a parser for strings that contain a system
    /// of linear equations that constructs the matrix equations needed
    /// to solve the system of equations.
    /// </summary>
    public class LinearEquationParser
    {
        private static readonly int m_maximumNumberLength = 20;

        private int m_startPosition;
        private int m_equationIndex;
        private LinearEquationParserState m_parserState;
        private bool m_negativeOperatorFlag;
        private bool m_equalSignInEquationFlag;
        private bool m_atLeastOneVariableInEquationFlag;
        private bool m_termBeforeEqualSignExistsFlag;
        private bool m_termAfterEqualSignExistsFlag;


        /// <summary>
        /// This property returns the last status value of the parser.
        /// </summary>
        /// <returns>A value of type LinearEquationParserStatus</returns>
        public LinearEquationParserStatus LastStatusValue
        {
            get;
            set;
        }

        /// <summary>
        /// This property gets the position in the input line where the last
        /// error occurred.  This should only be invoked if the Parser() method
        /// returns an error status value.
        /// </summary>
        /// <returns>The position in the input line where an error occurred</returns>
        public int ErrorPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LinearEquationParser()
        {
            Reset();
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~LinearEquationParser()
        {
        }

        public LinearEquationParserStatus Parse(string inputLine,
                                                Sparse2DMatrix<int, int, double> aMatrix,
                                                SparseArray<int, double> bVector,
                                                SparseArray<string, int> variableNameIndexMap,
                                                ref int numberOfEquations)
        {
            //------------------------------------------------------------------
            // Trim any space characters from the end of the line.
            //------------------------------------------------------------------

            inputLine.TrimEnd(null);

            //------------------------------------------------------------------
            // Assume success status.
            //------------------------------------------------------------------

            int positionIndex = 0;
            SetLastStatusValue(LinearEquationParserStatus.Success, positionIndex);

            //------------------------------------------------------------------
            // Skip whitespace characters
            //------------------------------------------------------------------

            SkipSpaces(inputLine, ref positionIndex);

    

            m_startPosition = positionIndex;

        

            bool operatorFoundLast = false;

            while (positionIndex < inputLine.Length)
            {
                if (m_parserState == LinearEquationParserState.ParseTerm)
                {
                    //------------------------------------------------------
                    // Skip whitespace characters
                    //------------------------------------------------------

                    SkipSpaces(inputLine, ref positionIndex);

                    if (positionIndex < inputLine.Length)
                    {
                        if (GetTerm(inputLine,
                                    ref positionIndex,
                                    aMatrix,
                                    bVector,
                                    variableNameIndexMap))
                        {
                            m_parserState = LinearEquationParserState.ParseOperator;
                            operatorFoundLast = false;
                        }
                        else
                        {
                            if (LastStatusValue == LinearEquationParserStatus.Success)
                            {
                                SetLastStatusValue(LinearEquationParserStatus.ErrorIllegalEquation,
                                                   positionIndex);
                            }

                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else if (m_parserState == LinearEquationParserState.ParseOperator)
                {
                    //------------------------------------------------------
                    // Skip whitespace characters
                    //------------------------------------------------------

                    SkipSpaces(inputLine, ref positionIndex);

                    if (positionIndex < inputLine.Length)
                    {
                        //------------------------------------------------------
                        // Check for plus sign, minus sign, or an equal sign.
                        //------------------------------------------------------

                        if (GetOperator(inputLine, ref positionIndex))
                        {
                            m_parserState = LinearEquationParserState.ParseTerm;
                            operatorFoundLast = true;
                        }
                        else
                        {
                            if (LastStatusValue != LinearEquationParserStatus.Success)
                            {
                                if (positionIndex == m_startPosition)
                                {
                                    SetLastStatusValue(LinearEquationParserStatus.ErrorIllegalEquation,
                                                       positionIndex);
                                }
                            }

                            break;
                        }
                    }
                }
            }

            // If an operator was found at 
            if ((positionIndex >= inputLine.Length) && (positionIndex > 0) && (!operatorFoundLast))
            {
                ResetForNewEquation();
                numberOfEquations = m_equationIndex;
            }

            return LastStatusValue;
        }

        /// <summary>
        /// This function resets the parser to its initial state for parsing
        /// a new set of simultaneous linear equations.
        /// </summary>
        void Reset()
        {
            m_startPosition = 0;
            ErrorPosition = 0;
            LastStatusValue = LinearEquationParserStatus.Success;
            m_negativeOperatorFlag = false;
            m_equalSignInEquationFlag = false;
            m_atLeastOneVariableInEquationFlag = false;
            m_termBeforeEqualSignExistsFlag = false;
            m_termAfterEqualSignExistsFlag = false;
            m_parserState = LinearEquationParserState.ParseTerm;
            m_equationIndex = 0;
        }

        /// <summary>
        /// This function calculate the status value for an incomplete equation.
        /// This should be called if the IsCompleteEquation() method returns false.
        /// </summary>
        /// <returns>An enum value of type 'LinearEquationParserStatus'</returns>
        private LinearEquationParserStatus GetEquationStatus()
        {
            LinearEquationParserStatus status = LinearEquationParserStatus.Success;

            if ((!m_equalSignInEquationFlag)
                && (!m_termBeforeEqualSignExistsFlag)
                && (!m_termAfterEqualSignExistsFlag)
                && (!m_atLeastOneVariableInEquationFlag))
            {
                status = LinearEquationParserStatus.SuccessNoEquation;
            }
            else if (!m_equalSignInEquationFlag)
            {
                status = LinearEquationParserStatus.ErrorNoEqualSign;
            }
            else if (!m_termBeforeEqualSignExistsFlag)
            {
                status = LinearEquationParserStatus.ErrorNoTermBeforeEqualSign;
            }
            else if (!m_termAfterEqualSignExistsFlag)
            {
                status = LinearEquationParserStatus.ErrorNoTermAfterEqualSign;
            }
            else if (!m_atLeastOneVariableInEquationFlag)
            {
                status = LinearEquationParserStatus.ErrorNoVariableInEquation;
            }
            else
            {
                status = LinearEquationParserStatus.Success;
            }

            return status;
        }


        /// <summary>
        /// This function resets the parser to process a new equation.
        /// </summary>
        private void ResetForNewEquation()
        {
            m_startPosition = 0;
            m_negativeOperatorFlag = false;
            m_equalSignInEquationFlag = false;
            m_atLeastOneVariableInEquationFlag = false;
            m_termBeforeEqualSignExistsFlag = false;
            m_termAfterEqualSignExistsFlag = false;
            m_parserState = LinearEquationParserState.ParseTerm;
            m_equationIndex++;
        }

        private bool GetTerm(string inputLine,
                             ref int positionIndex,
                             Sparse2DMatrix<int, int, double> aMatrix,
                             SparseArray<int, double> bVector,
                             SparseArray<string, int> variableNameIndexMap)
        {

            bool numberIsNegativeFlag = false;

            GetSign(inputLine,
                    ref positionIndex,
                    ref numberIsNegativeFlag);

            //------------------------------------------------------------------
            // Skip whitespace characters
            //------------------------------------------------------------------

            SkipSpaces(inputLine, ref positionIndex);

            //------------------------------------------------------------------
            // Check to see if this is a number or a variable.
            //------------------------------------------------------------------

            string numberString = "";

            bool haveNumberFlag = GetNumber(inputLine,
                                            ref positionIndex,
                                            ref numberString);

            //------------------------------------------------------------------
            // If an error occurred then abort.
            //------------------------------------------------------------------

            if (LastStatusValue != LinearEquationParserStatus.Success)
            {
                return false;
            }

            //------------------------------------------------------------------
            // If there was a number encountered then test to see if the
            // number has an exponent.
            //------------------------------------------------------------------

            if (haveNumberFlag)
            {
                if (positionIndex < inputLine.Length)
                {
                    //----------------------------------------------------------
                    // Does the number have an exponent?
                    //----------------------------------------------------------

                    if (inputLine[positionIndex] == '^')
                    {
                        positionIndex++;

                        //------------------------------------------------------
                        // Does the exponent have a sign.
                        //------------------------------------------------------

                        bool negativeExponentFlag = false;

                        GetSign(inputLine,
                                ref positionIndex,
                                ref negativeExponentFlag);

                        //------------------------------------------------------
                        // Get the exponent digits.
                        //------------------------------------------------------

                        string exponentString = "";

                        if (GetNumber(inputLine,
                                      ref positionIndex,
                                      ref exponentString))
                        {
                            //--------------------------------------------------
                            // Is the exponent a valid exponent.
                            //--------------------------------------------------

                            int exponentLength = exponentString.Length;

                            if (exponentLength <= 2)
                            {
                                bool exponent_error_flag = false;

                                for (int i = 0; i < exponentLength; ++i)
                                {
                                    if (!Char.IsDigit(exponentString[i]))
                                    {
                                        exponent_error_flag = true;
                                    }
                                }

                                if (!exponent_error_flag)
                                {
                                    numberString += 'E';

                                    if (negativeExponentFlag)
                                    {
                                        numberString += '-';
                                    }

                                    numberString += exponentString;
                                }
                                else
                                {
                                    SetLastStatusValue(LinearEquationParserStatus.ErrorIllegalExponent,
                                                       positionIndex);
                                    return false;
                                }
                            }
                            else
                            {
                                SetLastStatusValue(LinearEquationParserStatus.ErrorIllegalExponent,
                                                   positionIndex);
                                return false;
                            }
                        }
                        else
                        {
                            SetLastStatusValue(LinearEquationParserStatus.ErrorMissingExponent,
                                               positionIndex);
                            return false;
                        }
                    }
                }
            }

            //------------------------------------------------------------------
            // Skip whitespace characters
            //------------------------------------------------------------------

            SkipSpaces(inputLine, ref positionIndex);

            string variableName = "";

            bool haveVariableNameFlag = GetVariableName(inputLine,
                                                        ref positionIndex,
                                                        ref variableName);

            //------------------------------------------------------------------
            // Calculate the sign of the value. The sign is negated
            // if the equals sign has been encountered.
            //------------------------------------------------------------------

            bool negativeFlag =
                m_equalSignInEquationFlag ^ m_negativeOperatorFlag ^ numberIsNegativeFlag;

            double value = 0.0;

            if (haveNumberFlag)
            {
                value = Convert.ToDouble(numberString);

                if (negativeFlag)
                {
                    value = -value;
                }
            }
            else
            {
                value = 1.0;

                if (negativeFlag)
                {
                    value = -value;
                }
            }

            //------------------------------------------------------------------
            // If a variable was encountered then add to the aMatrix.
            //------------------------------------------------------------------

            bool haveTermFlag = false;

            if (haveVariableNameFlag)
            {
                //--------------------------------------------------------------
                // If either a number or a variable is found then a term was
                // found.
                //--------------------------------------------------------------

                haveTermFlag = true;

                //--------------------------------------------------------------
                // Each equation must have at least one variable.
                // Record that a variable was encountered in this equation.
                //--------------------------------------------------------------

                m_atLeastOneVariableInEquationFlag = true;

                //--------------------------------------------------------------
                // If this variable has not been encountered before then add
                // the variable to the variableNameIndexMap.
                //--------------------------------------------------------------

                int variableNameIndex = 0;

                if (!variableNameIndexMap.TryGetValue(variableName, out variableNameIndex))
                {
                    // This is a new variable. Add it to the map.
                    variableNameIndex = variableNameIndexMap.Count;
                    variableNameIndexMap[variableName] = variableNameIndex;
                }

                aMatrix[m_equationIndex, variableNameIndex] =
                    aMatrix[m_equationIndex, variableNameIndex] + value;
            }
            else if (haveNumberFlag)
            {
                //--------------------------------------------------------------
                // If either a number or a variable is found then a term was
                // found.
                //--------------------------------------------------------------

                haveTermFlag = true;

                //--------------------------------------------------------------
                // Put the value in the B vector.
                //--------------------------------------------------------------

                bVector[m_equationIndex] = bVector[m_equationIndex] - value;
            }
            else
            {
                haveTermFlag = false;
                SetLastStatusValue(LinearEquationParserStatus.ErrorNoTermEncountered,
                                   positionIndex);
                return false;
            }

            //------------------------------------------------------------------
            // There must be at least one term on each side of the equal sign.
            //------------------------------------------------------------------

            if (haveTermFlag)
            {
                if (m_equalSignInEquationFlag)
                {
                    m_termAfterEqualSignExistsFlag = true;
                }
                else
                {
                    m_termBeforeEqualSignExistsFlag = true;
                }
            }

            //------------------------------------------------------------------
            // Skip whitespace characters
            //------------------------------------------------------------------

            SkipSpaces(inputLine, ref positionIndex);

            return haveTermFlag;
        }


        private bool GetSign(string inputLine,
                             ref int positionIndex,
                             ref bool negativeFlag)
        {
            //------------------------------------------------------------------
            // Check for a plus or a minus sign.
            //------------------------------------------------------------------

            bool haveSignFlag = false;
            negativeFlag = false;

            if (positionIndex < inputLine.Length)
            {
                char c = inputLine[positionIndex];

                if (c == '+')
                {
                    haveSignFlag = true;
                    positionIndex++;
                }
                else if (c == '-')
                {
                    haveSignFlag = true;
                    negativeFlag = true;
                    positionIndex++;
                }
            }

            return haveSignFlag;
        }


        private bool GetNumber(string inputLine,
                               ref int positionIndex,
                               ref string numberString)
        {
            int decimalPointCount = 0;
            int digitLength = 0;
            bool haveNumberFlag = false;
            bool continueFlag = positionIndex < inputLine.Length;

            while (continueFlag)
            {
                Char c = inputLine[positionIndex];

                continueFlag = (c == '.');

                if (Char.IsDigit(c))
                {
                    if (++digitLength > m_maximumNumberLength)
                    {
                        SetLastStatusValue(LinearEquationParserStatus.ErrorTooManyDigits,
                                           positionIndex);
                        return false;
                    }

                    haveNumberFlag = true;
                    numberString += c;
                    positionIndex++;
                    continueFlag = positionIndex < inputLine.Length;
                }
                else
                {
                    continueFlag = c == '.';

                    if (continueFlag)
                    {
                        if (++decimalPointCount > 1)
                        {
                            SetLastStatusValue(LinearEquationParserStatus.ErrorMultipleDecimalPoints,
                                               positionIndex);
                            return false;
                        }

                        numberString += c;
                        positionIndex++;
                        continueFlag = positionIndex < inputLine.Length;
                    }
                }
            }

            if (numberString.Length > m_maximumNumberLength)
            {
                SetLastStatusValue(LinearEquationParserStatus.ErrorTooManyDigits,
                                   positionIndex);
                return false;
            }

            return haveNumberFlag;
        }

        private bool GetVariableName(string inputLine,
                                     ref int positionIndex,
                                     ref string variableName)
        {
            bool haveVariableNameFlag = false;
            bool continueFlag = positionIndex < inputLine.Length;

            while (continueFlag)
            {
                Char c = inputLine[positionIndex];

                continueFlag = (Char.IsLetter(c) || c == '_');

                if (continueFlag)
                {
                    haveVariableNameFlag = true;
                    variableName += c;
                    positionIndex++;
                    continueFlag = positionIndex < inputLine.Length;
                }
            }

            return haveVariableNameFlag;
        }


        private bool GetOperator(string inputLine, ref int positionIndex)
        {
            //------------------------------------------------------------------
            // Skip whitespace characters
            //------------------------------------------------------------------

            SkipSpaces(inputLine, ref positionIndex);

            //------------------------------------------------------------------
            // Check for an equals sign.
            //------------------------------------------------------------------

            m_negativeOperatorFlag = false;
            bool haveEqualSignFlag = false;

            if (positionIndex < inputLine.Length)
            {
                if (inputLine[positionIndex] == '=')
                {
                    if (!m_equalSignInEquationFlag)
                    {
                        m_equalSignInEquationFlag = true;
                        haveEqualSignFlag = true;
                        positionIndex++;
                    }
                    else
                    {
                        SetLastStatusValue(LinearEquationParserStatus.ErrorMultipleEqualSigns,
                                           positionIndex);
                        return false;
                    }
                }
            }

            bool haveSignFlag = GetSign(inputLine,
                                        ref positionIndex,
                                        ref m_negativeOperatorFlag);

            return haveSignFlag || haveEqualSignFlag;
        }

        private void SkipSpaces(string inputLine, ref int positionIndex)
        {
            bool continueFlag = positionIndex < inputLine.Length;

            while (continueFlag)
            {
                char c = inputLine[positionIndex];

                continueFlag = Char.IsWhiteSpace(c);

                if (continueFlag)
                {
                    positionIndex++;
                    continueFlag = positionIndex < inputLine.Length;
                }
            }
        }

        private void SetLastStatusValue(LinearEquationParserStatus status,
                                        int positionIndex)
        {
            LastStatusValue = status;
            ErrorPosition = positionIndex;
        }
    }
}
