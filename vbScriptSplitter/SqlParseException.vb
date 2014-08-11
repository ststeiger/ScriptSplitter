
'Imports System.Runtime.Serialization
'Imports System.Security.Permissions


Namespace Subtext.Scripting.Exceptions


    <Serializable> _
    Public Class SqlParseException
        Inherits Exception


        Public Sub New()
        End Sub


        Public Sub New(message As String)
            MyBase.New(message)
        End Sub


        Public Sub New(message As String, exception As Exception)
            MyBase.New(message, exception)
        End Sub


        Protected Sub New(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub


        <System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags:=System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)> _
        Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
            MyBase.GetObjectData(info, context)
        End Sub


    End Class


End Namespace
