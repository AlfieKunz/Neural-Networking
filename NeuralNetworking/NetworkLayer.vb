Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices


Public Class NetworkLayer
    Public NodeCount, InputNodeCount As UInt32

    Public Weights(,) As Double
    Public Bias() As Double

    Public CostWeightGradients(,) As Double
    Public CostBiasGradients() As Double

    Private WeightVelocities(,) As Double
    Private BiasVelocities() As Double

    Public BaseDerivativeValues(,) As Double
    Public WInputValues(,) As Double
    Public ActivationValues(,) As Double

    Protected Shared e As Double = 2.71828
    Protected Shared MasterSettings As NetworkSettings


    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        NodeCount = NoOfNodes
        InputNodeCount = NoOfInputNodes
        ReDim Weights(NoOfNodes - 1, NoOfInputNodes - 1)
        ReDim Bias(NoOfNodes - 1)

        ReDim CostWeightGradients(NoOfNodes - 1, NoOfInputNodes - 1)
        ReDim CostBiasGradients(NoOfNodes - 1)

        ReDim WeightVelocities(NoOfNodes - 1, NoOfInputNodes - 1)
        ReDim BiasVelocities(NoOfNodes - 1)


        MasterSettings.ActivationFunction = Settings.ActivationFunction
        MasterSettings.CostIndex = Settings.CostIndex
        MasterSettings.Regularisation = Settings.Regularisation
        MasterSettings.BatchSize = Settings.BatchSize
        MasterSettings.Momentum = Settings.Momentum


        'Gives random values to all the weights (between [-1,1]/sqrt[InputNodeCount]).
        Static RNDGen As New Random()
        Dim x1, x2 As Double
        For n = 0 To NodeCount - 1
            For m = 0 To InputNodeCount - 1
                x1 = 1 - RNDGen.NextDouble()
                x2 = 1 - RNDGen.NextDouble()
                Weights(n, m) = Math.Sqrt(-2 * Math.Log(x1) / InputNodeCount) * Math.Cos(2 * 3.14 * x2)
            Next
        Next
    End Sub

    Public Sub CalibrateParameters(ByVal MiniBatchSize As UInt16, ByVal Regularisation As Double)
        MasterSettings.BatchSize = MiniBatchSize
        MasterSettings.Regularisation = Regularisation
    End Sub


    Public Sub CalibrateNoOfLearningThreads(ByVal LearningThreads As UInt16)
        ReDim BaseDerivativeValues(LearningThreads - 1, NodeCount - 1)
        ReDim WInputValues(LearningThreads - 1, NodeCount - 1)
        ReDim ActivationValues(LearningThreads - 1, NodeCount - 1)
    End Sub


    Public Overridable Function ActivationFunction(ByRef InputData() As Double) As Double()
        For x = 0 To InputData.Length - 1
            InputData(x) = 1 / (1 + (e ^ -x))
        Next
        Return InputData
    End Function
    Public Overridable Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Return InputData(ThreadIndex, Index) * (1 - InputData(ThreadIndex, Index))
    End Function



    Public Function CalculateOutputValues(ByRef InputValues() As Double) As Double()
        Dim OutputValues(NodeCount - 1) As Double

        For n = 0 To NodeCount - 1
            OutputValues(n) = Bias(n)
            For m = 0 To InputNodeCount - 1
                OutputValues(n) += InputValues(m) * Weights(n, m)
            Next
        Next

        OutputValues = ActivationFunction(OutputValues)
        Return OutputValues
    End Function
    Public Function CalculateOutputValues(ByRef InputValues() As Double, ByVal ThreadIndex As UInt16) As Double()
        Dim OutputValues(NodeCount - 1) As Double

        For n = 0 To NodeCount - 1
            OutputValues(n) = Bias(n)
            For m = 0 To InputNodeCount - 1
                OutputValues(n) += InputValues(m) * Weights(n, m)
            Next
            WInputValues(ThreadIndex, n) = OutputValues(n)
        Next

        OutputValues = ActivationFunction(OutputValues)
        Copy1DArrayToThread2DArray(OutputValues, ActivationValues, ThreadIndex)
        Return OutputValues
    End Function

    Public Sub Copy1DArrayToThread2DArray(ByVal SourceArray() As Double, ByVal DestinationArray(,) As Double, ByVal ThreadIndex As UInt16)
        For n = 0 To NodeCount - 1
            DestinationArray(ThreadIndex, n) = SourceArray(n)
        Next
    End Sub






    Public Sub ApplyGradients(ByVal LearnRate As Double)
        Dim Velocity As Double
        'Regularisation: to prevent over-fitting, we decay all weights by a constant factor (called weight-decay, and is equivalent to adding Gaussian
        'noise to weights or L2 regularisation).
        Dim WeightDecay As Double = 1 - (MasterSettings.Regularisation * LearnRate)

        For n = 0 To NodeCount - 1
            For m = 0 To InputNodeCount - 1

                Velocity = (WeightVelocities(n, m) * MasterSettings.Momentum) - (CostWeightGradients(n, m) * LearnRate)
                WeightVelocities(n, m) = Velocity

                Weights(n, m) = (Weights(n, m) * WeightDecay) + Velocity
                CostWeightGradients(n, m) = 0
            Next
            Velocity = (BiasVelocities(n) * MasterSettings.Momentum) - (CostBiasGradients(n) * LearnRate)
            BiasVelocities(n) = Velocity

            Bias(n) += Velocity
            CostBiasGradients(n) = 0
        Next
    End Sub





    Public Sub CalculateOutputLayerBaseDerivativeValues(ByVal ExpectedValueIndex As UInt16, ByVal ThreadIndex As UInt16)
        'dC/da * da/dx.
        Dim HelperValue As Double
        For n = 0 To NodeCount - 1
            HelperValue = If(n = ExpectedValueIndex, 1, 0)
            BaseDerivativeValues(ThreadIndex, n) = CalculateCostDerivative(ActivationValues(ThreadIndex, n), HelperValue) * ActivationDerivative(WInputValues, ThreadIndex, n)
        Next
    End Sub
    Public Function CalculatePreviousLayerBaseDerivativeValues(ByVal ExpectedValueIndex As UInt16, ByRef PreviousLayerWInputValues(,) As Double, ByVal ThreadIndex As UInt16) As Double()
        'PreviousBDV = dx2/da * da/dx1.
        Dim PreviousLayerBaseDerivativeValues(InputNodeCount - 1) As Double
        For m = 0 To InputNodeCount - 1
            For n = 0 To NodeCount - 1
                PreviousLayerBaseDerivativeValues(m) += BaseDerivativeValues(ThreadIndex, n) * Weights(n, m)
            Next
            PreviousLayerBaseDerivativeValues(m) *= ActivationDerivative(PreviousLayerWInputValues, ThreadIndex, m)
        Next
        Return PreviousLayerBaseDerivativeValues
    End Function

    Public Sub UpdateGradients(ByRef PreviousLayerActivationValues(,) As Double, ByVal ThreadIndex As UInt16)
        For n = 0 To NodeCount - 1
            For m = 0 To InputNodeCount - 1
                'CostWeightGradients(n, m) += BaseDerivativeValues(ThreadIndex, n) * PreviousLayerActivationValues(ThreadIndex, m)
                RaceConditionAdd(CostWeightGradients(n, m), BaseDerivativeValues(ThreadIndex, n) * PreviousLayerActivationValues(ThreadIndex, m))
            Next
            'CostBiasGradients(n) += BaseDerivativeValues(ThreadIndex, n)
            RaceConditionAdd(CostBiasGradients(n), BaseDerivativeValues(ThreadIndex, n))
        Next
    End Sub
    Public Sub UpdateGradients(ByRef PreviousLayerActivationValues() As Double, ByVal ThreadIndex As UInt16)
        For n = 0 To NodeCount - 1
            For m = 0 To InputNodeCount - 1
                'CostWeightGradients(n, m) += BaseDerivativeValues(ThreadIndex, n) * PreviousLayerActivationValues(m)
                RaceConditionAdd(CostWeightGradients(n, m), BaseDerivativeValues(ThreadIndex, n) * PreviousLayerActivationValues(m))
            Next
            'CostBiasGradients(n) += BaseDerivativeValues(ThreadIndex, n)
            RaceConditionAdd(CostBiasGradients(n), BaseDerivativeValues(ThreadIndex, n))
        Next
    End Sub

    Public Sub RaceConditionAdd(ByRef dir As Double, Value As Double)
        dir += Value

        'Dim CurrentVal, AttemptVal As Double
        'Do
        '    CurrentVal = dir
        '    AttemptVal = CurrentVal + Value
        'Loop While CurrentVal <> System.Threading.Interlocked.CompareExchange(dir, AttemptVal, CurrentVal)
    End Sub





    Public Function CalculateCostDerivative(ByVal PredictedValue As Double, ExpectedValue As Double) As Double
        If MasterSettings.CostIndex = 0 Then
            Return 2 * (PredictedValue - ExpectedValue)
        Else
            If PredictedValue = 0 OrElse PredictedValue = 1 Then
                Return 0
            Else
                Return (ExpectedValue - PredictedValue) / (PredictedValue * (PredictedValue - 1))
            End If
        End If
    End Function

End Class
