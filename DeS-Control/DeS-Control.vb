Imports PS3Lib


Public Class DeSCtrl


    Public Shared PS3 As New PS3API
    Public Shared api As String

    Dim lastEmber As Integer = 711

    'Dim CtrlPtr As UInteger = &H10F3A1A0& - TMAPI?
    Dim CtrlPtr As UInteger = &H10F3A160&
    Dim cllCMDs As String()
    Dim repeatCount As Integer


    Dim DoNotQuitPtr As UInteger = &H386001EC



    Dim storedCMD(5) As UInteger


    Private WithEvents refTimerPress As New System.Windows.Forms.Timer()
    Private WithEvents refTimerPlay As New System.Windows.Forms.Timer()
    Private WithEvents updTimer As New System.Windows.Forms.Timer()


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PS3.ChangeAPI(SelectAPI.TargetManager)

        wb.Navigate("http://www.twitch.tv/wulf2k/chat?popout=")
        REM wkb.Navigate("http://www.twitch.tv/wulf2k/chat?popout=")

        updTimer.Interval = 1000
        updTimer.Enabled = True
        updTimer.Start()
    End Sub
    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click



        If rbCCAPI.Checked Then
            PS3.ChangeAPI(SelectAPI.ControlConsole)

            If PS3.ConnectTarget(txtPS3IP.Text) Then
                'PS3.CCAPI.Notify(11, "Connected")
                If PS3.AttachProcess() Then
                    'PS3.CCAPI.Notify(11, "Attached")
                    txtPS3IP.Enabled = False
                Else
                    MsgBox("Failed to attach")
                End If
            Else
                MsgBox("No connection")
            End If


        Else
            PS3.ChangeAPI(SelectAPI.TargetManager)
            If PS3.TMAPI.ConnectTarget(txtPS3IP.Text) Then
                If PS3.AttachProcess() Then
                    txtPS3IP.Enabled = False
                Else
                    txtPS3IP.Enabled = True
                    REM MsgBox("Failed to attach")
                End If
            Else
                txtPS3IP.Enabled = True
                REM MsgBox("No TMAPI connection")
            End If
        End If
    End Sub


    Private Function getMemByteArray(ByRef loc As UInteger, ByVal count As Integer) As Byte()
        Dim byt(count - 1) As Byte
        PS3.GetMemory(loc, byt)
        Return byt
    End Function
    Private Function ReverseFourBytes(ByVal bytes() As Byte)
        Return {bytes(3), bytes(2), bytes(1), bytes(0)}
    End Function

    Private Sub Float2Four(ByVal loc As UInteger, val As Single)
        Dim byt(3) As Byte
        byt = ReverseFourBytes(BitConverter.GetBytes(val))
        PS3.SetMemory(loc, byt)
    End Sub
    Private Sub UInteger2Four(ByVal loc As UInteger, ByRef val As UInteger)
        Dim byt(3) As Byte
        byt(0) = Convert.ToByte((Math.Floor(val / &H1000000)) Mod &H100)
        byt(1) = Convert.ToByte((Math.Floor(val / &H10000)) Mod &H100)
        byt(2) = Convert.ToByte((Math.Floor(val / &H100)) Mod &H100)
        byt(3) = Convert.ToByte(val Mod &H100)
        PS3.SetMemory(loc, byt)
    End Sub
    Private Function Four2UInteger(ByVal loc As UInteger) As UInteger
        Dim bytes(3) As Byte
        bytes = getMemByteArray(loc, 4)
        Return BitConverter.ToUInt32(ReverseFourBytes(bytes), 0)
    End Function

    Private Sub btnTake_Click(sender As Object, e As EventArgs) Handles btnTake.Click
        UInteger2Four(&H2A6398, &H4954EA28&)

        UInteger2Four(&HC458C8, &H60000000&)
        UInteger2Four(&HC469D8, &H60000000&)
        UInteger2Four(&HC469DC, &H60000000&)
        UInteger2Four(&HC469D0, &H60000000&)
        UInteger2Four(&HC469D4, &H60000000&)

        UInteger2Four(&HC457E4, &HA00302F8&)
        UInteger2Four(&HC45860, &HA00302FA&)
        UInteger2Four(&HC4583C, &HA00302FA&)
        UInteger2Four(&HC4587C, &HA00302FA&)

        UInteger2Four(&H17F4DC0, &H482A639A&)
    End Sub
    Private Sub btnRestore_Click(sender As Object, e As EventArgs) Handles btnRestore.Click
        UInteger2Four(&H2A6398, &H60000000&)

        UInteger2Four(&HC458C8, &H7C234D2E&)
        UInteger2Four(&HC469D8, &HD17F03C0&)
        UInteger2Four(&HC469DC, &HD19F03C4&)
        UInteger2Four(&HC469D0, &HD01F03CC&)
        UInteger2Four(&HC469D4, &HD1BF03C8&)

        UInteger2Four(&HC457E4, &HA00302C8&)
        UInteger2Four(&HC45860, &HA00302CA&)
        UInteger2Four(&HC4583C, &HA00302CA&)
        UInteger2Four(&HC4587C, &HA00302CA&)
    End Sub

    Private Sub press(ByVal interval)

        refTimerPress.Interval = interval
        refTimerPress.Enabled = True
        refTimerPress.Start()
    End Sub
    Private Sub refTimerPress_Tick() Handles refTimerPress.Tick

        Dim buttons As UInteger
        buttons = 0

        If chkHoldO.Checked Then buttons = buttons Or &H20
        If chkHoldL1.Checked Then buttons = buttons Or &H4

        UInteger2Four(CtrlPtr + &H2F8, buttons)

        Float2Four(CtrlPtr + &H3C0, 0)
        Float2Four(CtrlPtr + &H3C4, 0)
        Float2Four(CtrlPtr + &H3C8, 0)
        Float2Four(CtrlPtr + &H3CC, 0)

        refTimerPress.Enabled = False

    End Sub
    Private Sub play(ByVal interval)
        refTimerPlay.Interval = interval
        refTimerPlay.Enabled = True
        refTimerPlay.Start()

        UInteger2Four(&H2A6398, &H60000000&)
    End Sub
    Private Sub refTimerPlay_Tick() Handles refTimerPlay.Tick
        refTimerPlay.Enabled = False
        If chkCMDPause.Checked = True Then UInteger2Four(&H2A6398, &H4954EA28&)
        btnConnect.PerformClick()
    End Sub

    Private Sub updTimer_Tick() Handles updTimer.Tick
        btnUpdate.PerformClick()
    End Sub


    Private Sub PS3Controller(buttons As UInteger, RLR As Single, RUD As Single, LLR As Single, LUD As Single, hold As Integer, unpause As Integer)
        If chkHoldO.Checked Then buttons = (buttons Or &H20)
        If chkHoldL1.Checked Then buttons = (buttons Or &H4)

        UInteger2Four(CtrlPtr + &H2F8, buttons)
        Float2Four(CtrlPtr + &H3C0, RLR)
        Float2Four(CtrlPtr + &H3C4, RUD)
        Float2Four(CtrlPtr + &H3C8, LLR)
        Float2Four(CtrlPtr + &H3CC, LUD)

        press(hold)
        play(unpause)

    End Sub

    Private Function parseEmber(ByVal txt As String) As Integer
        Dim ember = 0
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - 5)
        ember = Val(txt)
        Return ember
    End Function
    Private Function parseChat(ByVal txt As String) As String()
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - InStr(2, txt, Chr(13)))


        Return {Microsoft.VisualBasic.Left(txt, InStr(7, txt, ":") - 1), Microsoft.VisualBasic.Right(txt, txt.Length - InStr(7, txt, ":") - 1)}
    End Function
    Private Sub execCMD(cmd As String)

        REM PS3Controller ( buttons, R stick left/right, R stick up/down, L stick left/right, L stick up/down, _
        REM                 hold button length, unpause timer)

        Select Case cmd
            Case "wf"
                PS3Controller(0, 0, 0, 0, 1, 1000, 1000)
            Case "wl"
                PS3Controller(0, 0, 0, -1, 0, 1000, 1000)
            Case "wb"
                PS3Controller(0, 0, 0, 0, -1, 1000, 1000)
            Case "wr"
                PS3Controller(0, 0, 0, 1, 0, 1000, 1000)

            Case "wfl"
                PS3Controller(0, 0, 0, -1, 1, 1000, 1000)
            Case "wfr"
                PS3Controller(0, 0, 0, 1, 1, 1000, 1000)
            Case "wbl"
                PS3Controller(0, 0, 0, -1, -1, 1000, 1000)
            Case "wbr"
                PS3Controller(0, 0, 0, 1, -1, 1000, 1000)

            Case "rf"
                PS3Controller(&H20&, 0, 0, 0, 1, 300, 1500)
            Case "rl"
                PS3Controller(&H20&, 0, 0, -1, 0, 300, 1500)
            Case "rb"
                PS3Controller(&H20&, 0, 0, 0, -1, 300, 1500)
            Case "rr"
                PS3Controller(&H20&, 0, 0, 1, 0, 300, 1500)

            Case "lu"
                PS3Controller(0, 0, 1, 0, 0, 150, 150)
            Case "ll"
                PS3Controller(0, -1, 0, 0, 0, 150, 150)
            Case "lr"
                PS3Controller(0, 1, 0, 0, 0, 150, 150)
            Case "ld"
                PS3Controller(0, 0, -1, 0, 0, 150, 150)

            Case "du"
                PS3Controller(&H100000, 0, 0, 0, 0, 150, 500)
            Case "dd"
                PS3Controller(&H400000, 0, 0, 0, 0, 150, 500)
            Case "dl"
                PS3Controller(&H800000, 0, 0, 0, 0, 150, 1000)
            Case "dr"
                PS3Controller(&H200000, 0, 0, 0, 0, 150, 1000)

            Case "sel"
                PS3Controller(&H10000, 0, 0, 0, 0, 150, 500)
            Case "start"
                PS3Controller(&H80000, 0, 0, 0, 0, 150, 500)
            Case "tri"
                PS3Controller(&H10, 0, 0, 0, 0, 150, 500)
            Case "sq"
                PS3Controller(&H80, 0, 0, 0, 0, 150, 500)
            Case "o"
                PS3Controller(&H20, 0, 0, 0, 0, 150, 500)
            Case "x"
                REM Check if menu selected is Quit Game
                If Four2UInteger(DoNotQuitPtr) = &H4076 Then
                    PS3Controller(&H100000, 0, 0, 0, 0, 150, 500)
                Else
                    PS3Controller(&H40, 0, 0, 0, 0, 150, 500)
                End If

            Case "l2"
                PS3Controller(&H1, 0, 0, 0, 0, 150, 500)
            Case "l1"
                PS3Controller(&H4, 0, 0, 0, 0, 500, 500)
            Case "r2"
                PS3Controller(&H2, 0, 0, 0, 0, 150, 1000)
            Case "r1"
                PS3Controller(&H8, 0, 0, 0, 0, 150, 750)

            Case "l3"
                PS3Controller(&H20000, 0, 0, 0, 0, 150, 500)
            Case "r3"
                PS3Controller(&H40000, 0, 0, 0, 0, 150, 500)

            Case "holdo"
                chkHoldO.Checked = Not chkHoldO.Checked
                Dim buttons As UInteger
                buttons = Four2UInteger(CtrlPtr + &H2F8)
                If buttons And &H20 = &H20 Then
                    buttons -= &H20
                Else
                    buttons += &H20
                End If
                UInteger2Four(CtrlPtr + &H2F8, buttons)
            Case "l1t"
                chkHoldL1.Checked = Not chkHoldL1.Checked
                Dim buttons As UInteger
                buttons = Four2UInteger(CtrlPtr + &H2F8)
                If buttons And &H4 = &H4 Then
                    buttons -= &H4
                Else
                    buttons += &H4
                End If
                UInteger2Four(CtrlPtr + &H2F8, buttons)
            Case "h"
                PS3Controller(0, 0, 0, 0, 0, 10, 500)


            Case "pause"
                chkCMDPause.Checked = True
                UInteger2Four(&H2A6398, &H4954EA28&)
            Case "nopause"
                chkCMDPause.Checked = False
                UInteger2Four(&H2A6398, &H60000000&)
            Case "flong"
                PS3Controller(0, 0, 0, 0, 1, 4000, 4000)

            Case "reconnect"
                btnConnect.PerformClick()

            Case "wfx3"
                PS3Controller(0, 0, 0, 0, 1, 3000, 3000)
            Case "llx3"
                PS3Controller(0, -1, 0, 0, 0, 450, 450)
            Case "lrx3"
                PS3Controller(0, 1, 0, 0, 0, 450, 450)
            Case "hx2"
                PS3Controller(0, 0, 0, 0, 0, 10, 1000)
            Case "hx3"
                PS3Controller(0, 0, 0, 0, 0, 10, 1500)

            Case "hwf"
                PS3Controller(0, 0, 0, 0, 0.5, 1000, 1000)
            Case "hwl"
                PS3Controller(0, 0, 0, -0.5, 0, 1000, 1000)
            Case "hwb"
                PS3Controller(0, 0, 0, 0, -0.5, 1000, 1000)
            Case "hwr"
                PS3Controller(0, 0, 0, 0.5, 0, 1000, 1000)

            Case "fr1"
                PS3Controller(&H8, 0, 0, 0, 1, 250, 1000)


        End Select

    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        Dim Elems As HtmlElementCollection
        REM Dim Elems As WebKit.DOM.NodeList
        Dim ember As Integer

        Dim entry(2) As String

        Try
            Elems = wb.Document.GetElementsByTagName("li")
            REM Elems = wkb.Document.GetElementsByTagName("li")

            REM For Each elem As WebKit.DOM.Element In Elems
            For Each elem As HtmlElement In Elems


                If elem.GetAttribute("id").Contains("ember") Then

                    ember = parseEmber(elem.GetAttribute("id"))
                    If ember > lastEmber Then
                        lastEmber = ember

                        entry = parseChat(elem.InnerText)
                        REM entry = parseChat(elem.TextContent)
                        execCMD(entry(1))

                    End If
                End If


            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
End Class
