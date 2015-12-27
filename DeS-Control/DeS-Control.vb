Imports PS3Lib

'The following code needs to be active in the Demon's Souls EBOOT.BIN

'       At 0x2a63a4:
'bl 0x170d588	49 46 71 E5
'----	
'       At 0x170d588
'stdu %r1, -0x40(%r1)	F8 21 FF C1
'std %r3, 0x8(%r1)	F8 61 00 08 
'std %r4, 0x10(%r1)	F8 81 00 10 
'std %r5, 0x18(%r1)	F8 A1 00 18 
'lis %r3, 0x170	3C 60 01 70 
'ori %r3, %r3, 0xd580	60 63 D5 80 
'
'lwz %r4, 0x0(%r3)	80 83 00 00 
'lwz %r5, 0x4(%r3)	80 A3 00 04 
'addi %r5, %r5, -0x1	38 A5 FF FF 
'
'cmpwi cr7, %r5, -0x1	2F 85 FF FF
'beq cr7, 0x8	41 9E 00 08 
'stw %r5, 0x4(%r3)	90 A3 00 04 
'
'cmpwi cr7, %r4, 0x0	2F 84 00 00
'beq cr7, 0xC	41 9E 00 0C 
'
'cmpwi cr7 %r5, 0x0	2F 85 FF FF
'beq cr7, %r5, -0x24	41 9E FF DC
'
'ld %r5, 0x18(%r1)	E8 A1 00 18 
'ld %r4, 0x10(%r1)	E8 81 00 10 
'ld %r3, 0x8(%r1)	E8 61 00 08 
'addi %r1, %r1, 0x40	38 21 00 40 
'blr	4E 80 00 20 




Public Class DeSCtrl


    Public Shared PS3 As New PS3API
    Public Shared api As String

    Dim lastEmber As Integer = 711

    'Dim CtrlPtr As UInteger = &H10F3A1A0& - TMAPI?
    'Dim CtrlPtr As UInteger = &H10F3A160& - CCAPI?
    Dim CtrlPtr As UInteger = &H10F3A1A0&
    Dim repeatCount As Integer


    Dim collectVotes As Boolean
    Dim lastVoter As String = ""

    Dim DoNotQuitPtr As UInteger = &H386001EC

    Dim pauseModePtr As UInteger = &H170D580&
    Dim framePtr As UInteger = &H170D584&

    Dim QueuedInput As New List(Of QdInput)
    Dim votes As New List(Of Votes)
    Dim voteTimer As Integer = 15000

    Dim cllModNames As New List(Of String)

    Private WithEvents refTimerVote As New System.Windows.Forms.Timer()
    Private WithEvents refTimerPress As New System.Windows.Forms.Timer()
    Private WithEvents refTimerPlay As New System.Windows.Forms.Timer()
    Private WithEvents refTimerPost As New System.Windows.Forms.Timer()
    Private WithEvents updTimer As New System.Windows.Forms.Timer()


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PS3.ChangeAPI(SelectAPI.TargetManager)

        'wb.Navigate("http://www.twitch.tv/wulf2k/chat")
        wb.Navigate("www.twitch.tv/twitchplaysdark/chat")

        cllModNames.Add("Wulf2k")
        cllModNames.Add("wulf2kbot")

        refTimerPress.Interval = 500
        refTimerPress.Enabled = True
        refTimerPress.Start()

        updTimer.Interval = 1000
        updTimer.Enabled = True
        updTimer.Start()
    End Sub
    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        If rbCCAPI.Checked Then
            PS3.ChangeAPI(SelectAPI.ControlConsole)
            refTimerPress.Interval = 500

            If PS3.ConnectTarget(txtPS3IP.Text) Then
                If PS3.AttachProcess() Then
                    txtPS3IP.Enabled = False
                Else
                    txtChat.Text += "Failed to attach" & Environment.NewLine
                End If
            Else
                txtChat.Text += "No connection" & Environment.NewLine
            End If
        Else
            PS3.ChangeAPI(SelectAPI.TargetManager)
            refTimerPress.Interval = 50
            If PS3.TMAPI.ConnectTarget(txtPS3IP.Text) Then
                If PS3.AttachProcess() Then
                    txtPS3IP.Enabled = False
                Else
                    txtPS3IP.Enabled = True
                End If
            Else
                txtPS3IP.Enabled = True
            End If
        End If
    End Sub

    Private Sub PushQ(ByRef buttons As UInteger, RStickLR As Single, RStickUD As Single, LStickLR As Single, _
                      LStickUD As Single, time As UInteger)
        QueuedInput.Add(New QdInput() With {.buttons = buttons, .RstickLR = RStickLR, .RstickUD = RStickUD, _
                                            .LStickLR = LStickLR, .LStickUD = LStickUD, .time = time})
    End Sub
    Private Sub PopQ()
        QueuedInput.RemoveAt(0)
    End Sub

    Private Sub pause(ByVal pauseMode As Boolean)
        If pauseMode Then
            UInteger2Four(pauseModePtr, 1)
        Else
            UInteger2Four(pauseModePtr, 0)
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
        chkCMDPause.Checked = True
        UInteger2Four(&HC458C8, &H60000000&)
        UInteger2Four(&HC469D8, &H60000000&)
        UInteger2Four(&HC469DC, &H60000000&)
        UInteger2Four(&HC469D0, &H60000000&)
        UInteger2Four(&HC469D4, &H60000000&)

        UInteger2Four(&HC457E4, &HA00302F8&)
        UInteger2Four(&HC45860, &HA00302FA&)
        UInteger2Four(&HC4583C, &HA00302FA&)
        UInteger2Four(&HC4587C, &HA00302FA&)
    End Sub
    Private Sub btnRestore_Click(sender As Object, e As EventArgs) Handles btnRestore.Click
        chkCMDPause.Checked = False
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

    Private Sub press()
        If Four2UInteger(framePtr) = 0 Then
            If QueuedInput.Count > 0 Then
                UInteger2Four(CtrlPtr + &H2F8, QueuedInput(0).buttons)
                Float2Four(CtrlPtr + &H3C0, QueuedInput(0).RstickLR)
                Float2Four(CtrlPtr + &H3C4, QueuedInput(0).RstickUD)
                Float2Four(CtrlPtr + &H3C8, QueuedInput(0).LStickLR)
                Float2Four(CtrlPtr + &H3CC, QueuedInput(0).LStickUD)
                UInteger2Four(framePtr, QueuedInput(0).time)

                PopQ()
            Else
                Dim buttons = 0
                If chkHoldX.Checked Then buttons = (buttons Or &H40)
                If chkHoldO.Checked Then buttons = (buttons Or &H20)
                If chkHoldL1.Checked Then buttons = (buttons Or &H4)

                UInteger2Four(CtrlPtr + &H2F8, buttons)
                Float2Four(CtrlPtr + &H3C0, 0)
                Float2Four(CtrlPtr + &H3C4, 0)
                Float2Four(CtrlPtr + &H3C8, 0)
                Float2Four(CtrlPtr + &H3CC, 0)
            End If
        End If
    End Sub
    Private Sub refTimerPress_Tick() Handles refTimerPress.Tick
        press()
    End Sub
    Private Sub play()
        refTimerPlay.Interval = 5000
        refTimerPlay.Enabled = True
        refTimerPlay.Start()
    End Sub
    Private Sub refTimerPlay_Tick() Handles refTimerPlay.Tick
        btnConnect.PerformClick()
        If QueuedInput.Count > 0 Then
            press()
        Else
            refTimerPlay.Enabled = False
        End If
    End Sub
    Private Sub refTimerVote_Tick() Handles refTimerVote.Tick
        collectVotes = Not collectVotes

        If collectVotes Then

            Dim outputtext As String
            outputtext = "Collecting votes for " & voteTimer / 1000 & "s."

            If chkHoldL1.Checked Or chkHoldO.Checked Or chkHoldX.Checked Then
                outputtext = outputtext & "  Currently holding:  X - " & chkHoldX.Checked & ". L1 = " & chkHoldL1.Checked & ". O = " & chkHoldO.Checked & "."
            End If

            refTimerVote.Interval = voteTimer
            outputChat(outputtext)
        Else

            Dim queuetime As UInteger = 0
            For i = 0 To votes.Count - 1
                If votes.Item(i).command = "flong" Then
                    queuetime += 4000
                Else
                    queuetime += 1000
                End If
            Next

            refTimerVote.Interval = 15000 + queuetime

            TallyVotes()
        End If
    End Sub
    Private Sub refTimerPost_Tick() Handles refTimerPost.Tick
        Dim Elems As HtmlElementCollection
        Dim elem As HtmlElement

        Try
            Elems = wb.Document.GetElementsByTagName("button")
            elem = Elems(0)
            elem.InvokeMember("click")
        Catch ex As Exception
            txtChat.Text += ex.Message & Environment.NewLine
        End Try

        refTimerPost.Stop()
    End Sub

    Private Sub TallyVotes()
        Dim cllCll As New List(Of String())

        Dim cllMoveCMD As String()
        Dim cllRollCMD As String()
        Dim cllCamCMD As String()
        Dim cllBtnCMD As String()
        Dim cllCombatCMD As String()
        Dim cllToggleCMD As String()
        Dim cllSysCMD As String()

        Dim voteCat(7) As Integer
        Dim voteTally As Integer
        Dim voteWin As Integer

        Dim subvoteTally As Integer
        Dim subvoteWin As Integer


        Dim catVotes(7) As List(Of Votes)
        Dim subcatVotes As New List(Of Votes)

        For i = 0 To 6
            catVotes(i) = New List(Of Votes)
        Next

        If votes.Count > 0 Then
            cllMoveCMD = {"wf", "wl", "wb", "wr", "wfl", "wfr", "wbl", "wbr", "flong", "hwf", "hwl", "hwr", "hwb", _
                          "hwfl", "hwfr", "hwbl", "hwbr"}
            cllRollCMD = {"rf", "rl", "rb", "rr"}
            cllCamCMD = {"lu", "ll", "lr", "ld", "r3", "hlu", "hll", "hlr", "hld"}
            cllBtnCMD = {"sq", "tri", "o", "x", "du", "dd", "dl", "dr", "start", "l3", "sel"}
            cllCombatCMD = {"l1", "l2", "r1", "r2", "fr1", "h", "hh", "fa"}
            cllToggleCMD = {"holdo", "holdx", "holdl1"}
            cllSysCMD = {"pause", "nopause", "votemode", "novotemode"}

            cllCll.Add(cllMoveCMD)
            cllCll.Add(cllRollCMD)
            cllCll.Add(cllCamCMD)
            cllCll.Add(cllBtnCMD)
            cllCll.Add(cllCombatCMD)
            cllCll.Add(cllToggleCMD)
            cllCll.Add(cllSysCMD)


            REM Find winning category of votes
            For i = 0 To votes.Count - 1
                For j = 0 To cllCll.Count - 1
                    If cllCll.Item(j).Contains(votes.Item(i).command) Then
                        voteCat(j) += 1
                        catVotes(j).Add(votes.Item(i))
                    End If
                Next
            Next

            For i = 0 To 6
                If voteCat(i) > voteTally Then
                    voteWin = i
                    voteTally = voteCat(i)
                End If
            Next


            REM Work with reduced set of votes here from winning category
            ReDim voteCat(cllCll.Item(voteWin).Count)

            For i = 0 To catVotes(voteWin).Count - 1
                voteCat(Array.IndexOf(cllCll.Item(voteWin), catVotes(voteWin).Item(i).command)) += 1
            Next

            For i = 0 To voteCat.Count - 1
                If voteCat(i) > subvoteTally Then
                    subvoteWin = i
                    subvoteTally = voteCat(i)
                End If
            Next

            For i = 0 To votes.Count - 1
                If votes.Item(i).command = cllCll.Item(voteWin)(subvoteWin) Then
                    subcatVotes.Add(votes.Item(i))
                End If
            Next

            subcatVotes.Sort(Function(x, y) x.cmdmulti.CompareTo(y.cmdmulti))

            outputChat("Winning command: " & subcatVotes.Item(0).command & " x" & _
                       subcatVotes.Item(subcatVotes.Count / 2).cmdmulti)

            For i = 0 To subcatVotes.Item(subcatVotes.Count / 2 - 1).cmdmulti - 1
                execCMD(subcatVotes.Item(0).command)
            Next

        Else
            outputChat("No votes.")
            refTimerVote.Interval = 1000
        End If

        votes.Clear()
    End Sub

    Private Sub updTimer_Tick() Handles updTimer.Tick
        btnUpdate.PerformClick()
    End Sub
    Private Sub PS3Controller(buttons As UInteger, RLR As Single, RUD As Single, LLR As Single, LUD As Single, hold As Integer)
        If chkHoldX.Checked Then buttons = (buttons Or &H40)
        If chkHoldO.Checked Then buttons = (buttons Or &H20)
        If chkHoldL1.Checked Then buttons = (buttons Or &H4)
        PushQ(buttons, RLR, RUD, LLR, LUD, hold)

        'If refTimerPress.Enabled = False Then refTimerPress.Enabled = True

    End Sub

    Private Function parseEmber(ByVal txt As String) As Integer
        Dim ember = 0
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - 5)
        ember = Val(txt)
        Return ember
    End Function
    Private Function parseChat(ByVal txt As String) As String()
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - InStr(2, txt, Chr(13)))
        If Asc(txt(0)) = 10 Then txt = Microsoft.VisualBasic.Right(txt, txt.Length - 1)

        Dim username As String
        username = Microsoft.VisualBasic.Left(txt, InStr(1, txt, ":") - 1)

        If txt.Split(":").Length > 2 Then
            txt = Microsoft.VisualBasic.Right(txt, txt.Length - InStr(2, txt, " "))
        End If

        If Microsoft.VisualBasic.Left(txt, 11) = "BanTimeout " Then txt = Microsoft.VisualBasic.Right(txt, txt.Length - 11)

        If txtChat.Text.Length > 1000 Then txtChat.Text = ""
        txtChat.Text += txt & Environment.NewLine



        Return {Microsoft.VisualBasic.Left(txt, InStr(1, txt, ":") - 1).ToLower, Microsoft.VisualBasic.Right(txt, txt.Length - InStr(1, txt, ":") - 1).ToLower}
    End Function
    Private Sub outputChat(ByVal txt As String)
        wb.Document.GetElementById("ember651").InnerText = txt

        refTimerPost.Interval = 100
        refTimerPost.Enabled = True
        refTimerPost.Start()
    End Sub

    Private Sub ProcessCMD(entry() As String)

        Dim CllCMDList As String()

        CllCMDList = {"wf", "wl", "wb", "wr", "wfl", "wfr", "wbl", "wbr", "flong", _
                        "hwf", "hwl", "hwr", "hwb", _
                        "hwfl", "hwfr", "hwbl", "hwbr", _
                        "rf", "rl", "rb", "rr", _
                        "lu", "ll", "lr", "ld", "r3", _
                        "hlu", "hll", "hlr", "hld", _
                        "du", "dd", "dl", "dr", _
                        "sel", "start", "tri", "sq", "o", "x", "l3", _
                        "l2", "l1", "r2", "r1", "h", "hh", "fa", "fr1", _
                        "holdo", "holdx", "holdl1", _
                        "pause", "nopause", "votemode", "novotemode", "delaydn", "delayup"}

        Dim tmpuser = entry(0)
        Dim tmpcmd = entry(1)
        Dim CMDmulti As Integer = 1
        Dim currvote As New Votes

        If tmpcmd.Length > 2 Then


            If IsNumeric(tmpcmd(tmpcmd.Length - 1)) And tmpcmd(tmpcmd.Length - 2) = "x" Then
                CMDmulti = Val(tmpcmd(tmpcmd.Length - 1))
                tmpcmd = Microsoft.VisualBasic.Left(tmpcmd, tmpcmd.Length - 2)
            End If
        End If

        If CMDmulti > 5 Then
            CMDmulti = 5
        End If

        Select Case tmpcmd
            Case "flong"
                If CMDmulti > 2 Then CMDmulti = 2
            Case "sq"
                If CMDmulti > 2 Then CMDmulti = 2
        End Select


        If CllCMDList.Contains(tmpcmd) Then

            If (chkVoting.Checked = False) Or tmpcmd = "delaydn" Or tmpcmd = "delayup" Then
                'If Not (tmpcmd = "delaydn" Or tmpcmd = "delayup") Then
                ' If Not tmpuser = lastVoter Then
                'outputChat("Change in operator detected.  Entering vote mode automatically.")
                'chkVoting.Checked = Not chkVoting.Checked
                'Else
                For i = 0 To CMDmulti - 1
                    If QueuedInput.Count < 50 Then execCMD(tmpcmd)
                Next
                'End If
            End If
            'End If
            If chkVoting.Checked = True Then
                If collectVotes Then
                    currvote = votes.Find(Function(v As Votes) v.username = tmpuser)

                    If currvote IsNot Nothing Then
                        votes.Remove(currvote)
                    End If

                    currvote = New Votes
                    currvote.username = tmpuser
                    currvote.command = tmpcmd
                    currvote.cmdmulti = CMDmulti

                    votes.Add(currvote)

                    lastVoter = tmpuser
                End If
            End If
        End If

    End Sub

    Private Sub execCMD(cmd As String)

        REM PS3Controller ( buttons, R stick left/right, R stick up/down, L stick left/right, L stick up/down, _
        REM                 hold button length)

        Select Case cmd
            Case "wf"
                PS3Controller(0, 0, 0, 0, 1, 30)
            Case "wl"
                PS3Controller(0, 0, 0, -1, 0, 30)
            Case "wb"
                PS3Controller(0, 0, 0, 0, -1, 30)
            Case "wr"
                PS3Controller(0, 0, 0, 1, 0, 30)

            Case "wfl"
                PS3Controller(0, 0, 0, -1, 1, 30)
            Case "wfr"
                PS3Controller(0, 0, 0, 1, 1, 30)
            Case "wbl"
                PS3Controller(0, 0, 0, -1, -1, 30)
            Case "wbr"
                PS3Controller(0, 0, 0, 1, -1, 30)

            Case "hwf"
                PS3Controller(0, 0, 0, 0, 0.5, 30)
            Case "hwl"
                PS3Controller(0, 0, 0, -0.5, 0, 30)
            Case "hwb"
                PS3Controller(0, 0, 0, 0, -0.5, 30)
            Case "hwr"
                PS3Controller(0, 0, 0, 0.5, 0, 30)

            Case "hwfl"
                PS3Controller(0, 0, 0, -0.5, 0.5, 30)
            Case "hwfr"
                PS3Controller(0, 0, 0, 0.5, 0.5, 30)
            Case "hwbl"
                PS3Controller(0, 0, 0, -0.5, -0.5, 30)
            Case "hwbr"
                PS3Controller(0, 0, 0, 0.5, -0.5, 30)

            Case "flong"
                PS3Controller(0, 0, 0, 0, 1, 120)

            Case "rf"
                PS3Controller(0, 0, 0, 0, 1, 4)
                PS3Controller(&H20&, 0, 0, 0, 1, 2)
                PS3Controller(0, 0, 0, 0, 0, 18)
            Case "rl"
                PS3Controller(0, 0, 0, -1, 0, 4)
                PS3Controller(&H20&, 0, 0, -1, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 20)
            Case "rb"
                PS3Controller(0, 0, 0, 0, -1, 4)
                PS3Controller(&H20&, 0, 0, 0, -1, 2)
                PS3Controller(0, 0, 0, 0, 0, 18)
            Case "rr"
                PS3Controller(0, 0, 0, 1, 0, 4)
                PS3Controller(&H20&, 0, 0, 1, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 20)

            Case "lu"
                PS3Controller(0, 0, 1, 0, 0, 4)
            Case "ll"
                PS3Controller(0, -1, 0, 0, 0, 4)
            Case "lr"
                PS3Controller(0, 1, 0, 0, 0, 4)
            Case "ld"
                PS3Controller(0, 0, -1, 0, 0, 4)

            Case "hlu"
                PS3Controller(0, 0, 0.5, 0, 0, 2)
            Case "hll"
                PS3Controller(0, -0.5, 0, 0, 0, 2)
            Case "hlr"
                PS3Controller(0, 0.5, 0, 0, 0, 2)
            Case "hld"
                PS3Controller(0, 0, -0.5, 0, 0, 2)

            Case "du"
                PS3Controller(&H100000, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 2)
            Case "dd"
                PS3Controller(&H400000, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 2)
            Case "dl"
                PS3Controller(&H800000, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 22)
            Case "dr"
                PS3Controller(&H200000, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 22)

            Case "sel"
                PS3Controller(&H10000, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 2)
            Case "start"
                PS3Controller(&H80000, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 8)

            Case "tri"
                PS3Controller(&H10, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 22)
            Case "sq"
                PS3Controller(&H80, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 58)
            Case "o"
                PS3Controller(&H20, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 16)
            Case "x"
                REM Check if menu selected is Quit Game
                If Four2UInteger(DoNotQuitPtr) = &H4076 Then
                    PS3Controller(&H100000, 0, 0, 0, 0, 2)
                    PS3Controller(0, 0, 0, 0, 0, 16)
                    outputChat("Aborting attempt to quit game.")
                Else
                    PS3Controller(&H40, 0, 0, 0, 0, 2)
                    PS3Controller(0, 0, 0, 0, 0, 16)
                End If

            Case "l2"
                PS3Controller(&H1, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 20)
            Case "l1"
                PS3Controller(&H4, 0, 0, 0, 0, 30)
            Case "r2"
                PS3Controller(&H2, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 28)
            Case "r1"
                PS3Controller(&H8, 0, 0, 0, 0, 2)
                PS3Controller(0, 0, 0, 0, 0, 28)

            Case "fr1"
                PS3Controller(&H8, 0, 0, 0, 1, 2)
                PS3Controller(0, 0, 0, 0, 0, 48)

            Case "l3"
                PS3Controller(&H20000, 0, 0, 0, 0, 2)
            Case "r3"
                PS3Controller(&H40000, 0, 0, 0, 0, 2)
                PS3Controller(&H40000, 0, 0, 0, 0, 4)

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
                refTimerVote.Interval = 1000
            Case "holdx"
                chkHoldX.Checked = Not chkHoldX.Checked
                Dim buttons As UInteger
                buttons = Four2UInteger(CtrlPtr + &H2F8)
                If buttons And &H40 = &H40 Then
                    buttons -= &H40
                Else
                    buttons += &H40
                End If
                UInteger2Four(CtrlPtr + &H2F8, buttons)
                refTimerVote.Interval = 1000
            Case "holdl1"
                chkHoldL1.Checked = Not chkHoldL1.Checked
                Dim buttons As UInteger
                buttons = Four2UInteger(CtrlPtr + &H2F8)
                If buttons And &H4 = &H4 Then
                    buttons -= &H4
                Else
                    buttons += &H4
                End If
                UInteger2Four(CtrlPtr + &H2F8, buttons)
                refTimerVote.Interval = 1000
            Case "h"
                PS3Controller(0, 0, 0, 0, 0, 16)
            Case "hh"
                PS3Controller(0, 0, 0, 0, 0, 8)

            Case "fa"
                PS3Controller(0, 0, 0, 0, 0, 2)

            Case "pause"
                chkCMDPause.Checked = True
                'UInteger2Four(&H2A63A4, &H4954EA1C&)
                refTimerVote.Interval = 1000
            Case "nopause"
                chkCMDPause.Checked = False
                'UInteger2Four(&H2A63A4, &H60000000&)
                refTimerVote.Interval = 1000

            Case "votemode"
                chkVoting.Checked = True
                refTimerVote.Interval = 1000
            Case "novotemode"
                chkVoting.Checked = False
                refTimerVote.Interval = 1000

            Case "delaydn"
                If voteTimer > 10000 Then voteTimer -= 1000
            Case "delayup"
                If voteTimer < 60000 Then voteTimer += 1000
        End Select
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

        Dim Elems As HtmlElementCollection
        Dim ember As Integer

        Dim entry(2) As String

        Try
            Elems = wb.Document.GetElementsByTagName("li")
            For Each elem As HtmlElement In Elems
                If elem.GetAttribute("id").Contains("ember") Then
                    ember = parseEmber(elem.GetAttribute("id"))
                    If ember > lastEmber Then
                        lastEmber = ember

                        entry = parseChat(elem.InnerText)

                        ProcessCMD(entry)
                    End If
                End If
            Next

        Catch ex As Exception
            txtChat.Text += ex.Message & Environment.NewLine
        End Try
    End Sub

    Private Sub chkVoting_CheckedChanged(sender As Object, e As EventArgs) Handles chkVoting.CheckedChanged
        If chkVoting.Checked Then
            refTimerVote.Interval = 5000
            refTimerVote.Enabled = True
            refTimerVote.Start()
            chkCMDPause.Checked = True
        Else
            refTimerVote.Enabled = False
            refTimerVote.Stop()
        End If
    End Sub

    Private Sub chkCMDPause_CheckedChanged(sender As Object, e As EventArgs) Handles chkCMDPause.CheckedChanged
        pause(chkCMDPause.Checked)
    End Sub
End Class

Public Class QdInput
    Public buttons As UInteger
    Public RstickLR As Single
    Public RstickUD As Single
    Public LStickLR As Single
    Public LStickUD As Single
    Public time As UInteger
End Class
Public Class Votes
    Public username As String
    Public command As String
    Public cmdmulti As Integer = 1
End Class