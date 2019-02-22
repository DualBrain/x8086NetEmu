﻿Partial Public Class X8086
    Public Const MemSize As UInt32 = &H100000UI  ' 1MB
    Public Const ROMStart As UInt32 = &HC0000UI

    Public ReadOnly Memory(MemSize - 1) As Byte

    Private address As UInt32
    Private Const shl2 As UInt16 = 1 << 2
    Private Const shl3 As UInt16 = 1 << 3

    Public Class MemoryAccessEventArgs
        Inherits EventArgs

        Public Enum AccessModes
            Read
            Write
        End Enum

        Public ReadOnly Property AccessMode As AccessModes
        Public ReadOnly Property Address As UInt32

        Public Sub New(address As UInt32, accesMode As AccessModes)
            Me.Address = address
            Me.AccessMode = accesMode
        End Sub
    End Class

    Public Event MemoryAccess(sender As Object, e As MemoryAccessEventArgs)

    Public Class GPRegisters
        Implements ICloneable

        Public Enum RegistersTypes
            NONE = -1

            AL = 0
            AH = AL Or shl2
            AX = AL Or shl3

            BL = 3
            BH = BL Or shl2
            BX = BL Or shl3

            CL = 1
            CH = CL Or shl2
            CX = CL Or shl3

            DL = 2
            DH = DL Or shl2
            DX = DL Or shl3

            ES = 12
            CS = ES + 1
            SS = ES + 2
            DS = ES + 3

            SP = 24
            BP = SP + 1
            SI = SP + 2
            DI = SP + 3
            IP = SP + 4
        End Enum

        Private mActiveSegmentRegister As RegistersTypes = RegistersTypes.DS
        Private mActiveSegmentChanged As Boolean = False

        Public Property Val(reg As RegistersTypes) As UInt16
            Get
                Select Case reg
                    Case RegistersTypes.AX : Return AX
                    Case RegistersTypes.AH : Return AH
                    Case RegistersTypes.AL : Return AL

                    Case RegistersTypes.BX : Return BX
                    Case RegistersTypes.BH : Return BH
                    Case RegistersTypes.BL : Return BL

                    Case RegistersTypes.CX : Return CX
                    Case RegistersTypes.CH : Return CH
                    Case RegistersTypes.CL : Return CL

                    Case RegistersTypes.DX : Return DX
                    Case RegistersTypes.DH : Return DH
                    Case RegistersTypes.DL : Return DL

                    Case RegistersTypes.CS : Return CS
                    Case RegistersTypes.IP : Return IP

                    Case RegistersTypes.SS : Return SS
                    Case RegistersTypes.SP : Return SP

                    Case RegistersTypes.DS : Return DS
                    Case RegistersTypes.SI : Return SI

                    Case RegistersTypes.ES : Return ES
                    Case RegistersTypes.DI : Return DI

                    Case RegistersTypes.BP : Return BP

                    Case Else : Throw New Exception("Invalid Register")
                End Select
            End Get
            Set(value As UInt16)
                Select Case reg
                    Case RegistersTypes.AX : AX = value
                    Case RegistersTypes.AH : AH = value
                    Case RegistersTypes.AL : AL = value

                    Case RegistersTypes.BX : BX = value
                    Case RegistersTypes.BH : BH = value
                    Case RegistersTypes.BL : BL = value

                    Case RegistersTypes.CX : CX = value
                    Case RegistersTypes.CH : CH = value
                    Case RegistersTypes.CL : CL = value

                    Case RegistersTypes.DX : DX = value
                    Case RegistersTypes.DH : DH = value
                    Case RegistersTypes.DL : DL = value

                    Case RegistersTypes.CS : CS = value
                    Case RegistersTypes.IP : IP = value

                    Case RegistersTypes.SS : SS = value
                    Case RegistersTypes.SP : SP = value

                    Case RegistersTypes.DS : DS = value
                    Case RegistersTypes.SI : SI = value

                    Case RegistersTypes.ES : ES = value
                    Case RegistersTypes.DI : DI = value

                    Case RegistersTypes.BP : BP = value

                    Case Else : Throw New Exception("Invalid Register")
                End Select
            End Set
        End Property

        Public Property AX As UInt16
            Get
                Return (CShort(AH) << 8) Or AL
            End Get
            Set(value As UInt16)
                AH = value >> 8
                AL = value
            End Set
        End Property
        Public Property AL As Byte
        Public Property AH As Byte

        Public Property BX As UInt16
            Get
                Return (CShort(BH) << 8) Or BL
            End Get
            Set(value As UInt16)
                BH = value >> 8
                BL = value
            End Set
        End Property
        Public Property BL As Byte
        Public Property BH As Byte

        Public Property CX As UInt16
            Get
                Return (CShort(CH) << 8) Or CL
            End Get
            Set(value As UInt16)
                CH = value >> 8
                CL = value
            End Set
        End Property
        Public Property CL As Byte
        Public Property CH As Byte

        Public Property DX As UInt16
            Get
                Return (CShort(DH) << 8) Or DL
            End Get
            Set(value As UInt16)
                DH = value >> 8
                DL = value
            End Set
        End Property
        Public Property DL As Byte
        Public Property DH As Byte

        Public Property CS As UInt16
        Public Property IP As UInt16

        Public Property SS As UInt16
        Public Property SP As UInt16

        Public Property DS As UInt16
        Public Property SI As UInt16

        Public Property ES As UInt16
        Public Property DI As UInt16

        Public Property BP As UInt16

        Public Sub ResetActiveSegment()
            mActiveSegmentChanged = False
            mActiveSegmentRegister = RegistersTypes.DS
        End Sub

        Public Property ActiveSegmentRegister As RegistersTypes
            Get
                Return mActiveSegmentRegister
            End Get
            Set(value As RegistersTypes)
                mActiveSegmentRegister = value
                mActiveSegmentChanged = True
            End Set
        End Property

        Public ReadOnly Property ActiveSegmentValue As UInt32
            Get
                Return Val(mActiveSegmentRegister)
            End Get
        End Property

        Public ReadOnly Property ActiveSegmentChanged As Boolean
            Get
                Return mActiveSegmentChanged
            End Get
        End Property

        Public ReadOnly Property PointerAddressToString() As String
            Get
                Return CS.ToString("X4") + ":" + IP.ToString("X4")
            End Get
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            Dim reg = New GPRegisters With {
                .AX = AX,
                .BX = BX,
                .CX = CX,
                .DX = DX,
                .ES = ES,
                .CS = CS,
                .SS = SS,
                .DS = DS,
                .SP = SP,
                .BP = BP,
                .SI = SI,
                .DI = DI,
                .IP = IP
            }
            If mActiveSegmentChanged Then reg.ActiveSegmentRegister = mActiveSegmentRegister
            Return reg
        End Function
    End Class

    Public Class GPFlags
        Implements ICloneable

        Public Enum FlagsTypes
            CF = 2 ^ 0
            PF = 2 ^ 2
            AF = 2 ^ 4
            ZF = 2 ^ 6
            SF = 2 ^ 7
            TF = 2 ^ 8
            [IF] = 2 ^ 9
            DF = 2 ^ 10
            [OF] = 2 ^ 11
        End Enum

        Public Property CF As Byte
        Public Property PF As Byte
        Public Property AF As Byte
        Public Property ZF As Byte
        Public Property SF As Byte
        Public Property TF As Byte
        Public Property [IF] As Byte
        Public Property DF As Byte
        Public Property [OF] As Byte

        Public Property EFlags() As UInt16
            Get
                Return CF * FlagsTypes.CF Or
                        1 * 2 ^ 1 Or
                       PF * FlagsTypes.PF Or
                        0 * 2 ^ 3 Or
                       AF * FlagsTypes.AF Or
                        0 * 2 ^ 5 Or
                       ZF * FlagsTypes.ZF Or
                       SF * FlagsTypes.SF Or
                       TF * FlagsTypes.TF Or
                     [IF] * FlagsTypes.IF Or
                       DF * FlagsTypes.DF Or
                     [OF] * FlagsTypes.OF Or
                            &HF000 ' IOPL, NT and bit 15 are always "1" on 8086
            End Get
            Set(value As UInt16)
                CF = If((value And FlagsTypes.CF) = FlagsTypes.CF, 1, 0)
                ' Reserved 1
                PF = If((value And FlagsTypes.PF) = FlagsTypes.PF, 1, 0)
                ' Reserved 0
                AF = If((value And FlagsTypes.AF) = FlagsTypes.AF, 1, 0)
                ' Reserved 0
                ZF = If((value And FlagsTypes.ZF) = FlagsTypes.ZF, 1, 0)
                SF = If((value And FlagsTypes.SF) = FlagsTypes.SF, 1, 0)
                TF = If((value And FlagsTypes.TF) = FlagsTypes.TF, 1, 0)
                [IF] = If((value And FlagsTypes.IF) = FlagsTypes.IF, 1, 0)
                DF = If((value And FlagsTypes.DF) = FlagsTypes.DF, 1, 0)
                [OF] = If((value And FlagsTypes.OF) = FlagsTypes.OF, 1, 0)
            End Set
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            Return New GPFlags With {.EFlags = EFlags}
        End Function
    End Class

    Public Sub LoadBIN(fileName As String, segment As UInt16, offset As UInt16)
        'Console.WriteLine($"Loading: {fileName} @ {segment:X4}:{offset:X4}")
        fileName = X8086.FixPath(fileName)

        If IO.File.Exists(fileName) Then
            CopyToMemory(IO.File.ReadAllBytes(fileName), segment, offset)
        Else
            ThrowException("File Not Found: " + vbCrLf + fileName)
        End If
    End Sub

    Public Sub CopyToMemory(bytes() As Byte, segment As UInt16, offset As UInt16)
        CopyToMemory(bytes, X8086.SegmentOffetToAbsolute(segment, offset))
    End Sub

    Public Sub CopyToMemory(bytes() As Byte, address As UInt32)
        ' TODO: We need to implement some checks to prevent loading code into ROM areas.
        '       Something like this, for example:
        '       If address + bytes.Length >= ROMStart Then ...
        Array.Copy(bytes, 0, Memory, address, bytes.Length)
    End Sub

    Public Sub CopyFromMemory(bytes() As Byte, address As UInt32)
        Array.Copy(Memory, address, bytes, 0, bytes.Length)
    End Sub

    Public Property Registers As GPRegisters
        Get
            Return mRegisters
        End Get
        Set(value As GPRegisters)
            mRegisters = value
        End Set
    End Property

    Public Property Flags As GPFlags
        Get
            Return mFlags
        End Get
        Set(value As GPFlags)
            mFlags = value
        End Set
    End Property

    Private Sub PushIntoStack(value As UInt16)
        mRegisters.SP -= 2
        RAM16(mRegisters.SS, mRegisters.SP,, True) = value
    End Sub

    Private Function PopFromStack() As UInt16
        mRegisters.SP += 2
        Return RAM16(mRegisters.SS, mRegisters.SP - 2,, True)
    End Function

    Public Shared Function SegmentOffetToAbsolute(segment As UInt16, offset As UInt16) As UInt32
        Return (CUInt(segment) << 4UI) + offset
    End Function

    Public Shared Function AbsoluteToSegment(address As UInt32) As UInt16
        Return (address >> 4UI) And &HFFF00UI
    End Function

    Public Shared Function AbsoluteToOffset(address As UInt32) As UInt16
        Return address And &HFFFUI
    End Function

    Public Property RAM(address As UInt32, Optional ignoreHooks As Boolean = False) As Byte
        Get
            'If mDebugMode Then RaiseEvent MemoryAccess(Me, New MemoryAccessEventArgs(address, MemoryAccessEventArgs.AccessModes.Read))
            'Return FromPreftch(address)

            If Not ignoreHooks Then
                For i As Integer = 0 To memHooks.Count - 1
                    If memHooks(i).Invoke(address, tmpUVal, MemHookMode.Read) Then Return tmpUVal
                Next
            End If

            Return Memory(address And &HFFFFFUI) ' "Call 5" Legacy Interface: http://www.os2museum.com/wp/?p=734
        End Get
        Set(value As Byte)
            If Not ignoreHooks Then
                For i As Integer = 0 To memHooks.Count - 1
                    If memHooks(i).Invoke(address, value, MemHookMode.Write) Then Exit Property
                Next
            End If

            Memory(address) = value

            'If mDebugMode Then RaiseEvent MemoryAccess(Me, New MemoryAccessEventArgs(address, MemoryAccessEventArgs.AccessModes.Write))
        End Set
    End Property

    Public Property RAM8(segment As UInt16, offset As UInt16, Optional inc As Byte = 0, Optional ignoreHooks As Boolean = False) As Byte
        Get
            Return RAM(SegmentOffetToAbsolute(segment, offset + inc), ignoreHooks)
        End Get
        Set(value As Byte)
            RAM(SegmentOffetToAbsolute(segment, offset + inc), ignoreHooks) = value
        End Set
    End Property

    Public Property RAM16(segment As UInt16, offset As UInt16, Optional inc As Byte = 0, Optional ignoreHooks As Boolean = False) As UInt16
        Get
            address = SegmentOffetToAbsolute(segment, offset + inc)
            Return (CUInt(RAM(address + 1UI, ignoreHooks)) << 8UI) Or RAM(address, ignoreHooks)
        End Get
        Set(value As UInt16)
            address = SegmentOffetToAbsolute(segment, offset + inc)
            RAM(address, ignoreHooks) = value
            RAM(address + 1UI, ignoreHooks) = value >> 8UI
        End Set
    End Property

    Public Property RAMn(Optional ignoreHooks As Boolean = False) As UInt16
        Get
            If addrMode.Size = DataSize.Byte Then
                Return RAM8(mRegisters.ActiveSegmentValue, addrMode.IndAdr,, ignoreHooks)
            Else
                Return RAM16(mRegisters.ActiveSegmentValue, addrMode.IndAdr,, ignoreHooks)
            End If
        End Get
        Set(value As UInt16)
            If addrMode.Size = DataSize.Byte Then
                RAM8(mRegisters.ActiveSegmentValue, addrMode.IndAdr,, ignoreHooks) = value
            Else
                RAM16(mRegisters.ActiveSegmentValue, addrMode.IndAdr,, ignoreHooks) = value
            End If
        End Set
    End Property
End Class