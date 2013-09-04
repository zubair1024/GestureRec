Imports TouchlessLib

Public Class Form1

    Public Touchless As New TouchlessLib.TouchlessMgr
    Public Camera1 As TouchlessLib.Camera = Touchless.Cameras.ElementAt(0)
    Dim camw As Integer = 640
    Dim camh As Integer = 480
    Dim boximage As New Bitmap(camw, camh)
    Dim boxgfx As Graphics
    Dim mouselocation As Point = New Point(0, 0)
    Dim box1 As Rectangle
    Dim box2 As Rectangle
    Dim box1set As Boolean
    Dim box2set As Boolean
    Dim box1active As Boolean
    Dim box2active As Boolean
    Dim box1on As Boolean
    Dim box2on As Boolean
    Dim box1on2 As Boolean
    Dim box2on2 As Boolean
    Dim box1checksum As Integer
    Dim box2checksum As Integer
    Dim box1newchecksum As Integer
    Dim box2newchecksum As Integer
    Dim differencepercent As Integer = 20


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Touchless.CurrentCamera = Camera1

        Touchless.CurrentCamera.CaptureWidth = camw
        Touchless.CurrentCamera.CaptureHeight = camh
        PictureBox1.Size = New Size(camw, camh)
        Me.Size = New Size(camw, camh)
        PictureBox1.Location = New Point(0, 0)

    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick


        boximage = Touchless.CurrentCamera.GetCurrentImage
        If boximage IsNot Nothing Then
            boxgfx = Graphics.FromImage(boximage)


            If box1active = True Then
                boxgfx.DrawRectangle(New Pen(Brushes.Red, 2), New Rectangle(box1.X, box1.Y, mouselocation.X - box1.X, mouselocation.Y - box1.Y))
                box1checksum = 0
            End If
            If box2active = True Then
                boxgfx.DrawRectangle(New Pen(Brushes.Blue, 2), New Rectangle(box2.X, box2.Y, mouselocation.X - box2.X, mouselocation.Y - box2.Y))
                box2checksum = 0
            End If
            If box1set = True Then
                If box1on = True Then
                    boxgfx.DrawRectangle(New Pen(Brushes.Pink, 2), box1)
                Else
                    boxgfx.DrawRectangle(New Pen(Brushes.Red, 2), box1)
                End If
                box1newchecksum = 0
            End If
            If box2set = True Then
                If box2on = True Then
                    boxgfx.DrawRectangle(New Pen(Brushes.LightBlue, 2), box2)
                Else
                    boxgfx.DrawRectangle(New Pen(Brushes.Blue, 2), box2)
                End If
                box2newchecksum = 0
            End If


            imagediff()


            If box1set = True And box1newchecksum <> 0 Then
                Dim diff1 As Integer = (box1checksum / box1newchecksum) * 100
                boxgfx.DrawString(diff1.ToString & "%", SystemFonts.DefaultFont, Brushes.Red, box1.X, box1.Y)
                If diff1 > 100 + differencepercent Or diff1 < 100 - differencepercent Then
                    box1on = True
                Else
                    box1on = False
                End If
            End If
            If box2set = True And box2newchecksum <> 0 Then
                Dim diff2 As Integer = (box2checksum / box2newchecksum) * 100
                boxgfx.DrawString(diff2.ToString & "%", SystemFonts.DefaultFont, Brushes.Blue, box2.X, box2.Y)
                If diff2 > 100 + differencepercent Or diff2 < 100 - differencepercent Then
                    box2on = True
                Else
                    box2on = False
                End If
            End If


            If box1on = True And box1on2 = False Then
                box1on2 = True
                SendKeys.Send("{RIGHT}")
            End If
            If box2on = True And box2on2 = False Then
                box2on2 = True
                SendKeys.Send("{PGDN}")
            End If
            If box1on = False Then
                box1on2 = False
            End If
            If box2on = False Then
                box2on2 = False
            End If


            PictureBox1.Image = boximage
        End If

    End Sub


    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left And box2active = False Then
            box1set = False
            box1.X = e.X
            box1.Y = e.Y
            box1active = True
        End If
        If e.Button = Windows.Forms.MouseButtons.Right And box1active = False Then
            box2set = False
            box2.X = e.X
            box2.Y = e.Y
            box2active = True
        End If

    End Sub
    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        If box1active = True Then
            box1.Width = e.X - box1.X
            box1.Height = e.Y - box1.Y
            box1active = False
            box1set = True
        End If
        If box2active = True Then
            box2.Width = e.X - box2.X
            box2.Height = e.Y - box2.Y
            box2active = False
            box2set = True
        End If

    End Sub
    Private Sub PictureBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        mouselocation.X = e.X
        mouselocation.Y = e.Y
    End Sub

    Private Sub imagediff()
        Dim rect As New Rectangle(0, 0, camw, camh)
        Dim bmpData As System.Drawing.Imaging.BitmapData = boximage.LockBits(rect, _
            Drawing.Imaging.ImageLockMode.ReadWrite, boximage.PixelFormat)
        Dim ptr As IntPtr = bmpData.Scan0
        Dim bytes As Integer = bmpData.Stride * camh
        Dim rgbValues(bytes - 1) As Byte
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)

        Dim secondcounter As Integer
        Dim tempred As Integer
        Dim tempblue As Integer
        Dim tempgreen As Integer
        Dim tempalpha As Integer
        Dim tempx As Integer
        Dim tempy As Integer
        secondcounter = 0

        While secondcounter < rgbValues.Length
            tempblue = rgbValues(secondcounter)
            tempgreen = rgbValues(secondcounter + 1)
            tempred = rgbValues(secondcounter + 2)
            tempalpha = rgbValues(secondcounter + 3)
            tempalpha = 255

            tempy = ((secondcounter * 0.25) / camw)
            tempx = (secondcounter * 0.25) - (tempy * camw)
            If tempx < 0 Then
                tempx = tempx + camw
            End If

            If box1active = True Then
                If tempx >= box1.X And tempx <= mouselocation.X And tempy >= box1.Y And tempy <= mouselocation.Y Then
                    box1checksum = box1checksum + tempred
                    box1checksum = box1checksum + tempgreen
                    box1checksum = box1checksum + tempblue
                End If
            End If
            If box2active = True Then
                If tempx >= box2.X And tempx <= mouselocation.X And tempy >= box2.Y And tempy <= mouselocation.Y Then
                    box2checksum = box2checksum + tempred
                    box2checksum = box2checksum + tempgreen
                    box2checksum = box2checksum + tempblue
                End If
            End If
            If box1set = True Then
                If tempx >= box1.X And tempx <= (box1.X + box1.Width) And tempy >= box1.Y And tempy <= (box1.Y + box1.Height) Then
                    box1newchecksum = box1newchecksum + tempred
                    box1newchecksum = box1newchecksum + tempgreen
                    box1newchecksum = box1newchecksum + tempblue
                End If
            End If
            If box2set = True Then
                If tempx >= box2.X And tempx <= (box2.X + box2.Width) And tempy >= box2.Y And tempy <= (box2.Y + box2.Height) Then
                    box2newchecksum = box2newchecksum + tempred
                    box2newchecksum = box2newchecksum + tempgreen
                    box2newchecksum = box2newchecksum + tempblue
                End If
            End If

            rgbValues(secondcounter) = tempblue
            rgbValues(secondcounter + 1) = tempgreen
            rgbValues(secondcounter + 2) = tempred
            rgbValues(secondcounter + 3) = tempalpha

            secondcounter = secondcounter + 4
        End While

        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)


        boximage.UnlockBits(bmpData)
    End Sub

End Class