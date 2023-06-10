Imports System.Diagnostics.Eventing
Imports System.Environment
Imports System.IO
Imports System.Reflection
Imports System.Runtime
Imports System.Text
Imports System.Text.Json
Imports Microsoft.SqlServer
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms

Class MainWindow
    Dim ConfigFile As Config
    Dim DecPassword As String
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not File.Exists("config.json") Then
            File.WriteAllText("config.json", JsonSerializer.Serialize(New Config))
        End If
        Using sr As New StreamReader("config.json")
            Dim Config = sr.ReadToEnd()
            ConfigFile = JsonSerializer.Deserialize(Of Config)(Config)
        End Using
        UseProxy.IsChecked = ConfigFile.UseProxy
        NoVsync.IsChecked = ConfigFile.DisableVsync
        UnlimitFps.IsChecked = ConfigFile.UnlockFps
        AutoLogin.IsChecked = ConfigFile.AutoLogin
        ProxyUrl.Text = ConfigFile.ProxyUrl
        Username.Text = ConfigFile.Username
        Dim DecTemp1 = ConfigFile.EncPassword
        Dim DecTemp2 = ""
        For DecNum As Integer = 1 To DecTemp1.Length
            If IsNumeric(DecTemp1.Substring(DecNum - 1, 1)) Then
                DecTemp2 += (9 - Integer.Parse(DecTemp1.Substring(DecNum - 1, 1))).ToString
            Else
                DecTemp2 += DecTemp1.Substring(DecNum - 1, 1)
            End If
        Next
        DecPassword = Encoding.UTF8.GetString(Convert.FromHexString(DecTemp2))
        Password.Password = DecPassword
        LoadAsync()
    End Sub
    Private Async Sub LoadAsync()
        Dim WebviewArgu = "--disable-features=msSmartScreenProtection --in-process-gpu --disable-web-security --no-sandbox --renderer-process-limit=1 --single-process"
        If ConfigFile.UseProxy Then
            WebviewArgu += " --proxy-server=""" + ConfigFile.ProxyUrl + """"
        End If
        If ConfigFile.DisableVsync Then
            WebviewArgu += " --disable-gpu-vsync"
        End If
        If ConfigFile.UnlockFps Then
            WebviewArgu += " --disable-frame-rate-limit"
        End If
        Dim options As New CoreWebView2EnvironmentOptions With {
            .AdditionalBrowserArguments = WebviewArgu
        }
        ''SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", WebviewArgu)
        Directory.CreateDirectory(CurrentDirectory + "\QinliliWebview2\")
        ''SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", CurrentDirectory + "\QinliliWebview2\")
        Dim webView2Environment = Await CoreWebView2Environment.CreateAsync(, CurrentDirectory + "\QinliliWebview2\", options)
        Await WebView.EnsureCoreWebView2Async(webView2Environment)
        WebView.IsEnabled = True
        WebView.CoreWebView2.Settings.IsStatusBarEnabled = False
        WebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = False
        WebView.CoreWebView2.Navigate("https://pc-play.games.dmm.com/play/houkago/")
    End Sub
    Private Async Sub WebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles WebView.NavigationCompleted
        If e.IsSuccess Then
            'MsgBox(WebView.CoreWebView2.Source)
            If WebView.CoreWebView2.Source.Contains("not-available-in-your-region") Then
                MsgBox("你的网络环境存在异常，请检查网络环境或设置代理",, "加载失败")
            End If
            If WebView.CoreWebView2.Source.Contains("service/login/password") Then
                If ConfigFile.AutoLogin Then
                    Await WebView.CoreWebView2.ExecuteScriptAsync("document.getElementById(""login_id"").value=""" + ConfigFile.Username + """;")
                    Await WebView.CoreWebView2.ExecuteScriptAsync("document.getElementById(""password"").value=""" + DecPassword + """;")
                    Await WebView.CoreWebView2.ExecuteScriptAsync("document.getElementById(""use_auto_login"").checked=true;")
                    Await WebView.CoreWebView2.ExecuteScriptAsync("document.querySelector(""input[type='submit']"").click();")
                End If
            End If
            If WebView.CoreWebView2.Source.Contains("gadgets/ifr") Then
                Dim Reader = New StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Houkago.window.js"))
                Dim PreCachedDragJS = Await Reader.ReadToEndAsync()
                Await WebView.CoreWebView2.ExecuteScriptAsync(PreCachedDragJS)
            End If
        Else
            If Not e.WebErrorStatus = CoreWebView2WebErrorStatus.ConnectionAborted Then
                MsgBox("出错了，错误代码" + e.WebErrorStatus.ToString(),, "加载失败")
            End If
        End If
    End Sub

    Private Async Sub WebView_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs) Handles WebView.NavigationStarting
        If e.Uri.Contains("play/houkago/") Then
            Await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.addEventListener('DOMContentLoaded',() => {location.replace(document.getElementById(""game_frame"").src)})")
        End If
    End Sub
    Private Sub AutoLogin_Checked(sender As Object, e As RoutedEventArgs) Handles AutoLogin.Checked
        Username.IsEnabled = True
        Password.IsEnabled = True
    End Sub

    Private Sub AutoLogin_Unchecked(sender As Object, e As RoutedEventArgs) Handles AutoLogin.Unchecked
        Username.IsEnabled = False
        Password.IsEnabled = False
    End Sub

    Private Sub UseProxy_Checked(sender As Object, e As RoutedEventArgs) Handles UseProxy.Checked
        ProxyUrl.IsEnabled = True
    End Sub

    Private Sub UseProxy_Unchecked(sender As Object, e As RoutedEventArgs) Handles UseProxy.Unchecked
        ProxyUrl.IsEnabled = False
    End Sub

    Private Sub SaveConfig_Click(sender As Object, e As RoutedEventArgs) Handles SaveConfig.Click
        ConfigFile.UseProxy = UseProxy.IsChecked
        ConfigFile.DisableVsync = NoVsync.IsChecked
        ConfigFile.UnlockFps = UnlimitFps.IsChecked
        ConfigFile.AutoLogin = AutoLogin.IsChecked
        ConfigFile.ProxyUrl = ProxyUrl.Text
        ConfigFile.Username = Username.Text
        DecPassword = Password.Password
        Dim EncTemp1 = Convert.ToHexString(Encoding.UTF8.GetBytes(DecPassword))
        Dim EncTemp2 = ""
        For EncNum As Integer = 1 To EncTemp1.Length
            If IsNumeric(EncTemp1.Substring(EncNum - 1, 1)) Then
                EncTemp2 += (9 - Integer.Parse(EncTemp1.Substring(EncNum - 1, 1))).ToString
            Else
                EncTemp2 += EncTemp1.Substring(EncNum - 1, 1)
            End If
        Next
        ConfigFile.EncPassword = EncTemp2
        Using sw As New StreamWriter("config.json")
            sw.Write(JsonSerializer.Serialize(ConfigFile))
        End Using
        Process.Start(Process.GetCurrentProcess().MainModule.FileName)
        Windows.Application.Current.Shutdown()
    End Sub

    Private Sub ClearCache_Click(sender As Object, e As RoutedEventArgs) Handles ClearCache.Click
        WebView.CoreWebView2.Profile.ClearBrowsingDataAsync(256)
        MsgBox("缓存清理成功，正在刷新页面...",, "清理成功")
        WebView.Reload()
    End Sub

    Private Sub GameTab_GotFocus(sender As Object, e As RoutedEventArgs) Handles GameTab.GotFocus
        WebView.CoreWebView2.ExecuteScriptAsync("onWindowResize()")
    End Sub

    Class Config
        Public Property UseProxy As Boolean
        Public Property ProxyUrl As String = ""
        Public Property DisableVsync As Boolean
        Public Property UnlockFps As Boolean
        Public Property AutoLogin As Boolean
        Public Property Username As String = ""
        Public Property EncPassword As String = ""
    End Class

    Private Sub ClearAccount_Click(sender As Object, e As RoutedEventArgs) Handles ClearAccount.Click
        Dim result As MessageBoxResult = MessageBox.Show("你确定要清除当前账号的全部WebView数据吗？",
                              "清除数据",
                              MessageBoxButton.YesNo)
        If result = MessageBoxResult.Yes Then
            MsgBox("程序将重新启动以清除数据",, "清除数据")
            Process.Start(Process.GetCurrentProcess().MainModule.FileName, "--clean")
            Windows.Application.Current.Shutdown()
            Return
        End If
    End Sub
End Class
