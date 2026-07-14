<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class NeuralNetworking
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        SimulationWindow = New PictureBox()
        LearnButton = New Button()
        FunctionTypeBar = New TrackBar()
        FunctionTypeLabel = New Label()
        NetworkConfigBox = New TextBox()
        NetworkConfigButton = New Button()
        ConfigLabel = New Label()
        StopButton = New Button()
        LearnRateBox = New TextBox()
        Label3 = New Label()
        Label1 = New Label()
        Label2 = New Label()
        Label4 = New Label()
        Label5 = New Label()
        ActFuncOptions = New ContextMenuStrip(components)
        SigmoidToolStripMenuItem = New ToolStripMenuItem()
        PerceptronToolStripMenuItem = New ToolStripMenuItem()
        TanhToolStripMenuItem = New ToolStripMenuItem()
        ReLUToolStripMenuItem = New ToolStripMenuItem()
        SiLUToolStripMenuItem = New ToolStripMenuItem()
        SoftmaxToolStripMenuItem = New ToolStripMenuItem()
        CostFuncOptions = New ContextMenuStrip(components)
        MeanSquaredToolStripMenuItem = New ToolStripMenuItem()
        CrossEntropyToolStripMenuItem = New ToolStripMenuItem()
        ActFuncButton = New Button()
        CostFuncButton = New Button()
        OutFuncOptions = New ContextMenuStrip(components)
        OSigmoidToolStripMenuItem = New ToolStripMenuItem()
        OPerceptronToolStripMenuItem = New ToolStripMenuItem()
        OTanhToolStripMenuItem = New ToolStripMenuItem()
        OReLUToolStripMenuItem = New ToolStripMenuItem()
        OSiLUToolStripMenuItem = New ToolStripMenuItem()
        OSoftmaxToolStripMenuItem = New ToolStripMenuItem()
        OutFuncButton = New Button()
        SaveNetworkBtn = New Button()
        LoadNetworkBtn = New Button()
        ModeTrackbar = New TrackBar()
        ModeLabel = New Label()
        ClearButton = New Button()
        PredictionTable = New Label()
        Label6 = New Label()
        MiniBatchSizeBox = New TextBox()
        ImageChooserBar = New TrackBar()
        ImageIndexLabel = New Label()
        ImageLabel = New Label()
        DiagnosticsButton = New Button()
        PressCLabel = New Label()
        Regu = New Label()
        RegularisationBox = New TextBox()
        CType(SimulationWindow, ComponentModel.ISupportInitialize).BeginInit()
        CType(FunctionTypeBar, ComponentModel.ISupportInitialize).BeginInit()
        ActFuncOptions.SuspendLayout()
        CostFuncOptions.SuspendLayout()
        OutFuncOptions.SuspendLayout()
        CType(ModeTrackbar, ComponentModel.ISupportInitialize).BeginInit()
        CType(ImageChooserBar, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' SimulationWindow
        ' 
        SimulationWindow.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        SimulationWindow.Location = New Point(14, 15)
        SimulationWindow.Margin = New Padding(4, 3, 4, 3)
        SimulationWindow.Name = "SimulationWindow"
        SimulationWindow.Size = New Size(640, 640)
        SimulationWindow.TabIndex = 0
        SimulationWindow.TabStop = False
        ' 
        ' LearnButton
        ' 
        LearnButton.Font = New Font("Segoe UI", 12.0F)
        LearnButton.Location = New Point(14, 667)
        LearnButton.Margin = New Padding(4, 3, 4, 3)
        LearnButton.Name = "LearnButton"
        LearnButton.Size = New Size(137, 87)
        LearnButton.TabIndex = 1
        LearnButton.Text = "Learn! :D"
        LearnButton.UseVisualStyleBackColor = True
        ' 
        ' FunctionTypeBar
        ' 
        FunctionTypeBar.LargeChange = 1
        FunctionTypeBar.Location = New Point(493, 667)
        FunctionTypeBar.Margin = New Padding(4, 3, 4, 3)
        FunctionTypeBar.Maximum = 4
        FunctionTypeBar.Minimum = 1
        FunctionTypeBar.Name = "FunctionTypeBar"
        FunctionTypeBar.Size = New Size(163, 56)
        FunctionTypeBar.TabIndex = 3
        FunctionTypeBar.Value = 2
        ' 
        ' FunctionTypeLabel
        ' 
        FunctionTypeLabel.AutoSize = True
        FunctionTypeLabel.Location = New Point(546, 707)
        FunctionTypeLabel.Margin = New Padding(4, 0, 4, 0)
        FunctionTypeLabel.Name = "FunctionTypeLabel"
        FunctionTypeLabel.Size = New Size(46, 20)
        FunctionTypeLabel.TabIndex = 4
        FunctionTypeLabel.Text = "Curve"
        ' 
        ' NetworkConfigBox
        ' 
        NetworkConfigBox.Location = New Point(803, 187)
        NetworkConfigBox.Margin = New Padding(4, 3, 4, 3)
        NetworkConfigBox.Name = "NetworkConfigBox"
        NetworkConfigBox.Size = New Size(158, 27)
        NetworkConfigBox.TabIndex = 5
        NetworkConfigBox.TextAlign = HorizontalAlignment.Center
        ' 
        ' NetworkConfigButton
        ' 
        NetworkConfigButton.Font = New Font("Segoe UI", 10.8F)
        NetworkConfigButton.Location = New Point(689, 577)
        NetworkConfigButton.Margin = New Padding(4, 3, 4, 3)
        NetworkConfigButton.Name = "NetworkConfigButton"
        NetworkConfigButton.Size = New Size(256, 48)
        NetworkConfigButton.TabIndex = 6
        NetworkConfigButton.Text = "Configure Network"
        NetworkConfigButton.UseVisualStyleBackColor = True
        ' 
        ' ConfigLabel
        ' 
        ConfigLabel.AutoSize = True
        ConfigLabel.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold Or FontStyle.Underline)
        ConfigLabel.Location = New Point(673, 125)
        ConfigLabel.Margin = New Padding(4, 0, 4, 0)
        ConfigLabel.Name = "ConfigLabel"
        ConfigLabel.Size = New Size(236, 28)
        ConfigLabel.TabIndex = 7
        ConfigLabel.Text = "Network Configuration:"
        ' 
        ' StopButton
        ' 
        StopButton.Font = New Font("Segoe UI", 13.8F)
        StopButton.Location = New Point(14, 667)
        StopButton.Margin = New Padding(4, 3, 4, 3)
        StopButton.Name = "StopButton"
        StopButton.Size = New Size(137, 87)
        StopButton.TabIndex = 1
        StopButton.Text = "STOP!!"
        StopButton.UseVisualStyleBackColor = True
        StopButton.Visible = False
        ' 
        ' LearnRateBox
        ' 
        LearnRateBox.Location = New Point(803, 243)
        LearnRateBox.Margin = New Padding(4, 3, 4, 3)
        LearnRateBox.Name = "LearnRateBox"
        LearnRateBox.Size = New Size(91, 27)
        LearnRateBox.TabIndex = 5
        LearnRateBox.Text = "0.5"
        LearnRateBox.TextAlign = HorizontalAlignment.Center
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(693, 247)
        Label3.Margin = New Padding(4, 0, 4, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(82, 20)
        Label3.TabIndex = 4
        Label3.Text = "Learn Rate:"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(673, 192)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(99, 20)
        Label1.TabIndex = 8
        Label1.Text = "Network Size:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(673, 303)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(113, 20)
        Label2.TabIndex = 9
        Label2.Text = "Activation Func:"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(699, 360)
        Label4.Margin = New Padding(4, 0, 4, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(92, 20)
        Label4.TabIndex = 9
        Label4.Text = "Output Func:"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(669, 417)
        Label5.Margin = New Padding(4, 0, 4, 0)
        Label5.Name = "Label5"
        Label5.Size = New Size(101, 20)
        Label5.TabIndex = 9
        Label5.Text = "Cost Function:"
        ' 
        ' ActFuncOptions
        ' 
        ActFuncOptions.ImageScalingSize = New Size(20, 20)
        ActFuncOptions.Items.AddRange(New ToolStripItem() {SigmoidToolStripMenuItem, PerceptronToolStripMenuItem, TanhToolStripMenuItem, ReLUToolStripMenuItem, SiLUToolStripMenuItem, SoftmaxToolStripMenuItem})
        ActFuncOptions.Name = "ActFuncOptions"
        ActFuncOptions.Size = New Size(150, 148)
        ' 
        ' SigmoidToolStripMenuItem
        ' 
        SigmoidToolStripMenuItem.Name = "SigmoidToolStripMenuItem"
        SigmoidToolStripMenuItem.Size = New Size(149, 24)
        SigmoidToolStripMenuItem.Text = "Sigmoid"
        ' 
        ' PerceptronToolStripMenuItem
        ' 
        PerceptronToolStripMenuItem.Name = "PerceptronToolStripMenuItem"
        PerceptronToolStripMenuItem.Size = New Size(149, 24)
        PerceptronToolStripMenuItem.Text = "Perceptron"
        ' 
        ' TanhToolStripMenuItem
        ' 
        TanhToolStripMenuItem.Name = "TanhToolStripMenuItem"
        TanhToolStripMenuItem.Size = New Size(149, 24)
        TanhToolStripMenuItem.Text = "Tanh"
        ' 
        ' ReLUToolStripMenuItem
        ' 
        ReLUToolStripMenuItem.Name = "ReLUToolStripMenuItem"
        ReLUToolStripMenuItem.Size = New Size(149, 24)
        ReLUToolStripMenuItem.Text = "ReLU"
        ' 
        ' SiLUToolStripMenuItem
        ' 
        SiLUToolStripMenuItem.Name = "SiLUToolStripMenuItem"
        SiLUToolStripMenuItem.Size = New Size(149, 24)
        SiLUToolStripMenuItem.Text = "SiLU"
        ' 
        ' SoftmaxToolStripMenuItem
        ' 
        SoftmaxToolStripMenuItem.Name = "SoftmaxToolStripMenuItem"
        SoftmaxToolStripMenuItem.Size = New Size(149, 24)
        SoftmaxToolStripMenuItem.Text = "Softmax"
        ' 
        ' CostFuncOptions
        ' 
        CostFuncOptions.ImageScalingSize = New Size(20, 20)
        CostFuncOptions.Items.AddRange(New ToolStripItem() {MeanSquaredToolStripMenuItem, CrossEntropyToolStripMenuItem})
        CostFuncOptions.Name = "CostFuncOptions"
        CostFuncOptions.Size = New Size(171, 52)
        ' 
        ' MeanSquaredToolStripMenuItem
        ' 
        MeanSquaredToolStripMenuItem.Name = "MeanSquaredToolStripMenuItem"
        MeanSquaredToolStripMenuItem.Size = New Size(170, 24)
        MeanSquaredToolStripMenuItem.Text = "MeanSquared"
        ' 
        ' CrossEntropyToolStripMenuItem
        ' 
        CrossEntropyToolStripMenuItem.Name = "CrossEntropyToolStripMenuItem"
        CrossEntropyToolStripMenuItem.Size = New Size(170, 24)
        CrossEntropyToolStripMenuItem.Text = "CrossEntropy"
        ' 
        ' ActFuncButton
        ' 
        ActFuncButton.Location = New Point(821, 300)
        ActFuncButton.Margin = New Padding(4, 3, 4, 3)
        ActFuncButton.Name = "ActFuncButton"
        ActFuncButton.Size = New Size(140, 37)
        ActFuncButton.TabIndex = 12
        ActFuncButton.Text = "Sigmoid"
        ActFuncButton.TextAlign = ContentAlignment.MiddleLeft
        ActFuncButton.UseVisualStyleBackColor = True
        ' 
        ' CostFuncButton
        ' 
        CostFuncButton.Location = New Point(803, 412)
        CostFuncButton.Margin = New Padding(4, 3, 4, 3)
        CostFuncButton.Name = "CostFuncButton"
        CostFuncButton.Size = New Size(159, 37)
        CostFuncButton.TabIndex = 12
        CostFuncButton.Text = "MeanSquared"
        CostFuncButton.TextAlign = ContentAlignment.MiddleLeft
        CostFuncButton.UseVisualStyleBackColor = True
        ' 
        ' OutFuncOptions
        ' 
        OutFuncOptions.ImageScalingSize = New Size(20, 20)
        OutFuncOptions.Items.AddRange(New ToolStripItem() {OSigmoidToolStripMenuItem, OPerceptronToolStripMenuItem, OTanhToolStripMenuItem, OReLUToolStripMenuItem, OSiLUToolStripMenuItem, OSoftmaxToolStripMenuItem})
        OutFuncOptions.Name = "ActFuncOptions"
        OutFuncOptions.Size = New Size(150, 148)
        ' 
        ' OSigmoidToolStripMenuItem
        ' 
        OSigmoidToolStripMenuItem.Name = "OSigmoidToolStripMenuItem"
        OSigmoidToolStripMenuItem.Size = New Size(149, 24)
        OSigmoidToolStripMenuItem.Text = "Sigmoid"
        ' 
        ' OPerceptronToolStripMenuItem
        ' 
        OPerceptronToolStripMenuItem.Name = "OPerceptronToolStripMenuItem"
        OPerceptronToolStripMenuItem.Size = New Size(149, 24)
        OPerceptronToolStripMenuItem.Text = "Perceptron"
        ' 
        ' OTanhToolStripMenuItem
        ' 
        OTanhToolStripMenuItem.Name = "OTanhToolStripMenuItem"
        OTanhToolStripMenuItem.Size = New Size(149, 24)
        OTanhToolStripMenuItem.Text = "Tanh"
        ' 
        ' OReLUToolStripMenuItem
        ' 
        OReLUToolStripMenuItem.Name = "OReLUToolStripMenuItem"
        OReLUToolStripMenuItem.Size = New Size(149, 24)
        OReLUToolStripMenuItem.Text = "ReLU"
        ' 
        ' OSiLUToolStripMenuItem
        ' 
        OSiLUToolStripMenuItem.Name = "OSiLUToolStripMenuItem"
        OSiLUToolStripMenuItem.Size = New Size(149, 24)
        OSiLUToolStripMenuItem.Text = "SiLU"
        ' 
        ' OSoftmaxToolStripMenuItem
        ' 
        OSoftmaxToolStripMenuItem.Name = "OSoftmaxToolStripMenuItem"
        OSoftmaxToolStripMenuItem.Size = New Size(149, 24)
        OSoftmaxToolStripMenuItem.Text = "Softmax"
        ' 
        ' OutFuncButton
        ' 
        OutFuncButton.Location = New Point(821, 358)
        OutFuncButton.Margin = New Padding(4, 3, 4, 3)
        OutFuncButton.Name = "OutFuncButton"
        OutFuncButton.Size = New Size(140, 37)
        OutFuncButton.TabIndex = 13
        OutFuncButton.Text = "Sigmoid"
        OutFuncButton.TextAlign = ContentAlignment.MiddleLeft
        OutFuncButton.UseVisualStyleBackColor = True
        ' 
        ' SaveNetworkBtn
        ' 
        SaveNetworkBtn.Font = New Font("Segoe UI", 10.2F)
        SaveNetworkBtn.Location = New Point(689, 657)
        SaveNetworkBtn.Margin = New Padding(4, 3, 4, 3)
        SaveNetworkBtn.Name = "SaveNetworkBtn"
        SaveNetworkBtn.Size = New Size(117, 70)
        SaveNetworkBtn.TabIndex = 14
        SaveNetworkBtn.Text = "Save Network"
        SaveNetworkBtn.UseVisualStyleBackColor = True
        ' 
        ' LoadNetworkBtn
        ' 
        LoadNetworkBtn.Font = New Font("Segoe UI", 10.2F)
        LoadNetworkBtn.Location = New Point(827, 657)
        LoadNetworkBtn.Margin = New Padding(4, 3, 4, 3)
        LoadNetworkBtn.Name = "LoadNetworkBtn"
        LoadNetworkBtn.Size = New Size(117, 70)
        LoadNetworkBtn.TabIndex = 14
        LoadNetworkBtn.Text = "Load Network"
        LoadNetworkBtn.UseVisualStyleBackColor = True
        ' 
        ' ModeTrackbar
        ' 
        ModeTrackbar.LargeChange = 1
        ModeTrackbar.Location = New Point(684, 15)
        ModeTrackbar.Margin = New Padding(4, 3, 4, 3)
        ModeTrackbar.Maximum = 3
        ModeTrackbar.Minimum = 1
        ModeTrackbar.Name = "ModeTrackbar"
        ModeTrackbar.Size = New Size(273, 56)
        ModeTrackbar.TabIndex = 3
        ModeTrackbar.Value = 1
        ' 
        ' ModeLabel
        ' 
        ModeLabel.AutoSize = True
        ModeLabel.Location = New Point(789, 63)
        ModeLabel.Margin = New Padding(4, 0, 4, 0)
        ModeLabel.Name = "ModeLabel"
        ModeLabel.Size = New Size(55, 20)
        ModeLabel.TabIndex = 4
        ModeLabel.Text = "Graphs"
        ' 
        ' ClearButton
        ' 
        ClearButton.Location = New Point(517, 667)
        ClearButton.Margin = New Padding(4, 3, 4, 3)
        ClearButton.Name = "ClearButton"
        ClearButton.Size = New Size(117, 70)
        ClearButton.TabIndex = 15
        ClearButton.Text = "Clear Window"
        ClearButton.UseVisualStyleBackColor = True
        ClearButton.Visible = False
        ' 
        ' PredictionTable
        ' 
        PredictionTable.AutoSize = True
        PredictionTable.Font = New Font("Segoe UI", 10.2F, FontStyle.Bold)
        PredictionTable.Location = New Point(986, 132)
        PredictionTable.Margin = New Padding(4, 0, 4, 0)
        PredictionTable.Name = "PredictionTable"
        PredictionTable.Size = New Size(172, 23)
        PredictionTable.TabIndex = 2
        PredictionTable.Text = "Network Prediction:"
        PredictionTable.Visible = False
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(671, 473)
        Label6.Margin = New Padding(4, 0, 4, 0)
        Label6.Name = "Label6"
        Label6.Size = New Size(115, 20)
        Label6.TabIndex = 4
        Label6.Text = "Mini-Batch Size:"
        ' 
        ' MiniBatchSizeBox
        ' 
        MiniBatchSizeBox.Location = New Point(826, 467)
        MiniBatchSizeBox.Margin = New Padding(4, 3, 4, 3)
        MiniBatchSizeBox.Name = "MiniBatchSizeBox"
        MiniBatchSizeBox.Size = New Size(91, 27)
        MiniBatchSizeBox.TabIndex = 5
        MiniBatchSizeBox.Text = "500"
        MiniBatchSizeBox.TextAlign = HorizontalAlignment.Center
        ' 
        ' ImageChooserBar
        ' 
        ImageChooserBar.Location = New Point(166, 658)
        ImageChooserBar.Margin = New Padding(4, 3, 4, 3)
        ImageChooserBar.Maximum = 10000
        ImageChooserBar.Name = "ImageChooserBar"
        ImageChooserBar.Size = New Size(334, 56)
        ImageChooserBar.TabIndex = 16
        ImageChooserBar.Visible = False
        ' 
        ' ImageIndexLabel
        ' 
        ImageIndexLabel.AutoSize = True
        ImageIndexLabel.Location = New Point(240, 705)
        ImageIndexLabel.Margin = New Padding(4, 0, 4, 0)
        ImageIndexLabel.Name = "ImageIndexLabel"
        ImageIndexLabel.Size = New Size(146, 20)
        ImageIndexLabel.TabIndex = 17
        ImageIndexLabel.Text = "Current Image: None"
        ImageIndexLabel.TextAlign = ContentAlignment.MiddleCenter
        ImageIndexLabel.Visible = False
        ' 
        ' ImageLabel
        ' 
        ImageLabel.AutoSize = True
        ImageLabel.Location = New Point(254, 733)
        ImageLabel.Margin = New Padding(4, 0, 4, 0)
        ImageLabel.Name = "ImageLabel"
        ImageLabel.Size = New Size(134, 20)
        ImageLabel.TabIndex = 17
        ImageLabel.Text = "Image Label: None"
        ImageLabel.TextAlign = ContentAlignment.MiddleCenter
        ImageLabel.Visible = False
        ' 
        ' DiagnosticsButton
        ' 
        DiagnosticsButton.Font = New Font("Segoe UI", 9.0F)
        DiagnosticsButton.Location = New Point(1031, 22)
        DiagnosticsButton.Margin = New Padding(4, 3, 4, 3)
        DiagnosticsButton.Name = "DiagnosticsButton"
        DiagnosticsButton.Size = New Size(126, 67)
        DiagnosticsButton.TabIndex = 14
        DiagnosticsButton.Text = "Output Diagnostics"
        DiagnosticsButton.UseVisualStyleBackColor = True
        DiagnosticsButton.Visible = False
        ' 
        ' PressCLabel
        ' 
        PressCLabel.AutoSize = True
        PressCLabel.Font = New Font("Segoe UI", 7.8F)
        PressCLabel.Location = New Point(527, 737)
        PressCLabel.Margin = New Padding(4, 0, 4, 0)
        PressCLabel.Name = "PressCLabel"
        PressCLabel.Size = New Size(82, 17)
        PressCLabel.TabIndex = 17
        PressCLabel.Text = "(or Press 'C')"
        PressCLabel.TextAlign = ContentAlignment.MiddleCenter
        PressCLabel.Visible = False
        ' 
        ' Regu
        ' 
        Regu.AutoSize = True
        Regu.Location = New Point(680, 527)
        Regu.Margin = New Padding(4, 0, 4, 0)
        Regu.Name = "Regu"
        Regu.Size = New Size(107, 20)
        Regu.TabIndex = 4
        Regu.Text = "Regularisation:"
        ' 
        ' RegularisationBox
        ' 
        RegularisationBox.Location = New Point(826, 523)
        RegularisationBox.Margin = New Padding(4, 3, 4, 3)
        RegularisationBox.Name = "RegularisationBox"
        RegularisationBox.Size = New Size(91, 27)
        RegularisationBox.TabIndex = 5
        RegularisationBox.Text = "0.5"
        RegularisationBox.TextAlign = HorizontalAlignment.Center
        ' 
        ' NeuralNetworking
        ' 
        AutoScaleDimensions = New SizeF(120.0F, 120.0F)
        AutoScaleMode = AutoScaleMode.Dpi
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        ClientSize = New Size(1214, 773)
        Controls.Add(PressCLabel)
        Controls.Add(ImageLabel)
        Controls.Add(ImageIndexLabel)
        Controls.Add(ImageChooserBar)
        Controls.Add(ClearButton)
        Controls.Add(LoadNetworkBtn)
        Controls.Add(DiagnosticsButton)
        Controls.Add(SaveNetworkBtn)
        Controls.Add(OutFuncButton)
        Controls.Add(CostFuncButton)
        Controls.Add(ActFuncButton)
        Controls.Add(Label5)
        Controls.Add(Label4)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(ConfigLabel)
        Controls.Add(NetworkConfigButton)
        Controls.Add(MiniBatchSizeBox)
        Controls.Add(RegularisationBox)
        Controls.Add(LearnRateBox)
        Controls.Add(NetworkConfigBox)
        Controls.Add(Regu)
        Controls.Add(Label6)
        Controls.Add(Label3)
        Controls.Add(ModeLabel)
        Controls.Add(FunctionTypeLabel)
        Controls.Add(ModeTrackbar)
        Controls.Add(FunctionTypeBar)
        Controls.Add(PredictionTable)
        Controls.Add(StopButton)
        Controls.Add(LearnButton)
        Controls.Add(SimulationWindow)
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        Name = "NeuralNetworking"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Neural Networking"
        CType(SimulationWindow, ComponentModel.ISupportInitialize).EndInit()
        CType(FunctionTypeBar, ComponentModel.ISupportInitialize).EndInit()
        ActFuncOptions.ResumeLayout(False)
        CostFuncOptions.ResumeLayout(False)
        OutFuncOptions.ResumeLayout(False)
        CType(ModeTrackbar, ComponentModel.ISupportInitialize).EndInit()
        CType(ImageChooserBar, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents SimulationWindow As PictureBox
    Friend WithEvents LearnButton As Button
    Friend WithEvents FunctionTypeBar As TrackBar
    Friend WithEvents FunctionTypeLabel As Label
    Friend WithEvents NetworkConfigBox As TextBox
    Friend WithEvents NetworkConfigButton As Button
    Friend WithEvents ConfigLabel As Label
    Friend WithEvents StopButton As Button
    Friend WithEvents LearnRateBox As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents ActFuncOptions As ContextMenuStrip
    Friend WithEvents SigmoidToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TanhToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReLUToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SiLUToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SoftmaxToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CostFuncOptions As ContextMenuStrip
    Friend WithEvents MeanSquaredToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CrossEntropyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ActFuncButton As Button
    Friend WithEvents CostFuncButton As Button
    Friend WithEvents OutFuncOptions As ContextMenuStrip
    Friend WithEvents OSigmoidToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OTanhToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OReLUToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OSiLUToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OSoftmaxToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PerceptronToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OPerceptronToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OutFuncButton As Button
    Friend WithEvents SaveNetworkBtn As Button
    Friend WithEvents LoadNetworkBtn As Button
    Friend WithEvents ModeTrackbar As TrackBar
    Friend WithEvents ModeLabel As Label
    Friend WithEvents ClearButton As Button
    Friend WithEvents PredictionTable As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents MiniBatchSizeBox As TextBox
    Friend WithEvents ImageChooserBar As TrackBar
    Friend WithEvents ImageIndexLabel As Label
    Friend WithEvents ImageLabel As Label
    Friend WithEvents DiagnosticsButton As Button
    Friend WithEvents PressCLabel As Label
    Friend WithEvents Regu As Label
    Friend WithEvents RegularisationBox As TextBox
End Class
