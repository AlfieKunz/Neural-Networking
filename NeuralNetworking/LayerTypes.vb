Imports System.Windows

Public Class NetworkLayerSigmoid
    Inherits NetworkLayer
    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        MyBase.New(NoOfNodes, NoOfInputNodes, Settings)
    End Sub
    Public Overrides Function ActivationFunction(ByRef InputData() As Double) As Double()
        For x = 0 To InputData.Length - 1
            InputData(x) = 1 / (1 + (e ^ -InputData(x)))
        Next
        Return InputData
    End Function
    Public Overrides Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Dim ActivationValue As Double = 1 / (1 + (e ^ -InputData(ThreadIndex, Index)))
        Return ActivationValue * (1 - ActivationValue)
    End Function
End Class



Public Class NetworkLayerPerceptron
    Inherits NetworkLayer
    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        MyBase.New(NoOfNodes, NoOfInputNodes, Settings)
    End Sub
    Public Overrides Function ActivationFunction(ByRef InputData() As Double) As Double()
        For x = 0 To InputData.Length - 1
            InputData(x) = If(InputData(x) > 0, 1, 0)
        Next
        Return InputData
    End Function
    Public Overrides Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Return 0
    End Function
End Class



Public Class NetworkLayerTanh
    Inherits NetworkLayer
    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        MyBase.New(NoOfNodes, NoOfInputNodes, Settings)
    End Sub
    Public Overrides Function ActivationFunction(ByRef InputData() As Double) As Double()
        Dim HelperValue As Double
        For x = 0 To InputData.Length - 1
            HelperValue = e ^ (2 * InputData(x))
            InputData(x) = (HelperValue - 1) / (HelperValue + 1)
        Next
        Return InputData
    End Function
    Public Overrides Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Dim HelperValue As Double = e ^ (2 * InputData(ThreadIndex, Index))
        Dim Activation As Double = (HelperValue - 1) / (HelperValue + 1)
        Return 1 - Activation ^ 2
    End Function
End Class



Public Class NetworkLayerReLU
    Inherits NetworkLayer
    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        MyBase.New(NoOfNodes, NoOfInputNodes, Settings)
    End Sub
    Public Overrides Function ActivationFunction(ByRef InputData() As Double) As Double()
        For x = 0 To InputData.Length - 1
            InputData(x) = Math.Max(0, InputData(x))
        Next
        Return InputData
    End Function
    Public Overrides Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Return If(InputData(ThreadIndex, Index) > 0, 1, 0)
    End Function
End Class



Public Class NetworkLayerSiLU
    Inherits NetworkLayer
    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        MyBase.New(NoOfNodes, NoOfInputNodes, Settings)
    End Sub
    Public Overrides Function ActivationFunction(ByRef InputData() As Double) As Double()
        For x = 0 To InputData.Length - 1
            InputData(x) = InputData(x) / (1 + e ^ -InputData(x))
        Next
        Return InputData
    End Function
    Public Overrides Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Dim Fx As Double = 1 / (1 + e ^ -InputData(ThreadIndex, Index))
        Return InputData(ThreadIndex, Index) * Fx * (1 - Fx) + Fx
    End Function
End Class



Public Class NetworkLayerSoftmax
    Inherits NetworkLayer
    Public Sub New(ByVal NoOfNodes As UInt16, ByVal NoOfInputNodes As UInt16, ByVal Settings As NetworkSettings)
        MyBase.New(NoOfNodes, NoOfInputNodes, Settings)
    End Sub
    Public Overrides Function ActivationFunction(ByRef InputData() As Double) As Double()
        Dim ExponentialSum As Double
        For x = 0 To InputData.Length - 1
            ExponentialSum += e ^ InputData(x)
        Next

        For x = 0 To InputData.Length - 1
            InputData(x) = (e ^ InputData(x)) / ExponentialSum
        Next
        Return InputData
    End Function
    Public Overrides Function ActivationDerivative(ByRef InputData(,) As Double, ByVal ThreadIndex As UInt16, ByVal Index As UInt16) As Double
        Dim ExponentialSum, HelperValue As Double
        For x = 0 To InputData.GetUpperBound(1)
            ExponentialSum += e ^ InputData(ThreadIndex, x)
        Next
        HelperValue = (e ^ InputData(ThreadIndex, Index))
        Return (HelperValue * ExponentialSum - (HelperValue * HelperValue)) / (ExponentialSum ^ 2)
    End Function
End Class