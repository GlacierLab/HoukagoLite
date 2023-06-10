Option Strict Off
Option Explicit On

Public Class Application
    Inherits Windows.Application

    Public Sub InitializeComponent()
        If Command() = "--clean" Then
            StartupUri = New Uri("Clean.xaml", UriKind.Relative)
        Else
            StartupUri = New Uri("MainWindow.xaml", UriKind.Relative)
        End If
    End Sub
    Public Shared Sub Main()
        'MsgBox(String.Join(",", Assembly.GetExecutingAssembly().GetManifestResourceNames().ToArray()))
        'MsgBox(Environment.ProcessPath)
        Dim app As New Application()
        app.InitializeComponent()
        app.Run()
    End Sub

End Class