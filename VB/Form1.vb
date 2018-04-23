Imports Microsoft.VisualBasic
#Region "Usings"
Imports System
Imports System.IO
Imports System.Windows.Forms
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.Preview
Imports DevExpress.XtraPrinting.Drawing
#End Region

Namespace SaveRestoreWatermark
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub

		#Region "Internal Fields"
		Private Enum WatermarkStorage
			Registry
			XML
			Stream
		End Enum
		Private registryPath As String = "HKEY_CURRENT_USER\Software\MyCompany\MyTool\"
		Private xmlFile As String = "test.xml"
		Private stream As New MemoryStream()
		#End Region

		#Region "Prepare Example"
		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			Me.productsTableAdapter.Fill(Me.nwindDataSet.Products)

			radioGroup1.SelectedIndex = 0

			Dim wm As Watermark = printableComponentLink1.PrintingSystem.Watermark
			wm.Text = "Change the Watermark," & Constants.vbCrLf & "then close and re-open" & Constants.vbCrLf & "the form."
			wm.ShowBehind = False
			wm.TextDirection = DirectionMode.Horizontal

			SaveWatermark(printableComponentLink1, WatermarkStorage.Registry)
			SaveWatermark(printableComponentLink1, WatermarkStorage.Stream)
			SaveWatermark(printableComponentLink1, WatermarkStorage.XML)
		End Sub
		#End Region

		#Region "Get Watermark Storage"
		Private Function GetStorage() As WatermarkStorage
			Dim storage As WatermarkStorage = WatermarkStorage.Registry

			Select Case radioGroup1.SelectedIndex
				Case 0
						storage = WatermarkStorage.Registry
						Exit Select
				Case 1
						storage = WatermarkStorage.XML
						Exit Select
				Case 2
						storage = WatermarkStorage.Stream
						Exit Select
			End Select

			Return storage
		End Function
		#End Region

		Private Sub btnShowPreview_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnShowPreview.Click
			RestoreWatermark(printableComponentLink1, GetStorage())

			printableComponentLink1.CreateDocument()

			AddHandler printableComponentLink1.PrintingSystem.PreviewFormEx.FormClosed, AddressOf PreviewFormEx_FormClosed

			printableComponentLink1.ShowPreview()
		End Sub

		Private Sub PreviewFormEx_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs)
			SaveWatermark(printableComponentLink1, GetStorage())
		End Sub

		Private Sub RestoreWatermark(ByVal pcl As PrintableComponentLink, ByVal storage As WatermarkStorage)
			Select Case storage
				Case WatermarkStorage.Registry
						pcl.PrintingSystem.Watermark.RestoreFromRegistry(registryPath)
						Exit Select
				Case WatermarkStorage.XML
						If File.Exists(xmlFile) Then
							pcl.PrintingSystem.Watermark.RestoreFromXml(xmlFile)
						End If
						Exit Select
				Case WatermarkStorage.Stream
						pcl.PrintingSystem.Watermark.RestoreFromStream(stream)
						stream.Seek(0, SeekOrigin.Begin)
						Exit Select
			End Select
		End Sub

		Private Sub SaveWatermark(ByVal pcl As PrintableComponentLink, ByVal storage As WatermarkStorage)
			Select Case storage
				Case WatermarkStorage.Registry
						pcl.PrintingSystem.Watermark.SaveToRegistry(registryPath)
						Exit Select
				Case WatermarkStorage.XML
						pcl.PrintingSystem.Watermark.SaveToXml(xmlFile)
						Exit Select
				Case WatermarkStorage.Stream
						pcl.PrintingSystem.Watermark.SaveToStream(stream)
						stream.Seek(0, SeekOrigin.Begin)
						Exit Select
			End Select
		End Sub

	End Class
End Namespace