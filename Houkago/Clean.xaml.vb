Imports System.IO
Imports System.Threading
Imports System.Environment
Imports System.ComponentModel

Public Class Clean
    Private Sub Clean_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        RemoveDirectories()
    End Sub

    Private Sub RemoveDirectories()
        ThreadPool.QueueUserWorkItem(Function(o)
                                         Thread.Sleep(250)
                                         Dim FileList = FileIO.FileSystem.GetFiles(CurrentDirectory + "\QinliliWebview2\",
          FileIO.SearchOption.SearchAllSubDirectories)
                                         Dispatcher.BeginInvoke(New Action(Function()
                                                                               progressBar1.Minimum = 0
                                                                               progressBar1.Value = 0
                                                                               progressBar1.Maximum = FileList.Count
                                                                           End Function))
                                         For Each strFile In FileList
                                             File.Delete(strFile)
                                             Dispatcher.BeginInvoke(New Action(Function()
                                                                                   progressBar1.Value = progressBar1.Value + 1
                                                                               End Function))
                                         Next

                                         Dim dirInfo As New DirectoryInfo("QinliliWebview2")
                                         Dim dirs = dirInfo.GetDirectories()
                                         For Each dir As DirectoryInfo In dirs
                                             dir.Delete(True)
                                         Next
                                         Thread.Sleep(500)
                                         Dispatcher.BeginInvoke(New Action(Function()
                                                                               Process.Start(Process.GetCurrentProcess().MainModule.FileName)
                                                                               Windows.Application.Current.Shutdown()
                                                                           End Function))
                                     End Function, Nothing)
    End Sub

    Private Sub Clean_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
    End Sub
End Class
