Imports System.Runtime.InteropServices

Public Class frmMain
    Private Declare Function record Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer

    Dim StartDate As String
    Dim IsRecording As Boolean = False
    Public Const MOD_SHIFT As Integer = &H4 'Shift key
    Public Const MOD_WIN As Integer = &H8 'Windows key
    Public Const WM_HOTKEY As Integer = &H312

    <DllImport("User32.dll")>
    Public Shared Function RegisterHotKey(ByVal hwnd As IntPtr,
                        ByVal id As Integer, ByVal fsModifiers As Integer,
                        ByVal vk As Integer) As Integer
    End Function

    <DllImport("User32.dll")>
    Public Shared Function UnregisterHotKey(ByVal hwnd As IntPtr,
                      ByVal id As Integer) As Integer
    End Function

    Private Sub StartRecording()
        StartDate = Date.Now.ToString("yyyy-MM-dd HH.mm.ss")
        btnRecord.Text = "Recording.."
        btnRecord.BackColor = Color.Red
        IsRecording = True
        record("open new Type waveaudio Alias recsound", "", 0, 0)
        record("record recsound", "", 0, 0)
    End Sub

    Private Sub StopRecording()
        IsRecording = False
        btnRecord.BackColor = Color.Empty
        btnRecord.Text = "Begin Dictation"
        record("save recsound """ & My.Settings.AudioPath & "\rec-" & StartDate & ".mp3""", "", 0, 0)
        record("close recsound", "", 0, 0)
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RegisterHotKey(Me.Handle, 100, MOD_WIN, Keys.CapsLock)
        RegisterHotKey(Me.Handle, 200, MOD_WIN Or MOD_SHIFT, Keys.CapsLock)

        My.Settings.AudioPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments

    End Sub

    Private Sub btnRecord_Click(sender As Object, e As EventArgs) Handles btnRecord.Click
        If Not IsRecording Then
            StartRecording()
        Else
            StopRecording()
        End If
    End Sub


    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_HOTKEY Then
            Dim id As IntPtr = m.WParam
            Select Case (id.ToString)
                Case "100"
                    StartRecording()
                Case "200"
                    StopRecording()
            End Select
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        UnregisterHotKey(Me.Handle, 100)
        UnregisterHotKey(Me.Handle, 200)
    End Sub

    Private Sub trayIcon_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles trayIcon.MouseDoubleClick
        If Me.Visible = False Then
            Me.Visible = True
        End If
    End Sub

    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Visible = False
        End If
    End Sub
End Class
