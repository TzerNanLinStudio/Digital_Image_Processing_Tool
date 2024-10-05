Imports System
Imports System.Windows.Forms
Imports Emgu.CV
Imports Emgu.CV.Structure

Namespace WindowsFormsApp
    Public Partial Class Form1
        Inherits Form

        Private bkGrayWhite As Gray = New Gray(255)
        Private img_Color As Image(Of Rgb, Byte)
        Private img_Gray As Image(Of Gray, Byte)
        Private img_SizeX As Integer
        Private img_SizeY As Integer
        Private img_Threshold As Image(Of Gray, Byte)
        Private img_Canny As Image(Of Gray, Byte)
        Private img_CannyforContours As Image(Of Gray, Byte)

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Private Sub Go_Click(ByVal sender As Object, ByVal e As EventArgs)
            If Me.RadioButton_Color.Checked Then
                img_Color = New Image(Of Rgb, Byte)("R:\Edge_1.bmp")
                Me.ImageBox1.Image = img_Color
                img_SizeX = img_Color.Width
                img_SizeY = img_Color.Height
            ElseIf Me.RadioButton_Gray.Checked Then
                img_Gray = New Image(Of Gray, Byte)("R:\31_1.bmp")
                Me.ImageBox1.Image = img_Gray
                img_SizeX = img_Gray.Width
                img_SizeY = img_Gray.Height
            End If

            img_Threshold = New Image(Of Gray, Byte)(img_SizeX, img_SizeY, bkGrayWhite)

            ' Dim imgresult As Image(Of Rgb, Byte) = New Image(Of Rgb, Byte)(img.Width, img.Height, New Rgb(255, 255, 255))
            If Me.RadioButton_Color.Checked Then
                img_Threshold = img_Color.Convert(Of Gray, Byte)()
            ElseIf Me.RadioButton_Gray.Checked Then
                img_Gray.CopyTo(img_Threshold)
            End If

            CvInvoke.AdaptiveThreshold(img_Threshold, img_Threshold, 255, CvEnum.AdaptiveThresholdType.MeanC, CvEnum.ThresholdType.Binary, 351, 10)
            img_Canny = New Image(Of Gray, Byte)(img_SizeX, img_SizeY, bkGrayWhite)
            CvInvoke.Canny(img_Threshold, img_Canny, Decimal.ToDouble(Me.NumericUpDown_CannyTh1.Value), Decimal.ToDouble(Me.NumericUpDown_CannyTh2.Value)) ' '25, 25 * 2, 3)
            img_CannyforContours = New Image(Of Gray, Byte)(img_SizeX, img_SizeY, bkGrayWhite)
            img_Canny.CopyTo(img_CannyforContours)
            Dim contours = New Util.VectorOfVectorOfPoint()
            Dim hierarchy As Mat = New Mat()
            CvInvoke.FindContours(img_CannyforContours, contours, hierarchy, CvEnum.RetrType.External, CvEnum.ChainApproxMethod.ChainApproxNone)
            Dim sumX As Long = 0
            Dim sumY As Long = 0

            For i As Integer = 0 To contours.Size - 1 ' section

                ' ''''''''''''''找大面積的
                Dim areaMax As Integer = img_SizeX * img_SizeY
                Dim area As Double = CvInvoke.ContourArea(contours(i))

                'if (area > 0) 
                '{ 
                '    UI_RichTextBox_SystemLog_Text("area = " + area); 
                '}
                ' 篩選輪廓面積大於三分之一整體圖片面積的輪廓
                If area < areaMax / 10000 Then
                    Continue For
                End If

                UI_RichTextBox_SystemLog_Text("Section:" & i & ", area = " & area)

                '畫在原始影像上
                If Me.RadioButton_Color.Checked Then
                    CvInvoke.DrawContours(img_Color, contours, i, New MCvScalar(0, 0, 0), 2, CvEnum.LineType.EightConnected, hierarchy, 2147483647)
                ElseIf Me.RadioButton_Gray.Checked Then
                    CvInvoke.DrawContours(img_Gray, contours, i, New MCvScalar(0, 0, 0), 2, CvEnum.LineType.EightConnected, hierarchy, 2147483647)
                End If

                '畫輪廓圖上
                CvInvoke.DrawContours(img_CannyforContours, contours, i, New MCvScalar(0, 0, 0), 2, CvEnum.LineType.EightConnected, hierarchy, 2147483647)

                'Chaincode
                For j As Integer = 0 To contours(i).Size - 1
                    sumX += contours(i)(j).X
                    sumY += contours(i)(j).Y
                    ' UI_RichTextBox_SystemLog_Text("X = " & contours(i)(j).X.ToString & ", Y = " & contours(i)(j).Y.ToString & ", sumX = " & sumX.ToString & ", sumY = " & sumY.ToString)
                Next

                UI_RichTextBox_SystemLog_Text("Meam_sumX = " & sumX / contours(i).Size & ", Meam_sumY = " & sumY / contours(i).Size)
                sumX = 0
                sumY = 0
                Threading.Thread.Sleep(0)
                Application.DoEvents()
                'MessageBox.Show("this ,next!");
            Next

            'UI_RichTextBox_SystemLog_Text("sumX = " + sumX.ToString() + ", sumY = " + sumY.ToString());

            ' ''''''''''''''找大面積的
            ' Dim areaMax As Integer = img.Width * img.Height
            ' For i = 0 To contours.Size - 1
            ' Dim area As Integer = CvInvoke.ContourArea(contours(i))

            ' '筛选轮廓面积大于三分之一整体图片面积的轮廓
            ' If area <areaMax / 3 Then
            ' Continue For
            ' End If
            ' CvInvoke.DrawContours(imgresult, contours, i, New MCvScalar(0, 0, 0), 2, CvEnum.LineType.EightConnected, hierarchy, 2147483647)
            ' Next

            ' imgresult.Save("R:\Edge_1_result.bmp")

        End Sub

        Private Sub Button_Save_Click(ByVal sender As Object, ByVal e As EventArgs)
            img_Threshold.Save("R:\Edge_1_Threshold.bmp")
            img_Canny.Save("R:\Edge_1_Canny.bmp")
        End Sub

        Private Sub RadioButton_Img_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If Me.RadioButton_Img.Checked Then
                UpdateImage()
            End If
        End Sub

        Private Sub RadioButton_ImgTH_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If Me.RadioButton_ImgTH.Checked Then
                UpdateImage()
            End If
        End Sub

        Private Sub RadioButton_ImgCanny_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If Me.RadioButton_ImgCanny.Checked Then
                UpdateImage()
            End If
        End Sub

        Private Sub UpdateImage()
            If Me.RadioButton_Img.Checked Then

                If Me.RadioButton_Color.Checked Then
                    Me.ImageBox1.Image = img_Color
                ElseIf Me.RadioButton_Gray.Checked Then
                    Me.ImageBox1.Image = img_Gray
                End If

                UI_RichTextBox_SystemLog_Text("Changing to image!")
            ElseIf Me.RadioButton_ImgTH.Checked Then
                Me.ImageBox1.Image = img_Threshold
                UI_RichTextBox_SystemLog_Text("Changing to image threshold!")
            ElseIf Me.RadioButton_ImgCanny.Checked Then
                Me.ImageBox1.Image = img_Canny
                UI_RichTextBox_SystemLog_Text("Changing to image canny!")
            End If
        End Sub

        Private Sub RichTextBox_SystemLog_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
            Me.RichTextBox_SystemLog.ScrollToCaret()
        End Sub

        Public Delegate Sub RichTextBox_SystemLog_TextCallback(ByVal result As String)

        Private Sub UI_RichTextBox_SystemLog_Text(ByVal result As String)
            If Me.RichTextBox_SystemLog.InvokeRequired Then
                'this.Invoke(new RichTextBox_SystemLog_TextCallback(new System.EventHandler(this.UI_RichTextBox_SystemLog_Text)), new object[] {result});
                Dim d As RichTextBox_SystemLog_TextCallback = New RichTextBox_SystemLog_TextCallback(AddressOf UI_RichTextBox_SystemLog_Text)
                Invoke(d, New Object() {result})
            Else
                Me.RichTextBox_SystemLog.AppendText(result & Microsoft.VisualBasic.Constants.vbCrLf)
                Update()
            End If
        End Sub

        Private Sub NumericUpDown_CannyTh1_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
            If img_Threshold Is Nothing Or img_Canny Is Nothing Then

                If True Then

                    If TypeOf img_Threshold Is Object Or TypeOf img_Canny Is Object Then
                        CvInvoke.Canny(img_Threshold, img_Canny, Decimal.ToDouble(Me.NumericUpDown_CannyTh1.Value), Decimal.ToDouble(Me.NumericUpDown_CannyTh2.Value)) ' '25, 25 * 2, 3)
                        UpdateImage()
                    End If
                End If
            End If
        End Sub
    End Class
End Namespace
