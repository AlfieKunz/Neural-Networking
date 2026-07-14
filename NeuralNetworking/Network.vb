Public Structure NetworkSettings
    Dim ActivationFunction As String
    Dim CostIndex As SByte '0 = Mean Squared, 1 = Cross Entropy.
    Dim Regularisation As Double
    Dim BatchSize As UInt32
    Dim TrainingSize As UInt32
    Dim Momentum As Double
End Structure


Public Class NeuralNetwork
    Private NoOfInputs As UInt16
    Private Layers() As NetworkLayer
    Private MasterSettings As New NetworkSettings

    Private CostFunctionIndex As SByte


    Public Sub New(ByVal NetworkConfiguration() As UInt16, ByVal ActivationFunction As String, ByVal OutputFunction As String, ByVal CostFunction As String, ByVal Regularisation As Double, ByVal MiniBatchSize As UInt16)
        NoOfInputs = NetworkConfiguration(0)
        ReDim Layers(NetworkConfiguration.Length - 2)

        Select Case CostFunction
            Case "MeanSquared"
                CostFunctionIndex = 0
            Case "CrossEntropy"
                CostFunctionIndex = 1
            Case Else
                Console.WriteLine("Invalid Cost Function. Reverting to Mean Squared...")
                CostFunctionIndex = 0
        End Select


        MasterSettings.CostIndex = CostFunctionIndex
        MasterSettings.BatchSize = MiniBatchSize
        MasterSettings.Momentum = 0.9
        MasterSettings.Regularisation = Regularisation


        For n = 1 To NetworkConfiguration.Length - 1
            'Output layer of the network - activation function -> OutputFunction
            If n = NetworkConfiguration.Length - 1 Then ActivationFunction = OutputFunction

            MasterSettings.ActivationFunction = ActivationFunction
            Select Case ActivationFunction
                Case "Sigmoid"
                    Layers(n - 1) = New NetworkLayerSigmoid(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
                Case "Perceptron"
                    Layers(n - 1) = New NetworkLayerPerceptron(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
                Case "Tanh"
                    Layers(n - 1) = New NetworkLayerTanh(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
                Case "ReLU"
                    Layers(n - 1) = New NetworkLayerReLU(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
                Case "SiLU"
                    Layers(n - 1) = New NetworkLayerSiLU(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
                Case "Softmax"
                    Layers(n - 1) = New NetworkLayerSoftmax(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
                Case Else
                    Console.WriteLine("Invalid Activation Function. Reverting to Sigmoid...")
                    Layers(n - 1) = New NetworkLayerSigmoid(NetworkConfiguration(n), NetworkConfiguration(n - 1), MasterSettings)
            End Select
        Next
    End Sub

    Public Sub CalibrateParameters(ByVal MiniBatchSize As UInt16, ByVal Regularisation As Double)
        MasterSettings.BatchSize = MiniBatchSize
        MasterSettings.Regularisation = Regularisation
        For Each Layer In Layers
            Layer.CalibrateParameters(MiniBatchSize, Regularisation)
        Next
    End Sub

    Public Function RunDataThroughNetwork(ByVal InputData() As Double) As Double()
        For n = 0 To Layers.Length - 1
            InputData = Layers(n).CalculateOutputValues(InputData)
        Next
        Return InputData
    End Function
    Public Function RunDataThroughNetwork(ByVal InputData() As Double, ByVal ThreadIndex As UInt16) As Double()
        For n = 0 To Layers.Length - 1
            InputData = Layers(n).CalculateOutputValues(InputData, ThreadIndex)
        Next
        Return InputData
    End Function

    Public Function CalculatePrediction(ByVal InputData() As Double) As UInt16
        Dim OutputData() As Double = RunDataThroughNetwork(InputData)
        Dim MaxIndex As UInt16
        For n = 1 To OutputData.Length - 1
            If OutputData(n) > OutputData(MaxIndex) Then MaxIndex = n
        Next
        Return MaxIndex
    End Function



    Public Function CalculateCost(ByRef TestPoints() As DataPoint) As Double
        Return CalculateCost(TestPoints, 0, TestPoints.Length - 1)
    End Function
    Public Function CalculateCost(ByRef TestPoints() As DataPoint, ByVal StartIndex As UInt32, ByVal EndIndex As UInt32) As Double
        Dim TestOutputData() As Double
        Dim TestPointHelper As Double
        Dim DataPointCost As Double
        For Index = StartIndex To EndIndex

            TestOutputData = RunDataThroughNetwork(TestPoints(Index).InputSpace)
            For n = 0 To TestOutputData.Length - 1
                TestPointHelper = If(n = TestPoints(Index).ExpectedValueIndex, 1, 0)
                If CostFunctionIndex = 0 Then
                    DataPointCost = TestOutputData(n) - TestPointHelper
                    CalculateCost += DataPointCost * DataPointCost
                Else
                    DataPointCost = If(TestPointHelper = 1, -Math.Log(TestOutputData(n)), -Math.Log(1 - TestOutputData(n)))
                    If Not Double.IsNaN(DataPointCost) Then CalculateCost += DataPointCost
                End If
            Next

        Next

        ''Total all the squares of the weighs (alternative of weight decay: L2 Regularisation).
        'DataPointCost = 0
        'For Each Layer In Layers
        '    For a = 0 To Layer.NodeCount - 1
        '        For b = 0 To Layer.InputNodeCount - 1
        '            TestPointHelper = Layer.Weights(a, b)
        '            DataPointCost += TestPointHelper * TestPointHelper
        '        Next
        '    Next
        'Next
        'CalculateCost += DataPointCost * MasterSettings.Regularisation 'The derivative of 1/2 * lambda * w^2 is w * lambda.

        Return CalculateCost / (2 * (EndIndex - StartIndex + 1))
    End Function

    Public Function CalculateNoOfPointsCorrect(ByVal TestPoints() As DataPoint) As UInt32
        CalculateNoOfPointsCorrect = 0
        For Each Point In TestPoints
            If CalculatePrediction(Point.InputSpace) = Point.ExpectedValueIndex Then CalculateNoOfPointsCorrect += 1
        Next
    End Function




    Private PerformanceThreadCorrectValues() As Double
    Private PerformanceThreadCostValues() As Double
    Public Function CalculateDatasetPerformance(ByVal TrainingDatabase() As DataPoint, ByVal TestDatabase() As DataPoint) As Double()
        Dim Performance(2) As Double '0 = Training %, 1 = Test %, 2 = Training Cost %.
        Dim DatabaseScalar As UInt16 = 1000
        Dim TrainingDatabaseStepSize As Byte = 5
        Dim TestDatabaseStepSize As Byte = 2

        Dim StepSize As UInt32 = TrainingDatabase.Length \ DatabaseScalar
        ReDim PerformanceThreadCorrectValues(StepSize)
        ReDim PerformanceThreadCostValues(StepSize)
        System.Threading.Tasks.Parallel.For(0, StepSize, Sub(n)
                                                             CalculateDatasetPerformanceThread(n, TrainingDatabase, n * DatabaseScalar, Math.Min(n * DatabaseScalar + (DatabaseScalar - 1), TrainingDatabase.Length - 1), TrainingDatabaseStepSize, True)
                                                         End Sub)
        Performance(0) = 100 * PerformanceThreadCorrectValues.Sum() / (TrainingDatabase.Length \ TrainingDatabaseStepSize)
        Performance(2) = PerformanceThreadCostValues.Sum() / (2 * TrainingDatabase.Length \ TrainingDatabaseStepSize)

        StepSize = TestDatabase.Length \ DatabaseScalar
        ReDim PerformanceThreadCorrectValues(StepSize)
        System.Threading.Tasks.Parallel.For(0, StepSize, Sub(n)
                                                             CalculateDatasetPerformanceThread(n, TestDatabase, n * DatabaseScalar, Math.Min(n * DatabaseScalar + (DatabaseScalar - 1), TestDatabase.Length - 1), TestDatabaseStepSize, False)
                                                         End Sub)
        Performance(1) = 100 * PerformanceThreadCorrectValues.Sum / (TestDatabase.Length \ TestDatabaseStepSize)

        Return Performance
    End Function
    Private Sub CalculateDatasetPerformanceThread(ByVal ThreadIndex As UInt32, ByRef Database() As DataPoint, ByVal StartIndex As UInt32, EndIndex As UInt32, ByVal StepSize As Byte, ByVal CalculateCost As Boolean)
        Dim TestOutputData() As Double

        Dim TestPointHelper As Double
        Dim DataPointCost, TotalCost As Double
        Dim MaxIndex, TotalGuessesCorrect As UInt32

        For Index = StartIndex To EndIndex Step StepSize
            TestOutputData = RunDataThroughNetwork(Database(Index).InputSpace)

            MaxIndex = 0
            For n = 0 To TestOutputData.Length - 1

                If CalculateCost Then
                    TestPointHelper = If(n = Database(Index).ExpectedValueIndex, 1, 0)
                    If CostFunctionIndex = 0 Then
                        DataPointCost = TestOutputData(n) - TestPointHelper
                        TotalCost += DataPointCost * DataPointCost
                    Else
                        DataPointCost = If(TestPointHelper = 1, -Math.Log(TestOutputData(n)), -Math.Log(1 - TestOutputData(n)))
                        If Not Double.IsNaN(DataPointCost) Then TotalCost += DataPointCost
                    End If
                End If

                If TestOutputData(n) > TestOutputData(MaxIndex) Then MaxIndex = n
            Next

            If MaxIndex = Database(Index).ExpectedValueIndex Then TotalGuessesCorrect += 1
        Next

        PerformanceThreadCostValues(ThreadIndex) = TotalCost
        PerformanceThreadCorrectValues(ThreadIndex) = TotalGuessesCorrect
    End Sub

    Public Function CalculateIndividualObjectPerformance(ByVal Database() As DataPoint) As (Stats As Decimal(), Occurances As Integer())
        Dim TestOutputData() As Double
        Dim TotalDiagnostics(Layers(Layers.Length - 1).NodeCount - 1) As Decimal
        Dim TotalOccurances(Layers(Layers.Length - 1).NodeCount - 1) As Integer
        Dim MaxIndex As UInt16
        For Index = 0 To Database.Length - 1
            TestOutputData = RunDataThroughNetwork(Database(Index).InputSpace)

            MaxIndex = 0
            TotalOccurances(Database(Index).ExpectedValueIndex) += 1
            For n = 0 To TestOutputData.Length - 1
                If TestOutputData(n) > TestOutputData(MaxIndex) Then MaxIndex = n
            Next
            If MaxIndex = Database(Index).ExpectedValueIndex Then
                TotalDiagnostics(Database(Index).ExpectedValueIndex) += 1
            End If
        Next

        Return (TotalDiagnostics, TotalOccurances)
    End Function




    Public Sub CalibrateNoOfLearningThreads(ByVal LearningThreads As UInt16)
        For Each Layer In Layers
            Layer.CalibrateNoOfLearningThreads(LearningThreads)
        Next
    End Sub


    Public Sub Learn(ByRef TrainingPoints() As DataPoint, ByVal LearnRate As Double)
        Learn(TrainingPoints, 0, TrainingPoints.Length - 1, LearnRate)
    End Sub

    Public Sub Learn(ByVal TrainingPoints() As DataPoint, ByVal StartIndex As UInt32, ByVal EndIndex As UInt32, ByVal LearnRate As Double)

        'To do: Process datapoints in parallel.

        If Layers.Length > 1 Then

            System.Threading.Tasks.Parallel.For(StartIndex, EndIndex, Sub(i)
                                                                          Dim ThreadIndex As UInt32 = i - StartIndex
                                                                          Dim TempBaseDerivativeValues() As Double
                                                                          RunDataThroughNetwork(TrainingPoints(i).InputSpace, ThreadIndex)


                                                                          Layers(Layers.Length - 1).CalculateOutputLayerBaseDerivativeValues(TrainingPoints(i).ExpectedValueIndex, ThreadIndex)
                                                                          Layers(Layers.Length - 1).UpdateGradients(Layers(Layers.Length - 2).ActivationValues, ThreadIndex)

                                                                          For n = Layers.Length - 2 To 1 Step -1
                                                                              TempBaseDerivativeValues = Layers(n + 1).CalculatePreviousLayerBaseDerivativeValues(TrainingPoints(i).ExpectedValueIndex, Layers(n).WInputValues, ThreadIndex)
                                                                              Layers(n).Copy1DArrayToThread2DArray(TempBaseDerivativeValues, Layers(n).BaseDerivativeValues, ThreadIndex)
                                                                              Layers(n).UpdateGradients(Layers(n - 1).ActivationValues, ThreadIndex)
                                                                          Next

                                                                          TempBaseDerivativeValues = Layers(1).CalculatePreviousLayerBaseDerivativeValues(TrainingPoints(i).ExpectedValueIndex, Layers(0).WInputValues, ThreadIndex)
                                                                          Layers(0).Copy1DArrayToThread2DArray(TempBaseDerivativeValues, Layers(0).BaseDerivativeValues, ThreadIndex)
                                                                          Layers(0).UpdateGradients(TrainingPoints(i).InputSpace, ThreadIndex)
                                                                      End Sub)



        Else
            System.Threading.Tasks.Parallel.For(StartIndex, EndIndex, Sub(i)
                                                                          Dim ThreadIndex As UInt32 = i - StartIndex
                                                                          RunDataThroughNetwork(TrainingPoints(i).InputSpace, ThreadIndex)
                                                                          Layers(0).CalculateOutputLayerBaseDerivativeValues(TrainingPoints(i).ExpectedValueIndex, ThreadIndex)
                                                                          Layers(0).UpdateGradients(TrainingPoints(i).InputSpace, ThreadIndex)
                                                                      End Sub)
        End If

        For Each Layer In Layers
            Layer.ApplyGradients(LearnRate / (EndIndex - StartIndex + 1))
        Next
    End Sub





    Public Function GetTotalNodeCount() As UInt32
        Return GetTotalBiasCount() + NoOfInputs
    End Function
    Public Function GetTotalWeightCount() As UInt32
        GetTotalWeightCount = 0
        For Each Layer In Layers
            GetTotalWeightCount += Layer.NodeCount * Layer.InputNodeCount
        Next
    End Function
    Public Function GetTotalBiasCount() As UInt16
        GetTotalBiasCount = 0
        For Each Layer In Layers
            GetTotalBiasCount += Layer.NodeCount
        Next
    End Function

    Public Function GetAllWeightValues() As List(Of Double)
        Dim WeightList As New List(Of Double)
        For a = 0 To Layers.Length - 1
            For b = 0 To Layers(a).NodeCount - 1
                For c = 0 To Layers(a).InputNodeCount - 1
                    WeightList.Add(Layers(a).Weights(b, c))
                Next
            Next
        Next
        Return WeightList
    End Function
    Public Function GetAllBiasValues() As List(Of Double)
        Dim BiasList As New List(Of Double)
        For a = 0 To Layers.Length - 1
            For b = 0 To Layers(a).NodeCount - 1
                BiasList.Add(Layers(a).Bias(b))
            Next
        Next
        Return BiasList
    End Function
    Public Sub SetAllWeightValues(ByVal WeightList As List(Of Double))
        Dim a, b, c As UInt16
        For n = 0 To WeightList.Count - 1
            If Layers(a).InputNodeCount = c Then c = 0 : b += 1
            If Layers(a).NodeCount = b Then b = 0 : a += 1
            Layers(a).Weights(b, c) = WeightList(n)
            c += 1
        Next
    End Sub
    Public Sub SetAllBiasValues(ByVal BiasList As List(Of Double))
        Dim a, b As UInt16
        For n = 0 To BiasList.Count - 1
            If Layers(a).NodeCount = b Then b = 0 : a += 1
            Layers(a).Bias(b) = BiasList(n)
            b += 1
        Next
    End Sub

End Class