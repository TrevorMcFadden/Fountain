NotInheritable Class App
    Inherits Application
    Protected Overrides Sub OnLaunched(e As LaunchActivatedEventArgs)
        Dim RootFrame As Frame = TryCast(Window.Current.Content, Frame)
        If RootFrame Is Nothing Then
            RootFrame = New Frame()
            AddHandler RootFrame.NavigationFailed, AddressOf OnNavigationFailed
            If e.PreviousExecutionState = 3 Then
            End If
            Window.Current.Content = RootFrame
        End If
        If e.PrelaunchActivated = False Then
            If RootFrame.Content Is Nothing Then
                RootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If
            Dim UwpTitleBar As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
            UwpTitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent
            UwpTitleBar.BackgroundColor = Windows.UI.Colors.Transparent
            Dim CoreTitleBar As Core.CoreApplicationViewTitleBar = Core.CoreApplication.GetCurrentView().TitleBar
            CoreTitleBar.ExtendViewIntoTitleBar = True
            Window.Current.Activate()
        End If
    End Sub
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim Deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        Deferral.Complete()
    End Sub
End Class