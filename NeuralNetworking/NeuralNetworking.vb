Imports System.ComponentModel.DataAnnotations
Imports System.Configuration
Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Policy
Imports System.Threading
Imports System.Threading.Tasks.Dataflow
Imports Accessibility
Imports Microsoft.VisualBasic.Devices

Public Structure DataPoint
    Dim InputSpace() As Double
    Dim ExpectedValueIndex As SByte
End Structure

Public Class NeuralNetworking
    'Allows the use of the Console in this FormsApp.
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function AllocConsole() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    Private Network As NeuralNetwork
    Private NetworkConfiguration() As UInt16 = {2, 3, 2}
    Private LearnRate As Double = 0.25
    Private BatchSize As UInt16
    Private Regularisation As Double
    Private ActivationFunction As String = "Sigmoid"
    Private OutputFunction As String = "Sigmoid"
    Private CostFunction As String = "CrossEntropy"
    Private CurrentCost As Double
    Private ABORTLearning As Boolean

    Private CurrentMode As String = "Graphs"

    Private DataPointsFileName As String = "Databases\Graph Database\Curve.txt"
    Private GraphDatabase(500 - 1) As DataPoint

    Private DigitTrainingDatabase() As DataPoint
    Private DigitValidationDatabase() As DataPoint
    Private DigitTestDatabase() As DataPoint

    Private DoodleTrainingDatabase() As DataPoint
    Private DoodleTestDatabase() As DataPoint
    Private NoOfDigitsInDatabase, NoOfDoodlesInDatabase As SByte
    Private DigitDatabaseNames(), DoodleDatabaseNames() As String

    Private CurrentDatabase() As DataPoint

    Private Const MapSize As UInt16 = 512

    Private GraphInputMap(MapSize ^ 2 - 1) As Boolean 'Is this point on the map good, or bad?
    Private DrawingInputMap(783) As Double


    Private CurrentlySelectedMenuStrip As SByte = -1
    Private DrawingStatus As New DrawingInfo
    Private Structure DrawingInfo
        Dim isDrawing As Boolean
        Dim StartValue As Point
        Dim EndValue As Point
    End Structure


    Private Sub NeuralNetworking_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AllocConsole()
        DoubleBuffered = True
        Me.KeyPreview = True

        AddHandler LearnButton.Click, AddressOf Learn
        AddHandler StopButton.Click, AddressOf StopButton_Click
        AddHandler FunctionTypeBar.ValueChanged, AddressOf FunctionTypeBar_ValueChanged
        AddHandler NetworkConfigButton.Click, AddressOf NetworkConfigButton_Click

        AddHandler ActFuncButton.Click, AddressOf FuncButton_Click
        AddHandler OutFuncButton.Click, AddressOf FuncButton_Click
        AddHandler CostFuncButton.Click, AddressOf FuncButton_Click
        AddHandler ActFuncOptions.ItemClicked, AddressOf FuncOptions_ItemClicked
        AddHandler OutFuncOptions.ItemClicked, AddressOf FuncOptions_ItemClicked
        AddHandler CostFuncOptions.ItemClicked, AddressOf FuncOptions_ItemClicked

        AddHandler SaveNetworkBtn.Click, AddressOf SaveNetworkBtn_Click
        AddHandler LoadNetworkBtn.Click, AddressOf LoadNetworkBtn_Click
        AddHandler ModeTrackbar.ValueChanged, AddressOf ModeTrackbar_ValueChanged
        AddHandler SimulationWindow.MouseDown, AddressOf SimulationWindow_MouseDown
        AddHandler SimulationWindow.MouseMove, AddressOf SimulationWindow_MouseMove
        AddHandler SimulationWindow.MouseUp, AddressOf SimulationWindow_MouseUp

        AddHandler DiagnosticsButton.Click, AddressOf DiagnosticsButton_Click
        AddHandler ClearButton.Click, AddressOf ClearButton_Click
        AddHandler MyBase.KeyDown, AddressOf C_Key_Clicked
        AddHandler ImageChooserBar.ValueChanged, AddressOf ImageChooserBar_ValueChanged

        Application.EnableVisualStyles()



        LearnRateBox.Text = LearnRate
        For Each Layer In NetworkConfiguration
            NetworkConfigBox.Text &= Layer & ","
        Next
        NetworkConfigBox.Text = NetworkConfigBox.Text.TrimEnd(",")
        ActFuncButton.Text = ActivationFunction
        OutFuncButton.Text = OutputFunction
        CostFuncButton.Text = CostFunction

        Select Case CurrentMode
            Case "Graphs"
                ModeTrackbar.Value = 1
            Case "Digits"
                ModeTrackbar.Value = 2
            Case "Doodles"
                ModeTrackbar.Value = 3
        End Select
        ModeLabel.Text = CurrentMode


        'GeneratePrecomputedDataPoints()
        ImportDatabases()

        ConfigureScreen()
        ConfigureNetwork()

    End Sub

    Private Sub ConfigureScreen()
        Select Case CurrentMode
            Case "Graphs"
                FunctionTypeBar.Visible = True
                FunctionTypeLabel.Visible = True
                ClearButton.Visible = False
                PressCLabel.Visible = False
                SimulationWindow.Width = 512
                SimulationWindow.Height = 512
                Me.Width = 800
                CurrentDatabase = GraphDatabase
                PredictionTable.Visible = False
                DiagnosticsButton.Visible = False
                ImageChooserBar.Visible = False
                ImageIndexLabel.Visible = False
                ImageLabel.Visible = False
            Case Else
                FunctionTypeBar.Visible = False
                FunctionTypeLabel.Visible = False
                ClearButton.Visible = True
                PressCLabel.Visible = True
                SimulationWindow.Width = 504
                SimulationWindow.Height = 504
                Me.Width = 1010
                If CurrentMode = "Digits" Then
                    CurrentDatabase = DigitTrainingDatabase
                ElseIf CurrentMode = "Doodles" Then
                    CurrentDatabase = DoodleTrainingDatabase
                End If
                PredictionTable.Visible = True
                DiagnosticsButton.Visible = True
                ImageIndexLabel.Visible = True
                ImageLabel.Visible = True
                ImageChooserBar.Visible = True
                ClearButton_Click()
        End Select
        NetworkConfigBox.Text = ""
        For Each Layer In NetworkConfiguration
            NetworkConfigBox.Text &= Layer & ","
        Next
        NetworkConfigBox.Text = NetworkConfigBox.Text.TrimEnd(",")
        SimulationWindow.Refresh()
    End Sub

    Dim ApplyRandomTransformations As Boolean = True
    Private Sub ImportDatabases()
        Console.WriteLine("~~~~~~~ Startup - Importing Databases ~~~~~~~")
        LoadGraphDatabase()


        Dim DatabasePath As String = Application.StartupPath & "Databases\Digit Database"
        Dim DatabaseFiles As String() = Directory.GetFiles(DatabasePath)
        NoOfDigitsInDatabase = DatabaseFiles.Length
        ReDim DigitDatabaseNames(NoOfDigitsInDatabase - 1)

        ReDim DigitTrainingDatabase(NoOfDigitsInDatabase * 5000 - 1)
        ReDim DigitTestDatabase(NoOfDigitsInDatabase * 1000 - 1)
        ReDim DigitValidationDatabase(NoOfDigitsInDatabase * 1000 - 1)


        Dim TrainingCounter, ValidationCounter, TestCounter As UInt32
        Dim Map(784 - 1) As Byte
        For i = 0 To NoOfDigitsInDatabase - 1
            Console.Write("Loading Digit Database... (" & (i + 1) & "/" & NoOfDigitsInDatabase & ")")
            Console.SetCursorPosition(0, Console.CursorTop)
            Using Stream As New FileStream(DatabaseFiles(i), FileMode.Open)
                Using BR As New BinaryReader(Stream)

                    Dim DoodleName As String = DatabaseFiles(i).Substring(DatabasePath.Length + 1)
                    DigitDatabaseNames(i) = DoodleName.Substring(0, DoodleName.Length - 4)

                    For n = 1 To 7000
                        If n <= 5000 Then
                            ReDim DigitTrainingDatabase(TrainingCounter).InputSpace(783)
                            BR.Read(Map, 0, 784)
                            For m = 0 To 783
                                DigitTrainingDatabase(TrainingCounter).InputSpace(m) = Map(m) / 256
                            Next
                            DigitTrainingDatabase(TrainingCounter).ExpectedValueIndex = i
                            TrainingCounter += 1
                        ElseIf n <= 6000 Then
                            ReDim DigitTestDatabase(TestCounter).InputSpace(783)
                            BR.Read(Map, 0, 784)
                            For m = 0 To 783
                                DigitTestDatabase(TestCounter).InputSpace(m) = Map(m) / 256
                            Next
                            DigitTestDatabase(TestCounter).ExpectedValueIndex = i
                            TestCounter += 1
                        Else
                            ReDim DigitValidationDatabase(ValidationCounter).InputSpace(783)
                            BR.Read(Map, 0, 784)
                            For m = 0 To 783
                                DigitValidationDatabase(ValidationCounter).InputSpace(m) = Map(m) / 256
                            Next
                            DigitValidationDatabase(ValidationCounter).ExpectedValueIndex = i
                            ValidationCounter += 1
                        End If
                    Next
                End Using
            End Using
        Next
        Console.WriteLine("Digit Database Successfully Loaded.")

        If ApplyRandomTransformations Then
            Console.Write("Applying Random Transformations to Digit Database... (1/3)")
            Console.SetCursorPosition(0, Console.CursorTop)
            ApplyRandomTransformationsToDatabase(DigitTrainingDatabase)
            Console.Write("Applying Random Transformations to Digit Database... (2/3)")
            Console.SetCursorPosition(0, Console.CursorTop)
            ApplyRandomTransformationsToDatabase(DigitValidationDatabase)
            Console.Write("Applying Random Transformations to Digit Database... (3/3)")
            Console.SetCursorPosition(0, Console.CursorTop)
            ApplyRandomTransformationsToDatabase(DigitTestDatabase)
            Console.WriteLine("Random Digit Transformations Successfully Applied.              ")
        End If




        DatabasePath = Application.StartupPath & "Databases\Doodle Database"
        Dim DoodleFiles As String() = Directory.GetFiles(DatabasePath)
        NoOfDoodlesInDatabase = DoodleFiles.Length

        ReDim DoodleTrainingDatabase(NoOfDoodlesInDatabase * 5000 - 1)
        ReDim DoodleTestDatabase(NoOfDoodlesInDatabase * 1000 - 1)
        ReDim DoodleDatabaseNames(NoOfDoodlesInDatabase - 1)

        TrainingCounter = 0
        ValidationCounter = 0
        TestCounter = 0
        For i = 0 To NoOfDoodlesInDatabase - 1
            Console.Write("Loading Doodle Database... (" & (i + 1) & "/" & NoOfDoodlesInDatabase & ")")
            Console.SetCursorPosition(0, Console.CursorTop)
            Using Stream As New FileStream(DoodleFiles(i), FileMode.Open)
                Using BR As New BinaryReader(Stream)

                    Dim DoodleName As String = DoodleFiles(i).Substring(DatabasePath.Length + 1)
                    DoodleDatabaseNames(i) = DoodleName.Substring(0, DoodleName.Length - 4)

                    For n = 1 To 6000
                        If n <= 5000 Then
                            ReDim DoodleTrainingDatabase(TrainingCounter).InputSpace(783)
                            BR.Read(Map, 0, 784)
                            For m = 0 To 783
                                DoodleTrainingDatabase(TrainingCounter).InputSpace(m) = Map(m) / 256
                            Next
                            DoodleTrainingDatabase(TrainingCounter).ExpectedValueIndex = i
                            TrainingCounter += 1
                        Else
                            ReDim DoodleTestDatabase(TestCounter).InputSpace(783)
                            BR.Read(Map, 0, 784)
                            For m = 0 To 783
                                DoodleTestDatabase(TestCounter).InputSpace(m) = Map(m) / 256
                            Next
                            DoodleTestDatabase(TestCounter).ExpectedValueIndex = i
                            TestCounter += 1
                        End If
                    Next
                End Using
            End Using
        Next
        Console.WriteLine("Doodle Database Successfully Loaded.")


        If ApplyRandomTransformations Then
            Console.Write("Applying Random Transformations to Doodle Database... (1/2)")
            Console.SetCursorPosition(0, Console.CursorTop)
            ApplyRandomTransformationsToDatabase(DoodleTrainingDatabase)
            Console.Write("Applying Random Transformations to Doodle Database... (2/2)")
            Console.SetCursorPosition(0, Console.CursorTop)
            ApplyRandomTransformationsToDatabase(DoodleTestDatabase)
            Console.WriteLine("Random Doodle Transformations Successfully Applied.        ")
        End If


        Console.WriteLine("~~~~~~~~~~~~~ Startup Complete ~~~~~~~~~~~~~~" & vbCrLf)
    End Sub

    Private Sub LoadGraphDatabase()
        Console.Write("Loading Graph Database...")
        Console.SetCursorPosition(0, Console.CursorTop)
        FileOpen(1, Application.StartupPath & DataPointsFileName, OpenMode.Input)
        Dim Index As UInt16 = 0
        While Not EOF(1) AndAlso Index < GraphDatabase.Length
            Dim Entry() As String = LineInput(1).Split(" ")
            ReDim GraphDatabase(Index).InputSpace(1)
            GraphDatabase(Index).InputSpace(0) = CDbl(Entry(0)) / 100
            GraphDatabase(Index).InputSpace(1) = CDbl(Entry(1)) / 100
            GraphDatabase(Index).ExpectedValueIndex = 1 - Entry(2)
            Index += 1
        End While
        FileClose(1)
        Console.WriteLine("Graph Database Successfully Loaded.")
    End Sub

    Private Sub ApplyRandomTransformationsToDatabase(ByVal Database() As DataPoint)
        'Scales the images randomly.
        System.Threading.Tasks.Parallel.For(0, 100, Sub(i)
                                                        Dim StartValue As UInt32 = i * (Database.Length / 100)
                                                        For n = StartValue To StartValue + (Database.Length / 100) - 1
                                                            ApplyRandomTransformationsToInputSpace(Database(n).InputSpace)
                                                        Next
                                                    End Sub)
    End Sub

    Private Function ApplyRandomTransformationsToInputSpace(ByRef InputSpace() As Double) As Double()
        Static RNDGen As New Random()

        'Scales size.
        Dim ScaleFactor As Double = RNDGen.NextDouble() * 0.45 + 0.55
        ScaleInputMap(InputSpace, ScaleFactor)


        'Scales X & Y positions.
        Dim ScaledXPos, ScaledYPos As Int16
        ScaledXPos = RNDGen.Next(-3, 4) + Math.Round((1 - ScaleFactor) * 10)
        ScaledYPos = (RNDGen.Next(-3, 4) + Math.Round((1 - ScaleFactor) * 10)) * 28

        If ScaledXPos > 0 Then
            For n = 783 - ScaledXPos To 0 Step -1
                If (n + ScaledXPos) Mod 28 > (n Mod 28) Then
                    InputSpace(n + ScaledXPos) = InputSpace(n)
                End If
                InputSpace(n) = 0
            Next
        ElseIf ScaledXPos < 0 Then
            For n = -ScaledXPos To 783
                If (n + ScaledXPos) Mod 28 < (n Mod 28) Then
                    InputSpace(n + ScaledXPos) = InputSpace(n)
                End If
                InputSpace(n) = 0
            Next
        End If

        If ScaledYPos > 0 Then
            For n = 783 To 0 Step -1
                If n + ScaledYPos < 783 Then
                    InputSpace(n + ScaledYPos) = InputSpace(n)
                End If
                InputSpace(n) = 0
            Next
        ElseIf ScaledYPos < 0 Then
            For n = 0 To 783
                If n + ScaledYPos > 0 Then
                    InputSpace(n + ScaledYPos) = InputSpace(n)
                End If
                InputSpace(n) = 0
            Next
        End If



        For n = 0 To 783
            'Applies a small colour boost to the image.
            If InputSpace(n) > 0.1 Then
                InputSpace(n) = Math.Min(1, InputSpace(n) * 1.2)
            End If


            'Apply Random Noise:
            If RNDGen.NextDouble() < 0.012 Then
                InputSpace(n) = Math.Abs(InputSpace(n) - RNDGen.NextDouble() * 0.6)
            End If
        Next

        Return InputSpace
    End Function

    Private Sub ScaleInputMap(ByRef Image() As Double, ByVal ScalingFactor As Double)
        Dim ScaledImage(783) As Double
        Dim OriginalRow, OriginalColumn As Double
        Dim RowLB, RowUB, ColumnLB, ColumnUB As Byte
        Dim RowWeight, ColumnWeight, Interpolation As Double

        For x = 0 To 27
            For y = 0 To 27
                OriginalRow = Math.Min(x / ScalingFactor, 27)
                OriginalColumn = Math.Min(y / ScalingFactor, 27)

                RowLB = Math.Floor(OriginalRow)
                RowUB = Math.Ceiling(OriginalRow)
                ColumnLB = Math.Floor(OriginalColumn)
                ColumnUB = Math.Ceiling(OriginalColumn)

                RowWeight = OriginalRow - RowLB
                ColumnWeight = OriginalColumn - ColumnLB

                Interpolation = (1 - RowWeight) * ((1 - ColumnWeight) * Image(RowLB * 28 + ColumnLB) +
                                      ColumnWeight * Image(RowLB * 28 + ColumnUB))

                Interpolation += RowWeight * ((1 - ColumnWeight) * Image(RowUB * 28 + ColumnLB) +
                                 ColumnWeight * Image(RowUB * 28 + ColumnUB))

                ScaledImage(x * 28 + y) = Interpolation
            Next
        Next

        Array.Copy(ScaledImage, Image, 784)
    End Sub



    Public Sub GeneratePrecomputedDataPoints()
        Static RNGen As New Random()
        Dim XPos, YPos As UInt16
        Dim OutputTrue As Boolean


        FileOpen(1, Application.StartupPath & "\Databases\Graph Database\Trig.txt", OpenMode.Output)
        For n = 1 To GraphDatabase.Length
            XPos = RNGen.Next(15, 499)
            YPos = RNGen.Next(15, 499)

            'Line Algorithm.
            'OutputTrue = YPos >= 2 * XPos - 50

            'Curve Algorithm.
            'OutputTrue = 0 >= XPos ^ 2 + YPos ^ 2 - 3 * XPos * YPos - XPos - YPos + (300) ^ 2

            'Circle Algorithm.
            'OutputTrue = Math.Sqrt((XPos - 275) ^ 2 + (YPos - 315) ^ 2) <= 140

            'Trig Algorithm.
            'OutputTrue = (80 * Math.Sin(XPos / 50) + 175 <= YPos) AndAlso (YPos <= 80 * Math.Sin(XPos / 50) + 325)

            If OutputTrue Then
                PrintLine(1, XPos & " " & YPos & " 1")
            Else
                PrintLine(1, XPos & " " & YPos & " 0")
            End If
        Next
        FileClose(1)
    End Sub

    Public Sub ConfigureNetwork()
        Dim DebugString As String = "Calibrating New Network, with Configuration: {"
        For Each Layer In NetworkConfiguration
            DebugString &= Layer & ","
        Next
        Console.WriteLine(vbCrLf & DebugString.TrimEnd(",") & "} ..." & vbCrLf)
        BatchSize = Val(MiniBatchSizeBox.Text)
        Regularisation = Val(RegularisationBox.Text)

        Network = New NeuralNetwork(NetworkConfiguration, ActivationFunction, OutputFunction, CostFunction, Regularisation, BatchSize)


        If CurrentMode = "Graphs" Then
            UpdateGraphNetwork()
        End If
    End Sub

    'Private Sub UpdateWeightBiasTrackbars(ByVal NeedsConfiguring As Boolean)
    '    WeightList = Network.GetAllWeightValues()
    '    BiasList = Network.GetAllBiasValues()

    '    If NeedsConfiguring Then
    '        If WeightTrackbars IsNot Nothing Then
    '            For Each Trackbar In WeightTrackbars
    '                Trackbar.Dispose()
    '            Next
    '        End If
    '        If BiasTrackbars IsNot Nothing Then
    '            For Each Trackbar In BiasTrackbars
    '                Trackbar.Dispose()
    '            Next
    '        End If

    '        ReDim WeightTrackbars(WeightList.Count - 1)
    '        ReDim BiasTrackbars(BiasList.Count - 1)

    '        For n = 0 To WeightList.Count - 1
    '            WeightTrackbars(n) = New TrackBar
    '            Me.Controls.Add(WeightTrackbars(n))
    '            AddHandler WeightTrackbars(n).ValueChanged, AddressOf HandleTrackerUpdate
    '            WeightTrackbars(n).Location = New Point(550, n * 60 + 55)
    '            WeightTrackbars(n).Minimum = -100
    '            WeightTrackbars(n).Maximum = 100
    '        Next
    '        For n = 0 To BiasList.Count - 1
    '            BiasTrackbars(n) = New TrackBar
    '            Me.Controls.Add(BiasTrackbars(n))
    '            AddHandler BiasTrackbars(n).ValueChanged, AddressOf HandleTrackerUpdate
    '            BiasTrackbars(n).Location = New Point(650, n * 60 + 55)
    '            BiasTrackbars(n).Minimum = -100
    '            BiasTrackbars(n).Maximum = 100
    '        Next
    '    End If


    '    For n = 0 To WeightList.Count - 1
    '        WeightTrackbars(n).Value = WeightList(n) / 100
    '    Next
    '    For n = 0 To BiasList.Count - 1
    '        BiasTrackbars(n).Value = BiasList(n) / 100
    '    Next
    'End Sub

    'Public Sub HandleTrackerUpdate()
    '    'Saves the weights & biases to the network.
    '    For n = 0 To WeightTrackbars.Count - 1
    '        WeightList(n) = WeightTrackbars(n).Value / 100
    '    Next
    '    For n = 0 To BiasTrackbars.Count - 1
    '        BiasList(n) = BiasTrackbars(n).Value / 100
    '    Next
    '    Network.SetAllWeightValues(WeightList)
    '    Network.SetAllBiasValues(BiasList)
    '    UpdateGraphNetwork()
    '    CostLabel.Text = "Cost: " & Math.Round(CurrentCost, 4)
    'End Sub




    Private Sub DrawSimulation(ByVal sender As Object, ByVal e As PaintEventArgs) Handles SimulationWindow.Paint
        Dim g As Graphics = e.Graphics
        If CurrentMode = "Graphs" Then
            'Draws the Network Prediction Map onto the screen.
            Using Brush As New SolidBrush(Color.Pink)
                g.FillRectangle(Brush, 0, 0, MapSize, MapSize)
            End Using
            For y = 0 To MapSize - 1
                For x = 0 To MapSize - 1
                    If GraphInputMap(y * 512 + x) Then
                        Using Brush As New SolidBrush(Color.PaleGreen)
                            g.FillRectangle(Brush, x, y, 2, 2)
                        End Using
                    End If
                Next
            Next


            Dim Size As UInt16 = 15
            For Each DataEntry In GraphDatabase
                Dim XValue As UInt16 = (DataEntry.InputSpace(0) * 100) - Size / 2
                Dim YValue As UInt16 = (DataEntry.InputSpace(1) * 100) - Size / 2
                If DataEntry.ExpectedValueIndex = 0 Then
                    Using Brush As New SolidBrush(Color.Green)
                        g.FillEllipse(Brush, XValue, YValue, Size, Size)
                    End Using
                Else
                    Using Brush As New SolidBrush(Color.Red)
                        g.FillEllipse(Brush, XValue, YValue, Size, Size)
                    End Using
                End If
            Next

            'Dim TempPen As Pen
            'TempPen = New Pen(Drawing.Color.Blue, 2)
            'Dim CircleCentreX As Int16 = 275
            'Dim CircleCentreY As Int16 = 315
            'Dim CircleRadius As Int16 = 140
            'g.DrawEllipse(TempPen, CircleCentreX - CircleRadius, CircleCentreY - CircleRadius, CircleRadius * 2, CircleRadius * 2)
        Else
            Using Brush As New SolidBrush(SimulationWindow.BackColor)
                g.FillRectangle(Brush, 0, 0, MapSize, MapSize)
            End Using

            Dim SizeMultiplier As Int16 = 18
            Dim ImageIndex As UInt32 = 1414
            Dim MapToUse() As Double = DrawingInputMap 'DigitTrainingDatabase(18223).InputSpace
            For n = 0 To 783
                Dim XValue As Int16 = (n Mod 28) * SizeMultiplier
                Dim YValue As Int16 = (n \ 28) * SizeMultiplier
                Dim PixelColour As Byte = Math.Min(MapToUse(n) * 256, 255)
                Using Brush As New SolidBrush(Color.FromArgb(PixelColour, PixelColour, PixelColour))
                    g.FillRectangle(Brush, XValue, YValue, SizeMultiplier, SizeMultiplier)
                End Using
            Next
        End If
    End Sub


    Private Sub SimulationWindow_MouseDown(sender As Object, e As MouseEventArgs)
        If CurrentMode <> "Graphs" AndAlso (e.Button = MouseButtons.Left OrElse e.Button = MouseButtons.Right) Then
            DrawingStatus.isDrawing = True
            DrawingStatus.StartValue = e.Location
            SimulationWindow_MouseMove(sender, e)
        End If
    End Sub
    Private Sub SimulationWindow_MouseMove(sender As Object, e As MouseEventArgs)
        Static RNDGen As New Random()
        If CurrentMode <> "Graphs" AndAlso DrawingStatus.isDrawing Then
            Dim MouseRadius As Double = If(CurrentMode = "Digits", 0.9, 0.67) * If(e.Button = MouseButtons.Left, 1.0, 0.5)
            Dim ScaledXPos As Double = e.Location.X / 18
            Dim ScaledYPos As Double = e.Location.Y / 18
            Dim FlatArrayPos As Integer
            Dim Distance As Double
            If (ScaledXPos >= 0 AndAlso ScaledXPos < 28) AndAlso (ScaledYPos >= 0 AndAlso ScaledYPos < 28) Then

                'Defines a radius around the mouse's position, defining the colour.
                For x = Math.Floor(ScaledXPos - MouseRadius) To Math.Floor(ScaledXPos + MouseRadius)
                    For y = Math.Floor(ScaledYPos - MouseRadius) To Math.Floor(ScaledYPos + MouseRadius)
                        If (x >= 0 AndAlso x < 28) AndAlso (y >= 0 AndAlso y < 28) Then
                            FlatArrayPos = y * 28 + x
                            If e.Button = MouseButtons.Left Then
                                Distance = Math.Sqrt((x + 0.5 - ScaledXPos) ^ 2 + (y + 0.5 - ScaledYPos) ^ 2) / (Math.Sqrt(2) * MouseRadius)
                                DrawingInputMap(FlatArrayPos) = Math.Max(DrawingInputMap(FlatArrayPos), 1 - Distance ^ 3)
                            Else
                                DrawingInputMap(FlatArrayPos) = 0
                            End If
                        End If
                    Next
                Next
                SimulationWindow.Invalidate()
            End If
            If RNDGen.NextDouble <= 0.25 Then PopulatePredictionTable()



            'Old function.
            'Dim ScaledXPos As Double = e.Location.X / 18
            'Dim ScaledYPos As Double = e.Location.Y / 18
            'If (ScaledXPos > 0 AndAlso ScaledXPos < 28) AndAlso (ScaledYPos > 0 AndAlso ScaledYPos < 28) Then

            '    If e.Button = MouseButtons.Left Then
            '        Dim Distance As Double = Math.Sqrt((0.5 - (ScaledXPos Mod 1)) ^ 2 + (0.5 - (ScaledYPos Mod 1)) ^ 2)
            '        'Dim ScaledDistace As Double = 2 - (1 - Distance) ^ -0.4
            '        Dim ScaledDistace As Double = Math.Min(2.08 - (0.79 - Distance) ^ -0.25, 1)
            '        Dim Colour As Byte = Math.Round(ScaledDistace * 255)
            '        DrawingInputMap(Math.Floor(ScaledYPos) * 28 + Math.Floor(ScaledXPos)) = Math.Max(DrawingInputMap(Math.Floor(ScaledYPos) * 28 + Math.Floor(ScaledXPos)), Colour / 256)
            '    Else
            '        DrawingInputMap(Math.Floor(ScaledYPos) * 28 + Math.Floor(ScaledXPos)) = 0
            '    End If

            '    SimulationWindow.Invalidate()
            'End If
            'If RNDGen.NextDouble <= 0.25 Then PopulatePredictionTable()
        End If
    End Sub

    Private Sub PopulatePredictionTable()
        PredictionTable.Text = "Network Prediction:"
        Dim NetworkPrediction() As Double = Network.RunDataThroughNetwork(DrawingInputMap)
        Dim TotalActivation As Double
        For Each Value In NetworkPrediction
            TotalActivation += Value
        Next
        If TotalActivation = 0 Then TotalActivation = Double.MaxValue

        Dim IndexArray(NetworkPrediction.Length - 1) As String
        Select Case CurrentMode
            Case "Digits"
                Array.Copy(DigitDatabaseNames, IndexArray, IndexArray.Length)
            Case "Doodles"
                Array.Copy(DoodleDatabaseNames, IndexArray, IndexArray.Length)
        End Select


        'Sorts NetworkPrediction.
        Dim TempPercentage As Double
        Dim TempItem As String

        For i = 0 To NetworkPrediction.Length - 2
            For j = 0 To NetworkPrediction.Length - i - 2
                If NetworkPrediction(j) < NetworkPrediction(j + 1) Then
                    'Swaps NetworkPrediction.
                    TempPercentage = NetworkPrediction(j)
                    NetworkPrediction(j) = NetworkPrediction(j + 1)
                    NetworkPrediction(j + 1) = TempPercentage

                    'Swaps IndexArray.
                    TempItem = IndexArray(j)
                    IndexArray(j) = IndexArray(j + 1)
                    IndexArray(j + 1) = TempItem
                End If
            Next
        Next

        For n = 0 To IndexArray.Length - 1
            PredictionTable.Text &= vbCrLf & IndexArray(n) & ": " & Math.Round(NetworkPrediction(n) / TotalActivation * 100, 2) & "%."
        Next
    End Sub

    Private Sub SimulationWindow_MouseUp(sender As Object, e As MouseEventArgs)
        If CurrentMode <> "Graphs" AndAlso DrawingStatus.isDrawing Then
            DrawingStatus.isDrawing = False
        End If
    End Sub



    Private Sub UpdateGraphNetwork()
        'Dim LearnTimer As New Stopwatch()
        'LearnTimer.Start()

        Dim Tasks(3) As Task
        For n = 0 To 3
            Dim TempValue As SByte = n
            Tasks(n) = (Task.Run(Sub() UpdateThreadOperation(TempValue)))
        Next
        Task.WaitAll(Tasks)
        SimulationWindow.Invalidate()
        CurrentCost = Network.CalculateCost(CurrentDatabase)

        'LearnTimer.Stop()
        'Console.WriteLine("Learning Complete in: " & LearnTimer.Elapsed.TotalMilliseconds & "ms. No of Nodes Correct = " & Network.CalculateNoOfPointsCorrect(PredeterminedDataPoints) & "/256. Cost = " & Math.Round(Cost, 6) & ".")
    End Sub


    Private Sub Learn()
        Dim LearnThread As New Task(AddressOf LearnThreadOperation)
        ABORTLearning = False
        StopButton.Visible = True
        LearnRate = LearnRateBox.Text
        NetworkConfigButton.Enabled = False
        LoadNetworkBtn.Enabled = False
        DiagnosticsButton.Enabled = False
        ModeTrackbar.Enabled = False

        LearnThread.Start()
        While Not LearnThread.IsCompleted
            Thread.Sleep(10)
            'CostLabel.Text = "Cost: " & Math.Round(CurrentCost, 4)
            Application.DoEvents()
        End While
        NetworkConfigButton.Enabled = True
        LoadNetworkBtn.Enabled = True
        DiagnosticsButton.Enabled = True
        ModeTrackbar.Enabled = True
        If CurrentMode <> "Graphs" Then
            Console.SetCursorPosition(0, Console.CursorTop - 4)
            For n = 1 To 4
                Console.WriteLine(New String(" ", 90))
            Next
            Console.SetCursorPosition(0, Console.CursorTop - 6)
        End If
    End Sub
    Private Sub LearnThreadOperation()
        Static RNDGen As New Random()
        Dim MiniBatchCounter, EpochCounter, BatchesPerEpoch As UInt64
        Dim StartIndex, EndIndex As UInt32
        Dim PercentNetworkUpdated As Double = If(CurrentMode = "Graphs", 0.05, 1)
        Dim UpdateFrequency As UInt16 = 250 'How many times will the console be updated per epoch?

        Dim TrainingDatabase(CurrentDatabase.Length - 1) As DataPoint
        Array.Copy(CurrentDatabase, TrainingDatabase, CurrentDatabase.Length)

        If BatchSize >= TrainingDatabase.Length OrElse BatchSize <= 0 Then
            BatchSize = TrainingDatabase.Length
        End If
        Network.CalibrateNoOfLearningThreads(BatchSize)

        Console.WriteLine(vbCrLf)
        While Not ABORTLearning
            If CurrentMode <> "Graphs" Then Console.WriteLine(vbCrLf & vbCrLf & vbCrLf & vbCrLf)

            If BatchSize < TrainingDatabase.Length Then
                'Uses the Fiser-Yates Shuffle to construct a randomised set of TrainingDatabase
                '(which will then be split up into  small mini-batches).
                Dim TempValue As DataPoint
                Dim Index As UInt64
                For n = TrainingDatabase.Length - 1 To 0 Step -1
                    Index = RNDGen.Next(0, n + 1)
                    TempValue = TrainingDatabase(n)
                    TrainingDatabase(n) = TrainingDatabase(Index)
                    TrainingDatabase(Index) = TempValue
                Next
            End If
            StartIndex = 0
            BatchesPerEpoch = Math.Ceiling(TrainingDatabase.Length / BatchSize)

            For n As UInt64 = 1 To BatchesPerEpoch
                If ABORTLearning Then Exit While
                EndIndex = Math.Min(StartIndex + BatchSize, TrainingDatabase.Length - 1) - 1

                Network.Learn(TrainingDatabase, StartIndex, EndIndex, LearnRate)
                MiniBatchCounter += 1
                If CurrentMode <> "Graphs" Then
                    If n Mod (BatchesPerEpoch \ UpdateFrequency) = 0 OrElse n = BatchesPerEpoch Then OutputLearnStatsToConsole(n * BatchSize, EpochCounter, 100 * n / BatchesPerEpoch, False)
                End If
                StartIndex = EndIndex + 1
            Next
            EpochCounter += 1

            If RNDGen.NextDouble() <= PercentNetworkUpdated Then
                If CurrentMode = "Graphs" Then
                    UpdateGraphNetwork()
                    Console.WriteLine("Epoch " & EpochCounter & " Complete. No of Mini-Batches Complete: " & MiniBatchCounter & ". Current Cost = " & Math.Round(CurrentCost, 6) & ".")
                Else
                    OutputLearnStatsToConsole(0, EpochCounter, 0, True)
                End If
            End If
        End While
    End Sub

    Private Sub OutputLearnStatsToConsole(ByVal CurrentBatch As UInt64, ByVal EpochCount As UInt64, ByVal Percentage As Decimal, ByVal NewEpoch As Boolean)
        Console.SetCursorPosition(0, Console.CursorTop - 6)
        Console.WriteLine()
        If NewEpoch Then

            Dim TestDatabase() As DataPoint = If(CurrentMode = "Digits", DigitTestDatabase, DoodleTestDatabase)

            Dim Performance() As Double = Network.CalculateDatasetPerformance(CurrentDatabase, TestDatabase)
            Console.Write("Epoch " & EpochCount & " Complete.".PadRight(13 - If(EpochCount >= 10, 1, 0) - If(EpochCount >= 100, 1, 0)))
            Console.WriteLine("Training Accuracy: " & Performance(0).ToString("00.00") & "%.  Test Accuracy: " & Performance(1).ToString("00.00") & "%.   Cost =  " & Performance(2).ToString("0.00000") & ".")
        Else
            Console.WriteLine(New String(" ", 60))
            Console.WriteLine("Current Epoch: " & EpochCount.ToString("N0") + 1 & ". Total DataPoint Count: " & CurrentBatch.ToString("N0") & "/" & CurrentDatabase.Length.ToString("N0") & ".")
            Console.WriteLine(" " & New String("-", 50) & "  ")

            Dim PercentageString As String
            If Percentage = 100 Then
                PercentageString = New String(ChrW(&H2588), 19) & " Validating " & New String(ChrW(&H2588), 19)
            Else
                PercentageString = New String(ChrW(&H2588), Math.Truncate(Percentage / 2)).PadRight(50)
                PercentageString = PercentageString.Substring(0, 22) & Percentage.ToString("00.00") & "%" & PercentageString.Substring(28)
            End If

            Console.Write("|")
            Console.ForegroundColor = If(Percentage < 100, ConsoleColor.Green, ConsoleColor.DarkYellow)
            Console.Write(PercentageString)
            Console.ResetColor()
            Console.WriteLine("|")
            Console.WriteLine(" " & New String("-", 50))
        End If
    End Sub




    Private Sub UpdateThreadOperation(ByVal Index As SByte)
        Dim TempPredictionValue As UInt16
        For y = 0 To MapSize - 1 Step 2
            For x = Index * 2 To MapSize - 1 Step 8
                TempPredictionValue = Network.CalculatePrediction({x / 100, y / 100})
                GraphInputMap(y * 512 + x) = If(TempPredictionValue = 0, True, False)
            Next
        Next
    End Sub






    Private Sub FunctionTypeBar_ValueChanged()
        Select Case FunctionTypeBar.Value
            Case 1
                DataPointsFileName = "Databases\Graph Database\Line.txt"
                FunctionTypeLabel.Text = "Line"
            Case 2
                DataPointsFileName = "Databases\Graph Database\Curve.txt"
                FunctionTypeLabel.Text = "Curve"
            Case 3
                DataPointsFileName = "Databases\Graph Database\Circle.txt"
                FunctionTypeLabel.Text = "Circle"
            Case 4
                DataPointsFileName = "Databases\Graph Database\Trig.txt"
                FunctionTypeLabel.Text = "Trig"
        End Select
        LoadGraphDatabase()
        UpdateGraphNetwork()
    End Sub

    Private Sub NetworkConfigButton_Click()
        NetworkConfigBox.Text = (NetworkConfigBox.Text.TrimEnd(",")).Replace(" ", "")
        Dim StringConfig() As String = NetworkConfigBox.Text.Split(",")
        Dim IntConfig(StringConfig.Length - 1) As UInt16
        Dim ValidConfiguration As Boolean = True
        For n = 0 To StringConfig.Length - 1
            IntConfig(n) = Val(StringConfig(n))
            If IntConfig(n) = 0 Then ValidConfiguration = False : Exit For
        Next

        Dim StartValue, EndValue As UInt16
        Select Case CurrentMode
            Case "Graphs"
                StartValue = 2
                EndValue = 2
            Case "Digits"
                StartValue = 784
                EndValue = NoOfDigitsInDatabase
            Case "Doodles"
                StartValue = 784
                EndValue = NoOfDoodlesInDatabase
        End Select
        If Not (IntConfig(0) = StartValue AndAlso IntConfig.Last = EndValue) Then ValidConfiguration = False


        If ValidConfiguration Then
            NetworkConfiguration = IntConfig
            ConfigureNetwork()
            If CurrentMode <> "Graphs" Then PopulatePredictionTable()
        Else
            Console.WriteLine("Error whilst configuring Network.")
        End If
    End Sub

    Private Sub StopButton_Click()
        StopButton.Visible = False
        ABORTLearning = True
    End Sub


    Private Sub FuncButton_Click(sender As Object, e As EventArgs) Handles OutFuncButton.Click
        Select Case sender.name(0)
            Case "A"
                ActFuncOptions.Show(sender, 0, sender.height)
                CurrentlySelectedMenuStrip = 0
            Case "O"
                OutFuncOptions.Show(sender, 0, sender.height)
                CurrentlySelectedMenuStrip = 1
            Case "C"
                CostFuncOptions.Show(sender, 0, sender.height)
                CurrentlySelectedMenuStrip = 2
        End Select
    End Sub

    Private Sub FuncOptions_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        Dim AbstractedName As String = (e.ClickedItem.Name).Remove((e.ClickedItem.Name.Length) - "ToolMenuStripItem".Length)
        If CurrentlySelectedMenuStrip = 1 Then AbstractedName = AbstractedName.Remove(0, 1)
        Select Case CurrentlySelectedMenuStrip
            Case 0
                ActivationFunction = AbstractedName
                ActFuncButton.Text = AbstractedName
            Case 1
                OutputFunction = AbstractedName
                OutFuncButton.Text = AbstractedName
            Case 2
                CostFunction = AbstractedName
                CostFuncButton.Text = AbstractedName
        End Select
    End Sub

    Private Sub SaveNetworkBtn_Click()
        Dim NetworkWeights, NetworkBias As New List(Of Double)
        NetworkWeights = Network.GetAllWeightValues()
        NetworkBias = Network.GetAllBiasValues()
        Using SR As New StreamWriter(Application.StartupPath & "Saved Networks\Saved Network - " & DateTime.Now().ToString("yyyy.MM.dd.HH.mm.ss") & ".txt")
            SR.WriteLine(NetworkConfigBox.Text)
            SR.WriteLine(LearnRateBox.Text)
            SR.WriteLine(ActivationFunction)
            SR.WriteLine(OutputFunction)
            SR.WriteLine(CostFunction)
            SR.WriteLine(MiniBatchSizeBox.Text)
            SR.WriteLine(RegularisationBox.Text)
            SR.WriteLine()
            For Each Weight In NetworkWeights
                SR.WriteLine(Weight)
            Next
            SR.WriteLine(":")
            For Each Bias In NetworkBias
                SR.WriteLine(Bias)
            Next
        End Using
        Console.WriteLine(vbCrLf & "Network Successfully Saved." & vbCrLf)
    End Sub

    Private Sub LoadNetworkBtn_Click()
        Dim SelectFilePopup As New OpenFileDialog()
        SelectFilePopup.InitialDirectory = Application.StartupPath & "Saved Networks"
        Dim UserResult As DialogResult = SelectFilePopup.ShowDialog()
        If UserResult = DialogResult.OK Then
            Dim UserSelectedPath As String = SelectFilePopup.FileName

            Dim NetworkWeights, NetworkBias As New List(Of Double)
            Dim Codex As Boolean
            Using SR As New StreamReader(UserSelectedPath)
                NetworkConfigBox.Text = SR.ReadLine()
                LearnRateBox.Text = SR.ReadLine()
                ActivationFunction = SR.ReadLine()
                ActFuncButton.Text = ActivationFunction
                OutputFunction = SR.ReadLine()
                OutFuncButton.Text = OutputFunction
                CostFunction = SR.ReadLine()
                CostFuncButton.Text = CostFunction
                MiniBatchSizeBox.Text = SR.ReadLine()
                RegularisationBox.Text = SR.ReadLine()
                NetworkConfigButton_Click()

                SR.ReadLine()

                While Not SR.EndOfStream
                    Dim InputLine As String = SR.ReadLine()
                    If InputLine = ":" Then
                        Codex = True
                    Else
                        If Codex Then
                            NetworkBias.Add(InputLine)
                        Else
                            NetworkWeights.Add(InputLine)
                        End If
                    End If
                End While
            End Using
            Console.WriteLine()
            If NetworkWeights.Count = Network.GetTotalWeightCount() AndAlso NetworkBias.Count = Network.GetTotalBiasCount() Then
                Network.SetAllWeightValues(NetworkWeights)
                Network.SetAllBiasValues(NetworkBias)
                If CurrentMode = "Graphs" Then UpdateGraphNetwork()
                Console.WriteLine("Network Successfully Loaded.")
                If CurrentMode <> "Graphs" Then PopulatePredictionTable()
            Else
                Console.WriteLine("Error whilst loading Network - Network Size does not match current configuration.")
            End If
            Console.WriteLine()

        End If
    End Sub



    Private Sub ModeTrackbar_ValueChanged()
        Select Case ModeTrackbar.Value
            Case 1
                CurrentMode = "Graphs"
                NetworkConfiguration = {2, 3, 2}
                LearnRateBox.Text = "0.25"
                MiniBatchSizeBox.Text = "500"
                RegularisationBox.Text = "0.05"
            Case 2
                CurrentMode = "Digits"
                NetworkConfiguration = {784, 100, NoOfDigitsInDatabase}
                ImageChooserBar.Maximum = 1000 * NoOfDigitsInDatabase
                LearnRateBox.Text = "0.05"
                MiniBatchSizeBox.Text = "100"
                RegularisationBox.Text = "0.25"
            Case 3
                CurrentMode = "Doodles"
                NetworkConfiguration = {784, 150, NoOfDoodlesInDatabase}
                ImageChooserBar.Maximum = 1000 * NoOfDoodlesInDatabase
                LearnRateBox.Text = "0.05"
                MiniBatchSizeBox.Text = "100"
                RegularisationBox.Text = "0.5"
        End Select
        ModeLabel.Text = CurrentMode
        ConfigureNetwork()
        ConfigureScreen()
    End Sub

    Private Sub ClearButton_Click()
        If ImageChooserBar.Value = 0 Then
            ImageChooserBar_ValueChanged()
        Else
            ImageChooserBar.Value = 0
        End If
    End Sub
    Private Sub C_Key_Clicked(sender As Object, e As KeyEventArgs)
        If CurrentMode <> "Graphs" AndAlso e.KeyCode = Keys.C Then ClearButton_Click()
    End Sub

    Private Sub ImageChooserBar_ValueChanged()
        Dim Database() As DataPoint = If(CurrentMode = "Digits", DigitTestDatabase, DoodleTestDatabase)

        If ImageChooserBar.Value = 0 Then
            For n = 0 To DrawingInputMap.Length - 1
                DrawingInputMap(n) = 0
            Next
            ImageIndexLabel.Text = "Current Image: None"
            ImageLabel.Text = "Image Label: None"
        Else
            Array.Copy(Database(ImageChooserBar.Value - 1).InputSpace, DrawingInputMap, 783)
            ImageIndexLabel.Text = "Current Image: " & ImageChooserBar.Value
            ImageLabel.Text = "Image Label: " & Database(ImageChooserBar.Value - 1).ExpectedValueIndex
        End If
        SimulationWindow.Invalidate()
        PopulatePredictionTable()
    End Sub


    Private Sub DiagnosticsButton_Click()
        Console.Write(vbCrLf & vbCrLf & "Calculating Specific Performance Stats...")
        Console.SetCursorPosition(0, Console.CursorTop)

        Dim PerformanceResults As (Stats As Decimal(), Occurances As Integer())
        Dim Outputs() As String
        Console.Write("Calculating Specific Performance Stats...")
        Console.SetCursorPosition(0, Console.CursorTop)

        Select Case CurrentMode
            Case "Digits"
                PerformanceResults = Network.CalculateIndividualObjectPerformance(DigitTestDatabase)
                ReDim Outputs(9)
                For n = 0 To 9
                    Outputs(n) = n
                Next
            Case Else 'Represents "Doodles"
                PerformanceResults = Network.CalculateIndividualObjectPerformance(DoodleTestDatabase)
                ReDim Outputs(NoOfDoodlesInDatabase - 1)
                Array.Copy(DoodleDatabaseNames, Outputs, Outputs.Length)
        End Select

        Console.WriteLine("Specific Network Performance Stats...    ")
        For n = 0 To PerformanceResults.Stats.Length - 1
            Console.WriteLine(Outputs(n) & " : " & Math.Round(100 * PerformanceResults.Stats(n) / PerformanceResults.Occurances(n), 2) & "% Correct.")
        Next
        Console.WriteLine("Total Database Accuracy: " & Math.Round(100 * PerformanceResults.Stats.Sum() / PerformanceResults.Occurances.Sum(), 2) & "% Correct.")
        Console.WriteLine()
    End Sub

End Class
